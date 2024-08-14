using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services;
using Newtonsoft.Json;
using Smead.Security;
using System;
using System.Text;
using System.Data;
using MSRecordsEngine.Models;
using System.Collections.Generic;
using MSRecordsEngine.Services.Interface;
using System.Threading.Tasks;
using System.Globalization;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BarcodeTrackerController : ControllerBase
    {
        private readonly CommonControllersService<BarcodeTrackerController> _commonService;
        private readonly ITrackingServices _trackingServices;
        private TableItem _objItem;
        private TableItem _destItem;
        BarcodeTrackerModel Barcodemodel = new BarcodeTrackerModel();
        public BarcodeTrackerController(CommonControllersService<BarcodeTrackerController> commonControllersService,ITrackingServices trackingServices)
        {
            _commonService = commonControllersService;
            _trackingServices = trackingServices;
        }

        [Route("Index")]
        [HttpPost]
        public BarcodeTrackerModel Index(Passport passport)
        {

            Barcodemodel.additionalField1Type = "";
            Barcodemodel.lblAdditional1 = "";
            Barcodemodel.lblAdditional2 = "";
            Barcodemodel.additionalField2 = "";

            try
            {
                Barcodemodel.hdnPrefixes = LoadPrefixes(passport);
                string additionalField1 = Navigation.GetSystemSetting("TrackingAdditionalField1Desc", passport);
                if (!string.IsNullOrEmpty(additionalField1))
                {
                    string additionalField1Type = Navigation.GetSystemSetting("TrackingAdditionalField1Type", passport);
                    if (!string.IsNullOrEmpty(additionalField1Type))
                    {
                        Barcodemodel.additionalField1Type = additionalField1Type;
                        Barcodemodel.lblAdditional1 = additionalField1 + ":";
                        // Barcodemodel.chekAdditionSystemseting = 1
                    }
                }
                else
                {
                    Barcodemodel.additionalField1Type = "";
                }
                // 'memo additional field
                string additionalField2 = Navigation.GetSystemSetting("TrackingAdditionalField2Desc", passport.Connection());
                if (!string.IsNullOrEmpty(additionalField2))
                {
                    Barcodemodel.lblAdditional2 = additionalField2 + ":";
                    Barcodemodel.additionalField2 = additionalField2;
                    // Barcodemodel.chekAdditionSystemseting = 1
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return Barcodemodel;
        }

        [Route("Dropdownlist")]
        [HttpPost]
        public List<SelectListItem> Dropdownlist(Passport passport)
        {
            var ls = new List<SelectListItem>();
            try
            {
                using (var conn = passport.Connection())
                {
                    var dtSuggest = Tracking.GetTrackingSelectData(conn);
                    foreach (DataRow row in dtSuggest.Rows)
                        ls.Add(new SelectListItem() { Text = Convert.ToString(row["Id"]), Value = Convert.ToString(row["Id"]) });
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return ls;
        }

        [Route("ClickBarcodeTextDestination")] 
        [HttpPost]
        public async Task<string> ClickBarcodeTextDestination(DetectDestinationChangeParam param)
        {
            try
            {
                if (string.IsNullOrEmpty(param.txtDestination))
                {
                    if (!IsDestination(_destItem))
                        throw new Exception(string.Format("Destination Bar code required", param.txtDestination));
                }
                else
                {
                    using (var conn = param.passport.Connection())
                    {
                        _objItem = Tracking.TranslateBarcode(param.txtObject, false, conn);
                        _destItem = Tracking.TranslateBarcode(param.txtDestination, true, conn);
                        if (!IsDestination(_destItem))
                            throw new Exception(string.Format("'{0}' is not a valid destination", param.txtDestination));

                        if (_destItem != null)
                        {
                            param.barcodemodel.getDestination = Navigation.GetItemName(_destItem.TableName, _destItem.ID, param.passport, conn);
                            param.barcodemodel.CheckgetDestination = true;
                        }
                        else
                        {
                            param.barcodemodel.getDestination = string.Format("'{0}' not found", param.txtDestination);
                            param.barcodemodel.CheckgetDestination = false;
                        }
                        Barcodemodel = param.barcodemodel;
                        await SetDueBackDate(_destItem, param.txtDestination, param.shortDateFormat, param.timeoffSet, param.passport.ConnectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }

            return JsonConvert.SerializeObject(param.barcodemodel);
        }

        [Route("DetectDestinationChange")]
        [HttpPost]
        public BarcodeTrackerModel DetectDestinationChange(DetectDestinationChangeParam param)
        {
            try
            {
                using (var conn = param.passport.Connection())
                {
                    try
                    {
                        _destItem = Tracking.TranslateBarcode(param.txtDestination, true, conn);
                        _objItem = Tracking.TranslateBarcode(param.txtObject, true, conn);
                        param.barcodemodel.detectDestination = !DestinationIsHigher(_destItem, _objItem);
                    }
                    catch (Exception ex)
                    {
                        _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
                        param.barcodemodel.serverErrorMsg = ex.Message;
                        param.barcodemodel.detectDestination = false;
                    }
                }
                return param.barcodemodel;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return param.barcodemodel;
        }

        [Route("ClickBarckcodeTextTolistBox")] 
        [HttpPost]
        public async Task<string> ClickBarckcodeTextTolistBox(ClickBarckcodeTextTolistBoxParam param)
        {
            Barcodemodel.serverErrorMsg = "";
            try
            {
                if (string.IsNullOrEmpty(param.txtObject))
                {
                    throw new Exception(string.Format("Object Bar code required", param.txtObject));
                }
                if (string.IsNullOrEmpty(param.additional1))
                    param.additional1 = " ";
                if (string.IsNullOrEmpty(param.additional2))
                    param.additional2 = " ";

                using (var conn = param.passport.Connection())
                {
                    _objItem = Tracking.TranslateBarcode(param.txtObject, false, conn);
                    _destItem = Tracking.TranslateBarcode(param.txtDestination, true, conn);
                }
                if (string.IsNullOrEmpty(param.hdnPrefixes))
                    LoadPrefixes(param.passport);
                if (_objItem != null)
                {
                    if (IsDestination(_destItem) && !DestinationIsHigher(_destItem, _objItem))
                        throw new Exception(string.Format("Tracking Object '{0}' does not fit into Tracking Destination '{1}''", _objItem.TableName, param.txtDestination));
                    var user = new User(param.passport, true);
                    await StartTransfer(_destItem, _objItem, param.txtObject, param.txtDestination, param.txtDueBackDate, param.passport,param.userName, param.additional1, param.additional2);
                    Barcodemodel.returnDestination = Navigation.GetItemName(_destItem.TableName, _destItem.ID, param.passport);
                    Barcodemodel.returnObjectItem = "  └─► " + Navigation.GetItemName(_objItem.TableName, _objItem.ID, param.passport);
                }
                else
                {
                    Barcodemodel.serverErrorMsg = string.Format("'{0}' not found", param.txtObject);
                }
            }
            catch (Exception ex)
            {
               _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
                Barcodemodel.serverErrorMsg = ex.Message;
            }
         
            return JsonConvert.SerializeObject(Barcodemodel);
        }

        #region private methods
        private bool IsDestination(TableItem _destItem)
        {
            if (_destItem != null)
            {
                return _destItem.TrackingTable > 0;
            }
            return false;
        }

        private bool DestinationIsHigher(TableItem _destItem, TableItem _objItem)
        {
            if (_destItem != null & _objItem != null)
            {
                if (_objItem.TrackingTable != -1)
                {
                    return _destItem.TrackingTable < _objItem.TrackingTable;
                }
                else if (_destItem.TrackingTable != -1)
                {
                    return true;
                }
            }
            return false;
        }

        private string LoadPrefixes(Passport passport)
        {
            var sb = new StringBuilder();

            using (var conn = passport.Connection())
            {
                var dt = Tracking.GetTrackingContainerTypes(conn);
                foreach (DataRow row in dt.Rows)
                {
                    if (string.IsNullOrEmpty(row["BarCodePrefix"].ToString()))
                    {
                        sb.Append(" ,");
                    }
                    else
                    {
                        sb.Append(string.Format("{0},", row["BarCodePrefix"].ToString().ToUpper()));
                    }
                }
            }
            return sb.ToString();
        }

        private async Task SetDueBackDate(TableItem item, string textm,string shortdateformat,string offSetVal,string connectionString)
        {
            Barcodemodel.chkDueBackDate = await _trackingServices.IsOutDestination(item.TableName, item.ID, connectionString);
            Barcodemodel.formatDueBackDate = shortdateformat;
            if (Convert.ToBoolean(Barcodemodel.chkDueBackDate))
            {
                Barcodemodel.DueBackDateText = _commonService.GetConvertCultureDate(Convert.ToString(await _trackingServices.GetDueBackDate(item.TableName, item.ID, connectionString)),shortdateformat,offSetVal);
            }
            else
            {
                Barcodemodel.DueBackDateText = "[None]";
            }
        }

        private async Task StartTransfer(TableItem destItem, TableItem objItem, string txtObject, string txtDestination, string txtDueBackDate,Passport passport,string userName,string? additional1 = null, string? additional2 = null)
        {
            if (string.IsNullOrEmpty(txtObject))
            {
                txtObject = "";
            }
            if (string.IsNullOrEmpty(txtDestination))
            {
                txtDestination = "";
            }
            var dateStr = new DateTime();
            if (await _trackingServices.IsOutDestination(destItem.TableName, destItem.ID, passport.ConnectionString))
            {
                if (txtDueBackDate is not null)
                {
                    if (txtDueBackDate.Trim().Length == 0)
                    {
                        Barcodemodel.serverErrorMsg = "Due Back Date is required";
                        return;
                    }

                    if (!DateTime.TryParse(txtDueBackDate, out _))
                    {
                        Barcodemodel.serverErrorMsg = "Due Back Date is invalid";
                        return;
                    }
                    dateStr = DateTime.Parse(txtDueBackDate.Trim(), CultureInfo.CurrentCulture);
                    if (DateTime.Parse(DateTime.Now.ToShortDateString()) > DateTime.Parse(dateStr.ToShortDateString()))
                    {
                        Barcodemodel.serverErrorMsg = "Object Due Back Date cannot be less than Today";
                        return;
                    }
                }
            }

            await _trackingServices.PrepareDataForTransfer(objItem.TableName, objItem.ID, destItem.TableName, destItem.ID, dateStr, userName, passport, additional1, additional2);
        }

        #endregion
    }
}


