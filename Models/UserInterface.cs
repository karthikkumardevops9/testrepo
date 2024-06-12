using Smead.Security;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public class UserInterface
    {
        public class popupdocViewer
        {
            public string filesizeMB { get; set; }
            public string tabName { get; set; }
            public string tableId { get; set; }
            public string viewId { get; set; }
            public string name { get; set; }
        }
    }
    public class SaveNewAttachmentInPopupWindow
    {
        public UserInterface.popupdocViewer model { get; set; }
        public Passport passport { get; set; }
        public List<string> liststring { get; set; }
        public List<int> listints { get; set; }
        public List<object> listobject { get; set; }
    }

}
