using Smead.Security;
using System.Collections.Generic;

namespace MSRecordsEngine.Models.FusionModels
{
    public class LinkScriptModel
    {
        public LinkScriptModel()
        {
            ButtonsList = new List<Button>();
            ControllerList = new List<CreateController>();
        }

        public string ErrorMsg { get; set; }
        public List<CreateController> ControllerList { get; set; }
        public List<Button> ButtonsList { get; set; }
        public string lblHeading { get; set; }
        public string Title { get; set; }
        public string ReturnMessage { get; set; }
        public bool ContinuetoAnotherDialog { get; set; }
        public bool UnloadPromptWindow { get; set; }
        public bool Showprompt { get; set; }
        public bool isSuccessful { get; set; }
        public bool isBeforeAddLinkScript { get; set; }
        public bool isAfterAddLinkScript { get; set; }
        public bool isBeforeEditLinkScript { get; set; }
        public bool isAfterEditLinkScript { get; set; }
        public bool isBeforeDeleteLinkScript { get; set; }
        public bool isAfterDeleteLinkScript { get; set; }
        public string ScriptName { get; set; }
        public bool GridRefresh { get; set; }
        public bool Successful { get; set; }
        public string keyValue { get; set; }

        public class CreateController
        {
            public CreateController()
            {
                dropdownItems = new List<dropdownprop>();
                listboxItems = new List<listBox>();
            }

            public string Text { get; set; }
            public string Id { get; set; }
            public string Css { get; set; }
            public List<dropdownprop> dropdownItems { get; set; }
            public List<listBox> listboxItems { get; set; }
            public string Groupname { get; set; }
            public string ControlerType { get; set; }
            public int dropIndex { get; set; }
            public string rowCounter { get; set; }
        }

        public class Button
        {
            public string Text { get; set; }
            public string Id { get; set; }
            public string Css { get; set; }
        }

        public class dropdownprop
        {
            public string text { get; set; }
            public string value { get; set; }
        }

        public class listBox
        {
            public string text { get; set; }
            public string value { get; set; }
        }
    }

    public class linkscriptUidata
    {
        public string id { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
}
