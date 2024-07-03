using MSRecordsEngine.Services.Interface;

namespace MSRecordsEngine.Services
{
    public class DatabaseMap : IDatabaseMap
    {
        public string RemoveTableNameFromField(string sFieldName)
        {
            string RemoveTableNameFromFieldRet = default;
            int i;
            RemoveTableNameFromFieldRet = sFieldName;
            i = sFieldName.IndexOf(".");
            if (i > 0)
            {
                RemoveTableNameFromFieldRet = sFieldName.Substring(i + 1);
            }
            RemoveTableNameFromFieldRet = RemoveTableNameFromFieldRet.Trim();
            return RemoveTableNameFromFieldRet;
        }
    }
}
