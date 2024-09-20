using Microsoft.AspNetCore.Http;
using Smead.Security;
using System.Collections.Generic;

namespace MSRecordsEngine.Models.FusionModels
{
    public class Exporter : BaseModel
    {
        public string lblTitle { get; set; }
        public string HtmlMessage { get; set; }
        public bool isRequireBtn { get; set; }
        public bool IsBackgroundProcessing { get; set; }
        public bool Permission { get; set; } = true;
        public ExporterJsonModel ParamVal { get; set; }
    }
    public class ExporterJsonModel
    {
        public string tableName { get; set; } = "";
        public string viewName { get; set; } = "";
        public int viewId { get; set; } = 0;
        public List<HeaderProps> Headers { get; set; }
        public List<string> ListofselectedIds { get; set; }
        public List<List<string>> DataRows { get; set; }
        public bool IsBackgroundProcessing { get; set; } = false;
        public int TaskType { get; set; } = 0;
        public string FileName { get; set; }
        public int RecordCount { get; set; } = 0;
        public bool IsSelectAllData { get; set; } = false;
        public bool Reconciliation { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string Path { get; set; }
        public string ExportReportPath { get; set; }
        public bool IsCSV { get; set; } = false;
        public int crumbLevel { get; set; } = 0;

    }
    public class HeaderProps
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string HeaderName { get; set; }
    }
    public class ExporterJsonModelReqModel
    {
        public ExporterJsonModel paramss { get; set; }
    }
    public class ExporterReportJsonModel : ExporterJsonModel
    {
        //public AuditReportSearch.UIproperties ReportAuditFilterProperties { get; set; }
        public string ReportType { get; set; }
    }
    public class ExporterReportJsonModelReq
    {
        public ExporterReportJsonModel paramss { get; set; }
    }
    public class ExportAll : BaseModel
    {
        private ExporterJsonModel @params { get; set; }
        public string AllQuery { get; set; }
        public int CurrentLevel { get; set; }
    }

}
