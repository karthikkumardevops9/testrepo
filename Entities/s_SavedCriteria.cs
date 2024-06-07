using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class s_SavedCriteria
    {
				public int Id { get; set; } 
				private Nullable<int> _UserId;
				public Nullable<int> UserId 
				{
					get
					{
							if (_UserId == null)
								return 0;
							else
								return _UserId;
					}
					set { _UserId = value; }
				}
				public string SavedName { get; set; } 
				public int SavedType { get; set; } 
				private Nullable<int> _ViewId;
				public Nullable<int> ViewId 
				{
					get
					{
							if (_ViewId == null)
								return 0;
							else
								return _ViewId;
					}
					set { _ViewId = value; }
				}
    }
}
