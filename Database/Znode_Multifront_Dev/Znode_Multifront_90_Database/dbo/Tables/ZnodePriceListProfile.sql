CREATE TABLE [dbo].[ZnodePriceListProfile] (
    [PriceListProfileId] INT      IDENTITY (1, 1) NOT NULL,
    [PriceListId]        INT      NOT NULL,
    [PortalProfileId]    INT      NULL,
    [Precedence]         INT      NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePriceListProfile] PRIMARY KEY CLUSTERED ([PriceListProfileId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePriceListProfile_ZnodePortalProfile] FOREIGN KEY ([PortalProfileId]) REFERENCES [dbo].[ZnodePortalProfile] ([PortalProfileID]),
    CONSTRAINT [FK_ZnodePriceListProfile_ZnodePriceList] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId])
);





