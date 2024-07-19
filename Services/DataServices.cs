using MSRecordsEngine.Entities;
using System;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Runtime.InteropServices;
using System.ComponentModel;
using MSRecordsEngine.Models;
using System.Collections.Generic;
using System.Linq;
using MSRecordsEngine.Services.Interface;


namespace MSRecordsEngine.Services
{
    public class DataServices : IDataServices
    {
        private static IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);
        
        public string GetConnectionString(Databas DBToOpen)
        {
            return GetConnectionString(DBToOpen, true);
        }
        
        public string GetConnectionString(Databas DBToOpen, bool includeProvider)
        {
            string sConnect = string.Empty;
            if (includeProvider)
                sConnect = string.Format("Provider={0}; ", DBToOpen.DBProvider);

            sConnect += string.Format("Data Source={0}; Initial Catalog={1}; ", DBToOpen.DBServer, DBToOpen.DBDatabase);

            if (!string.IsNullOrEmpty(DBToOpen.DBUserId))
            {
                sConnect += string.Format("User Id={0}; Password={1};", DBToOpen.DBUserId, DBToOpen.DBPassword);
            }
            else
            {
                sConnect += "Persist Security Info=True;Integrated Security=SSPI;";
            }

            return sConnect;
        }

        //public static Databas GetDBObjForTable(Table oTable, string ConnectionString, bool bCheckIfDisconnected = false)
        //{
        //    try
        //    {
        //        using (var context = new TABFusionRMSContext())
        //        {

        //        }

        //        var oDatabase = new Databas();
        //        if (oTable is not null)
        //        {
        //            if (!string.IsNullOrEmpty(oTable.DBName))
        //            {
        //                oDatabase = lDatabaseEntities.Where(m => m.DBName.Trim().ToLower().Equals(oTable.DBName.Trim().ToLower())).FirstOrDefault();
        //            }
        //            else
        //            {
        //                var dbEngine = new DatabaseEngine();
        //                oDatabase.DBName = dbEngine.DBName;
        //                oDatabase.DBType = dbEngine.DBType;
        //                oDatabase.DBConnectionText = dbEngine.DBConnectionText;
        //                oDatabase.DBConnectionTimeout = dbEngine.DBConnectionTimeout;
        //                oDatabase.DBDatabase = dbEngine.DBDatabase;
        //                oDatabase.DBPassword = dbEngine.DBPassword;
        //                oDatabase.DBProvider = dbEngine.DBProvider;
        //                oDatabase.DBServer = dbEngine.DBServer;
        //                oDatabase.DBUseDBEngineUIDPWD = dbEngine.DBUseDBEngineUIDPWD;
        //                oDatabase.DBUserId = dbEngine.DBUserId;
        //            }
        //        }
        //        else
        //        {
        //            var dbEngine = new DatabaseEngine();
        //            oDatabase.DBName = dbEngine.DBName;
        //            oDatabase.DBType = dbEngine.DBType;
        //            oDatabase.DBConnectionText = dbEngine.DBConnectionText;
        //            oDatabase.DBConnectionTimeout = dbEngine.DBConnectionTimeout;
        //            oDatabase.DBDatabase = dbEngine.DBDatabase;
        //            oDatabase.DBPassword = dbEngine.DBPassword;
        //            oDatabase.DBProvider = dbEngine.DBProvider;
        //            oDatabase.DBServer = dbEngine.DBServer;
        //            oDatabase.DBUseDBEngineUIDPWD = dbEngine.DBUseDBEngineUIDPWD;
        //            oDatabase.DBUserId = dbEngine.DBUserId;
        //        }
        //        return oDatabase;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}

        public bool ProcessADOCommand(string ConnectionString ,ref string sSQL, bool bDoNoCount = false)
        {
            int recordaffected = default;

            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    if (bDoNoCount)
                    {
                        sSQL = "SET NOCOUNT OFF;" + sSQL + ";SET NOCOUNT ON";
                    }
                    recordaffected = conn.Execute(sSQL, commandType: CommandType.Text);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAStringType(Enums.DataTypeEnum eDataType)
        {
            switch (eDataType)
            {
                case Enums.DataTypeEnum.rmBSTR:
                case Enums.DataTypeEnum.rmChar:
                case Enums.DataTypeEnum.rmVarChar:
                case Enums.DataTypeEnum.rmLongVarChar:
                case Enums.DataTypeEnum.rmWChar:
                case Enums.DataTypeEnum.rmVarWChar:
                case Enums.DataTypeEnum.rmLongVarWChar:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public bool IsADateType(Enums.DataTypeEnum eDataType)
        {
            switch (eDataType)
            {
                case Enums.DataTypeEnum.rmDate:
                case Enums.DataTypeEnum.rmDBDate:
                case Enums.DataTypeEnum.rmDBTimeStamp:
                case Enums.DataTypeEnum.rmDBTime:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public bool IsANumericType(Enums.DataTypeEnum eDataType)
        {
            switch (eDataType)
            {
                case Enums.DataTypeEnum.rmTinyInt:
                case Enums.DataTypeEnum.rmUnsignedTinyInt:
                case Enums.DataTypeEnum.rmSmallInt:
                case Enums.DataTypeEnum.rmUnsignedSmallInt:
                case Enums.DataTypeEnum.rmInteger:
                case Enums.DataTypeEnum.rmBigInt:
                case Enums.DataTypeEnum.rmUnsignedInt:
                case Enums.DataTypeEnum.rmUnsignedBigInt:
                case Enums.DataTypeEnum.rmSingle:
                case Enums.DataTypeEnum.rmDouble:
                case Enums.DataTypeEnum.rmDecimal:
                case Enums.DataTypeEnum.rmNumeric:
                case Enums.DataTypeEnum.rmVarNumeric:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public bool IsSysAdmin(string tableName, string ConnectionString, bool bDBOwnerOK = true)
        {
            bool IsSysAdminRet = default;
            var dt = new DataTable();

            if (tableName.Length == 0)
            {
                return IsSysAdminRet == true;
            }

            using (var conn = CreateConnection(ConnectionString))
            {
                string argsSql = "SELECT IS_SRVROLEMEMBER ('sysadmin')";
                string arglError = "";
                var res = conn.ExecuteReader(argsSql, commandType:CommandType.Text);
                if(res != null)
                    dt.Load(res);

                if (dt != null && dt.Rows.Count > 0)
                {
                    IsSysAdminRet = Convert.ToInt32(dt.Rows[0][0]) != 0;
                    dt = null;
                }
                if (IsSysAdminRet | !bDBOwnerOK)
                {
                    string argsSql1 = "SELECT IS_MEMBER ('db_owner')";
                    string arglError1 = "";
                    res = conn.ExecuteReader(argsSql1, commandType: CommandType.Text);
                    if (res != null)
                        dt.Load(res);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        IsSysAdminRet = Convert.ToInt32(dt.Rows[0][0]) != 0;
                        dt = null;
                    }
                }
            }
            return IsSysAdminRet;
        }

        public object IsContainField(string ConnectionString, string tableName, List<SchemaColumns> schemaColumnList, string fieldName, List<KeyValuePair<string, string>> DDList)
        {
            var bHasAField = default(bool);
            bool bIsSystemAdmin;
            foreach (SchemaColumns schemaColumnObj in schemaColumnList)
            {
                if (schemaColumnObj.ColumnName.Trim().ToLower().Equals(fieldName.Trim().ToLower()))
                {
                    bHasAField = true;
                    break;
                }
            }
            bIsSystemAdmin = IsSysAdmin(tableName, ConnectionString);
            if (!bHasAField & bIsSystemAdmin)
            {
                DDList.Add(new KeyValuePair<string, string>(fieldName, fieldName));
            }
            foreach (SchemaColumns schemaColumnObj in schemaColumnList)
            {
                if (!SchemaInfoDetails.IsSystemField(schemaColumnObj.ColumnName))
                {
                    switch (schemaColumnObj.DataType)
                    {
                        case Enums.DataTypeEnum.rmBoolean:
                        case Enums.DataTypeEnum.rmSmallInt:
                        case Enums.DataTypeEnum.rmUnsignedSmallInt:
                        case Enums.DataTypeEnum.rmTinyInt:
                        case Enums.DataTypeEnum.rmUnsignedTinyInt:
                            {
                                DDList.Add(new KeyValuePair<string, string>(schemaColumnObj.ColumnName, schemaColumnObj.ColumnName));
                                break;
                            }

                        default:
                            {
                                break;
                            }

                    }
                }
            }
            return DDList;
        }

        public object IsContainStringField(string ConnectionString, string tableName, List<SchemaColumns> schemaColumnList, string fieldName, List<KeyValuePair<string, string>> DDList)
        {
            var bHasAField = default(bool);
            bool eIsSystemAdmin;
            foreach (SchemaColumns schemaColumnObj in schemaColumnList)
            {
                if (schemaColumnObj.ColumnName.Trim().ToLower().Equals(fieldName.Trim().ToLower()))
                {
                    bHasAField = true;
                    break;
                }
            }
            eIsSystemAdmin = IsSysAdmin(tableName, ConnectionString);
            if (!bHasAField & eIsSystemAdmin)
            {
                DDList.Add(new KeyValuePair<string, string>(fieldName, fieldName));
                bHasAField = false;
            }

            foreach (SchemaColumns oSchemaColumnObj in schemaColumnList)
            {
                if (!SchemaInfoDetails.IsSystemField(oSchemaColumnObj.ColumnName) & oSchemaColumnObj.IsString)
                {
                    DDList.Add(new KeyValuePair<string, string>(oSchemaColumnObj.ColumnName, oSchemaColumnObj.ColumnName));
                }
            }
            return DDList;
        }

        public bool RemoveTrackingStatusField(string ConnectionString, string tableName, string fieldName)
        {
            try
            {
                var schemaIndexList = SchemaIndex.GetTableIndexes(tableName, ConnectionString);
                var schemaColumnList = SchemaInfoDetails.GetSchemaInfo(tableName, ConnectionString);
                bool boolProcessSQL = false;
                foreach (SchemaIndex schemaIndexObj in schemaIndexList)
                {
                    if (schemaIndexObj.IndexName.Trim().ToLower().Equals(fieldName.Trim().ToLower()))
                    {

                        string indexDropSQL = "Drop Index [" + tableName + "]." + schemaIndexObj.IndexName;
                        boolProcessSQL = ProcessADOCommand(ConnectionString, ref indexDropSQL, false);
                        break;
                    }
                }
                foreach (SchemaColumns schemaInfoObj in schemaColumnList)
                {
                    if (schemaInfoObj.ColumnName.Trim().ToLower().Equals(fieldName.Trim().ToLower()))
                    {

                        string columnDropSQL = "ALTER TABLE " + tableName + " DROP COLUMN " + fieldName;
                        boolProcessSQL = ProcessADOCommand(ConnectionString, ref columnDropSQL, false);
                        break;
                    }
                }
                return boolProcessSQL;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IdFieldIsString(string tablename, string idfield)
        {
            try
            {
       
                var schemacolumnlist = SchemaInfoDetails.GetSchemaInfo(tablename, DatabaseMap.RemoveTableNameFromField(idfield));
                if (schemacolumnlist is not null)
                {
                    return schemacolumnlist[0].IsString;
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

        public string IDQueryValue(bool IfIdFieldIsString, string sID)
        {
            if (IfIdFieldIsString)
            {
                if (sID is null)
                    sID = string.Empty;
                return "'" + sID.Replace("'", "''") + "'";
            }

            if (sID is null)
                sID = "0";
            return sID;
        }

        public int ProcessADOCommand(string ConnectionString ,ref string sSQL, [Optional, DefaultParameterValue(false)] bool bDoNoCount, [Optional, DefaultParameterValue(-1)] ref int lError, [Optional, DefaultParameterValue("")] ref string sErrorMsg)
        {
            int recordaffected = default;
            using (var conn = CreateConnection(ConnectionString))
            {
                try
                {
                    if (bDoNoCount)
                    {
                        sSQL = "SET NOCOUNT OFF;" + sSQL + ";SET NOCOUNT ON";
                    }
                    recordaffected = conn.Execute("");
                }
                catch (Exception ex)
                {
                    if (lError != -1)
                    {
                        lError = Marshal.GetLastWin32Error();
                        sErrorMsg = new Win32Exception(lError).Message;
                    }
                    return -1;
                }
            }
            return recordaffected;
        }
    }
}
