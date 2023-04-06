CREATE TABLE [dbo].[ZnodeCMSArea] (
    [CMSAreaId]    INT           IDENTITY (1, 1) NOT NULL,
    [AreaName]     VARCHAR (100) NOT NULL,
    [IsWidgetArea] BIT           CONSTRAINT [DF_ZnodeCMSArea_IsWidgetArea] DEFAULT ((0)) NOT NULL,
    [CreatedBy]    INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   INT           NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCMSArea] PRIMARY KEY CLUSTERED ([CMSAreaId] ASC)
);



