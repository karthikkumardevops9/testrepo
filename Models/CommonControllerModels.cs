using MSRecordsEngine.Entities;
using Smead.Security;
using System.Collections.Generic;

namespace MSRecordsEngine.Models
{
    public class CheckAttachmentMaxSize_Request
    {
        public Passport Passport { get; set; }
        public string FileSizeMB { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
    }

    public class CheckAttachmentMaxSize_Response
    {
        public int ErrorNumber { get; set; }
        public string CheckConditions { get; set; }
        public string WarringMsg { get; set; }
    }
    public class DocumentViewrApiModel
    {
        public int TotalPages { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public long SizeDisk { get; set; }
        public string? FilePath { get; set; }
        public List<string> stringPath { get; set; }
        public string fileName { get; set; }
        public string serverPath { get; set; }
        public bool Ispcfile { get; set; }
    }

    public class SaveOrphanAttachment_Request
    {
        public List<DocumentViewrApiModel> DocumentViewrApiModels { get; set; }
        public Passport Passport { get; set; }
        public List<FileInfoModel> FileInfoModels { get; set; }
    }

    public class FileInfoModel
    {
        public string Path;
        public string OrgFileName;
    }

    public class SetGridOrders_Requesst
    {
        public string GridName { get; set; }
        public string ConnectionString { get; set; }
        public List<GridSettingsColumns> gridSettingsColumns { get; set; }
    }

    public class CheckaddNewAttachmentPermission_Request
    {
        public string TableName { get; set; }
        public Passport Passport { get; set; }
    }

    public class LoadAttachmentData_Response
    {
        public Table Table { get; set; }
        public string TableId { get; set; }
        public string Display { get; set; }
    }

    public class LoadAttachmentData_Request
    {
        public Passport Passport { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
    }

    public class GetAllOrphansData
    {
        public string FullPath { get; set; }
        public string OrgFileName { get; set; }
        public string orgfullpath { get; set; }
        public string PointerId { get; set; }
        public string ScanDateTime { get; set; }
        public int TrackablesRecordVersion { get; set; }
        public int RecordType { get; set; }
        public int RowNum { get; set; }
        public int TrackablesId { get; set; }
    }

    public class LoadAttachmentOrphanData_Request
    {
        public string ConnectionString { get; set; }
        public string Filter { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int UserId { get; set; }
    }

    public class GetAttchmentName
    {
        public int AttachmentNumber { get; set; }
        public string FullPath { get; set; }
        public short TrackablesId { get; set; }
        public string OrgFileName { get; set; }
    }

    public class GetAllImageFlyOut_Response
    {
        public bool IsTableEntityNull { get; set; }
        public List<GetAttchmentName> GetAttchmentNames { get; set; }
    }
}
