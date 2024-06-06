SELECT Directories.*, 
    Volumes.ImageTableName AS ImageTableName, Volumes.DirDiskMBLimitation, Volumes.DirCountLimitation, ISNULL(Volumes.Active, 0) AS VolumeActive, 
    Directories.DirFullFlag, Directories.Id AS DirectoriesId, 
    SystemAddresses.PhysicalDriveLetter + CAST(Volumes.PathName AS VARCHAR(260)) + ISNULL(Directories.Path,'') + '\' AS FullPath 
FROM Directories
INNER JOIN         Volumes ON Directories.VolumesId = Volumes.Id 
INNER JOIN SystemAddresses ON Volumes.SystemAddressesId = SystemAddresses.Id 
WHERE  (Volumes.Active <> 0) AND (Volumes.Active IS NOT NULL) {0} 
  AND ((Directories.DirFullFlag = 0) OR (Directories.DirFullFlag IS NULL)) 
  AND (Directories.VolumesId = @volumeId)
ORDER BY Directories.VolumesId, Directories.Id