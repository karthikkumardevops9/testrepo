DECLARE @value as int

SELECT @value = (SELECT [Attachments] FROM [Tables] WHERE [TableName] = @tableName)
SELECT @value = AVG(ISNULL(@value , 0))
SELECT @value 