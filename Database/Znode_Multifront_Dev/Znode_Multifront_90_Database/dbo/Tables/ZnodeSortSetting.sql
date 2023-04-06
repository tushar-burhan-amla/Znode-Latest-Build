CREATE TABLE [dbo].[ZnodeSortSetting] (
    [SortSettingId] INT           IDENTITY (1, 1) NOT NULL,
    [SortName]      VARCHAR (300) NOT NULL,
    [SortValue]     INT           NOT NULL,
    [DisplayOrder]  INT           NOT NULL,
    [CreatedBy]     INT           NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeSortSetting] PRIMARY KEY CLUSTERED ([SortSettingId] ASC)
);

