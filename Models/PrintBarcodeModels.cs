using MSRecordsEngine.Entities;
using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System.Collections.Generic;
using System.Data;

namespace MSRecordsEngine.Models
{

    public class TestPrintBarcodeApi_Request
    {
        public string ConnectionString { get; set; }
        public UserInterfaceJsonReqModel Request { get; set; }
    }

    public class PrintBarcodeResponse 
    {
        public List<dropdown> labelDesign { get; set; } = new List<dropdown>();
        public List<dropdown> labelForm { get; set; } = new List<dropdown>();
        public string totalLabels { get; set; }
        public string totalPages { get; set; }
        public string totalSkipped { get; set; }
        public OneStripJob oneStripJob { get; set; }
        public OneStripForm oneStripForms { get; set; }
        public List<OneStripForm> OneStripForms { get; set; } = new List<OneStripForm>();
        public List<OneStripJob> OneStripJobs { get; set; } = new List<OneStripJob>();
        public List<OneStripJobField> oneStripJobFields { get; set; } = new List<OneStripJobField>();
        public int strtPrinting { get; set; }
        public string labelFileName { get; set; }
        public string labelFileDynamicpath { get; set; }
        public bool labelOutline { get; set; }
        public string formSelectionid { get; set; }
        public string Msg { get; set; }
        public bool isError { get; set; } = false;
        public List<GridData> GridDatas { get; set; }
    }

    public class dropdown
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    public class PrintBarcodeCommonModel
    {
        public string labelFileDynamicpath { get; set; }
        public string labelFileName { get; set; }
        public string msg { get; set; }
        public int formSelectionid { get; set; }
    }

    public class GenerateBarcodeOnchange_Request
    {
        public UserInterfaceJsonReqModel ReqModel { get; set; }
        public List<OneStripForm> OneStripForms { get; set; }
        public List<OneStripJob> oneStripJobs { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SetDefaultBarcodeForm_Request
    {
        public UserInterfaceJsonReqModel ReqModel { get; set; }
        public string ConnectionString { get; set; }
        public List<OneStripForm> OneStripForms { get; set; }
    }

    public class GridData
    {
        public List<GridRow> GridRows { get; set; }
    }

    public class GridRow
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

}
