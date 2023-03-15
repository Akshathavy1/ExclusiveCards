CREATE TABLE [Exclusive].[PartnerRewards] (
    [Id]                      INT             IDENTITY (1, 1) NOT NULL,
    [RewardKey]               NVARCHAR (20)   NOT NULL,
    [PartnerId]               INT             NULL,
    [CreatedDate]             DATETIME2 (7)   NOT NULL,
    [LatestValue]             DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [ValueDate]               DATETIME2 (7)   NULL,
    [TotalConfirmedWithdrawn] DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [Password]                NVARCHAR (255)  NULL,
    CONSTRAINT [PK_PartnerRewards] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PartnerRewards_Partner_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Exclusive].[Partner] ([Id]), 
    CONSTRAINT [CK_PartnerRewards_RewardKeyUnique] UNIQUE ([RewardKey])
);


GO
CREATE NONCLUSTERED INDEX [IX_PartnerRewards_PartnerId]
    ON [Exclusive].[PartnerRewards]([PartnerId] ASC);

