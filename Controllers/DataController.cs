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
using Smead.Security;
using System.Data.Entity.Validation;
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

        [HttpPost]
        [Route("test")]
        public void test(MicroArguments model)
        {
            var pass = model.passport;
            try
            {
                throw new Exception("hello this is an error message!");
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

    }

    public class MicroArguments
    {
        public object model { get; set; }
        public Passport passport { get; set; }
        public List<string> liststring { get; set; }
        public List<int> listints { get; set; }
        public List<object> listobject { get; set; }
    }
}