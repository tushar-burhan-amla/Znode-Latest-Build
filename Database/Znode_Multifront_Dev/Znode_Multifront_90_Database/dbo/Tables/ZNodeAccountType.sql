CREATE TABLE [dbo].[ZnodeAccountType] (
    [AccountTypeID]   INT            NOT NULL,
    [AccountTypeName] NVARCHAR (MAX) NOT NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED ([AccountTypeID] ASC) WITH (FILLFACTOR = 90)
);

