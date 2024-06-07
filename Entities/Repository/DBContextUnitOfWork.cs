using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Entities;

namespace MSRecordsEngine.Repository
{
    public class DBContextUnitOfWork : Repository.RepositoriesUnitOfWork
    {
        public DBContextUnitOfWork()
        {
            // Context = NEVER HARD CODE THIS VALUE.
            var session = new HttpContextAccessor().HttpContext;

            if (session.Session is not null)
            {
                string ConnectionString = session.Session.GetString("ConnectionString");
                if (ConnectionString is not null)
                {
                    string strConn = ConnectionString;
                    if (!strConn.Contains("MultipleActiveResultSets"))
                    {
                        strConn = strConn + ";MultipleActiveResultSets=True";
                    }
                    Repository.RepositoryKeys.StrConnection = strConn;
                    this.Context = new TABFusionRMSContext(strConn);
                }
                else
                {
                    this.Context = new TABFusionRMSContext();
                }
            }
            else
            {
                this.Context = new TABFusionRMSContext();
            }

        }

        public DBContextUnitOfWork(object strConn)
        {
            this.Context = new TABFusionRMSContext(Conversions.ToString(strConn));
        }
    }
}