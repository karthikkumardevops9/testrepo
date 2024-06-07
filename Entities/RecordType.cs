using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class RecordType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        private Nullable<bool> _TrackHistory;
        public Nullable<bool> TrackHistory
        {
            get
            {
                if (_TrackHistory == null)
                    return false;
                else
                    return _TrackHistory;
            }
            set { _TrackHistory = value; }
        }
    }
}
