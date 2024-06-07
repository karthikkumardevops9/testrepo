using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Correspondence
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        private Nullable<DateTime> _Date;
        public Nullable<DateTime> Date
        {
            get
            {
                return _Date;
            }
            set { _Date = value; }
        }
        public string Description { get; set; }
        public string RetentionCodesId { get; set; }
        private Nullable<DateTime> _DateClosed;
        public Nullable<DateTime> DateClosed
        {
            get
            {
                return _DateClosed;
            }
            set { _DateClosed = value; }
        }
        private Nullable<DateTime> _DateOpened;
        public Nullable<DateTime> DateOpened
        {
            get
            {
                return _DateOpened;
            }
            set { _DateOpened = value; }
        }
        private Nullable<DateTime> _DateOther;
        public Nullable<DateTime> DateOther
        {
            get
            {
                return _DateOther;
            }
            set { _DateOther = value; }
        }
        public bool C_slRetentionInactive { get; set; }
        private Nullable<bool> _C_slRetentionInactiveFinal;
        public Nullable<bool> C_slRetentionInactiveFinal
        {
            get
            {
                if (_C_slRetentionInactiveFinal == null)
                    return false;
                else
                    return _C_slRetentionInactiveFinal;
            }
            set { _C_slRetentionInactiveFinal = value; }
        }
        private Nullable<int> _C_slRetentionDispositionStatus;
        public Nullable<int> C_slRetentionDispositionStatus
        {
            get
            {
                if (_C_slRetentionDispositionStatus == null)
                    return 0;
                else
                    return _C_slRetentionDispositionStatus;
            }
            set { _C_slRetentionDispositionStatus = value; }
        }
    }
}
