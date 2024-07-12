using MSRecordsEngine.Entities;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;



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
        public enum DataTypeEnum
        {
            rmArray = 8192,
            rmBigInt = 20,
            rmBinary = 128,
            rmBoolean = 11,
            rmBSTR = 8,
            rmChapter = 136,
            rmChar = 129,
            rmCurrency = 6,
            rmDate = 7,
            rmDBDate = 133,
            rmDBTime = 134,
            rmDBTimeStamp = 135,
            rmDecimal = 14,
            rmDouble = 5,
            rmEmpty = 0,
            rmError =10,
            rmFileTime = 64,
            rmGUID = 72,
            rmIDispatch =9,
            rmInteger = 3,
            rmIUnknown = 13,
            rmLongVarBinary = 205,
            rmLongVarChar = 201,
            rmLongVarWChar = 203,
            rmNumeric = 131,
            rmPropVariant = 138,
            rmSingle = 4,
            rmSmallInt = 2,
            rmTinyInt = 16,
            rmUnsignedBigInt = 21,
            rmUnsignedInt = 19,
            rmUnsignedSmallInt = 18,
            rmUnsignedTinyInt = 17,
            rmUserDefined = 132,
            rmVarBinary = 204,
            rmVarChar = 200,
            rmVariant = 12,
            rmVarNumeric = 139,
            rmVarWChar = 202,
            rmWChar = 130
        }
        public enum SecureObjectType
        {
            Application = 1,
            Table = 2,
            View = 3,
            Annotations = 4,
            WorkGroup = 5,
            Attachments = 6,
            Reports = 7,
            Retention = 8,
            LinkScript = 9,
            ScanRules = 10,
            Volumes = 11,
            OutputSettings = 12,
            Orphans = 13
        }
        public enum geViewColumnsLookupType
        {
            ltUndefined = -1,
            ltDirect,
            ltLookup,
            ltImageFlag,
            ltFaxFlag,
            ltCOLDFlag,
            ltPCFilesFlag,
            ltReserved,
            ltChildrenFlag,
            ltRowNumber,
            ltAnyFlag,
            ltTrackingStatus,
            ltTrackingLocation,
            ltChildLookdownCommaDisplayDups,
            ltChildLookdownLFDisplayDups,
            ltChildLookdownCommaHideDups,
            ltChildLookdownLFHideDups,
            ltChildrenCounts,
            ltChildLookdownTotals,
            ltTableUserName,
            ltTableGetUserNameLookup,
            ltRetentionFlag,
            ltAltTableAnyFlag,
            ltCurrentlyAtDisplay,
            ltCurrentlyAtTableName,
            ltSignature,
            ltUserDisplayNameLookup
        }
        public enum meRetentionCodeAssignment
        {
            rcaManual = 0,
            rcaCurrentTable = 1,
            rcaRelatedTable = 2
        }
        public enum geViewColumnDisplayType
        {
            cvAlways,
            cvBaseTab,
            cvPopupTab,
            cvNotVisible,
            cvSmartColumns,
            cvTrackingSmartColumns,
            cvBasedOnProperty,
            cvNeverVisible
        }
        public enum FieldAttributeEnum
        {
            rmFldCacheDeferred = 4096,
            rmFldFixed = 16,
            rmFldIsChapter = 8192,
            rmFldIsCollection = 262144,
            rmFldIsDefaultStream = 131072,
            rmFldIsNullable = 32,
            rmFldIsRowURL = 65536,
            rmFldKeyColumn = 32768,
            rmFldLong = 128,
            rmFldMayBeNull = 64,
            rmFldMayDefer = 2,
            rmFldNegativeScale = 16384,
            rmFldRowID = 256,
            rmFldRowVersion = 512,
            rmFldUnknownUpdatable = 8,
            rmFldUnspecified = -1,
            rmFldUpdatable = 4
        }

        public enum SavedType
        {
            Query = 0,
            Favorite = 1,
            Valt = 2
        }
        public enum ReportsType
        {
            PastDueTrackableItemsReport,
            ObjectOut,
            ObjectsInventory,
            RequestNew,
            RequestNewBatch,
            RequestPullList,
            RequestException,
            RequestInProcess,
            RequestWaitList,
            RetentionFinalDisposition,
            RetentionCertifieDisposition,
            RetentionInactivePullList,
            RetentionInactiveRecords,
            RetentionRecordsOnHold,
            RetentionCitations,
            RetentionCitationsWithRetCodes,
            RetentionCodes,
            RetentionCodesWithCitations
        }

        public enum SubmitType
        {
            Purged,
            Archived,
            Destroyed
        }
        public enum LoginUserType
        {
            Single = 0,
            Azure = 1,
            Okta = 2,
        }
        public enum ViewType
        {
            FusionView,
            Favorite,
            GlobalSearch
        }
        public enum BackgroundTaskProcess
        {
            Normal = 1,
            Background,
            ExceedMaxLimit,
            ServiceNotEnabled,
            NoSelection
        }
    }

    public class CollectionsClass
    {
        public static List<string> mcEngineTablesList;
        public static List<string> mcEngineTablesOkayToImportList;
        public static List<string> mcEngineTablesNotNeededList;

        public static List<string> EngineTablesOkayToImportList
        {
            get
            {
                if (mcEngineTablesOkayToImportList is null)
                {
                    mcEngineTablesOkayToImportList = new List<string>();
                }

                if (mcEngineTablesOkayToImportList.Count == 0)
                {
                    mcEngineTablesOkayToImportList.Add("slretentioncodes");
                    mcEngineTablesOkayToImportList.Add("slretentioncitations");
                    mcEngineTablesOkayToImportList.Add("slretentioncitacodes");
                }

                return mcEngineTablesOkayToImportList;
            }

        }

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

        public static bool IsEngineTableOkayToImport(string sTableName)
        {
            int iIndex;
            int iCount = EngineTablesOkayToImportList.Count - 1;

            var loopTo = iCount;
            for (iIndex = 0; iIndex <= loopTo; iIndex++)
            {
                if (EngineTablesOkayToImportList[iIndex].Trim().ToLower().Equals(sTableName.Trim().ToLower()))
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

        public static bool CheckTablesPermission(List<Table> lTableEntities, bool mbMgrGroup, Passport passport, string TablePermission)
        {
            bool bAtLeastOneTablePermission = false;
            //http.Session.Remove("TablesPermission");
            try
            {
                if (!string.IsNullOrEmpty(TablePermission))
                {
                    if (!mbMgrGroup)
                    {
                        foreach (var oTable in lTableEntities)
                        {
                            if (!IsEngineTable(oTable.TableName))
                            {
                                if (passport.CheckPermission(oTable.TableName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Table, (Smead.Security.Permissions.Permission)Enums.PassportPermissions.Configure))
                                {
                                    bAtLeastOneTablePermission = true;
                                    break;
                                }
                            }
                        }
                    }
                    //http.Session.SetString("TablesPermission", bAtLeastOneTablePermission.ToString());
                }
                else
                {
                    if (string.IsNullOrEmpty(TablePermission))
                    {
                        bAtLeastOneTablePermission = false;
                    }
                }
            }
            catch (Exception)
            {

            }

            return bAtLeastOneTablePermission;
        }

        public static int CheckReportsPermission(List<Table> lTableEntities, List<View> lViewEntities, Passport passport, string iCntRpt)
        {
            int iCntRpts = 0;
            //httpContext.Session.Remove("iCntRpts");
            //var iCntRpt = httpContext.Session.GetString("iCntRpts");
            if (!string.IsNullOrEmpty(iCntRpt))
            {
                foreach (var oTable in lTableEntities)
                {
                    if (!IsEngineTable(oTable.TableName))
                    {
                        if (passport.CheckPermission(oTable.TableName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Table, (Smead.Security.Permissions.Permission)Enums.PassportPermissions.View))
                        {
                            var lTableViewList = lViewEntities.Where(x => (x.TableName.Trim().ToLower() ?? "") == (oTable.TableName.Trim().ToLower() ?? ""));
                            foreach (var oView in lTableViewList)
                            {
                                if ((bool)oView.Printable)
                                {
                                    if (passport.CheckPermission(oView.ViewName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Reports, (Smead.Security.Permissions.Permission)Enums.PassportPermissions.Configure))
                                    {
                                        if (passport.CheckPermission(oView.ViewName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Reports, (Smead.Security.Permissions.Permission)Enums.PassportPermissions.View))
                                        {
                                            if (NotSubReport(lTableEntities, lViewEntities, oView, oTable.TableName))
                                            {
                                                iCntRpts = iCntRpts + 1;
                                                //httpContext.Session.SetString("iCntRpts", iCntRpts.ToString());
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (iCntRpts > 0)
                                break;
                        }
                    }
                }
            }
            return Convert.ToInt32(iCntRpts);
            //return Convert.ToInt32(httpContext.Session.GetString("iCntRpts"));
        }

        public static bool CheckViewsPermission(List<View> lViewEntities, bool mbMgrGroup, Passport passport, string TablesPermission, string ViewPermission)
        {
            bool bAtLeastOneViewPermission = false;
            //httpContext.Session.Remove("ViewPermission");
            try
            {
                if (!string.IsNullOrEmpty(ViewPermission))
                {
                    if (!mbMgrGroup)
                    {
                        foreach (var oView in lViewEntities)
                        {
                            if (passport.CheckPermission(oView.ViewName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.View, (Smead.Security.Permissions.Permission)Enums.PassportPermissions.Configure))
                            {
                                bAtLeastOneViewPermission = true;
                                break;
                            }
                        }
                    }

                    //httpContext.Session.SetString("TablesPermission", bAtLeastOneViewPermission.ToString());
                }
                else
                {
                    if (string.IsNullOrEmpty(TablesPermission))
                    {
                        bAtLeastOneViewPermission = false;
                    }
                    else
                    {
                        bAtLeastOneViewPermission = Convert.ToBoolean(TablesPermission); 
                    }

                }
            }
            catch (Exception)
            {

            }

            return bAtLeastOneViewPermission;
        }

        public static bool NotSubReport(List<Table> lTableEntities, List<View> IqViewEntities, View oView, string pTableName)
        {
            bool NotSubReportRet = default;
            var tableEntity = lTableEntities;
            View oTempView;
            var lViewEntities = IqViewEntities;
            var lLoopViewEntities = lViewEntities.Where(x => x.TableName.Trim().ToLower() == pTableName);

            NotSubReportRet = true;

            foreach (var oTable in tableEntity.ToList())
            {
                foreach (var currentOTempView in lLoopViewEntities)
                {
                    oTempView = currentOTempView;
                    if (oTempView.SubViewId == oView.Id)
                    {
                        NotSubReportRet = false;
                        break;
                    }
                }
            }

            oTempView = null;
            return NotSubReportRet;
        }
    }

    public class GridColumns
    {
        public int ColumnSrNo
        {
            get
            {
                return _ColumnSrNo;
            }
            set
            {
                _ColumnSrNo = value;
            }
        }
        private int _ColumnSrNo;
        public int ColumnId
        {
            get
            {
                return _ColumnId;
            }
            set
            {
                _ColumnId = value;
            }
        }
        private int _ColumnId = 0;
        public string ColumnName
        {
            get
            {
                return _ColumnName;
            }
            set
            {
                _ColumnName = value;
            }
        }
        private string _ColumnName;
        public string ColumnDisplayName
        {
            get
            {
                return _ColumnDisplayName;
            }
            set
            {
                _ColumnDisplayName = value;
            }
        }
        private string _ColumnDisplayName;
        public string ColumnDataType
        {
            get
            {
                return _ColumnDataType;
            }
            set
            {
                _ColumnDataType = value;
            }
        }
        private string _ColumnDataType;
        public string ColumnMaxLength
        {
            get
            {
                return _ColumnMaxLength;
            }
            set
            {
                _ColumnMaxLength = value;
            }
        }
        private string _ColumnMaxLength;
        public bool IsPk
        {
            get
            {
                return _IsPk;
            }
            set
            {
                _IsPk = value;
            }
        }
        private bool _IsPk;
        public bool AutoInc
        {
            get
            {
                return _AutoInc;
            }
            set
            {
                _AutoInc = value;
            }
        }
        private bool _AutoInc;
        public bool IsNull
        {
            get
            {
                return _IsNull;
            }
            set
            {
                _IsNull = value;
            }
        }
        private bool _IsNull;
        public bool ReadOnlye
        {
            get
            {
                return _ReadOnlye;
            }
            set
            {
                _ReadOnlye = value;
            }
        }
        private bool _ReadOnlye;
    }

    public class CoulmnSchemaInfo
    {
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set;}
        public string DATA_TYPE { get; set;}
        public int CHARACTER_MAXIMUM_LENGTH { get; set;}
        public string IS_NULLABLE { get; set;}
        public bool IsAutoIncrement { get; set;}
    }

    public sealed class Keys
    {
        public static string _connMARSbuild(string conn)
        {
            if (!string.IsNullOrEmpty(conn))
            {
                if (!conn.Contains("MultipleActiveResultSets"))
                {
                    conn = conn + ";MultipleActiveResultSets=True";
                }
            }
            return conn;
        }
    }

}
