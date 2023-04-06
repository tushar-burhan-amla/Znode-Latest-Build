
CREATE PROCEDURE [dbo].[Znode_AssociatePortalBrand]   
(  
	 @PortalId INT = 0,  
	 @BrandId  VARCHAR(MAX) = '',  
	 @IsAssociated BIT, -----0 = UnAssociate, 1 = Associate  
	 @UserId INT,  
	 @Status BIT OUT  
)  
AS  
BEGIN  
 SET NOCOUNT ON  
  
 BEGIN TRY  
 BEGIN TRAN
	 DECLARE @GetDate DATETIME= dbo.Fn_GetDate();  
	 DECLARE @DisplayOrder INT = 999; 
  
	 IF ( @IsAssociated = 1 )  
	 BEGIN  
		  INSERT INTO ZnodePortalBrand ( PortalId, BrandId, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )   
		  SELECT @PortalId, P.Item ,@DisplayOrder, @UserId, @GetDate, @UserId, @GetDate  
		  FROM dbo.Split ( @BrandId , ',' ) P  
		  WHERE P.Item NOT IN ( SELECT PortalBrandId FROM ZnodePortalBrand BP WHERE  BP.BrandId=P.Item AND BP.PortalId = @PortalId ) 
		   and P.Item <> '' 
	 END  
	 ELSE IF ( @IsAssociated = 0 )  
	 BEGIN    
		DELETE FROM ZnodePortalBrand  
		WHERE EXISTS( SELECT * FROM dbo.Split ( @BrandId , ',' ) P WHERE ZnodePortalBrand.BrandId = P.Item AND ZnodePortalBrand.PortalId = @PortalId )   
		DELETE FROM ZnodeCMSWidgetBrand 
		WHERE EXISTS( SELECT * FROM dbo.Split ( @BrandId , ',' ) P WHERE ZnodeCMSWidgetBrand.BrandId = P.Item AND ZnodeCMSWidgetBrand.CMSMappingId = @PortalId AND ZnodeCMSWidgetBrand. TypeOFMapping = 'PortalMapping') 
	 END  
  
	 SELECT 1 AS ID, CAST(1 AS bit) AS Status;  
 COMMIT TRAN
END TRY  
BEGIN CATCH    
	   ROLLBACK TRAN
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_AssociatePortalBrand @PortalId = '+CAST(@PortalId AS VARCHAR(max))+',@BrandId='+CAST(@BrandId AS VARCHAR(50))+',@IsUnAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@UserId='+CAST( @UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
		  SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
		  EXEC Znode_InsertProcedureErrorLog  
		  @ProcedureName = 'Znode_AssociatePortalBrand',  
		  @ErrorInProcedure = @Error_procedure,  
		  @ErrorMessage = @ErrorMessage,  
		  @ErrorLine = @ErrorLine,  
		  @ErrorCall = @ErrorCall;  
END CATCH  
  
END  