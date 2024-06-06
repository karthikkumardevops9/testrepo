using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Properties; // Install-Package Microsoft.VisualBasic
//using Smead.RecordsManagement.ImagingCS.Attachments;

namespace MSRecordsEngine.Imaging
{
    [CLSCompliant(true)]
    [Serializable()]
    public partial class SecurityInfo
    {
        private const int AnnotationObjectTypeId = 4;
        private const int ChildAttachmentObjectTypeId = 2;
        private const int AttachmentObjectTypeId = 6;
        private const int OrphanObjectTypeId = 13;
        private const string Orphans = " orphans"; // space needs to be at the front because that is how it is recorded in the SecureObject table.

        public bool IsInAdministratorGroup = false;
        public readonly List<string> AnnotationPermissions = new List<string>();
        public readonly List<string> AttachmentPermissions = new List<string>();
        public readonly List<string> OrphanPermissions = new List<string>();
        public readonly List<string> VolumePermissions = new List<string>();

        public SecurityInfo()
        {
            // WCF proxy requires an empty constructor
        }

        public SecurityInfo(string tableName, int userId, SqlConnection conn) : this()
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = Orphans;

            LoadVolumePermissions(userId, conn);
            LoadPermissions(tableName, userId, conn);
            LoadPermissions(Orphans, userId, conn);
            LoadIfInAdministratorGroup(userId, conn);
        }

        public SecurityInfo(string tableName, int userId, int directoriesId, SqlConnection conn) : this()
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = Orphans;

            LoadVolumePermissions(userId, directoriesId, conn);
            LoadPermissions(tableName, userId, conn);
            LoadPermissions(Orphans, userId, conn);
            LoadIfInAdministratorGroup(userId, conn);
        }

        private void LoadIfInAdministratorGroup(int userID, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.IsUserInAdministratorGroup, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        IsInAdministratorGroup = Conversions.ToInteger(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                IsInAdministratorGroup = false;
            }
        }

        private void LoadPermissions(string tableName, int userId, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.GetPermissionsAttachment, conn))
            {
                cmd.Parameters.AddWithValue("@userID", userId);
                cmd.Parameters.AddWithValue("@tableName", tableName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    LoadOrphanPermissions(tableName, dt.Rows);
                    LoadAttachmentPermissions(tableName, dt.Rows);
                }
            }
        }

        private void LoadAttachmentPermissions(string tableName, DataRowCollection rows)
        {
            if (string.Compare(tableName.Trim(), Orphans.Trim(), true) == 0)
                return;

            foreach (DataRow row in rows)
            {
                AddAnnotationPermission((int)row["SecureObjectTypeID"], row["Permission"].ToString());
                AddAttachmentPermission((int)row["SecureObjectTypeID"], row["Permission"].ToString());
            }
        }

        private void LoadOrphanPermissions(string tableName, DataRowCollection rows)
        {
            if (string.Compare(tableName.Trim(), Orphans.Trim(), true) != 0)
                return;

            foreach (DataRow row in rows)
                AddOrphanPermission((int)row["SecureObjectTypeID"], row["Permission"].ToString());
        }

        private void AddAnnotationPermission(int secureObjectTypeId, string permission)
        {
            if (secureObjectTypeId == AnnotationObjectTypeId)
            {
                permission = permission.ToLower().Replace(Conversions.ToString(' '), string.Empty);
                if (!AnnotationPermissions.Contains(permission))
                    AnnotationPermissions.Add(permission);
            }
        }

        private void AddAttachmentPermission(int secureObjectTypeId, string permission)
        {
            if (secureObjectTypeId == AttachmentObjectTypeId || secureObjectTypeId == ChildAttachmentObjectTypeId)
            {
                permission = permission.ToLower().Replace(Conversions.ToString(' '), string.Empty);
                if (!AttachmentPermissions.Contains(permission))
                    AttachmentPermissions.Add(permission);
            }
        }

        private void AddOrphanPermission(int secureObjectTypeId, string permission)
        {
            if (secureObjectTypeId == OrphanObjectTypeId)
            {
                permission = permission.ToLower().Replace(Conversions.ToString(' '), string.Empty);
                if (!OrphanPermissions.Contains(permission))
                    OrphanPermissions.Add(permission);
            }
        }

        private void LoadVolumePermissions(int userId, SqlConnection conn)
        {
            int directoriesId = 0;
            DataTable dt = Attachments.GetOutputSetting(Attachments.GetDefaultOutputSetting(conn), userId, 0, conn);

            if (dt is not null && dt.Rows.Count > 0)
            {
                try
                {
                    directoriesId = (int)dt.Rows[0]["DirectoriesId"];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    directoriesId = 0;
                }
            }

            LoadVolumePermissions(userId, directoriesId, conn);
        }

        private void LoadVolumePermissions(int userId, int directoriesId, SqlConnection conn)
        {
            if (directoriesId <= 0)
                return;

            try
            {
                string secureObjectName = GetVolumeName(directoriesId, conn);

                if (!string.IsNullOrEmpty(secureObjectName))
                {
                    using (var cmd = new SqlCommand(Resources.GetPermissionsVolume, conn))
                    {
                        cmd.Parameters.AddWithValue("@userID", userId);
                        cmd.Parameters.AddWithValue("@secureObjectName", secureObjectName);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            LoadVolumePermissions(da);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LoadVolumePermissions(SqlDataAdapter da)
        {
            string permission;
            var dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                permission = row["Permission"].ToString().ToLower().Replace(Convert.ToString(' '), string.Empty);
                if (!VolumePermissions.Contains(permission))
                    VolumePermissions.Add(permission);
            }
        }

        private void LoadVolumePermissions(SqlDataAdapter da, string permission)
        {
            var dt = new DataTable();
            permission = permission.ToLower().Replace(Conversions.ToString(' '), string.Empty);
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                if (!VolumePermissions.Contains(permission))
                    VolumePermissions.Add(permission);
            }
        }

        private string GetVolumeName(int directoriesId, SqlConnection conn)
        {
            try
            {
                int volumeId = 0;

                using (var cmd = new SqlCommand(Resources.GetVolumeIdFromDirectories, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", directoriesId);
                    volumeId = Conversions.ToInteger(cmd.ExecuteScalar());
                }

                if (volumeId == 0)
                    return string.Empty;

                using (var cmd = new SqlCommand(Resources.GetVolumeByID, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", volumeId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtName = new DataTable();
                        da.Fill(dtName);
                        return dtName.Rows[0]["Name"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
