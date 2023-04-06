--ZPD-20528 Dt.21-June-2022
INSERT INTO dbo.ZnodeMultifront
	(VersionName, Descriptions, MajorVersion, MinorVersion, LowerVersion, BuildVersion, PatchIndex, CreatedBy, 
		CreatedDate, ModifiedBy, ModifiedDate) 
SELECT N'Znode_Multifront_9_7_3_GA', N'Upgrade GA Release by 973',9,7,3,973,0,2, GETDATE(),2, GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeMultifront WHERE VersionName='Znode_Multifront_9_7_3_GA' 
	AND MajorVersion=9 AND MinorVersion=7 AND LowerVersion=3 AND BuildVersion=973);

--Dt.16-August-2022
INSERT INTO dbo.ZnodeMultifront
	(VersionName, Descriptions, MajorVersion, MinorVersion, LowerVersion, BuildVersion, PatchIndex, CreatedBy, 
		CreatedDate, ModifiedBy, ModifiedDate) 
SELECT N'Znode_Multifront_9_7_4_GA', N'Upgrade GA Release by 974',9,7,4,974,0,2, GETDATE(),2, GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeMultifront WHERE VersionName='Znode_Multifront_9_7_4_GA' 
	AND MajorVersion=9 AND MinorVersion=7 AND LowerVersion=4 AND BuildVersion=974);
