﻿CREATE TABLE [dbo].[Games]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SessionId] INT NOT NULL, 
    [GameParameterId] INT NULL,
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME,
    [Surplus] INT NOT NULL, 
    [TurksDisValue] INT NOT NULL, 
    [MachineDisValue] INT NOT NULL, 
    [TimeOut] INT NOT NULL, 
    [Stubborn] FLOAT NOT NULL, 
    [MachineStarts] BIT NOT NULL, 
    [TurksProfit] INT NULL, 
    [ShowMachinesDisValue] BIT NOT NULL DEFAULT 0, 
    [AlgoVersion] NTEXT NULL, 
    CONSTRAINT [FK_Games_ToSessions] FOREIGN KEY ([SessionId]) REFERENCES [Sessions]([Id]), 
    CONSTRAINT [FK_Games_ToGameParameters] FOREIGN KEY ([GameParameterId]) REFERENCES [GameParameters]([Id])
)
