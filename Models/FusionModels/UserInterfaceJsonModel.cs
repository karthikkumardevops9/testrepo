using System.Collections.Generic;

namespace MSRecordsEngine.Models.FusionModels
{
   
    public class UserInterfaceJsonModel
    {
        public UserInterfaceJsonModel()
        {
            MoveRecords = new MoveRecords();
            Transfer = new Transfer();
            Request = new RequestUi();
        }

        public List<string> ids { get; set; }
        public string TableName { get; set; }
        public string ViewName { get; set; }

        public int ViewId { get; set; }

        public int RecordCount { get; set; }
        public PrintBarcode PrintBarcode { get; set; }
        public MoveRecords MoveRecords { get; set; }
        public Transfer Transfer { get; set; }
        public RequestUi Request
        {
            get;
            set;
        }
        public ImportFavorite ImportFavorite
        {
            get;
            set;
        }
    }

    public class ImportFavoriteReqModel
    {

        public ImportFavorite ImportFavorite
        {
            get;
            set;
        }
    }

    public class DialogMsgConfirmDeleteReqModel
    {
        public UserInterfaceJsonModel paramss { get; set; }
    }


    public class UserInterfaceJsonReqModel
    {
        public UserInterfaceJsonModel paramss { get; set; }

    }
    public class BtnMoveItemsReqModel
    {
        public UserInterfaceJsonModel paramsUI { get; set; }
    }

    public class PrintBarcode
    {
        public int labelFormSelectedValue
        {
            get;
            set;
        }
        public int labelDesignSelectedValue
        {
            get;
            set;
        }
        public bool labelOutline
        {
            get;
            set;
        }
        public int strtPrinting
        {
            get;
            set;
        }
        public int labelIndex
        {
            get;
            set;
        }
        public bool isLabelDropdown
        {
            get;
            set;
        }
        public List<SortableFileds> sortableFields
        {
            get;
            set;
        } = new List<SortableFileds>();
    }

    public class MoveRecords
    {
        public int MoveViewid
        {
            get;
            set;
        }
        public string TextFilter
        {
            get;
            set;
        }
        public string fieldName
        {
            get;
            set;
        }
        public string fieldItemValue
        {
            get;
            set;
        }
    }

    public class Transfer
    {
        public string ContainerTableName
        {
            get;
            set;
        }
        public string ContainerViewid
        {
            get;
            set;
        }
        public string ContainerItemValue
        {
            get;
            set;
        }
        public string TxtDueBack
        {
            get;
            set;
        }
        public string TextFilter
        {
            get;
            set;
        }
        public bool IsSelectAllData
        {
            get;
            set;
        }
    }

    public class RequestUi
    {
        public string TextFilter
        {
            get;
            set;
        }
        public string Employeeid
        {
            get;
            set;
        }
        public string Priotiry
        {
            get;
            set;
        }
        public string Instruction
        {
            get;
            set;
        }
        public string ReqDate
        {
            get;
            set;
        }
        public bool ischeckWaitlist
        {
            get;
            set;
        }
    }

    public class ImportFavorite
    {
        public string SelectedDropdown
        {
            get;
            set;
        }
        public string ext
        {
            get;
            set;
        }
        public bool chk1RowFieldNames { get; set; } = false;
        public string sourceFile
        {
            get;
            set;
        }
        public bool isRightColumnChk { get; set; } = false;
        public bool isLeftColumnChk { get; set; } = false;
        public string favoritListSelectorid
        {
            get;
            set;
        }
        public List<ListOfFieldName> Targetfileds
        {
            get;
            set;
        }
        public string ColumnSelect
        {
            get;
            set;
        }
        public bool IscolString { get; set; } = false;
    }
    public class TabQuikUI
    {
        public string pTableName { get; set; }
        public string dtTABQUIKData { get; set; }
    }
    public class TabQuickSQL
    {
        public string SQLStatement { get; set; }
        public string sTableName { get; set; }
        public bool IsSelectStatement { get; set; }
    }
    public class TabQuickSaveTABQUIKFields
    {
        public string sOperation { get; set; }
        public string JobName { get; set; }
        public string TableName { get; set; }
        public string SQLSelectString { get; set; }
        public string SQLUpdateString { get; set; }
        public string dtTABQUIKData { get; set; }

    }
    
}
