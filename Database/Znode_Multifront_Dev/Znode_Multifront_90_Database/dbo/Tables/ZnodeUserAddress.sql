CREATE TABLE [dbo].[ZnodeUserAddress] (
    [UserAddressId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]        INT      NOT NULL,
    [AddressId]     INT      NOT NULL,
    [CreatedBy]     INT      NOT NULL,
    [CreatedDate]   DATETIME NOT NULL,
    [ModifiedBy]    INT      NOT NULL,
    [ModifiedDate]  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeUserAddress] PRIMARY KEY CLUSTERED ([UserAddressId] ASC),
    CONSTRAINT [FK_ZnodeUserAddress_ZnodeAddress] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[ZnodeAddress] ([AddressId]),
    CONSTRAINT [FK_ZnodeUserAddress_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);




GO
CREATE NONCLUSTERED INDEX [Idx_ZnodeUserAddress_UserId_AddressId]
    ON [dbo].[ZnodeUserAddress]([UserId] ASC, [AddressId] ASC);

