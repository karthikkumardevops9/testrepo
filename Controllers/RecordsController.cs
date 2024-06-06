using FusionWebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Models;
using MSRecordsEngine.Repository;
using System.Linq;

namespace MSRecordsEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsController : ControllerBase
    {
        private ILogger<RecordsController> _logger;
        public RecordsController(ILogger<RecordsController> logger)
        {
            _logger = logger;
        }

        public void testing()
        {
            
        }


    }

}