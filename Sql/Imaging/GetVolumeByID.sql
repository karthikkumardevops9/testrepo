SELECT Volumes.*, SystemAddresses.PhysicalDriveLetter, 
    SystemAddresses.PhysicalDriveLetter + CAST(Volumes.PathName AS VARCHAR(260)) AS FullPath 
FROM [Volumes] 
INNER JOIN SystemAddresses ON Volumes.SystemAddressesId = SystemAddresses.Id 
WHERE Volumes.Id = @Id