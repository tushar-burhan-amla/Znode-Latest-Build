CREATE TABLE [dbo].[ZnodeHighlight] (
    [HighlightId]     INT            IDENTITY (1, 1) NOT NULL,
    [MediaId]         INT            NULL,
    [HighlightCode]   VARCHAR (600)  NULL,
    [DisplayPopup]    BIT            CONSTRAINT [DF_SC_Highlight_DisplayPopup] DEFAULT ((1)) NOT NULL,
    [Hyperlink]       NVARCHAR (MAX) NULL,
    [HighlightTypeId] INT            CONSTRAINT [DF_ZnodeHighlight_HighlightTypeId] DEFAULT ((1)) NULL,
    [IsActive]        BIT            NULL,
    [DisplayOrder]    INT            NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_SC_Highlight] PRIMARY KEY CLUSTERED ([HighlightId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeHighlight_ZnodeHighlightType] FOREIGN KEY ([HighlightTypeId]) REFERENCES [dbo].[ZnodeHighlightType] ([HighlightTypeId])
);



