using MSRecordsEngine.Entities;
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
}
