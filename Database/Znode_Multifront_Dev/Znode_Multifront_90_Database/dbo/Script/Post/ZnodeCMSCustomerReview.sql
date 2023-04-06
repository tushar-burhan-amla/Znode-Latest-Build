
--dt\01\10\2019  

		 UPDATE ZCCR set ZCCR.SKU = ZPPD.SKU
			 FROM ZnodePublishProductDetail ZPPD
			 INNER JOIN ZnodeCMSCustomerReview ZCCR on ZPPD.PublishProductId = ZCCR.PublishProductId
			 INNER JOIN ZnodePublishProduct ZPP on ZPPD.PublishProductId = ZPP.PublishProductId
			 where ZCCR.SKU is null