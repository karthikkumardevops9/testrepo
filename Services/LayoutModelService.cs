using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services
{
    public class LayoutModelService : ILayoutModelService
    {
        public string LoadTasks(Passport passport)
        {
            var sbMenu = new StringBuilder();
            foreach (var item in Navigation.GetTasksMvc(passport))
            {
                var replaced_item = item.Replace("style='color: blue;'", "");
                sbMenu.Append(string.Format("<li>{0}</li>", replaced_item));
            }
            return sbMenu.ToString();
        }
    }
}
