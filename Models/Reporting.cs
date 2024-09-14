using Microsoft.AspNetCore.Http;
using Smead.Security;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System;
using System.Globalization;
using MSRecordsEngine.Models.FusionModels;

namespace MSRecordsEngine.Models
{
    public class ReportingMenu : BaseModel
    {
        public string AccessMenu { get; set; }
        public string dateFormat { get; set; }
        public PagingModel Paging { get; set; } = new PagingModel();
    }

    public class AuditReportSearch : BaseModel
    {
        public AuditReportSearch()
        {
            userDDL = new List<DDLprops>();
            objectDDL = new List<DDLprops>();
            ListOfRows = new List<ArrayList>();
            ListOfHeader = new List<string>();
        }
        public PagingModel Paging { get; set; } = new PagingModel();
        public List<DDLprops> userDDL { get; set; }
        public List<DDLprops> objectDDL { get; set; }
        public string SubTitle { get; set; }
        public List<ArrayList> ListOfRows { get; set; }
        public List<string> ListOfHeader { get; set; }
        public string dateFormat { get; set; }
        public class UIproperties
        {
            public string UserName { get; set; }
            public string UserDDLId { get; set; }
            public string ObjectId { get; set; }
            public string ObjectName { get; set; }
            public string Id { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public bool AddEditDelete { get; set; } = false;
            public bool SuccessLogin { get; set; } = false;
            public bool ConfDataAccess { get; set; } = false;
            public bool FailedLogin { get; set; } = false;
            public bool ChildTable { get; set; } = false;
            public int PageNumber { get; set; } = 0;
        }
        public class DDLprops
        {
            public string text { get; set; }
            public int value { get; set; }
            public string valuetxt { get; set; }
            public bool isIdstring { get; set; }
        }
    }
    public class RunAuditSearchReqModel
    {
        public AuditReportSearch.UIproperties paramss { get; set; }
    }
    public class ReportsModels : BaseModel
    {
        public ReportsModels()
        {
            ListOfHeader = new List<string>();
            ListOfRows = new List<List<string>>();
            ddlSelectReport = new List<DDLItems>();
        }

        public PagingModel Paging { get; set; } = new PagingModel();
        public ReportingJsonModel UI { get; set; }
        private Dictionary<string, string> IdsByTable = null;
        private Dictionary<string, DataTable> Descriptions = null;
        public List<string> ListOfHeader { get; set; }
        public List<List<string>> ListOfRows { get; set; }
        public string DisplayNotAuthorized { get; set; }
        public string PageTitle { get; set; }
        public string lblTitle { get; set; }
        public string lblSubtitle { get; set; }
        public string lblReportDate { get; set; }
        public string TotalRowsCount { get; set; }
        protected CultureInfo CultureInfo { get; set; }
        private DateTime dateFromTxt { get; set; }
        protected DataTable _TrackingTables { get; set; }
        public string lblSelectReport { get; set; }
        public List<DDLItems> ddlSelectReport { get; set; }
        protected int ddlid { get; set; }
        private bool isPullListDDLCall { get; set; }
    }
    public class RetentionReportModel : ReportsModels
    {
        public RetentionReportModel()
        {
            PermanentArchive = new RetentionButtons();
            Purge = new RetentionButtons();
            Destruction = new RetentionButtons();
            SubmitDisposition = new RetentionButtons();
        }
        public RetentionButtons PermanentArchive { get; set; }
        public RetentionButtons Purge { get; set; }
        public RetentionButtons Destruction { get; set; }
        public RetentionButtons SubmitDisposition { get; set; }
    }
    public class RetentionButtons : BaseModel
    {
        public RetentionButtons()
        {
            ddlSelection = new List<DDLItems>();
        }
        public string username { get; set; }
        public string TodayDate { get; set; }
        public List<DDLItems> ddlSelection { get; set; }
        public bool isBtnVisibal { get; set; }
        public string btnText { get; set; }
        public string btnSubmitText { get; set; }
        public string btnSetSubmitType { get; set; }
    }

    public class ReportingJsonModel
    {
        public int reportType { get; set; }
        public int pageNumber { get; set; }
        public string tableName { get; set; } = "";
        public List<items> ListofPullItem { get; set; }
        public string id { get; set; } = "";
        public bool isQueryFromDDL { get; set; }
        public bool isBatchRequest { get; set; }
        public List<string> ids { get; set; }
        public string udate { get; set; }
        public string ddlSelected { get; set; }
        public string username { get; set; }
        public string locationId { get; set; }
        public string submitType { get; set; }
        public int reportId { get; set; }
        public bool isCountRecord { get; set; }
    }

    public class ReportingJsonModelReq
    {
        public ReportingJsonModel paramss { get; set; }
    }
    public class PagingModel
    {
        public int TotalPage { get; set; }
        public int TotalRecord { get; set; }
        public int PerPageRecord { get; set; }
        public int PageNumber { get; set; }
    }
    public class items
    {
        public string tableName { get; set; }
        public string tableid { get; set; }
    }

    public class DDLItems
    {
        public string text { get; set; }
        public string value { get; set; }
        public string Id { get; set; }
    }

}
