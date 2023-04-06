CREATE TABLE [dbo].[ZnodeApplicationSetting] (
    [ApplicationSettingId]    INT             IDENTITY (1, 1) NOT NULL,
    [GroupName]               VARCHAR (100)   NULL,
    [ItemName]                VARCHAR (100)   NULL,
    [Setting]                 NVARCHAR (MAX)  NULL,
    [ViewOptions]             NVARCHAR (MAX)  NULL,
    [FrontPageName]           NVARCHAR (200)  NULL,
    [FrontObjectName]         NVARCHAR (200)  NULL,
    [IsCompressed]            BIT             CONSTRAINT [DF__ZNodeAppl__IsCom__71D1E811] DEFAULT ((0)) NOT NULL,
    [OrderByFields]           NVARCHAR (1000) NULL,
    [ItemNameWithoutCurrency] NVARCHAR (100)  NULL,
    [CreatedByName]           NVARCHAR (100)  NULL,
    [ModifiedByName]          NVARCHAR (100)  NULL,
    [CreatedBy]               INT             NOT NULL,
    [CreatedDate]             DATETIME        NOT NULL,
    [ModifiedBy]              INT             NOT NULL,
    [ModifiedDate]            DATETIME        NOT NULL,
    CONSTRAINT [PK_ZNnodeApplicationSetting] PRIMARY KEY CLUSTERED ([ApplicationSettingId] ASC)
);





