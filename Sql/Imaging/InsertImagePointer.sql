INSERT INTO ImagePointers (TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, ScanSequence, ScanDateTime, 
                           FileName, PageNumber, ImageHeight, ImageWidth, ImageSize, PageCount, OrgFullPath, OrgFileName)
VAlUES                    (@trackableId, @versionNumber, @directoriesId, 1, getdate(), 
                           @fileName, @pageNumber, @imageHeight, @imageWidth, @imageSize, @pageCount, @orgFullPath, @orgFileName)
SELECT SCOPE_IDENTITY()                          