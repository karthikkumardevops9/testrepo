using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLGrabberField
    {
        public int Id { get; set; }
        private Nullable<int> _SLGrabberFunctionsId;
        public Nullable<int> SLGrabberFunctionsId
        {
            get
            {
                if (_SLGrabberFunctionsId == null)
                    return 0;
                else
                    return _SLGrabberFunctionsId;
            }
            set { _SLGrabberFunctionsId = value; }
        }
        private Nullable<int> _Sequence;
        public Nullable<int> Sequence
        {
            get
            {
                if (_Sequence == null)
                    return 0;
                else
                    return _Sequence;
            }
            set { _Sequence = value; }
        }
        public string SmeadlinkFieldName { get; set; }
        private Nullable<bool> _IsIdField;
        public Nullable<bool> IsIdField
        {
            get
            {
                if (_IsIdField == null)
                    return false;
                else
                    return _IsIdField;
            }
            set { _IsIdField = value; }
        }
        private Nullable<bool> _IsOkayIfMissing;
        public Nullable<bool> IsOkayIfMissing
        {
            get
            {
                if (_IsOkayIfMissing == null)
                    return false;
                else
                    return _IsOkayIfMissing;
            }
            set { _IsOkayIfMissing = value; }
        }
        public string SmeadlinkTableName { get; set; }
    }
}
