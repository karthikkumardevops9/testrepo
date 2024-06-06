IF (@attachmentType = 5)
BEGIN
    UPDATE PCFilesPointers SET OrgFileName = @orgFileName WHERE Id = @pointerId
END
ELSE
BEGIN
    UPDATE ImagePointers SET OrgFileName = @orgFileName WHERE Id = @pointerId
END
