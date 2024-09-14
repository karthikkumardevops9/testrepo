using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IDataGridService
    {
        public Task SaveNewsURL(NewUrlprops model);
        public Task<ViewQueryWindow> DrawQuery(ViewQueryWindowProps prop);
        public Task<GridDataBinding> RunQuery(SearchQueryRequestModal prop);
        public Task<string> GetTotalRowsForGrid(SearchQueryRequestModal prop);
        public Task<ScriptReturn> LinkscriptButtonClick([FromBody] linkscriptPropertiesUI props);
        public LinkScriptModel BuiltControls(ScriptReturn scriptresult);
        public Task<ScriptReturn> LinkscriptEvents(linkscriptPropertiesUI props);
        public Task<bool> FlowButtonsClickEvent(linkscriptPropertiesUI props);
        public Task<TabquikApi> TabQuikInitiator(TabquickpropUI props);
        public Task<TrackingModeld> GetTrackingPerRow(trackableUiParams props);
        public Task<GridDataBinding> BuildNewFavoriteData(ReturnFavoritTogridReqModel req);
        public Task<bool> DeleteSavedCriteria(FavoriteRecordReqModel req);
        public Task<MyFavorite> AddNewFavorite(FavoriteRecordReqModel req);
        public Task<MyFavorite> UpdateFavorite(FavoriteRecordReqModel req);
        public Task<bool> DeleteFavoriteRecord(FavoriteRecordReqModel req);
        public Task<Myquery> SaveNewQuery(SaveNewUpdateDeleteQueryReqModel req);
        public Task<Myquery> UpdateQuery(SaveNewUpdateDeleteQueryReqModel req);
        public Task<Myquery> DeleteQuery(SaveNewUpdateDeleteQueryReqModel req);
        public Task<GlobalSearch> RunglobalSearch(GlobalSearchReqModel req);
        public Task<GridDataBinding> GlobalSearchClick(GlobalSearchReqModel props);
        public Task<GridDataBinding> GlobalSearchAllClick(GlobalSearchReqModel req);
        public Task<Saverows> SetDatabaseChanges(DatabaseChangesReq props);
        public Task DeleteRowsFromGrid(DeleteRowsFromGridReqModel req, Deleterows model);
        public Task<GridDataBinding> TaskBarClick(UserInterfaceProps props);
        public Task<ReportingPerRow> ExecuteReporting(ReportingReqModel props);
        public Task<PagingModel> ExecuteReportingCount(ReportingReqModel props);
        public Task<RetentionInfo> OnDropdownChange(RetentionInfoUpdateReqModel req);
        public Task<RetentionInfo> RetentionInfoUpdate(RetentionInfoUpdateReqModel req);
        public Task<bool> CheckBeforeAddTofavorite(UserInterfaceProps props);
        public Task<MyFavorite> StartDialogAddToFavorite(UserInterfaceProps props);
        public Task<AuditReportSearch> loadDialogReportSearch(UserInterfaceProps props);
        public Task<RetentionInfo> GetRetentionInfoPerRow(RetentionInfoUpdateReqModel req);
        public List<string> DialogMsgConfirmDelete(DialogMsgConfirm req);
    }
}
