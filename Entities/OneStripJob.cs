using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class OneStripJob
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        private Nullable<short> _Inprint;
        public Nullable<short> Inprint
        {
            get
            {
                return _Inprint;
            }
            set { _Inprint = value; }
        }
        private Nullable<int> _OneStripFormsId;
        public Nullable<int> OneStripFormsId
        {
            get
            {
                if (_OneStripFormsId == null)
                    return 0;
                else
                    return _OneStripFormsId;
            }
            set { _OneStripFormsId = value; }
        }
        private Nullable<short> _UserUnits;
        public Nullable<short> UserUnits
        {
            get
            {
                return _UserUnits;
            }
            set { _UserUnits = value; }
        }
        private Nullable<double> _LabelWidth;
        public Nullable<double> LabelWidth
        {
            get
            {
                return _LabelWidth;
            }
            set { _LabelWidth = value; }
        }
        private Nullable<double> _LabelHeight;
        public Nullable<double> LabelHeight
        {
            get
            {
                return _LabelHeight;
            }
            set { _LabelHeight = value; }
        }
        private Nullable<bool> _DrawLabels;
        public Nullable<bool> DrawLabels
        {
            get
            {
                if (_DrawLabels == null)
                    return false;
                else
                    return _DrawLabels;
            }
            set { _DrawLabels = value; }
        }
        private Nullable<int> _LastCounter;
        public Nullable<int> LastCounter
        {
            get
            {
                if (_LastCounter == null)
                    return 0;
                else
                    return _LastCounter;
            }
            set { _LastCounter = value; }
        }
        public string SQLString { get; set; }
        public string SQLUpdateString { get; set; }
        public string LSAfterPrinting { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseTableName { get; set; }
        public string DZNName { get; set; }
        private Nullable<int> _Sampling;
        public Nullable<int> Sampling
        {
            get
            {
                if (_Sampling == null)
                    return 0;
                else
                    return _Sampling;
            }
            set { _Sampling = value; }
        }
        public string FLDFieldNames { get; set; }

    }
}
