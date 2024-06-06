using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public partial class Setting
    {
        public int Id { get; set; }
        public string Section { get; set; }
        public string Item { get; set; }
        public string ItemValue { get; set; }
    }
}
