INSERT INTO Annotations (TableId, [Table], Annotation, DeskOf, NoteDateTime, UserName) 
SELECT @newId, [Table], Annotation, DeskOf, NoteDateTime, UserName
FROM Annotations WHERE [Table] = 'REDLINE' AND TableId = @currentId
