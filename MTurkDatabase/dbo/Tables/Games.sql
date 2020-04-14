CREATE TABLE [dbo].[Games]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SessionId] INT NOT NULL, 
    [StartTime] DATETIME NOT NULL, 
    [TurksDisValue] INT NOT NULL, 
    [MachineDisValue] INT NOT NULL, 
    CONSTRAINT [FK_Games_ToSessions] FOREIGN KEY ([SessionId]) REFERENCES [Sessions]([Id])
)
