CREATE TABLE [dbo].[_productInfo] (
    [LocaleId]             INT            NOT NULL,
    [AttributeValue]       NVARCHAR (MAX) NULL,
    [PimAttributeId]       INT            NULL,
    [PimAttributeFamilyId] NCHAR (10)     NULL,
    [PimAttributeValueId]  INT            NOT NULL
);

