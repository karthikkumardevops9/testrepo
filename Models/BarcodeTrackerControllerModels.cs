using Smead.Security;

namespace MSRecordsEngine.Models
{

    public class DetectDestinationChangeParam
    {
        public Passport passport { get; set; }
        public string txtDestination { get; set; }
        public string txtObject { get; set; }
        public string hdnPrefixes { get; set; }
        public string shortDateFormat { get; set; }
        public string timeoffSet { get; set; }
        public BarcodeTrackerModel barcodemodel { get; set; }
    }
    public class BarcodeTrackerModel
        {
            public string lblAdditional1 { get; set; }
            // Public Property trlblAdditional1_1 As Boolean 
            // Public Property trlblAdditional1_2 As Boolean
            public string additionalField1Type { get; set; }
            public string additionalField2 { get; set; }
            public string lblAdditional2 { get; set; }
            public string hdnPrefixes { get; set; }
            public string getDestination { get; set; }
            public bool CheckgetDestination { get; set; }
            public string serverErrorMsg { get; set; }
            public string returnDestination { get; set; }
            public string returnObjectItem { get; set; }
            public bool detectDestination { get; set; }
            public int chekAdditionSystemseting { get; set; }
            public bool chkDueBackDate { get; set; }
            public string formatDueBackDate { get; set; }
            public string DueBackDateText { get; set; }

        }
        public class ClickBarckcodeTextTolistBoxParam
        {
            public Passport passport { get; set; }
            public string txtDestination { get; set; }
            public string txtObject { get; set; }
            public string hdnPrefixes { get; set; }
            public string txtDueBackDate { get; set; }
            public string userName {  get; set; }
            public string? additional1 { get; set; }
            public string? additional2 { get; set; }
        }
   
}
