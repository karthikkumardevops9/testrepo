using Microsoft.VisualBasic;
using MSRecordsEngine.Imaging;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace MSRecordsEngine.Models.FusionModels
{
    public partial class DocumentViewerModel
    {
        public DocumentViewerModel()
        {
            AttachmentList = new List<UIparams>();
            VersionList = new List<UIparams>();
            MsgFileCheckout = new List<string>();
            AttachmentCartList = new List<AttachmentCart>();
            IsVault = false;
            isPermission = true;
        }
        public string DocumentService { get; set; }
        public string RecordId { get; set; }
        public string TableName { get; set; }
        public bool IsVault { get; set; }
        public string Tableid { get; set; }
        public string crumbName { get; set; }
        public List<UIparams> VersionList { get; set; }
        public List<UIparams> AttachmentList { get; set; }
        public List<AttachmentCart> AttachmentCartList { get; set; }
        public bool renameOnScan { get; set; }
        public string documentKey { get; set; }
        public string itemName { get; set; }
        public string FilePath { get; set; }
        public string ErrorMsg { get; set; }
        public string paths { get; set; }
        public string Name { get; set; }
        public string HasLink { get; set; }
        public string AttachmentNumberClick { get; set; }
        public bool isError { get; set; }
        public string NewProperty
        {
            get
            {
                return AttachmentNumberClick;
            }
            set
            {
                AttachmentNumberClick = value;
            }
        }
        public UIparams Params { get; set; }
        public List<string> MsgFileCheckout { get; set; }
        public int isCheckoutTome { get; set; }
        public int isCheckout { get; set; }
        public int isCheckoutDesktop { get; set; }
        public bool isPermission { get; set; }
    }

    public partial class DocumentViewerOrphanModel : DocumentViewerModel
    {
        public DocumentViewerOrphanModel()
        {

        }
        public string DocumentService { get; set; }
        public int PointerId = 0;
        public int trackbalesId;
        public int RecordType;
        public string FileName;
        public DocumentViewerModel ViewModel;
        public bool ShouldLast = false;
        public int AttachmentNumber = -1;
    }

    public partial class UIparams
    {
        public UIparams()
        {
            // empty constructor
        }

        public UIparams(string filename, string path, string attachmentNumber, int versionNumber, int pageNumber, int RecordType, int NoteCount, int PointerId, int trackablsid, Passport passport, DocumentViewerModel ViewModel)
        {
            var encrypt = Navigation.EncryptURLParameters(path);
            this.path = encrypt;
            FileName = filename;
            VersionNumber = versionNumber;
            attchNumber = attachmentNumber;
            PageNumber = pageNumber;
            this.RecordType = RecordType;
            if (!File.Exists(path))
            {
                this.path = "NF";
            }
            // Me.Note = NoteCount
            this.PointerId = PointerId.ToString().PadLeft(30, '0');
            Note = CheckNote(PointerId, passport);

            TrackbleId = trackablsid;


        }
        public string path { get; set; }
        public string FileName { get; set; }
        public int VersionNumber { get; set; }
        public string attchNumber { get; set; }
        public int PageNumber { get; set; }
        public int RecordType { get; set; }
        public int Note { get; set; }
        public string PointerId { get; set; }
        public int TrackbleId { get; set; }
        public int HasAnnotation { get; set; }
        public int CountAnnotation { get; set; }
        public int CountNoteImage { get; set; }

        private int CheckNote(int pointerid, Passport passport)
        {
            var lstImages = new List<int>();

            string sql = "select count(*) from Annotations a where a.TableId = RIGHT('000000000000000000000000000000' + CAST(@pointerid AS VARCHAR), 30)";
            using (SqlConnection conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pointerid", pointerid);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }

    public partial class FileDownloads
    {
        internal Passport _passport { get; set; }
        internal List<string> pathString { get; set; }
        internal string _serverPath { get; set; }
        public List<string> deleteTempFile { get; set; }
        public string Path { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
        public int AttachNum { get; set; }
        public int AttachVer { get; set; }

        internal bool CheckIsDesktopFileBeforeDownload(FileDownloads @params)
        {
            pathString = new List<string>();

            using (var cmd = new SqlCommand("SP_RMS_GetFilesPaths", _passport.Connection()))
            {
                cmd.CommandType = (CommandType)Convert.ToInt32(CommandType.StoredProcedure);
                cmd.Parameters.AddWithValue("@tableId", @params.TableId);
                cmd.Parameters.AddWithValue("@tableName", @params.TableName);
                cmd.Parameters.AddWithValue("@AttachmentNumber", @params.AttachNum);
                cmd.Parameters.AddWithValue("@RecordVersion", @params.AttachVer);

                var adp = new SqlDataAdapter(cmd);
                var dTable = new DataTable();
                int datat = adp.Fill(dTable);
                foreach (DataRow row in dTable.Rows)
                {
                    var getpath = row["FullPath"];
                    pathString.Add((string)getpath);
                }
            }

            if (pathString.Count > 1)
                return true;
            return false;
        }
    }

    public partial class AttachmentsFileInfo
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public partial class ZipStreamWrapper : Stream
    {

        public ZipStreamWrapper(Stream stream)
        {
            baseStream = stream;
            lengthf = 0;
        }

        private int lengthf;
        private Stream baseStream;

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return lengthf;
            }
        }

        public override long Position
        {
            get
            {
                return lengthf;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
            lengthf += count;
        }
    }

    public partial class AttachmentCart
    {
        public AttachmentCart(int Id, int userid, string record, string filepath, string filename, int attacnum, int attachver)
        {
            userId = userid;
            Record = record;
            var encrypt = Navigation.EncryptURLParameters(filepath);
            filePath = Convert.ToString(Strings.Chr(225)) + encrypt;
            fileName = filename;
            this.Id = Id;
            attachNum = attacnum;
            attachVer = attachver;

        }
        public int Id { get; set; }
        public int userId { get; set; }
        public string Record { get; set; }
        public string filePath { get; set; }
        public string fileName { get; set; }
        public int note { get; set; }
        public int attachNum { get; set; }
        public int attachVer { get; set; }

    }

    public class FileDownloadModelReq
    {
        public List<FileDownloads> paramss { get; set; }
        public int viewid { get; set; }
    }
}
