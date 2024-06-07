using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class PurchaseOrder
    {
        public string POId { get; set; }
        public string ReqId { get; set; }
        private Nullable<DateTime> _PODate;
        public Nullable<DateTime> PODate
        {
            get
            {
                return _PODate;
            }
            set { _PODate = value; }
        }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string RetentionCodesId { get; set; }
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
