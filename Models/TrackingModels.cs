using MSRecordsEngine.Entities;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using System.Collections.Generic;
using System.Data;

namespace MSRecordsEngine.Models
{
    public class BuildTrackingLocationSQL
    {
        public Table Table { get; set; }
        public string BuildTrackingLocationSQLRet { get; set; }
    }

    public class ValidateFromOneTableReturn
    {
        public bool ValidateFromOneTableRet { get; set; }
        public string From { get; set; }
    }

    public class CreateJoinTables
    {
        public string Joins { get; set; }
        public List<Table> Tables { get; set; }
        public bool CreateJoinTablesRet { get; set; }
    }

    public class ColumnComboboxResult
    {
        public string ValueFieldName { get; set; }
        public string ThisFieldHeading { get; set; }
        public string FirstLookupHeading { get; set; }
        public string SecondLookupHeading { get; set; }
        public DataTable Table { get; set; }
    }
    public class TrackingModeld
    {
        public TrackingModeld()
        {
            ListofRequests = new List<Requestlist>();
        }
        private Parameters @params { get; set; }
        private int ViewId { get; set; }
        private string TrackRowId { get; set; }
        public string lblTracking { get; set; }
        public string lblTrackTime { get; set; }
        public string lblDueBack { get; set; }
        public List<Requestlist> ListofRequests { get; set; }
        public string TableName { get; set; }
        public bool isDeleteAllow { get; set; } = false;
        //request properties
        public string NewRequestCount { get; set; }
        public string RequestNewButtonLabel { get; set; }
        public string imgRequestNewButton { get; set; }
        public string ancRequestNewButton { get; set; }
        //request exception properties
        public string NewExceptionCount { get; set; }
        public string RequestExceptionButtonLabel { get; set; }
        public string imgRequestExceptionButton { get; set; }
        public string ancRequestExceptionButton { get; set; }
        public bool isError {  get; set; }
        public string Msg { get; set; }
    }
    //public class trackableUiParams
    //{
    //    public int ViewId { get; set; }
    //    public string RowKeyid { get; set; }
    //    public string TableName { get; set; }
    //}

    public class Requestlist
    {
        public string DateRequested { get; set; }
        public string EmployeeName { get; set; }
        public string DateNeeded { get; set; }
        public string Status { get; set; }
        public string reqid { get; set; }
    }

    public class InnerTruncateTrackingHistory_Response
    {
        public bool Success { get; set; }
        public string KeysType { get; set; }
    }

    public class CheckaddNewAttachmentPermission_Response
    {
        public bool Success { get; set; }
        public int ErrorNumber { get; set; }
        public string ExpMaxSize { get; set; }
    }
}
