using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MSRecordsEngine.Services.Interface
{
    public interface IDataServices
    {
        public string GetConnectionString(Databas DBToOpen);
        public string GetConnectionString(Databas DBToOpen, bool includeProvider);
        public bool ProcessADOCommand(string ConnectionString, ref string sSQL, bool bDoNoCount = false);
        public bool IsAStringType(Enums.DataTypeEnum eDataType);
        public bool IsADateType(Enums.DataTypeEnum eDataType);
        public bool IsANumericType(Enums.DataTypeEnum eDataType);
        public bool IsSysAdmin(string tableName, string ConnectionString, bool bDBOwnerOK = true);
        public object IsContainField(string ConnectionString, string tableName, List<SchemaColumns> schemaColumnList, string fieldName, List<KeyValuePair<string, string>> DDList);
        public object IsContainStringField(string ConnectionString, string tableName, List<SchemaColumns> schemaColumnList, string fieldName, List<KeyValuePair<string, string>> DDList);
        public bool RemoveTrackingStatusField(string ConnectionString, string tableName, string fieldName);
        public bool IdFieldIsString(string tablename, string idfield);
        public string IDQueryValue(bool IfIdFieldIsString, string sID);
        public int ProcessADOCommand(string ConnectionString, ref string sSQL, [Optional, DefaultParameterValue(false)] bool bDoNoCount, [Optional, DefaultParameterValue(-1)] ref int lError, [Optional, DefaultParameterValue("")] ref string sErrorMsg);
    }
}
