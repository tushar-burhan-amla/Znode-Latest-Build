CREATE TABLE [dbo].[ZNodeZipCode] (
    [ZipCodeID]  INT            IDENTITY (1, 1) NOT NULL,
    [ZIP]        NVARCHAR (50)  NULL,
    [ZIPType]    NVARCHAR (50)  NULL,
    [CityName]   NVARCHAR (255) NULL,
    [CityType]   NVARCHAR (50)  NULL,
    [StateName]  NVARCHAR (255) NULL,
    [StateAbbr]  NVARCHAR (50)  NULL,
    [AreaCode]   NVARCHAR (50)  NULL,
    [Latitude]   DECIMAL (9, 6) NULL,
    [Longitude]  DECIMAL (9, 6) NULL,
    [CountyName] VARCHAR (255)  NULL,
    [CountyFIPS] VARCHAR (50)   NULL,
    [StateFIPS]  VARCHAR (50)   NULL,
    [MSACode]    VARCHAR (50)   NULL,
    [TimeZone]   VARCHAR (50)   NULL,
    [UTC]        DECIMAL (3, 1) NULL,
    [DST]        CHAR (1)       NULL,
    CONSTRAINT [PK_ZNodeZipCode] PRIMARY KEY CLUSTERED ([ZipCodeID] ASC)
);

