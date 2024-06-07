using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using MSRecordsEngine.Entities;

namespace MSRecordsEngine.Imaging
{
    //[MessageContract()]
    //public partial class UploadResponse
    //{
    //    [MessageHeader(MustUnderstand = true)]
    //    public string TempFileName = string.Empty;
    //}

    //[MessageContract()]
    //public partial class FileUpload
    //{
    //    [MessageHeader(MustUnderstand = true)]
    //    public Imaging.TicketData Metadata;
    //    [MessageBodyMember(Order = 0)]
    //    public Stream UpStream;
    //}

    //[DataContract()]
    //public partial class TicketData
    //{
    //    [DataMember()]
    //    internal string Ticket;
    //    [DataMember()]
    //    internal int UserId;
    //    [DataMember()]
    //    internal string DatabaseName;
    //    [DataMember()]
    //    internal string TableName;
    //    [DataMember()]
    //    internal string TableId;

    //    public TicketData(string ticket, int userId, string databaseName, string tableName, string tableId)
    //    {
    //        Ticket = ticket;
    //        UserId = userId;
    //        DatabaseName = databaseName;
    //        TableName = tableName;
    //        TableId = tableId;
    //    }
    //}
}
