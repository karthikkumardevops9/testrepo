using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Services;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using MSRecordsEngine.RecordsManager;
using Dapper;
using MSRecordsEngine.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MSRecordsEngine.Entities;
using Newtonsoft.Json;
using MSRecordsEngine.Models.FusionModels;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PrintBarcodeController : ControllerBase
    {
        private IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);
        private readonly CommonControllersService<PrintBarcodeController> _commonService;


        public PrintBarcodeController(CommonControllersService<PrintBarcodeController> commonControllersService)
        {
            _commonService = commonControllersService;
        }


        [Route("InitiateBarcodePopup")]
        [HttpPost]
        public async Task<string> InitiateBarcodePopup(TestPrintBarcodeApi_Request _Request)
        {
            var req = _Request.Request;

            var model = new PrintBarcodeResponse();
            var jsonObject = string.Empty;
            try
            {
                string inClause = string.Empty;

                if (!string.Join(",", req.paramss.ids.ToArray()).Equals(""))
                {
                    inClause = " IN ('" + string.Join("','", req.paramss.ids.ToArray()) + "')";
                }
                else
                {
                    model.Msg = "Table label SQL statement returned no data. Unable to continue";
                    return jsonObject;
                }

                int labelId = 0;
                string labelName = string.Empty;
                var oneStripForms = new List<OneStripForm>();
                var oneStripJobs = new List<OneStripJob>();
                var LabelDataFields = new List<OneStripJobField>();

                using (var conn = CreateConnection(_Request.ConnectionString))
                {
                    var dtJobsSql = "SELECT * FROM [OneStripJobs] WHERE [TableName] = @tableName AND [InPrint] = 0";
                    var dtJobparam = new DynamicParameters();
                    dtJobparam.Add("@TableName", req.paramss.TableName);

                    oneStripJobs = (await conn.QueryAsync<OneStripJob>(dtJobsSql, dtJobparam, commandType: CommandType.Text)).ToList();
                    foreach (var item in oneStripJobs)
                        model.labelDesign.Add(new dropdown() { text = item.Name.Text(), value = item.Id.Text() });

                    var dtFormsSql = "SELECT * FROM[OneStripForms] WHERE[Inprint] = @Inprint";
                    var dtFormsparam = new DynamicParameters();
                    dtFormsparam.Add("@Inprint", 0);
                    oneStripForms = (await conn.QueryAsync<OneStripForm>(dtFormsSql, dtFormsparam, commandType: CommandType.Text)).ToList();
                    foreach(var item in oneStripForms)
                        model.labelForm.Add(new dropdown() { text = item.Name.Text(), value = item.Id.Text() });

                }

                labelId = oneStripJobs[0].Id.IntValue();  
                model.formSelectionid = oneStripJobs[0].OneStripFormsId.Text();

                labelName = oneStripJobs[0].Name.Text();

                //var loadPrintDataRequest = new LoadPrintDataRequest();
                //loadPrintDataRequest.OneStripJobs = oneStripJobs;
                //loadPrintDataRequest.LabelId = labelId;
                //loadPrintDataRequest.InClause = inClause;

                //model.LoadPrintDataRequest = loadPrintDataRequest;

                LabelDataFields = await LoadPrintDataFields(_Request.ConnectionString, labelId);
                var gridDataTable = await LoadPrintData(_Request.ConnectionString, inClause, oneStripJobs, labelId, Orderby(req.paramss));

                var lstGridData = new List<GridData>();
                foreach (DataRow gridRow in gridDataTable.Rows)
                {
                    var gridData = new GridData();
                    var clmData = new List<GridRow>();
                    foreach (var item in LabelDataFields)
                    {
                        var columnData = new GridRow();
                        if (!string.IsNullOrWhiteSpace(item.FieldName))
                        {
                            if (gridDataTable.Columns.Contains(item.FieldName))
                            {
                                columnData.FieldName = item.FieldName;
                                columnData.FieldValue = gridRow[item.FieldName].Text();
                            }
                            else
                            {
                                columnData.FieldName = item.FieldName;
                                columnData.FieldValue = item.FieldName;
                            }
                            clmData.Add(columnData);
                        }
                    }
                    gridData.GridRows = clmData;
                    lstGridData.Add(gridData);
                }

                model.oneStripJob = oneStripJobs[0];    
                model.oneStripForms = oneStripForms.Where(x => x.Id == Convert.ToInt64(model.formSelectionid)).ToList()[0];
                model.oneStripJobFields = LabelDataFields;
                model.OneStripForms = oneStripForms;
                model.OneStripJobs = oneStripJobs;
                model.GridDatas = lstGridData;

                var setting = new JsonSerializerSettings();
                setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                jsonObject = JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented, setting);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }


        [Route("GenerateBarcodeOnchange")]
        [HttpPost]
        public async Task<string> GenerateBarcodeOnchange(GenerateBarcodeOnchange_Request _Request)
        {
            var req = _Request.ReqModel;
            var dtForms = _Request.OneStripForms;
            var dtJobs = _Request.oneStripJobs;
            var ConnectionString = _Request.ConnectionString;

            var model = new PrintBarcodeResponse();
            var m = new PrintBarcodeCommonModel();
            model.labelOutline = req.paramss.PrintBarcode.labelOutline;
            model.strtPrinting = req.paramss.PrintBarcode.strtPrinting;
            var jsonObject = string.Empty;
            try
            {
                string inClause = string.Empty;

                if (!string.Join(",", req.paramss.ids.ToArray()).Equals(""))
                {
                    inClause = " IN ('" + string.Join("','", req.paramss.ids.ToArray()) + "')";
                }
                else
                {
                    model.Msg = "Table label SQL statement returned no data. Unable to continue";
                    return jsonObject;
                }
                
                int labelId = req.paramss.PrintBarcode.labelDesignSelectedValue;
                
                if (req.paramss.PrintBarcode.isLabelDropdown)
                    model.formSelectionid = dtJobs[req.paramss.PrintBarcode.labelIndex].OneStripFormsId.ToString();
                else
                    model.formSelectionid = req.paramss.PrintBarcode.labelFormSelectedValue.ToString();

                var dtFormat = new List<OneStripJobField>();

                //var loadPrintDataRequest = new LoadPrintDataRequest();
                //loadPrintDataRequest.InClause = inClause;
                //loadPrintDataRequest.OneStripJobs = dtJobs;
                //loadPrintDataRequest.LabelId = labelId;
                //model.LoadPrintDataRequest = loadPrintDataRequest;
                dtFormat = await LoadPrintDataFields(ConnectionString, labelId);

                var gridDataTable = await LoadPrintData(_Request.ConnectionString, inClause, dtJobs, labelId, Orderby(req.paramss));

                var lstGridData = new List<GridData>();
                foreach (DataRow gridRow in gridDataTable.Rows)
                {
                    var gridData = new GridData();
                    var clmData = new List<GridRow>();
                    foreach (var item in dtFormat)
                    {
                        var columnData = new GridRow();
                        if (!string.IsNullOrWhiteSpace(item.FieldName))
                        {
                            if (gridDataTable.Columns.Contains(item.FieldName))
                            {
                                columnData.FieldName = item.FieldName;
                                columnData.FieldValue = gridRow[item.FieldName].Text();
                            }
                            else
                            {
                                columnData.FieldName = item.FieldName;
                                columnData.FieldValue = item.FieldName;
                            }
                            clmData.Add(columnData);
                        }
                    }
                    gridData.GridRows = clmData;
                    lstGridData.Add(gridData);
                }

                model.oneStripJob = dtJobs[req.paramss.PrintBarcode.labelIndex];
                model.oneStripForms = dtForms.Where(x => x.Id == Convert.ToInt64(model.formSelectionid)).ToList()[0];
                model.oneStripJobFields = dtFormat;
                model.GridDatas = lstGridData;
                var setting = new JsonSerializerSettings();
                setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                jsonObject = JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented, setting);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }


        [Route("SetDefaultBarcodeForm")]
        [HttpPost]
        public async Task<List<OneStripJob>> SetDefaultBarcodeForm(SetDefaultBarcodeForm_Request _Request)
        {
            var req = _Request.ReqModel;
            var ConnectionString = _Request.ConnectionString;
            var oneStripForms = _Request.OneStripForms;


            var oneStripJobs = new List<OneStripJob>();
            try
            {
                int frmId = req.paramss.PrintBarcode.labelFormSelectedValue;
                var labelId = req.paramss.PrintBarcode.labelDesignSelectedValue;
                var dtForms = oneStripForms;
                var frmObject = dtForms.Where(x => x.Id == frmId).FirstOrDefault();

                var labelHeight = frmObject.LabelHeight;
                var labelWidth = frmObject.LabelWidth;

                var sqlCmd = string.Empty;

                using (var conn = CreateConnection(ConnectionString))
                {
                    sqlCmd = "Update [OneStripJobs] SET [OneStripFormsId] = @frmId, [LabelHeight] = @lblHeight, [LabelWidth] = @lblWidth WHERE [Id] = @id";
                    var param = new DynamicParameters();
                    param.Add("@frmId", frmId);
                    param.Add("@lblHeight", labelHeight);
                    param.Add("@lblWidth", labelWidth);
                    param.Add("@id", labelId);
                    await conn.ExecuteScalarAsync(sqlCmd, param, commandType: CommandType.Text);
                    sqlCmd = string.Empty;


                    sqlCmd = "SELECT * FROM [OneStripJobs] WHERE [TableName] = @tableName AND [InPrint] = 0";
                    var @params = new DynamicParameters();
                    @params.Add("@TableName", req.paramss.TableName);

                    oneStripJobs = (await conn.QueryAsync<OneStripJob>(sqlCmd, @params, commandType: CommandType.Text)).ToList();
                }

            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return oneStripJobs;
        }


        [Route("DownloadbarcodefileAsync")]
        [HttpGet]
        public async Task DownloadbarcodefileAsync(int labelId, string inClause, string ConnectionString)
        {
            try
            {
                await LabelPrintUpdate(labelId, inClause, ConnectionString);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        #region Private Methods

        private string Orderby(UserInterfaceJsonModel @params)
        {
            string str = string.Empty;
            string ascdesc = string.Empty;
            int counter = 0;
            if (@params.PrintBarcode == null)
                return "";
            foreach (SortableFileds item in @params.PrintBarcode.sortableFields)
            {
                counter = counter + 1;
                if (item.SortOrderDesc == false || item.SortOrderDesc == null)
                {
                    ascdesc = "asc";
                }
                else
                {
                    ascdesc = "desc";
                }
                if (counter == @params.PrintBarcode.sortableFields.Count)
                {
                    str += item.FieldName + " " + ascdesc;
                }
                else
                {
                    str += item.FieldName + " " + ascdesc + ",";
                }
            }

            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            else
            {
                return "order by " + str;
            }
        }

        private async Task<DataTable> LoadPrintData(string ConnectionString, string inClause, List<OneStripJob> dtJobs, int labelId, string orderby)
        {
            var dtData = new DataTable("LabelData");
            var LabelData = new List<dynamic>();
            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var job = dtJobs.Where(x => x.Id == labelId).FirstOrDefault();
                    string query = Navigation.NormalizeString(job.SQLString.ToString());

                    if (query.EndsWith(";"))
                        query = query.Substring(0, query.Length - 1).Trim();

                    query = query.Replace("= %ID%", inClause);
                    query = query.Replace("=%ID%", inClause);
                    query = query.Replace("='%ID%'", inClause);
                    query = query.Replace("= '%ID%'", inClause);

                    var sqlCmd = string.Format("{0} {1}", query, orderby);

                    var da = await conn.ExecuteReaderAsync(sqlCmd, commandType: CommandType.Text);
                    dtData.Load(da);

                    LabelData = (await conn.QueryAsync<dynamic>(sqlCmd, commandType: CommandType.Text)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dtData;
        }

        private async Task<List<OneStripJobField>> LoadPrintDataFields(string ConnectionString, int labelId)
        {
            var LabelDataFields = new List<OneStripJobField>();

            try
            {
                using (var conn = CreateConnection(ConnectionString))
                {
                    var query = "SELECT * FROM [OneStripJobFields] WHERE [OneStripJobsID] = @JobID";
                    var param = new DynamicParameters();
                    param.Add("@JobID", labelId);
                    LabelDataFields = (await conn.QueryAsync<OneStripJobField>(query, param, commandType: CommandType.Text)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return LabelDataFields;
        }

        private async Task LabelPrintUpdate(int labelId, string inClause, string ConnectionString)
        {
            using (var conn = CreateConnection(ConnectionString))
            {
                string sqlUpdate;
                var sqlCmd = "SELECT [SQLUpdateString] FROM [OneStripJobs] WHERE [Id] = @labelId";
                var param = new DynamicParameters();
                param.Add("@labelId", labelId);
                try
                {
                    sqlUpdate = (await conn.ExecuteScalarAsync(sqlCmd, param, commandType: CommandType.Text)).ToString();
                }
                catch (Exception)
                {
                    sqlUpdate = string.Empty;
                }

                if (string.IsNullOrEmpty(sqlUpdate))
                    return;

                sqlUpdate = Navigation.NormalizeString(sqlUpdate);
                inClause = string.Format("IN ({0})", inClause);

                sqlUpdate = sqlUpdate.Replace("= %ID%", inClause);
                sqlUpdate = sqlUpdate.Replace("=%ID%", inClause);
                sqlUpdate = sqlUpdate.Replace("='%ID%'", inClause);
                sqlUpdate = sqlUpdate.Replace("= '%ID%'", inClause);

                await conn.ExecuteScalarAsync(sqlUpdate, commandType: CommandType.Text);
            }
        }

        #endregion


    }
}
