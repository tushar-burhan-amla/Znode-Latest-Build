CREATE TABLE [dbo].[ZnodeSearchQueryType] (
    [SearchQueryTypeId]       INT             IDENTITY (1, 1) NOT NULL,
    [ParentSearchQueryTypeId] INT             NULL,
    [QueryTypeName]           NVARCHAR (200)  NOT NULL,
    [QueryBuilderClassName]   NVARCHAR (200)  NULL,
    [HelpDescription]         NVARCHAR (1000) NULL,
    [CreatedBy]               INT             NOT NULL,
    [CreatedDate]             DATETIME        NOT NULL,
    [ModifiedBy]              INT             NOT NULL,
    [ModifiedDate]            DATETIME        NOT NULL,
    [DisplayOrder]            INT             NULL,
    CONSTRAINT [PK_ZnodeSearchQueryType] PRIMARY KEY CLUSTERED ([SearchQueryTypeId] ASC)
);







