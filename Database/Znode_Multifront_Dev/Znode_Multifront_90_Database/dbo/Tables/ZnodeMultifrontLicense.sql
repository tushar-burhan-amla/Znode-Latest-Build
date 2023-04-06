CREATE TABLE [dbo].[ZnodeMultifrontLicense] (
    [MultifrontLicenseId] INT            IDENTITY (1, 1) NOT NULL,
    [LicenseType]         VARCHAR (100)  NOT NULL,
    [SerialKey]           NVARCHAR (200) NULL,
    [InstallationDate]    DATETIME       NULL,
    [ExpirationDate]      DATETIME       NULL,
    CONSTRAINT [PK_ZnodeMultifrontLicense] PRIMARY KEY CLUSTERED ([MultifrontLicenseId] ASC)
);

