using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ViewFilter
    {
        public int Id { get; set; }
        private Nullable<int> _Sequence;
        public Nullable<int> Sequence
        {
            get
            {
                if (_Sequence == null)
                    return 0;
                else
                    return _Sequence;
            }
            set { _Sequence = value; }
        }
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
        private Nullable<int> _ColumnNum;
        public Nullable<int> ColumnNum
        {
            get
            {
                if (_ColumnNum == null)
                    return 0;
                else
                    return _ColumnNum;
            }
            set { _ColumnNum = value; }
        }
        public string OpenParen { get; set; }
        public string Operator { get; set; }
        public string FilterData { get; set; }
        public string CloseParen { get; set; }
        public string JoinOperator { get; set; }
        private Nullable<bool> _Active;
        public Nullable<bool> Active
        {
            get
            {
                if (_Active == null)
                    return false;
                else
                    return _Active;
            }
            set { _Active = value; }
        }
        private Nullable<int> _DisplayColumnNum;
        public Nullable<int> DisplayColumnNum
        {
            get
            {
                if (_DisplayColumnNum == null)
                    return 0;
                else
                    return _DisplayColumnNum;
            }
            set { _DisplayColumnNum = value; }
        }
        private Nullable<bool> _PartOfView;
        public Nullable<bool> PartOfView
        {
            get
            {
                if (_PartOfView == null)
                    return false;
                else
                    return _PartOfView;
            }
            set { _PartOfView = value; }
        }
    }
}
