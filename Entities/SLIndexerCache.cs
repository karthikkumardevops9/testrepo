using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLIndexerCache
    {
        public int Id { get; set; }
        public int IndexType { get; set; }
        public string IndexTableName { get; set; }
        public string IndexFieldName { get; set; }
        public string IndexTableId { get; set; }
        public string IndexData { get; set; }
        private Nullable<int> _OrphanType;
        public Nullable<int> OrphanType
        {
            get
            {
                if (_OrphanType == null)
                    return 0;
                else
                    return _OrphanType;
            }
            set { _OrphanType = value; }
        }
        private Nullable<short> _RecordVersion;
        public Nullable<short> RecordVersion
        {
            get
            {
                return _RecordVersion;
            }
            set { _RecordVersion = value; }
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
        private Nullable<int> _AttachmentNumber;
        public Nullable<int> AttachmentNumber
        {
            get
            {
                if (_AttachmentNumber == null)
                    return 0;
                else
                    return _AttachmentNumber;
            }
            set { _AttachmentNumber = value; }
        }
    }
}
