using Microsoft.AspNetCore.Http;
using MSRecordsEngine.Entities;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Collections.Generic;

namespace MSRecordsEngine.Models.FusionModels
{
    public partial class DashboardModel
    {
        public string DashboardListHtml { get; set; }
        public string DashboardListJsonS { get; set; }
        public string LanguageCulture { get; set; }
        public string ErrorMessage { get; set; }
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

    public partial class ChartModel
    {
        public string X;
        public int Y;
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
