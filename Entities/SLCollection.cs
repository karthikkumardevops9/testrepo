using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Operator { get; set; }
        private Nullable<DateTime> _CreateDate;
        public Nullable<DateTime> CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set { _CreateDate = value; }
        }
        private Nullable<int> _SecurityGroupId;
        public Nullable<int> SecurityGroupId
        {
            get
            {
                if (_SecurityGroupId == null)
                    return 0;
                else
                    return _SecurityGroupId;
            }
            set { _SecurityGroupId = value; }
        }
    }
}
