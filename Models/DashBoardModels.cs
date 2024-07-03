using MSRecordsEngine.Entities;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Collections.Generic;
using System.Globalization;

namespace MSRecordsEngine.Models
{
    public class DashBoardParam
    {
        public int UserId { get; set; }
        public string ConnectionString { get; set; }
    } 
    public class DashBoardReturn
    {
        public string DashboardListHtml { get; set; }
        public string LanguageCulture { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class GetWorkGroupTableMenuParam
    {
        public short WorkGroupId { get; set; }
        public Passport Passport { get; set; }
    }
    public class ReturnWorkGroupTableMenu
    {
        public string WorkGroupMenuString { get; set; }
        public bool isError { get; set; } = false;
        public string Msg { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class SetDashboardDetailsParam
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
    public class SetDashboardDetailsResonse
    {
        public string ErrorMessage { get; set; }
        public string DashboardListHtml { get; set; }
        public SLUserDashboard ud { get; set; }
        public bool isError { get; set; } = false;
        public string Msg { get; set; }
    }
    public class GetViewMenuParams
    {
        public Passport passport { get; set; }
        public string TableName { get; set; }
    }
    public class GetViewMenuReturns
    {
        public string ViewsTbNameString { get; set; }
        public bool isError { get; set; } = false;
        public string Msg { get; set; }
    }
    public class GetDashboardDetailParam
    {
        public string ConnectionString { get; set; }
        public int DashboardId { get; set; }
    }
    public class GetDashboardDetailsReturn
    {
        public bool isError { get; set; } = false;
        public string Msg { get; set; }
        public SLUserDashboard ud { get; set; }
    }
    public partial class CommonModel
    {
        public int Id;
        public string Name;
        public string UserName;
    }
    public partial class CommonDropdown : CommonModel
    {
        public string SId;
        public string FieldName;
    }
    public class GetViewColumnMenuParam
    {
        public int ViewId { get; set; }
        public Passport Passport { get; set; }
        public string ShortDatePattern { get; set; }
        public CultureInfo culture { get; set; }
    }
    public class SetDashboardJsonParam
    {
        public string Json { get; set; }
        public string ConnectionString { get; set; }
        public int DashboardId { get; set; }

    }
    public class SetDashboardJsonReturn
    {
        public SLUserDashboard MyProperty { get; set; }
        public bool isError { get; set; } = false;
        public string Msg { get; set; }
        public SLUserDashboard ud { get; set; }
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
    public class RenameDashboardNameReturn
    {
        public bool isError { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string DashboardListHtml { get; set; }
        public SLUserDashboard ud { get; set; }
        public string Msg { get; set; }
    }
    public class DeleteDashboardParam
    {
        public int DashboardId { get; set; }
        public string ConnectionString { get; set; }
    }
    public class DeleteDashboardReturn
    {
        public bool isError { get; set; } = false;
        public string Msg { get; set; }
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
    public partial class TableModel
    {
        public string TableName;
    }
    public class widgetDataParam
    {
        public Passport passport { get; set; }
        public string widgetObjectJson { get; set; }
    }
    public partial class ChartModel
    {
        public string X;
        public int Y;
    }
    public class ChartDataResModel
    {
        public bool isError { get; set; } = false;
        public string JsonString { get; set; }
        public string Msg { get; set; }
        public bool Permission { get; set; } = true;
        public string DataString { get; set; }
    }
   
    public partial class ChartOperatinModel
    {
        public string X;
        public int Y;
        public int AuditType;
        public string AuditTypeValue;
    }

    public partial class ChartOperatinModelRes
    {
        public ChartOperatinModelRes()
        {
            Data = new List<ChartModel>();
        }
        public string AuditType;
        public List<ChartModel> Data;
    }
    public partial class OperationChartDataResModel
    {
        public string DataString { get; set; }
        public string JsonString { get; set; } = string.Empty;
        public string TaskList { get; set; }
        public bool Permission { get; set; } = true;
        public int Count { get; set; }
        public bool isError { get; set; } = false;
        public string Msg { get; set; }

    }
}
