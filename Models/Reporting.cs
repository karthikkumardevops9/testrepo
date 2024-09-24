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
    public class ReportingMenu
    {
        public string AccessMenu { get; set; }
        public string dateFormat { get; set; }
        public PagingModel Paging { get; set; } = new PagingModel();
    }

    public class AuditReportSearch
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
        public class DDLprops
        {
            public string text { get; set; }
            public int value { get; set; }
            public string valuetxt { get; set; }
            public bool isIdstring { get; set; }
        }
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
        public Dictionary<string, string> IdsByTable = null;
        
        internal Dictionary<string, DataTable> Descriptions = null;
        public List<string> ListOfHeader { get; set; }
        public List<List<string>> ListOfRows { get; set; }
        public string DisplayNotAuthorized { get; set; }
        public string PageTitle { get; set; }
        public string lblTitle { get; set; }
        public string lblSubtitle { get; set; }
        public string lblReportDate { get; set; }
        public string TotalRowsCount { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public DateTime dateFromTxt { get; set; }
        internal DataTable _TrackingTables { get; set; }
        public string lblSelectReport { get; set; }
        public List<DDLItems> ddlSelectReport { get; set; }
        public int ddlid { get; set; }
        public bool isPullListDDLCall { get; set; }
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

    public class PagingModel
    {
        public PagingModel()
        {
            PerPageRecord = 1000;
        }
        public int TotalPage { get; set; }
        public int TotalRecord { get; set; }
        public int PerPageRecord { get; set; }
        public int PageNumber { get; set; }
    }

    public class DDLItems
    {
        public string text { get; set; }
        public string value { get; set; }
        public string Id { get; set; }
    }
    public class ReportCommonModel
    {
        public bool isError { get; set; }
        public string errortype { get; set; }
        public int iscreated { get; set; }
        public string message { get; set; }
        public string Msg { get; set; }
    }

}
