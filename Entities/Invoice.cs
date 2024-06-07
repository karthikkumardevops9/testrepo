using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Invoice
    {
        public int Id { get; set; }
        public string POId { get; set; }
        public string InvoiceId { get; set; }
        private Nullable<DateTime> _InvoiceDate;
        public Nullable<DateTime> InvoiceDate
        {
            get
            {
                return _InvoiceDate;
            }
            set { _InvoiceDate = value; }
        }
    }
}
