IF (@toPageNumber < @fromPageNumber)
BEGIN
UPDATE ImagePointers SET PageNumber = PageNumber + 1
    FROM ImagePointers p
    INNER JOIN  Userlinks ON p.TrackablesId = Userlinks.TrackablesId
    WHERE (UserLinks.IndexTable = @tableName) 
      AND (UserLinks.IndexTableId = @tableId) 
      AND (UserLinks.AttachmentNumber = @attachmentNumber)
      AND (p.TrackablesRecordVersion < 0)
      AND (p.PageNumber >= @toPageNumber AND p.PageNumber < @fromPageNumber)
END
ELSE
BEGIN
    UPDATE ImagePointers SET PageNumber = PageNumber - 1
    FROM ImagePointers p
    INNER JOIN  Userlinks ON p.TrackablesId = Userlinks.TrackablesId
    WHERE (UserLinks.IndexTable = @tableName) 
      AND (UserLinks.IndexTableId = @tableId) 
      AND (UserLinks.AttachmentNumber = @attachmentNumber)
      AND (p.TrackablesRecordVersion < 0)
      AND (p.PageNumber <= @toPageNumber AND p.PageNumber > @fromPageNumber)
END

UPDATE ImagePointers SET PageNumber = @toPageNumber WHERE Id = @Id
