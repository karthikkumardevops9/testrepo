using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Attribute
    {
        public int Id { get; set; }
        private Nullable<int> _HeadBackColor1;
        public Nullable<int> HeadBackColor1
        {
            get
            {
                if (_HeadBackColor1 == null)
                    return 0;
                else
                    return _HeadBackColor1;
            }
            set { _HeadBackColor1 = value; }
        }
        private Nullable<int> _HeadBackColor2;
        public Nullable<int> HeadBackColor2
        {
            get
            {
                if (_HeadBackColor2 == null)
                    return 0;
                else
                    return _HeadBackColor2;
            }
            set { _HeadBackColor2 = value; }
        }
        private Nullable<int> _HeadBackColor3;
        public Nullable<int> HeadBackColor3
        {
            get
            {
                if (_HeadBackColor3 == null)
                    return 0;
                else
                    return _HeadBackColor3;
            }
            set { _HeadBackColor3 = value; }
        }
        private Nullable<int> _HeadForeColor1;
        public Nullable<int> HeadForeColor1
        {
            get
            {
                if (_HeadForeColor1 == null)
                    return 0;
                else
                    return _HeadForeColor1;
            }
            set { _HeadForeColor1 = value; }
        }
        private Nullable<int> _HeadForeColor2;
        public Nullable<int> HeadForeColor2
        {
            get
            {
                if (_HeadForeColor2 == null)
                    return 0;
                else
                    return _HeadForeColor2;
            }
            set { _HeadForeColor2 = value; }
        }
        private Nullable<int> _HeadForeColor3;
        public Nullable<int> HeadForeColor3
        {
            get
            {
                if (_HeadForeColor3 == null)
                    return 0;
                else
                    return _HeadForeColor3;
            }
            set { _HeadForeColor3 = value; }
        }
        private Nullable<int> _InactiveBackColor1;
        public Nullable<int> InactiveBackColor1
        {
            get
            {
                if (_InactiveBackColor1 == null)
                    return 0;
                else
                    return _InactiveBackColor1;
            }
            set { _InactiveBackColor1 = value; }
        }
        private Nullable<int> _InactiveBackColor2;
        public Nullable<int> InactiveBackColor2
        {
            get
            {
                if (_InactiveBackColor2 == null)
                    return 0;
                else
                    return _InactiveBackColor2;
            }
            set { _InactiveBackColor2 = value; }
        }
        private Nullable<int> _InactiveBackColor3;
        public Nullable<int> InactiveBackColor3
        {
            get
            {
                if (_InactiveBackColor3 == null)
                    return 0;
                else
                    return _InactiveBackColor3;
            }
            set { _InactiveBackColor3 = value; }
        }
        private Nullable<int> _InactiveForeColor1;
        public Nullable<int> InactiveForeColor1
        {
            get
            {
                if (_InactiveForeColor1 == null)
                    return 0;
                else
                    return _InactiveForeColor1;
            }
            set { _InactiveForeColor1 = value; }
        }
        private Nullable<int> _InactiveForeColor2;
        public Nullable<int> InactiveForeColor2
        {
            get
            {
                if (_InactiveForeColor2 == null)
                    return 0;
                else
                    return _InactiveForeColor2;
            }
            set { _InactiveForeColor2 = value; }
        }
        private Nullable<int> _InactiveForeColor3;
        public Nullable<int> InactiveForeColor3
        {
            get
            {
                if (_InactiveForeColor3 == null)
                    return 0;
                else
                    return _InactiveForeColor3;
            }
            set { _InactiveForeColor3 = value; }
        }
        private Nullable<int> _GridBackColor1;
        public Nullable<int> GridBackColor1
        {
            get
            {
                if (_GridBackColor1 == null)
                    return 0;
                else
                    return _GridBackColor1;
            }
            set { _GridBackColor1 = value; }
        }
        private Nullable<int> _GridBackColor2;
        public Nullable<int> GridBackColor2
        {
            get
            {
                if (_GridBackColor2 == null)
                    return 0;
                else
                    return _GridBackColor2;
            }
            set { _GridBackColor2 = value; }
        }
        private Nullable<int> _GridBackColor3;
        public Nullable<int> GridBackColor3
        {
            get
            {
                if (_GridBackColor3 == null)
                    return 0;
                else
                    return _GridBackColor3;
            }
            set { _GridBackColor3 = value; }
        }
        private Nullable<int> _GridForeColor1;
        public Nullable<int> GridForeColor1
        {
            get
            {
                if (_GridForeColor1 == null)
                    return 0;
                else
                    return _GridForeColor1;
            }
            set { _GridForeColor1 = value; }
        }
        private Nullable<int> _GridForeColor2;
        public Nullable<int> GridForeColor2
        {
            get
            {
                if (_GridForeColor2 == null)
                    return 0;
                else
                    return _GridForeColor2;
            }
            set { _GridForeColor2 = value; }
        }
        private Nullable<int> _GridForeColor3;
        public Nullable<int> GridForeColor3
        {
            get
            {
                if (_GridForeColor3 == null)
                    return 0;
                else
                    return _GridForeColor3;
            }
            set { _GridForeColor3 = value; }
        }
        private Nullable<int> _TabBackColor1;
        public Nullable<int> TabBackColor1
        {
            get
            {
                if (_TabBackColor1 == null)
                    return 0;
                else
                    return _TabBackColor1;
            }
            set { _TabBackColor1 = value; }
        }
        private Nullable<int> _TabBackColor2;
        public Nullable<int> TabBackColor2
        {
            get
            {
                if (_TabBackColor2 == null)
                    return 0;
                else
                    return _TabBackColor2;
            }
            set { _TabBackColor2 = value; }
        }
        private Nullable<int> _TabBackColor3;
        public Nullable<int> TabBackColor3
        {
            get
            {
                if (_TabBackColor3 == null)
                    return 0;
                else
                    return _TabBackColor3;
            }
            set { _TabBackColor3 = value; }
        }
        private Nullable<int> _TabForeColor1;
        public Nullable<int> TabForeColor1
        {
            get
            {
                if (_TabForeColor1 == null)
                    return 0;
                else
                    return _TabForeColor1;
            }
            set { _TabForeColor1 = value; }
        }
        private Nullable<int> _TabForeColor2;
        public Nullable<int> TabForeColor2
        {
            get
            {
                if (_TabForeColor2 == null)
                    return 0;
                else
                    return _TabForeColor2;
            }
            set { _TabForeColor2 = value; }
        }
        private Nullable<int> _TabForeColor3;
        public Nullable<int> TabForeColor3
        {
            get
            {
                if (_TabForeColor3 == null)
                    return 0;
                else
                    return _TabForeColor3;
            }
            set { _TabForeColor3 = value; }
        }
        private Nullable<int> _SortFieldBackColor1;
        public Nullable<int> SortFieldBackColor1
        {
            get
            {
                if (_SortFieldBackColor1 == null)
                    return 0;
                else
                    return _SortFieldBackColor1;
            }
            set { _SortFieldBackColor1 = value; }
        }
        private Nullable<int> _SortFieldBackColor2;
        public Nullable<int> SortFieldBackColor2
        {
            get
            {
                if (_SortFieldBackColor2 == null)
                    return 0;
                else
                    return _SortFieldBackColor2;
            }
            set { _SortFieldBackColor2 = value; }
        }
        private Nullable<int> _SortFieldBackColor3;
        public Nullable<int> SortFieldBackColor3
        {
            get
            {
                if (_SortFieldBackColor3 == null)
                    return 0;
                else
                    return _SortFieldBackColor3;
            }
            set { _SortFieldBackColor3 = value; }
        }
        private Nullable<int> _SortFieldForeColor1;
        public Nullable<int> SortFieldForeColor1
        {
            get
            {
                if (_SortFieldForeColor1 == null)
                    return 0;
                else
                    return _SortFieldForeColor1;
            }
            set { _SortFieldForeColor1 = value; }
        }
        private Nullable<int> _SortFieldForeColor2;
        public Nullable<int> SortFieldForeColor2
        {
            get
            {
                if (_SortFieldForeColor2 == null)
                    return 0;
                else
                    return _SortFieldForeColor2;
            }
            set { _SortFieldForeColor2 = value; }
        }
        private Nullable<int> _SortFieldForeColor3;
        public Nullable<int> SortFieldForeColor3
        {
            get
            {
                if (_SortFieldForeColor3 == null)
                    return 0;
                else
                    return _SortFieldForeColor3;
            }
            set { _SortFieldForeColor3 = value; }
        }
        public string ColorName { get; set; }
    }
}
