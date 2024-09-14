 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Smead.Security;


namespace MSRecordsEngine.Models
{
   
    // RETENTION INFO
   public class RetentionInfo
    {
        public RetentionInfo()
        {
            DropdownRetentionCode = new List<dropdownCode>();
            RetentionStatus = new retentionStatus();
            RetentionArchive = new retentionArchive();
            RetentionInfoInactivityDate = new retentionInactive();
            ListOfHeader = new List<string>();
            ListOfRows = new List<List<string>>();
        }
        public List<dropdownCode> DropdownRetentionCode { get; set; }
        public string RetentionDescription { get; set; }
        public string RetentionItem { get; set; }
        public retentionStatus RetentionStatus { get; set; }
        public retentionInactive RetentionInfoInactivityDate { get; set; }
        public string lblRetentionArchive { get; set; }
        public retentionArchive RetentionArchive { get; set; }
        public string rowid { get; set; }
        public int viewid { get; set; }
        internal DataRow retentionItemRow { get; set; }
        public string selectedItemText { get; set; }
        public List<string> HoldingTable { get; set; }
        public List<List<string>> ListOfRows { get; set; }
        public List<string> ListOfHeader { get; set; }
        public bool BtnAdd { get; set; }
        public bool BtnDelete { get; set; }
        public bool BtnEdit { get; set; }
        public bool DDLDrop { get; set; }
        public string TableName { get; set; }
        public bool Disposed { get; set; }
        public int DispositionType { get; set; }
        public List<List<string>> ReturnOnerow { get; set; }
        public int SldestructionCertid { get; set; }

        public enum meFinalDisposition
        {
            fdNone = 0,
            fdPermanentArchive = 1,
            fdDestruction = 2,
            fdPurge = 3
        }
    }

    public class dropdownCode
    {
        public string value { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
    }

    public class retentionStatus
    {
        public string text { get; set; }
        public string color { get; set; }
    }

    public class retentionArchive
    {
        public object text { get; set; }
        public object color { get; set; }
    }

    public class retentionInactive
    {
        public string text { get; set; }
        public string color { get; set; }
    }

    public class holdingTableprop
    {
        public string RetentionCode { get; set; }
        public bool RetentionHold { get; set; }
        public DateTime? SnoozeUntil { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public int SLDestructionCertsId { get; set; }
        public string HoldReason { get; set; }
        public bool? LegalHold { get; set; }
        public int? Id { get; set; }
    }
   
}