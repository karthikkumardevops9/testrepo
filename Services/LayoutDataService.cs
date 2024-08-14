using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Repository;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MSRecordsEngine.Services
{
    public class LayoutDataService : ILayoutDataService
    {
        private int _ALId { get; set; }
        private int _LId { get; set; }
        private int _FALId { get; set; }
        //layout
        public async Task BindUserAccessMenu(Passport _passport, LayoutModel model)
        {
            //IRepository<s_SavedCriteria> _is_SavedCriteria = new Repositories<s_SavedCriteria>();
            var sbMenu = new StringBuilder();
            string strLId;
            // ''' Start : Bind My Queries and My Favourites Menu
            var lWorkGroupItem = new List<WorkGroupItem>();

            if (_passport.CheckPermission(Common.SECURE_MYQUERY, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
            {
                lWorkGroupItem.Add(new WorkGroupItem()
                {
                    ID = (long)Enums.SavedType.Query,
                    WorkGroupName = "My Queries"
                });
            }

            if (_passport.CheckPermission(Common.SECURE_MYFAVORITE, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
            {
                lWorkGroupItem.Add(new WorkGroupItem()
                {
                    ID = (long)Enums.SavedType.Favorite,
                    WorkGroupName = "My Favorites"
                });
            }

            foreach (WorkGroupItem workGroupItem in lWorkGroupItem)
            {
                strLId = string.Format("L{0}", _LId);
                sbMenu.Append("<ul class='drillDownMenu MyQuery_Fav'><li>");
                if (workGroupItem.ID == 1L)
                {
                    sbMenu.Append(string.Format("<a href='#' id='MyfavClickMenu'><i class='font_icon theme_color fa fa-database'></i>{0}</a>", workGroupItem.WorkGroupName));
                }
                else if (workGroupItem.ID == 0L)
                {
                    sbMenu.Append(string.Format("<a href='#' id='MyQueryClickMenu'><i class='font_icon theme_color fa fa-database'></i>{0}</a>", workGroupItem.WorkGroupName));
                    // ElseIf workGroupItem.ID = 2 Then
                    // sbMenu.Append(String.Format("<a href='#' onclick=""obJgridfunc.LoadViewValt(this,'{0}')""><i class='font_icon theme_color fa fa-database'></i>{1}</a>", workGroupItem.WorkGroupName, workGroupItem.WorkGroupName))
                }
                var subMenu = new List<s_SavedCriteria>();
                using (var context = new TABFusionRMSContext(_passport.ConnectionString))
                {
                    subMenu = context.s_SavedCriteria.Where(a => a.UserId == _passport.UserId && a.SavedType == workGroupItem.ID).ToList();
                }
                //var subMenu = _is_SavedCriteria.All().Where(x => x.UserId == _passport.UserId && x.SavedType == workGroupItem.()ID).ToList();
                if (subMenu.Count > 0)
                {
                    _LId += 1;
                    strLId = string.Format("{0}{1}", strLId, _LId);
                    sbMenu.Append("<ul>");
                    await BindSavedSubMenu(subMenu, sbMenu, strLId, _passport);
                    sbMenu.Append("</ul>");
                }
                else
                {
                    sbMenu.Append("<ul>");
                    sbMenu.Append("</ul>");
                }
                sbMenu.Append("</li></ul>");
                _LId += 1;
            }
            // ''' End : Bind My Queries and My Favourites Menu
            foreach (WorkGroupItem workGroupItem in Navigation.GetWorkGroups(_passport))
            {
                strLId = string.Format("L{0}", _LId);
                sbMenu.Append("<ul class='drillDownMenu'><li>");
                sbMenu.Append(string.Format("<a href='#' id='RA{0}'><i class='font_icon theme_color fa fa-database'></i>{1}</a>", strLId, workGroupItem.WorkGroupName));

                var subMenu = Navigation.GetWorkGroupMenu((short)workGroupItem.ID, _passport);

                if (subMenu.Count > 0)
                {
                    _LId += 1;
                    strLId = string.Format("{0}{1}", strLId, _LId);
                    sbMenu.Append("<ul>");
                    await BindUserSubMenu(subMenu, sbMenu, strLId, _passport);
                    sbMenu.Append("</ul>");
                }
                sbMenu.Append("</li></ul>");
                _LId += 1;
            }

            model.Layout.UserAccessMenuHtml = sbMenu.ToString();
        }
        public async Task HandleAdminMenu(Passport _passport, LayoutModel model)
        {
            //IRepository<ImportLoad> _iImportLoad = new Repositories<ImportLoad>();
            //IRepository<ImportField> _iImportField = new Repositories<ImportField>();
            await LinkAdminSecurityCheck(_passport, model);
            // Check for LABEL MANAGER access
            if (_passport.CheckPermission(Common.SECURE_LABEL_SETUP, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
            {
                model.Layout.LinkLabelManager = string.Format("<a target=\"_blank\" href=\"/LabelManager\">{0}</a>", "Label Manager");
            }
            else
            {
                model.Layout.LinkLabelManager = "";
            }
            //List<KeyValuePair<string, bool>> lstImportLoad = null;
            //lstImportLoad = Keys.GetImportDDL(_iImportLoad.All(), _iImportField.All(), _passport);
            bool ImportPermission = _passport.CheckPermission(Common.SECURE_IMPORT_SETUP, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access);
            if (ImportPermission)
            {
                model.Layout.LinkImport = string.Format("<a target=\"_blank\" title=\"Import\" href=\"/Import\">{0}</a>", "Import");
            }
            //else if (lstImportLoad.Count == 0)
            //{
            //    model.Layout.LinkImport = "";
            //}
            //else
            //{
            //    model.Layout.LinkImport = string.Format("<a target=\"_blank\" title=\"Import\" href=\"/Import\">{0}</a>", "Import");
            //}

            if (_passport.CheckPermission(Common.SECURE_ORPHANS, Smead.Security.SecureObject.SecureObjectType.Orphans, Permissions.Permission.View))
            {
                model.Layout.Vault = string.Format("<a target=\"_blank\" onclick=\"CheckForLicense('FAttachment')\">{0}</a>", "Vault");
            }

            if (_passport.CheckPermission(Common.SECURE_TRACKING, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
            {
                model.Layout.LinkTracking = string.Format("<a title=\"Tracking\" onclick=\"CheckForLicense('FTransfer', 'Tracking')\">{0}</a>", "Tracking");
            }
            else
            {
                model.Layout.LinkTracking = "";
            }
            if (_passport.CheckPermission(Common.SECURE_RETENTION_SETUP, Smead.Security.SecureObject.SecureObjectType.Retention, Permissions.Permission.Access))
            {
            }
            // Session("bRetentionPermission") = True
            else
            {
                // Session("bRetentionPermission") = False
            }
            if (_passport.CheckPermission(Common.SECURE_DASHBOARD, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
            {
                //LinkLabelDashboard = string.Format("<a href=\"/Dashboard\" target=\"_blank\">{0}</a>", "Dashboard");
                model.Layout.LinkLabelDashboard = $"<a href='#' onclick=\"CheckForLicense('FDashboard')\">{"Dashboard"}</a>";
            }
            if (_passport.CheckPermission(Common.SECURE_RETENTION_SETUP, Smead.Security.SecureObject.SecureObjectType.Retention, Permissions.Permission.Access))
            {
                model.Layout.Retention = $"<a href='#' onclick=\"CheckForLicense('FRetention')\">{"Retention"}</a>";
            }

            // If _passport.CheckPermission(Common.SECURE_REPORTS, Smead.SecurityCS.SecureObject.SecureObjectType.Application, Permissions.Permission.View) Then
            model.Layout.Reports = string.Format("<a href=\"/Reports/Index\" target=\"_blank\">{0}</a>", "Reports");
            // End If
        }
        public async Task BackgroundStatusNotifications(Passport _passport, LayoutModel model)
        {
            int backgroundStatusNotification = 0;
            await Task.Run(() =>
            {
                using (var context = new TABFusionRMSContext(_passport.ConnectionString))
                {
                    backgroundStatusNotification = context.SLServiceTasks.Where(x => x.UserId == _passport.UserId && x.IsNotification == true).ToList().Count;
                }
            });
            if (backgroundStatusNotification > 0)
            {
                model.Layout.BackgroundStatusNotification = backgroundStatusNotification.ToString();
            }
            else
            {
                model.Layout.BackgroundStatusNotification = 0.ToString();
            }
        }
        //task bar
        public async Task LoadTasks(Passport _passport, LayoutModel model)
        {
            var sbMenu = new StringBuilder();
            await Task.Run(() =>
            {
                foreach (var item in Navigation.GetTasksMvc(_passport))
                {
                    var replaced_item = item.Replace("style='color: blue;'", "");
                    sbMenu.Append(string.Format("<li>{0}</li>", replaced_item));
                }
            });
            model.Taskbar.TaskList = sbMenu.ToString();
        }
        public async Task GetTaskLightValues(Passport _passport, LayoutModel model)
        {
            model.Taskbar.RequestNewButtonLabel = "No New Requests";
            model.Taskbar.RequestNewButton = "0";
            model.Taskbar.imgRequestNewButton = "/Content/themes/TAB/img/top-action-req-green.png";
            model.Taskbar.ancRequestNewButton = "";

            model.Taskbar.RequestBatchButtonLabel = "No Batch Requests";
            model.Taskbar.RequestBatchButton = "0";
            model.Taskbar.imgRequestBatchButton = "/Content/themes/TAB/img/top-action-req-green.png";
            model.Taskbar.ancRequestBatchButton = "";

            model.Taskbar.RequestExceptionButtonLabel = "No Request Exceptions";
            model.Taskbar.RequestExceptionButton = "0";
            model.Taskbar.imgRequestExceptionButton = "/Content/themes/TAB/img/top-action-req-green.png";
            model.Taskbar.ancRequestExceptionButton = "";
            model.Taskbar.LicenseType = _passport.License.LicenseType;

            var lstTask = await Navigation.GetTaskLightValuesAsync(_passport);

            if (lstTask is null)
                return;

            if (lstTask[0] > 0)
            {
                model.Taskbar.RequestNewButtonLabel = "New Request Count";
                model.Taskbar.RequestNewButton = lstTask[0].ToString();
                model.Taskbar.imgRequestNewButton = "/Content/themes/TAB/img/top-action-req-red.png";
                model.Taskbar.ancRequestNewButton = "/Reports/Index/newRequest";
            }

            if (lstTask[1] > 0)
            {
                model.Taskbar.RequestBatchButtonLabel = "Batch Request Count";
                model.Taskbar.RequestBatchButton = lstTask[1].ToString();
                model.Taskbar.imgRequestBatchButton = "/Content/themes/TAB/img/top-action-req-red.png";
                model.Taskbar.ancRequestBatchButton = "/handler.aspx?r=NewBatchesReport&requesting=1";
            }

            if (lstTask[2] > 0)
            {
                model.Taskbar.RequestExceptionButtonLabel = "Request Exception Count";
                model.Taskbar.RequestExceptionButton = lstTask[2].ToString();
                model.Taskbar.imgRequestExceptionButton = "/Content/themes/TAB/img/top-action-req-red.png";
                model.Taskbar.ancRequestExceptionButton = "/Reports/Index/exceptions";
            }
        }
        //news feed
        public async Task LoadNews(Passport _passport, LayoutModel model)
        {
            string Linkfeed = Navigation.GetSetting("News", "NewsURL", _passport);

            //string LinkSetting = ConfigurationManager.AppSettings["NewsURL"];
            model.NewsFeed.isAdmin = _passport.IsAdmin();
            if (Navigation.GetSetting("News", "Display", _passport).ToLower() == "true" || Navigation.GetSetting("News", "Display", _passport).ToLower() == "1")
            {
                model.NewsFeed.isDisplay = true;
            }
            else
            {
                model.NewsFeed.isDisplay = false;
            }
            if (Linkfeed.Length > 0)
            {
                if (Linkfeed.Contains("recordsmanagement.tab.com"))
                {
                    model.NewsFeed.newsURL = "http://recordsmanagement.tab.com/feed/";
                    ReadXmlFeed(_passport, model);
                    model.NewsFeed.TitleNews = "TAB FusionRMS News";
                    model.NewsFeed.IsTabNewsFeed = 1;
                    model.NewsFeed.UrlNewsFeed = Linkfeed;
                }
                else
                {
                    model.NewsFeed.TitleNews = "News Feed";
                    model.NewsFeed.UrlNewsFeed = Linkfeed;
                    model.NewsFeed.IsTabNewsFeed = 0;
                }
            }
        }
        //get footer
        public async Task GetFooter(Passport _passport, LayoutModel model)
        {
            model.Footer.LblAttempt = await GetLastLoginFailedAndAttempt(_passport, model);
            //var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            //var copyright = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            //model.Footer.LblService = copyright;
            model.Footer.LblServiceVer = _passport.License.LicenseType;
        }
        //end footer
        private async Task BindSavedSubMenu(List<s_SavedCriteria> subMenu, StringBuilder sbMenu, string strLId, Passport _passport)
        {
            if (subMenu.Count > 0)
            {
                await Task.Run(() =>
                {
                    foreach (var V in subMenu)
                    {
                        using (var context = new TABFusionRMSContext(_passport.ConnectionString))
                        {
                            var objView = context.Views.Where(x => x.Id == V.ViewId);
                            if (objView is not null)
                            {
                                if (_passport.CheckPermission(objView.FirstOrDefault().ViewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.View))
                                {
                                    if (V.SavedType == 1)
                                    {
                                        // sbMenu.Append(String.Format("<li><a id=""A{2}{3}"" onClick=""obJfavorite.LoadFavoriteTogrid(this,'{0}_{4}_{5}', 1)"">{1}</a></li>", V.ViewId, V.SavedName, strLId, _ALId.ToString, V.Id, V.SavedType))
                                        sbMenu.Append(string.Format("<li><div class=\"row\"><div class=\"col-md-10\"><a data-location=\"1\" data-viewname=\"{1}\"  data-viewid=\"favorite\" onClick=\"obJfavorite.LoadFavoriteTogrid(this,'{0}_{4}_{5}', 1)\">{1}</a></div><div class=\"col-md-2\"><i onclick=\"obJfavorite.DeleteCeriteria(this,{4}, {0})\" style=\"margin-top:11px;margin-left: -6px;\" class=\"fa fa-trash-o\"></i></div></div></li>", V.ViewId, V.SavedName, strLId, _ALId.ToString(), V.Id, V.SavedType));
                                    }
                                    else
                                    {
                                        sbMenu.Append(string.Format("<li><div class=\"row\"><div class=\"col-md-10\"><a data-location=\"2\" data-viewname=\"{1}\" data-viewid=\"query\" onClick=\"obJmyquery.LoadSaveQuery(this,'{0}_{4}_{5}')\">{1}</a></div><div class=\"col-md-2\"><i onclick=\"obJmyquery.DeleteQuery(this, {4})\" style=\"margin-top:11px;margin-left: -6px;\" class=\"fa fa-trash-o\"></div></div></i>", V.ViewId, V.SavedName, strLId, _ALId.ToString(), V.Id, V.SavedType));
                                    }

                                    _ALId += 1;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                });
            }
            _ALId += 1;
        }
        private async Task BindUserSubMenu(List<TableItem> subMenu, StringBuilder sbMenu, string parentIdNo, Passport _passport)
        {
            foreach (var table in subMenu)
            {
                var lstViewItems =  await Navigation.GetViewsByTableNameAsync(table.TableName, _passport);

                if (lstViewItems.Count > 0)
                {
                    sbMenu.Append(string.Format("<li><a href='#' id='A{0}{1}'>{2}</a>", parentIdNo, _ALId, table.UserName));
                    sbMenu.Append("<ul>");

                    foreach (var V in lstViewItems)
                    {
                        sbMenu.Append(string.Format("<li><a  name=\"viewAccess\" data-viewname=\"{1}\" data-location=\"0\" data-viewid=\"{0}\"  onClick=\"obJgridfunc.LoadView(this,'{0}')\">{1}</a></li>", V.Id, V.ViewName, parentIdNo, _ALId.ToString(), _FALId.ToString()));
                        _FALId += 1;
                    }
                    sbMenu.Append("</ul></li>");
                }
                _ALId += 1;
            }
        }

        private async Task LinkAdminSecurityCheck(Passport _passport, LayoutModel model)
        {
            await Task.Run(() =>
            {
                bool mbMgrGroup = _passport.CheckAdminPermission(Permissions.Permission.Access);
                bool testSECURE_SECURITY = _passport.CheckPermission(Common.SECURE_SECURITY, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access);
                bool testSECURE_SECURITY_USER = _passport.CheckPermission(Common.SECURE_SECURITY_USER, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access);
                bool testSECURE_REPORT_STYLES = _passport.CheckPermission(Common.SECURE_REPORT_STYLES, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access);

                if (!mbMgrGroup && !_passport.CheckPermission(Common.SECURE_SECURITY, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access) && !_passport.CheckPermission(Common.SECURE_SECURITY_USER, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access) && !_passport.CheckPermission(Common.SECURE_REPORT_STYLES, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access))
                {
                    model.Layout.LinkAdmin = "";
                }
                else
                {
                    model.Layout.LinkAdmin = string.Format("<a title=\"Admin\" href=\"/Admin\" target=\"_blank\">{0}</a>", "Admin");
                }
            });
        }

        private void ReadXmlFeed(Passport _passport, LayoutModel model)
        {
            var err = new ErrorBaseModel();
            bool rssFeedError = false;
            try
            {
                var rssSettings = new XmlReaderSettings();
                rssSettings.DtdProcessing = DtdProcessing.Ignore;
                using (var rssStream = WebRequest.Create(model.NewsFeed.newsURL).GetResponse().GetResponseStream())
                {
                    var rssDocument = new XmlDocument();
                    using (var rssReader = XmlReader.Create(rssStream, rssSettings))
                    {
                        rssDocument.Load(rssReader);
                    }

                    using (var rssList = rssDocument.SelectNodes("rss/channel/item"))
                    {
                        rssFeedError = rssList.Count == 0;
                        if (!rssFeedError)
                        {
                            for (int i = 0, loopTo = rssList.Count - 1; i <= loopTo; i++)
                            {
                                string title = string.Empty;
                                string link = string.Empty;
                                string description = string.Empty;
                                title = rssList.Item(i).SelectSingleNode("title").InnerText;
                                link = rssList.Item(i).SelectSingleNode("link").InnerText;
                                description = rssList.Item(i).SelectSingleNode("description").InnerText;
                                model.NewsFeed.BlockHtml = string.Format("<span style='margin: 0px; font-size:18px; padding: 0px;'><a style='font-size:18px;' href='{0}' target='new'>{1}</a></span><p align='justify' style='color: gray;'>{2}</p>", link, title, description);
                                model.NewsFeed.LstBlockHtml.Add(model.NewsFeed.BlockHtml);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Eventlogs.LogError(ex, err, 0, "");
                rssFeedError = true;
            }

            if (rssFeedError)
            {
                // divNews.Attributes.Add("style", "width: 100%; border: 1px solid #666666; background-color: White; padding: 5px 5px 20px 5px;")
                // NewsFrame.Attributes.Add("src", newsURL)
            }
        }
        private async Task<string> GetLastLoginFailedAndAttempt(Passport _passport, LayoutModel model)
        {
            string strLastLogin = "";
            if (_passport.UserName is not null)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        using (var context = new TABFusionRMSContext(_passport.ConnectionString))
                        {
                            var lSLAuditLogin = context.SLAuditLogins.OrderByDescending(x => x.Id).Take(2).ToList();

                            var oSLAuditLoginLast = lSLAuditLogin.OrderByDescending(x => x.Id).FirstOrDefault();
                            var LastLoginDate = oSLAuditLoginLast?.LoginDateTime ?? DateTime.Now;

                            var oSLAuditLoginPrevLast = lSLAuditLogin.OrderBy(x => x.Id).Skip(1).FirstOrDefault();
                            var prevLastLoginDate = oSLAuditLoginPrevLast?.LoginDateTime ?? DateTime.Now;

                            using (var context1 = new TABFusionRMSContext(_passport.ConnectionString))
                            {
                                var lSLAuditFailedLogin = context1.SLAuditFailedLogins
                                    .Where(x => x.LoginDateTime > prevLastLoginDate && x.LoginDateTime < LastLoginDate)
                                    .ToList();

                                var LastLoginAttempt = lSLAuditFailedLogin.Count;

                                strLastLogin = string.Format(
                                    "Last successful login by {2} on : {0} <br> Number of unsuccessful login attempts since last login: {1} ",
                                    prevLastLoginDate, LastLoginAttempt, _passport.UserName);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return strLastLogin;
        }

    }
}
