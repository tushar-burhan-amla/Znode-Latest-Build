CREATE PROCEDURE [dbo].[Znode_DeleteUserDetails]  
(
    @UserId VARCHAR(2000) = NULL,
    @Status INT OUT   ,   
    @UserIds Transferid READONLY,
    @IsForceFullyDelete BIT =0,
	@IsCallInternal BIT = 0
)
AS
/*  
	Summary: This Procedure Is used to delete user details
	Unit Testing:
	EXEC Znode_DeleteUserDetails
*/  
     BEGIN  
         BEGIN TRY  
             SET NOCOUNT ON;  
             BEGIN TRAN A;  
    DECLARE @StatusOut Table (Id INT ,Message NVARCHAR(max), Status BIT )  
             DECLARE @V_table TABLE (  
                                    USERID1 NVARCHAR(200)  
                                    );  
             DECLARE @V_tabledeleted TABLE (  
                                           UserId1      INT ,  
                                           AspnetUserid NVARCHAR(1000)  
                                           );  
             DECLARE @TBL_DeleteduserName TABLE (  
                                                id NVARCHAR(MAX)  
                                                );  
             INSERT INTO @V_tabledeleted  
                    SELECT ITEM , b.AspNetUserId  
                    FROM dbo.split ( @UserId , ','  
                                   ) AS a INNER JOIN ZnodeUser AS b ON ( a.Item = b.UserId )  
           WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsOrderDetails zood WHERE zood.UserId= b.UserId)  
      AND UserId <> 2   
       INSERT INTO @V_tabledeleted            
       SELECT a.Id , b.AspNetUserId  
                    FROM @UserIds AS a   
     INNER JOIN ZnodeUser AS b ON ( a.Id = b.UserId )  
           WHERE (NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsOrderDetails zood WHERE zood.UserId= b.UserId) OR @IsForceFullyDelete =1 )   
           AND UserId <> 2        
             DELETE FROM ZnodeUserProfile  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeUserProfile.UserId  
                          );  
             DELETE FROM ZnodeUserAddress  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeUserAddress.UserId  
                          );   
  
   --------  

 
   delete from ZnodeOmsPersonalizeCartItem   
   where OmsSavedCartLineItemId in (  
    select OmsSavedCartLineItemId  from ZnodeOmsSavedCartLineItem where OmsSavedCartId in (  
    select OmsSavedCartId FROM ZnodeOmsSavedCart  
    where OmsCookieMappingId in (select OmsCookieMappingId from ZnodeOmsCookieMapping  
           WHERE EXISTS ( SELECT TOP 1 1  
              FROM @V_tabledeleted AS a  
              WHERE a.UserId1 = ZnodeOmsCookieMapping.UserId  
               ))));  
  
   delete from ZnodeOmsSavedCartLineItem where OmsSavedCartId in (  
   select OmsSavedCartId FROM ZnodeOmsSavedCart  
   where OmsCookieMappingId in (select OmsCookieMappingId from ZnodeOmsCookieMapping  
           WHERE EXISTS ( SELECT TOP 1 1  
              FROM @V_tabledeleted AS a  
              WHERE a.UserId1 = ZnodeOmsCookieMapping.UserId  
               )));  
 
   DELETE FROM ZnodeOmsSavedCart  
   where OmsCookieMappingId in (select OmsCookieMappingId from ZnodeOmsCookieMapping  
           WHERE EXISTS ( SELECT TOP 1 1  
              FROM @V_tabledeleted AS a  
              WHERE a.UserId1 = ZnodeOmsCookieMapping.UserId  
               ));  
   
   DELETE FROM ZnodeOmsCookieMapping  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeOmsCookieMapping.UserId  
                          );  
   
   DELETE ZGCH  
   FROM ZnodeGiftCardHistory ZGCH  
   INNER JOIN ZnodeGiftCard ZGC ON (ZGCH.GiftCardId = ZGC.GiftCardId)  
   WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZGC.UserId)  
  
   DELETE FROM ZnodeGiftCard              
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeGiftCard.UserId  
                          );  
     ---------  
 
             DELETE FROM ZnodeAccountUserOrderApproval  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeAccountUserOrderApproval.UserId  
                          );  
             DELETE FROM AspNetUserRoles  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.aspNetUserId = AspNetUserRoles.UserId  
                          );  
						
           
             DELETE FROM dbo.ZnodeAccountUserPermission  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeAccountUserPermission.UserId  
                          );  
       
    DELETE FROM ZnodeSalesRepCustomerUserPortal  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeSalesRepCustomerUserPortal.CustomerUserid  
                          );  
   
       delete from ZnodeSalesRepCustomerUserPortal   
    WHERE exists(select TOP 1 1  FROM ZnodeUserPortal ZUP where EXISTS ( SELECT TOP 1 1 FROM @V_tabledeleted AS a WHERE a.UserId1 = ZUP.UserId )  
                 and ZnodeSalesRepCustomerUserPortal.UserPortalId = ZUP.UserPortalId );  
       
    DELETE FROM ZnodeUserPortal  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeUserPortal.UserId  
                          );  
						
             DELETE FROM ZnodeAccountUserOrderApproval  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeAccountUserOrderApproval.UserId  
                                  OR  
                                  TBDL.UserId1 = ZnodeAccountUserOrderApproval.ApprovalUserId  
                          );  
						   
    DELETE FROM ZnodeOmsUsersReferralUrl   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsUsersReferralUrl.UserId  
                           );  
						    
    DELETE FROM ZnodeOmsReferralCommission   
    WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsReferralCommission.UserId        
                          );  
						  
   DELETE FROM ZnodeUserWishList   
   WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeUserWishList.UserId        
                          );  
						   
    DELETE FROM ZnodeDepartmentUser   
    WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeDepartmentUser.UserId        
                          );  
						  
    DELETE FROM ZnodeUserPromotion   
    WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeUserPromotion.UserId        
                          );  
   
    DELETE FROM AspNetUserClaims   
    WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = AspNetUserClaims.UserId        
                          );   
					
    DELETE FROM ZnodeNote   
    WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeNote.UserId        
                          );   
  
    DELETE FROM ZnodeOmsQuotePersonalizeItem WHERE OmsQuoteLineItemId IN (SELECT OmsQuoteLineItemId FROM ZnodeOmsQuoteLineItem  WHERE OmsQuoteId IN (SELECT OmsQuoteId FROM ZnodeOmsQuote   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsQuote.UserId        
                          ) ))   
						  
    DELETE FROM ZnodeOmsQuoteLineItem  WHERE OmsQuoteId IN (SELECT OmsQuoteId FROM ZnodeOmsQuote   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsQuote.UserId        
                          ) )  
   
   DELETE FROM ZnodeUserApprovers  
   WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeUserApprovers.ApproverUserId        
                          )   
  
   
   DELETE FROM ZnodeOMSQuoteApproval   
     WHERE exists(select *  FROM ZnodeOmsQuote   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsQuote.UserId        
                          ) AND ZnodeOMSQuoteApproval.OMSQuoteId = ZnodeOmsQuote.OmsQuoteId) ;  
   
   DELETE FROM ZnodeOmsQuoteComment   
     WHERE exists(select *  FROM ZnodeOmsQuote   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsQuote.UserId        
                          ) AND ZnodeOmsQuoteComment.OMSQuoteId = ZnodeOmsQuote.OmsQuoteId) ;  
  
  
  
   DELETE FROM ZnodeOmsNotes   
     WHERE exists(select *  FROM ZnodeOmsQuote   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsQuote.UserId        
                          ) AND ZnodeOmsNotes.OMSQuoteId = ZnodeOmsQuote.OmsQuoteId) ;  
  
   DELETE FROM ZnodeOmsQuoteHistory   
     WHERE exists(select *  FROM ZnodeOmsQuote   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsQuote.UserId        
                          ) AND ZnodeOmsQuoteHistory.OMSQuoteId = ZnodeOmsQuote.OmsQuoteId) ;   
  
    DELETE FROM ZnodeOmsQuote   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsQuote.UserId        
                          );   
						 
    DELETE FROM ZnodeAccountUserPermission   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeAccountUserPermission.UserId        
                          );  
						
     DELETE FROM ZnodePriceListUser   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodePriceListUser.UserId        
                          );   
						  
     DELETE FROM ZnodeMediaFolderUser   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeMediaFolderUser.UserId        
                          ); 
						  
     DELETE FROM ZnodeAccountUserOrderApproval   
     WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeAccountUserOrderApproval.UserId        
                          );   
						   
    DELETE FROM ZnodeOmsTemplateLineItem WHERE OmsTemplateId IN (SELECT OmsTemplateId   
    FROM ZnodeOmsTemplate WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsTemplate.UserId        
                          ) )  
						
  
    DELETE FROM dbo.ZnodeFormBuilderGlobalAttributeValueLocale   WHERE FormBuilderGlobalAttributeValueId IN   
    (SELECT FormBuilderGlobalAttributeValueId  FROM dbo.ZnodeFormBuilderGlobalAttributeValue  WHERE FormBuilderSubmitId IN (SELECT FormBuilderSubmitId FROM dbo.ZnodeFormBuilderSubmit WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeFormBuilderSubmit.UserId        
                          ) ))  
  
     DELETE FROM dbo.ZnodeFormBuilderGlobalAttributeValue  WHERE FormBuilderSubmitId IN (SELECT FormBuilderSubmitId FROM dbo.ZnodeFormBuilderSubmit WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeFormBuilderSubmit.UserId        
                          ) )  
						
     DELETE FROM dbo.ZnodeFormBuilderSubmit WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeFormBuilderSubmit.UserId        
                          )    

    DELETE FROM ZnodeOmsTemplate  WHERE  EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS TBDL  
                            WHERE TBDL.UserId1 = ZnodeOmsTemplate.UserId        
                          )   
      
      
   DELETE FROM ZnodeCaseRequestHistory WHERE CaseRequestId IN (SELECT CaseRequestId  FROM ZnodeCaseRequest  WHERE EXISTS (SELECT TOP  1 1 FROM @V_tabledeleted AS TBDL WHERE TBDL.UserId1 = ZnodeCaseRequest.UserId))  
     DELETE FROM ZnodeCaseRequest  WHERE EXISTS (SELECT TOP  1 1 FROM @V_tabledeleted AS TBDL WHERE TBDL.UserId1 = ZnodeCaseRequest.UserId)  
     DECLARE @OrderId Transferid   
    INSERT INTO @OrderId   
    SELECT DISTINCT  a.OmsOrderId   
    FROM ZnodeOMsOrder A   
    INNER JOIN ZnodeOmsOrderDetails b ON(b.OmsOrderId = a.OmsOrderId)  
    WHERE EXISTS (SELECT TOP 1  1 FROM @V_tabledeleted t WHERE b.UserId = t.UserId1 )  
    INSERT INTO @StatusOut (Id ,Status)   
   EXEC  Znode_DeleteOrderById   @OmsOrderIds =@OrderId ,@Status = 0   
   UPDATE ZnodePublishCatalogLog  SET Userid =2   
   WHERE UserId IN (SELECT UserId1 FROM @V_tabledeleted t)  
  
      
    DELETE FROM ZnodeCMSCustomerReview WHERE UserId IN (SELECT UserId1 FROM @V_tabledeleted t)  
             DELETE FROM ZnodeBlogNewsCommentLocale where BlogNewsCommentId in (select BlogNewsCommentId from  ZnodeBlogNewsComment WHERE UserId IN  
     (SELECT UserId1 FROM @V_tabledeleted t))  
    DELETE FROM ZnodeBlogNewsComment WHERE UserId IN (SELECT UserId1 FROM @V_tabledeleted t)  
  
   -----------  

   DELETE FROM ZnodePublishedPortalXml  
   WHERE EXISTS(SELECT *  FROM ZnodePublishPortalLog  
      WHERE EXISTS ( SELECT TOP 1 1  
         FROM @V_tabledeleted AS TBDL  
         WHERE TBDL.UserId1 = ZnodePublishPortalLog.UserId  
      AND ZnodePublishedPortalXml.PublishPortalLogId = ZnodePublishPortalLog.PublishPortalLogId));  
  
   DELETE FROM ZnodePublishPortalLog  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = ZnodePublishPortalLog.UserId        
           );   

   DELETE FROM ZnodeUserGlobalAttributeValueLocale  
   WHERE EXISTS(SELECT *  FROM ZnodeUserGlobalAttributeValue  
      WHERE EXISTS ( SELECT TOP 1 1  
         FROM @V_tabledeleted AS TBDL  
         WHERE TBDL.UserId1 = ZnodeUserGlobalAttributeValue.UserId  
      AND ZnodeUserGlobalAttributeValueLocale.UserGlobalAttributeValueId = ZnodeUserGlobalAttributeValue.UserGlobalAttributeValueId));   
   DELETE FROM ZnodeUserGlobalAttributeValue  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = ZnodeUserGlobalAttributeValue.UserId        
           );   

   DELETE FROM ZnodeOMSQuoteApproval  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = ZnodeOMSQuoteApproval.UserId        
           );   

   DELETE FROM AspNetUserLogins  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = AspNetUserLogins.UserId        
           );   
  
   DELETE FROM ZnodePasswordLog  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.aspNetUserId = ZnodePasswordLog.UserId        
           );   
   
   DELETE FROM ZnodeSavedReportViews  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = ZnodeSavedReportViews.UserId        
           );   
 
   DELETE FROM ZnodeSearchActivity  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = ZnodeSearchActivity.UserId        
           );   
  
   DELETE FROM ZnodeOmsCustomerShipping  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = ZnodeOmsCustomerShipping.UserId        
           );   

   DELETE FROM ZnodeOMSQuoteApproval  
   WHERE EXISTS ( SELECT TOP 1 1  
          FROM @V_tabledeleted AS TBDL  
          WHERE TBDL.UserId1 = ZnodeOMSQuoteApproval.ApproverUserId        
           );   
     
	DELETE FROM ZnodeTradeCentricUser  
	WHERE EXISTS ( SELECT TOP 1 1
          FROM @V_tabledeleted AS TBDL
          WHERE TBDL.UserId1 = ZnodeTradeCentricUser.UserId
           );
   ------------  
     
  
   DELETE FROM ZnodeUser  
             OUTPUT deleted.UserId  
                    INTO @V_table  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.UserId1 = ZnodeUser.UserId  
                          );  
 
             DELETE FROM AspNetUsers  
             OUTPUT deleted.UserName  
                    INTO @TBL_DeleteduserName  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @V_tabledeleted AS a  
                            WHERE a.AspnetUserid = AspNetUsers.Id  
                          );  
  
             DELETE FROM AspNetZnodeUser  
             WHERE EXISTS ( SELECT TOP 1 1  
                            FROM @TBL_DeleteduserName AS TBUN  
                            WHERE TBUN.id = AspNetZnodeUser.AspNetZnodeUserId  
                          );  
      
     
     IF  @IsCallInternal = 0    
     BEGIN   
             IF ( SELECT COUNT(1)  
                  FROM @V_tabledeleted  
                ) = ( SELECT COUNT(1)  
                      FROM dbo.split ( @UserId , ','  
                                     )  
                    ) OR @UserId IS NULL   
                 BEGIN  
                     SELECT 0 AS ID , CAST(1 AS BIT) AS Status;  
                 END;  
             ELSE  
                 BEGIN  
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;  
                 END;  
         END   
             SET @Status = 1;  
             COMMIT TRAN A;  
         END TRY  
         BEGIN CATCH  
   SELECT ERROR_MESSAGE()  
              DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteUserDetails @UserId = '+@UserId+',@Status='+CAST(@Status AS VARCHAR(200));  
             SET @Status = 0;  
             SELECT 0 AS ID,  
                    CAST(0 AS BIT) AS Status;  
    ROLLBACK TRAN A;  
             EXEC Znode_InsertProcedureErrorLog  
                  @ProcedureName = 'Znode_DeleteUserDetails',  
                  @ErrorInProcedure = @Error_procedure,  
                  @ErrorMessage = @ErrorMessage,  
                  @ErrorLine = @ErrorLine,  
                  @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;