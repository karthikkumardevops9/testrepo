INSERT INTO PCFilesPointers (TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, 
    FileName, Pages, OrgDirectoriesId, OrgFileName, PCFilesEditGrp, PCFilesNVerGrp, OrgFullPath)
SELECT TrackablesId, @versionNumber, ScanDirectoriesId, 
    @fileName AS FileName, Pages, OrgDirectoriesId, OrgFileName, PCFilesEditGrp, PCFilesNVerGrp, OrgFullPath
FROM PCFilesPointers WHERE Id = @Id;
SELECT SCOPE_IDENTITY()
