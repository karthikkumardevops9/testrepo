using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using Smead.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IReportAAdminService
    {
        public string GetBindReportsMenus(string root, List<Table> lTableEntities, List<View> lViewEntities,
            List<ReportStyle> lReportStyleEntities, Passport _passport, int iCntRpt);

        public Task<RtnFillViewColField> FillViewColField(List<Table> tableObjList, List<RelationShip> relationObjList, List<KeyValuePair<string, string>> FieldNameList, Table orgTable, List<RelationShip> relationShipEntity, bool bDoUpper, int iLevel, bool bNumericOnly, string connectionString);
    }
}
