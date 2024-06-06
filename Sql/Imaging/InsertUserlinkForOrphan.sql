DECLARE @attachmentNumber INT

SELECT @attachmentNumber = 
    (SELECT TOP 1 UserLinks.AttachmentNumber FROM UserLinks 
       WHERE (UserLinks.IndexTable = @tableName) 
        AND (UserLinks.IndexTableId = @tableId) 
       ORDER BY UserLinks.AttachmentNumber DESC) + 1

SELECT @attachmentNumber = AVG(ISNULL(@attachmentNumber, 1))

INSERT INTO Userlinks (TrackablesId, RecordVersion, IndexTableId, IndexTable, AttachmentNumber, VersionUpdated)
	VALUES (@trackableId, 1, @tableId, @tableName, @attachmentNumber, 1)
		