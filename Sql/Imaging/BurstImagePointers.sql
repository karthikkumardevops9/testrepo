INSERT INTO ImagePointers (TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, ScanSequence, ScanDateTime,
    [FileName], PageNumber, CRC, Orientation, Skew, Front, ImageHeight, ImageWidth, ImageSize, 
    BarCodeCount, BarCodes, [PageCount], OrgDirectoriesId, OrgFileName, OrgFullPath) 
SELECT TrackablesId, @versionNumber, ScanDirectoriesId, 1, getdate(), 
    @fileName,  @pageNumber, CRC, Orientation, Skew, Front, ImageHeight, ImageWidth, ImageSize, 
    BarCodeCount, BarCodes, 1, OrgDirectoriesId, OrgFileName, OrgFullPath 
FROM ImagePointers WHERE Id = @Id;
SELECT SCOPE_IDENTITY()
