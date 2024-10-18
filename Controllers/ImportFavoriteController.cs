using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.Services;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using Dapper;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImportFavoriteController : ControllerBase
    {
        private readonly CommonControllersService<ImportFavoriteController> _commonService;
        private IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);
        public ImportFavoriteController(CommonControllersService<ImportFavoriteController> commonControllersService)
        {
            _commonService = commonControllersService;
        }


        [Route("GetImportFavoritpopup")]
        [HttpGet]
        public List<ImDropdownprops> GetImportFavoritpopup(string ConnectionString, int UserId)
        {
            try
            {
                var popup = BindDropdownList(ConnectionString, UserId);
                return popup;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        [Route("ImportFavoriteDDLChange")]
        [HttpPost]
        public ImportFavoriteModel ImportFavoriteDDLChange(ImportFavoriteDDLChangeParam importFavoriteDDLChangeParam)
        {
            var @params = importFavoriteDDLChangeParam.importFavoriteReqModel;
            var passport = importFavoriteDDLChangeParam.Passport;

            string viewid = @params.ImportFavorite.SelectedDropdown;
            var model = new ImportFavoriteModel();
            model.isfieldsExist = true;
            try
            {
                if (!(Convert.ToDouble(viewid) == 0d))
                {
                    string strQuery = string.Format("Select (SUBSTRING(FieldName, charindex('.',FieldName)+1,LEN(FieldName))) as FieldName,id from ViewColumns where viewsid={0} and LookupType = {1}", viewid, (int)Enums.geViewColumnsLookupType.ltDirect);

                    var dsFields = new DataTable();
                    using (SqlConnection conn = passport.Connection())
                    {
                        using (var cmd = new SqlCommand(strQuery, conn))
                        {
                            using (var da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dsFields);
                            }
                        }
                    }

                    var p = new Parameters(Int32.Parse(viewid), passport);

                    if (dsFields.Rows.Count > 0)
                    {
                        if (dsFields.Rows.Count > 0)
                        {
                            foreach (DataRow row in dsFields.Rows)
                            {
                                Type fieldType = Navigation.GetFieldType(p.TableName, row["FieldName"].ToString(), passport);
                                model.ListOfFieldName.Add(new ListOfFieldName() { text = row["FieldName"].ToString(), value = fieldType.Name });
                            }
                        }
                    }
                    else
                    {
                        model.isfieldsExist = false;
                    }
                }
                else
                {
                    model.isfieldsExist = false;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return model;
        }


        [Route("UploadingFileStreamAsync")]
        [HttpPost]
        public List<string> UploadingFileStreamAsync([FromForm] IFormFile fileStream, [FromForm] string extention, [FromForm] bool chk1RowHeader)
        {
            var columns = new List<string>();
            try
            {
                if (fileStream != null && fileStream.Length > 0)
                {
                    using (var stream = fileStream.OpenReadStream())
                    {
                        columns = GetColumnFromStream(extention, stream, chk1RowHeader);
                    }
                }
                else
                {
                    throw new Exception("Uploaded file is empty.");
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return columns;
        }


        [Route("StartInsertDataToFavorite")]
        [HttpPost]
        public async Task<ImportFavoriteModel> StartInsertDataToFavorite([FromForm] IFormFile fileStream, [FromForm] string model)
        {
            //var response = new StartInsertDataToFavorite_Response();

            var uiParams = JsonConvert.DeserializeObject<StartInsertDataToFavoriteParam>(model);
            var param = new Parameters(uiParams.m.ViewId, uiParams.Passport);

            var m = uiParams.m;
            var table = param.TableName;
            var pkey = param.PrimaryKey;
            var ConnectionString = uiParams.Passport.ConnectionString;
            var SessionDelmiter = uiParams.SessionDelmiter;
            //var UserId = uiParams.Passport.UserId;
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var dataset = await GetImportFavoriteData(m, table, pkey, ConnectionString, SessionDelmiter, fileStream);
                stopwatch.Stop();

                var elapsedTime = stopwatch.Elapsed;
                Debug.WriteLine($"Time taken to complete import: {elapsedTime}");

                m.ImportFavReport = WriteImportFavoriteStatusReport(dataset, pkey);
                if (dataset != null)
                {
                    m.UserMsg = "Records has been Imported into the Selected Favorite Successfully. Click on the 'Report' to see detailed information";
                    m.isValidate = true;
                }
                else
                {
                    //response.Success = false;
                    m.isValidate = false;
                    m.Msg = "File contains no data";
                }
                return m;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                if (ex.Message.Contains("Conversion failed when converting the varchar value"))
                {
                    m.errorType = "w";
                    m.UserMsg = string.Format("Source and Target field datatype mismatch - Please select correct target field from below 'or' Change the data in provided source file");
                }
                else
                {
                    m.UserMsg = ex.Message;
                }
                return m;
            }
        }

        #region Private Methods

        private List<ImDropdownprops> BindDropdownList(string ConnectionString, int UserId)
        {
            var lst = new List<ImDropdownprops>();
            var dsCriteria = new DataTable();
            string strQuery = string.Format("select SavedName,(CAST(id as varchar(20))+'|'+cast(isnull(ViewId,'0') AS varchar(20))) as ViewId from s_SavedCriteria where SavedType = {0} and UserId = {1}", (int)Enums.SavedType.Favorite, UserId);
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(strQuery, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsCriteria);
                    }
                }
            }

            foreach (DataRow row in dsCriteria.Rows)
                lst.Add(new ImDropdownprops() { text = (string)row["SavedName"], value = (string)row["ViewId"] });
            return lst;
        }

        private async Task<DataSet> GetImportFavoriteData(ImportFavoriteModel m, string table, string pkey, string ConnectionString, string SessionDelmiter, IFormFile sourceFile)
        {
            var lsttrg = new List<string>();
            var dataTableResult = new DataTable();
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(string));
            try
            {

                foreach (DataRow col in GetDataFromFileStream(m, SessionDelmiter, sourceFile).Rows)
                {
                    string datacol = string.Format("'{0}'", col[Convert.ToInt32(m.ColumnSelect)].ToString());
                    m.lstcoldata.Add(datacol);
                    dataTable.Rows.Add(col[Convert.ToInt32(m.ColumnSelect)]);
                }

                using (var connection = CreateConnection(ConnectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Data", dataTable.AsTableValuedParameter("[dbo].[TableType_RMS_ImportFavoriteIds]"));
                    parameters.Add("@TableName", table);
                    parameters.Add("@PrimaryKey", pkey);
                    parameters.Add("@SavedCriteriaId", m.favoritListSelectorid);
                    parameters.Add("@TargetedField", m.Targetfileds[0].text);

                    var result = await connection.ExecuteReaderAsync("[dbo].[SP_RMS_InsertImportFavoriteData]", parameters, commandType: CommandType.StoredProcedure, commandTimeout:120);

                    var dataSet = new DataSet();
                    dataTableResult.Load(result);
                    dataSet.Tables.Add(dataTableResult);

                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetColumnLetterFromColumnIndex(int colIndex)
        {
            int div = colIndex;
            string colLetter = string.Empty;
            int modnum = 0;

            while (div > 0)
            {
                modnum = (div - 1) % 26;
                colLetter = Convert.ToChar(65 + modnum) + colLetter;
                div = (div - modnum) / 26;
            }

            return string.Format("Column {0}", colLetter);
        }

        #region Use Streame

        private DataTable GetDataFromFileStream(ImportFavoriteModel m, string SessionDelmiter, IFormFile sourceFile)
        {
            var dtFileDataTable = new DataTable();
            string extension = Path.GetExtension(sourceFile.FileName).TrimStart('.');

            if (sourceFile != null && sourceFile.Length > 0)
            {
                using (var stream = sourceFile.OpenReadStream())
                {
                    switch (extension.ToLower())
                    {
                        case "xls":
                        case "xlsx":
                            {
                                dtFileDataTable = GetSampleDataFromExcelStream(stream, extension, m.chk1RowFieldNames);
                                break;
                            }
                        case "csv":
                            {
                                dtFileDataTable = GetDataTableFromCSVStream(stream, m.chk1RowFieldNames);
                                break;
                            }
                        case "txt":
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    string delimiter = SessionDelmiter;
                                    string strColName;

                                    var firstLine = reader.ReadLine();
                                    if (firstLine != null)
                                    {
                                        for (int index = 1; index <= firstLine.Split(delimiter).Length; index++)
                                        {
                                            strColName = GetColumnLetterFromColumnIndex(index);
                                            dtFileDataTable.Columns.Add(strColName);
                                        }

                                        bool isFirstRow = true;
                                        if (!m.chk1RowFieldNames)
                                        {
                                            var dtRow = dtFileDataTable.NewRow();
                                            dtRow.ItemArray = firstLine.Split(delimiter);
                                            dtFileDataTable.Rows.Add(dtRow);
                                        }
                                        while (!reader.EndOfStream)
                                        {
                                            var line = reader.ReadLine();
                                            if (line == null) continue;

                                            if (isFirstRow && m.chk1RowFieldNames)
                                            {
                                                isFirstRow = false;
                                                continue;
                                            }

                                            var dtRow = dtFileDataTable.NewRow();
                                            dtRow.ItemArray = line.Split(delimiter);
                                            dtFileDataTable.Rows.Add(dtRow);
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            return dtFileDataTable;
        }


        private List<string> GetColumnFromStream(string extension, Stream stream, bool chkFirstRow)
        {
            var lst = new List<string>();
            string strDelimiter = "";
            extension = extension.ToLower();

            switch (extension)
            {
                case "txt":
                case "csv":
                    {
                        strDelimiter = extension == "csv" ? "," : "\t";

                        using (var reader = new StreamReader(stream))
                        {
                            if (chkFirstRow)
                            {
                                reader.ReadLine();
                            }

                            var headers = reader.ReadLine()?.Split(strDelimiter);
                            if (headers != null)
                            {
                                for (int index = 0; index < headers.Length; index++)
                                {
                                    lst.Add(GetColumnLetterFromColumnIndex(index + 1));
                                }
                            }
                        }
                        break;
                    }
                case "xls":
                case "xlsx":
                    {
                        lst = getColumnFromFileStream(stream, extension, chkFirstRow);
                        break;
                    }
                default:
                    throw new NotSupportedException($"The file extension '{extension}' is not supported.");
            }
            return lst;
        }


        private static DataTable GetSampleDataFromExcelStream(Stream fileStream, string fileExtension, bool isHeader) //use stream
        {
            DataTable dataTable = new DataTable();

            fileStream.Position = 0;

            IWorkbook workbook;
            if (fileExtension.Equals("xls", StringComparison.OrdinalIgnoreCase))
            {
                workbook = new HSSFWorkbook(fileStream);
            }
            else if (fileExtension.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            else
            {
                throw new NotSupportedException($"The file extension '{fileExtension}' is not supported.");
            }

            ISheet sheet = workbook.GetSheetAt(0);
            int startRowIndex = isHeader ? 1 : 0;

            IRow headerRow = sheet.GetRow(0);
            if (isHeader && headerRow != null)
            {
                for (int j = 0; j < headerRow.LastCellNum; j++)
                {
                    string columnName = headerRow.GetCell(j) != null ? headerRow.GetCell(j).ToString() : $"Column{j + 1}";
                    dataTable.Columns.Add(columnName);
                }
            }
            else
            {
                for (int j = 0; j < sheet.GetRow(startRowIndex).LastCellNum; j++)
                {
                    dataTable.Columns.Add($"Column{j + 1}");
                }
            }

            for (int i = startRowIndex; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null)
                {
                    DataRow dataRow = dataTable.NewRow();

                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        dataRow[j] = row.GetCell(j)?.ToString() ?? "";
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            workbook.Close();
            return dataTable;
        }


        private static List<string> getColumnFromFileStream(Stream fileStream, string fileExtension, bool IsHeader)
        {
            List<string> colArray = new List<string>();
            DataTable dtExcelData = new DataTable();
            dtExcelData = GetSampleDataFromExcelStream(fileStream, fileExtension, IsHeader);
            if (dtExcelData != null)
            {
                if (IsHeader)
                {
                    if (dtExcelData.Columns.Count > 0)
                    {
                        for (var index = 0; index < dtExcelData.Columns.Count; index++)
                            colArray.Add(dtExcelData.Columns[index].Caption);
                    }
                }
                else
                {
                    if (dtExcelData.Columns.Count > 0)
                    {
                        for (var index = 1; index <= dtExcelData.Columns.Count; index++)
                            colArray.Add(GetColumnLetterFromColumnIndex(index));
                    }
                }
            }
            return colArray;
        }


        private static DataTable GetDataTableFromCSVStream(Stream csvStream, bool chk1RowFieldNames)
        {
            var csvData = new DataTable();
            try
            {
                using (var csvReader = new TextFieldParser(new StreamReader(csvStream)))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;

                    var colFields = csvReader.ReadFields();
                    if (chk1RowFieldNames)
                    {
                        for (int i = 0; i < colFields.Length; i++)
                        {
                            csvData.Columns.Add(colFields[i] ?? $"Column{i + 1}");
                        }
                    }
                    else
                    {
                        for (int index = 1; index <= colFields.Length; index++)
                        {
                            var strColName = GetColumnLetterFromColumnIndex(index);
                            csvData.Columns.Add(strColName);
                        }

                        csvData.Rows.Add(colFields);
                    }

                    while (!csvReader.EndOfData)
                    {
                        var fieldData = csvReader.ReadFields();
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (string.IsNullOrEmpty(fieldData[i]))
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading CSV data", ex);
            }

            return csvData;
        }


        private string WriteImportFavoriteStatusReport(DataSet dataset, string pkey) //retirn string that can be used in place of physical file
        {
            StringBuilder htmlContent = new StringBuilder();

            //m.AReportUrl = string.Format("D://TestImportFav/{0}/Report.html", UserId);

            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine("<style>");
            htmlContent.AppendLine("body { font-family: Arial; }");
            htmlContent.AppendLine(".imported { color: green; }");
            htmlContent.AppendLine(".not-existed { color: red; }");
            htmlContent.AppendLine(".duplicate { color: blue; }");
            htmlContent.AppendLine(".accordion { background-color: #eee; color: #444; cursor: pointer; padding: 18px; width: 100%; border: none; text-align: left; outline: none; font-size: 15px; transition: 0.4s; }");
            htmlContent.AppendLine(".active, .accordion:hover { background-color: #ccc; }");
            htmlContent.AppendLine(".panel { padding: 0 18px; display: none; background-color: white; overflow: hidden; }");
            htmlContent.AppendLine(".arrow { float: right; }");
            htmlContent.AppendLine("</style>");
            htmlContent.AppendLine($"<h2 style=\"text-align:center; margin-top:20px;\">TAB FusionRMS Import Favorite Status Report</h2><hr style=\"width: 37%;\">");
            htmlContent.AppendLine("<body>");

            StringBuilder importedRecords = new StringBuilder();
            StringBuilder notExistedRecords = new StringBuilder();
            StringBuilder duplicateRecords = new StringBuilder();

            int importedCount = 0;
            int notExistedCount = 0;
            int duplicateCount = 0;

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                object idValue = row["ImportFavIds"];
                string targetFieldValue = row[pkey].ToString();
                object tableIdValue = row["TableId"];

                // Imported Records
                if (idValue != DBNull.Value && tableIdValue == DBNull.Value && !string.IsNullOrWhiteSpace(targetFieldValue))
                {
                    importedRecords.Append($"{targetFieldValue}:(TargetedField Id: {idValue}), ");
                    importedCount++;
                }

                // Records Not Existed
                if (idValue != DBNull.Value && tableIdValue == DBNull.Value && string.IsNullOrWhiteSpace(targetFieldValue))
                {
                    notExistedRecords.Append($"(TargetedField Id: {idValue}), ");
                    notExistedCount++;
                }

                // Duplicate Records
                if (idValue != DBNull.Value && tableIdValue != DBNull.Value && !string.IsNullOrWhiteSpace(targetFieldValue))
                {
                    duplicateRecords.Append($"{targetFieldValue}:(TargetedField Id: {idValue}), ");
                    duplicateCount++;
                }
            }

            if (importedRecords.Length > 0)
            {
                htmlContent.AppendLine($"<button class='accordion imported'>Imported Records (Count: {importedCount})<span class='arrow'>&#9660;</span></button>");
                htmlContent.AppendLine("<div class='panel'>");
                htmlContent.AppendLine($"<p>{importedRecords.ToString().TrimEnd(", ".ToCharArray())}</p>");
                htmlContent.AppendLine("</div>");
            }

            if (notExistedRecords.Length > 0)
            {
                htmlContent.AppendLine($"<button class='accordion not-existed' style='color:red'>Records Not Existed (Count: {notExistedCount})<span class='arrow'>&#9660;</span></button>");
                htmlContent.AppendLine("<div class='panel'>");
                htmlContent.AppendLine($"<p style='color:red'>{notExistedRecords.ToString().TrimEnd(", ".ToCharArray())}</p>");
                htmlContent.AppendLine("</div>");
            }

            if (duplicateRecords.Length > 0)
            {
                htmlContent.AppendLine($"<button class='accordion duplicate'>Duplicate Records (Count: {duplicateCount})<span class='arrow'>&#9660;</span></button>");
                htmlContent.AppendLine("<div class='panel'>");
                htmlContent.AppendLine($"<p>{duplicateRecords.ToString().TrimEnd(", ".ToCharArray())}</p>");
                htmlContent.AppendLine("</div>");
            }

            htmlContent.AppendLine("</div>");

            htmlContent.AppendLine("<script>");
            htmlContent.AppendLine("var acc = document.getElementsByClassName('accordion');");
            htmlContent.AppendLine("var i;");
            htmlContent.AppendLine("for (i = 0; i < acc.length; i++) {");
            htmlContent.AppendLine("    acc[i].addEventListener('click', function() {");
            htmlContent.AppendLine("        this.classList.toggle('active');");
            htmlContent.AppendLine("        var panel = this.nextElementSibling;");
            htmlContent.AppendLine("        if (panel.style.display === 'block') {");
            htmlContent.AppendLine("            panel.style.display = 'none';");
            htmlContent.AppendLine("            this.querySelector('.arrow').innerHTML = '&#9660;';");
            htmlContent.AppendLine("        } else {");
            htmlContent.AppendLine("            panel.style.display = 'block';");
            htmlContent.AppendLine("            this.querySelector('.arrow').innerHTML = '&#9650;';");
            htmlContent.AppendLine("        }");
            htmlContent.AppendLine("    });");
            htmlContent.AppendLine("}");
            htmlContent.AppendLine("</script>");

            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");

            return htmlContent.ToString();
        }


        #endregion

        #endregion
    }
}
