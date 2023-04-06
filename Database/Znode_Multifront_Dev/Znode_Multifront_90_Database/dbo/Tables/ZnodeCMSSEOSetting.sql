CREATE TABLE [dbo].[ZnodeCMSSEOSetting] (
    [CMSSEOSettingId]   INT            IDENTITY (1, 1) NOT NULL,
    [Title]             NVARCHAR (100) NOT NULL,
    [Keywords]          NVARCHAR (MAX) NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [PageName]          NVARCHAR (100) NOT NULL,
    [IdRedirect]        BIT            NOT NULL,
    [ProductId]         INT            NULL,
    [CategoryId]        INT            NULL,
    [CMSContentPagesId] INT            NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSEOSetting] PRIMARY KEY CLUSTERED ([CMSSEOSettingId] ASC)
);

