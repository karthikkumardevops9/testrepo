using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Location
    {
        public int Id { get; set; }
        public string Description { get; set; }
        private Nullable<bool> _Out;
        public Nullable<bool> Out
        {
            get
            {
                if (_Out == null)
                    return false;
                else
                    return _Out;
            }
            set { _Out = value; }
        }
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
        private Nullable<bool> _Requestable;
        public Nullable<bool> Requestable
        {
            get
            {
                if (_Requestable == null)
                    return false;
                else
                    return _Requestable;
            }
            set { _Requestable = value; }
        }
        private Nullable<bool> _InactiveStorage;
        public Nullable<bool> InactiveStorage
        {
            get
            {
                if (_InactiveStorage == null)
                    return false;
                else
                    return _InactiveStorage;
            }
            set { _InactiveStorage = value; }
        }
        private Nullable<bool> _ArchiveStorage;
        public Nullable<bool> ArchiveStorage
        {
            get
            {
                if (_ArchiveStorage == null)
                    return false;
                else
                    return _ArchiveStorage;
            }
            set { _ArchiveStorage = value; }
        }
    }
}
