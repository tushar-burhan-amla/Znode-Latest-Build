CREATE TABLE [dbo].[ZNodePaymentType] (
    [PaymentTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (50)  NOT NULL,
    [Description]   TEXT          NULL,
    [IsActive]      BIT           CONSTRAINT [DF_SC_PaymentType_ActiveInd] DEFAULT ((1)) NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    [BehaviorType] VARCHAR (100) NULL,
    [Code]          VARCHAR (100) NULL,
    CONSTRAINT [PK_SC_PaymentType] PRIMARY KEY CLUSTERED ([PaymentTypeId] ASC)
);



