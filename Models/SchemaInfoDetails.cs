using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Data;
using MSRecordsEngine.Entities;
using Dapper;
using System.Linq;

namespace MSRecordsEngine.Models
{
    public class SchemaColumns
    {
        protected string msTableCatalog;
        protected string msTableSchema;
        protected string msTableName;
        protected string msColumnName;
        protected string msColumnGuid;
        protected string msColumnPropid;
        protected string msOrdinalPosition;
        protected string msColumnHasDefault;
        protected string msColumnDefault;
        protected string msColumnFlag;
        protected string msIsNullable;
        protected Enums.DataTypeEnum meDataType;
        protected string msTypeGuid;
        protected int miCharacterMaxLength;
        protected string msCharacterOctetLength;
        protected string msNumericPrecision;
        protected string msNumericScale;
        protected string msDataTimePrecision;
        protected string msCharacterSetCatalog;
        protected string msCharacterSetSchema;
        protected string msCharacterSetName;
        protected string msCollationCatalog;
        protected string msCollationSchema;
        protected string msCollationName;
        protected string msDomainCatalog;
        protected string msDomainSchema;
        protected string msDomainName;
        protected string msDescription;
        protected string msOrdinal;

        public string TableCatalog
        {
            get
            {
                return msTableCatalog;
            }
            set
            {
                msTableCatalog = Strings.Trim(value);
            }
        }

        public string TableSchema
        {
            get
            {
                return msTableSchema;
            }
            set
            {
                msTableSchema = Strings.Trim(value);
            }
        }

        public string TableName
        {
            get
            {
                return msTableName;
            }
            set
            {
                msTableName = Strings.Trim(value);
            }
        }

        public string ColumnName
        {
            get
            {
                return msColumnName;
            }
            set
            {
                msColumnName = Strings.Trim(value);
            }
        }

        public string ColumnGuid
        {
            get
            {
                return msColumnGuid;
            }
            set
            {
                msColumnGuid = Strings.Trim(value);
            }
        }

        public string ColumnPropid
        {
            get
            {
                return msColumnPropid;
            }
            set
            {
                msColumnPropid = Strings.Trim(value);
            }
        }

        public string OrdinalPosition
        {
            get
            {
                return msOrdinalPosition;
            }
            set
            {
                msOrdinalPosition = Strings.Trim(value);
            }
        }

        public string ColumnHasDefault
        {
            get
            {
                return msColumnHasDefault;
            }
            set
            {
                msColumnHasDefault = Strings.Trim(value);
            }
        }

        public string ColumnDefault
        {
            get
            {
                return msColumnDefault;
            }
            set
            {
                msColumnDefault = Strings.Trim(value);
            }
        }

        public string ColumnFlag
        {
            get
            {
                return msColumnFlag;
            }
            set
            {
                msColumnFlag = Strings.Trim(value);
            }
        }

        public string IsNullable
        {
            get
            {
                return msIsNullable;
            }
            set
            {
                msIsNullable = Strings.Trim(value);
            }
        }

        public Enums.DataTypeEnum DataType
        {
            get
            {
                return meDataType;
            }
            set
            {
                meDataType = value;
            }
        }

        public string TypeGuid
        {
            get
            {
                return msTypeGuid;
            }
            set
            {
                msTypeGuid = Strings.Trim(value);
            }
        }

        public int CharacterMaxLength
        {
            get
            {
                return miCharacterMaxLength;
            }
            set
            {
                miCharacterMaxLength = value;
            }
        }

        public string CharacterOctetLength
        {
            get
            {
                return msCharacterOctetLength;
            }
            set
            {
                msCharacterOctetLength = Strings.Trim(value);
            }
        }

        public string NumericPrecision
        {
            get
            {
                return msNumericPrecision;
            }
            set
            {
                msNumericPrecision = Strings.Trim(value);
            }
        }

        public string NumericScale
        {
            get
            {
                return msNumericScale;
            }
            set
            {
                msNumericScale = Strings.Trim(value);
            }
        }

        public string DataTimePrecision
        {
            get
            {
                return msDataTimePrecision;
            }
            set
            {
                msDataTimePrecision = Strings.Trim(value);
            }
        }

        public string CharacterSetCatalog
        {
            get
            {
                return msCharacterSetCatalog;
            }
            set
            {
                msCharacterSetCatalog = Strings.Trim(value);
            }
        }

        public string CharacterSetSchema
        {
            get
            {
                return msCharacterSetSchema;
            }
            set
            {
                msCharacterSetSchema = Strings.Trim(value);
            }
        }

        public string CharacterSetName
        {
            get
            {
                return msCharacterSetName;
            }
            set
            {
                msCharacterSetName = Strings.Trim(value);
            }
        }

        public string CollationCatalog
        {
            get
            {
                return msCollationCatalog;
            }
            set
            {
                msCollationCatalog = Strings.Trim(value);
            }
        }

        public string CollationSchema
        {
            get
            {
                return msCollationSchema;
            }
            set
            {
                msCollationSchema = Strings.Trim(value);
            }
        }

        public string CollationName
        {
            get
            {
                return msCollationName;
            }
            set
            {
                msCollationName = Strings.Trim(value);
            }
        }

        public string DomainCatalog
        {
            get
            {
                return msDomainCatalog;
            }
            set
            {
                msDomainCatalog = Strings.Trim(value);
            }
        }

        public string DomainSchema
        {
            get
            {
                return msDomainSchema;
            }
            set
            {
                msDomainSchema = Strings.Trim(value);
            }
        }

        public string DomainName
        {
            get
            {
                return msDomainName;
            }
            set
            {
                msDomainName = Strings.Trim(value);
            }
        }

        public string Description
        {
            get
            {
                return msDescription;
            }
            set
            {
                msDescription = Strings.Trim(value);
            }
        }

        public string Ordinal
        {
            get
            {
                return msOrdinal;
            }
            set
            {
                msOrdinal = Strings.Trim(value);
            }
        }

        public bool IsString
        {
            get
            {
                return SchemaInfoDetails.IsAStringType(meDataType);
            }
        }

        public bool IsADate
        {
            get
            {
                return SchemaInfoDetails.IsADateType(meDataType);
            }
        }

    }

    public sealed class SchemaInfoDetails : SchemaColumns
    {
        private static IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);

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

        public static bool IsANumericType(Enums.DataTypeEnum eDataType)
        {
            switch (eDataType)
            {
                case Enums.DataTypeEnum.rmTinyInt:
                case Enums.DataTypeEnum.rmUnsignedTinyInt:
                case Enums.DataTypeEnum.rmSmallInt:
                case Enums.DataTypeEnum.rmUnsignedSmallInt:
                case Enums.DataTypeEnum.rmInteger:
                case Enums.DataTypeEnum.rmBigInt:
                case Enums.DataTypeEnum.rmUnsignedInt:
                case Enums.DataTypeEnum.rmUnsignedBigInt:
                case Enums.DataTypeEnum.rmSingle:
                case Enums.DataTypeEnum.rmDouble:
                case Enums.DataTypeEnum.rmDecimal:
                case Enums.DataTypeEnum.rmNumeric:
                case Enums.DataTypeEnum.rmVarNumeric:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        public static bool IsSystemField(string sFieldName)
        {
            return sFieldName.Substring(0, 1) == "%";
        }

        public static List<SchemaColumns> GetSchemaInfo(string sTableName, string ConnectionString, string sIdFieldName = "")
        {
            var list = new List<SchemaColumns>();
            var parma = new SchemaObject();
            if (string.IsNullOrEmpty(Strings.Trim(sIdFieldName)))
            {
                parma.table_name = sTableName;
                parma.table_schema = null;
                parma.column_name = null;

            }
            else if (!string.IsNullOrEmpty(sTableName) && !string.IsNullOrEmpty(sIdFieldName))
            {
                parma.table_name = sTableName;
                parma.table_schema = null;
                parma.column_name = sIdFieldName;
            }
            else
            {
                parma.table_name = null;
                parma.table_schema = null;
                parma.column_name = sIdFieldName;
            }

            using (var conn = CreateConnection(ConnectionString))
            {
                var getschemslist = conn.Query("sp_columns_rowset", parma, commandType: System.Data.CommandType.StoredProcedure);
                foreach (var row in getschemslist.ToList())
                {
                    var item = new SchemaColumns();
                    item.TableCatalog = Convert.ToString(row.TABLE_CATALOG);
                    item.TableSchema = Convert.ToString(row.TABLE_SCHEMA);
                    item.TableName = Convert.ToString(row.TABLE_NAME);
                    item.ColumnName = Convert.ToString(row.COLUMN_NAME);
                    item.ColumnGuid = Convert.ToString(row.COLUMN_GUID);
                    item.ColumnPropid = Convert.ToString(row.COLUMN_PROPID);
                    item.ColumnHasDefault = Convert.ToString(row.COLUMN_HASDEFAULT);
                    item.ColumnDefault = Convert.ToString(row.COLUMN_DEFAULT);
                    item.ColumnFlag = Convert.ToString(row.COLUMN_FLAGS);
                    item.IsNullable = Convert.ToString(row.IS_NULLABLE);
                    item.DataType = (Enums.DataTypeEnum)row.DATA_TYPE;
                    item.TypeGuid = Convert.ToString(row.TYPE_GUID);
                    if (row.CHARACTER_MAXIMUM_LENGTH == null)
                    {
                        item.CharacterMaxLength = 0;
                    }
                    else
                    {
                        item.CharacterMaxLength = (int)row.CHARACTER_MAXIMUM_LENGTH;
                    }
                    item.CharacterOctetLength = Convert.ToString(row.CHARACTER_OCTET_LENGTH);
                    item.NumericPrecision = Convert.ToString(row.NUMERIC_PRECISION);
                    item.NumericScale = Convert.ToString(row.NUMERIC_SCALE);
                    item.DataTimePrecision = Convert.ToString(row.DATETIME_PRECISION);
                    item.CharacterSetCatalog = Convert.ToString(row.CHARACTER_SET_CATALOG);
                    item.CharacterSetSchema = Convert.ToString(row.CHARACTER_SET_SCHEMA);
                    item.CharacterSetName = Convert.ToString(row.CHARACTER_SET_NAME);
                    item.CollationCatalog = Convert.ToString(row.COLLATION_CATALOG);
                    item.CollationSchema = Convert.ToString(row.COLLATION_SCHEMA);
                    item.CollationName = Convert.ToString(row.COLLATION_NAME);
                    item.DomainCatalog = Convert.ToString(row.DOMAIN_CATALOG);
                    item.DomainSchema = Convert.ToString(row.DOMAIN_SCHEMA);
                    item.DomainName = Convert.ToString(row.DOMAIN_NAME);
                    item.Description = Convert.ToString(row.DESCRIPTION);
                    item.Ordinal = Convert.ToString(row.COLUMN_LCID);
                    list.Add(item);
                }
            }
            return list;
        }
        public static List<SchemaColumns> GetTableSchemaInfo(string sTableName, string ConnectionString)
        {
            var list = new List<SchemaColumns>();
            var parma = new SchemaObject();
            parma.table_name = sTableName;
            parma.table_schema = null;
            parma.column_name = null;
            using (var conn = CreateConnection(ConnectionString))
            {
                var getschemslist = conn.Query("sp_columns_rowset", parma, commandType: System.Data.CommandType.StoredProcedure);
                foreach (var row in getschemslist.ToList())
                {
                    var item = new SchemaColumns();
                    item.TableCatalog = Convert.ToString(row.TABLE_CATALOG);
                    item.TableSchema = Convert.ToString(row.TABLE_SCHEMA);
                    item.TableName = Convert.ToString(row.TABLE_NAME);
                    item.ColumnName = Convert.ToString(row.COLUMN_NAME);
                    item.ColumnGuid = Convert.ToString(row.COLUMN_GUID);
                    item.ColumnPropid = Convert.ToString(row.COLUMN_PROPID);
                    item.ColumnHasDefault = Convert.ToString(row.COLUMN_HASDEFAULT);
                    item.ColumnDefault = Convert.ToString(row.COLUMN_DEFAULT);
                    item.ColumnFlag = Convert.ToString(row.COLUMN_FLAGS);
                    item.IsNullable = Convert.ToString(row.IS_NULLABLE);
                    item.DataType = (Enums.DataTypeEnum)row.DATA_TYPE;
                    item.TypeGuid = Convert.ToString(row.TYPE_GUID);
                    if (row.CHARACTER_MAXIMUM_LENGTH == null)
                    {
                        item.CharacterMaxLength = 0;
                    }
                    else
                    {
                        item.CharacterMaxLength = (int)row.CHARACTER_MAXIMUM_LENGTH;
                    }
                    item.CharacterOctetLength = Convert.ToString(row.CHARACTER_OCTET_LENGTH);
                    item.NumericPrecision = Convert.ToString(row.NUMERIC_PRECISION);
                    item.NumericScale = Convert.ToString(row.NUMERIC_SCALE);
                    item.DataTimePrecision = Convert.ToString(row.DATETIME_PRECISION);
                    item.CharacterSetCatalog = Convert.ToString(row.CHARACTER_SET_CATALOG);
                    item.CharacterSetSchema = Convert.ToString(row.CHARACTER_SET_SCHEMA);
                    item.CharacterSetName = Convert.ToString(row.CHARACTER_SET_NAME);
                    item.CollationCatalog = Convert.ToString(row.COLLATION_CATALOG);
                    item.CollationSchema = Convert.ToString(row.COLLATION_SCHEMA);
                    item.CollationName = Convert.ToString(row.COLLATION_NAME);
                    item.DomainCatalog = Convert.ToString(row.DOMAIN_CATALOG);
                    item.DomainSchema = Convert.ToString(row.DOMAIN_SCHEMA);
                    item.DomainName = Convert.ToString(row.DOMAIN_NAME);
                    item.Description = Convert.ToString(row.DESCRIPTION);
                    item.Ordinal = Convert.ToString(row.COLUMN_LCID);
                    list.Add(item);
                }

            }

            return list;
        }
        public List<SchemaTableColumnObject> GetColumnsSchema(string table, string ConnectionString)
        {
            var query = "SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE,  CASE WHEN COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 THEN 'yes' ELSE 'no' END AS IsAutoIncrement FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table";

            var model = new List<SchemaTableColumnObject>();
            try
            {
                var param = new { @table = table };

                using (var conn = CreateConnection(ConnectionString))
                {
                    model = conn.Query<SchemaTableColumnObject>(query, param, commandType: CommandType.Text).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return model;
        }
    }

    public class SchemaObject
    {
        public string @table_name { get; set; }
        public string @table_schema { get; set; }
        public string @column_name { get; set; }
    }
    public class SchemaTableObject
    {
        public string @table_name { get; set; }
        public string @table_schema { get; set; }
        public object @table_type { get; set; }
    }
    public class SchemaTableColumnObject
    {
        public string TABLE_SCHEMA { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
        public string CHARACTER_MAXIMUM_LENGTH { get; set; }
        public string IS_NULLABLE { get; set; }
        public string IsAutoIncrement { get; set; }
    }
}
