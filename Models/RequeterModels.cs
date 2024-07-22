using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public class Requesters : BaseModel
    {
        public List<RequstRadioBox> rdemployddlist { get; set; }
        public List<string> itemsTobeRequest { get; set; }
        public bool chkWaitList { get; set; }
        public string lblError { get; set; }
        public string RequestID { get; set; }
        public string Priority { get; set; }
        public string DateRequested { get; set; }
        public string DateNeeded { get; set; }
        public string Status { get; set; }
        public string Instruction { get; set; }
        public bool Fulfilled { get; set; }
        public string txtFor { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string By { get; set; }
        public string PullList { get; set; }
        public string PullDate { get; set; }
        public string PullOperator { get; set; }
        public bool ExceptionAllowed { get; set; }
        public string txtComment { get; set; }
        public bool isComment { get; set; }
        public bool isException { get; set; }
        //request properties
        public string NewRequestCount { get; set; }
        public string RequestNewButtonLabel { get; set; }
        public string imgRequestNewButton { get; set; }
        public string ancRequestNewButton { get; set; }
        //request exception properties
        public string NewExceptionCount { get; set; }
        public string RequestExceptionButtonLabel { get; set; }
        public string imgRequestExceptionButton { get; set; }
        public string ancRequestExceptionButton { get; set; }
    }

    public class RequstRadioBox
    {
        public string value { get; set; }
        public string text { get; set; }
        public bool disable { get; set; }
    }

    public class GetRequesterParam
    {
        public UserInterfaceJsonReqModel req { get; set; }
        public Passport passport { get; set; }
    }

    public class RequesterParam
    {
        public UserInterfaceJsonModel req { get; set; }
        public Passport passport { get; set; }
        public DateTime DateFormatedValue { get; set; }
    }
    
    public class DeleteTrackingRequestParam
    {
        public int Id { get; set; }
        public Passport passport { get; set; }
    }
    public class UpdateRequestParam
    {
        public Requesters req { get; set; }
        public Passport passport { get; set; }
    }
    public class GetRequestDetailsParam
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public Passport Passport { get; set; }
        public string OffTimeVal { get; set; }
        public string ShortPatternFormate { get; set; }
    }

 
}
