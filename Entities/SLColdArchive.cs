using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLColdArchive
    {
        public int Id { get; set; }
        public string ArchiveName { get; set; }
        private Nullable<int> _SLCOLDSetupFormsId;
        public Nullable<int> SLCOLDSetupFormsId
        {
            get
            {
                if (_SLCOLDSetupFormsId == null)
                    return 0;
                else
                    return _SLCOLDSetupFormsId;
            }
            set { _SLCOLDSetupFormsId = value; }
        }
        private Nullable<int> _DirectoriesId;
        public Nullable<int> DirectoriesId
        {
            get
            {
                if (_DirectoriesId == null)
                    return 0;
                else
                    return _DirectoriesId;
            }
            set { _DirectoriesId = value; }
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
    }
}
