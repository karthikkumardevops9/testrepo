using System;
using System.Data;
using System.Diagnostics;

namespace MSRecordsEngine.Imaging
{
    [CLSCompliant(true)]
    [Serializable()]
    public partial class VersionInfo
    {
        internal VersionInfo() : base()
        {
        }

        public VersionInfo(DataRow row) : this()
        {

            try
            {
                TrackablesId = (int)row["TrackablesId"];
                Version = (int)row["RecordVersion"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (!string.IsNullOrEmpty(row["Orphan"].ToString()))
            {
                Orphan = (bool)row["Orphan"];
            }
            if (!string.IsNullOrEmpty(row["OfficialRecord"].ToString()))
            {
                OfficialRecord = (bool)row["OfficialRecord"];
            }
        }

        public int TrackablesId
        {
            get
            {
                return _trackablesId;
            }
            set
            {
                _trackablesId = value;
            }
        }
        private int _trackablesId;

        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }
        private int _version;

        public bool Orphan
        {
            get
            {
                return _orphan;
            }
            set
            {
                _orphan = value;
            }
        }
        private bool _orphan;

        public bool OfficialRecord
        {
            get
            {
                return _officialRecord;
            }
            set
            {
                _officialRecord = value;
            }
        }
        private bool _officialRecord;
    }
}
