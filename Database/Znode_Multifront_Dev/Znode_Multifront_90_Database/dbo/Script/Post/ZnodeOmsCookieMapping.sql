IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsCookieMapping_UserId_PortalId' AND object_id = OBJECT_ID('ZnodeOmsCookieMapping'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ZnodeOmsCookieMapping_UserId_PortalId
			ON [dbo].ZnodeOmsCookieMapping (UserId,PortalId)
    END
	 