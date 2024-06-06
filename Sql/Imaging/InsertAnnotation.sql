INSERT INTO Annotations ([TableId], [Table], [Annotation], [DeskOf], [NoteDateTime], [UserName])
VALUES (@tableId, 'REDLINE', @annotation, @userName, getdate(), @userName)
