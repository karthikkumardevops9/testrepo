using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Directory
    {
        public int Id { get; set; }
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
        public string Description { get; set; }
        public string Path { get; set; }
        private Nullable<bool> _DirFullFlag;
        public Nullable<bool> DirFullFlag
        {
            get
            {
                if (_DirFullFlag == null)
                    return false;
                else
                    return _DirFullFlag;
            }
            set { _DirFullFlag = value; }
        }
    }
}
