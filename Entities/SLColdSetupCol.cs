using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLColdSetupCol
    {
        public int Id { get; set; }
        public string ColName { get; set; }
        private Nullable<int> _SeqNo;
        public Nullable<int> SeqNo
        {
            get
            {
                if (_SeqNo == null)
                    return 0;
                else
                    return _SeqNo;
            }
            set { _SeqNo = value; }
        }
        public int SLCOLDSetupRowsId { get; set; }
        private Nullable<int> _LineOffset;
        public Nullable<int> LineOffset
        {
            get
            {
                if (_LineOffset == null)
                    return 0;
                else
                    return _LineOffset;
            }
            set { _LineOffset = value; }
        }
        public string Position { get; set; }
        public string Length { get; set; }
        public string DefaultValue { get; set; }
    }
}
