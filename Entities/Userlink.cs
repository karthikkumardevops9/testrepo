using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Userlink
    {
        public int Id { get; set; }
        private Nullable<int> _TrackablesId;
        public Nullable<int> TrackablesId
        {
            get
            {
                if (_TrackablesId == null)
                    return 0;
                else
                    return _TrackablesId;
            }
            set { _TrackablesId = value; }
        }
        private Nullable<short> _RecordVersion;
        public Nullable<short> RecordVersion
        {
            get
            {
                return _RecordVersion;
            }
            set { _RecordVersion = value; }
        }
        public string IndexTableId { get; set; }
        public string IndexTable { get; set; }
        private Nullable<int> _AttachmentNumber;
        public Nullable<int> AttachmentNumber
        {
            get
            {
                if (_AttachmentNumber == null)
                    return 0;
                else
                    return _AttachmentNumber;
            }
            set { _AttachmentNumber = value; }
        }
        private Nullable<bool> _VersionUpdated;
        public Nullable<bool> VersionUpdated
        {
            get
            {
                if (_VersionUpdated == null)
                    return false;
                else
                    return _VersionUpdated;
            }
            set { _VersionUpdated = value; }
        }
    }
}
