using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Smead.Security;

namespace MSRecordsEngine.RecordsManager
{

    public class Request
    {
        private int _requestID;
        private bool _IdFieldIsString;
        private bool _employeeIdFieldIsString;

        public Request(string tableName, Passport passport)
        {
            _tableName = tableName;
            _IdFieldIsString = Navigation.FieldIsAString(_tableName, passport);
            _employeeIdFieldIsString = Navigation.FieldIsAString(Tracking.GetRequestorTableName(passport), passport);
        }

        public int RequestID
        {
            get
            {
                return _requestID;
            }
            set
            {
                _requestID = value;
            }
        }
        private string _tableName;
        public string TableName
        {
            get
            {
                return _tableName;
            }
        }
        private string _tableID;
        public string TableID
        {
            get
            {
                if (_IdFieldIsString)
                    return _tableID;
                return Navigation.PrepPad(_tableID);
            }
            set
            {
                _tableID = value;
            }
        }
        private DateTime _dateNeeded;
        public DateTime DateNeeded
        {
            get
            {
                return _dateNeeded;
            }
            set
            {
                _dateNeeded = value;
            }
        }
        private string _instructions;
        public string Instructions
        {
            get
            {
                return _instructions;
            }
            set
            {
                _instructions = value;
            }
        }
        private DateTime _dateRequested;
        public DateTime DateRequested
        {
            get
            {
                return _dateRequested;
            }
            set
            {
                _dateRequested = value;
            }
        }
        private string _employeeID;
        public string EmployeeID
        {
            get
            {
                if (_employeeIdFieldIsString)
                    return _employeeID;
                return Navigation.PrepPad(_employeeID);
            }
            set
            {
                _employeeID = value;
            }
        }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        private string _priority;
        public string Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }
        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        private string _RequestedBy;
        public string RequestedBy
        {
            get
            {
                return _RequestedBy;
            }
            set
            {
                _RequestedBy = value;
            }
        }
        private int _pullListID;
        public int PullListID
        {
            get
            {
                return _pullListID;
            }
            set
            {
                _pullListID = value;
            }
        }
        private string _exceptionComments;
        public string ExceptionComments
        {
            get
            {
                return _exceptionComments;
            }
            set
            {
                _exceptionComments = value;
            }
        }
    }

    public class Employee
    {
        private DataRow _tableInfo;
        private DataRow _employee;

        public Employee(SqlConnection conn)
        {
            string requestorTable = Tracking.GetRequestorTableName(conn);
            _tableInfo = Navigation.GetTableInfo(requestorTable, conn);
        }
        public Employee(DataRow tableInfo)
        {
            _tableInfo = tableInfo;
        }

        public void LoadByID(string id, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                string sql;

                if (Navigation.FieldIsAString(_tableInfo, conn))
                {
                    sql = string.Format("SELECT * FROM [{0}] WHERE [{1}] = '{2}'", _tableInfo["TableName"].ToString(), Navigation.MakeSimpleField(_tableInfo["IdFieldName"].ToString()), id.Replace("'", "''"));
                }
                else
                {
                    sql = string.Format("SELECT * FROM [{0}] WHERE [{1}] = {2}", _tableInfo["TableName"].ToString(), Navigation.MakeSimpleField(_tableInfo["IdFieldName"].ToString()), id);
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            _employee = dt.Rows[0];
                    }
                }
            }
        }

        public bool LoadByName(string name, Passport passport)
        {
            if (string.IsNullOrEmpty(_tableInfo["OperatorsIdField"].ToString()))
                return false;

            using (var conn = passport.Connection())
            {
                string sql = string.Format("SELECT * FROM [{0}] WHERE [{1}] = @operatorsId", _tableInfo["TableName"].ToString(), Navigation.MakeSimpleField(_tableInfo["OperatorsIdField"].ToString()));

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@operatorsId", name);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            _employee = dt.Rows[0];
                            return true;
                        }

                        return false;
                    }
                }
            }
        }

        public string ID
        {
            get
            {
                return _employee[Navigation.MakeSimpleField(_tableInfo["IdFieldName"].ToString())].ToString();
            }
        }
        public string Description
        {
            get
            {
                string desc1 = string.Empty;
                string desc2 = string.Empty;
                string descFieldName1 = string.Empty;
                string descFieldName2 = string.Empty;

                if (!string.IsNullOrEmpty(_tableInfo["DescFieldNameOne"].ToString()))
                {
                    if ((_tableInfo["DescFieldNameOne"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(_tableInfo) ?? "") & _tableInfo["BarCodePrefix"].ToString().Length > 0)
                    {
                        desc1 = Navigation.BarcodeText(_tableInfo, ID);
                    }
                    else
                    {
                        descFieldName1 = _tableInfo["DescFieldNameOne"].ToString();
                        desc1 = _employee[descFieldName1].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(_tableInfo["DescFieldNameTwo"].ToString()))
                {
                    if ((_tableInfo["DescFieldNameTwo"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(_tableInfo) ?? "") & _tableInfo["BarCodePrefix"].ToString().Length > 0)
                    {
                        desc2 = Navigation.BarcodeText(_tableInfo, ID);
                    }
                    else
                    {
                        descFieldName2 = _tableInfo["DescFieldNameTwo"].ToString();
                        desc2 = _employee[descFieldName2].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(desc1))
                {
                    string prefix1 = _tableInfo["DescFieldPrefixOne"].ToString();
                    if (!string.IsNullOrEmpty(prefix1))
                        desc1 = prefix1 + " " + desc1;
                }
                if (!string.IsNullOrEmpty(desc2))
                {
                    string prefix2 = _tableInfo["DescFieldPrefixTwo"].ToString();
                    if (!string.IsNullOrEmpty(prefix2))
                        desc2 = prefix2 + " " + desc2;
                }
                if (string.IsNullOrEmpty(desc1) & string.IsNullOrEmpty(desc2))
                    return ID;
                if (string.IsNullOrEmpty(desc1))
                    return desc2;
                if (string.IsNullOrEmpty(desc2))
                    return desc1;
                return desc1 + " " + desc2;
            }
        }
        public string Phone
        {
            get
            {
                if (string.IsNullOrEmpty(_tableInfo["TrackingPhoneFieldName"].ToString()))
                    return string.Empty;
                return _employee[_tableInfo["TrackingPhoneFieldName"].ToString()].ToString();
            }
        }
        public string MailStop
        {
            get
            {
                if (string.IsNullOrEmpty(_tableInfo["TrackingMailStopFieldName"].ToString()))
                    return string.Empty;
                return _employee[_tableInfo["TrackingMailStopFieldName"].ToString()].ToString();
            }
        }
    }

    public class Requesting
    {
        public List<Request> GetActiveRequests(int viewId, string tableId, Passport passport, SqlConnection conn)
        {
            string tableName = Navigation.GetViewTableName(viewId, conn);
            tableId = Navigation.PrepPad(tableName, tableId, passport);
            Request request;
            var requestList = new List<Request>();

            using (var requestAdapter = new RecordsManageTableAdapters.SLRequestorTableAdapter())
            {
                requestAdapter.Connection = conn;

                foreach (DataRow row in requestAdapter.GetActiveRequests(viewId, tableId).Rows)
                {
                    if (!Tracking.AlreadyAtEmployee(tableName, tableId, row["EmployeeId"].ToString(), conn))
                    {
                        request = new Request(tableName, passport);
                        request.RequestID = Conversions.ToInteger(row["Id"]);
                        request.DateRequested = Conversions.ToDate(row["DateRequested"]);
                        request.EmployeeID = row["EmployeeId"].ToString();
                        request.RequestedBy = row["RequestedBy"].ToString();
                        string requestTable = Tracking.GetRequestorTableName(conn);
                        request.Name = Navigation.GetItemName(requestTable, request.EmployeeID.ToString(), passport, conn);

                        if (row["DateNeeded"] is DBNull)
                        {
                            request.DateNeeded = new DateTime();
                        }
                        else
                        {
                            request.DateNeeded = Conversions.ToDate(row["DateNeeded"]);
                        }

                        request.Instructions = row["instructions"].ToString();
                        request.Priority = row["Priority"].ToString();
                        request.Status = row["Status"].ToString();
                        if (!(row["SLPullListsId"] is DBNull))
                        {
                            request.PullListID = Conversions.ToInteger(row["SLPullListsId"]);
                        }
                        else
                        {
                            request.PullListID = 0;
                        }

                        request.ExceptionComments = row["ExceptionComments"].ToString();
                        requestList.Add(request);
                    }
                }

                return requestList;
            }
        }

        public static List<Request> GetActiveRequests(string tableName, string tableId, Passport passport)
        {
            tableId = Navigation.PrepPad(tableName, tableId, passport);
            Request request;
            var requestList = new List<Request>();
            string sql = "SELECT      SlRequestor.SLPullListsId,  SLRequestor.Priority, SLRequestor.DateRequested, ISNULL(SLRequestor.DateReceived, 0) AS DateRecieved, " + "SLRequestor.Status, ISNULL(SLRequestor.ExceptionComments, '') AS Comments, SLRequestor.PriorityOrder, SLRequestor.TableId, SLRequestor.DateNeeded,   " + "SLRequestor.EmployeeId, SLRequestor.Instructions, SLRequestor.ExceptionComments, SLRequestor.Id, SLRequestor.RequestedBy FROM SLRequestor " + "WHERE SLRequestor.TableName=@TableName AND SLRequestor.TableId = @TableId AND SLRequestor.Status in ('New','In Process','WaitList','Exception','New Batch')";


            using (var conn = passport.Connection())
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TableID", tableId);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!Tracking.AlreadyAtEmployee(tableName, tableId, row["EmployeeId"].ToString(), conn))
                        {
                            request = new Request(tableName, passport);
                            request.RequestID = Conversions.ToInteger(row["Id"]);
                            request.DateRequested = Conversions.ToDate(row["DateRequested"]);
                            request.EmployeeID = row["EmployeeId"].ToString();
                            request.RequestedBy = row["RequestedBy"].ToString();
                            string requestTable = Tracking.GetRequestorTableName(conn);
                            request.Name = Navigation.GetItemName(requestTable, request.EmployeeID.ToString(), passport, conn);

                            if (row["DateNeeded"] is DBNull)
                            {
                                request.DateNeeded = new DateTime();
                            }
                            else
                            {
                                request.DateNeeded = Conversions.ToDate(row["DateNeeded"]);
                            }

                            request.Instructions = row["instructions"].ToString();
                            request.Priority = row["Priority"].ToString();
                            request.Status = row["Status"].ToString();
                            if (!(row["SLPullListsId"] is DBNull))
                            {
                                request.PullListID = Conversions.ToInteger(row["SLPullListsId"]);
                            }
                            else
                            {
                                request.PullListID = 0;
                            }

                            request.ExceptionComments = row["ExceptionComments"].ToString();
                            requestList.Add(request);
                        }
                    }
                }
            }
            return requestList;
        }

        public static Request GetRequest(int requestId, string tableName, Passport passport)
        {
            Request request;

            using (var conn = passport.Connection())
            {
                var cmd = new SqlCommand("SELECT * FROM SLRequestor WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", requestId);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        request = new Request(tableName, passport);
                        request.RequestID = Conversions.ToInteger(row["Id"]);
                        request.DateRequested = Conversions.ToDate(row["DateRequested"]);
                        request.EmployeeID = row["EmployeeId"].ToString();
                        request.RequestedBy = row["RequestedBy"].ToString();
                        string requestTable = Tracking.GetRequestorTableName(conn);
                        request.Name = Navigation.GetItemName(requestTable, request.EmployeeID.ToString(), passport, conn);

                        if (row["DateNeeded"] is DBNull)
                        {
                            request.DateNeeded = new DateTime();
                        }
                        else
                        {
                            request.DateNeeded = Conversions.ToDate(row["DateNeeded"]);
                        }

                        request.Instructions = row["instructions"].ToString();
                        request.Priority = row["Priority"].ToString();
                        request.Status = row["Status"].ToString();
                        request.PullListID = Conversions.ToInteger(row["SLPullListsID"]);
                        request.ExceptionComments = row["ExceptionComments"].ToString();
                        return request;
                    }
                    // Modified By Hemin
                    // Throw New NullReferenceException("The request could not be found.")
                    throw new NullReferenceException("The request could not be found.");
                }
            }
        }

        public static DataTable GetBatchRequests(Passport passport, string OperatorsID = "")
        {
            string sql = "SELECT * FROM SLBatchRequests";
            var dt = new DataTable();

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(string.Empty, conn))
                {
                    if (string.IsNullOrWhiteSpace(OperatorsID))
                    {
                        cmd.CommandText = sql + " ORDER BY PriorityOrder";
                    }
                    else
                    {
                        cmd.CommandText = sql + " WHERE OperatorsId = @OperatorsId ORDER BY PriorityOrder";
                        cmd.Parameters.AddWithValue("@OperatorsID", OperatorsID);
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public static DataTable GetBatchRequest(Passport passport, string Id)
        {
            string sql = "SELECT * FROM SLBatchRequests where Id=@Id";
            var dt = new DataTable();

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public static void DeleteBatchRequest(Passport passport, string Id)
        {
            string sql = "Delete FROM SLBatchRequests where Id=@Id";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void SaveBatchRequest(string OperatorsId, string Comment, string RequestedIds, string EmployeeId, bool StayOnTop, string RequestedTable, int PriorityOrder, Passport passport, int Id = -1)
        {
            string sql;

            if (Id == -1) // Insert
            {
                sql = "INSERT INTO SLBatchRequests (OperatorsId, DateCreated, Comment, RequestedIds, EmployeeId, StayOnTop, RequestedTable, PriorityOrder) ";
                sql += "VALUES (@OperatorsId, @DateCreated, @Comment, @RequestedIds, @EmployeeId, @StayOnTop, @RequestedTable, @PriorityOrder)";

                using (var conn = passport.Connection())
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@OperatorsId", OperatorsId);
                        cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Comment", Comment);
                        cmd.Parameters.AddWithValue("@RequestedIds", RequestedIds);
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmd.Parameters.AddWithValue("@StayOnTop", StayOnTop);
                        cmd.Parameters.AddWithValue("@RequestedTable", RequestedTable);
                        cmd.Parameters.AddWithValue("@PriorityOrder", PriorityOrder);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else // Update
            {
                sql = "UPDATE SLBatchRequests SET OperatorsId = @OperatorsId, Comment = @Comment, RequestedIds = @RequestedIds, EmployeeId = @EmployeeId, ";
                sql += "StayOnTop = @StayOnTop, RequestedTable = @RequestedTable, PriorityOrder = @PriorityOrder WHERE Id = @Id";

                using (var conn = passport.Connection())
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@OperatorsId", OperatorsId);
                        cmd.Parameters.AddWithValue("@Comment", Comment);
                        cmd.Parameters.AddWithValue("@RequestedIds", RequestedIds);
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmd.Parameters.AddWithValue("@StayOnTop", StayOnTop);
                        cmd.Parameters.AddWithValue("@RequestedTable", RequestedTable);
                        cmd.Parameters.AddWithValue("@PriorityOrder", PriorityOrder);
                        cmd.Parameters.AddWithValue("@Id", Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<Request> GetActiveRequests(int viewId, string tableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetActiveRequests(viewId, tableId, passport, conn);
            }
        }

        public void InsertRequest(Request request, Passport passport)
        {
            if (!passport.CheckPermission(request.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Request))
            {
                throw new Exception("Not authorized to request these items.");
            }
            else
            {
                using (var fileRoom = new RecordsManageTableAdapters.SLTableFileRoomOrderTableAdapter())
                {
                    using (var conn = passport.Connection())
                    {
                        string fileRoomSQL = string.Empty;
                        string sql = string.Empty;
                        string fileRoomOrder;
                        var tableInfo = Navigation.GetTableInfo(request.TableName, conn);
                        fileRoom.Connection = conn;

                        foreach (RecordsManage.SLTableFileRoomOrderRow row in fileRoom.GetData(request.TableName))
                        {
                            if (row.StartFromFront)
                            {
                                if (Navigation.FieldIsAString(tableInfo, conn))
                                {
                                    fileRoomSQL += string.Format("SUBSTRING(CAST({0} AS VARCHAR(50))+REPLICATE(' '," + (row.NumberofCharacters + row.StartingPosition).ToString() + "),{1},{2} )+", row.FieldName, (object)row.StartingPosition, (object)row.NumberofCharacters);
                                }
                                else
                                {
                                    fileRoomSQL += string.Format("SUBSTRING(CAST({0} AS VARCHAR(50))+REPLICATE('0'," + (row.NumberofCharacters + row.StartingPosition).ToString() + "),{1},{2} )+", row.FieldName, (object)row.StartingPosition, (object)row.NumberofCharacters);
                                }
                            }
                            else if (Navigation.FieldIsAString(tableInfo, conn))
                            {
                                fileRoomSQL += string.Format("SUBSTRING(RIGHT(REPLICATE(' ', " + ((row.NumberofCharacters + row.StartingPosition).ToString() + ")+CAST({0} AS VARCHAR(50)),{1}),1,{2})+"), row.FieldName, (object)row.StartingPosition, (object)row.NumberofCharacters);
                            }
                            else
                            {
                                fileRoomSQL += string.Format("SUBSTRING(RIGHT(REPLICATE('0', " + ((row.NumberofCharacters + row.StartingPosition).ToString() + ")+CAST({0} AS VARCHAR(50)),{1}),1,{2})+"), row.FieldName, (object)row.StartingPosition, (object)row.NumberofCharacters);
                            }
                        }

                        if (!string.IsNullOrEmpty(fileRoomSQL))
                        {
                            sql = "SELECT " + Strings.Left(fileRoomSQL, Strings.Len(fileRoomSQL) - 1) + " AS fileRoomOrder FROM " + request.TableName + " WHERE " + tableInfo["idFieldName"].ToString() + "=";

                            if (Navigation.FieldIsAString(tableInfo, conn))
                            {
                                sql += "'" + request.TableID + "'";
                            }
                            else
                            {
                                sql += request.TableID;
                            }

                            using (var cmd = new SqlCommand(sql, conn))
                            {
                                fileRoomOrder = cmd.ExecuteScalar().ToString();
                            }
                        }
                        else
                        {
                            fileRoomOrder = string.Empty;
                        }

                        sql = "INSERT INTO SLRequestor (TableName, TableId, EmployeeId, Priority, Status, Instructions, DateNeeded, PriorityOrder, RequestedBy, " + "DateRequested, FileRoomOrder, SLPullListsId) VALUES (@tableName, @tableID, @employeeID, @Priority, @Status, @Instructions, @DateNeeded, " + "@PriorityOrder, @RequestedBy, GETDATE(), @FileRoomOrder, @SLPullListsId)";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@tableName", request.TableName);
                            cmd.Parameters.AddWithValue("@tableID", request.TableID);
                            cmd.Parameters.AddWithValue("@employeeID", request.EmployeeID);
                            cmd.Parameters.AddWithValue("@Priority", request.Priority);
                            cmd.Parameters.AddWithValue("@Status", request.Status);
                            cmd.Parameters.AddWithValue("@Instructions", request.Instructions);
                            cmd.Parameters.AddWithValue("@DateNeeded", request.DateNeeded);
                            cmd.Parameters.AddWithValue("@PriorityOrder", Conversions.ToInteger(Interaction.IIf(request.Priority.ToLower() == "standard", 0, 2)));
                            cmd.Parameters.AddWithValue("@RequestedBy", request.RequestedBy);
                            cmd.Parameters.AddWithValue("@FileRoomOrder", fileRoomOrder);
                            cmd.Parameters.AddWithValue("@SLPullListsId", request.PullListID);

                            cmd.ExecuteNonQuery();
                        }
                        if (Navigation.CBoolean(Navigation.GetSystemSetting("EMailWaitListEnabled", conn)))
                        {
                            if (request.Status == "WaitList")
                            {
                                if ((Tracking.GetTrackedItemLocation(passport, request.TableName, request.TableID) ?? "") == (Tracking.GetRequestorTableName(passport.Connection()) ?? ""))
                                {
                                    string message = "The following item(s) have been requested by: ";
                                    string employeeID = Tracking.GetTrackedItemLocationID(passport, request.TableName, request.TableID);
                                    var employee = new Employee(conn);
                                    employee.LoadByID(request.EmployeeID, passport);
                                    message = message + employee.Description + Environment.NewLine + Environment.NewLine + "Current Location: ";
                                    message = message + Tracking.GetTrackedItemLocationDescription(passport, request.TableName, request.TableID) + Environment.NewLine + Environment.NewLine;
                                    message = message + "Item             Description                              Date Requested" + Environment.NewLine;
                                    message = message + "------------------------------------------------------------------------" + Environment.NewLine;
                                    string item = request.TableName + ": " + Navigation.StripLeadingZeros(request.TableID);
                                    string description = Navigation.GetItemName(request.TableName, request.TableID, passport);
                                    string dateRequested = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                                    int firstlength = 17 - item.Length;
                                    int secondlength = 0;
                                    if (firstlength < 0)
                                    {
                                        secondlength = 55 - (description.Length + 17 - firstlength);
                                        firstlength = 0;
                                    }
                                    else
                                    {
                                        secondlength = 55 - (description.Length + 17);
                                    }
                                    if (secondlength < 0)
                                        secondlength = 0;
                                    message = message + item + new string(' ', firstlength) + description + new string(' ', secondlength) + dateRequested;
                                    if (!string.IsNullOrEmpty(request.Instructions))
                                    {
                                        message = message + Environment.NewLine + "Request Instructions: " + request.Instructions;
                                    }

                                    Navigation.SendEmail(message, Navigation.GetEmployeeEmailByID(employeeID, passport), Navigation.GetUserEmail(passport), "Wait List Notification", "", conn);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DeleteRequest(int requestId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                string sql = "SELECT [TableName], [TableId] from [SLRequestor] WHERE [Id] = @requestID; UPDATE [SLRequestor] SET [Status] = 'Deleted', [DateDeleted] = GetDate(), [DeleteOperatorID] = @UserID WHERE [Id] = @requestID AND [Status] <> 'WaitList'; DELETE FROM [SLRequestor] WHERE [Id] = @requestID AND [Status] = 'WaitList'";

                using (var cmd = new SqlCommand(sql, passport.Connection()))
                {
                    cmd.Parameters.AddWithValue("@requestID", requestId);
                    cmd.Parameters.AddWithValue("@UserID", new User(passport, true).UserName);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                            Tracking.UpdateNextRequestStatus(dt.Rows[0]["TableName"].ToString(), dt.Rows[0]["TableID"].ToString(), passport, conn);
                    }
                }
            }
        }

        public static void UpdateRequest(int requestId, bool isFulfulled, bool isException, string comment, Passport passport)
        {
            if (isException)
            {
                ExceptionRequest(requestId, comment, passport);
            }
            else if (isFulfulled)
            {
                FulfillRequest(requestId, passport);
            }
            else
            {
                ClearExceptionRequest(requestId, passport);
            }
        }

        public static void FulfillRequest(int requestId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                string sql = "UPDATE [SLRequestor] SET [Status] = 'Fulfilled', [DateReceived] = GetDate() WHERE [Id] = @requestID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@requestID", requestId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ExceptionRequest(int requestId, string comment, Passport passport)
        {
            string sql;
            var dt = new DataTable();

            using (var conn = passport.Connection())
            {
                sql = "UPDATE [SLRequestor] SET [Status] = 'Exception', [ExceptionComments] = @ExceptionComment, [DateExceptioned] = GetDate() WHERE [Id] = @requestID"; // and Status<>'Exception'"
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@requestID", requestId);
                    cmd.Parameters.AddWithValue("@ExceptionComment", comment);
                    cmd.ExecuteNonQuery();
                }

                if (Navigation.CBoolean(Navigation.GetSystemSetting("EMailExceptionEnabled", conn)))
                {
                    using (var cmd = new SqlCommand("SELECT * FROM [SLRequestor] WHERE [Id] = @requestID", conn))
                    {
                        cmd.Parameters.AddWithValue("@requestID", requestId);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        string message = "The following Item(s) are currently unavailable to fulfill your request: " + Environment.NewLine + Environment.NewLine;
                        // Dim employeeID = Tracking.GetTrackedItemLocationID(passport, dt.Rows(0)("TableName").ToString, dt.Rows(0)("TableId").ToString)
                        var employee = new Employee(conn);
                        employee.LoadByID(dt.Rows[0]["EmployeeId"].ToString(), passport);
                        message += "Item                                                      Requested by" + Environment.NewLine;
                        message += "----------------------------------------------------------------------" + Environment.NewLine;
                        string item = Navigation.GetItemName(dt.Rows[0]["TableName"].ToString(), dt.Rows[0]["TableId"].ToString(), passport);
                        string description = employee.Description;
                        // Dim dateRequested = DateTime.Now.ToShortDateString & " " & DateTime.Now.ToShortTimeString
                        int firstlength = 70 - item.Length - description.Length;
                        if (firstlength < 0)
                            firstlength = 1;
                        message = message + item + new string(' ', firstlength) + description;
                        if (!string.IsNullOrEmpty(comment))
                            message += Environment.NewLine + "Reason: " + comment;

                        Navigation.SendEmail(message, Navigation.GetEmployeeEmailByID(employee.ID, passport), Navigation.GetUserEmail(passport), "Request Exception", "", conn);
                    }
                }
            }
        }

        public static void ClearExceptionRequest(int requestId, Passport passport)
        {
            string sql = "UPDATE [SLRequestor] SET [Status] = 'New', [ExceptionComments] = NULL WHERE [Id] = @requestID AND [Status] = 'Exception'";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@requestID", requestId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ProcessRequest(int viewID, string tableID, string employeeID, string status, Passport passport)
        {
            string tableName = Navigation.GetViewTableName(viewID, passport);
            tableID = Navigation.PrepPad(tableName, tableID, passport);
            employeeID = Navigation.PrepPad(Tracking.GetRequestorTableName(passport), employeeID, passport);

            using (var rqAdapter = new RecordsManageTableAdapters.SLRequestorTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    rqAdapter.Connection = conn;
                    rqAdapter.ProcessRequest(DateTime.Today, status, tableName, employeeID, tableID);
                }
            }
        }

        public static void CheckRequestable(ref System.Text.StringBuilder message, List<string> ids, string tableName, string EmployeeID, string Instructions, string Priority, DateTime DateNeeded, bool WaitList, Passport Passport, DataRow tableInfo)
        {
            var requests = new Requesting();
            foreach (string tableId in ids)
            {
                string item = Navigation.GetItemName(tableName, tableId, Passport, tableInfo);
                if (item.ToLower().Contains("display fields are not configured."))
                {
                    item = Strings.Replace(item, "display fields are not configured.", string.Empty, Compare: CompareMethod.Text).Trim();
                }
                else if (item.Contains(" "))
                {
                    item = item.Substring(0, item.IndexOf(" ")).Trim();
                }

                if (!get_IsRequestable(tableName, tableId, Passport))
                {
                    // Modified By Hemin
                    // message.Append(String.Format("""{0}"" is not in a requestable location.<br />", item))
                    message.Append(string.Format("'{0}' is not found or not trackable <br />", item));
                }
                else
                {
                    var user = new User(Passport, true);
                    if (!Tracking.AlreadyRequestedByCurrentEmployee(tableName, tableId, EmployeeID, Passport))
                    {
                        if (!Tracking.AlreadyAtAnEmployee(tableName, tableId, Passport))
                        {
                            var reqList = GetActiveRequests(tableName, tableId, Passport);

                            if (reqList.Count != 0)
                            {
                                if (!WaitList)
                                {
                                    // Modified By Hemin
                                    // message.Append(String.Format("""{0}"" is already requested.<br />", item))
                                    message.Append(string.Format("'{0}' is already requested<<br />", item));
                                }
                            }
                        }
                        else if (!WaitList)
                        {
                            // Modified By Hemin
                            // message.Append(String.Format("""{0}"" is already checked out.<br />", item))
                            message.Append(string.Format("'{0}' is already checked out <br/>", item));
                        }
                    }
                    else
                    {
                        // Modified By Hemin
                        // message.Append(String.Format("""{0}"" is already requested by that Employee<br />", item))
                        message.Append(string.Format("'{0}' is already requested by that Employee<br />", item));
                    }
                }
            }
        }

        public static void MakeRequest(ref System.Text.StringBuilder message, List<string> ids, string tableName, string EmployeeID, string Instructions, string Priority, DateTime DateNeeded, bool WaitList, Passport Passport, DataRow tableInfo, int PullListID = 0)
        {
            var requests = new Requesting();

            foreach (string tableId in ids)
            {
                string item = Navigation.GetItemName(tableName, tableId, Passport, tableInfo);

                if (item.ToLower().Contains("display fields are not configured."))
                {
                    item = Strings.Replace(item, "display fields are not configured.", string.Empty, Compare: CompareMethod.Text).Trim();
                }
                else if (item.Contains(" "))
                {
                    item = item.Substring(0, item.IndexOf(" ")).Trim();
                }
                if (!get_IsRequestable(tableName, tableId, Passport))
                {
                    // Modified By Hemin
                    // message.Append(String.Format("""{0}"" is not in a requestable location.<br />", item))
                    message.Append(string.Format("One or more of the selected objects may not be requested at this time <br />", item));
                }
                else
                {
                    var user = new User(Passport, true);
                    var req = new Request(tableName, Passport);

                    req.TableID = tableId;
                    req.EmployeeID = EmployeeID;
                    req.Instructions = Instructions;
                    req.Priority = Priority;
                    req.Status = string.Empty;
                    if (DateNeeded == default)
                    {
                        req.DateNeeded = DateAndTime.DateAdd(DateInterval.Day, 1d, DateTime.Today);
                    }
                    else
                    {
                        req.DateNeeded = DateNeeded;
                    }

                    req.RequestedBy = user.UserName;
                    req.PullListID = PullListID;

                    if (!Tracking.AlreadyRequestedByCurrentEmployee(req.TableName, req.TableID, req.EmployeeID, Passport))
                    {
                        if (!Tracking.AlreadyAtAnEmployee(req.TableName, req.TableID, Passport))
                        {
                            var reqList = GetActiveRequests(tableName, tableId, Passport);
                            if (reqList.Count == 0)
                            {
                                req.Status = "New";
                            }
                            else if (WaitList)
                            {
                                req.Status = "WaitList";
                            }
                            else
                            {
                                req.Status = "Deleted";
                                // Modified By Hemin
                                // message.Append(String.Format("""{0}"" is already requested.", item))
                                message.Append(string.Format("'{0}' is already requested", item));
                            }
                        }
                        else if (WaitList)
                        {
                            req.Status = "WaitList";
                        }
                        else
                        {
                            req.Status = "Deleted";
                            // Modified By Hemin
                            // message.Append(String.Format("""{0}"" is already checked out.", item))
                            message.Append(string.Format("'{0}' is already checked out", item));
                        }
                        // Modified By Hemin
                        // If .Status.Length = 0 Then Throw New Exception("No Status")
                        if (req.Status.Length == 0)
                            throw new Exception("No Status");
                        requests.InsertRequest(req, Passport);
                    }
                    else
                    {
                        message.Append(string.Format("'{0}' is already requested by that Employee", item));
                    }
                    // If CBoolean(GetSystemSetting("AllowWaitList", Passport)) And chkWaitList.Checked Then
                    // Else
                    // Dim requesting As New Requesting
                    // If requesting.GetActiveRequests(LevelManger.ActiveLevel.Parameters.ViewID, req.TableID, Passport).Count > 0 Then
                    // message = "There already is an open request for item " & GetItemName(req.TableName, req.TableID, Passport) & ". This item cannot be requested at this time"
                    // ClientScript.RegisterClientScriptBlock(Me.GetType, "error", "<script>alert('" & message & "');</script>")
                    // End If
                    // End If
                }
            }
        }

        public static int MakePullList(string Comment, bool BatchPull, int PriorityOrder, Passport Passport)
        {
            string sql = "INSERT INTO SLPullLists ([OperatorsId], [DateCreated], [SLBatchRequestComment], [BatchPullList], [BatchPrinted], [PriorityOrder]) VALUES " + "(@OperatorsId, @DateCreated, @SLBatchRequestComment, @BatchPullList, @BatchPrinted, @PriorityOrder) SELECT SCOPE_IDENTITY() ";
            int id = 0;
            var user = new User(Passport, true);

            using (var conn = Passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@OperatorsId", user.UserName);
                    cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    cmd.Parameters.AddWithValue("@SLBatchRequestComment", Comment);
                    cmd.Parameters.AddWithValue("@BatchPullList", BatchPull);
                    cmd.Parameters.AddWithValue("@BatchPrinted", false);
                    cmd.Parameters.AddWithValue("@PriorityOrder", PriorityOrder);
                    id = Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }

            return id;
        }

        public static bool get_IsRequestable(string tableName, string tableID, Passport passport)
        {
            if (passport.CheckSetting(tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Request))
            {
                using (var conn = passport.Connection())
                {
                    Dictionary<string, string> argidsByTable = null;
                    var trackingList = Tracking.GetTrackableStatus(tableName, tableID, passport, conn, idsByTable: ref argidsByTable);

                    if (trackingList is not null && trackingList.Count > 0)
                    {
                        var firstContainer = trackingList[0].Containers[0];
                        var firstTable = Navigation.GetTableInfo(firstContainer.Type, conn);
                        if (Conversions.ToInteger(firstTable["TrackingTable"]) != 1)
                            return true;

                        try
                        {
                            return Navigation.CBoolean(Navigation.GetSingleFieldValue(firstContainer.Type, firstContainer.ID, firstTable["TrackingRequestableFieldName"].ToString(), conn)[0]);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}