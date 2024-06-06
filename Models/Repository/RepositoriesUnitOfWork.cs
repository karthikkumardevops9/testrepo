
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace MSRecordsEngine.Repository
{

    // Namespace TabFusionRMS.Repository
    public class RepositoriesUnitOfWork : Repository.IUnitOfWork
    {
        /// <summary>
        /// Created property for Data context
        /// </summary>
        public DbContext Context
        {
            get
            {
                return m_Context;
            }
            set
            {
                m_Context = value;
            }
        }
        private DbContext m_Context;

        /// <summary>
        /// Default constructor
        /// </summary>

        public RepositoriesUnitOfWork()
        {
        }

        /// <summary>
        /// When every transaction completed commit is called.
        /// </summary>
        public void Commit()
        {
            bool saveFailed = false;
            do
            {
                saveFailed = false;
                try
                {
                    Context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList().ForEach(entry =>
                    {
                        // To work around it...
                        var currentValues = entry.CurrentValues.Clone();
                        entry.Reload();
                        entry.CurrentValues.SetValues(currentValues);
                    });
                }
            }
            while (saveFailed);
        }

        /// <summary>
        /// Heavy data is loaded when page is scrolled.
        /// </summary>
        public bool LazyLoadingEnabled
        {
            get
            {
                return Context.Configuration.LazyLoadingEnabled;
            }
            set
            {
                Context.Configuration.LazyLoadingEnabled = value;
            }
        }

        /// <summary>
        /// Dispose is called to clear out memory
        /// </summary>
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
// End Namespace