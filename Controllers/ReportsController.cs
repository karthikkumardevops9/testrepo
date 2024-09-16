using FusionWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Services.Interface;
using MSRecordsEngine.Services;

namespace MSRecordsEngine.Controllers
{
    public class ReportsController : Controller
    {
        private readonly CommonControllersService<ReportsController> _commonService;
        private readonly ILayoutDataService _layoutService;
        private readonly IDataGridService _datagridService;
        public ReportsController(CommonControllersService<ReportsController> commonControllersService, ILayoutDataService layoutservice, IDataGridService datagridService)
        {
            _commonService = commonControllersService;
            _layoutService = layoutservice;
            _datagridService = datagridService;
        }

    }
}
