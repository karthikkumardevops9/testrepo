BEGIN TRY  
	Begin Tran
	 
	UPDATE [Trackables] SET [Orphan] = 0, [RecordVersion] = 1, [ID] = @trackableId 
	WHERE [ID] = @trackableId AND [RecordVersion] = 1


	INSERT INTO Userlinks (TrackablesId, RecordVersion, IndexTableId, IndexTable, AttachmentNumber, VersionUpdated)
			                                VALUES (@trackableId, 1, @recordId, @tableName, @attachmentNumber, 1)

	commit tran

END TRY  
BEGIN CATCH  
     Rollback Tran
END CATCH