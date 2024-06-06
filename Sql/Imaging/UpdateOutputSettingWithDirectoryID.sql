UPDATE [OutputSettings] SET [DirectoriesID] = @DirectoriesID  WHERE [ID] = @Name AND [DirectoriesID] <> @DirectoriesID 

SELECT OutputSettings.*, 
    Volumes.ImageTableName AS ImageTableName, ISNULL(Volumes.Active, 0) AS VolumeActive, 
    SystemAddresses.PhysicalDriveLetter + CAST(Volumes.PathName AS VARCHAR(260)) + ISNULL(Directories.Path,'') + '\' AS FullPath 
FROM OutputSettings 
INNER JOIN     Directories ON OutputSettings.DirectoriesId = Directories.Id 
INNER JOIN         Volumes ON Directories.VolumesId = Volumes.Id 
INNER JOIN SystemAddresses ON Volumes.SystemAddressesId = SystemAddresses.Id 
WHERE (OutputSettings.InActive = 0 OR OutputSettings.InActive IS NULL)
  AND (Volumes.Active <> 0) AND (Volumes.Active IS NOT NULL) 
  AND (OutputSettings.ID = @Name)