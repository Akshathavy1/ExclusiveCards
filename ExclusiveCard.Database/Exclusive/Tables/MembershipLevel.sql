CREATE TABLE [Exclusive].[MembershipLevel] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (50) NULL,
    [Level]       INT           NOT NULL,
    CONSTRAINT [PK_MembershipLevel] PRIMARY KEY CLUSTERED ([Id] ASC)
);

