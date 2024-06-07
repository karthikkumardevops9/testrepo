using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SecureUserGroup
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public virtual SecureGroup SecureGroup { get; set; }
        public virtual SecureUser SecureUser { get; set; }
    }
}
