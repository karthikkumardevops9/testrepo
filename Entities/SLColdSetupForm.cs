using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLColdSetupForm
    {
        public int Id { get; set; }
        public string LoadName { get; set; }
        public string FormName { get; set; }
        public string LineFormat { get; set; }
        public string PageMask { get; set; }
        private Nullable<int> _PageStartsOnLine;
        public Nullable<int> PageStartsOnLine
        {
            get
            {
                if (_PageStartsOnLine == null)
                    return 0;
                else
                    return _PageStartsOnLine;
            }
            set { _PageStartsOnLine = value; }
        }
        private Nullable<int> _MaxNumOfLinesPerPage;
        public Nullable<int> MaxNumOfLinesPerPage
        {
            get
            {
                if (_MaxNumOfLinesPerPage == null)
                    return 0;
                else
                    return _MaxNumOfLinesPerPage;
            }
            set { _MaxNumOfLinesPerPage = value; }
        }
        private Nullable<int> _NumOfPagesBeforeError;
        public Nullable<int> NumOfPagesBeforeError
        {
            get
            {
                if (_NumOfPagesBeforeError == null)
                    return 0;
                else
                    return _NumOfPagesBeforeError;
            }
            set { _NumOfPagesBeforeError = value; }
        }
        private Nullable<int> _FirstPageStartsOnLine;
        public Nullable<int> FirstPageStartsOnLine
        {
            get
            {
                if (_FirstPageStartsOnLine == null)
                    return 0;
                else
                    return _FirstPageStartsOnLine;
            }
            set { _FirstPageStartsOnLine = value; }
        }
        private Nullable<bool> _PageMaskFormFeed;
        public Nullable<bool> PageMaskFormFeed
        {
            get
            {
                if (_PageMaskFormFeed == null)
                    return false;
                else
                    return _PageMaskFormFeed;
            }
            set { _PageMaskFormFeed = value; }
        }
        private Nullable<int> _SkipLinesBeforeStarting;
        public Nullable<int> SkipLinesBeforeStarting
        {
            get
            {
                if (_SkipLinesBeforeStarting == null)
                    return 0;
                else
                    return _SkipLinesBeforeStarting;
            }
            set { _SkipLinesBeforeStarting = value; }
        }
        public string ImagePathPageOne { get; set; }
        public string ImagePathPageTwo { get; set; }
        private Nullable<int> _CPI;
        public Nullable<int> CPI
        {
            get
            {
                if (_CPI == null)
                    return 0;
                else
                    return _CPI;
            }
            set { _CPI = value; }
        }
        private Nullable<int> _LPI;
        public Nullable<int> LPI
        {
            get
            {
                if (_LPI == null)
                    return 0;
                else
                    return _LPI;
            }
            set { _LPI = value; }
        }
        private Nullable<int> _PageHeight;
        public Nullable<int> PageHeight
        {
            get
            {
                if (_PageHeight == null)
                    return 0;
                else
                    return _PageHeight;
            }
            set { _PageHeight = value; }
        }
        private Nullable<int> _PageWidth;
        public Nullable<int> PageWidth
        {
            get
            {
                if (_PageWidth == null)
                    return 0;
                else
                    return _PageWidth;
            }
            set { _PageWidth = value; }
        }
        private Nullable<int> _CharsWide;
        public Nullable<int> CharsWide
        {
            get
            {
                if (_CharsWide == null)
                    return 0;
                else
                    return _CharsWide;
            }
            set { _CharsWide = value; }
        }
        private Nullable<int> _LinesHigh;
        public Nullable<int> LinesHigh
        {
            get
            {
                if (_LinesHigh == null)
                    return 0;
                else
                    return _LinesHigh;
            }
            set { _LinesHigh = value; }
        }
        private Nullable<int> _LastArchiveId;
        public Nullable<int> LastArchiveId
        {
            get
            {
                if (_LastArchiveId == null)
                    return 0;
                else
                    return _LastArchiveId;
            }
            set { _LastArchiveId = value; }
        }
        public string FontName { get; set; }
        private Nullable<bool> _GreenBar;
        public Nullable<bool> GreenBar
        {
            get
            {
                if (_GreenBar == null)
                    return false;
                else
                    return _GreenBar;
            }
            set { _GreenBar = value; }
        }
        private Nullable<int> _OffsetX;
        public Nullable<int> OffsetX
        {
            get
            {
                if (_OffsetX == null)
                    return 0;
                else
                    return _OffsetX;
            }
            set { _OffsetX = value; }
        }
        private Nullable<int> _OffsetY;
        public Nullable<int> OffsetY
        {
            get
            {
                if (_OffsetY == null)
                    return 0;
                else
                    return _OffsetY;
            }
            set { _OffsetY = value; }
        }
        public string SpecialProcessing { get; set; }
        private Nullable<int> _ArchiveVolumesId;
        public Nullable<int> ArchiveVolumesId
        {
            get
            {
                if (_ArchiveVolumesId == null)
                    return 0;
                else
                    return _ArchiveVolumesId;
            }
            set { _ArchiveVolumesId = value; }
        }
        private Nullable<bool> _AppendToLastArchive;
        public Nullable<bool> AppendToLastArchive
        {
            get
            {
                if (_AppendToLastArchive == null)
                    return false;
                else
                    return _AppendToLastArchive;
            }
            set { _AppendToLastArchive = value; }
        }
        private Nullable<int> _ArchiveDirectoriesId;
        public Nullable<int> ArchiveDirectoriesId
        {
            get
            {
                if (_ArchiveDirectoriesId == null)
                    return 0;
                else
                    return _ArchiveDirectoriesId;
            }
            set { _ArchiveDirectoriesId = value; }
        }
        public string ArchivePrefix { get; set; }
        public string DebugFile { get; set; }
        public string DebugLevel { get; set; }
        private Nullable<bool> _MakeMatchReport;
        public Nullable<bool> MakeMatchReport
        {
            get
            {
                if (_MakeMatchReport == null)
                    return false;
                else
                    return _MakeMatchReport;
            }
            set { _MakeMatchReport = value; }
        }
        private Nullable<int> _NextArchiveNumber;
        public Nullable<int> NextArchiveNumber
        {
            get
            {
                if (_NextArchiveNumber == null)
                    return 0;
                else
                    return _NextArchiveNumber;
            }
            set { _NextArchiveNumber = value; }
        }
        private Nullable<bool> _NoBreaks;
        public Nullable<bool> NoBreaks
        {
            get
            {
                if (_NoBreaks == null)
                    return false;
                else
                    return _NoBreaks;
            }
            set { _NoBreaks = value; }
        }
        private Nullable<int> _NumPagesToProcess;
        public Nullable<int> NumPagesToProcess
        {
            get
            {
                if (_NumPagesToProcess == null)
                    return 0;
                else
                    return _NumPagesToProcess;
            }
            set { _NumPagesToProcess = value; }
        }
    }
}
