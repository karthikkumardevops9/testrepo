using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services;
using Newtonsoft.Json;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RetentionController : ControllerBase
    {
        private readonly CommonControllersService<RetentionController> _commonService;
        private static IDbConnection CreateConnection(string connectionString)
         => new SqlConnection(connectionString);

        public RetentionController(CommonControllersService<RetentionController> commonService)
        {
            _commonService = commonService;   
        }

        [Route("GetRetentionCodes")]
        [HttpGet]
        public async Task<object> GetRetentionCodes(string connectionString,string sidx, string sord, int page, int rows)
        {
            object jsonData = new object();
            try
            {
                using(var context = new TABFusionRMSContext(connectionString))
                {

                    var pRetentionCodesEntities =  (from t in await context.SLRetentionCodes.ToListAsync()
                                                         select new { t.SLRetentionCodesId, t.Id, t.Description, t.Notes });


                    jsonData = pRetentionCodesEntities.AsQueryable().GetJsonListForGrid(sord, page, rows, "Id");
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonData;
        }

        [Route("GetCitationCodesByRetenton")]
        [HttpGet]
        public async Task<string> GetCitationCodesByRetenton(string connectionString, string pRetentionCodeId)
        {
            var lstCitationCodes = new List<string>();
            string jsonObject = "";
            try
            {
                using(var context = new TABFusionRMSContext(connectionString))
                {
                    var lstRetentionCodeEntity =  await context.SLRetentionCitaCodes.Where(x => x.SLRetentionCodesId == pRetentionCodeId).ToListAsync();
                    foreach (SLRetentionCitaCode item in lstRetentionCodeEntity)
                        lstCitationCodes.Add(item.SLRetentionCitationsCitation);

                    var pRetentionCodeEntities = from t in await context.SLRetentionCitations.ToListAsync()
                                                 where lstCitationCodes.Contains(t.Citation)
                                                 select new { t.SLRetentionCitationId, t.Citation, t.Subject };

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pRetentionCodeEntities, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonObject;
        }

        [Route("SetRetentionCode")]
        [HttpPost]
        public async Task<ReturnErrorTypeWithMsg> SetRetentionCode(SetRetentionCodeParam param)
        {
            var res = new ReturnErrorTypeWithMsg();
            try
            {
                using(var context = new TABFusionRMSContext(param.passport.ConnectionString))
                {
                    if (param.PRetentionCode.SLRetentionCodesId > 0)
                    {
                        if (await context.SLRetentionCodes.AnyAsync(x => x.Id.Trim().ToLower() == param.PRetentionCode.Id.Trim().ToLower() && x.SLRetentionCodesId != param.PRetentionCode.SLRetentionCodesId) == false)
                        {
                            param.PRetentionCode.RetentionLegalHold = param.PRetentionLegalHold;
                            param.PRetentionCode.RetentionPeriodForceToEndOfYear = param.PRetentionPeriodForceToEndOfYear;
                            param.PRetentionCode.InactivityForceToEndOfYear = param.PInactivityForceToEndOfYear;
                            param.PRetentionCode.RetentionPeriodOther = 0;
                            param.PRetentionCode.InactivityEventType = string.IsNullOrEmpty(param.InactivityEventType) ? "N/A" : param.InactivityEventType;
                            param.PRetentionCode.RetentionEventType = string.IsNullOrEmpty(param.RetentionEventType) ? "N/A" : param.RetentionEventType;

                            var retentionCode = await context.SLRetentionCodes.FirstOrDefaultAsync(x => x.SLRetentionCodesId == param.PRetentionCode.SLRetentionCodesId);

                            var updateRetention = retentionCode;
                            updateRetention.Id = param.PRetentionCode.Id;
                            updateRetention.Description = param.PRetentionCode.Description;
                            updateRetention.DeptOfRecord = param.PRetentionCode.DeptOfRecord;
                            updateRetention.InactivityEventType = param.PRetentionCode.InactivityEventType;
                            updateRetention.InactivityForceToEndOfYear = param.PRetentionCode.InactivityForceToEndOfYear;
                            updateRetention.InactivityPeriod = param.PRetentionCode.InactivityPeriod;
                            updateRetention.Notes = param.PRetentionCode.Notes;
                            updateRetention.RetentionLegalHold = param.PRetentionLegalHold;
                            updateRetention.RetentionPeriodUser = param.PRetentionCode.RetentionPeriodUser;
                            updateRetention.RetentionPeriodTotal = param.PRetentionCode.RetentionPeriodTotal;
                            updateRetention.RetentionPeriodLegal = param.PRetentionCode.RetentionPeriodLegal;
                            updateRetention.RetentionEventType = param.PRetentionCode.RetentionEventType;
                            updateRetention.RetentionPeriodOther = param.PRetentionCode.RetentionPeriodOther;
                            updateRetention.RetentionPeriodForceToEndOfYear = param.PRetentionCode.RetentionPeriodForceToEndOfYear;
                            updateRetention.SLRetentionCodesId = param.PRetentionCode.SLRetentionCodesId;
                            context.Entry(updateRetention).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                            List<Table> pTableEntity = await context.Tables.ToListAsync();

                            if (pTableEntity is not null)
                            {
                                foreach (Table table in pTableEntity)
                                {
                                    if (Convert.ToBoolean(table.RetentionInactivityActive))
                                    {
                                        Retention.UpdateInActiveFlag(table, param.PRetentionCode, param.passport, param.PRetentionPeriodForceToEndOfYear);
                                    }
                                    if (Convert.ToBoolean(table.RetentionPeriodActive) && retentionCode.Id != param.PRetentionCode.Id)
                                    {
                                        var sSQL = "UPDATE [" + table.TableName + "] SET [" + DatabaseMap.RemoveTableNameFromField(table.RetentionFieldName) + "] = '" + param.PRetentionCode.Id + "' WHERE [" + DatabaseMap.RemoveTableNameFromField(table.RetentionFieldName) + "] = '" + retentionCode.Id + "'";
                                        await GetInfoUsingDapper.ProcessADOCommand(param.passport.ConnectionString,sSQL, false);
                                    }

                                    if (param.PRetentionLegalHold)
                                    {
                                        var sSQL = string.Format("DELETE FROM [SLDestructCertItems] WHERE ([TableName] = '{0}' AND [RetentionCode] = '{1}' AND [DispositionDate] IS NULL)", table.TableName, param.PRetentionCode.Id);
                                        await GetInfoUsingDapper.ProcessADOCommand(param.passport.ConnectionString,sSQL, false);
                                    }
                                }
                            }

                            res.JsonRetCodeObj = await GetRetentionCodeId(param.PRetentionCode.Id,param.passport.ConnectionString);

                            res.ErrorType = "s";
                            res.ErrorMessage = "Selected Retention code properties has been updated successfully";
                        }
                        else
                        {
                            res.ErrorType = "w";
                            res.ErrorMessage = "This Retention Code Has already been defined";
                        }
                    }
                    else if(await context.SLRetentionCodes.AnyAsync(x => x.Id.Trim().ToLower() == param.PRetentionCode.Id.Trim().ToLower()) == false)
                    {
                        param.PRetentionCode.RetentionLegalHold = param.PRetentionLegalHold;
                        param.PRetentionCode.RetentionPeriodForceToEndOfYear = param.PRetentionPeriodForceToEndOfYear;
                        param.PRetentionCode.InactivityForceToEndOfYear = param.PInactivityForceToEndOfYear;
                        param.PRetentionCode.RetentionPeriodOther = 0;
                        param.PRetentionCode.InactivityEventType = string.IsNullOrEmpty(param.InactivityEventType) ? "N/A" : param.InactivityEventType;
                        param.PRetentionCode.RetentionEventType = string.IsNullOrEmpty(param.RetentionEventType) ? "N/A" : param.RetentionEventType;

                        context.SLRetentionCodes.Add(param.PRetentionCode);
                        await context.SaveChangesAsync();

                        if (param.PRetentionLegalHold)
                        {
                            List<Table> pTableEntity = await context.Tables.ToListAsync();
                            foreach (Table table in pTableEntity)
                            {  
                                string sSQL = string.Format("DELETE FROM [SLDestructCertItems] WHERE ([TableName] = '{0}' AND [RetentionCode] = '{1}' AND [DispositionDate] IS NULL)", table.TableName, param.PRetentionCode.Id);
                                await GetInfoUsingDapper.ProcessADOCommand(param.passport.ConnectionString, sSQL, false);
                            }
                        }
                        res.JsonRetCodeObj = await GetRetentionCodeId(param.PRetentionCode.Id,param.passport.ConnectionString);
                        res.ErrorType = "s";
                        res.ErrorMessage = "Provided Retention code has been successfully added to the list";
                    }
                    else
                    {
                        res.ErrorType = "w";
                        res.ErrorMessage = "This Retention Code Has already been defined";
                    }
                }

            }
            catch (Exception ex)
            {
                res.ErrorType = "e";
                res.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return res;
        }

        [Route("EditRetentionCode")]
        [HttpGet]
        public async Task<string> EditRetentionCode(string connectionString, string pRetentionCode)
        {
            string jsonObject = string.Empty;
            try
            {
                using(var context = new TABFusionRMSContext(connectionString))
                {
                    var pRetentionCodeEntity = await context.SLRetentionCodes.FirstOrDefaultAsync(x => x.Id == pRetentionCode);

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pRetentionCodeEntity, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex) {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonObject;
        }

        [Route("RemoveRetentionCodeEntity")]
        [HttpGet]
        public async Task<ReturnErrorTypeWithMsg> RemoveRetentionCodeEntity(string connectionString,string pRetentionCode)
        {
            var res = new ReturnErrorTypeWithMsg();
            try
            {
                using(var context = new TABFusionRMSContext(connectionString))
                {
                    var pRetentionCodesEntity = await context.SLRetentionCodes.FirstOrDefaultAsync(x => x.Id == pRetentionCode);

                    context.SLRetentionCodes.Remove(pRetentionCodesEntity);
                    await context.SaveChangesAsync();

                    var pRetentionCitaCodes = await context.SLRetentionCitaCodes.Where(x => x.SLRetentionCodesId.ToString().Trim().ToLower().Equals(pRetentionCode.Trim().ToLower())).ToListAsync();
                    context.SLRetentionCitaCodes.RemoveRange(pRetentionCitaCodes);
                    await context.SaveChangesAsync();
                    res.ErrorType = "s";
                    res.ErrorMessage = "Selected Retention code has been deleted successfully";
                }
            }
            catch (Exception ex) 
            {
                res.ErrorType = "e";
                res.ErrorMessage= "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return res;
        }

        [Route("IsRetentionCodeInUse")]
        [HttpGet]
        public async Task<ReturnErrorTypeWithMsg> IsRetentionCodeInUse(string pRetentionCode,string connectionString)
        {
            var res = new ReturnErrorTypeWithMsg();
            bool bRetCodeUsed = false;
            try
            {
                string SQL = "";
                using(var context = new TABFusionRMSContext(connectionString))
                {
                    var tableList = await context.Tables.ToListAsync();
                    foreach (var oTable in tableList)
                    {
                        if (Convert.ToBoolean(oTable.RetentionPeriodActive))
                        {
                            SQL = "SELECT COUNT(" + DatabaseMap.RemoveTableNameFromField(oTable.IdFieldName) + ") AS TotalCount FROM [" + oTable.TableName + "] WHERE [" + DatabaseMap.RemoveTableNameFromField(oTable.RetentionFieldName) + "] = '" + pRetentionCode + "'";
                            if (oTable.TableName != "Operators")
                            {
                                using (var con = CreateConnection(connectionString))
                                {
                                    var records = await con.ExecuteScalarAsync<int>(SQL);
                                    bRetCodeUsed = records > 0;
                                }
                            }
                        }
                        if (bRetCodeUsed)
                        {
                            res.ErrorType = "e";
                            res.ErrorMessage = "This Retention Code is currently assigned to records and cannot be deleted";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            res.InUse = bRetCodeUsed;
            return res;
        }

        [Route("AssignCitationToRetention")]
        [HttpPost]
        public async Task<ReturnErrorTypeWithMsg> AssignCitationToRetention(AssignCitationToRetentionParam param)
        {
            var res = new ReturnErrorTypeWithMsg();
            try
            {
                using(var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pRetentionCitaCode = new SLRetentionCitaCode();
                    pRetentionCitaCode.SLRetentionCodesId = param.RetentionCodeId;
                    pRetentionCitaCode.SLRetentionCitationsCitation = param.CitationCodeId;
                    context.SLRetentionCitaCodes.Add(pRetentionCitaCode);
                    await context.SaveChangesAsync();
                    res.ErrorType = "s";
                    res.ErrorMessage = "Record saved successfully";
                }
            }
            catch (Exception ex) 
            {
                res.ErrorType = "e";
                res.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return res;
        }

        [Route("GetCitationsCodeToAdd")]
        [HttpGet]
        public async Task<object> GetCitationsCodeToAdd(string connectionString,string sidx, string sord, int page, int rows, string pRetentionCodeId)
        {
            object jsonData;
            try
            {
                var lstCitationIds = new List<string>();

               using(var context = new TABFusionRMSContext(connectionString))
                {
                    var lstRetentionCitaCodes =  await context.SLRetentionCitaCodes.Where(x => x.SLRetentionCodesId == pRetentionCodeId).ToListAsync();

                    foreach (SLRetentionCitaCode item in lstRetentionCitaCodes)
                        lstCitationIds.Add(item.SLRetentionCitationsCitation);

                    var pCitationCodesEntities = from t in await context.SLRetentionCitations.ToListAsync()
                                                 where !lstCitationIds.Contains(t.Citation)
                                                 select new { t.SLRetentionCitationId, t.Citation, t.Subject };
                    jsonData = pCitationCodesEntities.AsQueryable().GetJsonListForGrid(sord, page, rows, "Citation");
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonData;
        }

        [Route("ReplicateCitationForRetentionOnSaveAs")]
        [HttpPost]
        public async Task<ReturnErrorTypeWithMsg> ReplicateCitationForRetentionOnSaveAs(ReplicationCitationParam param)
        {
            var result = new ReturnErrorTypeWithMsg();
            var pNewRetCitaCodeEntity = new SLRetentionCitaCode();
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString)) {
                    var pRetentionCitaCodesEntities = await context.SLRetentionCitaCodes.Where(x => x.SLRetentionCodesId == param.pCopyFromRetCode).ToListAsync();
                    foreach (var pRetCitaEntity in pRetentionCitaCodesEntities)
                    {
                        pNewRetCitaCodeEntity.SLRetentionCitationsCitation = pRetCitaEntity.SLRetentionCitationsCitation;
                        pNewRetCitaCodeEntity.SLRetentionCodesId = param.pCopyToRetCode;
                        context.SLRetentionCitaCodes.Add(pNewRetCitaCodeEntity);
                        await context.SaveChangesAsync();
                    }
                    result.ErrorType = "s";
                    result.ErrorMessage = "Record saved successfully";
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                result.ErrorType = "e";
                result.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }
            return result;
        }

        [Route("GetRetentionYearEndValue")]
        [HttpGet]
        public async Task<RetentionYearEndReturn> GetRetentionYearEndValue(string connectionString)
        {
            DateTime dYearEnd;            
            var res = new RetentionYearEndReturn();
            res.lblRetentionYrEnd = "(Retention Year Ends December 31)";
            try
            {
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    var pSystemEntity = await context.Systems.FirstOrDefaultAsync();
                    if (pSystemEntity.RetentionYearEnd > System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(DateTime.Now))
                    {
                        dYearEnd = new DateTime(DateTime.Now.Year, pSystemEntity.RetentionYearEnd ?? 0 + 1, 0);
                    }
                    else
                    {
                        dYearEnd = new DateTime(DateTime.Now.Year + 1, pSystemEntity.RetentionYearEnd ?? 0 + 1, 0);
                    }
                    string formattedDate = dYearEnd.ToString("MMMM dd");
                    res.lblRetentionYrEnd = string.Format("(Retention Year Ends {0})", formattedDate);
                    res.citaStatus = pSystemEntity.RetentionTurnOffCitations ?? false;
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return res;
        }

        [Route("GetCitationCodes")]
        [HttpGet]
        public async Task<object> GetCitationCodes(string sidx, string sord, int page, int rows,string connectionString)
        {
            object jsonData = new object();
            try
            {
                using (var context = new TABFusionRMSContext(connectionString)) 
                {
                    var pCitationCodesEntities = from t in await context.SLRetentionCitations.ToListAsync()
                                                 select new { t.SLRetentionCitationId, t.Citation, t.Subject };

                    jsonData = pCitationCodesEntities.AsQueryable().GetJsonListForGrid(sord, page, rows, "Citation");
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonData;
        }

        [Route("GetRetentionCodesByCitation")]
        [HttpGet]
        public async Task<string> GetRetentionCodesByCitation(string pCitationCodeId, string connectionString)
        {
            string jsonObject = "";
            try
            {
                var lstRetentionCodes = new List<string>();
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    var pRetentionCitaEntities = from t in await context.SLRetentionCitaCodes.ToListAsync()
                                                 select t.SLRetentionCodesId;

                    var lstCitationCodeEntity = await context.SLRetentionCitaCodes.Where(x => x.SLRetentionCitationsCitation == pCitationCodeId).ToListAsync();
                    
                    foreach (SLRetentionCitaCode item in lstCitationCodeEntity)
                        lstRetentionCodes.Add(item.SLRetentionCodesId);
                    
                    var pRetentionCodesEntities = from t in await context.SLRetentionCodes.ToListAsync()
                                                  where lstRetentionCodes.Contains(t.Id)
                                                  select new { t.SLRetentionCodesId, t.Id, t.Description };

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pRetentionCodesEntities, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }

        [Route("SetCitationCode")]
        [HttpPost]
        public async Task<ReturnErrorTypeWithMsg> SetCitationCode(SetCitationCodeParam param)
        {
            var res = new ReturnErrorTypeWithMsg();
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    if (param.PCitationCode.SLRetentionCitationId > 0)
                    {
                        if (await context.SLRetentionCitations.AnyAsync(x => x.Citation.Trim().ToLower() == param.PCitationCode.Citation.Trim().ToLower() && x.SLRetentionCitationId != param.PCitationCode.SLRetentionCitationId) == false)
                        {
                            context.Entry(param.PCitationCode).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                            res.ErrorType = "s";
                            res.ErrorMessage = "Selected Citation code properties has been updated successfully";
                        }
                        else
                        {
                            res.ErrorType = "w";
                            res.ErrorMessage = "This Citation Code Has already been defined";
                        }
                    }
                    else if (await context.SLRetentionCitations.AnyAsync(x => x.Citation.Trim().ToLower() == param.PCitationCode.Citation.Trim().ToLower()) == false)
                    {
                        context.SLRetentionCitations.Add(param.PCitationCode);
                        await context.SaveChangesAsync();
                        res.ErrorType = "s";
                        res.ErrorMessage = "Provided Citation code has been successfully added to the list";
                    }
                    else
                    {
                        res.ErrorType = "w";
                        res.ErrorMessage = "This Citation Code Has already been defined";

                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return res;
        }

        [Route("EditCitationCode")]
        [HttpGet]
        public async Task<string> EditCitationCode(int pCitationCodeId,string connectionString)
        {
            string jsonObject = "";
            try
            {
                using (var context = new TABFusionRMSContext(connectionString)) 
                {
                    var pRetentionCodeEntity = await context.SLRetentionCitations.FirstOrDefaultAsync(x => x.SLRetentionCitationId == pCitationCodeId);

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pRetentionCodeEntity, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }

        [Route("RemoveAssignedCitationCode")]
        [HttpPost]
        public async Task<ReturnErrorTypeWithMsg> RemoveAssignedCitationCode(AssignCitationToRetentionParam param)
        {
            var result = new ReturnErrorTypeWithMsg();
            try
            {
                using(var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pRetentionCitaCodeEntity = await context.SLRetentionCitaCodes.FirstOrDefaultAsync(x => x.SLRetentionCitationsCitation == param.CitationCodeId & x.SLRetentionCodesId == param.RetentionCodeId);
                    context.SLRetentionCitaCodes.Remove(pRetentionCitaCodeEntity);
                    await context.SaveChangesAsync();
                    result.ErrorType = "s";
                    result.ErrorMessage = "Selected Citation code has been deleted successfully";
                }

            }
            catch (Exception ex) 
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return result;
        }

        [Route("GetCountOfRetentionCodesForCitation")]
        [HttpGet]
        public async Task<int> GetCountOfRetentionCodesForCitation(string pCitationCodeId,string connectionString)
        {
            int retentionCodeCount = 0;
            try
            {
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    var pRetentionCitaCode = await context.SLRetentionCitaCodes.ToListAsync();

                    retentionCodeCount = (from rc in pRetentionCitaCode
                                          where rc.SLRetentionCitationsCitation == pCitationCodeId
                                          select rc.SLRetentionCodesId).Count();
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return retentionCodeCount;

        }

        [Route("RemoveCitationCodeEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeWithMsg> RemoveCitationCodeEntity(RemoveCitationCodeParam param)
        {
            var res = new ReturnErrorTypeWithMsg();
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pCitationCodesEntity = await context.SLRetentionCitations.FirstOrDefaultAsync(x => x.Citation == param.CitationCodeId);
                    context.SLRetentionCitations.Remove(pCitationCodesEntity);
                    
                    var pRetentionCitaCodesEntity = await context.SLRetentionCitaCodes.Where(x => x.SLRetentionCitationsCitation == param.CitationCodeId).ToListAsync();
                    context.SLRetentionCitaCodes.RemoveRange(pRetentionCitaCodesEntity);
                    await context.SaveChangesAsync();
                    res.ErrorType = "s";
                    res.ErrorMessage = "Selected Citation code has been deleted successfully";
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return res;
        }

        [Route("ReplaceRetentionCodeInternal")]
        [HttpPost]  
        public async Task<ReturnErrorTypeWithMsg> ReplaceRetentionCodeInternal(ReplaceRetentionParam param)
        {
            var res = new ReturnErrorTypeWithMsg();
            try
            {
                bool bSuccess = false;
                using (var context = new TABFusionRMSContext(param.Passport.ConnectionString))
                {
                    var pTableEntity = await context.Tables.FirstOrDefaultAsync(m => m.TableId == param.TableId);
                    Retention.UpdateRetentionData(pTableEntity.TableName, param.NewRetentionCode, param.OldRetentionCode, param.Passport);
                    string sSQL = string.Format("UPDATE [{0}] SET [{1}] = '{2}' WHERE [{1}] = '{3}'", pTableEntity.TableName, DatabaseMap.RemoveTableNameFromField(pTableEntity.RetentionFieldName), param.NewRetentionCode, param.OldRetentionCode);
                    if (param.OldRetentionCode.Length == 0)
                        sSQL += string.Format(" OR [{0}] IS NULL", DatabaseMap.RemoveTableNameFromField(pTableEntity.RetentionFieldName));
                    if (!param.updateDisposedRecords)
                        sSQL += " AND ([%slRetentionDispositionStatus] IS NULL OR [%slRetentionDispositionStatus] = 0)";
                    
                    bSuccess = await GetInfoUsingDapper.ProcessADOCommand(param.Passport.ConnectionString, sSQL, false);
                    if (bSuccess)
                    {
                        res.ErrorType = "s";
                        res.ErrorMessage = "New Retention code has been assigned to the records in the Selected Table";
                    }
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return res;
        }

        [Route("GetRetentionTablesList")]
        [HttpPost]
        public async Task<string> GetRetentionTablesList(Passport passport)
        {
            string jsonObject = "";
            try
            {
                var lstRetentionCode = new List<Table>();
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var pTables = await context.Tables.Where(x => x.RetentionPeriodActive == true || x.RetentionInactivityActive == true && !String.IsNullOrEmpty(x.RetentionFieldName)).ToListAsync();
                    foreach (var oTable in pTables)
                    {
                        if (passport.CheckPermission(oTable.TableName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Table, Smead.Security.Permissions.Permission.View))
                        {
                            lstRetentionCode.Add(oTable);
                        }
                    }
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(lstRetentionCode, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }

        [Route("GetRetentionCodeList")]
        [HttpGet]
        public async Task<string> GetRetentionCodeList(string connectionString)
        {
            string jsonObject = "";
            try
            {
                using(var context = new TABFusionRMSContext(connectionString))
                {
                    var pRetentionCodes = await context.SLRetentionCodes.OrderBy(x => x.Id).ToListAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pRetentionCodes, Formatting.Indented, Setting);
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }

        [Route("IsRetentionTurnOffCitations")]
        [HttpGet]
        public async Task<bool> IsRetentionTurnOffCitations(string connectionString)
        {
            bool RetentionTurnOffCitations = false;
            try
            {
                using (var context = new TABFusionRMSContext(connectionString)) 
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    RetentionTurnOffCitations = (bool)pSystemEntity.RetentionTurnOffCitations;
                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return RetentionTurnOffCitations;
        }

        #region Private Methods
        private async Task<string> GetRetentionCodeId(string pRetentionCode,string ConnectionString)
        {
            string jsonObject = "";
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pRetentionCodeEntity =  await context.SLRetentionCodes.FirstOrDefaultAsync(x => x.Id == pRetentionCode);

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    
                    jsonObject = JsonConvert.SerializeObject(pRetentionCodeEntity, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonObject;
        }
        #endregion 
    }
}
