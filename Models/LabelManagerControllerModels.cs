using Microsoft.AspNetCore.Mvc;
using MSRecordsEngine.Entities;

namespace MSRecordsEngine.Models
{
    public class LabelRequestParam
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
    }

    public class ReturnLabelDetails
    {
        public string onestripjob { get; set; }
        public string onestripform { get; set; }
        public string barCodePrefix { get; set; }
        public int rowCount { get; set; }
        public string onestripjobfields { get; set; }
    }

    public class CreateSQLStringParam
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }

    public class GetFirstValueParam
    {
        public string TableName { get; set; }
        public string Field { get; set; }
        public string SqlString { get; set; }
        public string ConnectionString { get; set; }
        public string CultureShortPattern { get; set; }
        public string OffSetVal { get; set; }
        public bool  BWithTime{ get; set; }
        public bool ConvertToLocalTimeZone { get; set; }
    }

    public class ReturnFirstValue
    {
        public string Str { get; set; }
        public string BarcodePrefix { get; set; }
    }

    public class ApiResponse
    {
        public string ErrType { get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
    }
    public class GetNextRecordParam
    {
        public int RowNo { get; set; }
        public string TableName { get; set; }
        public string SqlString { get; set; }
        public string ConnectionString { get; set; }
    }
    public class AddLabelParam
    {
        public OneStripJob OneStripJobs { get; set; }
        public bool DrawLabels { get; set; }
        public string ConnectionString { get; set; }
    }
    public class AddlabelRespose
    {
        public string ErrType { get; set; }
        public string ErrMsg { get; set; }
        public string OneStripJobs { get; set; }
        public string OneStripFields { get; set; }
        public int LabelId { get; set; }
        public int RowCount { get; set; }
        public string BarCodePrefix { get; set; }
    }
    public class SetLabelObjectsParam
    {
        public string JsonArray { get; set; }
        public int OneStripJobsId { get; set; }
        public string ConnectionString { get; set; }
    }
    public class SetAsDefautParam
    {
        public string ConnectionString { get; set; }
        public int OneStripJobsId { get; set; }
        public int OneStripFormsId{ get; set; }
    }
    public class ReturnSetAsDefault
    {
        public string ErrType { get; set; }
        public string ErrMsg { get; set; }
    }
}
