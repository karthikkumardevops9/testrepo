using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class TabSet
    {
        public short Id { get; set; }
        public string UserName { get; set; }
        public string TabFontName { get; set; }
        private Nullable<float> _TabFontSize;
        public Nullable<float> TabFontSize
        {
            get
            {
                return _TabFontSize;
            }
            set { _TabFontSize = value; }
        }
        private Nullable<bool> _TabFontBold;
        public Nullable<bool> TabFontBold
        {
            get
            {
                if (_TabFontBold == null)
                    return false;
                else
                    return _TabFontBold;
            }
            set { _TabFontBold = value; }
        }
        private Nullable<short> _TabMaxWidth;
        public Nullable<short> TabMaxWidth
        {
            get
            {
                return _TabMaxWidth;
            }
            set { _TabMaxWidth = value; }
        }
        private Nullable<short> _TabCutSize;
        public Nullable<short> TabCutSize
        {
            get
            {
                return _TabCutSize;
            }
            set { _TabCutSize = value; }
        }
        private Nullable<short> _TabHeight;
        public Nullable<short> TabHeight
        {
            get
            {
                return _TabHeight;
            }
            set { _TabHeight = value; }
        }
        private Nullable<short> _TabRowOffset;
        public Nullable<short> TabRowOffset
        {
            get
            {
                return _TabRowOffset;
            }
            set { _TabRowOffset = value; }
        }
        private Nullable<short> _TabShape;
        public Nullable<short> TabShape
        {
            get
            {
                return _TabShape;
            }
            set { _TabShape = value; }
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
        private Nullable<bool> _StartupTabset;
        public Nullable<bool> StartupTabset
        {
            get
            {
                if (_StartupTabset == null)
                    return false;
                else
                    return _StartupTabset;
            }
            set { _StartupTabset = value; }
        }
        public string Picture { get; set; }
    }
}
