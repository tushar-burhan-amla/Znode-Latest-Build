CREATE TABLE [dbo].[ZnodeCaseRequest] (
    [CaseRequestId]  INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]       INT            NOT NULL,
    [UserId]         INT            NULL,
    [OwnerUserId]    INT            NULL,
    [CaseStatusId]   INT            NULL,
    [CasePriorityId] INT            NULL,
    [CaseTypeId]     INT            NULL,
    [CaseOrigin]     VARCHAR (50)   NULL,
    [Title]          VARCHAR (255)  NOT NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [FirstName]      VARCHAR (100)  NULL,
    [LastName]       VARCHAR (100)  NULL,
    [CompanyName]    VARCHAR (100)  NULL,
    [EmailId]        VARCHAR (200)  NULL,
    [PhoneNumber]    VARCHAR (20)   NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    [Custom1]  NVARCHAR (MAX)  NULL,
    [Custom2]  NVARCHAR (MAX)  NULL,
    [Custom3]  NVARCHAR (MAX)  NULL,
    [Custom4]  NVARCHAR (MAX)  NULL,
    [Custom5]  NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_CaseRequest] PRIMARY KEY CLUSTERED ([CaseRequestId] ASC),
    CONSTRAINT [FK_ZnodeCaseRequest_ZnnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeCaseRequest_ZnnodeUser_OwnerUserId] FOREIGN KEY ([OwnerUserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeCaseRequest_ZnodeCasePriority] FOREIGN KEY ([CasePriorityId]) REFERENCES [dbo].[ZnodeCasePriority] ([CasePriorityId]),
    CONSTRAINT [FK_ZnodeCaseRequest_ZnodeCaseStatus] FOREIGN KEY ([CaseStatusId]) REFERENCES [dbo].[ZnodeCaseStatus] ([CaseStatusId]),
    CONSTRAINT [FK_ZnodeCaseRequest_ZnodeCaseType] FOREIGN KEY ([CaseTypeId]) REFERENCES [dbo].[ZnodeCaseType] ([CaseTypeId]),
    CONSTRAINT [FK_ZnodeCaseRequest_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);





