using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualBasic.CompilerServices;
using Smead.Security;
namespace MSRecordsEngine.RecordsManager
{
    public class FolderStatus
    {

        public class FolderStatusType
        {
            private string _userName;
            public string UserName
            {
                get
                {
                    return _userName;
                }
                set
                {
                    _userName = value;
                }
            }
            private DateTime _statusTime;
            public DateTime StatusTime
            {
                get
                {
                    return _statusTime;
                }
                set
                {
                    _statusTime = value;
                }
            }
            private string _status;
            public string Status
            {
                get
                {
                    return _status;
                }
                set
                {
                    _status = value;
                }
            }
        }

        public List<FolderStatusType> GetFolderStatusHistory(int folderID, Passport passport)
        {
            FolderStatusType fs;
            var lst = new List<FolderStatusType>();
            var folderStatusAdapter = new RecordsManageTableAdapters.StatusHistoryTableAdapter();
            folderStatusAdapter.Connection = passport.StaticConnection();
            foreach (DataRow row in folderStatusAdapter.GetFolderStatus(folderID).Rows)
            {
                fs = new FolderStatusType();
                fs.UserName = row["Operator"].ToString();
                fs.StatusTime = Conversions.ToDate(row["StatusChangeDateTime"]);
                fs.Status = row["NewStatus"].ToString();
                lst.Add(fs);
            }
            return lst;
        }

        public class FolderState
        {
            private string _state;
            public string State
            {
                get
                {
                    return _state;
                }
                set
                {
                    _state = value;
                }
            }
            private int _workFlowStep;
            public int WorkFlowStep
            {
                get
                {
                    return _workFlowStep;
                }
                set
                {
                    _workFlowStep = value;
                }
            }
        }

        public List<FolderState> GetFolderStatusList(Passport passport)
        {
            var list = new List<FolderState>();
            FolderState fso;
            var statusList = new RecordsManageTableAdapters.FolderStatusTableAdapter();
            statusList.Connection = passport.StaticConnection();
            foreach (DataRow row in statusList.GetFolderStatusList())
            {
                fso = new FolderState();
                fso.State = row["FolderStatusID"].ToString();
                fso.WorkFlowStep = Conversions.ToInteger(row["WorkFlowStep"]);
                list.Add(fso);
            }
            return list;
        }

    }
}