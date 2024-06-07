using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.DirectoryServices.ActiveDirectory;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagingController : ControllerBase
    {
        private ILogger<ImagingController> _logger;
        public ImagingController(ILogger<ImagingController> logger)
        {
            _logger = logger;
        }
       

    }
}
