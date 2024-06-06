

using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;
using System;

namespace MSRecordsEngine.RecordsManager
{
    public partial class ViewsTableAdapter {
    }
    public partial class TablesTableAdapter
    {
        public partial class SmeadCounter
        {
            public string FieldName;
            public int CounterValue;
        }

        public static SmeadCounter TakeNextSmeadCounterTableID(string tableName, object requestedID, SqlConnection connection)
        {
            SqlCommand cmd;
            var cnt = default(int);
            var da = new SqlDataAdapter();
            var dt = new DataTable();
            string IdString = string.Empty;
            if (requestedID is not null)
                IdString = requestedID.ToString();

            cmd = new SqlCommand();
            cmd.CommandText = "SELECT ISNULL([CounterFieldName],'') as CounterFieldName, [IdFieldName] from [Tables] WHERE [TableName]=@tableName";
            cmd.Parameters.AddWithValue("@tableName", tableName);
            cmd.Connection = connection;
            da.SelectCommand = cmd;
            da.Fill(dt);

            if (dt.Rows[0]["CounterFieldName"].ToString().Length > 0)
            {
                if (IdString.Length > 0)
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM [" + tableName + "] WHERE [" + Navigation.MakeSimpleField(dt.Rows[0]["IdFieldName"].ToString()) + "]=@requestedID";
                    cmd.Parameters.AddWithValue("@requestedID", IdString);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        throw new Exception("ID '" + IdString + "' is already in use.  Select a different value.");

                    if (!Information.IsNumeric(IdString))
                    {
                        cnt = -1;
                    }
                    else
                    {
                        cmd.CommandText = "SELECT [" + dt.Rows[0]["CounterFieldName"].ToString() + "] FROM [System]";
                        cnt = Convert.ToInt32(cmd.ExecuteScalar());

                        if (cnt <= Convert.ToInt32(IdString))
                        {
                            cmd.CommandText = "UPDATE [System] SET [" + dt.Rows[0]["CounterFieldName"].ToString() + "] = " + (Convert.ToInt32(IdString) + 1);
                            cmd.ExecuteNonQuery();
                        }

                        cnt = Convert.ToInt32(IdString);
                    }
                }
                else
                {
                    cmd.CommandText = "SELECT [" + dt.Rows[0]["CounterFieldName"].ToString() + "] FROM [System]";
                    cnt = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.CommandText = "UPDATE [System] SET [" + dt.Rows[0]["CounterFieldName"].ToString() + "] = " + (cnt + 1);
                    cmd.ExecuteNonQuery();
                }
            }

            var smeadcounter = new SmeadCounter();
            smeadcounter.CounterValue = cnt;
            smeadcounter.FieldName = Navigation.MakeSimpleField(dt.Rows[0]["IdFieldName"].ToString());
            return smeadcounter;
        }
    }
}
