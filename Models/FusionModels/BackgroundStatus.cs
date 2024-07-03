using Microsoft.AspNetCore.Http;
using MSRecordsEngine.Entities;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Repository;
using Smead.Security;
using System.Data.SqlClient;
using System;
using System.Linq;
using System.Data.Entity;
using MSRecordsEngine.Models;
using Microsoft.VisualBasic;

namespace MSRecordsEngine.Models.FusionModels
{
    public class BackgroundStatus
    {
        public static object ChangeNotification(int userId, string ConnectionString)
        {
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var backgroundStatusNotification = context.SLServiceTasks.Where(x => x.UserId == userId && x.IsNotification == true).ToList();
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
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return "True";
        }

        // Written for MVC model Moti Mashiah (since the data processing model written specific for aspx objec and controller MVC can't use it).
        public static bool InsertData(DataProcessingModel prop, string rowQuery, Passport passport)
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
                    if (ex.InnerException is not null)
                        prop.ErrorMessage += string.Format("{0}InnerException: {1}", Constants.vbCrLf, ex.InnerException.Message);
                    IsSuccess = false;
                    throw;
                }

                if (IsSuccess && slServiceTask.Id != 0)
                {
                    int returnVal;

                    try
                    {

                        var @params = new Parameters(prop.viewId, passport);

                        using (SqlConnection conn = passport.Connection())
                        {
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

                            using (var cmd = new SqlCommand(sqlQuery, conn))
                            {
                                cmd.CommandTimeout = 20 * 60; // 20 minutes 
                                returnVal = cmd.ExecuteNonQuery();
                            }
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
    }
}
