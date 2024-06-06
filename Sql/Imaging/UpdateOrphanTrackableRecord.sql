UPDATE [Trackables] 
SET [Orphan] = 0, [RecordVersion] = @newVersion, [ID] = @newTrackableID 
WHERE [ID] = @oldTrackableID AND [RecordVersion] = 1
