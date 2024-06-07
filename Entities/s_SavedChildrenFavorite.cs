using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class s_SavedChildrenFavorite
    {
				public int Id { get; set; } 
				private Nullable<int> _SavedCriteriaId;
				public Nullable<int> SavedCriteriaId 
				{
					get
					{
							if (_SavedCriteriaId == null)
								return 0;
							else
								return _SavedCriteriaId;
					}
					set { _SavedCriteriaId = value; }
				}
				public string TableId { get; set; } 
    }
}
