CREATE TABLE [dbo].[GameParameters]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Surplus] INT NOT NULL, 
    [TurksDisValue] INT NOT NULL, 
    [MachineDisValue] INT NOT NULL, 
    [TimeOut] INT NOT NULL, 
    [Stubborn] FLOAT NOT NULL, 
    [MachineStarts] BIT NOT NULL
)
