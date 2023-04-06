CREATE TABLE [dbo].[ZnodePortalCountry] (
    [PortalCountryId] INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]        INT           NOT NULL,
    [CountryCode]     NVARCHAR (20) NOT NULL,
    [IsDefault]       BIT           NOT NULL,
    [CreatedBy]       INT           NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [ModifiedBy]      INT           NOT NULL,
    [ModifiedDate]    DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePortalCountry] PRIMARY KEY CLUSTERED ([PortalCountryId] ASC)
);





