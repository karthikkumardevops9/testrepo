using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SecureObject
    {
        public SecureObject()
        {
            this.SecureObject1 = new List<SecureObject>();
            this.SecureObjectPermissions = new List<SecureObjectPermission>();
        }

        public int SecureObjectID { get; set; }
        public string Name { get; set; }
        public int SecureObjectTypeID { get; set; }
        public int BaseID { get; set; }
        public virtual ICollection<SecureObject> SecureObject1 { get; set; }
        public virtual SecureObject SecureObject2 { get; set; }
        public virtual ICollection<SecureObjectPermission> SecureObjectPermissions { get; set; }
    }
}
