using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Document
    {
        public int Id { get; set; }
        private Nullable<int> _FolderId;
        public Nullable<int> FolderId
        {
            get
            {
                if (_FolderId == null)
                    return 0;
                else
                    return _FolderId;
            }
            set { _FolderId = value; }
        }
        public string DocumentTypeId { get; set; }
        public string DocumentDescription { get; set; }
        public bool Received { get; set; }
        public string ExceptionNote { get; set; }
        private Nullable<DateTime> _DocumentDate;
        public Nullable<DateTime> DocumentDate
        {
            get
            {
                return _DocumentDate;
            }
            set { _DocumentDate = value; }
        }
    }
}
