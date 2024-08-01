using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using Smead.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IViewService
    {
        public string GetBindViewMenus(string root, List<Table> lTableEntities, List<View> lViewEntities, Passport _passport);
        public Task<List<GridColumns>> GetColumnsData(List<View> lView, List<ViewColumn> lViewColumns, List<Table> lTables, int intViewsId, string sAction, string ConnectionString);
        public Task<Dictionary<string, string>> GetFieldTypeAndSize(Table oTables, string sFieldName, string ConnectionString);
        public Task<Dictionary<string, string>> BindTypeAndSize(string ConnectionString, string sFieldName, string sTableName, Table oTables = null);
        public ProcessFilterResult ProcessFilter(List<ViewFilter> cViewFilters, List<ViewColumn> cViewColumns, List<Table> cTables, string ConnectionString, View oViews, Table oTable, bool bActiveFilters, string sReturnStr, bool bQBEFilter, bool bConvertMemoField = true);
        public Task<FiltereOperaterValue> FillOperatorsDropDownOnChange(Dictionary<string, bool> filterControls, List<View> lView, List<Table> lTable, int iColumnNum, string TableName, string ConnectionString);
        public bool DataLocked(string sFieldName, string table, string ConnectionString);
        public Dictionary<int, ViewColumn> FillFilterFieldNames(List<ViewColumn> lViewColumn);
        public void CreateViewsEntity(View oldViews, View newViews);
        public Task SQLViewDelete(int Id, string ConnectionString);
    }
}
