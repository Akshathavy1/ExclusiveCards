CREATE TABLE [Exclusive].[MigrationMapping] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [OldId]      BIGINT         NOT NULL,
    [NewId]      INT            NOT NULL,
    [EntityType] NVARCHAR (100) NULL,
    CONSTRAINT [PK_MigrationMapping] PRIMARY KEY CLUSTERED ([Id] ASC)
);

