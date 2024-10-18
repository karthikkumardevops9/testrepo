using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Services;
using Newtonsoft.Json.Linq;


namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LabelManagerController : ControllerBase
    {
        private readonly CommonControllersService<LabelManagerController> _commonService;
        private IDbConnection CreateConnection(string connectionString)
         => new SqlConnection(connectionString);
        public LabelManagerController(CommonControllersService<LabelManagerController> commonControllersService)
        {
            _commonService = commonControllersService;
        }

        [Route("GetAllLabelList")]
        [HttpGet]
        public async Task<List<OneStripJob>> GetAllLabelList(string ConnectionString)
        {
            List<OneStripJob> oneStripJobs = null;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    oneStripJobs = await context.OneStripJobs.Where(x => x.Inprint == 0).ToListAsync();

                }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return oneStripJobs;
        }

        [Route("GetFormList")]
        [HttpGet]
        public async Task<string> GetFormList(string ConnectionString)
        {
            string stringResult = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var oneStripForms = await context.OneStripForms.Where(x => x.Inprint == 0).ToListAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    stringResult = JsonConvert.SerializeObject(oneStripForms, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return stringResult;
        }

        [Route("GetLabelDetails")]
        [HttpPost]
        public async Task<ReturnLabelDetails> GetLabelDetails(LabelRequestParam param)
        {
            string onestripjobs = "";
            string onestripform = "";
            string onestripjobfields = "";
            var oBarCodePrefix = "";
            var tableName = "";
            var rowCount = 0;
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {

                    var pOneStripJob = await context.OneStripJobs.FirstOrDefaultAsync(x => x.Name.Trim().ToLower().Equals(param.Name.Trim().ToLower()));
                    var jobsId = pOneStripJob?.Id ?? 0;
                    var pOneStripJobField = await context.OneStripJobFields.Where(x => x.OneStripJobsId == jobsId).ToListAsync();
                    var oTable = await context.Tables.FirstOrDefaultAsync(x => x.TableName == pOneStripJob.TableName);
                    var pOneStripForm = await context.OneStripForms.FirstOrDefaultAsync(x => x.Id == pOneStripJob.OneStripFormsId);

                    if (oTable != null)
                    {
                        oBarCodePrefix = oTable.BarCodePrefix;
                        tableName = pOneStripJob.TableName;
                        rowCount = await GetCountAsync(tableName, param.ConnectionString);
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    onestripjobfields = JsonConvert.SerializeObject(pOneStripJobField, Formatting.Indented, Setting);
                    onestripjobs = JsonConvert.SerializeObject(pOneStripJob, Formatting.Indented, Setting);
                    onestripform = JsonConvert.SerializeObject(pOneStripForm, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            var result = new ReturnLabelDetails()
            {
                onestripjob = onestripjobs,
                onestripform = onestripform,
                barCodePrefix = oBarCodePrefix,
                rowCount = rowCount,
                onestripjobfields = onestripjobfields
            };
            return result;
        }

        [Route("CreateSQLString")]
        [HttpPost]
        public async Task<string> CreateSQLString(CreateSQLStringParam param)
        {
            string SQLString = string.Empty;
            try
            {
                DataTable lOutput = new DataTable();
                List<string> tablePrimaryKeys = new List<string>();
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pTable = await context.Tables.FirstOrDefaultAsync(x => x.TableName.Trim().ToLower().Equals(param.TableName.Trim().ToLower()) && !string.IsNullOrEmpty(x.DBName.Trim().ToLower()));

                    if (pTable != null)
                    {
                        var DbEntity = await context.Databases.FirstOrDefaultAsync(x => x.DBName.Trim().ToLower().Equals(pTable.DBName.Trim().ToLower()));
                        if (DbEntity != null)
                            param.ConnectionString = _commonService.GetConnectionString(DbEntity, false);
                    }

                    using (var con = CreateConnection(param.ConnectionString))
                    {
                        var sSql = "SELECT * FROM [" + param.TableName + "]";
                        var reader = await con.ExecuteReaderAsync(sSql, CommandType.Text);
                        lOutput.Load(reader, LoadOption.OverwriteChanges);

                        sSql = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC  INNER JOIN  INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
                                 ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME WHERE TC.TABLE_NAME = @TableName AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY';";
                        var res = await con.QueryAsync<string>(sSql, new { TableName = param.TableName });
                        tablePrimaryKeys = res.ToList();
                    }
                    string pColumn = null;
                    if (tablePrimaryKeys.Count == 0)
                    {
                        for (int i = 0; i <= lOutput.Columns.Count - 1; i += 1)
                        {
                            if (lOutput.Columns[i].Caption.Trim().ToLower().Equals("id"))
                                pColumn = lOutput.Columns[i].Caption;

                            if (pColumn == null & lOutput.Columns.Count > 0)
                                pColumn = lOutput.Columns[0].Caption;
                        }
                    }
                    else
                        pColumn = tablePrimaryKeys[0].ToString();

                    if (pColumn == null)
                        SQLString = "SELECT [" + param.TableName + "].* FROM [" + param.TableName + "]"; // WHERE " + tableName + "." + pColumn + " = '%ID%'"
                    else
                        SQLString = "SELECT [" + param.TableName + "].* FROM [" + param.TableName + "] WHERE " + param.TableName + "." + pColumn + " = '%ID%'";
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return SQLString;
        }

        [Route("GetFirstValue")]
        [HttpPost]
        public async Task<ReturnFirstValue> GetFirstValue(GetFirstValueParam param)
        {
            var sString = "";
            var sSql = "";
            DataTable lOutput = new DataTable();
            var oBarCodePrefix = "";
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pTable = await context.Tables.FirstOrDefaultAsync(x => x.TableName.Trim().ToLower().Equals(param.TableName.Trim().ToLower()) && !string.IsNullOrEmpty(x.DBName.Trim().ToLower()));
                    var oTable = await context.Tables.FirstOrDefaultAsync(x => x.TableName.Trim().ToLower().Equals(param.TableName.Trim().ToLower()));

                    oBarCodePrefix = oTable.BarCodePrefix;

                    if (pTable != null)
                    {
                        var DbEntity = await context.Databases.FirstOrDefaultAsync(x => x.DBName.Trim().ToLower().Equals(pTable.DBName.Trim().ToLower()));
                        if (DbEntity != null)
                            param.ConnectionString = _commonService.GetConnectionString(DbEntity, false);
                    }
                    var finalSQL = param.SqlString.Split(new string[] { "WHERE" }, StringSplitOptions.None)[0];
                    var dateColumn = true;
                    if (param.Field.ToString().ToLower().Contains("date"))
                    {
                        sSql = "SELECT TOP 1 *, CONVERT(VARCHAR(19)," + param.Field + ",121) As DateColumn, " + finalSQL.Substring(7);
                        // sSql = "SELECT TOP 1 *,CONVERT(VARCHAR(19)," + field + ",121) As DateColumn FROM " + table
                        dateColumn = true;
                    }
                    else
                    {
                        sSql = "SELECT TOP 1 " + finalSQL.Substring(7);
                        // sSql = "SELECT TOP 1 * FROM " + table
                        dateColumn = false;
                    }
                    using (var con = CreateConnection(param.ConnectionString))
                    {
                        var reader = await con.ExecuteReaderAsync(sSql, CommandType.Text);
                        lOutput.Load(reader, LoadOption.OverwriteChanges);
                        if (lOutput.Rows.Count == 0)
                            sString = param.Field;
                        else if (dateColumn)
                            sString = _commonService.GetConvertCultureDate(lOutput.Rows[0]["DateColumn"].ToString(), param.CultureShortPattern, param.OffSetVal, param.BWithTime, param.ConvertToLocalTimeZone);
                        else
                            sString = lOutput.Rows[0][param.Field].ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                sString = param.Field;
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }

            return new ReturnFirstValue() { Str = sString, BarcodePrefix = oBarCodePrefix };
        }

        [Route("AddLabel")]
        [HttpPost]
        public async Task<AddlabelRespose> AddLabel(AddLabelParam param)
        {
            AddlabelRespose result = new AddlabelRespose();
            string sMessage = string.Empty;
            int lError = 0;
            var qlist = new List<object>();
            var Setting = new JsonSerializerSettings();
            Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var oTable = await context.Tables.FirstOrDefaultAsync(x => x.TableName == param.OneStripJobs.TableName);
                    result.BarCodePrefix = oTable.BarCodePrefix;
                    var sSQL = param.OneStripJobs.SQLString.Replace("%ID%", "0");
                    var uSQL = param.OneStripJobs.SQLUpdateString;

                    if (sSQL.Trim().Length > 0)
                    {
                        using (var conn = CreateConnection(param.ConnectionString))
                        {
                            var data = await conn.QueryAsync<object>(sSQL);
                            qlist = data.ToList();

                            if (qlist.Count > 0)
                            {
                                if ((lError == -2147217865))
                                {
                                    // sMessage = sMessage & " contains an Invalid Table Name."
                                    result.ErrType = "e";
                                    result.ErrMsg = "The SQL Statement contains an Invalid Table Name";
                                }
                                else
                                {
                                    // sMessage = sMessage & " is Invalid."
                                    result.ErrType = "e";
                                    result.ErrMsg = "The SQL Statement is Invalid";
                                }
                                return result;
                            }

                            if ((uSQL != null))
                            {
                                uSQL = uSQL.Replace("%ID%", "0");
                                uSQL = _commonService.InjectWhereIntoSQL(uSQL, "0=1");
                                // check by raju
                                qlist = conn.Query(uSQL).ToList();
                                if (qlist.Count > 0)
                                {
                                    if ((lError == -2147217865))
                                    {
                                        // sMessage = sMessage & " contains an Invalid Table Name."
                                        result.ErrType = "e";
                                        result.ErrMsg = "The SQL Statement contains an Invalid Table Name";
                                    }
                                    else
                                    {
                                        // sMessage = sMessage & " is Invalid."
                                        result.ErrType = "e";
                                        result.ErrMsg = "The Update SQL Statement is Invalid";
                                    }
                                    // Fail
                                    return result;
                                }
                            }
                        }
                    }

                    if (param.OneStripJobs.Id > 0)
                    {
                        var tempOneStripJobs = await context.OneStripJobs.FirstOrDefaultAsync(x => x.Name.Trim().ToLower().Equals(param.OneStripJobs.Name.Trim().ToLower()) && x.Id != param.OneStripJobs.Id);
                        if (tempOneStripJobs == null)
                        {
                            param.OneStripJobs.Inprint = 0;
                            param.OneStripJobs.UserUnits = 0;
                            param.OneStripJobs.LastCounter = 0;
                            param.OneStripJobs.DrawLabels = param.DrawLabels;
                            context.Entry(param.OneStripJobs).State = EntityState.Modified;
                            await context.SaveChangesAsync();

                            result.LabelId = (await context.OneStripJobs.Where(x => x.Name.Trim().ToLower().Equals(param.OneStripJobs.Name.Trim().ToLower())).FirstOrDefaultAsync())?.Id ?? 0;

                            var oOneStripJobs = await context.OneStripJobs.FirstOrDefaultAsync(x => x.Name.Trim().ToLower().Equals(param.OneStripJobs.Name.Trim().ToLower()));
                            var oOneStripJobFields = context.OneStripJobFields.Where(x => x.OneStripJobsId == param.OneStripJobs.Id);
                            result.OneStripFields = JsonConvert.SerializeObject(oOneStripJobFields, Formatting.Indented, Setting);
                            result.OneStripJobs = JsonConvert.SerializeObject(oOneStripJobs, Formatting.Indented, Setting);
                            result.ErrType = "s";
                            result.ErrMsg = "Label Updated Successfully";
                        }
                        else
                        {
                            result.ErrType = "w";
                            result.ErrMsg = "Label Name Already Exists!";
                        }
                    }
                    else
                    {
                        var pOneStripJob = await context.OneStripJobs.FirstOrDefaultAsync(x => x.Name.Trim().ToLower().Equals(param.OneStripJobs.Name.Trim().ToLower()));
                        if (pOneStripJob == null)
                        {
                            param.OneStripJobs.Inprint = 0;
                            param.OneStripJobs.UserUnits = 0;
                            param.OneStripJobs.LastCounter = 0;
                            param.OneStripJobs.DrawLabels = param.DrawLabels;
                            context.OneStripJobs.Add(param.OneStripJobs);
                            await context.SaveChangesAsync();
                            result.LabelId = (await context.OneStripJobs.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == param.OneStripJobs.Name.Trim().ToLower()))?.Id ?? 0;
                            result.ErrType = "s";
                            result.ErrMsg = "Label Created Successfully";
                        }
                        else
                        {
                            result.LabelId = 0;
                            result.ErrType = "w";
                        }
                    }
                    var tableName = param.OneStripJobs.TableName.ToString();
                    result.RowCount = await GetCountAsync(tableName, param.ConnectionString);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("SetLabelObjects")]
        [HttpPost]
        public async Task<string> SetLabelObjects(SetLabelObjectsParam param)
        {
            string result = string.Empty;
            var Setting = new JsonSerializerSettings();
            //Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            var jsonObject = JsonConvert.DeserializeObject<JObject>(param.JsonArray);

            var LabelObjectType = new DataTable();
            LabelObjectType.Columns.Add("Id", typeof(int));
            LabelObjectType.Columns.Add("OneStripJobsId", typeof(int));
            LabelObjectType.Columns.Add("FieldName", typeof(string));
            LabelObjectType.Columns.Add("Format", typeof(string));
            LabelObjectType.Columns.Add("Type", typeof(string));
            LabelObjectType.Columns.Add("XPos", typeof(double));
            LabelObjectType.Columns.Add("YPos", typeof(double));
            LabelObjectType.Columns.Add("BCStyle", typeof(int));
            LabelObjectType.Columns.Add("BCWidth", typeof(double));
            LabelObjectType.Columns.Add("BCHeight", typeof(double));
            LabelObjectType.Columns.Add("Order", typeof(int));
            LabelObjectType.Columns.Add("ForeColor", typeof(string));
            LabelObjectType.Columns.Add("BackColor", typeof(string));
            LabelObjectType.Columns.Add("FontSize", typeof(double));
            LabelObjectType.Columns.Add("FontName", typeof(string));
            LabelObjectType.Columns.Add("FontBold", typeof(bool));
            LabelObjectType.Columns.Add("FontItalic", typeof(bool));
            LabelObjectType.Columns.Add("FontUnderline", typeof(bool));
            LabelObjectType.Columns.Add("FontStrikeThru", typeof(bool));
            LabelObjectType.Columns.Add("FontTransparent", typeof(bool));
            LabelObjectType.Columns.Add("FontOrientation", typeof(int));
            LabelObjectType.Columns.Add("Alignment", typeof(int));
            LabelObjectType.Columns.Add("BCBarWidth", typeof(double));
            LabelObjectType.Columns.Add("BCDirection", typeof(int));
            LabelObjectType.Columns.Add("BCUPCNotches", typeof(int));
            LabelObjectType.Columns.Add("StartChar", typeof(int));
            LabelObjectType.Columns.Add("MaxLen", typeof(int));
            LabelObjectType.Columns.Add("SpecialFunctions", typeof(int));

            try
            {
                foreach (var property in jsonObject.Properties())
                {
                    var item = property.Value as JObject;
                    if (item == null)
                        continue;

                    var labelObject = new
                    {
                        Id = (item["Id"] == null || string.IsNullOrWhiteSpace(item["Id"].ToString())) ? 0 : Convert.ToInt32(item["Id"]),
                        FieldName = Convert.ToString(item["FieldName"]),
                        Format = Convert.ToString(item["Format"]),
                        Type = Convert.ToString(item["Type"]),
                        XPos = (item["XPos"] == null || string.IsNullOrWhiteSpace(item["XPos"].ToString())) ? 0.0 : Convert.ToDouble(item["XPos"]),
                        YPos = (item["YPos"] == null || string.IsNullOrWhiteSpace(item["YPos"].ToString())) ? 0.0 : Convert.ToDouble(item["YPos"]),
                        BCStyle = (item["BCStyle"] == null || string.IsNullOrWhiteSpace(item["BCStyle"].ToString())) ? 0 : Convert.ToInt32(item["BCStyle"]),
                        BCWidth = (item["BCWidth"] == null || string.IsNullOrWhiteSpace(item["BCWidth"].ToString())) ? 0.0 : Convert.ToDouble(item["BCWidth"]),
                        BCHeight = (item["BCHeight"] == null || string.IsNullOrWhiteSpace(item["BCHeight"].ToString())) ? 0.0 : Convert.ToDouble(item["BCHeight"]),
                        Order = (item["Order"] == null || string.IsNullOrWhiteSpace(item["Order"].ToString())) ? 0 : Convert.ToInt32(item["Order"]),
                        ForeColor = Convert.ToString(item["ForeColor"]),
                        BackColor = Convert.ToString(item["BackColor"]),
                        FontSize = (item["FontSize"] == null || string.IsNullOrWhiteSpace(item["FontSize"].ToString())) ? 0.0 : Convert.ToDouble(item["FontSize"]),
                        FontName = Convert.ToString(item["FontName"]),
                        FontBold = (bool?)item["FontBold"] ?? false,
                        FontItalic = (bool?)item["FontItalic"] ?? false,
                        FontUnderline = (bool?)item["FontUnderline"] ?? false,
                        FontStrikeThru = (bool?)item["FontStrikeThru"] ?? false,
                        FontTransparent = (bool?)item["FontTransparent"] ?? false,
                        FontOrientation = (item["FontOrientation"] == null || string.IsNullOrWhiteSpace(item["FontOrientation"].ToString())) ? 0 : Convert.ToInt32(item["FontOrientation"]),
                        Alignment = (item["Alignment"] == null || string.IsNullOrWhiteSpace(item["Alignment"].ToString())) ? 0 : Convert.ToInt32(item["Alignment"]),
                        BCBarWidth = (item["BCBarWidth"] == null || string.IsNullOrWhiteSpace(item["BCBarWidth"].ToString())) ? 0.0 : Convert.ToDouble(item["BCBarWidth"]),
                        BCDirection = (item["BCDirection"] == null || string.IsNullOrWhiteSpace(item["BCDirection"].ToString())) ? 0 : Convert.ToInt32(item["BCDirection"]),
                        BCUPCNotches = (item["BCUPCNotches"] == null || string.IsNullOrWhiteSpace(item["BCUPCNotches"].ToString())) ? 0 : Convert.ToInt32(item["BCUPCNotches"]),
                        StartChar = (item["StartChar"] == null || string.IsNullOrWhiteSpace(item["StartChar"].ToString())) ? 0 : Convert.ToInt32(item["StartChar"]),
                        MaxLen = (item["MaxLen"] == null || string.IsNullOrWhiteSpace(item["MaxLen"].ToString())) ? 0 : Convert.ToInt32(item["MaxLen"]),
                        SpecialFunctions = (item["SpecialFunctions"] == null || string.IsNullOrWhiteSpace(item["SpecialFunctions"].ToString())) ? 0 : Convert.ToInt32(item["SpecialFunctions"])
                    };

                    LabelObjectType.Rows.Add(
                        labelObject.Id, param.OneStripJobsId, labelObject.FieldName, labelObject.Format, labelObject.Type,
                        labelObject.XPos, labelObject.YPos, labelObject.BCStyle, labelObject.BCWidth, labelObject.BCHeight,
                        labelObject.Order, labelObject.ForeColor, labelObject.BackColor, labelObject.FontSize, labelObject.FontName,
                        labelObject.FontBold, labelObject.FontItalic, labelObject.FontUnderline, labelObject.FontStrikeThru,
                        labelObject.FontTransparent, labelObject.FontOrientation, labelObject.Alignment, labelObject.BCBarWidth,
                        labelObject.BCDirection, labelObject.BCUPCNotches, labelObject.StartChar, labelObject.MaxLen, labelObject.SpecialFunctions
                    );
                }


                using (var con = CreateConnection(param.ConnectionString))
                {
                    var queryParam = new DynamicParameters();
                    queryParam.Add("LabelObjectType", LabelObjectType.AsTableValuedParameter("dbo.TableType_RMS_AddEditLabelObject"));
                    queryParam.Add("OneStripJobsId", param.OneStripJobsId);
                    await con.ExecuteAsync("SP_RMS_AddEditLabelObjectDetails", queryParam, commandType: CommandType.StoredProcedure);
                }
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pOneStripJobFields = await context.OneStripJobFields.Where(x => x.OneStripJobsId == param.OneStripJobsId).ToListAsync();
                    result = JsonConvert.SerializeObject(pOneStripJobFields, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("DeleteLabel")]
        [HttpPost]
        public async Task<ApiResponse> DeleteLabel(LabelRequestParam param)
        {
            ApiResponse result = new ApiResponse();
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pOneStripJob = await context.OneStripJobs.FirstOrDefaultAsync(x => x.Name.Trim().ToLower().Equals(param.Name.Trim().ToLower()));

                    var jobsId = pOneStripJob?.Id ?? 0;
                    var pOneStripJobField = await context.OneStripJobFields.Where(x => x.OneStripJobsId == jobsId).ToListAsync();
                    context.OneStripJobs.Remove(pOneStripJob);
                    context.OneStripJobFields.RemoveRange(pOneStripJobField);
                    await context.SaveChangesAsync();
                    result.ErrType = "s";
                    result.ErrorMsg = "Label Deleted Successfully";
                }

            }
            catch (Exception ex)
            {
                result.ErrType = "e";
                result.ErrorMsg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("GetNextRecord")]
        [HttpPost]
        public async Task<string> GetNextRecord(GetNextRecordParam param)
        {
            string rowData = null;
            try
            {
                DataTable lOutput = new DataTable();
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var pTable = await context.Tables.FirstOrDefaultAsync(x => x.TableName.Trim().ToLower().Equals(param.TableName.Trim().ToLower()) && !string.IsNullOrEmpty(x.DBName.Trim().ToLower()));

                    Databas pDatabaseEntity = null;
                    if (pTable != null)
                    {
                        pDatabaseEntity = await context.Databases.FirstOrDefaultAsync(x => x.DBName.Trim().ToLower().Equals(pTable.DBName.Trim().ToLower()));
                        if (pDatabaseEntity != null)
                            param.ConnectionString = _commonService.GetConnectionString(pDatabaseEntity, false);
                    }
                    string finalSQL = param.SqlString.Split(new string[] { "WHERE" }, StringSplitOptions.None)[0];
                    using (var con = CreateConnection(param.ConnectionString))
                    {
                        string countQuery = "Select * from (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT '')) AS  RowNum, " + finalSQL.Substring(7) + ") T1 WHERE T1.RowNum = @RowNo";

                        var reader = await con.ExecuteReaderAsync(countQuery, new { RowNo = param.RowNo });
                        lOutput.Load(reader);

                        var Setting = new JsonSerializerSettings();
                        Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

                        if (lOutput != null)
                            rowData = JsonConvert.SerializeObject(lOutput, Formatting.Indented, Setting);
                        else
                            rowData = "[]";

                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return rowData;
        }

        [Route("SetAsDefault")]
        [HttpPost]
        public async Task<ReturnSetAsDefault> SetAsDefaut(SetAsDefautParam param)
        {
            var result = new ReturnSetAsDefault();
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var oONeStripJobs = await context.OneStripJobs.FirstOrDefaultAsync(x => x.Id == param.OneStripJobsId);
                    var oOneStripForms = await context.OneStripForms.FirstOrDefaultAsync(x => x.Id == param.OneStripFormsId);

                    oONeStripJobs.OneStripFormsId = param.OneStripFormsId;
                    oONeStripJobs.LabelHeight = oOneStripForms.LabelHeight;
                    oONeStripJobs.LabelWidth = oOneStripForms.LabelWidth;
                    context.Entry(oONeStripJobs).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    result.ErrType = "s";
                    result.ErrMsg = "";

                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                result.ErrType = "e";
                result.ErrMsg = "";
            }
            return result;
        }

        #region Private Method
        private async Task<int> GetCountAsync(string tableName, string connectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    var pTable = await context.Tables.FirstOrDefaultAsync(x => x.TableName.Trim().ToLower().Equals(tableName.Trim().ToLower()) && !string.IsNullOrEmpty(x.DBName.Trim().ToLower()));

                    Databas pDatabaseEntity = null;
                    if (pTable != null)
                    {
                        pDatabaseEntity = await context.Databases.FirstOrDefaultAsync(x => x.DBName.Trim().ToLower().Equals(pTable.DBName.Trim().ToLower()));
                        if (pDatabaseEntity != null)
                            connectionString = _commonService.GetConnectionString(pDatabaseEntity, false);
                    }

                    using (var conn = CreateConnection(connectionString))
                    {
                        var countQuery = "SELECT COUNT(1) FROM " + tableName;
                        var loutput = await conn.ExecuteScalarAsync(countQuery, CommandType.Text);
                        return Convert.ToInt32(loutput);
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                return 0;
            }
        }
        #endregion
    }
}
