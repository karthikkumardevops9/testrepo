using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLSignature
    {
        public int Id { get; set; }
        public string Table { get; set; }
        private Nullable<int> _TableId;
        public Nullable<int> TableId
        {
            get
            {
                if (_TableId == null)
                    return 0;
                else
                    return _TableId;
            }
            set { _TableId = value; }
        }
        public byte[] Signature { get; set; }
    }
}
