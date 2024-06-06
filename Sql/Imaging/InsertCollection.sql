INSERT INTO [SLCollections] ([Name], [Operator], [CreateDate]) VALUES (@userId, @userId, getdate())
SELECT SCOPE_IDENTITY()

