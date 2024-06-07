using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Databas
    {
        public int Id { get; set; }
        public string DBName { get; set; }
        private Nullable<int> _DBType;
        public Nullable<int> DBType
        {
            get
            {
                if (_DBType == null)
                    return 0;
                else
                    return _DBType;
            }
            set { _DBType = value; }
        }
        public string DBConnectionText { get; set; }
        private Nullable<int> _DBConnectionTimeout;
        public Nullable<int> DBConnectionTimeout
        {
            get
            {
                if (_DBConnectionTimeout == null)
                    return 0;
                else
                    return _DBConnectionTimeout;
            }
            set { _DBConnectionTimeout = value; }
        }
        public string DBDatabase { get; set; }
        public string DBPassword { get; set; }
        public string DBProvider { get; set; }
        public string DBServer { get; set; }
        private Nullable<bool> _DBUseDBEngineUIDPWD;
        public Nullable<bool> DBUseDBEngineUIDPWD
        {
            get
            {
                if (_DBUseDBEngineUIDPWD == null)
                    return false;
                else
                    return _DBUseDBEngineUIDPWD;
            }
            set { _DBUseDBEngineUIDPWD = value; }
        }
        public string DBUserId { get; set; }
    }
}
