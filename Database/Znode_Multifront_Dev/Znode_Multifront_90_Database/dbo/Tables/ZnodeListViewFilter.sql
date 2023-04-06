CREATE TABLE [dbo].[ZnodeListViewFilter] (
    [ListViewFilterId] INT            IDENTITY (1, 1) NOT NULL,
    [ListViewId]       INT            NULL,
    [FilterName]       NVARCHAR (MAX) NULL,
    [Operator]         VARCHAR (100)  NULL,
    [Value]            NVARCHAR (MAX) NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeListViewFilters] PRIMARY KEY CLUSTERED ([ListViewFilterId] ASC),
    CONSTRAINT [FK_ZnodeListViewFilter_ZnodeListView] FOREIGN KEY ([ListViewId]) REFERENCES [dbo].[ZnodeListView] ([ListViewId])
);

