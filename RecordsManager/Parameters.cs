using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic;
using Smead.Security;

namespace MSRecordsEngine.RecordsManager
{

    public enum queryTypeEnum
    {
        Text = 1,
        KeyValuePair = 2,
        AdvancedFilter = 3,
        OpenTable = 4,
        Schema = 5,
        LikeItemName = 6
    }

    public enum ScopeEnum
    {
        Database = 1,
        Table = 2,
        Node = 3
    }

    public enum SearchEnum
    {
        HTML,
        XML,
        API
    }

    public class Parameters : ICloneable
    {

        private Passport _passport;
        private SqlConnection _connection;
        public bool Processed;
        public bool fromChartReq;

        public Parameters(Passport passport)
        {
            _passport = passport;
            _data = new DataTable();
            _thinClient = true;
            _includeViewFilters = true;
            _requestedRows = 100;
            _filterList = new List<FieldValue>();
            _paged = false;
            _pageIndex = 1;
            fromChartReq = false;
        }

        public Parameters(int viewID, Passport passport)
        {
            if (viewID == 0)
                return;
            _passport = passport;
            _filterList = new List<FieldValue>();
            ChangeViewID(viewID);
            _paged = false;
            _pageIndex = 1;
        }

        public Parameters(string TableName, Passport passport)
        {
            // If String.IsNullOrEmpty(TableName) Then Return
            _passport = passport;
            _filterList = new List<FieldValue>();
            _tableName = TableName;
            _tableInfo = Navigation.GetTableInfo(_tableName, _passport);
            _primaryKey = Navigation.MakeSimpleField(_tableInfo["IdFieldName"].ToString());
            _idFieldDataType = GetIDFieldDataType();
            _paged = false;
            _pageIndex = 1;
        }

        public void ChangeViewID(int viewId)
        {
            if (viewId == 0)
                return;
            _data = new DataTable();
            _viewId = viewId;
            _altViewId = Navigation.GetAltViewID(_viewId, _passport);
            _tableName = Navigation.GetViewTableName(_viewId, _passport);
            _viewName = Navigation.GetViewName(_viewId, _passport);

            try
            {
                _tableInfo = Navigation.GetTableInfo(_tableName, _passport);
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine(ex.Message);

                if (string.IsNullOrEmpty(_viewName))
                {
                    throw new NullReferenceException(string.Format("The View Id \"{0}\" refers to a Table(\"{1}\") that cannot be found or is not available.", viewId, _tableName));
                }
                else
                {
                    throw new NullReferenceException(string.Format("The View \"{0}\" refers to a Table(\"{1}\") that cannot be found or is not available.", _viewName, _tableName));
                }
            }

            _primaryKey = Navigation.MakeSimpleField(_tableInfo["IdFieldName"].ToString());
            if (_altViewId != 0)
            {
                _viewColumns = Navigation.GetViewColumns(_altViewId, _passport);
            }
            else
            {
                _viewColumns = Navigation.GetViewColumns(_viewId, _passport);
            }

            _thinClient = true;
            _includeViewFilters = true;
            _idFieldDataType = GetIDFieldDataType();
        }

        public Type IdFieldDataType
        {
            get
            {
                if (_idFieldDataType is null)
                {
                    _idFieldDataType = GetIDFieldDataType();
                }
                return _idFieldDataType;
            }
        }
        private Type _idFieldDataType;

        private Type GetIDFieldDataType()
        {
            using (var conn = _passport.Connection())
            {
                using (var cmd = new SqlCommand(string.Format("SELECT [{0}] FROM [{1}] WHERE 0 = 1", PrimaryKey, TableName), conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        da.FillSchema(dt, SchemaType.Source);
                        return dt.Columns[0].DataType;
                    }
                }
            }
        }

        public object Clone()
        {
            return (Parameters)MemberwiseClone();
        }

        public string SQL
        {
            get
            {
                return _sql;
            }
            set
            {
                _sql = value;
            }
        }
        private string _sql = string.Empty;

        public string TableName
        {
            get
            {
                return _tableName;
            }
        }
        private string _tableName;

        private int RowCount
        {
            get
            {
                return _rowCount;
            }
        }
        private int _rowCount;

        public DataRow TableInfo
        {
            get
            {
                if (_tableInfo is null & !string.IsNullOrEmpty(_tableName) & _passport is not null)
                {
                    _tableInfo = Navigation.GetTableInfo(_tableName, _passport);
                }
                return _tableInfo;
            }
        }
        private DataRow _tableInfo;

        public string PrimaryKey
        {
            get
            {
                return _primaryKey;
            }
        }
        private string _primaryKey;

        public int ViewId
        {
            get
            {
                return _viewId;
            }
        }
        private int _viewId;

        public int AltViewId
        {
            get
            {
                return _altViewId;
            }
        }
        private int _altViewId;

        public string ViewName
        {
            get
            {
                return _viewName;
            }
        }
        private string _viewName = string.Empty;

        public RecordsManage.ViewColumnsDataTable ViewColumns
        {
            get
            {
                return _viewColumns;
            }
        }
        private RecordsManage.ViewColumnsDataTable _viewColumns;

        public Parameters Parameter
        {
            get
            {
                return _parameters;
            }
        }
        private Parameters _parameters;

        public bool NewRecord
        {
            get
            {
                return _newRecord;
            }
            set
            {
                // _requestedRows = 0
                _newRecord = value;
                _includeViewFilters = !value;
            }
        }
        private bool _newRecord;

        public bool SavedRecord
        {
            get
            {
                return _savedRecord;
            }
            set
            {
                // _requestedRows = 0
                _savedRecord = value;
            }
        }
        private bool _savedRecord;

        public string CursorValue
        {
            get
            {
                return _cursorValue;
            }
            set
            {
                _cursorValue = value;
            }
        }
        private string _cursorValue = string.Empty;

        public string KeyField
        {
            get
            {
                if (string.IsNullOrEmpty(_keyField))
                {
                    return _primaryKey;
                }
                else
                {
                    return _keyField;
                }
            }
            set
            {
                _keyField = value;
            }
        }
        private string _keyField = string.Empty;

        public string KeyValue
        {
            get
            {
                return _keyValue;
            }
            set
            {
                _keyValue = value;
            }
        }
        private string _keyValue = string.Empty;

        public string ParentField
        {
            get
            {
                return _parentField;
            }
            set
            {
                _parentField = value;
            }
        }
        private string _parentField = string.Empty;

        public string ParentValue
        {
            get
            {
                return _parentValue;
            }
            set
            {
                _parentValue = value;
            }
        }
        private string _parentValue = string.Empty;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
                else
                {
                    // value = value.Replace("%", "[%]").Replace("_", "[_]")
                    // value = value.Replace(";", String.Empty).Replace("<", String.Empty)
                    value = value.Replace("  ", " ").Trim();
                }

                _text = value;

                var arr = Strings.Split(_text, " ");
                _keyWords.Clear();
                if (arr.Length > 0)
                    _keyWords.AddRange(arr.ToList());
            }
        }
        private string _text = string.Empty;

        public List<string> KeyWords
        {
            get
            {
                return _keyWords;
            }
        }
        private List<string> _keyWords = new List<string>();

        public int RequestedRows
        {
            get
            {
                if (_requestedRows == 0 & _newRecord == false & _queryType != queryTypeEnum.Schema)
                {
                    _requestedRows = 100;
                }

                return _requestedRows;
            }
            set
            {
                _requestedRows = value;
            }
        }
        private int _requestedRows;

        public int PageIndex
        {
            get
            {
                if (_pageIndex < 1)
                    _pageIndex = 1;
                return _pageIndex;
            }
            set
            {
                _pageIndex = value;
            }
        }
        private int _pageIndex;

        public bool Paged
        {
            get
            {
                return _paged;
            }
            set
            {
                _paged = value;
            }
        }
        private bool _paged;

        public string ViewSort
        {
            get
            {
                if (_viewSort is null)
                    return string.Empty;
                return _viewSort;
            }
            set
            {
                _viewSort = value;
            }
        }
        private string _viewSort;

        public int TotalRows
        {
            get
            {
                return _totalRows;
            }
            set
            {
                _totalRows = value;
            }
        }
        private int _totalRows;
        public string TotalRowsQuery
        {
            get
            {
                return _totalRowsQuery;
            }
            set
            {
                _totalRowsQuery = value;
            }
        }
        private string _totalRowsQuery;
        public List<FieldValue> FilterList
        {
            get
            {
                return _filterList;
            }
            set
            {
                _filterList = value;
            }
        }
        private List<FieldValue> _filterList;

        public string Filter
        {
            get
            {
                return " (" + FieldValue.ToWhereClause(_filterList) + ") ";
            }
            // Set(ByVal value As String)
            // _keyField = String.Empty
            // _keyValue = String.Empty
            // _filter = value
            // End Set
        }
        // Private _filter As String = String.Empty

        public string WhereFilter
        {
            get
            {
                string filter = FieldValue.ToWhereClause(_filterList);
                if (string.IsNullOrEmpty(filter))
                    return string.Empty;
                return " WHERE (" + filter + ") ";
            }
        }

        public string AndFilter
        {
            get
            {
                string filter = FieldValue.ToWhereClause(_filterList);
                if (string.IsNullOrEmpty(filter))
                    return string.Empty;
                return " AND (" + filter + ") ";
            }
        }

        public CultureInfo Culture
        {
            get
            {
                if (_culture is null)
                    _culture = new CultureInfo("en-US");
                return _culture;
            }
            set
            {
                _culture = value;
            }
        }
        private CultureInfo _culture;

        public queryTypeEnum QueryType
        {
            get
            {
                if (_queryType == 0)
                    _queryType = queryTypeEnum.OpenTable;
                return _queryType;
            }
            set
            {
                if (value == queryTypeEnum.Schema)
                    RequestedRows = 0;
                if (value == queryTypeEnum.OpenTable)
                    _text = string.Empty;
                _queryType = value;
            }
        }
        private queryTypeEnum _queryType;

        public ScopeEnum Scope
        {
            get
            {
                return _Scope;
            }
            set
            {
                _Scope = value;
            }
        }
        private ScopeEnum _Scope;

        public string SortClause
        {
            get
            {
                if (_sortField.Length == 0)
                    return string.Empty;
                if (_sortDecending)
                    return _sortField + " DESC";
                return _sortField + " ASC";
            }
        }

        public string SortField
        {
            get
            {
                return _sortField;
            }
            set
            {
                _sortField = value;
            }
        }
        private string _sortField = string.Empty;

        public bool SortDecending
        {
            get
            {
                return _sortDecending;
            }
            set
            {
                _sortDecending = value;
            }
        }
        private bool _sortDecending;

        public DataTable Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        private DataTable _data;

        public int Indent
        {
            get
            {
                return _indent;
            }
            set
            {
                _indent = value;
            }
        }
        private int _indent;

        public DataTable AttachmentCache
        {
            get
            {
                return _attachmentCache;
            }
            set
            {
                _attachmentCache = value;
            }
        }
        private DataTable _attachmentCache = new DataTable();

        public string APISearchResults
        {
            get
            {
                return _APISearchResults;
            }
            set
            {
                _APISearchResults = value;
            }
        }
        private string _APISearchResults;

        public string HTMLSearchResults
        {
            get
            {
                return _HTMLSearchResults;
            }
            set
            {
                _HTMLSearchResults = value;
            }
        }
        private string _HTMLSearchResults;

        public string XMLSearchResults
        {
            get
            {
                return _XMLSearchResults;
            }
            set
            {
                _XMLSearchResults = value;
            }
        }
        private string _XMLSearchResults;

        public bool IncludeAttachments
        {
            get
            {
                return _includeAttachments;
            }
            set
            {
                _includeAttachments = value;
            }
        }
        private bool _includeAttachments;

        public SearchEnum SearchType
        {
            get
            {
                return _searchType;
            }
            set
            {
                _searchType = value;
            }
        }
        private SearchEnum _searchType;

        public bool ThinClient
        {
            get
            {
                return _thinClient;
            }
            set
            {
                _thinClient = value;
            }
        }
        private bool _thinClient;

        public bool Filtered
        {
            get
            {
                if (_queryType == queryTypeEnum.AdvancedFilter)
                    return true;
                if (_queryType == queryTypeEnum.Text)
                    return true;
                if (_queryType == queryTypeEnum.KeyValuePair)
                    return true;
                return false;
            }
        }

        public bool IncludeViewFilters
        {
            get
            {
                return _includeViewFilters;
            }
            set
            {
                _includeViewFilters = value;
            }
        }
        private bool _includeViewFilters;

        public string WhereClause
        {
            get
            {
                return _whereClause;
            }
            set
            {
                _whereClause = value;
            }
        }
        private string _whereClause = string.Empty;

        public string NodeClause
        {
            get
            {
                return _nodeClause;
            }
            set
            {
                _nodeClause = value;
            }
        }
        private string _nodeClause = string.Empty;

        public int ScrollPosition
        {
            get
            {
                return _scrollPosition;
            }
            set
            {
                _scrollPosition = value;
            }
        }
        private int _scrollPosition = 0;

        public string BeforeData
        {
            get
            {
                return _beforeData;
            }
            set
            {
                _beforeData = value;
            }
        }

        public string BeforeDataTrimmed
        {
            get
            {
                return TrimTrailingCrLf(BeforeData);
            }
        }
        private string _beforeData = string.Empty;

        public string AfterData
        {
            get
            {
                return _afterData;
            }
            set
            {
                _afterData = value;
            }
        }

        public string AfterDataTrimmed
        {
            get
            {
                return TrimTrailingCrLf(AfterData);
            }
        }
        private string _afterData = string.Empty;

        private string TrimTrailingCrLf(string s)
        {
            if (s.Length > 2 && s.EndsWith(Constants.vbCrLf))
                s = s.Substring(0, s.Length - 2);
            if (s.Length > 1 && s.EndsWith(Constants.vbLf))
                s = s.Substring(0, s.Length - 1);
            if (s.Length > 1 && s.EndsWith(Constants.vbCr))
                s = s.Substring(0, s.Length - 1);
            if (s.Length > Environment.NewLine.Length && s.EndsWith(Environment.NewLine))
                s = s.Substring(0, s.Length - Environment.NewLine.Length);
            return s.Trim();
        }
        // added by Moti mashiah
        public bool IsMVCCall
        {
            get
            {
                return _IsMVCCall;
            }
            set
            {
                _IsMVCCall = value;
            }
        }
        private bool _IsMVCCall = false;
    }

    public class FilterType
    {
        public FilterType(Parameters param)
        {
            _parent = param;
        }

        public Parameters Parent
        {
            get
            {
                return _parent;
            }
        }
        private Parameters _parent;

        public queryTypeEnum QueryType
        {
            get
            {
                if (_queryType == 0)
                    _queryType = queryTypeEnum.OpenTable;
                return _queryType;
            }
            set
            {
                // If value = queryTypeEnum.Schema Then RequestedRows = 0
                if (_queryType == queryTypeEnum.OpenTable)
                    _text = string.Empty;
                _queryType = value;
            }
        }
        private queryTypeEnum _queryType;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
                else
                {
                    // value = value.Replace("%", "[%]").Replace("_", "[_]")
                    // value = value.Replace(";", String.Empty).Replace("<", String.Empty)
                    value = value.Replace("  ", " ").Trim();
                }

                _text = value;

                var arr = Strings.Split(_text, " ");
                _keyWords.Clear();
                if (arr.Length > 0)
                    _keyWords.AddRange(arr.ToList());
            }
        }
        private string _text = string.Empty;

        public List<string> KeyWords
        {
            get
            {
                return _keyWords;
            }
        }
        private List<string> _keyWords = new List<string>();

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _keyField = string.Empty;
                _keyValue = string.Empty;
                _filter = value;
            }
        }
        private string _filter = string.Empty;

        public string KeyField
        {
            get
            {
                if (string.IsNullOrEmpty(_keyField))
                {
                    return _parent.PrimaryKey;
                }
                else
                {
                    return _keyField;
                }
            }
            set
            {
                _keyField = value;
            }
        }
        private string _keyField = string.Empty;

        public string KeyValue
        {
            get
            {
                return _keyValue;
            }
            set
            {
                _keyValue = value;
            }
        }
        private string _keyValue = string.Empty;
    }
}