using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public class Transfers : BaseModel
    {
        public Transfers()
        {
            lsTaransferType = new List<string>();
            lsViews = new List<List<TransferRadioBox>>();
            trDestinationsItem = new List<TransferRadioBox>();
            itemsTobeTransfer = new List<string>();
        }
        public bool isDueBack { get; set; }
        public List<string> lsTaransferType { get; set; }
        public List<List<TransferRadioBox>> lsViews { get; set; }
        public List<TransferRadioBox> trDestinationsItem { get; set; }
        public List<string> itemsTobeTransfer { get; set; }
        public string userMsg { get; set; }
        public string HtmlMessage { get; set; }
        public string lblTitle { get; set; }
        public bool isBackground { get; set; }
        public string lblTransfer { get; set; }
    }

    public class TransferRadioBox
    {
        public string text { get; set; }
        public string value { get; set; }
        public string ContainerTableName { get; set; }
        public int ContainerViewid { get; set; }
    }

    public class TransferCommonParam
    {
        public UserInterfaceJsonReqModel req { get; set; }
        public Passport passport { get; set; }
    }

    public class TransferReturn
    {
        public Transfers Model { get; set; }
        public bool IsBackground { get; set; } = false;
    }

    public class CountAllTransferParam
    {
        public string HoldTotalRowQuery { get; set; }
        public Passport passport { get; set; }
    }

    public class BtnTransferParam
    {
        public string HoldTotalRowQuery { get; set; }
        public UserInterfaceJsonReqModel req { get; set; }
        public Passport passport { get; set; }
        public bool IsBackground { get; set; }
        public bool IsDataProcessingNetworkPath { get; set; }
        public string DataProcessingFilesPath { get; set; }
        public string RootPath { get; set; }
    }

    public class BackgroundTransferTask_Request
    {
        public string TableName { get; set; }
        public string TableId { get; set; }
        public string DestinationTableName { get; set; }
        public string DestinationTableId { get; set; }
        public DateTime DueBackDate { get; set; }
        public string UserName { get; set; }
        public Passport passport { get; set; }
    }
}
