CREATE TABLE [dbo].[ZnodePortalApprovalLevel] (
    [PortalApprovalLevelId] INT           IDENTITY (1, 1) NOT NULL,
    [ApprovalLevelName]     VARCHAR (500) NOT NULL,
    [CreatedBy]             INT           NOT NULL,
    [CreatedDate]           DATETIME      NOT NULL,
    [ModifiedBy]            INT           NOT NULL,
    [ModifiedDate]          DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePortalApprovalLevel] PRIMARY KEY CLUSTERED ([PortalApprovalLevelId] ASC)
);

