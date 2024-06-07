using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SysNextTrackable
    {
        private Nullable<int> _NextTrackablesId;
        public Nullable<int> NextTrackablesId
        {
            get
            {
                if (_NextTrackablesId == null)
                    return 0;
                else
                    return _NextTrackablesId;
            }
            set { _NextTrackablesId = value; }
        }
    }
}
