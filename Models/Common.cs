using System.Collections.Generic;
using System.ComponentModel;

namespace MSRecordsEngine.Models
{
    public sealed class Common
    {
        public static string[] dataType = new string[] { "datetime", "decimal", "double", "int16", "int32", "int64" };
        public const string BOOLEAN_TYPE = "boolean";
        // Data types and sizes constants
        public const string FT_BINARY = "Binary";
        public const string FT_LONG_INTEGER = "Long Integer";
        public const string FT_LONG_INTEGER_SIZE = "4";
        public const string FT_AUTO_INCREMENT = "Automatic Counter";
        public const string FT_AUTO_INCREMENT_SIZE = "4";
        public const string FT_TEXT = "Text";
        public const string FT_SHORT_INTEGER = "Short Integer";
        public const string FT_SHORT_INTEGER_SIZE = "2";
        public const string FT_BOOLEAN = "Yes/No";
        public const string FT_BOOLEAN_SIZE = "1";
        public const string FT_DOUBLE = "Floating Number (Double)";
        public const string FT_DOUBLE_SIZE = "8";
        public const string FT_DATE = "Date";
        public const string FT_DATE_SIZE = "8";
        public const string FT_MEMO = "Memo";
        public const string FT_MEMO_SIZE = "N/A";
        public const string FT_SMEAD_COUNTER = "Counter";
        public const string FT_SMEAD_COUNTER_SIZE = "4";
        public const string FT_UNKNOWN = "Unknown";

        public const string SECURE_APPLICATION = "Applications";
        // IMPORTANT! Space is required at the beginning of SECURE_REPORTS* constants
        public const string SECURE_REPORTS_AUDIT = " Auditing";
        public const string SECURE_REPORTS_REQUEST = " Requestor";
        public const string SECURE_REPORTS_RETENTION = " Retention";
        public const string SECURE_REPORTS_TRACKING = " Tracking";
        public const string SECURE_LABEL_SETUP = "Label Integration";
        public const string SECURE_SQL_SCRIPT = "Database Scripting";
        public const string SECURE_IMPORT_SETUP = "Import Setup";
        public const string SECURE_IPUBLISH = "Snapshot";
        public const string SECURE_ORPHANS = " Orphans";
        public const string SECURE_OPTIONS = "Options";
        public const string SECURE_RETENTION_DISPO = "Disposition";
        public const string SECURE_RETENTION_SETUP = "Code Maintenance";
        public const string SECURE_RETENTION_ARCHIVE = "View Archived Records";
        public const string SECURE_RETENTION_DESTROY = "View Destroyed Records";
        public const string SECURE_RETENTION_INACTIVE = "View Inactive Records";
        public const string SECURE_REPORT_STYLES = "Report Styles";
        public const string SECURE_SCANNER = "Scanner";
        public const string SECURE_SECURITY = "Security Configuration";
        public const string SECURE_SECURITY_USER = "Security Users";
        public const string SECURE_STORAGE = "Storage Configuration";
        public const string SECURE_TRACKING = "Tracking";
        public const string SECURE_MYQUERY = "My Queries";
        public const string SECURE_MYFAVORITE = "My Favorites";
        private const string EncryptionKey = "MAKV2SPBNI99212";
        public const string SECURE_DASHBOARD = "Dashboard";
        public const string SECURE_REPORTS = "Reports";
    }

    public class Enums
    {
        public enum SecureObjects
        {
            Application = Smead.Security.SecureObject.SecureObjectType.Application,
            Table = Smead.Security.SecureObject.SecureObjectType.Table,
            View = Smead.Security.SecureObject.SecureObjectType.View,
            Annotations = Smead.Security.SecureObject.SecureObjectType.Annotations,
            WorkGroup = Smead.Security.SecureObject.SecureObjectType.WorkGroup,
            Attachments = Smead.Security.SecureObject.SecureObjectType.Attachments,
            Reports = Smead.Security.SecureObject.SecureObjectType.Reports,
            Retention = Smead.Security.SecureObject.SecureObjectType.Retention,
            LinkScript = Smead.Security.SecureObject.SecureObjectType.LinkScript,
            ScanRules = Smead.Security.SecureObject.SecureObjectType.ScanRules,
            Volumes = Smead.Security.SecureObject.SecureObjectType.Volumes,
            OutputSettings = Smead.Security.SecureObject.SecureObjectType.OutputSettings,
            Orphans = Smead.Security.SecureObject.SecureObjectType.Orphans
        }
        public enum PassportPermissions
        {
            Access = Smead.Security.Permissions.Permission.Access,
            Execute = Smead.Security.Permissions.Permission.Execute,
            View = Smead.Security.Permissions.Permission.View,
            Add = Smead.Security.Permissions.Permission.Add,
            Edit = Smead.Security.Permissions.Permission.Edit,
            Delete = Smead.Security.Permissions.Permission.Delete,
            Destroy = Smead.Security.Permissions.Permission.Destroy,
            Transfer = Smead.Security.Permissions.Permission.Transfer,
            Request = Smead.Security.Permissions.Permission.Request,
            RequestHigh = Smead.Security.Permissions.Permission.RequestHigh,
            Configure = Smead.Security.Permissions.Permission.Configure,
            PrintLabel = Smead.Security.Permissions.Permission.PrintLabel,
            Scanning = Smead.Security.Permissions.Permission.Scanning,
            Index = Smead.Security.Permissions.Permission.Index,
            Move = Smead.Security.Permissions.Permission.Move,
            Copy = Smead.Security.Permissions.Permission.Copy,
            Redact = Smead.Security.Permissions.Permission.Redact,
            Versioning = Smead.Security.Permissions.Permission.Versioning,
            Import = Smead.Security.Permissions.Permission.Import,
            Export = Smead.Security.Permissions.Permission.Export,
            Email = Smead.Security.Permissions.Permission.Email,
            Print = Smead.Security.Permissions.Permission.Print,
            RequestOnBehalf = Smead.Security.Permissions.Permission.RequestOnBehalf
        }
        public enum BackgroundTaskType
        {
            Normal = 2,
            Transfer,
            Export
        }
        public enum BackgroundTaskInDetail
        {
            Normal = 2,
            Transfer,
            ExportCSV,
            ExportTXT
        }

        public enum BackgroundTaskStatus
        {
            [Description("Pending")]
            Pending = 1,
            [Description("InProgress")]
            InProgress,
            [Description("Completed")]
            Completed,
            [Description("Error")]
            Error,
            [Description("InQue")]
            InQue
        }

    }

    public class CollectionsClass
    {
        public static List<string> mcEngineTablesList;
        public static List<string> mcEngineTablesOkayToImportList;
        public static List<string> mcEngineTablesNotNeededList;

        public static bool IsEngineTable(string sTableName)
        {
            int iIndex;
            int iCount = EngineTablesList.Count - 1;

            var loopTo = iCount;
            for (iIndex = 0; iIndex <= loopTo; iIndex++)
            {
                if (EngineTablesList[iIndex].Trim().ToLower().Equals(sTableName.Trim().ToLower()))
                    return true;
            }
            return false;
        }

        public static List<string> EngineTablesList
        {
            get
            {
                if (mcEngineTablesList is null)
                {
                    mcEngineTablesList = new List<string>();
                    mcEngineTablesOkayToImportList = new List<string>();
                    mcEngineTablesNotNeededList = new List<string>();
                }

                if (mcEngineTablesList.Count == 0)
                {
                    mcEngineTablesList.Add("AddInReports");
                    mcEngineTablesList.Add("Annotations");
                    mcEngineTablesList.Add("AssetStatus");
                    mcEngineTablesList.Add("Attributes");
                    mcEngineTablesList.Add("COMMLabels");
                    mcEngineTablesList.Add("COMMLabelLines");
                    mcEngineTablesList.Add("CoverLetterLines");
                    mcEngineTablesList.Add("CoverLetters");
                    mcEngineTablesList.Add("Databases");
                    mcEngineTablesList.Add("DBVersion");
                    mcEngineTablesList.Add("DestCertDetail");
                    mcEngineTablesList.Add("DestCerts");
                    mcEngineTablesList.Add("Directories");
                    mcEngineTablesList.Add("FaxAddresses");
                    mcEngineTablesList.Add("FaxConfigurations");
                    mcEngineTablesList.Add("FaxesInBound");
                    mcEngineTablesList.Add("FaxesOutBound");
                    mcEngineTablesList.Add("FieldDefinitions");
                    mcEngineTablesList.Add("ImagePointers");
                    mcEngineTablesList.Add("ImageTablesList");
                    mcEngineTablesList.Add("ImportFields");
                    mcEngineTablesList.Add("ImportJobs");
                    mcEngineTablesList.Add("ImportLoads");
                    mcEngineTablesList.Add("LinkScript");
                    mcEngineTablesList.Add("LinkScriptFeatures");
                    mcEngineTablesList.Add("LinkScriptHeader");
                    mcEngineTablesList.Add("LitigationSupport");
                    mcEngineTablesList.Add("LitigationTrackables");
                    mcEngineTablesList.Add("OfficeDocTypes");
                    mcEngineTablesList.Add("OneStripForms");
                    mcEngineTablesList.Add("OneStripJobFields");
                    mcEngineTablesList.Add("OneStripJobs");
                    mcEngineTablesList.Add("OutputSettings");
                    mcEngineTablesList.Add("PCFilesPointers");
                    mcEngineTablesList.Add("RecordTypes");
                    mcEngineTablesList.Add("RelationShips");
                    mcEngineTablesList.Add("ReportStyles");
                    mcEngineTablesList.Add("Retention");
                    mcEngineTablesList.Add("RetentionLists");
                    mcEngineTablesList.Add("ScanBatches");
                    mcEngineTablesList.Add("ScanFormLines");
                    mcEngineTablesList.Add("ScanForms");
                    mcEngineTablesList.Add("ScanList");
                    mcEngineTablesList.Add("ScanRules");
                    mcEngineTablesList.Add("SDLKFolderTypes");
                    mcEngineTablesList.Add("SDLKPullLists");
                    mcEngineTablesList.Add("SDLKRequestor");
                    mcEngineTablesList.Add("SDLKStatus");
                    mcEngineTablesList.Add("SDLKStatusHistory");
                    mcEngineTablesList.Add("Settings");
                    mcEngineTablesList.Add("SLColdArchives");
                    mcEngineTablesList.Add("SLColdPointers");
                    mcEngineTablesList.Add("SLColdSetupCols");
                    mcEngineTablesList.Add("SLColdSetupForms");
                    mcEngineTablesList.Add("SLColdSetupRows");
                    mcEngineTablesList.Add("SLIndexWizard");
                    mcEngineTablesList.Add("SLIndexWizardCols");
                    mcEngineTablesList.Add("SLPullLists");
                    mcEngineTablesList.Add("SLRequestor");
                    mcEngineTablesList.Add("SLRetentionCitations");
                    mcEngineTablesList.Add("SLRetentionCitaCodes");
                    mcEngineTablesList.Add("SLRetentionCodes");
                    mcEngineTablesList.Add("SLRetentionInactive");
                    mcEngineTablesList.Add("SLDestructionCerts");
                    mcEngineTablesList.Add("SLDestructCertItems");
                    mcEngineTablesList.Add("SLTableFileRoomOrder");
                    mcEngineTablesList.Add("SLTrackingSelectData");
                    mcEngineTablesList.Add("SLAuditConfData");
                    mcEngineTablesList.Add("SLAuditFailedLogins");
                    mcEngineTablesList.Add("SLAuditLogins");
                    mcEngineTablesList.Add("SLAuditUpdChildren");
                    mcEngineTablesList.Add("SLAuditUpdates");
                    mcEngineTablesList.Add("SLBatchRequests");
                    mcEngineTablesList.Add("SysNextTrackable");
                    mcEngineTablesList.Add("System");
                    mcEngineTablesList.Add("SystemAddresses");
                    mcEngineTablesList.Add("Tables");
                    mcEngineTablesList.Add("Tabletabs");
                    mcEngineTablesList.Add("Tabsets");
                    mcEngineTablesList.Add("TrackableHeaders");
                    mcEngineTablesList.Add("Trackables");
                    mcEngineTablesList.Add("TrackingHistory");
                    mcEngineTablesList.Add("TrackingStatus");
                    mcEngineTablesList.Add("Userlinks");
                    mcEngineTablesList.Add("ViewColumns");
                    mcEngineTablesList.Add("ViewFilters");
                    mcEngineTablesList.Add("ViewParms");
                    mcEngineTablesList.Add("Views");
                    mcEngineTablesList.Add("Volumes");
                    mcEngineTablesList.Add("SLTextSearchItems");
                    mcEngineTablesList.Add("SLGrabberFunctions");
                    mcEngineTablesList.Add("SLGrabberControls");
                    mcEngineTablesList.Add("SLGrabberFields");
                    mcEngineTablesList.Add("SLGrabberFldParts");
                    mcEngineTablesList.Add("SLServiceTasks");
                    mcEngineTablesList.Add("SLIndexer");
                    mcEngineTablesList.Add("SLIndexerCache");
                    mcEngineTablesList.Add("SLCollections");
                    mcEngineTablesList.Add("SLCollectionItems");
                    mcEngineTablesList.Add("SLSignature");
                    mcEngineTablesList.Add("SecureGroup");
                    mcEngineTablesList.Add("SecureObject");
                    mcEngineTablesList.Add("SecureObjectPermission");
                    mcEngineTablesList.Add("SecurePermission");
                    mcEngineTablesList.Add("SecureUser");
                    mcEngineTablesList.Add("SecureUserGroup");
                    mcEngineTablesList.Add("SecurePermissionDescription");
                    mcEngineTablesList.Add("LookupType");
                    mcEngineTablesList.Add("GridSettings");
                    mcEngineTablesList.Add("GridColumn");
                    mcEngineTablesList.Add("MobileDetails");
                    // Add ViewTable in list
                    mcEngineTablesList.Add("vwColumnsAll");
                    mcEngineTablesList.Add("vwGetOutputSetting");
                    mcEngineTablesList.Add("vwGridSettings");
                    mcEngineTablesList.Add("vwTablesAll");
                    // new system tables for 10.1.x  RVW 09/10/2017
                    mcEngineTablesList.Add("s_SavedCriteria");
                    mcEngineTablesList.Add("s_SavedChildrenQuery");
                    mcEngineTablesList.Add("s_SavedChildrenFavorite");
                    // new system tables for 10.1.x  RVW 02/15/2018
                    mcEngineTablesList.Add("SLServiceTaskItems");
                    // new system tables for 10.2.x  RVW 03/08/2019
                    mcEngineTablesList.Add("s_AttachmentCart");
                    // new system tables for 11.0.x  RVW 09/22/2021
                    mcEngineTablesList.Add("SLUserDashboard");

                    mcEngineTablesOkayToImportList.Add("slretentioncodes");
                    mcEngineTablesOkayToImportList.Add("slretentioncitations");
                    mcEngineTablesOkayToImportList.Add("slretentioncitacodes");

                    mcEngineTablesNotNeededList.Add("coverletterlines");
                    mcEngineTablesNotNeededList.Add("coverletters");
                    mcEngineTablesNotNeededList.Add("destcertdetail");
                    mcEngineTablesNotNeededList.Add("destcerts");
                    mcEngineTablesNotNeededList.Add("faxaddresses");
                    mcEngineTablesNotNeededList.Add("faxconfigurations");
                    mcEngineTablesNotNeededList.Add("faxesinbound");
                    mcEngineTablesNotNeededList.Add("faxesoutbound");
                    mcEngineTablesNotNeededList.Add("fielddefinitions");
                    mcEngineTablesNotNeededList.Add("litigationsupport");
                    mcEngineTablesNotNeededList.Add("litigationtrackables");
                    mcEngineTablesNotNeededList.Add("retention");
                    mcEngineTablesNotNeededList.Add("retentionlists");
                    mcEngineTablesNotNeededList.Add("scanformlines");
                    mcEngineTablesNotNeededList.Add("scanforms");
                    mcEngineTablesNotNeededList.Add("sdlkfoldertypes");
                    mcEngineTablesNotNeededList.Add("sdlkpulllists");
                    mcEngineTablesNotNeededList.Add("sdlkrequestor");
                    mcEngineTablesNotNeededList.Add("sdlkstatus");
                    mcEngineTablesNotNeededList.Add("sdlkstatushistory");
                    mcEngineTablesNotNeededList.Add("trackableheaders");
                    mcEngineTablesNotNeededList.Add("viewparms");
                    mcEngineTablesNotNeededList.Add("slloggedinusers");
                    mcEngineTablesNotNeededList.Add("devicetype");
                    mcEngineTablesNotNeededList.Add("members");
                    mcEngineTablesNotNeededList.Add("securitygroups");
                    mcEngineTablesNotNeededList.Add("operators");
                    mcEngineTablesNotNeededList.Add("operators_back");
                }

                return mcEngineTablesList;
            }
        }
    }

    

    
}
