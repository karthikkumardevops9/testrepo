using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Data.Entity;
using MSRecordsEngine.Services;
using Microsoft.Extensions.Logging;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataToolsLayoutController : ControllerBase
    {
        private readonly CommonControllersService<DataToolsLayoutController> _commonService;

        public DataToolsLayoutController(CommonControllersService<DataToolsLayoutController> commonService)
        {
            _commonService = commonService;
        }

        [Route("GetDateForm")]
        [HttpGet]
        public async Task<List<LookupType>> GetDateForm(string connectionString)
        {
            var oList = new List<LookupType>();
            try
            {
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    oList = await context.LookupTypes.Where(m => m.LookupTypeForCode.Trim().ToUpper().Equals("DTFRM".Trim().ToUpper())).OrderBy(m => m.SortOrder).ToListAsync();

                    return oList;
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw;

            }
        }

    }
}
