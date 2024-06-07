using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class GridColumn
    {
        public int GridColumnId { get; set; }
        public int GridSettingsId { get; set; }
        public int GridColumnSrNo { get; set; }
        public string GridColumnName { get; set; }
        public string GridColumnDisplayName { get; set; }
        public bool IsActive { get; set; }
        public bool IsSortable { get; set; }
        public bool IsCheckbox { get; set; }
        public virtual GridSetting GridSetting { get; set; }
    }
}
