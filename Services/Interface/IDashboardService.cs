using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IDashboardService
    {
        Task<bool> CheckDashboardNameDuplicate(int DId, string Name, int UId, string ConnectionString);
        Task<int> InsertDashbaord(string ConnectionString, string Name, int UId, string Json);
        Task<string> GetDashboardListHtml(string ConnectionString, int UserId);
        Task<List<TableModel>> GetTableNames(object tableIds, string ConnectionString);
        Task<SLUserDashboard> GetDashbaordId(int Id, string ConnectionString);
        Task<int> DeleteDashboard(int DashboardId, string ConnectionString);
        Task<int> UpdateDashbaordName(string Name, int Id, string ConnectionString);
        Task<int> UpdateDashboardJson(string Json, int Id, string ConnectionString);
        Task<List<CommonDropdown>> TrackableTable(Passport passport);
        Task<List<CommonDropdown>> AuditTable(Passport passport);
        Task<List<CommonDropdown>> Users(string connectionString);
        Task<List<ChartModel>> GetBarPieChartData(string tableName, int viewId, string columnName, Passport passport);
        Task<List<ChartModel>> GetTrackedChartData(object tableIds, object filter, object period, string connectionString);
        Task<List<ChartOperatinModelRes>> GetOperationChartData(string tableIds, string usersIds, string AuditTypeIds, string period, string filter, string connectionString);
        Task<List<ChartModel>> GetTimeSeriesChartData(string tableName, string columnName, int viewId, string period, string filter, Passport passport);
        Task<int> GetBarPieChartDataCount(string tableName, string columnName, string connectionString);
        Task<int> GetTimeSeriesChartDataCount(string tableName, string columnName, string period, string filter, string connectionString);
        Task<int> GetTrackedChartDataCount(string tableIds, string filter, string period, string connectionString);
        Task<int> GetOperationChartDataCount(string tableIds, string usersIds, string AuditTypeIds, string period, string filter, string connectionString);
        List<CommonDropdown> GetViewColumnOnlyWithType(string tableName, int viewId, string shortDatePattern, Passport passport);
        List<CommonDropdown> ViewColumns(int ViewId, string ShortDatePattern, Passport passport);
        Task<List<ChartModel>> GetBarPieChartData(string tableName, object columnName, string connectionString);
        Task<List<ChartModel>> GetTimeSeriesChartData(string tableName, string columnName, string period, string filter, string connectionString);

    }
}
