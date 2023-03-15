CREATE TABLE [Exclusive].[OfferRedemption] (
    [MembershipCardId] INT           NOT NULL,
    [OfferId]          INT           NOT NULL,
    [State]            INT           NOT NULL,
    [FileId]           INT           NULL,
    [CreatedDate]      DATETIME2 (7) NOT NULL,
    [UpdatedDate]      DATETIME2 (7) NULL,
    [CustomerRef]      NVARCHAR (20) NULL,
    CONSTRAINT [PK_OfferRedemption] PRIMARY KEY CLUSTERED ([MembershipCardId] ASC, [OfferId] ASC),
    CONSTRAINT [FK_OfferRedemption_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [Exclusive].[Files] ([Id]),
    CONSTRAINT [FK_OfferRedemption_MembershipCard_MembershipCardId] FOREIGN KEY ([MembershipCardId]) REFERENCES [Exclusive].[MembershipCard] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferRedemption_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferRedemption_Status_State] FOREIGN KEY ([State]) REFERENCES [Exclusive].[Status] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_OfferRedemption_State]
    ON [Exclusive].[OfferRedemption]([State] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OfferRedemption_OfferId]
    ON [Exclusive].[OfferRedemption]([OfferId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OfferRedemption_FileId]
    ON [Exclusive].[OfferRedemption]([FileId] ASC);

