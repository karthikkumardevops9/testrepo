using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ScanBatch
    {
        public int Id { get; set; }
        private Nullable<DateTime> _BatchStartDateTime;
        public Nullable<DateTime> BatchStartDateTime
        {
            get
            {
                return _BatchStartDateTime;
            }
            set { _BatchStartDateTime = value; }
        }
        private Nullable<int> _PageCount;
        public Nullable<int> PageCount
        {
            get
            {
                if (_PageCount == null)
                    return 0;
                else
                    return _PageCount;
            }
            set { _PageCount = value; }
        }
        private Nullable<int> _DocumentCount;
        public Nullable<int> DocumentCount
        {
            get
            {
                if (_DocumentCount == null)
                    return 0;
                else
                    return _DocumentCount;
            }
            set { _DocumentCount = value; }
        }
        private Nullable<short> _BelowDeleteSizeCount;
        public Nullable<short> BelowDeleteSizeCount
        {
            get
            {
                return _BelowDeleteSizeCount;
            }
            set { _BelowDeleteSizeCount = value; }
        }
        private Nullable<int> _RescannedCount;
        public Nullable<int> RescannedCount
        {
            get
            {
                if (_RescannedCount == null)
                    return 0;
                else
                    return _RescannedCount;
            }
            set { _RescannedCount = value; }
        }
        private Nullable<int> _AutoIndexedCount;
        public Nullable<int> AutoIndexedCount
        {
            get
            {
                if (_AutoIndexedCount == null)
                    return 0;
                else
                    return _AutoIndexedCount;
            }
            set { _AutoIndexedCount = value; }
        }
        private Nullable<int> _LastScanSequence;
        public Nullable<int> LastScanSequence
        {
            get
            {
                if (_LastScanSequence == null)
                    return 0;
                else
                    return _LastScanSequence;
            }
            set { _LastScanSequence = value; }
        }
        public string ScanRulesIdUsed { get; set; }
        public string UserName { get; set; }
        private Nullable<int> _RecordType;
        public Nullable<int> RecordType
        {
            get
            {
                if (_RecordType == null)
                    return 0;
                else
                    return _RecordType;
            }
            set { _RecordType = value; }
        }
    }
}
