using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Smead.Security;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSRecordsEngine.Models.FusionModels
{
    public class LayoutModel
    {
        public LayoutModel()
        {
            Layout = new MainLayout();
            Taskbar = new TasksBar();
            NewsFeed = new NewsFeed();
            Footer = new Footer();
        }
        public MainLayout Layout { get; set; }
        public TasksBar Taskbar { get; set; }
        public NewsFeed NewsFeed { get; set; }
        public Footer Footer { get; set; }

    }
    public class MainLayout
    {
        public IConfiguration config { get; set; }
        public string LinkLabelManager { get; set; }
        public string LinkImport { get; set; }
        public string LinkTracking { get; set; }
        public string Reports { get; set; }
        public string Vault { get; set; }
        public string LinkAdmin { get; set; }
        public string UserAccessMenuHtml { get; set; }
        public string BackgroundStatusNotification { get; set; }
        public string LanguageCulture { get; set; }
        public string LinkLabelDashboard { get; set; }
        public string Retention { get; set; }
        private int _ALId { get; set; }
        private int _LId { get; set; }
        private int _FALId { get; set; }

    }

    public class TasksBar
    {
        public string TaskList { get; set; }
        public string RequestNewButtonLabel { get; set; }
        public string RequestBatchButtonLabel { get; set; }
        public string RequestExceptionButtonLabel { get; set; }
        public string RequestNewButton { get; set; }
        public string imgRequestNewButton { get; set; }
        public string ancRequestNewButton { get; set; }
        public string RequestBatchButton { get; set; }
        public string imgRequestBatchButton { get; set; }
        public string ancRequestBatchButton { get; set; }
        public string RequestExceptionButton { get; set; }
        public string imgRequestExceptionButton { get; set; }
        public string ancRequestExceptionButton { get; set; }
        public string LicenseType { get; set; }
    }
    public class NewsFeed
    {
        public NewsFeed()
        {
            LstBlockHtml = new List<string>();
        }
        public string newsURL { get; set; }
        public string TitleNews { get; set; }
        public string BlockHtml { get; set; }
        public List<string> LstBlockHtml { get; set; }
        public int IsTabNewsFeed { get; set; }
        public string UrlNewsFeed { get; set; }
        public bool isAdmin { get; set; }
        public bool isDisplay { get; set; }
        [JsonIgnore]
        public IConfiguration configuration { get; set; }
    }
    
    public class Footer
    {
        public string LblAttempt { get; set; }
        public string LblService { get; set; }
        public string LblServiceVer { get; set; }
    }
}
