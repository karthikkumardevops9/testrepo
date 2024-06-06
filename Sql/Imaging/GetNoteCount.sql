SELECT COUNT(Id) 
  FROM Annotations
 WHERE TableId = @pointerId
   AND [Table] = @noteTable