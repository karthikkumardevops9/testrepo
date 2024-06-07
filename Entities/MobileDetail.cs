using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class MobileDetail
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public string DBName { get; set; }
        private Nullable<DateTime> _Time;
        public Nullable<DateTime> Time
        {
            get
            {
                return _Time;
            }
            set { _Time = value; }
        }
        public bool IsSignOut { get; set; }
        public string Token { get; set; }
    }
}
