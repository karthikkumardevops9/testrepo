using Microsoft.AspNetCore.Mvc.Rendering;
using Smead.Security;

namespace MSRecordsEngine.Models.FusionModels
{
    public class LoginModel
    {
        public string SelectedDatabase { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public CommonEnums.LoginUserType LoginUserType { get; set; }
        public SelectList DatabaseList { get; set; }
        public string ErrorMsg { get; set; }
        public bool IsCloud { get; set; }
        public string Version { get; set; }
        public bool Signin { get; set; }
        public bool chkSaveAuth { get; set; }
        public bool isError { get; set; } = false;
    }
    public class DatabaseList
    {
        public int value { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
    }
  
}
