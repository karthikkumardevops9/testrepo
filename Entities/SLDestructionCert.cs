using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLDestructionCert
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        private Nullable<DateTime> _DateCreated;
        public Nullable<DateTime> DateCreated
        {
            get
            {
                return _DateCreated;
            }
            set { _DateCreated = value; }
        }
        public string ApprovedBy { get; set; }
        private Nullable<DateTime> _DateDestroyed;
        public Nullable<DateTime> DateDestroyed
        {
            get
            {
                return _DateDestroyed;
            }
            set { _DateDestroyed = value; }
        }
        private Nullable<bool> _ImagesDeleted;
        public Nullable<bool> ImagesDeleted
        {
            get
            {
                if (_ImagesDeleted == null)
                    return false;
                else
                    return _ImagesDeleted;
            }
            set { _ImagesDeleted = value; }
        }
        public string NetworkLoginName { get; set; }
        public string Domain { get; set; }
        public string ComputerName { get; set; }
        public string MacAddress { get; set; }
        public string IP { get; set; }
        private Nullable<int> _RetentionDispositionType;
        public Nullable<int> RetentionDispositionType
        {
            get
            {
                if (_RetentionDispositionType == null)
                    return 0;
                else
                    return _RetentionDispositionType;
            }
            set { _RetentionDispositionType = value; }
        }
    }
}
