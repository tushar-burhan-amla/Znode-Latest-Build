CREATE TABLE [dbo].[ZnodePimAddonGroupLocale] (
    [PimAddonGroupLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [PimAddonGroupId]       INT            NOT NULL,
    [AddonGroupName]        NVARCHAR (MAX) NOT NULL,
    [LocaleId]              INT            NOT NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAddonGroupLocale] PRIMARY KEY CLUSTERED ([PimAddonGroupLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimAddonGroupLocale_ZnodePimAddonGroup] FOREIGN KEY ([PimAddonGroupId]) REFERENCES [dbo].[ZnodePimAddonGroup] ([PimAddonGroupId])
);



