using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class COMMLabel
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        private Nullable<int> _OneStripJobsId;
        public Nullable<int> OneStripJobsId
        {
            get
            {
                if (_OneStripJobsId == null)
                    return 0;
                else
                    return _OneStripJobsId;
            }
            set { _OneStripJobsId = value; }
        }
        public string LabelStartString { get; set; }
        public string LabelEndString { get; set; }
        public string LowercaseChars { get; set; }
        public string RemovedChars { get; set; }
        private Nullable<int> _OutputType;
        public Nullable<int> OutputType
        {
            get
            {
                if (_OutputType == null)
                    return 0;
                else
                    return _OutputType;
            }
            set { _OutputType = value; }
        }
        public string Name { get; set; }
        private Nullable<bool> _NoTimeOut;
        public Nullable<bool> NoTimeOut
        {
            get
            {
                if (_NoTimeOut == null)
                    return false;
                else
                    return _NoTimeOut;
            }
            set { _NoTimeOut = value; }
        }
        private Nullable<int> _TimeOut;
        public Nullable<int> TimeOut
        {
            get
            {
                if (_TimeOut == null)
                    return 0;
                else
                    return _TimeOut;
            }
            set { _TimeOut = value; }
        }
    }
}
