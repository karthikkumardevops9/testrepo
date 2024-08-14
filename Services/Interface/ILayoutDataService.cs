using Microsoft.VisualBasic;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Repository;
using Smead.Security;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace MSRecordsEngine.Services.Interface
{
    public interface ILayoutDataService
    {
        //layout
        public Task BindUserAccessMenu(Passport _passport, LayoutModel model);
        public Task HandleAdminMenu(Passport _passport, LayoutModel model);
        public Task BackgroundStatusNotifications(Passport _passport, LayoutModel model);
        //task bar
        public Task LoadTasks(Passport _passport, LayoutModel model);
        //news feed
        public Task GetTaskLightValues(Passport _passport, LayoutModel model);
        public Task LoadNews(Passport _passport, LayoutModel model);
        //get footer
        public Task GetFooter(Passport _passport, LayoutModel model);
        //end footer
    }
}
