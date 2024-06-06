using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System;
using Microsoft.Extensions.Logging;
namespace FusionWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //// For developer
    ///If you want to authorize users, use the GenerateToken controller 
    ///and uncomment the [Authorize] attribute.
    //[Authorize]
    public class DataController : ControllerBase
    {
        private ILogger<DataController> _logger;
        public DataController(ILogger<DataController> logger)
        {
           _logger = logger;
        }
        [HttpGet]
        [Route("TestingGet")]
        public string GetCompany(string CompanyName)
        {
            _logger.LogWarning("test warning message");
            return $"Testing.......{CompanyName}";
        }
        [HttpPost]
        [Route("TestingPost")]
        public Company GetCompanyModel(Company person)
        {
            var p = new Company();
            p.CompanyName = person.CompanyName;
            p.City = person.City;
            return p;
        }
    }
    public class Company
    {
        public string CompanyName { get; set; }         
        public string City { get; set; }    
    }

}