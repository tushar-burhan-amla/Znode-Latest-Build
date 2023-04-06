CREATE TABLE [dbo].[ZnodePromotionType] (
    [PromotionTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [ClassType]       NVARCHAR (50)  NULL,
    [ClassName]       NVARCHAR (50)  NULL,
    [Name]            VARCHAR (MAX)  NULL,
    [Description]     NVARCHAR (MAX) NULL,
    [IsActive]        BIT            CONSTRAINT [DF_ZnodePromotionType_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePromotionType] PRIMARY KEY CLUSTERED ([PromotionTypeId] ASC)
);

