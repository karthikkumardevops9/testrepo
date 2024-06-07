using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class AssetStatu
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
        private Nullable<DateTime> _ProcessedDateTime;
        public Nullable<DateTime> ProcessedDateTime
        {
            get
            {
                return _ProcessedDateTime;
            }
            set { _ProcessedDateTime = value; }
        }
        private Nullable<bool> _Out;
        public Nullable<bool> Out
        {
            get
            {
                if (_Out == null)
                    return false;
                else
                    return _Out;
            }
            set { _Out = value; }
        }
        public string TrackingAdditionalField1 { get; set; }
        public string TrackingAdditionalField2 { get; set; }
        public string UserName { get; set; }
        private Nullable<DateTime> _DateDue;
        public Nullable<DateTime> DateDue
        {
            get
            {
                return _DateDue;
            }
            set { _DateDue = value; }
        }
    }
}
