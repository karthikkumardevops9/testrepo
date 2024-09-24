using MSRecordsEngine.Models;
using System.Threading.Tasks;
using static MSRecordsEngine.RecordsManager.Retention;

namespace MSRecordsEngine.Services.Interface
{
    public interface IReportsService
    {
        public Task<ReportingMenu> ReportingMenu(UserInterfaceProps props);
        public Task<AuditReportSearch> RunAuditSearch(UIproperties paramss, bool ispaging);
        public Task<ReportsModels> InitiateReports(ReportingJsonModelReq req);
        public Task<ReportsModels> InitiateReportsPagination(ReportingJsonModelReq req);
        public Task<RetentionReportModel> InitiateRetentionReport(ReportingJsonModelReq req);
        public Task<RetentionReportModel> InitiateRetentionReportPagination(ReportingJsonModelReq req);
        public Task<ReportsModels> BtnSendRequestToThePullList(ReportingJsonModelReq req);
        public Task<ReportCommonModel> BtnSubmitInactive(ReportingJsonModelReq param);
        public Task<ReportCommonModel> BtnSubmitDisposition(ReportingJsonModelReq req);
        public Task<int> CreateEligibleRecordsForReport(UserInterfaceProps props, FinalDisposition iCurrDispositionType);
        public Task<AuditReportSearch> GetauditReportView(UserInterfaceProps props);
        public Task<RetentionButtons> GetInactivePopup(UserInterfaceProps props);
        public Task<RetentionButtons> GetSubmitForm(UserInterfaceProps props);
    }
}
