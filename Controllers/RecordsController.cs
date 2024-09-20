using MsRecordEngine.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Repository;
using MSRecordsEngine.Services;
using System.Linq;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecordsController : ControllerBase
    {
        private readonly CommonControllersService<RecordsController> _commonService;
        public RecordsController(CommonControllersService<RecordsController> commonControllersService)
        {
            _commonService = commonControllersService;
        }
    }

}