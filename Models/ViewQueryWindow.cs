using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Smead.Security;
using System.Data;
using MSRecordsEngine.RecordsManager;

// QUERY WINDOW MODEL RETURN DOM OBJECT
namespace MSRecordsEngine.Models
{

    public class ViewQueryWindow
    {
        public ViewQueryWindow()
        {
            ListOfRows = new List<string>();
            MyqueryList = new List<queryList>();
            listMyqueryDatatype = new List<string>();
        }
        private Query _query { get; set; }
        private Parameters _Params { get; set; }
        public List<string> ListOfRows { get; set; }
        public List<queryList> MyqueryList { get; set; }
        public List<string> listMyqueryDatatype { get; set; }
        public bool hasMyQueryAceess { get; set; } = false;
        public string dateFormat { get; set; }
        private int crumblevel { get; set; }
        public string ChildKeyField { get; set; }
    }
    public class QueryHoldingProps
    {
        public bool IsWhereClauseRequest { get; set; }
        public string WhereClauseStr { get; set; }
        public bool GsIsGlobalSearch { get; set; }
        public string GsSearchText { get; set; }
        public bool GsIncludeAttchment { get; set; }
        public bool GsIsAllGlobalRequest { get; set; }
        public string HoldTotalRowQuery { get; set; }
        public List<TableEditableHeaderquery> EditableCells { get; set; }
    }

    public class queryList
    {
        public string operators { get; set; }
        public string columnName { get; set; }
        public string values { get; set; }
        public string SaveName { get; set; }
        public int ViewId { get; set; }
        public int SavedCriteriaid { get; set; }
        public int type { get; set; }
        public string ColumnType { get; set; }
    }

    public class TableEditableHeaderquery
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
}