CREATE TABLE [dbo].[_productOtherInfo] (
    [LocaleId]             INT            NOT NULL,
    [AttributeValue]       NVARCHAR (MAX) NULL,
    [PimAttributeId]       INT            NULL,
    [PimAttributeFamilyId] NCHAR (10)     NULL,
    [PimAttributeValueId]  INT            NOT NULL
);

