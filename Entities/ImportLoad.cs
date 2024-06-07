using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ImportLoad
    {
        public int ID { get; set; }
        public string IdFieldName { get; set; }
        public string InputFile { get; set; }
        private Nullable<int> _MaxDupCount;
        public Nullable<int> MaxDupCount
        {
            get
            {
                if (_MaxDupCount == null)
                    return 0;
                else
                    return _MaxDupCount;
            }
            set { _MaxDupCount = value; }
        }
        public string TableSheetName { get; set; }
        public string TrackDestinationId { get; set; }
        public string LoadName { get; set; }
        public string RecordType { get; set; }
        private Nullable<short> _RecordLength;
        public Nullable<short> RecordLength
        {
            get
            {
                return _RecordLength;
            }
            set { _RecordLength = value; }
        }
        public string Delimiter { get; set; }
        public string Duplicate { get; set; }
        private Nullable<bool> _UpdateParent;
        public Nullable<bool> UpdateParent
        {
            get
            {
                if (_UpdateParent == null)
                    return false;
                else
                    return _UpdateParent;
            }
            set { _UpdateParent = value; }
        }
        public string SQLQuery { get; set; }
        private Nullable<bool> _ReverseOrder;
        public Nullable<bool> ReverseOrder
        {
            get
            {
                if (_ReverseOrder == null)
                    return false;
                else
                    return _ReverseOrder;
            }
            set { _ReverseOrder = value; }
        }
        private Nullable<bool> _SaveImageAsNewPage;
        public Nullable<bool> SaveImageAsNewPage
        {
            get
            {
                if (_SaveImageAsNewPage == null)
                    return false;
                else
                    return _SaveImageAsNewPage;
            }
            set { _SaveImageAsNewPage = value; }
        }
        private Nullable<bool> _DifferentImagePath;
        public Nullable<bool> DifferentImagePath
        {
            get
            {
                if (_DifferentImagePath == null)
                    return false;
                else
                    return _DifferentImagePath;
            }
            set { _DifferentImagePath = value; }
        }
        public string ScanRule { get; set; }
        public string ReplaceThisPath { get; set; }
        public string ReplaceWithPath { get; set; }
        private Nullable<bool> _DeleteSourceFile;
        public Nullable<bool> DeleteSourceFile
        {
            get
            {
                if (_DeleteSourceFile == null)
                    return false;
                else
                    return _DeleteSourceFile;
            }
            set { _DeleteSourceFile = value; }
        }
        private Nullable<bool> _DeleteSourceImage;
        public Nullable<bool> DeleteSourceImage
        {
            get
            {
                if (_DeleteSourceImage == null)
                    return false;
                else
                    return _DeleteSourceImage;
            }
            set { _DeleteSourceImage = value; }
        }
        private Nullable<int> _DirectFromHandheld;
        public Nullable<int> DirectFromHandheld
        {
            get
            {
                if (_DirectFromHandheld == null)
                    return 0;
                else
                    return _DirectFromHandheld;
            }
            set { _DirectFromHandheld = value; }
        }
        private Nullable<int> _OneStripJobsId;
        public Nullable<int> OneStripJobsId
        {
            get
            {
                if (_OneStripJobsId == null)
                    return 0;
                else
                    return _OneStripJobsId;
            }
            set { _OneStripJobsId = value; }
        }
        private Nullable<int> _ImportPrintType;
        public Nullable<int> ImportPrintType
        {
            get
            {
                if (_ImportPrintType == null)
                    return 0;
                else
                    return _ImportPrintType;
            }
            set { _ImportPrintType = value; }
        }
        public string DatabaseName { get; set; }
        private Nullable<DateTime> _DateDue;
        public Nullable<DateTime> DateDue
        {
            get
            {
                return _DateDue;
            }
            set { _DateDue = value; }
        }
        private Nullable<bool> _DoReconciliation;
        public Nullable<bool> DoReconciliation
        {
            get
            {
                if (_DoReconciliation == null)
                    return false;
                else
                    return _DoReconciliation;
            }
            set { _DoReconciliation = value; }
        }
        public string FileName { get; set; }
        private Nullable<bool> _FirstRowHeader;
        public Nullable<bool> FirstRowHeader
        {
            get
            {
                if (_FirstRowHeader == null)
                    return false;
                else
                    return _FirstRowHeader;
            }
            set { _FirstRowHeader = value; }
        }
        private Nullable<bool> _SaveImageAsNewVersion;
        public Nullable<bool> SaveImageAsNewVersion
        {
            get
            {
                if (_SaveImageAsNewVersion == null)
                    return false;
                else
                    return _SaveImageAsNewVersion;
            }
            set { _SaveImageAsNewVersion = value; }
        }
        private Nullable<int> _FromHandHeldEnum;
        public Nullable<int> FromHandHeldEnum
        {
            get
            {
                if (_FromHandHeldEnum == null)
                    return 0;
                else
                    return _FromHandHeldEnum;
            }
            set { _FromHandHeldEnum = value; }
        }
        private Nullable<bool> _SaveImageAsNewVersionAsOfficialRecord;
        public Nullable<bool> SaveImageAsNewVersionAsOfficialRecord
        {
            get
            {
                if (_SaveImageAsNewVersionAsOfficialRecord == null)
                    return false;
                else
                    return _SaveImageAsNewVersionAsOfficialRecord;
            }
            set { _SaveImageAsNewVersionAsOfficialRecord = value; }
        }
        public string TempInputFile { get; set; }
    }
}
