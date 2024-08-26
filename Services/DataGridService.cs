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
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.VisualBasic;
using static MSRecordsEngine.Models.FusionModels.MyFavorite;
using System.Collections;
using SecureObject = Smead.Security.SecureObject;
using static MSRecordsEngine.Models.Enums;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static MSRecordsEngine.Models.FusionModels.LinkScriptModel;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Linq;
using Microsoft.VisualBasic.CompilerServices;

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
            var param = new Parameters(prop.ViewId, prop.passport);
            param.QueryType = queryTypeEnum.Schema;
            //param.Culture = new CultureInfo("en-US");//Keys.GetCultureCookies(_httpContext);
            param.Scope = ScopeEnum.Table;
            param.ParentField = prop.ChildKeyField;
            //param.Culture.DateTimeFormat.ShortDatePattern = "";//Keys.GetCultureCookies(_httpContext).DateTimeFormat.ShortDatePattern;
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
        public async Task<GridDataBinding> BuildNewData(SearchQueryRequestModal props)
        {
            var model = new GridDataBinding();
            model.ItemDescription = Navigation.GetItemName(props.paramss.preTableName, props.paramss.Childid, props.passport);
            if (props.searchQuery != null)
                model.fvList = CreateQuery(props);

            await BuildNewTableData(props, model);

            return model;
        }
        public async Task<string> GetTotalRowsForGrid(SearchQueryRequestModal props)
        {
            int TotalPages = 0;
            int TotalRows = 0;
            int RequestedRows = 0;

            Parameters @params = new Parameters(props.paramsUI.ViewId, props.passport);
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                RequestedRows = (int)context.Views.Where(x => x.Id == props.paramsUI.ViewId).FirstOrDefault().MaxRecsPerFetch;
            }
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                TotalRows = await Query.TotalQueryRowCountAsync(props.HoldTotalRowQuery, conn);
            }


            if (TotalRows / (double)@params.RequestedRows > 0 & TotalRows / (double)@params.RequestedRows < 1)
                TotalPages = 1;
            else if (TotalRows % @params.RequestedRows == 0)
                TotalPages = (int)(TotalRows / (double)@params.RequestedRows);
            else
            {
                int tp = (int)(TotalRows / (double)@params.RequestedRows);
                TotalPages = tp + 1;
            }

            return TotalRows + "|" + TotalPages + "|" + @params.RequestedRows;
        }
        public async Task<ScriptReturn> LinkscriptButtonClick([FromBody] linkscriptPropertiesUI props)
        {
            var _param = new Parameters(props.ViewId, props.passport);
            var scriptflow = await ScriptEngine.RunScriptWorkFlowAsync(props.WorkFlow, _param.TableName, props.TableId, props.ViewId, props.passport, props.Rowids);

            return scriptflow;
        }
        public async Task<ScriptReturn> LinkscriptEvents(linkscriptPropertiesUI props)
        {
            var model = new ScriptReturn();
            await Task.Run(() =>
            {
                model = ScriptEngine.RunScript(props.InternalEngine.ScriptName, props.InternalEngine.CurrentTableName, props.InternalEngine.RecordId, props.InternalEngine.ViewId, props.passport, props.passport.Connection(), props.InternalEngine.Caller, props.InternalEngine.GetSelectedRowIds);
            });

            return model;
        }
        public LinkScriptModel BuiltControls(ScriptReturn scriptresult)
        {
            LinkScriptModel model = new LinkScriptModel();
            SetHeadingAndTitle(scriptresult, model);
            model.UnloadPromptWindow = scriptresult.ScriptControlDictionary.Count == 0;

            foreach (var item in scriptresult.ScriptControlDictionary)
            {
                switch (item.Value.ControlType)
                {
                    case ScriptControls.ControlTypes.ctTextBox:
                        CreateController text = new CreateController();
                        if (!string.IsNullOrEmpty(item.Value.GetProperty(ScriptControls.ControlProperties.cpText).ToString()))
                            text.Text = item.Value.GetProperty(ScriptControls.ControlProperties.cpText).ToString();

                        text.Id = item.Key;
                        text.Css = "form-control";
                        text.ControlerType = "textbox";
                        model.ControllerList.Add(text);
                        break;

                    case ScriptControls.ControlTypes.ctLabel:
                        CreateController label = new CreateController();
                        label.Text = item.Value.GetProperty(ScriptControls.ControlProperties.cpCaption).ToString();
                        label.Id = item.Key;
                        label.Css = "control-label";
                        label.ControlerType = "label";
                        model.ControllerList.Add(label);
                        break;
                    case ScriptControls.ControlTypes.ctComboBox:
                        CreateController dropdown = new CreateController();
                        int j = 0;
                        foreach (var _item in item.Value.ItemList)
                        {
                            dropdownprop prop = new dropdownprop();
                            prop.text = _item;
                            prop.value = item.Value.ItemDataList[j];
                            // dropdown.Text = _item
                            // If j < item.Value.ItemDataList.Count Then _item = item.Value.ItemDataList(j)
                            j = j + 1;
                            // dropdown.Items.Add(listitem)
                            dropdown.dropdownItems.Add(prop);
                        }
                        dropdown.Id = item.Key;
                        dropdown.Css = "form-control";
                        dropdown.ControlerType = "dropdown";
                        dropdown.dropIndex = Convert.ToInt32(item.Value.GetProperty(ScriptControls.ControlProperties.cpListindex));
                        model.ControllerList.Add(dropdown);
                        break;

                    case ScriptControls.ControlTypes.ctOption:
                        CreateController radiobutton = new CreateController();
                        radiobutton.Text = item.Value.GetProperty(ScriptControls.ControlProperties.cpCaption).ToString();
                        radiobutton.Groupname = "LinkScriptRadioButtons";
                        radiobutton.Id = item.Key;
                        radiobutton.ControlerType = "radiobutton";
                        model.ControllerList.Add(radiobutton);
                        break;
                    case ScriptControls.ControlTypes.ctCheck:
                        CreateController checkbox = new CreateController();
                        checkbox.Text = item.Value.GetProperty(ScriptControls.ControlProperties.cpCaption).ToString();
                        checkbox.Id = item.Key;
                        checkbox.ControlerType = "checkbox";
                        model.ControllerList.Add(checkbox);
                        break;

                    case ScriptControls.ControlTypes.ctListBox:
                        CreateController listBox = new CreateController();
                        //var j = 0;
                        foreach (var _item in item.Value.ItemList)
                        {
                            listBox prop = new listBox();
                            prop.text = _item;
                            prop.value = item.Value.ItemDataList[0];
                            listBox.listboxItems.Add(prop);
                        }
                        listBox.rowCounter = 4.ToString();
                        listBox.Id = item.Key;
                        listBox.Css = "form-control";
                        listBox.ControlerType = "listBox";
                        listBox.dropIndex = Convert.ToInt32(item.Value.GetProperty(ScriptControls.ControlProperties.cpListindex));
                        model.ControllerList.Add(listBox);
                        break;
                    case ScriptControls.ControlTypes.ctButton:
                        Button button = new Button();
                        if (item.Value.GetProperty(ScriptControls.ControlProperties.cpCaption) != string.Empty)
                            button.Text = Convert.ToString(item.Value.GetProperty(ScriptControls.ControlProperties.cpCaption));
                        else
                            button.Text = item.Key;

                        if (button.Text.Contains("&"))
                        {
                            button.Text = button.Text.Replace("&&", "!!!!!!ampersandescape!!!!!!!");
                            button.Text = button.Text.Replace("&", "");
                            button.Text = button.Text.Replace("!!!!!!ampersandescape!!!!!!!", "&");
                        }

                        button.Css = "btn btn-success text-uppercase";
                        button.Id = item.Key;
                        // AddHandler button.Click, AddressOf FlowButton_Click
                        model.ButtonsList.Add(button);
                        break;
                    case ScriptControls.ControlTypes.ctMemoBox:
                        CreateController tx = new CreateController();
                        tx.Text = item.Value.GetProperty(ScriptControls.ControlProperties.cpText).ToString();
                        tx.Id = item.Key;
                        tx.Css = "form-control";
                        tx.ControlerType = "textarea";
                        model.ControllerList.Add(tx);
                        break;
                }
            }
            return model;
        }
        public async Task<bool> FlowButtonsClickEvent(linkscriptPropertiesUI props)
        {
            var engine = props.InternalEngine;
            string[] selectedrow = props.InternalEngine.GetSelectedRowIds;
            bool EngineReturn = false;
            await Task.Run(() =>
            {
                EngineReturn = ScriptEngine.RunScript(ref engine, engine.ScriptName, engine.CurrentTableName, engine.RecordId, engine.ViewId, props.passport, engine.Caller, ref selectedrow);
            });
            return EngineReturn;
        }
        public async Task<TabquikApi> TabQuikInitiator(TabquickpropUI props)
        {
            var model = new TabquikApi();
            
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                await GetLicense(model, props, conn);
                GetTabquikData(model, props, conn);
            }

            return model;

        }
        private async Task GetLicense(TabquikApi model, TabquickpropUI props, SqlConnection conn)
        {
            // get the license
            var key = await Navigation.GetSettingAsync("TABQUIK", "Key", conn);
            var keys = key.Split('-');

            if (key is not null)
            {
                if (key.Length > 1)
                {
                    model.CustomerID = keys[0];
                    model.ContactID = keys[1];
                }
                else
                {
                    model.CustomerID = keys[0];
                }
            }
        }
        private void GetTabquikData(TabquikApi model, TabquickpropUI props, SqlConnection conn)
        {
            string inClause = System.String.Format(" IN ({0})", props.RowsSelected);
            var param = new Parameters(props.ViewId, props.passport);
            DataTable dtData = new DataTable("LabelData");
            DataTable dtJobs = new DataTable();
            DataTable dtFormat = new DataTable("Formats");
            DataTable dtClone;

            using (var cmd = new SqlCommand("SELECT * FROM OneStripJobs WHERE TableName = @tableName AND InPrint = 5", conn))
            {
                cmd.Parameters.AddWithValue("@TableName", param.TableName);
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtJobs);
                }
            }

            if (dtJobs.Rows.Count == 0)
            {
                model.ErrorMsg = "No labels have been integrated for this table. Unable to continue";
                return;
            }
            var rep = dtJobs.Rows[0]["SQLString"].ToString().Replace("= %ID%", inClause);
            using (var cmd = new SqlCommand(rep, conn))
            {
                cmd.CommandText = cmd.CommandText.Replace("=%ID%", inClause);
                cmd.CommandText = cmd.CommandText.Replace("='%ID%'", inClause);
                cmd.CommandText = cmd.CommandText.Replace("= '%ID%'", inClause);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtData);
                }
            }

            if (dtData.Rows.Count == 0)
            {
                model.ErrorMsg = "Table label SQL statement returned no data. Unable to continue";
                return;
            }

            using (var cmd = new SqlCommand("SELECT * FROM OneStripJobFields WHERE OneStripJobsID = @JobID", conn))
            {
                cmd.Parameters.AddWithValue("@JobID", dtJobs.AsEnumerable().ElementAtOrDefault(0)["Id"]);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtFormat);
                }
            }

            if (dtFormat.Rows.Count == 0)
            {
                model.ErrorMsg = "No label format fields returned. Unable to continue";
                return;
            }

            dtClone = dtData.Clone();

            foreach (DataColumn col in dtClone.Columns)
                col.DataType = typeof(string);

            foreach (DataRow row in dtData.Rows)
                dtClone.ImportRow(row);


            var datalist = new StringBuilder();

            foreach (DataRow rowData in dtClone.Rows)
            {
                var rowValues = new List<string>();
                foreach (DataColumn col in dtClone.Columns)
                {
                    rowValues.Add(rowData[col.ColumnName].ToString());
                }
                datalist.Append(string.Join("~", rowValues));

                if (rowData != dtClone.Rows[dtClone.Rows.Count - 1])
                {
                    datalist.Append("*!*");
                }
            }

            model.DataTQ = datalist.ToString();
            model.DataTQ = model.DataTQ.Replace(@"\", @"\\");
            model.DataTQ = model.DataTQ.Replace("'", @"\'");

            LabelPrintUpdate(dtJobs, inClause, conn);
        }
        private void LabelPrintUpdate(DataTable dtJobs, string inClause, SqlConnection conn)
        {
            if (dtJobs.Rows.Count == 0
                || string.IsNullOrEmpty(dtJobs.Rows[0]["SQLUpdateString"].ToString())
                || dtJobs.Rows[0]["SQLUpdateString"].ToString().IndexOf("<YourTable>") != -1) return;

            var rep = dtJobs.Rows[0]["SQLUpdateString"].ToString().Replace("= %ID%", inClause);

            using (var cmd = new SqlCommand(rep, conn))
            {
                cmd.CommandText = cmd.CommandText.Replace("=%ID%", inClause);
                cmd.CommandText = cmd.CommandText.Replace("='%ID%'", inClause);
                cmd.CommandText = cmd.CommandText.Replace("= '%ID%'", inClause);
                cmd.ExecuteScalar();
            }
        }
        private void SetHeadingAndTitle(ScriptReturn scriptresult, LinkScriptModel model)
        {
            model.lblHeading = scriptresult.Engine.Heading;
            model.Title = scriptresult.Engine.Title;
        }
        private async Task BuildNewTableData(SearchQueryRequestModal props, GridDataBinding model)
        {
            var _query = new Query(props.passport);
            var pr = new Parameters(props.paramss.ViewId, props.passport);

            pr.ParentField = props.paramss.ChildKeyField;
            model.IdFieldDataType = pr.IdFieldDataType.FullName;

            model.ViewName = pr.ViewName;
            model.TableName = pr.TableName;
            model.ViewId = pr.ViewId;

            if (model.fvList.Count > 0)
            {
                fieldValueParams(pr, model);
            }
            if (model.IsWhereClauseRequest)
            {
                WhereClauseParams(pr, model);
            }
            if (model.GsIsGlobalSearch)
            {
                GlobalSearchParams(pr, model, props);
            }
            pr.Paged = true;
            pr.PageIndex = props.paramss.pageNum;
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                var views = context.Views.Where(a => a.Id == props.paramss.ViewId).FirstOrDefault();
                pr.RequestedRows = (int)views.MaxRecsPerFetch;
                model.RowPerPage = (int)views.MaxRecsPerFetch;
            }
            pr.IsMVCCall = true;
            await _query.FillDataAsync(pr);
            // get the string totalrow query
            model.TotalRowsQuery = pr.TotalRowsQuery;
            if (BuildDrillDownLinks(pr, props, model) > 0)
            {
                model.HasDrillDowncolumn = true;
            }
            else
            {
                model.HasDrillDowncolumn = false;
            }
            BuildNewTableHeaderData(model, props, pr);
            // build toolbar buttons
            buildToolBarButtons(pr.Data.Rows.Count, model, pr, props);
            // check if table is trackable
            IsTableTrackable(model, props, pr);
            // get sortable fields
            SetSortablefields(pr, props, model);
            // build breadcrumbs right click
            BuildBreadCrumbRightClick(model, props);
            Buildrows(pr, model, props);
        }
        private void BuildBreadCrumbRightClick(GridDataBinding model, SearchQueryRequestModal props)
        {
            // get right click views
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT Id, ViewName FROM Views WHERE TableName = @tableName AND (Printable IS NULL OR Printable = 0) order by ViewOrder", conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", model.TableName);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            if (props.passport.CheckPermission(row["ViewName"].ToString(), SecureObject.SecureObjectType.View, Permissions.Permission.View))
                            {
                                model.ListOfBreadCrumbsRightClick.Add(new BreadCrumbsRightClick() { viewId = Convert.ToInt32(row["Id"]), viewName = Convert.ToString(row["ViewName"]) });
                            }

                            // Dim x = Convert.ToString(dt.Rows(0).ItemArray(0))
                        }
                        Convert.ToString(dt.Rows[0].ItemArray[0]);
                    }
                }
            }
        }
        private void IsTableTrackable(GridDataBinding model, SearchQueryRequestModal props, Parameters pr)
        {
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                model.ShowTrackableTable = (bool)context.Tables.Where(a => (a.TableName ?? "") == (pr.TableName ?? "")).FirstOrDefault().Trackable;
            }
        }
        private void SetSortablefields(Parameters pr, SearchQueryRequestModal props, GridDataBinding model)
        {
            foreach (RecordsManage.ViewColumnsRow row in Navigation.GetsortableFields(pr.ViewId, props.passport))
            {
                string fieldname = string.Empty;
                if (row.FieldName.Contains("."))
                {
                    fieldname = Navigation.MakeSimpleField(row.FieldName);
                }
                else
                {
                    fieldname = row.FieldName;
                }
                model.sortableFields.Add(new SortableFileds() { FieldName = fieldname, SortOrder = row.SortOrder, SortOrderDesc = row.SortOrderDesc });
            }
        }
        private void buildToolBarButtons(int rowCount, GridDataBinding model, Parameters pr, SearchQueryRequestModal props)
        {
            var sb = new StringBuilder();
            ToolBarQueryButton(sb);
            ToolBarNewRecordButton(sb, props, pr, model);
            if (rowCount > 0)
            {
                ToolBarFileButton(sb, props, pr, model);
                ToolBarArrowButton(sb, props, pr, model);
                ToolBarFavoriteButton(sb, props, model);
                sb.Append(string.Format("<input type=\"button\" name=\"saveRow\" value=\"{0}\" id=\"saveRow\" class=\"btn btn-secondary tab_btn\" style=\"min-width: 70px; margin-left:4px\" />", "Save Edit"));
                sb.Append(string.Format("<span style=\"margin-left: 11px\"> # of Rows Selected: <span id=\"rowcounter\"> 0</span></span>"));
                // sb.Append(Environment.NewLine + String.Format("<input type=""text"" style=""height: 34;width: 155px;""placeholder=""Search in page"" id=""searchInpage"">"))
            }
            sb.Append(string.Format("<span id=\"emptymsg\" style=\"display:none;\" class=\"emptymsg-txt\"><i>Click on <strong>New</strong> Button to Add Record(s) into the Table View</i></span>"));

            // sb.Append(Environment.NewLine + String.Format("<input type=""button"" style=""min-width: 80px;top:0"" id=""autosavebtn"" type=""button"" class=""btn btn-secondary tab_btn"" value=""{0}"" />", Languages.Translation("Autosave")))
            sb.Append(Environment.NewLine + string.Format(" <label class=\"switch pull-right\"><input type=\"checkbox\" id=\"autosavebtn\"><div class=\"slider round\"><span class=\"on\">ON</span><span class=\"off\">OFF</span></div></label><span class=\"pull-right\" style=\"position: relative;left: -9px; top: 4px;\">{0}</span>", "AutoSave"));
            model.ToolBarHtml = sb.ToString();
        }
        private void ToolBarQueryButton(StringBuilder sb)
        {
            sb.Append(string.Format("<button class=\"btn btn-secondary tab_btn\" onclick=\"obJgridfunc.RefreshGrid(this)\"><img src=\"/Content/themes/TAB/css/images/refresh30px.png\" width=\"20px;\"></button>"));
            sb.Append(string.Format("<input type=\"button\" name=\"btnQuery\" value=\"{0}\" id=\"btnQuery\" class=\"btn btn-secondary tab_btn\" style=\"min-width: 70px; margin-left:3px\" />", "Query"));
        }
        private void ToolBarNewRecordButton(StringBuilder sb, SearchQueryRequestModal props, Parameters pr, GridDataBinding model)
        {
            if (props.passport.CheckPermission(pr.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.Add) && !(model.fViewType == (int)ViewType.Favorite))
            {
                sb.Append(Environment.NewLine + string.Format("<input type=\"button\" onclick=\"obJaddnewrecord.LoadNewRowDialog()\" name=\"btnNew\" value=\"{0}\" id=\"btnNew\" class=\"btn btn-secondary tab_btn\" />", "New"));
            }
        }
        private void ToolBarFileButton(StringBuilder sb, SearchQueryRequestModal props, Parameters pr, GridDataBinding model)
        {
            // CREATE Tool button dropdown file
            sb.Append(Environment.NewLine + "<div class=\"btn-group\">");
            sb.Append(Environment.NewLine + "<button class=\"btn btn-secondary dropdown-toggle tab_btn\" data-toggle=\"dropdown\" type=\"button\" aria-expanded=\"False\">");
            sb.Append(Environment.NewLine + "<i class=\"fa fa-file-text-o fa-fw\"></i>");
            sb.Append(Environment.NewLine + "<i class=\"fa fa-angle-down\"></i>");
            sb.Append(Environment.NewLine + "</button>");
            sb.Append(Environment.NewLine + "<ul class=\"dropdown-menu btn_menu\">");
            // add print button button
            if (props.passport.CheckPermission(pr.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.Print))
            {
                model.RightClickToolBar.Menu1Print = true;
                sb.Append(Environment.NewLine + string.Format("<li><a id=\"btnPrint\">{0}</a></li>", "Print"));
            }
            // check if customer has license for tabquik
            bool hasTabquikLicense = !string.IsNullOrEmpty(Navigation.GetSetting("TABQUIK", "Key", props.passport));
            if (hasTabquikLicense)
            {
                var labelExists = Navigation.LabelExists(pr.TableName, props.passport);
                bool setbutton = labelExists == Navigation.Enums.eLabelExists.Color | labelExists == Navigation.Enums.eLabelExists.BWAndColor;
                if (setbutton)
                {
                    model.RightClickToolBar.Menu1Tabquick = true;
                    sb.Append(Environment.NewLine + string.Format("<li><a onclick=\"CheckForLicense('FTabQuick')\">{0}</a></li>", "Tabquik"));
                }

            }
            // add print label
            var islabelExist = Navigation.LabelExists(pr.TableName, props.passport);

            if (props.passport.CheckPermission(pr.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.PrintLabel) && (islabelExist & Navigation.Enums.eLabelExists.BlackAndWhite) == Navigation.Enums.eLabelExists.BlackAndWhite)
            {
                model.RightClickToolBar.Menu1btnBlackWhite = true;
                sb.Append(Environment.NewLine + string.Format("<div id=\"ulPrintButtons\" class=\"div_listed\">"));
                sb.Append(Environment.NewLine + string.Format("<li><a id = \"btnBlackWhite\" onclick=\"CheckForLicense('FLabelBlackwhite')\">{0}</a></li>", "Black & White"));
                sb.Append(Environment.NewLine + string.Format("</div>"));
            }
            // add export 
            if (props.passport.CheckPermission(pr.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.Export))
            {
                model.RightClickToolBar.Menu1btnExportCSV = true;
                model.RightClickToolBar.Menu1btnExportCSVAll = true;
                model.RightClickToolBar.Menu1btnExportTXT = true;
                model.RightClickToolBar.Menu1btnExportTXTAll = true;
                sb.Append(string.Format("<li><a id=\"btnExportCSV\">{0}</a><a id=\"ButtonCSVHidden\" style=\"display: none;\">Export Selected (CSVHidden)</a></li>", "Export Selected" + "(csv)"));
                sb.Append(string.Format("<li><a id=\"btnExportCSVAll\">{0}</a></li>", "Export All" + "(csv)"));
                sb.Append(string.Format("<li><a id=\"btnExportTXT\">{0}</a><a id=\"ButtinTXTHidden\" style=\"display: none;\">Export Selected (TXTHidden)</a></li>", "Export Selected" + "(txt)"));
                sb.Append(string.Format("<li><a id=\"btnExportTXTAll\">{0}</a></li>", "Export All" + "(txt)"));
            }
            sb.Append(Environment.NewLine + "</ul>");
            sb.Append(Environment.NewLine + "</div>");
            // END Tool button dropdown file
        }
        private void ToolBarArrowButton(StringBuilder sb, SearchQueryRequestModal props, Parameters pr, GridDataBinding model)
        {
            // CREATE tool button dropdown arrow
            sb.Append(Environment.NewLine + "<div class=\"btn-group\">");
            sb.Append(Environment.NewLine + "<button class=\"btn btn-secondary dropdown-toggle tab_btn\" data-toggle=\"dropdown\" type=\"button\">");
            sb.Append(Environment.NewLine + "<i class=\"fa fa-send-o fa-fw\"></i>");
            sb.Append(Environment.NewLine + "<i class=\"fa fa-angle-down\"></i>");
            sb.Append(Environment.NewLine + "</button>");

            sb.Append(Environment.NewLine + "<ul class=\"dropdown-menu btn_menu\">");
            LinkScriptLoadWorkFlowButtons(sb, pr, props);
            if (props.passport.CheckPermission(pr.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Request))
            {
                sb.Append(Environment.NewLine + string.Format("<li id=\"divRequestTransfer\"><a onclick=\"CheckForLicense('FRequest')\">{0}</a></li>", "Request"));
            }

            if (model.HasAttachmentcolumn && props.passport.CheckPermission(" Orphans", SecureObject.SecureObjectType.Orphans, Permissions.Permission.Index) && props.passport.CheckPermission(" Orphans", SecureObject.SecureObjectType.Orphans, Permissions.Permission.View) && props.passport.CheckPermission(pr.TableName, SecureObject.SecureObjectType.Attachments, Permissions.Permission.Add) && CheckOrphanVolumPermission(props))
            {
                sb.Append(Environment.NewLine + string.Format("<li id=\"divRequestTransfer\"><a onclick=\"obJvaultfunction.AttachOrphanRecord()\" id=\"OrphanAttachid\">{0}</a></li>", "Attach from Vault"));

            }
            if (props.passport.CheckPermission(pr.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
            {
                model.RightClickToolBar.Menu2btnRequest = true;
                model.RightClickToolBar.Menu2btnTransfer = true;
                model.RightClickToolBar.Menu2btnTransfersTransferAll = true;

                sb.Append(Environment.NewLine + string.Format("<li><a onclick=\"CheckForLicense('FTransfer')\">{0}</a></li>", "Transfer Selected"));
                sb.Append(Environment.NewLine + "<li><a id=\"ButtonTransferHidden\" style=\"display: none;\">Transfer(Hidden)</a></li>");
                sb.Append(string.Format(Environment.NewLine + string.Format("<li><a onclick=\"CheckForLicense('FTransfer', 'All')\">{0}</a></li>", "Transfer All")));
            }
            if (props.passport.CheckPermission(pr.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.Delete))
            {
                model.RightClickToolBar.Menu2delete = true;
                sb.Append(Environment.NewLine + string.Format("<li><a id=\"btndeleterow\">{0}</a></li>", "Delete"));
            }
            if (props.passport.CheckPermission(pr.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Move))
            {
                model.RightClickToolBar.Menu2move = true;
                sb.Append(Environment.NewLine + string.Format("<li><a id=\"btnMoverows\">{0}</a></li>", "Move"));
            }
            if (props.passport.CheckPermission(pr.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
            {
                model.RightClickToolBar.Menu2btnTracking = true;
                // sb.Append(String.Format("<li><a id=""btnTracking"" title=""Toggle tracking and request panes"">{0}</a></li>", Languages.Translation("btnTrackingShow")))
                sb.Append(Environment.NewLine + string.Format("<li><a id=\"btnTracking\" title=\"Toggle tracking and request panes\">{0}</a></li>", "Hide Tracking"));
            }
            sb.Append(Environment.NewLine + string.Format("<li><a onclick=\"obJlastquery.ResetQueries()\" title=\"reset queries\">{0}</a></li>", "Reset Queries"));
            sb.Append(Environment.NewLine + "</ul>");
            sb.Append(Environment.NewLine + "</div>");
        }
        private void ToolBarFavoriteButton(StringBuilder sb, SearchQueryRequestModal props, GridDataBinding model)
        {
            if (props.passport.CheckPermission(Common.SECURE_MYFAVORITE, SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
            {
                model.RightClickToolBar.Favorive = true;
                sb.Append(Environment.NewLine + "<div id=\"divFavOptions\" class=\"btn-group\">");
                sb.Append(Environment.NewLine + "<button class=\"btn btn-secondary dropdown-toggle tab_btn\" data-toggle=\"dropdown\" type=\"button\">");
                sb.Append(Environment.NewLine + "<i class=\"fa fa-heart-o fa-fw\"></i>");
                sb.Append(Environment.NewLine + "<i class=\"fa fa-angle-down\"></i>");
                sb.Append(Environment.NewLine + "</button>");
                sb.Append(Environment.NewLine + "<ul class=\"dropdown-menu btn_menu\">");
                sb.Append(Environment.NewLine + string.Format("<li><a id=\"btnAddFavourite\">{0}</a></li>", "New Favorite"));
                sb.Append(Environment.NewLine + string.Format("<li><a id=\"btnUpdateFavourite\">{0}</a></li>", "Add To Favorite"));
                sb.Append(Environment.NewLine + string.Format("<li id=\"lnkDeleteFavouriteRecords\" style=\"display: none\"><a id=\"btnDeleteFavourite\">{0}</a></li>", "Remove From Favorite"));
                sb.Append(Environment.NewLine + string.Format("<li><a id=\"btnImportFavourite\">{0}</a></li>", "Import Into Favorite"));
                sb.Append(Environment.NewLine + "</ul>");
                sb.Append(Environment.NewLine + "</div>");
            }
        }
        private List<TableHeadersProperty> BuildNewTableHeaderData(GridDataBinding model, SearchQueryRequestModal props, Parameters pr)
        {
            int columnOrder = 0;

            // hide column for pkey
            model.ListOfHeaders.Add(new TableHeadersProperty("pkey", "False", "none", "False", "False", columnOrder, "", false, "", "", false, 0, false));
            model.ListOfColumnWidths.Add(0);

            // if not hide the drill down column
            if (model.HasDrillDowncolumn)
            {
                columnOrder = columnOrder + 1;
                model.ListOfHeaders.Add(new TableHeadersProperty("drilldown", "False", "none", "False", "False", columnOrder, "", false, "", "", false, 0, false));
                model.ListOfColumnWidths.Add(30);
            }
            // create attachment header
            model.HasAttachmentcolumn = false;
            bool checkViewPermission = props.passport.CheckPermission(pr.TableName, SecureObject.SecureObjectType.Attachments, Permissions.Permission.View);

            if (checkViewPermission)
            {
                model.HasAttachmentcolumn = true;
                columnOrder = columnOrder + 1;
                model.ListOfHeaders.Add(new TableHeadersProperty("attachment", "False", "none", "False", "False", columnOrder, "", false, "", "", false, 0, false));
                // ListOfHeaders.Add("<i title='"" + Languages.Translation(""dataGridAttachment_AddAttachment"") + ""' class='fa fa-paperclip fa-flip-horizontal fa-2x theme_color'></i>" + "&&sorter:false")
                model.ListOfColumnWidths.Add(50);
            }

            // create table headers
            foreach (DataColumn col in pr.Data.Columns)
            {
                if (ShowColumn(col, props.paramss.crumbLevel, pr.ParentField))
                {
                    string dataType = col.DataType.Name;
                    var headerName = col.ExtendedProperties["heading"];
                    var isSortable = col.ExtendedProperties["sortable"];
                    var isdropdown = col.ExtendedProperties["dropdownflag"];
                    var isEditable = col.ExtendedProperties["editallowed"];
                    var editmask = col.ExtendedProperties["editmask"];
                    int columnWidth = col.ExtendedProperties["ColumnWidth"] == null ? 0 : Convert.ToInt32(col.ExtendedProperties["ColumnWidth"]);
                    int MaxLength = col.MaxLength;
                    bool isCounterField = false;
                    if (dataType == "Int16")
                    {
                        MaxLength = 5;
                    }
                    else if (dataType == "Int32")
                    {
                        MaxLength = 10;
                    }
                    else if (dataType == "Double")
                    {
                        MaxLength = 53;
                    }

                    var dataTypeFullName = col.DataType.FullName;
                    string ColumnName = col.ColumnName;
                    columnOrder = columnOrder + 1;
                    // build dropdown table
                    if (Convert.ToBoolean(col.ExtendedProperties["dropdownflag"]))
                    {
                        BuildDropdownForcolumn(col, columnOrder, model);
                        if (((DataTable)col.ExtendedProperties["LookupData"]).Columns.Count > 1)
                        {
                            ColumnName = Navigation.MakeSimpleField(((DataTable)col.ExtendedProperties["LookupData"]).TableName);
                            //ColumnName = Navigation.MakeSimpleField(col.ExtendedProperties("LookupData").TableName);
                        }
                    }
                    bool PrimaryKey = false;
                    if ((pr.PrimaryKey ?? "") == (ColumnName ?? ""))
                    {
                        isCounterField = !string.IsNullOrEmpty(pr.TableInfo["CounterFieldName"].ToString());
                        model.ListOfHeaders.Add(new TableHeadersProperty(Convert.ToString(headerName).ToString(), Convert.ToString(isSortable), dataType, Convert.ToString(isdropdown), Convert.ToString(isEditable), columnOrder, Convert.ToString(editmask), col.AllowDBNull, dataTypeFullName, ColumnName, true, MaxLength, isCounterField));
                        PrimaryKey = true;
                    }
                    else
                    {
                        model.ListOfHeaders.Add(new TableHeadersProperty(Convert.ToString(headerName), Convert.ToString(isSortable), dataType, Convert.ToString(isdropdown), Convert.ToString(isEditable), columnOrder, Convert.ToString(editmask), col.AllowDBNull, dataTypeFullName, ColumnName, false, MaxLength, isCounterField));
                    }
                    // holding editable model for lader edit and new row (UI)
                    if (Convert.ToBoolean(isEditable))
                    {
                        string DefaultRetentionId = string.Empty;
                        if ((pr.TableInfo["RetentionFieldName"].ToString() ?? "") == (ColumnName ?? ""))
                        {
                            DefaultRetentionId = pr.TableInfo["DefaultRetentionId"].ToString();
                        }
                        model.ListofEditableHeader.Add(new TableEditableHeader() { HeaderName = Convert.ToString(headerName), Issort = Convert.ToBoolean(isSortable), DataType = dataType, isDropdown = Convert.ToBoolean(isdropdown), isEditable = Convert.ToBoolean(isEditable), columnOrder = columnOrder, editMask = Convert.ToString(editmask), Allownull = col.AllowDBNull, DataTypeFullName = dataTypeFullName, ColumnName = ColumnName, IsPrimarykey = PrimaryKey, MaxLength = MaxLength, isCounterField = isCounterField, DefaultRetentionId = DefaultRetentionId });
                    }
                    model.ListOfColumnWidths.Add(columnWidth);
                }
            }

            return model.ListOfHeaders;
        }
        private void BuildDropdownForcolumn(DataColumn col, int colorder, GridDataBinding model)
        {
            List<string> valueList = new List<string>();
            List<string> displayList = new List<string>();

            DataTable dtLookupData = ((DataTable)col.ExtendedProperties["LookupData"]);

            foreach (DataRow row in dtLookupData.Rows)
            {
                if (dtLookupData.Columns.Count > 1)
                {
                    valueList.Add(row["Value"].ToString().Trim());
                    displayList.Add(row["Display"].ToString().Trim());
                }
                else
                {
                    valueList.Add(row["Display"].ToString().Trim());
                    displayList.Add(row["Display"].ToString().Trim());
                }
            }
            model.ListOfdropdownColumns.Add(new DropDownproperties(colorder, valueList, displayList));
        }
        private void fieldValueParams(Parameters pr, GridDataBinding model)
        {
            pr.QueryType = queryTypeEnum.AdvancedFilter;
            pr.FilterList = model.fvList;
        }
        private int BuildDrillDownLinks(Parameters pr, SearchQueryRequestModal props, GridDataBinding model)
        {
            var sb = new StringBuilder();
            string tables = "," + "" + ",";
            string lastTableName = null;
            int index = 0;
            foreach (var item in Navigation.GetChildViews(pr.ViewId, props.passport))
            {
                if (!tables.Contains("," + item.ChildTableName + ","))
                {
                    if ((item.ChildTableName ?? "") != (lastTableName ?? ""))
                    {
                        lastTableName = item.ChildTableName;
                        sb.Append(string.Format("<li><a data-location=\"3\" onclick=\"obJdrildownclick.Run(this,'{0}','{1}','{2}', '{3}', {4}, {5}, '{6}', '{7}')\">{3}</a></li>", item.ChildTableName, item.ChildKeyField, item.ChildViewID, item.ChildUserName, index, pr.ViewId, item.ChildViewName, item.ChildKeyType));
                        // item.ChildViewID,
                        // params.TableName,
                        // params.ViewName,
                        // params.ViewId,
                        // item.ChildUserName
                        model.ListOfBreadCrumbs.Add(new BreadCrumbsUI()
                        {
                            ChildKeyField = item.ChildKeyField,
                            ChildTableName = item.ChildTableName,
                            ChildUserName = item.ChildUserName,
                            ChildViewid = item.ChildViewID,
                            ChildViewName = item.ChildViewName,
                            TableName = pr.TableName,
                            ViewId = pr.ViewId,
                            ViewName = pr.ViewName,
                            ChildKeyType = item.ChildKeyType
                        });

                        index += 1;
                    }
                }
                var drillModel = new BreadCrumbsUI();
            }

            if (Tracking.get_IsContainer(pr.TableName, props.passport))
            {
                sb.Append(string.Format("<li><a onclick=\"obJreportsrecord.ContentsPerRow()\">{0}</a></li>", "Contents"));
            }

            if (props.passport.CheckPermission(" Auditing", SecureObject.SecureObjectType.Reports, Permissions.Permission.View) & Navigation.IsAuditingEnabled(pr.TableName, props.passport.ConnectionString))
            {
                sb.Append(string.Format("<li><a onclick=\"obJreportsrecord.AuditHistoryRow()\">{0}</a></li>", "Audit History"));
            }
            // Start: Added RetentionInfo Link
            string retentionField = pr.TableInfo["RetentionFieldName"].ToString();
            if (pr.Data.Rows.Count > 0 && !string.IsNullOrEmpty(retentionField) && (Navigation.CBoolean(pr.TableInfo["RetentionPeriodActive"]) || Navigation.CBoolean(pr.TableInfo["RetentionInactivityActive"])))
            {
                sb.Append(string.Format("<li><a onclick=\"obJretentioninfo.GetInfo()\">{0}</a></li>", "Retention Info"));
            }
            // End: Added RetentionInfo Link
            // If _passport.CheckSetting(params.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer) AndAlso
            // _passport.CheckPermission(" Tracking", Smead.SecurityCS.SecureObject.SecureObjectType.Reports, Smead.SecurityCS.Permissions.Permission.View) Then
            // sb.Append(String.Format("<li><a onclick=""obJreportsrecord.TrackingHistoryRow()"">{0}</a></li>", Languages.Translation("TrackingHistory")))
            // End If
            if (props.passport.CheckSetting(pr.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
            {
                sb.Append(string.Format("<li><a onclick=\"obJreportsrecord.TrackingHistoryRow()\">{0}</a></li>", "Tracking History"));
            }
            // sb.Append("</ul>")
            model.ListOfdrilldownLinks = sb.ToString();
            return model.ListOfdrilldownLinks.Count();
        }
        private void WhereClauseParams(Parameters pr, GridDataBinding model)
        {
            pr.QueryType = queryTypeEnum.OpenTable;
            pr.Scope = ScopeEnum.Table;
            pr.ParentField = string.IsNullOrEmpty(pr.ParentField) ? String.Empty : pr.ParentField;
            pr.ParentValue = string.IsNullOrEmpty(pr.ParentValue) ? String.Empty : pr.ParentValue;
            if (model.IsOpenWhereClause)
            {
                pr.WhereClause = model.WhereClauseStr;
            }
            else if (model.fvList.Count > 0)
            {
                pr.WhereClause = string.Format("{0} in ({1}) {2}", pr.KeyField, model.WhereClauseStr, pr.AndFilter);
            }
            else
            {
                pr.WhereClause = string.Format("{0} in ({1})", pr.KeyField, model.WhereClauseStr);
            }

        }
        private void GlobalSearchParams(Parameters pr, GridDataBinding model, SearchQueryRequestModal props)
        {
            if (model.GsIsAllGlobalRequest)
            {
                pr.QueryType = queryTypeEnum.Text;
                pr.Text = model.GsSearchText;
                pr.Scope = ScopeEnum.Table;
                pr.IncludeAttachments = model.GsIncludeAttchment;
            }
            else
            {
                pr.QueryType = queryTypeEnum.KeyValuePair;
                pr.Scope = ScopeEnum.Table;
                pr.KeyField = Navigation.GetPrimaryKeyFieldName(Navigation.GetViewTableName(pr.ViewId, props.passport), props.passport);
                pr.KeyValue = model.GsKeyvalue;
                pr.IncludeViewFilters = false;
            }
        }
        private List<FieldValue> CreateQuery(SearchQueryRequestModal props)
        {
            var list = new List<FieldValue>();
            if (!(props.searchQuery == null))
            {
                foreach (var row in props.searchQuery)
                {
                    var fv = new FieldValue(row.columnName, row.ColumnType);
                    if (!string.IsNullOrEmpty(row.operators.Trim()))
                    {
                        fv.Operate = row.operators;
                        if (string.IsNullOrEmpty(row.values))
                        {
                            fv.value = "";
                        }
                        else if (row.ColumnType == "System.DateTime")
                        {
                            if (row.values.Contains("|"))
                            {
                                var dt = row.values.Split('|');
                                string checkFieldDateStart = MSRecordsEngine.RecordsManager.DateFormat.get_ConvertCultureDate(dt[0].ToString());
                                string checkFieldDateEnd = MSRecordsEngine.RecordsManager.DateFormat.get_ConvertCultureDate(dt[1].ToString());
                                fv.value = string.Format("{0}|{1}", checkFieldDateStart, checkFieldDateEnd);
                            }
                            else
                            {
                                fv.value = MSRecordsEngine.RecordsManager.DateFormat.get_ConvertCultureDate(row.values.ToString());
                            }
                        }
                        else
                        {
                            fv.value = row.values;
                        }
                        list.Add(fv);
                    }
                }
            }
            return list;
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
        private void LinkScriptLoadWorkFlowButtons(StringBuilder sb, Parameters pr, SearchQueryRequestModal props)
        {
            var dt = Navigation.GetViewWorkFlows(pr.ViewId, props.passport);
            string Title = "";
            string ButtonName = "";
            string ScriptId = "";
            if (dt.Rows.Count != 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (!ReferenceEquals(dt.AsEnumerable().ElementAtOrDefault(0)["WorkFlow" + i.ToString()], DBNull.Value) && props.passport.CheckPermission(Convert.ToString(dt.AsEnumerable().ElementAtOrDefault(0)["WorkFlow" + i.ToString()]), SecureObject.SecureObjectType.LinkScript, Permissions.Permission.Execute))
                    {
                        ScriptId = dt.AsEnumerable().ElementAtOrDefault(0)["WorkFlow" + i.ToString()].ToString();
                        if (!ReferenceEquals(dt.AsEnumerable().ElementAtOrDefault(0)["WorkFlowDesc" + i.ToString()], DBNull.Value))
                        {
                            ButtonName = dt.AsEnumerable().ElementAtOrDefault(0)["WorkFlowDesc" + i.ToString()].ToString();
                        }
                        if (!ReferenceEquals(dt.AsEnumerable().ElementAtOrDefault(0)["WorkFlowToolTip" + i.ToString()], DBNull.Value))
                        {
                            Title = dt.AsEnumerable().ElementAtOrDefault(0)["WorkFlowToolTip" + i.ToString()].ToString();
                        }

                        sb.Append(Environment.NewLine + "<li><span>");
                        sb.Append(Environment.NewLine + string.Format("<a id=\"{0}\" title=\"{1}\" onclick=\"obJlinkscript.ClickButton(this)\" >{2}</a>", ScriptId, Title, ButtonName));
                        sb.Append(Environment.NewLine + "</li></span>");
                    }
                }
            }
        }
        private bool CheckOrphanVolumPermission(SearchQueryRequestModal props)
        {
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                foreach (var v in context.Volumes.ToList())
                {
                    if (props.passport.CheckPermission(v.Name, SecureObject.SecureObjectType.Volumes, Permissions.Permission.Add))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void Buildrows(Parameters pr, GridDataBinding model, SearchQueryRequestModal props)
        {
            // build rows
            foreach (DataRow dr in pr.Data.Rows)
            {
                // 'get the pkey
                var Cell = new TableColums();
                Cell.DataColumn = dr["pkey"].ToString();
                var ListOfColumn = new List<string>();
                ListOfColumn.Add(Cell.DataColumn);
                if (model.HasDrillDowncolumn)
                {
                    Cell.DataColumn = "drilldown";
                    ListOfColumn.Add(Cell.DataColumn);
                }
                if (model.HasAttachmentcolumn)
                {
                    if (!string.IsNullOrEmpty(dr["Attachments"].ToString()))
                    {
                        Cell.DataColumn = dr["Attachments"].ToString();
                    }
                    else
                    {
                        Cell.DataColumn = "0";
                    }
                    ListOfColumn.Add(Cell.DataColumn);
                }

                foreach (DataColumn col in pr.Data.Columns)
                {
                    if (ShowColumn(col, props.paramss.crumbLevel, pr.ParentField) & col.ColumnName.ToString().Length > 0)
                    {
                        if (Convert.ToString(col.ColumnName) is not null)
                        {

                            if (!string.IsNullOrEmpty(dr[col.ColumnName].ToString()))
                            {
                                if (col.DataType.Name == "DateTime")
                                {
                                    Cell.DataColumn = Convert.ToString(dr[col.ColumnName.ToString()]).Split(" ")[0];
                                }
                                else
                                {
                                    Cell.DataColumn = Convert.ToString(dr[col.ColumnName.ToString()]);
                                }
                            }
                            else
                            {
                                Cell.DataColumn = "";
                            }
                        }
                        ListOfColumn.Add(Cell.DataColumn.Trim());
                    }
                }
                model.ListOfDatarows.Add(ListOfColumn);
                //ListOfColumn = new List<string>();
            }
        }

    }
}
