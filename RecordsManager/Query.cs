using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Imaging;
using MSRecordsEngine.Properties;
using Smead.Security;
using static MSRecordsEngine.RecordsManager.FieldValue;



namespace MSRecordsEngine.RecordsManager
{

    public class Query
    {
        private static Passport _passport;
        private List<Parameters> _searchTreeResultList = new List<Parameters>();
        private string _APISearchResults = string.Empty;
        private string _HTMLSearchResults = string.Empty;
        private string _XMLSearchResults = string.Empty;
        private List<Parameters> _results;
        private const string INVALID_PARM_CHARS = "'\"|,-*/=~!^<>()[]{}";
        private HttpContext _httpContext;

        public Query(Passport passport)
        {
            _passport = passport;
        }

        public static string FullTextTemplate
        {
            get
            {
                return Resources.FullTextTemplate;
            }
        }

        public static string FullTextWithoutPreviousVersions
        {
            get
            {
                return Resources.FullTextWithoutPreviousVersions;
            }
        }

        public void Search(Parameters @params)
        {
            _results = new List<Parameters>();
            SearchBarCodes(@params);

            switch (@params.Scope)
            {
                case ScopeEnum.Database:
                    {
                        SearchDatabase(@params);
                        break;
                    }
                case ScopeEnum.Table:
                    {
                        SearchTable(@params);
                        break;
                    }
                case ScopeEnum.Node:
                    {
                        SearchNode(@params);
                        break;
                    }
            }

            this.ProcessRegularSearchResults(_results, @params.ThinClient, @params.RequestedRows, @params.SearchType, @params.IsMVCCall);
            @params.HTMLSearchResults = _HTMLSearchResults;
            @params.XMLSearchResults = _XMLSearchResults;
            @params.APISearchResults = _APISearchResults;
        }
        public List<Parameters> SearchMobile(Parameters @params)
        {
            _results = new List<Parameters>();
            SearchBarCodes(@params);
            switch (@params.Scope)
            {
                case var @case when @case == ScopeEnum.Database:
                    {
                        SearchDatabase(@params);
                        break;
                    }
                case var case1 when case1 == ScopeEnum.Table:
                    {
                        SearchTable(@params);
                        break;
                    }
                case var case2 when case2 == ScopeEnum.Node:
                    {
                        SearchNode(@params);
                        break;
                    }
            }
            // ProcessRegularSearchResultsForMobile(_results, params.ThinClient, params.XMLSearch)
            // params.HTMLSearchResults = _HTMLSearchResults
            // params.XMLSearchResults = _XMLSearchResults
            return _results;
        }

        private string GetPrefix(string text)
        {
            if (string.IsNullOrEmpty(text) || Information.IsNumeric(text))
                return string.Empty;
            text = text.Trim();
            var sb = new StringBuilder();

            for (int i = 0, loopTo = text.Length - 1; i <= loopTo; i++)
            {
                if (Information.IsNumeric(text.Substring(i, 1)))
                    break;
                sb.Append(text.Substring(i, 1));
            }

            return sb.ToString();
        }

        private bool SearchBarCodes(Parameters @params)
        {
            bool SearchBarCodesRet = default;
            if (string.IsNullOrEmpty(@params.Text.Trim()))
                return false;
            string prefix = this.GetPrefix(@params.Text);
            long numericSearchValue;

            try
            {
                if (string.IsNullOrEmpty(prefix))
                {
                    numericSearchValue = Conversions.ToLong(@params.Text.Trim());
                }
                else
                {
                    numericSearchValue = Conversions.ToLong(@params.Text.Trim().Substring(prefix.Length));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            string matchPrefixSQL = string.Format("SELECT TOP 1 [TableName], [idFieldName] FROM [Tables] WHERE [BarCodePrefix]='{0}'", prefix);

            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand(matchPrefixSQL, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 1)
                        {
                            foreach (ViewItem viewItem in Navigation.GetViewsByTableName(dt.Rows[0]["TableName"].ToString(), _passport))
                            {
                                var newParams = new Parameters(viewItem.Id, _passport);
                                newParams.ThinClient = @params.ThinClient;
                                newParams.Text = "";
                                newParams.KeyValue = numericSearchValue.ToString();
                                newParams.Scope = ScopeEnum.Table;
                                newParams.QueryType = queryTypeEnum.KeyValuePair;
                                newParams.RequestedRows = 1;
                                newParams.IncludeAttachments = false;
                                newParams.IncludeViewFilters = false;

                                if (newParams.ViewId > 0)
                                {
                                    FillData(newParams, MissingSchemaAction.Add);
                                    if (newParams.Data.Rows.Count > 0)
                                    {
                                        _results.Add(newParams);
                                        SearchBarCodesRet = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return SearchBarCodesRet;
        }

        private void SearchDatabase(Parameters @params)
        {
            var searchAdapter = new RecordsManageTableAdapters.SLTextSearchItemsTableAdapter();

            using (var conn = _passport.Connection())
            {
                searchAdapter.Connection = conn;

                foreach (DataRow row in searchAdapter.GetSearchTables())
                {
                    foreach (ViewItem viewItem in Navigation.GetViewsByTableName(row["IndexTableName"].ToString(), _passport))
                    {
                        var newParams = new Parameters(viewItem.Id, _passport);
                        newParams.ThinClient = @params.ThinClient;
                        newParams.Text = @params.Text;
                        newParams.KeyValue = @params.KeyValue;
                        newParams.Scope = @params.Scope;
                        newParams.QueryType = @params.QueryType;
                        newParams.RequestedRows = @params.RequestedRows;
                        newParams.NewRecord = true; // Setting this flag to TRUE, just to avoid going in set max fetch record count instead of requested rows.
                        newParams.IncludeAttachments = @params.IncludeAttachments;
                        newParams.IncludeViewFilters = false;

                        if (newParams.ViewId > 0)
                        {
                            if (Navigation.IsSearchableView(newParams.ViewId, _passport))
                                FillData(newParams, MissingSchemaAction.Add);
                            if (newParams.Data.Rows.Count > 0)
                                _results.Add(newParams);
                        }
                    }
                }
            }
        }

        private void SearchTable(Parameters @params)
        {
            using (var searchAdapter = new RecordsManageTableAdapters.SLTextSearchItemsTableAdapter())
            {
                using (var conn = _passport.Connection())
                {
                    searchAdapter.Connection = conn;

                    if (!string.IsNullOrEmpty(@params.TableName))
                    {
                        // Dim viewId As Integer = GetTableFirstSearchableViewId(params.TableName, _passport, conn)
                        // Fixed - FUS-5462: Search doesn't show results from all views when Current Table only 
                        foreach (ViewItem viewItem in Navigation.GetViewsByTableName(@params.TableName, _passport))
                        {
                            var newParams = new Parameters(viewItem.Id, _passport);
                            newParams.IncludeViewFilters = false;
                            newParams.Text = @params.Text;
                            newParams.RequestedRows = @params.RequestedRows;
                            newParams.KeyValue = @params.KeyValue;
                            newParams.Scope = @params.Scope;
                            newParams.QueryType = @params.QueryType;
                            newParams.NewRecord = true; // Setting this flag to TRUE, just to avoid going in set max fetch record count instead of requested rows.
                            newParams.IncludeAttachments = @params.IncludeAttachments;

                            if (newParams.ViewId > 0)
                            {
                                FillData(newParams, MissingSchemaAction.Add);
                                if (newParams.Data.Rows.Count > 0)
                                    _results.Add(newParams);
                            }
                        }
                    }
                }
            }
        }

        private void SearchNode(Parameters @params)
        {
            string parentTableName = string.Empty;
            var usedTables = new List<string>();
            var newParams = new Parameters(@params.ViewId, _passport);
            newParams.ThinClient = @params.ThinClient;
            newParams.Text = @params.Text;
            newParams.KeyValue = @params.KeyValue;
            newParams.Scope = @params.Scope;
            newParams.QueryType = @params.QueryType;
            newParams.RequestedRows = @params.RequestedRows;
            newParams.IncludeAttachments = @params.IncludeAttachments;
            newParams.IncludeViewFilters = false;

            if (Navigation.IsAStringType(@params.IdFieldDataType))
            {
                newParams.NodeClause = "('" + @params.CursorValue + "')";
            }
            else
            {
                newParams.NodeClause = "(" + @params.CursorValue + ")";
            }

            if (newParams.ViewId > 0)
            {
                FillData(newParams, MissingSchemaAction.Add);
                if (newParams.Data.Rows.Count > 0)
                    _results.Add(newParams);
            }

            this.LoadRelationships(@params.TableName);

            if (_relationships.Rows.Count > 0)
            {
                string whereclause = string.Empty;

                foreach (DataRow row in _relationships.Rows)
                {
                    string childKey = row["ChildKey"].ToString().Replace(".", "].[");
                    string childForKey = row["ChildForKey"].ToString().Replace(".", "].[");

                    if (Navigation.IsAStringType(@params.IdFieldDataType))
                    {
                        whereclause = string.Format("(SELECT [{0}] FROM [{1}] WHERE [{2}] = '{3}')", childKey, row["ChildTable"], childForKey, @params.CursorValue);
                    }
                    else
                    {
                        whereclause = string.Format("(SELECT [{0}] FROM [{1}] WHERE [{2}] = {3})", childKey, row["ChildTable"], childForKey, @params.CursorValue);
                    }

                    if (!usedTables.Contains(row["ChildTable"].ToString()))
                    {
                        usedTables.Add(row["ChildTable"].ToString());

                        foreach (ViewItem viewItem in Navigation.GetViewsByTableName(row["ChildTable"].ToString(), _passport))
                        {
                            newParams = new Parameters(viewItem.Id, _passport);
                            newParams.ThinClient = @params.ThinClient;
                            newParams.Text = @params.Text;
                            newParams.KeyValue = @params.KeyValue;
                            newParams.Scope = @params.Scope;
                            newParams.QueryType = @params.QueryType;
                            newParams.RequestedRows = @params.RequestedRows;
                            newParams.IncludeAttachments = @params.IncludeAttachments;
                            newParams.IncludeViewFilters = false;
                            newParams.NodeClause = whereclause;

                            if (newParams.ViewId > 0)
                            {
                                FillData(newParams, MissingSchemaAction.Add);
                                if (newParams.Data.Rows.Count > 0)
                                    _results.Add(newParams);
                            }
                        }
                    }
                }
            }

            this.ProcessRegularSearchResults(_results, @params.ThinClient, @params.RequestedRows, @params.SearchType);
        }

        public static int CommandTimeOut
        {
            get
            {
                if (_commandTimeOut == 0)
                    return 30;
                return _commandTimeOut;
            }
            set
            {
                _commandTimeOut = value;
            }
        }
        private static int _commandTimeOut;

        public static void DeleteTableItems(string tableName, List<string> tableIds, string clientIpAddress, Passport passport)
        {
            if (!passport.CheckPermission(tableName, SecureObject.SecureObjectType.Table,Smead.Security.Permissions.Permission.Delete))
            {
                throw new Exception("Insufficient permissions to delete these records");
            }

            using (var conn = passport.Connection())
            {
                Navigation.VerifyLegalDeletion(tableName, tableIds, conn);

                foreach (string item in tableIds)
                {
                    var result = ScriptEngine.RunScriptBeforeDelete(tableName, item, passport, conn);
                    if (!result.Successful)
                        throw new Exception(result.ReturnMessage);

                    DeleteTableItem(tableName, item, clientIpAddress, false, true, true, false, conn, passport);
                    ScriptEngine.RunScriptAfterDelete(tableName, item, passport, conn);
                }
            }
        }

        public static void DeleteTableItem(string tableName, string tableId, string clientIpAddress, Passport passport)
        {
            DeleteTableItem(tableName, tableId, clientIpAddress, true, true, false, passport);
        }

        public static void DeleteTableItem(string tableName, string tableId, string clientIpAddress, bool doVerify, bool deleteCertItems, bool isChildRow, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                DeleteTableItem(tableName, tableId, clientIpAddress, doVerify, deleteCertItems, true, isChildRow, conn, passport);
            }
        }

        public static void DeleteTableItem(string tableName, string tableId, string clientIpAddress, bool doVerify, bool deleteCertItems, bool deleteTableRow, bool isChildRow, SqlConnection conn, Passport passport)
        {
            if (doVerify)
                Navigation.VerifyLegalDeletion(tableName, tableId, conn);
            // delete associated attachments, then row w/auditing
            if (passport.CheckPermission(tableName, SecureObject.SecureObjectType.Attachments, Smead.Security.Permissions.Permission.Delete))
            {
                string ticket = passport.get_CreateTicket(string.Format(@"{0}\{1}", passport.ServerName, passport.DatabaseName), tableName, tableId);
                Attachments.DeleteAttachmentsForRow(clientIpAddress, ticket, passport.UserId, passport, tableName, tableId);
            }

            string beforeData = string.Empty;
            string pKey = Navigation.GetPrimaryKeyFieldName(tableName, conn);

            using (var cmd = new SqlCommand(string.Empty, conn))
            {
                var oTables = Navigation.GetTableInfo(tableName, conn);

                if (deleteTableRow)
                {
                    // gather beforeData
                    beforeData = Auditing.GetOldRecordDataForDelete(oTables, tableId, conn);
                    // delete table row and corresponding system table rows
                    cmd.CommandText = string.Format("DELETE FROM [{0}] WHERE [{1}] = @tableId", tableName, pKey);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DELETE c FROM [s_SavedChildrenFavorite] c INNER JOIN [s_SavedCriteria] s ON s.[Id] = c.[SavedCriteriaId] " + " INNER JOIN [Views] v ON v.[Id] = s.[ViewId] WHERE c.[TableId] = @tableId AND v.TableName = @tableName";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.ExecuteNonQuery();

                    if (Navigation.IsSearchableTable(tableName, conn))
                    {
                        cmd.CommandText = "DELETE FROM [SLIndexer] WHERE [IndexTableName] = @tableName AND [IndexTableID] = @tableId AND [IndexType] = 8";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@tableName", tableName);
                        cmd.Parameters.AddWithValue("@tableId", tableId);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (deleteCertItems)
                {
                    cmd.CommandText = "DELETE FROM [SLDestructCertItems] WHERE [TableName] = @tableName AND [TableId] = @tableId";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.ExecuteNonQuery();
                }

                if (Navigation.CBoolean(oTables["Trackable"]))
                {
                    cmd.CommandText = "DELETE FROM [AssetStatus]     WHERE [TrackedTable] = @tableName AND [TrackedTableId] = @tableId;" + Constants.vbCrLf + "DELETE FROM [TrackingStatus]  WHERE [TrackedTable] = @tableName AND [TrackedTableId] = @tableId";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", Navigation.PrepPad(tableName, tableId, conn));
                    cmd.ExecuteNonQuery();
                }
                // audit delete row 
                if (deleteTableRow && !string.IsNullOrWhiteSpace(beforeData))
                {
                    {
                        var withBlock = AuditType.WebAccess;
                        withBlock.TableName = tableName;
                        withBlock.TableId = tableId;
                        withBlock.ClientIpAddress = clientIpAddress;
                        withBlock.ActionType = AuditType.WebAccessActionType.DeleteRecord;
                        withBlock.BeforeData = beforeData;
                    }
                    if (isChildRow)
                        AuditType.WebAccess.ActionType = AuditType.WebAccessActionType.DeleteChildren;
                    Auditing.AuditUpdates(AuditType.WebAccess, passport);
                }
            }
        }

        public DataTable FillData(Parameters @params, bool concatenate = false)
        {
            return FillData(@params, MissingSchemaAction.AddWithKey, concatenate);
        }

        public string GetSQLForExport(Parameters @params)
        {
            RefineSQL(@params);
            return @params.SQL;
        }

        public DataTable FillData(Parameters @params, MissingSchemaAction action, bool concatenate = false)
        {
            RefineSQL(@params);
            if (!concatenate)
                @params.Data = new DataTable();
            // Dim t1 = DateTime.Now
            if (@params.SQL.Length > 0)
            {
                using (var conn = new SqlConnection(_passport.ConnectionString))
                {
                    using (var cmd = new SqlCommand(@params.SQL, conn))
                    {
                        cmd.CommandTimeout = CommandTimeOut;

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            try
                            {
                                da.MissingSchemaAction = action;
                                da.Fill(@params.Data);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                if (@params.Data.Columns.Count == 0)
                                    return @params.Data;

                                try
                                {
                                    @params.Data.Constraints.Add("pkeyConstraint", @params.Data.Columns["pkey"], true);
                                }
                                catch (Exception innerEx)
                                {
                                    Debug.WriteLine(innerEx.Message);
                                    @params.Data.Constraints.Add("RowNumConstraint", @params.Data.Columns["RowNum"], true);
                                }
                            }
                        }

                        ExtendViewColumnProperties(@params);
                    }
                }
            }
            // Dim diff = (DateTime.Now - t1).Milliseconds
            return @params.Data;
        }


        public async Task<DataTable> FillDataAsync(Parameters @params, bool concatenate = false)
        {
            return await FillDataAsync(@params, MissingSchemaAction.AddWithKey, concatenate);
        }
        public async Task<DataTable> FillDataAsync(Parameters @params, MissingSchemaAction action, bool concatenate = false)
        {
            RefineSQL(@params);
            if (!concatenate)
                @params.Data = new DataTable();
            // Dim t1 = DateTime.Now
            if (@params.SQL.Length > 0)
            {
                using (var conn = new SqlConnection(_passport.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(@params.SQL, conn))
                    {
                        cmd.CommandTimeout = CommandTimeOut;

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            try
                            {
                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    @params.Data.Load(reader);
                                    da.MissingSchemaAction = action;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                if (@params.Data.Columns.Count == 0)
                                    return @params.Data;

                                try
                                {
                                    @params.Data.Constraints.Add("pkeyConstraint", @params.Data.Columns["pkey"], true);
                                }
                                catch (Exception innerEx)
                                {
                                    Debug.WriteLine(innerEx.Message);
                                    @params.Data.Constraints.Add("RowNumConstraint", @params.Data.Columns["RowNum"], true);
                                }
                            }
                        }

                        ExtendViewColumnProperties(@params);
                    }
                }
            }
            // Dim diff = (DateTime.Now - t1).Milliseconds
            return @params.Data;
        }

        private void LoadRelationships(string ParentTable)
        {
            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand(Resources.SearchTree, conn))
                {
                    cmd.Parameters.AddWithValue("@ParentTable", ParentTable);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        _relationships = new DataTable();
                        da.Fill(_relationships);
                    }
                }
            }
        }
        private DataTable _relationships;

        private class SearchPathItem
        {
            public int Indent;
            public string Table;
            public string Field;
            public string Value;
            public string ItemName;
        }

        private void ProcessNode(Parameters mapParams, DataRow relationships, string searchText, string parentTableName)
        {
            foreach (DataRow subrow in mapParams.Data.Rows)
            {
                if (string.Compare(parentTableName, subrow.Table.ToString(), true) != 0)
                {
                    int viewId = Navigation.GetTableFirstSearchableViewId(subrow.Table.ToString(), _passport);
                    var searchParams = new Parameters(viewId, _passport);
                    searchParams.ThinClient = mapParams.ThinClient;
                    searchParams.RequestedRows = 500;
                    searchParams.KeyField = searchParams.PrimaryKey;
                    searchParams.KeyValue = subrow["pKey"].ToString();
                    searchParams.Indent = mapParams.Indent + 1;
                    searchParams.Text = searchText;
                    searchParams.QueryType = queryTypeEnum.Text;
                    searchParams.IncludeAttachments = true;
                    searchParams.IncludeViewFilters = false;

                    AddResultToSearchTree(searchParams, subrow["ItemName"].ToString());

                    if (relationships is null)
                        return;
                    string nodeParentTable = relationships["ChildTable" + mapParams.Indent.ToString()].ToString();

                    var newMapParams = new Parameters(Navigation.GetTableFirstSearchableViewId(nodeParentTable, _passport), _passport);
                    newMapParams.ThinClient = mapParams.ThinClient;
                    newMapParams.RequestedRows = 500;
                    newMapParams.QueryType = queryTypeEnum.KeyValuePair;
                    newMapParams.KeyField = Navigation.MakeSimpleField(relationships["ChildForKey" + mapParams.Indent.ToString()].ToString());
                    newMapParams.KeyValue = subrow["pkey"].ToString();
                    newMapParams.Data = FillData(newMapParams, MissingSchemaAction.Add);
                    newMapParams.Indent = mapParams.Indent + 1;

                    if (newMapParams.Data is not null)
                    {
                        ProcessNode(newMapParams, relationships, searchText, parentTableName);
                    }
                }
            }
        }

        private bool HasSearchTreeMatches(Parameters mapResult, string searchText)
        {
            var @params = new Parameters(mapResult.ViewId, _passport);
            @params.ThinClient = mapResult.ThinClient;
            @params.QueryType = queryTypeEnum.KeyValuePair;
            @params.Text = searchText;
            @params.KeyField = mapResult.KeyField;
            @params.KeyValue = mapResult.KeyValue;
            @params.Data = FillData(@params, MissingSchemaAction.Add);
            return Conversions.ToBoolean(@params.Data.Rows.Count);
        }

        private void AddResultToSearchTree(Parameters @params, string itemName)
        {
            @params.Data = FillData(@params, MissingSchemaAction.Add);
            if (@params.Data.Rows.Count > 0)
            {
                _searchTreeResultList.Add(@params);
            }
            else
            {
                // _HTMLSearchResults &= "<tr><td style='font-size: 9pt; padding-bottom: 5px; padding-left:" & (params.Indent + 1) * 20 & "px;'>" & itemName.ToString & "</td></tr>" & vbCrLf
            }
        }

        private void ExtendViewColumnProperties(Parameters @params)
        {
            using (var viewColAdapter = new RecordsManageTableAdapters.ViewColumnsTableAdapter())
            {
                using (var conn = _passport.Connection())
                {
                    viewColAdapter.Connection = conn;
                    DataTable viewColumnsData;
                    var colOrdinals = new Dictionary<string, int>();
                    if (@params.AltViewId != 0)
                    {
                        viewColumnsData = viewColAdapter.GetAllFieldProperties(@params.AltViewId);
                    }
                    else
                    {
                        viewColumnsData = viewColAdapter.GetAllFieldProperties(@params.ViewId);
                    }

                    DataRow[] viewColumnRows;
                    foreach (DataColumn queryColumn in @params.Data.Columns)
                    {
                        queryColumn.ExtendedProperties["visible"] = true;
                        queryColumn.ExtendedProperties["editmask"] = string.Empty;
                        queryColumn.ExtendedProperties["specialtype"] = "normal";
                        queryColumn.ExtendedProperties["columnvisible"] = 3; // 3=Not Visible
                        queryColumn.ExtendedProperties["columnvisiblespecial"] = string.Empty;
                        queryColumn.ExtendedProperties["lookuptype"] = string.Empty;
                        queryColumn.ExtendedProperties["heading"] = queryColumn.ColumnName.ToString();
                        queryColumn.ExtendedProperties["editallowed"] = false;
                        queryColumn.ExtendedProperties["dropdownflag"] = false;
                        queryColumn.ExtendedProperties["LookupData"] = null;
                        queryColumn.ExtendedProperties["LookupIdColumn"] = -1;
                        queryColumn.ExtendedProperties["LookupIdValue"] = string.Empty;
                        queryColumn.ExtendedProperties["regex"] = string.Empty;
                        queryColumn.ExtendedProperties["searchable"] = false;
                        queryColumn.ExtendedProperties["sortable"] = true;
                        queryColumn.ExtendedProperties["FilterField"] = true;
                        queryColumn.ExtendedProperties["ColumnWidth"] = 200;

                        if (string.Compare(queryColumn.ColumnName, "pkey", false) == 0)
                            queryColumn.ExtendedProperties["visible"] = false;
                        if (string.Compare(queryColumn.ColumnName, "attachments", false) == 0)
                            queryColumn.ExtendedProperties["visible"] = false;
                        if (string.Compare(queryColumn.ColumnName, "itemname", false) == 0)
                            queryColumn.ExtendedProperties["visible"] = false;
                        if (string.Compare(queryColumn.ColumnName, "slrequestable", false) == 0)
                            queryColumn.ExtendedProperties["visible"] = false;
                        string colName = Navigation.MakeSimpleField(queryColumn.ColumnName); // .Replace("_TrackingLookup", "")
                        bool isTrackingLookup = colName.EndsWith("_TrackingLookup");

                        if (isTrackingLookup)
                        {
                            colName = colName.Replace("_TrackingLookup", string.Empty);
                            viewColumnRows = viewColumnsData.Select(string.Format("(FieldName = '{0}' OR FieldName LIKE '%.{0}') AND LookupType = 11", colName), "ColumnNum");
                        }
                        else
                        {
                            viewColumnRows = viewColumnsData.Select(string.Format("(FieldName = '{0}' OR FieldName LIKE '%.{0}')", colName), "ColumnNum");
                        }

                        if (viewColumnRows.Count() > 0)
                        {
                            int colNum = 0;
                            if (viewColumnRows.Count() > 1)
                            {
                                if (colOrdinals.ContainsKey(colName))
                                {
                                    colNum = colOrdinals[colName] + 1;
                                    colOrdinals[colName] = colNum;
                                }
                                else
                                {
                                    colOrdinals.Add(colName, colOrdinals.Count);
                                }
                            }

                            queryColumn.ExtendedProperties["columnvisible"] = viewColumnRows[colNum]["columnOrder"];
                            queryColumn.ExtendedProperties["sortable"] = viewColumnRows[colNum]["sortablefield"];
                            queryColumn.ExtendedProperties["FilterField"] = viewColumnRows[colNum]["FilterField"];
                            queryColumn.ExtendedProperties["caplocks"] = viewColumnRows[colNum]["columnvisible"];
                            queryColumn.ExtendedProperties["lookuptype"] = viewColumnRows[colNum]["lookuptype"];
                            queryColumn.ExtendedProperties["heading"] = viewColumnRows[colNum]["heading"];
                            queryColumn.ExtendedProperties["editallowed"] = viewColumnRows[colNum]["editallowed"];
                            queryColumn.ExtendedProperties["ColumnWidth"] = Conversions.ToInteger(viewColumnRows[colNum]["ColumnWidth"]);
                            queryColumn.ExtendedProperties["LookupIdColumn"] = Conversions.ToInteger(viewColumnRows[colNum]["LookupIdCol"]);
                            if (@params.Data.Columns[queryColumn.ColumnName].AutoIncrement)
                                queryColumn.ExtendedProperties["editallowed"] = false;
                            queryColumn.ExtendedProperties["dropdownflag"] = Conversions.ToBoolean(viewColumnRows[colNum]["dropdownflag"]);
                            // moti mashiah test list return for lookup dropdown
                            if (Conversions.ToBoolean(queryColumn.ExtendedProperties["dropdownflag"]))
                            {
                                DataColumn queryColumn1 = queryColumn;
                                queryColumn.ExtendedProperties["LookupData"] = Navigation.GetListLookup(ref queryColumn1, @params, viewColumnRows[colNum], conn);
                            }

                            if (!string.IsNullOrEmpty(viewColumnRows[colNum]["editmask"].ToString()))
                            {
                                queryColumn.ExtendedProperties["editmask"] = viewColumnRows[colNum]["editmask"];

                                if (viewColumnRows[colNum]["editmask"].ToString().Contains("(###)")) // phone
                                {
                                    queryColumn.ExtendedProperties["regex"] = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";
                                    queryColumn.ExtendedProperties["specialtype"] = "phone";
                                }
                                else if (viewColumnRows[colNum]["editmask"].ToString().Contains("###-##-####")) // social
                                {
                                    queryColumn.ExtendedProperties["regex"] = @"^\d{3}-\d{2}-\d{4}$";
                                    queryColumn.ExtendedProperties["specialtype"] = "social";
                                }
                                else if (viewColumnRows[colNum]["editmask"].ToString().Contains("#####")) // zip
                                {
                                    queryColumn.ExtendedProperties["regex"] = @"^(\d{5}-\d{4}|\d{5}|\d{9})$|^([a-zA-Z]\d[a-zA-Z] \d[a-zA-Z]\d)$";
                                    queryColumn.ExtendedProperties["specialtype"] = "zip";
                                }
                                else if (viewColumnRows[colNum]["editmask"].ToString().Contains("@")) // email 
                                {
                                    if (string.IsNullOrEmpty(viewColumnRows[colNum]["editmask"].ToString().Replace("@", string.Empty))) // not email 
                                    {
                                        queryColumn.ExtendedProperties["specialtype"] = "normal";
                                    }
                                    else
                                    {
                                        queryColumn.ExtendedProperties["regex"] = @"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
                                        queryColumn.ExtendedProperties["specialtype"] = "email";
                                    }
                                }
                            }
                            else
                            {
                                queryColumn.ExtendedProperties["editmask"] = "";
                                queryColumn.ExtendedProperties["specialtype"] = "normal";
                            }

                            if (string.IsNullOrEmpty(queryColumn.ExtendedProperties["regex"].ToString()))
                            {
                                if (queryColumn.DataType.FullName.ToString().Contains("System.Int"))
                                {
                                    queryColumn.ExtendedProperties["regex"] = @"^\d+$";
                                }
                                else if (queryColumn.DataType.FullName.ToString().Contains("System.Date"))
                                {
                                    var seperator = new[] { '/', '-', '.', ' ' };
                                    string reg = "^";
                                    foreach (string dateElement in @params.Culture.DateTimeFormat.ShortDatePattern.Split(seperator))
                                    {
                                        if (reg != "^")
                                            reg += "[- /.]";

                                        if (dateElement.Length <= 2 & dateElement.ToLower().Contains("m"))
                                        {
                                            reg += "(0?[1-9]|1[0-2])";
                                        }
                                        else if (dateElement.Length <= 3 & dateElement.ToLower().Contains("m")) // 'Added by Hasmukh on 05/12/2016 for date formate changes 
                                        {
                                            reg += "(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)";
                                        }
                                        else if (dateElement.Length <= 2 & dateElement.ToLower().Contains("d"))
                                        {
                                            reg += @"(([0-2]?\d{1})|([3][0,1]{1}))";
                                        }
                                        else
                                        {
                                            reg += @"(([1]{1}[9]{1}[0-9]{1}\d{1})|([2-9]{1}\d{3})|(\d{2}))";
                                        }  // Added by Hasmukh on 05/12/2016 for date format changes (added "|(\d{2})" at last for support two digits)
                                    }

                                    reg += "$";
                                    queryColumn.ExtendedProperties["regex"] = reg;

                                    try
                                    {
                                        //var httpcontext = new HttpContextAccessor().HttpContext;
                                        //httpcontext.Session.SetString("dtRegex", reg.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.Message);
                                        // ignore exceptions, most likely launched from desktop client which has no reference (or need) for HttpContext.  RVW 07/27/2017
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        public static string GetSQLViewName(Parameters @params, bool clearWhereClause)
        {
            if (clearWhereClause)
                @params.WhereClause = string.Empty;

            var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter();
            viewAdapter.Connection = _passport.Connection();
            RecordsManage.ViewsRow viewsRow = (RecordsManage.ViewsRow)viewAdapter.GetViewByViewID(@params.ViewId).Rows[0];
            if (!@params.NewRecord && @params.Paged)
                @params.RequestedRows = viewsRow.MaxRecsPerFetch;

            string viewSafeSQL = Query.NormalizeString(viewsRow.SQLStatement);

            if (viewSafeSQL.ToLower().Contains(" order by"))
            {
                @params.ViewSort = viewSafeSQL.Substring(viewSafeSQL.ToLower().IndexOf(" order by"));
                viewSafeSQL = viewSafeSQL.Substring(0, viewSafeSQL.ToLower().IndexOf(" order by"));
                if (@params.ViewSort.Contains(".")) // multipart; make sure that it's not aliased out
                {
                    var sortTerms = @params.ViewSort.Split(' ');
                    foreach (var sortTerm in sortTerms)
                    {
                        if (sortTerm.Contains("."))
                        {
                            if (viewSafeSQL.ToLower().Substring(viewSafeSQL.ToLower().IndexOf(sortTerm.ToLower()) + sortTerm.Length).StartsWith(" as "))
                            {
                                string subStr = viewSafeSQL.Substring(viewSafeSQL.ToLower().IndexOf(sortTerm.ToLower()) + sortTerm.Length + 4).TrimStart();
                                string replaceTerm = "";
                                if (subStr.StartsWith("["))
                                {
                                    replaceTerm = subStr.Substring(0, subStr.IndexOf("]") + 1);
                                }
                                else
                                {
                                    foreach (char c in subStr)
                                    {
                                        if (c != ',' & !char.IsWhiteSpace(c))
                                        {
                                            replaceTerm += Conversions.ToString(c);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }

                                @params.ViewSort = @params.ViewSort.Replace(sortTerm, replaceTerm);
                            }
                        }
                    }
                }
            }

            string sqlViewName = string.Format("view__{0}", @params.ViewId.ToString());
            string cmdText = string.Format("CREATE VIEW {0} AS {1}", sqlViewName, viewSafeSQL);

            if (viewSafeSQL.ToLower().Contains("@@sl_username"))
            {
                var user = new User(_passport, true);
                sqlViewName = string.Format("view__{0}__{1}", @params.ViewId.ToString(), user.UserId.ToString());
                cmdText = string.Format("CREATE VIEW {0} AS {1}", sqlViewName, Strings.Replace(viewSafeSQL, "@@sl_username", user.UserName.Replace("'", "''"), 1, -1, CompareMethod.Text));
            }

            if (!Navigation.ViewExists(sqlViewName, _passport))
            {
                using (var cmd = new SqlCommand(cmdText, _passport.StaticConnection()))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            return sqlViewName;
        }

        private string GetSQLViewTable(Parameters @params)
        {
            @params.WhereClause = string.Empty;

            using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                using (var conn = _passport.Connection())
                {
                    viewAdapter.Connection = conn;
                    RecordsManage.ViewsRow viewsRow = (RecordsManage.ViewsRow)viewAdapter.GetViewByViewID(@params.ViewId).Rows[0];
                    string viewSafeSQL = Query.NormalizeString(viewsRow.SQLStatement);

                    if (viewSafeSQL.ToLower().Contains(" where "))
                    {
                        @params.WhereClause = viewSafeSQL.Substring(viewSafeSQL.ToLower().IndexOf(" where "));
                        var user = new User(_passport, true);
                        @params.WhereClause = Strings.Replace(@params.WhereClause, "@@sl_username", user.UserName.Replace("'", "''"), Compare: CompareMethod.Text);
                    }
                }
            }

            return @params.TableName;
        }

        private void ReplaceUserNameParameter(string viewName, Parameters @params) // Depreciated
        {
            if (string.IsNullOrEmpty(@params.WhereClause) || string.IsNullOrEmpty(@params.SQL))
                return;
            if (!@params.WhereClause.ToLower().Contains("'@@sl_username'") || @params.WhereClause.ToLower().IndexOf(" where ") == -1)
                return;

            var sb = new StringBuilder();
            var user = new User(_passport, true);
            sb.Append(@params.WhereClause.Substring(0, @params.WhereClause.ToLower().IndexOf("'@@sl_username'")));
            sb.Append(string.Format("'{0}'", user.UserName.Replace("'", "''")));
            sb.Append(@params.WhereClause.Substring(@params.WhereClause.ToLower().IndexOf("'@@sl_username'") + "'@@sl_username'".Length));

            if (@params.SQL.ToLower().Contains(string.Format("{0} where ", viewName)))
            {
                @params.SQL = Strings.Replace(@params.SQL, string.Format("{0} where ", viewName), string.Format("{0} {1} AND ", viewName, sb.ToString().Trim()), Compare: CompareMethod.Text);
            }
            else
            {
                @params.SQL += string.Format(" {0}", sb.ToString().Trim());
            }
        }

        public Parameters GetParameterObject(int viewId, string searchText)
        {
            return GetParameterObject(viewId, searchText, false);
        }

        public Parameters GetParameterObject(int viewId, string searchText, bool includeAttachments)
        {
            var @params = new Parameters(viewId, _passport);
            @params.IncludeAttachments = includeAttachments;
            @params.QueryType = queryTypeEnum.Text;
            @params.Text = searchText;
            @params.Scope = ScopeEnum.Table;

            RefineSQL(@params);
            return @params;
        }

        private bool AlreadyIncluded(string fieldName, List<string> included, string tableName)
        {
            if (included.Contains(fieldName))
                return true;
            if (included.Contains(tableName + "." + fieldName))
                return true;
            if (included.Contains(fieldName.Replace(tableName + ".", "")))
                return true;
            return false;
        }

        public string RefineSQLForSelectAll(Parameters @params)
        {
            string tempWhereClause = @params.WhereClause;
            if (@params.ViewId == 0)
                return string.Empty;
            string viewColumnsSQL = string.Empty;
            string viewFilters = string.Empty;

            string sqlViewTable = GetSQLViewName(@params, false);
            // set required fields for a result 
            viewColumnsSQL += "[" + @params.PrimaryKey + "] AS pkey";
            viewColumnsSQL = viewColumnsSQL.Replace("@pKey", @params.PrimaryKey);
            @params.SQL = string.Format("SELECT {0} FROM {1}", viewColumnsSQL, sqlViewTable);

            // 'Get default filters applied on views
            var viewFilterData = ViewFilterTable(@params);

            using (var conn = _passport.Connection())
            {
                // 'Build where for default filters
                foreach (RecordsManage.ViewColumnsRow viewColumn in @params.ViewColumns.Rows)
                    viewFilters += this.BuildViewFilterWhereClause(Conversions.ToInteger(viewColumn["ColumnNum"]), viewColumn["fieldName"].ToString(), viewColumn["EditMask"].ToString(), @params, viewFilterData);

                // '''Start : Hasmukh : Append where clause passed from UI 
                if (!string.IsNullOrEmpty(tempWhereClause))
                {
                    tempWhereClause = NormalizeString(tempWhereClause);
                    if (!string.IsNullOrEmpty(@params.WhereClause))
                    {
                        @params.WhereClause = @params.WhereClause + tempWhereClause.ToLower();
                    }
                    else if (tempWhereClause.ToLower().Substring(0, 10).Contains("where "))
                    {
                        @params.WhereClause = tempWhereClause.ToLower();
                    }
                    else
                    {
                        @params.WhereClause = " where " + tempWhereClause.ToLower();
                    }
                }

                if (!string.IsNullOrEmpty(@params.WhereClause) && !string.IsNullOrEmpty(@params.SQL))
                    @params.SQL += " " + @params.WhereClause;
                @params.SQL = Query.NormalizeString(@params.SQL);
                switch (@params.QueryType)
                {
                    case queryTypeEnum.AdvancedFilter: // Filter applied in query window
                        {
                            if (@params.SQL.ToLower().Contains(" where "))
                            {
                                @params.SQL += @params.AndFilter;
                            }
                            else
                            {
                                @params.SQL += @params.WhereFilter;
                            }

                            break;
                        }
                }

                if (viewFilters.Length > 0)
                {
                    if (@params.SQL.ToLower().Contains(" where "))
                    {
                        @params.SQL += " AND (" + viewFilters.Substring(0, viewFilters.Length - 5) + ")";
                    }
                    else
                    {
                        @params.SQL += " WHERE (" + viewFilters.Substring(0, viewFilters.Length - 5) + ")";
                    }
                }

                @params.SQL = @params.SQL.Replace("_!*!tabExt", "");
                @params.SQL = this.GetRetentionPermissionClause(@params.TableInfo, @params.SQL, conn);
                if (@params.TotalRows <= 0)
                    Query.TotalQueryRowCount(@params.SQL, conn);
            }
            return @params.SQL;
        }

        public void RefineSQL(Parameters @params)
        {
            // '''Start : Hasmukh : Get Whereclause into temp Variable
            string tempWhereClause = @params.WhereClause;
            int tempRequestedRows = @params.RequestedRows;
            // '''End : Hasmukh : Get Whereclause into temp Variable

            if (@params.ViewId == 0)
                return;

            string viewColumnsSQL = string.Empty;
            string viewFilters = string.Empty;
            var includedColumns = new List<string>();
            var orderBy = new List<string>();
            string orderClause = string.Empty;

            for (int i = 1; i <= 5; i++)
                orderBy.Add(string.Empty);

            try
            {
                CommandTimeOut = Conversions.ToInteger(@params.TableInfo["ADOQueryTimeOut"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                CommandTimeOut = 30;
            }

            string sqlViewTable = GetSQLViewName(@params, true);
            @params.SQL = "SELECT * FROM " + sqlViewTable;
            // set required fields for a result 
            viewColumnsSQL += "[" + @params.PrimaryKey + "] AS pkey,";
            viewColumnsSQL += this.GetAttachmentCountSQL(@params.TableInfo, sqlViewTable, Navigation.FieldIsAString(@params.TableName, Navigation.GetPrimaryKeyFieldName(@params.TableInfo), _passport)) + " AS Attachments,";
            viewColumnsSQL += this.GetItemNameSQL(@params.TableName, @params.TableInfo, @params.Culture.DateTimeFormat.ShortDatePattern) + " AS ItemName,";
            viewColumnsSQL += this.GetDescriptionsSQL(@params.TableName, @params.TableInfo);
            viewColumnsSQL += this.GetDispositionSQL(@params.TableInfo);

            try
            {
                if (Conversions.ToBoolean(Navigation.GetTableInfo("Locations", _passport)["TrackingRequestableFieldName"].ToString().Length))
                {
                    viewColumnsSQL += "1 AS [slRequestable],";
                }
                else
                {
                    viewColumnsSQL += "0 AS [slRequestable],";
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine(ex.Message);
                viewColumnsSQL += "0 AS [slRequestable],";
            }

            viewColumnsSQL = viewColumnsSQL.Replace("@pKey", @params.PrimaryKey);
            viewColumnsSQL = viewColumnsSQL.Replace("@ViewName", sqlViewTable);
            viewColumnsSQL = viewColumnsSQL.Replace("@TableName", @params.TableName);
            var viewFilterData = ViewFilterTable(@params);

            using (var conn = _passport.Connection())
            {
                var trackableTables = Tracking.GetTrackingContainerTypes(conn);

                //var length = default(int);
                foreach (RecordsManage.ViewColumnsRow viewColumn in @params.ViewColumns.Rows)
                {
                    viewFilters += this.BuildViewFilterWhereClause(Conversions.ToInteger(viewColumn["ColumnNum"]), viewColumn["fieldName"].ToString(), viewColumn["EditMask"].ToString(), @params, viewFilterData);
                    if ((Query.IncludeColumn(viewColumn.FieldName) | viewColumn.FieldName.ToLower() == "slfileroomorder") & !this.AlreadyIncluded(viewColumn.FieldName, includedColumns, @params.TableName))
                    {
                        // discuss refine sql duplicate column issue
                        // order by
                        if (viewColumn.SortOrder > 0)
                        {
                            int lengh =  default(int);
                            var type = Navigation.GetFieldType(@params.TableName, viewColumn.FieldName, ref lengh, _passport);
                            string sortField = "[" + Navigation.MakeSimpleField(viewColumn.FieldName) + "] ";
                            if (type.FullName == "System.String" & (lengh > 8000 | lengh < 0))
                                sortField = "CONVERT(VARCHAR(8000)," + sortField + ") ";
                            orderBy[viewColumn.SortOrder] = sortField + Interaction.IIf(viewColumn.SortOrderDesc, "DESC,", "ASC,").ToString();
                        }
                        // build columns
                        if (@params.KeyValue == " -query" & (int)viewColumn.LookupType == 0)
                        {
                            viewColumnsSQL += "[" + Navigation.MakeSimpleField(viewColumn.FieldName) + "],";
                        }
                        else
                        {
                            switch (viewColumn.LookupType)
                            {
                                case 0: // direct field 
                                    {
                                        switch (Navigation.MakeSimpleField(viewColumn.FieldName) ?? "")
                                        {
                                            case "SLFileRoomOrder":
                                                {
                                                    using (var fileRoom = new RecordsManageTableAdapters.SLTableFileRoomOrderTableAdapter())
                                                    {
                                                        string fileRoomSQL = string.Empty;
                                                        fileRoom.Connection = conn;

                                                        foreach (RecordsManage.SLTableFileRoomOrderRow row in fileRoom.GetData(@params.TableName))
                                                        {
                                                            if (row.StartFromFront)
                                                            {
                                                                fileRoomSQL += string.Format("SUBSTRING({0},{1},{2} ) +", row.FieldName, (object)row.StartingPosition, (object)row.NumberofCharacters);
                                                            }
                                                            else
                                                            {
                                                                fileRoomSQL += string.Format("SUBSTRING(RIGHT(REPLICATE('0', 7) + CAST({0} AS VARCHAR(50)), {1}), 1, {2})+", row.FieldName, (object)row.StartingPosition, (object)row.NumberofCharacters);
                                                            }
                                                        }

                                                        viewColumnsSQL += Strings.Left(fileRoomSQL, Strings.Len(fileRoomSQL) - 1) + " AS '" + viewColumn.FieldName + "',";
                                                        includedColumns.Add(viewColumn.FieldName);
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    viewColumnsSQL += "[" + Navigation.MakeSimpleField(viewColumn.FieldName) + "],";
                                                    includedColumns.Add(viewColumn.FieldName);
                                                    break;
                                                }
                                        }

                                        break;
                                    }
                                case 1: // parent lookup
                                    {
                                        if (!viewColumn.Heading.Contains(" For "))
                                        {
                                            try
                                            {
                                                string upperTableName = Strings.Split(viewColumn.FieldName, ".")[0];
                                                string upperTableFieldName = Strings.Split(viewColumn.FieldName, ".")[1];
                                                string lowerTableForeignKey = Navigation.GetLowerTableForeignKeyField(upperTableName, @params.TableName, conn);
                                                // Dim upperTablePrimaryKey = GetPrimaryKeyFieldName(upperTableName, conn)
                                                var upperTable = Navigation.GetUpperRelationship(@params.TableName, lowerTableForeignKey, conn);
                                                viewColumnsSQL += "(SELECT TOP 1 [" + upperTableFieldName + "] FROM [" + upperTableName + "] WHERE " + Navigation.MakeSimpleField(upperTable.AsEnumerable().ElementAtOrDefault(0)["UpperTableFieldName"].ToString()) + " = [" + sqlViewTable + "]." + Navigation.MakeSimpleField(lowerTableForeignKey) + ") AS '" + viewColumn.FieldName.ToString() + "',";
                                                includedColumns.Add(viewColumn.FieldName);
                                            }
                                            catch (Exception ex) // grand parent 
                                            {
                                                Debug.WriteLine(ex.Message);

                                                try
                                                {
                                                    string upperTableName = Strings.Split(viewColumn.FieldName, ".")[0];
                                                    string upperTableFieldName = Strings.Split(viewColumn.FieldName, ".")[1];
                                                    // Dim lowerTableForeignKey = GetLowerTableForeignKeyField(upperTableName, params.TableName, _passport)
                                                    string upperTablePrimaryKey = Navigation.GetPrimaryKeyFieldName(upperTableName, conn);
                                                    // Dim upperTable As DataTable = GetUpperRelationship(params.TableName, lowerTableForeignKey, conn)
                                                    string grandParentField = Navigation.GetGrandParentLookupField(@params.ViewId, viewColumn.FieldName, (int)viewColumn.LookupIdCol, conn);

                                                    string grandParentSQL = "(SELECT TOP 1 [" + upperTableFieldName + "] FROM [" + upperTableName + "] WHERE [" + upperTablePrimaryKey + "] = ";
                                                    grandParentSQL += "(SELECT [" + Strings.Split(grandParentField, ".")[1] + "] ";
                                                    grandParentSQL += " FROM [" + Strings.Split(grandParentField, ".")[0] + "] ";
                                                    grandParentSQL += " WHERE [" + Navigation.GetPrimaryKeyFieldName(Strings.Split(grandParentField, ".")[0], conn) + "] = ";
                                                    grandParentSQL += "[" + Navigation.MakeSimpleField(Navigation.GetLowerTableForeignKeyField(Strings.Split(grandParentField, ".")[0], @params.TableName, conn)) + "]";
                                                    grandParentSQL += " )";
                                                    grandParentSQL += ") AS '" + Navigation.MakeSimpleField(viewColumn.FieldName) + "',";
                                                    viewColumnsSQL += grandParentSQL;
                                                    includedColumns.Add(viewColumn.FieldName);
                                                }
                                                catch (Exception ex2)
                                                {
                                                    Debug.WriteLine(ex2.Message);
                                                }
                                            }
                                        }

                                        break;
                                    }
                                case 8: // row number
                                    {
                                        break;
                                    }
                                // done on the fly so not a lookup type 
                                case 11: // tracking lookup 
                                    {
                                        string idSql = "(SELECT TOP 1 " + viewColumn.FieldName + " FROM TrackingStatus WHERE TrackedTable = '" + @params.TableName + "'";

                                        if (Navigation.IsAStringType(@params.IdFieldDataType))
                                        {
                                            idSql += "AND TrackedTableID = " + sqlViewTable + "." + @params.PrimaryKey + ")";
                                        }
                                        else
                                        {
                                            idSql += "AND TrackedTableID = RIGHT('" + new string('0', 30) + "' + CAST(" + sqlViewTable + "." + @params.PrimaryKey + " AS VARCHAR(30)), 30))";
                                        }

                                        var trackedTable = trackableTables.Select("TrackingStatusFieldName='" + viewColumn.FieldName + "'");

                                        if (trackedTable.Count() > 0)
                                        {
                                            string desc = this.GetTrackingDisplayFields(trackedTable[0]);
                                            viewColumnsSQL += string.Format("(SELECT TOP 1 {0} FROM [{1}] WHERE [{2}] = {3}) AS {4},", desc, trackedTable[0]["TableName"], Navigation.MakeSimpleField(trackedTable[0]["IdFieldName"].ToString()), idSql, viewColumn.FieldName + "_TrackingLookup");
                                        }

                                        break;
                                    }
                                // Select Case viewColumn.FieldName.ToLower
                                // Case "locationsid"
                                // viewColumnsSQL &= "(SELECT TOP 1 Description FROM Locations WHERE Id=" & idSql & ") AS " & viewColumn.FieldName & ","
                                // includedColumns.Add(viewColumn.FieldName)
                                // Case "employeesid"
                                // viewColumnsSQL &= "(SELECT TOP 1 Name FROM Employees WHERE Id=" & idSql & ") AS " & viewColumn.FieldName & ","
                                // includedColumns.Add(viewColumn.FieldName)
                                // End Select
                                case 12:
                                case 13:
                                    {
                                        if (!string.IsNullOrEmpty(@params.KeyValue))
                                        {
                                            try
                                            {
                                                string lowerTableName = Strings.Split(viewColumn.FieldName, ".")[0];
                                                string lowerTableFieldName = Strings.Split(viewColumn.FieldName, ".")[1];
                                                string upperTableForeignKey = Navigation.GetLowerTableForeignKeyField(@params.TableName, lowerTableName, conn);
                                                string lookDownSQL = "SELECT [" + lowerTableFieldName + "] FROM [" + lowerTableName + "] WHERE CAST([" + Navigation.MakeSimpleField(upperTableForeignKey) + "] AS VARCHAR(50)) ='" + @params.KeyValue + "'";
                                                string buf = string.Empty;

                                                using (var cmd = new SqlCommand(lookDownSQL, conn))
                                                {
                                                    using (var da = new SqlDataAdapter(cmd))
                                                    {
                                                        var dt = new DataTable();
                                                        da.Fill(dt);

                                                        foreach (DataRow row in dt.Rows)
                                                        {
                                                            if ((int)viewColumn.LookupType == 12)
                                                            {
                                                                buf += row[0].ToString() + ", ";
                                                            }
                                                            else if ((int)viewColumn.LookupType == 13)
                                                            {
                                                                buf += row[0].ToString() + Constants.vbCrLf;
                                                            }
                                                        }

                                                        viewColumnsSQL += "'" + buf + "' AS '" + viewColumn.FieldName + "',";
                                                        includedColumns.Add(viewColumn.FieldName);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex.Message);
                                            }
                                        }

                                        break;
                                    }
                                case 16: // count child rows
                                    {
                                        if (!string.IsNullOrEmpty(@params.KeyValue) & !(@params.KeyValue == "0"))
                                        {
                                            viewColumnsSQL += "(SELECT TOP 1 COUNT(*) FROM [" + Navigation.GetLowerRelationships(@params.TableName, @params.PrimaryKey, conn).Rows[0]["LowerTableName"].ToString() + "] WHERE CAST(" + Navigation.MakeSimpleField(viewColumn.FieldName) + " AS VARCHAR(50)) = '" + @params.KeyValue + "') AS '" + viewColumn.FieldName.ToString() + "',";
                                            includedColumns.Add(viewColumn.FieldName);
                                        }

                                        break;
                                    }
                                case 17: // sum lookup
                                    {
                                        if ((int)viewColumn.LookupIdCol > 0)
                                        {
                                            var dtSum = Navigation.GetLowerRelationships(@params.TableName, @params.PrimaryKey, conn);
                                            viewColumnsSQL += "(SELECT TOP 1 SUM(" + viewColumn.FieldName + ") FROM " + dtSum.Rows[0]["LowerTableName"].ToString();
                                            viewColumnsSQL += this.GetSidewaysLookup(viewColumn.FieldName, dtSum.Rows[0], conn);
                                            viewColumnsSQL += " WHERE " + dtSum.Rows[0]["LowerTableFieldName"].ToString() + " = " + sqlViewTable + "." + @params.PrimaryKey + ") AS '" + viewColumn.FieldName + "',";
                                            includedColumns.Add(viewColumn.FieldName);
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                }

                // '''Start : Hasmukh : Append where clause passed from UI
                if (!string.IsNullOrEmpty(tempWhereClause))
                {
                    if (!string.IsNullOrEmpty(@params.WhereClause))
                    {
                        @params.WhereClause = @params.WhereClause + tempWhereClause.ToLower();
                    }
                    else if (tempWhereClause.Length > 9 && tempWhereClause.ToLower().Substring(0, 10).Contains("where "))
                    {
                        @params.WhereClause = tempWhereClause.ToLower();
                    }
                    else
                    {
                        @params.WhereClause = " where " + tempWhereClause.ToLower();
                    }
                }
                // '''End : Hasmukh : Append where clause passed from UI
                // '''Start : Hasmukh : Background Process export all records bug : FUS-5118
                if (tempRequestedRows < 0)
                {
                    @params.RequestedRows = tempRequestedRows;
                }
                // '''End : Hasmukh : Background Process export all records bug : FUS-5118

                if (!string.IsNullOrEmpty(@params.WhereClause) && !string.IsNullOrEmpty(@params.SQL))
                    @params.SQL += " " + @params.WhereClause;

                if (!string.IsNullOrEmpty(@params.ParentField) && !string.IsNullOrEmpty(@params.ParentValue))
                {
                    if (@params.SQL.ToLower().Contains("where"))
                    {
                        @params.SQL += " AND "; // CAST([" & params.ParentField & "] AS VARCHAR(50)) = '" & params.ParentValue & "'"
                    }
                    else
                    {
                        @params.SQL += " WHERE ";
                    } // CAST([" & params.ParentField & "] AS VARCHAR(50)) = '" & params.ParentValue & "'"
                    if (Navigation.FieldIsAString(@params.TableInfo, @params.ParentField, conn))
                    {
                        @params.SQL += "CAST([" + @params.ParentField + "] AS VARCHAR(50)) = '" + @params.ParentValue + "'";
                    }
                    else
                    {
                        @params.SQL += "[" + @params.ParentField + "] = " + @params.ParentValue;
                    }
                }

                switch (@params.QueryType)
                {
                    case queryTypeEnum.OpenTable:
                        {
                            break;
                        }
                    // no where clause
                    case queryTypeEnum.KeyValuePair:
                        {
                            if (@params.SQL.ToLower().Contains("where"))
                            {
                                @params.SQL += " AND ";
                            }
                            else
                            {
                                @params.SQL += " WHERE ";
                            }
                            if (Navigation.IsAStringType(@params.IdFieldDataType))
                            {
                                @params.SQL += "CAST([" + @params.KeyField + "] AS VARCHAR(50)) = '" + @params.KeyValue + "'";
                            }
                            else
                            {
                                @params.SQL += "[" + @params.KeyField + "] = " + @params.KeyValue;
                            }

                            break;
                        }
                    case queryTypeEnum.AdvancedFilter:
                        {
                            if (@params.SQL.ToLower().Contains("where"))
                            {
                                @params.SQL += @params.AndFilter;
                            }
                            else
                            {
                                @params.SQL += @params.WhereFilter;
                            }

                            break;
                        }
                    case queryTypeEnum.Text:
                        {
                            Debug.Print("{0} - {1} - {2}", @params.TableName, @params.ViewName, (object)@params.ViewId);
                            string fts = BuildFullTextWhereClause(@params, sqlViewTable, conn);
                            if (fts.Length > 0)
                            {
                                if (@params.SQL.ToLower().Contains("where"))
                                {
                                    @params.SQL += " AND (" + fts + ")";
                                }
                                else
                                {
                                    @params.SQL += " WHERE (" + fts + ")";
                                }
                            }
                            else
                            {
                                @params.SQL = string.Empty;
                                return;
                            }

                            break;
                        }
                    case queryTypeEnum.LikeItemName:
                        {
                            if (@params.SQL.ToLower().Contains("where"))
                            {
                                @params.SQL += " AND ";
                            }
                            else
                            {
                                @params.SQL += " WHERE ";
                            }
                            @params.SQL += this.GetItemNameSQL(@params.TableName, @params.TableInfo, @params.Culture.DateTimeFormat.ShortDatePattern) + " LIKE '%" + @params.Text + "%'";
                            break;
                        }
                }

                if (viewFilters.Length > 0)
                {
                    if (@params.SQL.ToLower().Contains("where"))
                    {
                        @params.SQL += " AND (" + viewFilters.Substring(0, viewFilters.Length - 5) + ")";
                    }
                    else
                    {
                        @params.SQL += " WHERE (" + viewFilters.Substring(0, viewFilters.Length - 5) + ")";
                    }
                }

                @params.SQL = @params.SQL.Replace("_!*!tabExt", "");
                @params.SQL = this.GetRetentionPermissionClause(@params.TableInfo, @params.SQL, conn);
                // this could be done via ajax.
                // If params.RequestedRows > 0 Then
                @params.TotalRowsQuery = @params.SQL;
                if (@params.NewRecord)
                {
                    @params.TotalRows = 0;
                }
                else if (!@params.IsMVCCall)
                {
                    if (@params.Paged && @params.PageIndex == 1)
                    {
                        @params.TotalRows = Query.TotalQueryRowCount(@params.SQL, conn);
                    }
                }
                if (viewColumnsSQL.Length > 1)
                    @params.SQL = Strings.Replace(@params.SQL, "select *", "select " + viewColumnsSQL.Substring(0, viewColumnsSQL.Length - 1), Compare: CompareMethod.Text);
            }

            if (@params.fromChartReq)
                return;

            if (@params.Paged)
            {
                string pageClause = "SELECT TOP ";
                if (@params.RequestedRows <= 0)
                    @params.RequestedRows = 100;
                pageClause += @params.RequestedRows.ToString() + " * FROM (SELECT ROW_NUMBER() OVER ";

                if (!string.IsNullOrEmpty(@params.ViewSort))
                {
                    pageClause += "(" + @params.ViewSort + ") AS RowNum,";
                }
                else if (@params.SortClause.Length > 0)
                {
                    pageClause += "(ORDER BY " + sqlViewTable + "." + @params.SortClause + ") AS RowNum,";
                }
                else
                {
                    for (int i = 0, loopTo = orderBy.Count - 1; i <= loopTo; i++)
                    {
                        if (orderBy[i].Length > 0)
                        {
                            if (Strings.InStr(pageClause, "ORDER BY", CompareMethod.Text) == 0)
                            {
                                pageClause += "(ORDER BY " + orderBy[i];
                            }
                            else
                            {
                                pageClause += orderBy[i];
                            }
                        }
                    }

                    if (pageClause.Contains("ORDER BY"))
                        pageClause = Strings.Left(pageClause, pageClause.Length - 1) + ", [" + @params.PrimaryKey + "] ASC) AS RowNum,";
                }

                if (!pageClause.ToUpper().Contains("ORDER BY"))
                    pageClause += "(ORDER BY " + @params.PrimaryKey + " ASC) AS RowNum,";

                @params.SQL = Strings.Replace(@params.SQL, "select", pageClause, 1, 1, CompareMethod.Text);
                @params.SQL += ") AS PagedResult WHERE RowNum > " + ((@params.PageIndex - 1) * @params.RequestedRows).ToString() + " and RowNum <= " + (@params.PageIndex * @params.RequestedRows).ToString();
                @params.SQL += " ORDER BY PagedResult.RowNum";
            }
            else
            {
                if (!string.IsNullOrEmpty(@params.ViewSort))
                {
                    @params.SQL += @params.ViewSort;
                }
                else if (@params.SortClause.Length > 0)
                {
                    if (Strings.InStr(@params.SQL, "ORDER BY", CompareMethod.Text) == 0)
                    {
                        @params.SQL += " ORDER BY " + sqlViewTable + "." + @params.SortClause;
                    }
                    else
                    {
                        @params.SQL += "," + sqlViewTable + "." + @params.SortClause;
                    }
                }
                else
                {
                    for (int i = 0, loopTo1 = orderBy.Count - 1; i <= loopTo1; i++)
                    {
                        if (orderBy[i].Length > 0)
                        {
                            if (Strings.InStr(@params.SQL, "ORDER BY", CompareMethod.Text) == 0)
                            {
                                @params.SQL += " ORDER BY " + orderBy[i];
                            }
                            else
                            {
                                @params.SQL += orderBy[i];
                            }
                        }
                    }

                    if (Conversions.ToString(@params.SQL.Last()) == ",")
                        @params.SQL = Strings.Left(@params.SQL, @params.SQL.Length - 1);
                }

                if (@params.RequestedRows == -1 | @params.RequestedRows == -2)
                {
                }
                // do not add top count for unlimited
                else if (@params.RequestedRows >= 0)
                {
                    @params.SQL = Strings.Replace(@params.SQL, "select", "select top " + @params.RequestedRows.ToString(), 1, 1, CompareMethod.Text);
                }
                else
                {
                    @params.SQL = Strings.Replace(@params.SQL, "select", "select top 50 ", 1, 1, CompareMethod.Text);
                }
            }
            // ReplaceUserNameParameter(sqlViewTable, params)
        }

        private string GetRetentionPermissionClause(DataRow tableInfo, string SQL, SqlConnection conn)
        {
            string rtn = SQL;

            if (Navigation.CBoolean(tableInfo["RetentionPeriodActive"]) || Navigation.CBoolean(tableInfo["RetentionInactivityActive"]))
            {
                bool includeInactiveRecords = false;
                bool includeArchivedRecords = false;
                bool includeDestroyedRecords = false;
                Navigation.GetRetentionPermissions(_passport.UserId, ref includeInactiveRecords, ref includeArchivedRecords, ref includeDestroyedRecords, conn);

                if (!includeInactiveRecords && Navigation.CBoolean(tableInfo["RetentionInactivityActive"]))
                    rtn = Navigation.InjectWhereIntoSQL(rtn, "([%slRetentionInactiveFinal] = 0 OR [%slRetentionInactiveFinal] IS Null)");

                if (Navigation.CBoolean(tableInfo["RetentionPeriodActive"]))
                {
                    if (!includeArchivedRecords)
                        rtn = Navigation.InjectWhereIntoSQL(rtn, "([%slRetentionDispositionStatus] <> 1 OR [%slRetentionDispositionStatus] IS Null)");
                    if (!includeDestroyedRecords)
                        rtn = Navigation.InjectWhereIntoSQL(rtn, "([%slRetentionDispositionStatus] <> 2 OR [%slRetentionDispositionStatus] IS Null)");
                }
            }

            return rtn;
        }

        private string GetSidewaysLookup(string fieldName, DataRow row, SqlConnection conn)
        {
            string str = string.Empty;
            if (!fieldName.Contains(".") || fieldName.ToLower().StartsWith(string.Concat(row["LowerTableName"].ToString(), ".").ToLower()))
                return str;
            foreach (DataRow dtrow in Navigation.GetUpperRelationships(row["LowerTableName"].ToString(), conn).Rows)
            {
                if ((dtrow["UpperTableName"].ToString() ?? "") == (fieldName.Substring(0, fieldName.IndexOf(".")) ?? ""))
                    str = " INNER JOIN " + dtrow["UpperTableName"].ToString() + " ON " + dtrow["UpperTableFieldName"].ToString() + "=" + dtrow["LowerTableFieldName"].ToString();
            }
            return str;
        }

        private string GetTrackingDisplayFields(DataRow row)
        {
            if (!string.IsNullOrEmpty(row["DescFieldNameOne"].ToString()))
            {
                var sb = new StringBuilder();

                if (!string.IsNullOrEmpty(row["DescFieldPrefixOne"].ToString()))
                {
                    sb.AppendFormat("'{0} ' + CAST([{1}] AS VARCHAR(8000)) ", row["DescFieldPrefixOne"].ToString().Replace("'", "''"), row["DescFieldNameOne"].ToString());
                }
                else
                {
                    sb.AppendFormat("CAST([{0}] AS VARCHAR(8000)) ", row["DescFieldNameOne"].ToString());
                }
                if (!string.IsNullOrEmpty(row["DescFieldNameTwo"].ToString()))
                {
                    if (!string.IsNullOrEmpty(row["DescFieldPrefixTwo"].ToString()))
                    {
                        sb.AppendFormat("+ ' {0} ' + CAST([{1}] AS VARCHAR(8000)) ", row["DescFieldPrefixTwo"].ToString().Replace("'", "''"), row["DescFieldNameTwo"].ToString());
                    }
                    else
                    {
                        sb.AppendFormat("+ ' ' + CAST([{0}] AS VARCHAR(8000)) ", row["DescFieldNameTwo"].ToString());
                    }
                }

                return sb.ToString();
            }
            else if (!string.IsNullOrEmpty(row["DescFieldNameTwo"].ToString()))
            {
                if (string.IsNullOrEmpty(row["DescFieldPrefixTwo"].ToString()))
                    return row["DescFieldNameTwo"].ToString();
                return string.Format("'{0} ' + CAST([{1}] AS VARCHAR(8000)) ", row["DescFieldPrefixTwo"].ToString().Replace("'", "''"), row["DescFieldNameTwo"].ToString());
            }
            else
            {
                return string.Format("'{0} (' + CAST({1} AS VARCHAR(8000)) + ') Display fields are not configured.'", row["TableName"].ToString(), row["IdFieldName"].ToString());
            }
        }

        private enum FullTextSearchTypes
        {
            NotDefined = 0,
            FileName = 1,
            Image = 2,
            PCFile = 3,
            COLD = 4,
            ImageContent = 5,
            PCFileContent = 6,
            COLDContent = 7,
            FieldName = 8
        }

        private string BuildViewFilterWhereClause(int columnNumber, string fieldName, string editMask, Parameters @params, DataTable dt)
        {
            if (!@params.IncludeViewFilters)
                return string.Empty;

            var rows = dt.Select("ColumnNum=" + columnNumber.ToString());
            if (rows.Count() > 0)
            {
                string filterSql = string.Empty;

                foreach (DataRow row in rows)
                {
                    if (!string.IsNullOrEmpty(row["FilterData"].ToString()))
                    {
                        if (!(row["Active"] is DBNull) && Conversions.ToBoolean(row["Active"]))
                        {
                            string OpenParen = string.Empty;
                            string CloseParen = string.Empty;
                            string joinOperator = "  AND ";

                            if (!string.IsNullOrEmpty(row["OpenParen"].ToString()))
                                OpenParen = row["OpenParen"].ToString();
                            if (!string.IsNullOrEmpty(row["CloseParen"].ToString()))
                                CloseParen = row["CloseParen"].ToString();
                            if (!string.IsNullOrEmpty(row["JoinOperator"].ToString()))
                                joinOperator = " " + row["JoinOperator"].ToString().Trim().PadRight(5, ' ');

                            filterSql += " " + OpenParen + " ";
                            filterSql += this.BuildValueClause(@params.TableName, Navigation.MakeSimpleField(fieldName), row["Operator"].ToString(), row["FilterData"].ToString(), editMask);
                            filterSql += CloseParen + joinOperator;
                        }
                    }
                }

                return filterSql;
            }
            else
            {
                return string.Empty;
            }
        }

        private DataTable ViewFilterTable(Parameters @params)
        {
            if (!@params.IncludeViewFilters)
                return null;
            string sql = "SELECT * FROM [ViewFilters] INNER JOIN [Views] ON [Views].[Id] = [ViewFilters].[ViewsId]" + " WHERE  ViewsID=@viewID" + " AND [Views].[FiltersActive] <> 0 AND [Views].[FiltersActive] IS NOT NULL";

            var cmd = new SqlCommand(sql, _passport.Connection());
            var da = new SqlDataAdapter();
            var dt = new DataTable();

            cmd.Parameters.AddWithValue("@viewID", (object)@params.ViewId);
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }

        public DataTable GetDatatableUsingSelectSQL(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return null;
            var cmd = new SqlCommand(sql, _passport.Connection());
            var da = new SqlDataAdapter();
            var dt = new DataTable();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }

        private string BuildViewFilterWhereClause(int columnNumber, string fieldName, string editMask, Parameters @params)
        {
            if (!@params.IncludeViewFilters)
                return string.Empty;

            string sql = "SELECT * FROM [ViewFilters] INNER JOIN [Views] ON [Views].[Id] = [ViewFilters].[ViewsId]" + " WHERE ColumnNum=@columnNum AND ViewsID=@viewID" + " AND [Views].[FiltersActive] <> 0 AND [Views].[FiltersActive] IS NOT NULL";

            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@columnNum", columnNumber);
                    cmd.Parameters.AddWithValue("@viewID", (object)@params.ViewId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                            return string.Empty;

                        string filterSql = string.Empty;

                        foreach (DataRow row in dt.Rows)
                        {
                            if (!(row["Active"] is DBNull) && Conversions.ToBoolean(row["Active"]))
                            {
                                string OpenParen = string.Empty;
                                string CloseParen = string.Empty;
                                string joinOperator = "  AND ";

                                if (!string.IsNullOrEmpty(row["OpenParen"].ToString()))
                                    OpenParen = row["OpenParen"].ToString();
                                if (!string.IsNullOrEmpty(row["CloseParen"].ToString()))
                                    CloseParen = row["CloseParen"].ToString();
                                if (!string.IsNullOrEmpty(row["JoinOperator"].ToString()))
                                    joinOperator = " " + row["JoinOperator"].ToString().Trim().PadRight(5, ' ');

                                filterSql += " " + OpenParen + " ";
                                filterSql += this.BuildValueClause(@params.TableName, Navigation.MakeSimpleField(fieldName), row["Operator"].ToString(), row["FilterData"].ToString(), editMask);
                                filterSql += CloseParen + joinOperator;
                            }
                        }

                        return filterSql;
                    }
                }
            }
        }

        private string BuildValueClause(string tableName, string fieldName, string filterOperator, string filterData, string editMask)
        {
            var valid = default(bool);
            var fieldLength = default(int);
            string rtn = string.Empty;
            var fieldType = Navigation.GetFieldType(tableName, fieldName, ref fieldLength, _passport);

            if (Navigation.IsAStringType(fieldType))
            {
                rtn = ViewFilterTranslation.BuildStringValue(filterData, fieldName, filterOperator, fieldLength <= 0 | fieldLength > 8000, ref valid);
            }
            else if (Navigation.IsADateType(fieldType))
            {
                rtn = ViewFilterTranslation.BuildDateValue(filterData, fieldName, filterOperator, ref valid);
            }
            else if (fieldType.Equals(typeof(bool)) | fieldType.Equals(typeof(byte)) && string.Compare(editMask, "yes/no", true) == 0)
            {
                rtn = ViewFilterTranslation.BuildCheckboxValue(filterData, fieldName, filterOperator, ref valid);
            }
            else
            {
                rtn = ViewFilterTranslation.BuildNumericValue(filterData, fieldName, filterOperator, ref valid);
            }

            if (valid)
                return rtn;
            return string.Empty;
        }

        private string BuildFullTextWhereClause(Parameters @params, string sqlViewTable, SqlConnection conn)
        {
            string tableIdStarter = "AND (IndexTableId = @TableId) ";
            string tableIdShowAll = "AND (a.IndexTableId = '{0}') ";
            string indexTypeWhereClause = "(IndexType = 8) ";
            string nodeWhereClause = string.Empty;

            if (@params.IncludeAttachments && Navigation.CBoolean(@params.TableInfo["Attachments"].ToString()))
            {
                indexTypeWhereClause = "((IndexType = 8) OR (IndexType = 6) OR (IndexType = 5)) ";

                if (!_passport.CheckPermission(@params.TableName, SecureObject.SecureObjectType.Attachments, Smead.Security.Permissions.Permission.Versioning))
                {
                    indexTypeWhereClause += Resources.FullTextWithoutPreviousVersions;
                }
            }

            if (!string.IsNullOrEmpty(@params.NodeClause))
                nodeWhereClause = "AND a.IndexTableId IN " + @params.NodeClause;
            string sql = string.Format(Resources.FullTextTemplate, "CONTAINS", string.Empty, indexTypeWhereClause, nodeWhereClause);

            if (Conversions.ToBoolean(@params.KeyValue.Length) && (@params.PrimaryKey ?? "") == (@params.KeyField ?? ""))
            {
                sql = string.Format(Resources.FullTextTemplate, "CONTAINS", tableIdStarter, indexTypeWhereClause, nodeWhereClause);
            }

            string modifiedText = Query.ProcessSearchTextRules(@params.Text);

            using (var cmd = new SqlCommand(sql, conn))
            {
                try
                {
                    cmd.CommandTimeout = Conversions.ToInteger(ConfigurationManager.AppSettings["FullTextTimeout"]);
                    if (cmd.CommandTimeout <= 0)
                        cmd.CommandTimeout = 120;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    cmd.CommandTimeout = 120;
                }

                if (Conversions.ToBoolean(@params.KeyValue.Length) && (@params.PrimaryKey ?? "") == (@params.KeyField ?? ""))
                {
                    cmd.Parameters.AddWithValue("@tableID", @params.KeyValue);
                    cmd.Parameters.AddWithValue("@searchWord", modifiedText);
                    cmd.Parameters.AddWithValue("@tableName", @params.TableName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@searchWord", modifiedText);
                    cmd.Parameters.AddWithValue("@tableName", @params.TableName);
                }

                using (var da = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        da.Fill(@params.AttachmentCache);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 7601 | ex.Number == 7616)
                        {
                            // Modified By Hemin
                            // Throw New Exception("The full text catalog and index have not been setup. Please have your administrator configure the Full Text Search Wizard.")
                            throw new Exception("The full text catalog and index have not been setup. Please have your administrator configure the Full Text Search Wizard");
                        }
                        throw;
                    }
                }
            }

            if (@params.AttachmentCache.Rows.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            if (Navigation.GetFieldDataType(@params.TableName, @params.PrimaryKey, conn) == "System.String")
            {
                sb.AppendFormat("[{0}].[{1}] ", sqlViewTable, @params.PrimaryKey);
            }
            else
            {
                sb.AppendFormat("CAST([{0}].[{1}] AS VARCHAR) ", sqlViewTable, @params.PrimaryKey);
            }

            if (Conversions.ToBoolean(@params.KeyValue.Length) && (@params.PrimaryKey ?? "") == (@params.KeyField ?? ""))
            {
                sb.AppendFormat(Resources.FullTextShowAllClause, @params.TableName, indexTypeWhereClause, string.Format(tableIdShowAll, @params.KeyValue.ToString()), nodeWhereClause, modifiedText);
            }
            else
            {
                sb.AppendFormat(Resources.FullTextShowAllClause, @params.TableName, indexTypeWhereClause, nodeWhereClause, string.Empty, modifiedText);
            }

            string whereClause = sb.ToString();

            while (whereClause.Contains(Constants.vbCrLf + Constants.vbCrLf))
                whereClause = whereClause.Replace(Constants.vbCrLf + Constants.vbCrLf, Constants.vbCrLf);

            return whereClause;
        }

        public static string BuildFullTextWhereClauseForSelectAll(Parameters @params, string sqlViewTable, SqlConnection conn)
        {
            if (@params.Data.Rows.Count == 1)
            {
                if (string.IsNullOrWhiteSpace(@params.KeyValue))
                    return string.Empty;
                if (Navigation.GetFieldDataType(@params.TableName, @params.PrimaryKey, conn) == "System.String")
                    return string.Format("[{0}] = '{1}'", @params.PrimaryKey, @params.KeyValue.Replace("'", "''"));
                return string.Format("[{0}] = {1}", @params.PrimaryKey, @params.KeyValue.Replace("'", "''"));
            }

            string tableIdStarter = "AND (IndexTableId = @TableId) ";
            string tableIdShowAll = "AND (a.IndexTableId = '{0}') ";
            string indexTypeWhereClause = "(IndexType = 8) ";
            string nodeWhereClause = string.Empty;

            if (@params.IncludeAttachments && Navigation.CBoolean(@params.TableInfo["Attachments"].ToString()))
            {
                indexTypeWhereClause = "((IndexType = 8) OR (IndexType = 6) OR (IndexType = 5)) ";

                if (!_passport.CheckPermission(@params.TableName, SecureObject.SecureObjectType.Attachments, Smead.Security.Permissions.Permission.Versioning))
                {
                    indexTypeWhereClause += Resources.FullTextWithoutPreviousVersions;
                }
            }

            if (!string.IsNullOrEmpty(@params.NodeClause))
                nodeWhereClause = "AND a.IndexTableId IN " + @params.NodeClause;
            string sql = string.Format(Resources.FullTextTemplate, "CONTAINS", string.Empty, indexTypeWhereClause, nodeWhereClause);

            if (Conversions.ToBoolean(@params.KeyValue.Length) && (@params.PrimaryKey ?? "") == (@params.KeyField ?? ""))
            {
                sql = string.Format(Resources.FullTextTemplate, "CONTAINS", tableIdStarter, indexTypeWhereClause, nodeWhereClause);
            }

            string modifiedText = Query.ProcessSearchTextRules(@params.Text);

            using (var cmd = new SqlCommand(sql, conn))
            {
                try
                {
                    cmd.CommandTimeout = Conversions.ToInteger(ConfigurationManager.AppSettings["FullTextTimeout"]);
                    if (cmd.CommandTimeout <= 0)
                        cmd.CommandTimeout = 120;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    cmd.CommandTimeout = 120;
                }

                if (Conversions.ToBoolean(@params.KeyValue.Length) && (@params.PrimaryKey ?? "") == (@params.KeyField ?? ""))
                {
                    cmd.Parameters.AddWithValue("@tableID", @params.KeyValue);
                    cmd.Parameters.AddWithValue("@searchWord", modifiedText);
                    cmd.Parameters.AddWithValue("@tableName", @params.TableName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@searchWord", modifiedText);
                    cmd.Parameters.AddWithValue("@tableName", @params.TableName);
                }

                using (var da = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        da.Fill(@params.AttachmentCache);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 7601 | ex.Number == 7616)
                        {
                            // Modified By Hemin
                            // Throw New Exception("The full text catalog and index have not been setup. Please have your administrator configure the Full Text Search Wizard.")
                            throw new Exception("The full text catalog and index have not been setup. Please have your administrator configure the Full Text Search Wizard");
                        }
                        throw;
                    }
                }
            }

            if (@params.AttachmentCache.Rows.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            if (Navigation.GetFieldDataType(@params.TableName, @params.PrimaryKey, conn) == "System.String")
            {
                sb.AppendFormat("[{0}].[{1}] ", sqlViewTable, @params.PrimaryKey);
            }
            else
            {
                sb.AppendFormat("CAST([{0}].[{1}] AS VARCHAR) ", sqlViewTable, @params.PrimaryKey);
            }

            if (Conversions.ToBoolean(@params.KeyValue.Length) && (@params.PrimaryKey ?? "") == (@params.KeyField ?? ""))
            {
                sb.AppendFormat(Resources.FullTextShowAllClause, @params.TableName, indexTypeWhereClause, string.Format(tableIdShowAll, @params.KeyValue.ToString()), nodeWhereClause, modifiedText);
            }
            else
            {
                sb.AppendFormat(Resources.FullTextShowAllClause, @params.TableName, indexTypeWhereClause, nodeWhereClause, string.Empty, modifiedText);
            }

            string whereClause = sb.ToString();

            while (whereClause.Contains(Constants.vbCrLf + Constants.vbCrLf))
                whereClause = whereClause.Replace(Constants.vbCrLf + Constants.vbCrLf, Constants.vbCrLf);

            return whereClause;
        }

        private static string ProcessSearchTextRules(string searchText)
        {
            int i = 0;
            var quoted = default(bool);
            string buffer = searchText.Replace("'", "''").ToLower().Trim();

            searchText = searchText.Replace("'", "''");
            // convert qoutes to 255 to protect them 
            foreach (char c in searchText)
            {
                i += 1;
                if (Conversions.ToString(c) == "\"")
                    quoted = !quoted;
                if (quoted && c == ' ')
                {
                    var midTmp = Conversions.ToString(Strings.Chr(255));
                    StringType.MidStmtStr(ref buffer, i, 1, midTmp);
                }
            }

            if (quoted)
            {
                if (buffer.StartsWith("\"") && buffer.EndsWith("\""))
                {
                    buffer = buffer.Substring(1, buffer.Length - 2).Replace("\"", "\"\"").Replace(" ", "\" \"");
                }
                else
                {
                    buffer = buffer.Replace("\"", "\"\"");
                }
            }
            // clean spaces
            while (buffer.Contains("  "))
                buffer = buffer.Replace("  ", " ");
            // strip noise words if not quoted
            // If Not quoted Then buf = StripNoiseWords(buf)
            // add the ands in the right place
            buffer = Strings.Replace(buffer, " and ", " ", 1, -1, CompareMethod.Text);
            buffer = Strings.Replace(buffer, " ", " AND ");
            buffer = Strings.Replace(buffer, " AND or AND ", " OR ");

            i = 0;
            var words = Strings.Split(buffer);

            foreach (string word in words)
            {
                var Word= word;

                Word = Word.Trim();
                if (string.Compare(Word, "AND") == 0 || string.Compare(Word, "OR") == 0 || Word.Contains("\""))
                    Word = Word;
                else
                    words[i] = "\"" + Word + "\"";
                i += 1;
            }

            buffer = Strings.Join(words, " ");
            if (quoted)
                return "\"" + buffer.Replace(Strings.Chr(255), ' ') + "\"";
            return buffer.Replace(Strings.Chr(255), ' ');
        }

        private string StripNoiseWords(string searchText)
        {
            var stopWords = new[] { "$,0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,about,after,all,also,an,and,another,any,are,as,at,be,because,been,before,being,between,both,but,by,came,can,come,could,did,do,does,each,else,for,from,get,got,had,has,have,he,her,here,him,himself,his,how,if,in,into,is,it,its,just,like,makemany,me,might,more,most,much,must,my,never,no,now,of,on,only,or,other,our,out,over,re,said,same,see,should,since,so,some,still,such,take,than,that,the,their,them,then,there,these,they,this,those,through,to,too,under,up,use,very,want,was,way,we,well,were,what,when,where,which,while,who,will,with,would,you,your" };
            var words = searchText.Split(' ');
            string cleanWords = string.Empty;

            foreach (var word in words)
            {
                if (!stopWords.Contains(word))
                    cleanWords = cleanWords + word + " ";
            }

            return cleanWords.Trim();
        }

        private string GetDispositionSQL(DataRow tableInfo)
        {
            try
            {
                if (!Navigation.CBoolean(tableInfo["RetentionPeriodActive"]))
                    return string.Empty;
                return " DispositionStatus = CASE ISNULL([%slRetentionDispositionStatus], 0) " + " WHEN 0 THEN 'Active'" + " WHEN 1 THEN 'Archived'" + " WHEN 2 THEN 'Destroyed'" + " WHEN 3 THEN 'Purged'" + " ELSE 'Error'" + " END,";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private string GetItemNameSQL(string TableName, DataRow drTableInfo, string dateFormat) // heymorgan
        {
            string desc0 = string.Empty;
            string desc1 = string.Empty;
            string desc2 = string.Empty;
            string desc3 = string.Empty;

            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameOne"].ToString()))
            {
                if ((drTableInfo["DescFieldNameOne"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(drTableInfo) ?? "") & drTableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc1 = "'" + drTableInfo["BarCodePrefix"].ToString() + "' + RIGHT('000000' + CAST([" + Navigation.MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + "] AS VARCHAR(255)), 6) ";
                }
                else
                {
                    if (Navigation.GetFieldDataType(TableName, drTableInfo["DescFieldNameOne"].ToString(), _passport) == "System.DateTime")
                        desc1 = "FORMAT([" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameOne"].ToString()) + "], '" + dateFormat + "')";
                    else
                        desc1 = "CAST([" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameOne"].ToString()) + "] AS VARCHAR(255))";
                }
            }

            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameTwo"].ToString()))
            {
                if ((drTableInfo["DescFieldNameTwo"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(drTableInfo) ?? "") & drTableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc2 = "'" + drTableInfo["BarCodePrefix"].ToString() + "' + RIGHT('000000' + CAST([" + Navigation.MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + "] AS VARCHAR(255)), 6) ";
                }
                else
                {
                    if (Navigation.GetFieldDataType(TableName, drTableInfo["DescFieldNameTwo"].ToString(), _passport) == "System.DateTime")
                        desc2 = "FORMAT([" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameTwo"].ToString()) + "], '" + dateFormat + "')";
                    else
                        desc2 = "CAST([" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameTwo"].ToString()) + "] AS VARCHAR(255))";
                }
            }

            if (desc1.Length > 0 & desc2.Length > 0)
            {
                return "CASE WHEN (NOT [" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameOne"].ToString()) + "] IS NULL) AND (NOT [" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameTwo"].ToString()) + "] IS NULL) THEN (RTRIM(" + desc1 + ") + ' - ' + RTRIM(" + desc2 + ")) " + "WHEN (NOT [" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameOne"].ToString()) + "] IS NULL) THEN RTRIM(" + desc1 + ") ELSE RTRIM(" + desc2 + ") END";
            }
            else if (desc1.Length > 0)
            {
                return "(RTRIM(" + desc1 + "))";
            }
            else if (desc2.Length > 0)
            {
                return "(RTRIM(" + desc2 + "))";
            }
            else
            {
                return "CAST([" + Navigation.MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + "] AS VARCHAR(255)) + ' | Configure display fields.'";
            }
        }

        private string GetDescriptionsSQL(string TableName, DataRow drTableInfo)
        {
            string desc0 = string.Empty;
            string desc1 = string.Empty;
            string desc2 = string.Empty;
            string desc3 = string.Empty;

            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameOne"].ToString()))
            {
                if ((drTableInfo["DescFieldNameOne"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(drTableInfo) ?? "") & drTableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc1 = "'" + drTableInfo["BarCodePrefix"].ToString() + "' + RIGHT('000000' + CAST([" + Navigation.MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + "] AS VARCHAR(255)), 6) ";
                }
                else
                {
                    desc1 = "CAST([" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameOne"].ToString()) + "] AS VARCHAR(255))";
                }
            }

            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameTwo"].ToString()))
            {
                if ((drTableInfo["DescFieldNameTwo"].ToString() ?? "") == (Navigation.GetPrimaryKeyFieldName(drTableInfo) ?? "") & drTableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc2 = "'" + drTableInfo["BarCodePrefix"].ToString() + "' + RIGHT('000000' + CAST([" + Navigation.MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + "] AS VARCHAR(255)), 6) ";
                }
                else
                {
                    desc2 = "CAST([" + Navigation.MakeSimpleField(drTableInfo["DescFieldNameTwo"].ToString()) + "] AS VARCHAR(255))";
                }
            }

            if (desc1.Length > 0 & desc2.Length > 0)
            {
                return "(RTRIM(" + desc1 + ")) AS ProcessedDescFieldNameOne, (RTRIM(" + desc2 + ")) AS ProcessedDescFieldNameTwo,";
            }
            else if (desc1.Length > 0)
            {
                return "(RTRIM(" + desc1 + ")) AS ProcessedDescFieldNameOne, '' AS ProcessedDescFieldNameTwo,";
            }
            else if (desc2.Length > 0)
            {
                return "(RTRIM(" + desc2 + ")) AS ProcessedDescFieldNameTwo, '' AS ProcessedDescFieldNameOne,";
            }
            else
            {
                return "CAST([" + Navigation.MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + "] AS VARCHAR(255)) +' | Configure display fields.'  AS ProcessedDescFieldNameOne, '' AS ProcessedDescFieldNameTwo,";
            }
        }

        public static string ConvertUSDateToLocal(string text, CultureInfo culture, string format = "d")
        {
            if (Information.IsDate(text))
                return DateTime.Parse(text, new CultureInfo("en-US")).ToString(format, culture);
            return text;
        }

        public static string GetDateProcessedItemName(DataRow dr, Parameters @params)
        {
            string processedItemName = string.Empty;
            string processedItemName1 = string.Empty;
            string processedItemName2 = string.Empty;

            if (!string.IsNullOrEmpty(dr["ProcessedDescFieldNameTwo"].ToString()))
            {
                if (dr["ProcessedDescFieldNameTwo"].GetType().Name == "DateTime")
                {
                    processedItemName2 = Query.ConvertUSDateToLocal(dr["ProcessedDescFieldNameTwo"].ToString(), @params.Culture, "f");
                }
                else
                {
                    processedItemName2 = dr["ProcessedDescFieldNameTwo"].ToString();
                }
            }
            if (!string.IsNullOrEmpty(dr["ProcessedDescFieldNameOne"].ToString()))
            {
                if (dr["ProcessedDescFieldNameOne"].GetType().Name == "DateTime")
                {
                    processedItemName1 = Query.ConvertUSDateToLocal(dr["ProcessedDescFieldNameOne"].ToString(), @params.Culture, "f");
                }
                else
                {
                    processedItemName1 = dr["ProcessedDescFieldNameOne"].ToString();
                }
                if (!string.IsNullOrEmpty(dr["ProcessedDescFieldNameTwo"].ToString()))
                {
                    processedItemName = processedItemName1 + " - " + processedItemName2;
                }
                else
                {
                    processedItemName = processedItemName1;
                }
            }
            else if (!string.IsNullOrEmpty(dr["ProcessedDescFieldNameTwo"].ToString()))
            {
                processedItemName = processedItemName2;
            }

            return processedItemName;
        }
        public static async Task<int> TotalQueryRowCountAsync(string sql, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) " + Strings.Right(sql, sql.Length - Strings.InStr(sql, " FROM ", CompareMethod.Text)), conn))
            {
                cmd.CommandTimeout = CommandTimeOut;

                try
                {
                    return Conversions.ToInteger(await cmd.ExecuteScalarAsync());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return 0;
                }
            }

        }

        public static int TotalQueryRowCount(string sql, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) " + Strings.Right(sql, sql.Length - Strings.InStr(sql, " FROM ", CompareMethod.Text)), conn))
            {
                cmd.CommandTimeout = CommandTimeOut;

                try
                {
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return 0;
                }
            }
            
        }



        public static int TotalQueryRowCountMVC(string sql, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) " + sql, conn))
            {
                cmd.CommandTimeout = CommandTimeOut;

                try
                {
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return 0;
                }
            }
        }

        public static int TotalQueryRowCount(string sql)
        {
            using (var conn = _passport.Connection())
            {
                return TotalQueryRowCount(sql, conn);
            }
        }

        public static int TotalQueryRowCount(SqlCommand cmd)
        {
            using (var newCmd = cmd.Clone())
            {
                newCmd.CommandTimeout = CommandTimeOut;
                newCmd.CommandText = "SELECT COUNT(*) " + Strings.Right(cmd.CommandText, cmd.CommandText.Length - Strings.InStr(cmd.CommandText, " FROM", CompareMethod.Text));

                if (newCmd.CommandText.ToUpper().Contains(" ORDER BY "))
                {
                    newCmd.CommandText = newCmd.CommandText.Substring(0, Strings.InStr(newCmd.CommandText, " ORDER BY ", CompareMethod.Text));
                }

                try
                {
                    return Conversions.ToInteger(newCmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return 0;
                }
            }
        }

        public int TotalQueryRowCountTest(SqlCommand cmd)
        {
            using (var conn = _passport.Connection())
            {
                using (var newCmd = new SqlCommand(string.Empty, conn))
                {
                    cmd.CommandTimeout = CommandTimeOut;
                    newCmd.CommandText = "SELECT TOP 1000 COUNT(*) " + Strings.Right(cmd.CommandText, cmd.CommandText.Length - Strings.InStr(cmd.CommandText, " FROM", CompareMethod.Text));
                    if (newCmd.CommandText.ToUpper().Contains(" ORDER BY "))
                    {
                        newCmd.CommandText = newCmd.CommandText.Substring(0, Strings.InStr(newCmd.CommandText, " ORDER BY ", CompareMethod.Text));
                    }
                    foreach (SqlParameter param in cmd.Parameters)
                        newCmd.Parameters.AddWithValue(param.ParameterName, param.Value);

                    return Conversions.ToInteger(newCmd.ExecuteScalar());
                }
            }
        }

        private static bool IncludeColumn(string columnName)
        {
            columnName = Navigation.MakeSimpleField(columnName);

            if (columnName.ToLower().StartsWith("%sl"))
                return false;
            if (columnName.ToLower().StartsWith("sl"))
                return false;
            if (columnName.ToLower().Contains("rowtimestamp"))
                return false;
            return true;
        }

        private string GetMaskedID(string tableName)
        {

            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT IdFieldName FROM TABLES WHERE TableName = @tableName", conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    string idField = cmd.ExecuteScalar().ToString();

                    cmd.CommandText = "SELECT TOP 1 ISNULL(EditMask, '') FROM ViewColumns WHERE FieldName = @fieldName";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@fieldName", idField);
                    string editMask = string.Empty;

                    try
                    {
                        editMask = cmd.ExecuteScalar().ToString();
                        editMask = Strings.Replace(editMask, @"\", "");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        editMask = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(editMask))
                    {
                        string testMask = string.Format(" LEFT('{0}', LEN('{0}') - LEN(CAST({1} AS char))) + CAST({1} AS CHAR) ", editMask, idField);
                        cmd.CommandText = string.Format("SELECT {0} FROM [{1}] WHERE 0 = 1", testMask, tableName);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            return testMask;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return idField.ToString();
                        }
                    }

                    return idField.ToString();
                }
            }
        }

        private bool HasRetention(string tableName)
        {
            bool HasRetentionRet = default;
            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT ISNULL(RetentionPeriodActive, 0) FROM Tables WHERE UserName = @tableName", conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    HasRetentionRet = Conversions.ToBoolean(cmd.ExecuteScalar());
                }
            }

            return HasRetentionRet;
        }

        private string GetAttachmentCountSQL(string tableName, string sqlViewName)
        {
            return "(SELECT COUNT(*) FROM [Userlinks] GROUP BY [IndexTableId], [IndexTable] HAVING ([IndexTableId] = [" + sqlViewName + "].[" + Navigation.MakeSimpleField(Navigation.GetPrimaryKeyFieldName(tableName, _passport).ToString()) + "]) AND ([IndexTable] = '" + tableName + "'))";
        }

        private string GetAttachmentCountSQL(DataRow tableInfo, string sqlViewName, bool isString)
        {
            if (!Navigation.CBoolean(tableInfo["Attachments"].ToString()))
                return "NULL";
            string sql = "(SELECT COUNT(*) FROM [Userlinks] GROUP BY [IndexTableId], [IndexTable] HAVING ([IndexTableId] = ";

            if (isString)
            {
                sql += "[" + sqlViewName + "].[" + Navigation.MakeSimpleField(Navigation.GetPrimaryKeyFieldName(tableInfo).ToString()) + "]";
            }
            else
            {
                sql += "RIGHT('000000000000000000000000000000' + CONVERT(VARCHAR,[" + sqlViewName + "].[" + Navigation.MakeSimpleField(Navigation.GetPrimaryKeyFieldName(tableInfo).ToString()) + "]), 30)";
            }

            return sql + ") AND ([IndexTable] = '" + tableInfo["tableName"].ToString() + "'))";
        }

        private void ProcessRegularSearchResults(List<Parameters> results, bool thinClient, SearchEnum searchType = SearchEnum.HTML)
        {
            ProcessRegularSearchResults(results, thinClient, 500, searchType);
        }

        private void ProcessRegularSearchResults(List<Parameters> results, bool thinClient, int requestedRows, SearchEnum searchType = SearchEnum.HTML, bool isMvccall = false)
        {
            switch (searchType)
            {
                case SearchEnum.HTML:
                    {
                        _HTMLSearchResults = "";
                        if (results.Count == 0)
                        {
                            // Modified By Hemin
                            // _HTMLSearchResults = "<div style='font-size: 14pt; vertical-align: top;'>No matching results found.</div>"
                            _HTMLSearchResults = "<div style='font-size: 14pt; vertical-align: top;'>No matching results found </div>";
                            // If thinClient Then _HTMLSearchResults &= "<br><br><a href='data.aspx' style='color: blue; font-size: 10pt; font-weight: 100;'>Go Back</a>"
                            return;
                        }

                        var htmlBuffer = new StringBuilder();
                        int counter;

                        foreach (Parameters result in results)
                        {
                            if (result.Data.Rows.Count > 0)
                            {
                                counter = 1;
                                htmlBuffer.Append("<tr><td style='padding-top: 10px; font-size: 12pt;'><b>");
                                if (result.Data.Rows.Count > requestedRows)
                                {
                                    // Modified By Hemin
                                    // htmlBuffer &= String.Format("Display limited to {0} results found in '{1}'", requestedRows.ToString, result.ViewName)
                                    htmlBuffer.Append(string.Format("Display limited to {0} results found in '{1}'", requestedRows.ToString(), result.ViewName));
                                }
                                else if (result.Data.Rows.Count == 1)
                                {
                                    // Modified By Hemin
                                    // htmlBuffer &= String.Format("{0} result found in '{1}'", result.Data.Rows.Count.ToString, result.ViewName)
                                    htmlBuffer.Append(string.Format("{0} result found in '{1}'", result.Data.Rows.Count.ToString(), result.ViewName));
                                }
                                else
                                {
                                    // Modified By Hemin
                                    // htmlBuffer &= String.Format("{0} results found in '{1}'", result.Data.Rows.Count.ToString, result.ViewName)
                                    htmlBuffer.Append(string.Format("First {0} results found in '{1}'", result.Data.Rows.Count.ToString(), result.ViewName));
                                }
                                // Modified By hemin
                                // htmlBuffer &= "&nbsp;&nbsp;" & "<a href=handler.aspx?t=" & result.ViewId & "&v=&search=1&showall=" & result.Text.Replace(" ", "%20") &
                                // "&include=" & Math.Abs(CInt(result.IncludeAttachments)) &
                                // " style='color: darkblue;font-weight: 100; text-decoration: underline; font-size: 7pt;' tooltip='Load matching results for this view.'>Show All</a></b></td></tr>"

                                if (isMvccall)
                                {
                                    htmlBuffer.Append(string.Format("&nbsp;&nbsp;<a onclick='obJglobalsearch.SearchAllClick(this, {0}, 1, \"{1}\", {2})'", (object)result.ViewId, System.Net.WebUtility.UrlEncode(result.Text), Math.Abs(Conversions.ToInteger(result.IncludeAttachments))));
                                }
                                else
                                {
                                    htmlBuffer.Append(string.Format("&nbsp;&nbsp;<a href=handler.aspx?t={0}&v=&search=1&showall={1}&include={2}", (object)result.ViewId, System.Net.WebUtility.UrlEncode(result.Text), Math.Abs(Conversions.ToInteger(result.IncludeAttachments))));
                                }

                                htmlBuffer.Append(string.Format(" style='color: darkblue;font-weight: 100; text-decoration: underline; font-size: 7pt;' tooltip='{0}'>{1}</a></b></td></tr>", "Load matching results for this view", "Show All"));

                                foreach (DataRow row in result.Data.Rows)
                                {
                                    htmlBuffer.Append(string.Format("<tr><td style='padding-left: 15px;'>{0}</td></tr>{1}", HTMLResult(result, row, counter, isMvccall), Constants.vbCrLf));
                                    counter += 1;
                                }
                            }
                        }

                        _HTMLSearchResults = string.Format("<table style='font-names: calibri;'>{0}</table>", htmlBuffer.ToString());
                        break;
                    }
                case SearchEnum.XML:
                    {
                        string resultCount = "";
                        // Modified By Hemin
                        // _XMLSearchResults = "<?xml version=""1.0"" encoding=""UTF-8""?>" &
                        // "<rss version=""2.0""  xmlns:opensearch = ""http://a9.com/-/spec/opensearch/1.1/""  xmlns:atom=""http://www.w3.org/2005/Atom"">" &
                        // "<channel>" &
                        // "<title>TAB FusionRMS Search</title>"

                        _XMLSearchResults = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<rss version=\"2.0\"  xmlns:opensearch = \"http://a9.com/-/spec/opensearch/1.1/\"  xmlns:atom=\"http://www.w3.org/2005/Atom\">" + "<channel>" + "<title>" + string.Format("{0} Search", "TAB FusionRMS") + "</title>";

                        if (results.Count == 0)
                        {
                            // Modified By hemin
                            // _XMLSearchResults += "<description>No matching results found.</description>"

                            _XMLSearchResults += "<description>No matching results found</description>";
                        }
                        else
                        {
                            // Modified by hemin
                            // _XMLSearchResults += "<description>Search results for """ + results(0).KeyValue + """</description>"

                            _XMLSearchResults += "<description>" + string.Format("Search results for '{0}'", results[0].KeyValue) + "</description>";
                            resultCount = results.Count.ToString();
                        }

                        _XMLSearchResults += "<opensearch:totalResults>" + results.Count.ToString() + "</opensearch:totalResults>" + " <opensearch:startIndex>1</opensearch:startIndex>" + "  <opensearch:itemsPerPage>25</opensearch:itemsPerPage>" + " <atom:link rel=\"search\" type=\"application/opensearchdescription+xml\" href=\"http://example.com/opensearchdescription.xml\"/>" + " <opensearch:Query role=\"request\" searchTerms=\"" + resultCount + "\" startPage=\"1\" />";
                        foreach (Parameters result in results)
                        {
                            foreach (DataRow row in result.Data.Rows)
                                _XMLSearchResults += XMLResult(result, row);
                        }

                        _XMLSearchResults += "</channel></rss>";
                        _XMLSearchResults = _XMLSearchResults.Replace("&", "&amp;");
                        break;
                    }
                case SearchEnum.API:
                    {
                        _APISearchResults = string.Empty;

                        foreach (Parameters result in results)
                        {
                            foreach (DataRow row in result.Data.Rows)
                                _APISearchResults += string.Format("{0},{1},{2}{3}", result.TableName, row["pkey"].ToString(), AttachmentLinks(result, row, string.Empty), Constants.vbCrLf);
                        }

                        break;
                    }
            }
        }

        private string AttachmentLinks(Parameters @params, DataRow resultRow, string href)
        {
            var foundIt = default(bool);
            var http = default(HttpContext);

            try
            {
                http = new HttpContextAccessor().HttpContext;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            foreach (DataRow attachRow in @params.AttachmentCache.Rows)
            {
                if ((resultRow["pkey"].ToString().ToLower() ?? "") == (attachRow["IndexTableID"].ToString().ToLower() ?? ""))
                {
                    switch ((FullTextSearchTypes)Conversions.ToInteger(attachRow["IndexType"]))
                    {
                        case FullTextSearchTypes.FileName:
                            {
                                break;
                            }
                        case FullTextSearchTypes.ImageContent:
                        case FullTextSearchTypes.PCFileContent:
                            {
                                foundIt = true;
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }

                    if (foundIt)
                    {
                        string attachmentNumber = attachRow["AttachmentNumber"].ToString();
                        string version = Conversions.ToInteger(attachRow["recordversion"]).ToString();
                        string pageNumber = attachRow["pageNumber"].ToString();

                        if (string.IsNullOrEmpty(href))
                        {
                            return string.Format("{0},{1},{2},True", attachmentNumber, version, pageNumber);
                        }
                        else
                        {
                            string tableName = @params.TableName;
                            string tableId = resultRow["pkey"].ToString();
                            string userId = _passport.UserId.ToString();
                            string ticket = _passport.get_CreateTicket(_passport.ServerAndDatabaseName, tableName, tableId).ToString();
                            string pass = Navigation.EncryptString(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}", Navigation.DelimiterText, ticket, userId, _passport.ServerAndDatabaseName, tableName, tableId, attachmentNumber, version, pageNumber, this.ViewerSafeSearchString(@params.Text)));

                            if (!@params.ThinClient)
                                href += "&desktop=1";
                            string strAttachmentLink = string.Format("(<a onClick=\"document.location.href='{0}';\" href='undocked.aspx?s=1&v={1}' name='undocked' target='undocked' style='text-decoration: underline; font-weight: 800; font-size: 8pt; color:#C35817 ;'>{2}</a>)", href, pass, "found in attachment");


                            if (@params.ThinClient)
                            {
                                // Dim browser As System.Web.HttpBrowserCapabilities = HttpContext.Current.Request.Browser
                                if (http is not null)
                                {
                                    if (http.Request.Headers["User-Agent"].ToString().Contains("MSIE") || http.Request.Headers["User-Agent"].ToString().Contains("Trident"))
                                    {
                                    }
                                    // use strAttachmentLink as is
                                    else
                                    {
                                        string encryptURL = Navigation.EncryptURLParameters(string.Format("id={0}&Table={1}&attachment={2}&itemname={3}", tableId, tableName, attachmentNumber.ToString(), resultRow["ItemName"].ToString()));
                                        strAttachmentLink = string.Format("(<a onclick=\"setCookie('searchInput', $('#DialogSearchInput').val())\" href='/DocumentViewer/Index?documentKey={0}' name='undocked' target='undocked' style='text-decoration: underline; font-weight: 800; font-size: 8pt; color:#C35817 ;'>{1}</a>)", encryptURL, "found in attachment");
                                    }
                                }
                            }

                            return strAttachmentLink;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(href))
                return "0,0,0,False";
            return string.Empty;
        }

        private string ViewerSafeSearchString(string searchText)
        {
            // searchText = Replace(searchText, "*", "", 1, -1, CompareMethod.Text)
            searchText = Strings.Replace(searchText, " or ", " ", 1, -1, CompareMethod.Text);
            searchText = Strings.Replace(searchText, " and ", " ", 1, -1, CompareMethod.Text);
            searchText = Strings.Replace(searchText, " ", "%20", 1, -1, CompareMethod.Text);
            return searchText;
        }

        private string HTMLResult(Parameters @params, DataRow row, int count, bool isMVCCall = false)
        {
            string HTMLResultRet = default;
            // Static count As Integer
            string linkHref;

            if (isMVCCall)
            {
                linkHref = string.Format("onclick=\"obJglobalsearch.SearchClick(this,{0},'{1}',{2})\"", (object)@params.ViewId, System.Net.WebUtility.UrlEncode(row["pKey"].ToString()), 1);
            }
            else
            {
                linkHref = string.Format("handler.aspx?t={0}&v={1}&search=1", (object)@params.ViewId, row["pKey"].ToString());
                linkHref = linkHref.Replace(" ", "%20");
            }

            string DescriptionText = string.Empty;
            // See TFS issue #369
            string pKey = @params.PrimaryKey;
            if (@params.Data.PrimaryKey.Length > 0)
                pKey = @params.Data.PrimaryKey[0].ToString();

            foreach (DataColumn col in @params.Data.Columns)
            {
                if (col.ColumnName.ToLower() != "formattedid" && col.ColumnName.ToLower() != "id" && col.ColumnName.ToLower() != "itemname" && (col.ColumnName.ToLower() ?? "") != (pKey.ToLower() ?? ""))
                {
                    if (col.DataType.FullName == "System.String" && !string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                    {
                        DescriptionText += (row[col.ColumnName].ToString().Length > 30 ? row[col.ColumnName].ToString().Substring(0, 30) + "..." : row[col.ColumnName].ToString()) + ", ";
                    }
                    else if (col.ColumnName.ToLower() != "slrequestable" && !string.IsNullOrEmpty(row[col.ColumnName].ToString()) && Information.IsNumeric(row[col.ColumnName]) && Conversions.ToDouble(row[col.ColumnName]) > 0d)
                    {
                        DescriptionText += row[col.ColumnName].ToString() + ", ";
                    }
                }
            }

            DescriptionText = HighlightWords(@params, DescriptionText);
            if (DescriptionText.EndsWith(", "))
                DescriptionText = DescriptionText.Substring(0, DescriptionText.Length - 2);

            string linkText = HighlightWords(@params, row["ItemName"].ToString());
            // count = count + 1
            if (isMVCCall)
            {
                HTMLResultRet = count.ToString() + ". <a style='text-decoration: underline; font-size: 10pt; color: #2200d5;' " + linkHref + ">" + (linkText.Length == 0 ? row["pkey"].ToString() : linkText) + "</a>";
            }
            else
            {
                HTMLResultRet = count.ToString() + ". <a style='text-decoration: underline; font-size: 10pt; color: #2200d5;' href=" + linkHref + ">" + (linkText.Length == 0 ? row["pkey"].ToString() : linkText) + "</a>";
            }

            HTMLResultRet += "&nbsp;&nbsp;" + AttachmentLinks(@params, row, linkHref);
            HTMLResultRet += "<div style='font-size: 9pt;'>" + DescriptionText + "</div>";
            DescriptionText = Query.HighlightWords(@params, "Table: " + @params.TableName + ", ID: " + row["pkey"].ToString());
            HTMLResultRet += "<div style='font-size: 8pt; color: green;'>" + DescriptionText + "</div>";
            return HTMLResultRet;
        }

        private string XMLResult(Parameters @params, DataRow row)
        {
            string qryString = string.Format("t={0}&v={1}&search=1", (object)@params.ViewId, row["pKey"].ToString());

            // qryString = "?enc=" & HttpUtility.UrlEncode(Encrypt.AesEncrypt(qryString))
            qryString = "?enc=" + System.Net.WebUtility.UrlEncode(Encrypt.AesEncrypt(qryString));

            // Sharepoint relative path
            // Dim linkHref As String = String.Format("../../SitePages/Result.aspx?t={0}&v={1}&search=1", params.ViewID, row("pKey").ToString)
            string linkHref = "../../Pages/FusionRMS.aspx" + qryString;

            string descriptionText = string.Empty;
            string pKey = @params.PrimaryKey;
            if (@params.Data.PrimaryKey.Length > 0)
                pKey = @params.Data.PrimaryKey[0].ToString();

            foreach (DataColumn col in @params.Data.Columns)
            {
                if (col.ColumnName.ToLower() != "formattedid" & col.ColumnName.ToLower() != "id" & col.ColumnName.ToLower() != "itemname" & (col.ColumnName.ToLower() ?? "") != (pKey.ToLower() ?? ""))
                {
                    if (col.DataType.FullName == "System.String" & !string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                    {
                        // descriptionText &= row(col.ColumnName).ToString & ", "
                        descriptionText += (row[col.ColumnName].ToString().Length > 30 ? row[col.ColumnName].ToString().Substring(0, 30) + "..." : row[col.ColumnName].ToString()) + ", ";
                    }
                }
            }

            descriptionText = HighlightWords(@params, descriptionText);
            if (descriptionText.EndsWith(", "))
                descriptionText = descriptionText.Substring(0, descriptionText.Length - 2);
            string linkText = HighlightWords(@params, row["ItemName"].ToString());

            string rtn = "<item><title>" + (linkText.Length == 0 ? row["pkey"].ToString() : linkText) + "</title><link>" + linkHref + "</link>";
            return rtn + "<description><![CDATA[" + descriptionText + "<br />Table:" + @params.TableName + ", ID:" + (row["pkey"].ToString() + "]]></description></item>");
        }

        private static string CorrectForUnmatchedQuotes(string keyWord)
        {
            int position = -1;

            if (keyWord.StartsWith("\"") && keyWord.EndsWith("\""))
            {
                position = keyWord.Substring(1, keyWord.Length - 2).IndexOf("\"");
                if (position == -1)
                    keyWord = keyWord.Replace("\"", string.Empty);
                else
                    keyWord = keyWord.Substring(1, keyWord.Length - 2);
            }
            else if (keyWord.StartsWith("\""))
            {
                position = keyWord.Substring(1, keyWord.Length - 1).IndexOf("\"");
                if (position == -1)
                    keyWord = keyWord.Replace("\"", string.Empty);
                else
                    keyWord = keyWord.Substring(1, keyWord.Length - 1);
            }
            else if (keyWord.EndsWith("\""))
            {
                position = keyWord.Substring(0, keyWord.Length - 1).IndexOf("\"");
                if (position == -1)
                    keyWord = keyWord.Replace("\"", string.Empty);
                else
                    keyWord = keyWord.Substring(0, keyWord.Length - 1);
            }
            else
            {
                keyWord = keyWord.Replace("\"", string.Empty);
            }

            return keyWord;
        }

        public static string HighlightWords(Parameters @params, string sentence)
        {
            if (string.IsNullOrEmpty(sentence))
                return string.Empty;
            if (@params.KeyWords.Count == 0)
                return sentence;

            sentence = sentence.TrimEnd();
            if (sentence.EndsWith(","))
                sentence = sentence.Substring(0, sentence.Length - 1);
            bool isAPhrase = @params.KeyWords[0].Trim().StartsWith("\"") && @params.KeyWords[@params.KeyWords.Count - 1].Trim().EndsWith("\"");

            string[] parts;
            foreach (string keyWord in @params.KeyWords)
            {
                var Key = keyWord;
                Key = Key.Replace("*", string.Empty);

                if (isAPhrase)
                {
                    Key = CorrectForUnmatchedQuotes(Key);
                }
                else if (Key.StartsWith("\"") && Key.EndsWith("\""))
                {
                    Key = Key.Substring(1, Key.Length - 2);
                }

                int andOrOffset = 0;
                if (string.Compare(Key, "and", true) == 0 || string.Compare(Key, "or", true) == 0)
                    andOrOffset = 1;

                if (isAPhrase || andOrOffset == 0)
                {
                    
                    int position = 1;
                    var sb = new StringBuilder();

                    if (andOrOffset > 0)
                    {
                        parts = Strings.Split(sentence, " " + Key + " ", -1, CompareMethod.Text);
                    }
                    else
                    {
                        parts = Strings.Split(sentence, Key, -1, CompareMethod.Text);
                    }

                    foreach (string part in parts)
                    {
                        position += part.Length + andOrOffset;
                        sb.Append(part + Strings.Space(andOrOffset));
                        sb.Append(Strings.Chr(254) + Strings.Mid(sentence, position, Key.Length) + Strings.Chr(255) + Strings.Space(andOrOffset));
                        position += Key.Length + andOrOffset;
                    }

                    if (andOrOffset > 0)
                    {
                        // replace any bogus highlight strings
                        sentence = sb.ToString().Replace(Strings.Chr(254) + "  " + Strings.Chr(255), string.Empty);
                        sentence = sentence.Replace(Strings.Chr(254) + " " + Strings.Chr(255), string.Empty);
                        sentence = sentence.Replace(Conversions.ToString(Strings.Chr(254)) + Strings.Chr(255), string.Empty).TrimEnd();
                    }
                    else
                    {
                        sentence = Strings.Left(sb.ToString(), sb.Length - 2).TrimEnd();
                    }
                }
            }

            sentence = sentence.Replace(Conversions.ToString(Strings.Chr(254)), "<font style='background-color: yellow;'>");
            return sentence.Replace(Conversions.ToString(Strings.Chr(255)), "</font>").TrimEnd();
        }

        public static Parameters Save(Parameters @params, string foreignKeyTable, string foreignKeyField, string foreignKeyValue, List<FieldValue> data, Passport Passport, ScriptReturn result)
        {
            string sql = string.Empty;
            object definedID = null;
            string counterField = "";
            string counterValue = "";
            string NewID = "";
            var query = new Query(Passport);
            // Languages.WriteTxtFile("Save Method Start =    Date Format = ")
            using (var conn = Passport.Connection())
            {
                using (var cmd = new SqlCommand(string.Empty, conn))
                {
                    if (@params.NewRecord)
                    {
                        sql = "INSERT INTO [" + @params.TableName + "] (insertfieldtoken) VALUES (insertvaluetoken);SELECT NEWID = SCOPE_IDENTITY()";
                        bool counterFieldInDataList = false;
                        bool hasACounterField = !string.IsNullOrEmpty(@params.TableInfo["CounterFieldName"].ToString());
                        string insertFields = string.Empty;
                        string insertValues = string.Empty;

                        foreach (FieldValue Item in data)
                        {
                            // Item.Field = Item.Field.Replace("_lookup", "")
                            if (Item.Field.ToLower().StartsWith(@params.TableName.ToLower() + "."))
                                Item.Field = Item.Field.Substring(@params.TableName.Length + 1);

                            if ((Item.Field.ToLower() ?? "") == (@params.PrimaryKey.ToLower() ?? ""))
                            {
                                // If Item.value Is Nothing Or Item.value.ToString = String.Empty Then
                                if (Item.value is not null && !string.IsNullOrEmpty(Item.value.ToString()) && !insertFields.ToLower().Contains("[" + Item.Field.ToLower() + "],"))
                                {
                                    if (result is not null)
                                    {
                                        if (result.NewTableId.Length > 0)
                                        {
                                            definedID = result.NewTableId;
                                        }
                                        else
                                        {
                                            definedID = Item.value;
                                        }
                                    }
                                    else
                                    {
                                        definedID = Item.value;
                                    }
                                    insertFields += "[" + Item.Field + "],";
                                    insertValues += "@" + Item.Field + ",";
                                    cmd.Parameters.AddWithValue("@" + Item.Field, Item.value.ToString().Replace("''", "'"));
                                }
                            }
                            else if (!string.IsNullOrEmpty(Item.value.ToString()) && !insertFields.ToLower().Contains("[" + Item.Field.ToLower() + "],"))
                            {
                                insertFields += "[" + Item.Field + "],";
                                insertValues += "@" + Item.Field + ",";

                                if (Item.value is DBNull)
                                {
                                    cmd.Parameters.AddWithValue("@" + Item.Field, Item.value);
                                }
                                else if (Item.DataType == "System.DateTime" && Item.value.ToString() == "")
                                {
                                    DateTime dateTimeValue;
                                    if (DateTime.TryParseExact(Item.value.ToString(), "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out dateTimeValue))
                                    {
                                        cmd.Parameters.AddWithValue("@" + Item.Field, dateTimeValue);
                                    }
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@" + Item.Field, Item.value.ToString().Replace("''", "'"));
                                }
                            }
                        }

                        if (definedID is null || hasACounterField)
                        {
                            var counter = TablesTableAdapter.TakeNextSmeadCounterTableID(@params.TableName, definedID, conn);

                            if (counter.CounterValue == 0)
                            {
                            }
                            // Throw New Exception("Please fill out all required fields.")
                            else
                            {
                                if (counter.CounterValue == -1)
                                {
                                    counterFieldInDataList = true;
                                    counterValue = definedID.ToString();
                                }
                                else
                                {
                                    counterValue = counter.CounterValue.ToString();

                                    foreach (var Item in data)
                                    {
                                        if (string.Compare(counter.FieldName, Item.Field, true) == 0)
                                        {
                                            if (Item.value is null || string.IsNullOrEmpty(Item.value.ToString()))
                                            {
                                                counterFieldInDataList = true;
                                                Item.value = counterValue;
                                            }
                                        }
                                    }
                                }
                                // need to discuss with Reggie //moti mashiah commented out the code as this code no needed.
                                if (definedID is null)
                                {
                                    counterField = counter.FieldName;
                                    insertFields += "[" + counter.FieldName + "],";
                                    insertValues += counterValue + ",";
                                }
                            }
                        }

                        if (Conversions.ToBoolean(foreignKeyField.Length) & Conversions.ToBoolean(foreignKeyTable.Length))
                        {
                            if (!insertFields.ToLower().Contains(("[" + foreignKeyField + "]").ToLower()) & !insertFields.ToLower().Contains(("[" + Navigation.MakeSimpleField(Navigation.GetLowerTableForeignKeyField(foreignKeyTable, @params.TableName, conn)) + "]").ToLower()))
                            {
                                insertFields += "[" + Navigation.MakeSimpleField(Navigation.GetLowerTableForeignKeyField(foreignKeyTable, @params.TableName, conn)) + "],";
                                insertValues += "@foreignKeyValue,";
                                cmd.Parameters.AddWithValue("@foreignKeyValue", foreignKeyValue);
                            }
                        }

                        sql = Strings.Replace(sql, "insertfieldtoken", insertFields.Substring(0, insertFields.Length - 1));
                        sql = Strings.Replace(sql, "insertvaluetoken", insertValues.Substring(0, insertValues.Length - 1));
                        cmd.CommandText = sql;

                        if (string.IsNullOrEmpty(counterValue) & definedID is null)
                        {
                            NewID = cmd.ExecuteScalar().ToString();
                        }
                        else
                        {
                            // Languages.WriteTxtFile("Before Execute command ")
                            cmd.ExecuteNonQuery();
                            // Languages.WriteTxtFile("after execute command ")
                        }

                        if (definedID is not null)
                        {
                            if (Navigation.GetFieldDataType(@params.TableName, @params.PrimaryKey, conn).ToString().Contains("System.Int"))
                            {
                                @params.KeyValue = Conversions.ToInteger(definedID).ToString();
                            }
                            else
                            {
                                @params.KeyValue = definedID.ToString();
                            }
                        }
                        else if (!string.IsNullOrEmpty(NewID))
                        {
                            @params.KeyValue = NewID;
                        }
                        else
                        {
                            @params.KeyValue = counterValue.ToString();
                        }

                        Query.SetDefaultTrackingLocation(@params.TableName, @params.TableInfo, @params.KeyValue, Passport, conn);
                        query.FillData(@params);
                        // SetRetentionInactiveFlag(params.TableInfo, GetSingleRow(params.TableInfo, params.KeyValue, params.KeyField, Passport), retentionCode, Passport)
                        string idFieldName = string.Empty;

                        if (hasACounterField && !counterFieldInDataList && !string.IsNullOrEmpty(counterField))
                        {
                            var item = new FieldValue(counterField, "system.string");
                            item.value = counterValue;
                            var counterData = new List<FieldValue>();
                            counterData.Add(item);
                            Query.UpdateSLIndexerData(@params.TableName, @params.KeyValue, counterData, conn, true);
                        }
                        else
                        {
                            idFieldName = Navigation.MakeSimpleField(@params.TableInfo["IdFieldName"].ToString());
                        }

                        Query.UpdateSLIndexerData(@params.TableName, @params.KeyValue, data, conn, true, idFieldName);
                        // result = ScriptEngine.RunScriptAfterAdd(params.TableName, params.KeyValue, Passport)
                        // If Not result.Successful Then Throw New Exception(result.ReturnMessage)
                        return @params;
                    }
                    else
                    {
                        // Dim result As ScriptReturn = ScriptEngine.RunScriptBeforeEdit(params.TableName, params.KeyValue, Passport)
                        // If Not result.Successful Then Throw New Exception(result.ReturnMessage)
                        if (Navigation.IsArchivedOrDestroyed(@params.TableName, @params.KeyValue, Navigation.Enums.meFinalDispositionStatusType.fdstArchived | Navigation.Enums.meFinalDispositionStatusType.fdstDestroyed, Passport.Connection()))
                        {
                            throw new Exception("You cannot modify records that are archived or destroyed.", new Exception("-1"));
                        }

                        sql = "UPDATE [" + @params.TableName + "] SET updatefieldtoken WHERE " + @params.PrimaryKey + " = @pKey" + @params.PrimaryKey;
                        string updateFields = string.Empty;

                        foreach (var Item in data)
                        {
                            // Item.Field = Item.Field.Replace("_lookup", "")
                            if (Item.Field.ToLower().StartsWith(@params.TableName.ToLower() + "."))
                                Item.Field = Item.Field.Substring(@params.TableName.Length + 1);

                            if (!updateFields.Contains("[" + Item.Field + "]=@" + Item.Field + ", "))
                            {
                                updateFields += "[" + Item.Field + "]=@" + Item.Field + ", ";
                                if (string.Compare(@params.PrimaryKey, Item.Field, true) == 0)
                                    NewID = Item.value.ToString();

                                if (Item.value is DBNull)
                                {
                                    cmd.Parameters.AddWithValue("@" + Item.Field, Item.value);
                                }
                                else if(Item.DataType == "System.DateTime" && Item.value == "")
                                {
                                    cmd.Parameters.AddWithValue("@" + Item.Field, DBNull.Value);
                                }
                                else if (Item.DataType == "System.DateTime" && Item.value != DBNull.Value)
                                {
                                    DateTime dateTimeValue;
                                    if(DateTime.TryParseExact(Item.value.ToString(), "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out dateTimeValue))
                                    {
                                        cmd.Parameters.AddWithValue("@" + Item.Field, dateTimeValue);
                                    }
                                }
                                else
                                {

                                    cmd.Parameters.AddWithValue("@" + Item.Field, Item.value.ToString().Replace("''", "'"));
                                }
                            }
                        }

                        cmd.Parameters.AddWithValue("@pKey" + @params.PrimaryKey, @params.KeyValue);
                        cmd.CommandText = Strings.Replace(sql, "updatefieldtoken", updateFields.Substring(0, updateFields.Length - 2));
                        // Modified By Hemin
                        // If cmd.ExecuteNonQuery() = 0 Then Throw New Exception("The record to be updated has been removed by another user.")
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("The record to be updated has been removed by another user");
                        if (NewID.Length > 0)
                            @params.KeyValue = NewID;
                        // retentionCode = SetRetentionCode(params.TableName, params.TableInfo, params.KeyValue, Passport)
                        // SetRetentionInactiveFlag(params.TableInfo, GetSingleRow(params.TableInfo, params.KeyValue, params.KeyField, Passport), retentionCode, Passport)
                        Retention.UpdateRetentionData(@params, data, conn);
                        Query.UpdateSLIndexerData(@params.TableName, @params.KeyValue, data, conn, false);
                        // ScriptEngine.RunScriptAfterEdit(params.TableName, params.KeyValue, Passport)
                        return @params;
                    }
                }
            }
        }
        public static string AddNewMultiRecords(Passport passport, string tableName, List<List<FieldValue>> datarow)
        {
            string msg = string.Empty;
            bool isfailed = false;
            var @params = new Parameters(tableName, passport);
            @params.Scope = ScopeEnum.Table;
            try
            {
                if (passport.CheckPermission(@params.TableName, SecureObject.SecureObjectType.Table, Smead.Security.Permissions.Permission.Add))
                {
                    for (int index = 0, loopTo = datarow.Count - 1; index <= loopTo; index++)
                        Query.SaveNewRecord(@params, "", @params.KeyField, "", datarow[index], passport);
                }
                else
                {
                    msg = $"No Permission";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isfailed = true;
            }
            if (!isfailed)
            {
                msg = $"Added {datarow.Count} Rows!";
            }
           
            return msg;
        }
        private static void SaveNewRecord(Parameters @params, string foreignKeyTable, string foreignKeyField, string foreignKeyValue, List<FieldValue> data, Passport passport)
        {
            string sql = string.Empty;
            object definedID = null;
            string counterField = "";
            string counterValue = "";
            string NewID = "";
            var query = new Query(passport);

            // Dim retentionCode As String = ""
            sql = "INSERT INTO [" + @params.TableName + "] (insertfieldtoken) VALUES (insertvaluetoken);SELECT NEWID = SCOPE_IDENTITY()";
            bool counterFieldInDataList = false;
            bool hasACounterField = !string.IsNullOrEmpty(@params.TableInfo["CounterFieldName"].ToString());
            string insertFields = string.Empty;
            string insertValues = string.Empty;
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(string.Empty, conn))
                {

                    foreach (FieldValue Item in data)
                    {
                        // Item.Field = Item.Field.Replace("_lookup", "")
                        if (Item.Field.ToLower().StartsWith(@params.TableName.ToLower() + "."))
                            Item.Field = Item.Field.Substring(@params.TableName.Length + 1);

                        if ((Item.Field.ToLower() ?? "") == (@params.PrimaryKey.ToLower() ?? ""))
                        {
                            // If Item.value Is Nothing Or Item.value.ToString = String.Empty Then
                            if (Item.value is not null && !string.IsNullOrEmpty(Item.value.ToString()) && !insertFields.ToLower().Contains("[" + Item.Field.ToLower() + "],"))
                            {

                                insertFields += "[" + Item.Field + "],";
                                insertValues += "@" + Item.Field + ",";
                                cmd.Parameters.AddWithValue("@" + Item.Field, Item.value.ToString().Replace("''", "'"));
                            }
                        }
                        else if (!string.IsNullOrEmpty(Item.value.ToString()) && !insertFields.ToLower().Contains("[" + Item.Field.ToLower() + "],"))
                        {
                            insertFields += "[" + Item.Field + "],";
                            insertValues += "@" + Item.Field + ",";
                            if (Item.value is DBNull)
                            {
                                cmd.Parameters.AddWithValue("@" + Item.Field, Item.value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@" + Item.Field, Item.value.ToString().Replace("''", "'"));
                            }
                        }
                    }

                    if (definedID is null || hasACounterField)
                    {
                        var counter = TablesTableAdapter.TakeNextSmeadCounterTableID(@params.TableName, definedID, conn);
                        if (counter.CounterValue == 0)
                        {
                        }
                        // Throw New Exception("Please fill out all required fields.")
                        else
                        {
                            if (counter.CounterValue == -1)
                            {
                                counterFieldInDataList = true;
                                counterValue = definedID.ToString();
                            }
                            else
                            {
                                counterValue = counter.CounterValue.ToString();
                                foreach (var Item in data)
                                {
                                    if (string.Compare(counter.FieldName, Item.Field, true) == 0)
                                    {
                                        if (Item.value is null || string.IsNullOrEmpty(Item.value.ToString()))
                                        {
                                            counterFieldInDataList = true;
                                            Item.value = counterValue;
                                        }
                                    }
                                }
                            }
                            // need to discuss with Reggie //moti mashiah commented out the code as this code no needed.
                            if (definedID is null)
                            {
                                counterField = counter.FieldName;
                                insertFields += "[" + counter.FieldName + "],";
                                insertValues += counterValue + ",";
                            }
                        }
                    }
                    if (Conversions.ToBoolean(foreignKeyField.Length) & Conversions.ToBoolean(foreignKeyTable.Length))
                    {
                        if (!insertFields.ToLower().Contains(("[" + foreignKeyField + "]").ToLower()) & !insertFields.ToLower().Contains(("[" + Navigation.MakeSimpleField(Navigation.GetLowerTableForeignKeyField(foreignKeyTable, @params.TableName, conn)) + "]").ToLower()))
                        {
                            insertFields += "[" + Navigation.MakeSimpleField(Navigation.GetLowerTableForeignKeyField(foreignKeyTable, @params.TableName, conn)) + "],";
                            insertValues += "@foreignKeyValue,";
                            cmd.Parameters.AddWithValue("@foreignKeyValue", foreignKeyValue);
                        }
                    }
                    sql = Strings.Replace(sql, "insertfieldtoken", insertFields.Substring(0, insertFields.Length - 1));
                    sql = Strings.Replace(sql, "insertvaluetoken", insertValues.Substring(0, insertValues.Length - 1));
                    cmd.CommandText = sql;

                    if (string.IsNullOrEmpty(counterValue) & definedID is null)
                    {
                        NewID = cmd.ExecuteScalar().ToString();
                    }
                    else
                    {
                        // Languages.WriteTxtFile("Before Execute command ")
                        cmd.ExecuteNonQuery();
                        // Languages.WriteTxtFile("after execute command ")
                    }
                    if (definedID is not null)
                    {
                        if (Navigation.GetFieldDataType(@params.TableName, @params.PrimaryKey, conn).ToString().Contains("System.Int"))
                        {
                            @params.KeyValue = Conversions.ToInteger(definedID).ToString();
                        }
                        else
                        {
                            @params.KeyValue = definedID.ToString();
                        }
                    }
                    else if (!string.IsNullOrEmpty(NewID))
                    {
                        @params.KeyValue = NewID;
                    }
                    else
                    {
                        @params.KeyValue = counterValue.ToString();
                    }

                    Query.SetDefaultTrackingLocation(@params.TableName, @params.TableInfo, @params.KeyValue, passport, conn);
                    query.FillData(@params);
                    // SetRetentionInactiveFlag(params.TableInfo, GetSingleRow(params.TableInfo, params.KeyValue, params.KeyField, Passport), retentionCode, Passport)
                    string idFieldName = string.Empty;

                    if (hasACounterField && !counterFieldInDataList && !string.IsNullOrEmpty(counterField))
                    {
                        var item = new FieldValue(counterField, "system.string");
                        item.value = counterValue;
                        var counterData = new List<FieldValue>();
                        counterData.Add(item);
                        Query.UpdateSLIndexerData(@params.TableName, @params.KeyValue, counterData, conn, true);
                    }
                    else
                    {
                        idFieldName = Navigation.MakeSimpleField(@params.TableInfo["IdFieldName"].ToString());
                    }
                    Query.UpdateSLIndexerData(@params.TableName, @params.KeyValue, data, conn, true, idFieldName);
                    AuditRecords(@params, data, passport, recordAction.Add);
                }
            }
        }
        public static string UpdateRecordsByColumn(string keyVal, string FieldName, string TableName, Passport passport, List<FieldValue> data, bool IsMultyupdate, string fieldNameType, List<string> ListBeforedata)
        {
            string msg = string.Empty;
            var @params = new Parameters(TableName, passport);
            var table = new DataTable();
            var lst = new List<string>();
            string command = string.Empty;
            if (keyVal.ToLower() == "null")
            {
                command = string.Format("select {0} from {1} where {2} is {3}", @params.PrimaryKey, TableName, FieldName, keyVal);
            }
            else if (fieldNameType == "text")
            {
                command = string.Format("select {0} from {1} where CAST({2} as nvarchar) = '{3}'", @params.PrimaryKey, TableName, FieldName, keyVal);
            }
            else
            {
                command = string.Format("select {0} from {1} where {2} = '{3}'", @params.PrimaryKey, TableName, FieldName, keyVal);
            }
             
            try
            {
                using (var conn = passport.Connection())
                {
                    using (var cmd = new SqlCommand(command, conn))
                    {
                        // cmd.Parameters.AddWithValue("@pkey", params.PrimaryKey)
                        // cmd.Parameters.AddWithValue("@fieldname", FieldName)
                        // cmd.Parameters.AddWithValue("@keyval", keyVal)
                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(table);
                            foreach (DataRow key in table.Rows)
                                lst.Add(key[0].ToString());
                        }
                    }
                }
                if (lst.Count > 1 && IsMultyupdate)
                {
                    SaveRecords(@params, lst, data, passport, ListBeforedata);
                    msg = $"Updated {lst.Count} records successfuly!";
                }
                else if (lst.Count > 1 && IsMultyupdate == false)
                {
                    msg = $"We found multiple records ({lst.Count}), If you want to update multiple record change the 'IsMultyupdate' to true";
                }
                else if (lst.Count == 1)
                {
                    SaveRecords(@params, lst, data, passport, ListBeforedata);
                    msg = $"Record Updated!";
                }
                else if (lst.Count == 0)
                {
                    msg = $"Row Not Found 0";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }
        public static void SaveRecords(Parameters @params, List<string> KeyValues, List<FieldValue> data, Passport passport, List<string> ListOfbeforeData)
        {
            string sql = string.Empty;
            string NewID = "";
            var query = new Query(passport);

            using (var conn = passport.Connection())
            {
                foreach (string keyVal in KeyValues)
                {
                    @params.KeyValue = keyVal;
                    using (var cmd = new SqlCommand(string.Empty, conn))
                    {
                        if (Navigation.IsArchivedOrDestroyed(@params.TableName, @params.KeyValue, Navigation.Enums.meFinalDispositionStatusType.fdstArchived | Navigation.Enums.meFinalDispositionStatusType.fdstDestroyed, passport.Connection()))
                        {
                            throw new Exception("You cannot modify records that are archived or destroyed.", new Exception("-1"));
                        }

                        sql = "UPDATE [" + @params.TableName + "] SET updatefieldtoken WHERE " + @params.PrimaryKey + " = @pKey" + @params.PrimaryKey;
                        string updateFields = string.Empty;

                        foreach (var Item in data)
                        {
                            if (Item.Field.ToLower().StartsWith(@params.TableName.ToLower() + "."))
                                Item.Field = Item.Field.Substring(@params.TableName.Length + 1);

                            if (!updateFields.Contains("[" + Item.Field + "]=@" + Item.Field + ", "))
                            {
                                updateFields += "[" + Item.Field + "]=@" + Item.Field + ", ";
                                if (string.Compare(@params.PrimaryKey, Item.Field, true) == 0)
                                    NewID = Item.value.ToString();

                                if (Item.value is DBNull)
                                {
                                    cmd.Parameters.AddWithValue("@" + Item.Field, Item.value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@" + Item.Field, Item.value.ToString().Replace("''", "'"));
                                }
                            }
                        }

                        cmd.Parameters.AddWithValue("@pKey" + @params.PrimaryKey, @params.KeyValue);
                        cmd.CommandText = Strings.Replace(sql, "updatefieldtoken", updateFields.Substring(0, updateFields.Length - 2));
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("The record to be updated has been removed by another user");

                        if (NewID.Length > 0)
                            @params.KeyValue = NewID;
                        Retention.UpdateRetentionData(@params, data, conn);
                        Query.UpdateSLIndexerData(@params.TableName, @params.KeyValue, data, conn, false);
                        
                    }
                }
                foreach (string item in ListOfbeforeData)
                {
                    @params.BeforeData = item;
                    AuditRecords(@params, data, passport, recordAction.updateBycolumn);
                }
            }
        }
        internal static void AuditRecords(Parameters param, List<FieldValue> lst, Passport passport, recordAction action)
        {
            param.AfterData = AuditAfterData(lst);
            var withBlock = AuditType.WebAccess;
            withBlock.TableName = param.TableName;
            withBlock.TableId = param.KeyValue;
            withBlock.ClientIpAddress = "RestAPI Call";
            switch (action)
            {
                case recordAction.Add:
                    withBlock.ActionType = AuditType.WebAccessActionType.AddRecord;
                    withBlock.BeforeData = param.BeforeDataTrimmed;
                    break;
                case recordAction.update:
                    withBlock.ActionType = AuditType.WebAccessActionType.UpdateRecord;
                    withBlock.BeforeData = param.BeforeDataTrimmed;
                    break;
                case recordAction.delete:
                    withBlock.ActionType = AuditType.WebAccessActionType.DeleteRecord;
                    withBlock.BeforeData = param.BeforeDataTrimmed;
                    break;
                case recordAction.updateBycolumn:
                    withBlock.ActionType = AuditType.WebAccessActionType.UpdateRecord;
                    withBlock.BeforeData = param.BeforeData;
                    break;
                default:
                    break;
            }
            
            withBlock.AfterData = param.AfterDataTrimmed;
            Auditing.AuditUpdates(withBlock, passport);
        }
        internal static string AuditAfterData(List<FieldValue> lst)
        {
            string afteradd = string.Empty;
            foreach (FieldValue fv in lst)
                afteradd += $"{fv.Field}: {fv.value}";
            return afteradd;
        }

        internal static void UpdateSLIndexerData(string tableName, string tableId, List<FieldValue> data, SqlConnection conn, bool addMode, string IdFieldName = "")
        {
            string sql = Resources.UpdateSLIndexer;
            if (addMode)
                sql = Resources.UpdateSLIndexerAdd;
            bool skipIdField = string.IsNullOrEmpty(IdFieldName);
            if (!skipIdField)
                Navigation.MakeSimpleField(IdFieldName);

            foreach (FieldValue item in data)
            {
                if (!skipIdField)
                    skipIdField = string.Compare(item.Field, IdFieldName, true) == 0;

                if (HasAValidValue(item))
                {
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@IndexType", 8);
                        cmd.Parameters.AddWithValue("@IndexTableName", tableName);
                        cmd.Parameters.AddWithValue("@IndexFieldName", item.Field);
                        cmd.Parameters.AddWithValue("@IndexTableID", tableId);
                        cmd.Parameters.AddWithValue("@IndexData", item.value.ToString());
                        cmd.Parameters.AddWithValue("@OrphanType", 0);
                        cmd.Parameters.AddWithValue("@PageNumber", 0);
                        cmd.Parameters.AddWithValue("@RecordVersion", 0);
                        cmd.Parameters.AddWithValue("@AttachmentNumber", 0);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            if (!skipIdField)
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@IndexType", 8);
                    cmd.Parameters.AddWithValue("@IndexTableName", tableName);
                    cmd.Parameters.AddWithValue("@IndexFieldName", IdFieldName);
                    cmd.Parameters.AddWithValue("@IndexTableID", tableId);
                    cmd.Parameters.AddWithValue("@IndexData", tableId);
                    cmd.Parameters.AddWithValue("@OrphanType", 0);
                    cmd.Parameters.AddWithValue("@PageNumber", 0);
                    cmd.Parameters.AddWithValue("@RecordVersion", 0);
                    cmd.Parameters.AddWithValue("@AttachmentNumber", 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateSLIndexerData(string tableName, string tableId, List<FieldValue> data, Passport passport, bool addMode, string IdFieldName = "")
        {
            using (var conn = passport.Connection())
            {
                UpdateSLIndexerData(tableName, tableId, data, conn, addMode);
            }
        }

        private static bool HasAValidValue(FieldValue item)
        {
            if (item.value is null || string.IsNullOrEmpty(item.value.ToString()))
                return false;

            string sType = "system.string";

            try
            {
                if (item.DataType is null)
                {
                    if (Information.IsNumeric(item.value))
                    {
                        sType = "system.double";
                    }
                    else if (Information.IsDate(item.value))
                    {
                        sType = "system.datetime";
                    }
                }
                else
                {
                    sType = item.DataType;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // do nothing
            }

            switch (sType.ToLower() ?? "")
            {
                case "system.boolean":
                case "system.int16":
                case "system.int32":
                case "system.int64":
                case "system.single":
                case "system.double":
                case "system.decimal":
                    {
                        return Conversions.ToDouble(item.value) != 0d;
                    }

                default:
                    {
                        return true;
                    }
            }
        }

        private static DataTable GetMappedTableToDataTableSchema(string tableName, DataTable dataTable, SqlConnection conn)
        {
            string selectSQL = "SELECT TOP 0 ";

            foreach (DataColumn column in dataTable.Columns)
            {
                if (IncludeColumn(column.ColumnName))
                {
                    selectSQL += "[" + column.ColumnName + "],";
                }
            }

            selectSQL = selectSQL.Substring(0, selectSQL.Length - 1);
            selectSQL += " FROM " + tableName;

            using (var schemaCommand = new SqlCommand(selectSQL, conn))
            {
                using (var da = new SqlDataAdapter(schemaCommand))
                {
                    var schema = new DataTable();
                    da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    da.Fill(schema);
                    return schema;
                }
            }
        }

        private static DataTable GetMappedTableToDataTableSchema(string tableName, DataTable dataTable, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetMappedTableToDataTableSchema(tableName, dataTable, conn);
            }
        }

        public static string Pagify(Parameters @params, string sql, string orderClause)
        {

            string pageClause = "SELECT TOP ";
            if (@params.RequestedRows <= 0)
                @params.RequestedRows = 250;
            pageClause += @params.RequestedRows.ToString() + (" * FROM (SELECT ROW_NUMBER() OVER (" + orderClause + ") AS RowNum,");
            pageClause = Strings.Replace(sql, "select", pageClause, 1, 1, CompareMethod.Text);
            pageClause += ") AS PagedResult WHERE RowNum > " + ((@params.PageIndex - 1) * @params.RequestedRows).ToString() + " and RowNum <= " + (@params.PageIndex * @params.RequestedRows).ToString();
            pageClause += " ORDER BY PagedResult.RowNum";
            return pageClause;
        }

        public static string ReplaceInvalidParameterCharacters(string ParamName)
        {
            return ReplaceInvalidParameterCharacters(ParamName, "_");
        }

        public static string ReplaceInvalidParameterCharacters(string ParamName, string ReplaceWith)
        {
            var sb = new StringBuilder();

            foreach (char c in ParamName)
            {
                if (INVALID_PARM_CHARS.Contains(Conversions.ToString(c)))
                {
                    sb.Append(ReplaceWith);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static void UpdateDataTableToDatabase(string tableName, string tableId, DataTable dataTable, DataRow currentRow, SqlConnection conn)
        {
            if (dataTable.Rows.Count == 0)
                return;
            if (dataTable.Columns.Count == 0)
                return;

            var row = currentRow;
            if (row is null)
                row = dataTable.Rows[0];

            string primaryKey = Navigation.GetPrimaryKeyFieldName(tableName, conn);
            var schema = GetMappedTableToDataTableSchema(tableName, dataTable, conn);

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string updateSQL = "UPDATE [" + tableName + "] SET ";

                foreach (DataColumn column in dataTable.Columns)
                {
                    if (schema.Columns[column.ColumnName] is not null)
                    {
                        if (!schema.Columns[column.ColumnName].ReadOnly)
                        {
                            if (!(schema.Columns[column.ColumnName].DataType.ToString() == "System.Byte[]"))
                            {
                                if (IncludeColumn(column.ColumnName))
                                {
                                    updateSQL += "[" + column.ColumnName + "]=@" + ReplaceInvalidParameterCharacters(column.ColumnName) + ",";
                                    cmd.Parameters.AddWithValue("@" + ReplaceInvalidParameterCharacters(column.ColumnName), row[column.ColumnName]);
                                }
                            }
                        }
                    }
                }

                updateSQL = updateSQL.Substring(0, updateSQL.Length - 1);
                updateSQL += " WHERE [" + primaryKey + "] = @tableId";

                cmd.Parameters.AddWithValue("@tableId", tableId);
                cmd.CommandText = updateSQL;
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateDataTableToDatabase(string tableName, string tableId, DataTable dataTable, DataRow currentRow, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                UpdateDataTableToDatabase(tableName, tableId, dataTable, currentRow, conn);
            }
        }

        public static void InsertDataTableToDatabase(string tableName, DataTable dataTable, DataRow currentRow, SqlConnection conn)
        {
            if (dataTable.Rows.Count == 0)
                return;
            if (dataTable.Columns.Count == 0)
                return;

            var row = currentRow;
            if (row is null)
                row = dataTable.Rows[0];

            string insertSQL = string.Empty;
            string fieldsSQL = string.Empty;
            string valuesSQL = string.Empty;

            string idColumn = string.Empty;
            var schema = GetMappedTableToDataTableSchema(tableName, dataTable, conn);

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                foreach (DataColumn column in dataTable.Columns)
                {
                    if (schema.Columns[column.ColumnName] is not null)
                    {
                        if (schema.Columns[column.ColumnName].AutoIncrement)
                            idColumn = column.ColumnName;

                        if (!schema.Columns[column.ColumnName].ReadOnly)
                        {
                            if (!ReferenceEquals(row[column.ColumnName], DBNull.Value))
                            {
                                if (!(schema.Columns[column.ColumnName].DataType.ToString() == "System.Byte[]"))
                                {
                                    if (IncludeColumn(column.ColumnName))
                                    {
                                        fieldsSQL += "[" + column.ColumnName + "],";
                                        valuesSQL += "@" + ReplaceInvalidParameterCharacters(column.ColumnName) + ",";
                                        cmd.Parameters.AddWithValue("@" + ReplaceInvalidParameterCharacters(column.ColumnName), row[column.ColumnName]);
                                    }
                                }
                            }
                        }
                    }
                }

                fieldsSQL = fieldsSQL.Substring(0, fieldsSQL.Length - 1);
                valuesSQL = valuesSQL.Substring(0, valuesSQL.Length - 1);
                insertSQL = "INSERT INTO [" + tableName + "] (" + fieldsSQL + ") VALUES (" + valuesSQL + ")";

                if (!string.IsNullOrEmpty(idColumn))
                {
                    cmd.CommandText = insertSQL + "; SELECT SCOPE_IDENTITY();";
                    row[idColumn] = cmd.ExecuteScalar();
                }
                else
                {
                    cmd.CommandText = insertSQL;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void InsertDataTableToDatabase(string tableName, DataTable dataTable, DataRow currentRow, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                InsertDataTableToDatabase(tableName, dataTable, currentRow, conn);
            }
        }

        private static void SetDefaultTrackingLocation(string tableName, DataRow row, string tableId, Passport passport, SqlConnection conn)
        {
            // Dim row As DataRow = GetTableInfo(tableName, passport)
            if (row["DefaultTrackingTable"] is not null && !string.IsNullOrEmpty(row["DefaultTrackingTable"].ToString()))
            {
                var user = new User(passport, true);
                Tracking.Transfer(tableName, tableId, row["DefaultTrackingTable"].ToString(), row["DefaultTrackingID"].ToString(), default(DateTime), user.UserName, passport, conn);
            }
        }

        private static void SetDefaultTrackingLocation(string tableName, DataRow row, string tableId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                SetDefaultTrackingLocation(tableName, row, tableId, passport, conn);
            }
        }

        private enum RetentionAssignmentMethods
        {
            Manually,
            CurrentTable,
            RelatedTable
        }

        public static string SetRetentionCode(string TableName, DataRow tableInfo, string tableId, Passport passport)
        {
            if (tableInfo["RetentionAssignmentMethod"] is DBNull)
                return string.Empty;

            var keyid = Navigation.MakeSimpleField(tableInfo["IdFieldName"].ToString());
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    switch ((RetentionAssignmentMethods)Conversions.ToInteger(tableInfo["RetentionAssignmentMethod"]))
                    {
                        case RetentionAssignmentMethods.CurrentTable:
                            {
                                cmd.CommandText = "UPDATE [" + TableName + "] SET [" + tableInfo["RetentionFieldName"].ToString() + "] = '" + tableInfo["DefaultRetentionId"].ToString() + "' WHERE [" + keyid + "] = @tableId AND [" + tableInfo["RetentionFieldName"].ToString() + "] IS NULL";
                                cmd.Parameters.AddWithValue("@tableId", tableId);
                                cmd.ExecuteNonQuery();
                                return tableInfo["DefaultRetentionId"].ToString();
                            }
                        case RetentionAssignmentMethods.RelatedTable:
                            {
                                // get related retention field value 
                                cmd.CommandText = "SELECT " + Navigation.GetForeignKeys(TableName, tableInfo["RetentionRelatedTable"].ToString(), passport).LowerKey + " FROM " + TableName + " WHERE " + keyid + " = @TableId";
                                cmd.Parameters.AddWithValue("@tableId", tableId);
                                string relatedRetentionFieldValue = cmd.ExecuteScalar().ToString();
                                // get retention code of related retention record
                                var RetTableInfo = Navigation.GetTableInfo(tableInfo["RetentionRelatedTable"].ToString(), conn);
                                cmd.CommandText = "SELECT " + RetTableInfo["RetentionFieldName"].ToString() + " FROM " + tableInfo["RetentionRelatedTable"].ToString() + " WHERE " + Navigation.GetForeignKeys(TableName, tableInfo["RetentionRelatedTable"].ToString(), passport).UpperKey + " = @retValue";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@retValue", relatedRetentionFieldValue);
                                string retentionCode = cmd.ExecuteScalar().ToString();
                                // update the retention code to the new record
                                cmd.CommandText = "UPDATE [" + TableName + "] SET [" + tableInfo["RetentionFieldName"].ToString() + "] = @defRetId WHERE " + keyid + " = @tableId";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@defRetId", retentionCode);
                                cmd.Parameters.AddWithValue("@tableId", tableId);
                                cmd.ExecuteNonQuery();
                                return retentionCode;
                            }
                        case RetentionAssignmentMethods.Manually: // Assign default retention code, if record has null or empty code. [FUS-6235]
                            {
                                if (!string.IsNullOrEmpty(tableInfo["RetentionFieldName"].ToString()))
                                {
                                    cmd.CommandText = "UPDATE [" + TableName + "] SET [" + tableInfo["RetentionFieldName"].ToString() + "] = '" + tableInfo["DefaultRetentionId"].ToString() + "' WHERE [" + keyid + "] = @tableId AND [" + tableInfo["RetentionFieldName"].ToString() + "] IS NULL OR [" + tableInfo["RetentionFieldName"].ToString() + "] = '' ";
                                    cmd.Parameters.AddWithValue("@tableId", tableId);
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "SELECT " + tableInfo["RetentionFieldName"].ToString() + " FROM " + TableName + " WHERE [" + keyid + "] = @tableId ";
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@tableId", tableId);
                                    string retentionCode = cmd.ExecuteScalar().ToString();
                                    return retentionCode;
                                }

                                return string.Empty; // do nothing 
                            }

                        default:
                            {
                                return string.Empty;
                            }
                    }
                }
            }
        }

        private static string NormalizeString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            s = s.Replace(Constants.vbTab, " ");
            s = s.Replace(Constants.vbCr, " ");
            s = s.Replace(Constants.vbLf, " ");

            while (s.Contains("  "))
                s = s.Replace("  ", " ");

            return s;
        }

        public DataTable SelectAllpkeyValuesFromGridBySql(string sql)
        {
            try
            {
                var cmd = new SqlCommand(sql, _passport.Connection());
                var dt = new DataTable();
                var da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataTable();
            }
        }

        public static string QueryPaging(int PageNum, int PerPageRecord)
        {
            if (PageNum > 0 & PerPageRecord > 0)
            {
                int fromRecord = PageNum * PerPageRecord - PerPageRecord;
                return string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", fromRecord, PerPageRecord);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    [Serializable()]
    public class FieldValue
    {
        public string Field;
        private object _value;
        private List<FieldValue> _ors;
        private bool _filter;
        private string _dataType;
        public string SpecialType;
        public string Operate;

        public FieldValue(string field, string dataType)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("\"Field\" parameter cannot be Null or an empty string");
            if (string.IsNullOrEmpty(dataType))
                throw new ArgumentNullException("\"DataType\" parameter cannot be Null or an empty string");

            Field = field;
            _dataType = dataType;
            _ors = new List<FieldValue>();
            Operate = string.Empty;
            _filter = false;
        }

        public string DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
            }
        }

        public bool Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
            }
        }

        public object value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _value = CleanData(this);
                if (!_filter)
                {
                    if (_value.ToString().Contains("'"))
                        _value = value.ToString().Replace("'", "''");
                }

                if (string.Compare(DataType, "system.string", true) != 0)
                {
                    if (_value.ToString().ToLower() == "true")
                        _value = "1";
                    if (_value.ToString().ToLower() == "false")
                        _value = "0";
                }
            }
        }

        public List<FieldValue> Ors
        {
            get
            {
                return _ors;
            }
            set
            {
                _ors = value;
            }
        }

        public static object CleanData(FieldValue fv)
        {
            if (fv.SpecialType is null)
                return fv.value;

            switch (fv.SpecialType.ToLower() ?? "")
            {
                case "phone":
                case "social":
                    {
                        string rstr = fv.value.ToString();
                        rstr = Strings.Replace(rstr, "-", "");
                        rstr = Strings.Replace(rstr, "(", "");
                        rstr = Strings.Replace(rstr, ")", "");
                        rstr = Strings.Replace(rstr, ".", "");
                        if (rstr is null)
                            return string.Empty;
                        return rstr;
                    }

                default:
                    {
                        switch (fv.DataType.ToLower() ?? "")
                        {
                            case "system.datetime":
                                {
                                    if (string.IsNullOrEmpty(fv.value.ToString()))
                                        return DBNull.Value;
                                    break;
                                }
                        }

                        return fv.value;
                    }
            }
        }

        public string ToWhereClauseSection()
        {
            if (string.IsNullOrEmpty(Operate.Trim()))
                return string.Empty;
            if (DataType is null)
                _dataType = "System.String";

            switch (Operate.ToUpper() ?? "")
            {
                case "IN":
                case "LIKE":
                case "CONTAINS": // FUS-5389 - (8/9/2018)
                    {
                        if (string.Compare(DataType, "System.String", true) == 0)
                            return "([" + Field + "] LIKE '%" + value.ToString().Replace("*", "%").Replace("?", "_") + "%')";
                        return "(CAST([" + Field + "] AS VARCHAR(8000)) LIKE '%" + value.ToString().Replace("*", "%").Replace("?", "_") + "%')";
                    }
                case "NOT CONTAINS": // FUS-5389 - (8/9/2018)
                    {
                        if (string.Compare(DataType, "System.String", true) == 0)
                            return "([" + Field + "] NOT LIKE '%" + value.ToString().Replace("*", "%").Replace("?", "_") + "%')";
                        return "(CAST([" + Field + "] AS VARCHAR(8000)) NOT LIKE '%" + value.ToString().Replace("*", "%").Replace("?", "_") + "%')";
                    }
                case "BEG":
                    {
                        if (string.Compare(DataType, "System.String", true) == 0)
                            return "([" + Field + "] LIKE '" + value.ToString().Replace("*", "%").Replace("?", "_") + "%')";
                        return "(CAST([" + Field + "] AS VARCHAR(8000)) LIKE '" + value.ToString().Replace("*", "%").Replace("?", "_") + "%')";
                    }
                case "ENDS WITH": // FUS-5390, FUS-4415 - (8/9/2018)
                    {
                        if (string.Compare(DataType, "System.String", true) == 0)
                            return "([" + Field + "] LIKE '%" + value.ToString().Replace("*", "%").Replace("?", "_") + "')";
                        return "(CAST([" + Field + "] AS VARCHAR(8000)) LIKE '%" + value.ToString().Replace("*", "%").Replace("?", "_") + "')";
                    }
                case "LITERAL":
                    {
                        return "(" + Field + " " + value.ToString() + ")";
                    }
                case "BETWEEN":
                    {
                        if (string.IsNullOrEmpty(value.ToString().Trim()))
                        {
                            return "";
                        }
                        else
                        {
                            var parts = value.ToString().Split(new char[] { '|' });
                            if (string.Compare(DataType, "System.Int32", true) == 0)
                            {
                                return "([" + Field + "] " + Operate + " " + Conversions.ToInteger(parts[0]) + " And " + Conversions.ToInteger(parts[1]) + ")";
                            }
                            else if (string.Compare(DataType, "System.Int64", true) == 0)
                            {
                                return "([" + Field + "] " + Operate + " " + Conversions.ToLong(parts[0]) + " And " + Conversions.ToLong(parts[1]) + ")";
                            }
                            else if (string.Compare(DataType, "System.Double", true) == 0)
                            {
                                return "([" + Field + "] " + Operate + " " + Conversions.ToDouble(parts[0]) + " And " + Conversions.ToDouble(parts[1]) + ")";
                            }
                            else if (string.Compare(DataType, "System.DateTime", true) == 0)
                            {
                                return "(CAST([" + Field + "] AS Date)) " + Operate + " (CAST('" + parts[0] + "' AS Date))" + " And " + " (CAST('" + parts[1] + "' AS Date)) ";
                            }
                            return "([" + Field + "] " + Operate + " '" + parts[0] + "' And '" + parts[1] + "' )";
                        }
                    }

                default:
                    {
                        if (string.Compare(DataType, "System.Int32", true) == 0)
                        {
                            if (string.IsNullOrEmpty(value.ToString().Trim()))
                                value = 0;
                            return "([" + Field + "] " + Operate + " " + Conversions.ToInteger(value) + ")";
                        }

                        if (string.Compare(DataType, "System.DateTime", true) == 0)
                            return "(CAST([" + Field + "] AS Date))" + Operate + " (CAST('" + value.ToString() + "' AS Date))";
                        return "([" + Field + "] " + Operate + " '" + value.ToString() + "')";
                    }
            }
        }

        public static string ToWhereClause(List<FieldValue> FieldValueList, bool IncludeWhere = false)
        {
            string whereClause = string.Empty;
            var orList = new List<FieldValue>();

            if (FieldValueList is not null)
            {
                foreach (var _fieldValue in FieldValueList)
                {
                    string whereClauseSection = _fieldValue.ToWhereClauseSection();
                    if (!string.IsNullOrEmpty(whereClauseSection) && !string.IsNullOrEmpty(whereClause))
                        whereClause += " AND ";
                    whereClause += whereClauseSection;

                    foreach (var _orFieldValue in _fieldValue.Ors)
                        orList.Add(_orFieldValue);
                }

                foreach (var _orFieldValue in orList)
                {
                    string whereClauseSection = _orFieldValue.ToWhereClauseSection();
                    if (!string.IsNullOrEmpty(whereClauseSection) && !string.IsNullOrEmpty(whereClause))
                        whereClause += " OR ";
                    whereClause += whereClauseSection;
                }
            }
            if (string.IsNullOrEmpty(whereClause))
                return string.Empty;
            if (IncludeWhere)
                return "WHERE " + whereClause;
            return whereClause;
        }

       public enum recordAction
        {
            Add = 1,
            update = 2,
            updateBycolumn = 3,
            delete = 4
        }
    }
}