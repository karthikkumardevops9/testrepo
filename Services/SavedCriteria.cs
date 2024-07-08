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
    public class SavedCriteria : ISavedCriteria
    {
        public async Task<Int32> SaveSavedCriteria(Int32 userId, string pErrorMessage, string FavouriteName, Int32 pViewId, string ConnectionString)
        {
            s_SavedCriteria ps_SavedCriteria = new s_SavedCriteria();
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {

                    ps_SavedCriteria.UserId = userId;
                    ps_SavedCriteria.SavedName = FavouriteName;
                    ps_SavedCriteria.SavedType = (int)Enums.SavedType.Favorite;
                    ps_SavedCriteria.ViewId = pViewId;
                    context.s_SavedCriteria.Add(ps_SavedCriteria);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                pErrorMessage = ex.Message;
                return -1;
            }
            return ps_SavedCriteria.Id;
        }

        public async Task<bool> SaveSavedChildrenFavourite(string pErrorMessage, bool isNewRecord, Int32 ps_SavedCriteriaId, Int32 pViewId, List<string> lSelectedItemList, string ConnectionString)
        {
            var IsSuccess = false;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    List<s_SavedChildrenFavorite> ls_SavedChildrenFavorite = new List<s_SavedChildrenFavorite>();
                    var Lists_SavedChildrenFavorite = await context.s_SavedChildrenFavorite.ToListAsync();
                    var Lists_SavedCriteria = await context.s_SavedCriteria.ToListAsync();

                    var finalOutPut = from child in Lists_SavedChildrenFavorite
                                      join par in Lists_SavedCriteria
                                      on child.SavedCriteriaId equals par.Id
                                      where par.Id == Convert.ToInt32(child.SavedCriteriaId)
                                      select new { par.ViewId, child.TableId, par.Id };

                    foreach (string tableId in lSelectedItemList)
                    {
                        if (isNewRecord | !(finalOutPut.Any(x => x.TableId == tableId && x.ViewId == pViewId && x.Id == ps_SavedCriteriaId)))
                        {
                            s_SavedChildrenFavorite ps_SavedChildrenFavorite = new s_SavedChildrenFavorite();
                            ps_SavedChildrenFavorite.SavedCriteriaId = ps_SavedCriteriaId;
                            ps_SavedChildrenFavorite.TableId = tableId;
                            ls_SavedChildrenFavorite.Add(ps_SavedChildrenFavorite);
                            await context.SaveChangesAsync();
                        }
                    }

                    context.s_SavedChildrenFavorite.AddRange(ls_SavedChildrenFavorite);
                    await context.SaveChangesAsync();

                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                pErrorMessage = ex.Message;
                IsSuccess = false;
            }
            return IsSuccess;
        }

        public async Task<bool> DeleteSavedCriteria(Int32 id, string SavedCriteriaType, string ConnectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var savedCriteria = await context.s_SavedCriteria.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (savedCriteria != null)
                    {
                        context.s_SavedCriteria.Remove(savedCriteria);
                        await context.SaveChangesAsync();
                        if (SavedCriteriaType == "1")
                        {
                            var s_s_SavedChildrenFavoriteList = await context.s_SavedChildrenFavorite.Where(x => x.SavedCriteriaId == id).ToListAsync();
                            if (s_s_SavedChildrenFavoriteList != null)
                                context.s_SavedChildrenFavorite.RemoveRange(s_s_SavedChildrenFavoriteList);
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            var odjdel = await context.s_SavedChildrenQuery.Where(x => x.SavedCriteriaId == id).ToListAsync();
                            if (odjdel != null)
                                context.s_SavedChildrenQuery.RemoveRange(odjdel);
                            await context.SaveChangesAsync();
                        }
                    }
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteFavouriteRecords(List<string> ids, Int32 savedCriteriaId, string ConnectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var SavedChildrenFavoriteList = await context.s_SavedChildrenFavorite.Where(m => m.SavedCriteriaId == savedCriteriaId && ids.Contains(m.TableId)).ToListAsync();
                    if (SavedChildrenFavoriteList != null)
                        context.s_SavedChildrenFavorite.RemoveRange(SavedChildrenFavoriteList);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }


        //other methods are used in import controller and import fav controller
    }
}
