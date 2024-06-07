using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class TrackingHistory
    {
        public int Id { get; set; }
        public string TrackedTableId { get; set; }
        public string TrackedTable { get; set; }
        public string LocationsId { get; set; }
        public string EmployeesId { get; set; }
        public string BoxesId { get; set; }
        private Nullable<DateTime> _TransactionDateTime;
        public Nullable<DateTime> TransactionDateTime
        {
            get
            {
                return _TransactionDateTime;
            }
            set { _TransactionDateTime = value; }
        }
        private Nullable<bool> _IsActualScan;
        public Nullable<bool> IsActualScan
        {
            get
            {
                if (_IsActualScan == null)
                    return false;
                else
                    return _IsActualScan;
            }
            set { _IsActualScan = value; }
        }
        private Nullable<int> _BatchId;
        public Nullable<int> BatchId
        {
            get
            {
                if (_BatchId == null)
                    return 0;
                else
                    return _BatchId;
            }
            set { _BatchId = value; }
        }
        public string UserName { get; set; }
        public string TrackingAdditionalField1 { get; set; }
        public string TrackingAdditionalField2 { get; set; }
    }
}
