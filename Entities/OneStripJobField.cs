using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class OneStripJobField
    {
        public int Id { get; set; }
        private Nullable<short> _SetNum;
        public Nullable<short> SetNum
        {
            get
            {
                return _SetNum;
            }
            set { _SetNum = value; }
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
        public string FieldName { get; set; }
        public string Format { get; set; }
        public string Type { get; set; }
        private Nullable<double> _XPos;
        public Nullable<double> XPos
        {
            get
            {
                return _XPos;
            }
            set { _XPos = value; }
        }
        private Nullable<double> _YPos;
        public Nullable<double> YPos
        {
            get
            {
                return _YPos;
            }
            set { _YPos = value; }
        }
        private Nullable<double> _FontSize;
        public Nullable<double> FontSize
        {
            get
            {
                return _FontSize;
            }
            set { _FontSize = value; }
        }
        public string FontName { get; set; }
        private Nullable<bool> _FontBold;
        public Nullable<bool> FontBold
        {
            get
            {
                if (_FontBold == null)
                    return false;
                else
                    return _FontBold;
            }
            set { _FontBold = value; }
        }
        private Nullable<bool> _FontItalic;
        public Nullable<bool> FontItalic
        {
            get
            {
                if (_FontItalic == null)
                    return false;
                else
                    return _FontItalic;
            }
            set { _FontItalic = value; }
        }
        private Nullable<bool> _FontUnderline;
        public Nullable<bool> FontUnderline
        {
            get
            {
                if (_FontUnderline == null)
                    return false;
                else
                    return _FontUnderline;
            }
            set { _FontUnderline = value; }
        }
        private Nullable<bool> _FontStrikeThru;
        public Nullable<bool> FontStrikeThru
        {
            get
            {
                if (_FontStrikeThru == null)
                    return false;
                else
                    return _FontStrikeThru;
            }
            set { _FontStrikeThru = value; }
        }
        private Nullable<bool> _FontTransparent;
        public Nullable<bool> FontTransparent
        {
            get
            {
                if (_FontTransparent == null)
                    return false;
                else
                    return _FontTransparent;
            }
            set { _FontTransparent = value; }
        }
        private Nullable<short> _FontOrientation;
        public Nullable<short> FontOrientation
        {
            get
            {
                return _FontOrientation;
            }
            set { _FontOrientation = value; }
        }
        private Nullable<short> _Alignment;
        public Nullable<short> Alignment
        {
            get
            {
                return _Alignment;
            }
            set { _Alignment = value; }
        }
        private Nullable<int> _ForeColor;
        public Nullable<int> ForeColor
        {
            get
            {
                if (_ForeColor == null)
                    return 0;
                else
                    return _ForeColor;
            }
            set { _ForeColor = value; }
        }
        private Nullable<int> _BackColor;
        public Nullable<int> BackColor
        {
            get
            {
                if (_BackColor == null)
                    return 0;
                else
                    return _BackColor;
            }
            set { _BackColor = value; }
        }
        private Nullable<int> _BCStyle;
        public Nullable<int> BCStyle
        {
            get
            {
                if (_BCStyle == null)
                    return 0;
                else
                    return _BCStyle;
            }
            set { _BCStyle = value; }
        }
        private Nullable<double> _BCBarWidth;
        public Nullable<double> BCBarWidth
        {
            get
            {
                return _BCBarWidth;
            }
            set { _BCBarWidth = value; }
        }
        private Nullable<short> _BCDirection;
        public Nullable<short> BCDirection
        {
            get
            {
                return _BCDirection;
            }
            set { _BCDirection = value; }
        }
        private Nullable<short> _BCUPCNotches;
        public Nullable<short> BCUPCNotches
        {
            get
            {
                return _BCUPCNotches;
            }
            set { _BCUPCNotches = value; }
        }
        private Nullable<double> _BCWidth;
        public Nullable<double> BCWidth
        {
            get
            {
                return _BCWidth;
            }
            set { _BCWidth = value; }
        }
        private Nullable<double> _BCHeight;
        public Nullable<double> BCHeight
        {
            get
            {
                return _BCHeight;
            }
            set { _BCHeight = value; }
        }
        private Nullable<short> _Order;
        public Nullable<short> Order
        {
            get
            {
                return _Order;
            }
            set { _Order = value; }
        }
        private Nullable<int> _StartChar;
        public Nullable<int> StartChar
        {
            get
            {
                if (_StartChar == null)
                    return 0;
                else
                    return _StartChar;
            }
            set { _StartChar = value; }
        }
        private Nullable<int> _MaxLen;
        public Nullable<int> MaxLen
        {
            get
            {
                if (_MaxLen == null)
                    return 0;
                else
                    return _MaxLen;
            }
            set { _MaxLen = value; }
        }
        private Nullable<int> _SpecialFunctions;
        public Nullable<int> SpecialFunctions
        {
            get
            {
                if (_SpecialFunctions == null)
                    return 0;
                else
                    return _SpecialFunctions;
            }
            set { _SpecialFunctions = value; }
        }
    }
}
