using MSRecordsEngine.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace MSRecordsEngine.Services
{
    public class GetInfoUsingDapper
    {
        private static IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);

        // Get schema info using default connection so we do not need to open connection every time
        public static async Task<DataRow> GetSchemaInfo(string connectionString, string sTableName, string sIdFieldName = "")
        {
            try
            {
                var dt = new DataTable();
                using (var conn = CreateConnection(connectionString))
                {
                    string sSQL = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS";
                    var lSchemaColumnList = new List<SchemaColumns>();
                    if (!string.IsNullOrEmpty(sTableName) && string.IsNullOrEmpty(sIdFieldName))
                    {
                        sSQL = sSQL + " where TABLE_NAME= '" + sTableName + "'";
                    }
                    else if (!string.IsNullOrEmpty(sTableName) && !string.IsNullOrEmpty(sIdFieldName))
                    {
                        sSQL = sSQL + " where TABLE_NAME= '" + sTableName + "' AND  COLUMN_NAME='" + sIdFieldName + "'";
                    }
                    else if (string.IsNullOrEmpty(sTableName) && !string.IsNullOrEmpty(sIdFieldName))
                    {
                        sSQL = sSQL + " where COLUMN_NAME='" + sIdFieldName + "'";
                    }
                    var res = await conn.ExecuteReaderAsync(sSQL, commandType: CommandType.Text);
                    if (res != null)
                        dt.Load(res);

                    return dt.Rows[0];
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> IdFieldIsString(string connectionString, string sTableName, string sIdFieldName)
        {
            try
            {
                var oDatatable = await GetSchemaInfo(connectionString, sTableName, DatabaseMap.RemoveTableNameFromField(sIdFieldName));
                if (oDatatable is not null)
                {
                    var datatype = oDatatable["DATA_TYPE"];
                    return IsAStringForSchema(Convert.ToString(datatype));
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public static bool IsAStringForSchema(string oDataType)
        {
            switch (oDataType ?? "")
            {
                case "char":
                case "varchar":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext":
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public static async Task<int> UserLinkIndexTableIdSize(string ConnectionString)
        {
            int miUserLinkIndexTableIdSize;
            var pSchemaInfo = await GetSchemaInfo(ConnectionString, "USERLINKS", "INDEXTABLEID");
            if (pSchemaInfo != null)
            {
                try
                {
                    var schemaInfo = pSchemaInfo["CHARACTER_MAXIMUM_LENGTH"];
                    miUserLinkIndexTableIdSize = Convert.ToInt32(schemaInfo);
                }
                catch (Exception)
                {
                    miUserLinkIndexTableIdSize = 30;
                }
            }
            else
            {
                miUserLinkIndexTableIdSize = 30;
            }
            pSchemaInfo = null;
            return miUserLinkIndexTableIdSize;
        }

        public static async Task<CoulmnSchemaInfo> GetCoulmnSchemaInfo(string ConnectionString, string tableName, string coulmnName)
        {

            var query = @$"SELECT 
                            c.TABLE_CATALOG,
                            c.TABLE_SCHEMA,
                            c.TABLE_NAME,
                            c.COLUMN_NAME,
                            c.DATA_TYPE,
                            c.CHARACTER_MAXIMUM_LENGTH,
                            c.IS_NULLABLE,
                            c.COLUMN_DEFAULT,
                            col.is_identity AS IsAutoIncrement
                        FROM 
                            INFORMATION_SCHEMA.COLUMNS c
                        INNER JOIN 
                            sys.tables t ON t.name = c.TABLE_NAME
                        INNER JOIN 
                            sys.columns col ON col.object_id = t.object_id AND col.name = c.COLUMN_NAME
                        WHERE 
                            c.TABLE_NAME = '{tableName}' AND c.COLUMN_NAME = '{coulmnName}';";

            using (var conn = CreateConnection(ConnectionString))
            {
                var columnSchema = await conn.QuerySingleOrDefaultAsync<CoulmnSchemaInfo>(query, commandType: CommandType.Text);
                return columnSchema;
            }

        }

        public static async Task<List<CoulmnSchemaInfo>> GetCoulmnSchemaInfo(string ConnectionString, string tableName)
        {

            var query = @$"SELECT 
                            c.TABLE_CATALOG,
                            c.TABLE_SCHEMA,
                            c.TABLE_NAME,
                            c.COLUMN_NAME,
                            c.DATA_TYPE,
                            c.CHARACTER_MAXIMUM_LENGTH,
                            c.IS_NULLABLE,
                            c.COLUMN_DEFAULT,
                            col.is_identity AS IsAutoIncrement
                        FROM 
                            INFORMATION_SCHEMA.COLUMNS c
                        INNER JOIN 
                            sys.tables t ON t.name = c.TABLE_NAME
                        INNER JOIN 
                            sys.columns col ON col.object_id = t.object_id AND col.name = c.COLUMN_NAME
                        WHERE 
                            c.TABLE_NAME = '{tableName}';";

            using (var conn = CreateConnection(ConnectionString))
            {
                var columnSchema = (await conn.QueryAsync<CoulmnSchemaInfo>(query, commandType: CommandType.Text)).ToList();
                return columnSchema;
            }

        }

        public static bool IsADateType(string eDataType)
        {
            switch (eDataType.ToLower())
            {
                case "date":
                case "datetime":
                case "datetime2":
                case "timestamp":
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public static bool IsAStringType(string eDataType)
        {
            switch (eDataType.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "nchar":
                case "ntext":
                case "text":
                case "char":
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public static async Task<bool> ProcessADOCommand(string sSQL, string connectionString,bool bDoNoCount = false)
        {
            int recordaffected = default;
            using (var conn = CreateConnection(connectionString))
            {
                try
                {
                    if (bDoNoCount)
                    {
                        sSQL = "SET NOCOUNT OFF;" + sSQL + ";SET NOCOUNT ON";
                    }
                    recordaffected = await conn.ExecuteAsync(sSQL, commandType: CommandType.Text);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
