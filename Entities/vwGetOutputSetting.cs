using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class vwGetOutputSetting
    {
        public int VolumesId { get; set; }
        public string OutputPath { get; set; }
        public string Name { get; set; }
        private Nullable<int> _SystemAddressesId;
        public Nullable<int> SystemAddressesId
        {
            get
            {
                if (_SystemAddressesId == null)
                    return 0;
                else
                    return _SystemAddressesId;
            }
            set { _SystemAddressesId = value; }
        }
        private Nullable<int> _DirDiskMBLimitation;
        public Nullable<int> DirDiskMBLimitation
        {
            get
            {
                if (_DirDiskMBLimitation == null)
                    return 0;
                else
                    return _DirDiskMBLimitation;
            }
            set { _DirDiskMBLimitation = value; }
        }
        private Nullable<int> _DirCountLimitation;
        public Nullable<int> DirCountLimitation
        {
            get
            {
                if (_DirCountLimitation == null)
                    return 0;
                else
                    return _DirCountLimitation;
            }
            set { _DirCountLimitation = value; }
        }
        private Nullable<bool> _Active;
        public Nullable<bool> Active
        {
            get
            {
                if (_Active == null)
                    return false;
                else
                    return _Active;
            }
            set { _Active = value; }
        }
        private Nullable<bool> _Online;
        public Nullable<bool> Online
        {
            get
            {
                if (_Online == null)
                    return false;
                else
                    return _Online;
            }
            set { _Online = value; }
        }
        public string OfflineLocation { get; set; }
        public string ImageTableName { get; set; }
        public string DeviceName { get; set; }
        public string PhysicalDriveLetter { get; set; }
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
