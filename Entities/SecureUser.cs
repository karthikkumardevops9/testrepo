using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SecureUser
    {
        public SecureUser()
        {
            this.SecureUserGroups = new List<SecureUserGroup>();
        }

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool AccountDisabled { get; set; }
        public string AccountType { get; set; }
        private Nullable<DateTime> _PasswordUpdate;
        public Nullable<DateTime> PasswordUpdate
        {
            get
            {
                return _PasswordUpdate;
            }
            set { _PasswordUpdate = value; }
        }
        public string C_oldPassword { get; set; }
        private Nullable<bool> _MustChangePassword;
        public Nullable<bool> MustChangePassword
        {
            get
            {
                if (_MustChangePassword == null)
                    return false;
                else
                    return _MustChangePassword;
            }
            set { _MustChangePassword = value; }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Misc1 { get; set; }
        public string Misc2 { get; set; }
        public string DisplayName { get; set; }
        public virtual ICollection<SecureUserGroup> SecureUserGroups { get; set; }
    }
}
