using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Trackable
    {
        public int Id { get; set; }
        public short RecordVersion { get; set; }
        private Nullable<int> _RecordTypesId;
        public Nullable<int> RecordTypesId
        {
            get
            {
                if (_RecordTypesId == null)
                    return 0;
                else
                    return _RecordTypesId;
            }
            set { _RecordTypesId = value; }
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
        private Nullable<bool> _Orphan;
        public Nullable<bool> Orphan
        {
            get
            {
                if (_Orphan == null)
                    return false;
                else
                    return _Orphan;
            }
            set { _Orphan = value; }
        }
        private Nullable<bool> _Verified;
        public Nullable<bool> Verified
        {
            get
            {
                if (_Verified == null)
                    return false;
                else
                    return _Verified;
            }
            set { _Verified = value; }
        }
        private Nullable<bool> _CheckedOut;
        public Nullable<bool> CheckedOut
        {
            get
            {
                if (_CheckedOut == null)
                    return false;
                else
                    return _CheckedOut;
            }
            set { _CheckedOut = value; }
        }
        private Nullable<DateTime> _CheckedOutDate;
        public Nullable<DateTime> CheckedOutDate
        {
            get
            {
                return _CheckedOutDate;
            }
            set { _CheckedOutDate = value; }
        }
        public string CheckedOutUser { get; set; }
        public string CheckedOutIP { get; set; }
        public string CheckedOutMAC { get; set; }
        public string CheckedOutFolder { get; set; }
        private Nullable<bool> _CheckoutLocked;
        public Nullable<bool> CheckoutLocked
        {
            get
            {
                if (_CheckoutLocked == null)
                    return false;
                else
                    return _CheckoutLocked;
            }
            set { _CheckoutLocked = value; }
        }
        private Nullable<bool> _OfficialRecord;
        public Nullable<bool> OfficialRecord
        {
            get
            {
                if (_OfficialRecord == null)
                    return false;
                else
                    return _OfficialRecord;
            }
            set { _OfficialRecord = value; }
        }
        private Nullable<bool> _OfficialRecordReconciliation;
        public Nullable<bool> OfficialRecordReconciliation
        {
            get
            {
                if (_OfficialRecordReconciliation == null)
                    return false;
                else
                    return _OfficialRecordReconciliation;
            }
            set { _OfficialRecordReconciliation = value; }
        }
        private Nullable<int> _CheckedOutUserId;
        public Nullable<int> CheckedOutUserId
        {
            get
            {
                if (_CheckedOutUserId == null)
                    return 0;
                else
                    return _CheckedOutUserId;
            }
            set { _CheckedOutUserId = value; }
        }
    }
}
