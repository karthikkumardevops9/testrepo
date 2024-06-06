using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public partial class SLTextSearchItem
    {
        public int Id { get; set; }
        public int IndexType { get; set; }
        public string IndexTableName { get; set; }
        public string IndexFieldName { get; set; }
        public string IndexTableId { get; set; }
    }
}
