
  AND ((RecordVersion = (SELECT TOP 1 RecordVersion FROM SLIndexer 
                         WHERE IndexTableName=a.IndexTableName and IndexTableId=a.IndexTableId 
                         ORDER BY RecordVersion Desc)  
   OR (RecordVersion=0) OR (RecordVersion IS NULL))) 
