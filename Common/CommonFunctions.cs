using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using static MSRecordsEngine.RecordsManager.DateFormat;
using System.Threading;
using System.Collections;

public static class CommonFunctions
{
    private static IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);

    public const string TRACKED_LOCATION_NAME = "SLTrackedDestination";
    public static string InjectWhereIntoSQL(string sSQL, string sNewWhere, string sOperator = "AND")
    {
        string InjectWhereIntoSQLRet = default;
        string sInitWhere = string.Empty;
        string sInitOrderBy = string.Empty;
        string sInitSelect = string.Empty;
        string sRetVal = sSQL;
        int iPos;

        sSQL = CommonFunctions.NormalizeString(sSQL);
        iPos = sSQL.IndexOf(" WHERE ", StringComparison.OrdinalIgnoreCase);

        if (iPos > 0)
        {
            sInitSelect = sSQL.Substring(0, iPos).Trim();
            sInitWhere = sSQL.Substring(iPos + " WHERE ".Length).Trim();

            iPos = sInitWhere.IndexOf(" ORDER BY ", StringComparison.OrdinalIgnoreCase);
            if (iPos > 0)
            {
                sInitOrderBy = sInitWhere.Substring(iPos + " ORDER BY ".Length).Trim();
                sInitWhere = sInitWhere.Substring(0, iPos).Trim();
            }
        }
        else
        {
            iPos = sSQL.IndexOf(" ORDER BY ", StringComparison.OrdinalIgnoreCase);
            if (iPos > 0)
            {
                sInitOrderBy = sSQL.Substring(iPos + " ORDER BY ".Length).Trim();
                sInitSelect = sSQL.Substring(0, iPos).Trim();
            }
            else
            {
                sInitSelect = sSQL.Trim();
            }
        }

        sRetVal = sInitSelect;

        if (!string.IsNullOrEmpty(sInitWhere))
        {
            if (!string.IsNullOrEmpty(sNewWhere.Trim()))
            {
                sRetVal += " WHERE (" + ParenEncloseStatement(sInitWhere) + " " + sOperator + " " + ParenEncloseStatement(sNewWhere) + ")";
            }
            else
            {
                sRetVal += " WHERE " + ParenEncloseStatement(sInitWhere);
            }
        }
        else if (!string.IsNullOrEmpty(sNewWhere.Trim()))
        {
            sRetVal += " WHERE " + ParenEncloseStatement(sNewWhere);
        }

        if (!string.IsNullOrEmpty(sInitOrderBy))
            sRetVal += " ORDER BY " + sInitOrderBy;
        InjectWhereIntoSQLRet = sRetVal;
        return InjectWhereIntoSQLRet;
    }

    public static List<Table> GetUsedTrackingTables(Table oTables, List<Table> trackingTable)
    {
        var cUsedTables = new List<Table>();
        if (oTables is not null)
        {
            foreach (Table oTmpTables in trackingTable)
            {
                // If container then only parent Containers should display
                if ((bool)(0 is var arg27 && oTables.TrackingTable is { } arg26 ? arg26 > arg27 : (bool?)null))
                {
                    if ((bool)(oTables.TrackingTable is var arg28 && oTmpTables.TrackingTable is { } arg29 && arg28.HasValue ? arg28 > arg29 : (bool?)null))
                    {
                        cUsedTables.Add(oTmpTables);
                    }
                }
                else if ((bool)(0 is var arg31 && oTmpTables.TrackingTable is { } arg30 ? arg30 > arg31 : (bool?)null))
                {
                    cUsedTables.Add(oTmpTables);
                }
            }
        }

        return cUsedTables;
    }

    public static bool CreateCoalesceFields(List<Table> cTables, string ConnectionString, ref string sFields, bool bIncludeAS = true)
    {
        bool CreateCoalesceFieldsRet = default;
        SchemaColumns oSchemaColumns;
        string sPrefixOne;
        string sFieldNameOne;
        string sPrefixTwo;
        string sFieldNameTwo;
        sFields = "";
        CreateCoalesceFieldsRet = false;
        if (cTables is null)
            return CreateCoalesceFieldsRet;

        foreach (Table oTmpTables in cTables)
        {
            sPrefixOne = oTmpTables.DescFieldPrefixOne;
            sFieldNameOne = oTmpTables.DescFieldNameOne;
            sPrefixTwo = oTmpTables.DescFieldPrefixTwo;
            sFieldNameTwo = oTmpTables.DescFieldNameTwo;
            if (string.IsNullOrEmpty(sFieldNameOne))
                sFieldNameOne = DatabaseMap.RemoveTableNameFromField(oTmpTables.IdFieldName);
            if (!string.IsNullOrEmpty(sPrefixOne))
                sFields += "'" + sPrefixOne + " ' + ";
            oSchemaColumns = SchemaInfoDetails.GetSchemaInfo(oTmpTables.TableName, ConnectionString, sFieldNameOne).Where(m => m.ColumnName.Trim().ToLower().Equals(sFieldNameOne.Trim().ToLower())).FirstOrDefault();
            if (oSchemaColumns.IsString)
            {
                sFields += oTmpTables.TableName + "." + sFieldNameOne;
            }
            else
            {
                sFields += "CAST(" + oTmpTables.TableName + "." + sFieldNameOne + " AS VARCHAR)";
            }

            oSchemaColumns = null;

            if (!string.IsNullOrEmpty(sFieldNameTwo))
            {
                sFields += " + ' " + sPrefixTwo + " ' + ";
                oSchemaColumns = SchemaInfoDetails.GetSchemaInfo(oTmpTables.TableName, ConnectionString, sFieldNameTwo).Where(m => m.ColumnName.Trim().ToLower().Equals(sFieldNameTwo.Trim().ToLower())).FirstOrDefault();
                if (oSchemaColumns.IsString)
                {
                    sFields += oTmpTables.TableName + "." + sFieldNameTwo + ", ";
                }
                else
                {
                    sFields += "CAST(" + oTmpTables.TableName + "." + sFieldNameTwo + " AS VARCHAR), ";
                }

                oSchemaColumns = null;
            }
            else
            {
                sFields += ", ";
            }
        }

        if (!string.IsNullOrEmpty(sFields))
        {
            sFields = string.Format("COALESCE({0}, 'Never Tracked')", sFields.Substring(0, sFields.Length - 2));
            if (bIncludeAS)
                sFields += " AS " + TRACKED_LOCATION_NAME;
            CreateCoalesceFieldsRet = true;
        }

        return CreateCoalesceFieldsRet;

    }

    public static string NormalizeString(string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        s = s.Replace("\t", " ");
        s = s.Replace("\r", " ");
        s = s.Replace("\n", " ");

        while (s.Contains("  "))
            s = s.Replace("  ", " ");

        return s;
    }

    private static string ParenEncloseStatement(string sSQL)
    {
        string ParenEncloseStatementRet = default;
        long iParenCount;
        long iMaxParenCount;
        int iIndex;
        bool bDoEnclose;
        bool bInString;
        string sCurChar;

        bDoEnclose = false;
        bInString = false;
        iIndex = 1;
        iParenCount = 0L;
        iMaxParenCount = 0L;
        while (iIndex <= sSQL.Length && !bDoEnclose)
        {
            sCurChar = sSQL.Substring(iIndex - 1, 1);
            if (sCurChar == "\"")
            {
                bInString = !bInString;
            }

            if (!bInString)
            {
                if (sCurChar == "(")
                {
                    iParenCount = iParenCount + 1L;
                }

                if (sCurChar == ")")
                {
                    iParenCount = iParenCount - 1L;
                }
            }

            if (iParenCount > iMaxParenCount)
            {
                iMaxParenCount = iParenCount;
            }

            if (iParenCount == 0L && iIndex > 1 && iIndex < sSQL.Length && iMaxParenCount > 0L)
            {
                bDoEnclose = true;
            }

            iIndex = iIndex + 1;
        }

        if (iMaxParenCount == 0L)
        {
            bDoEnclose = true;
        }

        if (bDoEnclose)
        {
            ParenEncloseStatementRet = "(" + sSQL + ")";
        }
        else
        {
            ParenEncloseStatementRet = sSQL;
        }

        return ParenEncloseStatementRet;
    }

    public static List<T> GetRecords<T>(string ConnectionString, string sSql, object param = null)
    {
        List<T> records = new List<T>();
        try
        {
            using (var conn = CreateConnection(ConnectionString))
            {
                if (param != null)
                    records = conn.Query<T>(sSql, param).ToList();
                else
                    records = conn.Query<T>(sSql).ToList();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        return records;
    }

    public static bool ProcessADOCommand(string ConnectionString, ref string sSQL, bool bDoNoCount = false)
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

    public static bool IsAStringType(Enums.DataTypeEnum eDataType)
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

    public static bool IsADateType(Enums.DataTypeEnum eDataType)
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

    public static bool IsANumericType(Enums.DataTypeEnum eDataType)
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

    public static bool IsSysAdmin(string tableName, string ConnectionString, bool bDBOwnerOK = true)
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
            var res = conn.ExecuteReader(argsSql, commandType: CommandType.Text);
            if (res != null)
                dt.Load(res);

            if (dt != null && dt.Rows.Count > 0)
            {
                IsSysAdminRet = Convert.ToInt32(dt.Rows[0][0]) != 0;
                dt = new DataTable();
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
                    dt = new DataTable();
                }
            }
        }
        return IsSysAdminRet;
    }

    public static List<KeyValuePair<string, string>> GetParamFromConnString(string ConnectionString, Databas dbObj = null)
    {
        string conString = ConnectionString;
        var builder = new SqlConnectionStringBuilder(conString);
        var conStrList = new List<KeyValuePair<string, string>>();
        // Dim sConnect As String = String.Empty
        string DBServer = builder.DataSource;
        string DBDatabase = builder.InitialCatalog;
        string DBUserId = builder.UserID;
        string DBPassword = builder.Password;
        conStrList.Add(new KeyValuePair<string, string>("DBServer", DBServer));
        conStrList.Add(new KeyValuePair<string, string>("DBDatabase", DBDatabase));
        conStrList.Add(new KeyValuePair<string, string>("DBUserId", DBUserId));
        conStrList.Add(new KeyValuePair<string, string>("DBPassword", DBPassword));
        return conStrList;
    }

    public static object IsContainField(string ConnectionString, string tableName, List<SchemaColumns> schemaColumnList, string fieldName, List<KeyValuePair<string, string>> DDList)
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

    public static object IsContainStringField(string ConnectionString, string tableName, List<SchemaColumns> schemaColumnList, string fieldName, List<KeyValuePair<string, string>> DDList)
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

    public static bool RemoveTrackingStatusField(string ConnectionString, string tableName, string fieldName)
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

    public static bool IdFieldIsString(string tablename, string idfield)
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

    public static object ConvertDTToJQGridResult(DataTable dtRecords, int totalRecords, string sidx, string sord, int page, int rows)
    {
        int totalPages = (int)Math.Round(Math.Truncate(Math.Ceiling(totalRecords / (float)rows)));
        int pageIndex = Convert.ToInt32(page) - 1;

        var Dv = dtRecords.AsDataView();
        if (!string.IsNullOrEmpty(sidx))
        {
            Dv.Sort = sidx + " " + sord;
        }

        var objListOfEmployeeEntity = new List<object>();
        foreach (DataRowView dRow in Dv)
        {
            var hashtable = new Hashtable();
            foreach (DataColumn column in dtRecords.Columns)
                hashtable.Add(column.ColumnName, dRow[column.ColumnName].ToString());
            objListOfEmployeeEntity.Add(hashtable);
        }
        var jsonData = new { total = totalPages, page, records = totalRecords, rows = objListOfEmployeeEntity };
        return jsonData;
    }

    public static string ValidateSQLStatement(string SQLString, string ConnectionString)
    {
        string sReturnErrorMessage = string.Empty;
        try
        {
            using (var conn = CreateConnection(ConnectionString))
            {
                SQLString = CommonFunctions.NormalizeString(SQLString);
                var testquery = conn.Query(SQLString);
                sReturnErrorMessage = string.Empty;
            }
        }
        catch (Exception ex)
        {
            sReturnErrorMessage = ex.Message;
        }
        return sReturnErrorMessage;
    }
    public static string ToClientDateFormats(this DateTime dt)
    {
        return string.Format(CultureInfo.CurrentCulture, "{0:d}", dt);
    }
    public static DateTime ConvertStringToCulture(string date, string dateformat, string culture = null)
    {
        if(culture == null)
        {
            return DateTime.ParseExact(date, dateformat, CultureInfo.InvariantCulture);
        }
        else
        {
            return DateTime.ParseExact(date, dateformat, new CultureInfo($"{culture}"));
        }
    }
    public static string ConvertStringToSqlCulture(string date, string dateformat)
    {
        DateTime parseDate = DateTime.ParseExact(date, dateformat, CultureInfo.InvariantCulture);
        string formatDateToSql = parseDate.ToString("yyyy-MM-dd");
        return formatDateToSql;
    }


}

