CREATE TABLE [dbo].[ZnodeTimeZone] (
    [TimeZoneId]          INT            IDENTITY (1, 1) NOT NULL,
    [TimeZoneDetailsCode] NVARCHAR (50)  NOT NULL,
    [TimeZoneDetailsDesc] NVARCHAR (200) NULL,
    [DifferenceInSeconds] INT            NOT NULL,
    [DaylightBeginsAt]    VARCHAR (100)  NULL,
    [DaylightEndsAt]      VARCHAR (100)  NULL,
    [DSTInSeconds]        INT            NULL,
    [IsDefault]           BIT            CONSTRAINT [DF_ZnodeTimeZone_IsDefault] DEFAULT ((0)) NOT NULL,
    [IsActive]            BIT            CONSTRAINT [DF_ZnodeTimeZone_IsActive] DEFAULT ((1)) NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    CONSTRAINT [PK_TimeZoneDetails] PRIMARY KEY CLUSTERED ([TimeZoneId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [UK_TimeZoneDetails] UNIQUE NONCLUSTERED ([TimeZoneDetailsCode] ASC) WITH (FILLFACTOR = 90)
);












GO

Create Trigger [dbo].[Trg_ZnodeTimeZone_GlobalSetting]  ON [dbo].[ZnodeTimeZone]
AFTER UPDATE AS 
BEGIN
 IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'TimeZone')
 BEGIN
  INSERT INTO ZnodeGlobalSetting (FeatureName
,FeatureValues
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)
SELECT 'TimeZone',[TimeZoneDetailsCode],1,GetDate(),1,Getdate() FROM [dbo].ZnodeTimeZone WHERE IsActive = 1 AND IsDefault = 1
 END 
 ELSE 
 BEGIN 
 UPDATE ZnodeGlobalSetting
 SET
FeatureValues = (SELECT [TimeZoneDetailsCode] FROM ZnodeTimeZone WHERE IsActive = 1 AND IsDefault = 1)
,CreatedBy  = (SELECT createdBy FROM Inserted ) 
,CreatedDate = (SELECT CreatedDate FROM Inserted ) 
,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
 WHERE FeatureName = 'TimeZone'
 
 END 
END