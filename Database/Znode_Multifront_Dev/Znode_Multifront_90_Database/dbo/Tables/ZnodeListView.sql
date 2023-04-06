CREATE TABLE [dbo].[ZnodeListView] (
    [ListViewId]           INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationSettingId] INT            NULL,
    [ViewName]             NVARCHAR (600) NULL,
    [XmlSetting]           NVARCHAR (MAX) NULL,
    [IsSelected]           BIT            NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    [SortColumn]           NVARCHAR (100) NULL,
    [SortType]             NVARCHAR (100) NULL,
    [IsPublic]             BIT            CONSTRAINT [DF_ZnodeListView_IsPublic] DEFAULT ((0)) NOT NULL,
    [IsDefault]            BIT            CONSTRAINT [DF_ZnodeListView_IsDefault] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZnodeListView] PRIMARY KEY CLUSTERED ([ListViewId] ASC),
    CONSTRAINT [FK_ZnodeListView_ZnodeApplicationSetting] FOREIGN KEY ([ApplicationSettingId]) REFERENCES [dbo].[ZnodeApplicationSetting] ([ApplicationSettingId])
);





