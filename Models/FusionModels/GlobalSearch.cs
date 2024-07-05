using Microsoft.AspNetCore.Http;
using MSRecordsEngine.RecordsManager;
using Smead.Security;

namespace MSRecordsEngine.Models.FusionModels
{
    public class GlobalSearch : BaseModel
    {
        private LevelManager _levelmanager;
        private int _viewId;
        private string _tableName;
        private int _rowid;
        private string _InputTxt;
        private bool _chkAttch;
        private bool _chkCurTable;
        private bool _chkUnderRow;
        public string HTMLSearchResults;
        
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

    public class GlobalSearchReqModel
    {
        public globalSearchUI paramss { get; set; }
    }
}
