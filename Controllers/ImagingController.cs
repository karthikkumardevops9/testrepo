using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Imaging;
using MSRecordsEngine.Services;
using MSRecordsEngine.Models;
using Smead.Security;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json.Bson;
using System.Threading.Tasks;
namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagingController : ControllerBase
    {
        private readonly CommonControllersService<ImagingController> _commonService;
        public ImagingController(CommonControllersService<ImagingController> commonControllersService)
        {
            _commonService = commonControllersService;
        }

        [Route("SaveNewAttachmentInPopupWindow")]
        [HttpPost]
        public async Task SaveNewAttachmentInPopupWindow(SaveNewAttachmentInPopupWindow param)
        {
            try
            {
                var ticket = param.passport.get_CreateTicket(string.Format(@"{0}\{1}", param.passport.ServerName, param.passport.DatabaseName), param.model.tabName, param.model.tableId).ToString();
                // Dim oDefaultOutputSetting = _iSystem.All.FirstOrDefault.DefaultOutputSettingsId
                string oDefaultOutputSetting = string.Empty;
                var filesinfo = await _commonService.Microservices.DocumentServices.GetCodecInfoFromFileList(param.liststring);
                foreach (var item in filesinfo)
                {
                    if (item.Ispcfile)
                    {
                        Attachments.AddAnAttachment(_commonService.GetClientIpAddress(), ticket, param.passport.UserId, param.passport, param.model.tabName, param.model.tableId, 0, oDefaultOutputSetting, item.FilePath, item.FilePath, Path.GetExtension(item.FilePath), false, param.model.name, false, 1, 0, 0, 0);
                    }
                    else
                    {
                        Attachments.AddAnAttachment(_commonService.GetClientIpAddress(), ticket, param.passport.UserId, param.passport, param.model.tabName, param.model.tableId, 0, oDefaultOutputSetting, item.FilePath, item.FilePath, Path.GetExtension(item.FilePath), false, param.model.name, true, item.TotalPages, item.Height, item.Width, item.SizeDisk);
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"MS-RecordEngine > ImagingController: {ex.Message}");
                throw;
            }

        }
    }
}
