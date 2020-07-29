﻿using Anoroc_User_Management.Services;
using Microsoft.EntityFrameworkCore;

namespace Anoroc_User_Management.Models
{
    /// <summary>
    /// This class specifies the Context of the Databse by using set Models to capture each Table in the database.
    /// It is used to create a complete picture of the database that can be used alongside the database
    /// </summary>
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options): base(options){ }
        public DbSet<PrimitiveLocation> Locations { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<PrimitiveCluster> Clusters { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrimitiveLocation>().HasKey(id => id.Location_ID);
            modelBuilder.Entity<Area>().HasKey(id => id.Area_ID);
            modelBuilder.Entity<PrimitiveCluster>().HasKey(id => id.Cluster_ID);
            modelBuilder.Entity<User>().HasKey(id => id.User_ID);
        }
    }
}