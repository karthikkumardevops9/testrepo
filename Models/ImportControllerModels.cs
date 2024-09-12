using MSRecordsEngine.Entities;
using Smead.Security;
using System;

namespace MSRecordsEngine.Models
{
    public class RunScriptAfterAddEditParam
    {
        public Passport Passport { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
    }

    public class RunScriptBeforeAddParam
    {
        public Passport Passport { get; set; }
        public string TableName { get; set; }
    }

    public class RunScriptBeforeEditParam
    {
        public Passport Passport { get; set; }
        public string TableName { get; set; }
        public string FieldIndex { get; set; }
    }

    public class DoTransferParam
    {
        public string TableName { get; set; }
        public string TableId { get; set; }
        public string DestinationTableName { get; set; }
        public string DestinationTableId { get; set; }
        public DateTime DueDate { get; set; }
        public string UserName { get; set; }
        public Passport passport { get; set; }
        public string TrackingAdditionalField1 { get; set; }
        public string TrackingAdditionalField2 { get; set; }
        public DateTime TransactionDateTime { get; set; }
    }

    public class RemoveRecordForInactiveEligibleListParam
    {
        public string SQL { get; set; }
        public Table table { get; set; }
        public string ConnectionString { get; set; }
        public string rowId { get; set; }
        public string fieldOpenDate { get; set; }
        public string fieldCreateDate { get; set; }
        public string fieldCloseDate { get; set; }
        public string fieldOtherDate { get; set; }
    }
}
