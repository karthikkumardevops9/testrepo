using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using System.Linq;
using MSRecordsEngine.Repository;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;

namespace MSRecordsEngine.Services
{
    public class DataGridService : IDataGridService
    {
        public async Task SaveNewsURL(NewUrlprops model)
        {
            await Navigation.SetSettingAsync("News", "NewsURL", model.NewUrl, model.passport);
        }
        public async Task<ViewQueryWindow> DrawQuery(ViewQueryWindowProps prop)
        {
            var m = new ViewQueryWindow();
            if (prop.passport.CheckPermission(Common.SECURE_MYQUERY, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
            {
                m.hasMyQueryAceess = true;
            }

            var query = new Query(prop.passport);
            var param = new Parameters(prop.viewId, prop.passport);
            param.QueryType = queryTypeEnum.Schema;
            param.Culture = new CultureInfo("en-US");//Keys.GetCultureCookies(_httpContext);
            param.Scope = ScopeEnum.Table;
            param.ParentField = prop.ChildKeyField;
            param.Culture.DateTimeFormat.ShortDatePattern = "";//Keys.GetCultureCookies(_httpContext).DateTimeFormat.ShortDatePattern;
            //var dateFormat = Keys.GetUserPreferences.sPreferedDateFormat.ToString().Trim().ToUpper();
            query.FillData(param);

            foreach (System.Data.DataColumn dc in param.Data.Columns)
            {
                StringBuilder sb = new();
                if (ShowColumn(dc, prop.crumblevel, param.ParentField) == true)
                {
                    // don't show column if the lookuptyp is 1 and it is not a dropdown.
                    if ((Convert.ToInt32(dc.ExtendedProperties["lookuptype"]) == 1
                        && !Convert.ToBoolean(dc.ExtendedProperties["dropdownflag"]) == true)
                            || !Convert.ToBoolean(dc.ExtendedProperties["FilterField"]) == true) { }
                    else
                    {
                        string buildRow = "<tr>" + BuildHeader(dc) + GetOperators(dc, dataType: dc.DataType.Name) + BuildTextBoxes(dc) + "</tr>";
                        m.ListOfRows.Add(buildRow);
                        m.listMyqueryDatatype.Add(dc.DataType.FullName);
                    }
                }
            }

            if (prop.ceriteriaId > 0)
            {
                await GetMyqueryList(prop, m);
            }
            return m;
        }
        private async Task GetMyqueryList(ViewQueryWindowProps prop, ViewQueryWindow m)
        {
            int id = 0;
            var getlist = new List<s_SavedChildrenQuery>();
            using (var context = new TABFusionRMSContext(prop.passport.ConnectionString))
            {
                id = context.s_SavedCriteria.Where(a => a.UserId == prop.passport.UserId & a.Id == prop.ceriteriaId).FirstOrDefault().Id;
                await Task.Run(() =>
                {
                    getlist = context.s_SavedChildrenQuery.Where(a => a.SavedCriteriaId == id).ToList();
                });
            }
            int index = 0;
            foreach (var itm in getlist)
            {
                var myq = new queryList();
                myq.ColumnType = m.listMyqueryDatatype[index];
                myq.columnName = itm.ColumnName;
                myq.operators = itm.Operator;
                myq.values = itm.CriteriaValue;
                index += 1;
                m.MyqueryList.Add(myq);
            }
        }
        private static bool ShowColumn(DataColumn col, int crumblevel, string parentField)
        {
            switch (Convert.ToInt32(col.ExtendedProperties["columnvisible"]))
            {
                case 3:  // Not visible
                    {
                        return false;
                    }
                case 1:  // Visible on level 1 only
                    {
                        if (crumblevel != 0)
                            return false;
                        break;
                    }
                case 2:  // Visible on level 2 and below only
                    {
                        if (crumblevel < 1)
                            return false;
                        break;
                    }
                case 4:  // Smart column- not visible in a drill down when it's the parent.
                    {
                        if (crumblevel > 0 & (parentField.ToLower() ?? "") == (col.ColumnName.ToLower() ?? ""))
                        {
                            return false;
                        }

                        break;
                    }
            }

            if (col.ColumnName.ToLower() == "formattedid")
                return false;
            // If col.ColumnName.ToLower = "id" Then Return False
            if (col.ColumnName.ToLower() == "attachments")
                return false;
            if (col.ColumnName.ToLower() == "slrequestable")
                return false;
            if (col.ColumnName.ToLower() == "itemname")
                return false;
            if (col.ColumnName.ToLower() == "pkey")
                return false;
            if (col.ColumnName.ToLower() == "dispositionstatus")
                return false;
            if (col.ColumnName.ToLower() == "processeddescfieldnameone")
                return false;
            if (col.ColumnName.ToLower() == "processeddescfieldnametwo")
                return false;
            if (col.ColumnName.ToLower() == "rownum")
                return false;
            return true;
        }
        private string BuildHeader(DataColumn dc)
        {
            // aspNetDisabled form-control formWindowTextBox
            string ColumnName;
            string Header = dc.ExtendedProperties["heading"].ToString() + ":";
            string Title = dc.ExtendedProperties["heading"].ToString();
            string dataType = dc.DataType.FullName;
            bool isdropDown = System.Convert.ToBoolean(dc.ExtendedProperties["dropdownflag"]);
            if (System.Convert.ToBoolean(dc.ExtendedProperties["dropdownflag"]) == true & dc.ExtendedProperties["LookupData"] != null)
                ColumnName = Navigation.MakeSimpleField(dc.ExtendedProperties["LookupData"].ToString());
            else
                ColumnName = dc.ColumnName;

            return string.Format("<td dropdown=\"{4}\" DataType=\"{2}\" ColumnName=\"{3}\" title=\"{0}\" style=\"width:30%;text-align:left;\">{1}</td>", Title, Header, dataType, ColumnName, isdropDown);
        }
        private string GetOperators(DataColumn dc, string dataType = null)
        {
            StringBuilder ListOfOperators = new StringBuilder();
            if (Common.BOOLEAN_TYPE == dataType.ToLower() || Convert.ToInt32(dc.ExtendedProperties["lookuptype"]) == 1)
            {
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", " ", " "));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "=", "Equals to"));
            }
            else
            {
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", " ", " "));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "=", "Equals to"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "<>", "Not equals to"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", ">", "Greater than"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", ">=", "Greater than equals to"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "<", "Less than"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "<=", "Less than equals to"));
                if (Common.dataType.Contains(dataType.ToLower()))
                    ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "Between", "Between"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "BEG", "Beginning with"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "Ends with", "Ends with"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "Contains", "Contains"));
                ListOfOperators.Append(string.Format("<option value=\"{0}\">{1}</option>", "Not contains", "Not contains"));
            }
            string returnOperators = string.Format("<td style=\"width:30%;text-align:center;\"><select class=\"form-control\" onchange=\"obJquerywindow.OperatorCondition(this)\" style=\"color:Black;border-color:Silver;border-width:1px;border-style:Solid;font-size:9pt;font-weight:bold;\">{0}</select></td>", ListOfOperators.ToString());
            return returnOperators;
        }
        private string BuildTextBoxes(object dc1)
        {
            var dc = (DataColumn)dc1;
            string buildInput = "";
            string placeHoldersValue = string.Empty;
            string HeaderId = dc.ExtendedProperties["heading"].ToString().Trim();
            var filedName = dc.ExtendedProperties["FieldName"];
            switch (dc.DataType.Name.ToString().ToLower())
            {
                case "string":
                    {
                        if (Convert.ToBoolean(dc.ExtendedProperties["dropdownflag"]))
                            buildInput = string.Format("<td onchange=\"obJquerywindow.WhenChangeValue(event)\" style=\"width:40%;\"><select type=\"text\" placeholder=\"{1}\" class=\"form-control\">{0}</select></td>", BuildDropDown(dc), placeHoldersValue);
                        else
                            buildInput = string.Format("<td onkeyup=\"obJquerywindow.WhenChangeValue(event)\" style=\"width:40%;\"><input type=\"text\" placeholder=\"{0}\" class=\"form-control\" ></td>", placeHoldersValue);
                        break;
                    }

                case "boolean":
                    {
                        buildInput = "<td onclick=\"obJquerywindow.WhenChangeValue(event)\" class=\"datacell\" style=\"border-width:0px;width:40%;text-align:left\"><input class=\"modal-checkbox\" type=\"checkbox\"></td>";
                        break;
                    }

                case "int16":
                case "int32":
                case "int64":
                case "decimal":
                    {
                        if (Convert.ToBoolean(dc.ExtendedProperties["dropdownflag"]))
                            buildInput = string.Format("<td onchange=\"obJquerywindow.WhenChangeValue(event)\" style=\"width:40%;\"><select type=\"text\" placeholder=\"{1}\" class=\"form-control\">{0}</select></td>", BuildDropDown(dc), placeHoldersValue);
                        else
                            buildInput = string.Format("<td onkeyup=\"obJquerywindow.WhenChangeValue(event)\" obJquerywindow.WhenChangeValue(event)\" style=\"width:40%;\"><input id=\"singelNumber\" type=\"number\" placeholder=\"{0}\" class=\"form-control\" ></td>", placeHoldersValue);
                        break;
                    }

                case "double":
                    {
                        buildInput = string.Format("<td onkeyup=\"obJquerywindow.WhenChangeValue(event)\" style=\"width:40%;\"><input type=\"number\" placeholder=\"{0}\" class=\"form-control\" ></td>", placeHoldersValue);
                        break;
                    }

                case "datetime":
                    {
                        var dateFormat = "";//Keys.GetUserPreferences.sPreferedDateFormat.ToString().Trim().ToUpper();
                        buildInput = string.Format("<td onchange=\"obJquerywindow.WhenChangeValue(event)\" style=\"width:40%;\"><input id=\"{0}\" placeholder=\"{1}\" autocomplete=\"off\" name=\"tabdatepicker\" class=\"form-control\" ></td>", HeaderId, dateFormat);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return buildInput;
        }
        private StringBuilder BuildDropDown(DataColumn col1)
        {
            //DataTable col = (DataTable)col1;
            var count = ((DataTable)col1.ExtendedProperties["LookupData"]).Rows.Count;

            var listItem = new StringBuilder(count);
            listItem.Append("<option value=\"\"> </option>");
            foreach (DataRow row in ((DataTable)col1.ExtendedProperties["LookupData"]).Rows)
            {
                if (((DataTable)col1.ExtendedProperties["LookupData"]).Columns.Count > 1)
                {
                    listItem.Append(string.Format("<option value=\"{0}\">{1}</option>", row["Value"].ToString(), row["Display"].ToString()));
                }
                else
                {
                    listItem.Append(string.Format("<option value=\"{0}\">{1}</option>", row["Display"].ToString(), row["Display"].ToString()));
                }
            }
            return listItem;
        }
    }
}
