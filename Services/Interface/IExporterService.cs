using Smead.Security;
using System.Text;

namespace MSRecordsEngine.Services.Interface
{
    public interface IExporterService
    {
        StringBuilder BuildString(Passport passport, int ViewId, string allquery, int currentLevel, string CultureShortPattern, string OffSetVal);
    }
}
