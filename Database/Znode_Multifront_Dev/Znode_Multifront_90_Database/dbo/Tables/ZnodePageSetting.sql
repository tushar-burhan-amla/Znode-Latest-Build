CREATE TABLE [dbo].[ZnodePageSetting] (
    [PageSettingId] INT           IDENTITY (1, 1) NOT NULL,
    [PageName]      VARCHAR (300) NOT NULL,
    [PageValue]     INT           NOT NULL,
    [DisplayOrder]  INT           NOT NULL,
    [CreatedBy]     INT           NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePageSetting] PRIMARY KEY CLUSTERED ([PageSettingId] ASC)
);

