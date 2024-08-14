using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface IDatabaseMapService
    {
        public Task<bool> IsSysAdmin(string sTableName, string ConnectionString, Table pTableEntity = null, Databas pDatabases = null);

        public bool IsADateType(Enums.DataTypeEnum eDataType);

        public Task<string> CreateNewTables(string pTableName, string pIdFieldName, Enums.meFieldTypes pFieldType, int pFieldSize, Databas pDatabaseEntity, Table pParentTableEntity, string ConnectionString);

        public Task<RtnSetTablesEntity> SetTablesEntity(string sTableName, string sUserName, string sIdFieldName, string sDatabaseName, Enums.meFieldTypes eFieldType, int iTableId, string ConnectionString);

        public Task<RtnSetViewsEntity> SetViewsEntity(string sTableName, string sViewName, string ConnectionString, int iViewId);

        public Task<bool> SetViewColumnEntity(int iViewId, string sTableName, string sIdFieldName, int sIdFieldType, string ConnectionString, Table pParentTable);

        public Task<RtnSetRelationshipsEntity> SetRelationshipsEntity(string sTableName, Table pParentTableEntity, string ConnectionString, int iRelId, string sExtraStr = "", string sIdFieldName = "");

        public Task<RtnSetTabSetEntity> SetTabSetEntity(int iTabSetId, int lViewId, string sTableName, string ConnectionString, int iTableTabId);

        public Task<List<Table>> GetAttachExistingTableList(List<Table> lTablesEntities, int iParentTableId, int iTabSetId, string ConnectionString);

        public Task<List<string>> GetChildTableIds(string pTableName, string ConnectionString);

        public Task<bool> CreateNewField(string sFieldName, Enums.DataTypeEnum eFieldType, long lSize, string sTableName, string ConnectionString);

        public Task<string> GetTableDependency(List<Table> lTableEntities, string ConnectionString, Table pUpperTableEntity, Table pLowerTableEntity, string sViewMessage);

        public Task<bool> DropTable(string pTableName, string ConnectionString);

        public Task<List<string>> GetAttachTableFieldsList(Table pTableEntity, Table pParentTableEntity, string ConnectionString);
    }
}
