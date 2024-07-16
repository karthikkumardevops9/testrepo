using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public class MoveradioBoxModel : BaseModel
    {
        public MoveradioBoxModel()
        {
            Data = new List<MoveradioBox>();
        }
        public List<MoveradioBox> Data { get; set; } = new List<MoveradioBox>();
    }
    public class Moving : BaseModel
    {
        public Moving()
        {
            rbDestinationsItem = new List<MoveradioBox>();
            rbDestinationTable = new List<string>();
            rbDestinationViews = new List<List<MoveradioBox>>();
            itemsTobeMove = new List<string>();
        }

        public List<MoveradioBox> rbDestinationsItem { get; set; }
        public List<string> rbDestinationTable { get; set; }
        public List<List<MoveradioBox>> rbDestinationViews { get; set; }
        public List<string> itemsTobeMove { get; set; }
        public string moveView { get; set; }
    }

    public class MoveradioBox
    {
        public string value { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
        public string ViewName { get; set; }
        public int viewId { get; set; }
        public string upperTable { get; set; }
    }

    public class MoveRowsCommonParam
    {
        public UserInterfaceJsonReqModel req { get; set; }
        public Passport passport { get; set; }
    }
    public class BtnMoveItemsParam
    {
        public BtnMoveItemsReqModel req { get; set; }
        public Passport passport { get; set; }
        public string ClientIpAddress { get; set; }
    }
}
