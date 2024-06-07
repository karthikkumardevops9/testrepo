using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class OneStripForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private Nullable<short> _Inprint;
        public Nullable<short> Inprint
        {
            get
            {
                return _Inprint;
            }
            set { _Inprint = value; }
        }
        private Nullable<double> _LabelOffsetX;
        public Nullable<double> LabelOffsetX
        {
            get
            {
                return _LabelOffsetX;
            }
            set { _LabelOffsetX = value; }
        }
        private Nullable<double> _LabelOffsetY;
        public Nullable<double> LabelOffsetY
        {
            get
            {
                return _LabelOffsetY;
            }
            set { _LabelOffsetY = value; }
        }
        private Nullable<double> _LabelWidth;
        public Nullable<double> LabelWidth
        {
            get
            {
                return _LabelWidth;
            }
            set { _LabelWidth = value; }
        }
        private Nullable<double> _LabelHeight;
        public Nullable<double> LabelHeight
        {
            get
            {
                return _LabelHeight;
            }
            set { _LabelHeight = value; }
        }
        private Nullable<double> _LabelWidthEdgeToEdge;
        public Nullable<double> LabelWidthEdgeToEdge
        {
            get
            {
                return _LabelWidthEdgeToEdge;
            }
            set { _LabelWidthEdgeToEdge = value; }
        }
        private Nullable<double> _LabelHeightEdgeToEdge;
        public Nullable<double> LabelHeightEdgeToEdge
        {
            get
            {
                return _LabelHeightEdgeToEdge;
            }
            set { _LabelHeightEdgeToEdge = value; }
        }
        private Nullable<short> _LabelsAcross;
        public Nullable<short> LabelsAcross
        {
            get
            {
                return _LabelsAcross;
            }
            set { _LabelsAcross = value; }
        }
        private Nullable<short> _LabelsDown;
        public Nullable<short> LabelsDown
        {
            get
            {
                return _LabelsDown;
            }
            set { _LabelsDown = value; }
        }
        private Nullable<double> _PageWidth;
        public Nullable<double> PageWidth
        {
            get
            {
                return _PageWidth;
            }
            set { _PageWidth = value; }
        }
        private Nullable<double> _PageHeight;
        public Nullable<double> PageHeight
        {
            get
            {
                return _PageHeight;
            }
            set { _PageHeight = value; }
        }
        public string WindowsPrinterSetting { get; set; }
        public string ColorPalette { get; set; }
        public string MultiDefinitionFile { get; set; }
        private Nullable<double> _LabelOffsetColumn;
        public Nullable<double> LabelOffsetColumn
        {
            get
            {
                return _LabelOffsetColumn;
            }
            set { _LabelOffsetColumn = value; }
        }
        private Nullable<double> _LabelOffsetRow;
        public Nullable<double> LabelOffsetRow
        {
            get
            {
                return _LabelOffsetRow;
            }
            set { _LabelOffsetRow = value; }
        }
        private Nullable<bool> _PrintLabelsTopToBottom;
        public Nullable<bool> PrintLabelsTopToBottom
        {
            get
            {
                if (_PrintLabelsTopToBottom == null)
                    return false;
                else
                    return _PrintLabelsTopToBottom;
            }
            set { _PrintLabelsTopToBottom = value; }
        }
        private Nullable<byte> _PrintLabelsLandscape;
        public Nullable<byte> PrintLabelsLandscape
        {
            get
            {
                return _PrintLabelsLandscape;
            }
            set { _PrintLabelsLandscape = value; }
        }
        private Nullable<bool> _Label2Printable;
        public Nullable<bool> Label2Printable
        {
            get
            {
                if (_Label2Printable == null)
                    return false;
                else
                    return _Label2Printable;
            }
            set { _Label2Printable = value; }
        }
        private Nullable<double> _LabelOffsetX2;
        public Nullable<double> LabelOffsetX2
        {
            get
            {
                return _LabelOffsetX2;
            }
            set { _LabelOffsetX2 = value; }
        }
        private Nullable<double> _LabelOffsetY2;
        public Nullable<double> LabelOffsetY2
        {
            get
            {
                return _LabelOffsetY2;
            }
            set { _LabelOffsetY2 = value; }
        }
        private Nullable<double> _LabelWidth2;
        public Nullable<double> LabelWidth2
        {
            get
            {
                return _LabelWidth2;
            }
            set { _LabelWidth2 = value; }
        }
        private Nullable<double> _LabelHeight2;
        public Nullable<double> LabelHeight2
        {
            get
            {
                return _LabelHeight2;
            }
            set { _LabelHeight2 = value; }
        }
        private Nullable<double> _LabelWidthEdgeToEdge2;
        public Nullable<double> LabelWidthEdgeToEdge2
        {
            get
            {
                return _LabelWidthEdgeToEdge2;
            }
            set { _LabelWidthEdgeToEdge2 = value; }
        }
        private Nullable<double> _LabelHeightEdgeToEdge2;
        public Nullable<double> LabelHeightEdgeToEdge2
        {
            get
            {
                return _LabelHeightEdgeToEdge2;
            }
            set { _LabelHeightEdgeToEdge2 = value; }
        }
        private Nullable<int> _LabelsAcross2;
        public Nullable<int> LabelsAcross2
        {
            get
            {
                if (_LabelsAcross2 == null)
                    return 0;
                else
                    return _LabelsAcross2;
            }
            set { _LabelsAcross2 = value; }
        }
        private Nullable<int> _LabelsDown2;
        public Nullable<int> LabelsDown2
        {
            get
            {
                if (_LabelsDown2 == null)
                    return 0;
                else
                    return _LabelsDown2;
            }
            set { _LabelsDown2 = value; }
        }
        public string TopImage { get; set; }
        public string BottomImage { get; set; }
        private Nullable<bool> _UseMultiDefinitionFile;
        public Nullable<bool> UseMultiDefinitionFile
        {
            get
            {
                if (_UseMultiDefinitionFile == null)
                    return false;
                else
                    return _UseMultiDefinitionFile;
            }
            set { _UseMultiDefinitionFile = value; }
        }
    }
}
