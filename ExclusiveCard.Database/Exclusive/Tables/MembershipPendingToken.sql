CREATE TABLE [Exclusive].[MembershipPendingToken] (
    [Id]                           INT              IDENTITY (1, 1) NOT NULL,
    [MembershipRegistrationCodeId] INT              NOT NULL,
    [Token]                        UNIQUEIDENTIFIER NOT NULL,
    [DateCreated]                  DATETIME2 (7)    NOT NULL,
    CONSTRAINT [PK_MembershipPendingToken] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MembershipPendingToken_MembershipRegistrationCode_MembershipRegistrationCodeId] FOREIGN KEY ([MembershipRegistrationCodeId]) REFERENCES [Exclusive].[MembershipRegistrationCode] ([Id]) ON DELETE CASCADE
);

