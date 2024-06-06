using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public partial class SLAuditFailedLogin
    {
        public int Id { get; set; }
        private Nullable<DateTime> _LoginDateTime;
        public Nullable<DateTime> LoginDateTime
        {
            get
            {
                return _LoginDateTime;
            }
            set { _LoginDateTime = value; }
        }
        public string OperatorsId { get; set; }
        public string NetworkLoginName { get; set; }
        public string Domain { get; set; }
        public string ComputerName { get; set; }
        public string MacAddress { get; set; }
        public string IP { get; set; }
        public string ReasonForFailure { get; set; }
        public string TextEntered { get; set; }
        public string Action { get; set; }
        public string DataBefore { get; set; }
        public string DataAfter { get; set; }
        public string Module { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
    }
}
