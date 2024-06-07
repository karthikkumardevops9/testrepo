using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLColdPointer
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
        private Nullable<int> _TrackablesRecordVersion;
        public Nullable<int> TrackablesRecordVersion
        {
            get
            {
                if (_TrackablesRecordVersion == null)
                    return 0;
                else
                    return _TrackablesRecordVersion;
            }
            set { _TrackablesRecordVersion = value; }
        }
        private Nullable<int> _ScanBatchesId;
        public Nullable<int> ScanBatchesId
        {
            get
            {
                if (_ScanBatchesId == null)
                    return 0;
                else
                    return _ScanBatchesId;
            }
            set { _ScanBatchesId = value; }
        }
        private Nullable<int> _ScanSequence;
        public Nullable<int> ScanSequence
        {
            get
            {
                if (_ScanSequence == null)
                    return 0;
                else
                    return _ScanSequence;
            }
            set { _ScanSequence = value; }
        }
        private Nullable<int> _ArchiveId;
        public Nullable<int> ArchiveId
        {
            get
            {
                if (_ArchiveId == null)
                    return 0;
                else
                    return _ArchiveId;
            }
            set { _ArchiveId = value; }
        }
        private Nullable<int> _StartingPage;
        public Nullable<int> StartingPage
        {
            get
            {
                if (_StartingPage == null)
                    return 0;
                else
                    return _StartingPage;
            }
            set { _StartingPage = value; }
        }
        private Nullable<int> _Pages;
        public Nullable<int> Pages
        {
            get
            {
                if (_Pages == null)
                    return 0;
                else
                    return _Pages;
            }
            set { _Pages = value; }
        }
        private Nullable<DateTime> _ScanDateTime;
        public Nullable<DateTime> ScanDateTime
        {
            get
            {
                return _ScanDateTime;
            }
            set { _ScanDateTime = value; }
        }
    }
}
