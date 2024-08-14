using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IDataGridService
    {
        public Task SaveNewsURL(NewUrlprops model);
        public Task<ViewQueryWindow> DrawQuery(ViewQueryWindowProps prop);
    }
}
