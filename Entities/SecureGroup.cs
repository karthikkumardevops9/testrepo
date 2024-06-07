using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SecureGroup
    {
        public SecureGroup()
        {
            this.SecureObjectPermissions = new List<SecureObjectPermission>();
            this.SecureUserGroups = new List<SecureUserGroup>();
        }

        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public string Description { get; set; }
        public string ActiveDirectoryGroup { get; set; }
        public string ActiveDirectoryPath { get; set; }
        public int AutoLogOffSeconds { get; set; }
        public int AutoLockSeconds { get; set; }
        private Nullable<int> _C_oldGroupID;
        public Nullable<int> C_oldGroupID
        {
            get
            {
                if (_C_oldGroupID == null)
                    return 0;
                else
                    return _C_oldGroupID;
            }
            set { _C_oldGroupID = value; }
        }
        public virtual ICollection<SecureObjectPermission> SecureObjectPermissions { get; set; }
        public virtual ICollection<SecureUserGroup> SecureUserGroups { get; set; }
    }
}
