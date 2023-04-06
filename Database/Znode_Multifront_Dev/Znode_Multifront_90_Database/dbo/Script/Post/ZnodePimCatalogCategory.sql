
if exists(select * from sys.procedures where name = 'Znode_DeleteProfileCatalogCategory')
	drop proc Znode_DeleteProfileCatalogCategory
go
if exists(select * from sys.procedures where name = 'Znode_InsertUpdateProfileCatalog')
	drop proc Znode_InsertUpdateProfileCatalog
go
if exists(select * from sys.procedures where name = 'Znode_DeletePimCatalogProducts')
	drop proc Znode_DeletePimCatalogProducts
go
if exists(select * from sys.procedures where name = 'Znode_DeletePimCatalogProducts')
	drop proc Znode_DeletePimCatalogProducts
go

if exists(select * from sys.tables where name = 'ZnodeProfileCatalogCategory')
	drop table ZnodeProfileCatalogCategory 
go
if exists(select * from sys.tables where name = 'ZnodePimCatalogCategory')
	drop table ZnodePimCatalogCategory 
go
if exists(select * from sys.tables where name = 'ZnodeProfileCategoryHierarchy')
	drop table ZnodeProfileCategoryHierarchy 
go
if exists(select * from sys.tables where name = 'ZnodeProfileCatalog')
	drop table ZnodeProfileCatalog 
go

update  ZnodePimCategoryProduct set DisplayOrder = 999 where DisplayOrder is null