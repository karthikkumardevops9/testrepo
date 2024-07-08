using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Services.Interface;

namespace MSRecordsEngine.Services
{

    public class ViewService : IViewService
    {
        public static Dictionary<int, ViewColumn> mcFilterColumns = new Dictionary<int, ViewColumn>();
        public async Task<List<GridColumns>> GetColumnsData(List<View> lView, List<ViewColumn> lViewColumns, List<Table> lTables, int intViewsId, string sAction, string ConnectionString)
        {
            var GridColumnEntities = new List<GridColumns>();
            try
            {
                if (!string.IsNullOrEmpty(sAction) && sAction.Trim().ToUpper().Equals("E"))
                {
                    var oViews = lView.Where(x => x.Id == intViewsId).FirstOrDefault();

                    if (oViews != null)
                    {
                        string sTableName = oViews.TableName;
                        int iViewsId = Convert.ToInt32(oViews.Id);
                        var oTable = lTables.Where(x => x.TableName.Trim().ToLower().Equals(sTableName.Trim().ToLower())).FirstOrDefault();
                        var olViewColumns = lViewColumns.Where(x => x.ViewsId == iViewsId).OrderBy(x => x.ColumnNum);

                        if (olViewColumns is not null)
                        {
                            if (olViewColumns.Count() == 0)
                            {
                                var oAltView = lView.Where(x => x.Id == oViews.AltViewId).FirstOrDefault();
                                olViewColumns = lViewColumns.Where(x => x.ViewsId == oAltView.Id).OrderBy(x => x.ColumnNum);
                            }
                        }

                        if (olViewColumns is not null)
                        {

                            foreach (ViewColumn column in olViewColumns)
                            {
                                var GridColumnEntity = new GridColumns();
                                GridColumnEntity.ColumnSrNo = column.Id;
                                GridColumnEntity.ColumnId = (int)column.ColumnNum;
                                GridColumnEntity.ColumnName = column.Heading;
                                var dict = await GetFieldTypeAndSize(oTable, column.FieldName, ConnectionString);
                                GridColumnEntity.ColumnDataType = dict["ColumnDataType"];
                                GridColumnEntity.ColumnMaxLength = dict["ColumnMaxLength"];
                                GridColumnEntity.IsPk = false;
                                GridColumnEntity.AutoInc = (bool)column.FilterField;
                                GridColumnEntities.Add(GridColumnEntity);
                            }

                        }

                    }
                }
                return GridColumnEntities;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        public async Task<Dictionary<string, string>> GetFieldTypeAndSize(Table oTables, string sFieldName, string ConnectionString)
        {

            string sTableName;

            var result = new Dictionary<string, string>();

            if (sFieldName.Contains("."))
            {
                sTableName = sFieldName.Substring(0, sFieldName.IndexOf('.'));
            }
            else
            {
                sTableName = oTables.TableName;
            }

            if (DatabaseMap.RemoveTableNameFromField(sFieldName) == "SLTrackedDestination" || DatabaseMap.RemoveTableNameFromField(sFieldName) == "SLFileRoomOrder")
            {
                var msFieldType = Common.FT_TEXT;
                var msFieldSize = Common.FT_MEMO_SIZE;

                result.Add("ColumnDataType", msFieldType);
                result.Add("ColumnMaxLength", msFieldSize);
                return result;
            }

            result = await BindTypeAndSize(ConnectionString, sFieldName, sTableName, oTables);
            return result;

        }

        public async Task<Dictionary<string, string>> BindTypeAndSize(string ConnectionString, string sFieldName, string sTableName, Table oTables = null)
        {
            var result = new Dictionary<string, string>();
            string argsSql = "Select * FROM [" + sTableName + "] WHERE 0 = 1";
            var oFields = await FieldWithOrWithoutTable(sFieldName, sTableName, ConnectionString);

            if (oFields is not null)
            {
                if (GetInfoUsingDapper.IsADateType(oFields.DATA_TYPE))
                {
                    result.Add("ColumnDataType", Common.FT_DATE);
                    result.Add("ColumnMaxLength", Common.FT_DATE_SIZE);
                }
                else if (GetInfoUsingDapper.IsAStringType(oFields.DATA_TYPE))
                {
                    if (oFields.CHARACTER_MAXIMUM_LENGTH <= 0 | oFields.CHARACTER_MAXIMUM_LENGTH >= 2000000)
                    {
                        result.Add("ColumnDataType", Common.FT_MEMO);
                        result.Add("ColumnMaxLength", Common.FT_MEMO_SIZE);
                    }
                    else
                    {
                        result.Add("ColumnDataType", Common.FT_TEXT);
                        result.Add("ColumnMaxLength", oFields.CHARACTER_MAXIMUM_LENGTH.ToString());
                    }
                }
                else
                {
                    switch (oFields.DATA_TYPE)
                    {
                        case "bit":
                            {
                                result.Add("ColumnDataType", Common.FT_BOOLEAN);
                                result.Add("ColumnMaxLength", Common.FT_BOOLEAN_SIZE);
                                break;
                            }
                        case "double":
                        case "currency":
                        case "decimal":
                        case "numeric":
                        case "single":
                        case "varnumeric":
                            {
                                result.Add("ColumnDataType", Common.FT_DOUBLE);
                                result.Add("ColumnMaxLength", Common.FT_DOUBLE_SIZE);
                                break;
                            }
                        case "bigint":
                        case "unsignedbigint":
                        case "int":
                            {
                                if (Convert.ToBoolean(oFields.IsAutoIncrement))
                                {
                                    result.Add("ColumnDataType", Common.FT_AUTO_INCREMENT);
                                    result.Add("ColumnMaxLength", Common.FT_AUTO_INCREMENT_SIZE);
                                }
                                else if (oTables is not null)
                                {
                                    if (!string.IsNullOrEmpty(oTables.CounterFieldName) && string.Compare(DatabaseMap.RemoveTableNameFromField(oFields.COLUMN_NAME), DatabaseMap.RemoveTableNameFromField(oTables.IdFieldName), StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        result.Add("ColumnDataType", Common.FT_SMEAD_COUNTER);
                                        result.Add("ColumnMaxLength", Common.FT_SMEAD_COUNTER_SIZE);
                                    }
                                    else
                                    {
                                        result.Add("ColumnDataType", Common.FT_LONG_INTEGER);
                                        result.Add("ColumnMaxLength", Common.FT_LONG_INTEGER_SIZE);
                                    }
                                }

                                break;
                            }
                        case "binary":
                            {
                                result.Add("ColumnDataType", Common.FT_BINARY);
                                result.Add("ColumnMaxLength", Common.FT_MEMO_SIZE);
                                break;
                            }
                        case "smallint":
                        case "tinyint":
                        case "short":
                            {
                                result.Add("ColumnDataType", Common.FT_SHORT_INTEGER);
                                result.Add("ColumnMaxLength", Common.FT_SHORT_INTEGER_SIZE);
                                break;
                            }
                    }
                }
            }
            oFields = null;
            return result;

        }

        public async Task<FiltereOperaterValue>  FillOperatorsDropDownOnChange(Dictionary<string, bool> filterControls, List<View> lView,
            List<Table> lTable, int iColumnNum, string TableName, string ConnectionString)
        {
            var result = new FiltereOperaterValue();
            var bLookup = default(bool);
            bool bDate = false;
            bool bYesNo = false;
            var lOperatorItems = new List<KeyValuePair<string, string>>();
            if (mcFilterColumns.Count != 0)
            {
                var oFilterColumns = mcFilterColumns.Where(m => m.Value.ColumnNum == iColumnNum).FirstOrDefault().Value;
                if (oFilterColumns is not null)
                {
                    bLookup = (bool)(Convert.ToInt32(Enums.geViewColumnsLookupType.ltUndefined) is var arg14 && oFilterColumns.LookupType is { } arg13 ? arg13 != arg14 : (bool?)null);
                    if (!bLookup)
                    {
                        if (oFilterColumns is not null)
                        {
                            string tableName = "";
                            if (string.IsNullOrEmpty(tableName))
                            {
                                if ((long)oFilterColumns.FieldName.IndexOf('.') > 1L)
                                {
                                    tableName = DatabaseMap.RemoveFieldNameFromField(oFilterColumns.FieldName);
                                }
                                else if ((oFilterColumns.ViewsId is null | (0 is var arg16 && oFilterColumns.ViewsId is { } arg15 ? arg15 == arg16 : (bool?)null)) == true)
                                {
                                    tableName = TableName;
                                }
                                else
                                {
                                    tableName = lView.Where(m => m.Id == oFilterColumns.ViewsId).FirstOrDefault()?.TableName ?? "";

                                }
                            }

                            if (!string.IsNullOrEmpty(tableName))
                            {
                                var tableObj = lTable.Where(m => m.TableName.Trim().ToLower().Equals(tableName.Trim().ToLower())).FirstOrDefault();
                                var oSchemaColumnList = SchemaInfoDetails.GetSchemaInfo(tableName, ConnectionString, DatabaseMap.RemoveTableNameFromField(oFilterColumns.FieldName));
                                var oSchemaColumn = new SchemaColumns();
                                foreach (var currentOSchemaColumn in oSchemaColumnList)
                                {
                                    oSchemaColumn = currentOSchemaColumn;
                                    if (string.Compare(oSchemaColumn.ColumnName, DatabaseMap.RemoveTableNameFromField(oFilterColumns.FieldName), StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        break;
                                    }
                                }

                                if (oSchemaColumn is not null)
                                {
                                    bYesNo = false;
                                    bDate = false;
                                    switch (oSchemaColumn.DataType)
                                    {
                                        case Enums.DataTypeEnum.rmBoolean:
                                        case Enums.DataTypeEnum.rmSmallInt:
                                        case Enums.DataTypeEnum.rmUnsignedSmallInt:
                                        case Enums.DataTypeEnum.rmTinyInt:
                                        case Enums.DataTypeEnum.rmUnsignedTinyInt:
                                            {
                                                bYesNo = true;
                                                break;
                                            }

                                        case Enums.DataTypeEnum.rmDate:
                                        case Enums.DataTypeEnum.rmDBDate:
                                        case Enums.DataTypeEnum.rmDBTime:
                                            {
                                                bDate = true;
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            filterControls.Add("FieldDDL", bLookup);
            filterControls.Add("chkYesNoField", bYesNo);
            filterControls.Add("FieldTextBox", !bLookup & !bYesNo);
            var lstKeyValuePair = FillOperatorsDropDown(lOperatorItems, true, bLookup, bYesNo, bDate);
            result.KeyValuePairs = lstKeyValuePair;
            result.DictionaryResult = filterControls;
            return result;
        }

        private async Task<CoulmnSchemaInfo> FieldWithOrWithoutTable(string sFieldName, string sTableName, string ConnectionString)
        {
            var columSchema = new CoulmnSchemaInfo();
            var fieldName = DatabaseMap.RemoveTableNameFromField(sFieldName);

            columSchema = await GetInfoUsingDapper.GetCoulmnSchemaInfo(ConnectionString, sTableName, fieldName);

            return columSchema;
        }

        private List<KeyValuePair<string, string>> FillOperatorsDropDown(List<KeyValuePair<string, string>> lOperatorItems, bool bIsString, bool bLookup = false, bool bYesNo = false, bool bDate = false)
        {
            try
            {
                lOperatorItems.Add(new KeyValuePair<string, string>("=", "="));
                if (!bLookup & !bYesNo)
                {
                    lOperatorItems.Add(new KeyValuePair<string, string>(">", ">"));
                    lOperatorItems.Add(new KeyValuePair<string, string>("<", "<"));
                    lOperatorItems.Add(new KeyValuePair<string, string>(">=", ">="));
                    lOperatorItems.Add(new KeyValuePair<string, string>("<=", "<="));
                }
                if (!bYesNo & !bDate)
                {
                    lOperatorItems.Add(new KeyValuePair<string, string>("<>", "<>"));
                    if (bIsString & !bLookup)
                    {
                        lOperatorItems.Add(new KeyValuePair<string, string>("In", "In"));
                        lOperatorItems.Add(new KeyValuePair<string, string>("BEG", "BEG"));
                    }
                }
            }
            catch (Exception)
            {

            }
            return lOperatorItems;
        }

    }
}