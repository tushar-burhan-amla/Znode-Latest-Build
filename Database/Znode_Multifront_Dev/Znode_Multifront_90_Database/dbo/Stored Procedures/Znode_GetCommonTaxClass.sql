CREATE PROCEDURE [dbo].[Znode_GetCommonTaxClass]
(
   @PortalId       INT =  0 
  ,@ProfileId      INT =0 
  ,@TaxClassIds VARCHAR(2000) = ''  OUT
  ,@UserId          INT = 0 
 )
 AS 
 /*
   Summary :- This  procedure is used to get the tax class on the basis of profiel and portal 
   Unit Testing 
   DECLARE @grtrtr VARCHAR(2000)
   EXEC Znode_GetCommonTaxClass 1,0,@grtrtr OUT , 8  SELECT @grtrtr

 */
 BEGIN 
  BEGIN TRY 
   SET NOCOUNT ON 

	  DECLARE @TBL_Profileids TABLE(ProfileId INT )
	  IF @UserId = -1 
	  BEGIN 
	   INSERT INTO @TBL_Profileids (ProfileId)
	   SELECT ProfileId 
	   FROM ZnodePortalProfile 
	   WHERE IsDefaultAnonymousProfile = 1 
       AND PortalId = @PortalId
    
	  END 
	  ELSE 
	  BEGIN 

	   INSERT INTO @TBL_Profileids (ProfileId)
	   SELECT ProfileId 
	   FROM ZnodePortalProfile  ZPP 
	   WHERE 
	  -- IsDefaultRegistedProfile = 1	    
	   PortalId = @PortalId
	   AND EXISTS (SELECT TOP 1 1 FROM ZnodeUserProfile ZUP  WHERE  ZUP.UserId = @UserId AND  ZUP.ProfileId = ZPP.ProfileId )

	  END 
	   IF EXISTS (SELECT TOP 1 1 FROM ZnodeProfile ZP INNER JOIN @TBL_Profileids TBP ON ( TBP.ProfileId =ZP.ProfileId AND TaxExempt  = 1   ))
	   --OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Profileids)
	   BEGIN 
			SET @TaxClassIds = ''
	   END   
       ELSE 
	   BEGIN 
	      	SET @TaxClassIds = SUBSTRING(	(    SELECT  ',' +CAST(ZPTC.TaxClassId   AS VARCHAR(50))
																	     FROM ZnodePortalTaxClass ZPTC 
                                                                         WHERE PortalId = @PortalId FOR XML PATH ('')  ) ,2 ,4000) 
	   END 

	 END TRY 
	 BEGIN CATCH 
	   DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCommonTaxClass @PortalId = '+CAST(@PortalId AS VARCHAR(50))+',@ProfileId='+CAST(@ProfileId AS VARCHAR(50))+',@TaxClassIds='+@TaxClassIds+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCommonTaxClass',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	 END CATCH 
 END