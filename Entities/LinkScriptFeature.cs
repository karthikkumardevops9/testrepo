using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class LinkScriptFeature
    {
        public string Id { get; set; }
        public string InstallLinkScript { get; set; }
        public string UninstallLinkScript { get; set; }
        private Nullable<bool> _Installed;
        public Nullable<bool> Installed
        {
            get
            {
                if (_Installed == null)
                    return false;
                else
                    return _Installed;
            }
            set { _Installed = value; }
        }
        public string Description { get; set; }
        public string InstallInfo { get; set; }
    }
}
