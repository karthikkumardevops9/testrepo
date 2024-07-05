using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IBackgroundStatusService
    {
        Task<object> ChangeNotification(int userId, string ConnectionString);
        Task<bool> InsertData(DataProcessingModel prop, string rowQuery, Passport passport);
    }
}
