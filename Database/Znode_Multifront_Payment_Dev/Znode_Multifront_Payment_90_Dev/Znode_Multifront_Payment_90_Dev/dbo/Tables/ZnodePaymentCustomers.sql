CREATE TABLE [dbo].[ZnodePaymentCustomers] (
    [CustomersGUID] UNIQUEIDENTIFIER CONSTRAINT [DF_ZnodePaymentCustomers_CustomersGUID] DEFAULT (newid()) NOT NULL,
    [FisrtName]     VARCHAR (100)    NULL,
    [LastName]      VARCHAR (100)    NULL,
    [CreatedDate]   DATETIME         NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    CONSTRAINT [PK_ZnodePaymentCustomers] PRIMARY KEY CLUSTERED ([CustomersGUID] ASC)
);



