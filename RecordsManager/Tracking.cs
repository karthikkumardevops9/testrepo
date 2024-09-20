using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Smead.Security;

namespace MSRecordsEngine.RecordsManager
{

    public class Container
    {
        private int _level;
        private bool _IdFieldIsString;

        public Container(string tableName, Passport passport)
        {
            _type = tableName;
            _IdFieldIsString = Navigation.FieldIsAString(_type, passport);
        }

        public Container(string tableName, bool isString)
        {
            _type = tableName;
            _IdFieldIsString = isString;
        }

        public Container(DataRow tableInfo, Passport passport)
        {
            _type = tableInfo["TableName"].ToString();
            _IdFieldIsString = Navigation.FieldIsAString(tableInfo, passport);
        }

        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
        }

        private string _ID;
        public string ID
        {
            get
            {
                if (_IdFieldIsString)
                    return _ID;
                return Navigation.PrepPad(_ID);
            }
            set
            {
                _ID = value;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private int _outType;
        public int OutType
        {
            get
            {
                return _outType;
            }
            set
            {
                _outType = value;
            }
        }

        private string _mailStop; // For Employees
        public string MailStop
        {
            get
            {
                return _mailStop;
            }
            set
            {
                _mailStop = value;
            }
        }

        private string _phone; // For Employees
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
            }
        }
    }

    public class TrackingTransaction
    {
        public enum OutTypes
        {
            UseOutField = 0,
            AlwaysOut = 1,
            AlwaysIn = 2
        }

        private bool _IdFieldIsString;

        public TrackingTransaction(string tableName, bool isString)
        {
            _type = tableName;
            _IdFieldIsString = isString;
        }

        public TrackingTransaction(string tableName, Passport passport)
        {
            _type = tableName;
            _IdFieldIsString = Navigation.FieldIsAString(_type, passport);
        }

        public TrackingTransaction(DataRow tableInfo, Passport passport)
        {
            _type = tableInfo["TableName"].ToString();
            _IdFieldIsString = Navigation.FieldIsAString(tableInfo, passport);
        }

        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
        }

        private string _ID;
        public string ID
        {
            get
            {
                if (_IdFieldIsString)
                    return _ID;
                return Navigation.PrepPad(_ID);
            }
            set
            {
                _ID = value;
            }
        }

        private List<Container> _Containers = new List<Container>();
        public List<Container> Containers
        {
            get
            {
                return _Containers;
            }
            set
            {
                _Containers = value;
            }
        }

        private DateTime _transactionDate;
        public DateTime TransactionDate
        {
            get
            {
                return _transactionDate;
            }
            set
            {
                _transactionDate = value;
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        private bool _out;
        public bool Out
        {
            get
            {
                return _out;
            }
            set
            {
                _out = value;
            }
        }

        private DateTime _dueBack;
        public DateTime DateDue
        {
            get
            {
                return _dueBack;
            }
            set
            {
                _dueBack = value;
            }
        }

        private bool _actualScan;
        public bool ActualScan
        {
            get
            {
                return _actualScan;
            }
            set
            {
                _actualScan = value;
            }
        }

        private string containerTypeToFind;
        private bool _findContainer(Container container)
        {
            return (container.Type ?? "") == (containerTypeToFind ?? "");
        }

        public Container FindContainer(string containerType)
        {
            containerTypeToFind = containerType;
            return _Containers.Find(_findContainer);
        }

        private string _trackingAdditionalField1;
        public string TrackingAdditionalField1
        {
            get
            {
                return _trackingAdditionalField1;
            }
            set
            {
                _trackingAdditionalField1 = value;
            }
        }

        private string _trackingAdditionalField2;
        public string TrackingAdditionalField2
        {
            get
            {
                return _trackingAdditionalField2;
            }
            set
            {
                _trackingAdditionalField2 = value;
            }
        }

    }

    public class Tracking
    {
        public static string ShowUserName(string userName, Passport passport)
        {
            if (Tracking.IsMember(Navigation.GetSetting("Security", "ShowUserName", passport), passport))
                return userName;
            return "'&lt;unknown user&gt;'";
        }

        private static bool IsMember(string groupList, Passport passport)
        {
            if (string.IsNullOrEmpty(groupList.Trim()))
                return false;
            if (groupList.Contains("-1"))
                return true;

            var user = new User(passport, true);
            var groups = user.GroupMembershipList;
            groupList = string.Format(",{0},", groupList);

            while (groupList.Contains(" "))
                groupList = groupList.Replace(" ", string.Empty);

            foreach (Groups.GroupItem grp in groups)
            {
                if (groupList.Contains(string.Format(",{0},", grp.GroupID.ToString())))
                    return true;
            }

            return false;
        }
        // moti mash step 1
        public static List<TrackingTransaction> GetTrackableHistory(string trackableType, string trackableID,
            bool statusOnly, Passport passport,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable)
        {
            trackableID = Navigation.PrepPad(trackableType, trackableID, passport);
            if (trackableType.Length == 0 | trackableID.Length == 0)
                return null;


            string sql = "SELECT TrackingHistory.*, ISNULL(SecureUser.DisplayName, " + ShowUserName("TrackingHistory.UserName", passport) + ") AS TrackingDisplayName " + "FROM TrackingHistory LEFT JOIN SecureUser ON TrackingHistory.UserName = SecureUser.UserName " + "WHERE TrackedTable = @TrackedTable AND TrackedTableId = @TrackedTableId ORDER BY TransactionDateTime DESC";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", trackableType);
                    cmd.Parameters.AddWithValue("@TrackedTableId", trackableID);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        Dictionary<string, DataTable> argdescriptions = null;
                        return BuildTracking(dtTracking, passport, trackingTables, trackedTables, ref idsByTable, descriptions: ref argdescriptions);
                    }
                }
            }
        }

        public static List<TrackingTransaction> GetPagedTrackableHistory(string trackableType,
            string trackableID, bool statusOnly,
            Passport passport, Parameters @params,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable)
        {
            trackableID = Navigation.PrepPad(trackableType, trackableID, passport);
            if (trackableType.Length == 0 | trackableID.Length == 0)
                return null;

            string sql = "SELECT TrackingHistory.*, ISNULL(SecureUser.DisplayName, " + ShowUserName("TrackingHistory.UserName", passport) + ") AS TrackingDisplayName " + "FROM TrackingHistory LEFT JOIN SecureUser ON TrackingHistory.UserName = SecureUser.UserName " + "WHERE TrackedTable = @TrackedTable AND TrackedTableId = @TrackedTableId";
            string orderClause = " ORDER BY TransactionDateTime DESC";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", trackableType);
                    cmd.Parameters.AddWithValue("@TrackedTableId", trackableID);

                    if (@params.Paged)
                    {
                        @params.TotalRows = Query.TotalQueryRowCount(cmd);
                        cmd.CommandText = Query.Pagify(@params, sql, orderClause);
                    }
                    else
                    {
                        cmd.CommandText = sql + orderClause;
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        Dictionary<string, DataTable> argdescriptions = null;
                        return BuildTracking(dtTracking, passport, trackingTables, trackedTables, ref idsByTable, descriptions: ref argdescriptions);
                    }
                }
            }
        }

        public static List<TrackingTransaction> GetTrackableStatus(string trackableType, string trackableId,
            Passport passport, SqlConnection conn,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable)
        {
            trackableId = Navigation.PrepPad(trackableType, trackableId, passport);
            if (trackableType.Length == 0 | trackableId.Length == 0)
                return null;

            string sql = "SELECT TrackingStatus.*, ISNULL(SecureUser.DisplayName, " + ShowUserName("TrackingStatus.UserName", passport) + ") AS TrackingDisplayName " + "FROM TrackingStatus LEFT JOIN SecureUser ON TrackingStatus.UserName = SecureUser.UserName " + "WHERE TrackedTable = @TrackedTable AND TrackedTableId = @TrackedTableId";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrackedTable", trackableType);
                cmd.Parameters.AddWithValue("@TrackedTableId", trackableId);
                cmd.Parameters[0].SqlDbType = SqlDbType.VarChar;
                cmd.Parameters[1].SqlDbType = SqlDbType.VarChar;
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dtTracking = new DataTable();
                    da.Fill(dtTracking);
                    Dictionary<string, DataTable> argdescriptions = null;
                    return BuildTracking(dtTracking, passport, conn, trackingTables, trackedTables, ref idsByTable, descriptions: ref argdescriptions);
                }
            }
        }

        public static List<TrackingTransaction> GetTrackableStatus(string trackableType,
            string trackableId, Passport passport,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackableStatus(trackableType, trackableId, passport, conn, trackingTables, trackedTables, ref idsByTable);
            }
        }

        public static List<TrackingTransaction> GetPastDueTrackables(DateTime fromDate,
            Passport passport, SqlConnection conn,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, DataTable> descriptions)
        {
            var trackingData = GetTrackingStatusFields(conn);
            string trackingStatusFields = "";

            foreach (DataRow trackingRow in trackingData.Rows)
                trackingStatusFields += trackingRow["TrackingStatusFieldName"].ToString() + ", ";

            string sql = "SELECT " + trackingStatusFields + "DateDue, Out, ProcessedDateTime, TrackedTable, TrackedTableId, TrackingAdditionalField1, " + "TrackingAdditionalField2, TransactionDateTime, ISNULL(SecureUser.DisplayName, " + ShowUserName("TrackingStatus.UserName", passport) + ") AS TrackingDisplayName " + "FROM TrackingStatus LEFT JOIN SecureUser ON TrackingStatus.UserName = SecureUser.UserName " + "WHERE (Out <> 0) AND (DateDue < @FromDate) ORDER BY DateDue";

            using (var cmd = new SqlCommand(sql, passport.Connection()))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dtTracking = new DataTable();
                    da.Fill(dtTracking);
                    return BuildTracking(dtTracking, passport, conn, trackingTables, trackedTables, ref idsByTable, ref descriptions);
                }
            }
        }

        public static List<TrackingTransaction> GetPagedPastDueTrackables(DateTime fromDate,
            Passport passport, Parameters @params,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, DataTable> descriptions)
        {
            using (var conn = passport.Connection())
            {
                // Dim trackingData = GetTrackingStatusFields(conn)
                // Dim trackingStatusFields = ""

                // For Each trackingRow As DataRow In trackingData.Rows
                // trackingStatusFields &= trackingRow("TrackingStatusFieldName").ToString & ", "
                // Next

                // Dim sql As String = "SELECT " & trackingStatusFields & "DateDue, Out, ProcessedDateTime, TrackedTable, TrackedTableId, TrackingAdditionalField1, " &
                // "TrackingAdditionalField2, TransactionDateTime, ISNULL(SecureUser.DisplayName, " & ShowUserName("TrackingStatus.UserName", passport) & ") AS TrackingDisplayName " &
                // "FROM TrackingStatus LEFT JOIN SecureUser ON TrackingStatus.UserName = SecureUser.UserName " &
                // "WHERE (Out <> 0) AND (DateDue < @FromDate)"

                string sql = PagedPastDueTrackablesQuery(passport, false);

                string orderClause = " ORDER BY DateDue";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);

                    if (@params.Paged)
                    {
                        @params.TotalRows = Query.TotalQueryRowCount(cmd);
                        cmd.CommandText = Query.Pagify(@params, sql, orderClause);
                    }
                    else
                    {
                        cmd.CommandText = sql + orderClause;
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        return BuildTracking(dtTracking, passport, conn, trackingTables, trackedTables, ref idsByTable, ref descriptions);
                    }
                }
            }
        }

        public static List<TrackingTransaction> GetPagedPastDueTrackablesList(DateTime fromDate,
            Passport passport, Parameters @params,
            int PerPageRecord,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, DataTable> descriptions)
        {
            using (var conn = passport.Connection())
            {

                string sql = PagedPastDueTrackablesQuery(passport, false);

                string orderClause = " ORDER BY DateDue";
                sql += orderClause;

                // NOTE: query will through exception if does not include ORDER BY
                sql += Query.QueryPaging(@params.PageIndex, PerPageRecord);

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        return BuildTracking(dtTracking, passport, conn, trackingTables, trackedTables, ref idsByTable, ref descriptions);
                    }
                }
            }
        }

        public static DataTable GetPagedPastDueTrackablesCount(DateTime fromDate, Passport passport,
            Parameters @params,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, DataTable> descriptions)
        {
            using (var conn = passport.Connection())
            {

                string sql = PagedPastDueTrackablesQuery(passport, true);
                var dtTracking = new DataTable();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtTracking);
                    }
                }
                return dtTracking;
            }
        }

        public static string PagedPastDueTrackablesQuery(Passport passport, bool count)
        {
            using (var conn = passport.Connection())
            {

                if (count)
                {
                    string sql = "SELECT COUNT(*) FROM TrackingStatus LEFT JOIN SecureUser ON TrackingStatus.UserName = SecureUser.UserName " + "WHERE (Out <> 0) AND (DateDue < @FromDate)";

                    return sql;
                }

                else
                {
                    var trackingData = GetTrackingStatusFields(conn);
                    string trackingStatusFields = "";

                    foreach (DataRow trackingRow in trackingData.Rows)
                        trackingStatusFields += trackingRow["TrackingStatusFieldName"].ToString() + ", ";

                    string sql = "SELECT " + trackingStatusFields + "DateDue, Out, ProcessedDateTime, TrackedTable, TrackedTableId, TrackingAdditionalField1, " + "TrackingAdditionalField2, TransactionDateTime, ISNULL(SecureUser.DisplayName, " + ShowUserName("TrackingStatus.UserName", passport) + ") AS TrackingDisplayName " + "FROM TrackingStatus LEFT JOIN SecureUser ON TrackingStatus.UserName = SecureUser.UserName " + "WHERE (Out <> 0) AND (DateDue < @FromDate)";
                    return sql;
                }

            }
        }

        public static List<TrackingTransaction> GetPastDueTrackables(DateTime fromDate,
            Passport passport,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, DataTable> descriptions)
        {
            using (var conn = passport.Connection())
            {
                return GetPastDueTrackables(fromDate, passport, conn, trackingTables, trackedTables, ref idsByTable, ref descriptions);
            }
        }

        public static List<TrackingTransaction> GetCurrentItemsOutReport(Parameters @params,
            Passport passport,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable)
        {

            var sb = new System.Text.StringBuilder();
            var row = GetRequestorTableInfo(passport);
            string tableName = row["TableName"].ToString();
            string idFieldName = Navigation.MakeSimpleField(row["IdFieldName"].ToString());
            var dataType = Navigation.GetFieldType(tableName, idFieldName, passport);

            sb = Tracking.CurrentItemsOutReportQuery(false, passport, @params, dataType, tableName, idFieldName, row);

            string orderClause = string.Format(" ORDER BY [{0}].[{1}]", tableName, idFieldName);

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sb.ToString(), conn))
                {
                    if (@params.Paged)
                    {
                        @params.TotalRows = Query.TotalQueryRowCount(cmd);
                        cmd.CommandText = Query.Pagify(@params, sb.ToString(), orderClause);
                    }
                    else
                    {
                        cmd.CommandText = sb.ToString() + orderClause;
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        Dictionary<string, DataTable> argdescriptions = null;
                        return BuildTracking(dtTracking, passport, conn, trackingTables, trackedTables, ref idsByTable, descriptions: ref argdescriptions);
                    }
                }
            }
        }



        public static List<TrackingTransaction> GetCurrentItemsOutReportList(Parameters @params,
            Passport passport, int perPageRecord,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable)
        {

            var sb = new System.Text.StringBuilder();
            var row = GetRequestorTableInfo(passport);
            string tableName = row["TableName"].ToString();
            string idFieldName = Navigation.MakeSimpleField(row["IdFieldName"].ToString());
            var dataType = Navigation.GetFieldType(tableName, idFieldName, passport);

            sb = Tracking.CurrentItemsOutReportQuery(false, passport, @params, dataType, tableName, idFieldName, row);

            string orderClause = string.Format(" ORDER BY [{0}].[{1}]", tableName, idFieldName);
            sb.Append(orderClause);

            // NOTE: query will through exception if does not include ORDER BY
            sb.Append(Query.QueryPaging(@params.PageIndex, perPageRecord).ToString());

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sb.ToString(), conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        Dictionary<string, DataTable> argdescriptions = null;
                        return BuildTracking(dtTracking, passport, conn, trackingTables, trackedTables, ref idsByTable, descriptions: ref argdescriptions);
                    }
                }
            }
        }

        public async static Task<List<TrackingTransaction>> GetCurrentItemsOutReportListAsync(Parameters @params,
    Passport passport, int perPageRecord,
    [Optional, DefaultParameterValue(null)] DataTable trackingTables,
    [Optional, DefaultParameterValue(null)] DataTable trackedTables,
    [Optional, DefaultParameterValue(null)] Dictionary<string, string> idsByTable)
        {

            var sb = new System.Text.StringBuilder();
            var row = GetRequestorTableInfo(passport);
            string tableName = row["TableName"].ToString();
            string idFieldName = Navigation.MakeSimpleField(row["IdFieldName"].ToString());
            var dataType = Navigation.GetFieldType(tableName, idFieldName, passport);

            sb = Tracking.CurrentItemsOutReportQuery(false, passport, @params, dataType, tableName, idFieldName, row);

            string orderClause = string.Format(" ORDER BY [{0}].[{1}]", tableName, idFieldName);
            sb.Append(orderClause);

            // NOTE: query will through exception if does not include ORDER BY
            sb.Append(Query.QueryPaging(@params.PageIndex, perPageRecord).ToString());

            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sb.ToString(), conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        Dictionary<string, DataTable> argdescriptions = null;
                        return BuildTracking(dtTracking, passport, conn, trackingTables, trackedTables, ref idsByTable, descriptions: ref argdescriptions);
                    }
                }
            }
        }

        public static DataTable GetCurrentItemsOutReportCount(Parameters @params,
            Passport passport,
            [Optional, DefaultParameterValue(null)] DataTable trackingTables,
            [Optional, DefaultParameterValue(null)] DataTable trackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable)
        {
            var sb = new System.Text.StringBuilder();
            var row = GetRequestorTableInfo(passport);
            string tableName = row["TableName"].ToString();
            string idFieldName = Navigation.MakeSimpleField(row["IdFieldName"].ToString());
            var dataType = Navigation.GetFieldType(tableName, idFieldName, passport);

            sb = Tracking.CurrentItemsOutReportQuery(true, passport, @params, dataType, tableName, idFieldName, row);

            var dtTracking = new DataTable();
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sb.ToString(), conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtTracking);
                    }
                }
            }

            return dtTracking;
        }

        private static System.Text.StringBuilder CurrentItemsOutReportQuery(bool count, Passport passport,
            Parameters @params, Type dataType, string tableName, string idFieldName, DataRow row)
        {
            string idForJoin;
            var sb = new System.Text.StringBuilder();
            if (count)
            {
                if (Navigation.IsAStringType(dataType) || Navigation.IsADateType(dataType))
                {
                    idForJoin = string.Format("[{0}].[{1}]", tableName, idFieldName);
                }
                else
                {
                    idForJoin = string.Format("RIGHT('000000000000000000000000000000' + CAST([{0}].[{1}] AS VARCHAR(30)), 30)", tableName, idFieldName);
                }

                sb.Append(string.Format("SELECT COUNT(*) "));
                sb.Append(string.Format(" FROM [{0}] INNER JOIN [TrackingStatus] ON [TrackingStatus].[{1}] = {2}", tableName, row["TrackingStatusFieldName"].ToString(), idForJoin));
                if (!string.IsNullOrEmpty(row["TrackingActiveFieldName"].ToString()))
                    sb.Append(string.Format(" WHERE ([{0}].[{1}] <> 0 AND [{0}].[{1}] IS NOT NULL)", tableName, row["TrackingActiveFieldName"].ToString()));
            }

            else
            {
                if (Navigation.IsAStringType(dataType) || Navigation.IsADateType(dataType))
                {
                    idForJoin = string.Format("[{0}].[{1}]", tableName, idFieldName);
                }
                else
                {
                    idForJoin = string.Format("RIGHT('000000000000000000000000000000' + CAST([{0}].[{1}] AS VARCHAR(30)), 30)", tableName, idFieldName);
                }

                sb.Append(string.Format("SELECT [TrackingStatus].[Id] AS TrackingStatusId, [TrackingStatus].[UserName] AS TrackingStatusUserName, [{0}].[{1}]", tableName, idFieldName));
                sb.Append(", [TrackingStatus].TrackedTable, [TrackingStatus].TransactionDateTime, [TrackingStatus].TrackedTableID, [TrackingStatus].[TrackingAdditionalField1], [TrackingStatus].[TrackingAdditionalField2]");
                sb.Append(string.Format(", [TrackingStatus].[{0}]", Navigation.MakeSimpleField(row["TrackingStatusFieldName"].ToString())));

                if (!string.IsNullOrEmpty(row["TrackingPhoneFieldName"].ToString()))
                    sb.Append(string.Format(", [{0}].[{1}]", tableName, row["TrackingPhoneFieldName"].ToString()));
                if (!string.IsNullOrEmpty(row["TrackingMailStopFieldName"].ToString()))
                    sb.Append(string.Format(", [{0}].[{1}]", tableName, row["TrackingMailStopFieldName"].ToString()));
                if (!string.IsNullOrEmpty(row["OperatorsIdField"].ToString()))
                    sb.Append(string.Format(", [{0}].[{1}]", tableName, row["OperatorsIdField"].ToString()));

                sb.Append(string.Format(", {0} AS TrackingDisplayName ", ShowUserName("TrackingStatus.UserName", passport)));
                sb.Append(string.Format(" FROM [{0}] INNER JOIN [TrackingStatus] ON [TrackingStatus].[{1}] = {2}", tableName, row["TrackingStatusFieldName"].ToString(), idForJoin));
                if (!string.IsNullOrEmpty(row["TrackingActiveFieldName"].ToString()))
                    sb.Append(string.Format(" WHERE ([{0}].[{1}] <> 0 AND [{0}].[{1}] IS NOT NULL)", tableName, row["TrackingActiveFieldName"].ToString()));

            }

            return sb;

        }



        public static bool AlreadyAtEmployee(string trackedTableName, string trackedTableId, string requestorId, SqlConnection conn)
        {
            var requestorRow = GetRequestorTableInfo(conn);
            string sql = string.Format("SELECT COUNT(*) FROM TrackingStatus WHERE TrackedTable = @TrackedTable AND TrackedTableID = @TrackedTableId AND {0} = @RequestorId", Navigation.MakeSimpleField(requestorRow["TrackingStatusFieldName"].ToString()));

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrackedTable", trackedTableName);
                cmd.Parameters.AddWithValue("@TrackedTableId", trackedTableId.PadLeft(30, '0'));
                cmd.Parameters.AddWithValue("@RequestorId", requestorId.PadLeft(30, '0'));
                return Conversions.ToBoolean(cmd.ExecuteScalar());
            }
        }

        public static bool AlreadyAtEmployee(string trackedTableName, string trackedTableId, string requestorId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return AlreadyAtEmployee(trackedTableName, trackedTableId, requestorId, conn);
            }
        }

        public static DataTable TrackingStatusByTableAndId(string trackedTableName, string trackedTableId, SqlConnection conn)
        {
            var requestorRow = GetRequestorTableInfo(conn);
            string sql = string.Format("SELECT {0} FROM TrackingStatus WHERE TrackedTable = @TrackedTable AND TrackedTableID = @TrackedTableId AND (NOT {0} IS NULL)", Navigation.MakeSimpleField(requestorRow["TrackingStatusFieldName"].ToString()));

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrackedTable", trackedTableName);
                cmd.Parameters.AddWithValue("@TrackedTableId", trackedTableId.PadLeft(30, '0'));

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable TrackingStatusByTableAndId(string trackedTableName, string trackedTableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return TrackingStatusByTableAndId(trackedTableName, trackedTableId, conn);
            }
        }

        public static DataTable GetTrackingStatus(string trackedTableName, string trackedTableId, string locationFieldName, Passport passport)
        {
            string sql = "SELECT  [TrackedTableId], [TrackedTable], [TransactionDateTime], [" + locationFieldName + "]  FROM TrackingStatus WHERE TrackedTable = @TrackedTable AND TrackedTableID = @TrackedTableId AND (NOT TransactionDateTime IS NULL) ORDER BY TransactionDateTime DESC";
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", trackedTableName);
                    if (Navigation.FieldIsAString(trackedTableName, passport))
                    {
                        cmd.Parameters.AddWithValue("@TrackedTableId", trackedTableId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@TrackedTableId", trackedTableId.PadLeft(30, '0'));
                    }
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;

                    }
                }
            }
        }

        public static bool AlreadyAtAnEmployee(string trackedTableName, string trackedTableId, SqlConnection conn)
        {
            var requestorRow = GetRequestorTableInfo(conn);
            string sql = string.Format("SELECT COUNT(*) FROM TrackingStatus WHERE TrackedTable = @TrackedTable AND TrackedTableID = @TrackedTableId AND ({0} IS NOT NULL AND {0} > '')", Navigation.MakeSimpleField(requestorRow["TrackingStatusFieldName"].ToString()));

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TrackedTable", trackedTableName);
                if (Navigation.FieldIsAString(trackedTableName, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTableId", trackedTableId);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@TrackedTableId", trackedTableId.PadLeft(30, '0'));
                }
                return Conversions.ToBoolean(cmd.ExecuteScalar());
            }
        }

        public static bool AlreadyAtAnEmployee(string trackedTableName, string trackedTableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return AlreadyAtAnEmployee(trackedTableName, trackedTableId, conn);
            }
        }

        public static bool AlreadyRequestedByCurrentEmployee(string requestedTableName, string requestedTableId, string EmployeeId, SqlConnection conn)
        {
            string sql = "SELECT COUNT(*) FROM SLRequestor WHERE TableName = @RequestedTable AND TableId= @RequestedTableID AND EmployeeId = @EmployeeId AND Status <> 'Deleted' AND Status <> 'Fulfilled' AND Status <> ''";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@RequestedTable", requestedTableName);
                if (Navigation.FieldIsAString(requestedTableName, conn))
                {
                    cmd.Parameters.AddWithValue("@RequestedTableId", requestedTableId);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@RequestedTableId", requestedTableId.PadLeft(30, '0'));
                }
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId.PadLeft(30, '0'));
                return Conversions.ToBoolean(cmd.ExecuteScalar());
            }
        }

        public static bool AlreadyRequestedByCurrentEmployee(string requestedTableName, string requestedTableId,
            string EmployeeId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return AlreadyRequestedByCurrentEmployee(requestedTableName, requestedTableId, EmployeeId, conn);
            }
        }

        public static string ConfirmRecord(TableItem tableItem, string activeFieldName, bool containsPrefix, SqlConnection conn)
        {
            if (tableItem is null)
                return string.Empty;
            return Tracking.ConfirmRecord(tableItem.TableName, tableItem.KeyFieldName, activeFieldName, tableItem.ID, containsPrefix, conn);
        }

        public static string ConfirmRecord(TableItem tableItem, string activeFieldName, bool containsPrefix, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return ConfirmRecord(tableItem, activeFieldName, containsPrefix, conn);
            }
        }

        public static string ConfirmRecord(string tableName, string keyFieldName, string activeFieldName, string value, bool containsPrefix, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(keyFieldName) || string.IsNullOrEmpty(value))
                return string.Empty;

            var dt = new DataTable();
            var tableInfo = Navigation.GetTableInfo(tableName, conn);
            keyFieldName = Navigation.MakeSimpleField(keyFieldName);
            string sql = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}] = @Id", Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString()), tableName, keyFieldName);

            if (!string.IsNullOrEmpty(activeFieldName))
            {
                activeFieldName = Navigation.MakeSimpleField(activeFieldName);
                sql = string.Format("SELECT [{0}], [{1}] FROM [{2}] WHERE [{3}] = @Id", Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString()), activeFieldName, tableName, keyFieldName);
            }

            using (var cmd = new SqlCommand(sql, conn))
            {
                if (containsPrefix && value.Length >= tableInfo["BarCodePrefix"].ToString().Length && string.Compare(tableInfo["BarCodePrefix"].ToString(), value.Substring(0, tableInfo["BarcodePrefix"].ToString().Length), true) == 0)
                {
                    cmd.Parameters.AddWithValue("@Id", value.Substring(tableInfo["BarcodePrefix"].ToString().Length));
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Id", value);
                }

                try
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }

                if (dt.Rows.Count == 0)
                    return string.Empty;
                if (!string.IsNullOrEmpty(activeFieldName) && !Navigation.CBoolean(dt.Rows[0][1]))
                    throw new Exception("Not Active");
                return dt.Rows[0][0].ToString();
            }
        }

        public static string ConfirmRecord(string tableName, string keyFieldName, string activeFieldName, string value, bool containsPrefix, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return ConfirmRecord(tableName, keyFieldName, activeFieldName, value, containsPrefix, conn);
            }
        }

        public static TableItem TranslateBarcode(string barcodeData, bool isDestination, SqlConnection conn, string tableName = "")
        {
            if (string.IsNullOrEmpty(barcodeData))
                return null;
            bool MoreThanOnePrefix = false;
            // Dim sql As String = "SELECT TableName, IdFieldName, BarCodePrefix, TrackingTable, TrackingActiveFieldName FROM Tables WHERE BarCodePrefix IS NOT NULL"
            string sql = "SELECT T.TableName, T.IdFieldName, T.BarCodePrefix, T.TrackingTable, T.TrackingActiveFieldName,T.Trackable,SL.ScanOrder, SL.FieldName " + Constants.vbCrLf + " FROM Tables T INNER JOIN Scanlist SL ON UPPER(T.TableName)=UPPER(SL.TableName) " + Constants.vbCrLf + " WHERE BarCodePrefix IS NOT NULL Order By SL.ScanOrder";
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    cmd.CommandText += " AND TableName=@TableName";
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                }
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (barcodeData.Length >= row["BarCodePrefix"].ToString().Length && string.Compare(row["BarCodePrefix"].ToString(), barcodeData.Substring(0, row["BarcodePrefix"].ToString().Length), true) == 0)
                        {
                            string id = string.Empty;
                            try
                            {
                                id = Tracking.ConfirmRecord(row["TableName"].ToString(), Navigation.MakeSimpleField(row["IdFieldName"].ToString()), row["TrackingActiveFieldName"].ToString(), barcodeData.Substring(row["BarcodePrefix"].ToString().Length), false, conn);
                            }
                            catch (Exception ex)
                            {
                                // Modified by Hemin
                                if (string.Compare(ex.Message, "Not Active") == 0)
                                    throw new Exception(string.Format("{0}", Interaction.IIf(isDestination, "Destination Not Active", "Object Not Active")));
                                throw;
                            }

                            if (!string.IsNullOrEmpty(id))
                            {
                                if (isDestination == true)
                                {
                                    if (Conversions.ToInteger(row["TrackingTable"]) > 0)
                                    {
                                        var ti = new TableItem();
                                        ti.TableName = row["TableName"].ToString();
                                        ti.KeyFieldName = Navigation.MakeSimpleField(row["IdFieldName"].ToString());
                                        ti.ID = id;
                                        ti.BarcodePrefix = row["BarCodePrefix"].ToString();
                                        ti.TrackingTable = Conversions.ToInteger(row["TrackingTable"]);
                                        if (ti.TrackingTable == 0)
                                            ti.TrackingTable = -1;
                                        return ti;
                                    }
                                    else
                                    {
                                        MoreThanOnePrefix = true;
                                    }
                                }
                                else if (Conversions.ToBoolean(row["Trackable"]))
                                {
                                    var ti = new TableItem();
                                    ti.TableName = row["TableName"].ToString();
                                    ti.KeyFieldName = Navigation.MakeSimpleField(row["IdFieldName"].ToString());
                                    ti.ID = id;
                                    ti.BarcodePrefix = row["BarCodePrefix"].ToString();
                                    ti.TrackingTable = Conversions.ToInteger(row["TrackingTable"]);
                                    if (ti.TrackingTable == 0)
                                        ti.TrackingTable = -1;
                                    return ti;
                                }
                                else
                                {
                                    MoreThanOnePrefix = true;
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(tableName))
                    {
                        cmd.CommandText = "SELECT ScanList.*, Tables.TrackingTable, Tables.TrackingActiveFieldName, Tables.Trackable FROM ScanList INNER JOIN Tables ON Tables.TableName = ScanList.TableName ORDER BY ScanOrder";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT ScanList.*, Tables.TrackingTable, Tables.TrackingActiveFieldName, Tables.Trackable FROM ScanList INNER JOIN Tables ON Tables.TableName = ScanList.TableName WHERE ScanList.TableName=@TableName ORDER BY ScanOrder";
                        // cmd.Parameters.AddWithValue("@TableName", tableName)
                    }

                    dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string id = string.Empty;
                        string barcodeStripped = barcodeData;
                        if (!string.IsNullOrEmpty(row["IdStripChars"].ToString()))
                            barcodeStripped = Strings.Replace(barcodeData, row["IdStripChars"].ToString(), string.Empty, 1, -1, CompareMethod.Text);

                        try
                        {
                            id = ConfirmRecord(row["TableName"].ToString(), row["FieldName"].ToString(), row["TrackingActiveFieldName"].ToString(), barcodeStripped, true, conn);
                        }
                        catch (Exception ex)
                        {
                            if (string.Compare(ex.Message, "Not Active") == 0)
                                throw new Exception(string.Format("{0} is not Active", Interaction.IIf(isDestination, "Destination", "Object")));
                            throw;
                        }

                        if (!string.IsNullOrEmpty(id))
                        {
                            if (isDestination == true)
                            {
                                if (Conversions.ToInteger(row["TrackingTable"]) > 0)
                                {
                                    var ti = new TableItem();
                                    ti.TableName = row["TableName"].ToString();
                                    ti.KeyFieldName = Navigation.MakeSimpleField(Navigation.GetTableInfo(ti.TableName, conn)["IdFieldName"].ToString());
                                    ti.TrackingTable = Conversions.ToInteger(row["TrackingTable"]);
                                    if (ti.TrackingTable == 0)
                                        ti.TrackingTable = -1;
                                    ti.ID = id;
                                    return ti;
                                }
                                else
                                {
                                    MoreThanOnePrefix = true;
                                }
                            }
                            else if (Conversions.ToBoolean(row["Trackable"]))
                            {
                                var ti = new TableItem();
                                ti.TableName = row["TableName"].ToString();
                                ti.KeyFieldName = Navigation.MakeSimpleField(Navigation.GetTableInfo(ti.TableName, conn)["IdFieldName"].ToString());
                                ti.TrackingTable = Conversions.ToInteger(row["TrackingTable"]);
                                if (ti.TrackingTable == 0)
                                    ti.TrackingTable = -1;
                                ti.ID = id;
                                return ti;
                            }
                            else
                            {
                                MoreThanOnePrefix = true;
                            }
                        }
                    }
                    if (MoreThanOnePrefix == true)
                    {
                        if (isDestination == true)
                        {
                            throw new Exception("Entered barcode is not a destination container");
                        }
                        else
                        {
                            throw new Exception("Not authorized to transfer");
                        }
                    }
                    // strip out strip characters found in tables table
                    // loop table and look for prefix and then return item type

                    // strip the prefix
                    // mask the remaining data from tables table
                    // look up record based on mask if there is one.
                    // verify access to object.
                    // If not found then use barcode search order.
                    // need due back, active, and email address. 

                    // added return value to get rid of compiler warning for now. 
                    // Not a big deal at the moment but will need to be addressed once this function is in use.  RVW 06/22/2012
                    return null;
                }
            }
        }

        public static TableItem TranslateBarcode(string barcodeData, bool isDestination, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return TranslateBarcode(barcodeData, isDestination, conn);
            }
        }

        public static bool TrackStatusExists(string trackableType, string trackableId, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) AS Expr1 FROM TrackingStatus WHERE (TrackedTable = @TrackableType) AND (TrackedTableId  = @TrackableID)", conn))
            {
                cmd.Parameters.AddWithValue("@TrackableType", trackableType);
                cmd.Parameters.AddWithValue("@TrackableID", trackableId);
                return Conversions.ToInteger(cmd.ExecuteScalar()) > 0;
            }
        }

        public static void InsertTrack(string trackableType, string trackableId, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("INSERT INTO TrackingStatus (TrackedTableId, TrackedTable, TransactionDateTime) VALUES (@TrackableID,@TrackableType, GETDATE())", conn))
            {
                cmd.Parameters.AddWithValue("@TrackableType", trackableType);
                cmd.Parameters.AddWithValue("@TrackableID", trackableId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Transfer_Old(string trackableType, string trackableId, string destinationType, string destinationId, DateTime DueBackDate, string userName, Passport passport, SqlConnection conn, string trackingAdditionalField1 = null, string trackingAdditionalField2 = null)
        {
            var destInfo = Navigation.GetTableInfo(destinationType, conn);
            var trackableInfo = Navigation.GetTableInfo(trackableType, conn);
            var trackableContainerTypes = GetTrackableContainerTypes(passport, conn);
            string additionalTrackingUpdate = "";

            if (!string.IsNullOrEmpty(trackingAdditionalField1))
            {
                if (!string.IsNullOrEmpty(trackingAdditionalField2))
                {
                    additionalTrackingUpdate = ", [TrackingAdditionalField1]=@TrackingAdditionalField1, [TrackingAdditionalField2]=@TrackingAdditionalField2 ";
                }
                else
                {
                    additionalTrackingUpdate = ", [TrackingAdditionalField1]=@TrackingAdditionalField1 ";
                }
            }
            else if (!string.IsNullOrEmpty(trackingAdditionalField2))
            {
                additionalTrackingUpdate = ", [TrackingAdditionalField2]=@TrackingAdditionalField2 ";
            }

            trackableId = Navigation.PrepPad(trackableType, trackableId, passport);
            destinationId = Navigation.PrepPad(destinationType, destinationId, passport);
            // Modified By Hemin
            // If Not passport.CheckSetting(trackableType, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer) Then Throw New Exception(trackableType & " are not transferable.")
            // If Not passport.CheckPermission(trackableType, SecureObject.SecureObjectType.Table, Permissions.Permission.View) Then Throw New Exception("Not authorized to transfer.")
            // If Not passport.CheckPermission(trackableType, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer) Then Throw New Exception("Not authorized to transfer.")

            if (!passport.CheckSetting(trackableType, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                throw new Exception(string.Format("{0} are not transferable", trackableType));
            if (!passport.CheckPermission(trackableType, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                throw new Exception("Not authorized to transfer");
            if (!passport.CheckPermission(trackableType, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                throw new Exception("Not authorized to transfer");

            if (DueBackDate != default && !((object)DueBackDate is DBNull) && Navigation.CBoolean(Navigation.GetSystemSetting("DateDueOn", conn)))
            {
                foreach (Container cont in trackableContainerTypes)
                {
                    if (string.Compare(cont.Type, destinationType, true) == 0)
                    {
                        if (cont.OutType != 2)
                        {
                            // Modified By Hemin
                            // If DueBackDate < Today Then Throw New Exception("Due back date cannot be less than today.")

                            if (DueBackDate < DateTime.Today)
                                throw new Exception("Due back date cannot be less than today");
                        }

                        break;
                    }
                }
            }

            string requestorTableName = GetRequestorTableName(conn);
            if ((destinationType ?? "") == (requestorTableName ?? ""))
            {
                // Modified By Hemin
                // If AlreadyAtEmployee(trackableType, trackableId, destinationId, conn) Then Throw New Exception("Trackable is already checked out to this employee.")

                if (AlreadyAtEmployee(trackableType, trackableId, destinationId, conn))
                    throw new Exception("Trackable is already checked out to this employee");
            }
            // make sure tracking status has a row. 
            using (var trackingAdapter = new RecordsManageTableAdapters.TrackingTableAdapter())
            {
                trackingAdapter.Connection = conn;
                if (!TrackStatusExists(trackableType, trackableId, conn))
                    InsertTrack(trackableType, trackableId, conn);
                var trackingContainerTypeInfos = GetTrackableContainerTypeInfos(conn);
                // clear destinations
                string clearSQL = "UPDATE TrackingStatus SET ";
                foreach (DataRow row in trackingContainerTypeInfos.Rows)
                {
                    if (clearSQL != "UPDATE TrackingStatus SET ")
                        clearSQL += ", ";
                    clearSQL += row["TrackingStatusFieldName"].ToString() + "=NULL";
                }
                clearSQL += " WHERE [TrackedTableID] = @TrackableID AND [TrackedTable] = @TrackableType";
                using (var cmd = new SqlCommand(clearSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackableType", trackableType);
                    cmd.Parameters.AddWithValue("@TrackableID", trackableId);
                    cmd.ExecuteNonQuery();
                }
                // bring in destination location information 
                foreach (Container cont in trackableContainerTypes)
                {
                    if (cont.Level <= 2)
                    {
                        string DestinationContainerField = GetTrackedTableKeyField(cont.Type, conn);

                        using (var cmd = new SqlCommand("UPDATE TrackingStatus SET " + DestinationContainerField + " = (SELECT " + DestinationContainerField + " FROM TrackingStatus WHERE [TrackedTableId] = @DestinationId AND [TrackedTable] = @DestinationType) " + "WHERE [TrackedTableID] = @TrackableId AND [TrackedTable] = @TrackableType", conn))
                        {
                            cmd.Parameters.AddWithValue("@TrackableType", trackableType);
                            cmd.Parameters.AddWithValue("@TrackableId", trackableId);
                            cmd.Parameters.AddWithValue("@DestinationType", destinationType);
                            cmd.Parameters.AddWithValue("@DestinationId", destinationId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                // tranfer the selected item wheter a container or item
                string destinationTypeKeyField = destInfo["TrackingStatusFieldName"].ToString();

                using (var cmd = new SqlCommand("UPDATE TrackingStatus SET [" + destinationTypeKeyField + "] = @DestinationId, [Out] = @Out, TransactionDateTime = GetDate(), userName = @userName, dateDue = @dueBackDate" + additionalTrackingUpdate + " WHERE [TrackedTableID] = @TrackableId AND [TrackedTable] = @TrackableType", conn))
                {
                    cmd.Parameters.AddWithValue("@TrackableType", trackableType);
                    cmd.Parameters.AddWithValue("@TrackableId", trackableId);
                    cmd.Parameters.AddWithValue("@DestinationType", destinationType);
                    cmd.Parameters.AddWithValue("@DestinationId", destinationId);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@dueBackDate", Interaction.IIf(DueBackDate.Equals(new DateTime()), DBNull.Value, DueBackDate));
                    if (trackingAdditionalField1 is not null)
                        cmd.Parameters.AddWithValue("@TrackingAdditionalField1", trackingAdditionalField1);
                    if (trackingAdditionalField2 is not null)
                        cmd.Parameters.AddWithValue("@TrackingAdditionalField2", trackingAdditionalField2);
                    TrackingTransaction.OutTypes outType;

                    try
                    {
                        outType = (TrackingTransaction.OutTypes)Conversions.ToInteger(destInfo["OutTable"]);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        outType = TrackingTransaction.OutTypes.UseOutField;
                    }

                    switch (outType)
                    {
                        case TrackingTransaction.OutTypes.UseOutField:
                            {
                                try
                                {
                                    bool isOut = Navigation.CBoolean(Navigation.GetSingleFieldValue(destinationType, destinationId, destInfo["TrackingOutFieldName"].ToString(), passport)[0]);
                                    cmd.Parameters.AddWithValue("@Out", isOut);
                                }
                                catch
                                {
                                    cmd.Parameters.AddWithValue("@Out", false);
                                } // No out field, assume always in

                                break;
                            }
                        case TrackingTransaction.OutTypes.AlwaysOut:
                            {
                                cmd.Parameters.AddWithValue("@Out", true);
                                break;
                            }
                        case TrackingTransaction.OutTypes.AlwaysIn:
                            {
                                cmd.Parameters.AddWithValue("@Out", false);
                                break;
                            }
                    }

                    cmd.ExecuteNonQuery();
                }
                // update status records if a container is being moved. 
                string trackableTypeKeyField = trackableInfo["TrackingStatusFieldName"].ToString();

                if (!string.IsNullOrEmpty(trackableTypeKeyField))
                {
                    string sql = string.Empty;
                    int trackingTable = Conversions.ToInteger(destInfo["TrackingTable"]);
                    if (trackingTable == 1)
                    {
                        var row = GetRequestorTableInfo(conn);
                        sql = string.Format("UPDATE TrackingStatus SET [{0}] = NULL, [{1}] = @DestinationId WHERE {2} = @TrackableId", Navigation.MakeSimpleField(row["TrackingStatusFieldName"].ToString()), destinationTypeKeyField, trackableTypeKeyField);
                    }
                    else if (trackingTable == 2)
                    {
                        var row = GetLocationTableInfo(conn);
                        sql = string.Format("UPDATE TrackingStatus SET [{0}] = NULL, [{1}] = @DestinationId WHERE {2} = @TrackableId", Navigation.MakeSimpleField(row["TrackingStatusFieldName"].ToString()), destinationTypeKeyField, trackableTypeKeyField);
                    }
                    else
                    {
                        sql = string.Format("UPDATE TrackingStatus SET [{0}] = @DestinationId WHERE {1} = @TrackableId", destinationTypeKeyField, trackableTypeKeyField);
                    }

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TrackableId", trackableId);
                        cmd.Parameters.AddWithValue("@DestinationId", destinationId);
                        cmd.ExecuteNonQuery();
                    }
                }
                // gather up container types and put in token string for replacement in String.Format below
                string contStr = string.Empty;
                foreach (var cont in trackableContainerTypes)
                    contStr += GetTrackedTableKeyField(cont.Type, conn) + ",";

                using (var cmd = new SqlCommand(string.Format("INSERT INTO TrackingHistory (TrackedTableID, TrackedTable, {0} TransactionDateTime, UserName,[TrackingAdditionalField1],[TrackingAdditionalField2]) " + "SELECT TrackedTableID, TrackedTable, {0} TransactionDateTime, UserName,[TrackingAdditionalField1],[TrackingAdditionalField2] FROM TrackingStatus " + "WHERE TrackedTable = @TrackedTable AND TrackedTableID = @TrackedTableId", contStr), conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", trackableType);
                    cmd.Parameters.AddWithValue("@TrackedTableId", trackableId);
                    cmd.ExecuteScalar();
                }

                if (trackableTypeKeyField.Length > 0)
                {
                    using (var cmd = new SqlCommand(string.Format("INSERT INTO TrackingHistory (TrackedTableID, TrackedTable, {0} TransactionDateTime, UserName,[TrackingAdditionalField1],[TrackingAdditionalField2]) " + "SELECT TrackedTableID, TrackedTable, {0} TransactionDateTime, UserName,[TrackingAdditionalField1],[TrackingAdditionalField2] FROM TrackingStatus " + "WHERE " + trackableTypeKeyField + "= @TrackableID", contStr), conn))
                    {
                        cmd.Parameters.AddWithValue("@TrackableID", trackableId);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (Navigation.CBoolean(Navigation.GetSystemSetting("EMailDeliveryEnabled", conn)))
                {

                    if ((destinationType ?? "") == (requestorTableName ?? ""))
                    {
                        string message = "The following item(s) are en route to: ";
                        string employeeID = destinationId;
                        var employee = new Employee(conn);
                        employee.LoadByID(employeeID, passport);
                        message = message + employee.Description + Environment.NewLine + Environment.NewLine;
                        message = message + "Item             Description                       Due Back" + Environment.NewLine;
                        message = message + "-----------------------------------------------------------" + Environment.NewLine;
                        string item = trackableType + ": " + Navigation.StripLeadingZeros(trackableId);
                        string description = Navigation.GetItemName(trackableType, trackableId, passport);
                        string dueBack = DueBackDate.ToShortDateString();
                        int firstlength = 17 - item.Length;
                        int secondlength = 0;
                        if (firstlength < 0)
                        {
                            secondlength = 50 - (description.Length + 17 - firstlength);
                            firstlength = 0;
                        }
                        else
                        {
                            secondlength = 50 - (description.Length + 17);
                        }
                        if (secondlength < 0)
                            secondlength = 0;
                        message = message + item + new string(' ', firstlength) + description + new string(' ', secondlength) + dueBack;
                        var waitLists = Requesting.GetActiveRequests(trackableType, trackableId, passport);
                        if (waitLists.Count > 0)
                        {
                            string waitListMessage = Environment.NewLine + Environment.NewLine + "                WaitList                                      Requested";
                            waitListMessage = waitListMessage + Environment.NewLine + "                -------------------------------------------------------";
                            foreach (Request req in waitLists)
                            {
                                waitListMessage = waitListMessage + Environment.NewLine + new string(' ', 16);
                                string employeeDesc = Navigation.GetItemName(destinationType, req.EmployeeID, passport);
                                string reqDate = req.DateRequested.ToShortDateString();
                                int length = 55 - employeeDesc.Length - reqDate.Length;
                                if (length < 0)
                                    length = 1;
                                waitListMessage = waitListMessage + employeeDesc + new string(' ', length) + reqDate;
                            }
                            message = message + waitListMessage;
                        }
                        Navigation.SendEmail(message, Navigation.GetEmployeeEmailByID(employeeID, passport), Navigation.GetUserEmail(passport), "Delivery Notification", "", conn);
                    }
                }


                if ((destinationType ?? "") == (requestorTableName ?? ""))
                {
                    Requesting.ProcessRequest(Navigation.GetTableFirstEligibleViewId(trackableType, passport, conn), trackableId, destinationId, "Fulfilled", passport);
                    UpdateNextRequestStatus(trackableType, trackableId, passport, conn);
                }

                var result = ScriptEngine.RunScriptAfterTrackingComplete(trackableType, trackableId, destinationType, destinationId, trackingAdditionalField1, trackingAdditionalField2, passport, conn);
                if (!result.Successful)
                    throw new Exception(result.ReturnMessage);
            }
        }

        public static void Transfer(string trackableTableName, string trackableId, string destinationTableName, string destinationId, DateTime DueBackDate, string userName, Passport passport, SqlConnection conn, string trackingAdditionalField1 = null, string trackingAdditionalField2 = null, bool IsTransferedFromBackground = false, DateTime dtTransactionDateTime = default)
        {

            if (!passport.CheckPermission(trackableTableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                throw new Exception("Not authorized to transfer");
            if (!passport.CheckSetting(trackableTableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                throw new Exception(string.Format("{0} are not transferable", trackableTableName));
            if (!passport.CheckPermission(trackableTableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                throw new Exception("Not authorized to transfer");

            trackableId = Navigation.PrepPad(trackableTableName, trackableId, conn);
            destinationId = Navigation.PrepPad(destinationTableName, destinationId, conn);

            string strSQL = string.Empty;
            userName = userName.Replace("'", "''");
            string tempSQL = "IF OBJECT_ID('tempdb..#InputRecordDataListForHistory') IS NOT NULL DROP TABLE #InputRecordDataListForHistory " + Constants.vbCrLf + "CREATE TABLE #InputRecordDataListForHistory " + Constants.vbCrLf + "( " + Constants.vbCrLf + "    ObjectTableId VARCHAR(50) COLLATE DATABASE_DEFAULT, " + Constants.vbCrLf + "    ObjectTable VARCHAR(50) COLLATE DATABASE_DEFAULT " + Constants.vbCrLf + ") " + Constants.vbCrLf + "INSERT INTO #InputRecordDataListForHistory VALUES ('{0}','{1}'); ";
            // Create temp table for strore object ids as well as existing record in tracking status table for current object
            string strCreateTempTableSQL = string.Format(tempSQL, trackableId, trackableTableName) + Constants.vbCrLf;
            // Fetch trackable tables
            string SQL = "SELECT TrackingTable,TableName,TrackingStatusFieldName,OutTable,TrackingOutFieldName,TrackingRequestableFieldName,IdFieldName FROM [dbo].[Tables] WHERE TrackingTable > 0 ORDER BY TrackingTable";
            var TrackingTablesModel = new List<TrackingTablesModel>();

            using (var cmd = new SqlCommand(SQL, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oTrackingTablesModel = new TrackingTablesModel();
                        oTrackingTablesModel.TrackingTable = Conversions.ToInteger(reader["TrackingTable"]);
                        oTrackingTablesModel.TableName = reader["TableName"].ToString().ToLower();
                        oTrackingTablesModel.TrackingStatusFieldName = reader["TrackingStatusFieldName"].ToString();
                        string outTable = reader["OutTable"].ToString();
                        oTrackingTablesModel.OutTable = Conversions.ToShort(Interaction.IIf(string.IsNullOrEmpty(outTable), 0, outTable));
                        oTrackingTablesModel.TrackingOutFieldName = reader["TrackingOutFieldName"].ToString();
                        oTrackingTablesModel.TrackingRequestableFieldName = reader["TrackingRequestableFieldName"].ToString();
                        oTrackingTablesModel.IdFieldName = reader["IdFieldName"].ToString();
                        TrackingTablesModel.Add(oTrackingTablesModel);
                    }
                }
            }
            // Find destination table field name to exclude from build update tracking field string
            string strDestinationTableFieldName = string.Empty;
            var oDestinationTable = TrackingTablesModel.Where(x => (x.TableName.Trim() ?? "") == (destinationTableName.Trim().ToLower() ?? "")).FirstOrDefault();
            if (oDestinationTable is null)
                throw new Exception("Destination table does not exists.");

            strDestinationTableFieldName = oDestinationTable.TrackingStatusFieldName.ToString().Trim();
            // Prepare variables for dynamic fields which eligible to update in child effected records into tracking status
            int iCount = 0;
            string strTrackingColumnsDeclare = string.Empty;
            string strTrackingColumnsForUpdate = string.Empty;
            string strTrackingColumnsForUpdateValues = string.Empty;
            string strTrackingColumnsForInsert = string.Empty;
            string strTrackingColumnsForTrackingHistory = string.Empty;
            string strTrackingColumnsForUpdateFromVariables = string.Empty;
            var TrackableFieldNameList = TrackingTablesModel.Where(m => !string.IsNullOrEmpty(m.TrackingStatusFieldName)).OrderBy(x => x.TrackingTable).Select(x => x.TrackingStatusFieldName).ToList();
            // Loop to concate fields eligible to update for current object
            foreach (string strFieldName in TrackableFieldNameList)
            {
                iCount += 1;
                if ((strFieldName.Trim().ToLower() ?? "") != (strDestinationTableFieldName.Trim().ToLower() ?? ""))
                {
                    strTrackingColumnsDeclare += Constants.vbCrLf + string.Format("DECLARE @v_{0} AS VARCHAR(50)", Query.ReplaceInvalidParameterCharacters(strFieldName));
                    strTrackingColumnsForUpdate += " @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + " = [" + strFieldName + "], ";
                    strTrackingColumnsForUpdateFromVariables += " [" + strFieldName + "] = @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + ", ";
                    strTrackingColumnsForUpdateValues += " @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + ", ";
                    strTrackingColumnsForInsert += " [" + strFieldName + "], ";
                }
                // Prepared string with all trackable fields for history table updates
                strTrackingColumnsForTrackingHistory += " [" + strFieldName + "], ";
                // Removed extra comma from concated fields string
                if (iCount == TrackableFieldNameList.Count)
                {
                    var MyChar = new[] { ',', ' ' };
                    strTrackingColumnsForUpdate = strTrackingColumnsForUpdate.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForUpdateFromVariables = strTrackingColumnsForUpdateFromVariables.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForUpdateValues = strTrackingColumnsForUpdateValues.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForInsert = strTrackingColumnsForInsert.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForTrackingHistory = strTrackingColumnsForTrackingHistory.Trim().TrimEnd(MyChar);
                }
            }
            // Set SQL string for get destination existing value from database into dynamic tracking columns
            string strSQLSetDestinationValueInTrackableColumns = strTrackingColumnsDeclare + Constants.vbCrLf + string.Format("SELECT TOP 1 {0} FROM [dbo].[TrackingStatus] WHERE TrackedTableId = '{1}' AND TrackedTable = '{2}'", strTrackingColumnsForUpdate, destinationId, destinationTableName) + Constants.vbCrLf;
            // Date Due manipulation and calculation before insert into table
            string strDateDueConditionForNull = "";
            string strDateDue = null;
            if (DueBackDate != DateTime.MinValue)
            {
                var dtDateDueDestCheck = default(DateTime);
                string SQLDueDate = string.Format("SELECT TOP 1 DateDue FROM TrackingStatus WHERE TrackedTableId='{0}' AND TrackedTable = '{1}'", destinationId, destinationTableName);
                using (var cmd = new SqlCommand(SQLDueDate, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!string.IsNullOrEmpty(reader["DateDue"].ToString()))
                            {
                                dtDateDueDestCheck = Conversions.ToDate(reader["DateDue"].ToString());
                            }
                        }
                    }
                }
                // dtDateDueDestCheck = _iTrackingStatus.Where(Function(x) x.TrackedTableId = strDestinationTableId And x.TrackedTable = strDestinationTableName).Select(Function(x) x.DateDue).FirstOrDefault()
                if (dtDateDueDestCheck != DateTime.MinValue)
                {
                    if (dtDateDueDestCheck < DueBackDate)
                    {
                        DueBackDate = Conversions.ToDate(dtDateDueDestCheck);
                    }
                }
                strDateDue = " '" + DueBackDate.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ";
                strDateDueConditionForNull += string.Format(" DateDue = CASE WHEN (DateDue > CAST('{0}' AS DATETIME) OR DateDue IS NULL) THEN {1} ELSE DateDue END ", DueBackDate.ToString("yyyy-MM-dd hh:mm:ss tt"), strDateDue.ToString());
            }
            else
            {
                strDateDueConditionForNull += " DateDue = CASE WHEN DateDue = NULL THEN NULL ELSE DateDue END ";
            }

            // Set Transaction date if passed
            // if its null then hardcode otherwise use the params pass
            string strTransactionDateTime = null;
            if (dtTransactionDateTime == default)
            {
                dtTransactionDateTime = DateTime.Now;
                strTransactionDateTime = " '" + dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ";
            }
            else
            {
                strTransactionDateTime = " '" + dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ";
                // strTransactionDateTime = " '" & dtTransactionDateTime.ToString() & "' "
            }

            // Tracking history only needed if we do not selected Reconciliation mode
            TrackingTransaction.OutTypes outType;
            var OutField = default(bool);
            try
            {
                outType = (TrackingTransaction.OutTypes)(int)oDestinationTable.OutTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                outType = TrackingTransaction.OutTypes.UseOutField;
            }

            switch (outType)
            {
                case TrackingTransaction.OutTypes.UseOutField:
                    {
                        try
                        {
                            OutField = Navigation.CBoolean(Navigation.GetSingleFieldValue(destinationTableName, destinationId, oDestinationTable.TrackingOutFieldName.ToString(), passport)[0]);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            OutField = false;
                        } // No out field, assume always in

                        break;
                    }
                case TrackingTransaction.OutTypes.AlwaysOut:
                    {
                        OutField = true;
                        break;
                    }
                case TrackingTransaction.OutTypes.AlwaysIn:
                    {
                        OutField = false;
                        break;
                    }
            }

            string strSQLObjectDest = "";
            string strSQLTrackingHistory = "";
            string strTrackableFieldNameforObjectTable = "";
            // Get container field name for Object table
            var oObjectTable = TrackingTablesModel.Where(x => (x.TableName.Trim() ?? "") == (trackableTableName.Trim().ToLower() ?? "")).FirstOrDefault();
            if (oObjectTable is not null) // Start : oObjectTable
            {
                strTrackableFieldNameforObjectTable = oObjectTable.TrackingStatusFieldName.ToString().Trim();
                if (!string.IsNullOrEmpty(strTrackableFieldNameforObjectTable))
                {
                    // Query set for add existing tracking for object table into temp table
                    tempSQL = Constants.vbCrLf + "DECLARE @tmpTblObjectDestUpdateIds AS TABLE (Id INT); " + Constants.vbCrLf + "INSERT INTO @tmpTblObjectDestUpdateIds " + Constants.vbCrLf + "SELECT Id FROM TrackingStatus " + Constants.vbCrLf + "WHERE {0} = '{1}'" + Constants.vbCrLf;
                    strSQLObjectDest += string.Format(tempSQL, strTrackableFieldNameforObjectTable.Trim(), trackableId);
                    // Update tracking status child records
                    tempSQL = Constants.vbCrLf + ";UPDATE TS SET {0} = '{1}',TransactionDateTime = {2},{3},[Out] = {4}, UserName = {6} " + Constants.vbCrLf + "FROM dbo.TrackingStatus TS " + Constants.vbCrLf + "INNER JOIN @tmpTblObjectDestUpdateIds tmpID ON tmpID.Id = TS.Id " + Constants.vbCrLf + "WHERE TransactionDateTime <= CAST('{5}' AS DATETIME); " + Constants.vbCrLf;
                    strSQLObjectDest += string.Format(tempSQL, strDestinationTableFieldName, destinationId, strTransactionDateTime, strDateDueConditionForNull, Interaction.IIf(OutField.ToString().ToLower() == "true", "1", "0"), dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt"), Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"));
                    // Insert child effected records into temp created tables into SQL
                    tempSQL = Constants.vbCrLf + "INSERT INTO #InputRecordDataListForHistory " + Constants.vbCrLf + "Select TrackedTableId,TrackedTable " + Constants.vbCrLf + "FROM TrackingStatus TS " + Constants.vbCrLf + "INNER JOIN @tmpTblObjectDestUpdateIds tmpID On tmpID.Id = TS.Id;" + Constants.vbCrLf;
                    strSQLObjectDest += tempSQL;
                    // Update child record in tracking status
                    string strTrackingColumnsBasedonTrackingOrder = "";
                    int intObjectTableTrackingOrder = oObjectTable.TrackingTable;

                    var oTrackingTablesBasedOnTrackingOrder = TrackingTablesModel.Where(x => x.TrackingTable < intObjectTableTrackingOrder & (x.TableName.Trim() ?? "") != (destinationTableName.Trim().ToLower() ?? "")).ToList();
                    if (oTrackingTablesBasedOnTrackingOrder is not null)
                    {
                        iCount = 0;
                        var lstringTracableFieldsNameByTrackingOrder = oTrackingTablesBasedOnTrackingOrder.Where(m => !string.IsNullOrEmpty(m.TrackingStatusFieldName)).OrderBy(x => x.TrackingTable).Select(x => x.TrackingStatusFieldName).ToList();
                        foreach (string strFieldName in lstringTracableFieldsNameByTrackingOrder)
                        {
                            iCount = iCount + 1;
                            strTrackingColumnsBasedonTrackingOrder = strTrackingColumnsBasedonTrackingOrder + " [" + strFieldName + "] = @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + ", ";
                            if (iCount == lstringTracableFieldsNameByTrackingOrder.Count)
                            {
                                var MyChar = new[] { ',', ' ' };
                                strTrackingColumnsBasedonTrackingOrder = strTrackingColumnsBasedonTrackingOrder.Trim().TrimEnd(MyChar);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(strTrackingColumnsBasedonTrackingOrder))
                    {
                        strSQLObjectDest += Constants.vbCrLf + string.Format("UPDATE TS Set {0} FROM TrackingStatus TS INNER JOIN @tmpTblObjectDestUpdateIds tmpID On tmpID.Id = TS.Id; ", strTrackingColumnsBasedonTrackingOrder);
                    }
                }
            } // End : oObjectTable

            tempSQL = Constants.vbCrLf + "INSERT INTO [dbo].[TrackingHistory] ([TrackedTableId], [TrackedTable], [TransactionDateTime], [IsActualScan], [BatchId], " + Constants.vbCrLf + "[UserName], [TrackingAdditionalField1], [TrackingAdditionalField2], " + Constants.vbCrLf + "[" + strDestinationTableFieldName + "]," + strTrackingColumnsForInsert + ") " + Constants.vbCrLf + "VALUES " + Constants.vbCrLf + "('{0}', '{1}', {2}, 1, CAST({3} As VARCHAR(20)), {4} , {5}, {6} , '" + destinationId + "', {7}); ";
            strSQLTrackingHistory += string.Format(tempSQL, trackableId, trackableTableName, Interaction.IIf(string.IsNullOrEmpty(strTransactionDateTime), "NULL", "" + strTransactionDateTime + ""), 0, Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField1), "NULL", "'" + trackingAdditionalField1 + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField2), "NULL", "'" + trackingAdditionalField2 + "'"), strTrackingColumnsForUpdateValues) + Constants.vbCrLf;

            if (!string.IsNullOrEmpty(strTrackableFieldNameforObjectTable))
            {
                tempSQL = Constants.vbCrLf + "INSERT INTO TrackingHistory (TrackedTableId, TrackedTable, TransactionDateTime, IsActualScan, BatchId, UserName, TrackingAdditionalField1, TrackingAdditionalField2, {0}) " + Constants.vbCrLf + "SELECT TrackedTableId, TrackedTable, TransactionDateTime, 0,CAST({1} AS VARCHAR(20)),{3},TrackingAdditionalField1, " + Constants.vbCrLf + "TrackingAdditionalField2, {2} " + Constants.vbCrLf + "FROM TrackingStatus TS " + Constants.vbCrLf + "INNER JOIN @tmpTblObjectDestUpdateIds tmpID ON tmpID.Id = TS.Id; ";
                strSQLTrackingHistory += string.Format(tempSQL, strTrackingColumnsForTrackingHistory, 0, strTrackingColumnsForTrackingHistory, Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"));
            }

            bool bRecExists = false;
            string SQLcheckExists = string.Format("SELECT Id FROM TrackingStatus WHERE TrackedTableId='{0}' AND TrackedTable = '{1}'", trackableId, trackableTableName);
            using (var cmd = new SqlCommand(SQLcheckExists, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        bRecExists = true;
                    }
                }
            }

            if (bRecExists)
            {
                tempSQL = Constants.vbCrLf + "UPDATE [dbo].[TrackingStatus] " + Constants.vbCrLf + "    SET [TrackedTableId] = '{0}' " + Constants.vbCrLf + "    ,[TrackedTable] = '{1}' " + Constants.vbCrLf + "    ,[TransactionDateTime] =  {2} " + Constants.vbCrLf + "    ,[Out] = {3}  " + Constants.vbCrLf + "    ,[TrackingAdditionalField1] = {4} " + Constants.vbCrLf + "    ,[TrackingAdditionalField2] = {5} " + Constants.vbCrLf + "    ,[UserName] = {6} " + Constants.vbCrLf + "    ,[DateDue] = {7} " + Constants.vbCrLf + "    ,[" + strDestinationTableFieldName + "] = '{8}' " + Constants.vbCrLf + "    , " + strTrackingColumnsForUpdateFromVariables + Constants.vbCrLf + "WHERE TrackedTableId= '{9}' AND TrackedTable='{10}' " + Constants.vbCrLf + "AND TransactionDateTime < CAST('" + dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt") + "' AS DATETIME); ";
                strSQL += strSQLSetDestinationValueInTrackableColumns + Constants.vbCrLf + string.Format(tempSQL, trackableId, trackableTableName, strTransactionDateTime, Interaction.IIf(OutField.ToString().ToLower() == "true", "1", "0"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField1), "NULL", "'" + trackingAdditionalField1 + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField2), "NULL", "'" + trackingAdditionalField2 + "'"), Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"), Interaction.IIf(string.IsNullOrEmpty(strDateDue), "NULL", strDateDue), destinationId, trackableId, trackableTableName) + Constants.vbCrLf;

                strSQL += strSQLObjectDest + Constants.vbCrLf;
                strSQL += strSQLTrackingHistory + Constants.vbCrLf;
            }
            else
            {
                tempSQL = Constants.vbCrLf + "INSERT INTO [dbo].[TrackingStatus] ([TrackedTableId], [TrackedTable], [Out], [TrackingAdditionalField1], [TrackingAdditionalField2], [UserName], [DateDue], [TransactionDateTime], " + Constants.vbCrLf + "[" + strDestinationTableFieldName + "]," + strTrackingColumnsForInsert + ") " + Constants.vbCrLf + "VALUES " + Constants.vbCrLf + "('{0}', '{1}', {2} , {3} , {4}, {5}, {6}, {7} , '" + destinationId + "', {8}); ";
                strSQL += strSQLSetDestinationValueInTrackableColumns + Constants.vbCrLf + string.Format(tempSQL, trackableId, trackableTableName, Interaction.IIf(OutField.ToString().ToLower() == "true", "1", "0"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField1), "NULL", "'" + trackingAdditionalField1 + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField2), "NULL", "'" + trackingAdditionalField2 + "'"), Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"), Interaction.IIf(string.IsNullOrEmpty(strDateDue), "NULL", strDateDue), Interaction.IIf(string.IsNullOrEmpty(strTransactionDateTime), "NULL", "" + strTransactionDateTime + ""), strTrackingColumnsForUpdateValues) + Constants.vbCrLf;

                strSQL += strSQLObjectDest + Constants.vbCrLf;
                strSQL += strSQLTrackingHistory + Constants.vbCrLf;
            }

            strSQL = strCreateTempTableSQL + " " + strSQL + " ;";

            string requestorTableName = GetRequestorTableName(conn);
            // 'Update status of most eligible requests (Commented below if condition because of needs to update relevant child records during transfer)
            // If destinationType = requestorTableName Then
            strSQL += Constants.vbCrLf + UpdateRequestsQuery(TrackingTablesModel, trackableTableName, trackableId, conn, passport, destinationId);
            // End If
            // 'Delete tracking history based on admin settings
            strSQL += Constants.vbCrLf + DeleteTrackingHistoryQuery(conn);

            int output;
            using (var cmd = new SqlCommand(strSQL, conn))
            {
                try
                {
                    output = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }

            if (!IsTransferedFromBackground)
            {
                if (Navigation.CBoolean(Navigation.GetSystemSetting("EMailDeliveryEnabled", conn)))
                {
                    if ((destinationTableName ?? "") == (requestorTableName ?? ""))
                    {
                        string message = "The following item(s) are en route to: ";
                        string employeeID = destinationId;
                        var employee = new Employee(conn);
                        employee.LoadByID(employeeID, passport);
                        message = message + employee.Description + Environment.NewLine + Environment.NewLine;
                        message = message + "Item             Description                       Due Back" + Environment.NewLine;
                        message = message + "-----------------------------------------------------------" + Environment.NewLine;
                        string item = trackableTableName + ": " + Navigation.StripLeadingZeros(trackableId);
                        string description = Navigation.GetItemName(trackableTableName, trackableId, passport);
                        string dueBack = DueBackDate.ToShortDateString();
                        int firstlength = 17 - item.Length;
                        int secondlength = 0;
                        if (firstlength < 0)
                        {
                            secondlength = 50 - (description.Length + 17 - firstlength);
                            firstlength = 0;
                        }
                        else
                        {
                            secondlength = 50 - (description.Length + 17);
                        }
                        if (secondlength < 0)
                            secondlength = 0;
                        message = message + item + new string(' ', firstlength) + description + new string(' ', secondlength) + dueBack;
                        var waitLists = Requesting.GetActiveRequests(trackableTableName, trackableId, passport);
                        if (waitLists.Count > 0)
                        {
                            string waitListMessage = Environment.NewLine + Environment.NewLine + "                WaitList                                      Requested";
                            waitListMessage = waitListMessage + Environment.NewLine + "                -------------------------------------------------------";
                            foreach (Request req in waitLists)
                            {
                                waitListMessage = waitListMessage + Environment.NewLine + new string(' ', 16);
                                string employeeDesc = Navigation.GetItemName(destinationTableName, req.EmployeeID, passport);
                                string reqDate = req.DateRequested.ToShortDateString();
                                int length = 55 - employeeDesc.Length - reqDate.Length;
                                if (length < 0)
                                    length = 1;
                                waitListMessage = waitListMessage + employeeDesc + new string(' ', length) + reqDate;
                            }
                            message = message + waitListMessage;
                        }
                        Navigation.SendEmail(message, Navigation.GetEmployeeEmailByID(employeeID, passport), Navigation.GetUserEmail(passport), "Delivery Notification", "", conn);
                    }
                }
            }

            var result = ScriptEngine.RunScriptAfterTrackingComplete(trackableTableName, trackableId, destinationTableName, destinationId, trackingAdditionalField1, trackingAdditionalField2, passport, conn);
            if (!result.Successful)
                throw new Exception(result.ReturnMessage);
        }

        public async static Task TransferAsync(string trackableTableName, string trackableId, string destinationTableName, string destinationId, DateTime DueBackDate, string userName, Passport passport, SqlConnection conn, string trackingAdditionalField1 = null, string trackingAdditionalField2 = null, bool IsTransferedFromBackground = false, DateTime dtTransactionDateTime = default)
        {

            if (!passport.CheckPermission(trackableTableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                throw new Exception("Not authorized to transfer");
            if (!passport.CheckSetting(trackableTableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                throw new Exception(string.Format("{0} are not transferable", trackableTableName));
            if (!passport.CheckPermission(trackableTableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                throw new Exception("Not authorized to transfer");

            trackableId = Navigation.PrepPad(trackableTableName, trackableId, conn);
            destinationId = Navigation.PrepPad(destinationTableName, destinationId, conn);

            string strSQL = string.Empty;
            userName = userName.Replace("'", "''");
            string tempSQL = "IF OBJECT_ID('tempdb..#InputRecordDataListForHistory') IS NOT NULL DROP TABLE #InputRecordDataListForHistory " + Constants.vbCrLf + "CREATE TABLE #InputRecordDataListForHistory " + Constants.vbCrLf + "( " + Constants.vbCrLf + "    ObjectTableId VARCHAR(50) COLLATE DATABASE_DEFAULT, " + Constants.vbCrLf + "    ObjectTable VARCHAR(50) COLLATE DATABASE_DEFAULT " + Constants.vbCrLf + ") " + Constants.vbCrLf + "INSERT INTO #InputRecordDataListForHistory VALUES ('{0}','{1}'); ";
            // Create temp table for strore object ids as well as existing record in tracking status table for current object
            string strCreateTempTableSQL = string.Format(tempSQL, trackableId, trackableTableName) + Constants.vbCrLf;
            // Fetch trackable tables
            string SQL = "SELECT TrackingTable,TableName,TrackingStatusFieldName,OutTable,TrackingOutFieldName,TrackingRequestableFieldName,IdFieldName FROM [dbo].[Tables] WHERE TrackingTable > 0 ORDER BY TrackingTable";
            var TrackingTablesModel = new List<TrackingTablesModel>();

            using (var cmd = new SqlCommand(SQL, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oTrackingTablesModel = new TrackingTablesModel();
                        oTrackingTablesModel.TrackingTable = Conversions.ToInteger(reader["TrackingTable"]);
                        oTrackingTablesModel.TableName = reader["TableName"].ToString().ToLower();
                        oTrackingTablesModel.TrackingStatusFieldName = reader["TrackingStatusFieldName"].ToString();
                        string outTable = reader["OutTable"].ToString();
                        oTrackingTablesModel.OutTable = Conversions.ToShort(Interaction.IIf(string.IsNullOrEmpty(outTable), 0, outTable));
                        oTrackingTablesModel.TrackingOutFieldName = reader["TrackingOutFieldName"].ToString();
                        oTrackingTablesModel.TrackingRequestableFieldName = reader["TrackingRequestableFieldName"].ToString();
                        oTrackingTablesModel.IdFieldName = reader["IdFieldName"].ToString();
                        TrackingTablesModel.Add(oTrackingTablesModel);
                    }
                }
            }
            // Find destination table field name to exclude from build update tracking field string
            string strDestinationTableFieldName = string.Empty;
            var oDestinationTable = TrackingTablesModel.Where(x => (x.TableName.Trim() ?? "") == (destinationTableName.Trim().ToLower() ?? "")).FirstOrDefault();
            if (oDestinationTable is null)
                throw new Exception("Destination table does not exists.");

            strDestinationTableFieldName = oDestinationTable.TrackingStatusFieldName.ToString().Trim();
            // Prepare variables for dynamic fields which eligible to update in child effected records into tracking status
            int iCount = 0;
            string strTrackingColumnsDeclare = string.Empty;
            string strTrackingColumnsForUpdate = string.Empty;
            string strTrackingColumnsForUpdateValues = string.Empty;
            string strTrackingColumnsForInsert = string.Empty;
            string strTrackingColumnsForTrackingHistory = string.Empty;
            string strTrackingColumnsForUpdateFromVariables = string.Empty;
            var TrackableFieldNameList = TrackingTablesModel.Where(m => !string.IsNullOrEmpty(m.TrackingStatusFieldName)).OrderBy(x => x.TrackingTable).Select(x => x.TrackingStatusFieldName).ToList();
            // Loop to concate fields eligible to update for current object
            foreach (string strFieldName in TrackableFieldNameList)
            {
                iCount += 1;
                if ((strFieldName.Trim().ToLower() ?? "") != (strDestinationTableFieldName.Trim().ToLower() ?? ""))
                {
                    strTrackingColumnsDeclare += Constants.vbCrLf + string.Format("DECLARE @v_{0} AS VARCHAR(50)", Query.ReplaceInvalidParameterCharacters(strFieldName));
                    strTrackingColumnsForUpdate += " @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + " = [" + strFieldName + "], ";
                    strTrackingColumnsForUpdateFromVariables += " [" + strFieldName + "] = @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + ", ";
                    strTrackingColumnsForUpdateValues += " @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + ", ";
                    strTrackingColumnsForInsert += " [" + strFieldName + "], ";
                }
                // Prepared string with all trackable fields for history table updates
                strTrackingColumnsForTrackingHistory += " [" + strFieldName + "], ";
                // Removed extra comma from concated fields string
                if (iCount == TrackableFieldNameList.Count)
                {
                    var MyChar = new[] { ',', ' ' };
                    strTrackingColumnsForUpdate = strTrackingColumnsForUpdate.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForUpdateFromVariables = strTrackingColumnsForUpdateFromVariables.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForUpdateValues = strTrackingColumnsForUpdateValues.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForInsert = strTrackingColumnsForInsert.Trim().TrimEnd(MyChar);
                    strTrackingColumnsForTrackingHistory = strTrackingColumnsForTrackingHistory.Trim().TrimEnd(MyChar);
                }
            }
            // Set SQL string for get destination existing value from database into dynamic tracking columns
            string strSQLSetDestinationValueInTrackableColumns = strTrackingColumnsDeclare + Constants.vbCrLf + string.Format("SELECT TOP 1 {0} FROM [dbo].[TrackingStatus] WHERE TrackedTableId = '{1}' AND TrackedTable = '{2}'", strTrackingColumnsForUpdate, destinationId, destinationTableName) + Constants.vbCrLf;
            // Date Due manipulation and calculation before insert into table
            string strDateDueConditionForNull = "";
            string strDateDue = null;
            if (DueBackDate != DateTime.MinValue)
            {
                var dtDateDueDestCheck = default(DateTime);
                string SQLDueDate = string.Format("SELECT TOP 1 DateDue FROM TrackingStatus WHERE TrackedTableId='{0}' AND TrackedTable = '{1}'", destinationId, destinationTableName);
                using (var cmd = new SqlCommand(SQLDueDate, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!string.IsNullOrEmpty(reader["DateDue"].ToString()))
                            {
                                dtDateDueDestCheck = Conversions.ToDate(reader["DateDue"].ToString());
                            }
                        }
                    }
                }
                // dtDateDueDestCheck = _iTrackingStatus.Where(Function(x) x.TrackedTableId = strDestinationTableId And x.TrackedTable = strDestinationTableName).Select(Function(x) x.DateDue).FirstOrDefault()
                if (dtDateDueDestCheck != DateTime.MinValue)
                {
                    if (dtDateDueDestCheck < DueBackDate)
                    {
                        DueBackDate = Conversions.ToDate(dtDateDueDestCheck);
                    }
                }
                strDateDue = " '" + DueBackDate.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ";
                strDateDueConditionForNull += string.Format(" DateDue = CASE WHEN (DateDue > CAST('{0}' AS DATETIME) OR DateDue IS NULL) THEN {1} ELSE DateDue END ", DueBackDate.ToString("yyyy-MM-dd hh:mm:ss tt"), strDateDue.ToString());
            }
            else
            {
                strDateDueConditionForNull += " DateDue = CASE WHEN DateDue = NULL THEN NULL ELSE DateDue END ";
            }

            // Set Transaction date if passed
            // if its null then hardcode otherwise use the params pass
            string strTransactionDateTime = null;
            if (dtTransactionDateTime == default)
            {
                dtTransactionDateTime = DateTime.Now;
                strTransactionDateTime = " '" + dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ";
            }
            else
            {
                strTransactionDateTime = " '" + dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ";
                // strTransactionDateTime = " '" & dtTransactionDateTime.ToString() & "' "
            }

            // Tracking history only needed if we do not selected Reconciliation mode
            TrackingTransaction.OutTypes outType;
            var OutField = default(bool);
            try
            {
                outType = (TrackingTransaction.OutTypes)(int)oDestinationTable.OutTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                outType = TrackingTransaction.OutTypes.UseOutField;
            }

            switch (outType)
            {
                case TrackingTransaction.OutTypes.UseOutField:
                    {
                        try
                        {
                            OutField = Navigation.CBoolean(Navigation.GetSingleFieldValue(destinationTableName, destinationId, oDestinationTable.TrackingOutFieldName.ToString(), passport)[0]);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            OutField = false;
                        } // No out field, assume always in

                        break;
                    }
                case TrackingTransaction.OutTypes.AlwaysOut:
                    {
                        OutField = true;
                        break;
                    }
                case TrackingTransaction.OutTypes.AlwaysIn:
                    {
                        OutField = false;
                        break;
                    }
            }

            string strSQLObjectDest = "";
            string strSQLTrackingHistory = "";
            string strTrackableFieldNameforObjectTable = "";
            // Get container field name for Object table
            var oObjectTable = TrackingTablesModel.Where(x => (x.TableName.Trim() ?? "") == (trackableTableName.Trim().ToLower() ?? "")).FirstOrDefault();
            if (oObjectTable is not null) // Start : oObjectTable
            {
                strTrackableFieldNameforObjectTable = oObjectTable.TrackingStatusFieldName.ToString().Trim();
                if (!string.IsNullOrEmpty(strTrackableFieldNameforObjectTable))
                {
                    // Query set for add existing tracking for object table into temp table
                    tempSQL = Constants.vbCrLf + "DECLARE @tmpTblObjectDestUpdateIds AS TABLE (Id INT); " + Constants.vbCrLf + "INSERT INTO @tmpTblObjectDestUpdateIds " + Constants.vbCrLf + "SELECT Id FROM TrackingStatus " + Constants.vbCrLf + "WHERE {0} = '{1}'" + Constants.vbCrLf;
                    strSQLObjectDest += string.Format(tempSQL, strTrackableFieldNameforObjectTable.Trim(), trackableId);
                    // Update tracking status child records
                    tempSQL = Constants.vbCrLf + ";UPDATE TS SET {0} = '{1}',TransactionDateTime = {2},{3},[Out] = {4}, UserName = {6} " + Constants.vbCrLf + "FROM dbo.TrackingStatus TS " + Constants.vbCrLf + "INNER JOIN @tmpTblObjectDestUpdateIds tmpID ON tmpID.Id = TS.Id " + Constants.vbCrLf + "WHERE TransactionDateTime <= CAST('{5}' AS DATETIME); " + Constants.vbCrLf;
                    strSQLObjectDest += string.Format(tempSQL, strDestinationTableFieldName, destinationId, strTransactionDateTime, strDateDueConditionForNull, Interaction.IIf(OutField.ToString().ToLower() == "true", "1", "0"), dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt"), Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"));
                    // Insert child effected records into temp created tables into SQL
                    tempSQL = Constants.vbCrLf + "INSERT INTO #InputRecordDataListForHistory " + Constants.vbCrLf + "Select TrackedTableId,TrackedTable " + Constants.vbCrLf + "FROM TrackingStatus TS " + Constants.vbCrLf + "INNER JOIN @tmpTblObjectDestUpdateIds tmpID On tmpID.Id = TS.Id;" + Constants.vbCrLf;
                    strSQLObjectDest += tempSQL;
                    // Update child record in tracking status
                    string strTrackingColumnsBasedonTrackingOrder = "";
                    int intObjectTableTrackingOrder = oObjectTable.TrackingTable;

                    var oTrackingTablesBasedOnTrackingOrder = TrackingTablesModel.Where(x => x.TrackingTable < intObjectTableTrackingOrder & (x.TableName.Trim() ?? "") != (destinationTableName.Trim().ToLower() ?? "")).ToList();
                    if (oTrackingTablesBasedOnTrackingOrder is not null)
                    {
                        iCount = 0;
                        var lstringTracableFieldsNameByTrackingOrder = oTrackingTablesBasedOnTrackingOrder.Where(m => !string.IsNullOrEmpty(m.TrackingStatusFieldName)).OrderBy(x => x.TrackingTable).Select(x => x.TrackingStatusFieldName).ToList();
                        foreach (string strFieldName in lstringTracableFieldsNameByTrackingOrder)
                        {
                            iCount = iCount + 1;
                            strTrackingColumnsBasedonTrackingOrder = strTrackingColumnsBasedonTrackingOrder + " [" + strFieldName + "] = @v_" + Query.ReplaceInvalidParameterCharacters(strFieldName) + ", ";
                            if (iCount == lstringTracableFieldsNameByTrackingOrder.Count)
                            {
                                var MyChar = new[] { ',', ' ' };
                                strTrackingColumnsBasedonTrackingOrder = strTrackingColumnsBasedonTrackingOrder.Trim().TrimEnd(MyChar);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(strTrackingColumnsBasedonTrackingOrder))
                    {
                        strSQLObjectDest += Constants.vbCrLf + string.Format("UPDATE TS Set {0} FROM TrackingStatus TS INNER JOIN @tmpTblObjectDestUpdateIds tmpID On tmpID.Id = TS.Id; ", strTrackingColumnsBasedonTrackingOrder);
                    }
                }
            } // End : oObjectTable

            tempSQL = Constants.vbCrLf + "INSERT INTO [dbo].[TrackingHistory] ([TrackedTableId], [TrackedTable], [TransactionDateTime], [IsActualScan], [BatchId], " + Constants.vbCrLf + "[UserName], [TrackingAdditionalField1], [TrackingAdditionalField2], " + Constants.vbCrLf + "[" + strDestinationTableFieldName + "]," + strTrackingColumnsForInsert + ") " + Constants.vbCrLf + "VALUES " + Constants.vbCrLf + "('{0}', '{1}', {2}, 1, CAST({3} As VARCHAR(20)), {4} , {5}, {6} , '" + destinationId + "', {7}); ";
            strSQLTrackingHistory += string.Format(tempSQL, trackableId, trackableTableName, Interaction.IIf(string.IsNullOrEmpty(strTransactionDateTime), "NULL", "" + strTransactionDateTime + ""), 0, Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField1), "NULL", "'" + trackingAdditionalField1 + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField2), "NULL", "'" + trackingAdditionalField2 + "'"), strTrackingColumnsForUpdateValues) + Constants.vbCrLf;

            if (!string.IsNullOrEmpty(strTrackableFieldNameforObjectTable))
            {
                tempSQL = Constants.vbCrLf + "INSERT INTO TrackingHistory (TrackedTableId, TrackedTable, TransactionDateTime, IsActualScan, BatchId, UserName, TrackingAdditionalField1, TrackingAdditionalField2, {0}) " + Constants.vbCrLf + "SELECT TrackedTableId, TrackedTable, TransactionDateTime, 0,CAST({1} AS VARCHAR(20)),{3},TrackingAdditionalField1, " + Constants.vbCrLf + "TrackingAdditionalField2, {2} " + Constants.vbCrLf + "FROM TrackingStatus TS " + Constants.vbCrLf + "INNER JOIN @tmpTblObjectDestUpdateIds tmpID ON tmpID.Id = TS.Id; ";
                strSQLTrackingHistory += string.Format(tempSQL, strTrackingColumnsForTrackingHistory, 0, strTrackingColumnsForTrackingHistory, Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"));
            }

            bool bRecExists = false;
            string SQLcheckExists = string.Format("SELECT Id FROM TrackingStatus WHERE TrackedTableId='{0}' AND TrackedTable = '{1}'", trackableId, trackableTableName);
            using (var cmd = new SqlCommand(SQLcheckExists, conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        bRecExists = true;
                    }
                }
            }

            if (bRecExists)
            {
                tempSQL = Constants.vbCrLf + "UPDATE [dbo].[TrackingStatus] " + Constants.vbCrLf + "    SET [TrackedTableId] = '{0}' " + Constants.vbCrLf + "    ,[TrackedTable] = '{1}' " + Constants.vbCrLf + "    ,[TransactionDateTime] =  {2} " + Constants.vbCrLf + "    ,[Out] = {3}  " + Constants.vbCrLf + "    ,[TrackingAdditionalField1] = {4} " + Constants.vbCrLf + "    ,[TrackingAdditionalField2] = {5} " + Constants.vbCrLf + "    ,[UserName] = {6} " + Constants.vbCrLf + "    ,[DateDue] = {7} " + Constants.vbCrLf + "    ,[" + strDestinationTableFieldName + "] = '{8}' " + Constants.vbCrLf + "    , " + strTrackingColumnsForUpdateFromVariables + Constants.vbCrLf + "WHERE TrackedTableId= '{9}' AND TrackedTable='{10}' " + Constants.vbCrLf + "AND TransactionDateTime < CAST('" + dtTransactionDateTime.ToString("yyyy-MM-dd hh:mm:ss tt") + "' AS DATETIME); ";
                strSQL += strSQLSetDestinationValueInTrackableColumns + Constants.vbCrLf + string.Format(tempSQL, trackableId, trackableTableName, strTransactionDateTime, Interaction.IIf(OutField.ToString().ToLower() == "true", "1", "0"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField1), "NULL", "'" + trackingAdditionalField1 + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField2), "NULL", "'" + trackingAdditionalField2 + "'"), Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"), Interaction.IIf(string.IsNullOrEmpty(strDateDue), "NULL", strDateDue), destinationId, trackableId, trackableTableName) + Constants.vbCrLf;

                strSQL += strSQLObjectDest + Constants.vbCrLf;
                strSQL += strSQLTrackingHistory + Constants.vbCrLf;
            }
            else
            {
                tempSQL = Constants.vbCrLf + "INSERT INTO [dbo].[TrackingStatus] ([TrackedTableId], [TrackedTable], [Out], [TrackingAdditionalField1], [TrackingAdditionalField2], [UserName], [DateDue], [TransactionDateTime], " + Constants.vbCrLf + "[" + strDestinationTableFieldName + "]," + strTrackingColumnsForInsert + ") " + Constants.vbCrLf + "VALUES " + Constants.vbCrLf + "('{0}', '{1}', {2} , {3} , {4}, {5}, {6}, {7} , '" + destinationId + "', {8}); ";
                strSQL += strSQLSetDestinationValueInTrackableColumns + Constants.vbCrLf + string.Format(tempSQL, trackableId, trackableTableName, Interaction.IIf(OutField.ToString().ToLower() == "true", "1", "0"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField1), "NULL", "'" + trackingAdditionalField1 + "'"), Interaction.IIf(string.IsNullOrEmpty(trackingAdditionalField2), "NULL", "'" + trackingAdditionalField2 + "'"), Interaction.IIf(string.IsNullOrEmpty(userName), "NULL", "'" + userName + "'"), Interaction.IIf(string.IsNullOrEmpty(strDateDue), "NULL", strDateDue), Interaction.IIf(string.IsNullOrEmpty(strTransactionDateTime), "NULL", "" + strTransactionDateTime + ""), strTrackingColumnsForUpdateValues) + Constants.vbCrLf;

                strSQL += strSQLObjectDest + Constants.vbCrLf;
                strSQL += strSQLTrackingHistory + Constants.vbCrLf;
            }

            strSQL = strCreateTempTableSQL + " " + strSQL + " ;";

            string requestorTableName = GetRequestorTableName(conn);
            // 'Update status of most eligible requests (Commented below if condition because of needs to update relevant child records during transfer)
            // If destinationType = requestorTableName Then
            strSQL += Constants.vbCrLf + UpdateRequestsQuery(TrackingTablesModel, trackableTableName, trackableId, conn, passport, destinationId);
            // End If
            // 'Delete tracking history based on admin settings
            strSQL += Constants.vbCrLf + DeleteTrackingHistoryQuery(conn);

            int output;
            using (var cmd = new SqlCommand(strSQL, conn))
            {
                try
                {
                    output = await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }

            if (!IsTransferedFromBackground)
            {
                if (Navigation.CBoolean(Navigation.GetSystemSetting("EMailDeliveryEnabled", conn)))
                {
                    if ((destinationTableName ?? "") == (requestorTableName ?? ""))
                    {
                        string message = "The following item(s) are en route to: ";
                        string employeeID = destinationId;
                        var employee = new Employee(conn);
                        employee.LoadByID(employeeID, passport);
                        message = message + employee.Description + Environment.NewLine + Environment.NewLine;
                        message = message + "Item             Description                       Due Back" + Environment.NewLine;
                        message = message + "-----------------------------------------------------------" + Environment.NewLine;
                        string item = trackableTableName + ": " + Navigation.StripLeadingZeros(trackableId);
                        string description = Navigation.GetItemName(trackableTableName, trackableId, passport);
                        string dueBack = DueBackDate.ToShortDateString();
                        int firstlength = 17 - item.Length;
                        int secondlength = 0;
                        if (firstlength < 0)
                        {
                            secondlength = 50 - (description.Length + 17 - firstlength);
                            firstlength = 0;
                        }
                        else
                        {
                            secondlength = 50 - (description.Length + 17);
                        }
                        if (secondlength < 0)
                            secondlength = 0;
                        message = message + item + new string(' ', firstlength) + description + new string(' ', secondlength) + dueBack;
                        var waitLists = Requesting.GetActiveRequests(trackableTableName, trackableId, passport);
                        if (waitLists.Count > 0)
                        {
                            string waitListMessage = Environment.NewLine + Environment.NewLine + "                WaitList                                      Requested";
                            waitListMessage = waitListMessage + Environment.NewLine + "                -------------------------------------------------------";
                            foreach (Request req in waitLists)
                            {
                                waitListMessage = waitListMessage + Environment.NewLine + new string(' ', 16);
                                string employeeDesc = Navigation.GetItemName(destinationTableName, req.EmployeeID, passport);
                                string reqDate = req.DateRequested.ToShortDateString();
                                int length = 55 - employeeDesc.Length - reqDate.Length;
                                if (length < 0)
                                    length = 1;
                                waitListMessage = waitListMessage + employeeDesc + new string(' ', length) + reqDate;
                            }
                            message = message + waitListMessage;
                        }
                        Navigation.SendEmail(message, Navigation.GetEmployeeEmailByID(employeeID, passport), Navigation.GetUserEmail(passport), "Delivery Notification", "", conn);
                    }
                }
            }

            var result = ScriptEngine.RunScriptAfterTrackingComplete(trackableTableName, trackableId, destinationTableName, destinationId, trackingAdditionalField1, trackingAdditionalField2, passport, conn);
            if (!result.Successful)
                throw new Exception(result.ReturnMessage);
        }
        public static void Transfer(string trackableType, string trackableID, string destinationType, string destinationID, DateTime DueBackDate, string userName, Passport passport, string trackingAdditionalField1 = null, string trackingAdditionalField2 = null, DateTime dtTransactionDateTime = default)
        {
            using (var conn = passport.Connection())
            {
                Transfer(trackableType, trackableID, destinationType, destinationID, DueBackDate, userName, passport, conn, trackingAdditionalField1, trackingAdditionalField2, false, dtTransactionDateTime);
            }
        }
        public async static Task TransferAsync(string trackableType, string trackableID, string destinationType, string destinationID, DateTime DueBackDate, string userName, Passport passport, string trackingAdditionalField1 = null, string trackingAdditionalField2 = null, DateTime dtTransactionDateTime = default)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
              await TransferAsync(trackableType, trackableID, destinationType, destinationID, DueBackDate, userName, passport, conn, trackingAdditionalField1, trackingAdditionalField2, false, dtTransactionDateTime);
            }
        }

        /// <summary>
        /// Prepared query for update request status for second container level
        /// </summary>
        /// <param name="oTrackableTables"></param>
        /// <param name="oTrackedTable"></param>
        /// <param name="oTrackedTableId"></param>
        /// <param name="conn"></param>
        /// <param name="passport"></param>
        /// <returns></returns>
        private static string UpdateRequestsQuery(List<TrackingTablesModel> oTrackableTables, string oTrackedTable, string oTrackedTableId, SqlConnection conn, Passport passport, string destinationId)
        {
            string strUpdateRequestSQL = "";
            string strUpdateMostEligibleRequestSQL = "";

            strUpdateRequestSQL += "IF OBJECT_ID('tempdb..#EffectedRecordForRemoveRequest') IS NOT NULL " + Constants.vbCrLf + "DROP TABLE #EffectedRecordForRemoveRequest " + Constants.vbCrLf + "CREATE TABLE #EffectedRecordForRemoveRequest " + Constants.vbCrLf + "( " + Constants.vbCrLf + "   ObjectTableId VARCHAR(50) COLLATE DATABASE_DEFAULT, " + Constants.vbCrLf + "    ObjectTable VARCHAR(50) COLLATE DATABASE_DEFAULT, " + Constants.vbCrLf + "    SecoundContainerId VARCHAR(50) COLLATE DATABASE_DEFAULT, " + Constants.vbCrLf + "    TransactionDateTime DATETIME " + Constants.vbCrLf + "); ";
            var oSecoundLevelContainer = oTrackableTables.Where(x => x.TrackingTable == 2).FirstOrDefault();
            string strSecoundLevelContainerName = "";
            if (oSecoundLevelContainer is not null)
            {
                strSecoundLevelContainerName = oSecoundLevelContainer.TrackingStatusFieldName.Trim();
            }

            strUpdateRequestSQL += Constants.vbCrLf + "INSERT INTO #EffectedRecordForRemoveRequest " + Constants.vbCrLf + "SELECT IDH.ObjectTableId, IDH.ObjectTable, TS." + strSecoundLevelContainerName + ", TS.TransactionDateTime" + Constants.vbCrLf + "From TrackingStatus TS " + Constants.vbCrLf + "INNER JOIN #InputRecordDataListForHistory IDH On IDH.ObjectTableId=TS.TrackedTableId And IDH.ObjectTable=TS.TrackedTable ";

            strUpdateRequestSQL += Constants.vbCrLf + "UPDATE SLR Set Status =  Case When SLR.EmployeeId =  '{0}' Then 'Fulfilled' WHEN Status IN ('New','In Process') THEN 'WaitList' Else Status End , " + Constants.vbCrLf + "DateReceived = CASE WHEN SLR.EmployeeId =  '{0}' THEN GETDATE() Else DateReceived End , " + Constants.vbCrLf + "SLPullListsId = CASE WHEN Status = 'In Process' THEN 0 Else SLPullListsId End ," + Constants.vbCrLf + "DatePulled =  CASE WHEN Status = 'In Process' THEN GETDATE() Else DatePulled End " + Constants.vbCrLf + "From SLRequestor SLR " + Constants.vbCrLf + "INNER JOIN #EffectedRecordForRemoveRequest ER ON SLR.TableId = ER.ObjectTableId And SLR.TableName = ER.ObjectTable " + Constants.vbCrLf + "WHERE SLR.Status Not IN ('Deleted','Fulfilled') ";

            strUpdateRequestSQL = string.Format(strUpdateRequestSQL, destinationId);
            var oLocationsTable = oTrackableTables.Where(x => x.TrackingTable == 1).FirstOrDefault();
            strUpdateMostEligibleRequestSQL += Constants.vbCrLf + "SELECT ROW_NUMBER() OVER(PARTITION BY SR.TableName,SR.TableId ORDER BY SR.[Priority], SR.[DateRequested] ) AS RowNum, " + Constants.vbCrLf + "SR.Id " + Constants.vbCrLf + "FROM SLRequestor SR " + Constants.vbCrLf + "INNER Join [TrackingStatus] TS ON SR.[TableName] = TS.[TrackedTable] And SR.[TableId] = TS.[TrackedTableId] And (TS.[" + Navigation.MakeSimpleField(oLocationsTable.TrackingStatusFieldName) + "] Is Not NULL) ";
            // WHERE SR.TableId = {0} AND SR.TableName = {1}", oTrackedTableId, oTrackedTable)
            // INNER JOIN @InputRecordTable AS INREC ON SR.TableId = INREC.ObjectTableId AND SR.TableName = INREC.ObjectTable "

            if (oLocationsTable is not null)
            {
                if (!string.IsNullOrEmpty(Strings.Trim(oLocationsTable.TrackingRequestableFieldName)))
                {
                    strUpdateMostEligibleRequestSQL += Constants.vbCrLf + " INNER JOIN [" + oLocationsTable.TableName + "] TSL ON TS.[" + Navigation.MakeSimpleField(oLocationsTable.TrackingStatusFieldName) + "] = ";

                    // Dim IfIdFieldIsString = GetInfoUsingADONET.IdFieldIsString(DefaultIDBManager, oLocationsTable.TableName, oLocationsTable.IdFieldName)
                    // If (IfIdFieldIsString) Then
                    if (Navigation.IsAStringType(Navigation.GetFieldType(oLocationsTable.TableName, oLocationsTable.IdFieldName, passport)))
                    {
                        strUpdateMostEligibleRequestSQL += " TSL.[" + Navigation.MakeSimpleField(oLocationsTable.IdFieldName) + "]";
                    }
                    else
                    {
                        SetUserlinkIdSize(conn);
                        strUpdateMostEligibleRequestSQL += " RIGHT('" + new string('0', _userLinkIndexTableIdSize) + "' + CONVERT(VARCHAR(" + _userLinkIndexTableIdSize.ToString() + "), TSL.[" + Navigation.MakeSimpleField(oLocationsTable.IdFieldName) + "]), " + _userLinkIndexTableIdSize.ToString() + ")";
                    }

                }
            }
            strUpdateMostEligibleRequestSQL += Constants.vbCrLf + string.Format("WHERE SR.[Status] = 'WaitList' " + Constants.vbCrLf + "And Not EXISTS(SELECT TOP 1 Id FROM SLRequestor SRSUB WHERE SRSUB.TableName = '{1}' AND SRSUB.TableId = '{0}' " + Constants.vbCrLf + "And SRSUB.[Status] IN ('New','New Batch','In Process','Exception')) " + Constants.vbCrLf + "And SR.TableId = '{0}' AND SR.TableName = '{1}'", oTrackedTableId, oTrackedTable);

            if (!string.IsNullOrEmpty(Strings.Trim(oLocationsTable.TrackingRequestableFieldName)))
            {
                strUpdateMostEligibleRequestSQL += " AND (TSL.[" + Navigation.MakeSimpleField(oLocationsTable.TrackingRequestableFieldName) + "] <> 0)";
            }

            strUpdateRequestSQL += Constants.vbCrLf + ";WITH cteEligibleRequest AS " + Constants.vbCrLf + "(" + Constants.vbCrLf + "" + strUpdateMostEligibleRequestSQL + " " + Constants.vbCrLf + ")  UPDATE SR Set SR.[Status] = 'New' FROM SLRequestor SR " + Constants.vbCrLf + "INNER JOIN cteEligibleRequest ER ON ER.Id = SR.Id WHERE ER.RowNum < 2; ";

            return strUpdateRequestSQL;
        }

        /// <summary>
        /// Prepared query for Delete Tracking history based on admin configuration
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static string DeleteTrackingHistoryQuery(SqlConnection conn)
        {
            string strDelTrackingHistory = "";
            string strMaxHistoryDays = Navigation.GetSystemSetting("MaxHistoryDays", conn);
            string strMaxHistoryItems = Navigation.GetSystemSetting("MaxHistoryItems", conn);
            int intMaxHistoryDays = Conversions.ToInteger(Interaction.IIf(string.IsNullOrEmpty(strMaxHistoryDays), 0, strMaxHistoryDays));
            int intMaxHistoryItems = Conversions.ToInteger(Interaction.IIf(string.IsNullOrEmpty(strMaxHistoryItems), 0, strMaxHistoryItems));

            if (intMaxHistoryDays > 0)
            {
                var dMaxDate = DateTime.FromOADate(DateTime.Now.ToOADate() - intMaxHistoryDays - 1d);
                string dUTC = dMaxDate.ToUniversalTime().ToString("yyyy-MM-dd hh:mm:ss tt");
                strDelTrackingHistory += Constants.vbCrLf + string.Format("DELETE TH  " + Constants.vbCrLf + "From TrackingHistory TH " + Constants.vbCrLf + "INNER JOIN	#InputRecordDataListForHistory RT ON RT.ObjectTable = TH.TrackedTable AND RT.ObjectTableId = TH.TrackedTableId " + Constants.vbCrLf + "Where TH.TransactionDateTime < CAST('{0}' AS DATETIME) ", dUTC);

            }
            if (intMaxHistoryItems > 0)
            {
                strDelTrackingHistory += Constants.vbCrLf + ";WITH cteDeleteHistory AS " + Constants.vbCrLf + "(" + Constants.vbCrLf + "    SELECT		ROW_NUMBER() OVER(PARTITION BY TH.TrackedTable,TH.TrackedTableId ORDER BY TH.TransactionDateTime DESC) AS RowNum, TH.Id " + Constants.vbCrLf + "    From TrackingHistory TH " + Constants.vbCrLf + "    INNER JOIN	(SELECT		TrackedTableId,TrackedTable " + Constants.vbCrLf + "		        From TrackingHistory " + Constants.vbCrLf + "		        GROUP BY	TrackedTableId,TrackedTable " + Constants.vbCrLf + "		        HAVING      COUNT(ID) > " + intMaxHistoryItems.ToString() + ") AS AST ON AST.TrackedTableId = TH.TrackedTableId And AST.TrackedTable = TH.TrackedTable " + Constants.vbCrLf + "    INNER JOIN	#InputRecordDataListForHistory RT ON RT.ObjectTable = TH.TrackedTable AND RT.ObjectTableId = TH.TrackedTableId " + Constants.vbCrLf + ") " + Constants.vbCrLf + "DELETE TH " + Constants.vbCrLf + "From TrackingHistory TH " + Constants.vbCrLf + "INNER JOIN cteDeleteHistory CDH on CDH.Id = TH.Id " + Constants.vbCrLf + "Where CDH.RowNum > " + intMaxHistoryItems.ToString() + "; ";
            }
            return strDelTrackingHistory;
        }

        /// <summary>
        /// Added for find field size for UserLinks table
        /// </summary>
        private const int UserLinkIndexTableIdSize = 30;
        private static int _userLinkIndexTableIdSize = 0;
        private static int SetUserlinkIdSize(SqlConnection conn)
        {
            if (_userLinkIndexTableIdSize > 0)
                return _userLinkIndexTableIdSize;
            try
            {
                var dt = FillDataTable("SELECT IndexTableId FROM Userlinks WHERE 0=1", conn);
                _userLinkIndexTableIdSize = dt.Columns[0].MaxLength;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _userLinkIndexTableIdSize = UserLinkIndexTableIdSize;
            }

            return _userLinkIndexTableIdSize;
        }

        internal static DataTable FillDataTable(string sql, SqlConnection conn)
        {
            try
            {
                var da = new SqlDataAdapter(new SqlCommand(sql, conn));
                var dt = new DataTable();
                da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                da.FillSchema(dt, SchemaType.Source);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static void UpdateNextRequestStatus(string sourceTableName, string sourceTableId, Passport passport, SqlConnection conn)
        {
            var requesting = new Requesting();
            int viewId = Navigation.GetTableFirstEligibleViewId(sourceTableName, passport, conn);
            if (requesting.GetActiveRequests(viewId, sourceTableId, passport, conn).Count == 0)
                return;
            var nextRequest = requesting.GetActiveRequests(Navigation.GetTableFirstEligibleViewId(sourceTableName, passport, conn), sourceTableId, passport, conn)[0];

            using (var cmd = new SqlCommand("UPDATE [SLRequestor] SET [Status] = @Status WHERE [Id] = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", (object)nextRequest.RequestID);

                string status = "New";
                Dictionary<string, string> argidsByTable = null;
                var tracks = GetTrackableStatus(sourceTableName, sourceTableId, passport, conn, idsByTable: ref argidsByTable);

                foreach (TrackingTransaction track in tracks)
                {
                    if (track.Out)
                    {
                        status = "WaitList";
                        break;
                    }
                }

                cmd.Parameters.AddWithValue("@Status", status);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateNextRequestStatus(string sourceTableName, string sourceTableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                UpdateNextRequestStatus(sourceTableId, sourceTableId, passport, conn);
            }
        }

        public static List<Container> GetTrackableContainerTypes(Passport passport, SqlConnection conn)
        {
            Container cont;
            var list = new List<Container>();

            foreach (DataRow row in GetTrackingContainerTypes(conn).Rows)
            {
                cont = new Container(row["TableName"].ToString(), passport);
                cont.ID = row["TrackingTable"].ToString();
                cont.Level = Conversions.ToInteger(row["TrackingTable"]);
                cont.OutType = Conversions.ToInteger(row["OutTable"]);
                list.Add(cont);
            }

            return list;
        }

        public static List<Container> GetTrackableContainerTypes(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackableContainerTypes(passport, conn);
            }
        }

        public static DataTable GetTrackingContainerTypes(SqlConnection conn)
        {
            string sql = "SELECT TableName, TrackingTable, IdFieldName, TrackingStatusFieldName, OutTable, BarCodePrefix, DescFieldNameOne, " + "DescFieldNameTwo, DescFieldPrefixOne, DescFieldPrefixTwo, [TrackingPhoneFieldName], [TrackingMailStopFieldName], " + "[TrackingRequestableFieldName], UserName " + "FROM [Tables] " + "WHERE (Not (TrackingStatusFieldName Is NULL)) " + "  AND TableName NOT IN (SELECT CAST(ItemValue AS VARCHAR(30)) AS ItemValue FROM Settings WHERE Section = 'Transfer' AND Item = 'ExcludeTable') " + "ORDER BY TrackingTable";

            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public async static Task<DataTable> GetTrackingContainerTypesAsync(string connectionstring)
        {
            string sql = "SELECT TableName, TrackingTable, IdFieldName, TrackingStatusFieldName, OutTable, BarCodePrefix, DescFieldNameOne, " + "DescFieldNameTwo, DescFieldPrefixOne, DescFieldPrefixTwo, [TrackingPhoneFieldName], [TrackingMailStopFieldName], " + "[TrackingRequestableFieldName], UserName " + "FROM [Tables] " + "WHERE (Not (TrackingStatusFieldName Is NULL)) " + "  AND TableName NOT IN (SELECT CAST(ItemValue AS VARCHAR(30)) AS ItemValue FROM Settings WHERE Section = 'Transfer' AND Item = 'ExcludeTable') " + "ORDER BY TrackingTable";
            using (var conn = new SqlConnection(connectionstring))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            
        }

        public static DataTable GetTrackableContainerTypeInfos(SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE (NOT (TrackingStatusFieldName IS NULL)) ORDER BY TrackingTable", conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static bool IsOutDestination(string TableName, string TableId, Passport Passport)
        {
            return IsOutDestination(TableName, TableId, TableId, Passport);
        }

        public static bool IsOutDestination(string TableName, string TableId, string TextEntered, SqlConnection conn)
        {
            var destInfo = Navigation.GetTableInfo(TableName, conn);
            TrackingTransaction.OutTypes outType;
            // Modified By Hemin
            // If CInt(destInfo("TrackingTable")) = 0 Then Throw New Exception(String.Format("""{0}"" is not a container", TextEntered.ToUpper))
            if (Conversions.ToInteger(destInfo["TrackingTable"]) == 0)
                throw new Exception(string.Format("{0} is not a container", TextEntered.ToUpper()));

            string outField = Navigation.GetTableInfo(TableName, conn)["TrackingOutFieldName"].ToString();

            try
            {
                outType = (TrackingTransaction.OutTypes)Conversions.ToInteger(destInfo["OutTable"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                outType = TrackingTransaction.OutTypes.UseOutField;
            }

            if (!string.IsNullOrEmpty(outField))
            {
                return Navigation.CBoolean(Navigation.GetSingleFieldValue(TableName, TableId, destInfo["TrackingOutFieldName"].ToString(), conn)[0]);
            }
            else
            {
                return outType == TrackingTransaction.OutTypes.AlwaysOut;
            }
        }

        public static bool IsOutDestination(string TableName, string TableId, string TextEntered, Passport Passport)
        {
            using (var conn = Passport.Connection())
            {
                return IsOutDestination(TableName, TableId, TextEntered, conn);
            }
        }

        public static DateTime CalcDueBackDate(string containerTableName, string tableId, Passport passport, SqlConnection conn)
        {
            tableId = Navigation.PrepPad(containerTableName, tableId, passport);

            using (var systemAdapter = new RecordsManageTableAdapters.SystemTableAdapter())
            {
                systemAdapter.Connection = conn;
                int defaultDays = 0;
                string test = Navigation.GetSingleFieldValue("System", "1", "DefaultDueBackDays", conn)[0].ToString();
                if (!string.IsNullOrEmpty(test) && Information.IsNumeric(test))
                    defaultDays = Conversions.ToInteger(test);
                string dueBackField = Navigation.GetTableInfo(containerTableName, conn)["TrackingDueBackDaysFieldName"].ToString();

                if (!string.IsNullOrEmpty(dueBackField))
                {
                    int dueDays = 0;
                    test = Navigation.GetSingleFieldValue(containerTableName, tableId, dueBackField, conn)[0].ToString();
                    if (!string.IsNullOrEmpty(test) && Information.IsNumeric(test))
                        dueDays = Conversions.ToInteger(test);
                    if (dueDays > defaultDays)
                        return DateAndTime.DateAdd(DateInterval.Day, dueDays, DateTime.Today);
                }

                return DateAndTime.DateAdd(DateInterval.Day, defaultDays, DateTime.Today);
            }
        }

        public static DateTime CalcDueBackDate(string containerTableName, string tableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return CalcDueBackDate(containerTableName, tableId, passport, conn);
            }

        }

        public static DataTable GetContainerContents(string containerTableName, string containerTableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                var tableInfo = Navigation.GetTableInfo(containerTableName, conn);
                string sql = string.Format("SELECT * FROM TrackingStatus WHERE [{0}] = @containerTableId", tableInfo["TrackingStatusFieldName"].ToString());

                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (Navigation.FieldIsAString(tableInfo, conn))
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId.PadLeft(30, '0'));
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable(Navigation.GetItemName(containerTableName, containerTableId, passport, conn));
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static DataTable GetContainerContentsPaging(string containerTableName, string containerTableId, Passport passport, int pageNumber, int perPageRecord)
        {
            using (var conn = passport.Connection())
            {
                var tableInfo = Navigation.GetTableInfo(containerTableName, conn);
                string sql = string.Format("SELECT * FROM TrackingStatus WHERE [{0}] = @containerTableId", tableInfo["TrackingStatusFieldName"].ToString());

                sql = sql + " order by id desc " + Query.QueryPaging(pageNumber, perPageRecord);

                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (Navigation.FieldIsAString(tableInfo, conn))
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId.PadLeft(30, '0'));
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable(Navigation.GetItemName(containerTableName, containerTableId, passport, conn));
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public static DataTable GetContainerContentsCount(string containerTableName, string containerTableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                var tableInfo = Navigation.GetTableInfo(containerTableName, conn);
                string sql = string.Format("SELECT count(*) FROM TrackingStatus WHERE [{0}] = @containerTableId", tableInfo["TrackingStatusFieldName"].ToString());

                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (Navigation.FieldIsAString(tableInfo, conn))
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId.PadLeft(30, '0'));
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable(Navigation.GetItemName(containerTableName, containerTableId, passport, conn));
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static async Task<DataTable> GetContainerContentsCountAsync(string containerTableName, string containerTableId, Passport passport)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                var tableInfo = Navigation.GetTableInfo(containerTableName, conn);
                string sql = string.Format("SELECT count(*) FROM TrackingStatus WHERE [{0}] = @containerTableId", tableInfo["TrackingStatusFieldName"].ToString());

                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (Navigation.FieldIsAString(tableInfo, conn))
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId.PadLeft(30, '0'));
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable(Navigation.GetItemName(containerTableName, containerTableId, passport, conn));
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public static DataTable GetPagedContainerContents(string containerTableName, string containerTableId, Parameters @params, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(string.Empty, conn))
                {
                    var tableInfo = Navigation.GetTableInfo(containerTableName, conn);
                    string sql = string.Format("SELECT * FROM TrackingStatus WHERE [{0}] = @containerTableId", tableInfo["TrackingStatusFieldName"].ToString());
                    string orderClause = " ORDER BY " + tableInfo["TrackingStatusFieldName"].ToString();
                    cmd.CommandText = sql;

                    if (Navigation.FieldIsAString(tableInfo, conn))
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@containerTableId", containerTableId.PadLeft(30, '0'));
                    }

                    if (@params.Paged)
                    {
                        @params.TotalRows = Query.TotalQueryRowCount(cmd);
                        cmd.CommandText = Query.Pagify(@params, sql, orderClause);
                    }
                    else
                    {
                        cmd.CommandText = sql + orderClause;
                    }

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable(Navigation.GetItemName(containerTableName, containerTableId, passport, conn));
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static bool get_IsContainer(string containerTableName, Passport passport)
        {
            if (object.ReferenceEquals(Navigation.GetTableInfo(containerTableName, passport)["TrackingStatusFieldName"], DBNull.Value))
                return false;
            return true;
        }

        public static List<Container> GetContainersByType(string containerTableName, string find, bool requestOnBehalf, Passport passport, SqlConnection conn)
        {
            Container cont;
            var list = new List<Container>();
            var tableInfo = Navigation.GetTableInfo(containerTableName, conn);
            int OutType = Conversions.ToInteger(tableInfo["OutTable"]);
            bool ContainerTypeIsString = Navigation.FieldIsAString(containerTableName, passport);
            var ItemNames = Navigation.GetItemNames(containerTableName, conn, tableInfo);

            foreach (DataRow row in GetTrackingContainers(containerTableName, find, requestOnBehalf, passport, conn).Rows)
            {
                string desc1 = "";
                string desc2 = "";
                var nameRows = ItemNames.Select("id='" + row[Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString())].ToString() + "'");
                cont = new Container(containerTableName, ContainerTypeIsString);
                cont.Name = "";
                cont.ID = row[Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString())].ToString();

                if (nameRows.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(tableInfo["DescFieldNameOne"].ToString()))
                    {
                        if ((tableInfo["DescFieldNameOne"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(tableInfo) ?? "") & tableInfo["BarCodePrefix"].ToString().Length > 0)
                        {
                            desc1 = Navigation.BarcodeText(tableInfo, row[Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString())].ToString());
                        }
                        else
                        {
                            desc1 = nameRows[0]["desc1"].ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(tableInfo["DescFieldNameTwo"].ToString()))
                    {
                        if ((tableInfo["DescFieldNameTwo"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(tableInfo) ?? "") & tableInfo["BarCodePrefix"].ToString().Length > 0)
                        {
                            desc2 = Navigation.BarcodeText(tableInfo, row[Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString())].ToString());
                        }
                        else
                        {
                            desc2 = nameRows[0]["desc2"].ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(desc1) & string.IsNullOrEmpty(desc2))
                    {
                        cont.Name = Navigation.DisplayFieldsNotConfigured(desc1, desc2, containerTableName, row[Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString())].ToString(), tableInfo, passport);
                    }
                    else if (string.IsNullOrEmpty(desc1))
                    {
                        cont.Name = desc2;
                    }
                    else if (string.IsNullOrEmpty(desc2))
                    {
                        cont.Name = desc1;
                    }
                    else
                    {
                        cont.Name = desc1 + " " + desc2;
                    }
                }

                cont.OutType = OutType;
                list.Add(cont);
            }

            return list;
        }

        public static List<Container> GetContainersByType(string containerTableName, string find, bool requestOnBehalf, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetContainersByType(containerTableName, find, requestOnBehalf, passport, conn);
            }
        }

        public static List<Container> GetContainersByType(string containerTableName, string find, Passport passport, SqlConnection conn)
        {
            return GetContainersByType(containerTableName, find, true, passport, conn);
        }

        public static List<Container> GetContainersByType(string containerTableName, string find, Passport passport)
        {
            return GetContainersByType(containerTableName, find, true, passport);
        }

        public static string GetTrackedTableKeyField(string trackableTableName, SqlConnection conn)
        {
            var dr = Navigation.GetTableInfo(trackableTableName, conn);
            return dr["TrackingStatusFieldName"].ToString();
        }

        public static string GetTrackedTableKeyField(string trackableTableName, Passport passport)
        {
            var dr = Navigation.GetTableInfo(trackableTableName, passport);
            return dr["TrackingStatusFieldName"].ToString();
        }

        public static DataTable GetTrackedTableInfo(SqlConnection conn, DataTable dtContainers = null, DataTable dtTrackedTables = null)
        {
            if (dtTrackedTables is null)
                dtTrackedTables = GetTrackedTables(conn);
            if (dtContainers is null)
                dtContainers = GetTrackingContainerTypes(conn);

            var trackingTableList = new List<string>();

            foreach (DataRow contRow in dtContainers.Rows)
                trackingTableList.Add(contRow["TableName"].ToString());
            foreach (DataRow trackRow in dtTrackedTables.Rows)
            {
                if (!trackingTableList.Contains(trackRow["TrackedTable"].ToString()))
                {
                    trackingTableList.Add(trackRow["TrackedTable"].ToString());
                }
            }
            return Navigation.GetMultipleTableInfo(trackingTableList, conn);
        }

        public static DataTable GetTrackedTableInfo(Passport passport, DataTable dtContainers = null, DataTable dtTrackedTables = null)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackedTableInfo(conn, dtContainers, dtTrackedTables);
            }
        }
        // moti mashiah build tracking need to discuss with Reggie. 
        private static List<TrackingTransaction> BuildTracking(DataTable dtTracking,
            Passport passport, SqlConnection conn,
            [Optional, DefaultParameterValue(null)] DataTable dtContainers,
            [Optional, DefaultParameterValue(null)] DataTable dtTrackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, DataTable> descriptions)
        {
            if (dtTracking.Rows.Count == 0)
                return null;
            var lstTrack = new List<TrackingTransaction>();
            TrackingTransaction track;
            Container container;

            if (dtTrackedTables is null)
                dtTrackedTables = GetTrackedTables(conn);
            if (dtContainers is null)
                dtContainers = GetTrackingContainerTypes(conn);

            var idIsString = new Dictionary<string, bool>();
            idsByTable = new Dictionary<string, string>();

            var tablesInfo = GetTrackedTableInfo(conn, dtContainers, dtTrackedTables);

            foreach (DataRow tableInfoRow in tablesInfo.Rows)
            {
                idIsString.Add(tableInfoRow["TableName"].ToString().ToLower(), Navigation.FieldIsAString(tableInfoRow, conn));
                idsByTable.Add(tableInfoRow["TableName"].ToString().ToLower(), "");
            }

            int i = 0;

            foreach (DataRow trackRow in dtTracking.Rows)
            {
                // Added check to see if TrackedTable exists.  RVW 05/30/2017
                if (idsByTable.ContainsKey(trackRow["TrackedTable"].ToString().ToLower()))
                {
                    if (string.IsNullOrEmpty(idsByTable[trackRow["TrackedTable"].ToString().ToLower()]))
                    {
                        idsByTable[trackRow["TrackedTable"].ToString().ToLower()] += "'" + trackRow["TrackedTableID"].ToString().Replace("'", "''") + "'";
                    }
                    else
                    {
                        idsByTable[trackRow["TrackedTable"].ToString().ToLower()] += ",'" + trackRow["TrackedTableID"].ToString().Replace("'", "''") + "'";
                    }
                }
            }

            if (descriptions is null)
            {
                descriptions = new Dictionary<string, DataTable>();

                foreach (DataRow tableInfo in tablesInfo.Rows)
                {
                    string ids = "";
                    if (idsByTable.TryGetValue(tableInfo["TableName"].ToString().ToLower(), out ids)) // If we have ids, prepopulate; otherwise it'll have to get each one individually
                    {
                        if (!string.IsNullOrEmpty(ids))
                        {
                            descriptions.Add(tableInfo["TableName"].ToString().ToLower(), Navigation.GetItemNames(tableInfo["TableName"].ToString(), conn, tableInfo, ids));
                        }
                    }
                }
            }

            bool isString;
            bool isString21;
            foreach (DataRow trackRow in dtTracking.Rows)
            {
                i += 1; // Test code to keep track of row when breaking
                var itemTrackedInfo = tablesInfo.Select("TableName='" + trackRow["TrackedTable"].ToString() + "'");

                if (idIsString.TryGetValue(trackRow["TrackedTable"].ToString().ToLower(), out isString))
                {
                    track = new TrackingTransaction(trackRow["TrackedTable"].ToString(), isString);
                }
                else if (itemTrackedInfo.Count() > 0)
                {
                    track = new TrackingTransaction(itemTrackedInfo[0], passport);
                }
                else
                {
                    track = new TrackingTransaction(trackRow["TrackedTable"].ToString(), passport);
                }

                try
                {
                    track.TransactionDate = Conversions.ToDate(trackRow["TransactionDateTime"]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    track.TransactionDate = Conversions.ToDate("1/1/1980");
                }

                try
                {
                    track.Out = Conversions.ToBoolean(trackRow["Out"]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    track.Out = false;
                }

                try
                {
                    track.DateDue = Conversions.ToDate(trackRow["DateDue"]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    track.DateDue = default;
                }

                // If trackRow.Table.Columns.Contains("IsActualScan") Then  'Column doesn't exist in the query that creates this table; look into this?
                // Try
                // track.ActualScan = CBool(trackRow("IsActualScan"))
                // Catch ex As Exception
                // track.ActualScan = False
                // End Try
                // Else
                track.ActualScan = false;
                // End If

                track.UserName = trackRow["TrackingDisplayName"].ToString(); // CStr(IIf(IsDBNull(trackRow("userName")), "", trackRow("userName")))
                track.ID = trackRow["TrackedTableID"].ToString();
                track.TrackingAdditionalField1 = trackRow["TrackingAdditionalField1"].ToString();
                track.TrackingAdditionalField2 = trackRow["TrackingAdditionalField2"].ToString();
                foreach (DataRow contRow in dtContainers.Rows)
                {
                    try
                    {
                        if (!(trackRow[contRow["TrackingStatusFieldName"].ToString()] is DBNull))
                        {
                            var itemNameInfo = tablesInfo.Select("TableName='" + contRow["tableName"].ToString() + "'");

                            if (itemNameInfo.Count() > 0)
                            {

                                if (idIsString.TryGetValue(contRow["tableName"].ToString().ToLower(), out isString21))
                                {
                                    container = new Container(contRow["tableName"].ToString(), isString21);
                                }
                                else
                                {
                                    container = new Container(itemNameInfo[0], passport);
                                }

                                var itemNames = new DataTable();

                                if (descriptions.TryGetValue(contRow["TableName"].ToString().ToLower(), out itemNames))
                                {
                                    var itemName = itemNames.Select("id='" + trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString().Replace("'", "''") + "'");

                                    if (itemName.Count() > 0)
                                    {
                                        container.Name = Navigation.ItemNamesRowToItemName(itemName[0], itemNameInfo[0], passport, trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString());
                                    }
                                    else
                                    {
                                        container.Name = Navigation.GetItemName(contRow["TableName"].ToString(), trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString(), passport, conn, itemNameInfo[0]);
                                    }
                                }
                                else
                                {
                                    container.Name = Navigation.GetItemName(contRow["TableName"].ToString(), trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString(), passport, conn, itemNameInfo[0]);
                                }
                            }
                            else
                            {
                                container = new Container(contRow["tableName"].ToString(), passport);
                                container.Name = Navigation.GetItemName(contRow["TableName"].ToString(), trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString(), passport, conn);
                            }
                            if (!ReferenceEquals(contRow["TrackingMailStopFieldName"], DBNull.Value) && trackRow.Table.Columns.Contains(contRow["TrackingMailStopFieldName"].ToString()))
                            {
                                container.MailStop = trackRow[contRow["TrackingMailStopFieldName"].ToString()].ToString();
                            }
                            else
                            {
                                container.MailStop = string.Empty;
                            }
                            if (!ReferenceEquals(contRow["TrackingPhoneFieldName"], DBNull.Value) && trackRow.Table.Columns.Contains(contRow["TrackingPhoneFieldName"].ToString()))
                            {
                                container.Phone = trackRow[contRow["TrackingPhoneFieldName"].ToString()].ToString();
                            }
                            else
                            {
                                container.Phone = string.Empty;
                            }
                            container.Level = Conversions.ToInteger(contRow["TrackingTable"]);
                            container.ID = trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString();
                            container.OutType = Conversions.ToInteger(contRow["OutTable"]);
                            track.Containers.Add(container);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        // Field doesn't exist in standard report, no need to do anything
                    }
                }

                lstTrack.Add(track);
            }

            return lstTrack;
        }

        private static List<TrackingTransaction> BuildTracking(DataTable dtTracking, Passport passport,
            [Optional, DefaultParameterValue(null)] DataTable dtContainers,
            [Optional, DefaultParameterValue(null)] DataTable dtTrackedTables,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, string> idsByTable,
            [Optional, DefaultParameterValue(null)] ref Dictionary<string, DataTable> descriptions)
        {
            using (var conn = passport.Connection())
            {
                return BuildTracking(dtTracking, passport, conn, dtContainers, dtTrackedTables, ref idsByTable, ref descriptions);
            }
        }

        public static DataRow GetLocationTableInfo(SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE [TrackingTable]=1", conn))
            {

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    throw new NullReferenceException("No Location table found.");
                }
            }
        }

        public static DataRow GetLocationTableInfo(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetLocationTableInfo(conn);
            }
        }

        public static DataRow GetRequestorTableInfo(SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE [TrackingTable]=2", conn))
            {

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    // Modified By hemin
                    // Throw New NullReferenceException("No requestor table found.")
                    throw new NullReferenceException("No requestor table found");
                }
            }
        }

        public static DataRow GetRequestorTableInfo(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetRequestorTableInfo(conn);
            }
        }

        public static string GetLocationsTableName(SqlConnection conn)
        {
            using (var cmdRequestor = new SqlCommand("SELECT [tableName] FROM [Tables] WHERE [TrackingTable]=1", conn))
            {
                return cmdRequestor.ExecuteScalar().ToString();
            }
        }

        public async static Task<string> GetLocationsTableNameAsync(string connstring)
        {
            using (var conn = new SqlConnection(connstring))
            {
                await conn.OpenAsync();
                using (var cmdRequestor = new SqlCommand("SELECT [tableName] FROM [Tables] WHERE [TrackingTable]=1", conn))
                {
                    return cmdRequestor.ExecuteScalar().ToString();
                }
            }
        }

        public static string GetLocationsTableName(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetLocationsTableName(conn);
            }
        }

        public static string GetRequestorTableName(SqlConnection conn)
        {
            using (var cmdRequestor = new SqlCommand("SELECT [tableName] FROM [Tables] WHERE [TrackingTable]=2", conn))
            {
                return cmdRequestor.ExecuteScalar().ToString();
            }
        }

        public static string GetRequestorTableName(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetRequestorTableName(conn);
            }
        }

        public static DataRow GetArchiveLocationTable(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT * FROM [Tables] WHERE TrackingTable=1 AND NOT ArchiveLocationField IS NULL", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        public static async Task<Dictionary<string, string>> GetArchiveLocationsAsync(Passport passport)
        {
            var archiveLocationTable = GetArchiveLocationTable(passport);
            var list = new Dictionary<string, string>();
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("SELECT * FROM [" + archiveLocationTable["TableName"].ToString() + "] WHERE [" + archiveLocationTable["ArchiveLocationField"].ToString() + "]=1", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            string id = row[Navigation.MakeSimpleField(archiveLocationTable["idFieldName"].ToString())].ToString();
                            list.Add(id, Navigation.ItemRowToItemName(row, archiveLocationTable, passport, id));

                        }
                    }
                }
            }
            return list;
        }
        public static Dictionary<string, string> GetArchiveLocations(Passport passport)
        {
            var archiveLocationTable = GetArchiveLocationTable(passport);
            var list = new Dictionary<string, string>();
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT * FROM [" + archiveLocationTable["TableName"].ToString() + "] WHERE [" + archiveLocationTable["ArchiveLocationField"].ToString() + "]=1", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            string id = row[Navigation.MakeSimpleField(archiveLocationTable["idFieldName"].ToString())].ToString();
                            list.Add(id, Navigation.ItemRowToItemName(row, archiveLocationTable, passport, id));

                        }
                    }
                }
            }
            return list;
        }

        public static DataRow GetInactiveLocationTable(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT * FROM [Tables] WHERE TrackingTable=1 AND NOT InactiveLocationField IS NULL", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static Dictionary<string, string> GetInactiveLocations(Passport passport)
        {
            var inactiveLocationTable = GetInactiveLocationTable(passport);
            var list = new Dictionary<string, string>();
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT * FROM [" + inactiveLocationTable["TableName"].ToString() + "] WHERE [" + inactiveLocationTable["InactiveLocationField"].ToString() + "]=1", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            string id = row[Navigation.MakeSimpleField(inactiveLocationTable["idFieldName"].ToString())].ToString();
                            list.Add(id, Navigation.ItemRowToItemName(row, inactiveLocationTable, passport, id));
                        }
                    }
                }
            }
            return list;
        }

        public async static Task<Dictionary<string, string>> GetInactiveLocationsAsync(Passport passport)
        {
            var inactiveLocationTable = GetInactiveLocationTable(passport);
            var list = new Dictionary<string, string>();
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("SELECT * FROM [" + inactiveLocationTable["TableName"].ToString() + "] WHERE [" + inactiveLocationTable["InactiveLocationField"].ToString() + "]=1", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            string id = row[Navigation.MakeSimpleField(inactiveLocationTable["idFieldName"].ToString())].ToString();
                            list.Add(id, Navigation.ItemRowToItemName(row, inactiveLocationTable, passport, id));
                        }
                    }
                }
            }
            return list;
        }
        public static DataTable GetTrackingContainers(string containerTableName, string find, bool requestOnBehalf, Passport passport, SqlConnection conn)
        {
            var tblInfo = Navigation.GetTableInfo(containerTableName, conn);
            int trackinglevel = Conversions.ToInteger(tblInfo["TrackingTable"]);
            string sql = "SELECT ";
            if (trackinglevel > 2)
                sql = "SELECT TOP 50 ";

            if (string.IsNullOrEmpty(find))
            {
                if (!string.IsNullOrEmpty(tblInfo["DescFieldNameOne"].ToString()))
                {
                    sql += tblInfo["IdFieldName"].ToString() + " FROM " + containerTableName + " ORDER BY " + tblInfo["DescFieldNameOne"].ToString();
                }
                else
                {
                    sql += tblInfo["IdFieldName"].ToString() + " FROM " + containerTableName + " ORDER BY " + tblInfo["IdFieldName"].ToString();
                }
            }
            else if (!string.IsNullOrEmpty(tblInfo["DescFieldNameOne"].ToString()) & !string.IsNullOrEmpty(tblInfo["DescFieldNameTwo"].ToString()))
            {
                sql += tblInfo["IdFieldName"].ToString() + " FROM " + containerTableName + " WHERE (CAST(" + tblInfo["IdFieldName"].ToString() + " AS VARCHAR) LIKE '%" + find + "%' OR " + tblInfo["DescFieldNameOne"].ToString() + " LIKE '%" + find + "%' OR " + tblInfo["DescFieldNameTwo"].ToString() + " LIKE '%" + find + "%') ORDER BY " + tblInfo["DescFieldNameOne"].ToString() + "," + tblInfo["DescFieldNameTwo"].ToString();
            }
            else if (!string.IsNullOrEmpty(tblInfo["DescFieldNameOne"].ToString()))
            {
                sql += tblInfo["IdFieldName"].ToString() + " FROM " + containerTableName + " WHERE (CAST(" + tblInfo["IdFieldName"].ToString() + " AS VARCHAR) LIKE '%" + find + "%' OR " + tblInfo["DescFieldNameOne"].ToString() + " LIKE '%" + find + "%') ORDER BY " + tblInfo["DescFieldNameOne"].ToString();
            }
            else if (!string.IsNullOrEmpty(tblInfo["DescFieldNameTwo"].ToString()))
            {
                sql += tblInfo["IdFieldName"].ToString() + " FROM " + containerTableName + " WHERE (CAST(" + tblInfo["IdFieldName"].ToString() + " AS VARCHAR) LIKE '%" + find + "%' OR " + tblInfo["DescFieldNameTwo"].ToString() + " LIKE '%" + find + "%') ORDER BY " + tblInfo["DescFieldNameTwo"].ToString();
            }
            else
            {
                sql += tblInfo["IdFieldName"].ToString() + " FROM " + containerTableName + " WHERE (CAST(" + tblInfo["IdFieldName"].ToString() + " AS VARCHAR) LIKE '%" + find + "%') ORDER BY " + tblInfo["IdFieldName"].ToString();
            }

            if (!string.IsNullOrEmpty(tblInfo["TrackingActiveFieldName"].ToString()))
            {
                if (sql.ToLower().Contains("where"))
                {
                    sql = Strings.Replace(sql, " ORDER BY ", string.Format(" AND [{0}] = 1 ORDER BY ", tblInfo["TrackingActiveFieldName"].ToString()), 1, Compare: CompareMethod.Text);
                }
                else
                {
                    sql = Strings.Replace(sql, " ORDER BY ", string.Format(" WHERE [{0}] = 1 ORDER BY ", tblInfo["TrackingActiveFieldName"].ToString()), 1, Compare: CompareMethod.Text);
                }
            }

            if (trackinglevel == 2 && !string.IsNullOrEmpty(tblInfo["OperatorsIdField"].ToString()) && !requestOnBehalf)
            {
                var user = new User(passport, true);

                if (sql.ToLower().Contains("where"))
                {
                    sql = Strings.Replace(sql, " ORDER BY ", string.Format(" AND [{0}] = '{1}' ORDER BY ", tblInfo["OperatorsIdField"].ToString(), user.UserName.Replace("'", "''")), 1, Compare: CompareMethod.Text);
                }
                else
                {
                    sql = Strings.Replace(sql, " ORDER BY ", string.Format(" WHERE [{0}] = '{1}' ORDER BY ", tblInfo["OperatorsIdField"].ToString(), user.UserName.Replace("'", "''")), 1, Compare: CompareMethod.Text);
                }
            }

            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetTrackingContainers(string containerTableName, string find, bool requestOnBehalf, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackingContainers(containerTableName, find, requestOnBehalf, passport, conn);
            }
        }

        public static DataTable GetRequestInfo(Passport passport, string status = "")
        {
            string requestSQL = "SELECT * FROM SLRequestor";
            if (!string.IsNullOrEmpty(status))
                requestSQL += " WHERE Status = @status";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(requestSQL, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@status", status));

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static string GetTrackedItemLocation(Passport passport, string tableName, string tableID)
        {
            string sql = "SELECT * FROM TrackingStatus WHERE TrackedTable = @tablename AND TrackedTableID = @tableid";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@tablename", tableName));
                    cmd.Parameters.Add(new SqlParameter("@tableID", tableID));

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        var dtLocations = GetTrackedItemLocations(passport);

                        foreach (DataRow dr in dtLocations.Rows)
                        {
                            if (!(dt.AsEnumerable().ElementAtOrDefault(0)[dr["TrackingStatusFieldName"].ToString()] is DBNull))
                                return dr["TableName"].ToString();
                        }

                        return string.Empty;
                    }
                }
            }
        }

        public static string GetTrackedItemLocationID(Passport passport, string tableName, string tableID)
        {
            string sql = "SELECT * FROM TrackingStatus WHERE TrackedTable = @tablename AND TrackedTableID = @tableid";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@tablename", tableName));
                    cmd.Parameters.Add(new SqlParameter("@tableID", tableID));

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        var dtLocations = GetTrackedItemLocations(passport);

                        foreach (DataRow dr in dtLocations.Rows)
                        {
                            if (!(dt.AsEnumerable().ElementAtOrDefault(0)[dr["TrackingStatusFieldName"].ToString()] is DBNull))
                                return dt.AsEnumerable().ElementAtOrDefault(0)[dr["TrackingStatusFieldName"].ToString()].ToString();
                        }

                        return string.Empty;
                    }
                }
            }
        }


        public static string GetTrackedItemLocationDescription(Passport passport, string tableName, string tableID)
        {
            string sql = "SELECT * FROM TrackingStatus WHERE TrackedTable = @tablename AND TrackedTableID = @tableid";
            string location = string.Empty;
            string locationId = string.Empty;
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@tablename", tableName));
                    cmd.Parameters.Add(new SqlParameter("@tableID", tableID));

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        var dtLocations = GetTrackedItemLocations(passport);

                        foreach (DataRow dr in dtLocations.Rows)
                        {
                            if (!(dt.AsEnumerable().ElementAtOrDefault(0)[dr["TrackingStatusFieldName"].ToString()] is DBNull))
                            {
                                location = dr["TableName"].ToString();
                                locationId = dt.AsEnumerable().ElementAtOrDefault(0)[dr["TrackingStatusFieldName"].ToString()].ToString();
                                return Navigation.GetItemName(location, locationId, passport);
                            }

                        }

                        return string.Empty;
                    }
                }
            }
        }

        public static DataTable GetTrackedItemLocations(SqlConnection conn)
        {
            string sql = "SELECT TableName, TrackingTable, TrackingStatusFieldName, OutTable FROM Tables WHERE (NOT (TrackingStatusFieldName IS NULL)) ORDER BY TrackingTable";

            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetTrackedItemLocations(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackedItemLocations(conn);
            }
        }

        public static DataTable GetTrackingSelectData(SqlConnection conn)
        {
            string sql = "SELECT * FROM SLTrackingSelectData ORDER BY SLTrackingSelectDataId";

            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetTrackingStatusFields(SqlConnection conn)
        {
            string sql = "SELECT DISTINCT TrackingStatusFieldName, TrackingTable FROM Tables WHERE(NOT (TrackingStatusFieldName IS NULL)) ORDER BY TrackingStatusFieldName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetTrackingStatusFields(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackingStatusFields(conn);
            }
        }

        public static DataTable GetTrackableTables(SqlConnection conn)
        {
            string sql = " SELECT TBL.*, case trackingtable when 0 then 9999 else TrackingTable end as o " + "FROM [dbo].[Tables] TBL INNER JOIN [dbo].[SecureObject] SO on SO.Name = TBL.TableName " + "INNER JOIN [dbo].[SecureObjectPermission] SOP on SO.SecureObjectID = SOP.SecureObjectID " + "WHERE(SOP.PermissionID = 8 And SOP.GroupID = 0 And SOP.SecureObjectID <> 0 And SOP.SecureObjectID <> 2 And tbl.TrackingTable > 0) " + "UNION " + "SELECT TBL.*,case trackingtable when 0 then 9999 else TrackingTable end as o FROM [dbo].[Tables] TBL " + " INNER JOIN [dbo].[SecureObject] SO on SO.Name = TBL.TableName " + "INNER JOIN [dbo].[SecureObjectPermission] SOP on SO.SecureObjectID = SOP.SecureObjectID " + "WHERE(SOP.PermissionID = 8 And SOP.GroupID = 0 And SOP.SecureObjectID <> 0 And SOP.SecureObjectID <> 2 And tbl.TrackingTable = 0) " + "ORDER BY o, tbl.TableName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetTrackableTables(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackableTables(conn);
            }
        }

        public async static Task<DataTable> GetTrackableTablesAsync(Passport passport)
        {
            string sql = " SELECT TBL.*, case trackingtable when 0 then 9999 else TrackingTable end as o " + "FROM [dbo].[Tables] TBL INNER JOIN [dbo].[SecureObject] SO on SO.Name = TBL.TableName " + "INNER JOIN [dbo].[SecureObjectPermission] SOP on SO.SecureObjectID = SOP.SecureObjectID " + "WHERE(SOP.PermissionID = 8 And SOP.GroupID = 0 And SOP.SecureObjectID <> 0 And SOP.SecureObjectID <> 2 And tbl.TrackingTable > 0) " + "UNION " + "SELECT TBL.*,case trackingtable when 0 then 9999 else TrackingTable end as o FROM [dbo].[Tables] TBL " + " INNER JOIN [dbo].[SecureObject] SO on SO.Name = TBL.TableName " + "INNER JOIN [dbo].[SecureObjectPermission] SOP on SO.SecureObjectID = SOP.SecureObjectID " + "WHERE(SOP.PermissionID = 8 And SOP.GroupID = 0 And SOP.SecureObjectID <> 0 And SOP.SecureObjectID <> 2 And tbl.TrackingTable = 0) " + "ORDER BY o, tbl.TableName";
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static DataTable GetTrackedTables(SqlConnection conn)
        {
            string strQuery = " SELECT t.TableName AS [TrackedTable] " + "FROM [Tables] t " + "INNER JOIN [SecureObject] o ON o.Name = t.TableName AND o.SecureObjectTypeID = 2 AND o.BaseID = 2 " + "INNER JOIN [SecureObjectPermission] p ON o.SecureObjectID = p.SecureObjectID AND p.PermissionID = 8 AND p.GroupID = 0 " + "ORDER BY t.TableName";



            using (var cmd = new SqlCommand(strQuery, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetTrackedTables(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackedTables(conn);
            }
        }

        public static void SetRetentionInactiveFlag(DataRow tableInfo, DataRow currentRow, string retentionCode, Passport passport)
        {
            bool activeStorage = true;
            DateTime inactiveDate = default;
            string inactivityEventType = string.Empty;
            bool idIsString = false;
            List<TrackingTransaction> trackingStatus = null;

            if (string.IsNullOrEmpty(retentionCode))
            {
                if (!string.IsNullOrEmpty(tableInfo["RetentionFieldName"].ToString()))
                {
                    try
                    {
                        retentionCode = currentRow[tableInfo["RetentionFieldName"].ToString()].ToString();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        retentionCode = string.Empty;
                    }
                }
            }

            if (string.IsNullOrEmpty(retentionCode))
                return;

            using (var conn = passport.Connection())
            {
                var codeRow = Retention.GetRetentionCode(retentionCode, conn);
                idIsString = Navigation.FieldIsAString(tableInfo, passport);
                inactiveDate = Retention.CalcRetentionInactiveDate(tableInfo, currentRow, retentionCode, passport, ref inactivityEventType, codeRow);

                if (inactivityEventType == "Date Last Tracked")
                {
                    Dictionary<string, string> argidsByTable = null;
                    trackingStatus = Tracking.GetTrackableStatus(tableInfo["TableName"].ToString(), Navigation.GetPrimaryKeyFieldName(tableInfo), passport, conn, idsByTable: ref argidsByTable);
                    if (trackingStatus is not null && trackingStatus.Count > 0)
                        inactiveDate = trackingStatus[0].TransactionDate;
                }

                if (inactiveDate != default)
                {
                    inactiveDate = Navigation.ApplyYearEndToDate(inactiveDate, Navigation.CBoolean(codeRow["InactivityForceToEndOfYear"]), Conversions.ToInteger(Navigation.GetSystemSetting("RetentionYearEnd", passport)));
                    var locations = Navigation.GetTrackingTablesAsDataTable(passport);

                    if (locations.Rows.Count > 0)
                    {
                        var location = locations.AsEnumerable().ElementAtOrDefault(0);

                        if (!string.IsNullOrEmpty(location["InactiveLocationField"].ToString()) & !string.IsNullOrEmpty(location["TrackingStatusFieldName"].ToString()))
                        {
                            if ((trackingStatus is null || trackingStatus.Count == 0) & inactivityEventType != "Date Last Tracked")
                            {
                                Dictionary<string, string> argidsByTable1 = null;
                                trackingStatus = GetTrackableStatus(tableInfo["TableName"].ToString(), currentRow[Navigation.GetPrimaryKeyFieldName(tableInfo)].ToString(), passport, conn, idsByTable: ref argidsByTable1);
                            }

                            if (trackingStatus is not null && trackingStatus.Count > 0)
                            {
                                var container = trackingStatus[0].FindContainer(location["TableName"].ToString());

                                if (container is not null) // can be nothing if not at a location- at an employee, etc
                                {
                                    string sql = "SELECT [" + location["InactiveLocationField"].ToString() + "] FROM [" + location["TableName"].ToString() + "] WHERE [" + location["IdFieldName"].ToString().Replace(".", "].[") + "] = @Id";
                                    using (var cmd = new SqlCommand(sql, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@Id", container.ID);

                                        using (var da = new SqlDataAdapter(cmd))
                                        {
                                            var dt = new DataTable();
                                            da.Fill(dt);
                                            if (dt.Rows.Count > 0)
                                                activeStorage = !Conversions.ToBoolean(dt.AsEnumerable().ElementAtOrDefault(0)[location["InactiveLocationField"].ToString()]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (inactiveDate.CompareTo(DateTime.Now) < 0 | !activeStorage) // set flag
                    {
                        if (!Navigation.CBoolean(currentRow["%slRetentionInactive"]))
                        {
                            Navigation.UpdateSingleField(tableInfo["TableName"].ToString(), currentRow[Navigation.GetPrimaryKeyFieldName(tableInfo)].ToString(), "%slRetentionInactive", "1", conn);
                        }
                    }
                    else if (Navigation.CBoolean(currentRow["%slRetentionInactive"]))
                    {
                        Navigation.UpdateSingleField(tableInfo["TableName"].ToString(), currentRow[Navigation.GetPrimaryKeyFieldName(tableInfo)].ToString(), "%slRetentionInactive", "0", conn);
                    }
                }
            }
        }
    }

    public class TrackingTablesModel
    {
        private int _TrackingTable;
        public int TrackingTable
        {
            get
            {
                return _TrackingTable;
            }
            set
            {
                _TrackingTable = value;
            }
        }

        public string TableName
        {
            get
            {
                return m_TableName;
            }
            set
            {
                m_TableName = value;
            }
        }
        private string m_TableName;

        public string TrackingStatusFieldName
        {
            get
            {
                return m_TrackingStatusFieldName;
            }
            set
            {
                m_TrackingStatusFieldName = value;
            }
        }
        private string m_TrackingStatusFieldName;

        private short? _OutTable;
        public short? OutTable
        {
            get
            {
                return _OutTable;
            }
            set
            {
                _OutTable = value;
            }
        }

        public string TrackingOutFieldName
        {
            get
            {
                return m_TrackingOutFieldName;
            }
            set
            {
                m_TrackingOutFieldName = value;
            }
        }
        private string m_TrackingOutFieldName;

        public string TrackingRequestableFieldName
        {
            get
            {
                return m_TrackingRequestableFieldName;
            }
            set
            {
                m_TrackingRequestableFieldName = value;
            }
        }
        private string m_TrackingRequestableFieldName;

        public string IdFieldName
        {
            get
            {
                return m_IdFieldName;
            }
            set
            {
                m_IdFieldName = value;
            }
        }
        private string m_IdFieldName;
    }


    // Reporting develop for MVC model| by Moti Mashiah
    public class TrackingReport
    {
        public TrackingReport(Passport passport, string tablename, string tableid)
        {
            _dtContainers = new DataTable();
            _dtTrackedTables = new DataTable();
            // _httpcontext = httpcontext
            _passport = passport;
            _tableName = tablename;
            _tableid = tableid;
            _idsByTable = new Dictionary<string, string>();
            _idIsString = new Dictionary<string, bool>();
            _descriptions = new Dictionary<string, DataTable>();
        }
        // Private Property _httpcontext As HttpContext
        private DataTable _dtContainers { get; set; }
        private DataTable _dtTrackedTables { get; set; }
        private Passport _passport { get; set; }
        private string _tableName { get; set; }
        private string _tableid { get; set; }
        private Dictionary<string, string> _idsByTable { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, bool> _idIsString { get; set; }
        private Dictionary<string, DataTable> _descriptions { get; set; }

        public List<TrackingTransaction> GetTrackableHistory()
        {
            _tableid = Navigation.PrepPad(_tableName, _tableid, _passport);
            if (_tableName.Length == 0 | _tableid.Length == 0)
                return null;
            var query = new Query(_passport);

            string sql = "SELECT TrackingHistory.*, ISNULL(SecureUser.DisplayName, " + ShowUserName("TrackingHistory.UserName", _passport) + ") AS TrackingDisplayName " + "FROM TrackingHistory LEFT JOIN SecureUser ON TrackingHistory.UserName = SecureUser.UserName " + "WHERE TrackedTable = @TrackedTable AND TrackedTableId = @TrackedTableId";
            string orderClause = " ORDER BY TransactionDateTime DESC";
            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", _tableName);
                    cmd.Parameters.AddWithValue("@TrackedTableId", _tableid);
                    cmd.CommandText = sql + orderClause;

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        return BuildTracking(dtTracking);
                    }
                }
            }
        }
        public DataTable GetTrackableHistoryCount()
        {
            _tableid = Navigation.PrepPad(_tableName, _tableid, _passport);
            if (_tableName.Length == 0 | _tableid.Length == 0)
                return null;
            var query = new Query(_passport);

            string sql = "SELECT count(*) FROM TrackingHistory LEFT JOIN SecureUser ON TrackingHistory.UserName = SecureUser.UserName " + "WHERE TrackedTable = @TrackedTable AND TrackedTableId = @TrackedTableId";
            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", _tableName);
                    cmd.Parameters.AddWithValue("@TrackedTableId", _tableid);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        return dtTracking;
                    }
                }
            }
        }
        public async Task<DataTable> GetTrackableHistoryCountAsync()
        {
            _tableid = Navigation.PrepPad(_tableName, _tableid, _passport);
            if (_tableName.Length == 0 | _tableid.Length == 0)
                return null;
            var query = new Query(_passport);

            string sql = "SELECT count(*) FROM TrackingHistory LEFT JOIN SecureUser ON TrackingHistory.UserName = SecureUser.UserName " + "WHERE TrackedTable = @TrackedTable AND TrackedTableId = @TrackedTableId";
            using (var conn = new SqlConnection(_passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", _tableName);
                    cmd.Parameters.AddWithValue("@TrackedTableId", _tableid);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        return dtTracking;
                    }
                }
            }
        }

        public List<TrackingTransaction> GetTrackableHistoryPaging(int PageNumber, int PerPageRecord)
        {
            _tableid = Navigation.PrepPad(_tableName, _tableid, _passport);
            if (_tableName.Length == 0 | _tableid.Length == 0)
                return null;
            var query = new Query(_passport);

            string sql = "SELECT TrackingHistory.*, ISNULL(SecureUser.DisplayName, " + ShowUserName("TrackingHistory.UserName", _passport) + ") AS TrackingDisplayName " + "FROM TrackingHistory LEFT JOIN SecureUser ON TrackingHistory.UserName = SecureUser.UserName " + "WHERE TrackedTable = @TrackedTable AND TrackedTableId = @TrackedTableId";
            sql += " ORDER BY TransactionDateTime DESC " + Query.QueryPaging(PageNumber, PerPageRecord);

            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackedTable", _tableName);
                    cmd.Parameters.AddWithValue("@TrackedTableId", _tableid);
                    // cmd.CommandText = sql & orderClause

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtTracking = new DataTable();
                        da.Fill(dtTracking);
                        return BuildTracking(dtTracking);
                    }
                }
            }
        }
        private List<TrackingTransaction> BuildTracking(DataTable dtTracking)
        {
            if (dtTracking.Rows.Count == 0)
                return null;
            var lstTrack = new List<TrackingTransaction>();
            TrackingTransaction track;
            var tablesInfo = GetTrackedTableInfo();
            loopThroughTableInfo(tablesInfo, dtTracking);
            int i = 0;

            bool isString;
            foreach (DataRow trackRow in dtTracking.Rows)
            {
                i += 1; // Test code to keep track of row when breaking
                var itemTrackedInfo = tablesInfo.Select("TableName='" + trackRow["TrackedTable"].ToString() + "'");

                if (_idIsString.TryGetValue(trackRow["TrackedTable"].ToString().ToLower(), out isString))
                {
                    track = new TrackingTransaction(trackRow["TrackedTable"].ToString(), isString);
                }
                else if (itemTrackedInfo.Count() > 0)
                {
                    track = new TrackingTransaction(itemTrackedInfo[0], _passport);
                }
                else
                {
                    track = new TrackingTransaction(trackRow["TrackedTable"].ToString(), _passport);
                }

                try
                {
                    track.TransactionDate = Conversions.ToDate(trackRow["TransactionDateTime"]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    track.TransactionDate = Conversions.ToDate("1/1/1980");
                }

                try
                {
                    track.Out = Conversions.ToBoolean(trackRow["Out"]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    track.Out = false;
                }

                try
                {
                    track.DateDue = Conversions.ToDate(trackRow["DateDue"]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    track.DateDue = default;
                }
                track.ActualScan = false;

                track.UserName = trackRow["TrackingDisplayName"].ToString(); // CStr(IIf(IsDBNull(trackRow("userName")), "", trackRow("userName")))
                track.ID = trackRow["TrackedTableID"].ToString();
                track.TrackingAdditionalField1 = trackRow["TrackingAdditionalField1"].ToString();
                track.TrackingAdditionalField2 = trackRow["TrackingAdditionalField2"].ToString();
                loopThroughContainer(trackRow, track, tablesInfo);
                lstTrack.Add(track);
            }

            return lstTrack;
        }
        private void loopThroughTableInfo(DataTable tablesInfo, DataTable dtTracking)
        {
            foreach (DataRow tableInfoRow in tablesInfo.Rows)
            {
                _idIsString.Add(tableInfoRow["TableName"].ToString().ToLower(), Navigation.FieldIsAString(tableInfoRow, _passport));
                _idsByTable.Add(tableInfoRow["TableName"].ToString().ToLower(), "");
            }
            foreach (DataRow trackRow in dtTracking.Rows)
            {
                if (_idsByTable.ContainsKey(trackRow["TrackedTable"].ToString().ToLower()))
                {
                    if (string.IsNullOrEmpty(_idsByTable[trackRow["TrackedTable"].ToString().ToLower()]))
                    {
                        _idsByTable[trackRow["TrackedTable"].ToString().ToLower()] += "'" + trackRow["TrackedTableID"].ToString().Replace("'", "''") + "'";
                    }
                    else
                    {
                        _idsByTable[trackRow["TrackedTable"].ToString().ToLower()] += ",'" + trackRow["TrackedTableID"].ToString().Replace("'", "''") + "'";
                    }
                }
            }

            _descriptions = new Dictionary<string, DataTable>();
            foreach (DataRow tableInfo in tablesInfo.Rows)
            {
                string ids = "";
                if (_idsByTable.TryGetValue(tableInfo["TableName"].ToString().ToLower(), out ids)) // If we have ids, prepopulate; otherwise it'll have to get each one individually
                {
                    if (!string.IsNullOrEmpty(ids))
                    {
                        _descriptions.Add(tableInfo["TableName"].ToString().ToLower(), Navigation.GetItemNames(tableInfo["TableName"].ToString(), _passport.Connection(), tableInfo, ids));
                    }
                }
            }
        }
        private void loopThroughContainer(DataRow trackRow, TrackingTransaction track, DataTable tablesInfo)
        {
            Container container;
            bool isString2;
            foreach (DataRow contRow in _dtContainers.Rows)
            {
                try
                {
                    if (!(trackRow[contRow["TrackingStatusFieldName"].ToString()] is DBNull))
                    {
                        var itemNameInfo = tablesInfo.Select("TableName='" + contRow["tableName"].ToString() + "'");

                        if (itemNameInfo.Count() > 0)
                        {

                            if (_idIsString.TryGetValue(contRow["tableName"].ToString().ToLower(), out isString2))
                            {
                                container = new Container(contRow["tableName"].ToString(), isString2);
                            }
                            else
                            {
                                container = new Container(itemNameInfo[0], _passport);
                            }

                            var itemNames = new DataTable();

                            if (_descriptions.TryGetValue(contRow["TableName"].ToString().ToLower(), out itemNames))
                            {
                                var itemName = itemNames.Select("id='" + trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString().Replace("'", "''") + "'");

                                if (itemName.Count() > 0)
                                {
                                    container.Name = Navigation.ItemNamesRowToItemName(itemName[0], itemNameInfo[0], _passport, trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString());
                                }
                                else
                                {
                                    container.Name = Navigation.GetItemName(contRow["TableName"].ToString(), trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString(), _passport, _passport.Connection(), itemNameInfo[0]);
                                }
                            }
                            else
                            {
                                container.Name = Navigation.GetItemName(contRow["TableName"].ToString(), trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString(), _passport, _passport.Connection(), itemNameInfo[0]);
                            }
                        }
                        else
                        {
                            container = new Container(contRow["tableName"].ToString(), _passport);
                            container.Name = Navigation.GetItemName(contRow["TableName"].ToString(), trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString(), _passport, _passport.Connection());
                        }
                        if (!ReferenceEquals(contRow["TrackingMailStopFieldName"], DBNull.Value) && trackRow.Table.Columns.Contains(contRow["TrackingMailStopFieldName"].ToString()))
                        {
                            container.MailStop = trackRow[contRow["TrackingMailStopFieldName"].ToString()].ToString();
                        }
                        else
                        {
                            container.MailStop = string.Empty;
                        }
                        if (!ReferenceEquals(contRow["TrackingPhoneFieldName"], DBNull.Value) && trackRow.Table.Columns.Contains(contRow["TrackingPhoneFieldName"].ToString()))
                        {
                            container.Phone = trackRow[contRow["TrackingPhoneFieldName"].ToString()].ToString();
                        }
                        else
                        {
                            container.Phone = string.Empty;
                        }
                        container.Level = Conversions.ToInteger(contRow["TrackingTable"]);
                        container.ID = trackRow[contRow["TrackingStatusFieldName"].ToString()].ToString();
                        container.OutType = Conversions.ToInteger(contRow["OutTable"]);
                        track.Containers.Add(container);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Field doesn't exist in standard report, no need to do anything
                }
            }
        }
        private DataTable GetTrackedTableInfo()
        {
            _dtTrackedTables = GetTrackedTables();
            _dtContainers = GetTrackingContainerTypes();

            var trackingTableList = new List<string>();

            foreach (DataRow contRow in _dtContainers.Rows)
                trackingTableList.Add(contRow["TableName"].ToString());
            foreach (DataRow trackRow in _dtTrackedTables.Rows)
            {
                if (!trackingTableList.Contains(trackRow["TrackedTable"].ToString()))
                {
                    trackingTableList.Add(trackRow["TrackedTable"].ToString());
                }
            }
            return Navigation.GetMultipleTableInfo(trackingTableList, _passport.Connection());
        }
        private DataTable GetTrackedTables()
        {
            string strQuery = " SELECT t.TableName AS [TrackedTable] " + "FROM [Tables] t " + "INNER JOIN [SecureObject] o ON o.Name = t.TableName AND o.SecureObjectTypeID = 2 AND o.BaseID = 2 " + "INNER JOIN [SecureObjectPermission] p ON o.SecureObjectID = p.SecureObjectID AND p.PermissionID = 8 AND p.GroupID = 0 " + "ORDER BY t.TableName";



            using (var cmd = new SqlCommand(strQuery, _passport.Connection()))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        private DataTable GetTrackingContainerTypes()
        {
            string sql = "SELECT TableName, TrackingTable, IdFieldName, TrackingStatusFieldName, OutTable, BarCodePrefix, DescFieldNameOne, " + "DescFieldNameTwo, DescFieldPrefixOne, DescFieldPrefixTwo, [TrackingPhoneFieldName], [TrackingMailStopFieldName], " + "[TrackingRequestableFieldName], UserName " + "FROM [Tables] " + "WHERE (Not (TrackingStatusFieldName Is NULL)) " + "  AND TableName NOT IN (SELECT CAST(ItemValue AS VARCHAR(30)) AS ItemValue FROM Settings WHERE Section = 'Transfer' AND Item = 'ExcludeTable') " + "ORDER BY TrackingTable";






            using (var cmd = new SqlCommand(sql, _passport.Connection()))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        private string ShowUserName(string userName, Passport passport)
        {
            if (this.IsMember(Navigation.GetSetting("Security", "ShowUserName", passport), passport))
                return userName;
            return "'&lt;unknown user&gt;'";
        }
        private bool IsMember(string groupList, Passport passport)
        {
            if (string.IsNullOrEmpty(groupList.Trim()))
                return false;
            if (groupList.Contains("-1"))
                return true;

            var user = new User(passport, true);
            var groups = user.GroupMembershipList;
            groupList = string.Format(",{0},", groupList);

            while (groupList.Contains(" "))
                groupList = groupList.Replace(" ", string.Empty);

            foreach (Groups.GroupItem grp in groups)
            {
                if (groupList.Contains(string.Format(",{0},", grp.GroupID.ToString())))
                    return true;
            }

            return false;
        }
        // contact report
        public static DataTable GetContainerContents(string containerType, string containerID, Parameters @params, Passport passport)
        {
            var cmd = new SqlCommand();
            var tableInfo = Navigation.GetTableInfo(containerType, passport);
            var query = new Query(passport);
            string sql = "SELECT * FROM TrackingStatus WHERE " + tableInfo["TrackingStatusFieldName"].ToString() + "=@ContainerID";
            string orderClause = " ORDER BY " + tableInfo["TrackingStatusFieldName"].ToString();
            cmd.CommandText = sql;
            if (Navigation.FieldIsAString(tableInfo, passport))
            {
                cmd.Parameters.AddWithValue("@ContainerID", containerID);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ContainerID", containerID.PadLeft(30, '0'));
            }

            cmd.Connection = passport.Connection();
            // If params.Paged Then
            // params.TotalRows = query.TotalQueryRowCount(cmd)
            // cmd.CommandText = Query.Pagify(params, sql, orderClause)
            // Else
            // cmd.CommandText = sql & orderClause
            // End If
            var dt = new DataTable(Navigation.GetItemName(containerType, containerID, passport));
            var da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }

    }
}