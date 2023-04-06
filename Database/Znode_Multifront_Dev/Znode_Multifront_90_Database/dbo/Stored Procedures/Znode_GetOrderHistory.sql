
CREATE PROCEDURE [dbo].[Znode_GetOrderHistory]
( @OrderId  INT )
AS 
   /*  
    Summary : This procedure is used to get the oms order history                 
  Records are fetched for those have history form Notes and History table	 	 
Unit Testing:

    EXEC [Znode_GetOrderHistory] 12
       
*/
    BEGIN

	   DECLARE @TBL_OrderDetails TABLE (OmsOrderDetailsId INT )

	   INSERT INTO @TBL_OrderDetails
	   SELECT OmsOrderDetailsId
	   FROM ZnodeOmsorderdetails
       WHERE OmsOrderId = @OrderId 

		;With Cte_GetHistory AS 
		(
		 SELECT  zhst.OmsOrderDetailsId,TransactionId, '' Notes,Message ,OrderAmount,zhst.CreatedBy,zhst.CreatedDate ,zhst.ModifiedBy,zhst.ModifiedDate
		 FROM ZnodeOmsHistory zhst
		 INNER JOIN @TBL_OrderDetails ZOD ON (ZOD.OmsOrderDetailsId = zhst.OmsOrderDetailsId)
		 WHERE zhst.OmsNotesId  IS NULL 
		 UNION ALL 
		 SELECT  zhst.OmsOrderDetailsId,TransactionId, zon.Notes Notes,Message ,OrderAmount,zhst.CreatedBy,zhst.CreatedDate ,zhst.ModifiedBy,zhst.ModifiedDate
		 FROM ZnodeOmsHistory zhst
		 INNER JOIN ZnodeOmsNotes zon  ON (ZON.OmsNotesId = ZHST.OmsNotesId)
		 INNER JOIN @TBL_OrderDetails ZOD ON (ZOD.OmsOrderDetailsId = zhst.OmsOrderDetailsId)
		 UNION ALL 
		 select  zon.OmsOrderDetailsId,null TransactionId,  Notes,'Order notes added successfully' Message ,null OrderAmount,CreatedBy ,createdDate, ModifiedBy,ModifiedDate
		 from ZnodeOmsNotes zon 
		 INNER JOIN @TBL_OrderDetails ZOD ON (ZOD.OmsOrderDetailsId = zon.OmsOrderDetailsId)
		 AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsHistory zhst WHERE zhst.OmsNotesId= zon.OmsNotesId  )
		)

		SELECT  OmsOrderDetailsId,TransactionId,  Notes, Message , OrderAmount,tHst.CreatedBy ,tHst.createdDate, tHst.ModifiedBy,tHst.ModifiedDate,zusr.Email 'UpdatedBy'
		FROM Cte_GetHistory tHst
		left join ZnodeUser zusr on zusr.UserId =tHst.ModifiedBy
		order by tHst.CreatedDate desc
				

    END;