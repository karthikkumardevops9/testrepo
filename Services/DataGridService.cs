using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services.Interface;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using SecureObject = Smead.Security.SecureObject;
using static MSRecordsEngine.Models.Enums;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using static MSRecordsEngine.Models.FusionModels.LinkScriptModel;
using Permissions = Smead.Security.Permissions;
using System.Data.Entity;
using System.Net;
using AuditType = MSRecordsEngine.RecordsManager.AuditType;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Smead.Security;
using static Leadtools.Annotations.WinForms.StringManager;
using Microsoft.Identity.Client;
using static MSRecordsEngine.Models.ReportingPerRow;
using System.Net.Http;
using MSRecordsEngine.Repository;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Configuration;
using static MSRecordsEngine.Models.RetentionInfo;
using System.Globalization;
using MSRecordsEngine.Properties;
using static MSRecordsEngine.Models.AuditReportSearch;
using MSRecordsEngine.Entities.Mapping;

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
        public async Task<GridDataBinding> RunQuery(SearchQueryRequestModal props)
        {
            var model = new GridDataBinding();
            model.IsWhereClauseRequest = props.GridDataBinding.IsWhereClauseRequest;
            model.WhereClauseStr = props.GridDataBinding.WhereClauseStr;
            model.GsIsGlobalSearch = props.GridDataBinding.GsIsGlobalSearch;
            model.GsSearchText = props.GridDataBinding.GsSearchText;
            model.GsIncludeAttchment = props.GridDataBinding.GsIncludeAttchment;
            model.GsIsAllGlobalRequest = props.GridDataBinding.GsIsAllGlobalRequest;

            props.GridDataBinding.ItemDescription = Navigation.GetItemName(props.paramss.preTableName, props.paramss.Childid, props.passport);
            if (props.searchQuery.Count > 0)
                model.fvList = CreateQuery(props);
            await BuildNewTableData(props, model);

            return model;
        }
        public async Task<GridDataBinding> BuildNewFavoriteData(ReturnFavoritTogridReqModel prop)
        {
            var model = new GridDataBinding();
            model.ViewId = prop.paramss.ViewId;
            model.PageNumber = prop.paramss.pageNum;
            model.crumbLevel = 0;
            model.fViewType = (int)ViewType.Favorite;
            model.IsWhereClauseRequest = true;
            model.WhereClauseStr = string.Format("select [TableId] from s_SavedChildrenFavorite where SavedCriteriaId = {0}", prop.paramss.FavCriteriaid);

            var props = new SearchQueryRequestModal();
            props.passport = prop.passport;
            props.DateFormat = prop.DateFormat;
            props.paramss.pageNum = prop.paramss.pageNum;
            props.paramss.ViewId = prop.paramss.ViewId;
            props.paramss.crumbLevel = 0;
            props.searchQuery = prop.searchQuery;

            if (prop.searchQuery != null)
                model.fvList = CreateQuery(props);

            await BuildNewTableData(props, model);

            return model;
        }
        public async Task<bool> DeleteSavedCriteria(FavoriteRecordReqModel props)
        {
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                if (props.paramss.FavCriteriaType == "1")
                {
                    var saveCriteria = context.s_SavedCriteria.Where(a => a.Id == props.paramss.FavCriteriaid);
                    var SavedChildrenFavorite = context.s_SavedChildrenFavorite.Where(x => x.SavedCriteriaId == props.paramss.FavCriteriaid).ToList();
                    if (saveCriteria != null)
                    {
                        context.s_SavedCriteria.Remove(saveCriteria.FirstOrDefault());
                        if (SavedChildrenFavorite.Count != 0)
                        {
                            context.s_SavedChildrenFavorite.RemoveRange(SavedChildrenFavorite);
                        }

                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    var querycert = context.s_SavedChildrenQuery.Where(a => a.SavedCriteriaId == props.paramss.FavCriteriaid).ToList();
                    if (querycert.Count != 0)
                    {
                        context.s_SavedChildrenQuery.RemoveRange(querycert);
                        await context.SaveChangesAsync();
                    }

                }
            }
            return true;
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


            if (TotalRows / (double)RequestedRows > 0 & TotalRows / (double)RequestedRows < 1)
                TotalPages = 1;
            else if (TotalRows % RequestedRows == 0)
                TotalPages = (int)(TotalRows / (double)RequestedRows);
            else
            {
                int tp = (int)(TotalRows / (double)RequestedRows);
                TotalPages = tp + 1;
            }

            return TotalRows + "|" + TotalPages + "|" + RequestedRows;
        }
        public async Task<ScriptReturn> LinkscriptButtonClick(linkscriptPropertiesUI props)
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
        public async Task<TrackingModeld> GetTrackingPerRow(trackableUiParams props)
        {
            var model = new TrackingModeld();

            await Task.Run(() =>
            {
                GetTrackingInfo(model, props);
                GetRequestWaitlist(model, props);
            });
            return model;
        }
        public async Task<MyFavorite> AddNewFavorite(FavoriteRecordReqModel props)
        {
            var listOfKeys = new List<string>();
            var model = new MyFavorite();

            using (var contex = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                if (contex.s_SavedCriteria.Where(a => a.SavedName.Trim() == props.paramss.NewFavoriteName.Trim()).ToList().Count > 0)
                {
                    model.isWarning = true;
                    model.Msg = string.Format("'{0}' already exists, Please try other name for favorite", props.paramss.NewFavoriteName);
                }
                else
                {
                    foreach (var rec in props.recordkeys)
                        listOfKeys.Add(rec.rowKeys);
                    var msg = "";
                    var ps_SavedCriteriaId = await SaveSavedCriteria(props.passport.UserId, msg, props.paramss.NewFavoriteName, props.paramss.ViewId, props.passport.ConnectionString);
                    if (ps_SavedCriteriaId != default)
                    {
                        var pOutPut = await SaveSavedChildrenFavourite(msg, true, ps_SavedCriteriaId, props.paramss.ViewId, listOfKeys, props.passport.ConnectionString);
                        model.SaveCriteriaId = ps_SavedCriteriaId;
                    }
                }
            }

            return model;
        }
        public async Task<MyFavorite> UpdateFavorite(FavoriteRecordReqModel props)
        {
            var model = new MyFavorite();
            var listOfKeys = new List<string>();
            foreach (var rec in props.recordkeys)
                listOfKeys.Add(rec.rowKeys);
            await SaveSavedChildrenFavourite("", true, props.paramss.FavCriteriaid, props.paramss.ViewId, listOfKeys, props.passport.ConnectionString);
            model.SaveCriteriaId = props.paramss.FavCriteriaid;
            return model;
        }
        public async Task<bool> DeleteFavoriteRecord(FavoriteRecordReqModel props)
        {
            bool isdeleted = false;
            var lst = new List<string>();
            foreach (var d in props.recordkeys)
                lst.Add(d.rowKeys);

            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                var SavedChildrenFavoriteList = await context.s_SavedChildrenFavorite.Where(m => m.SavedCriteriaId == props.paramss.FavCriteriaid && lst.Contains(m.TableId)).ToListAsync();
                if (SavedChildrenFavoriteList != null)
                    context.s_SavedChildrenFavorite.RemoveRange(SavedChildrenFavoriteList);
                await context.SaveChangesAsync();
                isdeleted = true;
            }
            return isdeleted;
        }
        public async Task<Myquery> SaveNewQuery(SaveNewUpdateDeleteQueryReqModel props)
        {
            var model = new Myquery();

            model.Msg = "success";
            s_SavedCriteria result = new s_SavedCriteria();
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                //check permission
                if (!props.passport.CheckPermission(Common.SECURE_MYQUERY, Smead.Security.SecureObject.SecureObjectType.Application, Permissions.Permission.Access))
                {
                    model.IsError = true;
                    model.Msg = "You don't have sufficient permission to add new query.";
                    return model;
                }
                //check if name exist 
                var check = context.s_SavedCriteria.Where(x => x.SavedName == props.paramss.SaveName & x.UserId == props.passport.UserId).FirstOrDefault();
                if (check != null)
                {
                    model.isNameExist = true;
                    return model;
                }
                // first save in parent table
                s_SavedCriteria savedCriteria = new s_SavedCriteria();
                savedCriteria.SavedName = props.paramss.SaveName;
                savedCriteria.SavedType = (int)Enums.SavedType.Query;
                savedCriteria.UserId = props.passport.UserId;
                savedCriteria.ViewId = props.paramss.ViewId;
                context.s_SavedCriteria.Add(savedCriteria);
                await context.SaveChangesAsync();
                result = context.s_SavedCriteria.Where(x => x.SavedName == props.paramss.SaveName & x.UserId == props.passport.UserId).FirstOrDefault();
                if (result != null)
                {
                    s_SavedChildrenQuery savedChildrenQuery = new s_SavedChildrenQuery();
                    foreach (queryListparams item in props.Querylist)
                    {
                        savedChildrenQuery.SavedCriteriaId = result.Id;
                        savedChildrenQuery.Operator = item.operators;
                        savedChildrenQuery.ColumnName = item.columnName;
                        savedChildrenQuery.CriteriaValue = item.values == null ? "" : item.values;
                        context.s_SavedChildrenQuery.Add(savedChildrenQuery);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    model.IsError = true;
                    model.Msg = "We couldn't add a new query. Please contact the system administrator";
                }
            }

            model.uiparam = string.Format("{0}_{1}_{2}", props.paramss.ViewId, result.Id, 0);
            return model;
        }
        public async Task<Myquery> UpdateQuery(SaveNewUpdateDeleteQueryReqModel props)
        {
            var model = new Myquery();
            s_SavedCriteria s_SavedCriteria = new s_SavedCriteria();

            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                context.s_SavedCriteria.Where(a => a.Id == props.paramss.SavedCriteriaid).FirstOrDefault();
                s_SavedCriteria.SavedName = props.paramss.SaveName;
                s_SavedCriteria.SavedType = (int)Enums.SavedType.Query;
                s_SavedCriteria.UserId = props.passport.UserId;
                s_SavedCriteria.ViewId = props.paramss.ViewId;
                await context.SaveChangesAsync();
                var savedChildrenQuery = context.s_SavedChildrenQuery.Where(a => a.SavedCriteriaId == props.paramss.SavedCriteriaid).ToList();
                int index = 0;
                foreach (var prop in savedChildrenQuery)
                {
                    var item = props.Querylist[index];
                    prop.SavedCriteriaId = props.paramss.SavedCriteriaid;
                    prop.Operator = item.operators;
                    prop.ColumnName = item.columnName;
                    prop.CriteriaValue = item.values == null ? "" : item.values;
                    await context.SaveChangesAsync();
                    index += 1;
                }
            }
            return model;
        }
        public async Task<Myquery> DeleteQuery(SaveNewUpdateDeleteQueryReqModel props)
        {
            var model = new Myquery();
            model.Msg = "success";
            s_SavedCriteria s_SavedCriteria = new s_SavedCriteria();
            s_SavedChildrenQuery s_SavedChildrenQuery = new s_SavedChildrenQuery();

            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                var delparent = context.s_SavedCriteria.Where(a => a.Id == props.paramss.SavedCriteriaid & a.UserId == props.passport.UserId).FirstOrDefault();
                context.s_SavedCriteria.Remove(delparent);
                var delchild = context.s_SavedChildrenQuery.Where(x => x.SavedCriteriaId == props.paramss.SavedCriteriaid).ToList();
                context.s_SavedChildrenQuery.RemoveRange(delchild);
                await context.SaveChangesAsync();
            }
            return model;
        }
        public async Task<GlobalSearch> RunglobalSearch(GlobalSearchReqModel props)
        {
            var model = new GlobalSearch();
            if (props.paramss.SearchInput is null || props.paramss.SearchInput.Length == 0)
                return model;

            var qry = new Query(props.passport);
            var @params = new Parameters(props.passport);
            var savedSearches = new List<string>();

            CheckBoxesconditions(@params, props);
            @params.Text = props.paramss.SearchInput;

            @params.IncludeAttachments = props.paramss.ChkAttch;

            @params.QueryType = queryTypeEnum.Text;
            // params.RequestedRows = 500
            string sMaxVal = string.Empty;
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                sMaxVal = await context.Settings.Where(x => x.Section == "GlobalSearch" & x.Item == "MaxRecordsFetch").Select(y => y.ItemValue).FirstOrDefaultAsync();
            }
            int globalSearchMaxVal = 25;
            //string sMaxVal = _iSettings.All().Where(x => x.Section == "GlobalSearch" & x.Item == "MaxRecordsFetch").Select(y => y.ItemValue).FirstOrDefault();
            if (!string.IsNullOrEmpty(sMaxVal))
            {
                globalSearchMaxVal = Convert.ToInt32(sMaxVal);
            }
            @params.RequestedRows = globalSearchMaxVal;
            @params.IsMVCCall = true;
            qry.Search(@params);
            model.HTMLSearchResults = @params.HTMLSearchResults;

            return model;

        }
        public async Task<GridDataBinding> GlobalSearchClick(GlobalSearchReqModel props)
        {
            var model = new GridDataBinding();
            model.ViewId = props.paramss.ViewId;
            model.crumbLevel = 0;

            model.GsIsGlobalSearch = true;
            model.GsKeyvalue = WebUtility.UrlDecode(props.paramss.KeyValue).Replace("'", "''");
            model.GsSearchText = props.paramss.SearchInput;
            model.GsIsAllGlobalRequest = false;

            var prop = new SearchQueryRequestModal();
            prop.passport = props.passport;
            prop.DateFormat = props.DateFormat;
            prop.paramss.ViewId = props.paramss.ViewId;
            prop.paramss.crumbLevel = 0;


            await BuildNewTableData(prop, model);
            return model;
        }
        public async Task<GridDataBinding> GlobalSearchAllClick(GlobalSearchReqModel props)
        {
            var model = new GridDataBinding();


            model.GsIsGlobalSearch = true;
            model.GsSearchText = props.paramss.SearchInput;
            model.GsIncludeAttchment = props.paramss.IncludeAttchment;
            model.GsIsAllGlobalRequest = true;

            var prop = new SearchQueryRequestModal();
            prop.passport = props.passport;
            prop.DateFormat = props.DateFormat;
            prop.paramss.ViewId = props.paramss.ViewId;
            prop.paramss.crumbLevel = 0;

            await BuildNewTableData(prop, model);

            return model;
        }
        public async Task<Saverows> SetDatabaseChanges(DatabaseChangesReq props)
        {
            var model = new Saverows();
            var pkeyId = props.Rowdata[props.Rowdata.Count - 1].value;

            //model = new Saverows(passport, req.paramss, Keys.GetClientIpAddress(httpContext), req.Rowdata, pkeyId, httpContext);
            if ((string.IsNullOrEmpty(pkeyId)))
            {
                // insert new row
                await AddNewRow(props, model);
            }
            else
            {
                // save edit row
                await EditRow(props, model);
            }
            return model;
        }
        public async Task DeleteRowsFromGrid(DeleteRowsFromGridReqModel req, Deleterows model)
        {
            var param = new Parameters(req.paramss.ViewId, req.passport);
            model.isError = false;

            if (req.passport.CheckPermission(param.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.Delete))
            {
                await Navigation.VerifyLegalDeletionAsync(param.TableName, req.rowData.ids, req.passport);
                ScriptReturn result = null;

                foreach (string item in req.rowData.ids)
                {
                    // run linkscript before delete
                    if (model.scriptDone == false)
                    {
                        LinkScriptRunBeforeDelete(result, param, item, model, req);
                        if (model.scriptReturn.isBeforeDeleteLinkScript)
                            return;
                    }
                    else
                    {
                        result = new ScriptReturn(true, "", 0.ToString(), false);
                    }
                    Query.DeleteTableItem(param.TableName, item, "00:00", false, true, false, req.passport);
                    // run linkscript after delete
                    LinkScriptRunAfterDelete(result, param, model, req);
                }
            }
            else
            {
                model.Msg = "You don't have sufficient Permission to Delete";
                model.isError = true;
            }

        }
        public async Task<GridDataBinding> TaskBarClick(UserInterfaceProps props)
        {
            var model = new GridDataBinding();
            model.IsOpenWhereClause = true;
            model.IsWhereClauseRequest = true;
            model.WhereClauseStr = "";
            model.ViewId = props.ViewId;
            model.fViewType = (int)ViewType.FusionView;

            var prop = new SearchQueryRequestModal();
            prop.paramss.ViewId = props.ViewId;
            prop.paramss.pageNum = 1;
            prop.paramss.crumbLevel = 0;
            prop.passport = props.passport;

            await BuildNewTableData(prop, model);
            return model;
        }
        public async Task<ReportingPerRow> ExecuteReporting(ReportingReqModel props)
        {
            // offset pages start from 
            var report = new ReportingPerRow();
            int offsetPages;
            if (props.paramss.pageNumber == 1)
            {
                offsetPages = 0;
            }
            else
            {
                offsetPages = props.paramss.pageNumber * 300;
            }
            // get item description
            string tablename = props.paramss.tableName.Replace("'", "''");
            string tableid = props.paramss.Tableid.Replace("'", "''");
            report.ItemDescription = Navigation.GetItemName(tablename, tableid, props.passport);
            var reportName = (ReportingPerRow.Reports)props.paramss.reportNum;
            switch (reportName)
            {
                case Reports.AuditHistoryPerRow:
                    {

                        await GenerateAuditHistoryPerRow(offsetPages, report, props);
                        break;
                    }
                case Reports.TrackingHistoryPerRow:
                    {
                        await GenerateTrackingHistoryPerRow(offsetPages, report, props);
                        break;
                    }
                case Reports.ContentsRow:
                    {
                        GenerateContentPerRow(offsetPages, report, props);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return report;
        }
        public async Task<PagingModel> ExecuteReportingCount(ReportingReqModel props)
        {
            // offset pages start from 
            //var report = new ReportingPerRow();
            //report.ItemDescription = Navigation.GetItemName(req.paramss.tableName,req.paramss.Tableid, req.passport);
            var model = new PagingModel();
            switch ((Reports)props.paramss.reportNum)
            {
                case Reports.AuditHistoryPerRow:
                    {
                        //GenerateAuditHistoryPerRowCount(report,req);
                        if (props.passport.CheckPermission(" Auditing", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                        {
                            model = await GenerateRowsAuditCount(props);
                        }

                        break;
                    }
                case Reports.TrackingHistoryPerRow:
                    {
                        if (props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                        {
                            //GenerateTrackingHistoryPerRowCount(this, _httpContext);
                            model = await GenerateRowsTrackingCount(props);
                        }
                        break;
                    }
                case Reports.ContentsRow:
                    {
                        //model.GenerateContentPerRowCount(this);
                        model = await GenerateRowsContentsCount(props);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return model;
        }
        public async Task<RetentionInfo> OnDropdownChange(RetentionInfoUpdateReqModel req)
        {
            var model = new RetentionInfo();
            var @params = new Parameters(req.ViewId, req.passport);
            model.RetentionItem = "Record Details";
            SetActiveItem(model, @params, req);
            var tableInfo = Navigation.GetTableInfo(@params.TableName, req.passport);
            var codeRow = Retention.GetRetentionCode(req.props.RetentionItemText, req.passport);
            RetentionArchiveDate(tableInfo, codeRow, model, req);
            RetentionInactiveDate(tableInfo, codeRow, model, req);
            SetRetentionStatus(tableInfo, codeRow, model, req);
            await RetentionArchiveinfo(model, req);
            return model;
        }
        public async Task<RetentionInfo> RetentionInfoUpdate(RetentionInfoUpdateReqModel req)
        {
            var model = new RetentionInfo();
            await UpdateRetentionCodeInTableRecord(req);
            await DeleteDestructCertItem(req);
            await UpdateDestructCertItem(req);
            model.ReturnOnerow = await ReturnOnerow(req);
            using (var conn = new SqlConnection(req.passport.ConnectionString))
            {
                await conn.OpenAsync();
                if (GetDestructionCertChildrenCount(req.props.SldestructionCertId, conn) == 0)
                    Retention.DeleteDestructionCertRecord(req.props.SldestructionCertId, conn);
            }
            return model;
        }
        public async Task<List<List<string>>> ReturnOnerow(RetentionInfoUpdateReqModel req)
        {
            var param = new Parameters(req.props.viewid, req.passport);
            var model = new SearchQueryRequestModal();
            model.passport = req.passport;
            model.ViewId = req.props.viewid;
            model.GridDataBinding.IsWhereClauseRequest = true;
            if (Navigation.IsAStringType(param.IdFieldDataType) || Navigation.IsADateType(param.IdFieldDataType))
                model.GridDataBinding.WhereClauseStr = string.Format("SELECT [{0}] FROM [{1}] where [{0}] = '{2}'", Navigation.MakeSimpleField(param.KeyField), param.TableName, req.props.rowid.Replace("'", "''"));
            else
                model.GridDataBinding.WhereClauseStr = string.Format("SELECT [{0}] FROM [{1}] where [{0}] = {2}", Navigation.MakeSimpleField(param.KeyField), param.TableName, req.props.rowid);
            var grid = new GridDataBinding();
            grid = await RunQuery(model);
            return grid.ListOfDatarows;
        }
        public async Task<bool> CheckBeforeAddTofavorite(UserInterfaceProps props)
        {
            bool hasList = false;
            try
            {
                using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
                {
                    var criteriaList = await context.s_SavedCriteria.Where(a => a.ViewId == props.ViewId & a.SavedType == 1).ToListAsync();
                    if (criteriaList.Count > 0)
                        hasList = true;
                    return hasList;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<MyFavorite> StartDialogAddToFavorite(UserInterfaceProps props)
        {
            var model = new MyFavorite();
            model.placeholder = "Select Favorite";
            model.label = "Favorite: ";

            IRepository<s_SavedCriteria> _is_SavedCriteria = new Repository.Repositories<s_SavedCriteria>();
            using (var contex = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                var criteriaList = await contex.s_SavedCriteria.Where(a => a.ViewId == props.ViewId & a.SavedType == 1).ToListAsync();
                if (criteriaList.Count > 0)
                {
                    foreach (var lst in criteriaList)
                    {
                        var ddl = new MyFavorite.FavoritedropdownList();
                        var name = lst.SavedName;
                        ddl.text = name;
                        ddl.value = lst.Id.ToString();
                        model.ListAddtoFavorite.Add(ddl);
                    }
                }
            }
            return model;
        }
        public async Task<AuditReportSearch> loadDialogReportSearch(UserInterfaceProps props)
        {
            var model = new AuditReportSearch();
            await BindUserDDL(props, model);
            await BindTableDDL(props, model);
            model.dateFormat = props.DateFormat;
            return model;
        }
        public async Task<RetentionInfo> GetRetentionInfoPerRow(RetentionInfoUpdateReqModel req)
        {
            var model = new RetentionInfo();
            var @params = new Parameters(req.props.viewid, req.passport);
            req.props.TableName = @params.TableName;
            var tableInfo = Navigation.GetTableInfo(@params.TableName, req.passport);
            model.RetentionItem = "Record Details";

            SetActiveItem(model, @params, req);
            var codeRow = BuildDropdwonList(model, req, tableInfo);
            SetRetentionStatus(tableInfo, codeRow, model, req);
            await RetentionArchiveinfo(model, req);
            await GenerateHoldingTable(model, @params, req);
            CheckIfRetentionAssign(@params, model);
            return model;
        }
        public List<string> DialogMsgConfirmDelete(DialogMsgConfirm req)
        {
            var lst = new List<string>();

            foreach (string id in req.paramss.ids)
                lst.Add(Navigation.GetItemName(req.paramss.TableName, id, req.passport));

            return lst;
        }
        private void CheckIfRetentionAssign(Parameters @params, RetentionInfo model)
        {
            // check if disable the dropdown in case there is no retention policy 
            if (@params.TableInfo["RetentionAssignmentMethod"] is DBNull)
            {
                model.DDLDrop = false;
            }
            else if (Convert.ToInt32(@params.TableInfo["RetentionAssignmentMethod"]) == 0)
            {
                try
                {
                    model.DDLDrop = Convert.ToInt32(model.retentionItemRow["%slRetentionDispositionStatus"]) == 0;
                }
                catch (Exception)
                {
                    model.DDLDrop = true;
                }
            }
            else
            {
                model.DDLDrop = false;
            }
        }
        private async Task GenerateHoldingTable(RetentionInfo model, Parameters @params, RetentionInfoUpdateReqModel req)
        {
            model.ListOfHeader.Add("Hold Type");
            model.ListOfHeader.Add("Snooze");
            model.ListOfHeader.Add("Reason");
            var result = new List<SLDestructCertItem>();
            using (var context = new TABFusionRMSContext(req.passport.ConnectionString))
            {
                result = await context.SLDestructCertItems.Where(x => x.TableName == @params.TableName && x.TableId == req.props.rowid && (x.RetentionHold == true || x.LegalHold == true)).OrderBy(x => x.Id).ToListAsync();
                if (result.Count == 0)
                {
                    var certid = await context.SLDestructCertItems.Where(x => x.TableName == @params.TableName && x.TableId == req.props.rowid).OrderBy(x => x.Id).FirstOrDefaultAsync();
                    if (certid is not null)
                        model.SldestructionCertid = (int)certid.SLDestructionCertsId;
                }
            }



            for (int index = 0, loopTo = result.Count - 1; index <= loopTo; index++)
            {
                var cell = new List<string>();
                var objNewSLDestructCertItem = new NewSLDestructCertItem();
                if (true is var arg12 && result[index].LegalHold is { } arg11 && arg11 == arg12)
                {
                    cell.Add("Legal");
                }
                else if (true is var arg14 && result[index].RetentionHold is { } arg13 && arg13 == arg14)
                {
                    cell.Add("Retention");
                }
                if (!string.IsNullOrEmpty(Convert.ToString(result[index].SnoozeUntil)))
                {
                    //DateTime.Parse(row["TransactionDateTime"].ToString()).ToString(props.DateFormat)
                    cell.Add(DateTime.Parse(result[index].SnoozeUntil.ToString()).ToString(req.DateFormat));
                }
                else
                {
                    cell.Add("");
                }
                cell.Add(result[index].HoldReason);
                model.ListOfRows.Add(cell);

                // TableHoldingTemp.Add(New holdingTableprop With {.Id = result(index).Id, .HoldReason = result(index).HoldReason, .LegalHold = result(index).LegalHold, .RetentionCode = result(index).RetentionCode, .RetentionHold = result(index).RetentionHold, .SLDestructionCertsId = result(index).SLDestructionCertsId, .SnoozeUntil = result(index).SnoozeUntil, .TableId = result(index).TableId, .TableName = result(index).TableName})
            }
        }
        private DataRow BuildDropdwonList(RetentionInfo model, RetentionInfoUpdateReqModel req, DataRow tableinfo)
        {
            model.selectedItemText = "";
            model.RetentionDescription = "";
            var dtRetention = Retention.GetRetentionCodes(req.passport);
            string retentionField = tableinfo["RetentionFieldName"].ToString();
            foreach (DataRow retRow in dtRetention.Rows)
            {
                if (string.Compare(model.retentionItemRow[retentionField].ToString(), retRow["id"].ToString(), true) == 0)
                {
                    model.RetentionDescription = retRow["description"].ToString();
                    model.selectedItemText = retRow["id"].ToString().ToUpper();
                    model.DropdownRetentionCode.Add(new dropdownCode() { value = retRow["description"].ToString(), text = retRow["id"].ToString().ToUpper(), selected = true });
                }
                else
                {
                    model.DropdownRetentionCode.Add(new dropdownCode() { value = retRow["description"].ToString(), text = retRow["id"].ToString().ToUpper(), selected = false });
                }
            }
            var codeRow = Retention.GetRetentionCode(model.selectedItemText, req.passport);
            RetentionArchiveDate(tableinfo, codeRow, model, req);
            RetentionInactiveDate(tableinfo, codeRow, model, req);
            return codeRow;
        }
        private async Task BindTableDDL(UserInterfaceProps props, AuditReportSearch model)
        {
            string sSQL;
            var dsObject = new DataTable();
            //sSQL = "select UserName,TableName +'|'+cast((select COUNT(*) from [dbo].[FNGetChildTables](TableName))AS varchar(20)) as ObjectValue from Tables where TableName = 'true' or AuditUpdate = 'true' or AuditConfidentialData = 'true'";
            sSQL = "select t.UserName, t.TableName + '|' + cast((select COUNT(*) from [dbo].[FNGetChildTables](t.TableName)) AS varchar(20)) as ObjectValue from Tables t inner join (select distinct SO.Name as TableName from SecureUserGroup as SUG inner join SecureObjectPermission as SOP on SUG.GroupID = SOP.GroupID inner join SecureObject as SO on SOP.SecureObjectID = SO.SecureObjectID where SUG.UserID = @UserId and SO.BaseID = 2) as virtualtable on virtualtable.TableName = t.TableName where t.TableName = 'true' or t.AuditUpdate = 'true' or t.AuditConfidentialData = 'true'";
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", props.passport.UserId);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsObject);
                    }
                }
            }
            model.objectDDL.Add(new DDLprops() { text = "Select Table", valuetxt = "-1" });
            model.objectDDL.Add(new DDLprops() { text = "All Tables", valuetxt = "All" });
            foreach (DataRow row in dsObject.Rows)
            {
                bool isIdstring = Navigation.FieldIsAString(row["UserName"].ToString(), props.passport);
                model.objectDDL.Add(new DDLprops() { text = row["UserName"].ToString(), valuetxt = row["ObjectValue"].ToString(), isIdstring = isIdstring });
            }

        }
        private async Task BindUserDDL(UserInterfaceProps props, AuditReportSearch model)
        {
            var bReturnAllUsers = default(bool);
            string sSQL;
            var dsSettings = new DataSet();
            var dsUser = new DataTable();
            if (props.passport.UsingActiveDirectory)
            {
                sSQL = "SELECT UserName, ISNULL(DisplayName,'<unknown user>') AS DispName FROM SecureUser WHERE AccountType = 'A'";
            }
            else if (props.passport.UsingAzureActiveDirectory)
            {
                sSQL = "SELECT UserName, ISNULL(DisplayName,'<unknown user>') AS DispName FROM SecureUser WHERE AccountType = 'Z'";
            }
            else
            {
                sSQL = "SELECT UserName, ISNULL(DisplayName,'<unknown user>') AS DispName FROM SecureUser WHERE AccountType = 'S'";
            }

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("select * from Settings where Section='AuditReport' and Item='ReturnAll'", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsSettings);
                    }
                }
            }

            if (dsSettings is not null)
            {
                if (dsSettings.Tables.Count > 0 && dsSettings.Tables[0].Rows.Count > 0)
                {

                    if (Convert.ToInt32(dsSettings.Tables[0].Rows[0]["Id"]) != 0 & Convert.ToString(dsSettings.Tables[0].Rows[0]["ItemValue"]).Length > 0)
                    {
                        bReturnAllUsers = Convert.ToBoolean(Convert.ToString(dsSettings.Tables[0].Rows[0]["ItemValue"]));
                    }
                }
                if (!bReturnAllUsers)
                {
                    sSQL = sSQL + " AND UserName IN " + Constants.vbCrLf;
                    sSQL = sSQL + " (SELECT DISTINCT [OperatorsId] FROM [SLAuditUpdates] UNION" + Constants.vbCrLf;
                    sSQL = sSQL + "  SELECT DISTINCT [OperatorsId] FROM [SLAuditConfData] UNION" + Constants.vbCrLf;
                    sSQL = sSQL + "  SELECT DISTINCT [OperatorsId] FROM [SLAuditLogins]" + Constants.vbCrLf;
                    sSQL = sSQL + "  WHERE [OperatorsId] IS NOT NULL AND [OperatorsId] > '')";
                }
            }

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sSQL, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsUser);
                    }
                }
            }
            model.userDDL.Add(new DDLprops() { text = "All Users", value = -1 });
            int ddlOrder = 0;
            foreach (DataRow row in dsUser.Rows)
            {
                model.userDDL.Add(new DDLprops() { text = row["UserName"].ToString(), value = ddlOrder });
                ddlOrder = ddlOrder + 1;
            }
        }
        private int GetDestructionCertChildrenCount(int destructionCertID, SqlConnection conn)
        {
            if (destructionCertID == 0)
                return 0;

            using (var cmd = new SqlCommand(Resources.GetDestructionCertChildrenCount, conn))
            {
                cmd.Parameters.AddWithValue("@DestructionCertID", destructionCertID);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return 0;
                    return Convert.ToInt32(dt.Rows[0]["ItemCount"]);
                }
            }
        }
        private async Task UpdateDestructCertItem(RetentionInfoUpdateReqModel param)
        {
            if (param.props.RetTableHolding.Count == 0)
                return;
            using (var context = new TABFusionRMSContext(param.passport.ConnectionString))
            {
                for (int index = 0, loopTo = param.props.RetTableHolding.Count - 1; index <= loopTo; index++)
                {
                    var objSLDestructCertItem = new SLDestructCertItem();

                    try
                    {
                        objSLDestructCertItem.ScheduledDestruction = CommonFunctions.ConvertStringToCulture(param.props.RetnArchive, param.DateFormat);
                    }
                    catch (Exception)
                    {
                        objSLDestructCertItem.ScheduledDestruction = default;
                    }

                    try
                    {
                        objSLDestructCertItem.ScheduledInactivity = CommonFunctions.ConvertStringToCulture(param.props.RetnArchive, param.DateFormat);
                    }
                    catch (Exception)
                    {
                        objSLDestructCertItem.ScheduledInactivity = default;
                    }

                    objSLDestructCertItem.RetentionCode = !string.IsNullOrEmpty(param.props.RetentionItemCode) ? param.props.RetentionItemCode : null;
                    objSLDestructCertItem.RetentionHold = Convert.ToBoolean(param.props.RetTableHolding[index].RetentionHold);
                    objSLDestructCertItem.LegalHold = Convert.ToBoolean(param.props.RetTableHolding[index].LegalHold);
                    objSLDestructCertItem.SnoozeUntil = param.props.RetTableHolding[index].SnoozeUntil;
                    objSLDestructCertItem.TableId = param.props.RetTableHolding[index].TableId;
                    objSLDestructCertItem.TableName = param.props.RetTableHolding[index].TableName;
                    objSLDestructCertItem.SLDestructionCertsId = Convert.ToInt32(param.props.RetTableHolding[index].SLDestructionCertsId);
                    objSLDestructCertItem.HoldReason = param.props.RetTableHolding[index].HoldReason;

                    if ((param.props.RetTableHolding[index].SnoozeUntil is null | (DateTime.Today is var arg16 && param.props.RetTableHolding[index].SnoozeUntil is { } arg15 ? arg15 > arg16 : (bool?)null)) == true)
                    {
                        context.SLDestructCertItems.Add(objSLDestructCertItem);
                    }
                }
                await context.SaveChangesAsync();
            }
        }
        private async Task DeleteDestructCertItem(RetentionInfoUpdateReqModel param)
        {
            using (var context = new TABFusionRMSContext(param.passport.ConnectionString))
            {
                var deleteRecord = await context.SLDestructCertItems.Where(x => (x.TableId ?? "") == (param.props.rowid ?? "") & (x.TableName ?? "") == (param.props.TableName ?? "")).ToListAsync();
                context.SLDestructCertItems.RemoveRange(deleteRecord);
            }

        }
        private async Task UpdateRetentionCodeInTableRecord(RetentionInfoUpdateReqModel param)
        {
            string query = "UPDATE [{0}] SET [{1}] = @retentionCode WHERE [{2}] = @tableID";
            using (var conn = new SqlConnection(param.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(string.Format(query, param.props.TableName, Navigation.GetRetentionFieldName(param.props.TableName, conn), Navigation.GetPrimaryKeyFieldName(param.props.TableName, conn)), conn))
                {
                    cmd.Parameters.AddWithValue("@retentionCode", param.props.RetentionItemCode);
                    cmd.Parameters.AddWithValue("@tableID", param.props.rowid);
                    cmd.ExecuteNonQuery();
                }
            }

        }
        private async Task RetentionArchiveinfo(RetentionInfo model, RetentionInfoUpdateReqModel req)
        {
            if (model.retentionItemRow is null)
                return;

            using (var context = new TABFusionRMSContext(req.passport.ConnectionString))
            {
                var objTables = await context.Tables.FirstOrDefaultAsync(x => (x.TableName ?? "") == (req.props.TableName ?? ""));
                if (objTables is not null)
                {
                    if (1 is var arg2 && objTables.RetentionFinalDisposition is { } arg1 && arg1 == arg2)
                    {
                        model.lblRetentionArchive = "Archive date:";
                    }
                    else if (2 is var arg4 && objTables.RetentionFinalDisposition is { } arg3 && arg3 == arg4)
                    {
                        model.lblRetentionArchive = "Destruction date:";
                    }
                    else if (3 is var arg6 && objTables.RetentionFinalDisposition is { } arg5 && arg5 == arg6)
                    {
                        model.lblRetentionArchive = "Purge date:";
                    }
                    else
                    {
                        model.lblRetentionArchive = "Archive date:";
                    }
                }
                else
                {
                    model.lblRetentionArchive = "Archive date:";
                }
            }
            // ceSnoozeDate.Format = Keys.GetCultureCookies().DateTimeFormat.ShortDatePattern
            // RetentionArchive = "Record Details"
        }
        private void SetRetentionStatus(DataRow tableinfo, DataRow codeRow, RetentionInfo model, RetentionInfoUpdateReqModel req)
        {
            if (!string.IsNullOrWhiteSpace(model.RetentionDescription))
            {
                if (Convert.ToBoolean(codeRow["RetentionLegalHold"]))
                {
                    model.RetentionStatus.text = "On Hold";
                }
                else
                {
                    model.RetentionStatus.text = "Retention Set";
                }
            }

            checkRetentionStatus(model, req);
        }
        private void checkRetentionStatus(RetentionInfo model, RetentionInfoUpdateReqModel req)
        {
            var destCert = Retention.GetDescCertRow(req.props.TableName, req.props.rowid, req.passport);

            if (destCert is not null)
            {
                bool disposed = false;
                var dispositiontype = meFinalDisposition.fdNone;
                bool isHold = Convert.ToBoolean(destCert["RetentionHold"]) | Convert.ToBoolean(destCert["LegalHold"]);
                if (destCert["DispositionDate"] is not null && !string.IsNullOrEmpty(destCert["DispositionDate"].ToString()))
                    disposed = true;
                if (destCert["DispositionType"] is not null)
                    dispositiontype = (meFinalDisposition)Convert.ToInt32(destCert["DispositionType"]);
                model.Disposed = disposed;
                model.DispositionType = (int)dispositiontype;

                if (isHold & dispositiontype == 0)
                {
                    model.RetentionStatus.text = "On Hold";
                }
                else if (dispositiontype == meFinalDisposition.fdDestruction)
                {
                    if (Convert.ToInt32(destCert["SLDestructionCertsId"]) == 0)
                    {
                        model.RetentionStatus.text = "Destroyed (parent disposed)";
                        model.RetentionStatus.color = "red";
                    }
                    else if (disposed)
                    {
                        var cert = Retention.GetDescCert(Convert.ToInt32(destCert["SLDestructionCertsId"]), req.passport);
                        // lblStatus.Text = "Destroyed [" + cert("Id").ToString + " - " + CDate(cert("DateCreated")).ToClientDateFormat + "]"
                        model.RetentionStatus.text = string.Format("Destroyed [{0}- {1}]", cert["Id"].ToString(), CommonFunctions.ToClientDateFormats(Convert.ToDateTime(cert["DateCreated"])));
                        model.RetentionStatus.color = "red";
                    }
                    else
                    {
                        var cert = Retention.GetDescCert(Convert.ToInt32(destCert["SLDestructionCertsId"]), req.passport);
                        // lblStatus.Text = "Eligible to be Destroyed [" + cert("Id").ToString + " - " + CDate(cert("DateCreated")).ToClientDateFormat + "]"
                        model.RetentionStatus.text = string.Format("Eligible to be Destroyed [{0}- {1}]", cert["Id"].ToString(), CommonFunctions.ToClientDateFormats(Convert.ToDateTime(cert["DateCreated"])));
                    }
                }
                else if (dispositiontype == meFinalDisposition.fdPermanentArchive)
                {
                    if (Convert.ToInt32(destCert["SLDestructionCertsId"]) == 0)
                    {
                        model.RetentionStatus.text = "Archived (parent disposed)";
                        model.RetentionStatus.color = "red";
                    }
                    else if (disposed)
                    {
                        var cert = Retention.GetDescCert(Convert.ToInt32(destCert["SLDestructionCertsId"]), req.passport);
                        // lblStatus.Text = "Archived [" + cert("Id").ToString + " - " + CDate(cert("DateCreated")).ToClientDateFormat + "]"
                        model.RetentionStatus.text = string.Format("Archived [{0}- {1}]", cert["Id"].ToString(), CommonFunctions.ToClientDateFormats(Convert.ToDateTime(cert["DateCreated"])));
                        model.RetentionStatus.color = "red";
                    }
                    else
                    {
                        var cert = Retention.GetDescCert(Convert.ToInt32(destCert["SLDestructionCertsId"]), req.passport);
                        model.RetentionStatus.text = string.Format("Eligible to be Archived [{0}- {1}]", cert["Id"].ToString(), CommonFunctions.ToClientDateFormats(Convert.ToDateTime(cert["DateCreated"])));
                    }
                }
                else
                {
                    try
                    {
                        var cert = Retention.GetDescCert(Convert.ToInt32(destCert["SLDestructionCertsId"]), req.passport);
                        dispositiontype = (meFinalDisposition)Convert.ToInt32(cert["RetentionDispositionType"]);

                        if (dispositiontype == meFinalDisposition.fdPermanentArchive)
                        {
                            model.RetentionStatus.text = string.Format("Eligible to be Archived [{0}- {1}]", cert["Id"].ToString(), CommonFunctions.ToClientDateFormats(Convert.ToDateTime(cert["DateCreated"])));
                        }
                        else if (dispositiontype == meFinalDisposition.fdDestruction)
                        {
                            model.RetentionStatus.text = string.Format("Eligible to be Destroyed [{0}- {1}]", cert["Id"].ToString(), CommonFunctions.ToClientDateFormats(Convert.ToDateTime(cert["DateCreated"])));
                        }
                        else if (dispositiontype == meFinalDisposition.fdPurge)
                        {
                            model.RetentionStatus.text = string.Format("Eligible for Purging [{0}- {1}]", cert["Id"].ToString(), CommonFunctions.ToClientDateFormats(Convert.ToDateTime(cert["DateCreated"])));
                        }
                        else
                        {
                            model.RetentionStatus.text = "Retention Set";
                        }
                    }
                    catch (Exception)
                    {
                        model.RetentionStatus.text = "Retention Set";
                    }
                }
            }
        }
        private void RetentionInactiveDate(DataRow tableInfo, DataRow codeRow, RetentionInfo model, RetentionInfoUpdateReqModel req)
        {
            meFinalDisposition dispositionType;

            if (!Convert.ToBoolean(tableInfo["RetentionInactivityActive"]))
            {
                model.RetentionInfoInactivityDate.text = "N/A";
                model.RetentionInfoInactivityDate.color = "black";
                return;
            }

            try
            {
                dispositionType = (meFinalDisposition)Convert.ToInt32(model.retentionItemRow["%slRetentionDispositionStatus"]);
            }
            catch (Exception)
            {
                dispositionType = meFinalDisposition.fdNone;
            }

            if (dispositionType != meFinalDisposition.fdNone)
            {
                model.RetentionInfoInactivityDate.text = "N/A";
                model.RetentionInfoInactivityDate.color = "black";
                return;
            }

            string dateField = string.Empty;

            switch (codeRow["InactivityEventType"].ToString() ?? "")
            {
                case "Date Opened":
                    {
                        dateField = tableInfo["RetentionDateOpenedField"].ToString();
                        break;
                    }
                case "Date Created":
                    {
                        dateField = tableInfo["RetentionDateCreateField"].ToString();
                        break;
                    }
                case "Date Closed":
                    {
                        dateField = tableInfo["RetentionDateClosedField"].ToString();
                        break;
                    }
                case "Date Other":
                    {
                        dateField = tableInfo["RetentionDateOtherField"].ToString();
                        break;
                    }
            }

            try
            {
                if (!string.IsNullOrEmpty(dateField))
                {
                    var dDispositionDate = Navigation.ApplyYearEndToDate(Convert.ToDateTime(model.retentionItemRow[dateField]), Convert.ToDouble(codeRow["InactivityPeriod"]), Convert.ToBoolean(codeRow["InactivityForceToEndOfYear"]), req.passport);
                    model.RetentionInfoInactivityDate.text = CommonFunctions.ToClientDateFormats(dDispositionDate);
                    model.RetentionInfoInactivityDate.color = Convert.ToString(Interaction.IIf(DateTime.Parse(model.RetentionInfoInactivityDate.text) > DateTime.Today, "black", "red"));
                }
                else
                {
                    model.RetentionInfoInactivityDate.text = "N/A";
                    model.RetentionInfoInactivityDate.color = "red";
                }
            }
            catch (Exception)
            {
                model.RetentionInfoInactivityDate.text = "NO DATE ENTERED";
                model.RetentionInfoInactivityDate.color = "red";
            }
        }
        private void RetentionArchiveDate(DataRow tableInfo, DataRow codeRow, RetentionInfo model, RetentionInfoUpdateReqModel req)
        {
            meFinalDisposition dispositionType;

            if (!Convert.ToBoolean(tableInfo["RetentionPeriodActive"]))
            {
                model.RetentionArchive.text = "N/A";
                model.RetentionArchive.color = "black";
                return;
            }

            try
            {
                dispositionType = (meFinalDisposition)Convert.ToInt32(model.retentionItemRow["%slRetentionDispositionStatus"]);
            }
            catch (Exception)
            {
                dispositionType = meFinalDisposition.fdNone;
            }

            if (dispositionType != meFinalDisposition.fdNone)
            {
                model.RetentionArchive.text = "N/A";
                model.RetentionArchive.color = "black";
                return;
            }

            string dateField = string.Empty;

            switch (codeRow["RetentionEventType"].ToString() ?? "")
            {
                case "Date Opened":
                    {
                        dateField = tableInfo["RetentionDateOpenedField"].ToString();
                        break;
                    }
                case "Date Created":
                    {
                        dateField = tableInfo["RetentionDateCreateField"].ToString();
                        break;
                    }
                case "Date Closed":
                    {
                        dateField = tableInfo["RetentionDateClosedField"].ToString();
                        break;
                    }
                case "Date Other":
                    {
                        dateField = tableInfo["RetentionDateOtherField"].ToString();
                        break;
                    }
            }

            try
            {
                if (!string.IsNullOrEmpty(dateField))
                {
                    var dDispositionDate = Navigation.ApplyYearEndToDate(Convert.ToDateTime(model.retentionItemRow[dateField]), Convert.ToDouble(codeRow["RetentionPeriodTotal"]), Convert.ToBoolean(codeRow["RetentionPeriodForceToEndOfYear"]), req.passport);
                    model.RetentionArchive.text = CommonFunctions.ToClientDateFormats(dDispositionDate);
                    model.RetentionArchive.color = Interaction.IIf(DateTime.Parse(Convert.ToString(model.RetentionArchive.text)) > DateTime.Today, "black", "red");
                }
                else
                {
                    model.RetentionArchive.text = "N/A";
                    model.RetentionArchive.color = "black";
                }
            }
            catch
            {
                model.RetentionArchive.text = "NO DATE ENTERED";
                model.RetentionArchive.color = "red";
            }
        }
        private void SetActiveItem(RetentionInfo model, Parameters @params, RetentionInfoUpdateReqModel req)
        {

            string query = string.Empty;
            if (Navigation.FieldIsAString(@params.TableInfo, @params.KeyField, req.passport))
            {
                query = string.Format("Select * from {0} where {1} = '{2}'", @params.TableName, @params.KeyField, req.props.rowid);
            }
            else
            {
                query = string.Format("Select * from {0} where {1} = {2}", @params.TableName, @params.KeyField, req.props.rowid);
            }

            DataSet loutput = new DataSet();
            using (SqlConnection connection = new SqlConnection(req.passport.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(loutput);
                    }
                }
            }

            model.retentionItemRow = loutput.Tables[0].Rows[0];
        }
        private async Task<PagingModel> GenerateRowsContentsCount(ReportingReqModel props)
        {
            var dt = await Tracking.GetContainerContentsCountAsync(props.paramss.tableName, props.paramss.Tableid, props.passport);
            if (dt.Rows.Count > 0)
            {
                return Execute(Convert.ToInt32(dt.Rows[0][0]), 100);
            }
            else
            {
                return Execute(0, 100);
            }
        }
        private async Task<PagingModel> GenerateRowsTrackingCount(ReportingReqModel props)
        {
            var model = new TrackingReport(props.passport, props.paramss.tableName, props.paramss.Tableid);
            var list = await model.GetTrackableHistoryCountAsync();
            if (list.Rows.Count > 0)
            {
                return Execute(Convert.ToInt32(list.Rows[0][0]), 100);
            }
            else
            {
                return Execute(0, 100);
            }
        }
        private async Task<PagingModel> GenerateRowsAuditCount(ReportingReqModel props)
        {
            // rows
            var data = await sqlQueryPagingCount(props);
            if (data.Rows.Count > 0)
            {
                return Execute(Convert.ToInt32(data.Rows[0][0]), 100);
            }
            else
            {
                return Execute(0, 100);
            }
        }
        private PagingModel Execute(int totalRecord, int recordePerpage)
        {
            var model = new PagingModel();
            model.PerPageRecord = recordePerpage;
            model.TotalRecord = totalRecord;
            if (model.TotalRecord > 0)
            {
                if (model.TotalRecord / (double)model.PerPageRecord > 0d & model.TotalRecord / (double)model.PerPageRecord < 1d)
                {
                    model.TotalPage = 1;
                }
                else if (model.TotalRecord % model.PerPageRecord == 0)
                {
                    model.TotalPage = (int)Math.Round(model.TotalRecord / (double)model.PerPageRecord);
                }
                else
                {
                    int tp = (int)Math.Round(Conversion.Int(model.TotalRecord / (double)model.PerPageRecord));
                    model.TotalPage = tp + 1;
                }
            }
            return model;
        }
        private async Task<DataTable> sqlQueryPagingCount(ReportingReqModel props)
        {
            string tableid = Navigation.PrepPad(props.paramss.tableName, props.paramss.Tableid, props.passport);
            // sql query
            string sql = string.Format(string.Format("SELECT count(*) FROM [SLAuditUpdates]" + " WHERE TableName = '{0}' AND TableId = '{1}' ", props.paramss.tableName, tableid));

            var data = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(data);
                    }
                }
            }
            return data;
        }
        private void GenerateContentPerRow(int offsetValue, ReportingPerRow report, ReportingReqModel props)
        {
            report.hasPermission = true;
            GenerateHeaderContent(report);
            GenerateRows(report, props);
        }
        private void GenerateHeaderContent(ReportingPerRow report)
        {
            report.ListOfHeader.Add("Tran Date");
            report.ListOfHeader.Add("Item Name");
            report.ListOfHeader.Add("UserName");
        }
        private void GenerateRows(ReportingPerRow report, ReportingReqModel props)
        {
            var dt = Tracking.GetContainerContentsPaging(props.paramss.tableName, props.paramss.Tableid, props.passport, report.Paging.PageNumber, report.Paging.PerPageRecord);
            var idsByTable = new Dictionary<string, string>();
            var permissionsByTable = new Dictionary<string, bool>();
            var listOfTableNames = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                if (!idsByTable.ContainsKey(row["TrackedTable"].ToString()))
                {
                    idsByTable.Add(row["TrackedTable"].ToString(), "");
                    listOfTableNames.Add(row["TrackedTable"].ToString());
                    permissionsByTable.Add(row["TrackedTable"].ToString(), props.passport.CheckPermission(Convert.ToString(row["TrackedTable"]), SecureObject.SecureObjectType.Table, Permissions.Permission.View));
                }
                if (string.IsNullOrEmpty(idsByTable[row["TrackedTable"].ToString()]))
                {
                    idsByTable[row["TrackedTable"].ToString()] += "'" + row["TrackedTableID"].ToString() + "'";
                }
                else
                {
                    idsByTable[row["TrackedTable"].ToString()] += ",'" + row["TrackedTableID"].ToString().Replace("'", "''") + "'";
                }
            }

            var tablesInfo = Navigation.GetMultipleTableInfo(listOfTableNames, props.passport);
            var descriptions = new Dictionary<string, DataTable>();

            foreach (DataRow tableInfo in tablesInfo.Rows)
            {
                string ids = string.Empty;
                if (idsByTable.TryGetValue(tableInfo["TableName"].ToString(), out ids)) // If we have ids, prepopulate; otherwise it'll have to get each one individually
                {
                    descriptions.Add(tableInfo["TableName"].ToString(), Navigation.GetItemNames(tableInfo["TableName"].ToString(), props.passport, tableInfo, ids));
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                bool permission = false;
                if (!permissionsByTable.TryGetValue(row["TrackedTable"].ToString(), out permission))
                {
                    permission = props.passport.CheckPermission(Convert.ToString(row["TrackedTable"]), SecureObject.SecureObjectType.Table, Permissions.Permission.View);
                }
                if (permission)
                {
                    var cell = new List<string>();
                    cell.Add(DateTime.Parse(row["TransactionDateTime"].ToString()).ToString(props.DateFormat));
                    cell.Add(Navigation.ExtractItemName(row["TrackedTable"].ToString(), row["TrackedTableID"].ToString(), descriptions, tablesInfo, props.passport));
                    cell.Add(Convert.ToString(row["UserName"]).ToString());
                    report.ListOfRows.Add(cell);
                }
            }
        }
        private async Task GenerateTrackingHistoryPerRow(int offsetValue, ReportingPerRow report, ReportingReqModel props)
        {

            if (!props.passport.CheckSetting(props.paramss.tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
            {
                report.Msg = "the tracking history report";
                report.hasPermission = false;
                return;
            }
            else
            {
                report.hasPermission = true;
            }
            GenerateHeaderTrackingHistory(report, props);
            await GenerateRowsTrackingHistory(report, props);
        }
        private void GenerateHeaderTrackingHistory(ReportingPerRow report, ReportingReqModel props)
        {
            //IRepository<TabFusionRMS.Models.System> _iSystem = new Repositories<TabFusionRMS.Models.System>();
            //var pSystemEntity = _iSystem.All().OrderBy(x => x.Id).FirstOrDefault();
            report.ListOfHeader.Add("Transaction Date");
            report.ListOfHeader.Add("Location");
            report.ListOfHeader.Add("Scan Operator");
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                var pSystemEntity = context.Systems.OrderBy(x => x.Id).FirstOrDefault();
                if (pSystemEntity.TrackingAdditionalField1Desc != null)
                    report.ListOfHeader.Add(pSystemEntity.TrackingAdditionalField1Desc);
                if (pSystemEntity.TrackingAdditionalField2Desc != null)
                    report.ListOfHeader.Add(pSystemEntity.TrackingAdditionalField2Desc);
            }
        }
        private async Task GenerateRowsTrackingHistory(ReportingPerRow report, ReportingReqModel props)
        {
            var model = new TrackingReport(props.passport, props.paramss.tableName, props.paramss.Tableid);
            var list = model.GetTrackableHistoryPaging(report.Paging.PageNumber, report.Paging.PerPageRecord);
            if (list is null)
                return;

            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {

                var pSystemEntity = await context.Systems.OrderBy(x => x.Id).FirstOrDefaultAsync();
                for (int i = 0, loopTo = list.Count - 1; i <= loopTo; i++)
                {
                    var cell = new List<string>();
                    var sb = new StringBuilder();
                    cell.Add(Convert.ToString(list[i].TransactionDate));
                    for (int j = 0, loopTo1 = list[i].Containers.Count - 1; j <= loopTo1; j++)
                        sb.AppendLine(list[i].Containers[j].Type + ":       " + list[i].Containers[j].Name);
                    cell.Add(sb.ToString());
                    cell.Add(list[i].UserName);
                    if (pSystemEntity.TrackingAdditionalField1Desc != null)
                        cell.Add(list[i].TrackingAdditionalField1);
                    if (pSystemEntity.TrackingAdditionalField2Desc != null)
                        cell.Add(list[i].TrackingAdditionalField2);

                    report.ListOfRows.Add(cell);
                }
            }
        }
        private async Task GenerateAuditHistoryPerRow(int offsetValue, ReportingPerRow report, ReportingReqModel props)
        {
            if (!props.passport.CheckPermission(" Auditing", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                report.Msg = "the audit history report";
                report.hasPermission = false;
                return;
            }
            else
            {
                report.hasPermission = true;
            }
            // headers
            GenerateHeaderAuditHistory(report);
            await GenerateRowsAuditHistory(offsetValue, report, props);
        }
        private void GenerateHeaderAuditHistory(ReportingPerRow report)
        {
            report.ListOfHeader.Add("Date");
            report.ListOfHeader.Add("User");
            report.ListOfHeader.Add("Network Login");
            report.ListOfHeader.Add("Domain");
            report.ListOfHeader.Add("ComputerName");
            report.ListOfHeader.Add("MacAddress");
            report.ListOfHeader.Add("IP");
            report.ListOfHeader.Add("Action");
            report.ListOfHeader.Add("DataBefore");
            report.ListOfHeader.Add("DataAfter");
        }
        private async Task GenerateRowsAuditHistory(int offsetValue, ReportingPerRow report, ReportingReqModel props)
        {
            // rows
            var data = await sqlQueryPaging(offsetValue, report, props);
            foreach (DataRow row in data.Rows)
            {
                var cell = new List<string>();
                foreach (DataColumn col in data.Columns)
                    cell.Add(row[col.Caption].ToString());
                report.ListOfRows.Add(cell);
            }
        }
        private async Task<DataTable> sqlQueryPaging(int offsetValue, ReportingPerRow report, ReportingReqModel props)
        {
            string tableid = Navigation.PrepPad(props.paramss.tableName, props.paramss.Tableid, props.passport);
            // sql query
            string sql = string.Format(string.Format("SELECT CONVERT(VARCHAR, [UpdateDateTime], 100) AS 'Date', [OperatorsId] AS 'User', [NetworkLoginName] AS 'Network Login'," + " [Domain], [ComputerName], [MacAddress], [IP], [Action], [DataBefore], [DataAfter] FROM [SLAuditUpdates]" + " WHERE TableName = '{0}' AND TableId = '{1}' ORDER BY UpdateDateTime DESC ", props.paramss.tableName, tableid));

            sql += Query.QueryPaging(report.Paging.PageNumber, report.Paging.PerPageRecord);

            var data = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(data);
                    }
                }
            }
            return data;
        }
        private void LinkScriptRunBeforeDelete(ScriptReturn result, Parameters param, string tableId, Deleterows model, DeleteRowsFromGridReqModel req)
        {
            result = ScriptEngine.RunScriptBeforeDelete(param.TableName, tableId, req.passport);
            model.scriptReturn.Successful = result.Successful;
            model.scriptReturn.GridRefresh = result.GridRefresh;
            model.scriptReturn.ReturnMessage = result.ReturnMessage;
            // check if there is a script and if script is not finished
            // check if script doesn't need any user interaction.
            if (result.Engine is not null)
            {
                model.LinkScriptSession = result.Engine;
                if (result.Engine.ShowPromptBool)
                {
                    model.scriptReturn.isBeforeDeleteLinkScript = true;
                    model.scriptReturn.ScriptName = result.Engine.ScriptName;
                }
                else
                {
                    model.scriptReturn.isBeforeDeleteLinkScript = false;
                    model.scriptReturn.ScriptName = "";
                }
            }
            else
            {
                model.scriptReturn.isBeforeDeleteLinkScript = false;
                model.scriptReturn.ScriptName = "";
            }
            if (!model.scriptReturn.Successful)
                return;
        }
        private void LinkScriptRunAfterDelete(ScriptReturn result, Parameters param, Deleterows model, DeleteRowsFromGridReqModel req)
        {
            result = ScriptEngine.RunScriptAfterDelete(param.TableName, param.KeyValue, req.passport);
            model.scriptReturn.Successful = result.Successful;
            model.scriptReturn.GridRefresh = result.GridRefresh;
            model.scriptReturn.ReturnMessage = result.ReturnMessage;
            if (result.Engine is not null)
            {
                model.LinkScriptSession = result.Engine;
                if (result.Engine.ShowPromptBool)
                {
                    model.scriptReturn.isAfterDeleteLinkScript = true;
                    model.scriptReturn.ScriptName = result.Engine.ScriptName;
                }
            }
        }
        private async Task AddNewRow(DatabaseChangesReq req, Saverows model)
        {
            // Dim _query = New Query(_passport)
            var param = new Parameters(req.paramss.ViewId, req.passport);
            model.TableName = param.TableName;
            if (req.passport.CheckPermission(param.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.Add))
            {
                param.Scope = ScopeEnum.Table;
                param.NewRecord = true;
                param.AfterData = req.paramss.AfterChange;
                model.IsNewRecord = true;
                param.RequestedRows = 1;
                ScriptReturn result = null;
                if (req.paramss.scriptDone == false)
                {
                    LinkScriptRunBeforeAdd(result, model, req);
                    if (model.scriptReturn.isBeforeAddLinkScript)
                    {
                        return;
                    }
                }
                else
                {
                    result = new ScriptReturn(true, "", 0.ToString(), false);
                }

                Query.Save(param, "", param.KeyField, "", DataFieldValues(req), req.passport, result);

                if (string.IsNullOrWhiteSpace(model.keyvalue) && !string.IsNullOrWhiteSpace(param.KeyValue))
                {
                    param.AfterData += string.Format("{1}: {2}{0}", Environment.NewLine, param.KeyField, param.KeyValue);
                }

                model.keyvalue = param.KeyValue;
                // save audit
                {
                    var withBlock = AuditType.WebAccess;
                    withBlock.TableName = param.TableName;
                    withBlock.TableId = param.KeyValue;
                    withBlock.ClientIpAddress = "00:00";
                    withBlock.ActionType = AuditType.WebAccessActionType.AddRecord;
                    withBlock.AfterData = param.AfterDataTrimmed;
                    withBlock.BeforeData = string.Empty;
                }

                Auditing.AuditUpdates(AuditType.WebAccess, req.passport);
                // linkscript After
                LinkScriptRunAfterAdd(result, model, req);

                string retentionCode = Query.SetRetentionCode(param.TableName, param.TableInfo, param.KeyValue, req.passport);
                DataRow row = Navigation.GetSingleRow(param.TableInfo, param.KeyValue, param.KeyField, req.passport);
                Tracking.SetRetentionInactiveFlag(param.TableInfo, row, retentionCode, req.passport);

                model.gridDatabinding = await GetLastRowadded(param, req);
                model.Msg = "success";
            }
            // Me.ListOfDatarows = returnValueonrow.ListOfDatarows
            // Me.ToolBarHtml = returnValueonrow.ToolBarHtml
            // Me.ListOfHeaders = returnValueonrow.ListOfHeaders
            // Me.HasDrillDowncolumn = returnValueonrow.HasDrillDowncolumn
            else
            {
                model.Msg = "You do not have sufficient Permission to Add";
                model.isError = true;
            }
        }
        private async Task EditRow(DatabaseChangesReq req, Saverows model)
        {
            // Dim _query = New Query(_passport)
            var param = new Parameters(req.paramss.ViewId, req.passport);
            model.TableName = param.TableName;
            if (req.passport.CheckPermission(param.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.Edit))
            {
                param.Scope = ScopeEnum.Table;
                param.KeyValue = req.Rowdata[req.Rowdata.Count - 1].value;
                param.NewRecord = false;
                param.BeforeData = req.paramss.BeforeChange;
                param.AfterData = req.paramss.AfterChange;
                model.IsNewRecord = false;
                model.keyvalue = req.Rowdata[req.Rowdata.Count - 1].value;
                //param.Culture = Keys.GetCultureCookies(_httpContext);

                // linkscript before
                ScriptReturn result = null;
                if (model.scriptDone == false)
                {
                    LinkScriptRunBeforeEdit(result, model, req);
                    if (model.scriptReturn.isBeforeEditLinkScript)
                    {
                        return;
                    }
                }
                else
                {
                    result = new ScriptReturn(true, "", 0.ToString(), false);
                }
                // save row
                Query.Save(param, "", param.KeyField, param.KeyValue, DataFieldValues(req), req.passport, result);
                model.keyvalue = param.KeyValue;
                // save audit
                {
                    var withBlock = AuditType.WebAccess;
                    withBlock.TableName = param.TableName;
                    withBlock.TableId = param.KeyValue;
                    withBlock.ClientIpAddress = "00:00";
                    withBlock.ActionType = AuditType.WebAccessActionType.UpdateRecord;
                    withBlock.AfterData = param.AfterDataTrimmed;
                    withBlock.BeforeData = param.BeforeDataTrimmed;
                }

                Auditing.AuditUpdates(AuditType.WebAccess, req.passport);

                LinkScriptRunAfterEdit(result, model, req);

                string retentionCode = Query.SetRetentionCode(param.TableName, param.TableInfo, param.KeyValue, req.passport);
                DataRow row = Navigation.GetSingleRow(param.TableInfo, param.KeyValue, param.KeyField, req.passport);
                Tracking.SetRetentionInactiveFlag(param.TableInfo, row, retentionCode, req.passport);
                model.gridDatabinding = await GetLastRowadded(param, req);
            }
            else
            {
                model.Msg = "You do not have sufficient Permission to Edit";
                model.isError = true;
                return;
            }
            model.Msg = "success";
        }
        private void LinkScriptRunBeforeEdit(ScriptReturn result, Saverows model, DatabaseChangesReq req)
        {
            result = ScriptEngine.RunScriptBeforeEdit(model.TableName, model.keyvalue, req.passport);
            model.scriptReturn.Successful = result.Successful;
            model.scriptReturn.GridRefresh = result.GridRefresh;
            model.scriptReturn.ReturnMessage = result.ReturnMessage;
            // check if there is a script and if script is not finished
            // check if script doesn't need any user interaction.
            if (result.Engine is not null)
            {
                model.LinkScriptSession = result.Engine;
                if (result.Engine.ShowPromptBool)
                {
                    model.scriptReturn.isBeforeEditLinkScript = true;
                    model.scriptReturn.ScriptName = result.Engine.ScriptName;
                }
                else
                {
                    model.scriptReturn.isBeforeEditLinkScript = false;
                    model.scriptReturn.ScriptName = "";
                }
            }
            else
            {
                model.scriptReturn.isBeforeEditLinkScript = false;
                model.scriptReturn.ScriptName = "";
            }
            if (!model.scriptReturn.Successful)
                return;

        }
        private void LinkScriptRunAfterEdit(ScriptReturn result, Saverows model, DatabaseChangesReq req)
        {
            result = ScriptEngine.RunScriptAfterEdit(model.TableName, model.keyvalue, req.passport);
            model.scriptReturn.Successful = result.Successful;
            model.scriptReturn.GridRefresh = result.GridRefresh;
            model.scriptReturn.ReturnMessage = result.ReturnMessage;
            if (result.Engine is not null)
            {
                model.LinkScriptSession = result.Engine;
                if (result.Engine.ShowPromptBool)
                {
                    model.scriptReturn.isAfterEditLinkScript = true;
                    model.scriptReturn.ScriptName = result.Engine.ScriptName;
                }
            }
        }
        private async Task<GridDataBinding> GetLastRowadded(Parameters param, DatabaseChangesReq req)
        {
            //GridDataBinding q = new GridDataBinding();
            var _query = new Query(req.passport);
            //var q = new GridDataBinding(_passport, _viewId, 1, crumbLevel, (int)ViewType.FusionView, childkeyfield, _httpContext);
            var model = new GridDataBinding();
            model.IsWhereClauseRequest = true;
            if (Navigation.IsAStringType(param.IdFieldDataType) || Navigation.IsADateType(param.IdFieldDataType))
            {
                model.WhereClauseStr = string.Format("SELECT [{0}] FROM [{1}] where [{0}] = '{2}'", Navigation.MakeSimpleField(param.KeyField), param.TableName, param.KeyValue.Replace("'", "''"));
            }
            else
            {
                model.WhereClauseStr = string.Format("SELECT [{0}] FROM [{1}] where [{0}] = {2}", Navigation.MakeSimpleField(param.KeyField), param.TableName, param.KeyValue);
            }

            var q = new SearchQueryRequestModal();
            q.passport = req.passport;
            q.paramss.ViewId = req.paramss.ViewId;
            q.paramss.ChildKeyField = req.paramss.childkeyfield;
            q.DateFormat = req.DateFormat;
            await BuildNewTableData(q, model);

            if (model.ListOfDatarows.Count == 0)
            {
                return model;
            }
            int counter = 0;
            foreach (TableHeadersProperty f in model.ListOfHeaders)
            {
                string value = model.ListOfDatarows[0][counter];
                if (f.DataTypeFullName == "System.DateTime" && !string.IsNullOrWhiteSpace(value))
                {
                    model.ListOfDatarows[0][counter] = DateTime.Parse(value.ToString()).ToString(req.DateFormat);
                }
                counter = counter + 1;
            }

            return model;
        }
        private void LinkScriptRunBeforeAdd(ScriptReturn result, Saverows model, DatabaseChangesReq req)
        {
            result = ScriptEngine.RunScriptBeforeAdd(model.TableName, req.passport);
            model.scriptReturn.Successful = result.Successful;
            model.scriptReturn.GridRefresh = result.GridRefresh;
            model.scriptReturn.ReturnMessage = result.ReturnMessage;
            // check if there is a script and if script is not finished
            // check if script doesn't need any user interaction.
            if (result.Engine is not null)
            {
                model.LinkScriptSession = result.Engine;
                if (result.Engine.ShowPromptBool)
                {
                    model.scriptReturn.isBeforeAddLinkScript = true;
                    model.scriptReturn.ScriptName = result.Engine.ScriptName;
                }
                else
                {
                    model.scriptReturn.isBeforeAddLinkScript = false;
                    model.scriptReturn.ScriptName = "";
                }
            }

            if (!model.scriptReturn.Successful)
                return;
        }
        private void LinkScriptRunAfterAdd(ScriptReturn result, Saverows model, DatabaseChangesReq req)
        {
            result = ScriptEngine.RunScriptAfterAdd(model.TableName, model.keyvalue, req.passport);
            model.scriptReturn.Successful = result.Successful;
            model.scriptReturn.GridRefresh = result.GridRefresh;
            model.scriptReturn.ReturnMessage = result.ReturnMessage;
            model.scriptReturn.keyValue = model.keyvalue;
            if (result.Engine is not null)
            {
                model.LinkScriptSession = result.Engine;
                if (result.Engine.ShowPromptBool)
                {
                    model.scriptReturn.isAfterAddLinkScript = true;
                    model.scriptReturn.ScriptName = result.Engine.ScriptName;
                }
            }
        }
        private List<FieldValue> DataFieldValues(DatabaseChangesReq p)
        {
            var lst = new List<FieldValue>();
            foreach (var row in p.Rowdata)
            {
                if (!string.IsNullOrEmpty(row.columnName) && !string.IsNullOrEmpty(row.DataTypeFullName))
                {
                    // If param.KeyField = row.columnName Then
                    // param.KeyValue = row.value
                    // End If
                    var field = new FieldValue(row.columnName, row.DataTypeFullName);
                    if (row.value is null)
                    {
                        field.value = "";
                    }
                    else if (row.DataTypeFullName == "System.DateTime")
                    {
                        field.value = DateTime.Parse(row.value.ToString()).ToString(p.DateFormat);
                    }
                    else
                    {
                        field.value = row.value;

                    }
                    lst.Add(field);
                }
            }
            return lst;
        }
        private void CheckBoxesconditions(dynamic @params, GlobalSearchReqModel req)
        {
            if (req.paramss.ChkUnderRow)
            {
                @params.Scope = ScopeEnum.Node;
                if (@params.TableName is not null && !string.IsNullOrEmpty(@params.TableName))
                {
                    @params.ChangeViewID(@params.ViewId);
                }
                @params.CursorValue = req.paramss.Currentrow;
            }
            else if (req.paramss.ChkcurTable)
            {
                @params.Scope = ScopeEnum.Table;
                if (@params.TableName is not null && !string.IsNullOrEmpty(@params.TableName))
                {
                    @params.ChangeViewID(Navigation.GetTableFirstSearchableViewId(@params.TableName, req.passport));
                }
            }
            else
            {
                @params.Scope = ScopeEnum.Database;
            }
        }
        private async Task<int> SaveSavedCriteria(int userId, string pErrorMessage, string FavouriteName, int pViewId, string ConnectionString)
        {
            s_SavedCriteria ps_SavedCriteria = new s_SavedCriteria();
            using (var context = new TABFusionRMSContext(ConnectionString))
            {

                ps_SavedCriteria.UserId = userId;
                ps_SavedCriteria.SavedName = FavouriteName;
                ps_SavedCriteria.SavedType = (int)Enums.SavedType.Favorite;
                ps_SavedCriteria.ViewId = pViewId;
                context.s_SavedCriteria.Add(ps_SavedCriteria);
                await context.SaveChangesAsync();
            }

            return ps_SavedCriteria.Id;
        }
        private async Task<bool> SaveSavedChildrenFavourite(string pErrorMessage, bool isNewRecord, int ps_SavedCriteriaId, int pViewId, List<string> lSelectedItemList, string ConnectionString)
        {
            var IsSuccess = false;

            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                List<s_SavedChildrenFavorite> ls_SavedChildrenFavorite = new List<s_SavedChildrenFavorite>();
                var Lists_SavedChildrenFavorite = await context.s_SavedChildrenFavorite.ToListAsync();
                var Lists_SavedCriteria = await context.s_SavedCriteria.ToListAsync();

                var finalOutPut = from child in Lists_SavedChildrenFavorite
                                  join par in Lists_SavedCriteria
                                  on child.SavedCriteriaId equals par.Id
                                  where par.Id == Convert.ToInt32(child.SavedCriteriaId)
                                  select new { par.ViewId, child.TableId, par.Id };

                foreach (string tableId in lSelectedItemList)
                {
                    if (isNewRecord | !(finalOutPut.Any(x => x.TableId == tableId && x.ViewId == pViewId && x.Id == ps_SavedCriteriaId)))
                    {
                        s_SavedChildrenFavorite ps_SavedChildrenFavorite = new s_SavedChildrenFavorite();
                        ps_SavedChildrenFavorite.SavedCriteriaId = ps_SavedCriteriaId;
                        ps_SavedChildrenFavorite.TableId = tableId;
                        ls_SavedChildrenFavorite.Add(ps_SavedChildrenFavorite);
                        await context.SaveChangesAsync();
                    }
                }

                context.s_SavedChildrenFavorite.AddRange(ls_SavedChildrenFavorite);
                await context.SaveChangesAsync();

                IsSuccess = true;
            }

            return IsSuccess;
        }
        private void GetTrackingInfo(TrackingModeld model, trackableUiParams props)
        {
            var @params = new Parameters(props.ViewId, props.passport);
            Dictionary<string, string> argidsByTable = null;
            var tracks = Tracking.GetTrackableStatus(Navigation.GetViewTableName(props.ViewId, props.passport), props.RowKeyid.ToString(), props.passport, idsByTable: ref argidsByTable);
            if (tracks is null | @params.NewRecord)
            {
                model.lblTracking = "Never Tracked";
            }
            else
            {
                model.lblTrackTime = string.Format("{0} by {1}", tracks[0].TransactionDate.ToString(props.DateFormat), tracks[0].UserName.ToString());
                foreach (Container cont in tracks[0].Containers)
                {
                    model.lblTracking += Navigation.GetItemName(cont.Type, cont.ID.ToString(), props.passport, true) + "<br>";
                    switch (cont.OutType)
                    {
                        case 0:
                            {
                                string trackingField = Navigation.GetTableInfo(cont.Type, props.passport)["TrackingOutFieldName"].ToString();
                                if (!string.IsNullOrEmpty(trackingField) && Convert.ToBoolean(Navigation.GetSingleFieldValue(cont.Type, cont.ID, trackingField, props.passport)[0]))
                                {
                                    model.lblDueBack = "Out".ToUpper();
                                    if (Convert.ToBoolean(Navigation.GetSystemSetting("DateDueOn", props.passport)))
                                    {
                                        if (!tracks[0].DateDue.Equals(new DateTime()))
                                            model.lblDueBack = "";
                                        model.lblDueBack += string.Format("{0} Due back on {1}", " - ", tracks[0].DateDue.ToString(props.DateFormat));
                                    }
                                }
                                else
                                {
                                    model.lblDueBack = "In".ToUpper();
                                }

                                break;
                            }
                        case 1:
                            {
                                model.lblDueBack = "Out".ToUpper() + " ";
                                if (Convert.ToBoolean(Navigation.GetSystemSetting("DateDueOn", props.passport)))
                                {
                                    if (!tracks[0].DateDue.Equals(new DateTime()))
                                        model.lblDueBack += string.Format("{0} Due back on {1}", " - ", tracks[0].DateDue.ToString(props.DateFormat));
                                }

                                break;
                            }
                        case 2:
                            {
                                model.lblDueBack = "In".ToUpper();
                                break;
                            }
                    }
                }
            }
        }
        private void GetRequestWaitlist(TrackingModeld model, trackableUiParams props)
        {
            var requests = new Requesting();
            string trckid = props.RowKeyid.ToString();
            foreach (Request req in requests.GetActiveRequests(props.ViewId, trckid, props.passport))
            {
                var rmodel = new Requestlist();
                rmodel.DateRequested = req.DateRequested.ToString(props.DateFormat);
                rmodel.EmployeeName = req.Name;
                rmodel.DateNeeded = req.DateNeeded.ToString(props.DateFormat);
                rmodel.Status = req.Status;
                rmodel.reqid = req.RequestID.ToString();
                model.ListofRequests.Add(rmodel);

                if (props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.Configure))
                {
                    model.isDeleteAllow = true;
                }
            }

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
                                string checkFieldDateStart = DateTime.Parse(dt[0].ToString()).ToString(props.DateFormat);
                                string checkFieldDateEnd = DateTime.Parse(dt[1].ToString()).ToString(props.DateFormat);
                                fv.value = string.Format("{0}|{1}", checkFieldDateStart, checkFieldDateEnd);
                            }
                            else
                            {
                                fv.value = DateTime.Parse(row.values.ToString()).ToString(props.DateFormat);
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
        private bool ShowColumn(DataColumn col, int crumblevel, string parentField)
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
