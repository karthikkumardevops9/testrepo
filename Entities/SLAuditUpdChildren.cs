using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLAuditUpdChildren
    {
        public int Id { get; set; }
        private Nullable<int> _SLAuditUpdatesId;
        public Nullable<int> SLAuditUpdatesId
        {
            get
            {
                if (_SLAuditUpdatesId == null)
                    return 0;
                else
                    return _SLAuditUpdatesId;
            }
            set { _SLAuditUpdatesId = value; }
        }
        public string TableName { get; set; }
        public string TableId { get; set; }
    }
}
