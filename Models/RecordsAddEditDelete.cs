using MSRecordsEngine.Entities;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{

    // SAVE NEW ROW, EDIT ROW FUNCTIONS RETURN MESSAGE TO DOM
    public class RecordsAddEditDelete
    {
    }
    public class Saverows
    {
        public Saverows()
        {
            scriptReturn = new LinkScriptModel();
            gridDatabinding = new GridDataBinding();
        }
        private int crumbLevel { get; set; }
        private string clientIpAddress { get; set; }
        private int _viewId { get; set; }
        private string _beforeChange { get; set; }
        private string _afterChange { get; set; }
        private bool _isSavefails { get; set; }
        public LinkScriptModel scriptReturn { get; set; }
        public bool IsNewRecord { get; set; }
        public string keyvalue { get; set; }
        public bool scriptDone { get; set; }
        public string TableName { get; set; }
        public string Msg {  get; set; }
        public bool isError { get; set; }
        public InternalEngine LinkScriptSession { get; set; }

        public GridDataBinding gridDatabinding { get; set; }

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
        public class RowsparamsUI
        {
            public string value { get; set; }
            public string columnName { get; set; }
            public string DataTypeFullName { get; set; }
        }

    }
    // DELETE ROWS FUNCTIONS RETURN MESSAGE TO DOM
    public class Deleterows
    {
        public Deleterows()
        {
            scriptReturn = new LinkScriptModel();
        }
        
        private List<string> ids { get; set; }
        private int _viewid { get; set; }
        private string clientIpAddress { get; set; }
        public LinkScriptModel scriptReturn { get; set; }
        public bool scriptDone { get; set; }

        public bool isError { get; set; }
        public string Msg { get; set; }
        public InternalEngine LinkScriptSession { get; set; }
        public int linkscriptSequence { get; set; }

        public class RowparamsUI
        {
            public List<string> ids { get; set; }
        }
        public class paramsUI
        {
            public int viewid { get; set; }
            public bool ScriptDone { get; set; }
        }
    }
}