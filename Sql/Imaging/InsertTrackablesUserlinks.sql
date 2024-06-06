IF EXISTS(SELECT 1 FROM Userlinks WHERE IndexTable = @tableName AND IndexTableId = @tableId AND AttachmentNumber = @attachmentNumber) BEGIN
	IF EXISTS(SELECT 1 FROM Trackables WHERE Id = (SELECT TrackablesId FROM Userlinks WHERE IndexTable = @tableName AND IndexTableId = @tableId AND AttachmentNumber = @attachmentNumber)) BEGIN
		SET @attachmentNumber = (SELECT TOP 1 AttachmentNumber + 1 FROM Userlinks WHERE IndexTable = @tableName AND IndexTableId = @tableId ORDER BY AttachmentNumber DESC)

		IF (SELECT @versionNumber) = 1 BEGIN
			IF (SELECT @orphan) = 0 BEGIN
				INSERT INTO Userlinks (TrackablesId, RecordVersion, IndexTableId, IndexTable, AttachmentNumber, VersionUpdated)
				VALUES (@trackableId, @versionNumber, @tableId, @tableName, @attachmentNumber, 1)
			END
		END
	END
	ELSE BEGIN
		SET @trackableId = (SELECT TrackablesId FROM Userlinks WHERE IndexTable = @tableName AND IndexTableId = @tableId AND AttachmentNumber = @attachmentNumber)
	END

	INSERT INTO Trackables (Id, RecordVersion, RecordTypesId, PageCount, Orphan, OfficialRecord, OfficialRecordReconciliation)
	VALUES (@trackableId, @versionNumber, @trackableType, @pageCount, @orphan, @officialRecord, @officialRecord)
END
ELSE BEGIN
	INSERT INTO Trackables (Id, RecordVersion, RecordTypesId, PageCount, Orphan, OfficialRecord, OfficialRecordReconciliation)
	VALUES (@trackableId, @versionNumber, @trackableType, @pageCount, @orphan, @officialRecord, @officialRecord)

	IF (SELECT @versionNumber) = 1 BEGIN
		IF (SELECT @orphan) = 0 BEGIN
			INSERT INTO Userlinks (TrackablesId, RecordVersion, IndexTableId, IndexTable, AttachmentNumber, VersionUpdated)
			VALUES (@trackableId, @versionNumber, @tableId, @tableName, @attachmentNumber, 1)
		END
	END
END

SELECT @trackableId