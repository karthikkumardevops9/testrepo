using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SystemAddress
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string PhysicalDriveLetter { get; set; }
        private Nullable<bool> _RequireTemporary;
        public Nullable<bool> RequireTemporary
        {
            get
            {
                if (_RequireTemporary == null)
                    return false;
                else
                    return _RequireTemporary;
            }
            set { _RequireTemporary = value; }
        }
        private Nullable<bool> _ActiveStorage;
        public Nullable<bool> ActiveStorage
        {
            get
            {
                if (_ActiveStorage == null)
                    return false;
                else
                    return _ActiveStorage;
            }
            set { _ActiveStorage = value; }
        }
        private Nullable<int> _BlockingSize;
        public Nullable<int> BlockingSize
        {
            get
            {
                if (_BlockingSize == null)
                    return 0;
                else
                    return _BlockingSize;
            }
            set { _BlockingSize = value; }
        }
        private Nullable<bool> _DirectorySizeLimits;
        public Nullable<bool> DirectorySizeLimits
        {
            get
            {
                if (_DirectorySizeLimits == null)
                    return false;
                else
                    return _DirectorySizeLimits;
            }
            set { _DirectorySizeLimits = value; }
        }
        private Nullable<int> _DirectoryMaxEntries;
        public Nullable<int> DirectoryMaxEntries
        {
            get
            {
                if (_DirectoryMaxEntries == null)
                    return 0;
                else
                    return _DirectoryMaxEntries;
            }
            set { _DirectoryMaxEntries = value; }
        }
    }
}
