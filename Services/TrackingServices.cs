using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Repository;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services
{
    public class TrackingServices : ITrackingServices
    {
        private const string SL_USERNAME = "@@SL_UserName";

        public const int TRACKING_LOCATION = 1;
        public const int TRACKING_EMPLOYEE = 2;
        private static int miBatchNum = 0;

        public static string msSignatureFile;
        public static string SignatureFile
        {
            get
            {
                return msSignatureFile;
            }
            set
            {
                msSignatureFile = value;
            }
        }

        public static DateTime StartTime
        {
            get
            {
                return msStartTime;
            }
            set
            {
                msStartTime = value;
            }
        }
        public static DateTime msStartTime;

        public static bool TelxonModeOn
        {
            get
            {
                return mbTelxonModeOn;
            }
            set
            {
                mbTelxonModeOn = value;
            }
        }
        public static bool mbTelxonModeOn;

        public static DateTime ScanDateTime
        {
            get
            {
                return mdScanDateTime;
            }
            set
            {
                mdScanDateTime = value;
            }
        }
        public static DateTime mdScanDateTime;

        public static string TelxonUserName
        {
            get
            {
                return msTelxonUserName;
            }
            set
            {
                msTelxonUserName = value;
            }
        }
        public static string msTelxonUserName;

        public static bool FromEXE
        {
            get
            {
                return mbFromEXE;
            }
            set
            {
                mbFromEXE = value;
            }
        }
        public static bool mbFromEXE { get; set; }
        private IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);


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
                    if (Strings.Len(oDestTable.TrackingDueBackDaysFieldName) > 0)
                    {
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
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    objectTable = await context.Tables.Where(m => m.TableName.Trim().ToLower().Equals(trackableType.Trim().ToLower())).FirstOrDefaultAsync();
                    destTable = await context.Tables.Where(m => m.TableName.Trim().ToLower().Equals(destinationType.Trim().ToLower())).FirstOrDefaultAsync();

                    bool IfObjIdFieldIsString = await GetInfoUsingDapper.IdFieldIsString(passport.ConnectionString, objectTable.TableName, objectTable.IdFieldName);
                    if (!IfObjIdFieldIsString)
                    {
                        int oUserLinkTableIdSize = await GetInfoUsingDapper.UserLinkIndexTableIdSize(passport.ConnectionString);
                        oObjectId = Strings.Right(new string('0', oUserLinkTableIdSize) + trackableID, oUserLinkTableIdSize);
                    }
                    else
                    {
                        oObjectId = trackableID;
                    }

                    bool IfDestIdFieldIsString = await GetInfoUsingDapper.IdFieldIsString(passport.ConnectionString, destTable.TableName, destTable.IdFieldName);
                    if (!IfDestIdFieldIsString)
                    {
                        int oUserLinkTableIdSize = await GetInfoUsingDapper.UserLinkIndexTableIdSize(passport.ConnectionString);
                        oDestinationId = Strings.Right(new string('0', oUserLinkTableIdSize) + destinationID, oUserLinkTableIdSize);
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

    }
}
