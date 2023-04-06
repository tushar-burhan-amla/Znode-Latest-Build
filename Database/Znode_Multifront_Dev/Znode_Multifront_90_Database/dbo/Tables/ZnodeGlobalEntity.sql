CREATE TABLE [dbo].[ZnodeGlobalEntity] (
    [GlobalEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [EntityName]     NVARCHAR (300) NULL,
    [IsActive]       BIT            NOT NULL,
    [TableName]      NVARCHAR (50)  NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
	[IsFamilyUnique] BIT		    NULL,
    CONSTRAINT [PK_ZnodeGlobalEntity] PRIMARY KEY CLUSTERED ([GlobalEntityId] ASC)
);



