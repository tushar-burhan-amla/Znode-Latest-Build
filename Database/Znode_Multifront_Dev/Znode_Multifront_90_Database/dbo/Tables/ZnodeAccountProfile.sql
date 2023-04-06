CREATE TABLE [dbo].[ZnodeAccountProfile] (
    [AccountProfileId] INT      IDENTITY (1, 1) NOT NULL,
    [AccountId]        INT      NULL,
    [ProfileId]        INT      NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    [IsDefault]        BIT      NOT NULL,
    CONSTRAINT [PK_ZnodeAccountProfile] PRIMARY KEY CLUSTERED ([AccountProfileId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeAccountProfile_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId]),
    CONSTRAINT [FK_ZnodeAccountProfile_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId])
);
GO
CREATE NONCLUSTERED INDEX [IDX_ZnodeAccountProfile_ProfileId]
ON [dbo].[ZnodeAccountProfile] ([ProfileId])
INCLUDE ([AccountId])
