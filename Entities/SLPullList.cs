using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLPullList
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
        public string SLBatchRequestComment { get; set; }
        private Nullable<bool> _BatchPullList;
        public Nullable<bool> BatchPullList
        {
            get
            {
                if (_BatchPullList == null)
                    return false;
                else
                    return _BatchPullList;
            }
            set { _BatchPullList = value; }
        }
        private Nullable<bool> _BatchPrinted;
        public Nullable<bool> BatchPrinted
        {
            get
            {
                if (_BatchPrinted == null)
                    return false;
                else
                    return _BatchPrinted;
            }
            set { _BatchPrinted = value; }
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
    }
}
