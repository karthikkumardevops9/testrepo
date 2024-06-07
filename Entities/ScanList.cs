using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ScanList
    {
        public int Id { get; set; }
        private Nullable<short> _ScanOrder;
        public Nullable<short> ScanOrder
        {
            get
            {
                return _ScanOrder;
            }
            set { _ScanOrder = value; }
        }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        private Nullable<short> _FieldType;
        public Nullable<short> FieldType
        {
            get
            {
                return _FieldType;
            }
            set { _FieldType = value; }
        }
        public string IdStripChars { get; set; }
        public string IdMask { get; set; }
    }
}
