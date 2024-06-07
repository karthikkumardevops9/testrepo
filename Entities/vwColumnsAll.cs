using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class vwColumnsAll
    {
        public long ID { get; set; }
        public string COLUMN_NAME { get; set; }
        public string TABLE_NAME { get; set; }
        public string DATA_TYPE { get; set; }
    }
}
