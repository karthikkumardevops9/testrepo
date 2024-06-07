using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLServiceTask
    {
        public int Id { get; set; }
        public string Type { get; set; }
        private Nullable<int> _Interval;
        public Nullable<int> Interval
        {
            get
            {
                if (_Interval == null)
                    return 0;
                else
                    return _Interval;
            }
            set { _Interval = value; }
        }
        public string Parameters { get; set; }
        private Nullable<int> _TaskType;
        public Nullable<int> TaskType
        {
            get
            {
                if (_TaskType == null)
                    return 0;
                else
                    return _TaskType;
            }
            set { _TaskType = value; }
        }
        public string EMailAddress { get; set; }
        public string CustomProcess { get; set; }
        private Nullable<int> _NotificationType;
        public Nullable<int> NotificationType
        {
            get
            {
                if (_NotificationType == null)
                    return 0;
                else
                    return _NotificationType;
            }
            set { _NotificationType = value; }
        }
        public string UserName { get; set; }
        private Nullable<int> _UserId;
        public Nullable<int> UserId
        {
            get
            {
                if (_UserId == null)
                    return 0;
                else
                    return _UserId;
            }
            set { _UserId = value; }
        }
        private Nullable<int> _ViewId;
        public Nullable<int> ViewId
        {
            get
            {
                if (_ViewId == null)
                    return 0;
                else
                    return _ViewId;
            }
            set { _ViewId = value; }
        }
        public string Status { get; set; }
        private Nullable<DateTime> _StartDate;
        public Nullable<DateTime> StartDate
        {
            get
            {
                return _StartDate;
            }
            set { _StartDate = value; }
        }
        private Nullable<DateTime> _EndDate;
        public Nullable<DateTime> EndDate
        {
            get
            {
                return _EndDate;
            }
            set { _EndDate = value; }
        }
        private Nullable<int> _RecordCount;
        public Nullable<int> RecordCount
        {
            get
            {
                if (_RecordCount == null)
                    return 0;
                else
                    return _RecordCount;
            }
            set { _RecordCount = value; }
        }
        public string DestinationTableName { get; set; }
        public string DestinationTableId { get; set; }
        private Nullable<DateTime> _DueBackDate;
        public Nullable<DateTime> DueBackDate
        {
            get
            {
                return _DueBackDate;
            }
            set { _DueBackDate = value; }
        }
        private Nullable<bool> _Reconciliation;
        public Nullable<bool> Reconciliation
        {
            get
            {
                return _Reconciliation;
            }
            set { _Reconciliation = value; }
        }        
        public string ReportLocation { get; set; }
        public string DownloadLocation { get; set; }
        private Nullable<bool> _IsNotification;
        public Nullable<bool> IsNotification
        {
            get
            {
                if (_IsNotification == null)
                    return false;
                else
                    return _IsNotification;
            }
            set { _IsNotification = value; }
        }
        private Nullable<DateTime> _CreateDate;
        public Nullable<DateTime> CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set { _CreateDate = value; }
        }
    }
}
