using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class DBVersion
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string ProductCode { get; set; }
        private Nullable<double> _ProductVersion;
        public Nullable<double> ProductVersion
        {
            get
            {
                return _ProductVersion;
            }
            set { _ProductVersion = value; }
        }
    }
}
