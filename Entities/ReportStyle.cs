using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ReportStyle
    {
        public int ReportStylesId { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        private Nullable<bool> _AltRowShading;
        public Nullable<bool> AltRowShading
        {
            get
            {
                if (_AltRowShading == null)
                    return false;
                else
                    return _AltRowShading;
            }
            set { _AltRowShading = value; }
        }
        private Nullable<bool> _FixedLines;
        public Nullable<bool> FixedLines
        {
            get
            {
                if (_FixedLines == null)
                    return false;
                else
                    return _FixedLines;
            }
            set { _FixedLines = value; }
        }
        private Nullable<int> _BlankLineSpacing;
        public Nullable<int> BlankLineSpacing
        {
            get
            {
                if (_BlankLineSpacing == null)
                    return 0;
                else
                    return _BlankLineSpacing;
            }
            set { _BlankLineSpacing = value; }
        }
        private Nullable<int> _BoxWidth;
        public Nullable<int> BoxWidth
        {
            get
            {
                if (_BoxWidth == null)
                    return 0;
                else
                    return _BoxWidth;
            }
            set { _BoxWidth = value; }
        }
        private Nullable<int> _ColumnSpacing;
        public Nullable<int> ColumnSpacing
        {
            get
            {
                if (_ColumnSpacing == null)
                    return 0;
                else
                    return _ColumnSpacing;
            }
            set { _ColumnSpacing = value; }
        }
        private Nullable<int> _HeaderSize;
        public Nullable<int> HeaderSize
        {
            get
            {
                if (_HeaderSize == null)
                    return 0;
                else
                    return _HeaderSize;
            }
            set { _HeaderSize = value; }
        }
        private Nullable<int> _MaxLines;
        public Nullable<int> MaxLines
        {
            get
            {
                if (_MaxLines == null)
                    return 0;
                else
                    return _MaxLines;
            }
            set { _MaxLines = value; }
        }
        private Nullable<int> _MinColumnWidth;
        public Nullable<int> MinColumnWidth
        {
            get
            {
                if (_MinColumnWidth == null)
                    return 0;
                else
                    return _MinColumnWidth;
            }
            set { _MinColumnWidth = value; }
        }
        private Nullable<int> _Orientation;
        public Nullable<int> Orientation
        {
            get
            {
                if (_Orientation == null)
                    return 0;
                else
                    return _Orientation;
            }
            set { _Orientation = value; }
        }
        private Nullable<int> _ShadowSize;
        public Nullable<int> ShadowSize
        {
            get
            {
                if (_ShadowSize == null)
                    return 0;
                else
                    return _ShadowSize;
            }
            set { _ShadowSize = value; }
        }
        private Nullable<int> _LineColor;
        public Nullable<int> LineColor
        {
            get
            {
                if (_LineColor == null)
                    return 0;
                else
                    return _LineColor;
            }
            set { _LineColor = value; }
        }
        private Nullable<int> _ShadeBoxColor;
        public Nullable<int> ShadeBoxColor
        {
            get
            {
                if (_ShadeBoxColor == null)
                    return 0;
                else
                    return _ShadeBoxColor;
            }
            set { _ShadeBoxColor = value; }
        }
        private Nullable<int> _ShadedLineColor;
        public Nullable<int> ShadedLineColor
        {
            get
            {
                if (_ShadedLineColor == null)
                    return 0;
                else
                    return _ShadedLineColor;
            }
            set { _ShadedLineColor = value; }
        }
        private Nullable<int> _ShadowColor;
        public Nullable<int> ShadowColor
        {
            get
            {
                if (_ShadowColor == null)
                    return 0;
                else
                    return _ShadowColor;
            }
            set { _ShadowColor = value; }
        }
        private Nullable<int> _TextBackColor;
        public Nullable<int> TextBackColor
        {
            get
            {
                if (_TextBackColor == null)
                    return 0;
                else
                    return _TextBackColor;
            }
            set { _TextBackColor = value; }
        }
        private Nullable<int> _TextForeColor;
        public Nullable<int> TextForeColor
        {
            get
            {
                if (_TextForeColor == null)
                    return 0;
                else
                    return _TextForeColor;
            }
            set { _TextForeColor = value; }
        }
        private Nullable<int> _TopMargin;
        public Nullable<int> TopMargin
        {
            get
            {
                if (_TopMargin == null)
                    return 0;
                else
                    return _TopMargin;
            }
            set { _TopMargin = value; }
        }
        private Nullable<int> _BottomMargin;
        public Nullable<int> BottomMargin
        {
            get
            {
                if (_BottomMargin == null)
                    return 0;
                else
                    return _BottomMargin;
            }
            set { _BottomMargin = value; }
        }
        private Nullable<int> _LeftMargin;
        public Nullable<int> LeftMargin
        {
            get
            {
                if (_LeftMargin == null)
                    return 0;
                else
                    return _LeftMargin;
            }
            set { _LeftMargin = value; }
        }
        private Nullable<int> _RightMargin;
        public Nullable<int> RightMargin
        {
            get
            {
                if (_RightMargin == null)
                    return 0;
                else
                    return _RightMargin;
            }
            set { _RightMargin = value; }
        }
        public string Heading1 { get; set; }
        public string Heading2 { get; set; }
        public string ColumnFontName { get; set; }
        private Nullable<int> _ColumnFontSize;
        public Nullable<int> ColumnFontSize
        {
            get
            {
                if (_ColumnFontSize == null)
                    return 0;
                else
                    return _ColumnFontSize;
            }
            set { _ColumnFontSize = value; }
        }
        private Nullable<bool> _ColumnFontBold;
        public Nullable<bool> ColumnFontBold
        {
            get
            {
                if (_ColumnFontBold == null)
                    return false;
                else
                    return _ColumnFontBold;
            }
            set { _ColumnFontBold = value; }
        }
        private Nullable<bool> _ColumnFontItalic;
        public Nullable<bool> ColumnFontItalic
        {
            get
            {
                if (_ColumnFontItalic == null)
                    return false;
                else
                    return _ColumnFontItalic;
            }
            set { _ColumnFontItalic = value; }
        }
        private Nullable<bool> _ColumnFontUnderlined;
        public Nullable<bool> ColumnFontUnderlined
        {
            get
            {
                if (_ColumnFontUnderlined == null)
                    return false;
                else
                    return _ColumnFontUnderlined;
            }
            set { _ColumnFontUnderlined = value; }
        }
        public string ColumnHeadingFontName { get; set; }
        private Nullable<int> _ColumnHeadingFontSize;
        public Nullable<int> ColumnHeadingFontSize
        {
            get
            {
                if (_ColumnHeadingFontSize == null)
                    return 0;
                else
                    return _ColumnHeadingFontSize;
            }
            set { _ColumnHeadingFontSize = value; }
        }
        private Nullable<bool> _ColumnHeadingFontBold;
        public Nullable<bool> ColumnHeadingFontBold
        {
            get
            {
                if (_ColumnHeadingFontBold == null)
                    return false;
                else
                    return _ColumnHeadingFontBold;
            }
            set { _ColumnHeadingFontBold = value; }
        }
        private Nullable<bool> _ColumnHeadingFontItalic;
        public Nullable<bool> ColumnHeadingFontItalic
        {
            get
            {
                if (_ColumnHeadingFontItalic == null)
                    return false;
                else
                    return _ColumnHeadingFontItalic;
            }
            set { _ColumnHeadingFontItalic = value; }
        }
        private Nullable<bool> _ColumnHeadingFontUnderlined;
        public Nullable<bool> ColumnHeadingFontUnderlined
        {
            get
            {
                if (_ColumnHeadingFontUnderlined == null)
                    return false;
                else
                    return _ColumnHeadingFontUnderlined;
            }
            set { _ColumnHeadingFontUnderlined = value; }
        }
        public string FooterFontName { get; set; }
        private Nullable<int> _FooterFontSize;
        public Nullable<int> FooterFontSize
        {
            get
            {
                if (_FooterFontSize == null)
                    return 0;
                else
                    return _FooterFontSize;
            }
            set { _FooterFontSize = value; }
        }
        private Nullable<bool> _FooterFontBold;
        public Nullable<bool> FooterFontBold
        {
            get
            {
                if (_FooterFontBold == null)
                    return false;
                else
                    return _FooterFontBold;
            }
            set { _FooterFontBold = value; }
        }
        private Nullable<bool> _FooterFontItalic;
        public Nullable<bool> FooterFontItalic
        {
            get
            {
                if (_FooterFontItalic == null)
                    return false;
                else
                    return _FooterFontItalic;
            }
            set { _FooterFontItalic = value; }
        }
        private Nullable<bool> _FooterFontUnderlined;
        public Nullable<bool> FooterFontUnderlined
        {
            get
            {
                if (_FooterFontUnderlined == null)
                    return false;
                else
                    return _FooterFontUnderlined;
            }
            set { _FooterFontUnderlined = value; }
        }
        public string HeadingL1FontName { get; set; }
        private Nullable<int> _HeadingL1FontSize;
        public Nullable<int> HeadingL1FontSize
        {
            get
            {
                if (_HeadingL1FontSize == null)
                    return 0;
                else
                    return _HeadingL1FontSize;
            }
            set { _HeadingL1FontSize = value; }
        }
        private Nullable<bool> _HeadingL1FontBold;
        public Nullable<bool> HeadingL1FontBold
        {
            get
            {
                if (_HeadingL1FontBold == null)
                    return false;
                else
                    return _HeadingL1FontBold;
            }
            set { _HeadingL1FontBold = value; }
        }
        private Nullable<bool> _HeadingL1FontItalic;
        public Nullable<bool> HeadingL1FontItalic
        {
            get
            {
                if (_HeadingL1FontItalic == null)
                    return false;
                else
                    return _HeadingL1FontItalic;
            }
            set { _HeadingL1FontItalic = value; }
        }
        private Nullable<bool> _HeadingL1FontUnderlined;
        public Nullable<bool> HeadingL1FontUnderlined
        {
            get
            {
                if (_HeadingL1FontUnderlined == null)
                    return false;
                else
                    return _HeadingL1FontUnderlined;
            }
            set { _HeadingL1FontUnderlined = value; }
        }
        public string HeadingL2FontName { get; set; }
        private Nullable<int> _HeadingL2FontSize;
        public Nullable<int> HeadingL2FontSize
        {
            get
            {
                if (_HeadingL2FontSize == null)
                    return 0;
                else
                    return _HeadingL2FontSize;
            }
            set { _HeadingL2FontSize = value; }
        }
        private Nullable<bool> _HeadingL2FontBold;
        public Nullable<bool> HeadingL2FontBold
        {
            get
            {
                if (_HeadingL2FontBold == null)
                    return false;
                else
                    return _HeadingL2FontBold;
            }
            set { _HeadingL2FontBold = value; }
        }
        private Nullable<bool> _HeadingL2FontItalic;
        public Nullable<bool> HeadingL2FontItalic
        {
            get
            {
                if (_HeadingL2FontItalic == null)
                    return false;
                else
                    return _HeadingL2FontItalic;
            }
            set { _HeadingL2FontItalic = value; }
        }
        private Nullable<bool> _HeadingL2FontUnderlined;
        public Nullable<bool> HeadingL2FontUnderlined
        {
            get
            {
                if (_HeadingL2FontUnderlined == null)
                    return false;
                else
                    return _HeadingL2FontUnderlined;
            }
            set { _HeadingL2FontUnderlined = value; }
        }
        public string SubHeadingFontName { get; set; }
        private Nullable<int> _SubHeadingFontSize;
        public Nullable<int> SubHeadingFontSize
        {
            get
            {
                if (_SubHeadingFontSize == null)
                    return 0;
                else
                    return _SubHeadingFontSize;
            }
            set { _SubHeadingFontSize = value; }
        }
        private Nullable<bool> _SubHeadingFontBold;
        public Nullable<bool> SubHeadingFontBold
        {
            get
            {
                if (_SubHeadingFontBold == null)
                    return false;
                else
                    return _SubHeadingFontBold;
            }
            set { _SubHeadingFontBold = value; }
        }
        private Nullable<bool> _SubHeadingFontItalic;
        public Nullable<bool> SubHeadingFontItalic
        {
            get
            {
                if (_SubHeadingFontItalic == null)
                    return false;
                else
                    return _SubHeadingFontItalic;
            }
            set { _SubHeadingFontItalic = value; }
        }
        private Nullable<bool> _SubHeadingFontUnderlined;
        public Nullable<bool> SubHeadingFontUnderlined
        {
            get
            {
                if (_SubHeadingFontUnderlined == null)
                    return false;
                else
                    return _SubHeadingFontUnderlined;
            }
            set { _SubHeadingFontUnderlined = value; }
        }
        public string Heading1Left { get; set; }
        public string Heading1Center { get; set; }
        public string Heading1Right { get; set; }
        public string Heading2Center { get; set; }
        public string FooterLeft { get; set; }
        public string FooterCenter { get; set; }
        public string FooterRight { get; set; }
        private Nullable<bool> _ReportCentered;
        public Nullable<bool> ReportCentered
        {
            get
            {
                if (_ReportCentered == null)
                    return false;
                else
                    return _ReportCentered;
            }
            set { _ReportCentered = value; }
        }
    }
}
