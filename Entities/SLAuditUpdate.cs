using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLAuditUpdate
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
        public string OperatorsId { get; set; }
        public string NetworkLoginName { get; set; }
        public string Domain { get; set; }
        public string ComputerName { get; set; }
        public string MacAddress { get; set; }
        public string IP { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public string DataBefore { get; set; }
        public string DataAfter { get; set; }
        private Nullable<DateTime> _UpdateDateTime;
        public Nullable<DateTime> UpdateDateTime
        {
            get
            {
                return _UpdateDateTime;
            }
            set { _UpdateDateTime = value; }
        }
        private Nullable<int> _actionType;
        public Nullable<int> ActionType
        {
            get
            {   if (_actionType == null) return 0;
                return _actionType;
            }
            set { _actionType = value; }
        }
    }
}
