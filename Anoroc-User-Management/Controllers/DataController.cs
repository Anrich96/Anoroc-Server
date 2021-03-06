﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Anoroc_User_Management.Interfaces;
using Anoroc_User_Management.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Anoroc_User_Management.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        IDataService DataService;
        public DataController(IDataService dataService)
        {
            DataService = dataService;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet("SouthAfricaOverTime")]
        public async Task<IActionResult> SouthAfricaOverTime()
        {
            var response = await DataService.GetCasesPerDate();
            return Ok(response);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet("PredictAreas")]
        public IActionResult PredictAreas()
        {
            var response = DataService.PredictionAreas();
            return Ok(JsonConvert.SerializeObject(response));
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet("GetTrainningData")]
        public IActionResult GetTrainningData()
        {
            var response = DataService.GetTrainningData();
            return Ok(JsonConvert.SerializeObject(response));
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet("GetUpperBoundData")]
        public IActionResult GetUpperBoundData()
        {
            var response = DataService.GetUpperBoundData();
            return Ok(JsonConvert.SerializeObject(response));
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet("GetLowerBoundData")]
        public IActionResult GetLowerBoundData()
        {
            var response = DataService.GetLowerBoundData();
            return Ok(JsonConvert.SerializeObject(response));
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet("GetForecastData")]
        public IActionResult GetForecastData()
        {
            var response = DataService.GetForecastData();
            return Ok(JsonConvert.SerializeObject(response));
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet("GetAccuracyData")]
        public IActionResult GetAccuracyData()
        {
            var response = DataService.GetAccuracytData();
            return Ok(JsonConvert.SerializeObject(response));
        }

    }
}