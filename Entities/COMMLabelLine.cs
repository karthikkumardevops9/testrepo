using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class COMMLabelLine
    {
        public int Id { get; set; }
        private Nullable<int> _COMMLabelId;
        public Nullable<int> COMMLabelId
        {
            get
            {
                if (_COMMLabelId == null)
                    return 0;
                else
                    return _COMMLabelId;
            }
            set { _COMMLabelId = value; }
        }
        public string FieldName { get; set; }
        public string TextStartString { get; set; }
        public string TextEndString { get; set; }
        private Nullable<bool> _FindByField;
        public Nullable<bool> FindByField
        {
            get
            {
                if (_FindByField == null)
                    return false;
                else
                    return _FindByField;
            }
            set { _FindByField = value; }
        }
    }
}
