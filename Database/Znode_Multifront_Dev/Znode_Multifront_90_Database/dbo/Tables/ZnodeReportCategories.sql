CREATE TABLE [dbo].[ZnodeReportCategories] (
    [ReportCategoryId] INT           IDENTITY (1, 1) NOT NULL,
    [CategoryName]     VARCHAR (100) NULL,
    [IsActive]         BIT           NOT NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedBy]       INT           NOT NULL,
    [ModifiedDate]     DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeReportCategories] PRIMARY KEY CLUSTERED ([ReportCategoryId] ASC)
);

