using Dapper;
using Microsoft.VisualBasic;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services
{
    public class TrackingServices : ITrackingServices
    {
       
        private IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);
        private readonly CommonControllersService<TrackingServices> _commonService;

        public TrackingServices(CommonControllersService<TrackingServices> commonService)
        {
            _commonService = commonService;
        }

        //public static string StripLeadingZeros(string stripThis)
        //{
        //    if (string.IsNullOrEmpty(stripThis))
        //        return string.Empty;
        //    if (!Information.IsNumeric(stripThis))
        //        return stripThis;

        //    while (stripThis.Trim().Length > 0)
        //    {
        //        if (string.Compare(stripThis.Substring(0, 1), "0") != 0)
        //            return stripThis.Trim();
        //        stripThis = stripThis.Substring(1);
        //    }

        //    return stripThis.Trim();
        //}

        //public static string ZeroPaddedString(object oId)
        //{
        //    if (oId is null || oId.ToString().Length == 0)
        //    {
        //        return "";
        //    }
        //    else
        //    {
        //        string sId = oId.ToString();

        //        if (Information.IsNumeric(sId))
        //        {
        //            sId = StripLeadingZeros(sId);
        //            return sId.PadLeft(DatabaseMap.UserLinkIndexTableIdSize, '0');
        //        }
        //        else
        //        {
        //            return sId.Trim();
        //        }
        //    }
        //}

        public async Task<bool> IsOutDestination(string oDestinationTable, string oDestinationId, string ConnectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var oSystem = await context.Systems.FirstOrDefaultAsync();
                    var oDestTable = await context.Tables.Where(m => m.TableName.Trim().ToLower().Equals(oDestinationTable.Trim().ToLower())).FirstOrDefaultAsync();
                    var outType = default(bool);
                    if (oSystem.TrackingOutOn == true && oSystem.DateDueOn == true)
                    {
                        switch (oDestTable.OutTable)
                        {
                            case 0:
                                {
                                    try
                                    {
                                        if (oDestTable.DBName != null)
                                        {
                                            var oDatabase = await context.Databases.FirstOrDefaultAsync(m => m.DBName.Trim().ToLower().Equals(oDestTable.DBName.Trim().ToLower()));
                                            if (oDatabase != null)
                                                ConnectionString = _commonService.GetConnectionString(oDatabase, false);

                                        }
                                        string sSQL = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}]='{3}'", oDestTable.TrackingOUTFieldName, oDestTable.TableName, DatabaseMap.RemoveTableNameFromField(oDestTable.IdFieldName), oDestinationId);
                                        using (var conn = CreateConnection(ConnectionString))
                                        {
                                            outType = Convert.ToBoolean(await conn.ExecuteScalarAsync(sSQL));
                                        }
                                    }
                                    catch
                                    {
                                        outType = false;
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    outType = true;
                                    break;
                                }

                            case 2:
                                {
                                    outType = false;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        outType = false;
                    }
                    return outType;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DateTime> GetDueBackDate(string oDestinationTable, string oDestinationId, string ConnectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var oDestTable = await context.Tables.Where(m => m.TableName.Trim().ToLower().Equals(oDestinationTable.Trim().ToLower())).FirstOrDefaultAsync();
                    int oDueBackDaysInt = 0;
                    if (oDestTable.TrackingDueBackDaysFieldName.Length > 0)
                    {
                        if (oDestTable.DBName != null)
                        {
                            var oDatabase = await context.Databases.FirstOrDefaultAsync(m => m.DBName.Trim().ToLower().Equals(oDestTable.DBName.Trim().ToLower()));
                            if (oDatabase != null)
                                ConnectionString = _commonService.GetConnectionString(oDatabase, false);
                        }

                        string sSQL = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}]='{3}'", oDestTable.TrackingDueBackDaysFieldName, oDestTable.TableName, DatabaseMap.RemoveTableNameFromField(oDestTable.IdFieldName), oDestinationId);

                        using (var conn = CreateConnection(ConnectionString))
                        {
                            var oDueBackDays = await conn.ExecuteScalarAsync(sSQL);
                            if (!(oDueBackDays is DBNull))
                            {
                                oDueBackDaysInt = Convert.ToInt32(oDueBackDays);
                            }
                        }
                    }
                    if (oDueBackDaysInt <= 0)
                    {
                        var defalutDueBackDays = await context.Systems.OrderBy(m => m.Id).FirstOrDefaultAsync();
                        oDueBackDaysInt = (int)defalutDueBackDays.DefaultDueBackDays;
                    }

                    return DateTime.Now.AddDays(oDueBackDaysInt);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task PrepareDataForTransfer(string trackableType, string trackableID, string destinationType,
            string destinationID, DateTime DueBackDate, string userName, Passport passport, string trackingAdditionalField1 = null, string trackingAdditionalField2 = null)
        {
            string oDestinationId = null;
            string oObjectId = null;
            Table objectTable = null;
            Table destTable = null;
            string connStringObject = "";
            string connStringDestinaion = "";
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    objectTable = await context.Tables.Where(m => m.TableName.Trim().ToLower().Equals(trackableType.Trim().ToLower())).FirstOrDefaultAsync();
                    destTable = await context.Tables.Where(m => m.TableName.Trim().ToLower().Equals(destinationType.Trim().ToLower())).FirstOrDefaultAsync();

                    if (objectTable.DBName != null)
                    {
                        var oDatabase = await context.Databases.FirstOrDefaultAsync(m => m.DBName.Trim().ToLower().Equals(objectTable.DBName.Trim().ToLower()));
                        if (oDatabase != null)
                            connStringObject = _commonService.GetConnectionString(oDatabase, false);
                    }
                    connStringObject = connStringObject.Length > 0 ? connStringObject : passport.ConnectionString;

                    if (destTable.DBName != null)
                    {
                        var oDatabase = await context.Databases.FirstOrDefaultAsync(m => m.DBName.Trim().ToLower().Equals(destTable.DBName.Trim().ToLower()));
                        if (oDatabase != null)
                            connStringDestinaion = _commonService.GetConnectionString(oDatabase, false);
                    }
                    connStringDestinaion = connStringDestinaion.Length > 0 ? connStringDestinaion : passport.ConnectionString;

                    bool IfObjIdFieldIsString = await GetInfoUsingDapper.IdFieldIsString(connStringObject, objectTable.TableName, objectTable.IdFieldName);
                    if (!IfObjIdFieldIsString)
                    {
                        int oUserLinkTableIdSize = await GetInfoUsingDapper.UserLinkIndexTableIdSize(passport.ConnectionString);
                        oObjectId = (new string('0', oUserLinkTableIdSize) + trackableID).Substring(trackableID.Length, oUserLinkTableIdSize);
                    }
                    else
                    {
                        oObjectId = trackableID;
                    }

                    bool IfDestIdFieldIsString = await GetInfoUsingDapper.IdFieldIsString(connStringDestinaion, destTable.TableName, destTable.IdFieldName);
                    if (!IfDestIdFieldIsString)
                    {
                        int oUserLinkTableIdSize = await GetInfoUsingDapper.UserLinkIndexTableIdSize(passport.ConnectionString);
                        oDestinationId = (new string('0', oUserLinkTableIdSize) + destinationID).Substring(destinationID.Length, oUserLinkTableIdSize);
                    }
                    else
                    {
                        oDestinationId = destinationID;
                    }


                    DoTransfer(trackableType,
                               oObjectId,
                               destinationType,
                               oDestinationId,
                               false,
                               DueBackDate,
                               DateTime.Now,
                               trackingAdditionalField1,
                               trackingAdditionalField2,
                               userName,
                               passport);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void DoTransfer(string strObjectTableName,
                                       string strObjectTableId,
                                       string strDestinationTableName,
                                       string strDestinationTableId,
                                       bool bIsReconciliationOn,
                                       DateTime? dtDueDate,
                                       DateTime? dtTransactionDateTime,
                                       string strTrackingAdditionalField1,
                                       string strTrackingAdditionalField2,
                                       string strUserName,
                                       Passport passport)
        {
            try
            {
                RecordsManager.Tracking.Transfer(strObjectTableName, strObjectTableId, strDestinationTableName, strDestinationTableId, (DateTime)dtDueDate, strUserName, passport, strTrackingAdditionalField1, strTrackingAdditionalField2, (DateTime)dtTransactionDateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public BuildTrackingLocationSQL BuildTrackingLocationSQL(List<Table> itableQuery, string ConnectionString, string sCurrentSQL, Table oTables)
        {
            var model = new BuildTrackingLocationSQL();
            string BuildTrackingLocationSQLRet = default;
            int iFromPos = 0;
            int iWherePos = 0;
            List<Table> cUsedTables;
            string sPreFrom = string.Empty;
            string sFrom = string.Empty;
            string sWhere = string.Empty;
            string sReturnSQL = string.Empty;
            string sNewFields = string.Empty;
            string sJoinTables = string.Empty;

            sCurrentSQL = CommonFunctions.NormalizeString(sCurrentSQL);
            BuildTrackingLocationSQLRet = sCurrentSQL;

            if (oTables == null)
            {
                model.BuildTrackingLocationSQLRet = BuildTrackingLocationSQLRet;
                model.Table = oTables;
                return model;
            }

            sWhere = "";
            iFromPos = sCurrentSQL.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);

            if (iFromPos > 0)
            {
                sPreFrom = sCurrentSQL.Substring(0, iFromPos);
                sFrom = sCurrentSQL.Substring(iFromPos + " FROM ".Length);

                iWherePos = sFrom.IndexOf(" WHERE ", StringComparison.OrdinalIgnoreCase);
                if (iWherePos > 0)
                {
                    sWhere = sFrom.Substring(iWherePos);
                    sFrom = sFrom.Substring(0, iWherePos);
                }

                var validateFromOneTable = ValidateFromOneTable(sFrom);
                sFrom = validateFromOneTable.From;

                if (!validateFromOneTable.ValidateFromOneTableRet)
                {
                    model.BuildTrackingLocationSQLRet = BuildTrackingLocationSQLRet;
                    model.Table = oTables;
                    return model;
                }
            }
            else
            {
                model.BuildTrackingLocationSQLRet = BuildTrackingLocationSQLRet;
                model.Table = oTables;
                return model;
            }

            cUsedTables = CommonFunctions.GetUsedTrackingTables(oTables, itableQuery.Where(m => m.TrackingTable > 0).ToList());

            if (cUsedTables.Count > 0)
            {
                if (!CommonFunctions.CreateCoalesceFields(cUsedTables, ConnectionString, ref sNewFields, false))
                {
                    model.BuildTrackingLocationSQLRet = BuildTrackingLocationSQLRet;
                    model.Table = oTables;
                    return model;
                }

                var createJoinTables = CreateJoinTables(cUsedTables, sJoinTables);
                cUsedTables = createJoinTables.Tables;
                sJoinTables = createJoinTables.Joins;


                if (!createJoinTables.CreateJoinTablesRet)
                {
                    return model;
                }

                sReturnSQL = FixupFieldNames(sPreFrom, oTables.TableName) + ", " + sNewFields;
                sReturnSQL += " FROM " + new string('(', cUsedTables.Count + 1) + "[" + oTables.TableName + "]";
                sReturnSQL += " LEFT JOIN TrackingStatus ON ((TrackingStatus.TrackedTable = '" + oTables.TableName + "') AND ";
                sReturnSQL += "([" + oTables.TableName + "].[" + DatabaseMap.RemoveTableNameFromField(oTables.IdFieldName) + "] = TrackingStatus.TrackedTableId))) ";
                sReturnSQL += sJoinTables;

                if (!string.IsNullOrEmpty(sWhere.Trim()))
                    sReturnSQL += sWhere;
                BuildTrackingLocationSQLRet = sReturnSQL;
            }

            cUsedTables = null;
            model.BuildTrackingLocationSQLRet = BuildTrackingLocationSQLRet;
            model.Table = oTables;
            return model;
        }

        private ValidateFromOneTableReturn ValidateFromOneTable(string sFrom)
        {
            var model = new ValidateFromOneTableReturn();

            bool ValidateFromOneTableRet = true;

            sFrom = sFrom.Substring(" FROM ".Length).Trim();

            if (sFrom.Contains(",") || sFrom.Contains(" ") || sFrom.Contains(" JOIN "))
            {
                ValidateFromOneTableRet = false;
            }

            model.ValidateFromOneTableRet = ValidateFromOneTableRet;
            model.From = sFrom;
            return model;
        }

        private string FixupFieldNames(string sSelect, string sTableName)
        {
            string FixupFieldNamesRet = default;
            bool bSimpleField = false;
            bool bBogus = false;
            int iSelectPos = 0;
            int iCurLoc = 0;
            string sFields = string.Empty;
            string sField = string.Empty;
            string sResult = string.Empty;
            FixupFieldNamesRet = sSelect;
            sResult = "";

            iSelectPos = Strings.InStr(1, sSelect, "SELECT ", CompareMethod.Text);

            if (iSelectPos == 1)
            {
                sFields = Strings.Trim(Strings.Mid(sSelect, Strings.Len("SELECT ") + 1, Strings.Len(sSelect)));
                iCurLoc = 1;

                while (FindNextFieldName(sFields, ref sField, ref bSimpleField, ref bBogus, ref iCurLoc) == true)
                {
                    if (bBogus)
                    {
                        // ahhhh crap - now what?
                        return FixupFieldNamesRet;
                    }
                    // deal with complex field types
                    if (bSimpleField)
                    {
                        if (Strings.InStr(1, sField, ".", CompareMethod.Text) > 0)
                        {
                            sResult = sResult + sField + ", ";
                        }
                        else if (string.Compare(sField, "*") == 0)
                        {
                            sResult = sResult + "[" + sTableName + "]." + sField + ", ";
                        }
                        else
                        {
                            sResult = sResult + "[" + sTableName + "].[" + sField + "], ";
                        }
                    }
                    else
                    {
                        sResult = sResult + sField + ", ";
                    }
                }
            }

            sResult = Strings.Left(sResult, Strings.Len(sResult) - 2);
            FixupFieldNamesRet = "SELECT " + sResult;

            return FixupFieldNamesRet;
        }

        private bool FindNextFieldName(string sFields, ref string sField, ref bool bSimpleField, ref bool bBogus, ref int iCurLoc)
        {
            bool FindNextFieldNameRet = default;
            int iIndex;
            int iLen;
            int iParCount;
            string sChar;
            bool bDone;
            bool bInQuote;

            FindNextFieldNameRet = true;

            sFields = sFields + ",";
            iLen = Strings.Len(sFields);
            iIndex = iCurLoc;
            bDone = false;
            bInQuote = false;
            iParCount = 0;
            bBogus = false;
            bSimpleField = true;
            sField = "";

            if (iCurLoc > iLen)
            {
                FindNextFieldNameRet = false;
                return FindNextFieldNameRet;
            }

            while (!bDone & iIndex <= iLen)
            {
                sChar = Strings.Mid(sFields, iIndex, 1);

                if (sChar == "'")
                {
                    bInQuote = !bInQuote;
                    sField = sField + sChar;
                    bSimpleField = false;
                }
                else if (sChar == "(")
                {
                    iParCount = iParCount + 1;
                    sField = sField + sChar;
                    bSimpleField = false;
                }
                else if (sChar == ")")
                {
                    iParCount = iParCount - 1;
                    sField = sField + sChar;
                    bSimpleField = false;
                }
                else if (sChar == ",")
                {
                    if (!bInQuote & iParCount == 0)
                    {
                        bDone = true;
                    }
                    else
                    {
                        sField = sField + sChar;
                        bSimpleField = false;
                    }
                }
                else
                {
                    sField = sField + sChar;
                }

                iIndex = iIndex + 1;
            }

            iCurLoc = iIndex;
            if (!bDone)
                bBogus = true;
            sField = Strings.Trim(sField);
            return FindNextFieldNameRet;
        }

        private CreateJoinTables CreateJoinTables(List<Table> oTables, string sJoins)
        {
            var model = new CreateJoinTables();
            bool CreateJoinTablesRet = false;
            sJoins = string.Empty;

            foreach (var oTmpTables in oTables)
            {
                sJoins = sJoins + " LEFT JOIN [" + oTmpTables.TableName + "] ON (TrackingStatus." + oTmpTables.TrackingStatusFieldName + " = [" + oTmpTables.TableName + "].[" + DatabaseMap.RemoveTableNameFromField(oTmpTables.IdFieldName) + "]))";
            }

            if (!string.IsNullOrEmpty(sJoins.Trim()))
            {
                CreateJoinTablesRet = true;
            }

            model.CreateJoinTablesRet = CreateJoinTablesRet;
            model.Tables = oTables;
            model.Joins = sJoins;
            return model;
        }
    }
}
