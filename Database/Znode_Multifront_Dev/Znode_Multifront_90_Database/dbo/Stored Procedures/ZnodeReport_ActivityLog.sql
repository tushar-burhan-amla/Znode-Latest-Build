CREATE PROCEDURE [dbo].[ZnodeReport_ActivityLog]
(
		@PortalId		VARCHAR(max)  = ''
		,@BeginDate		DATE		  = NULL 
		,@EndDate       DATE          = NULL
		,@Category      NVARCHAR(510)	  = ''
) 
AS
/*
	 Summary : - This Procedure is used to get the activity log on the basis of portal and category 
	 Unit Testing
	 EXEC ZnodeReport_ActivityLog 2,'2017-01-01','2017-08-01',''
	*/
BEGIN 
 BEGIN TRY 
  SET NOCOUNT ON 
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
       IF ( @BeginDate  IS NULL   )
            BEGIN  
   -- If date is NULL then display current date activity log report.  
                SET @BeginDate = CONVERT(DATE,@GetDate,101);
            END  
        ELSE
        BEGIN
        SET @BeginDate = CONVERT(DATE,@BeginDate,101);  
        END
        
        SELECT  AL.[ActivityLogID],  
                AL.[ActivityLogTypeID],  
                Alt.[Name] [Name],  
                Alt.[TypeCategory]  AS 'Category',  
                AL.[ActivityCreateDate],  
                P.[StoreName] AS 'StoreName',  
                Al.ActivityEndDate,  
                AL.[Data1],  
                AL.[Data2],  
                AL.[Data3],  
                AL.[Status],  
                AL.[LongData],  
                AL.[Source],  
                AL.[Target],
                AL.[PortalID]  
        FROM    ZNodeActivityLog AL  
                INNER JOIN ZNodeActivityLogType Alt ON Alt.ActivityLogTypeID = AL.ActivityLogTypeID  
                INNER JOIN ZNodePortal P ON P.PortalId = AL.PortalId  
	    WHERE    (CAST(AL.[ActivityCreateDate] AS DATE) BETWEEN CASE
                                                                     WHEN @BeginDate IS NULL
                                                                     THEN CAST(AL.[ActivityCreateDate] AS DATE)
                                                                     ELSE @BeginDate
                                                                 END AND CASE
                                                                             WHEN @EndDate IS NULL
                                                                             THEN CAST(AL.[ActivityCreateDate] AS DATE)
                                                                             ELSE @EndDate
                                                                         END)
                AND Alt.TypeCategory LIKE '%' +CASE WHEN @Category = '0' THEN '' ELSE @Category END  + '%'  
                AND EXISTS(SELECT TOP 1 1 FROM dbo.split(@PortalId,',') SP WHERE (AL.PortalId = SP.Item OR @PortalId = '')  )
        ORDER BY AL.ActivityLogId DESC  

	END TRY 
	BEGIN CATCH 
	 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_ActivityLog @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(50))+',@EndDate='+CAST(@EndDate AS VARCHAR(50))+',@Category='+@Category+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_ActivityLog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH
END