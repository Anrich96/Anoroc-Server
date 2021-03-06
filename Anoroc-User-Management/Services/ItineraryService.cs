﻿using Anoroc_User_Management.Interfaces;
using Anoroc_User_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anoroc_User_Management.Services
{
    public class ItineraryService : IItineraryService
    {
        IClusterService ClusterService;
        IDatabaseEngine DatabaseEngine;
        protected int HIGHDENSITY;
        protected string WebAppToken;
        public ItineraryService(IClusterService clusterService, IDatabaseEngine databaseEngine, int _highDensity, string token)
        {
            HIGHDENSITY = _highDensity;
            DatabaseEngine = databaseEngine;
            ClusterService = clusterService;
            WebAppToken = token;
        }

        public void DeleteUserItinerary(string access_token, int _id)
        {
            DatabaseEngine.Delete_Itinerary_Risk_By_ID(_id);
        }

        public List<ItineraryRiskWrapper> GetItineraries(int pagination, string access_token)
        {
            var userItineraries = DatabaseEngine.Get_Itinerary_Risks_By_Token(access_token);
            var itineraryWrapper = new List<ItineraryRiskWrapper>();
            if (userItineraries != null)
            {
                if (pagination == -1)
                {
                    userItineraries.ForEach(itinerary =>
                    {
                        itineraryWrapper.Add(new ItineraryRiskWrapper(itinerary));
                    });
                }
                else
                {
                    if (pagination == 10)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var itinerary = userItineraries.ElementAtOrDefault(i);
                            if (itinerary != null)
                                itineraryWrapper.Add(new ItineraryRiskWrapper(itinerary));
                        }
                    }
                    else
                    {
                        for (int i = pagination; i < (pagination + 10); i++)
                        {
                            var itinerary = userItineraries.ElementAtOrDefault(i);
                            if (itinerary != null)
                                itineraryWrapper.Add(new ItineraryRiskWrapper(itinerary));
                        }
                    }
                }

                if (itineraryWrapper.Count != 0)
                    return itineraryWrapper;
                else
                    return null;
            }
            else
                return null;
        }


        public ItineraryRiskWrapper ProcessItinerary(Itinerary userItinerary, string access_token)
        {
            double averageClusterDensity = 0;

            ItineraryRisk itinerary = new ItineraryRisk(userItinerary.Created, access_token);

            if (userItinerary.Locations != null)
            {
                if (itinerary.LocationItineraryRisks == null)
                    itinerary.LocationItineraryRisks = new Dictionary<Location, int>();

                List<Location> locationList = userItinerary.Locations;

                locationList.ForEach(location =>
                {
                    var clusters = ClusterService.ClustersInRange(location, -1);

                    if (clusters != null)
                    {
                        averageClusterDensity = CalculateClusteringDensity(clusters);

                        if (averageClusterDensity == 0)
                            itinerary.LocationItineraryRisks.Add(location, RISK.NO_RISK);
                        else if (averageClusterDensity > HIGHDENSITY)
                            itinerary.LocationItineraryRisks.Add(location, RISK.HIGH_RISK);
                        else
                            itinerary.LocationItineraryRisks.Add(location, RISK.MEDIUM_RISK);
                    }
                    else
                    {
                        // Check unclustered current locations
                        var oldClusters = ClusterService.OldClustersInRange(location, -1);

                        if (oldClusters != null)
                        {
                            averageClusterDensity = CalculateClusteringDensity(clusters);
                            if (averageClusterDensity == 0)
                                itinerary.LocationItineraryRisks.Add(location, RISK.NO_RISK);
                            else if (averageClusterDensity > HIGHDENSITY)
                                itinerary.LocationItineraryRisks.Add(location, RISK.MODERATE_RISK);
                            else
                                itinerary.LocationItineraryRisks.Add(location, RISK.LOW_RISK);
                        }
                        else
                        {
                            // No clusters near this location
                            itinerary.LocationItineraryRisks.Add(location, RISK.NO_RISK);
                        }
                    }
                });

                itinerary.TotalItineraryRisk = CalculateTotalRisk(itinerary.LocationItineraryRisks);

                /*itinerary.UserAccessToken = DatabaseEngine.GetUserEmail(access_token)*/;
                if(access_token != WebAppToken)
                {

                    var _id = DatabaseEngine.Insert_Itinerary_Risk(itinerary);
                    itinerary.ID = _id;
                }
            }
            return new ItineraryRiskWrapper(itinerary);
        }

        private double CalculateClusteringDensity(List<Cluster> clusters)
        {
            double averageClusterDensity = 0;
            if (clusters != null)
            {
                if (clusters.Count > 0)
                {
                    averageClusterDensity = 0;
                    clusters.ForEach(cluster =>
                    {
                        averageClusterDensity += CalculateDensity(cluster);
                    });
                    averageClusterDensity /= clusters.Count;

                    averageClusterDensity *= 100;
                }
                return averageClusterDensity;
            }
            else
                return 0;
        }

        private int CalculateTotalRisk(Dictionary<Location, int> locationItineraryRisks)
        {
            if (locationItineraryRisks.Values.Count == 0)
                return 0;

            int risk = 0;
            for(int i = 0; i< locationItineraryRisks.Values.Count; i++)
            {
                risk += locationItineraryRisks.Values.ElementAt(i);
            }
            risk /= locationItineraryRisks.Values.Count;
            return risk;
        }

        public double CalculateDensity(Cluster cluster)
        {
            var area = Math.PI * Math.Pow(cluster.Cluster_Radius, 2);

            var density = cluster.Coordinates.Count / area;

            return density;
        }
    }
}
