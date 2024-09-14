using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RequesterController : ControllerBase
    {
        private readonly CommonControllersService<RequesterController> _commonService;
        private readonly ILayoutModelService _layoutModelService;

        public RequesterController(CommonControllersService<RequesterController> commonService, ILayoutModelService layoutModelService)
        {
            _commonService = commonService;
            _layoutModelService = layoutModelService;
        }

        [Route("GetRequesterpopup")]
        [HttpPost]
        public Requesters GetRequesterpopup(GetRequesterParam param)
        {
            var model = new Requesters();
            try
            {
                model.rdemployddlist = DrawRequestorList(param.req.paramss,param.passport);
                model.itemsTobeRequest = ItemsRequest(param.req.paramss, model,param.passport);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
                throw new Exception(ex.Message);
            }
            return model;
        }

        [Route("SearchEmployees")]
        [HttpPost]
        public Requesters SearchEmployees(RequesterParam param)
        {
            var model = new Requesters();
            try
            {
                model.rdemployddlist = DrawRequestorList(param.req,param.passport);
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
                throw new Exception(ex.Message);
            }
            return model;
        }

        [Route("SubmitRequest")]
        [HttpPost]
        public Requesters SubmitRequest(RequesterParam param)
        {
            var model = new Requesters();
            var message = new StringBuilder();
            var requests = new Requesting();
            if (string.IsNullOrEmpty(param.req.Request.Instruction))
                param.req.Request.Instruction = "";
            try
            {
                var reqList = requests.GetActiveRequests(param.req.ViewId, param.req.ids[0], param.passport);
                var tableInfo = Navigation.GetTableInfo(param.req.TableName, param.passport);
                Requesting.MakeRequest(ref message, param.req.ids, param.req.TableName, param.req.Request.Employeeid,param.req.Request.Instruction, param.req.Request.Priotiry,param.DateFormatedValue, param.req.Request.ischeckWaitlist, param.passport, tableInfo);

                var updateTaskBar = new TasksBar();
                updateTaskBar = _layoutModelService.ExecuteTasksbar(param.passport);

                model.NewRequestCount = updateTaskBar.RequestNewButton;
                model.RequestNewButtonLabel = updateTaskBar.RequestNewButtonLabel;
                model.imgRequestNewButton = updateTaskBar.imgRequestNewButton;
                model.ancRequestNewButton = updateTaskBar.ancRequestNewButton;

                model.NewExceptionCount = updateTaskBar.RequestExceptionButton;
                model.RequestExceptionButtonLabel = updateTaskBar.RequestExceptionButtonLabel;
                model.imgRequestExceptionButton = updateTaskBar.imgRequestExceptionButton;
                model.ancRequestExceptionButton = updateTaskBar.ancRequestExceptionButton;

            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
                throw new Exception(ex.Message);
            }
            if (message.Length > 0)
            {
                model.isError = true;
                model.Msg = message.ToString();
            }

            return model;
        }

        [Route("DeleteTrackingRequest")]
        [HttpPost]
        public TrackingModeld DeleteTrackingRequest(DeleteTrackingRequestParam param)
        {
            var model = new TrackingModeld();
            model.isDeleteAllow = true;
            try
            {
                Requesting.DeleteRequest(param.Id, param.passport);
                var updateTaskBar = new TasksBar();
                updateTaskBar = _layoutModelService.ExecuteTasksbar(param.passport);

                model.NewRequestCount = updateTaskBar.RequestNewButton;
                model.RequestNewButtonLabel = updateTaskBar.RequestNewButtonLabel;
                model.imgRequestNewButton = updateTaskBar.imgRequestNewButton;
                model.ancRequestNewButton = updateTaskBar.ancRequestNewButton;

                model.NewExceptionCount = updateTaskBar.RequestExceptionButton;
                model.RequestExceptionButtonLabel = updateTaskBar.RequestExceptionButtonLabel;
                model.imgRequestExceptionButton = updateTaskBar.imgRequestExceptionButton;
                model.ancRequestExceptionButton = updateTaskBar.ancRequestExceptionButton;
            }
            catch (Exception ex)
            {
                model.isError = true;
                model.Msg= "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
        
            return model;
        }

        [Route("UpdateRequest")]
        [HttpPost]
        public TrackingModeld UpdateRequest(UpdateRequestParam param)
        {
            var model = new TrackingModeld();
            try
            {
                Requesting.UpdateRequest(Convert.ToInt32(param.req.RequestID), param.req.Fulfilled, param.req.isException, param.req.txtComment, param.passport);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
                //Eventlogs.LogError(ex, model, 0, passport.DatabaseName);
                throw new Exception(ex.Message);
            }
            return model;
        }

        [Route("GetRequestDetails")]
        [HttpPost]
        public Requesters GetRequestDetails(GetRequestDetailsParam param)
        {
            var model = new Requesters();
            var requestInfo = Requesting.GetRequest(param.Id, param.TableName, param.Passport);
            model.RequestID = param.Id.ToString();
            model.Priority = requestInfo.Priority;
            model.DateRequested = _commonService.GetConvertCultureDate(requestInfo.DateRequested,param.ShortPatternFormate,param.OffTimeVal);
            model.DateNeeded = _commonService.GetConvertCultureDate(requestInfo.DateNeeded,param.ShortPatternFormate,param.OffTimeVal);
            model.Status = requestInfo.Status;
            model.Instruction = requestInfo.Instructions;
            model.Fulfilled = string.Compare(requestInfo.Status, "Fulfilled", true) == 0;
            try
            {
                var employeeTableInfo = Tracking.GetRequestorTableInfo(param.Passport);
                var requestedFor = new Employee(employeeTableInfo);
                var requestedBy = new Employee(employeeTableInfo);
                requestedFor.LoadByID(requestInfo.EmployeeID, param.Passport);
                model.txtFor = requestedFor.Description;
                model.Phone = requestedFor.Phone;
                model.Mail = requestedFor.MailStop;
                if (!string.IsNullOrEmpty(requestInfo.RequestedBy))
                {
                    if (requestedBy.LoadByName(requestInfo.RequestedBy,param.Passport))
                    {
                        model.By = requestedBy.Description;
                    }
                    else
                    {
                        model.By = Tracking.ShowUserName(requestInfo.RequestedBy, param.Passport);
                    }
                }
                else
                {
                    model.By = "(unknown user)";
                }

                if (requestInfo.PullListID != 0)
                {
                    var pullInfo = Navigation.GetPullList(requestInfo.PullListID, param.Passport);
                    model.PullList = requestInfo.PullListID.ToString();
                    model.PullDate = _commonService.GetConvertCultureDate(pullInfo["DateCreated"].ToString(), param.ShortPatternFormate,param.OffTimeVal);
                    model.PullOperator = pullInfo["OperatorsId"].ToString();
                }

                model.ExceptionAllowed = false;
                model.isException = string.Compare(requestInfo.Status, "Exception", true) == 0;

                if (param.Passport.CheckPermission(" Requestor", Smead.Security.SecureObject.SecureObjectType.Reports, Permissions.Permission.Configure))
                {
                    model.ExceptionAllowed = "|Exception|In Process|New|New Batch|".IndexOf(string.Format("|{0}|", requestInfo.Status)) >= 0;
                }

                if (model.ExceptionAllowed)
                    model.isComment = true;
                model.txtComment = requestInfo.ExceptionComments;

            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.Passport.DatabaseName} CompanyName: {param.Passport.License.CompanyName}");
                throw new Exception(ex.Message);
            }
            return model;
        }
        

        #region Private Methods
        private List<string> ItemsRequest(UserInterfaceJsonModel @params, Requesters model,Passport passport)
        {
            bool allowWaitList = Navigation.CBoolean(Navigation.GetSystemSetting("AllowWaitList", passport));
            var requests = new Requesting();
            var reqList = requests.GetActiveRequests(@params.ViewId, @params.ids[0], passport);
            model.chkWaitList = false;
            var lst = new List<string>();
            foreach (string id in @params.ids)
            {
                bool atAnEmployee = Tracking.AlreadyAtAnEmployee(@params.TableName, id, passport);
                if (allowWaitList && atAnEmployee | reqList.Count > 0)
                {
                    model.chkWaitList = true;
                }
                else if (!allowWaitList && atAnEmployee)
                {
                    model.lblError = "One or more items requested are checked out to other employees, these requests will not be processed";
                }
                string ItemName = Navigation.GetItemName(@params.TableName, id, passport);
                lst.Add(ItemName);
            }
            return lst;
        }

        private List<RequstRadioBox> DrawRequestorList(UserInterfaceJsonModel @params,Passport passport)
        {
            var li = new List<RequstRadioBox>();
            var lil = new List<RequstRadioBox>();
            bool _requestOnBehalf = passport.CheckPermission(@params.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.RequestOnBehalf);
            string userName = string.Empty;
            var requestorRow = Tracking.GetRequestorTableInfo(passport);
            string requestorTableName = requestorRow["TableName"].ToString();
            userName = Navigation.GetEmployeeRequestorName(passport);
            var TrackingStatus = Tracking.TrackingStatusByTableAndId(@params.TableName, "", passport);
            var rq = new Requesting();
            bool allowWaitList = Navigation.CBoolean(Navigation.GetSystemSetting("AllowWaitList", passport));

            using (var conn = passport.Connection())
            {
                foreach (var req in rq.GetActiveRequests(@params.ViewId, @params.ids[0], passport, conn))
                    lil.Add(new RequstRadioBox() { text = req.Name, value = req.EmployeeID });
            }
            foreach (Container item in Tracking.GetContainersByType(requestorTableName, @params.Request.TextFilter, _requestOnBehalf, passport))
            {
                var TrackingRows = TrackingStatus.Select(string.Format("{0}='{1}'", requestorRow["TrackingStatusFieldName"].ToString(), item.ID));
                var objli = new RequstRadioBox();
                if (!(TrackingRows.Count() > 0))
                {
                    objli.text = item.Name;
                    objli.value = item.ID;
                    objli.disable = false;

                    foreach (RequstRadioBox re in lil)
                    {
                        if ((re.value ?? "") == (objli.value ?? ""))
                        {
                            objli.disable = true;
                            objli.text += " - " + "Already Requested";
                        }
                    }

                    // Else
                    // 'li.Selected = Not _requestOnBehalf OrElse String.Compare(userName, item.Name, True) = 0
                    // End If
                }
                li.Add(objli);
            }
            return li;
        }

        #endregion

    }
}
