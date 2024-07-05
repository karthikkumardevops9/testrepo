using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using MSRecordsEngine.Entities;
using MSRecordsEngine.RecordsManager;
using System;
using MSRecordsEngine.Models;
using System.Data.Entity;
using System.Linq;
using Dapper;

namespace MSRecordsEngine.Services
{
    public class BackgroundStatusService : IBackgroundStatusService
    {
        private IDbConnection CreateConnection(string connectionString)
        => new SqlConnection(connectionString);
        public async Task<bool> InsertData(DataProcessingModel prop, string rowQuery, Passport passport)
        {

            bool IsSuccess = true;
            var slServiceTask = new SLServiceTask();

            try
            {
                try
                {
                    using (var context = new TABFusionRMSContext(passport.ConnectionString))
                    {
                        slServiceTask.Type = prop.FileName;
                        slServiceTask.EMailAddress = Navigation.GetUserEmail(passport);
                        slServiceTask.TaskType = prop.TaskType;
                        slServiceTask.UserName = passport.LoggedInUserName;
                        slServiceTask.UserId = passport.UserId;
                        slServiceTask.Status = Enum.GetName(typeof(Enums.BackgroundTaskStatus), 1);
                        slServiceTask.RecordCount = prop.RecordCount;
                        slServiceTask.ViewId = prop.viewId;
                        slServiceTask.CreateDate = DateTime.Now;
                        slServiceTask.DestinationTableName = prop.DestinationTableName;
                        slServiceTask.DestinationTableId = prop.DestinationTableId;
                        slServiceTask.IsNotification = false;
                        slServiceTask.Reconciliation = prop.Reconciliation;
                        if (!string.IsNullOrEmpty(prop.DueBackDate))
                            slServiceTask.DueBackDate = DateTime.Parse(prop.DueBackDate);

                        if (prop.TaskType is (int)Enums.BackgroundTaskInDetail.ExportCSV or (int)Enums.BackgroundTaskInDetail.ExportTXT)
                        {
                            slServiceTask.DownloadLocation = prop.Path;
                            slServiceTask.ReportLocation = prop.ExportReportPath;
                        }
                        else
                        {
                            slServiceTask.ReportLocation = prop.Path;
                        }

                        context.SLServiceTasks.Add(slServiceTask);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    prop.ErrorMessage = ex.Message;
                    if (ex.InnerException != null)
                        prop.ErrorMessage += string.Format("{0}InnerException: {1}", Environment.NewLine, ex.InnerException.Message);
                    IsSuccess = false;
                    throw;
                }

                if (IsSuccess && slServiceTask.Id != 0)
                {
                    int returnVal;

                    try
                    {
                        var queryObj = new Query(passport);
                        var @params = new Parameters(prop.viewId, passport);

                        
                            string sqlQuery = string.Empty;
                            string selectSql = string.Format("SELECT {0}, '{1}', [{2}]", slServiceTask.Id, @params.TableName, @params.KeyField);

                            if (prop.IsSelectAllData)
                            {
                                if (string.IsNullOrEmpty(rowQuery))
                                {
                                    string sqlViewName = Query.GetSQLViewName(@params, false);
                                    sqlQuery = string.Format("INSERT INTO [SLServiceTaskItems] ([SLServiceTaskId], [TableName], [TableId]) {0} FROM [{1}]", selectSql, sqlViewName);
                                }
                                else
                                {
                                    int fromIndex = rowQuery.ToUpper().IndexOf(" FROM ");

                                    if (fromIndex > 0)
                                    {
                                        sqlQuery = string.Format("INSERT INTO [SLServiceTaskItems] ([SLServiceTaskId], [TableName], [TableId]) {0} {1}", selectSql, rowQuery.Substring(fromIndex));
                                    }
                                    else
                                    {
                                        string sqlViewName = Query.GetSQLViewName(@params, false);
                                        sqlQuery = string.Format("INSERT INTO [SLServiceTaskItems] ([SLServiceTaskId], [TableName], [TableId]) {0} FROM [{1}]", selectSql, sqlViewName);
                                    }
                                }
                            }
                            else
                            {
                                string value = string.Empty;
                                string sqlViewName = Query.GetSQLViewName(@params, false);

                                if (@params.IdFieldDataType.FullName.ToLower().Contains("system.int"))
                                {
                                    value = string.Join(",", prop.ListofselectedIds);
                                }
                                else if (@params.IdFieldDataType.FullName.ToLower().Contains("system.string"))
                                {
                                    value = string.Format("'{0}'", string.Join("','", prop.ListofselectedIds));
                                }

                                sqlQuery = string.Format("INSERT INTO [SLServiceTaskItems] ([SLServiceTaskId], [TableName], [TableId]) {0} FROM [{1}] WHERE [{2}] IN ({3})", selectSql, sqlViewName, @params.KeyField, value);
                            }

                            using (var cmd = CreateConnection(passport.ConnectionString))
                            {
                                returnVal = await cmd.ExecuteAsync(sqlQuery, commandTimeout: 20 * 60);
                            }
                        
                    }
                    catch (Exception ex)
                    {
                        prop.ErrorMessage = ex.Message;
                        IsSuccess = false;
                        throw;
                    }

                    if (returnVal <= 0)
                    {
                        if (IsSuccess)
                            if (string.IsNullOrWhiteSpace(prop.ErrorMessage))
                                prop.ErrorMessage = "Error";
                        IsSuccess = false;
                    }
                    else
                    {
                        IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                prop.ErrorMessage = ex.Message;
                IsSuccess = false;
                throw;
            }

            return IsSuccess;
        }

        public async Task<object> ChangeNotification(int userId, string ConnectionString)
        {
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var backgroundStatusNotification = await context.SLServiceTasks.Where(x => x.UserId == userId && x.IsNotification == true).ToListAsync();
                if (backgroundStatusNotification.Count > 0)
                {
                    try
                    {
                        var slServiceTask = new SLServiceTask();
                        foreach (var item in backgroundStatusNotification)
                        {
                            item.IsNotification = false;
                            context.Entry(item).State = EntityState.Modified;
                        }
                        await context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return "True";
        }
    }
}
