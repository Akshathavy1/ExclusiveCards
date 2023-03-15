CREATE TABLE [Exclusive].[AspNetUserRoles] (
    [UserId] NVARCHAR (450) NOT NULL,
    [RoleId] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Exclusive].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Exclusive].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

