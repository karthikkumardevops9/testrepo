using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLRequestor
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
        public string EmployeeId { get; set; }
        private Nullable<DateTime> _DateRequested;
        public Nullable<DateTime> DateRequested
        {
            get
            {
                return _DateRequested;
            }
            set { _DateRequested = value; }
        }
        public string Priority { get; set; }
        private Nullable<DateTime> _DateReceived;
        public Nullable<DateTime> DateReceived
        {
            get
            {
                return _DateReceived;
            }
            set { _DateReceived = value; }
        }
        public string Status { get; set; }
        public string ExceptionComments { get; set; }
        private Nullable<int> _SLPullListsId;
        public Nullable<int> SLPullListsId
        {
            get
            {
                if (_SLPullListsId == null)
                    return 0;
                else
                    return _SLPullListsId;
            }
            set { _SLPullListsId = value; }
        }
        private Nullable<DateTime> _DatePulled;
        public Nullable<DateTime> DatePulled
        {
            get
            {
                return _DatePulled;
            }
            set { _DatePulled = value; }
        }
        private Nullable<DateTime> _DateExceptioned;
        public Nullable<DateTime> DateExceptioned
        {
            get
            {
                return _DateExceptioned;
            }
            set { _DateExceptioned = value; }
        }
        private Nullable<DateTime> _DateDeleted;
        public Nullable<DateTime> DateDeleted
        {
            get
            {
                return _DateDeleted;
            }
            set { _DateDeleted = value; }
        }
        public string DeleteOperatorId { get; set; }
        public string FileRoomOrder { get; set; }
        public string Instructions { get; set; }
        private Nullable<DateTime> _DateNeeded;
        public Nullable<DateTime> DateNeeded
        {
            get
            {
                return _DateNeeded;
            }
            set { _DateNeeded = value; }
        }
        private Nullable<int> _PriorityOrder;
        public Nullable<int> PriorityOrder
        {
            get
            {
                if (_PriorityOrder == null)
                    return 0;
                else
                    return _PriorityOrder;
            }
            set { _PriorityOrder = value; }
        }
        public string RequestedBy { get; set; }
    }
}
