using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SecurePermission
    {
        public SecurePermission()
        {
            this.SecureObjectPermissions = new List<SecureObjectPermission>();
        }

        public int PermissionID { get; set; }
        public string Permission { get; set; }
        public string Description { get; set; }
        private Nullable<int> _Ordinal;
        public Nullable<int> Ordinal
        {
            get
            {
                if (_Ordinal == null)
                    return 0;
                else
                    return _Ordinal;
            }
            set { _Ordinal = value; }
        }
        public string Children { get; set; }
        private Nullable<int> _Indent;
        public Nullable<int> Indent
        {
            get
            {
                if (_Indent == null)
                    return 0;
                else
                    return _Indent;
            }
            set { _Indent = value; }
        }
        public virtual ICollection<SecureObjectPermission> SecureObjectPermissions { get; set; }
    }
}
