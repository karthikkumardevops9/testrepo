using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class OfficeDocType
    {
        public int Id { get; set; }
        private Nullable<bool> _DefaultFormat;
        public Nullable<bool> DefaultFormat
        {
            get
            {
                if (_DefaultFormat == null)
                    return false;
                else
                    return _DefaultFormat;
            }
            set { _DefaultFormat = value; }
        }
        private Nullable<int> _SaveFormat;
        public Nullable<int> SaveFormat
        {
            get
            {
                if (_SaveFormat == null)
                    return 0;
                else
                    return _SaveFormat;
            }
            set { _SaveFormat = value; }
        }
        public string AppType { get; set; }
        public string DefaultExtension { get; set; }
        public string Description { get; set; }
        private Nullable<int> _EditorType;
        public Nullable<int> EditorType
        {
            get
            {
                if (_EditorType == null)
                    return 0;
                else
                    return _EditorType;
            }
            set { _EditorType = value; }
        }
        private Nullable<bool> _HardDelete;
        public Nullable<bool> HardDelete
        {
            get
            {
                if (_HardDelete == null)
                    return false;
                else
                    return _HardDelete;
            }
            set { _HardDelete = value; }
        }
    }
}
