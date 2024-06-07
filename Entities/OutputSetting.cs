using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class OutputSetting
    {
        public int DefaultOutputSettingsId { get; set; }
        public string Id { get; set; }
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
        public string FileNamePrefix { get; set; }
        public string FileExtension { get; set; }
        private Nullable<int> _NextDocNum;
        public Nullable<int> NextDocNum
        {
            get
            {
                if (_NextDocNum == null)
                    return 0;
                else
                    return _NextDocNum;
            }
            set { _NextDocNum = value; }
        }
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
        private Nullable<bool> _InActive;
        public Nullable<bool> InActive
        {
            get
            {
                if (_InActive == null)
                    return false;
                else
                    return _InActive;
            }
            set { _InActive = value; }
        }
        private Nullable<int> _VolumesId;
        public Nullable<int> VolumesId
        {
            get
            {
                if (_VolumesId == null)
                    return 0;
                else
                    return _VolumesId;
            }
            set { _VolumesId = value; }
        }
    }
}
