using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLDestructCertItem
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
        private Nullable<int> _SLDestructionCertsId;
        public Nullable<int> SLDestructionCertsId
        {
            get
            {
                if (_SLDestructionCertsId == null)
                    return 0;
                else
                    return _SLDestructionCertsId;
            }
            set { _SLDestructionCertsId = value; }
        }
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
        public string HoldReason { get; set; }
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
        private Nullable<DateTime> _ScheduledDestruction;
        public Nullable<DateTime> ScheduledDestruction
        {
            get
            {
                return _ScheduledDestruction;
            }
            set { _ScheduledDestruction = value; }
        }
        private Nullable<DateTime> _SnoozeUntil;
        public Nullable<DateTime> SnoozeUntil
        {
            get
            {
                return _SnoozeUntil;
            }
            set { _SnoozeUntil = value; }
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
        private Nullable<bool> _RetentionUpdated;
        public Nullable<bool> RetentionUpdated
        {
            get
            {
                if (_RetentionUpdated == null)
                    return false;
                else
                    return _RetentionUpdated;
            }
            set { _RetentionUpdated = value; }
        }
        private Nullable<bool> _DispositionFlag;
        public Nullable<bool> DispositionFlag
        {
            get
            {
                if (_DispositionFlag == null)
                    return false;
                else
                    return _DispositionFlag;
            }
            set { _DispositionFlag = value; }
        }
        private Nullable<DateTime> _DispositionDate;
        public Nullable<DateTime> DispositionDate
        {
            get
            {
                return _DispositionDate;
            }
            set { _DispositionDate = value; }
        }
        public string ApprovedBy { get; set; }
        private Nullable<int> _DispositionType;
        public Nullable<int> DispositionType
        {
            get
            {
                if (_DispositionType == null)
                    return 0;
                else
                    return _DispositionType;
            }
            set { _DispositionType = value; }
        }
    }
}
