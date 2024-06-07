using System;
using System.Data.Entity;

namespace MSRecordsEngine.Repository
{

    // Namespace TabFusionRMS.Repository
    /// <summary>
    /// Handle complete database operation 
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {

        DbContext Context { get; set; }

        void Commit();

        bool LazyLoadingEnabled { get; set; }
    }
}
// End Namespace