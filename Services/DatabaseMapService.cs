using Microsoft.VisualBasic;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Services.Interface;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Data.Entity;
using Dapper;
using System.Security.Cryptography;

namespace MSRecordsEngine.Services
{
    public class DatabaseMapService : IDatabaseMapService
    {
        private IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);

        public async Task<bool> IsSysAdmin(string sTableName, string ConnectionString, Table pTableEntity = null, Databas pDatabases = null)
        {
            bool IsSysAdminRet = default;
            int lError = 0;
            string sErrMsg = "";
            var dt = new DataTable();

            if (sTableName.Length == 0)
                return true;

            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                if (pDatabases == null)
                {
                    pDatabases = await context.Databases.Where(x => x.DBName == pTableEntity.DBName).FirstOrDefaultAsync();
                }
            }

            ConnectionString = GetConnectionString(pDatabases, false);

            using (var conn = CreateConnection(ConnectionString))
            {
                string argsSql = "SELECT IS_SRVROLEMEMBER ('sysadmin')";
                string arglError = "";
                var res = conn.ExecuteReader(argsSql, commandType: CommandType.Text);
                if (res != null)
                    dt.Load(res);

                if (dt != null && dt.Rows.Count > 0)
                {
                    IsSysAdminRet = Convert.ToInt32(dt.Rows[0][0]) != 0;
                    dt = new DataTable();
                }

                if (IsSysAdminRet)
                    return true;

                int arglError1 = 0;
                string argsErrorMessage1 = "";

                string argsSql1 = "SELECT IS_MEMBER ('db_owner')";
                res = conn.ExecuteReader(argsSql1, commandType: CommandType.Text);
                if (res != null)
                    dt.Load(res);
                if (dt != null && dt.Rows.Count > 0)
                {
                    IsSysAdminRet = Convert.ToInt32(dt.Rows[0][0]) != 0;
                    dt = new DataTable();
                }

                return IsSysAdminRet;
            }
        }

        public bool IsADateType(Enums.DataTypeEnum eDataType)
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

        public async Task<string> CreateNewTables(string pTableName, string pIdFieldName, Enums.meFieldTypes pFieldType, int pFieldSize, Databas pDatabaseEntity, Table pParentTableEntity, string ConnectionString)
        {
            bool pOutPut = false;
            string pOutPutStr = string.Empty;
            bool bHaveParent;
            string sSQLStr;
            string sErrMsg = string.Empty;
            List<SchemaColumns> oSchemaColumns;


            if (pDatabaseEntity is not null)
            {
                ConnectionString = GetConnectionString(pDatabaseEntity, false);
            }
            //else
            //{
            //    //sADOConn = DataServices.DBOpen(Enums.eConnectionType.conDefault, null);
            //}

            bHaveParent = pParentTableEntity != null;

            try
            {
                sSQLStr = "CREATE TABLE [" + pTableName + "] ";
                sSQLStr = sSQLStr + "([" + pIdFieldName + "] ";

                switch (pFieldType)
                {
                    case Enums.meFieldTypes.ftLong:
                    case Enums.meFieldTypes.ftSmeadCounter:
                        {
                            sSQLStr = sSQLStr + "INT NOT NULL";
                            sSQLStr = sSQLStr + " CONSTRAINT [" + pTableName + "_PrimaryKey] PRIMARY KEY ([" + pIdFieldName + "])";
                            break;
                        }
                    case Enums.meFieldTypes.ftCounter:
                        {
                            sSQLStr = sSQLStr + "INT IDENTITY(1,1) NOT NULL";
                            sSQLStr = sSQLStr + " CONSTRAINT [" + pTableName + "_PrimaryKey] PRIMARY KEY ([" + pIdFieldName + "])";
                            break;
                        }
                    case Enums.meFieldTypes.ftText:
                        {
                            sSQLStr = sSQLStr + "VARCHAR(" + pFieldSize + ") NOT NULL";
                            sSQLStr = sSQLStr + " CONSTRAINT [" + pTableName + "_PrimaryKey] PRIMARY KEY ([" + pIdFieldName + "])";
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                if (bHaveParent)
                {
                    sSQLStr = sSQLStr + ", [" + pParentTableEntity.TableName + DatabaseMap.RemoveTableNameFromField(pParentTableEntity.IdFieldName) + "]";
                    oSchemaColumns = SchemaInfoDetails.GetSchemaInfo(pParentTableEntity.TableName, ConnectionString, DatabaseMap.RemoveTableNameFromField(pParentTableEntity.IdFieldName));

                    if (oSchemaColumns[0].IsString)
                    {
                        sSQLStr = sSQLStr + " VARCHAR(" + oSchemaColumns[0].CharacterMaxLength + ")";
                    }
                    else if (oSchemaColumns[0].IsADate)
                    {
                        sSQLStr = sSQLStr + " DATETIME";
                    }
                    else
                    {
                        switch (oSchemaColumns[0].DataType)
                        {
                            case Enums.DataTypeEnum.rmInteger:
                                {
                                    if (oSchemaColumns[0].CharacterMaxLength == 2)
                                    {
                                        sSQLStr = sSQLStr + " SHORT INT";
                                    }
                                    else
                                    {
                                        sSQLStr = sSQLStr + " INT";
                                    }

                                    break;
                                }

                            default:
                                {
                                    Interaction.MsgBox("Unsupported Type IdFieldNameType");
                                    break;
                                }
                        }
                    }

                    oSchemaColumns = null;
                }

                sSQLStr = sSQLStr + ");";

                pOutPut = CommonFunctions.ProcessADOCommand(ConnectionString, ref sSQLStr, false);

                if (pOutPut)
                {
                    //string argsSql = "SELECT TOP 1 * FROM " + pTableName + "";
                    //rsADO = DataServices.GetADORecordSet(ref argsSql, sADOConn, ref sErrMsg);
                    //if (sErrMsg.ToLower().Contains("incorrect syntax near the keyword"))
                    //{
                    //    throw new Exception(sErrMsg);
                    //}

                    if (pFieldType == Enums.meFieldTypes.ftSmeadCounter)
                    {
                        sSQLStr = "ALTER TABLE System";
                        sSQLStr = sSQLStr + " ADD [" + pTableName + "Counter] INT NULL";

                        pOutPut = CommonFunctions.ProcessADOCommand(ConnectionString, ref sSQLStr, false);

                        List<SchemaColumns> oSchemaColumn;
                        oSchemaColumn = SchemaInfoDetails.GetSchemaInfo("System", ConnectionString, pTableName + "Counter");

                        if (oSchemaColumn.Count > 0)
                        {
                            await IncrementCounter(pTableName + "Counter", ConnectionString);
                            pOutPut = true;
                        }
                    }
                    else
                    {
                        pOutPut = true;
                    }
                }
                if (pOutPut == false)
                    pOutPutStr = "False";
                return pOutPutStr;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public async Task<RtnSetTablesEntity> SetTablesEntity(string sTableName, string sUserName, string sIdFieldName, string sDatabaseName, Enums.meFieldTypes eFieldType, int iTableId, string ConnectionString)
        {
            var model = new RtnSetTablesEntity();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pMaxSearchOrder = (await context.Tables.ToListAsync()).Max(x => x.SearchOrder);

                    var pTables = new Table();

                    pTables.TableName = sTableName;
                    pTables.UserName = sUserName;

                    pTables.AddGroup = 0;
                    pTables.DelGroup = 0;
                    pTables.EditGroup = 0;
                    pTables.MgrGroup = 0;
                    pTables.ViewGroup = 0;
                    pTables.PCFilesEditGrp = 0;
                    pTables.PCFilesNVerGrp = 0;
                    pTables.Attachments = false;
                    pTables.AttributesID = 1;

                    if (Strings.StrComp(sDatabaseName, "db_engine", Constants.vbTextCompare) != 0)
                    {
                        pTables.DBName = sDatabaseName;
                    }

                    pTables.IdFieldName = sTableName + "." + sIdFieldName;
                    pTables.IdFieldName2 = null;
                    pTables.TrackingTable = (short?)0;
                    pTables.Trackable = false;

                    pTables.Picture = null;
                    pTables.BarCodePrefix = null;

                    if (eFieldType == Enums.meFieldTypes.ftSmeadCounter)
                    {
                        pTables.CounterFieldName = sTableName + "Counter";
                    }
                    else
                    {
                        pTables.CounterFieldName = null;
                    }

                    pTables.DefaultDescriptionField = null;
                    pTables.DefaultDescriptionText = null;
                    pTables.DefaultRetentionId = null;
                    pTables.DescFieldNameOne = null;
                    pTables.DescFieldNameTwo = null;
                    pTables.DescFieldPrefixOne = null;
                    pTables.DescFieldPrefixOneTable = null;
                    pTables.DescFieldPrefixOneWidth = (short?)0;
                    pTables.DescFieldPrefixTwo = null;
                    pTables.DescFieldPrefixTwoTable = null;
                    pTables.DescFieldPrefixTwoWidth = (short?)0;
                    pTables.DescRelateTable1 = null;
                    pTables.DescRelateTable2 = null;
                    pTables.IdMask = null;
                    pTables.IdStripChars = null;
                    pTables.MaxRecordsAllowed = (short?)0;
                    pTables.OutTable = (short?)0;
                    pTables.RestrictAddToTable = (short?)0;
                    pTables.RuleDateField = null;
                    pTables.TrackingACTIVEFieldName = null;
                    pTables.TrackingOUTFieldName = null;
                    pTables.TrackingStatusFieldName = null;

                    pTables.ADOServerCursor = false;
                    pTables.ADOQueryTimeout = 30;
                    pTables.ADOCacheSize = 1;
                    pTables.DeleteAttachedGroup = 9999;
                    pTables.SearchOrder = pMaxSearchOrder + 1;
                    pTables.Trackable = false;
                    pTables.AllowBatchRequesting = false;

                    context.Tables.Add(pTables);
                    await context.SaveChangesAsync();

                    iTableId = pTables.TableId;

                    model.TableId = pTables.TableId;
                    model.Success = true;
                }
            }
            catch (Exception ex)
            {
                model.Success = false;
                return model;
            }

            return model;
        }

        public async Task<RtnSetViewsEntity> SetViewsEntity(string sTableName, string sViewName, string ConnectionString, int iViewId)
        {
            var model = new RtnSetViewsEntity();

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pView = new View();

                    pView.Printable = false;
                    pView.AltViewId = 0;
                    pView.DeleteGridAvail = false;
                    pView.FiltersActive = false;
                    pView.Picture = "";
                    pView.ReportStylesId = "All Requests Report";
                    pView.RowHeight = (short?)0;
                    pView.SQLStatement = "SELECT * FROM [" + sTableName + "]";
                    pView.SearchableView = true;
                    pView.TableName = sTableName;
                    pView.TablesDown = "";
                    pView.TablesId = 0;
                    pView.UseExactRowCount = false;
                    pView.VariableColWidth = true;
                    pView.VariableFixedCols = false;
                    pView.VariableRowHeight = true;
                    pView.ViewGroup = 0;
                    pView.ViewName = sViewName;
                    pView.ViewOrder = 1;
                    pView.ViewType = (short?)0;
                    pView.Visible = true;
                    pView.WorkFlow1 = "";
                    pView.WorkFlow1Pic = "";
                    pView.WorkFlowDesc1 = "";
                    pView.WorkFlow2 = "";
                    pView.WorkFlow2Pic = "";
                    pView.WorkFlowDesc2 = "";
                    pView.WorkFlow3 = "";
                    pView.WorkFlow3Pic = "";
                    pView.WorkFlowDesc3 = "";
                    pView.WorkFlow4 = "";
                    pView.WorkFlow4Pic = "";
                    pView.WorkFlowDesc4 = "";
                    pView.WorkFlow5 = "";
                    pView.WorkFlow5Pic = "";
                    pView.WorkFlowDesc5 = "";
                    pView.MaxRecsPerFetch = pView.MaxRecsPerFetch;
                    pView.MaxRecsPerFetchDesktop = pView.MaxRecsPerFetchDesktop;

                    context.Views.Add(pView);
                    await context.SaveChangesAsync();

                    iViewId = pView.Id;

                    model.Success = true;
                    model.ViewId = iViewId;
                }
            }
            catch (Exception ex)
            {
                model.Success = false;
                return model;
            }

            return model;
        }

        public async Task<bool> SetViewColumnEntity(int iViewId, string sTableName, string sIdFieldName, int sIdFieldType, string ConnectionString, Table pParentTable)
        {
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var pViewColumn = new ViewColumn();
                bool bEditAllowed;
                if (sIdFieldType == (int)Enums.meFieldTypes.ftSmeadCounter || sIdFieldType == (int)Enums.meFieldTypes.ftCounter)
                {
                    bEditAllowed = false;
                }
                else
                {
                    bEditAllowed = true;
                }

                pViewColumn = CreateViewColumnEntity(iViewId, bEditAllowed, Enums.geViewColumnDisplayType.cvAlways, sTableName, sIdFieldName, Enums.geViewColumnsLookupType.ltDirect, 0, 1, 1);

                context.ViewColumns.Add(pViewColumn);
                await context.SaveChangesAsync();

                pViewColumn = null;
                if (pParentTable != null)
                {
                    pViewColumn = new ViewColumn();

                    pViewColumn = CreateViewColumnEntity(iViewId, bEditAllowed, Enums.geViewColumnDisplayType.cvSmartColumns, sTableName, pParentTable.TableName + DatabaseMap.RemoveTableNameFromField(pParentTable.IdFieldName), Enums.geViewColumnsLookupType.ltDirect, 1, 0, 0);

                    context.ViewColumns.Add(pViewColumn);
                    await context.SaveChangesAsync();
                }

                return true;
            }

        }

        public async Task<RtnSetRelationshipsEntity> SetRelationshipsEntity(string sTableName, Table pParentTableEntity, string ConnectionString, int iRelId, string sExtraStr = "", string sIdFieldName = "")
        {
            var model = new RtnSetRelationshipsEntity();

            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var pRelationShip = new RelationShip();
                int iHiTab;

                if (pParentTableEntity != null)
                {
                    iHiTab = 0;

                    var lRelationShipEntities = await context.RelationShips.Where(x => x.UpperTableName.Trim().ToLower().Equals(pParentTableEntity.TableName.Trim().ToLower())).ToListAsync();

                    if (lRelationShipEntities.Count() > 0)
                    {
                        iHiTab = (int)lRelationShipEntities.Max(x => x.TabOrder);
                    }

                    iHiTab = iHiTab + 1;

                    pRelationShip.DrillDownViewGroup = 0;
                    pRelationShip.IdTypes = (short?)0;
                    if (!string.IsNullOrEmpty(sIdFieldName))
                    {
                        pRelationShip.LowerTableFieldName = sTableName + "." + sIdFieldName;
                    }
                    else
                    {
                        pRelationShip.LowerTableFieldName = sTableName + "." + pParentTableEntity.TableName + DatabaseMap.RemoveTableNameFromField(pParentTableEntity.IdFieldName) + sExtraStr;
                    }

                    pRelationShip.LowerTableName = sTableName;
                    pRelationShip.TabOrder = (short?)iHiTab;
                    pRelationShip.UpperTableFieldName = pParentTableEntity.TableName + "." + DatabaseMap.RemoveTableNameFromField(pParentTableEntity.IdFieldName);
                    pRelationShip.UpperTableName = pParentTableEntity.TableName;

                    context.RelationShips.Add(pRelationShip);
                    await context.SaveChangesAsync();

                    iRelId = pRelationShip.Id;
                    pRelationShip = null;

                    model.Success = true;
                    model.RelationshipId = iRelId;
                }
            }

            return model;
        }

        public async Task<RtnSetTabSetEntity> SetTabSetEntity(int iTabSetId, int lViewId, string sTableName, string ConnectionString, int iTableTabId)
        {
            var model = new RtnSetTabSetEntity();

            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var pTabletab = new TableTab();
                long lHighestOrder;
                var lTabletabEntities = await context.TableTabs.ToListAsync();

                var pTabletabEntity = lTabletabEntities.Where(x => x.TabSet == iTabSetId && x.TableName.Trim().ToLower().Equals(sTableName.Trim().ToLower())).FirstOrDefault();

                var flag = true;

                if (pTabletabEntity != null)
                {
                    if (pTabletabEntity.Id > 0)
                    {
                        if (pTabletabEntity.TopTab == true)
                        {
                            pTabletabEntity.TopTab = true;

                            context.TableTabs.Add(pTabletabEntity);
                            await context.SaveChangesAsync();

                            iTableTabId = pTabletabEntity.Id;
                        }
                        pTabletab = null;
                        flag = false;
                    }
                }

                if (flag)
                {
                    lHighestOrder = lTabletabEntities.Where(x => x.TabSet == iTabSetId).Count();
                    lHighestOrder = lHighestOrder + 1L;
                    pTabletab.BaseView = lViewId;
                    pTabletab.TableName = sTableName;
                    pTabletab.TabOrder = (short?)lHighestOrder;
                    pTabletab.TabSet = (short?)iTabSetId;
                    pTabletab.TopTab = true;

                    context.TableTabs.Add(pTabletab);
                    await context.SaveChangesAsync();

                    iTableTabId = pTabletab.Id;
                    pTabletab = null;
                }

                model.Success = true;
                model.TableTabId = iTableTabId;

            }

            return model;
        }

        public async Task<List<Table>> GetAttachExistingTableList(List<Table> lTablesEntities, int iParentTableId, int iTabSetId, string ConnectionString)
        {
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                string sTableName;
                var lEngineTablesList = CollectionsClass.EngineTablesList;
                var lEngineTablesNotNeededList = CollectionsClass.EngineTablesNotNeededList;
                var lTableList = new List<string>();

                lTablesEntities = (from q in lTablesEntities
                                   where !lEngineTablesList.Any(x => (x.Trim().ToLower() ?? "") == (q.TableName.Trim().ToLower() ?? ""))
                                   select q).ToList();

                lTablesEntities = (from q in lTablesEntities
                                   where !lEngineTablesNotNeededList.Any(x => (x.Trim().ToLower() ?? "") == (q.TableName.Trim().ToLower() ?? ""))
                                   select q).ToList();

                if (iParentTableId == 0)
                {
                    var lTableTabsEntities = await context.TableTabs.ToListAsync();
                    var lTableTabsTableList = from q in lTableTabsEntities.Where(x => x.TabSet == iTabSetId)
                                              select q.TableName;

                    foreach (var sTableNameLoop in lTableTabsTableList)
                        lTableList.Add(sTableNameLoop);

                    lTablesEntities = (from q in lTablesEntities
                                       where !lTableList.Any(x => (x.Trim().ToLower() ?? "") == (q.TableName.Trim().ToLower() ?? ""))
                                       select q).ToList();
                }
                else
                {
                    sTableName = lTablesEntities.Where(x => x.TableId == iParentTableId).FirstOrDefault().TableName;
                    var lRel = await context.RelationShips.ToListAsync();

                    lTablesEntities = (from q in lTablesEntities
                                       where !q.TableName.Trim().ToLower().Equals(sTableName.Trim().ToLower())
                                       select q).ToList();

                    lTableList = new List<string>();

                    foreach (var sTableNameLoop in (await GetChildTableIds(sTableName, ConnectionString)))
                        lTableList.Add(sTableNameLoop);

                    lTablesEntities = (from q in lTablesEntities
                                       where !lTableList.Any(x => (x.Trim().ToLower() ?? "") == (q.TableName.Trim().ToLower() ?? ""))
                                       select q).ToList();
                }
            }

            return lTablesEntities.ToList();
        }

        public async Task<List<string>> GetChildTableIds(string pTableName, string ConnectionString)
        {
            var lTableNames = new List<string>();
            using (var conn = CreateConnection(ConnectionString))
            {
                var query = "SELECT * FROM dbo.FNGetAllChildParentTableList('" + pTableName + "');";
                lTableNames = (await conn.QueryAsync<string>(query, commandType: CommandType.Text)).ToList();
            }
            return lTableNames;
        }

        public async Task<bool> CreateNewField(string sFieldName, Enums.DataTypeEnum eFieldType, long lSize, string sTableName, string ConnectionString)
        {
            string sSQLStr;

            // create the new field:
            sSQLStr = "ALTER TABLE [" + sTableName + "]";
            sSQLStr = sSQLStr + " ADD [" + sFieldName + "] ";

            if (CommonFunctions.IsAStringType(eFieldType))
            {
                sSQLStr = sSQLStr + " VARCHAR(" + lSize + ") NULL";
            }
            else if (CommonFunctions.IsADateType(eFieldType))
            {
                sSQLStr = sSQLStr + " SMALLDATETIME NULL";
            }
            else
            {
                switch (eFieldType)
                {
                    case Enums.DataTypeEnum.rmTinyInt:
                    case Enums.DataTypeEnum.rmUnsignedTinyInt:
                        {
                            sSQLStr = sSQLStr + " TINYINT NULL DEFAULT 0";
                            break;
                        }
                    case Enums.DataTypeEnum.rmSmallInt:
                    case Enums.DataTypeEnum.rmUnsignedSmallInt:
                        {
                            sSQLStr = sSQLStr + " SMALLINT NULL";
                            break;
                        }
                    case Enums.DataTypeEnum.rmInteger:
                    case Enums.DataTypeEnum.rmBigInt:
                    case Enums.DataTypeEnum.rmUnsignedInt:
                    case Enums.DataTypeEnum.rmUnsignedBigInt:
                        {
                            sSQLStr = sSQLStr + " INT NULL";
                            break;
                        }
                    case Enums.DataTypeEnum.rmSingle:
                        {
                            sSQLStr = sSQLStr + " REAL NULL";
                            break;
                        }
                    case Enums.DataTypeEnum.rmDouble:
                        {
                            sSQLStr = sSQLStr + " FLOAT NULL";
                            break;
                        }

                    default:
                        {
                            Interaction.MsgBox("Unsupported Type IdFieldNameType");
                            break;
                        }
                }
            }

            return CommonFunctions.ProcessADOCommand(ConnectionString, ref sSQLStr, false);
        }

        public async Task<string> GetTableDependency(List<Table> lTableEntities, string ConnectionString, Table pUpperTableEntity, Table pLowerTableEntity, string sViewMessage)
        {
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var lViewEntities = await context.Views.ToListAsync();
                var lViewColumnEntities = await context.ViewColumns.ToListAsync();

                // Iterate over the table entities
                foreach (var oTables in lTableEntities)
                {
                    var lLoopViewEntities = lViewEntities.Where(x => x.TableName.Trim().ToLower() == oTables.TableName.Trim().ToLower());

                    foreach (var oViews in lLoopViewEntities)
                    {
                        var lInViewColumnEntities = lViewColumnEntities.Where(x => x.ViewsId == oViews.Id);

                        foreach (var oViewColumns in lInViewColumnEntities)
                        {
                            switch (oViewColumns.LookupType)
                            {
                                case 1:
                                    {
                                        if (oViewColumns.FieldName.Contains("."))
                                        {
                                            if (ReferenceEquals(oTables, pLowerTableEntity))
                                            {
                                                var fieldPrefix = oViewColumns.FieldName.Substring(0, oViewColumns.FieldName.IndexOf("."));
                                                if (string.Equals(fieldPrefix, pUpperTableEntity.TableName, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    if (!sViewMessage.Contains($"  Used in View\t\"{oViews.ViewName}\"{Environment.NewLine}"))
                                                    {
                                                        sViewMessage += $"<li> Used in View \t\"{oViews.ViewName}\"</li>";
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }

                                case 12:
                                case 13:
                                case 14:
                                case 15:
                                    {
                                        if (oViewColumns.LookupIdCol == -1)
                                        {
                                            if (oViewColumns.FieldName.Contains("."))
                                            {
                                                if (ReferenceEquals(oTables, pUpperTableEntity))
                                                {
                                                    var fieldPrefix = oViewColumns.FieldName.Substring(0, oViewColumns.FieldName.IndexOf("."));
                                                    if (string.Equals(fieldPrefix, pLowerTableEntity.TableName, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        if (!sViewMessage.Contains($"  Used in View\t\"{oViews.ViewName}\"{Environment.NewLine}"))
                                                        {
                                                            sViewMessage += $"<li> Used in View \t\"{oViews.ViewName}\"</li>";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (oViewColumns.FieldName.Contains("."))
                                        {
                                            var fieldPrefix = oViewColumns.FieldName.Substring(0, oViewColumns.FieldName.IndexOf("."));
                                            if (string.Equals(fieldPrefix, pUpperTableEntity.TableName, StringComparison.OrdinalIgnoreCase))
                                            {
                                                if (oViewColumns.LookupIdCol >= 0 && oViewColumns.LookupIdCol < lInViewColumnEntities.Count())
                                                {
                                                    var oTmpViewColumns = lInViewColumnEntities
                                                        .FirstOrDefault(x => x.LookupIdCol == oViewColumns.LookupIdCol + 1);
                                                    if (oTmpViewColumns != null)
                                                    {
                                                        var tmpFieldPrefix = oTmpViewColumns.FieldName.Substring(0, oTmpViewColumns.FieldName.IndexOf("."));
                                                        if (string.Equals(tmpFieldPrefix, pLowerTableEntity.TableName, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            if (!sViewMessage.Contains($"  Used in View\t\"{oViews.ViewName}\"{Environment.NewLine}"))
                                                            {
                                                                sViewMessage += $"<li> Used in View \t\"{oViews.ViewName}\"</li>";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(sViewMessage))
                {
                    sViewMessage = $"<ul>{sViewMessage}</ul>";
                }
            }

            return sViewMessage;
        }

        public async Task<bool> DropTable(string pTableName, string ConnectionString)
        {
            bool rtn;
            string sSQLStr;

            try
            {
                sSQLStr = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + pTableName.Trim() + "]') AND type in (N'U')) ";
                sSQLStr = sSQLStr + "DROP TABLE [" + pTableName.Trim() + "];";
                rtn = await GetInfoUsingDapper.ProcessADOCommand(sSQLStr, ConnectionString, false);
            }
            catch (Exception)
            {
                rtn = false;
            }

            if (rtn)
            {
                try
                {
                    sSQLStr = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + pTableName.Trim() + "]') AND type in (N'V')) ";
                    sSQLStr = sSQLStr + "DROP VIEW [" + pTableName.Trim() + "];";
                    await GetInfoUsingDapper.ProcessADOCommand(sSQLStr, ConnectionString, false);
                }
                catch (Exception)
                {
                }
            }

            return rtn;
        }

        public async Task<List<string>> GetAttachTableFieldsList(Table pTableEntity, Table pParentTableEntity, string ConnectionString)
        {
            var iFieldType = default(int);
            var lFieldSize = default(long);
            List<SchemaColumns> oSchemaColumns;
            var lColumnNames = new List<string>();
            var pDatabaseEntity = new Databas();

            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                if (pParentTableEntity != null)
                {
                    pDatabaseEntity = await context.Databases.Where(x => x.DBName.Trim().ToLower().Equals(pParentTableEntity.DBName.Trim().ToLower())).FirstOrDefaultAsync();

                    if (pDatabaseEntity != null)
                    {
                        ConnectionString = GetConnectionString(pDatabaseEntity, false);
                    }
                }

                var idFiledSchema = await GetInfoUsingDapper.GetCoulmnSchemaInfo(ConnectionString, pParentTableEntity.TableName, DatabaseMap.RemoveTableNameFromField(pParentTableEntity.IdFieldName));
                if(idFiledSchema != null)
                    lFieldSize = idFiledSchema.CHARACTER_MAXIMUM_LENGTH;


                pDatabaseEntity = null;
                pDatabaseEntity = await context.Databases.Where(x => x.DBName.Trim().ToLower().Equals(pTableEntity.DBName.Trim().ToLower())).FirstOrDefaultAsync();

                if(pDatabaseEntity != null)
                    ConnectionString = GetConnectionString(pDatabaseEntity, false);

                oSchemaColumns = SchemaInfoDetails.GetSchemaInfo(pParentTableEntity.TableName, ConnectionString, DatabaseMap.RemoveTableNameFromField(pParentTableEntity.IdFieldName));

                if (oSchemaColumns.Count > 0)
                {
                    iFieldType = (int)oSchemaColumns[0].DataType;
                    oSchemaColumns = null;
                }

                oSchemaColumns =  SchemaInfoDetails.GetSchemaInfo(Strings.Trim(pTableEntity.TableName), ConnectionString);

                foreach (var oSchema in oSchemaColumns)
                {
                    if (!SchemaInfoDetails.IsSystemField(oSchema.ColumnName))
                    {
                        var columnLength = await GetInfoUsingDapper.GetCoulmnSchemaInfo(ConnectionString, pTableEntity.TableName, DatabaseMap.RemoveTableNameFromField(oSchema.ColumnName));
                        if ((int)oSchema.DataType == iFieldType & lFieldSize == (long)columnLength.CHARACTER_MAXIMUM_LENGTH)
                        {
                            lColumnNames.Add(Strings.Trim(oSchema.ColumnName));
                        }
                    }
                }

                oSchemaColumns = null;
            }

            return lColumnNames;
        }

        private string GetConnectionString(Databas DBToOpen, bool includeProvider)
        {
            string sConnect = string.Empty;
            if (includeProvider)
                sConnect = string.Format("Provider={0}; ", DBToOpen.DBProvider);

            sConnect += string.Format("Data Source={0}; Initial Catalog={1}; ", DBToOpen.DBServer, DBToOpen.DBDatabase);

            if (!string.IsNullOrEmpty(DBToOpen.DBUserId))
            {
                sConnect += string.Format("User Id={0}; Password={1};", DBToOpen.DBUserId, DBToOpen.DBPassword);
            }
            else
            {
                sConnect += "Persist Security Info=True;Integrated Security=SSPI;";
                // sConnect = Keys.DefaultConnectionString(True, DBToOpen.DBDatabase)
            }

            return sConnect;
        }

        private async Task<bool> IncrementCounter(string sCounterName, string ConnectionString, string NewCounter = "0")
        {
            var bIsNull = default(bool);
            string counter = string.Empty;
            int lCount;
            var lErrorCount = default(int);
            int lError;
            string sErrMsg = string.Empty;
            string sSQL;
            string sQuote = string.Empty;

            SchemaColumns oSchemaColumn;
            int pCounter;
            ;

            oSchemaColumn = SchemaInfoDetails.GetSchemaInfo("System", ConnectionString, sCounterName).SingleOrDefault();
            if (oSchemaColumn != null && oSchemaColumn.IsString)
                sQuote = "'";

            lbl_IncrementCounter_Restart:
            ;

            sSQL = string.Format("SELECT [{0}] FROM System", sCounterName);
            string arglError = "";
            var dt = new DataTable();

            using (var conn = CreateConnection(ConnectionString))
            {
                //var res = await conn.ExecuteReaderAsync(sSQL, commandType: CommandType.Text);
                //dt.Load(res);

                var res = await conn.QueryFirstOrDefaultAsync<int?>(sSQL, commandType: CommandType.Text);

                if (res == null)
                {
                    bIsNull = true;
                    pCounter = 1;
                }
                else
                {
                    pCounter = Convert.ToInt32(res);
                }

                //if (dt.Rows.Count == 0)
                //{
                //    pCounter = 1;
                //}
                //else if (dt.Rows[0][sCounterName] == null)
                //{
                //    bIsNull = true;
                //    pCounter = 1;
                //}
                //else
                //{
                //    pCounter = Convert.ToInt32(dt.Rows[0][sCounterName]);
                //}

                counter = pCounter.ToString();

                if (Convert.ToDouble(counter) >= Convert.ToDouble(NewCounter))
                    NewCounter = (Convert.ToDouble(counter) + 1d).ToString();

                sSQL = string.Format("UPDATE [System] SET [{0}] = {1}{2}{1} WHERE ", sCounterName, sQuote, NewCounter);

                if (bIsNull)
                {
                    sSQL += string.Format("[{0}] IS NULL", sCounterName);
                }
                else
                {
                    sSQL += string.Format("[{0}] = {1}{2}{1}", sCounterName, sQuote, counter);
                }

                lError = 0;
                sErrMsg = string.Empty;

                lCount = ProcessADOCommand(ConnectionString, ref sSQL, true, ref lError, ref sErrMsg);

                if (sErrMsg.ToLower().Contains("overflow"))
                {
                    throw new OverflowException(string.Format("The value {0} is too large to fit into the System.{1} field.  Please contact your system administrator.", NewCounter, sCounterName));
                }

                if (lCount != 1)
                {
                    lErrorCount = lErrorCount + 1;

                    if (lErrorCount < 1000)
                    {
                        goto lbl_IncrementCounter_Restart;
                    }

                }

                return true;

            lbl_IncrementCounter:
                ;

                if (sErrMsg.ToLower().Contains("overflow"))
                {
                    throw new OverflowException(string.Format("The value {0} is too large to fit into the System.{1} field.  Please contact your system administrator.", NewCounter, sCounterName));
                }

                lErrorCount = lErrorCount + 1;

                if (lErrorCount < 1000)
                {
                    ;
                }
            }
        }

        private static int ProcessADOCommand(string ConnectionString, ref string sSQL, [Optional, DefaultParameterValue(false)] bool bDoNoCount, [Optional, DefaultParameterValue(-1)] ref int lError, [Optional, DefaultParameterValue("")] ref string sErrorMsg)
        {
            int recordaffected = default;
            var conn = new SqlConnection(ConnectionString);
            conn.Open();

            try
            {
                if (bDoNoCount)
                {
                    sSQL = "SET NOCOUNT OFF;" + sSQL + ";SET NOCOUNT ON";
                }
                recordaffected = conn.Execute(sSQL);
            }
            catch (Exception ex)
            {
                if (lError != -1)
                {
                    lError = Information.Err().Number;
                    sErrorMsg = Information.Err().Description;
                }
                conn.Close();
                return -1;
            }
            conn.Close();
            return recordaffected;
        }

        private static ViewColumn CreateViewColumnEntity(int iViewId, bool bEditAllowed, Enums.geViewColumnDisplayType eColumnOrder, string sTableName, string sIdFieldName, Enums.geViewColumnsLookupType eLookupType, int iColumnNum, int iFreezeOrder, int iSortOrder)
        {
            var pViewColumn = new ViewColumn();

            pViewColumn.ViewsId = iViewId;
            pViewColumn.ColumnNum = (short?)iColumnNum;
            pViewColumn.ColumnVisible = false;
            pViewColumn.ColumnWidth = (short?)200;
            pViewColumn.ColumnOrder = Convert.ToInt16(eColumnOrder);

            pViewColumn.EditAllowed = bEditAllowed;

            pViewColumn.FieldName = sTableName + "." + sIdFieldName;
            pViewColumn.Heading = sIdFieldName;
            pViewColumn.FilterField = true;
            pViewColumn.FreezeOrder = iFreezeOrder;
            pViewColumn.SortOrder = iSortOrder;
            pViewColumn.LookupType = Convert.ToInt16(eLookupType);
            pViewColumn.SortableField = true;

            pViewColumn.AlternateFieldName = "";
            pViewColumn.ColumnStyle = (short?)0;
            pViewColumn.DefaultLookupValue = "";
            pViewColumn.DropDownFilterIdField = "";
            pViewColumn.DropDownFilterMatchField = "";
            pViewColumn.DropDownFlag = (short?)0;
            pViewColumn.DropDownReferenceColNum = (short?)0;
            pViewColumn.DropDownReferenceValue = "";
            pViewColumn.DropDownTargetField = "";
            pViewColumn.EditMask = "";
            pViewColumn.FormColWidth = 0;
            pViewColumn.InputMask = "";
            pViewColumn.LookupIdCol = (short?)0;
            pViewColumn.MaskClipMode = false;
            pViewColumn.MaskInclude = false;
            pViewColumn.MaskPromptChar = "_";
            pViewColumn.MaxPrintLines = 1;
            pViewColumn.PageBreakField = false;
            pViewColumn.Picture = "";
            pViewColumn.PrinterColWidth = 0;
            pViewColumn.SortOrderDesc = false;
            pViewColumn.SuppressDuplicates = false;
            pViewColumn.SuppressPrinting = false;
            pViewColumn.VisibleOnForm = true;
            pViewColumn.VisibleOnPrint = true;
            pViewColumn.CountColumn = false;
            pViewColumn.SubtotalColumn = false;
            pViewColumn.PrintColumnAsSubheader = false;
            pViewColumn.RestartPageNumber = false;
            pViewColumn.LabelJustify = (int?)1;
            pViewColumn.LabelLeft = (int?)-1;
            pViewColumn.LabelTop = (int?)-1;
            pViewColumn.LabelWidth = (int?)-1;
            pViewColumn.LabelHeight = (int?)-1;
            pViewColumn.ControlLeft = (int?)-1;
            pViewColumn.ControlTop = (int?)-1;
            pViewColumn.ControlWidth = (int?)-1;
            pViewColumn.ControlHeight = (int?)-1;
            pViewColumn.TabOrder = (int?)-1;

            return pViewColumn;
        }

    }
}
