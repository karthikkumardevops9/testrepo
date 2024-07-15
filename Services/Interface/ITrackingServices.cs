using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using System.Collections.Generic;

namespace MSRecordsEngine.Services.Interface
{
    public interface ITrackingServices
    {
        public BuildTrackingLocationSQL BuildTrackingLocationSQL(List<Table> itableQuery, string ConnectionString, string sCurrentSQL, Table oTables);
    }
}
