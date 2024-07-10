using MSRecordsEngine.Entities;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Collections.Generic;
using System.Globalization;

namespace MSRecordsEngine.Models
{

    public partial class DashboardCommonModel : BaseModel
    {
        public DashboardCommonModel()
        {
            WorkGroupMenu = new List<TableItem>();
            ViewsByTableName = new List<ViewItem>();
        }

        public string ErrorMessage { get; set; }
        public string DashboardListHtml { get; set; }
        public SLUserDashboard ud { get; set; }
        public List<ViewItem> ViewsByTableName { get; set; }
        public List<TableItem> WorkGroupMenu { get; set; }
        public string ListAsString { get; set; }
    }
    public class DashBoardParam
    {
        public int UserId { get; set; }
        public string ConnectionString { get; set; }
    } 
    public class GetWorkGroupTableMenuParam
    {
        public short WorkGroupId { get; set; }
        public Passport Passport { get; set; }
    }
    public class SetDashboardDetailsParam
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
    public class GetViewMenuParams
    {
        public Passport passport { get; set; }
        public string TableName { get; set; }
    }
    public class GetDashboardDetailParam
    {
        public string ConnectionString { get; set; }
        public int DashboardId { get; set; }
    }
    public class GetViewColumnMenuParam
    {
        public int ViewId { get; set; }
        public Passport Passport { get; set; }
        public string ShortDatePattern { get; set; }
        public CultureInfo culture { get; set; }
        public string TableName { get; set; }
    }
    public class SetDashboardJsonParam
    {
        public string Json { get; set; }
        public string ConnectionString { get; set; }
        public int DashboardId { get; set; }

    }
    public class AddEditOperationReturn
    {
        public string Users { get; set; }
        public string AuditTable { get; set; }
        public List<EnumModel> AuditTypeList { get; set; }
    }
    public class RenameDashboardNameParam
    {
        public int DashboardId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string ConnectionString { get; set; }
    }

    public class DeleteDashboardParam
    {
        public int DashboardId { get; set; }
        public string ConnectionString { get; set; }
    }
    public class ValidPermissionParam
    {
        public string WidgetList { get; set; }
        public Passport Passport { get; set; }
    }
    public class ValidPermissionReturn
    {
        public bool isError { get; set; } = false;
        public string Msg { get; set; }
        public string JsonString { get; set; }
    }
    public class widgetDataParam
    {
        public Passport passport { get; set; }
        public string widgetObjectJson { get; set; }
    }
    public partial class ChartDataResModel : BaseModel
    {
        public string JsonString { get; set; }
        public bool Permission { get; set; } = true;
        public string DataString { get; set; }
        public string TaskList { get; set; }
    }
   
    public partial class ChartOperatinModel
    {
        public string X;
        public int Y;
        public int AuditType;
        public string AuditTypeValue;
    }

    public partial class OperationChartDataResModel : BaseModel
    {
        public string DataString { get; set; }
        public string JsonString { get; set; } = string.Empty;
        public string TaskList { get; set; }
        public bool Permission { get; set; } = true;
        public int Count { get; set; }
    }

    public class UpdateFavDashboardParam
    {
        public int DashboardId { get; set; }
        public bool IsFav { get; set; }
        public string ConnectionString { get; set; }
        public int UserId { get; set; }
    }

    public class CountDataReturn
    {
        public string JsonString { get; set; }
        public int Count { get; set; }
        public bool isError { get; set; } = false;
    }
}
