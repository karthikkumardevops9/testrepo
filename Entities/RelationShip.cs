using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class RelationShip
    {
        public int Id { get; set; }
        public string UpperTableName { get; set; }
        public string UpperTableFieldName { get; set; }
        public string LowerTableName { get; set; }
        public string LowerTableFieldName { get; set; }
        private Nullable<short> _TabOrder;
        public Nullable<short> TabOrder
        {
            get
            {
                return _TabOrder;
            }
            set { _TabOrder = value; }
        }
        private Nullable<short> _IdTypes;
        public Nullable<short> IdTypes
        {
            get
            {
                return _IdTypes;
            }
            set { _IdTypes = value; }
        }
        private Nullable<int> _DrillDownViewGroup;
        public Nullable<int> DrillDownViewGroup
        {
            get
            {
                if (_DrillDownViewGroup == null)
                    return 0;
                else
                    return _DrillDownViewGroup;
            }
            set { _DrillDownViewGroup = value; }
        }
    }
}
