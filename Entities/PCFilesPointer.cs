using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class PCFilesPointer
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
        public string FileName { get; set; }
        private Nullable<int> _ScanDirectoriesId;
        public Nullable<int> ScanDirectoriesId
        {
            get
            {
                if (_ScanDirectoriesId == null)
                    return 0;
                else
                    return _ScanDirectoriesId;
            }
            set { _ScanDirectoriesId = value; }
        }
        private Nullable<int> _OrgDirectoriesId;
        public Nullable<int> OrgDirectoriesId
        {
            get
            {
                if (_OrgDirectoriesId == null)
                    return 0;
                else
                    return _OrgDirectoriesId;
            }
            set { _OrgDirectoriesId = value; }
        }
        public string OrgFileName { get; set; }
        private Nullable<int> _PCFilesEditGrp;
        public Nullable<int> PCFilesEditGrp
        {
            get
            {
                if (_PCFilesEditGrp == null)
                    return 0;
                else
                    return _PCFilesEditGrp;
            }
            set { _PCFilesEditGrp = value; }
        }
        private Nullable<int> _PCFilesNVerGrp;
        public Nullable<int> PCFilesNVerGrp
        {
            get
            {
                if (_PCFilesNVerGrp == null)
                    return 0;
                else
                    return _PCFilesNVerGrp;
            }
            set { _PCFilesNVerGrp = value; }
        }
        public string OrgFullPath { get; set; }
        private Nullable<bool> _AddedToFTS;
        public Nullable<bool> AddedToFTS
        {
            get
            {
                if (_AddedToFTS == null)
                    return false;
                else
                    return _AddedToFTS;
            }
            set { _AddedToFTS = value; }
        }
        private Nullable<bool> _AddedToOCR;
        public Nullable<bool> AddedToOCR
        {
            get
            {
                if (_AddedToOCR == null)
                    return false;
                else
                    return _AddedToOCR;
            }
            set { _AddedToOCR = value; }
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
        private Nullable<DateTime> _ScanDateTime;
        public Nullable<DateTime> ScanDateTime
        {
            get
            {
                return _ScanDateTime;
            }
            set { _ScanDateTime = value; }
        }
        private Nullable<short> _BarCodeCount;
        public Nullable<short> BarCodeCount
        {
            get
            {
                return _BarCodeCount;
            }
            set { _BarCodeCount = value; }
        }
        public string BarCodes { get; set; }
        public string ScanMessage { get; set; }
        private Nullable<int> _PageNumber;
        public Nullable<int> PageNumber
        {
            get
            {
                if (_PageNumber == null)
                    return 0;
                else
                    return _PageNumber;
            }
            set { _PageNumber = value; }
        }
    }
}
