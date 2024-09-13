using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace MSRecordsEngine.Models
{
    // reporting per row
    public class ReportingPerRow
    {
        public ReportingPerRow()
        {
            ListOfHeader = new List<string>();
            ListOfRows = new List<List<string>>();
        }
        public PagingModel Paging { get; set; } = new PagingModel();
        private string _Tableid { get; set; }
        private string _tableName { get; set; }
        private int _viewId { get; set; }
        private int _pageNum { get; set; }
        private string ReportName { get; set; }
        public List<string> ListOfHeader { get; set; }
        public List<List<string>> ListOfRows { get; set; }
        public bool hasPermission { get; set; }
        public int TotalPages { get; set; }
        public int RowsPerPage { get; set; }
        public string Title { get; set; }
        public string ItemDescription { get; set; }
        public string Msg { get; set; }
        public enum Reports
        {
            AuditHistoryPerRow,
            TrackingHistoryPerRow,
            ContentsRow
        }
    }
}