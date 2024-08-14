using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Services;
using Smead.Security;
using System.Threading.Tasks;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.Services.Interface;
using MSRecordsEngine.Controllers;
using Microsoft.Extensions.Logging;
using System;
using MSRecordsEngine.Models;
using Leadtools.Barcode;
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
        private readonly ILayoutDataService _layoutService;
        private readonly IDataGridService _datagridService;
        public DataController(CommonControllersService<DataController> commonControllersService, ILayoutDataService layoutservice, IDataGridService datagridService)
        {
            _commonService = commonControllersService;
            _layoutService = layoutservice;
            _datagridService = datagridService;
        }
        [HttpPost]
        [Route("DataLayout")]
        public async Task<LayoutModel> DataLayout(Passport passport)
        {
            var model = new LayoutModel();
            try
            {
                await _layoutService.BindUserAccessMenu(passport, model);
                await _layoutService.HandleAdminMenu(passport, model);
                await _layoutService.BackgroundStatusNotifications(passport, model);
                await _layoutService.LoadTasks(passport, model);
                await _layoutService.GetTaskLightValues(passport, model);
                await _layoutService.LoadNews(passport, model);
                await _layoutService.GetFooter(passport, model);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        
            return model;
        }
        [HttpPost]
        [Route("SaveNewsURL")]
        public async Task SaveNewsURL(NewUrlprops model)
        {
            try
            {
                await _datagridService.SaveNewsURL(model);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
            
        }
        [HttpPost]
        [Route("LoadQueryWindow")]
        public async Task<ViewQueryWindow> LoadQueryWindow(ViewQueryWindowProps props)
        {
            try
            {
                return await _datagridService.DrawQuery(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
          
        }
    }
}