CREATE TABLE [dbo].[ZnodeCity] (
    [CityID]       INT            IDENTITY (1, 1) NOT NULL,
    [CityName]     NVARCHAR (255) NULL,
    [CityType]     NVARCHAR (50)  NULL,
    [ZIP]          NVARCHAR (50)  NULL,
    [ZIPType]      NVARCHAR (50)  NULL,
    [CountyCode]   NVARCHAR (255) NULL,
    [CountryCode]  VARCHAR (255)  NULL,
    [StateCode]    NVARCHAR (255) NULL,
    [Latitude]     DECIMAL (9, 6) NULL,
    [Longitude]    DECIMAL (9, 6) NULL,
    [CountyFIPS]   NVARCHAR (50)  NULL,
    [StateFIPS]    NVARCHAR (50)  NULL,
    [MSACode]      NVARCHAR (50)  NULL,
    [TimeZone]     NVARCHAR (50)  NULL,
    [UTC]          DECIMAL (3, 1) NULL,
    [DST]          CHAR (1)       NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeCity] PRIMARY KEY CLUSTERED ([CityID] ASC)
);








GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170331-202041]
    ON [dbo].[ZnodeCity]([CountryCode] ASC, [StateCode] ASC, [CityName] ASC, [ZIP] ASC);

