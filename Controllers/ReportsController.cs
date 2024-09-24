using MsRecordEngine.Controllers;
using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Services.Interface;
using MSRecordsEngine.Services;
using MSRecordsEngine.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using static MSRecordsEngine.RecordsManager.Retention;
using System.DirectoryServices.ActiveDirectory;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly CommonControllersService<ReportsController> _commonService;
        private readonly ILayoutDataService _layoutService;
        private readonly IReportsService _reportsservice;
        public ReportsController(CommonControllersService<ReportsController> commonControllersService, ILayoutDataService layoutservice, IReportsService reportsService)
        {
            _commonService = commonControllersService;
            _layoutService = layoutservice;
            _reportsservice = reportsService;
        }
        [HttpPost]
        [Route("ReportingMenu")]
        public async Task<ReportingMenu> ReportingMenu(UserInterfaceProps props)
        {
            try
            {
               return await _reportsservice.ReportingMenu(props);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("RunAuditSearch")]
        public async Task<AuditReportSearch> RunAuditSearch(UIproperties props)
        {
            try
            {
                return await _reportsservice.RunAuditSearch(props, false);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("RunAuditReportPagination")]
        public async Task<AuditReportSearch> RunAuditReportPagination(UIproperties props)
        {
            try
            {
                return await _reportsservice.RunAuditSearch(props, true);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("InitiateReports")]
        public async Task<ReportsModels> InitiateReports(ReportingJsonModelReq req)
        {
            try
            {
                return await _reportsservice.InitiateReports(req);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("InitiateReportsPagination")]    
        public async Task<ReportsModels> InitiateReportsPagination(ReportingJsonModelReq req)
        {
            try
            {
                return await _reportsservice.InitiateReportsPagination(req);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("InitiateRetentionReport")]
        public async Task<RetentionReportModel> InitiateRetentionReport(ReportingJsonModelReq req)
        {
            try
            {
               return await _reportsservice.InitiateRetentionReport(req);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("InitiateRetentionReportPagination")]
        public async Task<RetentionReportModel> InitiateRetentionReportPagination(ReportingJsonModelReq req)
        {
            try
            {
                return await _reportsservice.InitiateRetentionReportPagination(req);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("BtnSendRequestToThePullList")]
        public async Task<ReportsModels> BtnSendRequestToThePullList(ReportingJsonModelReq req)
        {
            try
            {
                return await _reportsservice.BtnSendRequestToThePullList(req);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("BtnSubmitInactive")]
        public async Task<ReportCommonModel> BtnSubmitInactive(ReportingJsonModelReq param)
        {
            try
            {
                return await _reportsservice.BtnSubmitInactive(param);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("BtnSubmitDisposition")]
        public async Task<ReportCommonModel> BtnSubmitDisposition(ReportingJsonModelReq req)
        {
            try
            {
                return await _reportsservice.BtnSubmitDisposition(req);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("BtnCreateNewArchived")]
        public async Task<int> BtnCreateNewArchived(UserInterfaceProps req)
        {
            try
            {
                return await _reportsservice.CreateEligibleRecordsForReport(req, FinalDisposition.PermanentArchive);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("BtnCreateNewPurged")]
        public async Task<int> BtnCreateNewPurged(UserInterfaceProps req)
        {
            try
            {
                return await _reportsservice.CreateEligibleRecordsForReport(req, FinalDisposition.Purge);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("BtnCreateNewDestruction")]
        public async Task<int> BtnCreateNewDestruction(UserInterfaceProps req)
        {
            try
            {
                return await _reportsservice.CreateEligibleRecordsForReport(req, FinalDisposition.Destruction);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpPost]
        [Route("GetauditReportView")]
        public async Task<AuditReportSearch> GetauditReportView(UserInterfaceProps props)
        {
            try
            {
                return await _reportsservice.GetauditReportView(props);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("GetInactivePullList")]
        public async Task<RetentionButtons> GetInactivePullList(UserInterfaceProps props)
        {
            try
            {
                return await _reportsservice.GetInactivePopup(props);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("GetSubmitForm")]
        public async Task<RetentionButtons> GetSubmitForm(UserInterfaceProps props)
        {
            try
            {
                return await _reportsservice.GetSubmitForm(props);
            }
            catch (System.Exception ex)
            {
                _commonService.Logger.LogError(ex.Message);
                throw;
            }
        }

    }
}
