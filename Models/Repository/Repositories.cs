using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace MSRecordsEngine.Repository
{

    // Namespace TabFusionRMS.Repository
    public class Repositories<T> : Repository.IRepository<T> where T : class
    {
        /// <summary>
        /// Created property for data base connection initialize
        /// </summary>
        public Repository.IUnitOfWork UnitOfWork
        {
            get
            {
                return m_UnitOfWork;
            }
            set
            {
                m_UnitOfWork = value;
            }
        }
        private Repository.IUnitOfWork m_UnitOfWork;

        public Repositories()
        {
            var _db = new Repository.DBContextUnitOfWork();
            UnitOfWork = _db;

            // Dim strCon = RepositoryKeys.StrConnection
            // Dim _db As New DBContextUnitOfWork(strCon)
            // Me.UnitOfWork = _db

        }

        /// <summary>
        /// Initailize object set according to the entity (ex. database table (tbl_login) data )
        /// </summary>
        private IDbSet<T> _objectset;
        private IDbSet<T> _objectsetTransaction;

        /// <summary>
        /// get object set according to the entity (ex. database table (tbl_login) data )
        /// </summary>
        private IDbSet<T> ObjectSet
        {
            get
            {
                if (_objectset is null)
                {
                    _objectset = UnitOfWork.Context.Set<T>();
                }
                return _objectset;
            }
        }

        private DbContextTransaction ObjectSetTransaction
        {
            get
            {
                return m_ObjectSetTransaction;
            }
            set
            {
                m_ObjectSetTransaction = value;
            }
        }
        private DbContextTransaction m_ObjectSetTransaction;

        /// <summary>
        /// Retrive all data from passing dynamic entities
        /// </summary>
        /// <returns>Entities</returns>
        public virtual IQueryable<T> All()
        {
            m_UnitOfWork.LazyLoadingEnabled = false;

            return ObjectSet.AsQueryable();
        }

        /// <summary>
        /// Retrive data from passing dynamic entities base on where condition on entities
        /// </summary>
        /// <param name="expression">Where condition</param>
        /// <returns>Entities</returns>
        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return ObjectSet.Where(expression);
        }

        /// <summary>
        /// Insert data in entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Add(T entity)
        {
            ObjectSet.Add(entity);
            UnitOfWork.Commit();
        }

        /// <summary>
        /// Insert data in entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void AddList(List<T> entity)
        {
            UnitOfWork.Context.Set<T>().AddRange(entity);
            UnitOfWork.Commit();
        }

        /// <summary>
        /// Update data in entity
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {

            UnitOfWork.Context.Entry(entity).State = EntityState.Modified;

            // ObjectSet.Remove(entity)
            // ObjectSet.Attach(entity)
            // UnitOfWork.Context.Entry(entity).CurrentValues.SetValues(entity)

            // UnitOfWork.Context.Entry(entity).Reload()
            // db.Entry(v).CurrentValues.SetValues(model);

            UnitOfWork.Commit();
        }

        /// <summary>
        /// Delete data in entity
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            ObjectSet.Remove(entity);
            UnitOfWork.Commit();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            UnitOfWork.Context.Set<T>().RemoveRange(entities);
            // db.[Set](Of T)().RemoveRange(entities)
            UnitOfWork.Commit();
        }

        public void BeginTransaction()
        {
            ObjectSetTransaction = UnitOfWork.Context.Database.BeginTransaction();
        }

        public void RollBackTransaction()
        {
            ObjectSetTransaction.Rollback();
            ObjectSetTransaction.Dispose();

        }
        public void CommitTransaction()
        {
            ObjectSetTransaction.Commit();
            ObjectSetTransaction.Dispose();
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return UnitOfWork.Context.Database.SqlQuery<T>(query, parameters);
        }

        // Public Function ExecuteStoredProcedure(storedProcedureName As String, ParamArray parameters As SqlClient.SqlParameter()) As IEnumerable(Of T) Implements IRepository(Of T).ExecuteStoredProcedure
        // Dim spSignature = New StringBuilder()
        // Dim spParameters As Object()
        // Dim hasTableVariables As Boolean = parameters.Any(Function(p) p.SqlDbType = SqlDbType.Structured)

        // spSignature.AppendFormat("EXECUTE {0}", storedProcedureName)
        // Dim length = parameters.Count() - 1

        // If hasTableVariables Then
        // Dim tableValueParameters = New List(Of SqlClient.SqlParameter)()

        // For i As Integer = 0 To parameters.Count() - 1
        // Select Case parameters(i).SqlDbType
        // Case SqlDbType.Structured
        // spSignature.AppendFormat(" @{0}", parameters(i).ParameterName)
        // tableValueParameters.Add(parameters(i))
        // Exit Select
        // Case SqlDbType.VarChar, SqlDbType.[Char], SqlDbType.Text, SqlDbType.NVarChar, SqlDbType.NChar, SqlDbType.NText, _
        // SqlDbType.Xml, SqlDbType.UniqueIdentifier, SqlDbType.Time, SqlDbType.[Date], SqlDbType.DateTime, SqlDbType.DateTime2, _
        // SqlDbType.DateTimeOffset, SqlDbType.SmallDateTime
        // ' TODO: some magic here to avoid SQL injections
        // spSignature.AppendFormat(" '{0}'", parameters(i).Value.ToString())
        // Exit Select
        // Case Else
        // spSignature.AppendFormat(" {0}", parameters(i).Value.ToString())
        // Exit Select
        // End Select

        // If i <> length Then
        // spSignature.Append(",")
        // End If
        // Next
        // spParameters = tableValueParameters.Cast(Of Object)().ToArray()
        // Else
        // For i As Integer = 0 To parameters.Count() - 1
        // spSignature.AppendFormat(" @{0}", parameters(i).ParameterName)
        // If i <> length Then
        // spSignature.Append(",")
        // End If
        // Next
        // spParameters = parameters.Cast(Of Object)().ToArray()
        // End If

        // Dim _db As New DBContextUnitOfWork()


        // Dim query = _db.Context.Database.SqlQuery(Of T)("GetTableData", spParameters)


        // Dim list = query.AsEnumerable()
        // Return list
        // End Function

    }
}
// End Namespace