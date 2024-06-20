using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Repository;
using MSRecordsEngine.Services;
using Newtonsoft.Json;
using Smead.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly CommonControllersService<AdminController> _commonService;
        private IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);
        public AdminController(CommonControllersService<AdminController> commonControllersService)
        {
            _commonService = commonControllersService;
        }

        #region Attachments All methods are moved

        [Route("GetOutputSettingList")]
        [HttpPost]
        public async Task<string> GetOutputSettingList(Passport passport) //completed testing
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var pOutputSettingsList = new List<string>();
                    var pOutputSettingsEntities = await context.SecureObjects.Select(t => new { t.SecureObjectID, t.Name, t.BaseID }).ToListAsync();
                    int pSecureObjectID = pOutputSettingsEntities.FirstOrDefault(x => x.Name.Trim().ToLower().Equals("output settings"))?.SecureObjectID ?? 0;
                    var filteredOutputSettingsEntities = pOutputSettingsEntities.Where(x => x.BaseID == pSecureObjectID).ToList();

                    foreach (var oOutputSettings in filteredOutputSettingsEntities)
                    {
                        var objOutputSetting = await context.OutputSettings.FirstOrDefaultAsync(x => x.Id.ToString().Trim().ToLower().Equals(oOutputSettings.Name.ToString().Trim().ToLower()));
                        if (objOutputSetting is not null)
                        {
                            if (passport.CheckPermission(oOutputSettings.Name, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.OutputSettings, (Permissions.Permission)Enums.PassportPermissions.Access))
                            {
                                pOutputSettingsList.Add(oOutputSettings.Name);
                            }
                        }
                    }
                    var setting = new JsonSerializerSettings();
                    setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

                    jsonObject = JsonConvert.SerializeObject(pOutputSettingsList, Newtonsoft.Json.Formatting.Indented, setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return jsonObject;
        }

        [Route("EditAttachmentSettingsEntity")]
        [HttpGet]
        public async Task<ReturnEditAttachmentSettingsEntity> EditAttachmentSettingsEntity(string ConnectionString) //completed testing
        {
            var model = new ReturnEditAttachmentSettingsEntity();
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    var lSettingsEntities = await context.Settings.ToListAsync();
                    var pSettingsEntityiAccessLocation = lSettingsEntities.Where(x => x.Section.Trim().ToLower().Equals("imageservice") && x.Item.Trim().ToLower().Equals("iaccesslocation")).FirstOrDefault();
                    var pSettingsEntityLocation = lSettingsEntities.Where(x => x.Section.Trim().ToLower().Equals("imageservice") && x.Item.Trim().ToLower().Equals("location")).FirstOrDefault();
                    string DefaultSettingId = "";
                    bool PrintingFooter = false;
                    bool RenameOnScan = false;
                    if (pSystemEntity is not null)
                    {
                        DefaultSettingId = pSystemEntity.DefaultOutputSettingsId;
                        PrintingFooter = (bool)pSystemEntity.PrintImageFooter;
                        RenameOnScan = (bool)pSystemEntity.RenameOnScan;
                    }
                    model.DefaultSettingId = DefaultSettingId;
                    model.PrintingFooter = PrintingFooter;
                    model.RenameOnScan = RenameOnScan;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return model;
        }

        [Route("SetAttachmentSettingsEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetAttachmentSettingsEntity(SetAttachmentSettingsEntityParam attachmentSettingsEntityParam) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pRenameOnScan = attachmentSettingsEntityParam.pRenameOnScan;
            var pDefaultOpSettingsId = attachmentSettingsEntityParam.pDefaultOpSettingsId;
            var pPrintImageFooter = attachmentSettingsEntityParam.pPrintImageFooter;

            try
            {
                using (var context = new TABFusionRMSContext(attachmentSettingsEntityParam.ConnectionString))
                {
                    var pOutputSettings = await context.OutputSettings.Where(x => x.Id.Equals(pDefaultOpSettingsId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

                    if (true is var arg44 && pOutputSettings.InActive is { } arg43 && arg43 == arg44)
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorMessage = "Selected output setting is InActive, Please select another",
                            ErrorType = "W",
                        };
                    }
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();                    var pDefaultOutputSettingName = await context.SecureObjects.Where(x => x.Name.Trim().ToLower().Equals(pDefaultOpSettingsId.Trim().ToLower())).FirstOrDefaultAsync();                    if (pDefaultOutputSettingName is not null)                    {                        pSystemEntity.DefaultOutputSettingsId = pDefaultOutputSettingName.Name;                    }
                    pSystemEntity.RenameOnScan = pRenameOnScan;                    pSystemEntity.PrintImageFooter = pPrintImageFooter;

                    context.Entry(pSystemEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    model.ErrorType = "s";                    model.ErrorMessage = "Image Service Location along with Default Output settings are applied successfully";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return model;
        }

        [Route("SetOutputSettingsEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetOutputSettingsEntity(SetOutputSettingsEntityParams setOutputSettingsEntityParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var passport = setOutputSettingsEntityParams.passport;
            var pOutputSettingEntity = setOutputSettingsEntityParams.outputSetting;
            var DirName = setOutputSettingsEntityParams.DirName;
            var pInActive = setOutputSettingsEntityParams.pInActive;
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {

                    string eMsg = string.Format("The Next Available Document Number must be entered and be between 1 and {0}.", Strings.Format(int.MaxValue, "#,###"));
                    if (pOutputSettingEntity.NextDocNum is not null)
                    {
                        int NextdocNum = Convert.ToInt32(pOutputSettingEntity.NextDocNum);
                        if (NextdocNum <= 0.0d | NextdocNum > (double)int.MaxValue)
                        {
                            return new ReturnErrorTypeErrorMsg
                            {
                                ErrorType = "e",
                                ErrorMessage = eMsg
                            };
                        }
                    }
                    else
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorType = "e",
                            ErrorMessage = eMsg
                        };
                    }

                    var oSecureObjectMain = new Smead.Security.SecureObject(passport);

                    int pSecureObjectID = (await context.SecureObjects.Where(x => x.Name.Trim().ToLower().Equals("output settings")).FirstOrDefaultAsync()).SecureObjectID;
                    if (pOutputSettingEntity.DefaultOutputSettingsId > 0)
                    {
                        int countSecureObject = await context.SecureObjects.Where(x => (x.Name.Trim().ToLower()) == (DirName.Trim().ToLower()) && x.BaseID.Equals(pSecureObjectID)).CountAsync();

                        if (countSecureObject > 1)
                        {
                            return new ReturnErrorTypeErrorMsg
                            {
                                ErrorType = "w",
                                ErrorMessage = string.Format("The record for '{0}' already exists", DirName.ToUpper())
                            };
                        }
                        else if (await context.OutputSettings.AnyAsync(x => (x.FileNamePrefix) == (pOutputSettingEntity.FileNamePrefix) && x.DefaultOutputSettingsId != pOutputSettingEntity.DefaultOutputSettingsId))
                        {
                            return new ReturnErrorTypeErrorMsg
                            {
                                ErrorType = "w",
                                ErrorMessage = string.Format("The Prefix \"{0}\" already exists", pOutputSettingEntity.FileNamePrefix)
                            };
                        }
                        else
                        {
                            if (pOutputSettingEntity.DirectoriesId is null)
                            {
                                pOutputSettingEntity.DirectoriesId = 0;
                            }
                            pOutputSettingEntity.Id = DirName.Trim();
                            if (pInActive == true)
                            {
                                pOutputSettingEntity.InActive = false;
                            }
                            else
                            {
                                pOutputSettingEntity.InActive = true;
                            }
                            context.Entry(pOutputSettingEntity).State = EntityState.Modified;
                        }
                        model.ErrorType = "s";
                        model.ErrorMessage = "Changes has been made successfully to selected Output Settings";
                    }
                    else
                    {
                        if (await context.SecureObjects.AnyAsync(x => (x.Name.Trim().ToLower()) == (DirName.Trim().ToLower()) && x.BaseID.Equals(pSecureObjectID)) == false)
                        {
                            if (await context.OutputSettings.AnyAsync(x => (x.FileNamePrefix) == (pOutputSettingEntity.FileNamePrefix)))
                            {
                                return new ReturnErrorTypeErrorMsg
                                {
                                    ErrorType = "w",
                                    ErrorMessage = string.Format("The Prefix \"{0}\" already exists", pOutputSettingEntity.FileNamePrefix)
                                };
                            }
                            else
                            {

                                oSecureObjectMain.Register(DirName, Smead.Security.SecureObject.SecureObjectType.OutputSettings, (int)Enums.SecureObjects.OutputSettings);

                                int pDirectoriesId = 0;
                                var pDirectoryEntities = await context.Directories.Where(x => x.VolumesId == pOutputSettingEntity.VolumesId).ToListAsync();
                                if (pDirectoryEntities.Count() > 0)
                                {
                                    pDirectoriesId = pDirectoryEntities.FirstOrDefault().Id;
                                }

                                pOutputSettingEntity.DirectoriesId = pDirectoriesId;
                                pOutputSettingEntity.Id = DirName.Trim();
                                if (pInActive == true)
                                {
                                    pOutputSettingEntity.InActive = false;
                                }
                                else
                                {
                                    pOutputSettingEntity.InActive = true;
                                }
                                context.OutputSettings.Add(pOutputSettingEntity);
                            }
                        }
                        else
                        {
                            return new ReturnErrorTypeErrorMsg
                            {
                                ErrorType = "w",
                                ErrorMessage = string.Format("The record for '{0}' already exists", DirName.ToUpper())
                            };
                        }
                        model.ErrorType = "s";
                        model.ErrorMessage = "New Output Settings was added into the default list";
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                }
                _commonService.Logger.LogError($"Error:{dbEx.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }
            return model;
        }

        [Route("EditOutputSettingsEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> EditOutputSettingsEntity(EditRemoveOutputSettingsEntityParams editRemoveOutputSettingsEntityParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pRowSelected = editRemoveOutputSettingsEntityParams.pRowSelected;

            try
            {
                using (var context = new TABFusionRMSContext(editRemoveOutputSettingsEntityParams.ConnectionString))
                {
                    if (pRowSelected is null)
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorType = "e",
                            ErrorMessage = "Null value found"
                        };

                    }
                    if (pRowSelected.Length == 0)
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorType = "e",
                            ErrorMessage = "Null value found"
                        };
                    }
                    string pOutputSettingsId = pRowSelected.GetValue(0).ToString();
                    if (string.IsNullOrWhiteSpace(pOutputSettingsId))
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorType = "e",
                            ErrorMessage = "Null value found"
                        };
                    }

                    var pOutputSettingsEntity = await context.OutputSettings.Where(x => x.Id.ToString().Trim().ToLower().Equals(pOutputSettingsId.Trim().ToLower())).FirstOrDefaultAsync();
                    if (pOutputSettingsEntity != null)
                    {
                        if (false is var arg48 && pOutputSettingsEntity.InActive is { } arg47 && arg47 == arg48)
                        {
                            pOutputSettingsEntity.InActive = true;
                        }
                        else
                        {
                            pOutputSettingsEntity.InActive = false;
                        }
                        var Setting = new JsonSerializerSettings();
                        Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                        model.stringValue1 = JsonConvert.SerializeObject(pOutputSettingsEntity, Newtonsoft.Json.Formatting.Indented, Setting);
                        model.stringValue2 = Convert.ToString(SetExampleFileName(Convert.ToString(pOutputSettingsEntity.NextDocNum.Value), pOutputSettingsEntity.FileNamePrefix, pOutputSettingsEntity.FileExtension));

                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            model.ErrorType = "s";
            return model;
        }

        [Route("RemoveOutputSettingsEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> RemoveOutputSettingsEntity(EditRemoveOutputSettingsEntityParams removeOutputSettingsEntityParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var passport = removeOutputSettingsEntityParams.passport;
            var pRowSelected = removeOutputSettingsEntityParams.pRowSelected;
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    if (pRowSelected is null)
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorType = "e",
                            ErrorMessage = "Null value found"
                        };

                    }
                    if (pRowSelected.Length == 0)
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorType = "e",
                            ErrorMessage = "Null value found"
                        };
                    }

                    int lSecureObjectId;
                    var oSecureObjectMain = new Smead.Security.SecureObject(passport);

                    string pOutputSettingsId = pRowSelected.GetValue(0).ToString();

                    if (string.IsNullOrWhiteSpace(pOutputSettingsId))
                    {
                        return new ReturnErrorTypeErrorMsg
                        {
                            ErrorType = "e",
                            ErrorMessage = "Null value found"
                        };
                    }
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    if (pSystemEntity != null)
                    {
                        var pSecureObjectEntity = await context.SecureObjects.Where(x => x.Name.Trim().ToLower().Equals(pOutputSettingsId.ToString().Trim().ToLower())).FirstOrDefaultAsync();

                        if (pSystemEntity.DefaultOutputSettingsId.Equals(pOutputSettingsId, StringComparison.CurrentCultureIgnoreCase))
                        {
                            model.ErrorType = "w";
                            model.ErrorMessage = string.Format("The {0} Output Settings is in use and cannot be removed", pSecureObjectEntity.Name);
                        }
                        else
                        {
                            var pOutputSettingsEntity = await context.OutputSettings.Where(x => x.Id.Trim().ToLower().Equals(pOutputSettingsId.ToString().Trim().ToLower())).FirstOrDefaultAsync();
                            if (pOutputSettingsEntity is not null)
                            {
                                context.OutputSettings.Remove(pOutputSettingsEntity);
                            }
                            if (pSecureObjectEntity is not null)
                            {
                                lSecureObjectId = oSecureObjectMain.GetSecureObjectID(pSecureObjectEntity.Name, Smead.Security.SecureObject.SecureObjectType.OutputSettings);
                                if (lSecureObjectId != 0)
                                    oSecureObjectMain.UnRegister(lSecureObjectId);
                            }

                            await context.SaveChangesAsync();
                            model.ErrorType = "s";
                            model.ErrorMessage = "Selected Output Settings was removed from the default list";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }
            return model;
        }

        #endregion

        #region Auditing All Methods are moved

        [Route("GetTablesForLabel")]
        [HttpGet]
        public async Task<string> GetTablesForLabel(string ConnectionString) //completed testing 
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var lTableEntites = await (from t in context.Tables
                                               orderby t.TableName
                                               select new
                                               {
                                                   t.AuditUpdate,
                                                   t.UserName,
                                                   t.AuditAttachments,
                                                   t.AuditConfidentialData
                                               }).ToListAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(lTableEntites, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonObject;
        }

        [Route("GetAuditPropertiesData")]
        [HttpPost]
        public async Task<ReturnGetAuditPropertiesData> GetAuditPropertiesData(GetAuditPropertiesDataParams getAuditPropertiesDataParams) //completed testing
        {
            var model = new ReturnGetAuditPropertiesData();
            var pTableId = getAuditPropertiesDataParams.TableId;

            try
            {
                using (var context = new TABFusionRMSContext(getAuditPropertiesDataParams.ConnectionString))
                {
                    var pTableEntites = await context.Tables.Where(x => x.TableId == pTableId).FirstOrDefaultAsync();
                    var pRelationShipEntites = context.RelationShips.Where(x => x.UpperTableName.Trim().ToLower().Equals(pTableEntites.TableName.Trim().ToLower()));
                    model.AuditConfidentialData = pTableEntites.AuditConfidentialData;
                    model.AuditUpdate = pTableEntites.AuditUpdate;
                    model.AuditAttachments = pTableEntites.AuditAttachments;
                    model.confenabled = pRelationShipEntites.Count() > 0 ? false : true;
                    model.attachenabled = (pTableEntites.Attachments == true) ? false : true;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return model;
        }

        [Route("SetAuditPropertiesData")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetAuditPropertiesData(SetAuditPropertiesDataParam setAuditPropertiesDataParam) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pTableId = setAuditPropertiesDataParam.TableId;
            var pAuditConfidentialData = setAuditPropertiesDataParam.AuditConfidentialData;
            var pAuditUpdate = setAuditPropertiesDataParam.AuditUpdate;
            var pAuditAttachments = setAuditPropertiesDataParam.AuditAttachments;
            var pIsChild = setAuditPropertiesDataParam.IsChild;
            var lTableIds = new List<int>();
            try
            {
                using (var context = new TABFusionRMSContext(setAuditPropertiesDataParam.ConnectionString))
                {
                    var pTableEntity = await context.Tables.Where(x => x.TableId.Equals(pTableId)).FirstOrDefaultAsync();
                    pTableEntity.AuditConfidentialData = pAuditConfidentialData;
                    pTableEntity.AuditUpdate = pAuditUpdate;
                    pTableEntity.AuditAttachments = pAuditAttachments;
                    context.Entry(pTableEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    if (pIsChild)
                    {
                        foreach (var pTableIdeach in await (GetChildTableIds(pTableEntity.TableName, setAuditPropertiesDataParam.ConnectionString)))
                        {
                            pTableEntity = await context.Tables.Where(x => x.TableId.Equals(pTableIdeach)).FirstOrDefaultAsync();
                            if (pTableEntity.AuditConfidentialData == false && pTableEntity.AuditUpdate == false && pTableEntity.AuditAttachments == false)
                            {
                                pTableEntity.AuditUpdate = pAuditUpdate;
                                context.Entry(pTableEntity).State = EntityState.Modified;
                                await context.SaveChangesAsync();
                                lTableIds.Add(pTableIdeach);
                            }
                        }
                    }
                    model.ErrorType = "s";
                    model.ErrorMessage = "Selected Audit Properties are applied Successfully";
                    model.intLst = lTableIds;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }

            return model;
        }

        [Route("RemoveTableFromList")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> RemoveTableFromList(RemoveTableFromListParam removeTableFromListParam) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pTableIds = removeTableFromListParam.TableId;

            try
            {
                using (var context = new TABFusionRMSContext(removeTableFromListParam.ConnectionString))
                {
                    foreach (string item in pTableIds)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            int pTableId = Convert.ToInt32(item);
                            var pTableEntity = await context.Tables.Where(x => x.TableId.Equals(pTableId)).FirstOrDefaultAsync();
                            pTableEntity.AuditConfidentialData = false;
                            pTableEntity.AuditUpdate = false;
                            pTableEntity.AuditAttachments = false;
                            context.Entry(pTableEntity).State = EntityState.Modified;
                        }
                    }
                    await context.SaveChangesAsync();
                    model.ErrorType = "s";
                    model.ErrorMessage = "Selected tables from the list are removed from Auditing";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }
            return model;
        }

        [Route("PurgeAuditData")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> PurgeAuditData(PurgeAuditDataParams purgeAuditDataParams) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pPurgeDate = purgeAuditDataParams.PurgeDate;
            var pUpdateData = purgeAuditDataParams.UpdateData;
            var pConfData = purgeAuditDataParams.ConfData;
            var pSuccessLoginData = purgeAuditDataParams.SuccessLoginData;
            var pFailLoginData = purgeAuditDataParams.FailLoginData;

            try
            {
                using (var context = new TABFusionRMSContext(purgeAuditDataParams.ConnectionString))
                {
                    bool bRecordExist = false;

                    if (pUpdateData == true)
                    {
                        var lSLAuditUpdateEntities = await context.SLAuditUpdates.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.UpdateDateTime) < pPurgeDate).ToListAsync();

                        var ids = new HashSet<int>(lSLAuditUpdateEntities.Select(x => x.Id));

                        var lSLAuditUpdChildrenEntities = await context.SLAuditUpdChildrens.Where(x => ids.Contains((int)x.SLAuditUpdatesId)).ToListAsync();
                        if (lSLAuditUpdChildrenEntities != null)
                        {
                            if (lSLAuditUpdChildrenEntities.Count() > 0)
                            {
                                bRecordExist = true;
                                context.SLAuditUpdChildrens.RemoveRange(lSLAuditUpdChildrenEntities);
                                context.SLAuditUpdates.RemoveRange(lSLAuditUpdateEntities);
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    if (pConfData == true)
                    {
                        var lSLAuditConfDataEntities = await context.SLAuditConfDatas.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.AccessDateTime) < pPurgeDate).ToListAsync();
                        if (lSLAuditConfDataEntities != null)
                        {
                            if (lSLAuditConfDataEntities.Count() > 0)
                            {
                                bRecordExist = true;
                                context.SLAuditConfDatas.RemoveRange(lSLAuditConfDataEntities);
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    if (pSuccessLoginData == true)
                    {
                        var lSLAuditLoginEntities = await context.SLAuditLogins.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.LoginDateTime) < pPurgeDate).ToListAsync();
                        if (lSLAuditLoginEntities is not null)
                        {
                            if (lSLAuditLoginEntities.Count() > 0)
                            {
                                bRecordExist = true;
                                context.SLAuditLogins.RemoveRange(lSLAuditLoginEntities);
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    if (pFailLoginData == true)
                    {
                        var lSLAuditFailedLoginEntities = await context.SLAuditFailedLogins.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.LoginDateTime) < pPurgeDate).ToListAsync();
                        if (lSLAuditFailedLoginEntities is not null)
                        {
                            if (lSLAuditFailedLoginEntities.Count() > 0)
                            {
                                bRecordExist = true;
                                context.SLAuditFailedLogins.RemoveRange(lSLAuditFailedLoginEntities);
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    if (bRecordExist == true)
                    {
                        model.ErrorType = "s";
                        model.ErrorMessage = "Selected Audit data has been purged successfully";
                    }
                    else
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = "No Audit data exists to purge based on the selection";
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }

            return model;
        }

        [Route("CheckChildTableExist")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> CheckChildTableExist(CheckChildTableExistParam checkChildTableExistParam)
        {
            var model = new ReturnErrorTypeErrorMsg();
            bool bChildExist = false;
            try
            {
                var pTableId = checkChildTableExistParam.TableId;
                using (var context = new TABFusionRMSContext(checkChildTableExistParam.ConnectionString))
                {
                    var oTable = await context.Tables.Where(x => x.TableId == pTableId).FirstOrDefaultAsync();
                    if (oTable != null)
                    {
                        if ((await GetChildTableIds(oTable.TableName.Trim(), checkChildTableExistParam.ConnectionString)).Count > 0)
                        {   
                            bChildExist = true;
                        }
                    }
                    model.ErrorType = "s";
                    model.ErrorMessage = "Record saved successfully";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }
            model.boolValue = bChildExist;
            return model;
        }

        #endregion

        #region Bar Code Search Order

        [Route("GetBarCodeList")]
        [HttpPost]
        public async Task<string> GetBarCodeList(BarCodeListParams barCodeListParams) //completed testing 
        {
            var page = barCodeListParams.page;
            var sord = barCodeListParams.sord;
            var rows = barCodeListParams.rows;

            var jsonData = string.Empty;
            try
            {

                using (var context = new TABFusionRMSContext(barCodeListParams.ConnectionString))
                {
                    var pBarCideEntities = await context.ScanLists.ToListAsync();
                    var pTable = await context.Tables.ToListAsync();
                    var oBarCodeEntities = new List<ScanList>();

                    foreach (ScanList scan in pBarCideEntities)
                    {
                        if (!string.IsNullOrEmpty(scan.TableName))
                        {
                            oBarCodeEntities.Add(scan);
                        }
                    }

                    var q = (from sc in oBarCodeEntities
                             join ta in pTable.ToList()
                           on sc.TableName.Trim().ToLower() equals ta.TableName.Trim().ToLower()
                             select new
                             {
                                 sc.Id,
                                 sc.IdMask,
                                 sc.IdStripChars,
                                 sc.ScanOrder,
                                 sc.TableName,
                                 sc.FieldName,
                                 sc.FieldType,
                                 ta.UserName
                             }
                    ).AsQueryable();
                    pBarCideEntities = pBarCideEntities.OrderBy(x => x.ScanOrder).ToList();

                    var setting = new JsonSerializerSettings();
                    setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonData = JsonConvert.SerializeObject(q.GetJsonListForGrid(sord, page, rows, "ScanOrder"), Newtonsoft.Json.Formatting.Indented, setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonData;
        }

        [Route("RemoveBarCodeSearchEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> RemoveBarCodeSearchEntity(RemoveBarCodeSearchEntityParams removeBarCodeSearchEntityParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pId = removeBarCodeSearchEntityParams.pId;
            var scan = removeBarCodeSearchEntityParams.scan;

            try
            {
                using (var context = new TABFusionRMSContext(removeBarCodeSearchEntityParams.ConnectionString))
                {
                    var pBarCodeRemovedEntity = await context.ScanLists.Where(x => x.Id == pId).FirstOrDefaultAsync();
                    context.ScanLists.Remove(pBarCodeRemovedEntity);
                    await context.SaveChangesAsync();
                    var pScanListEntityGreater = await context.ScanLists.Where(x => x.ScanOrder > scan).ToListAsync();

                    if (pScanListEntityGreater.Count() == 0 == false)
                    {
                        foreach (ScanList pScanList in pScanListEntityGreater.ToList())
                        {
                            pScanList.ScanOrder = (short?)(pScanList.ScanOrder - 1);

                            context.Entry(pScanList).State = EntityState.Modified;
                        }
                    }
                    await context.SaveChangesAsync();
                    model.ErrorType = "s";
                    model.ErrorMessage = "Selected Barcode Search order deleted successfully";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }

            return model;
        }

        //SetbarCodeSearchEntity uses ADO schema info 

        #endregion

        #region Background Process All Methods moved

        [Route("GetBackgroundProcess")]
        [HttpPost]
        public async Task<string> GetBackgroundProcess(GetBackgroundProcessParams getBackgroundProcessParams) //completed testing 
        {
            var jsonData = string.Empty;
            var oDataTable = new DataTable();
            try
            {
                using (var context = new TABFusionRMSContext(getBackgroundProcessParams.ConnectionString))
                {
                    var oBackGroundOption = await (from p in context.LookupTypes.Where(m => m.LookupTypeForCode.Trim().ToUpper().Equals("BGPCS".Trim()))
                                                   select p.LookupTypeValue).ToListAsync();

                    if (oBackGroundOption.Count > 0)
                    {
                        var oSettingList = await (from s in context.Settings
                                                  where oBackGroundOption.Contains(s.Section)
                                                  select s).ToListAsync();
                        oDataTable.Columns.Add(new DataColumn("Id"));
                        oDataTable.Columns.Add(new DataColumn("Section"));
                        oDataTable.Columns.Add(new DataColumn("MinValue"));
                        oDataTable.Columns.Add(new DataColumn("MaxValue"));

                        if (oSettingList != null)
                        {
                            int oId = 0;
                            DataRow[] foundRows;
                            foreach (var oItem in oSettingList)
                            {
                                if (oDataTable.Rows.Count != 0)
                                {
                                    foundRows = oDataTable.Select("Section = '" + oItem.Section.Trim() + "'");
                                    if (foundRows.Length != 0)
                                    {
                                        foundRows[0][oItem.Item] = oItem.ItemValue;
                                    }
                                    else
                                    {
                                        var dr = oDataTable.NewRow();
                                        oId = oId + 1;
                                        dr["Id"] = oId;
                                        dr["Section"] = oItem.Section;
                                        dr[oItem.Item] = oItem.ItemValue;
                                        oDataTable.Rows.Add(dr);
                                    }
                                }
                                else
                                {
                                    var dr = oDataTable.NewRow();
                                    oId = oId + 1;
                                    dr["Id"] = oId;
                                    dr["Section"] = oItem.Section;
                                    dr[oItem.Item] = oItem.ItemValue;
                                    oDataTable.Rows.Add(dr);
                                }
                            }
                        }
                    }
                    var setting = new JsonSerializerSettings();
                    setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonData = JsonConvert.SerializeObject(ConvertDataTableToJQGridResult(oDataTable, "", getBackgroundProcessParams.sord, getBackgroundProcessParams.page, getBackgroundProcessParams.rows), Newtonsoft.Json.Formatting.Indented, setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonData;
        }

        [Route("GetBackgroundOptions")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> GetBackgroundOptions(Passport passport) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var BackgroundOptionList = await context.LookupTypes.Where(m => m.LookupTypeForCode.Trim().ToUpper().Equals("BGPCS".Trim())).ToListAsync();
                    var lstBackgroundItems = new List<KeyValuePair<string, string>>();
                    foreach (LookupType oitem in BackgroundOptionList)
                        lstBackgroundItems.Add(new KeyValuePair<string, string>(oitem.LookupTypeValue, oitem.LookupTypeValue));
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(lstBackgroundItems, Newtonsoft.Json.Formatting.Indented, Setting);
                    model.ErrorType = "s";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
                model.ErrorType = "e";
            }
            return model;
        }

        [Route("SetBackgroundData")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetBackgroundData(SetBackgroundDataParam setBackgroundDataParam) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            string oId = setBackgroundDataParam.Id;
            string oSection = setBackgroundDataParam.Section;
            int oMinValue = setBackgroundDataParam.MinValue;
            int oMaxValue = setBackgroundDataParam.MaxValue;
            model.ErrorType = "s";
            try
            {
                using (var context = new TABFusionRMSContext(setBackgroundDataParam.ConnectionString))
                {
                    if (oMinValue == 0)                    {                        model.ErrorType = "w";                        model.ErrorMessage = "'MinValue' must be greater than zero";                    }
                    var oMinValueItem = await context.Settings.Where(m => m.Section.Trim().ToLower().Equals(oSection.Trim().ToLower()) & m.Item.Trim().ToLower().Equals("minvalue")).FirstOrDefaultAsync();
                    if (oMinValueItem is not null)                    {                        if (oId.Contains("jqg"))                        {                            model.ErrorType = "w";                            model.ErrorMessage = string.Format("Please update already configured \"{0}\" row", oMinValueItem.Section);                        }                        else                        {                            oMinValueItem.ItemValue = oMinValue.ToString();                            context.Entry(oMinValueItem).State = EntityState.Modified;                        }                    }
                    else                    {                        oMinValueItem = new Setting();                        oMinValueItem.Section = oSection.Trim();                        oMinValueItem.Item = "MinValue";                        oMinValueItem.ItemValue = oMinValue.ToString();                        context.Settings.Add(oMinValueItem);                    }

                    var oMaxValueItem = await context.Settings.Where(m => m.Section.Trim().ToLower().Equals(oSection.Trim().ToLower()) & m.Item.Trim().ToLower().Equals("maxvalue")).FirstOrDefaultAsync();                    if (oMaxValueItem is not null)                    {                        if (oId.Contains("jqg"))                        {                            model.ErrorType = "w";                            model.ErrorMessage = string.Format("Please update already configured \"{0}\" row", oMinValueItem.Section);                        }                        else                        {                            oMaxValueItem.ItemValue = oMaxValue.ToString();                            context.Entry(oMaxValueItem).State = EntityState.Modified;                        }                    }                    else                    {                        oMaxValueItem = new Setting();                        oMaxValueItem.Section = oSection.Trim();                        oMaxValueItem.Item = "MaxValue";                        oMaxValueItem.ItemValue = oMaxValue.ToString();                        context.Settings.Add(oMaxValueItem);                    }
                    if (model.ErrorType == "s")                        model.ErrorMessage = "Provided value/s are modified Successfully";

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                if (ex.Message == "Value was either too large or too small for an Int32.")
                {
                    model.ErrorMessage = "MaxValue/MinValue textboxes does allow only 10 digits";
                }
                else
                {
                    model.ErrorMessage = ex.Message;
                }
            }
            return model;
        }

        [Route("DeleteBackgroundProcessTasks")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> DeleteBackgroundProcessTasks(DeleteBackgroundProcessTasksParams deleteBackgroundProcessTasksParams) //completed testing Delete Query not Working in both  
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pchkBGStatusCompleted = deleteBackgroundProcessTasksParams.CheckkBGStatusCompleted;
            var pchkBGStatusError = deleteBackgroundProcessTasksParams.CheckBGStatusError;
            var pBGEndDate = deleteBackgroundProcessTasksParams.BGEndDate;
            try
            {
                using (var context = new TABFusionRMSContext(deleteBackgroundProcessTasksParams.ConnectionString))
                {
                    var lstOfStatus = new List<string>();
                    if (pchkBGStatusCompleted == true)
                    {
                        lstOfStatus.Add("Completed");
                    }
                    else if (pchkBGStatusError == true)
                    {
                        lstOfStatus.Add("Error");
                    }

                    if (lstOfStatus != null)
                    {
                        string status = "'" + string.Join("','", lstOfStatus) + "'";
                        status = status.Replace("\"", "");
                        string endDate = "'" + pBGEndDate.ToString("yyyy-MM-dd") + "'";

                        using (var conn = CreateConnection(deleteBackgroundProcessTasksParams.ConnectionString))
                        {
                            var dTable = new DataTable();
                            string qsqlpath = "select ReportLocation, DownloadLocation from SLServiceTasks WHERE Convert(Date, StartDate, 101) <= " + endDate + " AND Status IN (" + status + ")";
                            var res = await conn.ExecuteReaderAsync(qsqlpath, commandType: CommandType.Text);
                            if (res != null)
                                dTable.Load(res);
                            foreach (DataRow row in dTable.Rows)
                            {
                                string transferFile = row["ReportLocation"].ToString();
                                if (!string.IsNullOrEmpty(transferFile) && System.IO.File.Exists(transferFile))
                                {
                                    System.IO.File.Delete(transferFile);
                                }
                                string CsvFile = row["DownloadLocation"].ToString();
                                if (!string.IsNullOrEmpty(CsvFile) && System.IO.File.Exists(CsvFile))
                                {
                                    System.IO.File.Delete(CsvFile);
                                }
                            }

                            string sSql = "DELETE From SLServiceTaskItems WHERE SLServiceTaskId In (SELECT Id FROM SLServiceTasks WHERE Convert(Date, EndDate, 101) <= " + endDate + " AND Status IN (" + status + ")); DELETE From SLServiceTasks WHERE Convert(Date, EndDate, 101) <= " + endDate + " AND Status IN (" + status + ")";
                            await conn.ExecuteAsync(sSql, commandType: CommandType.Text);
                        }
                    }
                    model.ErrorType = "s";
                    model.ErrorMessage = "Task Deletion changes are applied Successfully";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }

            return model;
        }

        [Route("RemoveBackgroundSection")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> RemoveBackgroundSection(RemoveBackgroundSectionParams removeBackgroundSectionParams) //testing remaining not able to add background task to test api  
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(removeBackgroundSectionParams.ConnectionString))
                {
                    var SectionArrayObjectDes = JsonConvert.DeserializeObject<object>(removeBackgroundSectionParams.SectionArrayObject);
                    foreach (string oStr in (IEnumerable)SectionArrayObjectDes)
                    {
                        var oSetting = context.Settings.Where(m => m.Section.Trim().ToLower().Equals(oStr.Trim().ToLower()));
                        context.Settings.RemoveRange(oSetting);
                    }
                    await context.SaveChangesAsync();
                    model.ErrorType = "s";
                    model.ErrorMessage = "Selected section deleted successfully";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = ex.Message;
            }

            return model;
        }

        #endregion

        #region TABQUIK

        [Route("GetTabquikKey")]
        [HttpGet]
        public async Task<string> GetTabquikKey(string ConnectionString) //completed testing
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var tabquikkey = await context.Settings.Where(s => s.Item.Equals("Key") & s.Section.Equals("TABQUIK")).FirstOrDefaultAsync();
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(tabquikkey, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonObject;
        }

        #endregion

        #region Requestor All Wrking Methods moved

        [Route("RemoveRequestorEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> RemoveRequestorEntity(RemoveRequestorEntityParam removeRequestorEntityParam) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var statusVar = removeRequestorEntityParam.RequestStatus;

            try
            {
                using (var context = new TABFusionRMSContext(removeRequestorEntityParam.ConnectionString))
                {
                    var pSLRequestorEntity = await context.SLRequestors.Where(m => m.Status.Trim().ToLower().Equals(statusVar.Trim().ToLower())).ToListAsync();
                    if (pSLRequestorEntity.Count() != 0)
                    {
                        context.SLRequestors.RemoveRange(pSLRequestorEntity);
                        model.ErrorType = "s";
                        model.ErrorMessage = "Selected Citation code has been deleted successfully";
                    }
                    else
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = "No Data to purge";
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }

            return model;
        }

        //ResetRequestorLabel Remaining not able to call this api to test

        [Route("SetRequestorSystemEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetRequestorSystemEntity(SetRequestorSystemEntityParams setRequestorSystemEntityParams) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(setRequestorSystemEntityParams.ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    pSystemEntity.AllowWaitList = setRequestorSystemEntityParams.AllowList;
                    pSystemEntity.PopupWaitList = setRequestorSystemEntityParams.PopupList;
                    context.Entry(pSystemEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                model.ErrorMessage = "Record saved successfully";                model.ErrorType = "s";
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }

            return model;
        }

        [Route("GetRequestorSystemEntity")]
        [HttpGet]
        public async Task<string> GetRequestorSystemEntity(string ConnectionString) //completed testing
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pSystemEntity, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return jsonObject;
        }

        #endregion

        #region Tracking

        [Route("GetTrackingSystemEntity")]
        [HttpGet]
        public async Task<string> GetTrackingSystemEntity(string ConnectionString) //completed testing 
        {
            var result = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(m => m.Id).FirstOrDefaultAsync();

                    var setting = new JsonSerializerSettings();
                    setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    result = JsonConvert.SerializeObject(pSystemEntity, Newtonsoft.Json.Formatting.Indented, setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("SetTrackingHistoryData")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetTrackingHistoryData(SetTrackingHistoryDataParams setTrackingHistoryDataParams)
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(setTrackingHistoryDataParams.ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    if (setTrackingHistoryDataParams.MaxHistoryDays < 0)
                    {
                        pSystemEntity.MaxHistoryDays = 0;
                    }
                    else
                    {
                        pSystemEntity.MaxHistoryDays = setTrackingHistoryDataParams.MaxHistoryDays;
                    }
                    if (setTrackingHistoryDataParams.MaxHistoryItems < 0)
                    {
                        pSystemEntity.MaxHistoryItems = 0;
                    }
                    else
                    {
                        pSystemEntity.MaxHistoryItems = setTrackingHistoryDataParams.MaxHistoryItems;
                    }
                    context.Entry(pSystemEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    bool catchFlag;

                    string KeysType = "";

                    var res = await InnerTruncateTrackingHistory(setTrackingHistoryDataParams.ConnectionString, "", "");
                    catchFlag = res.Success;
                    KeysType = res.KeysType;
                    if (catchFlag == true)
                    {
                        if (KeysType == "s")
                        {
                            model.ErrorType = "s";
                            model.ErrorMessage = "History has been truncated";
                        }
                        else
                        {
                            model.ErrorType = "w";
                            model.ErrorMessage = "No more history to truncate";
                        }
                    }
                    else
                    {
                        model.ErrorType = "e";
                        model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }

            return model;
        }

        #endregion

        #region Retention

        [Route("GetRetentionPeriodTablesList")]
        [HttpGet]
        public async Task<ReturnRetentionPeriodTablesList> GetRetentionPeriodTablesList(string ConnectionString) //completed testing
        {
            var model = new ReturnRetentionPeriodTablesList();
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var lTableEntites = await (from t in context.Tables.OrderBy(m => m.TableName)
                                               select new { t.TableName, t.UserName, t.RetentionPeriodActive, t.RetentionInactivityActive }).ToListAsync();

                    var lSystem = await (from x in context.Systems
                                         select new { x.RetentionTurnOffCitations, x.RetentionYearEnd }).ToListAsync();

                    var lSLServiceTasks = await (from y in context.SLServiceTasks
                                                 select new { y.Type, y.Interval }).ToListAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.jsonObject = JsonConvert.SerializeObject(lTableEntites, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.systemJsonObject = JsonConvert.SerializeObject(lSystem, Newtonsoft.Json.Formatting.Indented, Setting);
                    model.serviceJsonObject = JsonConvert.SerializeObject(lSLServiceTasks, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return model;
        }

        #endregion


        [Route("BindAccordian")]
        [HttpPost]
        public async Task<string> BindAccordian(Passport passport) //completed testing 
        {
            var lstDataTbl = new Dictionary<string, string>();
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var pTablesEntities = await context.vwTablesAlls.ToListAsync();
                    foreach (vwTablesAll table in pTablesEntities)
                    {
                        if (CollectionsClass.IsEngineTable(table.TABLE_NAME))
                        {
                            table.UserName += "*";
                            lstDataTbl.Add(table.TABLE_NAME, table.UserName);
                        }
                        else if (passport.CheckPermission(table.TABLE_NAME, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                        {
                            lstDataTbl.Add(table.TABLE_NAME, table.UserName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }

            var Setting = new JsonSerializerSettings();
            Setting.PreserveReferencesHandling = PreserveReferencesHandling.None;
            string jsonObject = JsonConvert.SerializeObject(lstDataTbl, Newtonsoft.Json.Formatting.Indented, Setting);

            return jsonObject;
        }

        [Route("LoadAccordianTable")]
        [HttpPost]
        public async Task<string> LoadAccordianTable(Passport passport) //completed testing 
        {
            var pTablesList = new List<Table>();
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var pTablesEntities = await context.Tables.Where(m => (m.TableName.Trim().ToLower()) != ("Operators".Trim().ToLower())).OrderBy(m => m.TableName).ToListAsync();
                    var lAllTables = await context.vwTablesAlls.Select(x => x.TABLE_NAME).ToListAsync();

                    foreach (var oTable in pTablesEntities)
                    {
                        if (passport.CheckPermission(oTable.TableName, (Smead.Security.SecureObject.SecureObjectType)Models.Enums.SecureObjects.Table, (Permissions.Permission)Models.Enums.PassportPermissions.Configure))
                        {
                            pTablesList.Add(oTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }

            var Setting = new JsonSerializerSettings();
            Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            string jsonObject = JsonConvert.SerializeObject(pTablesList.OrderBy(m => m.UserName), Newtonsoft.Json.Formatting.Indented, Setting);
            return jsonObject;

        }

        [Route("GetReportInformation")]
        [HttpPost]
        public async Task<ReturnReportInfo> GetReportInformation(GetReportInformationParams getReportInformationParams) //completed testing 
        {
            var passport = getReportInformationParams.passport;
            var pReportID = getReportInformationParams.pReportID;
            var bIsAdd = getReportInformationParams.bIsAdd;

            var lstTblNames = new List<KeyValuePair<string, string>>();
            var lstReportStyles = new List<KeyValuePair<string, string>>();
            Table oTable;
            string lstTblNamesList = "";
            string lstReportStylesList = "";
            string lstChildTablesObjStr = "";
            var lstChildTables = new List<KeyValuePair<string, string>>();
            int lNextReport = 0;
            string sReportName = "";
            bool bFound;


            var lstRelatedChildTable = new List<RelationShip>();
            string tblName = "";
            string sReportStyleId = "";
            using (var context = new TABFusionRMSContext(passport.ConnectionString))
            {
                var tableEntity = await context.Tables.OrderBy(x => x.TableName).ToListAsync();
                var reportStyleEntity = await context.ReportStyles.ToListAsync();

                int subViewId2 = 0;
                int subViewId3 = 0;

                var pViewEntity = await context.Views.Where(x => x.Id == pReportID).FirstOrDefaultAsync();
                if (!(pViewEntity == null))
                {
                    if (pViewEntity.SubViewId > 0 && pViewEntity.SubViewId != 9999)
                    {
                        subViewId2 = (int)pViewEntity.SubViewId;
                    }
                }

                if (subViewId2 > 0 & subViewId2 != 9999)
                {
                    var pSubViewEntity = await context.Views.Where(x => x.Id == subViewId2).FirstOrDefaultAsync();
                    if (!(pSubViewEntity == null))
                    {
                        subViewId3 = (int)pSubViewEntity.SubViewId;
                    }
                }
                try
                {
                    foreach (var currentOTable in tableEntity)
                    {
                        oTable = currentOTable;
                        if (!CollectionsClass.IsEngineTable(oTable.TableName) & passport.CheckPermission(oTable.TableName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Table, (Permissions.Permission)Enums.PassportPermissions.View))
                        {
                            lstTblNames.Add(new KeyValuePair<string, string>(oTable.TableName, oTable.UserName));
                        }
                    }
                    foreach (var oReportStyle in reportStyleEntity)
                        lstReportStyles.Add(new KeyValuePair<string, string>(oReportStyle.ReportStylesId.ToString(), oReportStyle.Id));

                    if (pViewEntity != null)
                    {
                        lstRelatedChildTable = await context.RelationShips.Where(x => (x.UpperTableName) == (pViewEntity.TableName)).ToListAsync();
                        sReportName = pViewEntity.ViewName;
                        tblName = pViewEntity.TableName;
                        sReportStyleId = pViewEntity.ReportStylesId;
                    }

                    foreach (var lTableName in lstRelatedChildTable)
                    {
                        oTable = await context.Tables.Where(x => x.UserName.Equals(lTableName.LowerTableName)).FirstOrDefaultAsync();

                        if (oTable is not null)
                        {
                            lstChildTables.Add(new KeyValuePair<string, string>(oTable.TableName, oTable.UserName));
                            oTable = null;
                        }
                    }

                    if (Convert.ToBoolean(bIsAdd))
                    {
                        do
                        {
                            bFound = false;

                            if (lNextReport == 0)
                            {
                                sReportName = "New Report";
                            }
                            else
                            {
                                sReportName = "New Report " + lNextReport;
                            }

                            foreach (var oView in context.Views.ToList())
                            {
                                if (Strings.StrComp(oView.ViewName, sReportName, Constants.vbTextCompare) == 0)
                                {
                                    lNextReport = lNextReport + 1;
                                    bFound = true;
                                }
                            }
                            if (!bFound)
                            {
                                break;
                            }
                        }
                        while (true);
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

                    lstTblNamesList = JsonConvert.SerializeObject(lstTblNames, Formatting.Indented, Setting);
                    lstReportStylesList = JsonConvert.SerializeObject(lstReportStyles, Formatting.Indented, Setting);
                    lstChildTablesObjStr = JsonConvert.SerializeObject(lstChildTables, Formatting.Indented, Setting);

                }
                catch (Exception ex)
                {
                    _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
                }
                var retrunData = new ReturnReportInfo();
                retrunData.lstTblNamesList = lstTblNamesList;
                retrunData.lstReportStylesList = lstReportStylesList;
                retrunData.lstChildTablesObjStr = lstChildTablesObjStr;
                retrunData.sReportName = sReportName;
                retrunData.tblName = tblName;
                retrunData.sReportStyleId = sReportStyleId;
                retrunData.subViewId2 = subViewId2;
                retrunData.subViewId3 = subViewId3;

                return retrunData;
            }
        }


        #region Not Working 

        [Route("GetTablesView")]
        [HttpPost]
        public async Task<ReturnGetTablesView> GetTablesView(GetTablesViewParams getTablesViewParams) //not working   
        {
            var model = new ReturnGetTablesView();
            var lstViews = new List<KeyValuePair<string, string>>();
            var lstChildTables = new List<KeyValuePair<string, string>>();
            Table oTable;
            var pTableName = getTablesViewParams.pTableName;
            var passport = getTablesViewParams.passport;
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var tableEntity = await context.Tables.OrderBy(x => x.TableName).ToListAsync();
                    var lViewColumnEntities = await context.ViewColumns.ToListAsync();
                    var lLoopViewEntities = await context.Views.Where(x => (x.TableName.Trim().ToLower()) == (pTableName.ToLower())).ToListAsync();

                    foreach (var oView in lLoopViewEntities)
                    {
                        if (await (NotSubReport(oView, pTableName, passport)))
                        {
                            if (passport.CheckPermission(oView.ViewName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Reports, (Permissions.Permission)Enums.PassportPermissions.View) | passport.CheckPermission(oView.ViewName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.View, (Permissions.Permission)Enums.PassportPermissions.View))
                            {
                                lstViews.Add(new KeyValuePair<string, string>(oView.Id.ToString(), oView.ViewName));
                            }
                        }
                    }

                    var lstRelatedChildTable = await context.RelationShips.Where(x => (x.UpperTableName) == (pTableName)).ToListAsync();

                    foreach (var lTableName in lstRelatedChildTable)
                    {
                        oTable = await context.Tables.Where(x => x.TableName.Equals(lTableName.LowerTableName)).FirstOrDefaultAsync();

                        if (oTable != null)
                        {
                            lstChildTables.Add(new KeyValuePair<string, string>(oTable.TableName, oTable.UserName));
                            oTable = null;
                        }
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.lstViewStr = JsonConvert.SerializeObject(lstViews, Newtonsoft.Json.Formatting.Indented, Setting);
                    model.lstChildTablesObjStr = JsonConvert.SerializeObject(lstChildTables, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }

            return model;
        }

        

        #endregion


        #region Private Method

        private async Task<bool> NotSubReport(View oView, string pTableName, Passport passport)
        {
            bool NotSubReportRet = default;
            object lLoopViewEntities = null;
            View oTempView;

            using (var context = new TABFusionRMSContext(passport.ConnectionString))
            {
                var tableEntity = await context.Tables.ToListAsync();
                foreach (var oTable in tableEntity)
                {
                    lLoopViewEntities = await context.Views.Where(x => (x.TableName.Trim().ToLower()) == (oTable.TableName)).ToListAsync();

                    if (!(lLoopViewEntities == null))
                    {
                        foreach (View currentOTempView in (IEnumerable)lLoopViewEntities)
                        {
                            oTempView = currentOTempView;
                            if (oTempView.SubViewId == oView.Id)
                            {
                                NotSubReportRet = false;
                                break;
                            }
                        }
                    }
                }
                oTempView = null;
                return NotSubReportRet;
            }
        }

        private object SetExampleFileName(string pNextDocNum, string pFileNamePrefix, string pFileExtension)
        {
            string sBase36;
            if (Convert.ToDouble(pNextDocNum) >= 0.0d & Convert.ToDouble(pNextDocNum) <= int.MaxValue)
            {
                sBase36 = Convert10to36(Convert.ToDouble(pNextDocNum));
            }
            else
            {
                sBase36 = "";
            }
            return Strings.Trim(pFileNamePrefix) + Strings.Trim(sBase36) + "." + Strings.Trim(pFileExtension.TrimStart('.'));
        }

        private string Convert10to36(double dValue)
        {
            string Convert10to36Ret = default;
            double dRemainder;
            string sResult;
            sResult = "";
            dValue = Math.Abs(dValue);
            do
            {
                dRemainder = dValue - 36d * Conversion.Int(dValue / 36d);
                sResult = Strings.Mid("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", (int)Math.Round(dRemainder + 1d), 1) + sResult;
                dValue = Conversion.Int(dValue / 36d);
            }
            while (dValue > 0d);
            // Convert10to36 = InStr(6 - Len(sResult), "0") & sResult
            Convert10to36Ret = sResult.PadLeft(6, '0');
            return Convert10to36Ret;
        }

        private async Task<List<int>> GetChildTableIds(string pTableName, string connectionString)
        {
            var lTableIds = new List<int>();
            var query = "SELECT TableId FROM dbo.FNGetChildTables('" + pTableName + "');";

            using (var conn = CreateConnection(connectionString))
            {
                lTableIds = (await conn.QueryAsync<int>(query, commandType: CommandType.Text)).ToList();
            }

            return lTableIds;
        }

        private object ConvertDataTableToJQGridResult(DataTable dtRecords, string sidx, string sord, int page, int rows)
        {
            var totalRecords = default(int);
            var totalPages = default(int);
            int pageIndex;
            object lFinalResult = null;

            try
            {
                totalRecords = dtRecords.Rows.Count;
                totalPages = (int)Math.Round(Math.Truncate(Math.Ceiling(totalRecords / (float)rows)));
                pageIndex = Convert.ToInt32(page) - 1;
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

                lFinalResult = objListOfEmployeeEntity.Skip(pageIndex * rows).Take(rows);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            object jsonData = new { total = totalPages, page, records = totalRecords, rows = lFinalResult };
            return jsonData;
        }

        private async Task<HelperTrackingHistory> InnerTruncateTrackingHistory(string ConnectionString, string sTableName, [Optional, DefaultParameterValue("")] string sId)
        {
            var returnData = new HelperTrackingHistory();
            returnData.Success = true;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pSystem = await context.Systems.OrderBy(m => m.Id).FirstOrDefaultAsync();
                    if ((bool)(0 is var arg18 && pSystem.MaxHistoryDays is { } arg17 ? arg17 > arg18 : (bool?)null))
                    {
                        var dMaxDate = DateTime.FromOADate((double)(DateTime.Now.ToOADate() - pSystem.MaxHistoryDays - 1));
                        var dUTC = dMaxDate.ToUniversalTime();
                        if (!string.IsNullOrEmpty(sTableName))
                        {
                            var pTrackingHistory = await context.TrackingHistories.Where(m => m.TransactionDateTime < dUTC && (m.TrackedTable.Trim().ToLower().Equals(sTableName.Trim().ToLower()) && m.TrackedTableId.Trim().ToLower().Equals(sId.Trim().ToLower()))).ToListAsync();
                            if (pTrackingHistory.Count() == 0)
                            {
                                returnData.KeysType = "w";
                            }
                            else
                            {
                                context.TrackingHistories.RemoveRange(pTrackingHistory);
                                await context.SaveChangesAsync();
                                returnData.KeysType = "s";
                            }
                        }
                        else
                        {
                            var pTrackingHistory = await context.TrackingHistories.Where(m => m.TransactionDateTime < dUTC).Take(100).ToListAsync();

                            if (pTrackingHistory.Count() != 0)
                            {
                                context.TrackingHistories.RemoveRange(pTrackingHistory);
                                await context.SaveChangesAsync();
                                returnData.KeysType = "s";
                            }
                            else
                            {
                                returnData.KeysType = "w";
                            }
                        }
                    }

                    if ((bool)(0 is var arg26 && pSystem.MaxHistoryItems is { } arg25 ? arg25 > arg26 : (bool?)null))
                    {
                        if (string.IsNullOrEmpty(sTableName))
                        {
                            var trackHistory = await context.TrackingHistories.ToListAsync();
                            var sSQL = (from tq in trackHistory
                                        group tq by new { tq.TrackedTable, tq.TrackedTableId } into tGroup
                                        let groupName = tGroup.Key
                                        let TableIdCount = tGroup.Count()
                                        orderby groupName.TrackedTableId, groupName.TrackedTable descending
                                        select new { TableIdCount, groupName.TrackedTable, groupName.TrackedTableId }).ToList();
                            if (sSQL != null)
                            {
                                foreach (var Tracking in sSQL)
                                {
                                    if ((bool)(Tracking.TableIdCount is var arg27 && pSystem.MaxHistoryItems is { } arg28 ? arg27 < arg28 : (bool?)null))
                                    {
                                    }
                                    else
                                    {
                                        var res = await DeleteExtraTrackingHistory(ConnectionString, Tracking.TrackedTable, Tracking.TrackedTableId);
                                        returnData.Success = res.Success;
                                        returnData.KeysType = res.KeysType;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var res = await DeleteExtraTrackingHistory(ConnectionString ,sTableName, sId);
                            returnData.Success = res.Success;
                            returnData.KeysType = res.KeysType;
                        }
                    }
                }

                
                return returnData;
            }
            catch (Exception)
            {
                returnData.Success = false;
                return returnData;
            }
        }

        private async Task<HelperTrackingHistory> DeleteExtraTrackingHistory(string ConnectionString, string sTableName, string sId)
        {
            var returnData = new HelperTrackingHistory();
            try
            {
                using (var m = new TABFusionRMSContext(ConnectionString))
                {
                    var pSystem = await m.Systems.OrderBy(m => m.Id).FirstOrDefaultAsync();
                    int pSystem1 = Convert.ToInt32(pSystem.MaxHistoryItems);

                    var sSqlExtra = from tMain in m.TrackingHistories
                                    where (tMain.TrackedTable ?? "") == (sTableName.Trim() ?? "")
                                            & (tMain.TrackedTableId.Trim().ToLower() ?? "") == (sId.Trim().ToLower() ?? "")
                                            && !(from tSub in m.TrackingHistories
                                                 where (tSub.TrackedTable.Trim().ToLower() ?? "") == (sTableName.Trim().ToLower() ?? "")
                                                 & (tSub.TrackedTableId.Trim().ToLower() ?? "") == (sId.Trim().ToLower() ?? "")
                                                 orderby tSub.TransactionDateTime descending
                                                 select tSub.Id)
                                                 .Take(pSystem1)
                                                 .Contains(tMain.Id)
                                    select tMain;

                    for (int index = 1; index <= 2; index++)
                    {
                        var sSqlTotal = (from tMain in m.TrackingHistories
                                         where (tMain.TrackedTable ?? "") == (sTableName ?? "") && (tMain.TrackedTableId ?? "") == (sId ?? "")
                                         group tMain by new { tMain.TrackedTableId, tMain.TrackedTable } into tGroup
                                         let groupName = tGroup.Key
                                         let TotalCount = tGroup.Count()
                                         select new { TotalCount }).ToList();

                        if (sSqlTotal != null)
                        {
                            if (!(sSqlTotal.Count == 0))
                            {
                                foreach (var totalVar in sSqlTotal)
                                {
                                    if ((bool)(totalVar.TotalCount is var arg15 && pSystem.MaxHistoryItems is { } arg16 ? arg15 >= arg16 : (bool?)null))
                                    {
                                        m.TrackingHistories.RemoveRange(sSqlExtra);
                                    }
                                }
                                returnData.KeysType = "s";
                            }
                            else
                            {
                                returnData.KeysType = "w";
                            }
                        }
                    }
                    await m.SaveChangesAsync();
                    returnData.Success = true;
                    return returnData;
                }

                
            }
            catch (Exception)
            {
                returnData.Success = false;
                return returnData;
            }
        }

        #endregion
    }
}
