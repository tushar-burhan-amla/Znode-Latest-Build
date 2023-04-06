CREATE TABLE [dbo].[ZnodeFormBuilderSubmit] (
    [FormBuilderSubmitId] INT      IDENTITY (1, 1) NOT NULL,
    [FormBuilderId]       INT      NULL,
    [LocaleId]            INT      NOT NULL,
    [PortalId]            INT      NULL,
    [UserId]              INT      NULL,
    [CreatedBy]           INT      NOT NULL,
    [CreatedDate]         DATETIME NOT NULL,
    [ModifiedBy]          INT      NOT NULL,
    [ModifiedDate]        DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeFormBuilderSubmit] PRIMARY KEY CLUSTERED ([FormBuilderSubmitId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeFormBuilderSubmit_ZnodeFormBuilder] FOREIGN KEY ([FormBuilderId]) REFERENCES [dbo].[ZnodeFormBuilder] ([FormBuilderId]),
    CONSTRAINT [FK_ZnodeFormBuilderSubmit_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodeFormBuilderSubmit_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);



