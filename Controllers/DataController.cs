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
using MSRecordsEngine.Controllers;
using MSRecordsEngine.Services;
using Leadtools.ImageProcessing.Core;
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
        private readonly CommonControllersService<DataController> _commonService;
        public DataController(CommonControllersService<DataController> commonControllersService)
        {
            _commonService = commonControllersService;
        }

        [HttpGet]
        public void test()
        {
            try
            {
                throw new Exception("hello I am failure in ms record manager");
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

    }
}