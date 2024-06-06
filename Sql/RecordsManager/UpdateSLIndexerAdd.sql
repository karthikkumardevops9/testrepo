INSERT INTO SLIndexer (IndexType, IndexTableName, IndexFieldName, IndexTableID, IndexData, OrphanType, RecordVersion, PageNumber, AttachmentNumber) 
     SELECT IndexType, IndexTableName, IndexFieldName, @IndexTableID, @IndexData, @OrphanType, @RecordVersion, @PageNumber, @AttachmentNumber 
       FROM SLTextSearchItems
      WHERE (SLTextSearchItems.IndexType = @IndexType) 
        AND (SLTextSearchItems.IndexTableName = @IndexTableName) 
        AND (SLTextSearchItems.IndexFieldName = @IndexFieldName)






