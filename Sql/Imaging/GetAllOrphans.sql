SELECT ROW_NUMBER() OVER(ORDER BY SecureVault.TrackablesId DESC) AS RowNum, SecureVault.* 
FROM 
(SELECT DISTINCT Trackables.Id AS TrackablesId, Trackables.RecordVersion, Attachments.PageNumber AS PageNumber, Trackables.RecordTypesId AS RecordType, Trackables.Orphan, 
 'Orphans' AS TableUserName, 'Orphans' AS IndexTable, '' AS IndexTableId, 1 AS AttachmentNumber, Trackables.OfficialRecord, Volumes.ImageTableName AS ImageTableName, 
 Attachments.ScanDirectoriesId, Attachments.[FileName], Attachments.PointerId, Attachments.OrgFullPath, Attachments.OrgFileName, Attachments.ScanDateTime, 
 SystemAddresses.PhysicalDriveLetter + CAST(Volumes.PathName AS VARCHAR(260)) + ISNULL(Directories.Path,'') + '\' + Attachments.[FileName] AS FullPath, 
 Attachments.ScanBatchesId, Attachments.ScanSequence, ScanBatches.BatchStartDateTime AS ScanDate, ScanBatches.PageCount AS ScanPageCount, 
 ScanBatches.DocumentCount, ScanBatches.BelowDeleteSizeCount, ScanBatches.RescannedCount, ScanBatches.AutoIndexedCount, ScanBatches.LastScanSequence, 
 ScanBatches.ScanRulesIdUsed, ScanBatches.UserName AS ScanUserName, Attachments.TrackablesRecordVersion, Volumes.[Name] AS VolumnName 
 FROM 
 (SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, ImagePointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, PageNumber, [PageCount], [FileName], ScanBatchesId, ScanSequence, ScanDateTime FROM ImagePointers 
  UNION 
  SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, PCFilesPointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, PageNumber, 1 AS [PageCount], [FileName], ScanBatchesId, ScanSequence, ScanDateTime FROM PCFilesPointers 
 ) AS Attachments 
 INNER JOIN            Directories ON Directories.Id = Attachments.ScanDirectoriesId 
 INNER JOIN                Volumes ON Volumes.Id = Directories.VolumesId 
 INNER JOIN        SystemAddresses ON SystemAddresses.Id = Volumes.SystemAddressesId 
 INNER JOIN             Trackables ON Trackables.Id = Attachments.TrackablesId 
                                  AND Trackables.RecordVersion = Attachments.TrackablesRecordVersion 
 INNER JOIN           SecureObject ON SecureObject.[Name] = Volumes.[Name] 
                                  AND SecureObject.SecureObjectTypeID = 11 
                                  AND SecureObject.BaseID = 11 
 INNER JOIN SecureObjectPermission ON SecureObjectPermission.SecureObjectID = SecureObject.SecureObjectID 
                                  AND SecureObjectPermission.PermissionID = 3 
 INNER JOIN        SecureUserGroup ON SecureUserGroup.GroupID = SecureObjectPermission.GroupID 
 LEFT OUTER JOIN       ScanBatches ON ScanBatches.Id = Attachments.ScanBatchesId 
 WHERE (Trackables.Orphan <> 0) 
   AND (Trackables.Orphan IS NOT NULL) 
   AND (SecureUserGroup.UserID = @userid 
    OR  SecureObjectPermission.GroupID = -1)
) AS SecureVault
WHERE ISNULL(SecureVault.OrgFileName, '') like '%' + @FILTER + '%'
ORDER BY SecureVault.TrackablesId DESC, SecureVault.RecordVersion DESC, SecureVault.PageNumber, SecureVault.PointerId
OFFSET @OFFSET ROWS
FETCH NEXT @PerPageRecord ROWS ONLY