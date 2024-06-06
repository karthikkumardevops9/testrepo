UPDATE Trackables SET PageCount = PageCount + @pageCount 
WHERE (Id = @trackableId)
  AND ((RecordVersion = @versionNumber) OR ((RecordVersion < 0) AND (CheckedOutUserId = @userId)))

