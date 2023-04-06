CREATE TABLE [dbo].[ZnodeSavedReportViews] (
    [ReportViewId] INT            IDENTITY (1, 1) NOT NULL,
    [UserId]       INT            NOT NULL,
    [ReportCode]   VARCHAR (50)   NOT NULL,
    [ReportName]   VARCHAR (100)  NOT NULL,
    [LayoutXml]    NVARCHAR (MAX) NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    [IsActive]     BIT            NULL,
    CONSTRAINT [PK_ZnodeSavedReportViews] PRIMARY KEY CLUSTERED ([ReportViewId] ASC)
);

