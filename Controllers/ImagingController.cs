using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Imaging;
using MSRecordsEngine.Services;
using MSRecordsEngine.Models;
using Smead.Security;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Net;
using System.Net.Http;
using System;
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
        private async void SaveNewAttachmentInPopupWindow(UserInterface.popupdocViewer param, List<string> filePathList, Passport passport)
        {
            try
            {
                var ticket = passport.get_CreateTicket(string.Format(@"{0}\{1}", passport.ServerName, passport.DatabaseName), param.tabName, param.tableId).ToString();
                // Dim oDefaultOutputSetting = _iSystem.All.FirstOrDefault.DefaultOutputSettingsId
                string oDefaultOutputSetting = string.Empty;
                var filesinfo = await _commonService.Microservices.DocumentServices.GetCodecInfoFromFileList(filePathList);
                foreach (var item in filesinfo)
                {
                    if (item.Ispcfile)
                    {
                        Attachments.AddAnAttachment(_commonService.GetClientIpAddress(), ticket, passport.UserId, passport, param.tabName, param.tableId, 0, oDefaultOutputSetting, item.FilePath, item.FilePath, Path.GetExtension(item.FilePath), false, param.name, false, 1, 0, 0, 0);
                    }
                    else
                    {
                        Attachments.AddAnAttachment(_commonService.GetClientIpAddress(), ticket, passport.UserId, passport, param.tabName, param.tableId, 0, oDefaultOutputSetting, item.FilePath, item.FilePath, Path.GetExtension(item.FilePath), false, param.name, true, item.TotalPages, item.Height, item.Width, item.SizeDisk);
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"MS-RecordEngine > ImagingController: {ex.Message}");
            }
            
        }

    }
}
