using System;
using System.Data;
using System.Text;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services.Interface;
using Smead.Security;

namespace MSRecordsEngine.Services
{
    public class ExporterService : IExporterService
    {
        private readonly CommonControllersService<ExporterService> _controllerService;    
        public ExporterService(CommonControllersService<ExporterService> controllersService)
        {
            _controllerService= controllersService;
        }

        public StringBuilder BuildString(Passport passport, int ViewId, string allquery, int currentLevel,string CultureShortPattern,string OffSetVal)
        {
            var _query = new Query(passport);
            var p = new Parameters(ViewId,passport);
            p.Paged = false;
            p.RequestedRows = -1;
            string strQuery = Navigation.NormalizeString(allquery);
            if (strQuery.ToUpper().Contains(" WHERE "))
            {
                string indx = strQuery.ToUpper().IndexOf(" WHERE ").ToString();
                strQuery = strQuery.Substring((int)Math.Round(Convert.ToDouble(indx) + 7d));
                p.WhereClause = strQuery;
            }
            _query.FillData(p);
            
            var sb = new StringBuilder();

            BuildHeaderAll(sb, p, currentLevel);
            sb.Append(Environment.NewLine);
            BuildRowsAll(sb, p, currentLevel, CultureShortPattern, OffSetVal);

            return sb;
        }

        #region Private Method
        private void BuildHeaderAll(StringBuilder sb, Parameters p, int CurrentLevel)
        {
            int counter = 0;
            foreach (DataColumn head in p.Data.Columns)
            {
                counter = counter + 1;
                if (_controllerService.ShowColumn(head, CurrentLevel, p.ParentField))
                {
                    if (counter == p.Data.Columns.Count)
                    {
                        sb.Append(head.ExtendedProperties["heading"].ToString());
                    }
                    else
                    {
                        sb.Append(head.ExtendedProperties["heading"].ToString() + ",");
                    }
                }
            }
        }

        private void BuildRowsAll(StringBuilder sb, Parameters p, int CurrentLevel, string CultureShortPattern, string OffSetVal)
        {
            int counter = 0;
            foreach (DataRow row in p.Data.Rows)
            {
                foreach (DataColumn col in p.Data.Columns)
                {
                    counter = counter + 1;
                    if (_controllerService.ShowColumn(col, CurrentLevel, p.ParentField))
                    {
                        string fieldData = string.Empty;
                        string fieldName = col.ColumnName;
                        if (string.IsNullOrEmpty(row[fieldName].ToString()))
                        {
                            fieldData = "";
                        }
                        else
                        {
                            fieldData = Convert.ToString(row[fieldName]);
                        }

                        fieldData = CheckDataType(col, row, fieldName, CultureShortPattern, OffSetVal);

                        if (fieldData.Contains(","))
                        {
                            fieldData = $"\"{fieldData}\"";
                        }
                        FillDateInRow(counter, p.Data.Columns.Count, fieldData, sb);

                    }
                }
                sb.Append(Environment.NewLine);
                counter = 0;
            }

        }

        private string CheckDataType(object col, DataRow row, string fieldName, string CultureShortPattern, string OffSetVal)
        {
            var datatype = row[fieldName].GetType();
            if (datatype.Name == "DateTime")
            {
                return _controllerService.GetConvertCultureDate(row[fieldName].ToString(), CultureShortPattern, OffSetVal, bDetectTime: true);
            }
            if (datatype.Name == "Boolean")
            {
                if (Convert.ToBoolean(row[fieldName]))
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            return row[fieldName].ToString();
        }

        private void FillDateInRow(int counter, int colcount, string fieldData, StringBuilder sb)
        {
            if (counter == colcount)
            {
                sb.Append(fieldData);
            }
            else
            {
                sb.Append(fieldData + ",");
            }

            // If counter = viewcol.Rows.Count Then
            // sb.Append(fieldData)
            // Else
            // sb.Append(fieldData & ",")
            // End If
        }
        #endregion
    }
}
