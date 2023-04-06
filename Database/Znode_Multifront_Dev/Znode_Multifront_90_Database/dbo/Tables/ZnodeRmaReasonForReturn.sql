CREATE TABLE [dbo].[ZnodeRmaReasonForReturn] (
    [RmaReasonForReturnId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                 NVARCHAR (500) NOT NULL,
    [IsActive]             BIT            CONSTRAINT [DF_ZnodeRmaReasonForReturn_IsActive] DEFAULT ((0)) NOT NULL,
    [IsDefault]            BIT            NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeRmaReasonForReturn] PRIMARY KEY CLUSTERED ([RmaReasonForReturnId] ASC) WITH (FILLFACTOR = 90)
);



