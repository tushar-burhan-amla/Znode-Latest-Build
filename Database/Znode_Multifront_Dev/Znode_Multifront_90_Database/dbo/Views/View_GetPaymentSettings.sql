
CREATE  VIEW [dbo].[View_GetPaymentSettings]
AS
     SELECT a.PaymentSettingId,
            a.PaymentApplicationSettingId,
            b.GatewayName,
            c.Name PaymentTypeName,
            ISNULL(d.ProfileName, 'All Profiles') ProfileName,
            a.DisplayOrder,
            a.IsActive,
            d.ProfileId,
            c.PaymentTypeId,
			a.IsPoDocUploadEnable,
			a.IsPoDocRequire
     FROM ZnodePaymentSetting a
          LEFT JOIN ZnodePaymentGateway b ON(a.PaymentGatewayId = b.PaymentGatewayId)
          INNER JOIN ZNodePaymentType c ON(a.PaymentTypeId = c.PaymentTypeId)
		  INNER JOIN ZnodeProfilePaymentsetting ZPPP ON ZPPP.PaymentSettingId = a.PaymentSettingId
		  INNER JOIN ZnodeProfile ZPP ON ZPPP.ProfileId = Zpp.ProfileID
          LEFT JOIN ZnodeProfile d ON(ZPP.ProfileId = d.ProfileId);