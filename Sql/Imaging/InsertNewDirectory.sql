INSERT INTO [Directories]([Description], [DirFullFlag], [Path], [VolumesID])
VALUES (@description, @dirFullFlag, @path, @volumesID)
SELECT SCOPE_IDENTITY()
