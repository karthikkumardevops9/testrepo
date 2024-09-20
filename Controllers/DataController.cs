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
using MSRecordsEngine.RecordsManager;
using System.ComponentModel;
using Microsoft.Identity.Client;
using System.Net;
using System.Collections.Generic;
namespace MsRecordEngine.Controllers
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
        [HttpPost]
        [Route("RunQuery")]
        public async Task<GridDataBinding> RunQuery(SearchQueryRequestModal props)
        {
            var model = new GridDataBinding();
            try
            {
                model = await _datagridService.RunQuery(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
            return model;
        }
        [HttpPost]
        [Route("GetTotalrowsForGrid")]
        public async Task<string> GetTotalrowsForGrid(SearchQueryRequestModal req)
        {
            try
            {
                return await _datagridService.GetTotalRowsForGrid(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("LinkscriptButtonClick")]
        public async Task<ScriptReturn> LinkscriptButtonClick(linkscriptPropertiesUI props)
        {
            try
            {
                return await _datagridService.LinkscriptButtonClick(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("BuiltControls")]
        public LinkScriptModel BuiltControls(ScriptReturn scriptresult)
        {
            try
            {
                return _datagridService.BuiltControls(scriptresult);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("LinkscriptEvents")]
        public async Task<ScriptReturn> LinkscriptEvents(linkscriptPropertiesUI props)
        {
            try
            {
                return await _datagridService.LinkscriptEvents(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("FlowButtonsClickEvent")]
        public async Task<bool> FlowButtonsClickEvent(linkscriptPropertiesUI props)
        {
            try
            {
                return await _datagridService.FlowButtonsClickEvent(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("TabQuikInitiator")]
        public async Task<TabquikApi> TabQuikInitiator(TabquickpropUI props)
        {
            try
            {
                return await _datagridService.TabQuikInitiator(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("GetTrackbaleDataPerRow")]
        public async Task<TrackingModeld> GetTrackbaleDataPerRow(trackableUiParams track)
        {
            try
            {
                return await _datagridService.GetTrackingPerRow(track);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("ReturnFavoritTogrid")]
        public async Task<GridDataBinding> ReturnFavoritTogrid(ReturnFavoritTogridReqModel req)
        {
            try
            {
                return await _datagridService.BuildNewFavoriteData(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("DeleteFavorite")]
        public async Task<bool> DeleteFavorite(FavoriteRecordReqModel req)
        {
            bool deleted = false;
            try
            {
                req.paramss.FavCriteriaType = "1";
                deleted = await _datagridService.DeleteSavedCriteria(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
            return deleted;
        }
        [HttpPost]
        [Route("AddNewFavorite")]
        public async Task<MyFavorite> AddNewFavorite(FavoriteRecordReqModel req)
        {
            try
            {
                return await _datagridService.AddNewFavorite(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("UpdateFavorite")]
        public async Task<MyFavorite> UpdateFavorite(FavoriteRecordReqModel req)
        {
            try
            {
                return await _datagridService.UpdateFavorite(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("DeleteFavoriteRecord")]
        public async Task<bool> DeleteFavoriteRecord(FavoriteRecordReqModel req)
        {
            bool isdeleted = false;
            try
            {
                await _datagridService.DeleteFavoriteRecord(req);
                isdeleted = true;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
            return isdeleted;
        }
        [HttpPost]
        [Route("SaveNewQuery")]
        public async Task<Myquery> SaveNewQuery(SaveNewUpdateDeleteQueryReqModel req)
        {
            try
            {
                return await _datagridService.SaveNewQuery(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("UpdateQuery")]
        public async Task<Myquery> UpdateQuery(SaveNewUpdateDeleteQueryReqModel req)
        {
            try
            {
                return await _datagridService.UpdateQuery(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("DeleteQuery")]
        public async Task<Myquery> DeleteQuery(SaveNewUpdateDeleteQueryReqModel req)
        {
            try
            {
                return await _datagridService.DeleteQuery(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("RunglobalSearch")]
        public async Task<GlobalSearch> RunglobalSearch(GlobalSearchReqModel req)
        {
            try
            {
                return await _datagridService.RunglobalSearch(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("GlobalSearchClick")]
        public async Task<GridDataBinding> GlobalSearchClick(GlobalSearchReqModel req)
        {

            try
            {
                return await _datagridService.GlobalSearchClick(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }

        }
        [HttpPost]
        [Route("GlobalSearchAllClick")]
        public async Task<GridDataBinding> GlobalSearchAllClick(GlobalSearchReqModel req)
        {
            try
            {
                return await _datagridService.GlobalSearchAllClick(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("SetDatabaseChanges")]
        public async Task<Saverows> SetDatabaseChanges(DatabaseChangesReq props)
        {
            var save = new Saverows();
            try
            {
                save = await _datagridService.SetDatabaseChanges(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
            return save;
        }
        [HttpPost]
        [Route("DeleteRowsFromGrid")]
        public async Task<Deleterows> DeleteRowsFromGrid(DeleteRowsFromGridReqModel req)
        {
            var model = new Deleterows();
            try
            {
                await _datagridService.DeleteRowsFromGrid(req, model);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
            return model;
        }
        [HttpPost]
        [Route("TaskBarClick")]
        public async Task<GridDataBinding> TaskBarClick(UserInterfaceProps model)
        {
            try
            {
                return await _datagridService.TaskBarClick(model);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("CheckForduplicateId")]
        public async Task<bool> CheckForduplicateId(DatabaseChangesReq req)
        {
            try
            {
                return await Navigation.CheckIfDuplicatePrimaryKeyAsync(req.passport, req.paramss.Tablename, req.paramss.PrimaryKeyname, req.paramss.KeyValue);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("ReportingPerRow")]
        public async Task<ReportingPerRow> ReportingPerRow(ReportingReqModel req)
        {
            try
            {
               return await _datagridService.ExecuteReporting(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("ReportingCount")]
        public async Task<PagingModel> ReportingCount(ReportingReqModel req)
        {
            try
            {
                return await _datagridService.ExecuteReportingCount(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("OnDropdownChange")]
        public async Task<RetentionInfo> OnDropdownChange(RetentionInfoUpdateReqModel req)
        {
            try
            {
                return await _datagridService.OnDropdownChange(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("RetentionInfoUpdate")]
        public async Task<RetentionInfo> RetentionInfoUpdate(RetentionInfoUpdateReqModel req)
        {
            try
            {
                return await _datagridService.RetentionInfoUpdate(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("CheckBeforeAddTofavorite")]
        public async Task<bool> CheckBeforeAddTofavorite(UserInterfaceProps props)
        {
            try
            {
               return await _datagridService.CheckBeforeAddTofavorite(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("StartDialogAddToFavorite")]
        public async Task<MyFavorite> StartDialogAddToFavorite(UserInterfaceProps props)
        {
            try
            {
                return await _datagridService.StartDialogAddToFavorite(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("LoadDialogReportSearch")]
        public async Task<AuditReportSearch> LoadDialogReportSearch(UserInterfaceProps props)
        {
            try
            {
                return await _datagridService.loadDialogReportSearch(props);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("GetRetentionInfoPerRow")]
        public async Task<RetentionInfo> GetRetentionInfoPerRow(RetentionInfoUpdateReqModel req)
        {
            try
            {
               return await _datagridService.GetRetentionInfoPerRow(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("DialogMsgConfirmDelete")]
        public List<string> DialogMsgConfirmDelete(DialogMsgConfirm req)
        {
            try
            {
                return _datagridService.DialogMsgConfirmDelete(req);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

    }
}