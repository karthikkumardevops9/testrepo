using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MSRecordsEngine.Models
{
    public class UserInterfaceProps
    {
        public Passport passport { get; set; }
        public int ViewId { get; set; }
        public string TableId { get; set; }
        public string DateFormat { get; set; }
        public string StringHolder { get; set; }
    }
    public class NewUrlprops : UserInterfaceProps
    {
        public string NewUrl { get; set; }
    }

    public class ViewQueryWindowProps : UserInterfaceProps
    {
        public int ceriteriaId { get; set; }
        public string ChildKeyField { get; set; }
        public int crumblevel { get; set; }
    }
    public class SearchQueryRequestModal : UserInterfaceProps
    {
        public SearchQueryRequestModal()
        {
            paramss = new Searchparams();
            GridDataBinding = new GridDataBinding();
        }
        public Searchparams paramss { get; set; }
        public Searchparams paramsUI { get; set; }
        public List<searchQueryModel> searchQuery { get; set; }
        public string HoldTotalRowQuery { get; set; }
        public GridDataBinding GridDataBinding { get; set; }
    }

    public class Searchparams
    {
        public int ViewId { get; set; }
        public int pageNum { get; set; }
        public string ChildKeyField { get; set; }
        public string keyFieldValue { get; set; }
        public int firstCrumbChild { get; set; } = 0;
        public string columntype { get; set; }
        public string rowid { get; set; }
        public string preTableName { get; set; }
        public string Childid { get; set; }
        public string password { get; set; }
        public int ViewType { get; set; } = 0;
        public int crumbLevel { get; set; } = 0;
    }
    public class linkscriptPropertiesUI : UserInterfaceProps
    {
        public string WorkFlow { get; set; }
        public string[] Rowids { get; set; }
        public InternalEngine InternalEngine { get; set; }
    }
    public class TabquickpropUI : UserInterfaceProps
    {
        public string RowsSelected { get; set; }
        public string WebRootPath { get; set; }

    }
    public class trackableUiParams : UserInterfaceProps
    {
        public string RowKeyid { get; set; }
        public string TableName { get; set; }
    }


    public class ReturnFavoritTogridReqModel : UserInterfaceProps
    {
        public UiParams paramss { get; set; }

        public List<searchQueryModel> searchQuery { get; set; }
        public List<UirowsList> recordkeys { get; set; }
    }
    public class UirowsList
    {
        public string rowKeys { get; set; }

    }
    public class UiParams
    {
        public int ViewId { get; set; }

        public int FavCriteriaid { get; set; }

        public string FavCriteriaType { get; set; }

        public string NewFavoriteName { get; set; }

        public List<UirowsList> RowsSelected { get; set; }
        public int pageNum { get; set; }
    }

    public class FavoriteRecordReqModel : UserInterfaceProps
    {
        public UiParams paramss { get; set; }
        public List<UirowsList> recordkeys { get; set; }
    }

    public class SaveNewUpdateDeleteQueryReqModel : UserInterfaceProps
    {
        public queryListparams paramss { get; set; }
        public List<queryListparams> Querylist { get; set; }
    }
    public class queryListparams
    {
        public string operators { get; set; }
        public string columnName { get; set; }
        public string values { get; set; }
        public string SaveName { get; set; }
        public int ViewId { get; set; }
        public int SavedCriteriaid { get; set; }
        public int type { get; set; }
        public string ColumnType { get; set; }
    }

    public class GlobalSearchReqModel : UserInterfaceProps
    {
        public globalSearchUI paramss { get; set; }
    }
    public class globalSearchUI
    {
        public int ViewId { get; set; }
        public string TableName { get; set; }
        public int Currentrow { get; set; }
        public string SearchInput { get; set; }
        public bool ChkAttch { get; set; }
        public bool ChkcurTable { get; set; }
        public bool ChkUnderRow { get; set; }
        public string KeyValue { get; set; }
        public bool IncludeAttchment { get; set; }
        public int crumbLevel { get; set; }
    }
    public class DatabaseChangesReq : UserInterfaceProps
    {
        public DatabaseChangesReq()
        {
            Rowdata = new List<RowsparamsUI>();
        }
        public List<RowsparamsUI> Rowdata { get; set; }
        public paramsUI paramss { get; set; }
    }
    public class RowsparamsUI
    {
        public string value { get; set; }
        public string columnName { get; set; }
        public string DataTypeFullName { get; set; }
        public List<string> ids { get; set; }
    }
    public class paramsUI
    {
        public int ViewId { get; set; }
        public string BeforeChange { get; set; }
        public string AfterChange { get; set; }
        public string Tablename { get; set; }
        public string KeyValue { get; set; }
        public string PrimaryKeyname { get; set; }
        public bool scriptDone { get; set; }
        public bool IsNewRow { get; set; }
        public int crumbLevel { get; set; }
        public string childkeyfield { get; set; }

    }
    public class DeleteRowsFromGridReqModel : UserInterfaceProps
    {
        public RowsparamsUI rowData { get; set; }
        public paramsUI paramss { get; set; }
    }

    public class reportRowUIparams
    {
        public string Tableid { get; set; }
        public string tableName { get; set; }
        public int viewId { get; set; }
        public int reportNum { get; set; }
        public int pageNumber { get; set; }
        public string childid { get; set; }
        public int reportId { get; set; }
    }
    public class ReportingReqModel : UserInterfaceProps
    {
        public reportRowUIparams paramss { get; set; }
    }
    public class RetentionInfoUpdateReqModel : UserInterfaceProps
    {
        public retentionInfoUIparams props { get; set; }
    }
    public class retentionInfoUIparams
    {
        public retentionInfoUIparams()
        {
            RetTableHolding = new List<holdingTableprop>();
        }
        public string rowid { get; set; }
        public int viewid { get; set; }
        public string RetDescription { get; set; }
        public string RetentionItemText { get; set; }
        public string TableName { get; set; }
        public string RetentionItemCode { get; set; }
        public string RetnArchive { get; set; }
        public string RetnInactivityDate { get; set; }
        public List<holdingTableprop> RetTableHolding { get; set; }
        public int SldestructionCertId { get; set; }
    }
    public class DialogMsgConfirm : UserInterfaceProps
    {
        public DialogMSgConfirmParams paramss { get; set; }
    }
    public class DialogMSgConfirmParams
    {
        public List<string> ids { get; set; }
        public string TableName { get; set; }
        public int ViewId { get; set; }
    }

    public class UIproperties : UserInterfaceProps
    {
        public string UserName { get; set; }
        public string UserDDLId { get; set; }
        public string ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool AddEditDelete { get; set; } = false;
        public bool SuccessLogin { get; set; } = false;
        public bool ConfDataAccess { get; set; } = false;
        public bool FailedLogin { get; set; } = false;
        public bool ChildTable { get; set; } = false;
        public int PageNumber { get; set; } = 0;
    }
    public class ReportingJsonModelReq : UserInterfaceProps
    {
        public ReportingJsonModel paramss { get; set; }
    }
    public class ReportingJsonModel
    {
        public ReportingJsonModel()
        {
            ids = new List<string>();
            ListofPullItem = new List<Items>();
        }
        public int reportType { get; set; }
        public int pageNumber { get; set; }
        public string tableName { get; set; } = "";
        public List<Items> ListofPullItem { get; set; }
        public string id { get; set; } = "";
        public bool isQueryFromDDL { get; set; }
        public bool isBatchRequest { get; set; }
        public List<string> ids { get; set; }
        public string udate { get; set; }
        public string ddlSelected { get; set; }
        public string username { get; set; }
        public string locationId { get; set; }
        public string submitType { get; set; }
        public int reportId { get; set; }
        public bool isCountRecord { get; set; }
    }

    public class Items
    {
        public string tableName { get; set; }
        public string tableid { get; set; }
    }

}
