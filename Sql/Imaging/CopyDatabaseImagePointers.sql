INSERT INTO ImagePointers (TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, ScanDateTime, 
    FileName, PageNumber, CRC, Orientation, Skew, Front, ImageHeight, ImageWidth, ImageSize, 
    BarCodeCount, BarCodes, PageCount, OrgDirectoriesId, OrgFileName, OrgFullPath)
SELECT TrackablesId, @versionNumber, ScanDirectoriesId, getdate() AS ScanDateTime, 
         @fileName AS FileName, @pageNumber, CRC, Orientation, Skew, Front, ImageHeight, ImageWidth, ImageSize, 
    BarCodeCount, BarCodes, PageCount, OrgDirectoriesId, OrgFileName, OrgFullPath
FROM ImagePointers WHERE Id = @Id;
SELECT SCOPE_IDENTITY()
