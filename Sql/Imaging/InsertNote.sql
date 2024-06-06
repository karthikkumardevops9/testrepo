INSERT INTO Annotations ([Table], TableId, NoteDateTime, Annotation, UserName, DeskOf)
VALUES                  (@noteTable, @pointerId, getdate(), @Memo, @userName, @displayName)
SELECT SCOPE_IDENTITY()