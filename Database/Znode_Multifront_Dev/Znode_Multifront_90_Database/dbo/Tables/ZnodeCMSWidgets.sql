CREATE TABLE [dbo].[ZnodeCMSWidgets] (
    [CMSWidgetsId]   INT           IDENTITY (1, 1) NOT NULL,
    [Code]           NVARCHAR (50) NOT NULL,
    [DisplayName]    VARCHAR (100) NULL,
    [IsConfigurable] BIT           CONSTRAINT [DF_ZnodeCMSWidgets_IsConfigurable] DEFAULT ((0)) NOT NULL,
    [FileName]       VARCHAR (300) NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCMSWidgets] PRIMARY KEY CLUSTERED ([CMSWidgetsId] ASC)
);







