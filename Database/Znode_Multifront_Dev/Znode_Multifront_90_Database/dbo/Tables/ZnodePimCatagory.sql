CREATE TABLE [dbo].[ZnodePimCatagory] (
    [PimCategoryId] INT      IDENTITY (1, 1) NOT NULL,
    [IsActive]      BIT      CONSTRAINT [DF_ZnodePimCatagory_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]     INT      NOT NULL,
    [CreatedDate]   DATETIME NOT NULL,
    [ModifiedBy]    INT      NOT NULL,
    [ModifiedDate]  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimCatagory] PRIMARY KEY CLUSTERED ([PimCategoryId] ASC)
);

