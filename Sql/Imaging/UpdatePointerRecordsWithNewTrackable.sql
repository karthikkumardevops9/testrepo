UPDATE ImagePointers set ImagePointers.TrackablesID = @newTrackableID, ImagePointers.TrackablesRecordVersion = @newVersion 
WHERE ImagePointers.TrackablesID = @oldTrackableID AND ImagePointers.TrackablesRecordVersion = 1

UPDATE PCFilesPointers set PCFilesPointers.TrackablesID = @newTrackableID, PCFilesPointers.TrackablesRecordVersion = @newVersion 
WHERE PCFilesPointers.TrackablesID = @oldTrackableID AND PCFilesPointers.TrackablesRecordVersion = 1