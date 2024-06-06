SELECT o.Id, o.DefaultOutputSettingsId, o.DirectoriesId, o.FileExtension, o.NextDocNum, o.VolumesId, ISNULL(o.InActive, 0) AS InActive, 
     ISNULL(v.Active, 0) AS VolumeActive, v.[Name] AS VolumeName, v.ImageTableName AS ImageTableName, v.DirDiskMBLimitation, v.DirCountLimitation, d.DirFullFlag,
    s.PhysicalDriveLetter + CAST(v.PathName AS VARCHAR(260)) + ISNULL(d.Path,'') + '\' AS FullPath 
FROM OutputSettings o
INNER JOIN     Directories d ON d.Id = o.DirectoriesId 
INNER JOIN         Volumes v ON v.Id = o.VolumesId 
INNER JOIN SystemAddresses s ON s.Id = v.SystemAddressesId 
WHERE (d.VolumesId IN (SELECT dr.VolumesId FROM Directories dr INNER JOIN Volumes vo ON vo.Id = dr.VolumesId WHERE (dr.Id = @directoriesId)))
ORDER BY ISNULL(o.InActive, 0) ASC, ISNULL(v.Active, 0) DESC