using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Leadtools;
using Leadtools.Codecs;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Properties; // Install-Package Microsoft.VisualBasic


namespace MSRecordsEngine.Imaging
{
    internal partial class UpdateAttachment
    {
        private bool _appendToExisting;
        private SqlConnection _conn;
        private string _name = string.Empty;
        private int _directoriesId;
        private string _fullPath = string.Empty;
        private string _imageTableName = string.Empty;
        private string _prefix = string.Empty;
        private string _extension = string.Empty;
        private string _pcFileExtension = string.Empty;
        private int _nextDocNum;
        private int _volumeId;
        private string _userName = string.Empty;

        internal UpdateAttachment(DataRow outputSettingsRow, string userName, string pcFileExtension, SqlConnection conn) : this(outputSettingsRow, userName, pcFileExtension, false, conn)
        {
        }

        internal UpdateAttachment(DataRow outputSettingsRow, string userName, string pcFileExtension, bool appendToExisting, SqlConnection conn)
        {
            _conn = conn;
            _name = outputSettingsRow["Id"].ToString();
            _prefix = outputSettingsRow["FileNamePrefix"].ToString();
            _extension = outputSettingsRow["FileExtension"].ToString().ToLower();
            _directoriesId = Conversions.ToInteger(outputSettingsRow["DirectoriesId"]);
            _volumeId = Conversions.ToInteger(outputSettingsRow["VolumesId"]);
            _nextDocNum = Conversions.ToInteger(outputSettingsRow["NextDocNum"]);
            _fullPath = outputSettingsRow["FullPath"].ToString();
            _imageTableName = outputSettingsRow["ImageTableName"].ToString();
            _userName = userName;
            _pcFileExtension = pcFileExtension;
            _appendToExisting = appendToExisting;
        }
        internal UpdateAttachment(DataRow outputSettingsRow, string userName, string pcFileExtension, bool appendToExisting, SqlConnection conn, bool forceExtension)
        {
            _conn = conn;
            _name = outputSettingsRow["Id"].ToString();
            _prefix = outputSettingsRow["FileNamePrefix"].ToString();
            // Added Condition to force to take file extension from the parameter passed inplace of default output settings
            if (forceExtension)
            {
                _extension = pcFileExtension.ToString().ToLower();
            }
            else
            {
                _extension = outputSettingsRow["FileExtension"].ToString().ToLower();
            }
            _directoriesId = Conversions.ToInteger(outputSettingsRow["DirectoriesId"]);
            _volumeId = Conversions.ToInteger(outputSettingsRow["VolumesId"]);
            _nextDocNum = Conversions.ToInteger(outputSettingsRow["NextDocNum"]);
            _fullPath = outputSettingsRow["FullPath"].ToString();
            _imageTableName = outputSettingsRow["ImageTableName"].ToString();
            _userName = userName;
            _pcFileExtension = pcFileExtension;
            _appendToExisting = appendToExisting;
        }
        internal int AddPages(string fileName, int trackablesId, int versionNumber, int pageCount, int userId, string originalFullPath)
        {
            CodecsImageInfo info = GetCodecInfoFromFile(fileName, _pcFileExtension);
            int count = 1;
            int pointerId = 0;
            string newFileName = string.Empty;

            if (string.IsNullOrEmpty(_imageTableName))
            {
                newFileName = CreateNewFile(info is not null);
            }
            else
            {
                if (info is null)
                    throw new Exception("Directory Settings cannot be configured to save non-image attachments to the database.");
                newFileName = InsertImageTableRecord(_imageTableName, _extension, (int)Attachments.AttachmentTypes.tkImage, Output.FileToByteArray(fileName), _conn);
            }

            if (string.IsNullOrEmpty(newFileName))
                return 0;
            if (string.IsNullOrEmpty(_imageTableName))
                Attachment.CopyFile(_imageTableName, fileName, newFileName, _conn);

            if (!_appendToExisting)
                versionNumber = -versionNumber;

            if (info is null)
            {
                InsertPCFilePointer(trackablesId, versionNumber, count, newFileName, originalFullPath, string.Empty);
            }
            else
            {
                count = info.TotalPages;
                pointerId = InsertImagePointer(trackablesId, versionNumber, count, pageCount + 1, info.Height, info.Width, info.SizeDisk, newFileName, originalFullPath, string.Empty);
                if (pointerId != 0 & !string.IsNullOrEmpty(_imageTableName))
                    UpdateImagePointerImageTableName(pointerId, _imageTableName, _conn);
            }

            UpdateTrackablePageCount(trackablesId, versionNumber, userId, count, _conn);
            return count;
        }

        internal int AddPages(string fileName, int trackablesId, int versionNumber, int pageCount, int userId, string originalFullPath, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            int count = 1;
            int pointerId = 0;
            string newFileName = string.Empty;

            if (string.IsNullOrEmpty(_imageTableName))
            {
                newFileName = CreateNewFile(IsAnImage, false);
            }
            else
            {
                if (!IsAnImage)
                    throw new Exception("Directory Settings cannot be configured to save non-image attachments to the database.");
                newFileName = InsertImageTableRecord(_imageTableName, _extension, (int)Attachments.AttachmentTypes.tkImage, Output.FileToByteArray(fileName), _conn);
            }

            if (string.IsNullOrEmpty(newFileName))
                return 0;
            if (string.IsNullOrEmpty(_imageTableName))
                Attachment.CopyFile(_imageTableName, fileName, newFileName, _conn);

            if (!_appendToExisting)
                versionNumber = -versionNumber;

            if (!IsAnImage)
            {
                InsertPCFilePointer(trackablesId, versionNumber, count, newFileName, originalFullPath, string.Empty);
            }
            else
            {
                count = totalPages;
                pointerId = InsertImagePointer(trackablesId, versionNumber, count, pageCount + 1, height, width, sizeOnDisk, newFileName, originalFullPath, string.Empty);
                if (pointerId != 0 & !string.IsNullOrEmpty(_imageTableName))
                    UpdateImagePointerImageTableName(pointerId, _imageTableName, _conn);
            }

            UpdateTrackablePageCount(trackablesId, versionNumber, userId, count, _conn);
            return count;
        }

        internal static string InsertImageTableRecord(string imageTableName, string fileExtension, int trackableType, byte[] byteArray, SqlConnection conn)
        {
            string newRecordId = string.Empty;
            if (fileExtension.StartsWith("."))
                fileExtension = fileExtension.Substring(1);

            using (var cmd = new SqlCommand(string.Format(Resources.InsertImageTableRecords, imageTableName), conn))
            {
                cmd.Parameters.AddWithValue("@fileExtension", fileExtension);
                cmd.Parameters.AddWithValue("@trackableType", trackableType);
                newRecordId = cmd.ExecuteScalar().ToString();
            }

            return UpdateImageTableRecord(imageTableName, newRecordId, byteArray, conn);
        }

        internal static string UpdateImageTableRecord(string imageTableName, string recordId, byte[] byteArray, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(string.Format(Resources.GetDatabaseImage, imageTableName), conn))
            {
                cmd.Parameters.AddWithValue("@recordId", recordId);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.UpdateCommand = new SqlCommand(string.Format(Resources.UpdateImageTableRecords, imageTableName), conn);
                    da.UpdateCommand.Parameters.Add("@imageField", SqlDbType.Image, byteArray.Length, "ImageField");
                    da.UpdateCommand.Parameters.Add("@recordId", SqlDbType.Int, int.MaxValue, "Id");
                    var dt = new DataTable();

                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return string.Empty;

                    dt.Rows[0]["ImageField"] = byteArray;
                    da.Update(dt);
                    return recordId;
                }
            }
        }

        internal void CreateAttachmentRecord(string tableName, string tableId, int attachmentNumber, int versionNumber, bool officialRecord, int trackableId, string fileName, string originalFullPath, string renameFileName)
        {
            string newFileName = string.Empty;
            var info = GetCodecInfoFromFile(fileName, _pcFileExtension);
            if (!string.IsNullOrEmpty(_imageTableName) & info is null)
                throw new Exception("Directory Settings cannot be configured to save non-image attachments to the database.");
            if (versionNumber == 1 | trackableId == 0)
                trackableId = GetNextTrackableNumber();
            if (trackableId == 0)
                throw new Exception("Could not get next Trackable Id.");

            if (string.IsNullOrEmpty(_imageTableName))
            {
                newFileName = CreateNewFile(info is not null);
            }
            else
            {
                newFileName = InsertImageTableRecord(_imageTableName, _extension, (int)Attachments.AttachmentTypes.tkImage, Output.FileToByteArray(fileName), _conn);
            }

            if (string.IsNullOrEmpty(newFileName))
                return;
            if (string.IsNullOrEmpty(_imageTableName))
                Attachment.CopyFile(_imageTableName, fileName, newFileName, _conn);

            if (info is null)
            {
                trackableId = InsertTrackablesUserlinks(trackableId, tableName, tableId, (int)Attachments.AttachmentTypes.tkWPDoc, attachmentNumber, versionNumber, 1, officialRecord, false);
                InsertPCFilePointer(trackableId, versionNumber, 1, newFileName, originalFullPath, renameFileName);
            }
            else
            {
                trackableId = InsertTrackablesUserlinks(trackableId, tableName, tableId, (int)Attachments.AttachmentTypes.tkImage, attachmentNumber, versionNumber, info.TotalPages, officialRecord, false);
                int pointerId = InsertImagePointer(trackableId, versionNumber, info.TotalPages, 1, info.Height, info.Width, info.SizeDisk, newFileName, originalFullPath, renameFileName);
                if (pointerId != 0 & !string.IsNullOrEmpty(_imageTableName))
                    UpdateImagePointerImageTableName(pointerId, _imageTableName, _conn);
            }
        }

        internal void CreateAttachmentRecord(string tableName, string tableId, int attachmentNumber, int versionNumber, bool officialRecord, int trackableId, string fileName, string originalFullPath, string renameFileName, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            string newFileName = string.Empty;

            if (!string.IsNullOrEmpty(_imageTableName) & !IsAnImage)
                throw new Exception("Directory Settings cannot be configured to save non-image attachments to the database.");
            if (versionNumber == 1 | trackableId == 0)
                trackableId = GetNextTrackableNumber();
            if (trackableId == 0)
                throw new Exception("Could not get next Trackable Id.");

            if (string.IsNullOrEmpty(_imageTableName))
            {
                newFileName = CreateNewFile(IsAnImage, false);
            }
            else
            {
                newFileName = InsertImageTableRecord(_imageTableName, _extension, (int)Attachments.AttachmentTypes.tkImage, Output.FileToByteArray(fileName), _conn);
            }

            if (string.IsNullOrEmpty(newFileName))
                return;
            if (string.IsNullOrEmpty(_imageTableName))
                Attachment.CopyFile(_imageTableName, fileName, newFileName, _conn);

            if (!IsAnImage)
            {
                trackableId = InsertTrackablesUserlinks(trackableId, tableName, tableId, (int)Attachments.AttachmentTypes.tkWPDoc, attachmentNumber, versionNumber, 1, officialRecord, false);
                InsertPCFilePointer(trackableId, versionNumber, 1, newFileName, originalFullPath, renameFileName);
            }
            else
            {
                trackableId = InsertTrackablesUserlinks(trackableId, tableName, tableId, (int)Attachments.AttachmentTypes.tkImage, attachmentNumber, versionNumber, totalPages, officialRecord, false);
                int pointerId = InsertImagePointer(trackableId, versionNumber, totalPages, 1, height, width, sizeOnDisk, newFileName, originalFullPath, renameFileName);
                if (pointerId != 0 & !string.IsNullOrEmpty(_imageTableName))
                    UpdateImagePointerImageTableName(pointerId, _imageTableName, _conn);
            }
        }

        internal int CreateOrphanRecord(string fileName, string originalFullPath, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            if (!string.IsNullOrEmpty(_imageTableName) & !IsAnImage)
                throw new Exception("Directory Settings cannot be configured to save non-image attachments to the database.");

            string newFileName = string.Empty;
            int trackableId = GetNextTrackableNumber();
            if (trackableId == 0)
                throw new Exception("Could not get next Trackable Id.");

            if (string.IsNullOrEmpty(_imageTableName))
            {
                newFileName = CreateNewFile(IsAnImage, false);
                if (string.IsNullOrEmpty(newFileName))
                    return 0;
                Attachment.CopyFile(string.Empty, originalFullPath, newFileName, _conn);
            }
            else
            {
                newFileName = InsertImageTableRecord(_imageTableName, _extension, (int)Attachments.AttachmentTypes.tkImage, Output.FileToByteArray(originalFullPath), _conn);
            }

            if (!IsAnImage)
            {
                trackableId = InsertTrackablesUserlinks(trackableId, string.Empty, string.Empty, (int)Attachments.AttachmentTypes.tkWPDoc, 1, 1, 1, false, true);
                InsertPCFilePointer(trackableId, 1, 1, newFileName, originalFullPath, fileName);
            }
            else
            {
                trackableId = InsertTrackablesUserlinks(trackableId, string.Empty, string.Empty, (int)Attachments.AttachmentTypes.tkImage, 1, 1, totalPages, false, true);
                int pointerId = InsertImagePointer(trackableId, 1, totalPages, 1, height, width, sizeOnDisk, newFileName, originalFullPath, fileName);
                if (pointerId != 0 & !string.IsNullOrEmpty(_imageTableName))
                    UpdateImagePointerImageTableName(pointerId, _imageTableName, _conn);
            }

            return trackableId;
        }

        private void UpdateImagePointerImageTableName(int pointerId, string imageTableName, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.UpdateImagePointerImageTableName, conn))
            {
                cmd.Parameters.AddWithValue("@Id", pointerId);
                cmd.Parameters.AddWithValue("@imageTableName", imageTableName);
                cmd.ExecuteNonQuery();
            }
        }

        internal static Leadtools.Codecs.CodecsImageInfo GetCodecInfoFromStream(byte[] filestream)
        {
            try
            {
                Attachments.SetupCodec();
                var codec = new RasterCodecs();
                CodecsImageInfo info = codec.GetInformation(new MemoryStream(filestream), true);
                if (IsAPCFile(info.Format))
                    return default;
                return info;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return GetCodecInfoFromTempFile(filestream, Path.GetTempFileName());
            }
        }

        private static Leadtools.Codecs.CodecsImageInfo GetCodecInfoFromTempFile(byte[] byteArray, string fileName)
        {
            try
            {
                string err = Output.ByteArrayToFile(fileName, byteArray);
                if (!string.IsNullOrEmpty(err))
                    return default;
                return GetCodecInfoFromFile(fileName, string.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default;
            }
            finally
            {
                Exporter.DeleteFile(fileName);
            }
        }

        internal static Leadtools.Codecs.CodecsImageInfo GetCodecInfoFromFile(string fileName, string extension)
        {
            try
            {
                Attachments.SetupCodec();
                var codec = new RasterCodecs();
                CodecsImageInfo info = codec.GetInformation(fileName, true);
                if (IsAPCFile(info.Format, extension))
                    return default; 
                return info;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default;
            }
        }

        internal static bool IsAPCFile(RasterImageFormat Format)
        {
            return IsAPCFile(Format, string.Empty);
        }

        internal static bool IsAPCFile(string extension)
        {
            return IsAPCFile(RasterImageFormat.Tif, extension);
        }

        internal static bool IsAPCFile(RasterImageFormat Format, string extension)
        {
            switch (Format)
            {
                case var @case when @case == RasterImageFormat.RasPdf:
                case var case1 when case1 == RasterImageFormat.RasPdfG31Dim:
                case var case2 when case2 == RasterImageFormat.RasPdfG31Dim:
                case var case3 when case3 == RasterImageFormat.RasPdfG32Dim:
                case var case4 when case4 == RasterImageFormat.RasPdfG4:
                case var case5 when case5 == RasterImageFormat.RasPdfJbig2:
                case var case6 when case6 == RasterImageFormat.RasPdfJpeg:
                case var case7 when case7 == RasterImageFormat.RasPdfJpeg411:
                case var case8 when case8 == RasterImageFormat.RasPdfJpeg422:
                case var case9 when case9 == RasterImageFormat.RasPdfLzw:
                case var case10 when case10 == RasterImageFormat.PdfLeadMrc:
                case var case11 when case11 == RasterImageFormat.Eps:
                case var case12 when case12 == RasterImageFormat.EpsPostscript:
                case var case13 when case13 == RasterImageFormat.Postscript:
                case var case14 when case14 == RasterImageFormat.RtfRaster:
                case var case15 when case15 == RasterImageFormat.Unknown:
                    {
                        return true;
                    }
            }

            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            switch (extension.ToLower() ?? "")
            {
                case "pdf":
                case "fdf":
                    {
                        return true;
                    }
                case "xps":
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        private int InsertTrackablesUserlinks(int trackableId, string tableName, string tableId, int attachmentType, int attachmentNumber, int versionNumber, int pageCount, bool officialRecord, bool orphan)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.InsertTrackablesUserlinks, _conn))
                {
                    cmd.Parameters.AddWithValue("@trackableId", trackableId);
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@trackableType", attachmentType);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    cmd.Parameters.AddWithValue("@versionNumber", versionNumber);
                    cmd.Parameters.AddWithValue("@pageCount", pageCount);
                    cmd.Parameters.AddWithValue("@officialRecord", officialRecord);
                    cmd.Parameters.AddWithValue("@orphan", Math.Abs(Conversions.ToInteger(orphan)));
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        internal int InsertImagePointer(int trackableId, int versionNumber, int pageCount, int pageNumber, int imageHeight, int imageWidth, long imageSize, string fileName, string originalFullPath, string renameFileName)
        {
            try
            {
                fileName = Path.GetFileName(fileName);

                using (var cmd = new SqlCommand(Resources.InsertImagePointer, _conn))
                {
                    cmd.Parameters.AddWithValue("@trackableId", trackableId);
                    cmd.Parameters.AddWithValue("@versionNumber", versionNumber);
                    cmd.Parameters.AddWithValue("@pageCount", pageCount);
                    cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@imageHeight", imageHeight);
                    cmd.Parameters.AddWithValue("@imageWidth", imageWidth);
                    cmd.Parameters.AddWithValue("@imageSize", imageSize);
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    cmd.Parameters.AddWithValue("@directoriesId", _directoriesId);
                    cmd.Parameters.AddWithValue("@orgFullPath", originalFullPath);

                    if (string.IsNullOrEmpty(renameFileName))
                    {
                        cmd.Parameters.AddWithValue("@orgFileName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@orgFileName", renameFileName);
                    }

                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private int InsertScanBatch(int pageCount)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.InsertScanBatch, _conn))
                {
                    cmd.Parameters.AddWithValue("@pageCount", pageCount);
                    cmd.Parameters.AddWithValue("@userName", _userName);
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private void InsertPCFilePointer(int trackableId, int versionNumber, int pageCount, string fileName, string originalFullPath, string renameFileName)
        {
            try
            {
                fileName = Path.GetFileName(fileName);

                using (var cmd = new SqlCommand(Resources.InsertPCFilePointer, _conn))
                {
                    cmd.Parameters.AddWithValue("@trackableId", trackableId);
                    cmd.Parameters.AddWithValue("@versionNumber", versionNumber);
                    cmd.Parameters.AddWithValue("@pageCount", pageCount);
                    cmd.Parameters.AddWithValue("@directoriesId", _directoriesId);
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    cmd.Parameters.AddWithValue("@orgFullPath", originalFullPath);

                    if (string.IsNullOrEmpty(renameFileName))
                    {
                        cmd.Parameters.AddWithValue("@orgFileName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@orgFileName", renameFileName);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal static void UpdateTrackablePageCount(int trackableId, int versionNumber, int userId, int pageCount, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.UpdateTrackablePageCount, conn))
                {
                    cmd.Parameters.AddWithValue("@trackableId", trackableId);
                    cmd.Parameters.AddWithValue("@versionNumber", versionNumber);
                    cmd.Parameters.AddWithValue("@pageCount", pageCount);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal void CopyNewFiles(DataTable pointerTable, bool isAnImage, string inFileName, int totalPages)
        {
            int pageNumber = 0;

            foreach (DataRow row in pointerTable.Rows)
            {
                pageNumber += 1;
                string fullPath = row["FullPath"].ToString();
                string fileName = row["FileName"].ToString();
                string imageTableName = row["ImageTableName"].ToString();

                if (!string.IsNullOrEmpty(row["OrgFullPath"].ToString()))
                {
                    if (!string.IsNullOrEmpty(fileName) && Information.IsNumeric(fileName))
                    {
                        imageTableName = row["OrgFullPath"].ToString();
                        fullPath = row["FileName"].ToString();
                    }
                }

                if (string.IsNullOrEmpty(imageTableName) && !string.IsNullOrEmpty(_imageTableName))
                {
                    byte[] bmp = Output.FileToByteArray(fullPath);
                    fileName = InsertImageTableRecord(_imageTableName, Path.GetExtension(fullPath),(int) Attachments.AttachmentTypes.tkImage, bmp, _conn);
                    Attachment.UpdatePointerFileName((int)row["PointerId"], fileName, _imageTableName, _directoriesId, pageNumber, isAnImage, _conn, totalPages);
                    Attachment.DeleteFile(string.Empty, fullPath, _conn);
                }
                else if (string.IsNullOrEmpty(_imageTableName) && !string.IsNullOrEmpty(imageTableName))
                {
                    byte[] bmp = Attachment.BlobImageToStream(imageTableName, fullPath, _conn);
                    CreateNewFile(string.Empty, row["FullPath"].ToString(), (int)row["PointerId"], pageNumber, true, bmp, totalPages);
                    Attachment.DeleteFile(imageTableName, fullPath, _conn);
                }
                else if (pageNumber == pointerTable.Rows.Count)
                {
                    CreateNewFile(imageTableName, fullPath, (int)row["PointerId"], pageNumber, isAnImage, Output.FileToByteArray(inFileName), totalPages);
                }
                else
                {
                    CreateNewFile(imageTableName, fullPath,(int) row["PointerId"], pageNumber, isAnImage, default, totalPages);
                }
            }
        }

        internal void CopyNewFiles(DataTable pointerTable, bool isAnImage, string inFileName)
        {
            int pageNumber = 0;

            foreach (DataRow row in pointerTable.Rows)
            {
                pageNumber += 1;
                string fullPath = row["FullPath"].ToString();
                string fileName = row["FileName"].ToString();
                string imageTableName = row["ImageTableName"].ToString();

                if (!string.IsNullOrEmpty(row["OrgFullPath"].ToString()))
                {
                    if (!string.IsNullOrEmpty(fileName) && Information.IsNumeric(fileName))
                    {
                        imageTableName = row["OrgFullPath"].ToString();
                        fullPath = row["FileName"].ToString();
                    }
                }

                if (string.IsNullOrEmpty(imageTableName) && !string.IsNullOrEmpty(_imageTableName))
                {
                    byte[] bmp = Output.FileToByteArray(fullPath);
                    fileName = InsertImageTableRecord(_imageTableName, Path.GetExtension(fullPath),(int) Attachments.AttachmentTypes.tkImage, bmp, _conn);
                    Attachment.UpdatePointerFileName((int)row["PointerId"], fileName, _imageTableName, _directoriesId, pageNumber, isAnImage, _conn);
                    Attachment.DeleteFile(string.Empty, fullPath, _conn);
                }
                else if (string.IsNullOrEmpty(_imageTableName) && !string.IsNullOrEmpty(imageTableName))
                {
                    byte[] bmp = Attachment.BlobImageToStream(imageTableName, fullPath, _conn);
                    CreateNewFile(string.Empty, row["FullPath"].ToString(),(int) row["PointerId"], pageNumber, true, bmp);
                    Attachment.DeleteFile(imageTableName, fullPath, _conn);
                }
                else if (pageNumber == pointerTable.Rows.Count)
                {
                    CreateNewFile(imageTableName, fullPath,(int)row["PointerId"], pageNumber, isAnImage, Output.FileToByteArray(inFileName));
                }
                else
                {
                    CreateNewFile(imageTableName, fullPath,(int) row["PointerId"], pageNumber, isAnImage, default);
                }
            }
        }

        internal string ChangeFileName(int pointerId, string fullPath, bool replaceExisting, string fileName, ref int directoryId)
        {
            string newFullPath = fullPath;
            if (!replaceExisting)
                newFullPath = CreateNewFile(false);
            if (string.IsNullOrEmpty(newFullPath))
                return string.Empty;

            if (string.IsNullOrEmpty(Attachment.CopyFile(string.Empty, fileName, newFullPath, _conn)))
            {
                directoryId = _directoriesId;
                if (!replaceExisting)
                    Attachment.UpdatePointerFileName(pointerId, newFullPath, _directoriesId, 0, !IsAPCFile(_pcFileExtension), _conn);
                return Path.GetFileName(newFullPath);
            }

            return string.Empty;
        }

        private string CreateNewFile(bool isAnImage, bool checkPCFile = true)
        {
            string newPath = Path.GetDirectoryName(_fullPath);
            string fileName = ConvertToBase36(isAnImage, checkPCFile);

            while (!string.IsNullOrEmpty(fileName) && File.Exists(Path.Combine(newPath, fileName)))
                fileName = ConvertToBase36(isAnImage, checkPCFile);

            if (string.IsNullOrEmpty(fileName))
                return string.Empty;
            return Path.Combine(newPath, fileName);
        }

        private void CreateNewFile(string imageTableName, string fullPath, int pointerId, int pageNumber, bool isAnImage, byte[] byteArray, int totalPages)
        {
            if (!string.IsNullOrEmpty(imageTableName))
            {
                Attachment.UpdatePointerFileName(pointerId, fullPath, imageTableName, _directoriesId, pageNumber, isAnImage, _conn, totalPages);
                return;
            }

            string newFullPath = CreateNewFile(isAnImage, false);
            if (string.IsNullOrEmpty(newFullPath))
                return;

            if (byteArray is not null)
            {
                CreateNewFile(byteArray, newFullPath);
            }
            else
            {
                Attachment.CopyFile(imageTableName, fullPath, newFullPath, _conn);
            }

            Attachment.DeleteFile(imageTableName, fullPath, _conn);
            Attachment.UpdatePointerFileName(pointerId, newFullPath, string.Empty, _directoriesId, pageNumber, isAnImage, _conn, totalPages);
        }

        private void CreateNewFile(string imageTableName, string fullPath, int pointerId, int pageNumber, bool isAnImage, byte[] byteArray)
        {
            if (!string.IsNullOrEmpty(imageTableName))
            {
                Attachment.UpdatePointerFileName(pointerId, fullPath, imageTableName, _directoriesId, pageNumber, isAnImage, _conn);
                return;
            }

            string newFullPath = CreateNewFile(isAnImage);
            if (string.IsNullOrEmpty(newFullPath))
                return;

            if (byteArray is not null)
            {
                CreateNewFile(byteArray, newFullPath);
            }
            else
            {
                Attachment.CopyFile(imageTableName, fullPath, newFullPath, _conn);
            }

            Attachment.DeleteFile(imageTableName, fullPath, _conn);
            Attachment.UpdatePointerFileName(pointerId, newFullPath, _directoriesId, pageNumber, isAnImage, _conn);
        }

        private bool CreateNewFile(byte[] byteArray, string fullpath)
        {
            if (string.IsNullOrEmpty(fullpath))
                return false;

            try
            {
                if (File.Exists(fullpath))
                    File.SetAttributes(fullpath, FileAttributes.Normal);

                using (var fileStream = new FileStream(fullpath, FileMode.Create))
                {
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    fileStream.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private string CreateNewFile(byte[] byteArray, bool isAnImage)
        {
            string newFullPath = CreateNewFile(isAnImage);
            if (string.IsNullOrEmpty(newFullPath))
                return string.Empty;

            using (var fileStream = new FileStream(newFullPath, FileMode.Create))
            {
                fileStream.Write(byteArray, 0, byteArray.Length);
                fileStream.Close();
            }

            return Path.GetFileName(newFullPath);
        }

        private string ConvertToBase36(bool isAnImage, bool checkPCFile = true)
        {
            string fileName = string.Empty;
            string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int nextNumber = GetNextDocumentNumber();
            if (nextNumber == -1)
                return string.Empty;

            while (nextNumber != 0)
            {
                fileName = base36.Substring(nextNumber % 36, 1) + fileName;
                nextNumber = nextNumber / 36;
            }

            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            string extension = _pcFileExtension;
            if (isAnImage && !checkPCFile)
            {
                extension = _extension;
            }
            else if (isAnImage && !IsAPCFile(_extension))
            {
                extension = _extension;
            }

            if (extension.StartsWith("."))
                extension = extension.Substring(1);
            return string.Format("{0}{1}.{2}", _prefix, fileName.PadLeft(6, '0'), extension);
        }

        private int GetNextDocumentNumber()
        {
            using (var cmd = new SqlCommand(Resources.GetNextDocumentNumber, _conn))
            {
                cmd.Parameters.AddWithValue("@Id", _name);
                cmd.Parameters.AddWithValue("@nextDocNum", _nextDocNum + 1);
                cmd.Parameters.AddWithValue("@currentDocNum", _nextDocNum);

                for (int i = 1; i <= 1000; i++)
                {
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0)
                        return _nextDocNum;
                    _nextDocNum += 1;
                    cmd.Parameters["@nextDocNum"].Value = _nextDocNum + 1;
                    cmd.Parameters["@currentDocNum"].Value = _nextDocNum;
                }
            }

            return _nextDocNum;
        }

        private int GetNextTrackableNumber()
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetNextTrackableNumber, _conn))
                {
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
