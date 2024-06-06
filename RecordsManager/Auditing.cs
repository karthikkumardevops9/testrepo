using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using Smead.Security;

namespace MSRecordsEngine.RecordsManager
{

    public class Auditing
    {
        public static void AuditConfidentialDataAccess(string tableName, string tableId, string domainName, string computerName, string MacAddress, string remoteHost, Passport Passport)
        {
            AuditConfidentialDataAccess(tableName, tableId, domainName, computerName, MacAddress, remoteHost, "Web Access", Passport, DateTime.Now);
        }
        public static void AuditConfidentialDataAccess(string tableName, string tableId, string domainName, string computerName, string MacAddress, string remoteHost, string ModuleName, Passport Passport, DateTime AccessDateTime)
        {
        
            // Public Shared Sub AuditConfidentialDataAccess(tableName As String, tableId As String, remoteHost As String, ModuleName As String, Passport As Passport, AccessDateTime As Date)
            tableId = Navigation.PrepPad(tableName, tableId, Passport);
            var auditConf = new Smead.Security.SecurityDataSetTableAdapters.SLAuditConfDataTableAdapter();
            auditConf.Connection = Passport.StaticConnection();

            auditConf.InsertAuditConfData(tableName, tableId, Passport.UserName, string.Empty, domainName, computerName, MacAddress, remoteHost, AccessDateTime, ModuleName);
            var dt = WalkUpRelationshipsForAuditConf(tableName, Passport.StaticConnection());
            if (dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                string sql = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}] = @tableId", Navigation.MakeSimpleField(row["LowerTableFieldName"].ToString()), tableName, Navigation.MakeSimpleField(row["IdFieldName"].ToString()));

                using (var cmd = new SqlCommand(sql, Passport.StaticConnection()))
                {
                    cmd.Parameters.AddWithValue("@tableId", tableId);

                    try
                    {
                        tableId = cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        tableId = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(tableId))
                {
                    tableName = row["UpperTableName"].ToString();
                    AuditConfidentialDataAccess(tableName, tableId, domainName, computerName, MacAddress, remoteHost, ModuleName, Passport, AccessDateTime);
                }
            }
        }

        public static void AuditUpdates(AuditBaseModel model, string action, Passport Passport)
        {
            model.Action = action;
            AuditUpdates(model, Passport);
        }

        public static void AuditUpdates(AuditBaseModel model, Passport Passport)
        {
            if (model.BeforeData.Length < 3 && model.AfterData.Length < 3)
                return;
            if (!Navigation.CBoolean(Navigation.GetTableInfo(model.TableName, Passport)["AuditUpdate"]))
                return;
            model.TableId = Navigation.PrepPad(model.TableName, model.TableId, Passport.StaticConnection());
            // InsertUpdateAudit is not allowing changes due to an issue with the connection string, doing t his manually
            var auditUpdate = new Smead.Security.SecurityDataSetTableAdapters.SLAuditUpdatesTableAdapter();
            auditUpdate.Connection = Passport.StaticConnection();

            int auditID = Conversions.ToInteger(auditUpdate.InsertUpdateAudit(model.TableName, model.TableId, Passport.UserName, string.Empty, Passport.DomainName, Passport.ComputerName, Passport.MacAddress, model.ClientIpAddress, model.ModuleName, model.Action, model.BeforeData, model.AfterData, DateTime.Now, model.ActionTypeAsInteger));
            AuditUpdateChildren(auditID, model.TableName, model.TableId, Passport);
        }

        private static void AuditUpdateChildren(int auditUpdateId, string tableName, string tableId, Passport Passport)
        {
            string paddedTableid = Navigation.PrepPad(tableName, tableId, Passport);
            InsertAuditUpdateChildren(auditUpdateId, tableName, paddedTableid, Passport.StaticConnection());

            var dt = WalkUpRelationshipsForAuditUpdates(tableName, Passport.StaticConnection());
            if (dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                string sql = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}] = @tableId", Navigation.MakeSimpleField(row["LowerTableFieldName"].ToString()), tableName, Navigation.MakeSimpleField(row["IdFieldName"].ToString()));

                using (var cmd = new SqlCommand(sql, Passport.StaticConnection()))
                {
                    cmd.Parameters.AddWithValue("@tableId", tableId);

                    try
                    {
                        tableId = cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        tableId = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(tableId))
                {
                    tableName = row["UpperTableName"].ToString();
                    AuditUpdateChildren(auditUpdateId, tableName, tableId, Passport);
                }
            }
        }

        private static void InsertAuditUpdateChildren(int auditUpdateId, string tableName, string tableId, SqlConnection conn)
        {
            string sql = "INSERT INTO SLAuditUpdChildren (SLAuditUpdatesID, TableName, TableId) values (@AuditID, @TableName, @TableID)";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AuditID", auditUpdateId);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                cmd.Parameters.AddWithValue("@TableID", tableId);
                cmd.ExecuteNonQuery();
            }
        }

        private static DataTable WalkUpRelationshipsForAuditUpdates(string tableName, SqlConnection conn)
        {
            var returnTable = new DataTable();
            string sql = "SELECT r.*, t.IdFieldName FROM Relationships r, Tables t WHERE r.LowerTableName = @TableName AND r.LowerTableName = t.TableName AND r.UpperTableName IN (SELECT TableName from Tables WHERE AuditUpdate = 1)";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(returnTable);
                }
            }

            return returnTable;
        }

        private static DataTable WalkUpRelationshipsForAuditConf(string tableName, SqlConnection conn)
        {
            var returnTable = new DataTable();
            string sql = "SELECT r.*, t.IdFieldName FROM Relationships r, Tables t WHERE r.LowerTableName = @TableName AND r.LowerTableName = t.TableName AND r.UpperTableName IN (SELECT TableName from Tables WHERE AuditConfidentialData = 1)";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(returnTable);
                }
            }

            return returnTable;
        }

        public static string GetOldRecordDataForDelete(DataRow oTables, string sTableId, Passport passport)
        {
            using (var conn = passport.StaticConnection())
            {
                return GetOldRecordDataForDelete(oTables, sTableId, conn);
            }
        }

        public static string GetOldRecordDataForDelete(DataRow oTables, string sTableId, SqlConnection conn)
        {
            try
            {
                if (!Navigation.CBoolean(oTables["AuditUpdate"]))
                    return string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }

            var sb = new System.Text.StringBuilder();
            string sql = string.Format("SELECT * FROM [{0}] WHERE [{1}] = @Id", oTables["TableName"].ToString(), Navigation.MakeSimpleField(oTables["IdFieldName"].ToString()));

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", sTableId);

                using (var table = new DataWithCursor())
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(table);
                        if (table.Rows.Count == 0)
                            return string.Empty;

                        foreach (DataColumn field in table.Columns)
                        {
                            if (!(table.CurrentRow[field.Ordinal] is DBNull) & !ReferenceEquals(field.DataType, typeof(byte[])))
                            {
                                if (!string.IsNullOrEmpty(table.CurrentRow[field.Ordinal].ToString()))
                                {
                                    sb.AppendLine(string.Format("{0}: {1}", field.ColumnName, table.CurrentRow[field.Ordinal].ToString()));
                                }
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(sb.ToString()))
                return string.Empty;
            return sb.ToString().Substring(0, sb.ToString().Length - 2);
        }

        public new List<EnumModel> GetAuditTypeList()
        {

            var WebAccess = Enum.GetValues(typeof(AuditType.WebAccessActionType)).Cast<AuditType.WebAccessActionType>().ToList();
            var LinkScriptAudit = Enum.GetValues(typeof(AuditType.LinkScriptActionType)).Cast<AuditType.LinkScriptActionType>().ToList();
            var AttachmentViewer = Enum.GetValues(typeof(AuditType.AttachmentViewerActionType)).Cast<AuditType.AttachmentViewerActionType>().ToList();
            var Desktop = Enum.GetValues(typeof(AuditType.DesktopActionType)).Cast<AuditType.DesktopActionType>().ToList();
            var ImportWizard = Enum.GetValues(typeof(AuditType.ImportWizardActionType)).Cast<AuditType.ImportWizardActionType>().ToList();
            var Api = Enum.GetValues(typeof(AuditType.ApiActionType)).Cast<AuditType.ApiActionType>().ToList();
            var enumM = new List<EnumModel>();
            foreach (AuditType.WebAccessActionType en in WebAccess)
            {
                var model = new EnumModel();
                model.Name = "WebAccess-" + en.ToString();
                model.Value = (int)en;
                enumM.Add(model);
            }
            foreach (AuditType.LinkScriptActionType en in LinkScriptAudit)
            {
                var model = new EnumModel();
                model.Name = "LinkScriptAudit-" + en.ToString();
                model.Value = (int)en;
                enumM.Add(model);
            }
            foreach (AuditType.AttachmentViewerActionType en in AttachmentViewer)
            {
                var model = new EnumModel();
                model.Name = "AttachmentViewer-" + en.ToString();
                model.Value = (int)en;
                enumM.Add(model);
            }
            foreach (AuditType.DesktopActionType en in Desktop)
            {
                var model = new EnumModel();
                model.Name = "Desktop-" + en.ToString();
                model.Value = (int)en;
                enumM.Add(model);
            }
            foreach (AuditType.ImportWizardActionType en in ImportWizard)
            {
                var model = new EnumModel();
                model.Name = "ImportWizard-" + en.ToString();
                model.Value = (int)en;
                enumM.Add(model);
            }
            foreach (AuditType.ApiActionType en in Api)
            {
                var model = new EnumModel();
                model.Name = "Api-" + en.ToString();
                model.Value = (int)en;
                enumM.Add(model);
            }
            return enumM;
        }
    }

    public class EnumModel
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }

    // AUDIT TYPE CLASSES AND ENUMERATION
    public class AuditType
    {
        public static WebAccessAudit WebAccess { get; set; } = new WebAccessAudit();
        public static LinkScriptAudit LinkScriptAudit = new LinkScriptAudit();
        public static AttachmentViewerAudit AttachmentViewer = new AttachmentViewerAudit();
        public static DesktopAudit Desktop = new DesktopAudit();
        public static ImportWizardAudit ImportWizard = new ImportWizardAudit();
        public static ApiAudit Api = new ApiAudit();
        // The AuditType enums are duplicated in SRME.SLAuditUpdates.AuditType enum so we don't need to register another assembly for COM 
        // ALSO they are partially duplicated in Smead.RecordsManagementCS.Imaging.AuditType enum to avoid circular references
        // IMPORTANT to keep in sync.  RVW 10/27/2021
        public enum WebAccessActionType
        {
            AddRecord = 101,
            DeleteRecord,
            UpdateRecord,
            DeleteChildren = 104,
            OfficialRecordReconciled,
            RenameAttachment = 106,
            MoveRecord,
            MoveAttachment
        }
        public enum LinkScriptActionType
        {
            AddRecord = 201,
            DeleteRecord,
            UpdateRecord,
            RenameAttachment = 206
        }
        public enum AttachmentViewerActionType
        {
            AddAttachment = 301,
            AddVersion,
            AddPage,
            RenameAttachment = 306,
            MoveAttachment,
            AttachOrphan,
            DeleteAttachment,
            DeleteVersion,
            DeletePage,
            DeleteOrphan,
            CheckIn,
            CheckOut,
            UndoCheckOut,
            MarkOfficial,
            RotatePage,
            EditAnnotations,
            RotateAnnotations
        }
        public enum DesktopActionType
        {
            AddRecord = 401,
            DeleteRecord,
            UpdateRecord,
            DeleteChildren = 404,
            OfficialRecordReconciled,
            MoveRecord,
            DestroyRecord,
            UpdateRequest,
            DeleteRequest,
            ArchiveRequest
        }
        public enum ImportWizardActionType
        {
            AddRecord = 501,
            DeleteRecord,
            UpdateRecord
        }
        public enum ApiActionType
        {
            AddRecord = 601,
            DeleteRecord,
            UpdateRecord
        }
    }


    public abstract class AuditBaseModel
    {
        internal int _actionTypeAsInteger;
        public int AttachmentNumber = 0;
        public string TableName = string.Empty;
        public string TableId = string.Empty;
        public string ClientIpAddress = string.Empty;
        public string BeforeData = string.Empty;
        public string AfterData = string.Empty;
        public string ModuleName = string.Empty;
        public string Action = string.Empty;
        internal readonly string ActionNotSet = "Action not set";

        public int ActionTypeAsInteger
        {
            get
            {
                return _actionTypeAsInteger;
            }
        }


        public void SetAction(AuditType.WebAccessActionType actType)
        {
            switch (actType)
            {
                case AuditType.WebAccessActionType.AddRecord:
                    {
                        Action = "Add Record";
                        break;
                    }
                case AuditType.WebAccessActionType.UpdateRecord:
                    {
                        Action = "Update Record";
                        break;
                    }
                case AuditType.WebAccessActionType.DeleteRecord:
                    {
                        Action = "Delete Record";
                        break;
                    }
                case AuditType.WebAccessActionType.DeleteChildren:
                    {
                        Action = "TAB FusionRMS Delete All Children";
                        break;
                    }

                default:
                    {
                        Action = ActionNotSet;
                        break;
                    }
            }

            _actionTypeAsInteger = (int)actType;
        }
    }
    public class WebAccessAudit : AuditBaseModel
    {
        private AuditType.WebAccessActionType _actionType;
        public AuditType.WebAccessActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
                SetAction(_actionType);
            }
        }

        public WebAccessAudit()
        {
            ModuleName = "Web Access";
        }

        public new void SetAction(AuditType.WebAccessActionType actType)
        {
            base.SetAction(actType);
        }
    }
    public class LinkScriptAudit : AuditBaseModel
    {
        private AuditType.LinkScriptActionType _actionType;
        public AuditType.LinkScriptActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
                SetAction(_actionType);
            }
        }

        public LinkScriptAudit()
        {
            ModuleName = "LinkScript";
        }

        public void SetAction(AuditType.LinkScriptActionType actType)
        {
            switch (actType)
            {
                case AuditType.LinkScriptActionType.AddRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.AddRecord);
                        break;
                    }
                case AuditType.LinkScriptActionType.UpdateRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.UpdateRecord);
                        break;
                    }
                case AuditType.LinkScriptActionType.DeleteRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.DeleteRecord);
                        break;
                    }
                case AuditType.LinkScriptActionType.RenameAttachment:
                    {
                        Action = string.Concat("Renamed Attachment: ", AttachmentNumber.ToString());
                        break;
                    }

                default:
                    {
                        Action = ActionNotSet;
                        break;
                    }
            }

            _actionTypeAsInteger = (int)actType;
        }
    }
    public class AttachmentViewerAudit : AuditBaseModel
    {
        private AuditType.AttachmentViewerActionType _actionType;
        public AuditType.AttachmentViewerActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
                SetAction(_actionType);
            }
        }

        public AttachmentViewerAudit()
        {
            ModuleName = "Attachment Viewer";
        }

        public void SetAction(AuditType.AttachmentViewerActionType actType)
        {
            switch (actType)
            {
                case AuditType.AttachmentViewerActionType.AddAttachment:
                    {
                        Action = "Added Attachment";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.AddVersion:
                    {
                        Action = "Added Version";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.AddPage:
                    {
                        Action = "Added Page(s)";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.DeleteAttachment:
                    {
                        Action = "Deleted Attachment";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.DeletePage:
                    {
                        Action = "Deleted Page";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.DeleteOrphan:
                    {
                        Action = "Deleted Orphan";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.CheckIn:
                    {
                        Action = "CheckIn";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.CheckOut:
                    {
                        Action = "CheckOut";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.UndoCheckOut:
                    {
                        Action = "Undo Check Out";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.RenameAttachment:
                    {
                        Action = string.Concat("Renamed Attachment: ", AttachmentNumber.ToString());
                        break;
                    }
                case AuditType.AttachmentViewerActionType.RotatePage:
                    {
                        Action = "Rotated Page";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.EditAnnotations:
                    {
                        Action = "Edited Annotations";
                        break;
                    }
                case AuditType.AttachmentViewerActionType.RotateAnnotations:
                    {
                        Action = "Rotated All Annotations";
                        break;
                    }

                default:
                    {
                        Action = ActionNotSet;
                        break;
                    }
            }

            _actionTypeAsInteger = (int)actType;
        }
    }
    public class DesktopAudit : AuditBaseModel
    {
        private AuditType.DesktopActionType _actionType;
        public AuditType.DesktopActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
                SetAction(_actionType);
            }
        }

        public DesktopAudit()
        {
            ModuleName = "TAB FusionRMS";
        }

        public void SetAction(AuditType.DesktopActionType actType)
        {
            switch (actType)
            {
                case AuditType.DesktopActionType.AddRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.AddRecord);
                        break;
                    }
                case AuditType.DesktopActionType.UpdateRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.UpdateRecord);
                        break;
                    }
                case AuditType.DesktopActionType.DeleteRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.DeleteRecord);
                        break;
                    }
                case AuditType.DesktopActionType.DeleteChildren:
                    {
                        SetAction(AuditType.WebAccessActionType.DeleteChildren);
                        break;
                    }

                default:
                    {
                        Action = ActionNotSet;
                        break;
                    }
            }

            _actionTypeAsInteger = (int)actType;
        }
    }
    public class ImportWizardAudit : AuditBaseModel
    {
        private AuditType.ImportWizardActionType _actionType;
        public AuditType.ImportWizardActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
                SetAction(_actionType);
            }
        }

        public ImportWizardAudit()
        {
            ModuleName = "Import Wizard";
        }

        public void SetAction(AuditType.ImportWizardActionType actType)
        {
            switch (actType)
            {
                case AuditType.ImportWizardActionType.AddRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.AddRecord);
                        break;
                    }
                case AuditType.ImportWizardActionType.UpdateRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.UpdateRecord);
                        break;
                    }
                case AuditType.ImportWizardActionType.DeleteRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.DeleteRecord);
                        break;
                    }

                default:
                    {
                        Action = ActionNotSet;
                        break;
                    }
            }

            _actionTypeAsInteger = (int)actType;
        }
    }
    public class ApiAudit : AuditBaseModel
    {
        private AuditType.ApiActionType _actionType;
        public AuditType.ApiActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
                SetAction(_actionType);
            }
        }

        public ApiAudit()
        {
            ModuleName = "API";
        }

        public void SetAction(AuditType.ApiActionType actType)
        {
            switch (actType)
            {
                case AuditType.ApiActionType.AddRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.AddRecord);
                        break;
                    }
                case AuditType.ApiActionType.UpdateRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.UpdateRecord);
                        break;
                    }
                case AuditType.ApiActionType.DeleteRecord:
                    {
                        SetAction(AuditType.WebAccessActionType.DeleteRecord);
                        break;
                    }

                default:
                    {
                        Action = ActionNotSet;
                        break;
                    }
            }

            _actionTypeAsInteger = (int)actType;
        }
    }
}