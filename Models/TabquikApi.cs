using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualBasic;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Net;
using System;

namespace MSRecordsEngine.Models
{
    public class TabquikApi
    {
        public string CustomerID { get; set; }
        public string ContactID { get; set; }
        public string ErrorMsg { get; set; }
        public string srcUrl { get; set; }
        public string DataTQ { get; set; }
    }

}
