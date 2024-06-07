using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class LinkScriptHeader
    {
        public string Id { get; set; }
        private Nullable<int> _FormPageAction;
        public Nullable<int> FormPageAction
        {
            get
            {
                if (_FormPageAction == null)
                    return 0;
                else
                    return _FormPageAction;
            }
            set { _FormPageAction = value; }
        }
        public string DirectoryName { get; set; }
        public string OutputSettingsId { get; set; }
        private Nullable<int> _CallingType;
        public Nullable<int> CallingType
        {
            get
            {
                if (_CallingType == null)
                    return 0;
                else
                    return _CallingType;
            }
            set { _CallingType = value; }
        }
        private Nullable<bool> _PCFilesDeleteAfterCopy;
        public Nullable<bool> PCFilesDeleteAfterCopy
        {
            get
            {
                if (_PCFilesDeleteAfterCopy == null)
                    return false;
                else
                    return _PCFilesDeleteAfterCopy;
            }
            set { _PCFilesDeleteAfterCopy = value; }
        }
        private Nullable<int> _ViewGroup;
        public Nullable<int> ViewGroup
        {
            get
            {
                if (_ViewGroup == null)
                    return 0;
                else
                    return _ViewGroup;
            }
            set { _ViewGroup = value; }
        }
        private Nullable<int> _UIType;
        public Nullable<int> UIType
        {
            get
            {
                if (_UIType == null)
                    return 0;
                else
                    return _UIType;
            }
            set { _UIType = value; }
        }
    }
}
