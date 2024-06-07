using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLCollectionItem
    {
        public int Id { get; set; }
        private Nullable<int> _CollectionId;
        public Nullable<int> CollectionId
        {
            get
            {
                if (_CollectionId == null)
                    return 0;
                else
                    return _CollectionId;
            }
            set { _CollectionId = value; }
        }
        private Nullable<int> _Index;
        public Nullable<int> Index
        {
            get
            {
                if (_Index == null)
                    return 0;
                else
                    return _Index;
            }
            set { _Index = value; }
        }
        private Nullable<int> _PointersId;
        public Nullable<int> PointersId
        {
            get
            {
                if (_PointersId == null)
                    return 0;
                else
                    return _PointersId;
            }
            set { _PointersId = value; }
        }
        public string Table { get; set; }
        public string TableId { get; set; }
        private Nullable<int> _AttachmentType;
        public Nullable<int> AttachmentType
        {
            get
            {
                if (_AttachmentType == null)
                    return 0;
                else
                    return _AttachmentType;
            }
            set { _AttachmentType = value; }
        }
        public string DisplayText { get; set; }
        public string TableUserName { get; set; }
        public string Ticket { get; set; }
        public string Extension { get; set; }
    }
}
