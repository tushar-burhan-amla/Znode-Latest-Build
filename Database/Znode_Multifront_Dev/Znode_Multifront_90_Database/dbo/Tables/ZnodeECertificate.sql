CREATE TABLE [dbo].[ZnodeECertificate] (
    [ECertificateId]  INT             IDENTITY (1, 1) NOT NULL,
    [CertificateKey]  NVARCHAR (50)   NULL,
    [CertificateType] NVARCHAR (10)   NULL,
    [IssuedDate]      DATETIME        NULL,
    [IssuedAmount]    NUMERIC (28, 6) NOT NULL,
    [Custom1]         NVARCHAR (MAX)  NULL,
    [Custom2]         NVARCHAR (MAX)  NULL,
    [Custom3]         NVARCHAR (MAX)  NULL,
    [Custom4]         NVARCHAR (MAX)  NULL,
    [CreatedBy]       INT             NOT NULL,
    [CreatedDate]     DATETIME        NOT NULL,
    [ModifiedBy]      INT             NOT NULL,
    [ModifiedDate]    DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeECertificate] PRIMARY KEY CLUSTERED ([ECertificateId] ASC)
);

