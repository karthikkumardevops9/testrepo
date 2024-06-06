IF (@imageTableName <> '')
BEGIN
    IF (@imageTableName = 'NULL')
    BEGIN
        IF (@pageNumber = 0)
        BEGIN
            UPDATE ImagePointers SET FileName = @fileName, OrgFullPath = NULL, ScanDirectoriesID = @directoryId, PageCount = @pageCount WHERE Id = @Id
        END
        ELSE
        BEGIN
            UPDATE ImagePointers SET FileName = @fileName, OrgFullPath = NULL, ScanDirectoriesID = @directoryId, PageCount = @pageCount, PageNumber = @pageNumber WHERE Id = @Id
        END
    END
    ELSE
    BEGIN
        IF (@pageNumber = 0)
        BEGIN
            UPDATE ImagePointers SET FileName = @fileName, OrgFullPath = @imageTableName, ScanDirectoriesID = @directoryId, PageCount = @pageCount WHERE Id = @Id
        END
        ELSE
        BEGIN
            UPDATE ImagePointers SET FileName = @fileName, OrgFullPath = @imageTableName, ScanDirectoriesID = @directoryId, PageCount = @pageCount, PageNumber = @pageNumber WHERE Id = @Id
        END
    END
END
ELSE
BEGIN
    IF (@pageNumber = 0)
    BEGIN
        UPDATE ImagePointers SET FileName = @fileName, ScanDirectoriesID = @directoryId, PageCount = @pageCount WHERE Id = @Id
    END
    ELSE
    BEGIN
        UPDATE ImagePointers SET FileName = @fileName, ScanDirectoriesID = @directoryId, PageCount = @pageCount, PageNumber = @pageNumber WHERE Id = @Id
    END
END
