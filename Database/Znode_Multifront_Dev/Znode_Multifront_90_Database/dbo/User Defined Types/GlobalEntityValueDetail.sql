CREATE TYPE [dbo].[GlobalEntityValueDetail] AS TABLE (
    [GlobalAttributeId]             INT            NULL,
    [AttributeCode]                 VARCHAR (300)  NULL,
    [GlobalAttributeDefaultValueId] INT            NULL,
    [GlobalAttributeValueId]        INT            NULL,
    [LocaleId]                      INT            NULL,
    [GlobalEntityValueId]           INT            NULL,
    [AttributeValue]                NVARCHAR (MAX) NULL);

