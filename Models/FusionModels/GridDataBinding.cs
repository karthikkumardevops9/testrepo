using Microsoft.AspNetCore.Http;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Models.FusionModels
{
    public class GridDataBinding
    {
        public GridDataBinding()
        {
            ListOfColumn = new List<string>();
            fvList = new List<FieldValue>();
            ListOfHeaders = new List<TableHeadersProperty>();
            ListofEditableHeader = new List<TableEditableHeader>();
            ListOfDatarows = new List<List<string>>();
            ListOfAttachmentLinks = new List<string>();
            ListOfBreadCrumbsRightClick = new List<BreadCrumbsRightClick>();
            ListOfBreadCrumbs = new List<BreadCrumbsUI>();
            ListOfdropdownColumns = new List<DropDownproperties>();
            ListOfColumnWidths = new List<int>();
            sortableFields = new List<SortableFileds>();
            RightClickToolBar = new RightclickToolBar();

        }
        public int fViewType;
        private TableColums Cell;
        private List<string> ListOfColumn { get; set; }

        //[JsonIgnore]
        //private IRepository<Table> _tables { get; set; }

        //[JsonIgnore]
        //public IRepository<View> _iView { get; set; }
        public string WhereClauseStr { get; set; }
        public bool IsWhereClauseRequest { get; set; } = false;
        public bool GsIsGlobalSearch { get; set; } = false;
        public string GsKeyvalue { get; set; }
        public string GsSearchText { get; set; }
        public bool GsIsAllGlobalRequest { get; set; }
        public bool GsIncludeAttchment { get; set; }
        public string ToolBarHtml { get; set; }
        public List<FieldValue> fvList { get; set; }
        public List<TableHeadersProperty> ListOfHeaders { get; set; }
        public List<TableEditableHeader> ListofEditableHeader { get; set; }
        public List<List<string>> ListOfDatarows { get; set; }
        public bool HasAttachmentcolumn { get; set; }
        public bool HasDrillDowncolumn { get; set; }
        public List<string> ListOfAttachmentLinks { get; set; }
        public string ListOfdrilldownLinks { get; set; }
        public List<BreadCrumbsRightClick> ListOfBreadCrumbsRightClick { get; set; }
        public List<BreadCrumbsUI> ListOfBreadCrumbs { get; set; }
        public List<DropDownproperties> ListOfdropdownColumns { get; set; }
        public int PageNumber { get; set; }
        public int TotalRows { get; set; }
        public int RowPerPage { get; set; }
        public int TotalPagesNumber { get; set; }
        public bool ShowTrackableTable { get; set; }
        public string ChildKeyField { get; set; }
        public int ViewId { get; set; }
        public string ViewName { get; set; }
        public string TableName { get; set; }
        public RightclickToolBar RightClickToolBar { get; set; }
        internal bool IsOpenWhereClause { get; set; } = false;
        public string ChildKeyname { get; set; }
        public string Message { get; set; }
        public bool hasPermission { get; set; }
        public string ItemDescription { get; set; }
        public int crumbLevel { get; set; }
        public string dateFormat { get; set; }
        public List<int> ListOfColumnWidths { get; set; }
        public List<SortableFileds> sortableFields { get; set; } = new List<SortableFileds>();
        public string IdFieldDataType { get; set; }
        public string TotalRowsQuery { get; set; }
    }

    public class BreadCrumbsRightClick
    {
        public int viewId { get; set; }
        public string viewName { get; set; }
    }

    public class BreadCrumbsUI
    {
        public string ChildTableName { get; set; }
        public string ChildKeyField { get; set; }
        public string Childid { get; set; }
        public int ChildViewid { get; set; }
        public string TableName { get; set; }
        public string ViewName { get; set; }
        public int ViewId { get; set; }
        public string ChildUserName { get; set; }
        public string ChildViewName { get; set; }
        public string ChildKeyType { get; set; }
        public string preTableName { get; set; }
    }
    public class RightclickToolBar
    {
        public bool Menu1Print { get; set; } 
        public bool Menu1Tabquick { get; set; }
        public bool Menu1btnBlackWhite { get; set; }
        public bool Menu1btnExportCSV { get; set; }
        public bool Menu1btnExportCSVAll { get; set; }
        public bool Menu1btnExportTXT { get; set; }
        public bool Menu1btnExportTXTAll { get; set; }
        public bool Menu2btnTransfer { get; set; }
        public bool Menu2btnTransfersTransferAll { get; set; }
        public bool Menu2delete { get; set; }
        public bool Menu2move { get; set; }
        public bool Menu2btnTracking { get; set; }
        public bool Menu2btnRequest { get; set; }
        public bool Favorive { get; set; }
    }
    // SEARCH INTERNAL MODEL FOR QUERY SEARCH
    public class searchQueryModel
    {
        public string columnName { get; set; }
        public string ColumnType { get; set; }
        public string operators { get; set; }
        public string values { get; set; }
   

    }
    
    public class TableHeadersProperty
    {
        public TableHeadersProperty(string headername, string issort, string datatype, string isdropdown, string isEditable, int columnOrder, string editmask, bool allownull, string dataTypeFullName, string ColumnName, bool isprimarykey, int maxlength, bool iscounterField)
        {
            HeaderName = headername;
            Issort = Convert.ToBoolean(issort);
            DataType = datatype;
            isDropdown = Convert.ToBoolean(isdropdown);
            this.isEditable = Convert.ToBoolean(isEditable);
            this.columnOrder = columnOrder;
            editMask = editmask;
            Allownull = allownull;
            DataTypeFullName = dataTypeFullName;
            this.ColumnName = ColumnName;
            IsPrimarykey = isprimarykey;
            MaxLength = maxlength;
            isCounterField = iscounterField;
        }
        public string HeaderName { get; set; }
        public bool Issort { get; set; }
        public string DataType { get; set; }
        public bool isDropdown { get; set; }
        public bool isEditable { get; set; }
        public int columnOrder { get; set; }
        public string editMask { get; set; }
        public bool Allownull { get; set; }
        public string DataTypeFullName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimarykey { get; set; }
        public int MaxLength { get; set; }
        public bool isCounterField { get; set; }
    }

    public class TableEditableHeader
    {
        public string HeaderName { get; set; }
        public bool Issort { get; set; }
        public string DataType { get; set; }
        public bool isDropdown { get; set; }
        public bool isEditable { get; set; }
        public int columnOrder { get; set; }
        public string editMask { get; set; }
        public bool Allownull { get; set; }
        public string DataTypeFullName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimarykey { get; set; }
        public int MaxLength { get; set; }
        public bool isCounterField { get; set; }
        public string DefaultRetentionId { get; set; }
    }

    public class TableColums
    {
        public string DataColumn { get; set; }
    }

    public class DropDownproperties
    {
        public DropDownproperties(int colOrder, List<string> lvalue, List<string> ldisplay)
        {
            colorder = colOrder;
            ListValue = lvalue;
            ListDisplay = ldisplay;
        }
        public int colorder { get; set; }
        public string value { get; set; }
        public string display { get; set; }
        public List<string> ListValue { get; set; }
        public List<string> ListDisplay { get; set; }
    }

    public class SortableFileds
    {
        public string FieldName { get; set; }
        public int SortOrder { get; set; }
        public bool SortOrderDesc { get; set; }
    }
}
