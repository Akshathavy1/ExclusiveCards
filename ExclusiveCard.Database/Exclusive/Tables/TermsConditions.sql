CREATE TABLE [Exclusive].[TermsConditions] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Url]       NVARCHAR (100) NULL,
    [ValidFrom] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_TermsConditions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

