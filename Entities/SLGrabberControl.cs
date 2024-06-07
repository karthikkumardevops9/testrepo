using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLGrabberControl
    {
        public int Id { get; set; }
        private Nullable<int> _SLGrabberFunctionsId;
        public Nullable<int> SLGrabberFunctionsId
        {
            get
            {
                if (_SLGrabberFunctionsId == null)
                    return 0;
                else
                    return _SLGrabberFunctionsId;
            }
            set { _SLGrabberFunctionsId = value; }
        }
        public string Name { get; set; }
        private Nullable<bool> _UseControlId;
        public Nullable<bool> UseControlId
        {
            get
            {
                if (_UseControlId == null)
                    return false;
                else
                    return _UseControlId;
            }
            set { _UseControlId = value; }
        }
        private Nullable<int> _ControlId;
        public Nullable<int> ControlId
        {
            get
            {
                if (_ControlId == null)
                    return 0;
                else
                    return _ControlId;
            }
            set { _ControlId = value; }
        }
        private Nullable<bool> _UseClassName;
        public Nullable<bool> UseClassName
        {
            get
            {
                if (_UseClassName == null)
                    return false;
                else
                    return _UseClassName;
            }
            set { _UseClassName = value; }
        }
        public string ClassName { get; set; }
        private Nullable<bool> _UseSize;
        public Nullable<bool> UseSize
        {
            get
            {
                if (_UseSize == null)
                    return false;
                else
                    return _UseSize;
            }
            set { _UseSize = value; }
        }
        private Nullable<int> _Width;
        public Nullable<int> Width
        {
            get
            {
                if (_Width == null)
                    return 0;
                else
                    return _Width;
            }
            set { _Width = value; }
        }
        private Nullable<int> _Height;
        public Nullable<int> Height
        {
            get
            {
                if (_Height == null)
                    return 0;
                else
                    return _Height;
            }
            set { _Height = value; }
        }
        private Nullable<bool> _UsePosition;
        public Nullable<bool> UsePosition
        {
            get
            {
                if (_UsePosition == null)
                    return false;
                else
                    return _UsePosition;
            }
            set { _UsePosition = value; }
        }
        private Nullable<int> _Left;
        public Nullable<int> Left
        {
            get
            {
                if (_Left == null)
                    return 0;
                else
                    return _Left;
            }
            set { _Left = value; }
        }
        private Nullable<int> _Top;
        public Nullable<int> Top
        {
            get
            {
                if (_Top == null)
                    return 0;
                else
                    return _Top;
            }
            set { _Top = value; }
        }
    }
}
