CREATE TABLE [dbo].[ZnodeAddress] (
    [AddressId]             INT             IDENTITY (1, 1) NOT NULL,
    [FirstName]             VARCHAR (300)   NULL,
    [LastName]              VARCHAR (300)   NULL,
    [DisplayName]           NVARCHAR (600)  NULL,
    [CompanyName]           NVARCHAR (600)  NULL,
    [Address1]              VARCHAR (300)   NULL,
    [Address2]              VARCHAR (300)   NULL,
    [Address3]              VARCHAR (300)   NULL,
    [CountryName]           VARCHAR (3000)  NULL,
    [StateName]             VARCHAR (3000)  NULL,
    [CityName]              VARCHAR (3000)  NOT NULL,
    [PostalCode]            VARCHAR (50)    NOT NULL,
    [PhoneNumber]           VARCHAR (50)    NULL,
    [Mobilenumber]          VARCHAR (50)    NULL,
    [AlternateMobileNumber] VARCHAR (50)    NULL,
    [FaxNumber]             VARCHAR (30)    NULL,
    [IsDefaultBilling]      BIT             NOT NULL,
    [IsDefaultShipping]     BIT             NOT NULL,
    [IsActive]              BIT             NOT NULL,
    [ExternalId]            NVARCHAR (1000) NULL,
    [CreatedBy]             INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL,
    [ModifiedBy]            INT             NOT NULL,
    [ModifiedDate]          DATETIME        NOT NULL,
    [IsShipping]            BIT             CONSTRAINT [DF_ZnodeAddress_IsShipping] DEFAULT ((0)) NOT NULL,
    [IsBilling]             BIT             CONSTRAINT [DF_ZnodeAddress_IsBilling] DEFAULT ((0)) NOT NULL,
    [EmailAddress]          VARCHAR (50)    NULL,
	[Custom1]                 NVARCHAR (MAX)  NULL,
    [Custom2]                 NVARCHAR (MAX)  NULL,
    [Custom3]                 NVARCHAR (MAX)  NULL,
    [Custom4]                 NVARCHAR (MAX)  NULL,
    [Custom5]                 NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZNodeAddress] PRIMARY KEY CLUSTERED ([AddressId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_ZnodeAddress_IsDefaultShipping]
    ON [dbo].[ZnodeAddress]([IsDefaultShipping] ASC)
    INCLUDE([PostalCode]);


GO
CREATE NONCLUSTERED INDEX [IDX_ZnodeAddress_IsDefaultBilling]
    ON [dbo].[ZnodeAddress]([IsDefaultBilling] ASC)
    INCLUDE([PostalCode]);
Go
CREATE NONCLUSTERED INDEX Ind_ZnodeAddress_IsDefaultShipping_PostalCode
ON [dbo].[ZnodeAddress] ([IsDefaultShipping],[PostalCode])
Go
CREATE NONCLUSTERED INDEX Ind_ZnodeAddress_IsDefaultBilling_PostalCode
ON [dbo].[ZnodeAddress] ([IsDefaultBilling],[PostalCode])