CREATE TABLE [dbo].[Sessions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WorkerId] NCHAR(14) NOT NULL, 
    [Time] DATETIME NULL
)

GO

CREATE INDEX [WorkerId] ON [dbo].[Sessions] ([WorkerId])
