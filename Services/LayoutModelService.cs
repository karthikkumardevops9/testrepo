using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System.Text;

namespace MSRecordsEngine.Services
{
    public class LayoutModelService : ILayoutModelService
    {
        public TasksBar ExecuteTasksbar(Passport passport)
        {
            var res = new TasksBar();
            res = GetTaskLightValues(passport);
            res.TaskList = LoadTasks(passport);
            return res;
        }

        public string ExecuteDashboardTasksbar(Passport passport)
        {
            return LoadTasks(passport);
        }

        #region Private Methods
        private TasksBar GetTaskLightValues(Passport passport)
        {
            var result = new TasksBar();
            result.RequestNewButtonLabel = "No New Requests";
            result.RequestNewButton = "0";
            result.imgRequestNewButton = "/Content/themes/TAB/img/top-action-req-green.png";
            result.ancRequestNewButton = "";

            result.RequestBatchButtonLabel = "No Batch Requests";
            result.RequestBatchButton = "0";
            result.imgRequestBatchButton = "/Content/themes/TAB/img/top-action-req-green.png";
            result.ancRequestBatchButton = "";

            result.RequestExceptionButtonLabel = "No Request Exceptions";
            result.RequestExceptionButton = "0";
            result.imgRequestExceptionButton = "/Content/themes/TAB/img/top-action-req-green.png";
            result.ancRequestExceptionButton = "";

            var lstTask = Navigation.GetTaskLightValues(passport);

            if (lstTask == null)
                return result;

            if (lstTask[0] > 0)
            {
                result.RequestNewButtonLabel = "New Request Count";
                result.RequestNewButton = lstTask[0].ToString();
                result.imgRequestNewButton = "/Content/themes/TAB/img/top-action-req-red.png";
                result.ancRequestNewButton = "/Reports/Index/newRequest";
            }

            if (lstTask[1] > 0)
            {
                result.RequestBatchButtonLabel = "Batch Request Count";
                result.RequestBatchButton = lstTask[1].ToString();
                result.imgRequestBatchButton = "/Content/themes/TAB/img/top-action-req-red.png";
                result.ancRequestBatchButton = "/handler.aspx?r=NewBatchesReport&requesting=1";
            }

            if (lstTask[2] > 0)
            {
                result.RequestExceptionButtonLabel = "Request Exception Count";
                result.RequestExceptionButton = lstTask[2].ToString();
                result.imgRequestExceptionButton = "/Content/themes/TAB/img/top-action-req-red.png";
                result.ancRequestExceptionButton = "/Reports/Index/exceptions";
            }
            return result;
        }

        private string LoadTasks(Passport passport)
        {
            var sbMenu = new StringBuilder();
            foreach (var item in Navigation.GetTasksMvc(passport))
            {
                var replaced_item = item.Replace("style='color: blue;'", "");
                sbMenu.Append(string.Format("<li>{0}</li>", replaced_item));
            }
            return sbMenu.ToString();
        }
        #endregion
    }
}
