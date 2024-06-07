using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ImportJob
    {
        public int ID { get; set; }
        public string InputFile { get; set; }
        public string JobName { get; set; }
        private Nullable<short> _ReadOrder;
        public Nullable<short> ReadOrder
        {
            get
            {
                return _ReadOrder;
            }
            set { _ReadOrder = value; }
        }
        public string LoadName { get; set; }
        private Nullable<bool> _UseLoadInput;
        public Nullable<bool> UseLoadInput
        {
            get
            {
                if (_UseLoadInput == null)
                    return false;
                else
                    return _UseLoadInput;
            }
            set { _UseLoadInput = value; }
        }
        public string TempInputFile { get; set; }
    }
}
