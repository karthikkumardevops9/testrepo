IF EXISTS(SELECT 1 FROM SLIndexer 
          INNER JOIN SLTextSearchItems ON SLTextSearchItems.IndexType = SLIndexer.IndexType 
                                      AND SLTextSearchItems.IndexTableName = SLIndexer.IndexTableName 
                                      AND SLTextSearchItems.IndexFieldName = SLIndexer.IndexFieldName
          WHERE (SLIndexer.IndexType = @IndexType) 
            AND (SLIndexer.IndexTableName = @IndexTableName) 
            AND (SLIndexer.IndexFieldName = @IndexFieldName)
            AND (SLIndexer.IndexTableID = @IndexTableID)) 
BEGIN
    UPDATE SLIndexer SET IndexData=@IndexData, OrphanType=@OrphanType, RecordVersion=@RecordVersion, PageNumber=@PageNumber, AttachmentNumber=@AttachmentNumber 
    FROM SLIndexer 
    INNER JOIN SLTextSearchItems ON SLIndexer.IndexType = SLTextSearchItems.IndexType 
                                AND SLIndexer.IndexTableName = SLTextSearchItems.IndexTableName 
                                AND SLIndexer.IndexFieldName = SLTextSearchItems.IndexFieldName
    WHERE (SLIndexer.IndexType = @IndexType) 
      AND (SLIndexer.IndexTableName = @IndexTableName) 
      AND (SLIndexer.IndexFieldName = @IndexFieldName)
      AND (SLIndexer.IndexTableID = @IndexTableID)
END
ELSE 
BEGIN
    INSERT INTO SLIndexer (IndexType, IndexTableName, IndexFieldName, IndexTableID, IndexData, OrphanType, RecordVersion, PageNumber, AttachmentNumber) 
    SELECT IndexType, IndexTableName, IndexFieldName, @IndexTableID, @IndexData, @OrphanType, @RecordVersion, @PageNumber, @AttachmentNumber 
    FROM SLTextSearchItems
    WHERE (SLTextSearchItems.IndexType = @IndexType) 
      AND (SLTextSearchItems.IndexTableName = @IndexTableName) 
      AND (SLTextSearchItems.IndexFieldName = @IndexFieldName)
END
