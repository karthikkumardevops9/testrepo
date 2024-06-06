UPDATE Annotations SET [Annotation] = @annotation, [NoteDateTime] = getdate(), 
                       [DeskOf] = @userName, [UserName] = @userName 
 WHERE [TableId] = @tableId AND [Table] = 'REDLINE'