CREATE TABLE [dbo].[ZnodeProfile] (
    [ProfileId]                INT             IDENTITY (1, 1) NOT NULL,
    [ProfileName]              NVARCHAR (100)  NOT NULL,
    [ShowOnPartnerSignup]      BIT             CONSTRAINT [DF_ZNodeProfile_ShowOnPartnerSignup] DEFAULT ((0)) NOT NULL,
    [Weighting]                DECIMAL (16, 2) NULL,
    [TaxExempt]                BIT             CONSTRAINT [DF_ZNodeProfile_TaxExempt] DEFAULT ((1)) NOT NULL,
    [DefaultExternalAccountNo] VARCHAR (MAX)   NULL,
    [CreatedBy]                INT             NOT NULL,
    [CreatedDate]              DATETIME        NOT NULL,
    [ModifiedBy]               INT             NOT NULL,
    [ModifiedDate]             DATETIME        NOT NULL,
    [ParentProfileId]          INT             NULL,
	[PimCatalogId]          INT             NULL,
    CONSTRAINT [PK_SC_Profile] PRIMARY KEY CLUSTERED ([ProfileId] ASC),
	CONSTRAINT [FK_ZnodeProfile_ZnodePImCatalogId] FOREIGN KEY([PimCatalogId])REFERENCES [dbo].[ZnodePimCatalog] ([PimCatalogId])
);







