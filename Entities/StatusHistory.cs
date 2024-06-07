using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class StatusHistory
    {
        public int Id { get; set; }
        private Nullable<int> _FoldersId;
        public Nullable<int> FoldersId
        {
            get
            {
                if (_FoldersId == null)
                    return 0;
                else
                    return _FoldersId;
            }
            set { _FoldersId = value; }
        }
        public string Operator { get; set; }
        private Nullable<DateTime> _StatusChangeDateTime;
        public Nullable<DateTime> StatusChangeDateTime
        {
            get
            {
                return _StatusChangeDateTime;
            }
            set { _StatusChangeDateTime = value; }
        }
        public string NewStatus { get; set; }
    }
}
