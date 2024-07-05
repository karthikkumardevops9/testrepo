using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IViewService
    {
        public Task<List<GridColumns>> GetColumnsData(List<View> lView, List<ViewColumn> lViewColumns, List<Table> lTables, int intViewsId, string sAction, string ConnectionString);
        public Task<Dictionary<string, string>> GetFieldTypeAndSize(Table oTables, string sFieldName, string ConnectionString);
        public Task<Dictionary<string, string>> BindTypeAndSize(string ConnectionString, string sFieldName, string sTableName, Table oTables = null);
    }
}
