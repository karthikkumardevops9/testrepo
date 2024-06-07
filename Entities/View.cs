using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class View
    {
        private const int MAXRECS = 1000;
        private const int MAXRECSDESKTOP = 5000;
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ViewName { get; set; }
        public string SQLStatement { get; set; }
        private Nullable<int> _MaxRecsPerFetch;
        public Nullable<int> MaxRecsPerFetch
        {
            get
            {
                if (_MaxRecsPerFetch == null)
                    return MAXRECS;
                else
                    return _MaxRecsPerFetch;
            }
            set { _MaxRecsPerFetch = value; }
        }
        public string Picture { get; set; }
        public string ReportStylesId { get; set; }
        private Nullable<int> _ViewOrder;
        public Nullable<int> ViewOrder
        {
            get
            {
                if (_ViewOrder == null)
                    return 0;
                else
                    return _ViewOrder;
            }
            set { _ViewOrder = value; }
        }
        public string WorkFlow1 { get; set; }
        public string WorkFlow1Pic { get; set; }
        public string WorkFlowDesc1 { get; set; }
        public string WorkFlowToolTip1 { get; set; }
        public string WorkFlowHotKey1 { get; set; }
        public string WorkFlow2 { get; set; }
        public string WorkFlow2Pic { get; set; }
        public string WorkFlowDesc2 { get; set; }
        public string WorkFlowToolTip2 { get; set; }
        public string WorkFlowHotKey2 { get; set; }
        public string WorkFlow3 { get; set; }
        public string WorkFlow3Pic { get; set; }
        public string WorkFlowDesc3 { get; set; }
        public string WorkFlowToolTip3 { get; set; }
        public string WorkFlowHotKey3 { get; set; }
        public string WorkFlow4 { get; set; }
        public string WorkFlow4Pic { get; set; }
        public string WorkFlowDesc4 { get; set; }
        public string WorkFlowToolTip4 { get; set; }
        public string WorkFlowHotKey4 { get; set; }
        public string WorkFlow5 { get; set; }
        public string WorkFlow5Pic { get; set; }
        public string WorkFlowDesc5 { get; set; }
        public string WorkFlowToolTip5 { get; set; }
        public string WorkFlowHotKey5 { get; set; }
        private Nullable<int> _TablesId;
        public Nullable<int> TablesId
        {
            get
            {
                if (_TablesId == null)
                    return 0;
                else
                    return _TablesId;
            }
            set { _TablesId = value; }
        }
        private Nullable<int> _ViewGroup;
        public Nullable<int> ViewGroup
        {
            get
            {
                if (_ViewGroup == null)
                    return 0;
                else
                    return _ViewGroup;
            }
            set { _ViewGroup = value; }
        }
        private Nullable<bool> _Visible;
        public Nullable<bool> Visible
        {
            get
            {
                if (_Visible == null)
                    return false;
                else
                    return _Visible;
            }
            set { _Visible = value; }
        }
        private Nullable<bool> _VariableColWidth;
        public Nullable<bool> VariableColWidth
        {
            get
            {
                if (_VariableColWidth == null)
                    return false;
                else
                    return _VariableColWidth;
            }
            set { _VariableColWidth = value; }
        }
        private Nullable<bool> _VariableRowHeight;
        public Nullable<bool> VariableRowHeight
        {
            get
            {
                if (_VariableRowHeight == null)
                    return false;
                else
                    return _VariableRowHeight;
            }
            set { _VariableRowHeight = value; }
        }
        private Nullable<bool> _VariableFixedCols;
        public Nullable<bool> VariableFixedCols
        {
            get
            {
                if (_VariableFixedCols == null)
                    return false;
                else
                    return _VariableFixedCols;
            }
            set { _VariableFixedCols = value; }
        }
        private Nullable<short> _RowHeight;
        public Nullable<short> RowHeight
        {
            get
            {
                return _RowHeight;
            }
            set { _RowHeight = value; }
        }
        private Nullable<bool> _AddAllowed;
        public Nullable<bool> AddAllowed
        {
            get
            {
                if (_AddAllowed == null)
                    return false;
                else
                    return _AddAllowed;
            }
            set { _AddAllowed = value; }
        }
        private Nullable<short> _ViewType;
        public Nullable<short> ViewType
        {
            get
            {
                return _ViewType;
            }
            set { _ViewType = value; }
        }
        private Nullable<bool> _UseExactRowCount;
        public Nullable<bool> UseExactRowCount
        {
            get
            {
                if (_UseExactRowCount == null)
                    return false;
                else
                    return _UseExactRowCount;
            }
            set { _UseExactRowCount = value; }
        }
        public string TablesDown { get; set; }
        private Nullable<bool> _Printable;
        public Nullable<bool> Printable
        {
            get
            {
                if (_Printable == null)
                    return false;
                else
                    return _Printable;
            }
            set { _Printable = value; }
        }
        private Nullable<bool> _GrandTotal;
        public Nullable<bool> GrandTotal
        {
            get
            {
                if (_GrandTotal == null)
                    return false;
                else
                    return _GrandTotal;
            }
            set { _GrandTotal = value; }
        }
        private Nullable<int> _LeftIndent;
        public Nullable<int> LeftIndent
        {
            get
            {
                if (_LeftIndent == null)
                    return 0;
                else
                    return _LeftIndent;
            }
            set { _LeftIndent = value; }
        }
        private Nullable<int> _RightIndent;
        public Nullable<int> RightIndent
        {
            get
            {
                if (_RightIndent == null)
                    return 0;
                else
                    return _RightIndent;
            }
            set { _RightIndent = value; }
        }
        public string SubTableName { get; set; }
        private Nullable<int> _SubViewId;
        public Nullable<int> SubViewId
        {
            get
            {
                if (_SubViewId == null)
                    return 0;
                else
                    return _SubViewId;
            }
            set { _SubViewId = value; }
        }
        private Nullable<bool> _PrintWithoutChildren;
        public Nullable<bool> PrintWithoutChildren
        {
            get
            {
                if (_PrintWithoutChildren == null)
                    return false;
                else
                    return _PrintWithoutChildren;
            }
            set { _PrintWithoutChildren = value; }
        }
        private Nullable<bool> _SuppressHeader;
        public Nullable<bool> SuppressHeader
        {
            get
            {
                if (_SuppressHeader == null)
                    return false;
                else
                    return _SuppressHeader;
            }
            set { _SuppressHeader = value; }
        }
        private Nullable<bool> _SuppressFooter;
        public Nullable<bool> SuppressFooter
        {
            get
            {
                if (_SuppressFooter == null)
                    return false;
                else
                    return _SuppressFooter;
            }
            set { _SuppressFooter = value; }
        }
        private Nullable<bool> _PrintFrozenOnly;
        public Nullable<bool> PrintFrozenOnly
        {
            get
            {
                if (_PrintFrozenOnly == null)
                    return false;
                else
                    return _PrintFrozenOnly;
            }
            set { _PrintFrozenOnly = value; }
        }
        private Nullable<bool> _TrackingEverContained;
        public Nullable<bool> TrackingEverContained
        {
            get
            {
                if (_TrackingEverContained == null)
                    return false;
                else
                    return _TrackingEverContained;
            }
            set { _TrackingEverContained = value; }
        }
        private Nullable<bool> _PrintImages;
        public Nullable<bool> PrintImages
        {
            get
            {
                if (_PrintImages == null)
                    return false;
                else
                    return _PrintImages;
            }
            set { _PrintImages = value; }
        }
        private Nullable<bool> _PrintImageFullPage;
        public Nullable<bool> PrintImageFullPage
        {
            get
            {
                if (_PrintImageFullPage == null)
                    return false;
                else
                    return _PrintImageFullPage;
            }
            set { _PrintImageFullPage = value; }
        }
        private Nullable<bool> _PrintImageFirstPageOnly;
        public Nullable<bool> PrintImageFirstPageOnly
        {
            get
            {
                if (_PrintImageFirstPageOnly == null)
                    return false;
                else
                    return _PrintImageFirstPageOnly;
            }
            set { _PrintImageFirstPageOnly = value; }
        }
        private Nullable<bool> _PrintImageRedlining;
        public Nullable<bool> PrintImageRedlining
        {
            get
            {
                if (_PrintImageRedlining == null)
                    return false;
                else
                    return _PrintImageRedlining;
            }
            set { _PrintImageRedlining = value; }
        }
        private Nullable<int> _PrintImageLeftMargin;
        public Nullable<int> PrintImageLeftMargin
        {
            get
            {
                if (_PrintImageLeftMargin == null)
                    return 0;
                else
                    return _PrintImageLeftMargin;
            }
            set { _PrintImageLeftMargin = value; }
        }
        private Nullable<int> _PrintImageRightMargin;
        public Nullable<int> PrintImageRightMargin
        {
            get
            {
                if (_PrintImageRightMargin == null)
                    return 0;
                else
                    return _PrintImageRightMargin;
            }
            set { _PrintImageRightMargin = value; }
        }
        private Nullable<bool> _PrintImageAllVersions;
        public Nullable<bool> PrintImageAllVersions
        {
            get
            {
                if (_PrintImageAllVersions == null)
                    return false;
                else
                    return _PrintImageAllVersions;
            }
            set { _PrintImageAllVersions = value; }
        }
        private Nullable<int> _ChildColumnHeaders;
        public Nullable<int> ChildColumnHeaders
        {
            get
            {
                if (_ChildColumnHeaders == null)
                    return 0;
                else
                    return _ChildColumnHeaders;
            }
            set { _ChildColumnHeaders = value; }
        }
        private Nullable<bool> _SuppressImageDataRow;
        public Nullable<bool> SuppressImageDataRow
        {
            get
            {
                if (_SuppressImageDataRow == null)
                    return false;
                else
                    return _SuppressImageDataRow;
            }
            set { _SuppressImageDataRow = value; }
        }
        private Nullable<bool> _SuppressImageFooter;
        public Nullable<bool> SuppressImageFooter
        {
            get
            {
                if (_SuppressImageFooter == null)
                    return false;
                else
                    return _SuppressImageFooter;
            }
            set { _SuppressImageFooter = value; }
        }
        private Nullable<int> _DisplayMode;
        public Nullable<int> DisplayMode
        {
            get
            {
                if (_DisplayMode == null)
                    return 0;
                else
                    return _DisplayMode;
            }
            set { _DisplayMode = value; }
        }
        private Nullable<bool> _AutoRotateImage;
        public Nullable<bool> AutoRotateImage
        {
            get
            {
                if (_AutoRotateImage == null)
                    return false;
                else
                    return _AutoRotateImage;
            }
            set { _AutoRotateImage = value; }
        }
        private Nullable<bool> _GrandTotalOnSepPage;
        public Nullable<bool> GrandTotalOnSepPage
        {
            get
            {
                if (_GrandTotalOnSepPage == null)
                    return false;
                else
                    return _GrandTotalOnSepPage;
            }
            set { _GrandTotalOnSepPage = value; }
        }
        public string UserName { get; set; }
        private Nullable<bool> _IncludeFileRoomOrder;
        public Nullable<bool> IncludeFileRoomOrder
        {
            get
            {
                if (_IncludeFileRoomOrder == null)
                    return false;
                else
                    return _IncludeFileRoomOrder;
            }
            set { _IncludeFileRoomOrder = value; }
        }
        private Nullable<int> _AltViewId;
        public Nullable<int> AltViewId
        {
            get
            {
                if (_AltViewId == null)
                    return 0;
                else
                    return _AltViewId;
            }
            set { _AltViewId = value; }
        }
        private Nullable<bool> _DeleteGridAvail;
        public Nullable<bool> DeleteGridAvail
        {
            get
            {
                if (_DeleteGridAvail == null)
                    return false;
                else
                    return _DeleteGridAvail;
            }
            set { _DeleteGridAvail = value; }
        }
        private Nullable<bool> _FiltersActive;
        public Nullable<bool> FiltersActive
        {
            get
            {
                if (_FiltersActive == null)
                    return false;
                else
                    return _FiltersActive;
            }
            set { _FiltersActive = value; }
        }
        private Nullable<bool> _IncludeTrackingLocation;
        public Nullable<bool> IncludeTrackingLocation
        {
            get
            {
                if (_IncludeTrackingLocation == null)
                    return false;
                else
                    return _IncludeTrackingLocation;
            }
            set { _IncludeTrackingLocation = value; }
        }
        private Nullable<bool> _InTaskList;
        public Nullable<bool> InTaskList
        {
            get
            {
                if (_InTaskList == null)
                    return false;
                else
                    return _InTaskList;
            }
            set { _InTaskList = value; }
        }
        public string TaskListDisplayString { get; set; }
        private Nullable<int> _PrintAttachments;
        public Nullable<int> PrintAttachments
        {
            get
            {
                if (_PrintAttachments == null)
                    return 0;
                else
                    return _PrintAttachments;
            }
            set { _PrintAttachments = value; }
        }
        private Nullable<bool> _MultiParent;
        public Nullable<bool> MultiParent
        {
            get
            {
                if (_MultiParent == null)
                    return false;
                else
                    return _MultiParent;
            }
            set { _MultiParent = value; }
        }
        private Nullable<bool> _SearchableView;
        public Nullable<bool> SearchableView
        {
            get
            {
                if (_SearchableView == null)
                    return false;
                else
                    return _SearchableView;
            }
            set { _SearchableView = value; }
        }
        private Nullable<bool> _CustomFormView;
        public Nullable<bool> CustomFormView
        {
            get
            {
                if (_CustomFormView == null)
                    return false;
                else
                    return _CustomFormView;
            }
            set { _CustomFormView = value; }
        }
        private Nullable<int> _MaxRecsPerFetchDesktop;
        public Nullable<int> MaxRecsPerFetchDesktop
        {
            get
            {
                if (_MaxRecsPerFetchDesktop == null)
                    return MAXRECSDESKTOP;
                else
                    return _MaxRecsPerFetchDesktop;
            }
            set { _MaxRecsPerFetchDesktop = value; }
        }
    }
}
