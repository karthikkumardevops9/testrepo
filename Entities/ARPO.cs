using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ARPO
    {
        public string POId { get; set; }
        private Nullable<DateTime> _OrderDate;
        public Nullable<DateTime> OrderDate
        {
            get
            {
                return _OrderDate;
            }
            set { _OrderDate = value; }
        }
        public string CustomerId { get; set; }
        public string Customer { get; set; }
    }
}
