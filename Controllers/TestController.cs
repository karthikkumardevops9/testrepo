using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "RecordEngine is up and running!";
        }
    }
}
