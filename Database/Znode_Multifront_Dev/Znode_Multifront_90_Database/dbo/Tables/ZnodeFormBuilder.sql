CREATE TABLE [dbo].[ZnodeFormBuilder] (
    [FormBuilderId]   INT           IDENTITY (1, 1) NOT NULL,
    [FormCode]        VARCHAR (200) NULL,
    [FormDescription] VARCHAR (200) NULL,
    [CreatedBy]       INT           NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [ModifiedBy]      INT           NOT NULL,
    [ModifiedDate]    DATETIME      NOT NULL,
    [IsShowCaptcha]   BIT           DEFAULT ((0)) NOT NULL,
    [Custom1] NVARCHAR (MAX)  NULL,
    [Custom2] NVARCHAR (MAX)  NULL,
    [Custom3] NVARCHAR (MAX)  NULL,
    [Custom4] NVARCHAR (MAX)  NULL,
    [Custom5] NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZnodeFormBuilder] PRIMARY KEY CLUSTERED ([FormBuilderId] ASC),
    CONSTRAINT [UC_ZnodeFormBuilder] UNIQUE NONCLUSTERED ([FormCode] ASC)
);









