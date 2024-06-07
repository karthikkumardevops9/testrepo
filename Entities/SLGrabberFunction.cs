using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLGrabberFunction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private Nullable<bool> _UseParentWindowClass;
        public Nullable<bool> UseParentWindowClass
        {
            get
            {
                if (_UseParentWindowClass == null)
                    return false;
                else
                    return _UseParentWindowClass;
            }
            set { _UseParentWindowClass = value; }
        }
        public string ParentWindowClassName { get; set; }
        private Nullable<bool> _UseParentTitleMask;
        public Nullable<bool> UseParentTitleMask
        {
            get
            {
                if (_UseParentTitleMask == null)
                    return false;
                else
                    return _UseParentTitleMask;
            }
            set { _UseParentTitleMask = value; }
        }
        public string ParentTitleMask { get; set; }
        private Nullable<bool> _UseParentSize;
        public Nullable<bool> UseParentSize
        {
            get
            {
                if (_UseParentSize == null)
                    return false;
                else
                    return _UseParentSize;
            }
            set { _UseParentSize = value; }
        }
        private Nullable<int> _ParentWidth;
        public Nullable<int> ParentWidth
        {
            get
            {
                if (_ParentWidth == null)
                    return 0;
                else
                    return _ParentWidth;
            }
            set { _ParentWidth = value; }
        }
        private Nullable<int> _ParentHeight;
        public Nullable<int> ParentHeight
        {
            get
            {
                if (_ParentHeight == null)
                    return 0;
                else
                    return _ParentHeight;
            }
            set { _ParentHeight = value; }
        }
        private Nullable<bool> _UseParentPosition;
        public Nullable<bool> UseParentPosition
        {
            get
            {
                if (_UseParentPosition == null)
                    return false;
                else
                    return _UseParentPosition;
            }
            set { _UseParentPosition = value; }
        }
        private Nullable<int> _ParentLeft;
        public Nullable<int> ParentLeft
        {
            get
            {
                if (_ParentLeft == null)
                    return 0;
                else
                    return _ParentLeft;
            }
            set { _ParentLeft = value; }
        }
        private Nullable<int> _ParentTop;
        public Nullable<int> ParentTop
        {
            get
            {
                if (_ParentTop == null)
                    return 0;
                else
                    return _ParentTop;
            }
            set { _ParentTop = value; }
        }
        public string TableName { get; set; }
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
        public string ChildTableName { get; set; }
        private Nullable<int> _ChildViewsId;
        public Nullable<int> ChildViewsId
        {
            get
            {
                if (_ChildViewsId == null)
                    return 0;
                else
                    return _ChildViewsId;
            }
            set { _ChildViewsId = value; }
        }
        private Nullable<int> _OneStripJobsId;
        public Nullable<int> OneStripJobsId
        {
            get
            {
                if (_OneStripJobsId == null)
                    return 0;
                else
                    return _OneStripJobsId;
            }
            set { _OneStripJobsId = value; }
        }
        private Nullable<bool> _ShowTree;
        public Nullable<bool> ShowTree
        {
            get
            {
                if (_ShowTree == null)
                    return false;
                else
                    return _ShowTree;
            }
            set { _ShowTree = value; }
        }
        private Nullable<bool> _ShowImageViewer;
        public Nullable<bool> ShowImageViewer
        {
            get
            {
                if (_ShowImageViewer == null)
                    return false;
                else
                    return _ShowImageViewer;
            }
            set { _ShowImageViewer = value; }
        }
        private Nullable<bool> _ShowTrackingViewer;
        public Nullable<bool> ShowTrackingViewer
        {
            get
            {
                if (_ShowTrackingViewer == null)
                    return false;
                else
                    return _ShowTrackingViewer;
            }
            set { _ShowTrackingViewer = value; }
        }
        private Nullable<bool> _DeleteAfterPrint;
        public Nullable<bool> DeleteAfterPrint
        {
            get
            {
                if (_DeleteAfterPrint == null)
                    return false;
                else
                    return _DeleteAfterPrint;
            }
            set { _DeleteAfterPrint = value; }
        }
        public string HotKeyAssignment { get; set; }
        private Nullable<bool> _CanHotKeyWorkIfNotInFocus;
        public Nullable<bool> CanHotKeyWorkIfNotInFocus
        {
            get
            {
                if (_CanHotKeyWorkIfNotInFocus == null)
                    return false;
                else
                    return _CanHotKeyWorkIfNotInFocus;
            }
            set { _CanHotKeyWorkIfNotInFocus = value; }
        }
        private Nullable<int> _Action;
        public Nullable<int> Action
        {
            get
            {
                if (_Action == null)
                    return 0;
                else
                    return _Action;
            }
            set { _Action = value; }
        }
        private Nullable<bool> _PrintAfter;
        public Nullable<bool> PrintAfter
        {
            get
            {
                if (_PrintAfter == null)
                    return false;
                else
                    return _PrintAfter;
            }
            set { _PrintAfter = value; }
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
        private Nullable<bool> _Activated;
        public Nullable<bool> Activated
        {
            get
            {
                if (_Activated == null)
                    return false;
                else
                    return _Activated;
            }
            set { _Activated = value; }
        }
        public string ParentTitle { get; set; }
    }
}
