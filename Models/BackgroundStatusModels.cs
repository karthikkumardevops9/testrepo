using Smead.Security;

namespace MSRecordsEngine.Models
{
    public partial class BackgroundServiceTask
    {
        public int Id;
        public string CreateDate;
        public string StartDate;
        public string EndDate;
        public string Type;
        public string Status;
        public string RecordCount;
        public string ReportLocation;
        public string DownloadLocation;
        public string UserName;
    }

    public class BackgrundStatusListParam
    {
        public Passport passport { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public string ShortDatePattern { get; set; }
        public string OffSetTimeVal { get; set; }
    }

}
