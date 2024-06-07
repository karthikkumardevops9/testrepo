using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Volume
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PathName { get; set; }
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
        public string OSN_Volume { get; set; }
        public string DeviceTypeId { get; set; }
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
    }
}
