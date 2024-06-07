using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ImagePointer
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
        private Nullable<short> _TrackablesRecordVersion;
        public Nullable<short> TrackablesRecordVersion
        {
            get
            {
                return _TrackablesRecordVersion;
            }
            set { _TrackablesRecordVersion = value; }
        }
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
        public string FileName { get; set; }
        private Nullable<DateTime> _ScanDateTime;
        public Nullable<DateTime> ScanDateTime
        {
            get
            {
                return _ScanDateTime;
            }
            set { _ScanDateTime = value; }
        }
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
        private Nullable<int> _CRC;
        public Nullable<int> CRC
        {
            get
            {
                if (_CRC == null)
                    return 0;
                else
                    return _CRC;
            }
            set { _CRC = value; }
        }
        private Nullable<short> _Orientation;
        public Nullable<short> Orientation
        {
            get
            {
                return _Orientation;
            }
            set { _Orientation = value; }
        }
        private Nullable<double> _Skew;
        public Nullable<double> Skew
        {
            get
            {
                return _Skew;
            }
            set { _Skew = value; }
        }
        private Nullable<bool> _Front;
        public Nullable<bool> Front
        {
            get
            {
                if (_Front == null)
                    return false;
                else
                    return _Front;
            }
            set { _Front = value; }
        }
        private Nullable<int> _ImageHeight;
        public Nullable<int> ImageHeight
        {
            get
            {
                return _ImageHeight;
            }
            set { _ImageHeight = value; }
        }
        private Nullable<int> _ImageWidth;
        public Nullable<int> ImageWidth
        {
            get
            {
                return _ImageWidth;
            }
            set { _ImageWidth = value; }
        }
        private Nullable<int> _ImageSize;
        public Nullable<int> ImageSize
        {
            get
            {
                if (_ImageSize == null)
                    return 0;
                else
                    return _ImageSize;
            }
            set { _ImageSize = value; }
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
        private Nullable<int> _PageCount;
        public Nullable<int> PageCount
        {
            get
            {
                if (_PageCount == null)
                    return 0;
                else
                    return _PageCount;
            }
            set { _PageCount = value; }
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
        public string ScanMessage { get; set; }
    }
}
