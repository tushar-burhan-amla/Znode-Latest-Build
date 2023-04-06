CREATE TYPE [dbo].[SearchProfileAttributeList] AS TABLE (
    [AttributeCode] VARCHAR (300) NULL,
    [IsFacets]      BIT           NULL,
    [IsUseInSearch] BIT           NULL,
    [BoostValue]    INT           NULL,
    [IsNgramEnabled] BIT           Null);

