using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLRetentionCode
    {
        public int SLRetentionCodesId { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string DeptOfRecord { get; set; }
        public string Notes { get; set; }
        private Nullable<bool> _RetentionLegalHold;
        public Nullable<bool> RetentionLegalHold
        {
            get
            {
                if (_RetentionLegalHold == null)
                    return false;
                else
                    return _RetentionLegalHold;
            }
            set { _RetentionLegalHold = value; }
        }
        private Nullable<double> _RetentionPeriodLegal;
        public Nullable<double> RetentionPeriodLegal
        {
            get
            {
                return _RetentionPeriodLegal;
            }
            set { _RetentionPeriodLegal = value; }
        }
        private Nullable<double> _RetentionPeriodUser;
        public Nullable<double> RetentionPeriodUser
        {
            get
            {
                return _RetentionPeriodUser;
            }
            set { _RetentionPeriodUser = value; }
        }
        private Nullable<double> _RetentionPeriodOther;
        public Nullable<double> RetentionPeriodOther
        {
            get
            {
                return _RetentionPeriodOther;
            }
            set { _RetentionPeriodOther = value; }
        }
        private Nullable<double> _RetentionPeriodTotal;
        public Nullable<double> RetentionPeriodTotal
        {
            get
            {
                return _RetentionPeriodTotal;
            }
            set { _RetentionPeriodTotal = value; }
        }
        private Nullable<bool> _RetentionPeriodForceToEndOfYear;
        public Nullable<bool> RetentionPeriodForceToEndOfYear
        {
            get
            {
                if (_RetentionPeriodForceToEndOfYear == null)
                    return false;
                else
                    return _RetentionPeriodForceToEndOfYear;
            }
            set { _RetentionPeriodForceToEndOfYear = value; }
        }
        public string RetentionEventType { get; set; }
        public string InactivityEventType { get; set; }
        private Nullable<double> _InactivityPeriod;
        public Nullable<double> InactivityPeriod
        {
            get
            {
                return _InactivityPeriod;
            }
            set { _InactivityPeriod = value; }
        }
        private Nullable<bool> _InactivityForceToEndOfYear;
        public Nullable<bool> InactivityForceToEndOfYear
        {
            get
            {
                if (_InactivityForceToEndOfYear == null)
                    return false;
                else
                    return _InactivityForceToEndOfYear;
            }
            set { _InactivityForceToEndOfYear = value; }
        }
    }
}
