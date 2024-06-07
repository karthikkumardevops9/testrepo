using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SecureObjectPermission
    {
        public int SecureObjectPermissionID { get; set; }
        public int GroupID { get; set; }
        public int SecureObjectID { get; set; }
        public int PermissionID { get; set; }
        public virtual SecureGroup SecureGroup { get; set; }
        public virtual SecureObject SecureObject { get; set; }
        public virtual SecurePermission SecurePermission { get; set; }
    }
}
