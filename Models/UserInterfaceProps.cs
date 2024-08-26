using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MSRecordsEngine.Models
{
    public class UserInterfaceProps
    {
        public Passport passport { get; set; }
        public string ConnectionString { get; set; }
        public int ViewId { get; set; }
        public string TableId { get; set; }

    }
    public class NewUrlprops : UserInterfaceProps
    {
        public string NewUrl { get; set; }
    }

    public class ViewQueryWindowProps : UserInterfaceProps
    {
        public int ceriteriaId { get; set; }
        public string ChildKeyField { get; set; }
        public int crumblevel { get; set; }
    }
    public class SearchQueryRequestModal : UserInterfaceProps
    {
        public Searchparams paramss { get; set; }
        public Searchparams paramsUI { get; set; }
        public List<searchQueryModel> searchQuery { get; set; }
        public string HoldTotalRowQuery { get; set; }
    }

    public class Searchparams
    {
        public int ViewId { get; set; }
        public int pageNum { get; set; }
        public string ChildKeyField { get; set; }
        public string keyFieldValue { get; set; }
        public int firstCrumbChild { get; set; } = 0;
        public string columntype { get; set; }
        public string rowid { get; set; }
        public string preTableName { get; set; }
        public string Childid { get; set; }
        public string password { get; set; }
        public int ViewType { get; set; } = 0;
        public int crumbLevel { get; set; } = 0;
    }
    public class linkscriptPropertiesUI : UserInterfaceProps
    {
        public string WorkFlow { get; set; }
        public string[] Rowids { get; set; }       
        public InternalEngine InternalEngine { get; set; }
    }
    public class TabquickpropUI : UserInterfaceProps
    {
        public string RowsSelected { get; set; }
        public string WebRootPath { get; set; }

    }

}
