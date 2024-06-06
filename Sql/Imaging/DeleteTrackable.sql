DELETE FROM Trackables 
WHERE (Id = @trackableId)
  AND (RecordVersion = @versionNumber)
 
SELECT Id FROM Trackables WHERE Id = @trackableId