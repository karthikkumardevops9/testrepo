using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Data.Entity;
using MSRecordsEngine.Models;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Services.Interface;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BackgroundStatusController : ControllerBase
    {
        private readonly CommonControllersService<BackgroundStatusController> _commonService;
        private readonly IBackgroundStatusService _backgroundStatusService;

        public BackgroundStatusController(CommonControllersService<BackgroundStatusController> commonControllersService,IBackgroundStatusService backgroundStatusService)
        {
            _commonService = commonControllersService;
            _backgroundStatusService = backgroundStatusService;
        }

        [Route("GetBackgroundStatusList")]
        [HttpPost]
        public async Task<string> GetBackgroundStatusList(BackgrundStatusListParam microParams)
        {
            var passport = microParams.passport;
            var sidx = microParams.sidx;
            var sord = microParams.sord;
            var page = microParams.page;
            var rows = microParams.rows;

            JsonResult jsonData = new JsonResult(null);
            try
            {
                int totalRecords = 0;
                await _backgroundStatusService.ChangeNotification(passport.UserId, microParams.passport.ConnectionString);

                var tranfer = (int)Enums.BackgroundTaskType.Transfer;
                var csv = (int)Enums.BackgroundTaskInDetail.ExportCSV;
                var etxt = (int)Enums.BackgroundTaskInDetail.ExportTXT;

                List<SLServiceTask> recordsList;
                using (var context = new TABFusionRMSContext(Keys._connMARSbuild(passport.ConnectionString)))
                {
                    if (passport.IsAdmin())
                    {
                        recordsList = await context.SLServiceTasks.Where(x => x.TaskType == tranfer || x.TaskType == csv || x.TaskType == etxt).ToListAsync();
                    }
                    else
                    {
                        recordsList = await context.SLServiceTasks.Where(x => x.UserId == passport.UserId && x.TaskType == tranfer || x.TaskType == csv || x.TaskType == etxt).ToListAsync();
                    }

                    totalRecords = recordsList.Count;

                    switch (sidx ?? "")
                    {
                        case "CreateDate":
                            {
                                if (sord.ToLower() == "asc")
                                {
                                    recordsList = recordsList.OrderBy(y => y.CreateDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }
                                else
                                {
                                    recordsList = recordsList.OrderByDescending(y => y.CreateDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }

                                break;
                            }
                        case "StartDate":
                            {
                                if (sord.ToLower() == "asc")
                                {
                                    recordsList = recordsList.OrderBy(y => y.StartDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }
                                else
                                {
                                    recordsList = recordsList.OrderByDescending(y => y.StartDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }

                                break;
                            }
                        case "EndDate":
                            {
                                if (sord.ToLower() == "asc")
                                {
                                    recordsList = recordsList.OrderBy(y => y.StartDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }
                                else
                                {
                                    recordsList = recordsList.OrderByDescending(y => y.StartDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }

                                break;
                            }
                        case "Type":
                            {
                                if (sord.ToLower() == "asc")
                                {
                                    recordsList = recordsList.OrderBy(y => y.EndDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }
                                else
                                {
                                    recordsList = recordsList.OrderByDescending(y => y.EndDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }

                                break;
                            }
                        case "Status":
                            {
                                if (sord.ToLower() == "asc")
                                {
                                    recordsList = recordsList.OrderBy(y => y.Status).Skip(rows * (page - 1)).Take(rows).ToList();
                                }
                                else
                                {
                                    recordsList = recordsList.OrderByDescending(y => y.Status).Skip(rows * (page - 1)).Take(rows).ToList();
                                }

                                break;
                            }
                        case "UserName":
                            {
                                if (sord.ToLower() == "asc")
                                {
                                    recordsList = recordsList.OrderBy(y => y.UserName).Skip(rows * (page - 1)).Take(rows).ToList();
                                }
                                else
                                {
                                    recordsList = recordsList.OrderByDescending(y => y.UserName).Skip(rows * (page - 1)).Take(rows).ToList();
                                }

                                break;
                            }

                        default:
                            {
                                if (sord.ToLower() == "asc")
                                {
                                    recordsList = recordsList.OrderBy(y => y.StartDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }
                                else
                                {
                                    recordsList = recordsList.OrderByDescending(y => y.StartDate).Skip(rows * (page - 1)).Take(rows).ToList();
                                }

                                break;
                            }
                    }

                    if (totalRecords <= 0)
                    {
                        //return jsonData;
                        return "";
                    }
                    else
                    {
                        var lBackgroundServiceTask = new List<BackgroundServiceTask>();
                        foreach (var rowData in recordsList)
                        {
                            var objJobServiceTask = new BackgroundServiceTask();
                            switch (rowData.Status)
                            {
                                case var @case when @case == Enum.GetName(typeof(Enums.BackgroundTaskStatus), 1).ToString(): // --Pending
                                    {
                                        objJobServiceTask.Status = rowData.Status is null ? "" : "<span style='color:red'><b>" + rowData.Status + "</b></span>";
                                        objJobServiceTask.ReportLocation = "-";
                                        objJobServiceTask.DownloadLocation = "-";
                                        break;
                                    }
                                case var case1 when case1 == Enum.GetName(typeof(Enums.BackgroundTaskStatus), 2).ToString(): // --In-Progress
                                    {
                                        objJobServiceTask.Status = rowData.Status is null ? "" : "<span style='color:darkgoldenrod'><b>" + rowData.Status + "</b></span>";
                                        objJobServiceTask.ReportLocation = "-";
                                        objJobServiceTask.DownloadLocation = "-";
                                        break;
                                    }
                                case var case2 when case2 == Enum.GetName(typeof(Enums.BackgroundTaskStatus), 4).ToString(): // --Error
                                    {
                                        objJobServiceTask.Status = rowData.Status is null ? "" : "<span style='color:red'><b>" + rowData.Status + "</b></span>";
                                        objJobServiceTask.DownloadLocation = "-";
                                        if (rowData.ReportLocation is not null)
                                        {
                                            if (System.IO.File.Exists(rowData.ReportLocation))
                                            {
                                                string filename = Path.GetFileName(rowData.ReportLocation);
                                                objJobServiceTask.ReportLocation = "<a href='" + Url.Action("DownloadBackgroundStatus", "BackgroundStatus", new { url = EncryptURLParameters(filename) }) + "'><i class='fa fa-eye' aria-hidden='True'></i></a>";
                                            }
                                            else
                                            {
                                                objJobServiceTask.ReportLocation = "-";
                                            }
                                        }
                                        else
                                        {
                                            objJobServiceTask.ReportLocation = "-";
                                        }

                                        break;
                                    }
                                case var case3 when case3 == Enum.GetName(typeof(Enums.BackgroundTaskStatus), 3).ToString(): // --Completed
                                    {
                                        objJobServiceTask.Status = rowData.Status is null ? "" : "<span style='color:green'><b>" + rowData.Status + "</b></span>";
                                        if (rowData.TaskType is (int)Enums.BackgroundTaskInDetail.ExportCSV or (int)Enums.BackgroundTaskInDetail.ExportTXT)
                                        {
                                            if (rowData.DownloadLocation is not null)
                                            {
                                                if (System.IO.File.Exists(rowData.DownloadLocation))
                                                {
                                                    string filename = Path.GetFileName(rowData.DownloadLocation);
                                                    objJobServiceTask.DownloadLocation = "<a href='" + Url.Action("DownloadBackgroundStatus", "BackgroundStatus", new { url = EncryptURLParameters(filename) }) + "'><i class='fa fa-download' aria-hidden='True'></i></a>";
                                                }
                                                else
                                                {
                                                    objJobServiceTask.DownloadLocation = "-";
                                                }
                                            }
                                            else
                                            {
                                                objJobServiceTask.DownloadLocation = "-";
                                            }
                                            objJobServiceTask.ReportLocation = "-";
                                        }
                                        else
                                        {
                                            objJobServiceTask.DownloadLocation = "-";
                                            if (rowData.ReportLocation is not null)
                                            {
                                                if (System.IO.File.Exists(rowData.ReportLocation))
                                                {
                                                    string filename = Path.GetFileName(rowData.ReportLocation);
                                                    objJobServiceTask.ReportLocation = "<a href='" + Url.Action("DownloadBackgroundStatus", "BackgroundStatus", new { url = EncryptURLParameters(filename) }) + "'><i class='fa fa-eye' aria-hidden='True'></i></a>";
                                                }
                                                else
                                                {
                                                    objJobServiceTask.ReportLocation = "-";
                                                }
                                            }
                                            else
                                            {
                                                objJobServiceTask.ReportLocation = "-";
                                            }

                                        }

                                        break;
                                    }
                                case var case4 when case4 == Enum.GetName(typeof(Enums.BackgroundTaskStatus), 5).ToString(): // --In Que
                                    {
                                        objJobServiceTask.Status = rowData.Status is null ? "" : "<span style='color:navy'><b>" + rowData.Status + "</b></span>";
                                        objJobServiceTask.ReportLocation = "-";
                                        objJobServiceTask.DownloadLocation = "-";
                                        break;
                                    }
                            }

                            objJobServiceTask.Id = rowData.Id;
                            objJobServiceTask.CreateDate = rowData.CreateDate is null ? "-" : _commonService.GetConvertCultureDate(rowData.CreateDate.Value,microParams.ShortDatePattern,microParams.OffSetTimeVal, bDetectTime: true);
                            objJobServiceTask.StartDate = rowData.StartDate is null ? "-" : _commonService.GetConvertCultureDate(rowData.StartDate.Value, microParams.ShortDatePattern, microParams.OffSetTimeVal, bDetectTime: true);
                            objJobServiceTask.EndDate = rowData.EndDate is null ? "-" : _commonService.GetConvertCultureDate(rowData.EndDate.Value, microParams.ShortDatePattern, microParams.OffSetTimeVal, bDetectTime: true);
                            objJobServiceTask.Type = rowData.Type;
                            objJobServiceTask.RecordCount = rowData.RecordCount.ToString();
                            objJobServiceTask.UserName = rowData.UserName;
                            lBackgroundServiceTask.Add(objJobServiceTask);
                        }

                        int totalPages = (int)Math.Round(Math.Truncate(Math.Ceiling(totalRecords / (float)rows)));

                        //jsonData = new JsonResult(new { total = totalPages, page, records = totalRecords, rows = lBackgroundServiceTask });
                        //return jsonData;

                        var json = JsonConvert.SerializeObject(new { total = totalPages, page, records = totalRecords, rows = lBackgroundServiceTask });
                        return json;
                    }
                }

            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return ""; //jsonData;
        }

        private static string EncryptURLParameters(string clearText)
        {
            try
            {
                var clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (var encryptor = new AesCryptoServiceProvider())
                {
                    using (var pdb = new Rfc2898DeriveBytes("MAKV2SPBNI99212", new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(clearBytes, 0, clearBytes.Length);
                                cs.FlushFinalBlock();
                            }

                            clearText = "á" + Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return clearText;
        }

    }
}
