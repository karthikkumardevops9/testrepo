using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SecurePermissionDescription
    {
        public int SecureObjectID { get; set; }
        public int PermissionID { get; set; }
        public string Name { get; set; }
        public string Permission { get; set; }
        public string Description { get; set; }
    }
}
