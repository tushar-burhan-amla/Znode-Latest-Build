--date 10/05/2020 ZPD-11845
INSERT INTO ZnodeDomain(PortalId,DomainName,IsActive,ApiKey,ApplicationType,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDefault)
select (select min(PortalId) from ZnodePortal),'localhost:44392',1,'115915F1-7E6B-4386-A623-9779F27D9A5E','Admin',2,GETDATE(),2,GETDATE(),1
where not exists(select * from ZnodeDomain where PortalId = (select min(PortalId) from ZnodePortal) and DomainName = 'localhost:44392' )

INSERT INTO ZnodeDomain(PortalId,DomainName,IsActive,ApiKey,ApplicationType,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDefault)
select (select min(PortalId) from ZnodePortal),'localhost:44375',1,'115915F1-7E6B-4386-A623-9779F27D9A5E','API',2,GETDATE(),2,GETDATE(),1
where not exists(select * from ZnodeDomain where PortalId = (select min(PortalId) from ZnodePortal) and DomainName = 'localhost:44375' )

INSERT INTO ZnodeDomain(PortalId,DomainName,IsActive,ApiKey,ApplicationType,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDefault)
select (select min(PortalId) from ZnodePortal),'localhost:44315',1,'115915F1-7E6B-4386-A623-9779F27D9A5E','WebStore',2,GETDATE(),2,GETDATE(),1
where not exists(select * from ZnodeDomain where PortalId = (select min(PortalId) from ZnodePortal) and DomainName = 'localhost:44315' )