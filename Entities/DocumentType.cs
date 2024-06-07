using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class DocumentType
    {
        public string DocumentTypeId { get; set; }
        public string FolderTypeId { get; set; }
        private Nullable<short> _DateIncrement;
        public Nullable<short> DateIncrement
        {
            get
            {
                return _DateIncrement;
            }
            set { _DateIncrement = value; }
        }
        public string Description { get; set; }
    }
}
