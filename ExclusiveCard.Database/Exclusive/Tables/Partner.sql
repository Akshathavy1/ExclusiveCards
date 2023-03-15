CREATE TABLE [Exclusive].[Partner] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NULL,
    [ContactDetailId] INT            NULL,
    [BankDetailsId]   INT            NULL,
    [IsDeleted]       BIT            NOT NULL,
    [Type]            INT            DEFAULT ((1)) NOT NULL,
    [ImagePath]       NVARCHAR (512) NULL,
    [ManagementURL]   NVARCHAR (512) NULL,
    [MembershipCardPrefix] CHAR(2) NULL, 
    [AspNetUserId] NVARCHAR(450) NULL, 
    CONSTRAINT [PK_Partner] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Partner_BankDetail_BankDetailsId] FOREIGN KEY ([BankDetailsId]) REFERENCES [Exclusive].[BankDetail] ([Id]),
    CONSTRAINT [FK_Partner_ContactDetail_ContactDetailId] FOREIGN KEY ([ContactDetailId]) REFERENCES [Exclusive].[ContactDetail] ([Id]),
    CONSTRAINT [FK_Partner_AspNetUsers_AspNetUserId] FOREIGN KEY ([AspNetUserId]) REFERENCES [Exclusive].[AspNetUsers] ([Id])
);

