using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class LookupType
    {
        public int LookupTypeId { get; set; }
        public string LookupTypeCode { get; set; }
        public string LookupTypeValue { get; set; }
        public string LookupTypeDesc { get; set; }
        public string LookupTypeForCode { get; set; }
        private Nullable<int> _SortOrder;
        public Nullable<int> SortOrder
        {
            get
            {
                if (_SortOrder == null)
                    return 0;
                else
                    return _SortOrder;
            }
            set { _SortOrder = value; }
        }
        private Nullable<int> _LookupTypeParentCode;
        public Nullable<int> LookupTypeParentCode
        {
            get
            {
                if (_LookupTypeParentCode == null)
                    return 0;
                else
                    return _LookupTypeParentCode;
            }
            set { _LookupTypeParentCode = value; }
        }
        private Nullable<bool> _IsActive;
        public Nullable<bool> IsActive
        {
            get
            {
                if (_IsActive == null)
                    return false;
                else
                    return _IsActive;
            }
            set { _IsActive = value; }
        }
    }
}
