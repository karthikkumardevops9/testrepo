using System;
using System.Data;
using System.Diagnostics;

namespace MSRecordsEngine.Imaging
{
    [CLSCompliant(true)]
    [Serializable()]
    public partial class ScanBatchInfo
    {
        internal ScanBatchInfo() : base()
        {
        }

        public ScanBatchInfo(DataRow row) : this()
        {

            try
            {
                Id = (int)row["ScanBatchesId"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
                Sequence = (int)row["ScanSequence"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
                ScanDate = (DateTime)row["ScanDate"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                if (!string.IsNullOrEmpty(row["ScanUserName"].ToString()))
                    UserName = row["ScanUserName"].ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        private int _Id;

        public int Sequence
        {
            get
            {
                return _sequence;
            }
            set
            {
                _sequence = value;
            }
        }
        private int _sequence;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        private string _userName = string.Empty;

        public DateTime ScanDate
        {
            get
            {
                return _scanDate;
            }
            set
            {
                _scanDate = value;
            }
        }
        private DateTime _scanDate = new DateTime();
    }
}
