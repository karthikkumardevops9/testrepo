using System;
using System.Data;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace MSRecordsEngine.Imaging
{
    [CLSCompliant(true)]
    [Serializable()]
    public partial class CheckOutInfo
    {
        internal CheckOutInfo() : base()
        {
        }

        public CheckOutInfo(DataRow row)
        {
            Owner = row["CheckedOutUser"].ToString();
            IPAddress = row["CheckedOutIP"].ToString();
            MACAddress = row["CheckedOutMAC"].ToString();

            if (row["CheckedOut"] is DBNull)
            {
                IsCheckedOut = false;
            }
            else
            {
                IsCheckedOut = (bool)row["CheckedOut"];
            }

            if (row["CheckedOutUserId"] is DBNull)
            {
                OwnerId = 0;
            }
            else
            {
                OwnerId = (int)row["CheckedOutUserId"];
            }

            if (row["CheckedOutDate"] is DBNull)
            {
                DateTime = default;
            }
            else
            {
                DateTime = Conversions.ToDate(row["CheckedOutDate"]);
            }
        }

        public string Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }
        private string _owner = string.Empty;

        public int OwnerId
        {
            get
            {
                return _ownerId;
            }
            set
            {
                _ownerId = value;
            }
        }
        private int _ownerId;

        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
            set
            {
                _dateTime = value;
            }
        }
        private DateTime _dateTime;

        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }
        private string _location = string.Empty;

        public string MACAddress
        {
            get
            {
                return _MACAddress;
            }
            set
            {
                _MACAddress = value;
            }
        }
        private string _MACAddress = string.Empty;

        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                _IPAddress = value;
            }
        }
        private string _IPAddress = string.Empty;

        public bool IsCheckedOut
        {
            get
            {
                return _isCheckedOut;
            }
            set
            {
                _isCheckedOut = value;
            }
        }
        private bool _isCheckedOut;
    }
}
