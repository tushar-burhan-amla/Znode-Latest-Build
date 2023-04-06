CREATE TABLE [dbo].[ZnodeMongoIndex] (
    [MongoIndexId]     INT           IDENTITY (1, 1) NOT NULL,
    [CollectionName]   VARCHAR (50)  NOT NULL,
    [DelimitedColumns] VARCHAR (MAX) NOT NULL,
    [IsAscending]      BIT           NULL,
    [IsCompoundIndex]  BIT           NULL,
    [EntityType]       VARCHAR (50)  NULL,
    [Createdby]        INT           NULL,
    [CreatedDate]      DATETIME      NULL,
    [ModifiedBy]       INT           NULL,
    [ModifiedDate]     DATETIME      NULL,
    CONSTRAINT [PK_ZnodeMongoIndex] PRIMARY KEY CLUSTERED ([MongoIndexId] ASC)
);

