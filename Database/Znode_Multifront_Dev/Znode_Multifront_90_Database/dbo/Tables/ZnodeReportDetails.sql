CREATE TABLE [dbo].[ZnodeReportDetails] (
    [ReportDetailId]   INT            IDENTITY (1, 1) NOT NULL,
    [ReportCategoryId] INT            NOT NULL,
    [ReportCode]       VARCHAR (50)   NOT NULL,
    [ReportName]       VARCHAR (100)  NOT NULL,
    [Description]      VARCHAR (1000) NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    [IsActive]         BIT            NULL,
    CONSTRAINT [PK_ZnodeReportDetails] PRIMARY KEY CLUSTERED ([ReportDetailId] ASC),
    CONSTRAINT [FK_ZnodeReportDetails_ZnodeReportCategories] FOREIGN KEY ([ReportCategoryId]) REFERENCES [dbo].[ZnodeReportCategories] ([ReportCategoryId])
);

