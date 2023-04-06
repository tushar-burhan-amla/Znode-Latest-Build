CREATE TABLE [dbo].[ZnodeDepartment] (
    [DepartmentId]   INT           IDENTITY (1, 1) NOT NULL,
    [DepartmentName] VARCHAR (300) NULL,
    [AccountId]      INT           NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentId] ASC),
    CONSTRAINT [FK_ZnodeDepartment_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId])
);



