using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ViewColumn
    {
        public int Id { get; set; }
        private Nullable<int> _ViewsId;
        public Nullable<int> ViewsId
        {
            get
            {
                if (_ViewsId == null)
                    return 0;
                else
                    return _ViewsId;
            }
            set { _ViewsId = value; }
        }
        private Nullable<short> _ColumnNum;
        public Nullable<short> ColumnNum
        {
            get
            {
                return _ColumnNum;
            }
            set { _ColumnNum = value; }
        }
        public string FieldName { get; set; }
        public string Heading { get; set; }
        private Nullable<short> _LookupType;
        public Nullable<short> LookupType
        {
            get
            {
                return _LookupType;
            }
            set { _LookupType = value; }
        }
        private Nullable<short> _ColumnWidth;
        public Nullable<short> ColumnWidth
        {
            get
            {
                return _ColumnWidth;
            }
            set { _ColumnWidth = value; }
        }
        private Nullable<bool> _ColumnVisible;
        public Nullable<bool> ColumnVisible
        {
            get
            {
                if (_ColumnVisible == null)
                    return false;
                else
                    return _ColumnVisible;
            }
            set { _ColumnVisible = value; }
        }
        private Nullable<short> _ColumnOrder;
        public Nullable<short> ColumnOrder
        {
            get
            {
                return _ColumnOrder;
            }
            set { _ColumnOrder = value; }
        }
        private Nullable<short> _ColumnStyle;
        public Nullable<short> ColumnStyle
        {
            get
            {
                return _ColumnStyle;
            }
            set { _ColumnStyle = value; }
        }
        public string EditMask { get; set; }
        public string Picture { get; set; }
        private Nullable<short> _LookupIdCol;
        public Nullable<short> LookupIdCol
        {
            get
            {
                return _LookupIdCol;
            }
            set { _LookupIdCol = value; }
        }
        private Nullable<short> _SortField;
        public Nullable<short> SortField
        {
            get
            {
                return _SortField;
            }
            set { _SortField = value; }
        }
        private Nullable<bool> _SortableField;
        public Nullable<bool> SortableField
        {
            get
            {
                if (_SortableField == null)
                    return false;
                else
                    return _SortableField;
            }
            set { _SortableField = value; }
        }
        private Nullable<bool> _FilterField;
        public Nullable<bool> FilterField
        {
            get
            {
                if (_FilterField == null)
                    return false;
                else
                    return _FilterField;
            }
            set { _FilterField = value; }
        }
        private Nullable<bool> _CountColumn;
        public Nullable<bool> CountColumn
        {
            get
            {
                if (_CountColumn == null)
                    return false;
                else
                    return _CountColumn;
            }
            set { _CountColumn = value; }
        }
        private Nullable<bool> _SubtotalColumn;
        public Nullable<bool> SubtotalColumn
        {
            get
            {
                if (_SubtotalColumn == null)
                    return false;
                else
                    return _SubtotalColumn;
            }
            set { _SubtotalColumn = value; }
        }
        private Nullable<bool> _PrintColumnAsSubheader;
        public Nullable<bool> PrintColumnAsSubheader
        {
            get
            {
                if (_PrintColumnAsSubheader == null)
                    return false;
                else
                    return _PrintColumnAsSubheader;
            }
            set { _PrintColumnAsSubheader = value; }
        }
        private Nullable<bool> _RestartPageNumber;
        public Nullable<bool> RestartPageNumber
        {
            get
            {
                if (_RestartPageNumber == null)
                    return false;
                else
                    return _RestartPageNumber;
            }
            set { _RestartPageNumber = value; }
        }
        private Nullable<bool> _UseAsPrintId;
        public Nullable<bool> UseAsPrintId
        {
            get
            {
                if (_UseAsPrintId == null)
                    return false;
                else
                    return _UseAsPrintId;
            }
            set { _UseAsPrintId = value; }
        }
        private Nullable<bool> _DropDownSuggestionOnly;
        public Nullable<bool> DropDownSuggestionOnly
        {
            get
            {
                if (_DropDownSuggestionOnly == null)
                    return false;
                else
                    return _DropDownSuggestionOnly;
            }
            set { _DropDownSuggestionOnly = value; }
        }
        private Nullable<bool> _SuppressPrinting;
        public Nullable<bool> SuppressPrinting
        {
            get
            {
                if (_SuppressPrinting == null)
                    return false;
                else
                    return _SuppressPrinting;
            }
            set { _SuppressPrinting = value; }
        }
        private Nullable<bool> _ValueCount;
        public Nullable<bool> ValueCount
        {
            get
            {
                if (_ValueCount == null)
                    return false;
                else
                    return _ValueCount;
            }
            set { _ValueCount = value; }
        }
        public string AlternateFieldName { get; set; }
        public string DefaultLookupValue { get; set; }
        public string DropDownFilterIdField { get; set; }
        public string DropDownFilterMatchField { get; set; }
        private Nullable<short> _DropDownFlag;
        public Nullable<short> DropDownFlag
        {
            get
            {
                return _DropDownFlag;
            }
            set { _DropDownFlag = value; }
        }
        private Nullable<short> _DropDownReferenceColNum;
        public Nullable<short> DropDownReferenceColNum
        {
            get
            {
                return _DropDownReferenceColNum;
            }
            set { _DropDownReferenceColNum = value; }
        }
        public string DropDownReferenceValue { get; set; }
        public string DropDownTargetField { get; set; }
        private Nullable<bool> _EditAllowed;
        public Nullable<bool> EditAllowed
        {
            get
            {
                if (_EditAllowed == null)
                    return false;
                else
                    return _EditAllowed;
            }
            set { _EditAllowed = value; }
        }
        private Nullable<int> _FormColWidth;
        public Nullable<int> FormColWidth
        {
            get
            {
                if (_FormColWidth == null)
                    return 0;
                else
                    return _FormColWidth;
            }
            set { _FormColWidth = value; }
        }
        private Nullable<int> _FreezeOrder;
        public Nullable<int> FreezeOrder
        {
            get
            {
                if (_FreezeOrder == null)
                    return 0;
                else
                    return _FreezeOrder;
            }
            set { _FreezeOrder = value; }
        }
        public string InputMask { get; set; }
        private Nullable<bool> _MaskClipMode;
        public Nullable<bool> MaskClipMode
        {
            get
            {
                if (_MaskClipMode == null)
                    return false;
                else
                    return _MaskClipMode;
            }
            set { _MaskClipMode = value; }
        }
        private Nullable<bool> _MaskInclude;
        public Nullable<bool> MaskInclude
        {
            get
            {
                if (_MaskInclude == null)
                    return false;
                else
                    return _MaskInclude;
            }
            set { _MaskInclude = value; }
        }
        public string MaskPromptChar { get; set; }
        private Nullable<int> _MaxPrintLines;
        public Nullable<int> MaxPrintLines
        {
            get
            {
                if (_MaxPrintLines == null)
                    return 0;
                else
                    return _MaxPrintLines;
            }
            set { _MaxPrintLines = value; }
        }
        private Nullable<bool> _PageBreakField;
        public Nullable<bool> PageBreakField
        {
            get
            {
                if (_PageBreakField == null)
                    return false;
                else
                    return _PageBreakField;
            }
            set { _PageBreakField = value; }
        }
        private Nullable<int> _PrinterColWidth;
        public Nullable<int> PrinterColWidth
        {
            get
            {
                if (_PrinterColWidth == null)
                    return 0;
                else
                    return _PrinterColWidth;
            }
            set { _PrinterColWidth = value; }
        }
        private Nullable<int> _SortOrder;
        public Nullable<int> SortOrder
        {
            get
            {
                if (_SortOrder == null)
                    return 0;
                else
                    return _SortOrder;
            }
            set { _SortOrder = value; }
        }
        private Nullable<bool> _SortOrderDesc;
        public Nullable<bool> SortOrderDesc
        {
            get
            {
                if (_SortOrderDesc == null)
                    return false;
                else
                    return _SortOrderDesc;
            }
            set { _SortOrderDesc = value; }
        }
        private Nullable<bool> _SuppressDuplicates;
        public Nullable<bool> SuppressDuplicates
        {
            get
            {
                if (_SuppressDuplicates == null)
                    return false;
                else
                    return _SuppressDuplicates;
            }
            set { _SuppressDuplicates = value; }
        }
        private Nullable<bool> _VisibleOnForm;
        public Nullable<bool> VisibleOnForm
        {
            get
            {
                if (_VisibleOnForm == null)
                    return false;
                else
                    return _VisibleOnForm;
            }
            set { _VisibleOnForm = value; }
        }
        private Nullable<bool> _VisibleOnPrint;
        public Nullable<bool> VisibleOnPrint
        {
            get
            {
                if (_VisibleOnPrint == null)
                    return false;
                else
                    return _VisibleOnPrint;
            }
            set { _VisibleOnPrint = value; }
        }
        private Nullable<int> _AlternateSortColumn;
        public Nullable<int> AlternateSortColumn
        {
            get
            {
                if (_AlternateSortColumn == null)
                    return 0;
                else
                    return _AlternateSortColumn;
            }
            set { _AlternateSortColumn = value; }
        }
        private Nullable<int> _LabelLeft;
        public Nullable<int> LabelLeft
        {
            get
            {
                if (_LabelLeft == null)
                    return 0;
                else
                    return _LabelLeft;
            }
            set { _LabelLeft = value; }
        }
        private Nullable<int> _LabelTop;
        public Nullable<int> LabelTop
        {
            get
            {
                if (_LabelTop == null)
                    return 0;
                else
                    return _LabelTop;
            }
            set { _LabelTop = value; }
        }
        private Nullable<int> _LabelWidth;
        public Nullable<int> LabelWidth
        {
            get
            {
                if (_LabelWidth == null)
                    return 0;
                else
                    return _LabelWidth;
            }
            set { _LabelWidth = value; }
        }
        private Nullable<int> _LabelHeight;
        public Nullable<int> LabelHeight
        {
            get
            {
                if (_LabelHeight == null)
                    return 0;
                else
                    return _LabelHeight;
            }
            set { _LabelHeight = value; }
        }
        private Nullable<int> _ControlLeft;
        public Nullable<int> ControlLeft
        {
            get
            {
                if (_ControlLeft == null)
                    return 0;
                else
                    return _ControlLeft;
            }
            set { _ControlLeft = value; }
        }
        private Nullable<int> _ControlTop;
        public Nullable<int> ControlTop
        {
            get
            {
                if (_ControlTop == null)
                    return 0;
                else
                    return _ControlTop;
            }
            set { _ControlTop = value; }
        }
        private Nullable<int> _ControlWidth;
        public Nullable<int> ControlWidth
        {
            get
            {
                if (_ControlWidth == null)
                    return 0;
                else
                    return _ControlWidth;
            }
            set { _ControlWidth = value; }
        }
        private Nullable<int> _ControlHeight;
        public Nullable<int> ControlHeight
        {
            get
            {
                if (_ControlHeight == null)
                    return 0;
                else
                    return _ControlHeight;
            }
            set { _ControlHeight = value; }
        }
        private Nullable<int> _TabOrder;
        public Nullable<int> TabOrder
        {
            get
            {
                if (_TabOrder == null)
                    return 0;
                else
                    return _TabOrder;
            }
            set { _TabOrder = value; }
        }
        private Nullable<int> _LabelJustify;
        public Nullable<int> LabelJustify
        {
            get
            {
                if (_LabelJustify == null)
                    return 0;
                else
                    return _LabelJustify;
            }
            set { _LabelJustify = value; }
        }
    }
}
