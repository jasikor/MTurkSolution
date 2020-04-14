CREATE TABLE [dbo].[Moves]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GameId] INT NOT NULL, 
    [Time] DATETIME NOT NULL, 
    [MoveBy] NCHAR(4) NOT NULL, 
    [ProposedAmount] INT NOT NULL, 
    [OfferAccepted] BIT NOT NULL, 
    CONSTRAINT [FK_Moves_To] FOREIGN KEY ([GameId]) REFERENCES [Games]([Id])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'TURK, MACH, SYST',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Moves',
    @level2type = N'COLUMN',
    @level2name = N'MoveBy'