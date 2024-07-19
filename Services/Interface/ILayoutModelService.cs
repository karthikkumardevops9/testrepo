using MSRecordsEngine.Models.FusionModels;
using Smead.Security;

namespace MSRecordsEngine.Services.Interface
{
    public interface ILayoutModelService
    {
        TasksBar ExecuteTasksbar(Passport passport);
        string ExecuteDashboardTasksbar(Passport passport);
    }
}
