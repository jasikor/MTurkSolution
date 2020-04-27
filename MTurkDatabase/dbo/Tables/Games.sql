CREATE TABLE [dbo].[Games]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SessionId] INT NOT NULL, 
    [StartTime] DATETIME NOT NULL, 
    [Surplus] INT NOT NULL, 
    [TurksDisValue] INT NOT NULL, 
    [MachineDisValue] INT NOT NULL, 
    [TimeOut] INT NOT NULL, 
    [Stubborn] FLOAT NOT NULL, 
    [MachineStarts] BIT NOT NULL, 
    CONSTRAINT [FK_Games_ToSessions] FOREIGN KEY ([SessionId]) REFERENCES [Sessions]([Id])
)
