IN (SELECT CAST(IndexTableId AS VARCHAR) FROM SLIndexer a WHERE IndexTableName = '{0}' 
AND {1}
{2}
{3}
AND CONTAINS(IndexData, '{4}'))
