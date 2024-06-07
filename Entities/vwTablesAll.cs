using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class vwTablesAll
    {
        public string TABLE_CATALOG { get; set; }
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
        public string TABLE_TYPE { get; set; }
        public string UserName { get; set; }
        public string Expr1 { get; set; }
        private Nullable<long> _RowRank;
        public Nullable<long> RowRank
        {
            get
            {
                return _RowRank;
            }
            set { _RowRank = value; }
        }
    }
}
