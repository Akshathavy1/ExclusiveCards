CREATE TABLE [Exclusive].[Tag] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Tags]     NVARCHAR (50) NULL,
    [TagType]  NVARCHAR (50) NULL,
    [IsActive] BIT           NOT NULL,
    CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED ([Id] ASC)
);

