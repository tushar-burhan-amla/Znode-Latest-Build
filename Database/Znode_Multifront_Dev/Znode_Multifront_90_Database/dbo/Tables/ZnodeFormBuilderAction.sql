CREATE TABLE [dbo].[ZnodeFormBuilderAction] (
    [FormBuilderActionId] INT           IDENTITY (1, 1) NOT NULL,
    [FormBuilderId]       INT           NULL,
    [ButtonText]          VARCHAR (100) NULL,
    [IsTextMessage]       BIT           DEFAULT ((0)) NULL,
    [TextMessage]         VARCHAR (500) NULL,
    [RedirectURL]         VARCHAR (500) NULL,
    [DisplayOrder]        INT           NULL,
    [CreatedBy]           INT           NOT NULL,
    [CreatedDate]         DATETIME      NOT NULL,
    [ModifiedBy]          INT           NOT NULL,
    [ModifiedDate]        DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeFormBuilderAction] PRIMARY KEY CLUSTERED ([FormBuilderActionId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeFormBuilderAction_ZnodeFormBuilder] FOREIGN KEY ([FormBuilderId]) REFERENCES [dbo].[ZnodeFormBuilder] ([FormBuilderId])
);

