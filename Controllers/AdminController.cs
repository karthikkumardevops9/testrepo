using Dapper;
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
        public async Task<ReturnErrorTypeErrorMsg> CheckChildTableExist(CheckChildTableExistParam checkChildTableExistParam) //completed testing 
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

        #region Requestor All Methods moved 

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

        [Route("ResetRequestorLabel")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> ResetRequestorLabel(ResetRequestorLabelParam resetRequestorLabelParam) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(resetRequestorLabelParam.ConnectionString))
                {
                    var pOneStripJob = await context.OneStripJobs.Where(m => m.TableName.Trim().ToLower().Equals(resetRequestorLabelParam.TableName.Trim().ToLower())).FirstOrDefaultAsync();

                    if (pOneStripJob == null)
                    {
                        var rStripJob = new OneStripJob();
                        rStripJob.Name = "Requestor Default Label";
                        rStripJob.Inprint = (short?)0;
                        rStripJob.TableName = "SLRequestor";
                        rStripJob.OneStripFormsId = 101;
                        rStripJob.UserUnits = (short?)0;
                        rStripJob.LabelWidth = 5040;
                        rStripJob.LabelHeight = 1620;
                        rStripJob.DrawLabels = false;
                        rStripJob.LastCounter = 0;
                        rStripJob.SQLString = "SELECT * FROM [SLRequestor] WHERE [Id] = %ID%";
                        rStripJob.SQLUpdateString = "";
                        rStripJob.LSAfterPrinting = "";
                        context.OneStripJobs.Add(rStripJob);
                        await context.SaveChangesAsync();
                        model.ErrorType = "s";
                        model.ErrorMessage = "Record is added successfully";
                    }
                    else
                    {
                        pOneStripJob.Name = "Requestor Default Label";
                        pOneStripJob.Inprint = (short?)0;
                        pOneStripJob.TableName = "SLRequestor";
                        pOneStripJob.OneStripFormsId = 101;
                        pOneStripJob.UserUnits = (short?)0;
                        pOneStripJob.LabelWidth = 5040;
                        pOneStripJob.LabelHeight = 1620;
                        pOneStripJob.DrawLabels = false;
                        pOneStripJob.LastCounter = 0;
                        pOneStripJob.SQLString = "SELECT * FROM [SLRequestor] WHERE [Id] = %ID%";
                        pOneStripJob.SQLUpdateString = "";
                        pOneStripJob.LSAfterPrinting = "";
                        context.Entry(pOneStripJob).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                        model.ErrorType = "s";
                        model.ErrorMessage = "Action made on Email Notifications are applied Successfully";
                    }

                    var pOneStripJobId = await context.OneStripJobs.Where(m => m.TableName.Trim().ToLower().Equals(resetRequestorLabelParam.TableName.Trim().ToLower())).FirstOrDefaultAsync();
                    if (pOneStripJob == null)
                    {
                        model.ErrorType = "e";
                        model.ErrorMessage = "There is no record is exist for Default Requestor label";
                    }
                    else
                    {
                        var param = new DynamicParameters();
                        param.Add("@JobsId", pOneStripJobId.Id);
                        using (var conn = CreateConnection(resetRequestorLabelParam.ConnectionString))
                        {
                            await conn.ExecuteAsync("SP_RMS_AddRequestorJobFields", param, commandType: CommandType.StoredProcedure);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }
            return model;
        }

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
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetBackgroundOptions(string ConnectionString) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
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
                _commonService.Logger.LogError($"Error:{ex.Message}");
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
        public async Task<ReturnErrorTypeErrorMsg> DeleteBackgroundProcessTasks(DeleteBackgroundProcessTasksParams deleteBackgroundProcessTasksParams) //completed testing  
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

        #region Tracking All Methods moved

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
        public async Task<ReturnErrorTypeErrorMsg> SetTrackingHistoryData(SetTrackingHistoryDataParams setTrackingHistoryDataParams)  //completed testing
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

        [Route("SetTrackingSystemEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetTrackingSystemEntity(SetTrackingSystemEntityParam setTrackingSystemEntityParams)  //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(setTrackingSystemEntityParams.ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    pSystemEntity.TrackingOutOn = setTrackingSystemEntityParams.TrackingOutOn;
                    pSystemEntity.DateDueOn = setTrackingSystemEntityParams.DateDueOn;
                    pSystemEntity.TrackingAdditionalField1Desc = setTrackingSystemEntityParams.TrackingAdditionalField1Desc;
                    pSystemEntity.TrackingAdditionalField2Desc = setTrackingSystemEntityParams.TrackingAdditionalField2Desc;
                    pSystemEntity.TrackingAdditionalField1Type = setTrackingSystemEntityParams.TrackingAdditionalField1Type;

                    if (setTrackingSystemEntityParams.SystemTrackingMaxHistoryDays <= 0)
                    {
                        pSystemEntity.MaxHistoryDays = 0;
                    }
                    else
                    {
                        pSystemEntity.MaxHistoryDays = setTrackingSystemEntityParams.SystemTrackingMaxHistoryDays;
                    }
                    if (setTrackingSystemEntityParams.SystemTrackingMaxHistoryItems <= 0)
                    {
                        pSystemEntity.MaxHistoryItems = 0;
                    }
                    else
                    {
                        pSystemEntity.MaxHistoryItems = setTrackingSystemEntityParams.SystemTrackingMaxHistoryItems;
                    }
                    if (setTrackingSystemEntityParams.SystemTrackingDefaultDueBackDays == 0)
                    {
                        pSystemEntity.DefaultDueBackDays = (short?)1;
                    }
                    else
                    {
                        pSystemEntity.DefaultDueBackDays = setTrackingSystemEntityParams.SystemTrackingDefaultDueBackDays;
                    }
                    context.Entry(pSystemEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    model.ErrorType = "s";
                    model.ErrorMessage = "Properties relating to the Tracking are applied successfully";
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

        [Route("GetTrackingFieldList")]
        [HttpPost]
        public string GetTrackingFieldList(BarCodeList_TrackingFieldListParams trackingFieldListParams) //completed testing
        {
            var page = trackingFieldListParams.page;
            var sord = trackingFieldListParams.sord;
            var rows = trackingFieldListParams.rows;

            var jsonData = string.Empty;

            try
            {
                using (var context = new TABFusionRMSContext(trackingFieldListParams.ConnectionString))
                {
                    var pTrackingEntity = context.SLTrackingSelectDatas;
                    if (pTrackingEntity == null)
                    {
                        return jsonData;
                    }
                    else
                    {
                        var setting = new JsonSerializerSettings();
                        setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                        jsonData = JsonConvert.SerializeObject(pTrackingEntity.GetJsonListForGrid(sord, page, rows, "Id"), Newtonsoft.Json.Formatting.Indented, setting);
                        return jsonData;
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonData;
        }

        [Route("RemoveTrackingField")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> RemoveTrackingField(RemoveTrackingFieldParams removeTrackingFieldParams) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                var pTrackingFieldId = removeTrackingFieldParams.RowId;
                using (var context = new TABFusionRMSContext(removeTrackingFieldParams.ConnectionString))
                {
                    var pTrackingFieldEntity = await context.SLTrackingSelectDatas.Where(x => x.SLTrackingSelectDataId == pTrackingFieldId).FirstOrDefaultAsync();
                    if (pTrackingFieldEntity != null)
                    {
                        context.SLTrackingSelectDatas.Remove(pTrackingFieldEntity);
                        model.ErrorMessage = "Selected Additional Tracking field removed successfully"; // Keys.DeleteSuccessMessage()
                        model.ErrorType = "s";
                    }
                    else
                    {
                        model.ErrorMessage = "There is no record found in system";
                        model.ErrorType = "e";
                    }
                    await context.SaveChangesAsync();
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

        [Route("GetTrackingField")]
        [HttpPost]
        public async Task<string> GetTrackingField(RemoveTrackingFieldParams getTrackingFieldParam) //completed testing 
        {
            var jsonObject = string.Empty;

            try
            {
                using (var context = new TABFusionRMSContext(getTrackingFieldParam.ConnectionString))
                {
                    var pTrackingFieldId = getTrackingFieldParam.RowId;
                    var pTrackingFieldEntity = await context.SLTrackingSelectDatas.Where(x => x.SLTrackingSelectDataId == pTrackingFieldId).FirstOrDefaultAsync();
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pTrackingFieldEntity, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return jsonObject;
        }

        [Route("SetTrackingField")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetTrackingField(SLTrackingSelectDataParam slTrackingSelectDataParam) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pSLTrackingData = new SLTrackingSelectData();
            try
            {
                using (var context = new TABFusionRMSContext(slTrackingSelectDataParam.ConnectionString))
                {
                    if (slTrackingSelectDataParam.SLTrackingSelectDataId > 0)
                    {
                        if (await context.SLTrackingSelectDatas.AnyAsync(x => x.Id.Trim().ToLower() == slTrackingSelectDataParam.Id.Trim().ToLower() && x.SLTrackingSelectDataId != slTrackingSelectDataParam.SLTrackingSelectDataId) == false)
                        {
                            pSLTrackingData.SLTrackingSelectDataId = slTrackingSelectDataParam.SLTrackingSelectDataId;
                            pSLTrackingData.Id = slTrackingSelectDataParam.Id;
                            context.Entry(pSLTrackingData).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                            model.ErrorType = "s";
                            model.ErrorMessage = "Selected Additional Tracking field updated successfully";
                        }
                        else
                        {
                            model.ErrorType = "w";
                            model.ErrorMessage = "The record for Additional Tracking Field already exists";
                        }
                    }
                    else if (await context.SLTrackingSelectDatas.AnyAsync(x => (x.Id.Trim().ToLower()) == (slTrackingSelectDataParam.Id.Trim().ToLower())) == false)
                    {
                        pSLTrackingData.Id = slTrackingSelectDataParam.Id;
                        context.SLTrackingSelectDatas.Add(pSLTrackingData);
                        await context.SaveChangesAsync();
                        model.ErrorType = "s";
                        model.ErrorMessage = "Additional Tracking field added successfully";
                    }
                    else
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = "The record for Additional Tracking Field already exists";
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

        [Route("GetReconciliation")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetReconciliation(string ConnectionString)
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pAssetNumber = await context.AssetStatus.OrderBy(m => m.Id).ToListAsync();
                    int totalRecord = pAssetNumber.Count();
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(totalRecord, Newtonsoft.Json.Formatting.Indented, Setting);
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

        #region Email Notification All Methods moved

        private enum EmailType        {            etDelivery = 0x1,            etWaitList = 0x2,            etException = 0x4,            etCheckedOut = 0x8,            etRequest = 0x10,            etPastDue = 0x20,            etSimple = 0x40,            etBackground = 0x80        }

        [Route("SetEmailDetails")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetEmailDetails(SetEmailDetailsParams setEmailDetailsParams) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(setEmailDetailsParams.ConnectionString))
                {
                    EmailType eNotificationEnabled = default;
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    pSystemEntity.EMailDeliveryEnabled = setEmailDetailsParams.EMailDeliveryEnabled;
                    pSystemEntity.EMailWaitListEnabled = setEmailDetailsParams.EMailWaitListEnabled;
                    pSystemEntity.EMailExceptionEnabled = setEmailDetailsParams.EMailExceptionEnabled;

                    if (setEmailDetailsParams.EMailDeliveryEnabled)
                        eNotificationEnabled = (EmailType)(eNotificationEnabled += (int)EmailType.etDelivery);
                    if (setEmailDetailsParams.EMailWaitListEnabled)
                        eNotificationEnabled = (EmailType)(eNotificationEnabled + (int)EmailType.etWaitList);
                    if (setEmailDetailsParams.EMailExceptionEnabled)
                        eNotificationEnabled = (EmailType)(eNotificationEnabled + (int)EmailType.etException);
                    if (setEmailDetailsParams.EMailBackgroundEnabled)
                        eNotificationEnabled = (EmailType)(eNotificationEnabled + (int)EmailType.etBackground);
                    pSystemEntity.NotificationEnabled = Convert.ToInt32(eNotificationEnabled);

                    pSystemEntity.SMTPServer = setEmailDetailsParams.SystemEmailSMTPServer;
                    if (setEmailDetailsParams.SystemEmailSMTPServer != null)
                    {
                        pSystemEntity.SMTPPort = setEmailDetailsParams.SystemEmailSMTPPort;
                    }
                    else
                    {
                        pSystemEntity.SMTPPort = 25;
                    }
                    if (setEmailDetailsParams.SystemEmailEMailConfirmationType <= 0)
                    {
                        pSystemEntity.EMailConfirmationType = setEmailDetailsParams.SystemEmailEMailConfirmationType;
                    }
                    else
                    {
                        pSystemEntity.EMailConfirmationType = 0;
                    }
                    if (setEmailDetailsParams.SystemEmailSMTPUserAddress == null || setEmailDetailsParams.SystemEmailSMTPUserPassword == null)
                    {
                        pSystemEntity.SMTPUserPassword = pSystemEntity.SMTPUserPassword;
                        pSystemEntity.SMTPUserAddress = pSystemEntity.SMTPUserAddress;
                    }
                    else
                    {
                        pSystemEntity.SMTPUserAddress = setEmailDetailsParams.SystemEmailSMTPUserAddress;
                        string encrypted = GenerateKey(Convert.ToBoolean(1), setEmailDetailsParams.SystemEmailSMTPUserPassword, null);
                        pSystemEntity.SMTPUserPassword = encrypted;
                    }
                    pSystemEntity.SMTPAuthentication = setEmailDetailsParams.SMTPAuthentication;
                    context.Entry(pSystemEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    model.ErrorType = "s";
                    model.ErrorMessage = "Action made on Email Notifications are applied Successfully";
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

        [Route("GetSMTPDetails")]
        [HttpPost]
        public async Task<string> GetSMTPDetails(GetSMTPDetailsParams getSMTPDetailsParams) //completed testing 
        {
            var res = string.Empty;

            try
            {
                using (var context = new TABFusionRMSContext(getSMTPDetailsParams.ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    if (getSMTPDetailsParams.FlagSMPT)
                    {
                        if (pSystemEntity.SMTPUserPassword is not null)
                        {
                            var byteArray = Encoding.Default.GetBytes(pSystemEntity.SMTPUserPassword);
                            string encrypted = GenerateKey(Convert.ToBoolean(0), null, byteArray);
                            pSystemEntity.SMTPUserPassword = encrypted;
                        }
                    }
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    res = JsonConvert.SerializeObject(pSystemEntity, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return res;
        }

        #endregion

        #region Before Login warning message All Methods Moved

        [Route("GetWarningMessage")]
        [HttpGet]
        public string GetWarningMessage(string webRootPath) //completed testing
        {
            string showMessage = string.Empty;
            string warningMessage = string.Empty;
            string path = System.IO.Path.Combine(Convert.ToString(webRootPath), @"ImportFiles\WarningMessageXML.xml");

            if (System.IO.File.Exists(path))
            {
                XmlReader document = new XmlTextReader(path);
                while (document.Read())
                {
                    var type = document.NodeType;
                    if (type == XmlNodeType.Element)
                    {
                        if (document.Name == "ShowMessage")
                            showMessage = document.ReadInnerXml().ToString();
                        if (document.Name == "WarningMessage")
                            warningMessage = document.ReadInnerXml().ToString();
                    }
                }
                document.Close();
            }

            warningMessage = warningMessage.Remove(0, 1);
            warningMessage = warningMessage.Remove(warningMessage.Length - 1);
            string data = showMessage + "||" + warningMessage;
            var Setting = new JsonSerializerSettings();
            Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            var jsonObject = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented, Setting);

            return jsonObject;
        }

        [Route("SetWarningMessage")]
        [HttpPost]
        public ReturnErrorTypeErrorMsg SetWarningMessage(SetWarningMessageParams setWarningMessageParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pWarningMessage = setWarningMessageParams.WarningMessage;
            var pShowMessage = setWarningMessageParams.ShowMessage;
            try
            {
                if (pShowMessage.Trim().ToLower() == "yes" && string.IsNullOrEmpty(pWarningMessage))
                {
                    model.ErrorType = "w";
                    model.ErrorMessage = "Please enter value in message textbox";
                }
                else
                {
                    if (!string.IsNullOrEmpty(pWarningMessage))
                    {
                        if (Convert.ToString(pWarningMessage.TrimStart()[0]) != "\"")
                            pWarningMessage = "\"" + pWarningMessage; // firstLetter
                        if (Convert.ToString(pWarningMessage.Last()) != "\"")
                            pWarningMessage = pWarningMessage + "\""; // lastLetter
                    }

                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    string path = System.IO.Path.Combine(Convert.ToString(setWarningMessageParams.WebRootPath), @"ImportFiles\WarningMessageXML.xml");
                    var XmlWrt = XmlWriter.Create(path, settings);
                    // Write the Xml declaration.
                    XmlWrt.WriteStartDocument();
                    // Write a comment.
                    XmlWrt.WriteComment("Before login Warning Message Data.");
                    // Write the root element.
                    XmlWrt.WriteStartElement("Data");
                    // Write element.
                    XmlWrt.WriteStartElement("ShowMessage");
                    XmlWrt.WriteString(pShowMessage);
                    XmlWrt.WriteEndElement();
                    XmlWrt.WriteStartElement("WarningMessage");
                    XmlWrt.WriteString(pWarningMessage);
                    XmlWrt.WriteEndElement();
                    // Close the XmlTextWriter.
                    XmlWrt.WriteEndDocument();
                    XmlWrt.Close();
                    model.ErrorType = "s";
                    if (pShowMessage.Trim().ToLower() == "no")
                    {
                        model.ErrorMessage = "Sign In message not applied successfully";
                    }
                    else
                    {
                        model.ErrorMessage = "Sign In message has been applied successfully";
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

        #region Bar Code Search Order All Methods Moved 

        [Route("GetBarCodeList")]
        [HttpPost]
        public async Task<string> GetBarCodeList(BarCodeList_TrackingFieldListParams barCodeListParams) //completed testing 
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

        [Route("SetbarCodeSearchEntity")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetbarCodeSearchEntity(SetbarCodeSearchEntityParams setbarCodeSearchEntityParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var Id = setbarCodeSearchEntityParams.Id;
            var TableName = setbarCodeSearchEntityParams.TableName;
            var scanOrder = setbarCodeSearchEntityParams.ScanOrder;
            var FieldName = setbarCodeSearchEntityParams.FieldName;
            var IdStripChars = setbarCodeSearchEntityParams.IdStripChars;
            var IdMask = setbarCodeSearchEntityParams.IdMask;

            try
            {
                using (var context = new TABFusionRMSContext(setbarCodeSearchEntityParams.ConnectionString))
                {
                    var pBarCodeSearchEntity = new ScanList()
                    {
                        Id = Id,
                        FieldName = FieldName,
                        TableName = TableName,
                        IdStripChars = IdStripChars,
                        IdMask = IdMask
                    };

                    var oSchemaColumns = SchemaInfoDetails.GetSchemaInfo(pBarCodeSearchEntity.TableName, setbarCodeSearchEntityParams.ConnectionString, pBarCodeSearchEntity.FieldName);

                    if (oSchemaColumns.Count == 0)
                    {
                        var oTables = await context.Tables.Where(x => x.TableName.Trim().ToLower().Equals(pBarCodeSearchEntity.TableName.Trim().ToLower())).FirstOrDefaultAsync();
                        oSchemaColumns = SchemaInfoDetails.GetSchemaInfo(pBarCodeSearchEntity.TableName, setbarCodeSearchEntityParams.ConnectionString, pBarCodeSearchEntity.FieldName);
                    }

                    if (pBarCodeSearchEntity.Id > 0)
                    {
                        if (await context.ScanLists.AnyAsync(x => (x.TableName) == (pBarCodeSearchEntity.TableName) && (x.FieldName) == (pBarCodeSearchEntity.FieldName) && x.Id != pBarCodeSearchEntity.Id))
                        {
                            model.ErrorType = "w";
                            model.ErrorMessage = string.Format("The record for '{0}' already exists", pBarCodeSearchEntity.TableName.ToUpper());
                        }
                        else
                        {
                            var pScanList = await context.ScanLists.Where(x => x.Id == pBarCodeSearchEntity.Id).FirstOrDefaultAsync();
                            pScanList.TableName = pBarCodeSearchEntity.TableName;
                            pScanList.FieldName = pBarCodeSearchEntity.FieldName;
                            pScanList.FieldType = Convert.ToInt16(oSchemaColumns[0].DataType);
                            pScanList.IdStripChars = pBarCodeSearchEntity.IdStripChars;
                            pScanList.IdMask = pBarCodeSearchEntity.IdMask;
                            context.Entry(pScanList).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                            model.ErrorType = "s";
                            model.ErrorMessage = "Selected Barcode Search order updated successfully";
                        }
                    }
                    else if (await context.ScanLists.AnyAsync(x => (x.TableName) == (pBarCodeSearchEntity.TableName) && (x.FieldName) == (pBarCodeSearchEntity.FieldName)))
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = string.Format("The record for '{0}' already exists", pBarCodeSearchEntity.TableName.ToUpper());
                    }
                    else
                    {
                        pBarCodeSearchEntity.ScanOrder = (short?)(scanOrder + 1);
                        pBarCodeSearchEntity.FieldType = Convert.ToInt16(oSchemaColumns[0].DataType);
                        context.ScanLists.Add(pBarCodeSearchEntity);
                        await context.SaveChangesAsync();
                        model.ErrorType = "s";
                        model.ErrorMessage = "New Table entry has been added to Barcode Search Order";
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

        #region TABQUIK -> Field Mapping

        [Route("LoadTABQUIKFieldMappingPartial")]
        [HttpGet]
        public async Task<string> LoadTABQUIKFieldMappingPartial(string ConnectionString, int pTabquikId)
        {
            var jsonObject = string.Empty;
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                if (pTabquikId != 0)
                {
                    var oOneStripJob = await context.OneStripJobs.Where(x => x.Id == pTabquikId && x.Inprint == 5).FirstOrDefaultAsync();
                    var Setting = new JsonSerializerSettings();
                    var result = new
                    {
                        oOneStripJob.TableName,
                        oOneStripJob.SQLUpdateString
                    };
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            return jsonObject;
        }

        #endregion

        #region Retention All Methods Moved

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

        [Route("RemoveRetentionTableFromList")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> RemoveRetentionTableFromList(RemoveRetentionTableFromListParam removeRetentionTableFromListParam) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(removeRetentionTableFromListParam.ConnectionString))
                {
                    foreach (string item in removeRetentionTableFromListParam.TableIds)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            int pTableId = Convert.ToInt32(item);
                            var pTableEntity = await context.Tables.Where(x => x.TableId.Equals(pTableId)).FirstOrDefaultAsync();
                            pTableEntity.RetentionPeriodActive = false;
                            pTableEntity.RetentionInactivityActive = false;

                            context.Entry(pTableEntity).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                        }
                    }
                    model.ErrorType = "s";
                    model.ErrorMessage = "Table moved successfully.";
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

        [Route("GetRetentionPropertiesData")]
        [HttpPost]
        public async Task<ReturnGetRetentionPropertiesData> GetRetentionPropertiesData(GetRetentionPropertiesDataParams getRetentionPropertiesDataParams) //completed testing 
        {
            var model = new ReturnGetRetentionPropertiesData();
            var passport = getRetentionPropertiesDataParams.Passport;
            var pTableId = getRetentionPropertiesDataParams.TableId;
            var lstRetCodeFields = new List<string>();            var lstDateFields = new List<string>();            var lstRelatedTable = new List<string>();            var bFootNote = default(bool);            string lstRetentionCode = "";            string lstDateClosed = "";            string lstDateCreated = "";            string lstDateOpened = "";            string lstDateOther = "";

            bool bTrackable = false;


            using (var context = new TABFusionRMSContext(passport.ConnectionString))
            {
                var pTableEntites = await context.Tables.Where(x => x.TableId.Equals(pTableId)).FirstOrDefaultAsync();                model.ErrorType = "s";                model.ErrorMessage = "Record saved successfully";                bTrackable = passport.CheckPermission(pTableEntites.TableName.Trim(), (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Table, (Permissions.Permission)Enums.PassportPermissions.Transfer);                if (pTableEntites == null)                {                    return new ReturnGetRetentionPropertiesData
                    {
                        Success = false,
                        ErrorType = "e",
                        ErrorMessage = "Record not found."
                    };                }                var oTables = await context.Tables.Where(x => x.TableName.Trim().ToLower().Equals(pTableEntites.TableName.Trim().ToLower())).FirstOrDefaultAsync();                var dbRecordSet = SchemaInfoDetails.GetTableSchemaInfo(pTableEntites.TableName, passport.ConnectionString);                if (!dbRecordSet.Exists(x => x.ColumnName == "RetentionCodesId"))                {                    lstRetentionCode = "* RetentionCodesId";                    bFootNote = true;                }                if (!dbRecordSet.Exists(x => x.ColumnName == "DateOpened"))                {                    lstDateOpened = "* DateOpened";                    bFootNote = true;                }                if (!dbRecordSet.Exists(x => x.ColumnName == "DateClosed"))                {                    lstDateClosed = "* DateClosed";                    bFootNote = true;                }                if (!dbRecordSet.Exists(x => x.ColumnName == "DateCreated"))                {                    lstDateCreated = "* DateCreated";                    bFootNote = true;                }                if (!dbRecordSet.Exists(x => x.ColumnName == "DateOther"))                {                    lstDateOther = "* DateOther";                    bFootNote = true;                }                foreach (var oSchemaColumn in dbRecordSet)                {                    if (!SchemaInfoDetails.IsSystemField(oSchemaColumn.ColumnName))                    {                        if (oSchemaColumn.IsADate)                        {                            lstDateFields.Add(oSchemaColumn.ColumnName);                        }                        else if (oSchemaColumn.IsString && oSchemaColumn.CharacterMaxLength == 20)                        {                            lstRetCodeFields.Add(oSchemaColumn.ColumnName);                        }                    }                }                var Setting = new JsonSerializerSettings();                Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;                lstRetCodeFields.Sort();                lstDateFields.Sort();

                model.RetCodeFieldsObject = JsonConvert.SerializeObject(lstRetCodeFields, Newtonsoft.Json.Formatting.Indented, Setting);                model.DateFields = JsonConvert.SerializeObject(lstDateFields, Newtonsoft.Json.Formatting.Indented, Setting);                var lstRelatedTables = await context.RelationShips.Where(x => (x.LowerTableName) == (pTableEntites.TableName)).ToListAsync();                foreach (RelationShip item in lstRelatedTables)                    lstRelatedTable.Add(item.UpperTableName);                var lstTables = await (context.Tables.Where(x => x.RetentionPeriodActive == true && x.RetentionFinalDisposition != 0 && lstRelatedTable.Contains(x.TableName))).ToListAsync();                var pRetentionCodes = await context.SLRetentionCodes.OrderBy(x => x.Id).ToListAsync();                model.RelatedTblObj = JsonConvert.SerializeObject(lstTables, Newtonsoft.Json.Formatting.Indented, Setting);                model.TableEntity = JsonConvert.SerializeObject(pTableEntites, Newtonsoft.Json.Formatting.Indented, Setting);                model.RetentionCodesJSON = JsonConvert.SerializeObject(pRetentionCodes, Newtonsoft.Json.Formatting.Indented, Setting);                model.IsThereLocation = Tracking.GetArchiveLocations(passport);


                model.ListRetentionCode = lstRetentionCode;
                model.ListDateCreated = lstDateCreated;
                model.ListDateClosed = lstDateClosed;
                model.ListDateOpened = lstDateOpened;
                model.ListDateOther = lstDateOther;
                model.FootNote = bFootNote;
                model.ArchiveLocationField = pTableEntites.ArchiveLocationField;
                model.Trackable = bTrackable;
            }

            return model;
        }

        [Route("SetRetentionParameters")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetRetentionParameters(SetRetentionParametersParam setRetentionParametersParam) //Js Issue completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(setRetentionParametersParam.ConnectionString))
                {
                    var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();

                    pSystemEntity.RetentionTurnOffCitations = setRetentionParametersParam.IsUseCitaions;
                    pSystemEntity.RetentionYearEnd = setRetentionParametersParam.YearEnd;
                    context.Entry(pSystemEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    var pServiceTasks = await context.SLServiceTasks.OrderBy(x => x.Id).FirstOrDefaultAsync();
                    pServiceTasks.Interval = setRetentionParametersParam.InactivityPeriod;

                    context.Entry(pServiceTasks).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    model.ErrorType = "s";
                    model.ErrorMessage = "Properties relating to Retention are applied Successfully";
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

        [Route("SetRetentionTblPropData")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetRetentionTblPropData(SetRetentionTblPropDataParam setRetentionTblPropDataParam) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pTableId = setRetentionTblPropDataParam.TableId;
            var pInActivity = setRetentionTblPropDataParam.InActivity;
            var pAssignment = setRetentionTblPropDataParam.Assignment;
            var pDisposition = setRetentionTblPropDataParam.Disposition;
            var pDefaultRetentionId = setRetentionTblPropDataParam.DefaultRetentionId;
            var pRelatedTable = setRetentionTblPropDataParam.RelatedTable;
            var pRetentionCode = setRetentionTblPropDataParam.RetentionCode;
            var pDateOpened = setRetentionTblPropDataParam.DateOpened;
            var pDateClosed = setRetentionTblPropDataParam.DateClosed;
            var pDateCreated = setRetentionTblPropDataParam.DateCreated;
            var pOtherDate = setRetentionTblPropDataParam.OtherDate;

            string msgVerifyRetDisposition = "";            string sSQL = "";

            try
            {
                using (var context = new TABFusionRMSContext(setRetentionTblPropDataParam.ConnectionString))
                {
                    var pTableEntites = await context.Tables.Where(x => x.TableId.Equals(pTableId)).FirstOrDefaultAsync();
                    var oTables = await context.Tables.Where(x => x.TableName.Trim().ToLower().Equals(pTableEntites.TableName.Trim().ToLower())).FirstOrDefaultAsync();

                    var allArchiveLocation = await context.Locations.Where(loc => loc.ArchiveStorage == true).ToListAsync();
                    if (!string.IsNullOrEmpty(pTableEntites.DefaultTrackingId))
                    {
                        var tableArchiveLocation = await context.Locations.Where(loc => loc.Id.ToString() == pTableEntites.DefaultTrackingId).FirstOrDefaultAsync();
                        if (Convert.ToBoolean(tableArchiveLocation.ArchiveStorage) && pDisposition == 1 && allArchiveLocation.Count == 1)
                        {
                            model.ErrorMessage = "The Archival location is already set up as the Initial Tracking Destination";
                            throw new Exception(model.ErrorMessage);
                        }
                    }
                    var oViews = await context.Views.Where(x => x.TableName.Trim().ToLower().Equals(pTableEntites.TableName.Trim().ToLower())).FirstOrDefaultAsync();

                    if (pDisposition != 0 || pInActivity)
                    {
                        pTableEntites.RetentionAssignmentMethod = pAssignment;
                        pTableEntites.DefaultRetentionId = pDefaultRetentionId;
                        pTableEntites.RetentionRelatedTable = pRelatedTable;
                    }

                    pTableEntites.RetentionPeriodActive = pDisposition != 0;
                    pTableEntites.RetentionInactivityActive = pInActivity;
                    pTableEntites.RetentionFinalDisposition = pDisposition;

                    if (!string.IsNullOrEmpty(pRetentionCode))
                    {
                        if (pRetentionCode.Substring(0, 1) == "*")
                        {
                            SaveNewFieldToTable(pTableEntites.TableName, pRetentionCode.Substring(1).Trim(), Enums.DataTypeEnum.rmVarWChar, oViews.Id, setRetentionTblPropDataParam.ConnectionString);

                            pTableEntites.RetentionFieldName = pRetentionCode.Substring(1).Trim();
                        }
                        else
                        {
                            pTableEntites.RetentionFieldName = pRetentionCode;
                        }
                    }

                    if (!string.IsNullOrEmpty(pDateOpened))
                    {
                        if (pDateOpened.Substring(0, 1) == "*")
                        {
                            SaveNewFieldToTable(pTableEntites.TableName, pDateOpened.Substring(1).Trim(), Enums.DataTypeEnum.rmDate, oViews.Id, setRetentionTblPropDataParam.ConnectionString);

                            pTableEntites.RetentionDateOpenedField = pDateOpened.Substring(1).Trim();
                        }
                        else
                        {
                            pTableEntites.RetentionDateOpenedField = pDateOpened;
                        }
                    }

                    if (!string.IsNullOrEmpty(pDateClosed))
                    {
                        if (pDateClosed.Substring(0, 1) == "*")
                        {
                            SaveNewFieldToTable(pTableEntites.TableName, pDateClosed.Substring(1).Trim(), Enums.DataTypeEnum.rmDate, oViews.Id, setRetentionTblPropDataParam.ConnectionString);

                            pTableEntites.RetentionDateClosedField = pDateClosed.Substring(1).Trim();
                        }
                        else
                        {
                            pTableEntites.RetentionDateClosedField = pDateClosed;
                        }
                    }

                    if (!string.IsNullOrEmpty(pDateCreated))
                    {
                        if (pDateCreated.Substring(0, 1) == "*")
                        {
                            SaveNewFieldToTable(pTableEntites.TableName, pDateCreated.Substring(1).Trim(), Enums.DataTypeEnum.rmDate, oViews.Id, setRetentionTblPropDataParam.ConnectionString);

                            pTableEntites.RetentionDateCreateField = pDateCreated.Substring(1).Trim();
                        }
                        else
                        {
                            pTableEntites.RetentionDateCreateField = pDateCreated;
                        }
                    }

                    if (!string.IsNullOrEmpty(pOtherDate))
                    {
                        if (pOtherDate.Substring(0, 1) == "*")
                        {
                            SaveNewFieldToTable(pTableEntites.TableName, pOtherDate.Substring(1).Trim(), Enums.DataTypeEnum.rmDate, oViews.Id, setRetentionTblPropDataParam.ConnectionString);

                            pTableEntites.RetentionDateOtherField = pOtherDate.Substring(1).Trim();
                        }
                        else
                        {
                            pTableEntites.RetentionDateOtherField = pOtherDate;
                        }
                    }

                    context.Entry(pTableEntites).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    msgVerifyRetDisposition = await VerifyRetentionDispositionTypesForParentAndChildren(setRetentionTblPropDataParam.ConnectionString, pTableEntites.TableId);

                    sSQL = "ALTER TABLE [" + pTableEntites.TableName + "]";
                    sSQL = sSQL + " ADD [%slRetentionInactive] BIT DEFAULT 0";
                    ExecuteSqlCommand(setRetentionTblPropDataParam.ConnectionString, sSQL, false);
                    sSQL = "";

                    sSQL = "ALTER TABLE [" + pTableEntites.TableName + "]";
                    sSQL = sSQL + " ADD [%slRetentionInactiveFinal] BIT DEFAULT 0";
                    ExecuteSqlCommand(setRetentionTblPropDataParam.ConnectionString, sSQL, false);
                    sSQL = "";

                    sSQL = "ALTER TABLE [" + pTableEntites.TableName + "]";
                    sSQL = sSQL + " ADD [%slRetentionDispositionStatus] INT DEFAULT 0";
                    ExecuteSqlCommand(setRetentionTblPropDataParam.ConnectionString, sSQL, false);
                    sSQL = "";

                    model.ErrorType = "s";
                    model.ErrorMessage = "Record saved successfully";
                    model.stringValue1 = msgVerifyRetDisposition;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";                if (string.IsNullOrEmpty(model.ErrorMessage))                {
                    model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";                }
            }

            return model;
        }

        #endregion

        #region Security All Methods Moved

        [Route("CheckModuleLevelAccess")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> CheckModuleLevelAccess(CheckModuleLevelAccessParams checkModuleLevelAccessParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var passport = checkModuleLevelAccessParams.passport;
            var TablePermission = checkModuleLevelAccessParams.TablePermission;
            var iCntRpt = checkModuleLevelAccessParams.iCntRpt;
            var ViewPermission = checkModuleLevelAccessParams.ViewPermission;
            try
            {
                var mdlAccessDictionary = new Dictionary<string, bool>();
                bool bAddTabApplication = false;
                bool bAddTabDatabase = false;
                bool bAddTabDirectories = false;
                bool bAddTabData = false;
                bool bAddTabTables = false;
                bool bAddTabViews = false;
                bool bAddTabReports = false;
                bool bAddTabSecuirty = false;

                bool mbSecuriyGroup = false;
                bool mbMgrGroup = false;
                bool bAtLeastOneTablePermission = false;
                bool bAtLeastOneViewPermission = false;
                bool bAdminPermission = false;
                int iCntRpts = 0;

                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var lTableEntities = await context.Tables.ToListAsync();
                    var lViewEntities = await context.Views.ToListAsync();

                    mbSecuriyGroup = passport.CheckPermission(Common.SECURE_SECURITY, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access) | passport.CheckPermission(Common.SECURE_SECURITY_USER, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access);
                    mbMgrGroup = passport.CheckAdminPermission(Permissions.Permission.Access);

                    if (mbMgrGroup)
                    {
                        bAddTabApplication = true;
                        bAddTabDatabase = true;
                        bAddTabData = true;
                    }
                    bAddTabDirectories = passport.CheckPermission(Common.SECURE_STORAGE, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access);

                    iCntRpts = CollectionsClass.CheckReportsPermission(await context.Tables.ToListAsync(), await context.Views.ToListAsync(), passport, iCntRpt);
                    bAddTabReports = mbMgrGroup | iCntRpts > 0 | passport.CheckPermission(Common.SECURE_REPORT_STYLES, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access);

                    if (mbSecuriyGroup)
                    {
                        bAddTabSecuirty = true;
                    }
                    bAtLeastOneTablePermission = CollectionsClass.CheckTablesPermission(lTableEntities, mbMgrGroup, passport, TablePermission);
                    bAtLeastOneViewPermission = CollectionsClass.CheckViewsPermission(lViewEntities, mbMgrGroup, passport, TablePermission, ViewPermission);
                    bAdminPermission = passport.CheckAdminPermission(Permissions.Permission.Access);

                    bAddTabTables = mbMgrGroup | bAtLeastOneTablePermission;
                    bAddTabViews = mbMgrGroup | bAtLeastOneViewPermission;

                    mdlAccessDictionary.Add("Application", bAddTabApplication);
                    mdlAccessDictionary.Add("Database", bAddTabDatabase);
                    mdlAccessDictionary.Add("Directories", bAddTabDirectories);
                    mdlAccessDictionary.Add("Data", bAddTabData);
                    mdlAccessDictionary.Add("Tables", bAddTabTables);
                    mdlAccessDictionary.Add("Views", bAddTabViews);
                    mdlAccessDictionary.Add("Reports", bAddTabReports);
                    mdlAccessDictionary.Add("Security", bAddTabSecuirty);
                    mdlAccessDictionary.Add("AdminPermission", bAdminPermission);

                    model.DictionaryResult = mdlAccessDictionary;
                    model.AtLeastOneViewPermissionSessionValue = bAtLeastOneViewPermission;
                    model.AtLeastOneTablePermissionSessionValue = bAtLeastOneTablePermission;
                    model.intValue = iCntRpts;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }

            return model;
        }

        [Route("ValidateApplicationLink")]
        [HttpPost]
        public async Task<int> ValidateApplicationLink(ValidateApplicationLinkReq req) //completed testing 
        {
            var passport = req.passport;
            using (var context = new TABFusionRMSContext(passport.ConnectionString))
            {
                var oTables = await context.Tables.ToListAsync();
                bool bHaveRights = false;

                if (passport.CheckPermission(req.pModuleNameStr, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
                {
                    if (req.pModuleNameStr == "Import Setup")
                    {
                        foreach (var oTable in oTables)
                        {
                            if (!CollectionsClass.IsEngineTable(oTable.TableName) | CollectionsClass.IsEngineTableOkayToImport(oTable.TableName))
                            {
                                if (passport.CheckPermission(oTable.TableName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Table, (Permissions.Permission)Enums.PassportPermissions.Import))
                                {
                                    bHaveRights = true;
                                    break;
                                }
                            }
                        }

                        if (!bHaveRights)
                        {
                            if (passport.CheckPermission(Common.SECURE_TRACKING, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access))
                            {
                                bHaveRights = true;
                            }
                        }

                        if (bHaveRights)
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        } // Here 2 indicates permission issues for importing table.
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }

            }
        }

        #region Users All Methods moved

        [Route("LoadSecurityUserGridData")]
        [HttpPost]
        public string LoadSecurityUserGridData(LoadSecurityUserGridDataParams loadSecurityUserGridDataParams) //completed testing 
        {
            var jsonObject = string.Empty;
            var page = loadSecurityUserGridDataParams.page;
            var sord = loadSecurityUserGridDataParams.sord;
            var rows = loadSecurityUserGridDataParams.rows;
            try
            {
                using (var context = new TABFusionRMSContext(loadSecurityUserGridDataParams.ConnectionString))
                {
                    var pSecureUserEntities = from t in context.SecureUsers
                                              where t.UserID != -1 & (t.AccountType.ToLower() == "s" | t.AccountType.ToLower() == "z")
                                              select new { t.UserID, t.UserName, t.Email, t.FullName, t.AccountDisabled, t.MustChangePassword };

                    var setting = new JsonSerializerSettings();
                    setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pSecureUserEntities.GetJsonListForGrid(sord, page, rows, "UserName"), Newtonsoft.Json.Formatting.Indented, setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonObject;
        }

        [Route("SetUserDetails")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetUserDetails(SetUserDetailsParams pUserEntity) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(pUserEntity.ConnectionString))
                {
                    int pNextUserID = 0;
                    if (pUserEntity.UserID > 0)
                    {
                        if (await context.SecureUsers.AnyAsync(x => (x.UserName.Trim().ToLower()) == (pUserEntity.UserName.Trim().ToLower()) && x.UserID != pUserEntity.UserID) == false)
                        {
                            var pUserProfileEntity = await context.SecureUsers.Where(x => x.UserID == pUserEntity.UserID).FirstOrDefaultAsync();
                            {
                                pUserProfileEntity.UserName = Convert.ToString(Interaction.IIf(pUserEntity.UserName is null, "", pUserEntity.UserName));
                                pUserProfileEntity.FullName = Convert.ToString(Interaction.IIf(pUserEntity.FullName is null, "", pUserEntity.FullName));
                                pUserProfileEntity.Email = Convert.ToString(Interaction.IIf(pUserEntity.Email is null, "", pUserEntity.Email));
                                pUserProfileEntity.Misc1 = pUserEntity.Misc1;
                                pUserProfileEntity.Misc2 = pUserEntity.Misc2;
                                pUserProfileEntity.AccountDisabled = pUserEntity.AccountDisabled;
                            }

                            context.Entry(pUserProfileEntity).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                            model.ErrorType = "s";
                            model.ErrorMessage = "Changes made on selected user are updated successfully"; // Fixed FUS-6054
                        }
                        else
                        {
                            model.ErrorType = "w";
                            model.ErrorMessage = string.Format("The User Name {0} is already in use. Please use different User Name", pUserEntity.UserName);
                        }
                    }
                    else if (await context.SecureUsers.AnyAsync(x => (x.UserName.Trim().ToLower()) == (pUserEntity.UserName.Trim().ToLower())) == false)
                    {
                        var newUserEntity = new SecureUser();
                        newUserEntity.PasswordHash = "";
                        newUserEntity.AccountType = "S";
                        newUserEntity.PasswordUpdate = DateTime.Now;
                        newUserEntity.MustChangePassword = true;
                        newUserEntity.FullName = Convert.ToString(Interaction.IIf(pUserEntity.FullName is null, "", pUserEntity.FullName));
                        newUserEntity.Email = Convert.ToString(Interaction.IIf(pUserEntity.Email is null, "", pUserEntity.Email));
                        newUserEntity.AccountDisabled = pUserEntity.AccountDisabled;
                        newUserEntity.DisplayName = pUserEntity.UserName;
                        newUserEntity.UserName = pUserEntity.UserName;

                        context.SecureUsers.Add(newUserEntity);
                        await context.SaveChangesAsync();

                        //pNextUserID = pUserEntity.UserID; --need to debuge the value 
                        newUserEntity.PasswordHash = Smead.Security.Encrypt.HashPassword(pNextUserID, "password$");

                        context.Entry(newUserEntity).State = EntityState.Modified;
                        await context.SaveChangesAsync();

                        model.ErrorType = "s";
                        model.ErrorMessage = "New User has been added into list of Users successfully"; // Fixed FUS-6057
                    }
                    else
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = string.Format("The User Name {0} is already in use. Please use different User Name", pUserEntity.UserName);
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

        // Get the user details for EDIT purpose.
        [Route("EditUserProfile")]
        [HttpGet]
        public async Task<string> EditUserProfile(string ConnectionString, int UserId) //completed testing
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pUserProfileEntity = await context.SecureUsers.Where(x => x.UserID == UserId).FirstOrDefaultAsync();
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pUserProfileEntity, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return jsonObject;
        }

        [Route("DeleteUserProfile")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> DeleteUserProfile(string ConnectionString, int UserId) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pUserProfileEntity = await context.SecureUsers.Where(x => x.UserID == UserId).FirstOrDefaultAsync();
                    var pUserGroupEntities = await context.SecureUserGroups.Where(x => x.UserID == UserId).ToListAsync();

                    context.SecureUsers.Remove(pUserProfileEntity);
                    await context.SaveChangesAsync();
                    context.SecureUserGroups.RemoveRange(pUserGroupEntities);
                    await context.SaveChangesAsync();

                    model.ErrorType = "s";
                    model.ErrorMessage = "Selected User has been deleted from the list of Users successfully";
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

        [Route("SetUserPassword")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetUserPassword(SetUserPasswordParams setUserPasswordParams) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(setUserPasswordParams.ConnectionString))
                {
                    var pUserEntity = await context.SecureUsers.Where(x => x.UserID == setUserPasswordParams.UserId).FirstOrDefaultAsync();
                    pUserEntity.PasswordHash = Smead.Security.Encrypt.HashPassword(setUserPasswordParams.UserId, setUserPasswordParams.UserPassword);
                    pUserEntity.MustChangePassword = setUserPasswordParams.CheckedState;

                    context.Entry(pUserEntity).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    model.ErrorType = "s";
                    model.ErrorMessage = "Password has been changed successfully";
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

        [Route("SetGroupsAgainstUser")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetGroupsAgainstUser(SetGroupsAgainstUserParams setGroupsAgainstUserParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(setGroupsAgainstUserParams.ConnectionString))
                {
                    var pSecureUserGroup = new SecureUserGroup();
                    var pUserGrpEntities = await context.SecureUserGroups.Where(x => x.UserID == setGroupsAgainstUserParams.UserID).ToListAsync();
                    context.SecureUserGroups.RemoveRange(pUserGrpEntities);
                    await context.SaveChangesAsync();
                    if (!setGroupsAgainstUserParams.GroupList.GetValue(0).ToString().Equals("None"))
                    {
                        if (setGroupsAgainstUserParams.GroupList.Length > 0)
                        {
                            foreach (var gid in setGroupsAgainstUserParams.GroupList)
                            {
                                pSecureUserGroup.UserID = setGroupsAgainstUserParams.UserID;
                                pSecureUserGroup.GroupID = Convert.ToInt32(gid);

                                context.SecureUserGroups.Add(pSecureUserGroup);
                                await context.SaveChangesAsync();
                            }
                        }
                        model.ErrorType = "s";
                        model.ErrorMessage = "Selected Group has been assigned to the User Successfully";
                    }
                    else
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = "Please select at least one group";
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

        [Route("GetAssignedGroupsForUser")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetAssignedGroupsForUser(string ConnectionString, int UserId) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            var lstGroups = new List<string>();
            List<SecureUserGroup> pSecureGroupUserEntities;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    pSecureGroupUserEntities = await context.SecureUserGroups.Where(x => x.UserID == UserId).ToListAsync();

                    foreach (SecureUserGroup item in pSecureGroupUserEntities)
                        lstGroups.Add(item.GroupID.ToString());

                    var pSecureGroupEntities = await (from t in context.SecureGroups
                                                      where lstGroups.Contains(t.GroupID.ToString())
                                                      orderby t.GroupName ascending
                                                      select t).ToListAsync();
                    foreach (var item in pSecureGroupEntities)
                    {
                        item.SecureUserGroups.Clear();
                        item.SecureObjectPermissions.Clear();
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(pSecureGroupEntities, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data Retrieved.";
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

        [Route("GetAllGroupsList")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetAllGroupsList(string ConnectionString) //completed testing  
        {
            var model = new ReturnErrorTypeErrorMsg();
            List<SecureGroup> lstAllGroups;

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    lstAllGroups = await context.SecureGroups.Where(x => x.GroupID != -1).OrderBy(x => x.GroupName).ToListAsync();

                    foreach (var item in lstAllGroups)
                    {
                        item.SecureObjectPermissions.Clear();
                        item.SecureUserGroups.Clear();
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(lstAllGroups, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data Retrieved.";
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

        [Route("UnlockUserAccount")]
        [HttpPost]
        public ReturnErrorTypeErrorMsg UnlockUserAccount(UnlockUserAccountParams unlockUserAccountParams) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                unlockUserAccountParams.passport.LogFailedLogs("Unlock", unlockUserAccountParams.OperatorId);

                model.ErrorType = "s";
                model.ErrorMessage = string.Format("'{0}' user account unlocked successfully", unlockUserAccountParams.OperatorId);
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

        #region Groups All Methods moved 

        [Route("LoadSecurityGroupGridData")]
        [HttpGet]
        public string LoadSecurityGroupGridData(string sord, int page, int rows, string ConnectionString) //completed testing
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pSecureGroupEntities = from t in context.SecureGroups
                                               select new { t.GroupID, t.GroupName, t.Description, t.ActiveDirectoryGroup, AutoLockSeconds = t.AutoLockSeconds / 60d, AutoLogOffSeconds = t.AutoLogOffSeconds / 60d };

                    var setting = new JsonSerializerSettings();
                    setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pSecureGroupEntities.GetJsonListForGrid(sord, page, rows, "GroupName"), Newtonsoft.Json.Formatting.Indented, setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return jsonObject;
        }

        [Route("SetGroupDetails")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetGroupDetails(SetGroupDetailsParams pGroupEntity) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var newGroupEntity = new SecureGroup();
            try
            {
                using (var context = new TABFusionRMSContext(pGroupEntity.ConnectionString))
                {
                    if (pGroupEntity.GroupID > -2)
                    {
                        var pGroupProfileEntity = await context.SecureGroups.Where(x => x.GroupID == pGroupEntity.GroupID).FirstOrDefaultAsync();

                        pGroupProfileEntity.GroupName = pGroupEntity.GroupName;
                        pGroupProfileEntity.Description = string.IsNullOrEmpty(pGroupEntity.Description) ? "" : pGroupEntity.Description;
                        pGroupProfileEntity.ActiveDirectoryGroup = string.IsNullOrEmpty(pGroupEntity.ActiveDirectoryGroup) ? "" : pGroupEntity.ActiveDirectoryGroup;
                        pGroupProfileEntity.AutoLockSeconds = Convert.ToInt32(Interaction.IIf(Convert.ToBoolean(pGroupEntity.AutoLockSeconds), pGroupEntity.AutoLockSeconds * 60, 0));
                        pGroupProfileEntity.AutoLogOffSeconds = Convert.ToInt32(Interaction.IIf(Convert.ToBoolean(pGroupEntity.AutoLogOffSeconds), pGroupEntity.AutoLogOffSeconds * 60, 0));

                        context.Entry(pGroupProfileEntity).State = EntityState.Modified;
                        await context.SaveChangesAsync();

                        model.ErrorType = "s";
                        model.ErrorMessage = "Changes made on selected group are updated successfully";
                    }
                    else if (await context.SecureGroups.AnyAsync(x => (x.GroupName.Trim().ToLower()) == (pGroupEntity.GroupName.Trim().ToLower())) == false)
                    {
                        newGroupEntity.GroupName = Convert.ToString(Interaction.IIf(pGroupEntity.GroupName is null, "", pGroupEntity.GroupName));
                        newGroupEntity.Description = Convert.ToString(Interaction.IIf(pGroupEntity.Description is null, "", pGroupEntity.Description));
                        newGroupEntity.ActiveDirectoryGroup = string.IsNullOrEmpty(pGroupEntity.ActiveDirectoryGroup) ? "" : pGroupEntity.ActiveDirectoryGroup;
                        newGroupEntity.AutoLockSeconds = Convert.ToInt32(Interaction.IIf(Convert.ToBoolean(pGroupEntity.AutoLockSeconds), pGroupEntity.AutoLockSeconds * 60, 0));
                        newGroupEntity.AutoLogOffSeconds = Convert.ToInt32(Interaction.IIf(Convert.ToBoolean(pGroupEntity.AutoLogOffSeconds), pGroupEntity.AutoLogOffSeconds * 60, 0));
                        newGroupEntity.GroupType = "USERGROUP";

                        context.SecureGroups.Add(newGroupEntity);
                        await context.SaveChangesAsync();

                        model.ErrorType = "s";
                        model.ErrorMessage = "New Group has been added into list of Groups successfully";
                    }
                    else
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = "This Group name has been already defined";
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

        [Route("EditGroupProfile")]
        [HttpGet]
        public async Task<string> EditGroupProfile(string ConnectionString, int GroupId) //completed testing
        {
            var jsonObject = string.Empty;

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pGroupProfileEntity = await context.SecureGroups.Where(x => x.GroupID == GroupId).FirstOrDefaultAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pGroupProfileEntity, Newtonsoft.Json.Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return jsonObject;
        }

        [Route("DeleteGroupProfile")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> DeleteGroupProfile(string ConnectionString, int GroupId) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pGroupProfileEntity = await context.SecureGroups.Where(x => x.GroupID == GroupId).FirstOrDefaultAsync();
                    var pUserGroupEntities = await context.SecureUserGroups.Where(x => x.GroupID == GroupId).ToListAsync();

                    context.SecureGroups.Remove(pGroupProfileEntity);
                    context.SecureUserGroups.RemoveRange(pUserGroupEntities);
                    await context.SaveChangesAsync();

                    model.ErrorType = "s";
                    model.ErrorMessage = "Selected Group has been deleted from the list of Groups successfully";
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

        [Route("GetAssignedUsersForGroup")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetAssignedUsersForGroup(string ConnectionString, int GroupId) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var lstUsers = new List<string>();
            List<SecureUserGroup> pSecureGroupUserEntities;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    pSecureGroupUserEntities = await context.SecureUserGroups.Where(x => x.GroupID == GroupId).ToListAsync();

                    foreach (SecureUserGroup item in pSecureGroupUserEntities)
                        lstUsers.Add(item.UserID.ToString());

                    var pSecureUserEntities = await (from t in context.SecureUsers
                                                     where lstUsers.Contains(t.UserID.ToString())
                                                     orderby t.UserName ascending
                                                     select t).ToListAsync();

                    foreach (var item in pSecureUserEntities)
                    {
                        item.SecureUserGroups.Clear();
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(pSecureUserEntities, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "msgAdminCtrlDataSavedSuccessfully";
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

        [Route("GetAllUsersList")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetAllUsersList(string ConnectionString) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            List<SecureUser> lstAllUsers;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    lstAllUsers = await context.SecureUsers.Where(x => x.UserID != -1).OrderBy(x => x.UserName).ToListAsync();

                    foreach (var item in lstAllUsers)
                    {
                        item.SecureUserGroups.Clear();
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(lstAllUsers, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data Retrieved.";
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

        [Route("SetUsersAgainstGroup")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetUsersAgainstGroup(SetUsersAgainstGroupParams setUsersAgainstGroupParams) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pSecureUserGroup = new SecureUserGroup();
            try
            {
                using (var context = new TABFusionRMSContext(setUsersAgainstGroupParams.ConnectionString))
                {
                    var pUserGrpEntities = await context.SecureUserGroups.Where(x => x.GroupID == setUsersAgainstGroupParams.GroupId).ToListAsync();
                    context.SecureUserGroups.RemoveRange(pUserGrpEntities);
                    await context.SaveChangesAsync();
                    if (setUsersAgainstGroupParams.UserList != null)
                    {
                        if (setUsersAgainstGroupParams.UserList.Length > 0)
                        {
                            foreach (var uid in setUsersAgainstGroupParams.UserList)
                            {
                                pSecureUserGroup.GroupID = setUsersAgainstGroupParams.GroupId;
                                pSecureUserGroup.UserID = Convert.ToInt32(uid);

                                context.SecureUserGroups.Add(pSecureUserGroup);
                                await context.SaveChangesAsync();
                            }
                        }
                        model.ErrorType = "s";
                        model.ErrorMessage = "Selected user/s are assigned to the Group successfully";
                    }
                    else
                    {
                        model.ErrorType = "s";
                        model.ErrorMessage = "Please select at least one group";
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

        #region Securables All Methods moved

        [Route("GetListOfSecurablesType")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetListOfSecurablesType(string ConnectionString) //Completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pSecureObjectEntity = await context.SecureObjects.Where(x => x.BaseID == 0 & x.SecureObjectID > 0 & x.Name.Substring(0, 1) != " ").OrderBy(x => x.Name).ToListAsync();

                    var secureObjectsList = new List<SecureObjectsReturn>();
                    foreach (var item in pSecureObjectEntity)
                    {
                        var secureObjectReturn = new SecureObjectsReturn
                        {
                            SecureObjectID = item.SecureObjectID,
                            Name = item.Name,
                            SecureObjectTypeID = item.SecureObjectTypeID,
                            BaseID = item.BaseID
                        };

                        secureObjectsList.Add(secureObjectReturn);
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(secureObjectsList, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data retrieved successfully";
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

        [Route("GetListOfSecurableObjects")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetListOfSecurableObjects(string ConnectionString, int SecurableTypeID) //Completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    List<SecureObjectPermission> sopData = null;
                    sopData = await context.SecureObjectPermissions.ToListAsync();
                    var pSecureObjectEntity = await (from o in context.SecureObjects
                                                     join v in context.SecureObjects on o.BaseID equals v.SecureObjectID into ov
                                                     let ParentName = ov.FirstOrDefault().Name
                                                     where o.BaseID != 0 & o.SecureObjectTypeID == SecurableTypeID & !o.Name.StartsWith("slRetention") & !o.Name.StartsWith("security")
                                                     orderby o.Name
                                                     select new { o.SecureObjectID, o.Name, o.SecureObjectTypeID, o.BaseID, ParentName }).ToListAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(pSecureObjectEntity, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data retrieved successfully";
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

        [Route("GetPermissionsForSecurableObject")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetPermissionsForSecurableObject(string ConnectionString, int SecurableObjID) //Completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var dtPermissions = new DataTable();
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var sql = "SP_RMS_GetPermissionInfoForSecurableObj";
                    var param = new DynamicParameters();
                    param.Add("@SecurableObjID", SecurableObjID);
                    var res = await conn.ExecuteReaderAsync(sql, param, commandType: CommandType.StoredProcedure);
                    if (res != null)
                        dtPermissions.Load(res);
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(dtPermissions, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data retrieved successfully";
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

        [Route("SetPermissionsToSecurableObject")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetPermissionsToSecurableObject(SetPermissionsToSecurableObjectParams setPermissionsToSecurableObjectParams) //Completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();
            var pSecurableObjIds = setPermissionsToSecurableObjectParams.SecurableObjIds;
            var pPermisionIds = setPermissionsToSecurableObjectParams.PermisionIds;
            var pPermissionRvmed = setPermissionsToSecurableObjectParams.PermissionRvmed;

            var pTempPermissionIds = new List<int>();
            var pPermissionEntity = new SecureObjectPermission();
            var pSecurableIdsForView = new List<Entities.SecureObject>();
            var connectionString = setPermissionsToSecurableObjectParams.Passport.ConnectionString;
            try
            {
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    if (pSecurableObjIds.Length > 0)
                    {
                        foreach (int pSecurableID in pSecurableObjIds)
                        {
                            // Add all permission Id's which are new.
                            if (!(pPermisionIds == null))
                            {
                                if (pPermisionIds.Count > 0)
                                {
                                    foreach (var pPermissionId in pPermisionIds)
                                    {
                                        if (!(await context.SecureObjectPermissions.AnyAsync(x => (x.GroupID == 0 && x.SecureObjectID == pSecurableID) & x.PermissionID == pPermissionId)))
                                        {
                                            // 'Assigned new permission
                                            AddNewSecureObjectPermission(pSecurableID, pPermissionId, connectionString);
                                        }
                                    }
                                }
                            }

                            // Remove permission ids
                            if (!(pPermissionRvmed == null))
                            {
                                if (pPermissionRvmed.Count > 0)
                                {
                                    // Remove all associated secure object permission for Views
                                    pSecurableIdsForView = await context.SecureObjects.Where(x => x.BaseID == pSecurableID & x.SecureObjectTypeID == (int)Enums.SecureObjectType.View).ToListAsync();
                                    if (pSecurableIdsForView.Count > 0)
                                    {
                                        foreach (var SecureObj in pSecurableIdsForView)
                                        {
                                            int SecureObjID = SecureObj.SecureObjectID;
                                            foreach (var pPermissionId in pPermissionRvmed)
                                            {
                                                if (!(pPermissionId == (int)Enums.PassportPermissions.Configure))
                                                {
                                                    if (await context.SecureObjectPermissions.AnyAsync(x => x.SecureObjectID == SecureObj.SecureObjectID & x.PermissionID == pPermissionId))
                                                    {
                                                        var SecureObjectPerEntities = await context.SecureObjectPermissions.Where(x => x.SecureObjectID == SecureObj.SecureObjectID & x.PermissionID == pPermissionId).ToListAsync();
                                                        context.SecureObjectPermissions.RemoveRange(SecureObjectPerEntities);
                                                        await context.SaveChangesAsync();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Remove all associated secure object permission for Annotation and Attachments - added by Ganesh 12/01/2016.
                                    var selectedSecureObj = await context.SecureObjects.Where(x => x.SecureObjectID == pSecurableID).FirstOrDefaultAsync();
                                    // Execute this code if current selected Secure Object is from TABLE.
                                    if (selectedSecureObj.SecureObjectTypeID == (int)Enums.SecureObjectType.Table)
                                    {
                                        var relatedAnnotationObj = await context.SecureObjects.Where(x => (x.Name) == (selectedSecureObj.Name) & (x.SecureObjectTypeID == (int)Enums.SecureObjectType.Annotations | x.SecureObjectTypeID == (int)Enums.SecureObjectType.Attachments)).ToListAsync();

                                        if (relatedAnnotationObj.Count > 0)
                                        {
                                            foreach (var relatedObj in relatedAnnotationObj)
                                            {
                                                foreach (var pPermissionId in pPermissionRvmed)
                                                {
                                                    if (await context.SecureObjectPermissions.AnyAsync(x => x.SecureObjectID == relatedObj.SecureObjectID & x.PermissionID == pPermissionId))
                                                    {
                                                        var SecureObjectPerEntities = await context.SecureObjectPermissions.Where(x => x.SecureObjectID == relatedObj.SecureObjectID & x.PermissionID == pPermissionId).ToListAsync();
                                                        context.SecureObjectPermissions.RemoveRange(SecureObjectPerEntities);
                                                        await context.SaveChangesAsync();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    foreach (var pPermissionId in pPermissionRvmed)
                                    {
                                        if (await context.SecureObjectPermissions.AnyAsync(x => x.SecureObjectID == pSecurableID & x.PermissionID == pPermissionId))
                                        {
                                            var SecureObjectPerEntities = await context.SecureObjectPermissions.Where(x => x.SecureObjectID == pSecurableID & x.PermissionID == pPermissionId).ToListAsync();
                                            context.SecureObjectPermissions.RemoveRange(SecureObjectPerEntities);
                                            await context.SaveChangesAsync();

                                            // 'Keep sync Tables -> Tracking tab Tracking Object and Allow Requiresting checkboxs and Security Securables tab
                                            // 'Removed permission updates in Tables table
                                            if (pPermissionId == 8 | pPermissionId == 9)
                                            {
                                                await UpdateTablesTrackingObject("D", pSecurableID, pPermissionId, connectionString);
                                            }
                                        }

                                        // START: Delete entries for My query and My Fav
                                        var SecureObject = await context.SecureObjects.Where(x => x.SecureObjectID == pSecurableID).FirstOrDefaultAsync();
                                        if (SecureObject.Name.Equals(Common.SECURE_MYQUERY))
                                        {
                                            RemovePreviousDataForMyQueryOrFavoriate(Common.SECURE_MYQUERY, connectionString);
                                        }
                                        else if (SecureObject.Name.Equals(Common.SECURE_MYFAVORITE))
                                        {
                                            RemovePreviousDataForMyQueryOrFavoriate(Common.SECURE_MYFAVORITE, connectionString);
                                        }
                                        // END: Delete entries for My query and My Fav
                                    }
                                }
                            }
                        }
                        setPermissionsToSecurableObjectParams.Passport.FillSecurePermissions();
                        model.ErrorType = "s";
                        model.ErrorMessage = "Permissions saved successully";
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

        #region Permissions All Methods moved 

        [Route("GetPermisionsGroupList")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetPermisionsGroupList(string ConnectionString) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pSecureGroupEntity = await context.SecureGroups.Where(x => x.GroupID != 0).OrderBy(x => x.GroupName).ToListAsync();

                    foreach (var item in pSecureGroupEntity)
                    {
                        item.SecureObjectPermissions.Clear();
                        item.SecureUserGroups.Clear();
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(pSecureGroupEntity, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data retrieved successfully";
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

        [Route("GetListOfSecurableObjForPermissions")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetListOfSecurableObjForPermissions(string ConnectionString, int SecurableTypeID) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var dtSecurables = new DataTable();
                    var query = "SP_RMS_GetListOfSecurablesById";
                    var param = new DynamicParameters();
                    param.Add("@GroupID", 0);
                    param.Add("@SecurableTypeID", SecurableTypeID);

                    var res = await conn.ExecuteReaderAsync(query, param, commandType: CommandType.StoredProcedure);
                    if (res != null)
                        dtSecurables.Load(res);

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(dtSecurables, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data retrieved successfully";
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

        [Route("GetPermissionsBasedOnGroupId")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetPermissionsBasedOnGroupId(string ConnectionString, int GroupID, int SecurableObjID) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var dt = new DataTable();
                    var query = "SP_RMS_GetPermissionInfoObjBasedOnGroup";
                    var param = new DynamicParameters();
                    param.Add("@GroupID", GroupID);
                    param.Add("@SecurableObjID", SecurableObjID);

                    var res = await conn.ExecuteReaderAsync(query, param, commandType: CommandType.StoredProcedure);
                    if (res != null)
                        dt.Load(res);

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue1 = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented, Setting);

                    model.ErrorType = "s";
                    model.ErrorMessage = "Data retrieved successfully";
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

        [Route("SetGroupPermissions")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetGroupPermissions(SetGroupPermissionsParams setGroupPermissionsParams) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();
            var connectionString = setGroupPermissionsParams.Passport.ConnectionString;
            var pGroupIds = setGroupPermissionsParams.GroupIds;
            var pSecurableObjIds = setGroupPermissionsParams.SecurableObjIds;
            var pPermisionIds = setGroupPermissionsParams.PermisionIds;

            try
            {
                List<SecureObjectPermission> pSecurableObjectList = new List<SecureObjectPermission>();
                var pTempPermissionIds = new List<int>();
                var pPermissionEntity = new SecureObjectPermission();

                using (var context = new TABFusionRMSContext(connectionString))
                {
                    if (pSecurableObjIds.Length > 0)
                    {
                        foreach (int pGroupId in pGroupIds)
                        {
                            foreach (int pSecurableID in pSecurableObjIds)
                            {

                                pSecurableObjectList = await context.SecureObjectPermissions.Where(x => x.SecureObjectID == pSecurableID & x.GroupID == pGroupId).ToListAsync();
                                if (!(pPermisionIds == null))
                                {
                                    pTempPermissionIds.Clear();
                                    pTempPermissionIds.AddRange(pPermisionIds);
                                }

                                // Check for new permission ids and remove from list if exists system already.
                                foreach (var pSecurableObject in pSecurableObjectList)
                                {
                                    if (pTempPermissionIds.Count > 0)
                                    {
                                        var resfind = pTempPermissionIds.Find((x) => Convert.ToInt32(x) == pSecurableObject.PermissionID);
                                        if (resfind > 0)
                                        {
                                            pTempPermissionIds.Remove(resfind);
                                            await context.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            context.SecureObjectPermissions.Remove(pSecurableObject);
                                            await context.SaveChangesAsync();
                                        }
                                    }
                                    else if (pTempPermissionIds.Count == 0)
                                    {
                                        context.SecureObjectPermissions.Remove(pSecurableObject);
                                        await context.SaveChangesAsync();
                                    }

                                    // START: Delete entries for My query and My Fav
                                    var SecureObject = await context.SecureObjects.Where(x => x.SecureObjectID == pSecurableID).FirstOrDefaultAsync();
                                    if (SecureObject.Name.Equals(Common.SECURE_MYQUERY))
                                    {
                                        RemovePreviousDataForMyQueryOrFavoriate(Common.SECURE_MYQUERY, connectionString, pGroupId);
                                    }
                                    else if (SecureObject.Name.Equals(Common.SECURE_MYFAVORITE))
                                    {
                                        RemovePreviousDataForMyQueryOrFavoriate(Common.SECURE_MYFAVORITE, connectionString, pGroupId);
                                    }
                                    // END: Delete entries for My query and My Fav
                                }
                                // Get the new permissions ids and Insert those into system.
                                foreach (var pPermissionId in pTempPermissionIds)
                                {
                                    pPermissionEntity.GroupID = pGroupId;
                                    pPermissionEntity.SecureObjectID = pSecurableID;
                                    pPermissionEntity.PermissionID = pPermissionId;

                                    context.SecureObjectPermissions.Add(pPermissionEntity);
                                    await context.SaveChangesAsync();
                                }
                            }
                        }

                        // Reload the permissions dataset after updation of permissions.
                        setGroupPermissionsParams.Passport.FillSecurePermissions();

                        model.ErrorType = "s";
                        model.ErrorMessage = "Permissions saved successully";
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

        #endregion

        #region Report Style All Methods Moved 

        [Route("GetReportStyles")]
        [HttpGet]
        public string GetReportStyles(string ConnectionString, string sord, int page, int rows) //completed testing
        {
            var jsonObject = string.Empty;

            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var reportEntity = context.ReportStyles.OrderBy(m => m.ReportStylesId);

                if (reportEntity == null)
                {
                    return null;
                }
                else
                {
                    var setting = new JsonSerializerSettings();
                    setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(reportEntity.GetJsonListForGrid(sord, page, rows, "Id"), Newtonsoft.Json.Formatting.Indented, setting);
                }
            }
            return jsonObject;
        }

        [Route("GetReportStylesData")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> GetReportStylesData(string ConnectionString, string reportStyleVar, int selectedRowsVar = 0, bool cloneFlag = false) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    object reportStyleEntity;
                    object allReportStyle = null;
                    if (selectedRowsVar != 0)
                    {
                        reportStyleEntity = await context.ReportStyles.Where(m => m.Id.Equals(reportStyleVar.Trim()) & m.ReportStylesId.Equals(selectedRowsVar)).FirstOrDefaultAsync();
                    }
                    else
                    {
                        reportStyleEntity = await context.ReportStyles.Where(m => m.Id.Equals(reportStyleVar.Trim())).FirstOrDefaultAsync();
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    model.stringValue2 = JsonConvert.SerializeObject(reportStyleEntity, Newtonsoft.Json.Formatting.Indented, Setting);
                    if (cloneFlag)
                    {
                        allReportStyle = await context.ReportStyles.OrderBy(m => m.Id).ToListAsync();
                        bool bFound = false;
                        int iNextReport = 0;
                        string sReportStyleName = "New Report Style";
                        do
                        {
                            bFound = false;
                            if (iNextReport == 0)
                            {
                                sReportStyleName = "New Report Style";
                            }
                            else
                            {
                                sReportStyleName = "New Report Style " + iNextReport;
                            }

                            foreach (ReportStyle oReportStyle in (IEnumerable)allReportStyle)
                            {
                                if (Strings.StrComp(oReportStyle.Id.Trim().ToLower(), sReportStyleName.Trim().ToLower(), Constants.vbTextCompare) == 0)
                                {
                                    iNextReport = iNextReport + 1;
                                    sReportStyleName = oReportStyle.Id;
                                    bFound = true;
                                    break;
                                }
                            }
                            if (!bFound)
                            {
                                break;
                            }
                        }
                        while (true);

                        model.stringValue1 = JsonConvert.SerializeObject(sReportStyleName, Newtonsoft.Json.Formatting.Indented, Setting);
                        return model;
                    }
                    else
                    {
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return model;
        }

        [Route("RemoveReportStyle")]
        [HttpDelete]
        public async Task<ReturnErrorTypeErrorMsg> RemoveReportStyle(string ConnectionString, int selectedRowsVar, string reportStyleVar) //completed testing
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var reportStyleEntity = await context.ReportStyles.Where(m => m.Id.Trim().ToLower().Equals(reportStyleVar.Trim().ToLower()) & m.ReportStylesId == selectedRowsVar).FirstOrDefaultAsync();
                    if (reportStyleEntity != null)
                    {
                        context.ReportStyles.Remove(reportStyleEntity);
                        await context.SaveChangesAsync();
                    }

                    model.ErrorMessage = "Selected Reported Style has been deleted Successfully";
                    model.ErrorType = "s";
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

        [Route("SetReportStylesData")]
        [HttpPost]
        public async Task<ReturnErrorTypeErrorMsg> SetReportStylesData(ReportStyle formEntity, string ConnectionString, bool pFixedLines, bool pAltRowShading, bool pReportCentered) //completed testing 
        {
            var model = new ReturnErrorTypeErrorMsg();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    formEntity.FixedLines = pFixedLines;
                    formEntity.AltRowShading = pAltRowShading;
                    formEntity.ReportCentered = pReportCentered;
                    if (formEntity.ReportStylesId > 0)
                    {
                        var reportStyleEntity = await context.ReportStyles.Where(m => m.ReportStylesId == formEntity.ReportStylesId).FirstOrDefaultAsync();
                        if (!string.IsNullOrEmpty(reportStyleEntity.Id))
                        {
                            if (!reportStyleEntity.Id.Trim().ToLower().Equals(formEntity.Id.Trim().ToLower()))
                            {
                                var reportStyleAll = await context.ReportStyles.OrderBy(m => m.ReportStylesId).ToListAsync();
                                foreach (ReportStyle reportObj in reportStyleAll)
                                {
                                    if (reportObj.Id.Trim().ToLower().Equals(formEntity.Id.Trim().ToLower()) & reportObj.ReportStylesId != formEntity.ReportStylesId)
                                    {
                                        model.ErrorType = "w";
                                        model.ErrorMessage = string.Format("A Report Style already exists with the name {0}. Please select a different name.", reportObj.Id);
                                        return model;
                                    }
                                }
                            }
                        }

                        reportStyleEntity = AddReportStyle(reportStyleEntity, formEntity);
                        context.Entry(reportStyleEntity).State = EntityState.Modified;
                        await context.SaveChangesAsync();

                        model.ErrorType = "s";
                        model.ErrorMessage = "Selected Reported Style properties has been updated Successfully";
                    }
                    else
                    {
                        context.ReportStyles.Add(formEntity);
                        await context.SaveChangesAsync();
                        model.ErrorType = "s";
                        model.ErrorMessage = "Record saved successfully";
                    }
                    var reportBack = await context.ReportStyles.Where(m => m.Id.Trim().ToLower().Equals(formEntity.Id.Trim().ToLower())).FirstOrDefaultAsync();
                    model.stringValue1 = JsonConvert.SerializeObject(reportBack, Newtonsoft.Json.Formatting.Indented, Setting);
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

        private ReportStyle AddReportStyle(ReportStyle reportStyleEntity, ReportStyle formEntity)
        {
            reportStyleEntity.ReportStylesId = formEntity.ReportStylesId;
            reportStyleEntity.Id = formEntity.Id;
            reportStyleEntity.Description = formEntity.Description;
            reportStyleEntity.Heading1Left = formEntity.Heading1Left;
            reportStyleEntity.Heading1Center = formEntity.Heading1Center;
            reportStyleEntity.Heading1Right = formEntity.Heading1Right;
            reportStyleEntity.Heading2Center = formEntity.Heading2Center;
            reportStyleEntity.FooterLeft = formEntity.FooterLeft;
            reportStyleEntity.FooterCenter = formEntity.FooterCenter;
            reportStyleEntity.FooterRight = formEntity.FooterRight;
            reportStyleEntity.Orientation = formEntity.Orientation;
            reportStyleEntity.HeaderSize = formEntity.HeaderSize;
            reportStyleEntity.ShadowSize = formEntity.ShadowSize;
            reportStyleEntity.MinColumnWidth = formEntity.MinColumnWidth;
            reportStyleEntity.BlankLineSpacing = formEntity.BlankLineSpacing;
            reportStyleEntity.ColumnSpacing = formEntity.ColumnSpacing;
            reportStyleEntity.BoxWidth = formEntity.BoxWidth;
            reportStyleEntity.MaxLines = formEntity.MaxLines;
            reportStyleEntity.FixedLines = formEntity.FixedLines;
            reportStyleEntity.AltRowShading = formEntity.AltRowShading;
            reportStyleEntity.ReportCentered = formEntity.ReportCentered;
            reportStyleEntity.TextForeColor = formEntity.TextForeColor;
            reportStyleEntity.LineColor = formEntity.LineColor;
            reportStyleEntity.ShadeBoxColor = formEntity.ShadeBoxColor;
            reportStyleEntity.ShadowColor = formEntity.ShadowColor;
            reportStyleEntity.ShadedLineColor = formEntity.ShadedLineColor;
            reportStyleEntity.LeftMargin = formEntity.LeftMargin;
            reportStyleEntity.RightMargin = formEntity.RightMargin;
            reportStyleEntity.TopMargin = formEntity.TopMargin;
            reportStyleEntity.BottomMargin = formEntity.BottomMargin;
            reportStyleEntity.HeadingL1FontBold = formEntity.HeadingL1FontBold;
            reportStyleEntity.HeadingL1FontItalic = formEntity.HeadingL1FontItalic;
            reportStyleEntity.HeadingL1FontUnderlined = formEntity.HeadingL1FontUnderlined;
            reportStyleEntity.HeadingL1FontSize = formEntity.HeadingL1FontSize;
            reportStyleEntity.HeadingL1FontName = formEntity.HeadingL1FontName;

            reportStyleEntity.HeadingL2FontBold = formEntity.HeadingL2FontBold;
            reportStyleEntity.HeadingL2FontItalic = formEntity.HeadingL2FontItalic;
            reportStyleEntity.HeadingL2FontUnderlined = formEntity.HeadingL2FontUnderlined;
            reportStyleEntity.HeadingL2FontSize = formEntity.HeadingL2FontSize;
            reportStyleEntity.HeadingL2FontName = formEntity.HeadingL2FontName;


            reportStyleEntity.SubHeadingFontBold = formEntity.SubHeadingFontBold;
            reportStyleEntity.SubHeadingFontItalic = formEntity.SubHeadingFontItalic;
            reportStyleEntity.SubHeadingFontUnderlined = formEntity.SubHeadingFontUnderlined;
            reportStyleEntity.SubHeadingFontName = formEntity.SubHeadingFontName;
            reportStyleEntity.SubHeadingFontSize = formEntity.SubHeadingFontSize;

            reportStyleEntity.ColumnHeadingFontBold = formEntity.ColumnHeadingFontBold;
            reportStyleEntity.ColumnHeadingFontItalic = formEntity.ColumnHeadingFontItalic;
            reportStyleEntity.ColumnHeadingFontUnderlined = formEntity.ColumnHeadingFontUnderlined;
            reportStyleEntity.ColumnHeadingFontName = formEntity.ColumnHeadingFontName;
            reportStyleEntity.ColumnHeadingFontSize = formEntity.ColumnHeadingFontSize;

            reportStyleEntity.ColumnFontBold = formEntity.ColumnFontBold;
            reportStyleEntity.ColumnFontItalic = formEntity.ColumnFontItalic;
            reportStyleEntity.ColumnFontUnderlined = formEntity.ColumnFontUnderlined;
            reportStyleEntity.ColumnFontName = formEntity.ColumnFontName;
            reportStyleEntity.ColumnFontSize = formEntity.ColumnFontSize;

            reportStyleEntity.FooterFontBold = formEntity.FooterFontBold;
            reportStyleEntity.FooterFontItalic = formEntity.FooterFontItalic;
            reportStyleEntity.FooterFontUnderlined = formEntity.FooterFontUnderlined;
            reportStyleEntity.FooterFontName = formEntity.FooterFontName;
            reportStyleEntity.FooterFontSize = formEntity.FooterFontSize;

            return reportStyleEntity;
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

                    lstTblNamesList = JsonConvert.SerializeObject(lstTblNames, Newtonsoft.Json.Formatting.Indented, Setting);
                    lstReportStylesList = JsonConvert.SerializeObject(lstReportStyles, Newtonsoft.Json.Formatting.Indented, Setting);
                    lstChildTablesObjStr = JsonConvert.SerializeObject(lstChildTables, Newtonsoft.Json.Formatting.Indented, Setting);

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
                            var res = await DeleteExtraTrackingHistory(ConnectionString, sTableName, sId);
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

        private static string GenerateKey(bool boolFlag, string pwdString = null, byte[] pwdByteArray = null)
        {
            string transformPwd;
            try
            {
                using (var myRijndael = new RijndaelManaged())
                {
                    var pdb = new Rfc2898DeriveBytes("RandomKey", Encoding.ASCII.GetBytes("SaltValueMustBeUnique"));
                    myRijndael.Key = pdb.GetBytes(32);
                    myRijndael.IV = pdb.GetBytes(16);
                    if (boolFlag)
                    {
                        transformPwd = EncryptString(pwdString, myRijndael.Key, myRijndael.IV);
                    }
                    else
                    {
                        transformPwd = DecryptString(pwdByteArray, myRijndael.Key, myRijndael.IV);
                    }
                    return transformPwd;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Convert.ToString(false);
            }
        }

        public static string EncryptString(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText is null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key is null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV is null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }
            try
            {
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Key = Key;
                    rijAlg.IV = IV;
                    var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                    var msEncrypt = new System.IO.MemoryStream();
                    // Using msEncrypt As New IO.MemoryStream()
                    var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                    // Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                    using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Encoding.Default.GetString(msEncrypt.ToArray());
                    // End Using
                    // End Using
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Convert.ToString(false);
            }

        }

        private static string DecryptString(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText is null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (Key is null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV is null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }
            try
            {
                string plaintext = null;
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Key = Key;
                    rijAlg.IV = IV;
                    var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                    var msDecrypt = new System.IO.MemoryStream(cipherText);
                    // Using msDecrypt As New IO.MemoryStream(cipherText)
                    var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    // Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
                    using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                    // End Using
                    // End Using
                }
                return plaintext;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Convert.ToString(false);
            }

        }

        private async Task<string> VerifyRetentionDispositionTypesForParentAndChildren(string ConnectionString, int pTableId)
        {
            string sMessage = string.Empty;            Table oTable;            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pTableEntites = await context.Tables.Where(x => x.TableId.Equals(pTableId)).FirstOrDefaultAsync();
                    var lstRelatedTables = await context.RelationShips.Where(x => (x.LowerTableName) == (pTableEntites.TableName)).ToListAsync();
                    var lstRelatedChildTable = await context.RelationShips.Where(x => (x.UpperTableName) == (pTableEntites.TableName)).ToListAsync();

                    if (pTableEntites.RetentionFinalDisposition != 0)
                    {

                        foreach (var lTableName in lstRelatedTables)
                        {

                            oTable = await context.Tables.Where(x => x.TableName.Equals(lTableName.UpperTableName)).FirstOrDefaultAsync();

                            if (oTable is not null)
                            {
                                if (((oTable.RetentionPeriodActive == true) || (oTable.RetentionInactivityActive == true)) && (oTable.RetentionFinalDisposition != 0))
                                {
                                    if (oTable.RetentionFinalDisposition != pTableEntites.RetentionFinalDisposition)
                                        sMessage = Constants.vbTab + Constants.vbTab + oTable.UserName + Constants.vbCrLf;
                                }
                                oTable = null;
                            }

                        }

                        foreach (var lTableName in lstRelatedChildTable)
                        {

                            oTable = await context.Tables.Where(x => x.TableName.Equals(lTableName.LowerTableName)).FirstOrDefaultAsync();

                            if (oTable is not null)
                            {
                                if (((oTable.RetentionPeriodActive == true) || (oTable.RetentionInactivityActive == true)) && (oTable.RetentionFinalDisposition != 0))
                                {
                                    if ((oTable.RetentionFinalDisposition != pTableEntites.RetentionFinalDisposition))
                                        sMessage = Constants.vbTab + Constants.vbTab + oTable.UserName + Constants.vbCrLf;
                                }
                                oTable = null;
                            }

                        }

                        if (!string.IsNullOrEmpty(sMessage))
                        {
                            sMessage = string.Format("<b>WARNING:</b>;  The following related tables have a retention disposition set differently than this table: <b>{1}</b>; {0} This could give different results than expected. {0};Please correct the appropriate table if this is not what is intended.", Environment.NewLine, sMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return sMessage;
        }

        private bool SaveNewFieldToTable(string sTableName, string sFieldName, Enums.DataTypeEnum FieldType, int iViewsId, string ConnectionString)
        {
            string sSQL;

            sFieldName = Strings.Replace(sFieldName, "* ", "");
            sSQL = "ALTER TABLE [" + sTableName + "]";

            switch (FieldType)
            {
                case Enums.DataTypeEnum.rmDate:
                case Enums.DataTypeEnum.rmDBDate:
                case Enums.DataTypeEnum.rmDBTime:
                    {
                        sSQL = sSQL + " ADD [" + Strings.Trim(sFieldName) + "] DATETIME NULL";
                        break;
                    }
                case Enums.DataTypeEnum.rmBoolean:
                    {
                        sSQL = sSQL + " ADD [" + Strings.Trim(sFieldName) + "] BIT";
                        break;
                    }

                default:
                    {
                        sSQL = sSQL + " ADD [" + Strings.Trim(sFieldName) + "] VARCHAR(20) NULL";
                        break;
                    }
            }

            SQLViewDelete(iViewsId, ConnectionString);

            try
            {
                return Convert.ToInt32(ExecuteSqlCommand(ConnectionString, sSQL, false)) > -1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void SQLViewDelete(int Id, string ConnectionString)
        {
            string sql = string.Format("IF OBJECT_ID('view__{0}', 'V') IS NOT NULL DROP VIEW [view__{0}]", Id.ToString());
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    await conn.ExecuteAsync(sql, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
        }

        private bool ExecuteSqlCommand(string ConnectionString, string sSQL, bool bDoNoCount = false)
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

        private async Task AddNewSecureObjectPermission(int secureObjectId, int securePermissionId, string ConnectionString)
        {
            var secoreObjPermissionObj = new SecureObjectPermission();
            
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                secoreObjPermissionObj.GroupID = 0;
                secoreObjPermissionObj.SecureObjectID = secureObjectId;
                secoreObjPermissionObj.PermissionID = securePermissionId;
                
                context.SecureObjectPermissions.Add(secoreObjPermissionObj);
                await context.SaveChangesAsync();
                
                if (securePermissionId == 8 | securePermissionId == 9)
                {
                    await UpdateTablesTrackingObject("A", secureObjectId, securePermissionId, ConnectionString);
                }
            }
        }

        private async Task UpdateTablesTrackingObject(string action, int secureObjectId, int securePermissionId, string ConnectionString)
        {

            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var SecureObject = await context.SecureObjects.Where(m => m.SecureObjectID == secureObjectId).FirstOrDefaultAsync();
                if (SecureObject != null)
                {
                    var Tables = await context.Tables.Where(m => m.TableName.Trim().ToLower().Equals(SecureObject.Name.Trim().ToLower())).FirstOrDefaultAsync();
                    if (Tables != null)
                    {
                        if (securePermissionId == 8)
                        {
                            Tables.Trackable = (bool?)Interaction.IIf(action == "A", true, false);
                        }
                        if (securePermissionId == 9)
                        {
                            Tables.AllowBatchRequesting = (bool?)Interaction.IIf(action == "A", true, false);
                        }
                        context.Entry(Tables).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        private async void RemovePreviousDataForMyQueryOrFavoriate(string typeOfFunctionality, string connectionString, int pGroupId = 0)
        {
            using (var context = new TABFusionRMSContext(connectionString))
            {
                if (pGroupId == 0)
                {
                    // Handle removing data if action performed on "Securable" section.
                    if (typeOfFunctionality.Equals(Common.SECURE_MYQUERY))
                    {
                        var allSavedCritriaForMyQuery = await context.s_SavedCriteria.Where(x => x.SavedType == 0).ToListAsync();

                        foreach (var savedCritria in allSavedCritriaForMyQuery)
                        {
                            var savedChildrenQuery = await context.s_SavedChildrenQuery.Where(q => q.SavedCriteriaId == savedCritria.Id).ToListAsync();
                            context.s_SavedChildrenQuery.RemoveRange(savedChildrenQuery);
                            await context.SaveChangesAsync();
                        }
                        context.s_SavedCriteria.RemoveRange(allSavedCritriaForMyQuery);
                        await context.SaveChangesAsync();
                    }
                    else if (typeOfFunctionality.Equals(Common.SECURE_MYFAVORITE))
                    {
                        var allSavedCritriaForMyFav = await context.s_SavedCriteria.Where(x => x.SavedType == 1).ToListAsync();

                        foreach (var savedCritria in allSavedCritriaForMyFav)
                        {
                            var savedChildrenFav = await context.s_SavedChildrenQuery.Where(q => q.SavedCriteriaId == savedCritria.Id).ToListAsync();
                            context.s_SavedChildrenQuery.RemoveRange(savedChildrenFav);
                            await context.SaveChangesAsync();
                        }
                        context.s_SavedCriteria.RemoveRange(allSavedCritriaForMyFav);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {

                    // 'Handle remove data if action performed on "Permissions" section.
                    var lstUsersUnderGrpBeingDel = await context.SecureUserGroups.Where(x => x.GroupID == pGroupId).Select(y => y.UserID).ToListAsync();
                    var allSavedCriteria = await context.s_SavedCriteria.Select(x => x.UserId).Distinct().ToListAsync();
                    int secureObjectIdForMyQuery = await context.SecureObjects.Where(x => (x.Name) == Common.SECURE_MYQUERY).Select(y => y.SecureObjectID).FirstOrDefaultAsync();
                    int secureObjectIdForMyFav = await context.SecureObjects.Where(x => (x.Name) == Common.SECURE_MYFAVORITE).Select(y => y.SecureObjectID).FirstOrDefaultAsync();
                    bool IsMyQueryForEveryone = await context.SecureObjectPermissions.AnyAsync(x => x.GroupID == -1 & x.SecureObjectID == secureObjectIdForMyQuery);
                    bool IsMyFavForEveryone = await context.SecureObjectPermissions.AnyAsync(x => x.GroupID == -1 & x.SecureObjectID == secureObjectIdForMyFav);
                    int cntOfGrpUserPartOf = 0;
                    string SQLQuery;

                    using (var conn = CreateConnection(connectionString))
                    {
                        foreach (var userid in lstUsersUnderGrpBeingDel)
                        {
                            if (allSavedCriteria.Contains(userid))
                            {
                                if (typeOfFunctionality.Equals(Common.SECURE_MYQUERY))
                                {
                                    SQLQuery = string.Format(@"SELECT COUNT(SUG.GroupId) as cntGroups FROM SecureUser SU                                                                INNER JOIN SecureUserGroup SUG ON SU.UserID = SUG.UserID                                                                INNER JOIN SecureObjectPermission SOG ON SUG.GroupID = SOG.GroupID                                                                WHERE SU.UserID = {0} AND SecureObjectID = (SELECT SecureObjectID FROM SecureObject WHERE Name = '{1}')", userid, Common.SECURE_MYQUERY);

                                    cntOfGrpUserPartOf = Convert.ToInt32(await conn.ExecuteScalarAsync(SQLQuery, commandType: CommandType.Text));

                                    if (!IsMyQueryForEveryone)
                                    {
                                        // If user is not part of other group with 'My Queries' permission, then DELETE "Saved Queries" for that user.
                                        if (cntOfGrpUserPartOf == 0)
                                        {
                                            // Delete entries of my query for user under current group
                                            var allSavedCritriaForMyQuery = await context.s_SavedCriteria.Where(x => x.SavedType == 0 && x.UserId == userid).ToListAsync();

                                            foreach (var savedCritria in allSavedCritriaForMyQuery)
                                            {
                                                var savedChildrenQuery = await context.s_SavedChildrenQuery.Where(q => q.SavedCriteriaId == savedCritria.Id).ToListAsync();
                                                context.s_SavedChildrenQuery.RemoveRange(savedChildrenQuery);
                                                await context.SaveChangesAsync();
                                            }
                                            context.s_SavedCriteria.RemoveRange(allSavedCritriaForMyQuery);
                                            await context.SaveChangesAsync();
                                        }
                                    }
                                }
                                else if (typeOfFunctionality.Equals(Common.SECURE_MYFAVORITE))
                                {
                                    try
                                    {
                                        SQLQuery = string.Format(@"SELECT COUNT(SUG.GroupId) as cntGroups FROM SecureUser SU                                                                INNER JOIN SecureUserGroup SUG ON SU.UserID = SUG.UserID                                                                INNER JOIN SecureObjectPermission SOG ON SUG.GroupID = SOG.GroupID                                                                WHERE SU.UserID = {0} AND SecureObjectID = (SELECT SecureObjectID FROM SecureObject WHERE Name = '{1}')", userid, Common.SECURE_MYFAVORITE);

                                        cntOfGrpUserPartOf = Convert.ToInt32(await conn.ExecuteScalarAsync(SQLQuery, commandType: CommandType.Text));
                                        if (!IsMyFavForEveryone)
                                        {
                                            // If user is not part of other group with 'My Favorites' permission, then DELETE "Saved Favorites" for that user.
                                            if (cntOfGrpUserPartOf == 0)
                                            {
                                                // Delete entries of my favorites for user under current group
                                                var allSavedCritriaForMyFav = await context.s_SavedCriteria.Where(x => x.SavedType == 1 && x.UserId == userid).ToListAsync();
                                                foreach (var savedCritria in allSavedCritriaForMyFav)
                                                {
                                                    var savedChildrenFav = await context.s_SavedChildrenFavorite.Where(q => q.SavedCriteriaId == savedCritria.Id).ToListAsync();
                                                    context.s_SavedChildrenFavorite.RemoveRange(savedChildrenFav);
                                                    await context.SaveChangesAsync();
                                                }
                                                context.s_SavedCriteria.RemoveRange(allSavedCritriaForMyFav);
                                                await context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        //Keys.ErrorType = "e";
                                        //Keys.ErrorMessage = Keys.ErrorMessageJS();
                                    }
                                }
                            }
                            cntOfGrpUserPartOf = 0;
                            SQLQuery = "";
                        }
                        // 'Handle Scenario where users don't have My Query/My Fav permission with assigned group(s), but everyone group had permission.
                        if (typeOfFunctionality.Equals(Common.SECURE_MYQUERY))
                        {
                            if (!IsMyQueryForEveryone & pGroupId == -1)
                            {
                                var dtUserIds = new DataTable();

                                SQLQuery = string.Format(@"                                SELECT Distinct UserId FROM s_SavedCriteria                                 WHERE UserId NOT IN (                                        SELECT SU.UserID FROM SecureUser SU                                        INNER JOIN SecureUserGroup SUG ON SU.UserID = SUG.UserID                                        INNER JOIN SecureObjectPermission SOG ON SUG.GroupID = SOG.GroupID        	                            INNER JOIN SecureGroup SG ON SUG.GroupID = SG.GroupID            	                            AND SecureObjectID = (SELECT SecureObjectID FROM SecureObject WHERE Name = '{0}')                                )", Common.SECURE_MYQUERY);

                                var res = await conn.ExecuteReaderAsync(SQLQuery, commandType: CommandType.Text);
                                if (res != null)
                                    dtUserIds.Load(res);

                                foreach (DataRow useridRow in dtUserIds.Rows)
                                {
                                    int userid = Convert.ToInt32(useridRow["UserId"]);
                                    var allSavedCritriaForMyQuery = await context.s_SavedCriteria.Where(x => x.SavedType == 1 && x.UserId == userid).ToListAsync();

                                    foreach (var savedCritria in allSavedCritriaForMyQuery)
                                    {
                                        var savedChildrenQuery = await context.s_SavedChildrenQuery.Where(q => q.SavedCriteriaId == savedCritria.Id).ToListAsync();
                                        context.s_SavedChildrenQuery.RemoveRange(savedChildrenQuery);
                                        await context.SaveChangesAsync();
                                    }
                                    context.s_SavedCriteria.RemoveRange(allSavedCritriaForMyQuery);
                                    await context.SaveChangesAsync();
                                }
                            }
                        }
                        else if (typeOfFunctionality.Equals(Common.SECURE_MYFAVORITE))
                        {

                            if (!IsMyFavForEveryone & pGroupId == -1)
                            {
                                var dsUserIds = new DataSet();
                                var dtUserIds = new DataTable();

                                SQLQuery = string.Format(@"                                SELECT Distinct UserId FROM s_SavedCriteria                                 WHERE UserId NOT IN (                                        SELECT SU.UserID FROM SecureUser SU                                        INNER JOIN SecureUserGroup SUG ON SU.UserID = SUG.UserID                                        INNER JOIN SecureObjectPermission SOG ON SUG.GroupID = SOG.GroupID        	                            INNER JOIN SecureGroup SG ON SUG.GroupID = SG.GroupID            	                            AND SecureObjectID = (SELECT SecureObjectID FROM SecureObject WHERE Name = '{0}')                                )", Common.SECURE_MYFAVORITE);

                                var res = await conn.ExecuteReaderAsync(SQLQuery, commandType: CommandType.Text);
                                if (res != null)
                                    dtUserIds.Load(res);

                                foreach (DataRow useridRow in dtUserIds.Rows)
                                {
                                    int userid = Convert.ToInt32(useridRow["UserId"]);
                                    var allSavedCritriaForMyFav = await context.s_SavedCriteria.Where(x => x.SavedType == 1 && x.UserId == userid).ToListAsync();

                                    foreach (var savedCritria in allSavedCritriaForMyFav)
                                    {
                                        var savedChildrenFav = await context.s_SavedChildrenFavorite.Where(q => q.SavedCriteriaId == savedCritria.Id).ToListAsync();
                                        context.s_SavedChildrenFavorite.RemoveRange(savedChildrenFav);
                                        await context.SaveChangesAsync();
                                    }
                                    context.s_SavedCriteria.RemoveRange(allSavedCritriaForMyFav);
                                    await context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion
    }
}
