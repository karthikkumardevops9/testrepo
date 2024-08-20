using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IDataGridService
    {
        public Task SaveNewsURL(NewUrlprops model);
        public Task<ViewQueryWindow> DrawQuery(ViewQueryWindowProps prop);
        public Task<GridDataBinding> BuildNewData(SearchQueryRequestModal prop);
        public Task<string> GetTotalRowsForGrid(SearchQueryRequestModal prop);
        public Task<ScriptReturn> LinkscriptButtonClick([FromBody] linkscriptPropertiesUI props);
        public LinkScriptModel BuiltControls(ScriptReturn scriptresult);
        public Task<ScriptReturn> LinkscriptEvents(linkscriptPropertiesUI props);
        public Task<bool> FlowButtonsClickEvent(linkscriptPropertiesUI props);
    }
}
