using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Smead.Security;


namespace MSRecordsEngine.RecordsManager
{

    public class Export
    {
        protected static DataTable GetServiceTasks(SqlConnection SQLconn, int Id)
        {
            try
            {
                string Sql = string.Format("SELECT ST.ViewId, ST.DownloadLocation, ST.RecordCount " + Constants.vbCrLf + "From SLServiceTasks ST " + Constants.vbCrLf + "WHERE ST.Id = {0} ", Id);
                var dtServiceTasks = new DataTable();
                using (var conn = SQLconn)
                {
                    using (var cmd = new SqlCommand(Sql, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtServiceTasks);
                        }
                    }
                }
                return dtServiceTasks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static void ExportFiles(int ServiceTaskId, Passport passport, bool IsCSV)
        {
            string filePath = string.Empty;
            int viewId = 0;
            var dtServiceTasks = GetServiceTasks(passport.Connection(), ServiceTaskId);

            if (dtServiceTasks is not null)
            {
                if (dtServiceTasks.Rows.Count > 0)
                {
                    filePath = dtServiceTasks.Rows[0]["DownloadLocation"].ToString();
                    viewId = Conversions.ToInteger(dtServiceTasks.Rows[0]["ViewId"]);
                }
            }

            if (viewId == 0 || string.IsNullOrWhiteSpace(filePath))
                return;

            int TotalRecord = Conversions.ToInteger(dtServiceTasks.Rows[0]["RecordCount"]);
            int PageSize = 10000;
            int TotalPage = (int)Math.Round(Math.Ceiling(TotalRecord / (decimal)PageSize));

            var newParams = new Parameters(viewId, passport);
            newParams.WhereClause = string.Concat(newParams.KeyField, string.Format(" IN (SELECT [TableId] FROM [SLServiceTaskItems] WHERE [SLServiceTaskId] = {0}) ", ServiceTaskId));
            newParams.RequestedRows = -1;
            var dtExport = new DataTable();
            var httpcontext = new HttpContextAccessor().HttpContext;
            var query = new Query(passport);

            string pkeyName = "pkey";
            var dtExportData = new DataTable();
            var sqlWrapper = new StringBuilder();
            string sqlQuery = Navigation.NormalizeString(query.GetSQLForExport(newParams));
            sqlQuery = Strings.Split(sqlQuery, " ORDER BY ", Compare: CompareMethod.Text)[0].ToString();

            for (int i = 1, loopTo = TotalPage; i <= loopTo; i++)
            {
                sqlWrapper.Remove(0, sqlWrapper.Length);
                sqlWrapper.AppendLine("DECLARE @PageSize INT, @PageIndex INT ");
                sqlWrapper.AppendFormat("SELECT @PageSize = {0}, @PageIndex = {1} {2}", PageSize, i.ToString(), Environment.NewLine);
                sqlWrapper.AppendFormat("SELECT TOP {0} * INTO #testTemp FROM (SELECT ROW_NUMBER() OVER (ORDER BY [{1}]) AS RowNum, overall_count = COUNT(*) OVER(), * FROM {2}", PageSize.ToString(), pkeyName, Environment.NewLine);
                sqlWrapper.AppendFormat("    ({0}) {1}", sqlQuery, Environment.NewLine);
                sqlWrapper.AppendLine(" AS tmp) AS PagedResult WHERE RowNum > ((@PageIndex - 1) * @PageSize) And RowNum < (@PageIndex * @PageSize + 1) ORDER BY PagedResult.RowNum ");
                sqlWrapper.AppendLine("SELECT * FROM #testTemp ");
                sqlWrapper.AppendLine("DROP TABLE #testTemp");
                dtExportData.Clear();

                using (var conn = passport.Connection())
                {
                    using (var cmd = new SqlCommand(sqlWrapper.ToString(), conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtExportData);
                        }
                    }
                }

                dtExport.Merge(dtExportData);
            }

            var dtView = new DataTable();

            using (var cmd = new SqlCommand("SELECT * FROM ViewColumns WHERE ViewsID = @ViewID AND NOT Heading LIKE '%Id For%' ORDER BY ColumnNum", passport.Connection()))
            {
                cmd.Parameters.AddWithValue("@ViewID", viewId.ToString());
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtView);
                }
            }
            // Check which value should be as a header(This can be configured from 'Option' in Desktop application)
            // HeaderType Cases:
            // Case 0: No Header
            // Case 1: Internal Field Name
            // Case 2: Header
            int HeaderType = 2;
            var headerKeyValue = new List<KeyValuePair<string, string>>();

            switch (HeaderType)
            {
                case 0:
                    {
                        break;
                    }
                // do nothing, no headers
                case 1:
                    {
                        foreach (DataRow row in dtView.Rows)
                        {
                            if (Operators.ConditionalCompareObjectNotEqual(row["ColumnOrder"], 3, false))
                            {
                                if (Operators.ConditionalCompareObjectEqual(row["LookupType"], 0, false))
                                {
                                    headerKeyValue.Add(new KeyValuePair<string, string>(Navigation.MakeSimpleField(Conversions.ToString(row["FieldName"])), Conversions.ToString(row["FieldName"])));
                                }
                                else
                                {
                                    headerKeyValue.Add(new KeyValuePair<string, string>(Conversions.ToString(row["FieldName"]), Conversions.ToString(row["FieldName"])));
                                }
                            }
                        }

                        break;
                    }
                case 2:
                    {
                        foreach (DataRow row in dtView.Rows)
                        {
                            if (Operators.ConditionalCompareObjectNotEqual(row["ColumnOrder"], 3, false))
                            {
                                if (Operators.ConditionalCompareObjectEqual(row["LookupType"], 0, false))
                                {
                                    headerKeyValue.Add(new KeyValuePair<string, string>(Navigation.MakeSimpleField(Conversions.ToString(row["FieldName"])), Conversions.ToString(row["Heading"])));
                                }
                                else
                                {
                                    headerKeyValue.Add(new KeyValuePair<string, string>(Conversions.ToString(row["FieldName"]), Conversions.ToString(row["Heading"])));
                                }
                            }
                        }

                        break;
                    }
            }

            var sb = new StringBuilder();
            var columnHeaders = new List<string>();

            foreach (KeyValuePair<string, string> item in headerKeyValue)
            {
                if (dtExport.Columns.Contains(item.Key.Trim()))
                {
                    if (HeaderType == 1 || HeaderType == 2)
                    {
                        if (IsCSV)
                        {
                            sb.Append(item.Value + ",");
                        }
                        else
                        {
                            sb.Append("\"" + item.Value + "\"" + ",");
                        }
                    }
                    columnHeaders.Add(item.Key);
                }
            }

            if (sb.Length > 0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
                sb.Append(Environment.NewLine);
            }

            if (dtExport.Rows.Count != 0)
            {
                string itemValue1;
                foreach (DataRow row in dtExport.Rows)
                {
                    foreach (string item in columnHeaders)
                    {
                        var getDateType = row[item].GetType();

                        if (getDateType.Name == "DateTime")
                        {
                            itemValue1 = row[item].ToString();
                        }
                        // oItemVal = Keys.ConvertCultureDate(oDataRow(oItem).ToString, bDetectTime:=True)
                        else if (getDateType.Name == "Boolean")
                        {
                            if (Operators.ConditionalCompareObjectEqual(row[item], true, false))
                            {
                                itemValue1 = "1";
                            }
                            else
                            {
                                itemValue1 = "0";
                            }
                        }
                        else
                        {
                            itemValue1 = row[item].ToString();
                        }

                        if (IsCSV)
                        {
                            sb.Append(PrepForCSV(itemValue1) + ",");
                        }
                        else
                        {
                            sb.Append("\"" + itemValue1 + "\"" + ",");
                        }
                    }

                    sb = sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);
                }
            }
            // Changed by Nikunj: FUS-6090 Before created any file it is not able to open so coming Could not find file error.
            using (var outfile = new StreamWriter(filePath))
            {
                outfile.Write(sb.ToString());
                outfile.Flush();
            }
        }

        private static string PrepForCSV(string value)
        {
            if (value.Equals("True"))
            {
                return "\"-1\"";
            }
            if (value.Equals("False"))
            {
                return "\"0\"";
            }
            string temp = string.Format("\"{0}\"", value.Replace("\"", "\"\""));
            return temp;
        }
    }
}