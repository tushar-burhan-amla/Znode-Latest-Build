CREATE TABLE [dbo].[ZnodeProductReviewState] (
    [ProductReviewStateId] INT            NOT NULL,
    [ReviewStateName]      NVARCHAR (50)  NOT NULL,
    [Description]          NVARCHAR (250) NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeProductReviewState] PRIMARY KEY CLUSTERED ([ProductReviewStateId] ASC)
);



