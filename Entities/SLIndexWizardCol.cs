using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLIndexWizardCol
    {
        public int Id { get; set; }
        private Nullable<int> _ColumnNum;
        public Nullable<int> ColumnNum
        {
            get
            {
                if (_ColumnNum == null)
                    return 0;
                else
                    return _ColumnNum;
            }
            set { _ColumnNum = value; }
        }
        public string SLIndexWizardId { get; set; }
        public string ColumnType { get; set; }
        public string Prompt { get; set; }
        public string Field { get; set; }
        private Nullable<bool> _Required;
        public Nullable<bool> Required
        {
            get
            {
                if (_Required == null)
                    return false;
                else
                    return _Required;
            }
            set { _Required = value; }
        }
        private Nullable<bool> _Visible;
        public Nullable<bool> Visible
        {
            get
            {
                if (_Visible == null)
                    return false;
                else
                    return _Visible;
            }
            set { _Visible = value; }
        }
        public string FixedData { get; set; }
    }
}
