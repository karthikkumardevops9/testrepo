using Smead.Security;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MSRecordsEngine.Models
{
    public class UserInterfaceProps
    {
        public Passport passport { get; set;}
        public List<string> liststring { get; set; }
        public List<int> listints { get; set; }

    }
    public class NewUrlprops : UserInterfaceProps
    {
        public string NewUrl { get; set; }
    }

    public class ViewQueryWindowProps : UserInterfaceProps
    {
        public int viewId { get; set; }
        public int ceriteriaId { get; set; }
        public string ChildKeyField { get; set; }
        public int crumblevel { get; set; }
    }

}
