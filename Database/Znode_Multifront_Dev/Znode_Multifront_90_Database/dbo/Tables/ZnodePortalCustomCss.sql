CREATE TABLE [dbo].[ZnodePortalCustomCss] (
    [PortalCustomCssId]  INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]           INT            NOT NULL,
    [DynamicCssStyle]    NVARCHAR (MAX) NULL,
    [WYSIWYGFormatStyle] NVARCHAR (MAX) NULL,
    [IsActive]           BIT            CONSTRAINT [DF_ZnodePortalCustomCss_IsActive] DEFAULT ((1)) NOT NULL,
    [PublishStateId]     TINYINT        CONSTRAINT [DF_ZnodePortalCustomCss_PublishStateId] DEFAULT ((1)) NOT NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePortalCustomCss] PRIMARY KEY CLUSTERED ([PortalCustomCssId] ASC),
    CONSTRAINT [FK_ZnodePortalCustomCss_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

