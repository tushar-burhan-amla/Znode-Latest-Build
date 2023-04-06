CREATE TABLE [dbo].[ZnodeActions] (
    [ActionId]       INT           IDENTITY (1, 1) NOT NULL,
    [AreaName]       VARCHAR (100) NULL,
    [ControllerName] VARCHAR (100) NOT NULL,
    [ActionName]     VARCHAR (100) NOT NULL,
    [IsGlobalAccess] BIT           CONSTRAINT [DF_ZnodeActions_IsGlobalAccess] DEFAULT ((0)) NOT NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeActions] PRIMARY KEY CLUSTERED ([ActionId] ASC) WITH (FILLFACTOR = 90)
);



