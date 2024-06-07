using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ImportField
    {
        public int ID { get; set; }
        private Nullable<int> _SwingYear;
        public Nullable<int> SwingYear
        {
            get
            {
                if (_SwingYear == null)
                    return 0;
                else
                    return _SwingYear;
            }
            set { _SwingYear = value; }
        }
        public string ImportLoad { get; set; }
        private Nullable<short> _ReadOrder;
        public Nullable<short> ReadOrder
        {
            get
            {
                return _ReadOrder;
            }
            set { _ReadOrder = value; }
        }
        public string FieldName { get; set; }
        private Nullable<short> _StartPosition;
        public Nullable<short> StartPosition
        {
            get
            {
                return _StartPosition;
            }
            set { _StartPosition = value; }
        }
        private Nullable<short> _EndPosition;
        public Nullable<short> EndPosition
        {
            get
            {
                return _EndPosition;
            }
            set { _EndPosition = value; }
        }
        public string DefaultValue { get; set; }
        public string DateFormat { get; set; }
    }
}
