SELECT Trackables.RecordTypesId, Volumes.ImageTableName AS ImageTableName, Attachments.[FileName], Attachments.OrgFullPath, Attachments.OrgFileName, 
    SystemAddresses.PhysicalDriveLetter + CAST(Volumes.PathName AS VARCHAR(260)) + ISNULL(Directories.Path,'') + '\' + Attachments.[FileName] AS FullPath 
    FROM 
   (SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, ImagePointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, PageNumber, [PageCount], [FileName] FROM ImagePointers 
    UNION 
    SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, PCFilesPointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, PageNumber, 1 AS [PageCount], [FileName] FROM PCFilesPointers) 
    AS Attachments
INNER JOIN     Directories ON Attachments.ScanDirectoriesId = Directories.Id 
INNER JOIN         Volumes ON Directories.VolumesId = Volumes.Id 
INNER JOIN SystemAddresses ON Volumes.SystemAddressesId = SystemAddresses.Id 
INNER JOIN      Trackables ON Attachments.TrackablesRecordVersion = Trackables.RecordVersion 
                          AND Attachments.TrackablesId = Trackables.Id 
WHERE (Attachments.PointerId = @pointerId)  
  AND (Trackables.RecordTypesId = @attachmentType) 
