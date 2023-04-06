CREATE TABLE [dbo].[ZnodeUserProfile] (
    [UserProfileID] INT      IDENTITY (1, 1) NOT NULL,
    [ProfileId]     INT      NULL,
    [UserId]        INT      NOT NULL,
    [IsDefault]     BIT      CONSTRAINT [DF_ZnodeUserProfile_IsDefault] DEFAULT ((0)) NULL,
    [CreatedBy]     INT      NOT NULL,
    [CreatedDate]   DATETIME NOT NULL,
    [ModifiedBy]    INT      NOT NULL,
    [ModifiedDate]  DATETIME NOT NULL,
    CONSTRAINT [PK_ZNodePortalAccount] PRIMARY KEY CLUSTERED ([UserProfileID] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZNodeUserProfile_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId]),
    CONSTRAINT [FK_ZnodeUserProfile_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);








GO
CREATE NONCLUSTERED INDEX [IX_ZnodeUserProfile_UserId]
    ON [dbo].[ZnodeUserProfile]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodeUserProfile_ProfileId_UserId]
    ON [dbo].[ZnodeUserProfile]([ProfileId] ASC, [UserId] ASC);

