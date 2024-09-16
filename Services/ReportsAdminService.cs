using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Services.Interface;
using Smead.Security;

namespace MSRecordsEngine.Services
{

    public class ReportsAdminService : IReportAAdminService
    {
        public static Dictionary<string, string> mcFieldName = new Dictionary<string, string>();
        public static Dictionary<string, RelationShip> mcRelationships = new Dictionary<string, RelationShip>();
        public static Dictionary<string, int> mcLevel = new Dictionary<string, int>();
        public const string TRACKED_LOCATION_NAME = "SLTrackedDestination";

        public string GetBindReportsMenus(string root, List<Table> lTableEntities, List<View> lViewEntities,
            List<ReportStyle> lReportStyleEntities, Passport _passport, int iCntRpt)
        {
            string strALId = "";
            StringBuilder strViewMenu = new StringBuilder();


            bool mbMgrGroup = _passport.CheckAdminPermission((Permissions.Permission)Enums.PassportPermissions.Access);

            if (mbMgrGroup | iCntRpt > 0)
            {

                strALId = "ALRPT_1";
                strViewMenu.Append("<li>");

                strViewMenu.Append(string.Format("<a href='#' id='{0}' onclick=ReportRootItemClick('{1}') class='ReportDefinitions'>{2}</a>", strALId.ToString().Trim(), strALId.ToString().Trim(), "Report Definitions"));

                strViewMenu.Append("<ul>");
                foreach (var oTable in lTableEntities)
                {
                    if (!CollectionsClass.IsEngineTable(oTable.TableName))
                    {
                        if (_passport.CheckPermission(oTable.TableName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Table, (Permissions.Permission)Enums.PassportPermissions.View))
                        {
                            var lTableViewList = lViewEntities.Where(x => (x.TableName.Trim().ToLower() ?? "") == (oTable.TableName.Trim().ToLower() ?? ""));
                            foreach (var oView in lTableViewList)
                            {
                                if ((bool)oView.Printable)
                                {
                                    if (_passport.CheckPermission(oView.ViewName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Reports, (Permissions.Permission)Enums.PassportPermissions.Configure))
                                    {
                                        if (_passport.CheckPermission(oView.ViewName, (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Reports, (Permissions.Permission)Enums.PassportPermissions.View))
                                        {
                                            bool pCheckSubViewExist = true;
                                            if (oView.SubViewId is not null)
                                            {
                                                var oCheckSubViewExist = lViewEntities.Where(x => x.SubViewId == oView.Id).FirstOrDefault();
                                                if (oCheckSubViewExist is not null)
                                                {
                                                    pCheckSubViewExist = false;
                                                }
                                            }

                                            if (pCheckSubViewExist)
                                            {
                                                strViewMenu.Append(string.Format("<li><a id='FALRPT_{0}' onclick=ReportChildItemClick('{2}','{3}','FALRPT_{0}')>{1} ({4})</a></li>", oView.Id.ToString().Trim(), oView.ViewName, root, strALId, oTable.UserName));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                strViewMenu.Append("</ul></li>");
            }
            // '''''''''''''''''''''''''''''''''''''''''
            // Load the Report Styles
            // '''''''''''''''''''''''''''''''''''''''''
            if (_passport.CheckPermission("Report Styles", (Smead.Security.SecureObject.SecureObjectType)Enums.SecureObjects.Application, (Permissions.Permission)Enums.PassportPermissions.Access))
            {
                // Dim ReportStypeNode = New JSTreeView.ListItem("Report Styles", Guid.NewGuid.ToString() + "_rootReportStyle_-2", className:="jstree-open", dataJsTree:=dataJsTree)
                // rootNode.Nodes.Add(ReportStypeNode)
                strALId = "ALRPTSTL_2";
                strViewMenu.Append("<li>");
                strViewMenu.Append(string.Format("<a href='#' id='{0}' onclick=ReportStyleRootItemClick('{1}') class='ReportStyles'>{2}</a>", strALId.ToString().Trim(), strALId.ToString().Trim(), "Report Styles"));
                strViewMenu.Append("<ul>");

                foreach (var oReportStyles in lReportStyleEntities)
                    // Dim ChildReportStypeNode = New JSTreeView.ListItem(oReportStyles.Id, Guid.NewGuid.ToString() + "_childReportStyle_" + oReportStyles.ReportStylesId.ToString(), className:="jstree-open", dataJsTree:=dataJsTree)
                    // ReportStypeNode.Nodes.Add(ChildReportStypeNode)
                    strViewMenu.Append(string.Format("<li><a id='FALRPTSTL_{0}' onclick=ReportStyleChildItemClick('{2}','{3}','FALRPTSTL_{0}')>{1}</a></li>", oReportStyles.ReportStylesId.ToString().Trim(), oReportStyles.Id, root, strALId));
                strViewMenu.Append("</ul></li>");
            }
            // End If

            // treeNode.ListItems.Add(rootNode)
            // treeView.Nodes.Add(treeNode)
            return strViewMenu.ToString();
        }


        public async Task<RtnFillViewColField> FillViewColField(List<Table> tableObjList, List<RelationShip> relationObjList, List<KeyValuePair<string, string>> FieldNameList, Table orgTable, List<RelationShip> relationShipEntity, bool bDoUpper, int iLevel, bool bNumericOnly, string connectionString)
        {
            var model = new RtnFillViewColField();
            try
            {
                Table tableEntity;
                List<SchemaColumns> schemaColumn = new List<SchemaColumns>();
                string sFieldName = "";

                if (relationShipEntity != null)
                {
                    if (iLevel == 1)
                    {
                        mcFieldName.Clear();
                        mcRelationships.Clear();
                        mcLevel.Clear();
                    }

                    foreach (RelationShip relationObj in relationShipEntity)
                    {
                        if (bDoUpper)
                        {
                            tableEntity = tableObjList.Where(m => m.TableName.Trim().ToLower().Equals(relationObj.UpperTableName.Trim().ToLower())).FirstOrDefault();
                        }
                        else
                        {
                            tableEntity = tableObjList.Where(m => m.TableName.Trim().ToLower().Equals(relationObj.LowerTableName.Trim().ToLower())).FirstOrDefault();
                        }

                        if (tableEntity != null)
                        {
                            if (!tableEntity.TableName.Trim().ToLower().Equals(orgTable.TableName.Trim().ToLower()))
                            {
                                schemaColumn = SchemaInfoDetails.GetTableSchemaInfo(tableEntity.TableName, connectionString).ToList();
                            }

                            if (schemaColumn.Count > 0)
                            {
                                foreach (var schemaColumnItem in schemaColumn)
                                {
                                    if (bDoUpper)
                                    {
                                        if (!DatabaseMap.RemoveTableNameFromField(tableEntity.IdFieldName).Equals(DatabaseMap.RemoveTableNameFromField(schemaColumnItem.ColumnName)))
                                        {
                                            sFieldName = "";

                                            if (mcFieldName.Count != 0)
                                            {
                                                if (mcFieldName.ContainsKey($"{relationObj.UpperTableName.ToUpper().Trim()}.{schemaColumnItem.ColumnName.ToUpper().Trim()}"))
                                                {
                                                    sFieldName = mcFieldName[$"{relationObj.UpperTableName.ToUpper().Trim()}.{schemaColumnItem.ColumnName.ToUpper().Trim()}"];
                                                }
                                            }

                                            if (string.IsNullOrEmpty(sFieldName))
                                            {
                                                if (!bNumericOnly || SchemaInfoDetails.IsANumericType(schemaColumnItem.DataType))
                                                {
                                                    if (!SchemaInfoDetails.IsSystemField(schemaColumnItem.ColumnName))
                                                    {
                                                        FieldNameList.Add(new KeyValuePair<string, string>($"{relationObj.UpperTableName.Trim()}.{schemaColumnItem.ColumnName}", $"{relationObj.UpperTableName.Trim()}.{schemaColumnItem.ColumnName}"));
                                                        mcFieldName.Add($"{relationObj.UpperTableName.Trim()}.{schemaColumnItem.ColumnName}", relationObj.LowerTableFieldName);
                                                        mcRelationships.Add($"{relationObj.UpperTableName.Trim()}.{schemaColumnItem.ColumnName}", relationObj);
                                                        mcLevel.Add($"{relationObj.UpperTableName.Trim()}.{schemaColumnItem.ColumnName}", iLevel);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!DatabaseMap.RemoveTableNameFromField(relationObj.LowerTableFieldName).Equals(DatabaseMap.RemoveTableNameFromField(schemaColumnItem.ColumnName)))
                                        {
                                            if (mcFieldName.Count != 0)
                                            {
                                                if (mcFieldName.ContainsKey($"{relationObj.LowerTableName.ToUpper().Trim()}.{schemaColumnItem.ColumnName.ToUpper().Trim()}"))
                                                {
                                                    sFieldName = mcFieldName[$"{relationObj.LowerTableName.ToUpper().Trim()}.{schemaColumnItem.ColumnName.ToUpper().Trim()}"];
                                                }
                                            }

                                            if (string.IsNullOrEmpty(sFieldName))
                                            {
                                                if (!bNumericOnly || SchemaInfoDetails.IsANumericType(schemaColumnItem.DataType))
                                                {
                                                    if (!SchemaInfoDetails.IsSystemField(schemaColumnItem.ColumnName))
                                                    {
                                                        FieldNameList.Add(new KeyValuePair<string, string>($"{relationObj.LowerTableName.Trim()}.{schemaColumnItem.ColumnName}", $"{relationObj.LowerTableName.Trim()}.{schemaColumnItem.ColumnName}"));
                                                        mcFieldName.Add($"{relationObj.LowerTableName.Trim()}.{schemaColumnItem.ColumnName}", relationObj.UpperTableFieldName);
                                                        mcRelationships.Add($"{relationObj.LowerTableName.Trim()}.{schemaColumnItem.ColumnName}", relationObj);
                                                        mcLevel.Add($"{relationObj.LowerTableName.Trim()}.{schemaColumnItem.ColumnName}", iLevel);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                schemaColumn = new List<SchemaColumns>();
                            }
                        }

                        if (iLevel < 2)
                        {
                            var recursiveParent = relationObjList.Where(m => m.LowerTableName.Trim().ToLower().Equals(tableEntity.TableName.Trim().ToLower())).OrderBy(m => m.TabOrder).ToList();

                            if (recursiveParent != null)
                            {
                                model = await FillViewColField(tableObjList, relationObjList, FieldNameList, orgTable, recursiveParent, true, iLevel + 1, bNumericOnly, connectionString);
                            }
                        }
                    }
                }

                model.mcFieldName = mcFieldName;
                model.mcRelationships = mcRelationships;
                model.mcLevel = mcLevel;
                model.LstKeyValuePair = FieldNameList;

                return model;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }
    }
}