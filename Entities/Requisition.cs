using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Requisition
    {
        public string ReqId { get; set; }
        private Nullable<DateTime> _ReqDate;
        public Nullable<DateTime> ReqDate
        {
            get
            {
                return _ReqDate;
            }
            set { _ReqDate = value; }
        }
        public string Requestor { get; set; }
        public string Department { get; set; }
    }
}
