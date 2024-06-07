using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLColdSetupRow
    {
        public int Id { get; set; }
        public string RowName { get; set; }
        private Nullable<int> _SeqNo;
        public Nullable<int> SeqNo
        {
            get
            {
                if (_SeqNo == null)
                    return 0;
                else
                    return _SeqNo;
            }
            set { _SeqNo = value; }
        }
        private Nullable<int> _SLCOLDSetupFormsId;
        public Nullable<int> SLCOLDSetupFormsId
        {
            get
            {
                if (_SLCOLDSetupFormsId == null)
                    return 0;
                else
                    return _SLCOLDSetupFormsId;
            }
            set { _SLCOLDSetupFormsId = value; }
        }
        public string RowMask { get; set; }
        private Nullable<bool> _SendBreak;
        public Nullable<bool> SendBreak
        {
            get
            {
                if (_SendBreak == null)
                    return false;
                else
                    return _SendBreak;
            }
            set { _SendBreak = value; }
        }
        public string ScanFormsId { get; set; }
        private Nullable<int> _LineOnPage;
        public Nullable<int> LineOnPage
        {
            get
            {
                if (_LineOnPage == null)
                    return 0;
                else
                    return _LineOnPage;
            }
            set { _LineOnPage = value; }
        }
        private Nullable<int> _NewRowLine;
        public Nullable<int> NewRowLine
        {
            get
            {
                if (_NewRowLine == null)
                    return 0;
                else
                    return _NewRowLine;
            }
            set { _NewRowLine = value; }
        }
        private Nullable<bool> _ErrorOnMissingFromPage;
        public Nullable<bool> ErrorOnMissingFromPage
        {
            get
            {
                if (_ErrorOnMissingFromPage == null)
                    return false;
                else
                    return _ErrorOnMissingFromPage;
            }
            set { _ErrorOnMissingFromPage = value; }
        }
    }
}
