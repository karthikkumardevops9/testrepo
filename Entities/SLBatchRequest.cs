using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLBatchRequest
    {
        public int Id { get; set; }
        public string OperatorsId { get; set; }
        private Nullable<DateTime> _DateCreated;
        public Nullable<DateTime> DateCreated
        {
            get
            {
                return _DateCreated;
            }
            set { _DateCreated = value; }
        }
        public string Comment { get; set; }
        public string RequestedIds { get; set; }
        public string EmployeeId { get; set; }
        private Nullable<bool> _StayOnTop;
        public Nullable<bool> StayOnTop
        {
            get
            {
                if (_StayOnTop == null)
                    return false;
                else
                    return _StayOnTop;
            }
            set { _StayOnTop = value; }
        }
        public string RequestedTable { get; set; }
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
    }
}
