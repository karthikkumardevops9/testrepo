using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLAuditConfData
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
        private Nullable<DateTime> _AccessDateTime;
        public Nullable<DateTime> AccessDateTime
        {
            get
            {
                return _AccessDateTime;
            }
            set { _AccessDateTime = value; }
        }
        public string ModuleName { get; set; }
        public string Action { get; set; }
        public string DataBefore { get; set; }
        public string DataAfter { get; set; }
        public string Module { get; set; }
    }
}
