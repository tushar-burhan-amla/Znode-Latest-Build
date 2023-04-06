CREATE TABLE [dbo].[ZnodeCMSSpecialOfferContent] (
    [CMSSpecialOfferContentId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]                 INT            NOT NULL,
    [Title]                    NVARCHAR (100) NOT NULL,
    [ActivationDate]           DATETIME       NOT NULL,
    [ExpirationDate]           DATETIME       NOT NULL,
    [Description]              NVARCHAR (300) NULL,
    [CreatedBy]                INT            NOT NULL,
    [CreatedDate]              DATETIME       NOT NULL,
    [ModifiedBy]               INT            NOT NULL,
    [ModifiedDate]             DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSpecialOfferContent] PRIMARY KEY CLUSTERED ([CMSSpecialOfferContentId] ASC)
);

