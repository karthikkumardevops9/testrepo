using System;
using System.Linq;

namespace MSRecordsEngine.Repository
{

    public sealed class RepositoryKeys
    {
        private static string strConn { get; set; }
        public static string StrConnection
        {
            get
            {
                return strConn;
            }
            set
            {
                strConn = value;
            }
        }

        public static int GetNextIdentityForBatchNumber(string strTableName)
        {
            var _db = new Repository.DBContextUnitOfWork();
            int intIdentityKeyValue = Convert.ToInt32(_db.Context.Database.SqlQuery<decimal>("Select IDENT_CURRENT('" + strTableName + "')", new object[0]).FirstOrDefault());
            return intIdentityKeyValue;
        }
    }
}