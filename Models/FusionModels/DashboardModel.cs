using Microsoft.AspNetCore.Http;
using MSRecordsEngine.Entities;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Collections.Generic;

namespace MSRecordsEngine.Models.FusionModels
{
    public partial class DashboardModel : BaseModel
    {
        public TasksBar Taskbar { get; set; }
        public string DashboardListHtml { get; set; }
        public string DashboardListJsonS { get; set; }
        public string LanguageCulture { get; set; }

        private List<SLUserDashboard> DashboardList;
        public string ErrorMessage { get; set; }
    }

    public partial class DashboardJsonModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }
    }

    public partial class DashboardDropdown : BaseModel
    {
        public DashboardDropdown()
        {
            WorkGroup = new List<WorkGroupItem>();
            Table = new List<TableItem>();
        }
        public List<WorkGroupItem> WorkGroup;
        public List<TableItem> Table;
        public List<ViewItem> View;
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

    public partial class DashboardDataModel : BaseModel
    {
        public string ViewId;
        public string FieldName;
        public string TableName;
        private string Query;
        private HttpContext _httpContext;
        public List<CommonDropdown> ViewColumnEntity { get; set; }
        public DashboardDataModel()
        {
            ViewColumnEntity = new List<CommonDropdown>();
        }
    }

    public partial class ChartModel
    {
        public string X;
        public int Y;
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

    public partial class TableModel
    {
        public string TableName;
    }
}
