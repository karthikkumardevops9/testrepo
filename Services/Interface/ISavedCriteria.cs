using MSRecordsEngine.Entities;
using MSRecordsEngine.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services.Interface
{
    public interface ISavedCriteria
    {
        public Task<Int32> SaveSavedCriteria(Int32 userId, string pErrorMessage, string FavouriteName, Int32 pViewId, string ConnectionString);
        public Task<bool> SaveSavedChildrenFavourite(string pErrorMessage, bool isNewRecord, Int32 ps_SavedCriteriaId, Int32 pViewId, List<string> lSelectedItemList, string ConnectionString);
        public Task<bool> DeleteSavedCriteria(Int32 id, string SavedCriteriaType, string ConnectionString);
        public Task<bool> DeleteFavouriteRecords(List<string> ids, Int32 savedCriteriaId, string ConnectionString);
    }
}
