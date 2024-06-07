using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLServiceTaskItem
    {
				public int Id { get; set; } 
				private Nullable<int> _SLServiceTaskId;
				public Nullable<int> SLServiceTaskId 
				{
					get
					{
							if (_SLServiceTaskId == null)
								return 0;
							else
								return _SLServiceTaskId;
					}
					set { _SLServiceTaskId = value; }
				}
				public string TableName { get; set; } 
				public string TableId { get; set; } 
    }
}
