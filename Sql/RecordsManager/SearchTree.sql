With LowerRelationships 
AS (SELECT p.LowerTableName AS ChildTable, t.IdFieldName AS ChildKey, p.LowerTableFieldName AS ChildForKey, 0 AS [Level]
	FROM RelationShips p INNER JOIN Tables t ON t.TableName = p.LowerTableName WHERE p.UpperTableName = @ParentTable
	UNION ALL
	SELECT c.LowerTableName AS ChildTable, t.IdFieldName AS ChildKey, c.LowerTableFieldName AS ChildForKey, l.[Level] + 1
	FROM RelationShips c INNER JOIN LowerRelationships l ON l.ChildTable = c.UpperTableName INNER JOIN Tables t ON t.TableName = c.LowerTableName
)

SELECT * FROM LowerRelationships 