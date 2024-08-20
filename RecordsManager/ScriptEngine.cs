using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using static System.Globalization.CultureInfo;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Smead.Security;
using System.Threading.Tasks;

namespace MSRecordsEngine.RecordsManager
{

    public class ScriptReturn
    {
        public ScriptReturn() { }
        internal ScriptReturn(bool Successful)
        {
            _successful = Successful;
            _gridRefresh = false;
        }

        internal ScriptReturn(bool Successful, bool gridRefresh)
        {
            _successful = Successful;
            _gridRefresh = gridRefresh;
        }

        public ScriptReturn(bool Successful, string ReturnMessage, string NewTableId, bool gridRefresh)
        {
            _successful = Successful;
            _returnMessage = ReturnMessage;
            _newTableId = NewTableId;
            _gridRefresh = gridRefresh;
        }

        public ScriptReturn(bool Successful, string ReturnMessage, string MessageType, string NewTableId, bool gridRefresh)
        {
            _successful = Successful;
            _returnMessage = ReturnMessage;
            _messageType = MessageType;
            _newTableId = NewTableId;
            _gridRefresh = gridRefresh;
        }

        public ScriptReturn(bool Successful, string ReturnMessage, string MessageType, string NewTableId, DateTime DueBackDate, bool gridRefresh)
        {
            _successful = Successful;
            _returnMessage = ReturnMessage;
            _messageType = MessageType;
            _newTableId = NewTableId;
            _dueBackDate = DueBackDate;
            _gridRefresh = gridRefresh;
        }

        public ScriptReturn(bool Successful, InternalEngine engine)
        {
            _successful = Successful;
            _returnMessage = engine.ReturnMessage;
            _messageType = engine.MessageType;
            _newTableId = engine.NewRecordId;
            _engine = engine;
            _scriptControlDictionary = engine.ScriptControlDictionary;
            _currentScriptSequence = engine.CurrentScriptSequence;
            _gridRefresh = engine.GridRefresh;
        }

        public InternalEngine Engine
        {
            get
            {
                return _engine;
            }
            set
            {
                _engine = value;
            }
        }
        private InternalEngine _engine;

        public bool GridRefresh
        {
            get
            {
                return _gridRefresh;
            }
        }
        private bool _gridRefresh;

        public bool Successful
        {
            get
            {
                return _successful;
            }
        }
        private bool _successful;

        public double CurrentScriptSequence
        {
            get
            {
                return _currentScriptSequence;
            }
        }
        private double _currentScriptSequence;

        public Dictionary<string, ScriptControls> ScriptControlDictionary
        {
            get
            {
                return _scriptControlDictionary;
            }
        }
        private Dictionary<string, ScriptControls> _scriptControlDictionary;

        public string ReturnMessage
        {
            get
            {
                return _returnMessage;
            }
        }
        private string _returnMessage = string.Empty;

        public string MessageType
        {
            get
            {
                return _messageType;
            }
        }
        private string _messageType = "s";

        public string NewTableId
        {
            get
            {
                return _newTableId;
            }
        }
        private string _newTableId = string.Empty;

        public DateTime DueBackDate
        {
            get
            {
                return _dueBackDate;
            }
        }
        private DateTime _dueBackDate;
    }

    public class ScriptEngine
    {
        public static string ProductName = "TAB FusionRMS";

        public enum TrackingAdditionalFieldTypes
        {
            Selection,
            Suggestion,
            Text
        }

        public enum PageActionTypes
        {
            KeepAllPages,
            DeleteFirstPage,
            DeleteAllButFirstPage
        }
        // Types to describe where linkscript is being called from...
        public enum CallerTypes
        {
            Empty = -0x1,
            AnyCaller = 0x0,
            LibrarianViewLinkScriptButton = 0x1,
            ScannerNewDocument = 0x2,
            PCFilesLoad = 0x4,
            ImportCOLDRowChange = 0x8,
            ManualIndexingLinkButton = 0x10,
            TrackingAfterDestinationScanned = 0x20,
            TrackingAfterDestinationAccepted = 0x40,
            TrackingAfterObjectScanned = 0x80,
            TrackingAfterObjectAccepted = 0x100,
            TrackingAfterTrackingComplete = 0x200,
            ImportTrackingAfterDestinationScanned = 0x400,
            ImportTrackingAfterDestinationAccepted = 0x800,
            ImportTrackingAfterObjectScanned = 0x1000,
            ImportTrackingAfterObjectAccepted = 0x2000,
            ImportTrackingAfterTrackingComplete = 0x4000,
            PrintBarCodeLabelAfterPrint = 0x8000,
            FeatureInstallUninstall = 0x10000,
            LibrarianBeforeAdd = 0x20000,
            LibrarianBeforeEdit = 0x40000,
            LibrarianBeforeDelete = 0x80000,
            LibrarianAfterAdd = 0x100000,
            LibrarianAfterEdit = 0x200000,
            LibrarianAfterDelete = 0x400000,
            ImportBeforeAdd = 0x800000,
            ImportBeforeEdit = 0x1000000,
            ImportAfterAdd = 0x2000000,
            ImportAfterEdit = 0x4000000,
            FromOfficeConnectivity = 0x8000000
            // &H10000000
            // &H20000000
            // &H40000000 - this the largest value that fits into a 32bit integer.
        }

        public enum UITypes
        {
            NoUI,
            SimpleUI,
            ComplexUI,
            CompleteUI
        }

        public static ScriptReturn RunScriptAfterDestinationScanned(string TableName, string TableId, Passport passport, SqlConnection conn)
        {
            var systemRow = GetSystemRow(conn);
            if (systemRow is null)
                return new ScriptReturn(true);
            return ScriptEngine.RunScript(systemRow.LSAfterDestinationScanned, TableName, TableId, 0, passport, conn, CallerTypes.TrackingAfterDestinationScanned);
        }

        public static ScriptReturn RunScriptAfterDestinationAccepted(string TableName, string TableId, string DestinationTableName, string DestinationTableId, Passport passport, SqlConnection conn)
        {
            var systemRow = GetSystemRow(conn);
            if (systemRow is null)
                return new ScriptReturn(true);
            return ScriptEngine.RunScript(systemRow.LSAfterDestinationAccepted, TableName, TableId, 0, passport, conn, CallerTypes.TrackingAfterDestinationAccepted);
        }

        public static ScriptReturn RunScriptAfterObjectScanned(string TableName, string TableId, string DestinationTableName, string DestinationTableId, Passport passport, SqlConnection conn)
        {
            var systemRow = GetSystemRow(conn);
            if (systemRow is null)
                return new ScriptReturn(true);
            return ScriptEngine.RunScript(systemRow.LSAfterObjectScanned, TableName, TableId, 0, passport, conn, CallerTypes.TrackingAfterObjectScanned);
        }

        public static ScriptReturn RunScriptAfterObjectAccepted(string TableName, string TableId, string DestinationTableName, string DestinationTableId, Passport passport, SqlConnection conn)
        {
            var systemRow = GetSystemRow(conn);
            if (systemRow is null)
                return new ScriptReturn(true);
            return ScriptEngine.RunScript(systemRow.LSAfterObjectAccepted, TableName, TableId, 0, passport, conn, CallerTypes.TrackingAfterObjectAccepted);
        }

        public static ScriptReturn RunScriptAfterTrackingComplete(string TableName, string TableId, string DestinationTableName, string DestinationTableId, string AdditionalField, string AdditionalMemoField, Passport passport, SqlConnection conn)
        {
            var systemRow = GetSystemRow(conn);
            if (systemRow is null)
                return new ScriptReturn(true);
            return ScriptEngine.RunScript(systemRow.LSAfterTrackingComplete, TableName, TableId, 0, passport, conn, CallerTypes.TrackingAfterTrackingComplete, null, TableName, TableId, DestinationTableName, DestinationTableId, AdditionalField, AdditionalMemoField);
        }

        public static ScriptReturn RunScriptAfterTrackingComplete(string TableName, string TableId, string DestinationTableName, string DestinationTableId, string AdditionalField, string AdditionalMemoField, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptAfterTrackingComplete(TableName, TableId, DestinationTableName, DestinationTableId, AdditionalField, AdditionalMemoField, passport, conn);
            }
        }

        public static ScriptReturn RunScriptBeforeAdd(string TableName, Passport passport, SqlConnection conn)
        {
            var tableRow = GetTableRow(TableName, conn);
            if (tableRow is null || string.IsNullOrEmpty(tableRow["LSBeforeAddRecord"].ToString()))
                return new ScriptReturn(true);
            return RunScript(tableRow["LSBeforeAddRecord"].ToString(), TableName, string.Empty, 0, passport, conn, CallerTypes.LibrarianBeforeAdd);
        }

        public static ScriptReturn RunScriptBeforeAdd(string TableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptBeforeAdd(TableName, passport, conn);
            }
        }

        public static ScriptReturn RunScriptAfterAdd(string TableName, string TableId, Passport passport, SqlConnection conn)
        {
            var tableRow = GetTableRow(TableName, conn);
            if (tableRow is null || string.IsNullOrEmpty(tableRow["LSAfterAddRecord"].ToString()))
                return new ScriptReturn(true);
            return RunScript(tableRow["LSAfterAddRecord"].ToString(), TableName, TableId, 0, passport, conn, CallerTypes.LibrarianAfterAdd);
        }

        public static ScriptReturn RunScriptAfterAdd(string TableName, string TableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptAfterAdd(TableName, TableId, passport, conn);
            }
        }

        public static ScriptReturn RunScriptBeforeEdit(string TableName, string TableId, Passport passport, SqlConnection conn)
        {
            var tableRow = GetTableRow(TableName, conn);
            if (tableRow is null || string.IsNullOrEmpty(tableRow["LSBeforeEditRecord"].ToString()))
                return new ScriptReturn(true);
            return RunScript(tableRow["LSBeforeEditRecord"].ToString(), TableName, TableId, 0, passport, conn, CallerTypes.LibrarianBeforeEdit);
        }

        public static ScriptReturn RunScriptBeforeEdit(string TableName, string TableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptBeforeEdit(TableName, TableId, passport, conn);
            }
        }

        public static ScriptReturn RunScriptAfterEdit(string TableName, string TableId, Passport passport, SqlConnection conn)
        {
            var tableRow = GetTableRow(TableName, conn);
            if (tableRow is null || string.IsNullOrEmpty(tableRow["LSAfterEditRecord"].ToString()))
                return new ScriptReturn(true);
            return RunScript(tableRow["LSAfterEditRecord"].ToString(), TableName, TableId, 0, passport, conn, CallerTypes.LibrarianAfterEdit);
        }

        public static ScriptReturn RunScriptAfterEdit(string TableName, string TableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptAfterEdit(TableName, TableId, passport, conn);
            }
        }

        public static ScriptReturn RunScriptBeforeDelete(string TableName, string TableId, Passport passport, SqlConnection conn)
        {
            var tableRow = GetTableRow(TableName, conn);
            if (tableRow is null || string.IsNullOrEmpty(tableRow["LSBeforeDeleteRecord"].ToString()))
                return new ScriptReturn(true);
            return RunScript(tableRow["LSBeforeDeleteRecord"].ToString(), TableName, TableId, 0, passport, conn, CallerTypes.LibrarianBeforeDelete);
        }

        public static ScriptReturn RunScriptBeforeDelete(string TableName, string TableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptBeforeDelete(TableName, TableId, passport, conn);
            }
        }

        public static ScriptReturn RunScriptAfterDelete(string TableName, string TableId, Passport passport, SqlConnection conn)
        {
            var tableRow = GetTableRow(TableName, conn);
            if (tableRow is null || string.IsNullOrEmpty(tableRow["LSAfterDeleteRecord"].ToString()))
                return new ScriptReturn(true);
            return RunScript(tableRow["LSAfterDeleteRecord"].ToString(), TableName, TableId, 0, passport, conn, CallerTypes.LibrarianAfterDelete);
        }

        public static ScriptReturn RunScriptAfterDelete(string TableName, string TableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptAfterDelete(TableName, TableId, passport, conn);
            }
        }

        public static ScriptReturn RunScriptWorkFlow(string WorkFlow, string TableName, string TableId, int ViewId, Passport passport, SqlConnection conn, string[] selectedRowIds = null)
        {
            var tableRow = GetTableRow(TableName, conn);
            if (tableRow is null)
                return new ScriptReturn(true);

            bool successful = true;
            var engine = GetEngine(true, passport);
            if (UITypeIsValid(engine, WorkFlow, conn))
                successful = RunScript(ref engine, WorkFlow, TableName, TableId, ViewId, passport, CallerTypes.AnyCaller, ref selectedRowIds, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, conn);
            return new ScriptReturn(successful, engine);
        }

        public static ScriptReturn RunScriptWorkFlow(string WorkFlow, string TableName, string TableId, int ViewId, Passport passport, string[] selectedRowIds = null)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptWorkFlow(WorkFlow, TableName, TableId, ViewId, passport, conn, selectedRowIds);
            }
        }
        public static async Task<ScriptReturn> RunScriptWorkFlowAsync(string WorkFlow, string TableName, string TableId, int ViewId, Passport passport, string[] selectedRowIds = null)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                return RunScriptWorkFlow(WorkFlow, TableName, TableId, ViewId, passport, conn, selectedRowIds);
            }
        }

        public static ScriptReturn RunScriptHeader(string LinkScriptHeader, Passport passport, SqlConnection conn)
        {
            return RunScript(LinkScriptHeader, string.Empty, string.Empty, 0, passport, conn, CallerTypes.PCFilesLoad);
        }

        public static ScriptReturn RunScriptHeader(string LinkScriptHeader, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RunScriptHeader(LinkScriptHeader, passport, conn);
            }
        }

        internal static RecordsManage.SystemRow GetSystemRow(SqlConnection conn)
        {
            try
            {
                using (var da = new SqlDataAdapter("SELECT TOP 1 * FROM [System]", conn))
                {
                    var dt = new RecordsManage.SystemDataTable();
                    da.Fill(dt);
                    return (RecordsManage.SystemRow)dt.Rows[0];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        internal static RecordsManage.SystemRow GetSystemRow(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetSystemRow(conn);
            }
        }

        internal static DataRow GetTableRow(string tableName, SqlConnection conn)
        {
            try
            {
                return Navigation.GetTableInfo(tableName, conn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        internal static DataRow GetTableRow(string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTableRow(tableName, conn);
            }
        }

        private static RecordsManage.LinkScriptHeaderDataTable GetLinkscriptHeader(string scriptName, SqlConnection conn)
        {
            try
            {
                using (var scriptHeader = new RecordsManageTableAdapters.LinkScriptHeaderTableAdapter())
                {
                    scriptHeader.Connection = conn;
                    return scriptHeader.GetData(scriptName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static bool UITypeIsValid(InternalEngine engine, string scriptName, SqlConnection conn)
        {
            var linkscriptHeader = GetLinkscriptHeader(scriptName, conn);
            if (linkscriptHeader is null || linkscriptHeader.Rows.Count == 0)
                return true;
            var headerRow = linkscriptHeader[0];
            // engine.Caller = DirectCast(headerRow.CallingType, CallerTypes) 'HeyReggie (search for CallerTypes.Empty)
            return headerRow.UIType <= (int)engine.UIType;
        }

        private static InternalEngine GetEngine(bool gridRefresh, Passport passport)
        {
            return new InternalEngine(passport, true, gridRefresh, UITypes.ComplexUI); // .NoUI)
        }

        public static ScriptReturn RunScript(string scriptName, string tableName, string tableId, int viewId, Passport passport, SqlConnection conn, CallerTypes callerType, string[] selectedRowIds = null, string ObjectTableName = "", string ObjectID = "", string DestinationTableName = "", string DestinationID = "", string AdditionalField = "", string AdditionalMemoField = "")
        {
            bool successful = true;
            var engine = GetEngine(false, passport);
            if (UITypeIsValid(engine, scriptName, conn))
                successful = RunScript(ref engine, scriptName, tableName, tableId, viewId, passport, callerType, ref selectedRowIds, ObjectTableName, ObjectID, DestinationTableName, DestinationID, AdditionalField, AdditionalMemoField, conn);
            return new ScriptReturn(successful, engine);
        }

        private static ScriptReturn RunScript(string scriptName, string tableName, string tableId, string destinationTableName, string destinationTableId, Passport passport, SqlConnection conn, CallerTypes callerType)
        {
            bool successful = true;
            var engine = GetEngine(false, passport);
            if (UITypeIsValid(engine, scriptName, conn))
                successful = RunScript(ref engine, scriptName, tableName, tableId, destinationTableName, destinationTableId, passport, callerType, conn);
            return new ScriptReturn(successful, engine.ReturnMessage, engine.MessageType, engine.NewRecordId, engine.BCDueBack, engine.GridRefresh);
        }

        public static bool RunScript(ref InternalEngine engine, string scriptName, string tableName, string tableId, int viewId, Passport passport, CallerTypes callerType, ref string[] selectedRowIds, string ObjectTableName = "", string ObjectID = "", string DestinationTableName = "", string DestinationID = "")
        {
            using (var conn = passport.Connection())
            {
                return RunScript(ref engine, scriptName, tableName, tableId, viewId, passport, callerType, ref selectedRowIds, ObjectTableName, ObjectID, DestinationTableName, DestinationID, string.Empty, string.Empty, conn);
            }
        }

        private static string GetSQLDatePattern(SqlConnection conn)
        {
            return "MM/dd/yyyy"; // default to MM/dd/yyyy for now  HeyReggie 06/17/2022
                                 // not sure if retrieving from SQL is the best way to handle "temporarily resetting" short date pattern
                                 // (NOTE: dotNET may just prefer english - MM/dd/yyyy)
                                 // Dim value As String

            // Using cmd As New SqlCommand("SELECT date_format FROM sys.dm_exec_sessions WHERE session_id = @@SPID", conn)
            // Try
            // value = cmd.ExecuteScalar.ToString.ToLower
            // Catch ex As Exception
            // value = "mdy"
            // End Try
            // End Using

            // Select Case value
            // Case "ymd"
            // Return "yyyy/MM/dd"
            // Case "ydm"
            // Return "yyyy/dd/MM"
            // Case "myd"
            // Return "MM/yyyy/dd"
            // Case "dym"
            // Return "dd/yyyy/MM"
            // Case "dmy"
            // Return "dd/MM/yyyy"
            // Case Else
            // Return "MM/dd/yyyy"
            // End Select
        }

        private static bool RunScript(ref InternalEngine engine, string scriptName, string tableName, string tableId, int viewId, Passport passport, CallerTypes callerType, ref string[] selectedRowIds, string ObjectTableName, string ObjectID, string DestinationTableName, string DestinationID, string AdditionalField, string AdditionalMemoField, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(scriptName))
                return true;
            if (engine is null)
                return false;

            string[,] userlinks = null;
            var getSelectedRowIds = selectedRowIds;
            string[] setSelectedRowIds = null;

            string holdShortDatePattern = CurrentCulture.DateTimeFormat.ShortDatePattern;
            CurrentCulture.DateTimeFormat.ShortDatePattern = GetSQLDatePattern(conn);

            try
            {
                if (selectedRowIds is not null)
                {
                    setSelectedRowIds = new string[(selectedRowIds.Count())];

                    for (int i = 0, loopTo = selectedRowIds.Count() - 1; i <= loopTo; i++)
                        setSelectedRowIds[i] = string.Empty;
                }

                if (string.IsNullOrEmpty(AdditionalField))
                    AdditionalField = string.Empty;
                if (string.IsNullOrEmpty(AdditionalMemoField))
                    AdditionalMemoField = string.Empty;

                // If .Caller = CallerTypes.Empty Then .Caller = callerType 'HeyReggie (search for CallerTypes.Empty)
                engine.Caller = callerType;
                engine.ScriptName = scriptName;
                engine.Title = ProductName + " - [" + Strings.Trim(scriptName) + "]";
                engine.ActiveUser = GetUserName(passport);
                engine.BCDueBack = DateTime.Now.AddDays(1d);
                engine.UserLink = userlinks;
                engine.DeletePage = false;
                engine.StopScanner = false;
                engine.OutputImageName = string.Empty;
                engine.RecordId = tableId;
                engine.ViewId = viewId;

                engine.BCAdditionalField = AdditionalField;
                engine.BCAdditionalMemoField = AdditionalMemoField;
                engine.BCObjTableName = ObjectTableName;
                engine.BCObjTableId = ObjectID;
                engine.BCDestTableName = DestinationTableName;
                engine.BCDestTableId = DestinationID;
                engine.NewRecordId = string.Empty;

                engine.GetSelectedRowIds = getSelectedRowIds;
                if (engine.GetSelectedRowIds is not null)
                {
                    engine.GetSelectedRowIdsCount = engine.GetSelectedRowIds.Count();
                }
                else
                {
                    engine.GetSelectedRowIdsCount = 0;
                }

                engine.SetSelectedRowIds = setSelectedRowIds;
                if (engine.SetSelectedRowIds is not null)
                {
                    engine.SetSelectedRowIdsCount = engine.SetSelectedRowIds.Count();
                }
                else
                {
                    engine.SetSelectedRowIdsCount = 0;
                }

                engine.SetSelectedRowIdsChanged = false;
                engine.CurrentTableName = tableName;

                engine.Process();

                if (engine.SetSelectedRowIds is not null && engine.SetSelectedRowIds.Count() > 0)
                {
                    selectedRowIds = engine.SetSelectedRowIds;
                    engine.SetSelectedRowIdsChanged = true;
                }

                if (!engine.ShowPromptBool)
                    engine.ClearPublicVariables();

                userlinks = null;
                getSelectedRowIds = null;
                setSelectedRowIds = null;

                return !engine.StopScanner;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                CurrentCulture.DateTimeFormat.ShortDatePattern = holdShortDatePattern;
            }
        }

        private static bool RunScript(ref InternalEngine engine, string scriptName, string tableName, string tableId, string destinationTableName, string destinationTableId, Passport passport, CallerTypes callerType, SqlConnection conn, List<string> selectedRowIds = null)
        {
            if (string.IsNullOrEmpty(scriptName))
                return true;
            if (engine is null)
                return false;

            string[,] userlinks = null;
            string[] getSelectedRowIds = null;
            string[] setSelectedRowIds = null;

            string holdShortDatePattern = CurrentCulture.DateTimeFormat.ShortDatePattern;
            CurrentCulture.DateTimeFormat.ShortDatePattern = GetSQLDatePattern(conn);

            try
            {
                if (selectedRowIds is not null)
                {
                    setSelectedRowIds = new string[selectedRowIds.Count];

                    for (int i = 0, loopTo = selectedRowIds.Count - 1; i <= loopTo; i++)
                        setSelectedRowIds[i] = string.Empty;
                }

                // If .Caller = CallerTypes.Empty Then .Caller = callerType 'HeyReggie (search for CallerTypes.Empty)
                engine.Caller = callerType;
                engine.ScriptName = scriptName;
                engine.Title = ProductName + " - [" + Strings.Trim(scriptName) + "]";
                engine.ActiveUser = GetUserName(passport);
                engine.BCDueBack = DateTime.Now.AddDays(1d);
                engine.UserLink = userlinks;
                engine.DeletePage = false;
                engine.StopScanner = false;
                engine.OutputImageName = string.Empty;
                engine.RecordId = tableId;

                engine.NewRecordId = string.Empty;
                engine.GetSelectedRowIds = getSelectedRowIds;
                engine.GetSelectedRowIdsCount = 0;

                engine.SetSelectedRowIds = setSelectedRowIds;
                engine.SetSelectedRowIdsCount = 0;
                engine.SetSelectedRowIdsChanged = false;
                engine.CurrentTableName = tableName;

                engine.Process();
                engine.ClearPublicVariables();

                userlinks = null;
                getSelectedRowIds = null;
                setSelectedRowIds = null;

                return !engine.StopScanner;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                CurrentCulture.DateTimeFormat.ShortDatePattern = holdShortDatePattern;
            }
        }

        private static string GetUserName(Passport passport)
        {
            try
            {
                var user = new User(passport, true);
                return user.UserName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return passport.UserId.ToString();
            }
        }
    }

    public class InternalEngine
    {
        private class INIFile
        {
            private const short INI_STRING_LEN = 256;
            private const string INI_FILE = "sdlk5.ini";

            [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringA")]
            private static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, string lpReturnedString, int nSize, string lpFileName);
            [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringA")]
            private static extern int GetPrivateProfileStringLong(string lpApplicationName, int lpKeyName, string lpDefault, string lpReturnedString, int nSize, string lpFileName);
            [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringA")]
            private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);
            [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringA")]
            private static extern int WritePrivateProfileStringLong(string lpApplicationName, string lpKeyName, int lpString, string lpFileName);
            [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringA")]
            private static extern int WritePrivateProfileStringLongLong(string lpApplicationName, int lpKeyName, int lpString, string lpFileName);

            public static void WriteString(string section, string key, string value, string fileName = "")
            {
                int lRC;

                if (string.IsNullOrEmpty(fileName))
                    fileName = GetINIFileName();
                if (string.IsNullOrEmpty(fileName))
                    return;

                try
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        // Sending a NULL will Remove an unused section deleting all old stuff
                        lRC = INIFile.WritePrivateProfileStringLongLong(section, 0, 0, fileName);
                    }
                    else if (string.IsNullOrEmpty(value))
                    {
                        // Sending a NULL will Remove an unused entry
                        lRC = INIFile.WritePrivateProfileStringLong(section, key, 0, fileName);
                    }
                    else
                    {
                        lRC = INIFile.WritePrivateProfileString(section, key, value, fileName);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // ignore
                }
            }

            public static string ReadString(string section, string key, string defaultValue, string fileName = "")
            {
                var lLength = default(int);
                int lRC;
                string sReturnBuffer = string.Empty;


                if (string.IsNullOrEmpty(fileName))
                    GetINIFileName();
                if (string.IsNullOrEmpty(fileName))
                    return string.Empty;
                if (!IsValidPath(fileName))
                    return string.Empty;

                if (string.IsNullOrEmpty(key))
                {
                    lRC = lLength - 2;

                    while (lRC == lLength - 2)
                    {
                        lLength = lLength + INI_STRING_LEN;
                        sReturnBuffer = new string('\0', lLength);
                        lRC = INIFile.GetPrivateProfileStringLong(section, 0, defaultValue, sReturnBuffer, lLength, fileName);
                    }
                }
                else
                {
                    lRC = lLength - 1;

                    while (lRC == lLength - 1)
                    {
                        lLength = lLength + INI_STRING_LEN;
                        sReturnBuffer = new string('\0', lLength);
                        lRC = INIFile.GetPrivateProfileString(section, key, defaultValue, sReturnBuffer, lLength, fileName);
                    }
                }

                return sReturnBuffer.Substring(0, lRC);
            }

            private static string GetINIFileName()
            {
                try
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.System) + INI_FILE;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }
            }

            public static bool IsValidPath(string fileName)
            {
                if (string.IsNullOrEmpty(fileName))
                    fileName = GetINIFileName();
                if (string.IsNullOrEmpty(fileName))
                    return false;

                try
                {
                    return System.IO.File.Exists(fileName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        private Dictionary<string, ScriptControls> _scriptControlDictionary;
        private LinkScriptVariables _publicVariables;
        private DataTable _labelIdsTable;
        // Module variables...
        private bool _BCDestTrackable;
        private bool _BCDestActive;
        private bool _BCObjTrackable;
        private bool _BCObjActive;
        private bool _BCDoReconcile;
        private bool _BCCheckForDups;
        private bool _BCResetTree;
        private int _DBMajorVersion;
        private int _DBMinorVersion;
        private bool _deletePage;
        private bool _disableInitialTrackingLocation;
        private bool _frontPage;
        private bool _gridRefresh;
        private bool _raiseEventAllowed;
        private bool _status;
        private bool _stopScanner;
        private bool _setSelectedRowIdsChanged;
        private bool _debugMode;
        private bool _hasWaitForm;
        private DateTime _BCDueBack;
        private DateTime _BCScanDateTime;
        private int _oneStripFormsId;
        private int _onestripJobsId;
        private int _BCDestContainerNumber;
        private int _BCObjContainerNumber;
        private int _barCodeCount;
        private int[] _barCodeTop;
        private int[] _barCodeLeft;
        private int[] _barCodeType;
        private int _errorMode;
        private int _maxRecordsToFetch;
        private int _getSelectedRowIdsCount;
        private int _setSelectedRowIdsCount;
        private int _DPIVertical;
        private int _DPIHorizontal;
        private int _viewId;
        private int _attachmentNumber;
        private string _BCAdditionalField;
        private string _BCAdditionalMemoField;
        private string _BCDestWindow;
        private string _BCDestTableName;
        private string _BCDestTableId;
        private string _BCObjWindow;
        private string _BCObjTableName;
        private string _BCObjTableId;
        private string[] _barCode;
        private string _activeUser;
        private string _gridSQL;
        private string _gridCompleteSQL;
        private string _heading;
        private string _labelTableName;
        private string _newAttachmentName;
        private string _outputImageName;
        private string _recordId;
        private string _scriptName;
        private string[] _getSelectedRowIds;
        private string[] _setSelectedRowIds;
        private string[,] _userLink;
        private string _viewerImageFileName;
        private string _viewerImageTableName;
        private string _title;
        private string _currentTableName;
        private Passport _passport;

        private ScriptEngine.CallerTypes _caller;
        private ScriptEngine.UITypes _UIType;
        private MsgBoxResult _msgBoxResult;
        private LinkScriptVariables privateVariables;

        private string _newRecordId = string.Empty;
        private string _returnMessage = string.Empty;
        private string _messageType = "s";

        private int _saveAsNewPage;
        private int _saveAsNewVersion;
        private int _saveAsNewVersionAsOfficialRecord;

        private object _variable;
        private int[] _loopStack = null;
        private string[] _scriptStack = null;
        private bool _isFinalStackLevel;
        private ScriptForm _form;
        private ScriptWaitForm _waitForm;

        private double _currentScriptSequence;
        private bool _showPrompt;
        private int _currentIndex;
        private int _loopStart;
        private bool _returnToSubscript;

        public event LoadPromptEventEventHandler LoadPromptEvent;

        public delegate void LoadPromptEventEventHandler(ref bool bSuccessful);
        public event ShowErrorEventEventHandler ShowErrorEvent;

        public delegate void ShowErrorEventEventHandler(string Message, string ScriptName, string SequenceNumber, ref MsgBoxResult Result);
        public event ShowPromptEventEventHandler ShowPromptEvent;

        public delegate void ShowPromptEventEventHandler(ref bool bSuccessful);
        public event UnloadPromptEventEventHandler UnloadPromptEvent;

        public delegate void UnloadPromptEventEventHandler(ref bool bSuccessful);
        public event ShowDebugEventEventHandler ShowDebugEvent;

        public delegate void ShowDebugEventEventHandler(bool bShow, bool bActivate);
        public event ShowWaitFormEventEventHandler ShowWaitFormEvent;

        public delegate void ShowWaitFormEventEventHandler(bool bShow, ScriptWaitForm oWaitForm);
        public event WaitFormCloseEventEventHandler WaitFormCloseEvent;

        public delegate void WaitFormCloseEventEventHandler();
        public event RefreshWaitFormEventEventHandler RefreshWaitFormEvent;

        public delegate void RefreshWaitFormEventEventHandler(ScriptWaitForm oWaitForm);

        // rvw Public Event DoPrintLabels(oTables As Tables, oOneStripJobs As OneStripJobs, oOneStripForms As OneStripForms, LabelIdsTable As DataTable, bFromCommLabels As Boolean)

        public event AddControlEventHandler AddControl;

        public delegate void AddControlEventHandler(ScriptControls.ControlTypes eControlType, string sControlName, string sModifier);
        public event ClearControlsEventHandler ClearControls;

        public delegate void ClearControlsEventHandler();
        public event ControlAddItemEventHandler ControlAddItem;

        public delegate void ControlAddItemEventHandler(string sControlName, string sControlValue, int lItemData);
        public event ControlExistsEventHandler ControlExists;

        public delegate void ControlExistsEventHandler(string sKey, ref bool bExists);
        public event CaptionEventHandler Caption;

        public delegate void CaptionEventHandler(ref string sValue);
        public event GetControlPropEventHandler GetControlProp;

        public delegate void GetControlPropEventHandler(string sControlName, ScriptControls.ControlProperties eControlProperty, ref object vValue);
        public event SetControlPropEventHandler SetControlProp;

        public delegate void SetControlPropEventHandler(string sPrefix, ScriptControls.ControlProperties eControlProperty, object vValue);

        public InternalEngine(Passport passport, bool RaiseEventAllowed, bool GridRefresh, ScriptEngine.UITypes UIType) : base()
        {
            AddControl += InternalEngine_AddControl;
            ControlAddItem += InternalEngine_ControlAddItem;
            GetControlProp += InternalEngine_GetControlProp;
            SetControlProp += InternalEngine_SetControlProp;
            UnloadPromptEvent += InternalEngine_UnloadPromptEvent;
            // _caller = ScriptEngine.CallerTypes.Empty 'HeyReggie (search for CallerTypes.Empty)
            _passport = passport;
            _raiseEventAllowed = RaiseEventAllowed;
            _gridRefresh = GridRefresh;
            _UIType = UIType;

            _saveAsNewPage = -99;
            _saveAsNewVersion = -99;
            _saveAsNewVersionAsOfficialRecord = -99;
            _scriptControlDictionary = new Dictionary<string, ScriptControls>();
            _showPrompt = false;
            _currentScriptSequence = 0d;
            _BCAdditionalField = string.Empty;
            _BCAdditionalMemoField = string.Empty;
            _BCDestWindow = string.Empty;
            _BCDestTableName = string.Empty;
            _BCDestTableId = string.Empty;
            _BCObjWindow = string.Empty;
            _BCObjTableName = string.Empty;
            _BCObjTableId = string.Empty;
            _activeUser = string.Empty;
            _gridCompleteSQL = string.Empty;
            _gridSQL = string.Empty;
            _heading = string.Empty;
            _labelTableName = string.Empty;
            _outputImageName = string.Empty;
            _recordId = string.Empty;
            _scriptName = string.Empty;
            _viewerImageFileName = string.Empty;
            _viewerImageTableName = string.Empty;
            _title = string.Empty;
            _currentTableName = string.Empty;
            GetDBVersion();
        }

        ~InternalEngine()
        {
            if (_loopStack is not null)
                _loopStack = null;
        }

        public Dictionary<string, ScriptControls> ScriptControlDictionary
        {
            get
            {
                return _scriptControlDictionary;
            }
        }

        public double CurrentScriptSequence
        {
            get
            {
                return _currentScriptSequence;
            }
        }

        public bool ShowPromptBool
        {
            get
            {
                return _showPrompt;
            }
            set
            {
                _showPrompt = value;
            }
        }

        public ScriptForm Form
        {
            get
            {
                if (_form is null)
                    _form = new ScriptForm(_raiseEventAllowed);
                return _form;
            }
        }

        public ScriptWaitForm WaitForm
        {
            get
            {
                if (_waitForm is null)
                    _waitForm = new ScriptWaitForm(_raiseEventAllowed);
                return _waitForm;
            }
        }

        private int DBMajorVersion
        {
            get
            {
                return _DBMajorVersion;
            }
        }

        private int DBMinorVersion
        {
            get
            {
                return _DBMinorVersion;
            }
        }

        private bool get_IsValidCaller(ScriptEngine.CallerTypes callerType)
        {
            // Return ((Caller And callerType) = callerType) 'HeyReggie (search for CallerTypes.Empty)
            return Caller == ScriptEngine.CallerTypes.AnyCaller || (Caller & callerType) == callerType;
        }

        public string ActiveUser
        {
            get
            {
                return _activeUser;
            }
            set
            {
                _activeUser = value;
            }
        }

        public int AttachmentNumber
        {
            get
            {
                return _attachmentNumber;
            }
            set
            {
                _attachmentNumber = value;
            }
        }

        public string[] BarCode
        {
            get
            {
                return _barCode;
            }
            set
            {
                _barCode = value;
            }
        }

        public int BarCodeCount
        {
            get
            {
                return _barCodeCount;
            }
            set
            {
                _barCodeCount = value;
            }
        }

        public int[] BarCodeLeft
        {
            get
            {
                return _barCodeLeft;
            }
            set
            {
                _barCodeLeft = value;
            }
        }

        public int[] BarCodeTop
        {
            get
            {
                return _barCodeTop;
            }
            set
            {
                _barCodeTop = value;
            }
        }

        public int[] BarCodeType
        {
            get
            {
                return _barCodeType;
            }
            set
            {
                _barCodeType = value;
            }
        }

        public ScriptEngine.TrackingAdditionalFieldTypes BCAdditionalFieldType
        {
            get
            {
                try
                {
                    var systemRow = ScriptEngine.GetSystemRow(_passport);
                    return (ScriptEngine.TrackingAdditionalFieldTypes)systemRow.TrackingAdditionalField1Type;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return ScriptEngine.TrackingAdditionalFieldTypes.Text;
                }
            }
        }

        public string BCAdditionalField
        {
            get
            {
                return _BCAdditionalField;
            }
            set
            {
                switch (BCAdditionalFieldType)
                {
                    case ScriptEngine.TrackingAdditionalFieldTypes.Selection:
                        {
                            _BCAdditionalField = string.Empty;
                            if (!string.IsNullOrEmpty(GetTrackingSelectData(value, _passport)))
                                _BCAdditionalField = value;
                            break;
                        }

                    default:
                        {
                            _BCAdditionalField = value;
                            break;
                        }
                }
            }
        }

        public string BCAdditionalMemoField
        {
            get
            {
                return _BCAdditionalMemoField;
            }
            set
            {
                _BCAdditionalMemoField = value;
            }
        }

        public string BCDestWindow
        {
            get
            {
                return _BCDestWindow;
            }
            set
            {
                _BCDestWindow = value;
            }
        }

        public string BCObjWindow
        {
            get
            {
                return _BCObjWindow;
            }
            set
            {
                _BCObjWindow = value;
            }
        }

        public string BCDestTableName
        {
            get
            {
                return _BCDestTableName;
            }
            set
            {
                _BCDestTableName = value;
            }
        }

        public string BCDestTableId
        {
            get
            {
                return _BCDestTableId;
            }
            set
            {
                _BCDestTableId = value;
            }
        }

        public int BCDestContainerNumber
        {
            get
            {
                return _BCDestContainerNumber;
            }
            set
            {
                _BCDestContainerNumber = value;
            }
        }

        public bool BCDestTrackable
        {
            get
            {
                return _BCDestTrackable;
            }
            set
            {
                _BCDestTrackable = value;
            }
        }

        public bool BCDestActive
        {
            get
            {
                return _BCDestActive;
            }
            set
            {
                _BCDestActive = value;
            }
        }

        public string BCObjTableName
        {
            get
            {
                return _BCObjTableName;
            }
            set
            {
                _BCObjTableName = value;
            }
        }

        public string BCObjTableId
        {
            get
            {
                return _BCObjTableId;
            }
            set
            {
                _BCObjTableId = value;
            }
        }

        public int BCObjContainerNumber
        {
            get
            {
                return _BCObjContainerNumber;
            }
            set
            {
                _BCObjContainerNumber = value;
            }
        }

        public bool BCObjTrackable
        {
            get
            {
                return _BCObjTrackable;
            }
            set
            {
                _BCObjTrackable = value;
            }
        }

        public bool BCObjActive
        {
            get
            {
                return _BCObjActive;
            }
            set
            {
                _BCObjActive = value;
            }
        }

        public DateTime BCDueBack
        {
            get
            {
                return _BCDueBack;
            }
            set
            {
                _BCDueBack = value;
            }
        }

        public DateTime BCScanDateTime
        {
            get
            {
                return _BCScanDateTime;
            }
            set
            {
                _BCScanDateTime = value;
            }
        }

        public bool BCDoReconcile
        {
            get
            {
                return _BCDoReconcile;
            }
            set
            {
                _BCDoReconcile = value;
            }
        }

        public bool BCCheckForDups
        {
            get
            {
                return _BCCheckForDups;
            }
            set
            {
                _BCCheckForDups = value;
            }
        }

        public bool BCResetTree
        {
            get
            {
                return _BCResetTree;
            }
            set
            {
                _BCResetTree = value;
            }
        }
        //[Newtonsoft.Json.JsonIgnore]
        //public string Clipboard
        //{
        //    get
        //    {
        //        if (System.Windows.Forms.Clipboard.GetDataObject().GetDataPresent(System.Windows.Forms.DataFormats.Text))
        //            return System.Windows.Forms.Clipboard.GetDataObject().GetData(System.Windows.Forms.DataFormats.Text).ToString();
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        var oData = new System.Windows.Forms.DataObject();

        //        oData.SetData(System.Windows.Forms.DataFormats.Text, value);
        //        System.Windows.Forms.Clipboard.SetDataObject(oData);
        //    }
        //}

        public string CurrentTableName
        {
            get
            {
                string CurrentTableNameRet = default;
                CurrentTableNameRet = _currentTableName;
                return CurrentTableNameRet;
            }
            set
            {
                _currentTableName = value;
            }
        }

        public bool DeletePage
        {
            get
            {
                return _deletePage;
            }
            set
            {
                _deletePage = value;
            }
        }

        public bool DisableInitialTrackingLocation
        {
            get
            {
                return _disableInitialTrackingLocation;
            }
            set
            {
                _disableInitialTrackingLocation = value;
            }
        }

        public int DPIHorizontal
        {
            get
            {
                return _DPIHorizontal;
            }
            set
            {
                _DPIHorizontal = value;
            }
        }

        public int DPIVertical
        {
            get
            {
                return _DPIVertical;
            }
            set
            {
                _DPIVertical = value;
            }
        }

        public int ErrorMode
        {
            get
            {
                return _errorMode;
            }
            set
            {
                _errorMode = value;
            }
        }

        public bool FrontPage
        {
            get
            {
                return _frontPage;
            }
            set
            {
                _frontPage = value;
            }
        }

        public string[] GetSelectedRowIds
        {
            get
            {
                return _getSelectedRowIds;
            }
            set
            {
                _getSelectedRowIds = value;
            }
        }

        public int GetSelectedRowIdsCount
        {
            get
            {
                return _getSelectedRowIdsCount;
            }
            set
            {
                _getSelectedRowIdsCount = value;
            }
        }

        public bool GridRefresh
        {
            get
            {
                return _gridRefresh;
            }
            set
            {
                _gridRefresh = value;
            }
        }

        public string GridCompleteSQL
        {
            get
            {
                return _gridCompleteSQL;
            }
            set
            {
                _gridCompleteSQL = value;
            }
        }

        public string GridSQL
        {
            get
            {
                return _gridSQL;
            }
            set
            {
                _gridSQL = value;
            }
        }

        public string Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                _heading = value;
                Caption?.Invoke(ref value);
            }
        }

        public string LabelTableName
        {
            get
            {
                return _labelTableName;
            }
            set
            {
                _labelTableName = value;
            }
        }

        public ScriptEngine.CallerTypes Caller
        {
            get
            {
                return _caller;
            }
            set
            {
                _caller = value;
            }
        }

        public int MaxRecordsToFetch
        {
            get
            {
                return _maxRecordsToFetch;
            }
            set
            {
                _maxRecordsToFetch = value;
            }
        }

        public int MessageBoxResult
        {
            get
            {
                return (int)_msgBoxResult;
            }
            set
            {
                _msgBoxResult = (MsgBoxResult)value;
            }
        }

        public string NewRecordId
        {
            get
            {
                return _newRecordId;
            }
            set
            {
                _newRecordId = value;
            }
        }

        public int OneStripFormsId
        {
            get
            {
                return _oneStripFormsId;
            }
            set
            {
                _oneStripFormsId = value;
            }
        }

        public int OneStripJobsId
        {
            get
            {
                return _onestripJobsId;
            }
            set
            {
                _onestripJobsId = value;
            }
        }

        public string OutputImageName
        {
            get
            {
                return _outputImageName;
            }
            set
            {
                _outputImageName = value;
            }
        }

        public string RecordId
        {
            get
            {
                return _recordId;
            }
            set
            {
                _recordId = value;
            }
        }

        public int SaveAsNewPage
        {
            get
            {
                return _saveAsNewPage;
            }
            set
            {
                _saveAsNewPage = value;
            }
        }

        public int SaveAsNewVersion
        {
            get
            {
                return _saveAsNewVersion;
            }
            set
            {
                _saveAsNewVersion = value;
            }
        }

        public int SaveAsNewVersionAsOfficialRecord
        {
            get
            {
                return _saveAsNewVersionAsOfficialRecord;
            }
            set
            {
                _saveAsNewVersionAsOfficialRecord = value;
            }
        }

        public string ScriptName
        {
            get
            {
                return _scriptName;
            }
            set
            {
                _scriptName = value;
            }
        }

        public bool StopScanner
        {
            get
            {
                return _stopScanner;
            }
            set
            {
                _stopScanner = value;
            }
        }

        public string[] SetSelectedRowIds
        {
            get
            {
                return _setSelectedRowIds;
            }
            set
            {
                _setSelectedRowIds = value;
            }
        }

        public bool SetSelectedRowIdsChanged
        {
            get
            {
                return _setSelectedRowIdsChanged;
            }
            set
            {
                _setSelectedRowIdsChanged = value;
            }
        }

        public int SetSelectedRowIdsCount
        {
            get
            {
                return _setSelectedRowIdsCount;
            }
            set
            {
                _setSelectedRowIdsCount = value;
            }
        }

        public bool Status
        {
            get
            {
                bool StatusRet = default;
                StatusRet = _status;
                return StatusRet;
            }
            set
            {
                _status = value;
            }
        }

        public string[,] UserLink
        {
            get
            {
                if (_userLink is null)
                {
                    _userLink = new string[1, 1];
                    _userLink[0, 0] = string.Empty;
                }
                return _userLink;
            }
            set
            {
                _userLink = value;
            }
        }

        public string ReturnMessage
        {
            get
            {
                return _returnMessage;
            }
            set
            {
                _returnMessage = Strings.Trim(value);
            }
        }

        public string MessageType
        {
            get
            {
                return _messageType;
            }
        }

        public ScriptEngine.UITypes UIType
        {
            get
            {
                switch (_UIType)
                {
                    case ScriptEngine.UITypes.NoUI:
                    case ScriptEngine.UITypes.SimpleUI:
                    case ScriptEngine.UITypes.ComplexUI:
                    case ScriptEngine.UITypes.CompleteUI:
                        {
                            return _UIType;
                        }

                    default:
                        {
                            return ScriptEngine.UITypes.NoUI;
                        }
                }
            }

            set
            {
                switch (value)
                {
                    case ScriptEngine.UITypes.NoUI:
                    case ScriptEngine.UITypes.SimpleUI:
                    case ScriptEngine.UITypes.ComplexUI:
                    case ScriptEngine.UITypes.CompleteUI:
                        {
                            break;
                        }

                    default:
                        {
                            value = ScriptEngine.UITypes.NoUI;
                            break;
                        }
                }

                _UIType = value;
            }
        }

        public int ViewId
        {
            get
            {
                return _viewId;
            }
            set
            {
                _viewId = value;
            }
        }

        public string ViewerImageFileName
        {
            get
            {
                return _viewerImageFileName;
            }
            set
            {
                _viewerImageFileName = value;
            }
        }

        public string ViewerImageTableName
        {
            get
            {
                return _viewerImageTableName;
            }
            set
            {
                _viewerImageTableName = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Form.Text = value;
            }
        }

        private void GetDBVersion()
        {
            _DBMajorVersion = 0;
            _DBMinorVersion = 0;

            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT [Version] FROM [DBVersion]", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        using (var dt = new DataTable())
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count == 0)
                                return;
                            string version = dt.Rows[0][0].ToString();

                            if (version.IndexOf(".") > -1)
                            {
                                _DBMinorVersion = Conversions.ToInteger(version.Substring(version.IndexOf(".") + 1));
                                version = version.Substring(0, version.IndexOf("."));
                            }

                            if (version.Length > 2 && version.EndsWith("0"))
                                version = version.Substring(0, version.Length - 1);
                            _DBMajorVersion = Conversions.ToInteger(version);
                        }
                    }
                }
            }
        }

        private string GetTrackingSelectData(string value, SqlConnection conn)
        {
            string sql = "SELECT [Id] FROM SLTrackingSelectData WHERE [Id] = @Id";

            using (var cmd = new SqlCommand(sql, conn))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@Id", value);
                    cmd.Connection.Open();
                    return cmd.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
        }

        private string GetTrackingSelectData(string value, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackingSelectData(value, conn);
            }
        }

        public void Process()
        {
            _status = ProcessScript(false);
        }

        public object CurrentVariable
        {
            get
            {
                object CurrentVariableRet = default;
                if (!(_variable is ValueType))
                {
                    CurrentVariableRet = _variable;
                }
                else
                {
                    if (_variable is DBNull)
                        _variable = string.Empty;
                    CurrentVariableRet = _variable;
                }

                return CurrentVariableRet;
            }
            set
            {
                if (!(value is ValueType))
                {
                    if (value is not null)
                        _variable = value;
                }
                else
                {
                    if (value is DBNull)
                        value = string.Empty;
                    _variable = value;
                }
            }
        }

        private void DataAddNew(DataWithCursor table)
        {
            table.AddNew();
        }

        private void DataDelete(DataWithCursor table, string sTableName, SqlConnection conn)
        {
            bool bAuditUpdate = false;
            string sId = string.Empty;
            DataColumn oField;

            var currentTable = ScriptEngine.GetTableRow(sTableName, conn);
            if (currentTable is not null)
                bAuditUpdate = Navigation.CBoolean(currentTable["AuditUpdate"]);

            if (bAuditUpdate)
            {
                string sBeforeData = string.Empty;
                oField = Navigation.FieldWithOrWithoutTable(currentTable["IdFieldName"].ToString(), table.Columns);

                if (oField is not null)
                {
                    sId = table.CurrentRow[oField.Ordinal].ToString();
                    sBeforeData = GetOldRecordDataForDelete(currentTable, sId, conn);
                }

                {
                    ref var withBlock = ref AuditType.LinkScriptAudit;
                    withBlock.TableName = currentTable["TableName"].ToString();
                    withBlock.TableId = sId;
                    withBlock.ClientIpAddress = _passport.ServerIpAddress; // HeyReggie
                    withBlock.ActionType = AuditType.LinkScriptActionType.DeleteRecord;
                    withBlock.AfterData = string.Empty;
                    withBlock.BeforeData = sBeforeData;
                }

                Auditing.AuditUpdates(AuditType.LinkScriptAudit, _passport);
            }
            // Delete the record at the location of where the Pointer is set in this Recordset...
            table.CurrentRow.Delete();
        }

        private void DataUpdate(ref DataWithCursor table, string sTableName, ref string sBeforeData, ref string sAfterData, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(sBeforeData) && string.IsNullOrEmpty(sAfterData))
                return;

            string sId = string.Empty;
            bool bAuditUpdate = false;
            DataColumn oField = null;

            var currentTable = ScriptEngine.GetTableRow(sTableName, conn);

            if (currentTable is not null)
            {
                bAuditUpdate = Navigation.CBoolean(currentTable["AuditUpdate"]);
                // Do the counter field stuff
                string sFieldName = currentTable["CounterFieldName"].ToString().Trim();
                oField = Navigation.FieldWithOrWithoutTable(currentTable["IdFieldName"].ToString(), table.Columns);

                if (!string.IsNullOrEmpty(sFieldName) && oField is not null)
                {
                    if (string.IsNullOrEmpty(table.CurrentRow[oField.Ordinal].ToString()))
                    {
                        // Make Sure the id in question is not in use
                        // HeyReggie
                        var counter = TablesTableAdapter.TakeNextSmeadCounterTableID(currentTable["TableName"].ToString(), null, conn);
                        table.CurrentRow[oField.Ordinal] = (object)counter.CounterValue;

                        if (!string.IsNullOrEmpty(sAfterData))
                        {
                            sAfterData += Constants.vbCrLf + Navigation.MakeSimpleField(currentTable["IdFieldName"].ToString()) + ": " + table.CurrentRow[oField.Ordinal].ToString();
                        }
                    }
                }
            }
            // Call the Update function of the data services module to update and return the recordset...
            bool bInAddMode = table.IsAddRow();

            if (bInAddMode)
            {
                Query.InsertDataTableToDatabase(sTableName, (DataTable)table, table.CurrentRow, conn);
            }
            else
            {
                // get Id BEFORE row is updated
                if (oField is not null)
                    sId = table.CurrentRow[oField.Ordinal].ToString();
                Query.UpdateDataTableToDatabase(sTableName, sId, (DataTable)table, table.CurrentRow, conn);
            }
            // get Id again (bInAddMode - AFTER row is inserted; Not bInAddMode - if Id gets changed during update)
            if (oField is not null)
                sId = table.CurrentRow[oField.Ordinal].ToString();

            if (bAuditUpdate)
            {
                {
                    ref var withBlock = ref AuditType.LinkScriptAudit;
                    withBlock.TableName = currentTable["TableName"].ToString();
                    withBlock.TableId = sId;
                    withBlock.ClientIpAddress = _passport.ServerIpAddress; // HeyReggie
                    withBlock.ActionType = AuditType.LinkScriptActionType.UpdateRecord;
                    withBlock.AfterData = sAfterData;

                    if (bInAddMode)
                    {
                        withBlock.BeforeData = string.Empty;
                        withBlock.ActionType = AuditType.LinkScriptActionType.AddRecord;
                    }
                    else
                    {
                        withBlock.BeforeData = sBeforeData;
                    }
                }

                Auditing.AuditUpdates(AuditType.LinkScriptAudit, _passport);
            }
            // Update Default Tracking Location if available and is not disabled
            if (!_disableInitialTrackingLocation && bInAddMode && currentTable is not null && oField is not null)
            {
                // Save Default Tracking Destination if applicable.
                if (!string.IsNullOrEmpty(currentTable["DefaultTrackingId"].ToString()))
                {
                    sId = table.CurrentRow[oField.Ordinal].ToString().PadLeft(30, '0');
                    var oBarCodeTable = ScriptEngine.GetTableRow(currentTable["DefaultTrackingTable"].ToString(), conn);

                    if (oBarCodeTable is not null & !string.IsNullOrEmpty(currentTable["DefaultTrackingId"].ToString()) & !string.IsNullOrEmpty(sId))
                    {
                        Tracking.Transfer(currentTable["TableName"].ToString(), sId, currentTable["DefaultTrackingTable"].ToString(), currentTable["DefaultTrackingId"].ToString(), DateTime.Today.AddDays(1d), string.Empty, _passport, conn);
                    }
                }
            }

            if (!string.IsNullOrEmpty(sId))
            {
                var data = new List<FieldValue>();

                foreach (DataColumn currentOField in table.Columns)
                {
                    oField = currentOField;
                    var item = new FieldValue(oField.ColumnName, oField.DataType.ToString());
                    item.value = table.CurrentRow[oField.Ordinal];
                    data.Add(item);
                }

                Query.UpdateSLIndexerData(sTableName, sId, data, conn, bInAddMode);
            }
            // If bInAddMode Then table.RemoveNewRow()
        }

        private string GetOldRecordDataForDelete(DataRow oTables, string sTableId, SqlConnection conn)
        {
            string sql;
            string rtn = string.Empty;

            sql = "SELECT * FROM " + oTables["TableName"].ToString() + " WHERE " + Navigation.MakeSimpleField(oTables["IdFieldName"].ToString()) + " = @Id";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", sTableId);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var table = new DataWithCursor();
                    da.Fill(table);

                    if (table.Rows.Count > 0)
                    {
                        foreach (DataColumn oField in table.Columns)
                        {
                            if (!(table.CurrentRow[oField.Ordinal] is DBNull) & !ReferenceEquals(oField.DataType, typeof(byte[])))
                            {
                                if (!string.IsNullOrEmpty(table.CurrentRow[oField.Ordinal].ToString()))
                                {
                                    rtn += oField.ColumnName + ": " + table.CurrentRow[oField.Ordinal].ToString() + Constants.vbCrLf;
                                }
                            }
                        }
                    }

                    table.Dispose();
                    if (string.IsNullOrEmpty(rtn))
                        return string.Empty;
                    return rtn.Substring(0, rtn.Length - 2);
                }
            }
        }

        private int ProcessCommands(RecordsManage.LinkScriptRow scriptLine, int currentIndex, ref BaseCollection parameters, ref LinkScriptVariables privateVariables, SqlConnection conn)
        {
            bool auditUpdate;
            int modifier;
            string tableName;
            object argTwo = null;
            object argThree = null;
            ScriptControls.ControlTypes controlType;
            LinkScriptVariable.VarScope scope;
            LinkScriptVariable.VariableTypes variableType;
            DataRow table;
            DataWithCursor tableVariable;

            switch (this.NormalizeCommand(scriptLine.Command) ?? "")
            {
                case "DIM":
                    {
                        // Var type?
                        switch (scriptLine.ArgTwo.ToUpper().Trim() ?? "")
                        {
                            case "DATE":
                                {
                                    variableType = LinkScriptVariable.VariableTypes.vtDate;
                                    break;
                                }
                            case "DOUBLE":
                                {
                                    variableType = LinkScriptVariable.VariableTypes.vtDouble;
                                    break;
                                }
                            case "INTEGER":
                                {
                                    variableType = LinkScriptVariable.VariableTypes.vtInteger;
                                    break;
                                }
                            case "LONG":
                                {
                                    variableType = LinkScriptVariable.VariableTypes.vtLong;
                                    break;
                                }
                            case "STRING":
                                {
                                    variableType = LinkScriptVariable.VariableTypes.vtString;
                                    break;
                                }
                            case "SINGLE":
                                {
                                    variableType = LinkScriptVariable.VariableTypes.vtSingle;
                                    break;
                                }
                            case "RECORDSET":
                            case "DATATABLE":
                                {
                                    variableType = LinkScriptVariable.VariableTypes.vtDataTable;
                                    break;
                                }

                            default:
                                {
                                    throw new Exception(string.Format("Attempt to Dimension a unsupported Variable type({0}) - {1} [{2}].", scriptLine.ArgTwo.Trim(), scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                                }
                        }
                        // Scope?
                        switch (Strings.UCase(Strings.Trim(scriptLine.ArgThree)) ?? "")
                        {
                            case "PUBLIC":
                                {
                                    scope = LinkScriptVariable.VarScope.vsPublic;
                                    break;
                                }
                            case "PRIVATE":
                                {
                                    scope = LinkScriptVariable.VarScope.vsPrivate;
                                    break;
                                }

                            default:
                                {
                                    throw new Exception(string.Format("Invalid scope assignment in \"Dim\" command({0}) - {1} [{2}].", scriptLine.ArgThree.Trim(), scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                                }
                        }
                        // Create variable...
                        this.DimVariable(scriptLine.ArgOne, variableType, scope, ref privateVariables);
                        break;
                    }
                case "COMMENT":
                    {
                        break;
                    }
                // Just do nothing!
                case "GETTABLEDISPLAYSTRING":
                    {
                        this.GetTableDisplayString(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.Result, ref privateVariables, conn);
                        break;
                    }
                case "DESTROY":
                    {
                        this.SetToNothing(scriptLine.ArgOne, ref privateVariables);
                        break;
                    }
                case "MOVE":
                    {
                        this.MoveVariable(scriptLine.ArgOne, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "LENGTH":
                    {
                        this.LengthOfVariable(scriptLine.ArgOne, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "DELETEOSFILE":
                    {
                        this.DeleteOSFile(scriptLine.ArgOne, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "CALCDATE":
                    {
                        this.CalcDate(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "LEFT":
                    {
                        this.GetSubString(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.Result, LinkScriptVariable.CharTypes.ctLeft, ref privateVariables);
                        break;
                    }
                case "RIGHT":
                    {
                        this.GetSubString(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.Result, LinkScriptVariable.CharTypes.ctRight, ref privateVariables);
                        break;
                    }
                case "MID":
                    {
                        this.GetSubString(scriptLine.ArgOne, scriptLine.ArgThree, scriptLine.Result, LinkScriptVariable.CharTypes.ctMid, ref privateVariables, scriptLine.ArgTwo);
                        break;
                    }
                case "TRIM":
                    {
                        this.TrimVariable(scriptLine.ArgOne, scriptLine.Result, LinkScriptVariable.TrimTypes.ttTrim, ref privateVariables);
                        break;
                    }
                case "LTRIM":
                    {
                        this.TrimVariable(scriptLine.ArgOne, scriptLine.Result, LinkScriptVariable.TrimTypes.ttLTrim, ref privateVariables);
                        break;
                    }
                case "RTRIM":
                    {
                        this.TrimVariable(scriptLine.ArgOne, scriptLine.Result, LinkScriptVariable.TrimTypes.ttRTrim, ref privateVariables);
                        break;
                    }
                case "REPLACE":
                    {
                        bool argbReplaceAll = false;
                        this.ReplaceString(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, ref argbReplaceAll, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "REPLACEALL":
                    {
                        bool argbReplaceAll1 = true;
                        this.ReplaceString(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, ref argbReplaceAll1, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "ADD":
                    {
                        this.Math(scriptLine.ArgOne, LinkScriptVariable.MathTypes.mtAdd, scriptLine.ArgTwo, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "SUBTRACT":
                    {
                        this.Math(scriptLine.ArgOne, LinkScriptVariable.MathTypes.mtSubtract, scriptLine.ArgTwo, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "MULTIPLY":
                    {
                        this.Math(scriptLine.ArgOne, LinkScriptVariable.MathTypes.mtMultiply, scriptLine.ArgTwo, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "DIVIDE":
                    {
                        this.Math(scriptLine.ArgOne, LinkScriptVariable.MathTypes.mtDivide, scriptLine.ArgTwo, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "FORMAT":
                    {
                        this.FormatVar(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "MSGBOX":
                    {
                        switch (_UIType)
                        {
                            case ScriptEngine.UITypes.ComplexUI:
                            case ScriptEngine.UITypes.CompleteUI:
                                {
                                    _msgBoxResult = this.DisplayMsgBox(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                                    break;
                                }

                            default:
                                {
                                    if (Strings.Len(_returnMessage) > 0)
                                    {
                                        _returnMessage = _returnMessage + Constants.vbCrLf + this.ReturnMsgBoxPrompt(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                                    }
                                    else
                                    {
                                        _returnMessage = this.ReturnMsgBoxPrompt(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case "CONTROLCREATE":
                    {
                        switch (Strings.UCase(Strings.Trim(scriptLine.ArgTwo)) ?? "")
                        {
                            case "LABEL":
                                {
                                    controlType = ScriptControls.ControlTypes.ctLabel;
                                    break;
                                }
                            case "TEXTBOX":
                                {
                                    controlType = ScriptControls.ControlTypes.ctTextBox;
                                    break;
                                }
                            case "CHECKBOX":
                                {
                                    controlType = ScriptControls.ControlTypes.ctCheck;
                                    break;
                                }
                            case "RADIOGROUP":
                                {
                                    controlType = ScriptControls.ControlTypes.ctOption;
                                    break;
                                }
                            case "LISTBOX":
                                {
                                    controlType = ScriptControls.ControlTypes.ctListBox;
                                    break;
                                }
                            case "COMBOBOX":
                                {
                                    controlType = ScriptControls.ControlTypes.ctComboBox;
                                    break;
                                }
                            case "BUTTON":
                                {
                                    controlType = ScriptControls.ControlTypes.ctButton;
                                    break;
                                }
                            case "MEMO":
                                {
                                    controlType = ScriptControls.ControlTypes.ctMemoBox;
                                    break;
                                }

                            default:
                                {
                                    throw new Exception(string.Format("Attempt to create a unsupported Control type({0}) - {1} [{2}].", scriptLine.ArgTwo.Trim(), scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                                }
                        }

                        // If (controlType = ScriptControls.ControlTypes.ctButton) Then
                        // ScriptPrompt Command Button....
                        // fScriptPrompt.SetCommandButton(Trim(oLinkScript.ArgOne), Trim(oLinkScript.ArgOne), True)
                        // Else 'UserControl Controls collection...
                        modifier = 0;

                        if (!string.IsNullOrEmpty(Strings.Trim(scriptLine.ArgThree)))
                        {
                            variableType = this.GetVariableTypeAndSetValue(scriptLine.ArgThree, ref privateVariables);

                            if (variableType == LinkScriptVariable.VariableTypes.vtInteger | variableType == LinkScriptVariable.VariableTypes.vtLong)
                            {
                                modifier = Conversions.ToInteger(CurrentVariable);
                            }
                        }

                        AddControl?.Invoke(controlType, Strings.Trim(scriptLine.ArgOne), modifier.ToString());
                        break;
                    }
                // End If
                case "CONTROLADDITEM":
                    {
                        // Get VarTwo value...
                        variableType = this.GetVariableTypeAndSetValue(scriptLine.ArgTwo, ref privateVariables);
                        if (variableType != LinkScriptVariable.VariableTypes.vtDataTable)
                            argTwo = CurrentVariable;

                        if (!string.IsNullOrEmpty(scriptLine.ArgThree))
                        {
                            variableType = this.GetVariableTypeAndSetValue(scriptLine.ArgThree, ref privateVariables);

                            if (variableType != LinkScriptVariable.VariableTypes.vtDataTable)
                            {
                                argThree = CurrentVariable;
                                if (string.IsNullOrEmpty(argThree.ToString()))
                                    argThree = 0;
                            }
                            else
                            {
                                argThree = 0;
                            }
                        }
                        else
                        {
                            argThree = 0;
                        }

                        ControlAddItem?.Invoke(Strings.Trim(scriptLine.ArgOne), argTwo.ToString(), Conversions.ToInteger(argThree));
                        break;
                    }
                case "UNLOADPROMPT":
                    {
                        UnloadPrompt();
                        break;
                    }
                case "SHOWPROMPT":
                    {
                        _showPrompt = true;
                        _currentScriptSequence = scriptLine.ScriptSequence;
                        break;
                    }
                // If (LoadPrompt()) Then ShowPrompt()
                case "CLOSERECORDSET":
                    {
                        if (this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables) != LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            throw new Exception(string.Format("Error Processing RecordSet command({0}) - {1} [{2}].", scriptLine.Command.Trim(), scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                        }
                        else if (_variable is not null)
                        {
                            ((DataWithCursor)((LinkScriptVariable)_variable).Value).Dispose();
                            _variable = null;
                            ((DataWithCursor)privateVariables.Item(scriptLine.ArgOne).Value).Dispose();
                            privateVariables.Item(scriptLine.ArgOne).Value = new DataWithCursor();
                        }

                        break;
                    }
                case "CREATERECORDSET":
                    {
                        this.CreateDataTable(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables, conn);
                        break;
                    }
                // Case "SEEK": 'Seek command...(removed due to that ADO supports server side cursors only.  Matt did not want to have to specify cursor type.)
                case "MOVEFIRST":
                case "MOVELAST":
                case "MOVENEXT":
                case "MOVEPREVIOUS":
                case "DELETE":
                case "UPDATE":
                case "ADDNEW": // Recordset commands...
                    {
                        // Get VarOne value...
                        tableName = string.Empty;

                        if (this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables) != LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            throw new Exception(string.Format("Error Processing RecordSet command: {0} - {1} [{2}].", scriptLine.Command.Trim(), scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                        }
                        else
                        {
                            tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            tableName = ((LinkScriptVariable)CurrentVariable).TableName;
                        }

                        auditUpdate = false;

                        if (!string.IsNullOrEmpty(tableName))
                        {
                            try
                            {
                                table = ScriptEngine.GetTableRow(tableName, conn);
                                if (table is not null)
                                    auditUpdate = Navigation.CBoolean(table["AuditUpdate"]);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                table = null;
                            }
                        }

                        switch (this.NormalizeCommand(scriptLine.Command) ?? "")
                        {
                            case "MOVEFIRST":
                                {
                                    if (auditUpdate)
                                    {
                                        if (tableVariable is not null)
                                        {
                                            if (!tableVariable.EOF)
                                            {
                                                string argsBeforeData = ((LinkScriptVariable)CurrentVariable).BeforeData;
                                                string argsAfterData = ((LinkScriptVariable)CurrentVariable).AfterData;
                                                DataUpdate(ref tableVariable, tableName, ref argsBeforeData, ref argsAfterData, conn);
                                            }
                                        }
                                    }

                                    MoveFirst(tableVariable);
                                    ((LinkScriptVariable)CurrentVariable).ClearCollections();
                                    break;
                                }
                            case "MOVELAST":
                                {
                                    if (auditUpdate)
                                    {
                                        if (tableVariable is not null)
                                        {
                                            if (!tableVariable.EOF)
                                            {
                                                string argsBeforeData1 = ((LinkScriptVariable)CurrentVariable).BeforeData;
                                                string argsAfterData1 = ((LinkScriptVariable)CurrentVariable).AfterData;
                                                DataUpdate(ref tableVariable, tableName, ref argsBeforeData1, ref argsAfterData1, conn);
                                            }
                                        }
                                    }

                                    MoveLast(tableVariable);
                                    ((LinkScriptVariable)CurrentVariable).ClearCollections();
                                    break;
                                }
                            case "MOVENEXT":
                                {
                                    if (auditUpdate)
                                    {
                                        if (tableVariable is not null)
                                        {
                                            if (!tableVariable.EOF)
                                            {
                                                string argsBeforeData2 = ((LinkScriptVariable)CurrentVariable).BeforeData;
                                                string argsAfterData2 = ((LinkScriptVariable)CurrentVariable).AfterData;
                                                DataUpdate(ref tableVariable, tableName, ref argsBeforeData2, ref argsAfterData2, conn);
                                            }
                                        }
                                    }

                                    MoveNext(tableVariable);
                                    ((LinkScriptVariable)CurrentVariable).ClearCollections();
                                    break;
                                }
                            case "MOVEPREVIOUS":
                                {
                                    if (auditUpdate)
                                    {
                                        if (tableVariable is not null)
                                        {
                                            if (!tableVariable.EOF)
                                            {
                                                string argsBeforeData3 = ((LinkScriptVariable)CurrentVariable).BeforeData;
                                                string argsAfterData3 = ((LinkScriptVariable)CurrentVariable).AfterData;
                                                DataUpdate(ref tableVariable, tableName, ref argsBeforeData3, ref argsAfterData3, conn);
                                            }
                                        }
                                    }

                                    MovePrevious(tableVariable);
                                    ((LinkScriptVariable)CurrentVariable).ClearCollections();
                                    break;
                                }
                            case "DELETE":
                                {
                                    DataDelete(tableVariable, tableName, conn);
                                    ((LinkScriptVariable)CurrentVariable).ClearCollections();
                                    break;
                                }
                            case "UPDATE":
                                {
                                    string argsBeforeData4 = ((LinkScriptVariable)CurrentVariable).BeforeData;
                                    string argsAfterData4 = ((LinkScriptVariable)CurrentVariable).AfterData;
                                    DataUpdate(ref tableVariable, tableName, ref argsBeforeData4, ref argsAfterData4, conn);
                                    ((LinkScriptVariable)CurrentVariable).ClearCollections();
                                    break;
                                }
                            case "ADDNEW":
                                {
                                    if (auditUpdate)
                                    {
                                        if (tableVariable is not null)
                                        {
                                            if (!tableVariable.EOF)
                                            {
                                                string argsBeforeData5 = ((LinkScriptVariable)CurrentVariable).BeforeData;
                                                string argsAfterData5 = ((LinkScriptVariable)CurrentVariable).AfterData;
                                                DataUpdate(ref tableVariable, tableName, ref argsBeforeData5, ref argsAfterData5, conn);
                                            }
                                        }
                                    }

                                    DataAddNew(tableVariable);
                                    ((LinkScriptVariable)CurrentVariable).ClearCollections();
                                    break;
                                }
                        }

                        break;
                    }
                case "PARAMETERCLEAR":
                    {
                        ClearParameters(ref parameters);
                        break;
                    }
                case "PARAMETERADD":
                    {
                        string argsParmValue = scriptLine.ArgTwo;
                        this.ParameterAdd(scriptLine.ArgOne, scriptLine.ArgThree, ref argsParmValue, ref parameters, ref privateVariables);
                        scriptLine.ArgTwo = argsParmValue;
                        break;
                    }
                case "EXECUTESQL":
                    {
                        this.ExecuteSQL(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.Result, ref parameters, ref privateVariables, conn);
                        break;
                    }
                case "RENAMEATTACHMENT":
                    {
                        this.RenameAttachment(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables, conn);
                        break;
                    }
                case "USERLINK":
                    {
                        this.CreateUserlink(scriptLine.ArgOne, scriptLine.ArgTwo, ref privateVariables, conn);
                        break;
                    }
                case "CLOSE": // Not needed...
                    {
                        break;
                    }
                case "CLOSEALL": // Not needed...
                    {
                        break;
                    }
                case "INSTRING":
                    {
                        this.InString(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "APPEND":
                    {
                        this.AppendVariables(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "DEBUGON":
                    {
                        ShowDebug(true);
                        break;
                    }
                case "DEBUGOFF":
                    {
                        ShowDebug(false);
                        break;
                    }
                case "SCRIPTWAITON": // Show Script Wait Screen
                    {
                        ShowWaitForm(true);
                        break;
                    }
                case "SCRIPTWAITOFF": // Hide Script Wait Screen
                    {
                        ShowWaitForm(false);
                        break;
                    }
                case "STARTPROCESS":
                    {
                        break;
                    }
                // HeyReggie StartProcess(oLinkScript.ArgOne, oLinkScript.ArgTwo, oLinkScript.Result, privateVariables)
                case "FILECOPY":
                    {
                        this.FileCopyCmd(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "SETFOCUS":
                    {
                        this.SetFocusCmd(scriptLine.ArgOne);
                        break;
                    }
                case "FINDBARCODE":
                    {
                        break;
                    }
                // rvw FindBarCode(oLinkScript.ArgOne, oLinkScript.ArgTwo, oLinkScript.ArgThree, oLinkScript.Result, privateVariables)
                case "BARCODETRACK":
                    {
                        this.DoBarCodeTracking(scriptLine.Result, privateVariables, conn);
                        break;
                    }
                case "READINIVALUE":
                    {
                        this.ReadINIValue(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "WRITEINIVALUE":
                    {
                        this.WriteINIValue(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, ref privateVariables);
                        break;
                    }
                case "READSETTINGVALUE":
                    {
                        this.ReadSettingValue(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables);
                        break;
                    }
                case "WRITESETTINGVALUE":
                    {
                        this.WriteSettingValue(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, ref privateVariables);
                        break;
                    }
                case "PRINTLABELS":
                    {
                        break;
                    }
                // rvw PrintLabels()
                case "PRINTLABELSINIT":
                    {
                        break;
                    }
                // rvw PrintLabelsInit(oLinkScript.ArgOne, oLinkScript.ArgTwo, oLinkScript.ArgThree, privateVariables)
                case "PRINTLABELSDEINIT":
                    {
                        PrintLabelsDeInit();
                        break;
                    }
                case "PRINTLABELSADDID":
                    {
                        this.PrintLabelsAddId(scriptLine.ArgOne, ref privateVariables);
                        break;
                    }
                case "SENDEMAIL":
                    {
                        this.SendEmail(scriptLine.ArgOne, scriptLine.ArgTwo, scriptLine.ArgThree, scriptLine.Result, ref privateVariables, conn);
                        break;
                    }

                default:
                    {
                        throw new Exception(string.Format("Attempt to use a unsupported LinkScript Command({0}) - {1} [{2}].", scriptLine.Command.Trim(), scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                    }
            }
            // Set pointer to next LinkScript Record...
            return currentIndex + 1;
        }

        private bool ProcessScript(bool isSubScript)
        {
            using (var conn = _passport.Connection())
            {
                return ProcessScript(isSubScript, conn);
            }
        }

        public bool ProcessScript(bool isSubScript, SqlConnection conn)
        {
            // Dim loopStart As Integer = 0
            int currentIndex = 0;

            using (var script = new RecordsManageTableAdapters.LinkScriptTableAdapter())
            {
                script.Connection = conn;
                _isFinalStackLevel = true;

                if (_showPrompt)
                {
                    _showPrompt = false;
                    // _scriptControlDictionary.Clear()
                    currentIndex = _currentIndex;

                    while (Information.UBound(_scriptStack) > 1)
                    {
                        _scriptName = _scriptStack[Information.UBound(_scriptStack)];
                        currentIndex = this.ProcessScript(currentIndex, script.GetLinkscript(_scriptName), conn);
                        string argitem = "";
                        PopScriptStack(item: ref argitem);
                    }
                }
                else
                {
                    PushScriptStack(_scriptName);
                    if (!isSubScript)
                        _loopStack = new int[1];
                }

                _scriptName = _scriptStack[Information.UBound(_scriptStack)];
                currentIndex = this.ProcessScript(currentIndex, script.GetLinkscript(_scriptName), conn);

                if (_showPrompt & _isFinalStackLevel)
                {
                    _currentIndex = currentIndex;
                    _isFinalStackLevel = false;
                }
                else if (_isFinalStackLevel)
                {
                    if (isSubScript)
                    {
                        string argitem1 = "";
                        PopScriptStack(item: ref argitem1);
                    }
                }

                // If Not (isSubScript Or UBound(_scriptStack) > 1) Then ReDim _loopStack(0)
                Shutdown();
                return currentIndex != -1;
            }
        }

        private int ProcessScript(int currentIndex, RecordsManage.LinkScriptDataTable scriptLines, SqlConnection conn)
        {
            bool bRestartingElseIf = false;
            object vSelectCase = null;
            string sSequenceNumber = string.Empty;
            BaseCollection parameters = null;

            while (currentIndex > -1 & currentIndex < scriptLines.Count & !_showPrompt)
            {
                try
                {
                    var scriptline = scriptLines[currentIndex];

                    sSequenceNumber = scriptline.ScriptSequence.ToString();
                    // Are we in Debug mode?
                    if (_debugMode)
                        ShowDebug(true);
                    // Is this a flow-Control command?
                    switch (this.NormalizeCommand(scriptline.Command) ?? "")
                    {
                        // Flow Control Commands...
                        case "LOOP":
                        case "EXITDO":
                        case "DOUNTIL":
                        case "DOWHILE":
                        case "NEXT":
                        case "EXITFOR":
                        case "FOR":
                        case "ENDIF":
                        case "ELSE":
                        case "IF":
                        case "ELSEIF":
                        case "SELECTCASE":
                        case "CASE":
                        case "CASEELSE":
                        case "ENDSELECT":
                        case "RUNNEWSCRIPT":
                        case "GOSUBSCRIPT":
                        case "EXITSCRIPT":
                            {
                                // Process Flow-Control Commands...
                                currentIndex = this.FlowControl(scriptline, currentIndex, ref _loopStart, scriptLines, ref privateVariables, ref bRestartingElseIf, ref vSelectCase, conn);
                                break;
                            }

                        default:
                            {
                                // All non-flow control commands process here...
                                currentIndex = this.ProcessCommands(scriptline, currentIndex, ref parameters, ref privateVariables, conn);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    currentIndex = ShowError(ex.Message, sSequenceNumber);
                }
            }
            return currentIndex;
        }

        private int ShowError(string message, string sequenceNumber)
        {
            ShowErrorEvent?.Invoke(message, _scriptName, sequenceNumber, ref _msgBoxResult);
            message = string.Format("The following error occurred at line {0} in the \"{1}\" Linkscript:{2}{2}{3}", sequenceNumber, _scriptName, Constants.vbCrLf, message);
            _msgBoxResult = ScriptMsgBox(message);

            switch (_msgBoxResult)
            {
                case MsgBoxResult.Abort:
                case MsgBoxResult.Cancel:
                case MsgBoxResult.No:
                    {
                        throw new Exception(message);
                    }
            }

            return -1;
        }

        private string NormalizeCommand(string sCommand)
        {
            return sCommand.ToUpper().Replace(" ", string.Empty).Replace("_", string.Empty).Trim();
        }

        private void Shutdown()
        {
            try
            {
                if (_debugMode)
                    ShowDebug(false);
                if (_hasWaitForm)
                    WaitFormClose();
                PrintLabelsDeInit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void PrintLabelsDeInit()
        {
            if (_labelIdsTable is not null)
                _labelIdsTable.Dispose();
            _oneStripFormsId = 0;
            _onestripJobsId = 0;
            _labelTableName = string.Empty;
        }

        private void PrintLabelsAddId(string sIdValue, ref LinkScriptVariables privateVariables)
        {
            if (string.IsNullOrEmpty(sIdValue))
                throw new Exception("Error - ArgOne for command \"PrintLabelsAddIn\", not defined.");
            if (_labelIdsTable is null)
                return;
            // Get sTableName value...
            GetVariableTypeAndSetValue(sIdValue, ref privateVariables);

            _labelIdsTable.NewRow();
            _labelIdsTable.Rows.Add(new object[] { CurrentVariable.ToString() });
        }

        internal void WaitFormClose()
        {
            // Display Script Prompt...
            if (_raiseEventAllowed)
            {
                WaitFormCloseEvent?.Invoke();
                _hasWaitForm = false;
            }
            else
            {
                // Save state and pause
            }
        }

        internal bool LoadPrompt()
        {
            var bSuccessful = default(bool);
            if (_raiseEventAllowed)
                LoadPromptEvent?.Invoke(ref bSuccessful);
            return bSuccessful;
        }

        internal void RefreshWaitForm()
        {
            // Display Script Prompt...
            if (_raiseEventAllowed)
            {
                RefreshWaitFormEvent?.Invoke(WaitForm);
            }
            else
            {
                // Save state and pause
            }
        }

        internal void ShowDebug(bool bShow)
        {
            // Display Script Prompt...
            if (_raiseEventAllowed)
            {
                ShowDebugEvent?.Invoke(bShow, _debugMode);
                _debugMode = bShow;
            }
            else
            {
                // Save state and pause
            }
        }

        internal void ShowPrompt()
        {
            var bSuccessful = default(bool);
            // Display Script Prompt...
            if (_raiseEventAllowed)
            {
                ShowPromptEvent?.Invoke(ref bSuccessful);
            }
            else
            {
                // Save state and pause
            }
        }

        internal void ShowWaitForm(bool bShow)
        {
            // Display Script Prompt...
            if (_raiseEventAllowed)
            {
                ShowWaitFormEvent?.Invoke(bShow, WaitForm);
                if (bShow)
                    _hasWaitForm = true;
            }
            else
            {
                // Save state and pause
            }
        }

        internal bool UnloadPrompt()
        {
            var bSuccessful = default(bool);
            if (_raiseEventAllowed)
                UnloadPromptEvent?.Invoke(ref bSuccessful);
            return bSuccessful;
        }

        private MsgBoxResult ScriptMsgBox(string sPrompt, MsgBoxStyle eButtons = MsgBoxStyle.OkOnly, string sTitle = "")
        {
            // If (_UIType = ScriptEngine.UITypes.ComplexUI) Then
            // If String.IsNullOrEmpty(sTitle) Then sTitle = System.Reflection.Assembly.GetExecutingAssembly.GetName.Name
            // Return MsgBox(sPrompt, eButtons, sTitle)
            // End If

            // _stopScanner = True

            if (!string.IsNullOrEmpty(_returnMessage))
            {
                _returnMessage += Constants.vbCrLf + sPrompt;
            }
            else
            {
                _returnMessage = sPrompt;
            }

            return MsgBoxResult.Cancel;
        }

        private void AppendVariables(string sVarOne, string sVarTwo, string sVarThree, string sResult, ref LinkScriptVariables privateVariables)
        {
            object vOneValue = null;
            object vTwoValue = null;
            object vThreeValue = null;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"Append\", not defined.");
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception("Error - ArgTwo for command \"Append\", not defined.");
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // Get VarTwo Value...
            GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            vTwoValue = CurrentVariable;

            if (!string.IsNullOrEmpty(sVarThree))
            {
                // Get VarThree Value...
                GetVariableTypeAndSetValue(sVarThree, ref privateVariables);
                vThreeValue = CurrentVariable;
            }
            // Append values...
            if (!string.IsNullOrEmpty(sVarThree))
            {
                vOneValue = vOneValue.ToString() + vTwoValue.ToString() + vThreeValue.ToString();
            }
            else
            {
                vOneValue = vOneValue.ToString() + vTwoValue.ToString();
            }
            // Place appended VarOne and VarTwo in Result...
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception("Error - Result for command \"Append\", not defined.");
        }

        private void PushLoopStack(int item)
        {
            if (item <= 0)
                return;
            int counter = Information.UBound(_loopStack);

            for (int i = 1, loopTo = counter; i <= loopTo; i++)
            {
                if (_loopStack[counter] == item)
                    return;
            }
            // Resize array...
            counter = counter + 1;
            Array.Resize(ref _loopStack, counter + 1);
            _loopStack[counter] = item;
        }

        private int PopLoopStack(ref int item)
        {
            if (item <= 0)
                return _loopStack[0];
            int counter = Information.UBound(_loopStack);

            if (counter > 0)
            {
                counter = counter - 1;
                item = _loopStack[counter];
                Array.Resize(ref _loopStack, counter + 1);
            }

            return _loopStack[counter];
        }

        private void PushScriptStack(string item)
        {
            if (string.IsNullOrEmpty(item))
                return;
            int counter;
            if (_scriptStack is null)
            {
                _scriptStack = new string[1];
                counter = 0;
            }
            else
            {
                counter = Information.UBound(_scriptStack);
            }
            // For i As Integer = 1 To counter
            // If (_scriptStack(counter) = item) Then Return
            // Next
            // Resize array...
            counter = counter + 1;
            Array.Resize(ref _scriptStack, counter + 1);
            _scriptStack[counter] = item;
        }

        private string PopScriptStack([Optional, DefaultParameterValue("")] ref string item)
        {
            // If (String.IsNullOrEmpty(item)) Then Return _scriptStack(0)
            int counter = Information.UBound(_scriptStack);

            if (counter > 0)
            {
                // counter = (counter - 1)
                item = _scriptStack[counter];
                Array.Resize(ref _scriptStack, counter);
            }
            else
            {
                return string.Empty;
            }

            return item;
        }

        private void ClearParameters(ref BaseCollection parameters)
        {
            parameters = null;
        }

        private void ClearPrivateVariables(ref LinkScriptVariables privateVariables)
        {
            privateVariables = null;
        }

        public void ClearPublicVariables()
        {
            _publicVariables = null;
        }

        private bool IsVarAnObject(string sVarName, LinkScriptVariable.VarScope eScope, ref LinkScriptVariables privateVariables)
        {
            LinkScriptVariable var = null;

            try
            {
                switch (eScope)
                {
                    case LinkScriptVariable.VarScope.vsPublic:
                        {
                            var = GetVariable(sVarName);
                            break;
                        }
                    case LinkScriptVariable.VarScope.vsPrivate:
                        {
                            var = GetVariable(sVarName, ref privateVariables);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                if (var is not null)
                    return var.VariableType == LinkScriptVariable.VariableTypes.vtDataTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        private object ConvertToVariableType(string variableName, LinkScriptVariable.VarScope eScope, object vValue, ref LinkScriptVariables privateVariables)
        {
            LinkScriptVariable var = null;
            // NOTE:  Thist function assumes that the calling function called Public/PrivateVarExists method first...

            switch (eScope)
            {
                case LinkScriptVariable.VarScope.vsPublic:
                    {
                        // Is it an Object?
                        var = GetVariable(variableName);
                        break;
                    }
                case LinkScriptVariable.VarScope.vsPrivate:
                    {
                        // Is it an Object?
                        var = GetVariable(variableName, ref privateVariables);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            if (var is null)
                return vValue;

            switch (var.VariableType)
            {
                case LinkScriptVariable.VariableTypes.vtDate:
                    {
                        try
                        {
                            return Conversions.ToDate(vValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return DateTime.FromOADate(0d);
                        }
                    }
                case LinkScriptVariable.VariableTypes.vtDouble:
                    {
                        try
                        {
                            return Conversions.ToDouble(vValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return 0;
                        }
                    }
                case LinkScriptVariable.VariableTypes.vtInteger:
                    {
                        try
                        {
                            return Conversions.ToShort(vValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return (short)0;
                        }
                    }
                case LinkScriptVariable.VariableTypes.vtLong:
                    {
                        try
                        {
                            return Conversions.ToInteger(vValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return 0;
                        }
                    }
                case LinkScriptVariable.VariableTypes.vtProperty:
                    {
                        return vValue;
                    }
                case LinkScriptVariable.VariableTypes.vtDataTable:
                    {
                        return vValue;
                    }
                case LinkScriptVariable.VariableTypes.vtSingle:
                    {
                        try
                        {
                            return Conversions.ToSingle(vValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return 0;
                        }
                    }
                case LinkScriptVariable.VariableTypes.vtString:
                    {
                        try
                        {
                            return vValue.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return string.Empty;
                        }
                    }

                default:
                    {
                        return vValue;
                    }
            }
        }

        private void CalcDate(string sDateIn, string sIncrement, string sCalcType, string sResult, ref LinkScriptVariables privateVariables)
        {
            object vDateIn;
            object vIncrement;
            object vCalcType;
            object vResult;
            string sCalcKey;

            if (string.IsNullOrEmpty(sDateIn))
                throw new Exception("Error - ArgOne (Date In) for command \"CalcDate\", not defined.");
            if (string.IsNullOrEmpty(sIncrement))
                throw new Exception("Error - ArgTwo (Increment Value) for command \"CalcDate\", not defined.");
            if (string.IsNullOrEmpty(sCalcType))
                throw new Exception("Error - ArgThree (CalcType) for command \"CalcDate\", not defined.");
            // Get VarOne value...
            GetVariableTypeAndSetValue(sDateIn, ref privateVariables);
            vDateIn = CurrentVariable;
            // Get VarTwo value...
            GetVariableTypeAndSetValue(sIncrement, ref privateVariables);
            vIncrement = CurrentVariable;
            // Get VarThree value...
            GetVariableTypeAndSetValue(sCalcType, ref privateVariables);
            vCalcType = CurrentVariable;

            switch (vCalcType.ToString().ToUpper().Trim() ?? "")
            {
                case "YEAR":
                    {
                        sCalcKey = "yyyy";
                        break;
                    }
                case "QUARTER":
                    {
                        sCalcKey = "q";
                        break;
                    }
                case "MONTH":
                    {
                        sCalcKey = "m";
                        break;
                    }
                case "DAY":
                    {
                        sCalcKey = "d";
                        break;
                    }
                case "WEEKDAY":
                    {
                        sCalcKey = "w";
                        break;
                    }
                case "WEEK":
                    {
                        sCalcKey = "ww";
                        break;
                    }
                case "HOUR":
                    {
                        sCalcKey = "h";
                        break;
                    }
                case "MINUTE":
                    {
                        sCalcKey = "n";
                        break;
                    }
                case "SECOND":
                    {
                        sCalcKey = "s";
                        break;
                    }

                default:
                    {
                        throw new Exception("Error - ArgThree (Calc Type) for command \"CalcDate\", not valid.");
                    }
            }

            vResult = DateAndTime.DateAdd(sCalcKey, Conversions.ToDouble(vIncrement), vDateIn);
            // Place appended VarOne and VarTwo in Result...
            if (!SetResultValue(sResult, vResult, ref privateVariables))
                throw new Exception("Error - Result for command \"CalcDate\", not defined.");
        }

        private bool CreateDataTable(string sVarOne, string sVarTwo, string sLockType, object vResult, ref LinkScriptVariables privateVariables, SqlConnection conn)
        {
            string sTableName = string.Empty;
            string sSQL = string.Empty;
            LinkScriptVariable.VariableTypes eVarType;

            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"CreateRecordSet\", not defined.");
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception("Error - ArgTwo for command \"CreateRecordSet\", not defined.");
            // Get VarOne value...
            eVarType = GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtString)
                sTableName = CurrentVariable.ToString();
            // Get VarTwo value...
            eVarType = GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtString)
                sSQL = CurrentVariable.ToString();

            try
            {
                using (var cmd = new SqlCommand(sSQL, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var table = new DataWithCursor();
                        da.Fill(table);

                        if (this.SetResultValue(vResult.ToString(), table, ref privateVariables, sTableName))
                        {
                            if (table is not null)
                                table.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error - Attempting to ADO RecordSet (CreateRecordSet).");
            }

            return default;
        }

        private void MoveFirst(DataWithCursor table)
        {
            if (table is not null)
                table.MoveFirst();
        }

        private void MoveNext(DataWithCursor table)
        {
            if (table is not null)
                table.MoveNext();
        }

        private void MovePrevious(DataWithCursor table)
        {
            if (table is not null)
                table.MovePrevious();
        }

        private void MoveLast(DataWithCursor table)
        {
            if (table is not null)
                table.MoveLast();
        }

        private void ParameterAdd(string sParmName, string sParmType, ref string sParmValue, ref BaseCollection parameters, ref LinkScriptVariables privateVariables)
        {
            SqlDbType eDataType;
            object vParmValue;
            SqlParameter parameter;
            // Did var exist?
            if (string.IsNullOrEmpty(sParmValue))
                throw new Exception("Error - ArgTwo for command \"ParameterAdd\", not defined.");
            // Get Value to move...
            GetVariableTypeAndSetValue(sParmValue, ref privateVariables);
            vParmValue = CurrentVariable;
            // Set Datatype...
            switch (sParmType.ToUpper().Trim() ?? "")
            {
                case "STRING":
                    {
                        eDataType = SqlDbType.VarChar;
                        vParmValue = vParmValue.ToString();
                        break;
                    }
                case "INTEGER":
                    {
                        eDataType = SqlDbType.SmallInt;
                        vParmValue = Conversions.ToInteger(vParmValue);
                        break;
                    }
                case "SINGLE":
                    {
                        eDataType = SqlDbType.Real;
                        vParmValue = Conversions.ToSingle(vParmValue);
                        break;
                    }
                case "DOUBLE":
                    {
                        eDataType = SqlDbType.Float;
                        vParmValue = Conversions.ToDouble(vParmValue);
                        break;
                    }
                case "LONG":
                    {
                        eDataType = SqlDbType.Int;
                        vParmValue = Conversions.ToLong(vParmValue);
                        break;
                    }
                case "DATE":
                    {
                        eDataType = SqlDbType.Date;
                        vParmValue = Conversions.ToDate(vParmValue);
                        break;
                    }

                default:
                    {
                        throw new Exception(string.Format("Attempt to pass a unsupported Parameter type ({0}).", sParmType.Trim()));
                    }
            }

            parameter = new SqlParameter();
            // Set values...
            parameter.ParameterName = GetParameterName(sParmName, ref parameters);
            parameter.SqlDbType = eDataType; // adVarChar
            parameter.Value = vParmValue;
            // Add parameter to collection
            if (parameters is null)
                parameters = new BaseCollection();
            parameters.Add(parameter.ParameterName, parameter);
        }

        private string GetParameterName(string sParmName, ref BaseCollection parameters)
        {
            if (!string.IsNullOrEmpty(sParmName))
            {
                if (sParmName.StartsWith("@"))
                    return StripQuotes(sParmName);
                return "@" + StripQuotes(sParmName);
            }

            if (parameters is null)
                return "Param0";
            return string.Format("Param{0}", parameters.Count.ToString());
        }

        private void RenameAttachment(string sTableName, string sTableId, string sNewAttachmentName, string sResult, ref LinkScriptVariables privateVariables, SqlConnection conn)
        {
            object vResult = string.Empty;
            LinkScriptVariable.VariableTypes eVarType;

            if (string.IsNullOrEmpty(sTableName))
                throw new Exception("Error - TableName for command \"RenameAttachment\", not defined.");
            if (string.IsNullOrEmpty(sTableId))
                throw new Exception("Error - TableId for command \"RenameAttachment\", not defined.");
            if (string.IsNullOrEmpty(sNewAttachmentName))
                throw new Exception("Error - NewAttachmentName for command \"RenameAttachment\", not defined.");
            // Get TableName value...
            eVarType = GetVariableTypeAndSetValue(sTableName, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - TableName for command \"RenameAttachment\", cannot be a recordset.");
            sTableName = CurrentVariable.ToString();
            // Get TableId value...
            eVarType = GetVariableTypeAndSetValue(sTableId, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - TableId for command \"RenameAttachment\", cannot be a recordset.");
            sTableId = CurrentVariable.ToString();
            // Get TableId value...
            eVarType = GetVariableTypeAndSetValue(sNewAttachmentName, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - NewAttachmentName for command \"RenameAttachment\", cannot be a recordset.");
            sNewAttachmentName = CurrentVariable.ToString();

            var currentTable = ScriptEngine.GetTableRow(sTableName, conn);
            if (currentTable is null)
                throw new Exception(string.Format("Error - TableName for command \"RenameAttachment\", is not a valid Table Name (\"{0}\".)", sTableName));
            if (currentTable is not null && !Navigation.CBoolean(currentTable["Attachments"]))
                throw new Exception(string.Format("Error - TableName for command \"RenameAttachment\", does not allow attachments (\"{0}\".)", sTableName));

            string sql = "SELECT Attachments.PageNumber AS PageNumber,ISNULL(Trackables.RecordTypesId, 1) AS RecordType, Attachments.[FileName], Attachments.TrackablesId, Attachments.PointerId AS PointerId, OrgFullPath, OrgFileName " + "FROM " + "   (SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, ImagePointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, PageNumber, [FileName] FROM ImagePointers " + "    UNION " + "    SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, PCFilesPointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, 1 AS PageNumber, [FileName] FROM PCFilesPointers) " + "    AS Attachments " + "INNER JOIN       Trackables ON Attachments.TrackablesRecordVersion = Trackables.RecordVersion " + "                           AND Attachments.TrackablesId = Trackables.Id " + "INNER JOIN        Userlinks ON Trackables.Id = Userlinks.TrackablesId " + "INNER JOIN           Tables ON UserLinks.IndexTable = Tables.TableName " + "WHERE (UserLinks.IndexTable = @tableName) " + "  AND (UserLinks.IndexTableId = @tableId) " + "  AND (UserLinks.AttachmentNumber = @attachmentNumber) " + "ORDER BY Attachments.TrackablesId, ABS(Attachments.TrackablesRecordVersion), Attachments.PageNumber";
            var dt = new DataTable();

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", sTableName);
                cmd.Parameters.AddWithValue("@tableId", Navigation.PrepPad(sTableName, sTableId, _passport));
                cmd.Parameters.AddWithValue("@attachmentNumber", _attachmentNumber);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0)
            {
                vResult = "Data was not found";
            }
            else
            {
                sql = "UPDATE ImagePointers SET OrgFileName = @newAttachmentName WHERE TrackablesId = @trackablesId";
                if (Conversions.ToInteger(dt.Rows[0]["RecordType"]) == 5)
                    sql = "UPDATE PCFilesPointers SET OrgFileName = @newAttachmentName WHERE TrackablesId = @trackablesId";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@newAttachmentName", sNewAttachmentName);
                    cmd.Parameters.AddWithValue("@trackablesId", Conversions.ToInteger(dt.Rows[0]["TrackablesId"]));
                    cmd.ExecuteNonQuery();
                }

                string beforeData = "Old Attachment Name: ";
                string afterData = string.Format("New Attachment Name: {0}", sNewAttachmentName);

                if (!string.IsNullOrEmpty(dt.Rows[0]["OrgFileName"].ToString()))
                {
                    beforeData += dt.Rows[0]["OrgFileName"].ToString();
                }
                else if (!string.IsNullOrEmpty(dt.Rows[0]["OrgFullPath"].ToString()))
                {
                    beforeData += System.IO.Path.GetFileName(dt.Rows[0]["OrgFullPath"].ToString());
                }

                {
                    ref var withBlock = ref AuditType.LinkScriptAudit;
                    withBlock.AttachmentNumber = _attachmentNumber;
                    withBlock.TableName = sTableName;
                    withBlock.TableId = sTableId;
                    withBlock.ClientIpAddress = _passport.ServerIpAddress; // HeyReggie
                    withBlock.ActionType = AuditType.LinkScriptActionType.RenameAttachment;
                    withBlock.AfterData = afterData;
                    withBlock.BeforeData = beforeData;
                }

                Auditing.AuditUpdates(AuditType.LinkScriptAudit, _passport);
            }

            if (!SetResultValue(sResult, vResult, ref privateVariables))
                throw new Exception("Error - Result for command \"RenameAttachment\", not defined.");
        }

        private string StripQuotes(string stripThis, string quote = "\"")
        {
            try
            {
                if (string.IsNullOrEmpty(stripThis))
                    return string.Empty;
                if (string.Compare(stripThis.Substring(0, 1), quote) != 0)
                    return stripThis;
                if (string.Compare(stripThis.Substring(stripThis.Length - 1, 1), quote) != 0)
                    return stripThis;
                return stripThis.Substring(1, stripThis.Length - 2);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return stripThis;
            }
        }

        private void DimVariable(string sVarName, LinkScriptVariable.VariableTypes eVarType, LinkScriptVariable.VarScope eScope, ref LinkScriptVariables privateVariables)
        {
            LinkScriptVariable scriptVariable;

            switch (eScope)
            {
                case LinkScriptVariable.VarScope.vsPublic:
                    {
                        // Does it already exists?
                        if (!VariableExists(sVarName))
                        {
                            scriptVariable = new LinkScriptVariable();
                            scriptVariable.VariableType = eVarType;

                            if (scriptVariable.VariableType == LinkScriptVariable.VariableTypes.vtDataTable)
                                scriptVariable.Value = new DataWithCursor();
                            if (_publicVariables is null)
                                _publicVariables = new LinkScriptVariables();

                            _publicVariables.Add(sVarName, scriptVariable);
                            scriptVariable = null;
                        }
                        else
                        {
                            scriptVariable = GetVariable(sVarName);

                            if (scriptVariable is not null && scriptVariable.VariableType != eVarType)
                            {
                                throw new Exception(string.Format("Error - Public Variable \"{0}\" already defined. but as a different Type", sVarName));
                            }

                            scriptVariable = null;
                        }

                        break;
                    }
                case LinkScriptVariable.VarScope.vsPrivate:
                    {
                        // Does it already exists?
                        if (!VariableExists(sVarName, ref privateVariables))
                        {
                            scriptVariable = new LinkScriptVariable();
                            scriptVariable.VariableType = eVarType;

                            if (scriptVariable.VariableType == LinkScriptVariable.VariableTypes.vtDataTable)
                                scriptVariable.Value = new DataWithCursor();
                            if (privateVariables is null)
                                privateVariables = new LinkScriptVariables();

                            privateVariables.Add(sVarName, scriptVariable);
                            scriptVariable = null;
                        }
                        else
                        {
                            scriptVariable = GetVariable(sVarName, ref privateVariables);

                            if (scriptVariable is not null && scriptVariable.VariableType != eVarType)
                            {
                                throw new Exception(string.Format("Error - Private Variable \"{0}\" already defined. but as a different type", sVarName));
                            }

                            scriptVariable = null;
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private bool VariableExists(string variableName)
        {
            if (_publicVariables is null)
                return false;

            try
            {
                return _publicVariables.Item(variableName) is not null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private bool VariableExists(string variableName, ref LinkScriptVariables privateVariables)
        {
            if (privateVariables is null)
                return false;

            try
            {
                return privateVariables.Item(variableName) is not null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private int FlowControl(RecordsManage.LinkScriptRow scriptLine, int currentIndex, ref int loopStart, RecordsManage.LinkScriptDataTable activeLinkScript, ref LinkScriptVariables privateVariables, ref bool restartingElseIf, ref object selectCase, SqlConnection conn)
        {
            string sCommand;
            string tableName;
            LinkScriptVariable.VariableTypes eType;
            object argOne;
            object argTwo;
            object argThree;
            ;
            sCommand = this.NormalizeCommand(scriptLine.Command);

            switch (sCommand ?? "")
            {
                // Do loops...
                case "LOOP":
                    {
                        return loopStart;
                    }
                case "EXITDO":
                    {
                        currentIndex = FlowControl(currentIndex, activeLinkScript, "LOOP", string.Empty, "DOUNTIL|DOWHILE", "LOOP", ref restartingElseIf);
                        if (currentIndex <= activeLinkScript.Count)
                            loopStart = PopLoopStack(ref loopStart);
                        return currentIndex;
                    }
                case "DOUNTIL":
                    {
                        // Add to Loop Stack...
                        PushLoopStack(currentIndex);
                        // Get value for Arg One...
                        tableName = string.Empty;
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            argOne = ((LinkScriptVariable)CurrentVariable).Value;
                            tableName = ((LinkScriptVariable)CurrentVariable).TableName;
                        }
                        else
                        {
                            argOne = CurrentVariable;
                        }
                        // DebugPrint("{0} Arg 1 {1}={2}", "DoUntil", scriptLine.ArgOne, argOne.ToString)
                        // Get value for Arg Three...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgThree, ref privateVariables);
                        argThree = CurrentVariable;
                        // DebugPrint("{0} Arg 3 {1}={2}", "DoUntil", scriptLine.ArgThree, argThree.ToString)
                        // Did we mmet the comparison?
                        if (this.FlowCompare(argOne, scriptLine.ArgTwo, argThree, scriptLine, conn, ref tableName))
                        {
                            // Criteria met, exit Do...
                            currentIndex = FlowControl(currentIndex, activeLinkScript, "LOOP", string.Empty, "DOUNTIL|DOWHILE", "LOOP", ref restartingElseIf);
                            if (currentIndex <= activeLinkScript.Count)
                                loopStart = PopLoopStack(ref loopStart);
                            return currentIndex;
                        }
                        else
                        {
                            // Criteria not met, Continue...
                            loopStart = currentIndex;
                            return currentIndex + 1;
                        }
                    }
                case "DOWHILE":
                    {
                        // Add to Loop Stack...
                        PushLoopStack(currentIndex);
                        // Get value for Arg One...
                        tableName = string.Empty;
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            argOne = ((LinkScriptVariable)CurrentVariable).Value;
                            tableName = ((LinkScriptVariable)CurrentVariable).TableName;
                        }
                        else
                        {
                            argOne = CurrentVariable;
                        }
                        // Get value for Arg Three...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgThree, ref privateVariables);
                        argThree = CurrentVariable;
                        // Did we mmet the comparison?
                        if (this.FlowCompare(argOne, scriptLine.ArgTwo, argThree, scriptLine, conn, ref tableName))
                        {
                            // Criteria met, Continue...
                            loopStart = currentIndex;
                            return currentIndex + 1;
                        }
                        else
                        {
                            // Criteria not met, exit Do...
                            currentIndex = FlowControl(currentIndex, activeLinkScript, "LOOP", string.Empty, "DOUNTIL|DOWHILE", "LOOP", ref restartingElseIf);
                            if (currentIndex <= activeLinkScript.Count)
                                loopStart = PopLoopStack(ref loopStart);
                            return currentIndex;
                        }
                    }
                // For loops...
                case "NEXT":
                    {
                        this.Math(scriptLine.ArgOne, (LinkScriptVariable.MathTypes)0, "1", scriptLine.ArgOne, ref privateVariables);
                        return loopStart;
                    }
                case "EXITFOR":
                    {
                        currentIndex = FlowControl(currentIndex, activeLinkScript, "NEXT", string.Empty, "FOR", "NEXT", ref restartingElseIf);
                        if (currentIndex <= activeLinkScript.Count)
                            loopStart = PopLoopStack(ref loopStart);
                        return currentIndex;
                    }
                case "FOR":
                    {
                        // Get value for Arg Three...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgThree, ref privateVariables);
                        argThree = CurrentVariable;
                        // On round one set the starting variable
                        if (loopStart == 0)
                        {
                            // Get value for Arg two...
                            eType = this.GetVariableTypeAndSetValue(scriptLine.ArgTwo, ref privateVariables);
                            argOne = CurrentVariable;
                            this.SetResultValue(scriptLine.ArgOne, argOne, ref privateVariables);
                        }
                        // Get value for Arg two...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                        argOne = CurrentVariable;
                        // Add to Loop Stack...
                        PushLoopStack(currentIndex);
                        // Did we meet the comparison?
                        string argtableName = "";
                        if (FlowCompare(argOne, "<=", argThree, scriptLine, conn, tableName: ref argtableName))
                        {
                            // Criteria met, Continue...
                            loopStart = currentIndex;
                            return currentIndex + 1;
                        }
                        else
                        {
                            // Criteria not met, exit For...
                            currentIndex = FlowControl(currentIndex, activeLinkScript, "NEXT", string.Empty, "FOR", "NEXT", ref restartingElseIf);
                            if (currentIndex <= activeLinkScript.Count)
                                loopStart = PopLoopStack(ref loopStart);
                            return currentIndex;
                        }
                    }
                // If/Else...
                case "ENDIF":
                    {
                        return currentIndex + 1;
                    }
                case "ELSE":
                    {
                    lbl_ElseCase:

                        return FlowControl(currentIndex, activeLinkScript, "ENDIF", string.Empty, "IF", "ENDIF", ref restartingElseIf);
                    }
                case "ELSEIF":
                    {
                        if (!restartingElseIf)
                        {
                            // Do what Else does
                            //goto lbl_ElseCase;
                            return FlowControl(currentIndex, activeLinkScript, "ENDIF", string.Empty, "IF", "ENDIF", ref restartingElseIf);
                        }
                        else
                        {
                            restartingElseIf = false;
                            // Do what If Does
                            //goto lbl_IfCase;
                            tableName = string.Empty;
                            eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                            if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                            {
                                argOne = ((LinkScriptVariable)CurrentVariable).Value;
                                tableName = ((LinkScriptVariable)CurrentVariable).TableName;
                            }
                            else
                            {
                                argOne = CurrentVariable;
                            }
                            // Get value for Arg Three...
                            if (string.IsNullOrEmpty(scriptLine.ArgThree))
                            {
                                argThree = string.Empty;
                            }
                            else
                            {
                                eType = this.GetVariableTypeAndSetValue(scriptLine.ArgThree, ref privateVariables);
                                argThree = CurrentVariable;
                            }
                            // Did we met the comparison?
                            if (this.FlowCompare(argOne, scriptLine.ArgTwo, argThree, scriptLine, conn, ref tableName))
                            {
                                // Criteria met, Continue...
                                return currentIndex + 1;
                            }
                            else
                            {
                                // Criteria not met, Go to Else...
                                return FlowControl(currentIndex, activeLinkScript, "ELSE|ELSEIF|ENDIF", "ELSEIF", "IF", "ENDIF", ref restartingElseIf);
                            }
                        }
                    }
                case "IF":
                    {
                        //lbl_IfCase:;

                        // If...
                        // Get value for Arg One...
                        tableName = string.Empty;
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            argOne = ((LinkScriptVariable)CurrentVariable).Value;
                            tableName = ((LinkScriptVariable)CurrentVariable).TableName;
                        }
                        else
                        {
                            argOne = CurrentVariable;
                        }
                        // Get value for Arg Three...
                        if (string.IsNullOrEmpty(scriptLine.ArgThree))
                        {
                            argThree = string.Empty;
                        }
                        else
                        {
                            eType = this.GetVariableTypeAndSetValue(scriptLine.ArgThree, ref privateVariables);
                            argThree = CurrentVariable;
                        }
                        // Did we met the comparison?
                        if (this.FlowCompare(argOne, scriptLine.ArgTwo, argThree, scriptLine, conn, ref tableName))
                        {
                            // Criteria met, Continue...
                            return currentIndex + 1;
                        }
                        else
                        {
                            // Criteria not met, Go to Else...
                            return FlowControl(currentIndex, activeLinkScript, "ELSE|ELSEIF|ENDIF", "ELSEIF", "IF", "ENDIF", ref restartingElseIf);
                        }
                    }
                case "SELECTCASE":
                    {
                        // Get value for Arg One...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                        selectCase = CurrentVariable;
                        return FlowControl(currentIndex, activeLinkScript, "CASE|CASEELSE|ENDSELECT", "CASE|CASEELSE", "SELECTCASE", "ENDSELECT", ref restartingElseIf);
                    }
                case "CASE":
                case "CASEELSE":
                    {
                        if (!restartingElseIf)
                        {
                            // Dropped into so goto end
                            return FlowControl(currentIndex, activeLinkScript, "ENDSELECT", string.Empty, "SELECTCASE", "ENDSELECT", ref restartingElseIf);
                        }
                        else
                        {
                            restartingElseIf = false;

                            if (sCommand == "CASEELSE")
                            {
                                // CaseElse we are ready to run the next line...
                                return currentIndex + 1;
                            }
                            else
                            {
                                // Case To Check...
                                // Get value for Arg One...
                                eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                                argOne = CurrentVariable;
                                // Did we met the comparison?
                                string argtableName1 = "";
                                if (FlowCompare(selectCase, "=", argOne, scriptLine, conn, tableName: ref argtableName1))
                                {
                                    // Criteria met, Continue...
                                    return currentIndex + 1;
                                }
                                else
                                {
                                    // Criteria not met, Go to Next Case...
                                    return FlowControl(currentIndex, activeLinkScript, "CASE|CASEELSE|ENDSELECT", "CASE|CASEELSE", "SELECTCASE", "ENDSELECT", ref restartingElseIf);
                                }
                            }
                        }
                    }
                // Select Case/Case/End Select...
                case "ENDSELECT":
                    {
                        return currentIndex + 1;
                    }
                case "RUNNEWSCRIPT":
                    {
                        // Clear Private Vars previously declared...
                        ClearPrivateVariables(ref privateVariables);
                        // Get value for Arg One...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                        argOne = CurrentVariable;
                        // Get value for Arg Two...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgTwo, ref privateVariables);
                        argTwo = CurrentVariable;

                        if (string.Compare(argTwo.ToString(), "TRUE", true) == 0)
                        {
                            // Clear Public variables previously defined...
                            ClearPublicVariables();
                            // Clear Controls previously defined...
                            ClearControls?.Invoke();
                        }
                        // Run the new script...
                        _scriptName = Strings.Trim(argOne.ToString());
                        ProcessScript(false);
                        // Ends original calling script...
                        return -1;
                    }
                case "GOSUBSCRIPT":
                    {
                        // Add to Loop Stack...
                        PushLoopStack(currentIndex);
                        // Get value for Arg One...
                        eType = this.GetVariableTypeAndSetValue(scriptLine.ArgOne, ref privateVariables);
                        argOne = CurrentVariable;
                        // Run the new script...
                        _scriptName = Strings.Trim(argOne.ToString());
                        ProcessScript(true);
                        // Reset the Current ScriptName to the one we were in before we branched
                        _scriptName = Strings.Trim(scriptLine.ScriptName);
                        if (!_showPrompt & currentIndex <= activeLinkScript.Count)
                            loopStart = PopLoopStack(ref loopStart);
                        return currentIndex + 1;
                    }
                case "EXITSCRIPT":
                    {
                        // Ends script...
                        return -1;
                    }
            }
            // Return...
            return default;

        lbl_FlowControl:
            ;

            if (this.ScriptMsgBox("Critical Errors " + Information.Err().Number.ToString() + " " + Information.Err().Description + ". (ScriptEngine.FlowControl)", MsgBoxStyle.RetryCancel | MsgBoxStyle.Exclamation, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "-" + Strings.Trim(scriptLine.ScriptName) + "(" + scriptLine.ScriptSequence.ToString() + ")") == MsgBoxResult.Retry)
            {
                //                ;
                //#error Cannot convert ResumeStatementSyntax - see comment for details
                /* Cannot convert ResumeStatementSyntax, CONVERSION ERROR: Conversion for ResumeStatement not implemented, please report this issue in 'Resume' at character 153920


                Input:
                            Resume

                 */
            }
            else
            {
                return -1;
            }
        }

        private void Flow()
        {

        }

        private int FlowControl(int currentIndex, RecordsManage.LinkScriptDataTable activeLinkScript, string commandsToCheck, string commandsThatRestart, string commandNestingPlus, string commandNestingMinus, ref bool restartingElseIf)
        {
            if (currentIndex > activeLinkScript.Count)
                return currentIndex + 1;

            currentIndex += 1;
            int nesting = 0;
            commandsToCheck = string.Format("|{0}|", commandsToCheck.ToUpper());
            commandNestingPlus = string.Format("|{0}|", commandNestingPlus.ToUpper());
            commandNestingMinus = string.Format("|{0}|", commandNestingMinus.ToUpper());
            commandsThatRestart = string.Format("|{0}|", commandsThatRestart.ToUpper());

            var scriptLine = activeLinkScript[currentIndex];
            string sCommand = string.Format("|{0}|", this.NormalizeCommand(scriptLine.Command));

            while (!(commandsToCheck.Contains(sCommand) & nesting == 0))
            {
                if (commandNestingPlus.Contains(sCommand))
                    nesting += 1;
                if (commandNestingMinus.Contains(sCommand))
                    nesting -= 1;

                currentIndex += 1;
                if (currentIndex > activeLinkScript.Count)
                    break;

                scriptLine = activeLinkScript[currentIndex];
                sCommand = string.Format("|{0}|", this.NormalizeCommand(scriptLine.Command));
            }

            if (commandsThatRestart.Contains(sCommand))
            {
                restartingElseIf = true;
                return currentIndex;
            }
            else
            {
                return currentIndex + 1;
            }
        }

        private bool FlowCompare(object vFactorOne, string sCommand, object vFactorThree, RecordsManage.LinkScriptRow scriptLine, SqlConnection conn, [Optional, DefaultParameterValue("")] ref string tableName)
        {
            bool FlowCompareRet = default;
            string originalFactorOne = string.Empty;

            FlowCompareRet = false;
            if (vFactorOne is not null)
                originalFactorOne = vFactorOne.ToString();
            // The conversion theory here is that if one is a string try and convert it to the type of the other.
            // If it fails the conversion convert the other type to a string.
            ConvertArguments(ref vFactorOne, ref vFactorThree);

            if (string.Compare(sCommand, "~=", true) == 0)
            {
                if (string.Compare(vFactorOne.ToString(), vFactorThree.ToString(), true) == 0)
                    FlowCompareRet = true;
            }
            else
            {
                if (vFactorOne is DBNull)
                {
                    vFactorOne = " ";
                }
                else if (string.Compare(vFactorOne.ToString(), "FALSE", true) == 0)
                {
                    vFactorOne = "0";
                }
                else if (string.Compare(vFactorOne.ToString(), "TRUE", true) == 0)
                {
                    vFactorOne = "-1";
                }
                else if (vFactorOne.ToString().Length == 0)
                {
                    vFactorOne = "0";
                }
                if (vFactorThree is DBNull)
                {
                    vFactorThree = " ";
                }
                else if (string.Compare(vFactorThree.ToString(), "FALSE", true) == 0)
                {
                    vFactorThree = "0";
                }
                else if (string.Compare(vFactorThree.ToString(), "TRUE", true) == 0)
                {
                    vFactorThree = "-1";
                }
                else if (vFactorThree.ToString().Length == 0)
                {
                    vFactorThree = "0";
                }

                FlowCompareRet = CompareArguments(vFactorOne, vFactorThree, originalFactorOne, sCommand, scriptLine, conn, ref tableName);
            }

            return FlowCompareRet;
        }

        private bool CompareArguments(object vFactorOne, object vFactorThree, string originalFactorOne, string sCommand, RecordsManage.LinkScriptRow scriptLine, SqlConnection conn, [Optional, DefaultParameterValue("")] ref string tableName)
        {
            DataRow table;
            DataRow parentTable;
            DataWithCursor tableVariable;

            try
            {
                switch (sCommand.ToUpper().Trim() ?? "")
                {
                    case "=":
                        {
                            try
                            {
                                if (Conversions.ToDouble(vFactorOne) == Conversions.ToDouble(vFactorThree))
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                return string.Compare(vFactorOne.ToString(), vFactorThree.ToString()) == 0;
                            }

                            break;
                        }
                    case "<>":
                        {
                            try
                            {
                                if (Conversions.ToDouble(vFactorOne) != Conversions.ToDouble(vFactorThree))
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                return string.Compare(vFactorOne.ToString(), vFactorThree.ToString()) != 0;
                            }

                            break;
                        }
                    case ">":
                        {
                            try
                            {
                                if (Conversions.ToDouble(vFactorOne) > Conversions.ToDouble(vFactorThree))
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                return string.Compare(vFactorOne.ToString(), vFactorThree.ToString()) > 0;
                            }

                            break;
                        }
                    case ">=":
                        {
                            try
                            {
                                if (Conversions.ToDouble(vFactorOne) >= Conversions.ToDouble(vFactorThree))
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                return string.Compare(vFactorOne.ToString(), vFactorThree.ToString()) >= 0;
                            }

                            break;
                        }
                    case "<":
                        {
                            try
                            {
                                if (Conversions.ToDouble(vFactorOne) < Conversions.ToDouble(vFactorThree))
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                return string.Compare(vFactorOne.ToString(), vFactorThree.ToString()) < 0;
                            }

                            break;
                        }
                    case "<=":
                        {
                            try
                            {
                                if (Conversions.ToDouble(vFactorOne) <= Conversions.ToDouble(vFactorThree))
                                    return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                return string.Compare(vFactorOne.ToString(), vFactorThree.ToString()) <= 0;
                            }

                            break;
                        }
                    case "ISEMPTY":
                        {
                            return string.IsNullOrEmpty(originalFactorOne.Trim());
                        }
                    case "ISNUMERIC":
                        {
                            return Information.IsNumeric(originalFactorOne);
                        }
                    case "ISINTEGER":
                        {
                            if (Information.IsNumeric(originalFactorOne))
                                return !originalFactorOne.Contains(".");
                            return false;
                        }
                    case "ISDATE":
                        {
                            return Information.IsDate(originalFactorOne);
                        }
                    case "HASCHILDREN":
                        {
                            if (Information.VarType(vFactorOne) != VariantType.Object)
                            {
                                throw new Exception(string.Format("Flow Compare Argument One must be a recordset if using HasChildren - {0} [{1}].", scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                            }
                            else
                            {
                                tableVariable = (DataWithCursor)vFactorOne;
                                parentTable = ScriptEngine.GetTableRow(tableName, conn);

                                if (parentTable is null)
                                {
                                    throw new Exception(string.Format("Flow Compare Argument One must be a recordset if using HasChildren - {0} [{1}].", scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                                }
                                else if (tableVariable is not null)
                                {
                                    if (!tableVariable.EOF)
                                    {
                                        var oField = Navigation.FieldWithOrWithoutTable(parentTable["IdFieldName"].ToString(), tableVariable.Columns);

                                        if (oField is not null)
                                        {
                                            var oRelationships = Navigation.GetLowerRelationships(parentTable["TableName"].ToString(), parentTable["IdFieldName"].ToString(), conn);

                                            foreach (DataRow row in oRelationships.Rows)
                                            {
                                                table = ScriptEngine.GetTableRow(row["LowerTableName"].ToString(), conn);

                                                if (table is not null)
                                                {
                                                    string sql = "SELECT TOP 1 [" + row["LowerTableFieldName"].ToString() + "] FROM " + row["LowerTableName"].ToString() + " WHERE [" + row["LowerTableFieldName"].ToString() + "] = @fieldName";

                                                    using (var cmd = new SqlCommand(sql, conn))
                                                    {
                                                        cmd.Parameters.AddWithValue("@fieldName", tableVariable.CurrentRow[oField.Ordinal].ToString());
                                                        if (cmd.ExecuteScalar() is not null && cmd.ExecuteScalar().ToString().Length > 0)
                                                            return true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    case "HASATTACHMENTS":
                        {
                            if (Information.VarType(vFactorOne) != VariantType.Object)
                            {
                                throw new Exception(string.Format("Flow Compare Argument One must be a recordset if using HasAttachments - {0} [{1}].", scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                            }
                            else
                            {
                                tableVariable = (DataWithCursor)vFactorOne;
                                parentTable = ScriptEngine.GetTableRow(tableName, conn);

                                if (parentTable is null)
                                {
                                    throw new Exception(string.Format("Flow Compare Argument One must be a recordset if using HasAttachments - {0} [{1}].", scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                                }
                                else if (Navigation.CBoolean(parentTable["Attachments"]))
                                {
                                    if (tableVariable is not null)
                                    {
                                        if (!tableVariable.EOF)
                                        {
                                            var oField = Navigation.FieldWithOrWithoutTable(parentTable["IdFieldName"].ToString(), tableVariable.Columns);

                                            if (oField is not null)
                                            {
                                                string sql = "SELECT TOP 1 [Id] FROM [Userlinks] WHERE [IndexTable] = @tableName AND [IndexTableId] = @tableId";

                                                using (var cmd = new SqlCommand(sql, conn))
                                                {
                                                    cmd.Parameters.AddWithValue("@tableName", parentTable["TableName"].ToString());
                                                    cmd.Parameters.AddWithValue("@tableId", tableVariable.CurrentRow[oField.Ordinal].ToString());
                                                    if (cmd.ExecuteScalar() is null)
                                                        return false;
                                                    return cmd.ExecuteScalar().ToString().Length > 0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    default:
                        {
                            throw new Exception(string.Format("Flow Compare with invalid Compare Type({0})  - {1} [{2}].", sCommand.Trim(), scriptLine.ScriptName.Trim(), scriptLine.ScriptSequence.ToString().Trim()));
                        }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return default;
        }

        private void ConvertArguments(ref object vFactorOne, ref object vFactorThree)
        {
            ;

            if (vFactorOne is null)
                vFactorOne = string.Empty;
            if (vFactorThree is null)
                vFactorThree = string.Empty;

            if (Information.VarType(vFactorOne) != Information.VarType(vFactorThree))
            {
                if (Information.VarType(vFactorOne) == VariantType.String)
                {
                    switch (Information.VarType(vFactorThree))
                    {
                        case VariantType.Array:
                            {
                                break;
                            }
                        case VariantType.Boolean:
                            {
                                vFactorOne = (object)Navigation.CBoolean(vFactorOne);
                                break;
                            }
                        case VariantType.Byte:
                            {
                                vFactorOne = Conversions.ToByte(vFactorOne);
                                break;
                            }
                        case VariantType.Decimal:
                            {
                                vFactorOne = Conversions.ToDecimal(vFactorOne);
                                break;
                            }
                        case VariantType.Date:
                            {
                                vFactorOne = Conversions.ToDate(vFactorOne).ToOADate();
                                break;
                            }
                        case var @case when @case == VariantType.Decimal:
                            {
                                vFactorOne = Conversions.ToDecimal(vFactorOne);
                                break;
                            }
                        case VariantType.Double:
                            {
                                vFactorOne = Conversions.ToDouble(vFactorOne);
                                break;
                            }
                        case VariantType.Empty:
                            {
                                break;
                            }
                        case VariantType.Error:
                            {
                                break;
                            }
                        case VariantType.Short:
                            {
                                vFactorOne = Conversions.ToShort(vFactorOne);
                                break;
                            }
                        case VariantType.Integer:
                            {
                                vFactorOne = Conversions.ToInteger(vFactorOne);
                                break;
                            }
                        case VariantType.Null:
                            {
                                break;
                            }
                        case VariantType.Object:
                            {
                                break;
                            }
                        case VariantType.Single:
                            {
                                vFactorOne = Conversions.ToSingle(vFactorOne);
                                break;
                            }
                        case VariantType.String:
                            {
                                break;
                            }
                        case VariantType.UserDefinedType:
                            {
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }

                    if (Information.Err().Number != 0)
                        vFactorThree = vFactorThree.ToString();
                }
                else if (Information.VarType(vFactorThree) == VariantType.String)
                {
                    switch (Information.VarType(vFactorOne))
                    {
                        case VariantType.Array:
                            {
                                break;
                            }
                        case VariantType.Boolean:
                            {
                                vFactorThree = (object)Navigation.CBoolean(vFactorThree);
                                break;
                            }
                        case VariantType.Byte:
                            {
                                vFactorThree = Conversions.ToByte(vFactorThree);
                                break;
                            }
                        case VariantType.Decimal:
                            {
                                vFactorThree = Conversions.ToDecimal(vFactorThree);
                                break;
                            }
                        case VariantType.Date:
                            {
                                vFactorThree = Conversions.ToDate(vFactorThree).ToOADate();
                                break;
                            }
                        case var case1 when case1 == VariantType.Decimal:
                            {
                                vFactorThree = Conversions.ToDecimal(vFactorThree);
                                break;
                            }
                        case VariantType.Double:
                            {
                                vFactorThree = Conversions.ToDouble(vFactorThree);
                                break;
                            }
                        case VariantType.Empty:
                            {
                                break;
                            }
                        case VariantType.Error:
                            {
                                break;
                            }
                        case VariantType.Short:
                            {
                                vFactorThree = Conversions.ToShort(vFactorThree);
                                break;
                            }
                        case VariantType.Integer:
                            {
                                vFactorThree = Conversions.ToInteger(vFactorThree);
                                break;
                            }
                        case VariantType.Null:
                            {
                                break;
                            }
                        case VariantType.Object:
                            {
                                break;
                            }
                        case VariantType.Single:
                            {
                                vFactorThree = Conversions.ToSingle(vFactorThree);
                                break;
                            }
                        case VariantType.String:
                            {
                                break;
                            }
                        case VariantType.UserDefinedType:
                            {
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }

                    if (Information.Err().Number != 0)
                        vFactorOne = vFactorOne.ToString();
                }
                else if (Information.VarType(vFactorOne) == VariantType.Date)
                {
                    if (Information.IsNumeric(vFactorThree))
                    {
                        if (Conversions.ToInteger(vFactorThree) == 0)
                            vFactorThree = new DateTime().ToOADate();
                        vFactorOne = Conversions.ToDate(vFactorOne).ToOADate();
                    }
                }
                else if (Information.VarType(vFactorThree) == VariantType.Date)
                {
                    if (Information.IsNumeric(vFactorOne))
                    {
                        if (Conversions.ToInteger(vFactorOne) == 0)
                            vFactorOne = new DateTime().ToOADate();
                        vFactorThree = Conversions.ToDate(vFactorThree).ToOADate();
                    }
                }
            }
        }

        private int GetCurrentVariableAsInteger(string variableName, LinkScriptVariables privateVariables, int start, ref LinkScriptVariable.VariableTypes eType)
        {
            try
            {
                string sTempIndex = variableName.Substring(start + 1, variableName.LastIndexOf(")") - start - 1);
                eType = GetVariableTypeAndSetValue(sTempIndex, ref privateVariables);
                return Conversions.ToInteger(CurrentVariable);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        private object GetCurrentVariableAsObject(string variableName, LinkScriptVariables privateVariables, int start, ref LinkScriptVariable.VariableTypes eType)
        {
            try
            {
                string sTempIndex = variableName.Substring(start + 1, variableName.LastIndexOf(")") - start - 1);
                eType = GetVariableTypeAndSetValue(sTempIndex, ref privateVariables);
                return CurrentVariable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private string GetCurrentVariableAsString(string variableName, LinkScriptVariables privateVariables, int start, ref LinkScriptVariable.VariableTypes eType)
        {
            try
            {
                string sTempIndex = variableName.Substring(start + 1, variableName.LastIndexOf(")") - start - 1);
                eType = GetVariableTypeAndSetValue(sTempIndex, ref privateVariables);
                return CurrentVariable.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private string ParsePropertyFromVariableName(string variableName, int start, int prefixLength)
        {
            if (prefixLength > 0)
            {
                if (start > 0)
                    return variableName.Substring(prefixLength + 1, start - prefixLength - 1).Trim().ToUpper();
                return variableName.Substring(prefixLength + 1).Trim().ToUpper();
            }
            else
            {
                if (start > 0)
                    return variableName.Substring(0, start).Trim().ToUpper();
                return variableName.Trim().ToUpper();
            }
        }

        private object GetArrayItemValue(Array arrayIn, int index, int dimension)
        {
            if (arrayIn is null || arrayIn.GetLength(dimension) == 0)
                return string.Empty;
            if (index >= arrayIn.GetLowerBound(dimension) & index <= arrayIn.GetUpperBound(dimension))
                return arrayIn.GetValue(index);
            return string.Empty;
        }

        private void SendEmail(string message, string toAddress, string fromAddress, object result, ref LinkScriptVariables privateVariables, SqlConnection conn)
        {
            LinkScriptVariable.VariableTypes varType;

            if (string.IsNullOrEmpty(message))
                throw new Exception("Error - Message for command \"SendEmail\", not defined.");
            if (string.IsNullOrEmpty(toAddress))
            {
                if (string.IsNullOrEmpty(Form.EmailToAddress))
                    throw new Exception("Error - ToAddress for command \"SendEmail\", not defined.");
            }

            if (string.IsNullOrEmpty(fromAddress))
            {
                if (string.IsNullOrEmpty(Form.EmailFromAddress))
                    throw new Exception("Error - FromAddress for command \"SendEmail\", not defined.");
            }

            varType = GetVariableTypeAndSetValue(message, ref privateVariables);
            if (varType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - Message for command \"SendEmail\", cannot be a recordset.");
            message = _variable.ToString();

            if (string.IsNullOrEmpty(toAddress))
            {
                toAddress = Form.EmailToAddress;
            }
            else
            {
                varType = GetVariableTypeAndSetValue(toAddress, ref privateVariables);
                if (varType == LinkScriptVariable.VariableTypes.vtDataTable)
                    throw new Exception("Error - ToAddress for command \"SendEmail\", cannot be a recordset.");
                toAddress = _variable.ToString();
            }

            if (string.IsNullOrEmpty(fromAddress))
            {
                fromAddress = Form.EmailFromAddress;
            }
            else
            {
                varType = GetVariableTypeAndSetValue(fromAddress, ref privateVariables);
                if (varType == LinkScriptVariable.VariableTypes.vtDataTable)
                    throw new Exception("Error - FromAddress for command \"SendEmail\", cannot be a recordset.");
                fromAddress = _variable.ToString();
            }

            Navigation.SendEmail(message, toAddress, fromAddress, Form.EmailSubject, Form.EmailAttachments, conn);
        }

        private void SetArrayItemIntegerValue(ref int[] arrayIn, int index, int dimension, object value)
        {
            if (index < 0)
                return;

            if (index >= arrayIn.GetLowerBound(dimension) & index <= arrayIn.GetUpperBound(dimension))
            {
                arrayIn.SetValue(value, index);
            }
            else
            {
                Array.Resize(ref arrayIn, index);
                arrayIn.SetValue(value, index);
            }
        }

        private void SetArrayItemStringValue(ref string[] arrayIn, int index, int dimension, object value)
        {
            if (index < 0)
                return;

            if (index >= arrayIn.GetLowerBound(dimension) & index <= arrayIn.GetUpperBound(dimension))
            {
                arrayIn.SetValue(value, index);
            }
            else
            {
                Array.Resize(ref arrayIn, index);
                arrayIn.SetValue(value, index);
            }
        }

        private object GetArrayPropertyValue(string variableProperty, string variableName, ref LinkScriptVariables privateVariables, int start, LinkScriptVariable.VariableTypes eType)
        {
            int index;

            switch (variableProperty ?? "")
            {
                case "_GETSELECTEDROWIDS":
                    {
                        index = GetCurrentVariableAsInteger(variableName, privateVariables, start, ref eType);
                        return GetArrayItemValue(_getSelectedRowIds, index - 1, 0);
                    }

                default:
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            index = GetCurrentVariableAsInteger(variableName, privateVariables, start, ref eType);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
            }

            switch (variableProperty ?? "")
            {
                case "_BARCODE":
                    {
                        return GetArrayItemValue(_barCode, index - 1, 0);
                    }
                case "_BARCODELEFT":
                    {
                        return GetArrayItemValue(_barCodeLeft, index - 1, 0);
                    }
                case "_BARCODETOP":
                    {
                        return GetArrayItemValue(_barCodeTop, index - 1, 0);
                    }
                case "_BARCODETYPE":
                    {
                        return GetArrayItemValue(_barCodeType, index - 1, 0);
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        private void SetArrayPropertyValue(string variableProperty, string variableName, object value, ref LinkScriptVariables privateVariables, int start, LinkScriptVariable.VariableTypes eType)
        {
            int index;

            switch (variableProperty ?? "")
            {
                case "_SETSELECTEDROWIDS":
                    {
                        index = GetCurrentVariableAsInteger(variableName, privateVariables, start, ref eType);
                        SetArrayItemStringValue(ref _setSelectedRowIds, index - 1, 0, value);
                        if (_setSelectedRowIdsCount < index)
                            _setSelectedRowIdsCount = index;
                        if ((_setSelectedRowIds[index - 1] ?? "") != (value.ToString() ?? ""))
                            _setSelectedRowIdsChanged = true;
                        break;
                    }

                default:
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            index = GetCurrentVariableAsInteger(variableName, privateVariables, start, ref eType);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
            }

            switch (variableProperty ?? "")
            {
                case "_BARCODE":
                    {
                        SetArrayItemStringValue(ref _barCode, index - 1, 0, value);
                        if (_barCodeCount < index)
                            _barCodeCount = index;
                        break;
                    }
                case "_BARCODELEFT":
                    {
                        SetArrayItemIntegerValue(ref _barCodeLeft, index - 1, 0, value);
                        if (_barCodeCount < index)
                            _barCodeCount = index;
                        break;
                    }
                case "_BARCODETOP":
                    {
                        SetArrayItemIntegerValue(ref _barCodeTop, index - 1, 0, value);
                        if (_barCodeCount < index)
                            _barCodeCount = index;
                        break;
                    }
                case "_BARCODETYPE":
                    {
                        SetArrayItemIntegerValue(ref _barCodeType, index - 1, 0, value);
                        if (_barCodeCount < index)
                            _barCodeCount = index;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private string GetFieldFromFieldName(string fieldName, ref LinkScriptVariables privateVariables)
        {
            try
            {
                var eType = GetVariableTypeAndSetValue(fieldName, ref privateVariables);
                if (eType == LinkScriptVariable.VariableTypes.vtString)
                    return CurrentVariable.ToString();
                return StripQuotes(fieldName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StripQuotes(fieldName);
            }
        }

        private object GetFieldValue(string variablePrefix, string fieldName, ref LinkScriptVariables privateVariables)
        {
            fieldName = GetFieldFromFieldName(fieldName, ref privateVariables);
            var eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);
            if (eType != LinkScriptVariable.VariableTypes.vtDataTable)
                return null;

            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
            DataColumn column = null;

            if (Information.IsNumeric(fieldName))
            {
                column = tableVariable.Columns[Conversions.ToInteger(fieldName)];
            }
            else
            {
                column = Navigation.FieldWithOrWithoutTable(fieldName, tableVariable.Columns);
            }

            if (column is null)
                return null;
            return tableVariable.CurrentRow[column.Ordinal];
        }

        private void SetFieldValue(string variablePrefix, string fieldName, ref LinkScriptVariables privateVariables, object vValue)
        {
            fieldName = GetFieldFromFieldName(fieldName, ref privateVariables);
            var eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);
            if (eType != LinkScriptVariable.VariableTypes.vtDataTable)
                return;

            string tableName = ((LinkScriptVariable)CurrentVariable).TableName;
            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
            DataColumn column = null;

            if (Information.IsNumeric(fieldName))
            {
                column = tableVariable.Columns[Conversions.ToInteger(fieldName)];
            }
            else
            {
                column = Navigation.FieldWithOrWithoutTable(fieldName, tableVariable.Columns);
            }

            if (column is not null)
            {
                string sBeforeValue = string.Empty;
                List<string> cFieldNames = null;
                List<string> cFieldBeforeValues = null;
                List<string> cFieldAfterValues = null;

                sBeforeValue = tableVariable.CurrentRow[column.Ordinal].ToString();
                cFieldNames = ((LinkScriptVariable)CurrentVariable).FieldNames;
                cFieldBeforeValues = ((LinkScriptVariable)CurrentVariable).FieldBeforeValues;
                cFieldAfterValues = ((LinkScriptVariable)CurrentVariable).FieldAfterValues;

                try
                {
                    var vInterimValue = SubstituteNull(column, vValue);

                    if (vInterimValue is DBNull)
                    {
                        tableVariable.CurrentRow[column.Ordinal] = vInterimValue;
                    }
                    else if (Navigation.IsADateType(column.DataType))
                    {
                        tableVariable.CurrentRow[column.Ordinal] = (object)DateTime.Parse(vInterimValue.ToString());
                    }
                    else if (ReferenceEquals(column.DataType, typeof(bool)))
                    {
                        tableVariable.CurrentRow[column.Ordinal] = (object)Navigation.CBoolean(vInterimValue);
                    }
                    else if (ReferenceEquals(column.DataType, typeof(int)))
                    {
                        tableVariable.CurrentRow[column.Ordinal] = (object)Conversions.ToInteger(vInterimValue);
                    }
                    else if (ReferenceEquals(column.DataType, typeof(long)))
                    {
                        tableVariable.CurrentRow[column.Ordinal] = (object)Conversions.ToLong(vInterimValue);
                    }
                    else if (ReferenceEquals(column.DataType, typeof(float)))
                    {
                        tableVariable.CurrentRow[column.Ordinal] = (object)Conversions.ToSingle(vInterimValue);
                    }
                    else if (ReferenceEquals(column.DataType, typeof(double)))
                    {
                        tableVariable.CurrentRow[column.Ordinal] = (object)Conversions.ToDouble(vInterimValue);
                    }
                    else
                    {
                        tableVariable.CurrentRow[column.Ordinal] = vInterimValue;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }

                if (string.Compare(sBeforeValue, tableVariable.CurrentRow[column.Ordinal].ToString(), true) != 0)
                {
                    string sFieldName = string.Empty;

                    for (int i = 0, loopTo = cFieldNames.Count - 1; i <= loopTo; i++)
                    {
                        sFieldName = cFieldNames[i];

                        if (string.Compare(sFieldName, Navigation.MakeSimpleField(column.ColumnName), true) == 0)
                        {
                            cFieldAfterValues[i] = tableVariable.CurrentRow[column.Ordinal].ToString();
                            break;
                        }

                        sFieldName = string.Empty;
                    }

                    if (string.IsNullOrEmpty(sFieldName) && (!string.IsNullOrEmpty(sBeforeValue) || !string.IsNullOrEmpty(tableVariable.CurrentRow[column.Ordinal].ToString())))
                    {
                        cFieldBeforeValues.Add(sBeforeValue);
                        cFieldNames.Add(Navigation.MakeSimpleField(column.ColumnName));
                        cFieldAfterValues.Add(tableVariable.CurrentRow[column.Ordinal].ToString());
                    }
                }
            }

            this.SetResultValue(variablePrefix, tableVariable, ref privateVariables);
        }

        private object SubstituteNull(DataColumn column, object vValue)
        {
            if (vValue is null && column.AllowDBNull)
                return DBNull.Value;
            if (string.IsNullOrEmpty(vValue.ToString().Trim()) && column.AllowDBNull)
                return DBNull.Value;
            if (Navigation.IsAStringType(column.DataType))
                return vValue;

            if (Navigation.IsADateType(column.DataType))
            {
                if (!Information.IsNumeric(vValue))
                    return vValue;
                if ((vValue.Equals(DateTime.Parse("1899-12-30")) || vValue.Equals(new DateTime())) && column.AllowDBNull)
                    return DBNull.Value;

                try
                {
                    if (Conversions.ToDouble(vValue.ToString().Trim()) == 0d && column.AllowDBNull)
                        return DBNull.Value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return vValue;
                }
            }
            else if (!ReferenceEquals(column.DataType, typeof(bool)))
            {
                try
                {
                    if (Conversions.ToDouble(vValue.ToString().Trim()) == 0d && column.AllowDBNull)
                        return DBNull.Value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return vValue;
                }
            }

            return vValue;
        }

        private object GetProperty(string variableName, ref LinkScriptVariables privateVariables)
        {
            object GetPropertyRet = default;
            int start = 0;
            int prefixLength = 0;
            string variablePrefix = variableName;
            string variableProperty = string.Empty;

            var eType = default(LinkScriptVariable.VariableTypes);

            GetPropertyRet = null;
            // Remove Index from property...
            start = variableName.IndexOf("(");
            // Getprefix to property (if exists)...
            prefixLength = variableName.IndexOf("._");
            if (prefixLength > 0)
                variablePrefix = variableName.Substring(0, prefixLength);
            // Getvarprop to property (if exists)...
            variableProperty = ParsePropertyFromVariableName(variableName, start, prefixLength);

            switch (variableProperty ?? "")
            {
                case "_ACTIVEUSER":
                    {
                        return _activeUser;
                    }
                case "_APPPATH":
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory;
                        if (path.EndsWith(Conversions.ToString(System.IO.Path.DirectorySeparatorChar)) || path.EndsWith(Conversions.ToString(System.IO.Path.AltDirectorySeparatorChar)))
                            return path.Substring(0, path.Length - 1);
                        return path;
                    }
                case "_APPTITLE":
                    {
                        return ScriptEngine.ProductName;
                    }
                case "_APPVERSION":
                    {
                        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major * 10 + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
                    }
                case "_APPREVISION":
                    {
                        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
                    }
                case "_ATTACHMENTNUMBER":
                    {
                        return _attachmentNumber;
                    }
                case "_ALLOWNULLDATE":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpAllowNullDate, ref GetPropertyRet);
                        break;
                    }
                case "_BACKCOLOR":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpBackColor, ref GetPropertyRet);
                        break;
                    }
                case "_BARCODECOUNT":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            return _barCodeCount;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_BARCODE":
                case "_BARCODELEFT":
                case "_BARCODETOP":
                case "_BARCODETYPE":
                    {
                        return GetArrayPropertyValue(variableProperty, variableName, ref privateVariables, start, eType);
                    }
                case "_BCADDITIONALFIELD":
                    {
                        return _BCAdditionalField;
                    }
                case "_BCADDITIONALFIELDTYPE":
                    {
                        return BCAdditionalFieldType;
                    }
                case "_BCADDITIONALMEMOFIELD":
                    {
                        return _BCAdditionalMemoField;
                    }
                case "_BCCHECKFORDUPS":
                    {
                        return _BCCheckForDups;
                    }
                case "_BCDESTINATIONCONTAINERNUMBER":
                    {
                        return _BCDestContainerNumber;
                    }
                case "_BCDESTINATIONID":
                    {
                        return _BCDestTableId;
                    }
                case "_BCDESTINATIONISACTIVE":
                    {
                        return BCDestActive.ToString().ToUpper();
                    }
                case "_BCDESTINATIONISTRACKABLE":
                    {
                        return BCDestTrackable.ToString().ToUpper();
                    }
                case "_BCDESTINATIONTABLENAME":
                    {
                        return _BCDestTableName;
                    }
                case "_BCDESTINATIONWINDOW":
                    {
                        return _BCDestWindow;
                    }
                case "_BCDUEBACKDATE":
                    {
                        return _BCDueBack;
                    }
                case "_BCOBJECTCONTAINERNUMBER":
                    {
                        return _BCObjContainerNumber;
                    }
                case "_BCOBJECTID":
                    {
                        return _BCObjTableId;
                    }
                case "_BCOBJECTISACTIVE":
                    {
                        return BCObjActive.ToString().ToUpper();
                    }
                case "_BCOBJECTISTRACKABLE":
                    {
                        return BCObjTrackable.ToString().ToUpper();
                    }
                case "_BCOBJECTTABLENAME":
                    {
                        return _BCObjTableName;
                    }
                case "_BCOBJECTWINDOW":
                    {
                        return _BCObjWindow;
                    }
                case "_BCRECONCILEON":
                    {
                        return _BCDoReconcile;
                    }
                case "_BCRESETTREE":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.TrackingAfterDestinationAccepted) || get_IsValidCaller(ScriptEngine.CallerTypes.TrackingAfterObjectAccepted) || get_IsValidCaller(ScriptEngine.CallerTypes.TrackingAfterTrackingComplete))
                        {
                            return BCResetTree;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_BCSCANDATETIME":
                    {
                        return _BCScanDateTime;
                    }
                case "_CANCEL":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpCancel, ref GetPropertyRet);
                        break;
                    }
                case "_CAPTION":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpCaption, ref GetPropertyRet);
                        break;
                    }
                case "_CENTERPROMPTONPARENT":
                    {
                        break;
                    }
                // position is already handled in Visual Linkscripts; ignored.  RVW 04/22/2015
                //case "_CLIPBOARD":
                //    {
                //        return Clipboard;
                //    }
                case "_COLDCOLUMNDATA":
                    {
                        throw new Exception(string.Format("The \"{0}\" property is no longer available", variableProperty));
                    }
                case "_CRLF":
                    {
                        return Constants.vbCrLf;
                    }
                case "_DATE":
                    {
                        return DateTime.Today;
                    }
                case "_DBVERSION":
                    {
                        return DBMajorVersion;
                    }
                case "_DBREVISION":
                    {
                        return DBMinorVersion;
                    }
                case "_DEFAULT":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpDefault, ref GetPropertyRet);
                        break;
                    }
                case "_DELETEPAGE":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument) || get_IsValidCaller(ScriptEngine.CallerTypes.PCFilesLoad))
                        {
                            return _deletePage;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_DISABLEINITIALTRACKINGLOCATION":
                    {
                        return _disableInitialTrackingLocation;
                    }
                case "_DPIHORIZONTAL":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            return _DPIHorizontal;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_DPIVERTICAL":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            return _DPIVertical;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_EMAILATTACHMENTS":
                    {
                        return Form.EmailAttachments;
                    }
                case "_EMAILFROMADDRESS":
                    {
                        return Form.EmailFromAddress;
                    }
                case "_EMAILFROMNAME":
                    {
                        return Form.EmailFromName;
                    }
                case "_EMAILSUBJECT":
                    {
                        return Form.EmailSubject;
                    }
                case "_EMAILTOADDRESS":
                    {
                        return Form.EmailToAddress;
                    }
                case "_EMAILTONAME":
                    {
                        return Form.EmailToName;
                    }
                case "_ENABLED":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpEnabled, ref GetPropertyRet);
                        break;
                    }
                case "_EOF":
                    {
                        if (GetVariableTypeAndSetValue(variablePrefix, ref privateVariables) == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            return tableVariable.EOF.ToString().ToUpper();
                        }

                        return true.ToString().ToUpper();
                    }
                case "_ERRORMODE":
                    {
                        return _errorMode;
                    }
                case "_FIELD":
                    {
                        string fieldName = variableName.Substring(start + 1, variableName.LastIndexOf(")") - start - 1);
                        return GetFieldValue(variablePrefix, fieldName, ref privateVariables);
                    }
                case "_FIELDCOUNT":
                    {
                        eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);

                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            return (object)tableVariable.Columns.Count;
                        }

                        break;
                    }
                case "_FIELDISDATE":
                    {
                        eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);

                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            GetCurrentVariableAsObject(variableName, privateVariables, start, ref eType);

                            if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                            {
                                throw new Exception("Invalid use of Recordset Variable in _FieldIsDate()");
                            }
                            else
                            {
                                return Navigation.IsADateType(tableVariable.Columns[CurrentVariable.ToString()].DataType).ToString().ToUpper();
                            }
                        }

                        break;
                    }
                case "_FIELDISNUMERIC":
                    {
                        eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);

                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            GetCurrentVariableAsObject(variableName, privateVariables, start, ref eType);

                            if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                            {
                                throw new Exception("Invalid use of Recordset Variable in _FieldIsNumeric()");
                            }
                            else
                            {
                                return Navigation.IsAStringType(tableVariable.Columns[CurrentVariable.ToString()].DataType).ToString().ToUpper();
                            }
                        }

                        break;
                    }
                case "_FIELDISSTRING":
                    {
                        eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);

                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            GetCurrentVariableAsObject(variableName, privateVariables, start, ref eType);

                            if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                            {
                                throw new Exception("Invalid use of Recordset Variable in _FieldIsString()");
                            }
                            else
                            {
                                return Navigation.IsAStringType(tableVariable.Columns[CurrentVariable.ToString()].DataType).ToString().ToUpper();
                            }
                        }

                        break;
                    }
                case "_FIELDNAME":
                    {
                        eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);

                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            GetCurrentVariableAsObject(variableName, privateVariables, start, ref eType);

                            if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                            {
                                throw new Exception("Invalid use of Recordset Variable in _FieldName()");
                            }
                            else if (Information.VarType(CurrentVariable) == VariantType.Integer)
                            {
                                return tableVariable.Columns[Conversions.ToInteger(CurrentVariable.ToString())].ColumnName;
                            }
                            else
                            {
                                return tableVariable.Columns[CurrentVariable.ToString()].ColumnName;
                            }
                        }

                        break;
                    }
                case "_FONTBOLD":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontBold, ref GetPropertyRet);
                        break;
                    }
                case "_FONTITALIC":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontItalic, ref GetPropertyRet);
                        break;
                    }
                case "_FONTNAME":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontName, ref GetPropertyRet);
                        break;
                    }
                case "_FONTSIZE":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontSize, ref GetPropertyRet);
                        break;
                    }
                case "_FONTUNDERLINE":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontUnderline, ref GetPropertyRet);
                        break;
                    }
                case "_FORECOLOR":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpForeColor, ref GetPropertyRet);
                        break;
                    }
                case "_FORMHEIGHT":
                    {
                        return (object)Form.FormHeight;
                    }
                case "_FORMSIZE":
                    {
                        return Form.FormSize;
                    }
                case "_FORMWIDTH":
                    {
                        return (object)Form.FormWidth;
                    }
                case "_FRONTPAGE":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            return _frontPage;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_GETSELECTEDROWIDS":
                    {
                        return GetArrayPropertyValue(variableProperty, variableName, ref privateVariables, start, eType);
                    }
                case "_GETSELECTEDROWIDSCOUNT":
                    {
                        return _getSelectedRowIdsCount;
                    }
                case "_GRIDREFRESH":
                    {
                        return _gridRefresh;
                    }
                case "_GRIDCOMPLETESQL":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.LibrarianViewLinkScriptButton))
                        {
                            return _gridCompleteSQL;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_GRIDSQL":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.LibrarianViewLinkScriptButton))
                        {
                            return _gridSQL;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_HEADING":
                    {
                        return _heading;
                    }
                case "_HEIGHT":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpHeight, ref GetPropertyRet);
                        break;
                    }
                case "_ITEMDATA":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpItemData, ref GetPropertyRet);
                        break;
                    }
                case "_LABELSTABLENAME":
                    {
                        return _labelTableName;
                    }
                case "_LINKSCRIPTCALLERTYPE":
                    {
                        return Caller;
                    }
                case "_LISTINDEX":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpListindex, ref GetPropertyRet);
                        break;
                    }
                case "_LOCKED":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpLocked, ref GetPropertyRet);
                        break;
                    }
                case "_MAXLENGTH":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpMaxLength, ref GetPropertyRet);
                        break;
                    }
                case "_MAXRECORDSTOFETCH":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.LibrarianViewLinkScriptButton))
                        {
                            return _maxRecordsToFetch;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_MSGBOXRESULT":
                    {
                        return _msgBoxResult;
                    }
                case "_NEWATTACHMENTNAME":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            return _newAttachmentName;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_NEWRECORDID":
                    {
                        return _newRecordId;
                    }
                case "_NOW":
                    {
                        return DateTime.Now;
                    }
                case "_ONESTRIPFORMSID":
                    {
                        return _oneStripFormsId;
                    }
                case "_ONESTRIPJOBSID":
                    {
                        return _onestripJobsId;
                    }
                case "_OUTPUTIMAGENAME":
                    {
                        return _outputImageName;
                    }
                case "_PASSWORD":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpPassword, ref GetPropertyRet);
                        break;
                    }
                case "_RANGEMAX":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpRangeMax, ref GetPropertyRet);
                        break;
                    }
                case "_RANGEMIN":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpRangeMin, ref GetPropertyRet);
                        break;
                    }
                case "_RECORDCOUNT":
                    {
                        eType = GetVariableTypeAndSetValue(variablePrefix, ref privateVariables);

                        if (eType == LinkScriptVariable.VariableTypes.vtDataTable)
                        {
                            DataWithCursor tableVariable = (DataWithCursor)((LinkScriptVariable)CurrentVariable).Value;
                            return (object)tableVariable.Rows.Count;
                        }

                        break;
                    }
                case "_RECORDID":
                    {
                        return _recordId;
                    }
                case "_REQUIRED":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpRequired, ref GetPropertyRet);
                        break;
                    }
                case "_SAVEASNEWPAGE":
                    {
                        return _saveAsNewPage;
                    }
                case "_SAVEASNEWVERSION":
                    {
                        return _saveAsNewVersion;
                    }
                case "_SAVEASNEWVERSIONASOFFICIALRECORD":
                    {
                        return _saveAsNewVersionAsOfficialRecord;
                    }
                case "_SCRIPTWAITCANCELLED":
                    {
                        if (_hasWaitForm)
                            return (object)WaitForm.Cancelled;
                        return false;
                    }
                case "_SCRIPTWAITCANCELVISIBLE":
                    {
                        if (_hasWaitForm)
                            return (object)WaitForm.CancelVisible;
                        return false;
                    }
                case "_SCRIPTWAITDESCRIPTION":
                    {
                        if (_hasWaitForm)
                            return WaitForm.Description;
                        return string.Empty;
                    }
                case "_SCRIPTWAITDISPLAYMESSAGE":
                    {
                        if (_hasWaitForm)
                            return WaitForm.DisplayMessage;
                        return string.Empty;
                    }
                case "_SCRIPTWAITPERCENTAGE":
                    {
                        if (_hasWaitForm)
                            return (object)WaitForm.Percentage;
                        return 0;
                    }
                case "_SCRIPTWAITPERCENTVISIBLE":
                    {
                        if (_hasWaitForm)
                            return (object)WaitForm.PercentVisible;
                        return false;
                    }
                case "_SCRIPTWAITWINDOWCAPTION":
                    {
                        if (_hasWaitForm)
                            return WaitForm.WindowCaption;
                        return string.Empty;
                    }
                case "_SCRIPTNAME":
                    {
                        return _scriptName;
                    }
                case "_SETSELECTEDROWIDSCOUNT":
                    {
                        return _setSelectedRowIdsCount;
                    }
                case "_STOP":
                    {
                        return _stopScanner;
                    }
                case "_STOPSCANNER":
                    {
                        return _stopScanner;
                    }
                case "_TAB":
                    {
                        return Constants.vbTab;
                    }
                case "_TABLESATTACHMENTS":
                case "_TABLESBARCODEPREFIX":
                case "_TABLESCONTAINERNUMBER":
                case "_TABLESCOUNTERFIELDNAME":
                case "_TABLESDBACCESSTYPE":
                case "_TABLESDBNAME":
                case "_TABLESIDFIELDISSTRING":
                case "_TABLESIDFIELDNAME":
                case "_TABLESIDMASK":
                case "_TABLESIDSTRIPCHARS":
                case "_TABLESOUTTABLE":
                case "_TABLESSTATUSFIELDNAME":
                case "_TABLESTRACKINGACTIVEFIELDNAME":
                case "_TABLESTRACKINGOUTFIELDNAME":
                case "_TABLESUSERNAME":
                    {
                        // These are the Tables properties. They all take a table name as a subscript
                        string tableName = GetCurrentVariableAsString(variableName, privateVariables, start, ref eType);
                        var row = ScriptEngine.GetTableRow(tableName, _passport);
                        if (row is null)
                            return string.Empty;

                        switch (variableProperty ?? "")
                        {
                            case "_TABLESATTACHMENTS":
                                {
                                    return row["Attachments"].ToString().ToUpper();
                                }
                            case "_TABLESBARCODEPREFIX":
                                {
                                    return row["BarCodePrefix"].ToString();
                                }
                            case "_TABLESCONTAINERNUMBER":
                                {
                                    return row["TrackingTable"].ToString();
                                }
                            case "_TABLESCOUNTERFIELDNAME":
                                {
                                    return row["CounterFieldName"].ToString();
                                }
                            case "_TABLESDBACCESSTYPE":
                                {
                                    return 5;
                                }
                            case "_TABLESDBNAME":
                                {
                                    return row["DBName"].ToString();
                                }
                            case "_TABLESIDFIELDISSTRING":
                                {
                                    return Navigation.FieldIsAString(tableName, row["IdFieldName"].ToString(), _passport).ToString().ToUpper();
                                }
                            case "_TABLESIDFIELDNAME":
                                {
                                    return row["IdFieldName"].ToString();
                                }
                            case "_TABLESIDMASK":
                                {
                                    return row["IdMask"].ToString();
                                }
                            case "_TABLESIDSTRIPCHARS":
                                {
                                    return row["IdStripChars"].ToString();
                                }
                            case "_TABLESOUTTABLE":
                                {
                                    return row["OutTable"].ToString();
                                }
                            case "_TABLESSTATUSFIELDNAME":
                                {
                                    return row["TrackingStatusFieldName"].ToString();
                                }
                            case "_TABLESTRACKINGACTIVEFIELDNAME":
                                {
                                    return row["TrackingACTIVEFieldName"].ToString();
                                }
                            case "_TABLESTRACKINGOUTFIELDNAME":
                                {
                                    return row["TrackingOUTFieldName"].ToString();
                                }
                            case "_TABLESUSERNAME":
                                {
                                    return row["UserName"].ToString();
                                }

                            default:
                                {
                                    break;
                                }
                        }

                        break;
                    }
                case "_TABLENAME":
                    {
                        return CurrentTableName;
                    }
                case "_TEXT":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpText, ref GetPropertyRet);
                        break;
                    }
                case "_TEXTBOXTYPE":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpTextboxType, ref GetPropertyRet);
                        break;
                    }
                case "_TITLE":
                    {
                        return Title;
                    }
                case "_TOOLTIP":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpTooltip, ref GetPropertyRet);
                        break;
                    }
                case "_TRACKINGTABLESCOUNT":
                    {
                        return GetTrackingTableCount();
                    }
                case "_TRACKINGTABLEISTRACKABLE":
                case "_TRACKINGTABLESTABLENAME":
                case "_TRACKINGTABLESCONTAINERNUMBER":
                case "_TRACKINGTABLESSTATUSFIELDNAME":
                    {
                        return GetTrackingTableInfo(variableProperty, variableName, ref privateVariables, start, ref eType);
                    }
                case "_USERLINKTABLEID":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument) || get_IsValidCaller(ScriptEngine.CallerTypes.PCFilesLoad) || get_IsValidCaller(ScriptEngine.CallerTypes.ManualIndexingLinkButton))
                        {
                            int index = GetCurrentVariableAsInteger(variableName, privateVariables, start, ref eType);
                            return UserLink.GetValue(1, index - 1);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_USERLINKTABLENAME":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument) || get_IsValidCaller(ScriptEngine.CallerTypes.PCFilesLoad) || get_IsValidCaller(ScriptEngine.CallerTypes.ManualIndexingLinkButton))
                        {
                            int index = GetCurrentVariableAsInteger(variableName, privateVariables, start, ref eType);
                            return UserLink.GetValue(0, index - 1);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_VALIDATE":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpValidate, ref GetPropertyRet);
                        break;
                    }
                case "_VALUE":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpValue, ref GetPropertyRet);
                        break;
                    }
                case "_VIEWERIMAGEFILENAME":
                    {
                        return _viewerImageFileName;
                    }
                case "_VIEWERIMAGENAME":
                    {
                        return _viewerImageFileName;
                    }
                case "_VIEWERIMAGETABLE":
                    {
                        return _viewerImageTableName;
                    }
                case "_VIEWID":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.LibrarianViewLinkScriptButton))
                        {
                            return _viewId;
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }
                    }
                case "_VISIBLE":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpVisible, ref GetPropertyRet);
                        break;
                    }
                case "_WIDTH":
                    {
                        GetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpWidth, ref GetPropertyRet);
                        break;
                    }

                default:
                    {
                        throw new Exception(string.Format("The \"{0}\" property defined in script does not exist", variableProperty));
                    }
            }

            return GetPropertyRet;
        }

        private void SetProperty(string sResult, object vValue, ref LinkScriptVariables privateVariables)
        {
            int start = 0;
            int prefixLength = 0;
            string variablePrefix = string.Empty;
            string variableProperty = string.Empty;
            var eType = default(LinkScriptVariable.VariableTypes);
            // Remove Index from property...
            start = sResult.IndexOf("(");
            // Getprefix to property (if exists)...
            prefixLength = sResult.IndexOf("._");
            if (prefixLength > 0)
                variablePrefix = sResult.Substring(0, prefixLength);
            // Getvarprop to property (if exists)...
            variableProperty = ParsePropertyFromVariableName(sResult, start, prefixLength);
            // Return approriate property value...
            switch (variableProperty ?? "")
            {
                case "_ACTIVEUSER":
                    {
                        _activeUser = vValue.ToString();
                        break;
                    }
                case "_APPPATH":
                case "_APPVERSION":
                case "_APPREVISION":
                case "_APPTITLE":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" constant (read only).", variableProperty));
                    }
                case "_ATTACHMENTNUMBER":
                    {
                        if (Strings.Len(vValue.ToString()) > 0)
                        {
                            if (Information.IsNumeric(vValue))
                            {
                                _attachmentNumber = Conversions.ToInteger(vValue);
                            }
                            else
                            {
                                _attachmentNumber = 0;
                            }
                        }
                        else
                        {
                            _attachmentNumber = 0;
                        }

                        break;
                    }
                case "_ALLOWNULLDATE":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpAllowNullDate, vValue);
                        break;
                    }
                case "_BACKCOLOR":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpBackColor, vValue);
                        break;
                    }
                case "_BARCODECOUNT":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_BARCODE":
                case "_BARCODELEFT":
                case "_BARCODETOP":
                case "_BARCODETYPE":
                    {
                        SetArrayPropertyValue(variableProperty, sResult, vValue, ref privateVariables, start, eType);
                        break;
                    }
                case "_BCADDITIONALFIELD":
                    {
                        _BCAdditionalField = vValue.ToString();
                        break;
                    }
                case "_BCADDITIONALFIELDTYPE":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_BCADDITIONALMEMOFIELD":
                    {
                        _BCAdditionalMemoField = vValue.ToString();
                        break;
                    }
                case "_BCCHECKFORDUPS":
                    {
                        _BCCheckForDups = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_BCDESTINATIONCONTAINERNUMBER":
                    {
                        _BCDestContainerNumber = Conversions.ToInteger(vValue);
                        break;
                    }
                case "_BCDESTINATIONID":
                    {
                        _BCDestTableId = vValue.ToString();
                        break;
                    }
                case "_BCDESTINATIONISACTIVE":
                    {
                        _BCDestActive = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_BCDESTINATIONISTRACKABLE":
                    {
                        _BCDestTrackable = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_BCDESTINATIONTABLENAME":
                    {
                        _BCDestTableName = vValue.ToString();
                        break;
                    }
                case "_BCDESTINATIONWINDOW":
                    {
                        _BCDestWindow = vValue.ToString();
                        break;
                    }
                case "_BCDUEBACKDATE":
                    {
                        _BCDueBack = Conversions.ToDate(vValue);
                        break;
                    }
                case "_BCOBJECTCONTAINERNUMBER":
                    {
                        _BCObjContainerNumber = Conversions.ToInteger(vValue);
                        break;
                    }
                case "_BCOBJECTID":
                    {
                        _BCObjTableId = vValue.ToString();
                        break;
                    }
                case "_BCOBJECTISACTIVE":
                    {
                        _BCObjActive = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_BCOBJECTISTRACKABLE":
                    {
                        _BCObjTrackable = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_BCOBJECTTABLENAME":
                    {
                        _BCObjTableName = vValue.ToString();
                        break;
                    }
                case "_BCOBJECTWINDOW":
                    {
                        _BCObjWindow = vValue.ToString();
                        break;
                    }
                case "_BCRECONCILEON":
                    {
                        _BCDoReconcile = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_BCRESETTREE":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.TrackingAfterDestinationAccepted) || get_IsValidCaller(ScriptEngine.CallerTypes.TrackingAfterObjectAccepted) || get_IsValidCaller(ScriptEngine.CallerTypes.TrackingAfterTrackingComplete))
                        {
                            _BCResetTree = Navigation.CBoolean(vValue);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_BCSCANDATETIME":
                    {
                        _BCScanDateTime = Conversions.ToDate(vValue);
                        break;
                    }
                case "_CAPTION":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpCaption, vValue);
                        break;
                    }
                case "_CANCEL":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpCancel, vValue);
                        break;
                    }
                case "_CENTERPROMPTONPARENT":
                    {
                        break;
                    }
                // position is already handled in Visual Linkscripts; ignored.  RVW 04/22/2015
                //case "_CLIPBOARD":
                //    {
                //        Clipboard = vValue.ToString();
                //        break;
                //    }
                case "_COLDCOLUMNDATA":
                    {
                        throw new Exception(string.Format("The \"{0}\" property is no longer available", variableProperty));
                    }
                case "_CRLF":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_CURRENTUSEREMAIL":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_CURRENTUSERNAME":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_DATE":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_DEFAULT":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpDefault, vValue);
                        break;
                    }
                case "_DELETEPAGE":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument) || get_IsValidCaller(ScriptEngine.CallerTypes.PCFilesLoad))
                        {
                            _deletePage = Navigation.CBoolean(vValue);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_DISABLEINITIALTRACKINGLOCATION":
                    {
                        _disableInitialTrackingLocation = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_DPIHORIZONTAL":
                case "_DPIVERTICAL":
                case "_EOF":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_EMAILATTACHMENTS":
                    {
                        Form.EmailAttachments = vValue.ToString();
                        break;
                    }
                case "_EMAILFROMADDRESS":
                    {
                        Form.EmailFromAddress = vValue.ToString();
                        break;
                    }
                case "_EMAILFROMNAME":
                    {
                        Form.EmailFromName = vValue.ToString();
                        break;
                    }
                case "_EMAILSUBJECT":
                    {
                        Form.EmailSubject = vValue.ToString();
                        break;
                    }
                case "_EMAILTOADDRESS":
                    {
                        Form.EmailToAddress = vValue.ToString();
                        break;
                    }
                case "_EMAILTONAME":
                    {
                        Form.EmailToName = vValue.ToString();
                        break;
                    }
                case "_ENABLED":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpEnabled, vValue);
                        break;
                    }
                case "_ERRORMODE":
                    {
                        _errorMode = Conversions.ToInteger(vValue);
                        break;
                    }
                case "_FIELD":
                    {
                        string fieldName = sResult.Substring(start + 1, sResult.LastIndexOf(")") - start - 1);
                        SetFieldValue(variablePrefix, fieldName, ref privateVariables, vValue);
                        break;
                    }
                case "_FIELDISDATE":
                case "_FIELDISNUMERIC":
                case "_FIELDISSTRING":
                case "_FIELDCOUNT":
                case "_FIELDNAME":
                case var @case when @case == "_FIELDCOUNT":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_FONTBOLD":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontBold, vValue);
                        break;
                    }
                case "_FONTITALIC":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontItalic, vValue);
                        break;
                    }
                case "_FONTNAME":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontName, vValue);
                        break;
                    }
                case "_FONTSIZE":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontSize, vValue);
                        break;
                    }
                case "_FONTUNDERLINE":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpFontUnderline, vValue);
                        break;
                    }
                case "_FORECOLOR":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpForeColor, vValue);
                        break;
                    }
                case "_FORMHEIGHT":
                    {
                        Form.FormHeight = Conversions.ToInteger(vValue);
                        break;
                    }
                case "_FORMSIZE":
                    {
                        if (vValue.ToString().ToUpper() == "NORMAL")
                        {
                            Form.FormSize = ScriptForm.sfFormSize.Normal;
                        }
                        else if (vValue.ToString().ToUpper() == "MAXIMIZED")
                        {
                            Form.FormSize = ScriptForm.sfFormSize.Maximized;
                        }
                        else if (vValue.ToString().ToUpper() == "AUTOSIZE")
                        {
                            Form.FormSize = ScriptForm.sfFormSize.Autosize;
                        }
                        else
                        {
                            Form.FormSize = (ScriptForm.sfFormSize)Conversions.ToInteger(vValue);
                        }

                        break;
                    }

                case "_FORMWIDTH":
                    {
                        Form.FormWidth = Conversions.ToInteger(vValue);
                        break;
                    }
                case "_FRONTPAGE":
                case "_GETSELECTEDROWIDS":
                case "_GETSELECTEDROWIDSCOUNT":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_GETSELECTEDROWS":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_GRIDREFRESH":
                    {
                        _gridRefresh = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_GRIDCOMPLETESQL":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.LibrarianViewLinkScriptButton))
                        {
                            _gridCompleteSQL = vValue.ToString();
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_GRIDSQL":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.LibrarianViewLinkScriptButton))
                        {
                            _gridSQL = vValue.ToString();
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_HEADING":
                    {
                        _heading = vValue.ToString();
                        break;
                    }
                case "_HEIGHT":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpHeight, vValue);
                        break;
                    }
                case "_ITEMDATA":
                case "_LABELSTABLENAME":
                case "_LINKSCRIPTCALLERTYPE":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_LINENUMBER":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_LISTINDEX":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpListindex, vValue);
                        break;
                    }
                case "_LOCKED":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpLocked, vValue);
                        break;
                    }
                case "_MAXLENGTH":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpMaxLength, vValue);
                        break;
                    }
                case "_MAXRECORDSTOFETCH":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.LibrarianViewLinkScriptButton))
                        {
                            _maxRecordsToFetch = Conversions.ToInteger(vValue);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_MSGBOXRESULT":
                    {
                        _msgBoxResult = (MsgBoxResult)Conversions.ToInteger(vValue);
                        break;
                    }
                case "_NEWATTACHMENTNAME":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument))
                        {
                            _newAttachmentName = vValue.ToString();
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_NEWRECORDID":
                    {
                        _newRecordId = vValue.ToString();
                        break;
                    }
                case "_NOW":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_ONESTRIPFORMSID":
                    {
                        _oneStripFormsId = Conversions.ToInteger(vValue);
                        break;
                    }
                case "_ONESTRIPJOBSID":
                    {
                        _onestripJobsId = Conversions.ToInteger(vValue);
                        break;
                    }
                case "_OUTPUTIMAGENAME":
                    {
                        _outputImageName = vValue.ToString();
                        break;
                    }
                case "_PASSWORD":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpPassword, vValue);
                        break;
                    }
                case "_RANGEMAX":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpRangeMax, vValue);
                        break;
                    }
                case "_RANGEMIN":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpRangeMin, vValue);
                        break;
                    }
                case "_RECORDCOUNT":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_RECORDID":
                    {
                        _recordId = vValue.ToString();
                        break;
                    }
                case "_REQUIRED":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpRequired, vValue);
                        break;
                    }
                case "_SAVEASNEWPAGE":
                    {
                        if (Strings.Len(vValue.ToString()) > 0)
                        {
                            _saveAsNewPage = Conversions.ToInteger(Navigation.CBoolean(vValue));
                        }
                        else
                        {
                            _saveAsNewPage = 0;
                        }
                        if (_saveAsNewPage != 0 & _saveAsNewPage != -99)
                        {
                            _saveAsNewVersion = 0;
                            _saveAsNewVersionAsOfficialRecord = 0;
                        }

                        break;
                    }
                case "_SAVEASNEWVERSION":
                    {
                        if (Strings.Len(vValue.ToString()) > 0)
                        {
                            _saveAsNewVersion = Conversions.ToInteger(Navigation.CBoolean(vValue));
                        }
                        else
                        {
                            _saveAsNewVersion = 0;
                        }
                        if (_saveAsNewVersion != 0 & _saveAsNewVersion != -99)
                        {
                            _saveAsNewPage = 0;
                            _saveAsNewVersionAsOfficialRecord = 0;
                        }

                        break;
                    }
                case "_SAVEASNEWVERSIONASOFFICIALRECORD":
                    {
                        if (Strings.Len(vValue.ToString()) > 0)
                        {
                            _saveAsNewVersionAsOfficialRecord = Conversions.ToInteger(Navigation.CBoolean(vValue));
                        }
                        else
                        {
                            _saveAsNewVersionAsOfficialRecord = 0;
                        }
                        if (_saveAsNewVersionAsOfficialRecord != 0 & _saveAsNewVersionAsOfficialRecord != -99)
                        {
                            _saveAsNewPage = 0;
                            _saveAsNewVersion = 0;
                        }

                        break;
                    }
                case "_SCRIPTWAITCANCELLED":
                    {
                        WaitForm.Cancelled = Navigation.CBoolean(vValue);
                        RefreshWaitForm();
                        break;
                    }
                case "_SCRIPTWAITCANCELVISIBLE":
                    {
                        WaitForm.CancelVisible = Navigation.CBoolean(vValue);
                        RefreshWaitForm();
                        break;
                    }
                case "_SCRIPTWAITDESCRIPTION":
                    {
                        WaitForm.Description = vValue.ToString();
                        RefreshWaitForm();
                        break;
                    }
                case "_SCRIPTWAITDISPLAYMESSAGE":
                    {
                        WaitForm.DisplayMessage = vValue.ToString();
                        RefreshWaitForm();
                        break;
                    }
                case "_SCRIPTWAITPERCENTAGE":
                    {
                        WaitForm.Percentage = Conversions.ToInteger(vValue);
                        RefreshWaitForm();
                        break;
                    }
                case "_SCRIPTWAITPERCENTVISIBLE":
                    {
                        WaitForm.PercentVisible = Navigation.CBoolean(vValue);
                        RefreshWaitForm();
                        break;
                    }
                case "_SCRIPTWAITWINDOWCAPTION":
                    {
                        WaitForm.WindowCaption = vValue.ToString();
                        RefreshWaitForm();
                        break;
                    }
                case "_SCRIPTNAME":
                    {
                        _scriptName = vValue.ToString();
                        break;
                    }
                case "_SETSELECTEDROWIDS":
                    {
                        SetArrayPropertyValue(variableProperty, sResult, vValue, ref privateVariables, start, eType);
                        break;
                    }
                case "_SETSELECTEDROWIDSCOUNT":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_STOP":
                    {
                        _stopScanner = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_STOPSCANNER":
                    {
                        _stopScanner = Navigation.CBoolean(vValue);
                        break;
                    }
                case "_TAB":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_TABLENAME":
                case "_TABLESATTACHMENTS":
                case "_TABLESBARCODEPREFIX":
                case "_TABLESCONTAINERNUMBER":
                case "_TABLESCOUNTERFIELDNAME":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_TABLESDBACCESSTYPE":
                case "_TABLESDBNAME":
                case "_TABLESIDFIELDISSTRING":
                case "_TABLESIDFIELDNAME":
                case "_TABLESIDMASK":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_TABLESIDSTRIPCHARS":
                case "_TABLESOUTTABLE":
                case "_TABLESSTATUSFIELDNAME":
                case "_TABLESTRACKINGACTIVEFIELDNAME":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_TABLESTRACKINGOUTFIELDNAME":
                case "_TABLESUSERNAME":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_TEXT":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpText, vValue);
                        break;
                    }
                case "_TEXTBOXTYPE":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpTextboxType, vValue);
                        break;
                    }
                case "_TITLE":
                    {
                        Title = vValue.ToString();
                        break;
                    }
                case "_TOOLTIP":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpTooltip, vValue);
                        break;
                    }
                case "_TRACKINGTABLESCOUNT":
                case "_TRACKINGTABLEISTRACKABLE":
                case "_TRACKINGTABLESTABLENAME":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_TRACKINGTABLESCONTAINERNUMBER":
                case "_TRACKINGTABLESSTATUSFIELDNAME":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_USERLINKTABLEID":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument) || get_IsValidCaller(ScriptEngine.CallerTypes.PCFilesLoad) || get_IsValidCaller(ScriptEngine.CallerTypes.ManualIndexingLinkButton))
                        {
                            int index = GetCurrentVariableAsInteger(sResult, privateVariables, start, ref eType);
                            UserLink.SetValue(vValue, 1, index - 1);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_USERLINKTABLENAME":
                    {
                        if (get_IsValidCaller(ScriptEngine.CallerTypes.AnyCaller) || get_IsValidCaller(ScriptEngine.CallerTypes.ScannerNewDocument) || get_IsValidCaller(ScriptEngine.CallerTypes.PCFilesLoad) || get_IsValidCaller(ScriptEngine.CallerTypes.ManualIndexingLinkButton))
                        {
                            int index = GetCurrentVariableAsInteger(sResult, privateVariables, start, ref eType);
                            UserLink.SetValue(vValue, 0, index - 1);
                        }
                        else
                        {
                            throw new Exception(string.Format("The \"{0}\" property is not available from this calling source", variableProperty));
                        }

                        break;
                    }
                case "_VALIDATE":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpValidate, vValue);
                        break;
                    }
                case "_VALUE":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpValue, vValue);
                        break;
                    }
                case "_VIEWERIMAGEFILENAME":
                    {
                        _viewerImageFileName = vValue.ToString();
                        break;
                    }
                case "_VIEWERIMAGENAME":
                    {
                        _viewerImageFileName = vValue.ToString();
                        break;
                    }
                case "_VIEWERIMAGETABLE":
                    {
                        _viewerImageTableName = vValue.ToString();
                        break;
                    }
                case "_VIEWID":
                    {
                        throw new Exception(string.Format("Invalid use of the \"{0}\" property (read only).", variableProperty));
                    }
                case "_VISIBLE":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpVisible, vValue);
                        break;
                    }
                case "_WIDTH":
                    {
                        SetControlProp?.Invoke(variablePrefix, ScriptControls.ControlProperties.cpWidth, vValue);
                        break;
                    }

                default:
                    {
                        throw new Exception("The \"" + variableProperty + "\" property defined in script does not exist!");
                    }
            }
        }

        private int GetTrackingTableCount()
        {
            var allTables = Navigation.GetAllTables(_passport);
            int count = Navigation.GetTrackingTableCount(_passport);

            foreach (RecordsManage.TablesRow row in allTables)
            {
                if ((int)row.TrackingTable < 1 & row.Trackable)
                    count += 1;
            }

            return count;
        }

        private string GetTrackingTableInfo(string variableProperty, string variableName, ref LinkScriptVariables privateVariables, int start, ref LinkScriptVariable.VariableTypes eType)
        {
            var trackingTables = Navigation.GetTrackingTables(_passport);
            int trackingTablesCount = trackingTables.Count;
            int allTrackingTablesCount = trackingTablesCount;
            int index = GetCurrentVariableAsInteger(variableName, privateVariables, start, ref eType);

            if (index > 0 & index <= allTrackingTablesCount)
            {
                if (index <= trackingTablesCount)
                {
                    switch (variableProperty ?? "")
                    {
                        case "_TRACKINGTABLEISTRACKABLE":
                            {
                                return trackingTables[index - 1].Trackable.ToString().ToUpper();
                            }
                        case "_TRACKINGTABLESTABLENAME":
                            {
                                return trackingTables[index - 1].TableName;
                            }
                        case "_TRACKINGTABLESCONTAINERNUMBER":
                            {
                                return trackingTables[index - 1].TrackingTable.ToString();
                            }
                        case "_TRACKINGTABLESSTATUSFIELDNAME":
                            {
                                return trackingTables[index - 1].TrackingStatusFieldName;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }
                else
                {
                    trackingTablesCount = Navigation.GetTrackingTableCount(_passport);
                    var allTables = Navigation.GetAllTables(_passport);

                    foreach (RecordsManage.TablesRow oTables in allTables)
                    {
                        if ((int)oTables.TrackingTable < 1 & oTables.Trackable)
                        {
                            trackingTablesCount += 1;

                            if (trackingTablesCount == index)
                            {
                                switch (variableProperty ?? "")
                                {
                                    case "_TRACKINGTABLEISTRACKABLE":
                                        {
                                            return oTables.Trackable.ToString().ToUpper();
                                        }
                                    case "_TRACKINGTABLESTABLENAME":
                                        {
                                            return oTables.TableName;
                                        }
                                    case "_TRACKINGTABLESCONTAINERNUMBER":
                                        {
                                            return "99";
                                        }
                                    case "_TRACKINGTABLESSTATUSFIELDNAME":
                                        {
                                            break;
                                        }

                                    default:
                                        {
                                            break;
                                        }
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        private LinkScriptVariable.VariableTypes GetVariableTypeAndSetValue(string sVar, ref LinkScriptVariables privateVariables)
        {
            if (string.IsNullOrEmpty(sVar))
            {
                _variable = string.Empty;
                return LinkScriptVariable.VariableTypes.vtString;
            }
            if (Information.IsNumeric(sVar.Trim()))
            {
                _variable = Conversions.ToDouble(sVar.Trim());
                return LinkScriptVariable.VariableTypes.vtInteger;
            }
            // Is this a string (constant)?
            if (sVar.StartsWith("\""))
            {
                sVar = sVar.Substring(1);
                if (sVar.EndsWith("\""))
                    sVar = sVar.Substring(0, sVar.Length - 1);
                _variable = sVar;
                return LinkScriptVariable.VariableTypes.vtString;
            }

            if (sVar.Substring(0, 1) == "_" | sVar.Contains("._"))
            {
                // Property value specified...
                _variable = GetProperty(sVar, ref privateVariables);
                return LinkScriptVariable.VariableTypes.vtProperty;
            }
            // Is this a Public or Private Variable?
            else if (VariableExists(sVar))
            {
                // Is the public var an Object?
                if (this.IsVarAnObject(sVar, LinkScriptVariable.VarScope.vsPublic, ref privateVariables))
                {
                    _variable = GetVariable(sVar);
                    return LinkScriptVariable.VariableTypes.vtDataTable;
                }
                else
                {
                    _variable = GetVariableValue(sVar);
                    return LinkScriptVariable.VariableTypes.vtString;
                }
            }
            else if (VariableExists(sVar, ref privateVariables))
            {
                // Is the Private var an Object?
                if (this.IsVarAnObject(sVar, LinkScriptVariable.VarScope.vsPrivate, ref privateVariables))
                {
                    _variable = GetVariable(sVar, ref privateVariables);
                    return LinkScriptVariable.VariableTypes.vtDataTable;
                }
                else
                {
                    _variable = GetVariableValue(sVar, ref privateVariables);
                    return LinkScriptVariable.VariableTypes.vtString;
                }
            }
            else
            {
                switch (sVar.ToUpper().Trim() ?? "")
                {
                    case "ALPHA":
                        {
                            _variable = ScriptControls.TextboxTypes.ttAlpha;
                            break;
                        }
                    case "ALPHANUMERIC":
                        {
                            _variable = ScriptControls.TextboxTypes.ttAlphaNumeric;
                            break;
                        }
                    case "NUMERIC":
                        {
                            _variable = ScriptControls.TextboxTypes.ttNumeric;
                            break;
                        }
                    case "INTEGER":
                        {
                            _variable = ScriptControls.TextboxTypes.ttInteger;
                            break;
                        }
                    case "DATE":
                        {
                            _variable = ScriptControls.TextboxTypes.ttDate;
                            break;
                        }
                    case "SIMPLE":
                        {
                            _variable = Winform.ComboBoxStyle.Simple;
                            break;
                        }
                    case "DROPDOWN":
                        {
                            _variable = Winform.ComboBoxStyle.DropDownList;
                            break;
                        }

                    default:
                        {
                            throw new Exception(string.Format("Error - Variable \"{0}\" is not defined (GetVariableTypeAndSetValue).", sVar));
                        }
                }

                return LinkScriptVariable.VariableTypes.vtInteger;
            }
        }

        private LinkScriptVariable GetVariable(string sKey)
        {
            if (_publicVariables is null)
                return null;

            try
            {
                return _publicVariables.Item(sKey);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private LinkScriptVariable GetVariable(string sKey, ref LinkScriptVariables privateVariables)
        {
            if (privateVariables is null)
                return null;

            try
            {
                return privateVariables.Item(sKey);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private void SetVariable(string sKey, object vValue, string sTableName = "")
        {
            if (_publicVariables is not null)
            {
                var var = _publicVariables.Item(sKey);
                var.Value = vValue;

                if (!string.IsNullOrEmpty(sTableName))
                {
                    var.TableName = sTableName;
                    var.ClearCollections(); // The assumption its a createrecordset or executesql are the only places a table name is passed in
                }
            }
        }

        private void SetVariable(string sKey, object vValue, ref LinkScriptVariables privateVariables, string sTableName = "")
        {
            if (privateVariables is not null)
            {
                var var = privateVariables.Item(sKey.ToUpper().Trim());
                var.Value = vValue;

                if (!string.IsNullOrEmpty(sTableName))
                {
                    var.TableName = sTableName;
                    var.ClearCollections(); // The assumption is a createrecordset or executesql are the only places a table name is passed in
                }
            }
        }

        private object GetVariableValue(string sKey)
        {
            if (_publicVariables is null)
                return null;

            try
            {
                var var = _publicVariables.Item(sKey.ToUpper().Trim());
                if (var is not null)
                    return var.Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private object GetVariableValue(string sKey, ref LinkScriptVariables privateVariables)
        {
            if (privateVariables is null)
                return null;

            try
            {
                var var = privateVariables.Item(sKey.ToUpper().Trim());
                if (var is not null)
                    return var.Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private void SetVariableValue(string sKey, object vValue)
        {
            if (_publicVariables is not null)
            {
                try
                {
                    var var = _publicVariables.Item(sKey.ToUpper().Trim());
                    if (var is not null)
                        var.Value = vValue;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void SetVariableValue(string sKey, object vValue, ref LinkScriptVariables privateVariables)
        {
            if (privateVariables is not null)
            {
                try
                {
                    var var = privateVariables.Item(sKey.ToUpper().Trim());
                    if (var is not null)
                        var.Value = vValue;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void Math(string sVarOne, LinkScriptVariable.MathTypes eType, string sVarTwo, string sResult, ref LinkScriptVariables privateVariables)
        {
            var bOneIsADate = default(bool);
            string sCommandName;
            object vOneValue;
            object vTwoValue;
            LinkScriptVariable.VariableTypes eVarType;

            switch (eType)
            {
                case LinkScriptVariable.MathTypes.mtAdd:
                    {
                        sCommandName = "Add";
                        break;
                    }
                case LinkScriptVariable.MathTypes.mtDivide:
                    {
                        sCommandName = "Divide";
                        break;
                    }
                case LinkScriptVariable.MathTypes.mtMultiply:
                    {
                        sCommandName = "Multiply";
                        break;
                    }
                case LinkScriptVariable.MathTypes.mtSubtract:
                    {
                        sCommandName = "Subtract";
                        break;
                    }

                default:
                    {
                        sCommandName = "Math";
                        break;
                    }
            }

            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception(string.Format("Error - ArgOne for command \"{0}\", not defined.", sCommandName));
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception(string.Format("Error - ArgTwo for command \"{0}\", not defined.", sCommandName));
            // Get VarOne value...
            eVarType = GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // DebugPrint("{0} Arg 1 {1}={2}", "Math", sVarOne, vOneValue.ToString)
            if (eVarType != LinkScriptVariable.VariableTypes.vtDataTable & Information.VarType(vOneValue) == VariantType.Date)
            {
                bOneIsADate = true;
                vOneValue = Conversions.ToDate(vOneValue).ToOADate();
            }
            // Get VarTwo value...
            eVarType = GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            vTwoValue = CurrentVariable;
            // DebugPrint("{0} Arg 2 {1}={2}", "Math", sVarTwo, vTwoValue.ToString)
            if (eVarType != LinkScriptVariable.VariableTypes.vtDataTable & Information.VarType(vTwoValue) == VariantType.Date)
                vTwoValue = Conversions.ToDate(vTwoValue).ToOADate();
            // Do math on values...
            switch (eType)
            {
                case LinkScriptVariable.MathTypes.mtAdd:
                    {
                        vOneValue = Conversions.ToDouble(vOneValue) + Conversions.ToDouble(vTwoValue);
                        break;
                    }
                case LinkScriptVariable.MathTypes.mtDivide:
                    {
                        vOneValue = Conversions.ToDouble(vOneValue) / Conversions.ToDouble(vTwoValue);
                        break;
                    }
                case LinkScriptVariable.MathTypes.mtMultiply:
                    {
                        vOneValue = Conversions.ToDouble(vOneValue) * Conversions.ToDouble(vTwoValue);
                        break;
                    }
                case LinkScriptVariable.MathTypes.mtSubtract:
                    {
                        vOneValue = Conversions.ToDouble(vOneValue) - Conversions.ToDouble(vTwoValue);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            if (bOneIsADate)
                vOneValue = DateTime.FromOADate(Conversions.ToDouble(vOneValue));
            // Place appended VarOne and VarTwo in Result...
            // DebugPrint("{0} Result {1}={2}", "Math", sResult, vOneValue.ToString)
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception(string.Format("Error - Result for command \"{0}\", not defined.", sCommandName));
        }

        private bool SetResultValue(string sResult, object vValue, ref LinkScriptVariables privateVariables, string sTableName = "")
        {
            if (string.IsNullOrEmpty(sResult))
                return true;
            // Is this a Public or Private Variable?
            if (VariableExists(sResult))
            {
                // Is the public var an Object?
                if (this.IsVarAnObject(sResult, LinkScriptVariable.VarScope.vsPublic, ref privateVariables))
                {
                    SetVariable(sResult, vValue, sTableName);
                }
                else
                {
                    SetVariableValue(sResult, this.ConvertToVariableType(sResult, LinkScriptVariable.VarScope.vsPublic, vValue, ref privateVariables));
                }
            }
            else if (VariableExists(sResult, ref privateVariables))
            {
                // Is the Private var an Object?
                if (this.IsVarAnObject(sResult, LinkScriptVariable.VarScope.vsPrivate, ref privateVariables))
                {
                    SetVariable(sResult, vValue, ref privateVariables, sTableName);
                }
                else
                {
                    SetVariableValue(sResult, this.ConvertToVariableType(sResult, LinkScriptVariable.VarScope.vsPrivate, vValue, ref privateVariables), ref privateVariables);
                }
            }
            else if (Strings.Left(Strings.Trim(sResult), 1) == "_" | Strings.InStr(1, Strings.Trim(sResult), "._", CompareMethod.Text) > 0)
            {
                // Property value specified as result...
                SetProperty(sResult, vValue, ref privateVariables);
            }

            return true;
        }

        private void GetSubString(string sVarOne, string sVarTwo, string sResult, LinkScriptVariable.CharTypes eType, ref LinkScriptVariables privateVariables, object vStart = null)
        {
            int lNum = -1;
            var lStart = default(int);
            string sCommandName;
            object vOneValue;

            switch (eType)
            {
                case LinkScriptVariable.CharTypes.ctLeft:
                    {
                        sCommandName = "Left";
                        break;
                    }
                case LinkScriptVariable.CharTypes.ctMid:
                    {
                        sCommandName = "Mid";
                        break;
                    }
                case LinkScriptVariable.CharTypes.ctRight:
                    {
                        sCommandName = "Right";
                        break;
                    }

                default:
                    {
                        sCommandName = "Mid/Right/Left";
                        break;
                    }
            }
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception(string.Format("Error - ArgOne for command \"{0}\", not defined.", sCommandName));
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // DebugPrint("{0} Arg 1 {1}={2}", "GetSubString", vOneValue.ToString, vOneValue.ToString)
            // Get VarTwo value...
            if (!string.IsNullOrEmpty(sVarTwo))
            {
                GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
                try
                {
                    lNum = Conversions.ToInteger(CurrentVariable);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    switch (eType)
                    {
                        case LinkScriptVariable.CharTypes.ctMid:
                            {
                                lNum = -1;
                                break;
                            }

                        default:
                            {
                                lNum = 0;
                                break;
                            }
                    }
                }
                // DebugPrint("{0} Length {1}={2}", "GetSubString", sVarTwo, lNum.ToString)
            }
            // Get Start value (Mid only)...
            if (!(vStart == null))
            {
                GetVariableTypeAndSetValue(vStart.ToString(), ref privateVariables);
                try
                {
                    lStart = Conversions.ToInteger(CurrentVariable);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    lStart = 1;
                }

                // DebugPrint("{0} Start {1}={2}", "GetSubString", vStart.ToString, lStart.ToString)
            }
            // Get Chars accordingly...
            switch (eType)
            {
                case LinkScriptVariable.CharTypes.ctLeft:
                    {
                        vOneValue = Strings.Left(vOneValue.ToString(), lNum);
                        break;
                    }
                case LinkScriptVariable.CharTypes.ctMid:
                    {
                        if (lNum == -1)
                        {
                            vOneValue = Strings.Mid(vOneValue.ToString(), lStart);
                        }
                        else
                        {
                            vOneValue = Strings.Mid(vOneValue.ToString(), lStart, lNum);
                        }

                        break;
                    }
                case LinkScriptVariable.CharTypes.ctRight:
                    {
                        vOneValue = Strings.Right(vOneValue.ToString(), lNum);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            // Place in Result...
            // DebugPrint("{0} Result {1}={2}", "GetSubString", sResult, vOneValue.ToString)
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception(string.Format("Error - Result for command \"{0}\", not defined.", sCommandName));
        }

        private void TrimVariable(string sVarOne, string sResult, LinkScriptVariable.TrimTypes eType, ref LinkScriptVariables privateVariables)
        {
            string sCommandName;
            object vOneValue;

            switch (eType)
            {
                case LinkScriptVariable.TrimTypes.ttTrim:
                    {
                        sCommandName = "Trim";
                        break;
                    }
                case LinkScriptVariable.TrimTypes.ttLTrim:
                    {
                        sCommandName = "LTrim";
                        break;
                    }
                case LinkScriptVariable.TrimTypes.ttRTrim:
                    {
                        sCommandName = "RTrim";
                        break;
                    }

                default:
                    {
                        sCommandName = "Trim(L/R)";
                        break;
                    }
            }
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception(string.Format("Error - ArgOne for command \"{0}\", not defined.", sCommandName));
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // Trim accordingly...
            switch (eType)
            {
                case LinkScriptVariable.TrimTypes.ttTrim:
                    {
                        vOneValue = vOneValue.ToString().Trim();
                        break;
                    }
                case LinkScriptVariable.TrimTypes.ttLTrim:
                    {
                        vOneValue = vOneValue.ToString().TrimStart();
                        break;
                    }
                case LinkScriptVariable.TrimTypes.ttRTrim:
                    {
                        vOneValue = vOneValue.ToString().TrimEnd();
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            // Place the "trimmed" VarOne in Result...
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception(string.Format("Error - ArgOne for command \"{0}\", not defined.", sCommandName));
        }

        private void MoveVariable(string sVarOne, string sResult, ref LinkScriptVariables privateVariables)
        {
            object vOneValue;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"Move\", not defined.");
            // Get Value to move...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // Set Result vars value...
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception("Error - Result for command \"Move\", not defined.");
        }

        private void LengthOfVariable(string sVarOne, string sResult, ref LinkScriptVariables privateVariables)
        {
            object vOneValue;
            LinkScriptVariable.VariableTypes eVarType;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"Length\", not defined.");
            // Get Value to get the Length Of...
            eVarType = GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            if (eVarType != LinkScriptVariable.VariableTypes.vtString)
                throw new Exception("Error - ArgOne for command \"Length\" must be a string.");
            vOneValue = CurrentVariable.ToString().Length;
            // Set Result vars value...
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception("Error - Result for command \"Length\", not defined.");
        }

        private void DeleteOSFile(string sVarOne, string sResult, ref LinkScriptVariables privateVariables)
        {
            LinkScriptVariable.VariableTypes eVarType;
            string sErrorStr = string.Empty;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"DeleteOSFile\", not defined.");
            // Get Value to get the Length Of...
            eVarType = GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            if (eVarType != LinkScriptVariable.VariableTypes.vtString)
            {
                sErrorStr = "Error - ArgOne for command \"DeleteOSFile\" must be a string.";
            }
            else if (string.IsNullOrEmpty(CurrentVariable.ToString()))
            {
                sErrorStr = "Error - ArgOne for command \"DeleteOSFile\" must be a string.";
            }
            else
            {
                try
                {
                    System.IO.File.Delete(CurrentVariable.ToString());
                }
                catch (Exception ex)
                {
                    sErrorStr = ex.Message;
                }
            }
            // Set Result vars value...
            if (!string.IsNullOrEmpty(sResult))
            {
                if (!SetResultValue(sResult, sErrorStr, ref privateVariables))
                    throw new Exception("Error - Result for command \"DeleteOSFile\", not defined.");
            }
        }

        private void CreateUserlink(string sTableName, string sFieldName, ref LinkScriptVariables privateVariables, SqlConnection conn)
        {
            int iUserlinkCtr;
            string sVarTable;
            string sVarField;
            DataRow oTable;
            // Did var exist?
            if (string.IsNullOrEmpty(sTableName))
                throw new Exception("Error - ArgOne for command \"CreateUserlink\", not defined.");
            if (string.IsNullOrEmpty(sFieldName))
                throw new Exception("Error - ArgTwo for command \"CreateUserlink\", not defined.");
            // Get VarOne value...
            GetVariableTypeAndSetValue(sTableName, ref privateVariables);
            sVarTable = CurrentVariable.ToString();
            // Get VarTwo value...
            GetVariableTypeAndSetValue(sFieldName, ref privateVariables);
            sVarField = CurrentVariable.ToString();

            oTable = ScriptEngine.GetTableRow(StripQuotes(sTableName), conn);
            int _userLinkIndexTableIdSize = 30;
            if (oTable is not null)
                sVarField = sVarField.PadLeft(_userLinkIndexTableIdSize, '0');
            // Resize array...
            iUserlinkCtr = 0;
            iUserlinkCtr = Information.UBound(UserLink, 2);
            iUserlinkCtr = iUserlinkCtr + 1;
            UserLink = new string[2, iUserlinkCtr + 1];
            // Add to array...
            // Set this in the Prompt Screen property for calling application can access.
            UserLink[0, iUserlinkCtr] = sVarTable;
            UserLink[1, iUserlinkCtr] = sVarField;
        }

        private MsgBoxResult DisplayMsgBox(string sVarOne, string sVarTwo, string sVarThree, string sResult, ref LinkScriptVariables privateVariables)
        {
            MsgBoxResult rtn;
            object vOneValue;
            object vTwoValue;
            var eStyle = MsgBoxStyle.OkOnly;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"MsgBox\", not defined.");
            // Get VarOne value...
            _messageType = "s";
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // Get VarTwo value...
            if (!string.IsNullOrEmpty(sVarTwo))
            {
                GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
                vTwoValue = CurrentVariable;
            }
            else
            {
                vTwoValue = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }

            if (!string.IsNullOrEmpty(sVarThree))
            {
                if (Strings.InStr(1, sVarThree, "VBCRITICAL", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.Critical;
                    _messageType = "e";
                }
                else if (Strings.InStr(1, sVarThree, "VBEXCLAMATION", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.Exclamation;
                    _messageType = "w";
                }
                else if (Strings.InStr(1, sVarThree, "VBINFORMATION", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.Information;
                }
                else if (Strings.InStr(1, sVarThree, "VBQUESTION", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.Question;
                }

                if (Strings.InStr(1, sVarThree, "VBRETRYCANCEL", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.RetryCancel;
                }
                else if (Strings.InStr(1, sVarThree, "VBYESNOCANCEL", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.YesNoCancel;
                }
                else if (Strings.InStr(1, sVarThree, "VBYESNO", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.YesNo;
                }
                else if (Strings.InStr(1, sVarThree, "VBABORTRETRYIGNORE", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.AbortRetryIgnore;
                }
                else if (Strings.InStr(1, sVarThree, "VBOKCANCEL", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.OkCancel;
                }
                else if (Strings.InStr(1, sVarThree, "VBOKONLY", CompareMethod.Text) > 0)
                {
                    eStyle = eStyle | MsgBoxStyle.OkOnly;
                }
            }

            rtn = ScriptMsgBox(vOneValue.ToString(), eStyle, vTwoValue.ToString());
            // Place DisplayMsgBox in Result...
            if (!string.IsNullOrEmpty(sResult))
            {
                if (!SetResultValue(sResult, rtn, ref privateVariables))
                    throw new Exception("Error - Result for command \"MsgBox\", not defined.");
            }

            return rtn;
        }

        private string ReturnMsgBoxPrompt(string sVarOne, string sVarTwo, string sVarThree, string sResult, ref LinkScriptVariables privateVariables)
        {
            // Did var exist?
            _messageType = "s";
            if (string.IsNullOrEmpty(sVarOne))
                return string.Empty;

            if (!string.IsNullOrEmpty(sVarThree))
            {
                if (Strings.InStr(1, sVarThree, "VBYESNO", CompareMethod.Text) > 0)
                {
                    _msgBoxResult = MsgBoxResult.Yes;
                }
                else
                {
                    if (Strings.InStr(1, sVarThree, "VBCRITICAL", CompareMethod.Text) > 0)
                    {
                        _messageType = "e";
                    }
                    else if (Strings.InStr(1, sVarThree, "VBEXCLAMATION", CompareMethod.Text) > 0)
                    {
                        _messageType = "w";
                    }

                    _msgBoxResult = MsgBoxResult.Ok;
                }
            }
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            return CurrentVariable.ToString();
        }

        private void ExecuteSQL(string sVarOne, string sVarTwo, string sResult, ref BaseCollection parameters, ref LinkScriptVariables privateVariables, SqlConnection conn)
        {
            int lResult;
            string sql = string.Empty;
            string sTableName = string.Empty;
            LinkScriptVariable tempVariable;
            LinkScriptVariable.VariableTypes variableType;

            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"ExecuteSQL\", not defined.");
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception("Error - ArgTwo for command \"ExecuteSQL\", not defined.");

            try
            {
                var dt = new DataWithCursor();

                variableType = GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
                if (variableType != LinkScriptVariable.VariableTypes.vtDataTable)
                    sTableName = CurrentVariable.ToString();

                variableType = GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
                if (variableType != LinkScriptVariable.VariableTypes.vtDataTable)
                    sql = CurrentVariable.ToString();

                variableType = LinkScriptVariable.VariableTypes.vtLong;
                if (!string.IsNullOrEmpty(sResult))
                    variableType = GetVariableTypeAndSetValue(sResult, ref privateVariables);

                if (variableType == LinkScriptVariable.VariableTypes.vtDataTable)
                {
                    tempVariable = (LinkScriptVariable)CurrentVariable;
                    dt = (DataWithCursor)tempVariable.Value;
                }

                if (parameters is null)
                    parameters = new BaseCollection();

                using (var cmd = new SqlCommand(ReplaceQuestionMarksInSql(sql, parameters), conn))
                {
                    foreach (SqlParameter param in parameters)
                        cmd.Parameters.Add(param);

                    if (variableType == LinkScriptVariable.VariableTypes.vtDataTable)
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                            this.SetResultValue(sResult, dt, ref privateVariables, sTableName);
                        }
                    }
                    else
                    {
                        lResult = cmd.ExecuteNonQuery();
                        if (!string.IsNullOrEmpty(sResult))
                            SetResultValue(sResult, lResult, ref privateVariables);
                    }

                    cmd.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private string ReplaceQuestionMarksInSql(string sql, BaseCollection parameters)
        {
            if (parameters is null || parameters.Count == 0)
                return sql;
            int position = sql.ToLower().IndexOf(" where ");
            if (position == -1)
                return sql;

            string selectPart = sql.Substring(0, position);
            sql = sql.Substring(position);

            for (int i = 0, loopTo = parameters.Count - 1; i <= loopTo; i++)
            {
                position = sql.ToLower().IndexOf("?");
                if (position == -1)
                    break;
                sql = sql.Substring(0, position) + ((SqlParameter)parameters.BaseItem(i)).ParameterName + sql.Substring(position + 1);
            }

            return selectPart + sql;
        }

        private void StartProcess(string varOne, string varTwo, string result, ref LinkScriptVariables privateVariables)
        {
            var noWindow = default(bool);
            var waitForExit = default(bool);
            bool valid;
            int lPos;
            int lReturn;
            string exeName = string.Empty;
            string workingFolder = string.Empty;
            string procErrors = string.Empty;
            string procOutputs = string.Empty;
            object vOneValue;
            // Did var exist?
            if (string.IsNullOrEmpty(varOne))
                throw new Exception("Error - ArgOne for command \"StartProcess\", not defined.");
            // Get VarOne value...
            GetVariableTypeAndSetValue(varOne, ref privateVariables);
            vOneValue = CurrentVariable;

            exeName = vOneValue.ToString();
            lPos = exeName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);

            if (lPos > -1)
            {
                workingFolder = exeName.Substring(0, lPos);

                if (lPos > 0)
                {
                    exeName = exeName.Substring(lPos + 1);
                }
                else
                {
                    exeName = string.Empty;
                }
            }
            else
            {
                exeName = vOneValue.ToString();
            }

            if (!string.IsNullOrEmpty(exeName))
            {
                if (string.Compare(varTwo, "WAIT", true) == 0)
                {
                    valid = true;
                    waitForExit = true;
                }
                else if (string.Compare(varTwo, "NOWINDOW", true) == 0)
                {
                    valid = true;
                    noWindow = true;
                }
                else if (string.IsNullOrWhiteSpace(varTwo))
                {
                    throw new Exception("Error - ArgTwo for command \"StartProcess\" must be \"WAIT\", \"NOWINDOW\" or blank.");
                }
                else
                {
                    valid = true;
                }

                if (valid)
                {
                    try
                    {
                        using (var proc = new Process())
                        {
                            proc.StartInfo.FileName = exeName;
                            proc.StartInfo.WorkingDirectory = workingFolder;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = noWindow;
                            proc.StartInfo.RedirectStandardError = true;
                            proc.StartInfo.RedirectStandardOutput = true;

                            proc.Start();
                            procErrors = proc.StandardError.ReadToEnd();
                            procOutputs = proc.StandardOutput.ReadToEnd();
                            if (waitForExit)
                                proc.WaitForExit();
                            lReturn = proc.ExitCode;
                        }

                        if (!string.IsNullOrWhiteSpace(procErrors))
                        {
                            ScriptMsgBox(string.Format("Error(s) occurred in \"StartProcess\":{0}{1}", Environment.NewLine, procErrors));
                        }
                        else if (!string.IsNullOrWhiteSpace(result))
                        {
                            if (!SetResultValue(result, lReturn, ref privateVariables))
                                throw new Exception("Error - Result for command \"StartProcess\", not defined.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error running process \"{1}\" in \"StartProcess\":{0}{2}", Environment.NewLine, exeName, ex.Message), ex.InnerException);
                    }
                }
            }
        }

        private void FileCopyCmd(string sVarOne, string sVarTwo, string sResult, ref LinkScriptVariables privateVariables)
        {
            object vOneValue;
            object vTwoValue;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"FileCopy\", not defined.");
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception("Error - ArgTwo for command \"FileCopy\", not defined.");
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // Get VarTwo value...
            GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            vTwoValue = CurrentVariable;

            if (!string.IsNullOrEmpty(vOneValue.ToString()) & !string.IsNullOrEmpty(vTwoValue.ToString()))
            {
                try
                {
                    FileSystem.FileCopy(vOneValue.ToString(), vTwoValue.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void SetFocusCmd(string sVarOne)
        {
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"SetFocus\", not defined.");

            bool bExists = false;
            ControlExists?.Invoke(sVarOne, ref bExists);

            if (bExists)
            {
                SetControlProp?.Invoke(sVarOne, ScriptControls.ControlProperties.cpStartFocusHere, (object)true);
            }
            else
            {
                throw new Exception("Error - ArgOne for command \"SetFocus\", is not a valid control name");
            }
        }

        private void FormatVar(string sVarOne, string sVarTwo, string sResult, ref LinkScriptVariables privateVariables)
        {
            string sFormatString;
            object vOneValue;
            object vTwoValue;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"Format\", not defined.");
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception("Error - ArgTwo for command \"Format\", not defined.");
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // Get VarTwo value...
            GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            vTwoValue = CurrentVariable;
            // Format value...
            if (Information.IsDate(vOneValue))
            {
                sFormatString = vTwoValue.ToString();

                if (sFormatString.Contains("m/d/"))
                {
                    sFormatString = sFormatString.Replace("m/d/", "M/d/");
                }
                else if (sFormatString.Contains("mm/d/"))
                {
                    sFormatString = sFormatString.Replace("mm/d/", "MM/d/");
                }
                else if (sFormatString.Contains("mm/dd/"))
                {
                    sFormatString = sFormatString.Replace("mm/dd/", "MM/dd/");
                }
                else if (sFormatString.Contains("/m"))
                {
                    sFormatString = sFormatString.Replace("/m", "/M");
                }
                else if (sFormatString.Contains("/mm"))
                {
                    sFormatString = sFormatString.Replace("/mm", "/MM");
                }

                vTwoValue = sFormatString;
            }

            vOneValue = Strings.Format(vOneValue, vTwoValue.ToString());
            // Place appended VarOne and VarTwo in Result...
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception("Error - Result for command \"Format\", not defined.");
        }

        private void InString(string sVarOne, string sVarTwo, string sVarThree, string sResult, ref LinkScriptVariables privateVariables)
        {
            object vOneValue;
            object vTwoValue;
            object vThreeValue;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"InString\", not defined.");
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception("Error - ArgTwo for command \"InString\", not defined.");
            if (string.IsNullOrEmpty(sVarThree))
                throw new Exception("Error - ArgThree for command \"InString\", not defined.");
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // DebugPrint("{0} Arg 1 {1}={2}", "InString", sVarOne, vOneValue.ToString)
            // Get VarTwo value...
            GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            vTwoValue = CurrentVariable;
            // DebugPrint("{0} Arg 2 {1}={2}", "InString", sVarTwo, vTwoValue.ToString)
            // Get VarThree Value...
            GetVariableTypeAndSetValue(sVarThree, ref privateVariables);
            vThreeValue = CurrentVariable;
            // DebugPrint("{0} Arg 3 {1}={2}", "InString", sVarThree, vThreeValue.ToString)
            // Format value...
            if (Conversions.ToInteger("0" + vOneValue.ToString()) <= 0)
            {
                vOneValue = 1;
            }
            vOneValue = Strings.InStr(Conversions.ToInteger(vOneValue), vTwoValue.ToString(), vThreeValue.ToString(), CompareMethod.Text);
            // DebugPrint("{0} Result {1}={2}", "InString", sResult, vOneValue.ToString)
            // Place appended VarOne and VarTwo in Result...
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception("Error - Result for command \"InString\", not defined.");
        }

        private void GetTableDisplayString(string sVarOne, string sVarTwo, string sResult, ref LinkScriptVariables privateVariables, SqlConnection conn)
        {
            LinkScriptVariable.VariableTypes eVarType;
            DataRow oTables;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception("Error - ArgOne for command \"GetTableDisplayString\", not defined.");
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception("Error - ArgTwo for command \"GetTableDisplayString\", not defined.");
            if (string.IsNullOrEmpty(sResult))
                throw new Exception("Error - Result for command \"GetTableDisplayString\", not defined.");
            // Get VarOne value...
            string sTableName = string.Empty;
            eVarType = GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            if (eVarType != LinkScriptVariable.VariableTypes.vtDataTable)
                sTableName = CurrentVariable.ToString();

            oTables = ScriptEngine.GetTableRow(sTableName, conn);
            if (oTables is null)
                throw new Exception("Error - ArgOne for command \"GetTableDisplayString\", is not a valid Table Name.");
            // Get VarTwo value...
            string sId = string.Empty;
            eVarType = GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            if (eVarType != LinkScriptVariable.VariableTypes.vtDataTable)
                sId = CurrentVariable.ToString();

            string sDisplayStr = Navigation.GetItemName(sTableName, sId, _passport, conn);
            // Set Result vars value...
            if (!SetResultValue(sResult, sDisplayStr, ref privateVariables))
                throw new Exception("Error - Result for command \"GetTableDisplayString\", not defined.");
        }

        private void SetToNothing(string sVarName, ref LinkScriptVariables privateVariables)
        {
            // Check Publics...
            if (VariableExists(sVarName))
            {
                if (_publicVariables is not null)
                    _publicVariables.Remove(sVarName);
            }
            // Check Privates...
            else if (VariableExists(sVarName, ref privateVariables))
                privateVariables.Remove(sVarName);
        }

        private void ReplaceString(string sVarOne, string sVarTwo, string sVarThree, ref bool bReplaceAll, string sResult, ref LinkScriptVariables privateVariables)
        {
            string sCommandName = "Replace";
            if (bReplaceAll)
                sCommandName = "ReplaceAll";

            object vOneValue;
            object vTwoValue;
            object vThreeValue;
            // Did var exist?
            if (string.IsNullOrEmpty(sVarOne))
                throw new Exception(string.Format("Error - ArgOne for command \"{0}\", not defined.", sCommandName));
            if (string.IsNullOrEmpty(sVarTwo))
                throw new Exception(string.Format("Error - ArgTwo for command \"{0}\", not defined.", sCommandName));
            if (string.IsNullOrEmpty(sVarThree))
                throw new Exception(string.Format("Error - ArgThree for command \"{0}\", not defined.", sCommandName));
            // Get VarOne value...
            GetVariableTypeAndSetValue(sVarOne, ref privateVariables);
            vOneValue = CurrentVariable;
            // Get VarTwo value...
            GetVariableTypeAndSetValue(sVarTwo, ref privateVariables);
            vTwoValue = CurrentVariable;
            // Get VarThree value...
            GetVariableTypeAndSetValue(sVarThree, ref privateVariables);
            vThreeValue = CurrentVariable;
            // Replace values...
            if (bReplaceAll)
            {
                vOneValue = Strings.Replace(vOneValue.ToString(), vTwoValue.ToString(), vThreeValue.ToString(), 1); // Replace all...
            }
            else
            {
                vOneValue = Strings.Replace(vOneValue.ToString(), vTwoValue.ToString(), vThreeValue.ToString(), 1, 1);
            } // Relace first only...
              // Place Replaces value into Result...
            if (!SetResultValue(sResult, vOneValue, ref privateVariables))
                throw new Exception(string.Format("Error - Result for command \"{0}\", not defined.", sCommandName));
        }

        private void ReadINIValue(string sInSection, string sInKey, string sInDefaultValue, object vResult, ref LinkScriptVariables privateVariables)
        {
            string sSection;
            string sKey;
            string sDefault = string.Empty;
            LinkScriptVariable.VariableTypes eVarType;
            // Did var exist?
            if (string.IsNullOrEmpty(sInSection))
                throw new Exception("Error - ArgOne for command \"ReadINIValue\", not defined.");
            if (string.IsNullOrEmpty(sInKey))
                throw new Exception("Error - ArgTwo for command \"ReadINIValue\", not defined.");
            // Get sInSection value (Section)...
            eVarType = GetVariableTypeAndSetValue(sInSection, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgOne for command \"ReadINIValue\", cannot be a recordset.");
            sSection = CurrentVariable.ToString();
            // Get sInKey value (Key)...
            eVarType = GetVariableTypeAndSetValue(sInKey, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgTwo for command \"ReadINIValue\", cannot be a recordset.");
            sKey = CurrentVariable.ToString();

            if (!string.IsNullOrEmpty(sInDefaultValue))
            {
                // Get sInDefault value (Default)...
                eVarType = GetVariableTypeAndSetValue(sInDefaultValue, ref privateVariables);
                if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                    throw new Exception("Error - ArgThree for command \"ReadINIValue\", cannot be a recordset.");
                sDefault = CurrentVariable.ToString();
            }

            SetResultValue(vResult.ToString(), INIFile.ReadString(sSection, sKey, sDefault), ref privateVariables);
        }

        private void WriteINIValue(string sInSection, string sInKey, string sInNewValue, ref LinkScriptVariables privateVariables)
        {
            string sSection;
            string sKey;
            string sNew = string.Empty;
            LinkScriptVariable.VariableTypes eVarType;
            // Did var exist?
            if (string.IsNullOrEmpty(sInSection))
                throw new Exception("Error - ArgOne for command \"WriteINIValue\", not defined.");
            if (string.IsNullOrEmpty(sInKey))
                throw new Exception("Error - ArgTwo for command \"WriteINIValue\", not defined.");
            // Get sInSection value (Section)...
            eVarType = GetVariableTypeAndSetValue(sInSection, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgOne for command \"WriteINIValue\", cannot be a recordset.");
            sSection = CurrentVariable.ToString();
            // Get sInKey value (Key)...
            eVarType = GetVariableTypeAndSetValue(sInKey, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgTwo for command \"WriteINIValue\", cannot be a recordset.");
            sKey = CurrentVariable.ToString();
            // Did var exist?
            if (!string.IsNullOrEmpty(sInNewValue))
            {
                // Get sInNew value (New)...
                eVarType = GetVariableTypeAndSetValue(sInNewValue, ref privateVariables);
                if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                    throw new Exception("Error - ArgThree for command \"WriteINIValue\", cannot be a recordset.");
                sNew = CurrentVariable.ToString();
            }

            INIFile.WriteString(sSection, sKey, sNew);
        }

        private void ReadSettingValue(string sInSection, string sInKey, string sInDefaultValue, object vResult, ref LinkScriptVariables privateVariables)
        {
            string sReturnString = string.Empty;
            string sSection;
            string sKey;
            string sDefault = string.Empty;
            LinkScriptVariable.VariableTypes eVarType;
            // Did var exist?
            if (string.IsNullOrEmpty(sInSection))
                throw new Exception("Error - ArgOne for command \"ReadSettingValue\", not defined.");
            if (string.IsNullOrEmpty(sInKey))
                throw new Exception("Error - ArgTwo for command \"ReadSettingValue\", not defined.");
            // Get sInSection value (Section)...
            eVarType = GetVariableTypeAndSetValue(sInSection, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgOne for command \"ReadSettingValue\", cannot be a recordset.");
            sSection = CurrentVariable.ToString();
            // Get sInKey value (Key)...
            eVarType = GetVariableTypeAndSetValue(sInKey, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgTwo for command \"ReadSettingValue\", cannot be a recordset.");
            sKey = CurrentVariable.ToString();

            if (!string.IsNullOrEmpty(sInDefaultValue))
            {
                // Get sInDefault value (Default)...
                eVarType = GetVariableTypeAndSetValue(sInDefaultValue, ref privateVariables);
                if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                    throw new Exception("Error - ArgThree for command \"ReadSettingValue\", cannot be a recordset.");
                sDefault = CurrentVariable.ToString();
            }

            string itemValue = Navigation.GetSetting(sSection, sKey, _passport);

            if (string.IsNullOrEmpty(itemValue))
            {
                sReturnString = sDefault;
            }
            else
            {
                sReturnString = itemValue;
            }

            SetResultValue(vResult.ToString(), sReturnString, ref privateVariables);
        }

        private void WriteSettingValue(string sInSection, string sInKey, string sInNewValue, ref LinkScriptVariables privateVariables)
        {
            string sSection;
            string sKey;
            string sNew = string.Empty;
            LinkScriptVariable.VariableTypes eVarType;
            // Did var exist?
            if (string.IsNullOrEmpty(sInSection))
                throw new Exception("Error - ArgOne for command \"WriteSettingValue\", not defined.");
            if (string.IsNullOrEmpty(sInKey))
                throw new Exception("Error - ArgTwo for command \"WriteSettingValue\", not defined.");
            // Get sInSection value (Section)...
            eVarType = GetVariableTypeAndSetValue(sInSection, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgOne for command \"WriteSettingValue\", cannot be a recordset.");
            sSection = CurrentVariable.ToString();
            // Get sInKey value (Key)...
            eVarType = GetVariableTypeAndSetValue(sInKey, ref privateVariables);
            if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                throw new Exception("Error - ArgTwo for command \"WriteSettingValue\", cannot be a recordset.");
            sKey = CurrentVariable.ToString();
            // Did var exist?
            if (!string.IsNullOrEmpty(sInNewValue))
            {
                // Get sInNew value (New)...
                eVarType = GetVariableTypeAndSetValue(sInNewValue, ref privateVariables);
                if (eVarType == LinkScriptVariable.VariableTypes.vtDataTable)
                    throw new Exception("Error - ArgThree for command \"WriteSettingValue\", cannot be a recordset.");
                sNew = CurrentVariable.ToString();
            }

            Navigation.SetSetting(sSection, sKey, sNew, _passport);
        }

        private void DoBarCodeTracking(string result, LinkScriptVariables privateVariables, SqlConnection conn)
        {
            string rtn = string.Empty;
            var objTable = ScriptEngine.GetTableRow(BCObjTableName, _passport);
            if (objTable is null)
                rtn = "Object table name not valid. [" + BCObjTableName + "]";
            var destTable = ScriptEngine.GetTableRow(BCDestTableName, _passport);
            if (destTable is null)
                rtn = "Destination table name not valid. [" + BCDestTableName + "]";

            if (string.IsNullOrEmpty(rtn))
            {
                try
                {
                    Tracking.Transfer(BCObjTableName, BCObjTableId, BCDestTableName, BCDestTableId, BCDueBack, ActiveUser, _passport, conn);
                }
                catch (Exception ex)
                {
                    rtn = ex.Message;
                }
            }

            SetResultValue(result, rtn, ref privateVariables);
        }

        private void InternalEngine_AddControl(ScriptControls.ControlTypes eControlType, string sControlName, string sModifier)
        {
            var _control = new ScriptControls();
            _control.ControlType = eControlType;

            if (!_scriptControlDictionary.ContainsKey(sControlName))
            {
                _scriptControlDictionary.Add(sControlName, _control);
            }
            else
            {
                _scriptControlDictionary[sControlName] = _control;
            }
        }

        private void InternalEngine_ControlAddItem(string sControlName, string sControlValue, int lItemData)
        {
            if (_scriptControlDictionary.ContainsKey(sControlName))
            {
                var _control = _scriptControlDictionary[sControlName];
                _control.AddItem(sControlValue);
                _control.AddItemData(lItemData.ToString());
            }
        }

        private void InternalEngine_GetControlProp(string sControlName, ScriptControls.ControlProperties eControlProperty, ref object vValue)
        {
            if (_scriptControlDictionary.ContainsKey(sControlName))
            {
                var _control = _scriptControlDictionary[sControlName];
                vValue = _control.GetProperty(eControlProperty);
            }
        }

        private void InternalEngine_SetControlProp(string sPrefix, ScriptControls.ControlProperties eControlProperty, object vValue)
        {
            if (_scriptControlDictionary.ContainsKey(sPrefix))
            {
                var _control = _scriptControlDictionary[sPrefix];
                _control.SetProperty(eControlProperty, vValue);
                _scriptControlDictionary[sPrefix] = _control;
            }
            else if (sPrefix.ToLower() == "_heading")
            {
            }
            // ToDo: custom heading control so it doesn't mess up control/label order
            else
            {
                var _control = new ScriptControls();
                _control.SetProperty(eControlProperty, vValue);
                _scriptControlDictionary.Add(sPrefix, _control);
            }
        }

        private void InternalEngine_UnloadPromptEvent(ref bool bSuccessful)
        {
            _scriptControlDictionary.Clear();
        }

        private void DebugPrint(string format, params object[] args)
        {
            /* TODO ERROR: Skipped IfDirectiveTrivia
            #If DEBUGPRINT Then
            *//* TODO ERROR: Skipped DisabledTextTrivia
                    Debug.Print(format, args)
            *//* TODO ERROR: Skipped EndIfDirectiveTrivia
            #End If
            */
        }
    }
}