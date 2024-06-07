using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ARINV
    {
        public int Id { get; set; }
        public string InvNum { get; set; }
        private Nullable<DateTime> _InvDate;
        public Nullable<DateTime> InvDate
        {
            get
            {
                return _InvDate;
            }
            set { _InvDate = value; }
        }
        public string CustNum { get; set; }
        public string CustName { get; set; }
        public string CustPo { get; set; }
    }
}
