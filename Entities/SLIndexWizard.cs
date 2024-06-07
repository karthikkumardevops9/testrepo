using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLIndexWizard
    {
        public string Id { get; set; }
        public string FolderTableName { get; set; }
        public string FolderPrompt { get; set; }
        public string FolderSearchField { get; set; }
        private Nullable<bool> _AutoCreateFolder;
        public Nullable<bool> AutoCreateFolder
        {
            get
            {
                if (_AutoCreateFolder == null)
                    return false;
                else
                    return _AutoCreateFolder;
            }
            set { _AutoCreateFolder = value; }
        }
        private Nullable<bool> _FolderLevelIndexing;
        public Nullable<bool> FolderLevelIndexing
        {
            get
            {
                if (_FolderLevelIndexing == null)
                    return false;
                else
                    return _FolderLevelIndexing;
            }
            set { _FolderLevelIndexing = value; }
        }
        public string DocumemtTableName { get; set; }
        private Nullable<bool> _IncludeDocTypes;
        public Nullable<bool> IncludeDocTypes
        {
            get
            {
                if (_IncludeDocTypes == null)
                    return false;
                else
                    return _IncludeDocTypes;
            }
            set { _IncludeDocTypes = value; }
        }
        public string DocTypeTableName { get; set; }
        public string DocTypePrompt { get; set; }
        public string DocTypeSearchField { get; set; }
        private Nullable<int> _DocumentAutoCreate;
        public Nullable<int> DocumentAutoCreate
        {
            get
            {
                if (_DocumentAutoCreate == null)
                    return 0;
                else
                    return _DocumentAutoCreate;
            }
            set { _DocumentAutoCreate = value; }
        }
        private Nullable<int> _IndexingType;
        public Nullable<int> IndexingType
        {
            get
            {
                if (_IndexingType == null)
                    return 0;
                else
                    return _IndexingType;
            }
            set { _IndexingType = value; }
        }
        private Nullable<bool> _UseHeaderSheets;
        public Nullable<bool> UseHeaderSheets
        {
            get
            {
                if (_UseHeaderSheets == null)
                    return false;
                else
                    return _UseHeaderSheets;
            }
            set { _UseHeaderSheets = value; }
        }
        public string HeaderLabelName { get; set; }
        private Nullable<bool> _DiscardHeaderSheet;
        public Nullable<bool> DiscardHeaderSheet
        {
            get
            {
                if (_DiscardHeaderSheet == null)
                    return false;
                else
                    return _DiscardHeaderSheet;
            }
            set { _DiscardHeaderSheet = value; }
        }
        private Nullable<bool> _UseSeperatorSheets;
        public Nullable<bool> UseSeperatorSheets
        {
            get
            {
                if (_UseSeperatorSheets == null)
                    return false;
                else
                    return _UseSeperatorSheets;
            }
            set { _UseSeperatorSheets = value; }
        }
        public string SeperatorLabelName { get; set; }
        private Nullable<bool> _DiscardSeperatorSheet;
        public Nullable<bool> DiscardSeperatorSheet
        {
            get
            {
                if (_DiscardSeperatorSheet == null)
                    return false;
                else
                    return _DiscardSeperatorSheet;
            }
            set { _DiscardSeperatorSheet = value; }
        }
        public string OutputSettingsId { get; set; }
        public string LinkScriptHeaderId { get; set; }
        private Nullable<bool> _VerifyManualEntry;
        public Nullable<bool> VerifyManualEntry
        {
            get
            {
                if (_VerifyManualEntry == null)
                    return false;
                else
                    return _VerifyManualEntry;
            }
            set { _VerifyManualEntry = value; }
        }
        public string AddFolderHelpComment { get; set; }
        public string AddDocumentHelpComment { get; set; }
        private Nullable<bool> _AlwaysCreateFolder;
        public Nullable<bool> AlwaysCreateFolder
        {
            get
            {
                if (_AlwaysCreateFolder == null)
                    return false;
                else
                    return _AlwaysCreateFolder;
            }
            set { _AlwaysCreateFolder = value; }
        }
        private Nullable<bool> _LeaveDocTypesGlobal;
        public Nullable<bool> LeaveDocTypesGlobal
        {
            get
            {
                if (_LeaveDocTypesGlobal == null)
                    return false;
                else
                    return _LeaveDocTypesGlobal;
            }
            set { _LeaveDocTypesGlobal = value; }
        }
    }
}
