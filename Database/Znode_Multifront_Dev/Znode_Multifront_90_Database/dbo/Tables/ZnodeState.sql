CREATE TABLE [dbo].[ZnodeState] (
    [StateId]      INT            IDENTITY (1, 1) NOT NULL,
    [CountryCode]  VARCHAR (100)  NULL,
    [StateCode]    NVARCHAR (255) NULL,
    [StateName]    VARCHAR (200)  NULL,
    [IsActive]     BIT            DEFAULT ((0)) NOT NULL,
    [IsDefault]    BIT            DEFAULT ((0)) NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeState] PRIMARY KEY CLUSTERED ([StateId] ASC)
);




GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170331-202535]
    ON [dbo].[ZnodeState]([CountryCode] ASC);

