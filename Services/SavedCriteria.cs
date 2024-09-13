using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Repository;
using MSRecordsEngine.Services.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecordsEngine.Services
{
    //public class SavedCriteria : ISavedCriteria
    //{
    //    public async Task<bool> DeleteSavedCriteria(Int32 id, string SavedCriteriaType, string ConnectionString)
    //    {
    //        try
    //        {
    //            using (var context = new TABFusionRMSContext(ConnectionString))
    //            {
    //                var savedCriteria = await context.s_SavedCriteria.Where(x => x.Id == id).FirstOrDefaultAsync();
    //                if (savedCriteria != null)
    //                {
    //                    context.s_SavedCriteria.Remove(savedCriteria);
    //                    await context.SaveChangesAsync();
    //                    if (SavedCriteriaType == "1")
    //                    {
    //                        var s_s_SavedChildrenFavoriteList = await context.s_SavedChildrenFavorite.Where(x => x.SavedCriteriaId == id).ToListAsync();
    //                        if (s_s_SavedChildrenFavoriteList != null)
    //                            context.s_SavedChildrenFavorite.RemoveRange(s_s_SavedChildrenFavoriteList);
    //                        await context.SaveChangesAsync();
    //                    }
    //                    else
    //                    {
    //                        var odjdel = await context.s_SavedChildrenQuery.Where(x => x.SavedCriteriaId == id).ToListAsync();
    //                        if (odjdel != null)
    //                            context.s_SavedChildrenQuery.RemoveRange(odjdel);
    //                        await context.SaveChangesAsync();
    //                    }
    //                }
    //                return true;
    //            }
                
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine(ex.Message);
    //            return false;
    //        }
    //    }

    //    public async Task<bool> DeleteFavouriteRecords(List<string> ids, Int32 savedCriteriaId, string ConnectionString)
    //    {
    //        try
    //        {
    //            using (var context = new TABFusionRMSContext(ConnectionString))
    //            {
    //                var SavedChildrenFavoriteList = await context.s_SavedChildrenFavorite.Where(m => m.SavedCriteriaId == savedCriteriaId && ids.Contains(m.TableId)).ToListAsync();
    //                if (SavedChildrenFavoriteList != null)
    //                    context.s_SavedChildrenFavorite.RemoveRange(SavedChildrenFavoriteList);
    //                await context.SaveChangesAsync();
    //                return true;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine(ex.Message);
    //            return false;
    //        }
    //    }


    //    //other methods are used in import controller and import fav controller
    //}
}
