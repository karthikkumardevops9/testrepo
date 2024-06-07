using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLRetentionInactive
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
        private Nullable<int> _Batch;
        public Nullable<int> Batch
        {
            get
            {
                if (_Batch == null)
                    return 0;
                else
                    return _Batch;
            }
            set { _Batch = value; }
        }
        public string RetentionCode { get; set; }
        private Nullable<DateTime> _ScheduledInactivity;
        public Nullable<DateTime> ScheduledInactivity
        {
            get
            {
                return _ScheduledInactivity;
            }
            set { _ScheduledInactivity = value; }
        }
        private Nullable<DateTime> _EventDate;
        public Nullable<DateTime> EventDate
        {
            get
            {
                return _EventDate;
            }
            set { _EventDate = value; }
        }
        public string DeptOfRecord { get; set; }
        public string HoldReason { get; set; }
        private Nullable<bool> _LegalHold;
        public Nullable<bool> LegalHold
        {
            get
            {
                if (_LegalHold == null)
                    return false;
                else
                    return _LegalHold;
            }
            set { _LegalHold = value; }
        }
        private Nullable<bool> _RetentionHold;
        public Nullable<bool> RetentionHold
        {
            get
            {
                if (_RetentionHold == null)
                    return false;
                else
                    return _RetentionHold;
            }
            set { _RetentionHold = value; }
        }
        private Nullable<bool> _RetentionCodeHold;
        public Nullable<bool> RetentionCodeHold
        {
            get
            {
                if (_RetentionCodeHold == null)
                    return false;
                else
                    return _RetentionCodeHold;
            }
            set { _RetentionCodeHold = value; }
        }
        private Nullable<int> _SLDestructCertItemId;
        public Nullable<int> SLDestructCertItemId
        {
            get
            {
                if (_SLDestructCertItemId == null)
                    return 0;
                else
                    return _SLDestructCertItemId;
            }
            set { _SLDestructCertItemId = value; }
        }
        public string FileRoomOrder { get; set; }
        private Nullable<bool> _Selected;
        public Nullable<bool> Selected
        {
            get
            {
                if (_Selected == null)
                    return false;
                else
                    return _Selected;
            }
            set { _Selected = value; }
        }
    }
}
