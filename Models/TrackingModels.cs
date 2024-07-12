using MSRecordsEngine.Entities;
using System.Collections.Generic;

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
}
