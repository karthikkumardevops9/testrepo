using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MSRecordsEngine.Repository
{


    // Namespace TabFusionRMS.Repository
    /// <summary>
    /// Used for dependency injection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {

        Repository.IUnitOfWork UnitOfWork { get; set; }

        IQueryable<T> All();

        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        void Add(T entity);

        void AddList(List<T> entity);

        void Update(T entity);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);

        IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);
        // Function ExecuteStoredProcedure(storedProcedureName As String, ParamArray parameters As SqlClient.SqlParameter()) As IEnumerable(Of T)

        void BeginTransaction();
        void RollBackTransaction();
        void CommitTransaction();

    }
}
// End Namespace