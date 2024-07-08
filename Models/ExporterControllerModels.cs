using MSRecordsEngine.Models.FusionModels;
using Smead.Security;

namespace MSRecordsEngine.Models
{
    public class DialogConfirmExportParam
    {
        public ExporterJsonModelReqModel req { get; set; }
        public Passport passport { get; set; }
        public string sql { get; set; }
    }

    public class BuildStringParam
    {
        public Passport passport { get; set; }
        public bool DetectTime { get; set; }
        public int ParamViewId { get; set; }
        public string Query { get; set; }
        public int CurrentLevel { get; set; }
        public string CultureShortPattern { get; set; }
        public string OffSetVal { get; set; }

    }

    public class DialogConfirmExportReportParam
    {
        public ExporterReportJsonModelReq req { get; set; }
        public Passport passport { get; set; }
    }
    
    public class BackGroundProcessingParam
    {
        public bool isDataProcessingNetworkPath { get; set; }
        public string dataProcessingFilesPath { get; set; }
        public ExporterJsonModel exporterData { get; set; }
        public string rootPath{ get; set; }
        public string rowQuery { get; set; }
        public Passport passport { get; set; }

    }
   
    public class BuildStrForSelectedtParam
    {
        public ExporterJsonModel exporterData { get; set; }
        public string CultureShortPattern { get; set; }
        public string OffSetVal { get; set; }
    }

}
