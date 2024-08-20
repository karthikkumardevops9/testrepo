using MSRecordsEngine.Entities;
using MSRecordsEngine.Models.FusionModels;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using MSRecordsEngine.Models;
namespace MSRecordsEngine.Services
{
    public class DatabaseMap
    {

        public static TreeView GetBindTreeControl(List<MSRecordsEngine.Entities.System> lSystemEntities, List<Table> lTableEntities, List<TableTab> lTabletabEntities, List<TabSet> lTabsetsEntities, List<RelationShip> lRelationShipEntities)
        {
            var treeView = new TreeView();

            var treeNode = new TreeNode();
            var rootNode = new ListItem("Root node 1", "node1");

            var dataJsTree = new DataJsTree();
            // dataJsTree.Icon = "glyphicon glyphicon-folder-open"
            // dataJsTree.Icon = "/Images/icons/EMPLOYEE.ICO"
            dataJsTree.Opened = true;

            if (lSystemEntities.Count() > 0)
            {
                foreach (MSRecordsEngine.Entities.System pSystemEntity in lSystemEntities)
                {
                    var DatabaseNode = new ListItem(pSystemEntity.UserName, Guid.NewGuid().ToString() + "_root_" + pSystemEntity.Id.ToString() + "_0_1", className: "jstree-open", dataJsTree: dataJsTree);

                    int iTabsetsCount = lTabsetsEntities.Count();
                    int iTabsetsLoop = 0;
                    if (lTabsetsEntities.Count() > 0)
                    {
                        foreach (TabSet pTabsetsEntity in lTabsetsEntities.OrderBy(x => x.Id))
                        {
                            dataJsTree = new DataJsTree();
                            if (!string.IsNullOrEmpty(pTabsetsEntity.Picture))
                            {
                                dataJsTree.Icon = "/Images/icons/" + pTabsetsEntity.Picture;
                            }
                            else
                            {
                                // dataJsTree.Icon = "glyphicon glyphicon-folder-open"
                                dataJsTree.Icon = "/Images/icons/FOLDERS.ICO";
                            }
                            dataJsTree.Opened = true;

                            var TabsetNode = new ListItem(pTabsetsEntity.UserName, Guid.NewGuid().ToString() + "_Tabsets_" + pTabsetsEntity.Id.ToString() + "_" + iTabsetsLoop.ToString() + "_" + iTabsetsCount.ToString(), className: "jstree-open", dataJsTree: dataJsTree);
                            iTabsetsLoop = iTabsetsLoop + 1;
                            // DatabaseNode.Nodes.Add(New JSTreeView.ListItem(pTabsetsEntity.UserName, "level1_" + pTabsetsEntity.Id.ToString(), className:=""))
                            DatabaseNode.Nodes.Add(TabsetNode);

                            var lTabletabSelected = lTabletabEntities.Where(x => x.TabSet == pTabsetsEntity.Id).OrderBy(x => x.TabOrder);
                            int iTabletabCount = lTabletabSelected.Count();
                            int iTabletabLoop = 0;
                            if (lTabletabSelected.Count() > 0)
                            {
                                foreach (TableTab pTabletabEntity in lTabletabSelected)
                                {
                                    var pTableTabTableEntity = lTableEntities.Where(x => x.TableName.Trim().ToLower().Equals(pTabletabEntity.TableName.Trim().ToLower())).FirstOrDefault();
                                    // Dim pUserName = lTableEntities.Where(Function(x) x.TableName.Trim().ToLower().Equals(pTabletabEntity.TableName.Trim().ToLower())).FirstOrDefault().UserName
                                    if (pTableTabTableEntity is not null)
                                    {
                                        if (!string.IsNullOrEmpty(pTableTabTableEntity.UserName))
                                        {
                                            dataJsTree = new DataJsTree();
                                            if (!string.IsNullOrEmpty(pTableTabTableEntity.Picture))
                                            {
                                                dataJsTree.Icon = "/Images/icons/" + pTableTabTableEntity.Picture;
                                            }
                                            else
                                            {
                                                // dataJsTree.Icon = "glyphicon glyphicon-folder-open"
                                                dataJsTree.Icon = "/Images/icons/FOLDERS.ICO";
                                            }
                                            dataJsTree.Opened = true;

                                            var TableTabNode = new ListItem(pTableTabTableEntity.UserName, Guid.NewGuid().ToString() + "_Tabletabs_" + pTableTabTableEntity.TableId.ToString() + "_" + iTabletabLoop.ToString() + "_" + iTabletabCount.ToString(), className: "jstree-open", dataJsTree: dataJsTree);
                                            iTabletabLoop = iTabletabLoop + 1;
                                            TabsetNode.Nodes.Add(TableTabNode);

                                            var lRelationShipSelected = lRelationShipEntities.Where(x => x.UpperTableName.Trim().ToLower().Equals(pTabletabEntity.TableName.Trim().ToLower())).OrderBy(x => x.TabOrder);
                                            int iRelationShipCount = lRelationShipSelected.Count();
                                            int iRelationShipLoop = 0;
                                            foreach (RelationShip pRelationShipEntity in lRelationShipSelected)
                                            {
                                                var pRelTableEntity = lTableEntities.Where(x => x.TableName.Trim().ToLower().Equals(pRelationShipEntity.LowerTableName.Trim().ToLower())).FirstOrDefault();
                                                // pUserName = lTableEntities.Where(Function(x) x.TableName.Trim().ToLower().Equals(pRelationShipEntity.LowerTableName.Trim().ToLower())).FirstOrDefault().UserName
                                                if (pRelTableEntity is not null)
                                                {
                                                    if (!string.IsNullOrEmpty(pRelTableEntity.UserName))
                                                    {

                                                        dataJsTree = new DataJsTree();
                                                        if (!string.IsNullOrEmpty(pRelTableEntity.Picture))
                                                        {
                                                            dataJsTree.Icon = "/Images/icons/" + pRelTableEntity.Picture;
                                                        }
                                                        else
                                                        {
                                                            // dataJsTree.Icon = "glyphicon glyphicon-folder-open"
                                                            dataJsTree.Icon = "/Images/icons/FOLDERS.ICO";
                                                        }
                                                        dataJsTree.Opened = true;

                                                        var RelationShipNode = new ListItem(pRelTableEntity.UserName, Guid.NewGuid().ToString() + "_RelShips_" + pRelTableEntity.TableId.ToString() + "_" + iRelationShipLoop.ToString() + "_" + iRelationShipCount.ToString(), className: "jstree-open", dataJsTree: dataJsTree);
                                                        iRelationShipLoop = iRelationShipLoop + 1;
                                                        TableTabNode.Nodes.Add(RelationShipNode);

                                                        var pChildCheck = lRelationShipEntities.Where(x => x.UpperTableName.Trim().ToLower().Equals(pRelationShipEntity.LowerTableName.Trim().ToLower()));
                                                        if (pChildCheck.Count() > 0)
                                                        {
                                                            SetChildRecords(RelationShipNode, pRelationShipEntity.LowerTableName, lRelationShipEntities, lTableEntities);
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    rootNode.Nodes.Add(DatabaseNode);
                }
            }

            treeNode.ListItems.Add(rootNode);
            treeView.Nodes.Add(treeNode);
            return treeView;
        }

        public static void SetChildRecords(ListItem pListItem, string pTableName, List<RelationShip> lRelationEntities, List<Table> lTableEntities)
        {

            var dataJsTree = new DataJsTree();
            // dataJsTree.Icon = "glyphicon glyphicon-folder-open"
            // dataJsTree.Opened = True
            var lRelationShipEntities = lRelationEntities.Where(x => x.UpperTableName.Trim().ToLower().Equals(pTableName.Trim().ToLower())).OrderBy(x => x.TabOrder);
            int iRelationShipCount = lRelationShipEntities.Count();
            int iRelationShipLoop = 0;
            if (lRelationShipEntities.Count() > 0)
            {
                foreach (RelationShip pRelationShipEntity in lRelationShipEntities)
                {
                    var pRelTableEntity = lTableEntities.Where(x => x.TableName.Trim().ToLower().Equals(pRelationShipEntity.LowerTableName.Trim().ToLower())).FirstOrDefault();
                    // Dim pUserName = lTableEntities.Where(Function(x) x.TableName.Trim().ToLower().Equals(pRelationShipEntity.LowerTableName.Trim().ToLower())).FirstOrDefault().UserName
                    if (pRelTableEntity is not null)
                    {
                        dataJsTree = new DataJsTree();
                        if (!string.IsNullOrEmpty(pRelTableEntity.Picture))
                        {
                            dataJsTree.Icon = "/Images/icons/" + pRelTableEntity.Picture;
                        }
                        else
                        {
                            // dataJsTree.Icon = "glyphicon glyphicon-folder-open"
                            dataJsTree.Icon = "/Images/icons/FOLDERS.ICO";
                        }
                        dataJsTree.Opened = true;

                        var RelationShipChildNode = new ListItem(pRelTableEntity.UserName, Guid.NewGuid().ToString() + "_RelShips_" + pRelTableEntity.TableId.ToString() + "_" + iRelationShipLoop.ToString() + "_" + iRelationShipCount.ToString(), className: "jstree-open", dataJsTree: dataJsTree);
                        iRelationShipLoop = iRelationShipLoop + 1;
                        pListItem.Nodes.Add(RelationShipChildNode);

                        var pChildCheck = lRelationEntities.Where(x => x.UpperTableName.Trim().ToLower().Equals(pRelationShipEntity.LowerTableName.Trim().ToLower()));

                        if (pChildCheck.Count() > 0)
                        {
                            SetChildRecords(RelationShipChildNode, pRelationShipEntity.LowerTableName, lRelationEntities, lTableEntities);
                        }
                    }
                }

            }

        }

        public static string RemoveTableNameFromField(string sFieldName)
        {
            string RemoveTableNameFromFieldRet = default;
            int i;
            RemoveTableNameFromFieldRet = sFieldName;
            i = sFieldName.IndexOf(".");
            if (i > 0)
            {
                RemoveTableNameFromFieldRet = sFieldName.Substring(i + 1);
            }
            RemoveTableNameFromFieldRet = RemoveTableNameFromFieldRet.Trim();
            return RemoveTableNameFromFieldRet;
        }

        public static string RemoveTableNameFromFieldIfNotCurrentTable(string sFieldName, string currentTableName)
        {
            string RemoveTableNameFromFieldIfNotCurrentTableRet = default;
            string sFieldTableName;

            RemoveTableNameFromFieldIfNotCurrentTableRet = sFieldName;
            sFieldTableName = sFieldName;
            if (Strings.InStr(sFieldTableName, ".") > 1)
            {
                sFieldTableName = Strings.Left(sFieldTableName, Strings.InStr(sFieldTableName, ".") - 1);
                if (Convert.ToBoolean(Strings.StrComp(sFieldTableName, currentTableName, Constants.vbTextCompare)))
                {
                    RemoveTableNameFromFieldIfNotCurrentTableRet = Strings.Mid(sFieldName, Strings.InStr(sFieldName, ".") + 1);
                }
            }

            return RemoveTableNameFromFieldIfNotCurrentTableRet;
        }

        public static string RemoveFieldNameFromField(string sFieldName)
        {
            string RemoveFieldNameFromFieldRet = default;
            int i;
            RemoveFieldNameFromFieldRet = sFieldName;
            i = Strings.InStr(sFieldName, ".");
            if (i > 1)
            {
                RemoveFieldNameFromFieldRet = Strings.Trim(Strings.Left(sFieldName, i - 1));
            }
            return RemoveFieldNameFromFieldRet;
        }

        public static bool IsAStringType(Enums.DataTypeEnum eDataType)
        {
            switch (eDataType)
            {
                case Enums.DataTypeEnum.rmBSTR:
                case Enums.DataTypeEnum.rmChar:
                case Enums.DataTypeEnum.rmVarChar:
                case Enums.DataTypeEnum.rmLongVarChar:
                case Enums.DataTypeEnum.rmWChar:
                case Enums.DataTypeEnum.rmVarWChar:
                case Enums.DataTypeEnum.rmLongVarWChar:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public static bool IsADateType(Enums.DataTypeEnum eDataType)
        {
            switch (eDataType)
            {
                case Enums.DataTypeEnum.rmDate:
                case Enums.DataTypeEnum.rmDBDate:
                case Enums.DataTypeEnum.rmDBTimeStamp:
                case Enums.DataTypeEnum.rmDBTime:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public static int UserLinkIndexTableIdSize(string ConnectionString)
        {
            if (miUserLinkIndexTableIdSize == 0)
            {
                var pSchemaInfo = SchemaInfoDetails.GetSchemaInfo("USERLINKS", ConnectionString, "INDEXTABLEID");
                if (pSchemaInfo.Count > 0)
                {
                    try
                    {
                        miUserLinkIndexTableIdSize = pSchemaInfo[0].CharacterMaxLength;
                    }
                    catch (Exception)
                    {
                        miUserLinkIndexTableIdSize = 30;
                    }
                }
                else
                {
                    miUserLinkIndexTableIdSize = 30;
                }

                pSchemaInfo = null;
            }

            return miUserLinkIndexTableIdSize;
        }

        private static int miUserLinkIndexTableIdSize = 0;
    }
}
