CREATE TABLE [dbo].[ZnodePortalApprovalType] (
    [PortalApprovalTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [ApprovalTypeName]     NVARCHAR (500) NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATE           NOT NULL,
    CONSTRAINT [PK_ZnodePortalApprovalType] PRIMARY KEY CLUSTERED ([PortalApprovalTypeId] ASC)
);

