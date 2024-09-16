using MSRecordsEngine.Entities;

namespace MSRecordsEngine.Models.FusionModels
{
    public class NewSLDestructCertItem
    {
        private string _holdType;
        public string HoldType
        {
            get
            {
                return _holdType;
            }
            set
            {
                _holdType = value;
            }
        }

        private string _snoozeDate;
        public string SnoozeDate
        {
            get
            {
                return _snoozeDate;
            }
            set
            {
                _snoozeDate = value;
            }
        }
    }
}
