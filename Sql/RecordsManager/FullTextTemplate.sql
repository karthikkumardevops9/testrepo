SELECT DISTINCT IndexTableName, IndexTableId, IndexType, PageNumber, RecordVersion, AttachmentNumber  
FROM SLIndexer a 
WHERE {0}(IndexData, @SearchWord) 
  AND (IndexTableName=@TableName) 
  {1}  
  AND {2} 
  {3}
ORDER BY RecordVersion DESC 
OPTION (RECOMPILE)