using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class GridSetting
    {
        public GridSetting()
        {
            this.GridColumns = new List<GridColumn>();
        }

        public int GridSettingsId { get; set; }
        public string GridSettingsName { get; set; }
        public virtual ICollection<GridColumn> GridColumns { get; set; }
    }
}
