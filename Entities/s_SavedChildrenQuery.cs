using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class s_SavedChildrenQuery
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
				public string ColumnName { get; set; } 
				public string Operator { get; set; } 
				public string CriteriaValue { get; set; } 
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
    }
}
