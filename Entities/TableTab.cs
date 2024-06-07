using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class TableTab
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        private Nullable<int> _BaseView;
        public Nullable<int> BaseView
        {
            get
            {
                if (_BaseView == null)
                    return 0;
                else
                    return _BaseView;
            }
            set { _BaseView = value; }
        }
        private Nullable<short> _TabOrder;
        public Nullable<short> TabOrder
        {
            get
            {
                return _TabOrder;
            }
            set { _TabOrder = value; }
        }
        private Nullable<int> _TopTabViewGroup;
        public Nullable<int> TopTabViewGroup
        {
            get
            {
                if (_TopTabViewGroup == null)
                    return 0;
                else
                    return _TopTabViewGroup;
            }
            set { _TopTabViewGroup = value; }
        }
        private Nullable<bool> _TopTab;
        public Nullable<bool> TopTab
        {
            get
            {
                if (_TopTab == null)
                    return false;
                else
                    return _TopTab;
            }
            set { _TopTab = value; }
        }
        private Nullable<short> _TabSet;
        public Nullable<short> TabSet
        {
            get
            {
                return _TabSet;
            }
            set { _TabSet = value; }
        }
    }
}
