using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class PackingSlip
    {
        public int Id { get; set; }
        public string POId { get; set; }
        private Nullable<DateTime> _PSDate;
        public Nullable<DateTime> PSDate
        {
            get
            {
                return _PSDate;
            }
            set { _PSDate = value; }
        }
    }
}
