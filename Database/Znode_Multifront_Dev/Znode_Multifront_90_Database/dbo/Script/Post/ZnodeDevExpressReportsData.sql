
IF NOT EXISTS(SELECT TOP 1 1 FROM ZnodeReportCategories )
BEGIN
SET IDENTITY_INSERT [dbo].[ZnodeReportCategories] ON 
INSERT [dbo].[ZnodeReportCategories] ([ReportCategoryId], [CategoryName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Sales', 1, 2, CAST(N'2018-11-23T15:28:02.903' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.903' AS DateTime))
INSERT [dbo].[ZnodeReportCategories] ([ReportCategoryId], [CategoryName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'Product', 1, 2, CAST(N'2018-11-23T15:28:02.910' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.910' AS DateTime))
INSERT [dbo].[ZnodeReportCategories] ([ReportCategoryId], [CategoryName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'Review', 1, 2, CAST(N'2018-11-23T15:28:02.913' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.913' AS DateTime))
INSERT [dbo].[ZnodeReportCategories] ([ReportCategoryId], [CategoryName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'Customer', 1, 2, CAST(N'2018-11-23T15:28:02.917' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.917' AS DateTime))
INSERT [dbo].[ZnodeReportCategories] ([ReportCategoryId], [CategoryName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'General', 1, 2, CAST(N'2018-11-23T15:28:02.920' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.920' AS DateTime))
INSERT [dbo].[ZnodeReportCategories] ([ReportCategoryId], [CategoryName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'Custom Reports', 1, 2, CAST(N'2018-11-23T15:28:02.927' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.927' AS DateTime))
INSERT [dbo].[ZnodeReportCategories] ([ReportCategoryId], [CategoryName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'Product info', 1, 2, CAST(N'2018-11-23T15:28:02.930' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.930' AS DateTime))
SET IDENTITY_INSERT [dbo].[ZnodeReportCategories] OFF
END

IF NOT EXISTS(SELECT TOP 1 1 FROM ZnodeReportDetails )
BEGIN
SET IDENTITY_INSERT [dbo].[ZnodeReportDetails] ON 
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (1, 1, N'Orders', N'Orders', N'To view Orders for a particular date range with Order status.', 2, CAST(N'2018-11-23T15:28:02.937' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.937' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (2, 1, N'Coupons', N'Coupon Usage', N'To see the coupon usage on your site.', 2, CAST(N'2018-11-23T15:28:02.950' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.950' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (3, 1, N'SalesTax', N'Sales Tax', N'To see the sales tax details.', 2, CAST(N'2018-11-23T15:28:02.957' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.957' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (4, 1, N'AffiliateOrders', N'Affiliate Orders', N'To see the affiliate orders on your site.', 2, CAST(N'2018-11-23T15:28:02.970' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.970' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (5, 1, N'OrderPickList', N'Order Pick List', N'To get a list of items to be pulled from inventory for each order with a status of Submitted.', 2, CAST(N'2018-11-23T15:28:02.977' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.977' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (6, 2, N'BestSellerProduct', N'Best Sellers', N'To list your best selling products.', 2, CAST(N'2018-11-23T15:28:02.983' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.983' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (7, 2, N'InventoryReorder', N'INVENTORY RE-ORDER', N'To list all products whose inventory has dropped below the re-order level.', 2, CAST(N'2018-11-23T15:28:02.993' AS DateTime), 2, CAST(N'2018-11-23T15:28:02.993' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (8, 7, N'ProductInfo', N'Product Info', N'This report show the report product info.', 2, CAST(N'2018-11-23T15:28:03.003' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.003' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (9, 2, N'TopEarningProduct', N'Top Earning Products', N'To list the products that generate the most revenue on your site.', 2, CAST(N'2018-11-23T15:28:03.017' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.017' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (10, 2, N'PopularSearch', N'Popular Search', N'To see most popular search keywords entered by Customers on your site.', 2, CAST(N'2018-11-23T15:28:03.027' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.027' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (11, 4, N'Users', N'Users', N'To list Users from each profile and filter them by the date that they were created.', 2, CAST(N'2018-11-23T15:28:03.037' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.037' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (12, 4, N'EmailOptInCustomer', N'Email Opt-In Customer', N'To get a list of all Accounts who have selected to receive promotional email from you.', 2, CAST(N'2018-11-23T15:28:03.060' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.060' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (13, 4, N'MostFrequentCustomer', N'Most Frequent Customer', N'To find the customers who purchase from you most often.', 2, CAST(N'2018-11-23T15:28:03.067' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.067' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (14, 4, N'TopSpendingCustomers', N'Top Spending Customers', N'To list the products that generate the most revenue on your site.', 2, CAST(N'2018-11-23T15:28:03.077' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.077' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (15, 5, N'Vendors', N'Vendor', N'To see the Vendor details.', 2, CAST(N'2018-11-23T15:28:03.083' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.083' AS DateTime), 1)
INSERT [dbo].[ZnodeReportDetails] ([ReportDetailId], [ReportCategoryId], [ReportCode], [ReportName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (16, 3, N'ServiceRequest', N'Service Requests', N'To get a list of all service requests.', 2, CAST(N'2018-11-23T15:28:03.090' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.090' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[ZnodeReportDetails] OFF
END

IF NOT EXISTS(SELECT TOP 1 1 FROM ZnodeReportSetting)
BEGIN
SET IDENTITY_INSERT [dbo].[ZnodeReportSetting] ON 
INSERT [dbo].[ZnodeReportSetting] ([ReportSettingId], [ReportCode], [SettingXML], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [DisplayMode], [StyleSheetId], [DefaultLayoutXML]) VALUES (1, N'Orders', N'<?xml version="1.0" encoding="utf-16"?><columns><column><id>1</id><name>StoresName</name><headertext>Store Name</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>y</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>2</id><name>BeginDate</name><headertext>Begin Date</headertext><width>0</width><datatype>datetime</datatype><columntype>datetime</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>y</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>3</id><name>EndDate</name><headertext>End Date</headertext><width>0</width><datatype>datetime</datatype><columntype>datetime</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>y</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>4</id><name>OrderNumber</name><headertext>Order No.</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>5</id><name>OrderDate</name><headertext>Date and Time</headertext><width>0</width><datatype>datetime</datatype><columntype>datetime</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>6</id><name>OrderStatus</name><headertext>Status</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>7</id><name>BillingName</name><headertext>Billing Name</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>8</id><name>BillingFirstName</name><headertext>First Name</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>9</id><name>BillingLastName</name><headertext>Last Name</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>10</id><name>BillingCompanyName</name><headertext>Company Name</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>18</id><name>ShippingCity</name><headertext>Shipping City</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>11</id><name>ShippingStateCode</name><headertext>Shipping State</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>12</id><name>ShippingCountryCode</name><headertext>Shipping Country</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>13</id><name>BillingPhoneNumber</name><headertext>Billing Ph No</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>14</id><name>BillingEmailId</name><headertext>Billing Email ID</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>15</id><name>ShippingTypeName</name><headertext>Shipping Type</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>16</id><name>PaymentTypeName</name><headertext>Payment Type</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>17</id><name>TaxCost</name><headertext>Tax</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>18</id><name>ShippingCost</name><headertext>Shipping Cost</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>19</id><name>SubTotal</name><headertext>Sub Total</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>20</id><name>DiscountAmount</name><headertext>Discount</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>21</id><name>Total</name><headertext>Total Amount</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>y</mustshow><musthide>y</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>22</id><name>PurchaseOrderNumber</name><headertext>Purchase Order Number</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>23</id><name>SalesTax</name><headertext>Sales Tax</headertext><width>0</width><datatype>decimal</datatype><columntype>decimal</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>24</id><name>StoreName</name><headertext>Store Name</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>25</id><name>Symbol</name><headertext>Symbol</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>26</id><name>VisibleColumns</name><headertext>Visible Columns</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>y</isallowsearch><value></value><parametertype>text</parametertype></column><column><id>27</id><name>ChartType</name><headertext>Chart Type</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value>None,Bar,Pie</value><parametertype>dropdown</parametertype></column><column><id>28</id><name>XAxis</name><headertext>X Axis</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value>Order Date,Customer Name,Store Name</value><parametertype>dropdown</parametertype></column><column><id>29</id><name>YAxis</name><headertext>Y Axis</headertext><width>0</width><datatype>string</datatype><columntype>string</columntype><isvisible>y</isvisible><mustshow>n</mustshow><musthide>n</musthide><maxlength>0</maxlength><isallowsearch>n</isallowsearch><value>Total Amount</value><parametertype>dropdown</parametertype></column></columns>', 2, CAST(N'2018-11-23T15:28:03.120' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.120' AS DateTime), 1, NULL, N'<?xml version="1.0" encoding="utf-8"?>
<XtraReportsLayoutSerializer SerializerVersion="17.2.7.0" Ref="1" ControlType="Orders, Znode.Libraries.DevExpress.Report, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" Landscape="true" Margins="25, 27, 29, 42" PaperKind="Custom" PageWidth="1500" PageHeight="827" Version="17.2" RequestParameters="false" DataSource="#Ref-0" Font="Verdana, 6.75pt">
  <Parameters>BillingLastName
    <Item1 Ref="4" Description="Store Name" LookUpSettings="#Ref-2" MultiValue="true" ValueInfo="Fine Foods|Wine &amp; Cheese|Nut Wholesaler|ModernB2C" Name="StoresName" />
    <Item2 Ref="6" Description="Begin Date" ValueInfo="2018-10-01" Name="BeginDate" Type="#Ref-5" />
    <Item3 Ref="7" Description="End Date" ValueInfo="2018-10-31" Name="EndDate" Type="#Ref-5" />
    <Item4 Ref="9" Description="Visible Columns" LookUpSettings="#Ref-8" MultiValue="true" ValueInfo="Store Name|Order No.|Date and Time|Status|Billing Name|Payment Type|Tax|Shipping Cost|Sub Total|Discount|Total Amount" Name="VisibleColumns" />
  </Parameters>
  <Bands>
    <Item1 Ref="10" ControlType="DetailBand" Name="Detail" HeightF="25" TextAlignment="TopLeft" Padding="0,0,0,0,100">
      <Controls>
        <Item1 Ref="11" ControlType="XRTable" Name="xrTable2" SizeF="1448,25" LocationFloat="0, 0" EvenStyleName="EvenStyle" OddStyleName="OddStyle">
          <Rows>
            <Item1 Ref="12" ControlType="XRTableRow" Name="xrTableRow2" Weight="11.5">
              <Cells>
                <Item1 Ref="13" ControlType="XRTableCell" Name="xrTableCell26" Weight="0.068246096482670215" CanShrink="true" Text="xrTableCell26" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="14" Expression="[StoreName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                </Item1>
                <Item2 Ref="15" ControlType="XRTableCell" Name="xrTableCell18" Weight="0.056427902627327151" CanShrink="true" TextTrimming="None" Text="xrTableCell18" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="16" Expression="[OrderNumber]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="17" UseFont="false" />
                </Item2>
                <Item3 Ref="18" ControlType="XRTableCell" Name="xrTableCell19" Weight="0.061983704302449351" CanShrink="true" TextTrimming="None" Text="xrTableCell19" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="19" Expression="[OrderDate]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="20" UseFont="false" />
                </Item3>
                <Item4 Ref="21" ControlType="XRTableCell" Name="xrTableCell20" Weight="0.06060738129186833" CanShrink="true" TextTrimming="None" Text="xrTableCell20" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="22" Expression="[OrderStatus]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="23" UseFont="false" />
                </Item4>
                <Item5 Ref="24" ControlType="XRTableCell" Name="xrTableCell21" Weight="0.07332425447814632" CanShrink="true" TextTrimming="None" Text="xrTableCell21" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="25" Expression="[BillingFirstName] +'' ''+ [BillingLastName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="26" UseFont="false" />
                </Item5>
                <Item6 Ref="27" ControlType="XRTableCell" Name="xrTableCell24" Weight="0.044430920527531552" CanShrink="true" TextTrimming="None" Text="xrTableCell24" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="28" Expression="[ShippingCity]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="29" UseFont="false" />
                </Item6>
                <Item7 Ref="30" ControlType="XRTableCell" Name="xrTableCell25" Weight="0.038953133068130807" CanShrink="true" TextTrimming="None" Text="xrTableCell25" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="31" Expression="[ShippingStateCode]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="32" UseFont="false" />
                </Item7>
                <Item8 Ref="33" ControlType="XRTableCell" Name="xrTableCell27" Weight="0.061946451723105783" CanShrink="true" TextTrimming="None" Text="xrTableCell27" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="34" Expression="[ShippingCountryCode]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="35" UseFont="false" />
                </Item8>
                <Item9 Ref="36" ControlType="XRTableCell" Name="xrTableCell29" Weight="0.074221364001946219" CanShrink="true" TextTrimming="None" Text="xrTableCell29" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="37" Expression="[BillingEmailId]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="38" UseFont="false" />
                </Item9>
                <Item10 Ref="39" ControlType="XRTableCell" Name="xrTableCell6" Weight="0.062831401313047278" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="40" Expression="[ShippingTypeName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="41" UseFont="false" />
                </Item10>
                <Item11 Ref="42" ControlType="XRTableCell" Name="xrTableCell22" Weight="0.060383592656282244" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="43" Expression="[PaymentTypeName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="44" UseFont="false" />
                </Item11>
                <Item12 Ref="45" ControlType="XRTableCell" Name="xrTableCell30" Weight="0.056358764891089728" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell30" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="1,1,1,1,100">
                  <ExpressionBindings>
                    <Item1 Ref="46" Expression="[SubTotal]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="47" UseFont="false" UsePadding="false" />
                </Item12>
                <Item13 Ref="48" ControlType="XRTableCell" Name="xrTableCell31" Weight="0.064935194096351539" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell31" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="49" Expression="[ShippingCost]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="50" UseFont="false" />
                </Item13>
                <Item14 Ref="51" ControlType="XRTableCell" Name="xrTableCell32" Weight="0.058897048605883351" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell32" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="52" Expression="[TaxCost]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="53" UseFont="false" />
                </Item14>
                <Item15 Ref="54" ControlType="XRTableCell" Name="xrTableCell33" Weight="0.063411221826810277" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell33" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="55" Expression="[DiscountAmount]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="56" UseFont="false" />
                </Item15>
                <Item16 Ref="57" ControlType="XRTableCell" Name="xrTableCell34" Weight="0.072790928700972118" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell34" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="58" Expression="[TotalAmount]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="59" UseFont="false" />
                </Item16>
              </Cells>
            </Item1>
          </Rows>
        </Item1>
      </Controls>
    </Item1>
    <Item2 Ref="60" ControlType="TopMarginBand" Name="TopMargin" HeightF="29.16667" TextAlignment="TopLeft" Padding="0,0,0,0,100" />
    <Item3 Ref="61" ControlType="BottomMarginBand" Name="BottomMargin" HeightF="41.91666" TextAlignment="TopLeft" Padding="0,0,0,0,100">
      <Controls>
        <Item1 Ref="62" ControlType="XRPageInfo" Name="xrPageInfo1" PageInfo="DateTime" SizeF="501.5417,23" LocationFloat="0, 6.00001" StyleName="ReportFooterDateTime" Padding="2,2,0,0,100" CanPublish="false" />
        <Item2 Ref="63" ControlType="XRPageInfo" Name="xrPageInfo2" TextFormatString="Page {0} of {1}" TextAlignment="TopRight" SizeF="946.4584,23" LocationFloat="501.5417, 6.00001" StyleName="ReportFooterPaging" Padding="2,2,0,0,100" CanPublish="false" />
      </Controls>
    </Item3>
    <Item4 Ref="64" ControlType="ReportHeaderBand" Name="reportHeaderBand1" HeightF="94.37502">
      <SubBands>
        <Item1 Ref="65" ControlType="SubBand" Name="SubBand1" HeightF="290.625" Visible="false">
          <Controls>
            <Item1 Ref="66" ControlType="XRChart" Name="xrChart1" SizeF="1448,290.625" LocationFloat="0, 0" BorderColor="Black" Borders="None">
              <Chart Ref="67" AutoLayout="true" PaletteName="Flow">
                <DataContainer Ref="68">
                  <SeriesSerializable>
                    <Item1 Ref="69" Name="Series 1" ArgumentDataMember="XAxis" ValueDataMembersSerializable="YAxis" LabelsVisibility="True">
                      <Label Ref="70" Position="Top" TypeNameSerializable="SideBySideBarSeriesLabel" BackColor="Window" LineVisibility="True" TextPattern="{V:$0.00}">
                        <Border Ref="71" Visibility="False" />
                      </Label>
                    </Item1>
                  </SeriesSerializable>
                  <SeriesTemplate Ref="72" ToolTipSeriesPattern="{S:$0.00}">
                    <View Ref="73" ExplodeMode="All" TypeNameSerializable="PieSeriesView">
                      <TotalLabel Ref="74" Visible="true" />
                    </View>
                  </SeriesTemplate>
                </DataContainer>
                <Legend Ref="75" Visibility="False" Name="Default Legend" />
                <OptionsPrint Ref="76" ImageFormat="Metafile" />
                <Diagram Ref="77" TypeNameSerializable="XYDiagram">
                  <AxisX Ref="78" VisibleInPanesSerializable="-1" />
                  <AxisY Ref="79" VisibleInPanesSerializable="-1" />
                  <DefaultPane Ref="80" EnableAxisXScrolling="False" EnableAxisYScrolling="False" EnableAxisXZooming="False" EnableAxisYZooming="False">
                    <StackedBarTotalLabel Ref="81">
                      <ConnectorLineStyle Ref="82" />
                    </StackedBarTotalLabel>
                  </DefaultPane>
                </Diagram>
              </Chart>
            </Item1>
          </Controls>
        </Item1>
      </SubBands>
      <Controls>
        <Item1 Ref="83" ControlType="XRTable" Name="xrTable5" SizeF="825,30.00001" LocationFloat="297.9584, 60.38" StyleName="ReportHeaderTable1">
          <Rows>
            <Item1 Ref="84" ControlType="XRTableRow" Name="xrTableRow6" Weight="0.8">
              <Cells>
                <Item1 Ref="85" ControlType="XRTableCell" Name="xrTableCell54" Weight="1" Text="Store Name Equals:" StyleName="ReportHeaderTable1" Font="Verdana, 6.75pt" Padding="2,2,0,0,100" />
              </Cells>
            </Item1>
            <Item2 Ref="86" ControlType="XRTableRow" Name="xrTableRow7" Weight="0.79999999999999993">
              <Cells>
                <Item1 Ref="87" ControlType="XRTableCell" Name="xrTableCell55" Weight="1" Text="Fine Foods, Wine &amp; Cheese, Nut Wholesaler, ModernB2C" StyleName="ReportHeaderTableRow" Padding="2,2,0,0,100" />
              </Cells>
            </Item2>
          </Rows>
        </Item1>
        <Item2 Ref="88" ControlType="XRTable" Name="xrTable4" SizeF="287.3843,29.99998" LocationFloat="9.536743E-05, 60.38" StyleName="ReportHeaderTable1">
          <Rows>
            <Item1 Ref="89" ControlType="XRTableRow" Name="xrTableRow5" Weight="0.8">
              <Cells>
                <Item1 Ref="90" ControlType="XRTableCell" Name="xrTableCell53" Weight="1" Text="Order Date is Between:" StyleName="ReportHeaderTable1" Font="Verdana, 6.75pt" Padding="2,2,0,0,100" />
              </Cells>
            </Item1>
            <Item2 Ref="91" ControlType="XRTableRow" Name="xrTableRow4" Weight="0.79999999999999993">
              <Cells>
                <Item1 Ref="92" ControlType="XRTableCell" Name="xrTableCell52" Weight="1" Text="10/1/2018 12:00:00 AM - 10/31/2018 12:00:00 AM" StyleName="ReportHeaderTableRow" Font="Verdana, 6.75pt" Padding="2,2,0,0,100" />
              </Cells>
            </Item2>
          </Rows>
        </Item2>
        <Item3 Ref="93" ControlType="XRLabel" Name="xrLabel3" Text="ORDERS" TextAlignment="MiddleLeft" SizeF="211.2345,29.24998" LocationFloat="9.999998, 10.00001" StyleName="ReportHeaderText" Padding="2,2,0,0,100" CanPublish="false">
          <StylePriority Ref="94" UseTextAlignment="false" />
        </Item3>
        <Item4 Ref="95" ControlType="XRLabel" Name="xrLabel1" SizeF="1448,50" LocationFloat="0, 0" StyleName="ReportTitle" Padding="2,2,0,0,100" CanPublish="false" />
      </Controls>
    </Item4>
    <Item5 Ref="96" ControlType="GroupHeaderBand" Name="groupHeaderBand1" GroupUnion="WithFirstDetail" RepeatEveryPage="true" HeightF="48.00002">
      <Controls>
        <Item1 Ref="97" ControlType="XRPanel" Name="xrPanel1" SizeF="1448,48" LocationFloat="0, 0">
          <Controls>
            <Item1 Ref="98" ControlType="XRTable" Name="xrTable1" SizeF="1448,28" LocationFloat="7.947286E-06, 20.00001" Font="Verdana, 6.75pt">
              <Rows>
                <Item1 Ref="99" ControlType="XRTableRow" Name="xrTableRow1" Weight="1">
                  <Cells>
                    <Item1 Ref="100" ControlType="XRTableCell" Name="xrTableCell23" Weight="0.075828990991525846" CanShrink="true" Text="Store Name" StyleName="TableHeader" />
                    <Item2 Ref="101" ControlType="XRTableCell" Name="xrTableCell1" Weight="0.062697712896730956" CanShrink="true" TextTrimming="None" Text="Order No." StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="102" UseFont="false" />
                    </Item2>
                    <Item3 Ref="103" ControlType="XRTableCell" Name="xrTableCell2" Weight="0.068870757355785031" CanShrink="true" TextTrimming="None" Text="Date and Time" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="104" UseFont="false" />
                    </Item3>
                    <Item4 Ref="105" ControlType="XRTableCell" Name="xrTableCell3" Weight="0.067341551226841975" CanShrink="true" TextTrimming="None" Text="Status" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="106" UseFont="false" />
                    </Item4>
                    <Item5 Ref="107" ControlType="XRTableCell" Name="xrTableCell4" Weight="0.081471399904093328" CanShrink="true" TextTrimming="None" Text="Billing Name" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="108" UseFont="false" />
                    </Item5>
                    <Item6 Ref="109" ControlType="XRTableCell" Name="xrTableCell7" Weight="0.049367678877754291" CanShrink="true" TextTrimming="None" Text="Shipping City" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="110" UseFont="false" />
                    </Item6>
                    <Item7 Ref="111" ControlType="XRTableCell" Name="xrTableCell8" Weight="0.043281224224828573" CanShrink="true" TextTrimming="None" Text="Shipping State" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="112" UseFont="false" />
                    </Item7>
                    <Item8 Ref="113" ControlType="XRTableCell" Name="xrTableCell10" Weight="0.068829398553931082" CanShrink="true" TextTrimming="None" Text="Shipping Country" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="114" UseFont="false" />
                    </Item8>
                    <Item9 Ref="115" ControlType="XRTableCell" Name="xrTableCell12" Weight="0.082468141097089123" CanShrink="true" TextTrimming="None" Text="Billing Email ID" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="116" UseFont="false" />
                    </Item9>
                    <Item10 Ref="117" ControlType="XRTableCell" Name="xrTableCell5" Weight="0.069812749685619929" CanShrink="true" TextTrimming="None" Text="Shipping Type" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                      <StylePriority Ref="118" UseFont="false" />
                    </Item10>
                    <Item11 Ref="119" ControlType="XRTableCell" Name="xrTableCell9" Weight="0.0670928480258978" CanShrink="true" TextTrimming="None" Text="Payment Type" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                      <StylePriority Ref="120" UseFont="false" />
                    </Item11>
                    <Item12 Ref="121" ControlType="XRTableCell" Name="xrTableCell13" Weight="0.0626208948088319" CanShrink="true" TextTrimming="None" Text="Sub Total" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="122" UseFont="false" />
                    </Item12>
                    <Item13 Ref="123" ControlType="XRTableCell" Name="xrTableCell14" Weight="0.072150218037083561" CanShrink="true" TextTrimming="None" Text="Shipping Cost" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="124" UseFont="false" />
                    </Item13>
                    <Item14 Ref="125" ControlType="XRTableCell" Name="xrTableCell15" Weight="0.065441166705656251" CanShrink="true" TextTrimming="None" Text="Tax" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="126" UseFont="false" />
                    </Item14>
                    <Item15 Ref="127" ControlType="XRTableCell" Name="xrTableCell16" Weight="0.070456918953480663" CanShrink="true" TextTrimming="None" Text="Discount" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="2,2,0,0,100">
                      <StylePriority Ref="128" UseFont="false" />
                    </Item15>
                    <Item16 Ref="129" ControlType="XRTableCell" Name="xrTableCell17" Weight="0.080878807742879977" CanShrink="true" Multiline="true" TextTrimming="None" Text="Total Amount" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="130" UseFont="false" />
                    </Item16>
                  </Cells>
                </Item1>
              </Rows>
              <StylePriority Ref="131" UseFont="false" />
            </Item1>
          </Controls>
        </Item1>
      </Controls>
    </Item5>
    <Item6 Ref="132" ControlType="GroupFooterBand" Name="GroupFooter1" HeightF="28">
      <Controls>
        <Item1 Ref="133" ControlType="XRTable" Name="xrTable3" SizeF="1448,28" LocationFloat="0, 0" Font="Verdana, 6.75pt">
          <Rows>
            <Item1 Ref="134" ControlType="XRTableRow" Name="xrTableRow3" Weight="1">
              <Cells>
                <Item1 Ref="135" ControlType="XRTableCell" Name="xrTableCell35" Weight="0.075828990991525846" CanShrink="true" Text="Total" StyleName="TableHeader" />
                <Item2 Ref="136" ControlType="XRTableCell" Name="xrTableCell36" Weight="0.062697712896730956" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="137" UseFont="false" />
                </Item2>
                <Item3 Ref="138" ControlType="XRTableCell" Name="xrTableCell37" Weight="0.068870746676420208" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="139" UseFont="false" />
                </Item3>
                <Item4 Ref="140" ControlType="XRTableCell" Name="xrTableCell38" Weight="0.067341557226725648" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="141" UseFont="false" />
                </Item4>
                <Item5 Ref="142" ControlType="XRTableCell" Name="xrTableCell39" Weight="0.081471404583574478" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="143" UseFont="false" />
                </Item5>
                <Item6 Ref="144" ControlType="XRTableCell" Name="xrTableCell40" Weight="0.049367678877754291" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="145" UseFont="false" />
                </Item6>
                <Item7 Ref="146" ControlType="XRTableCell" Name="xrTableCell41" Weight="0.043281224224828573" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="147" UseFont="false" />
                </Item7>
                <Item8 Ref="148" ControlType="XRTableCell" Name="xrTableCell42" Weight="0.068829433694955047" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="149" UseFont="false" />
                </Item8>
                <Item9 Ref="150" ControlType="XRTableCell" Name="xrTableCell44" Weight="0.082468105956065185" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="151" UseFont="false" />
                </Item9>
                <Item10 Ref="152" ControlType="XRTableCell" Name="xrTableCell45" Weight="0.069812711614798281" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                  <StylePriority Ref="153" UseFont="false" />
                </Item10>
                <Item11 Ref="154" ControlType="XRTableCell" Name="xrTableCell46" Weight="0.067092927303642191" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                  <StylePriority Ref="155" UseFont="false" />
                </Item11>
                <Item12 Ref="156" ControlType="XRTableCell" Name="xrTableCell47" Weight="0.062620946298987581" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Sub Total" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="157" Expression="Sum([SubTotal])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="158" UseFont="false" />
                </Item12>
                <Item13 Ref="159" ControlType="XRTableCell" Name="xrTableCell48" Weight="0.072150041085525529" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Shipping Cost" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="160" Expression="Sum([ShippingCost])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="161" UseFont="false" />
                </Item13>
                <Item14 Ref="162" ControlType="XRTableCell" Name="xrTableCell49" Weight="0.065441357241460524" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Tax" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="163" Expression="Sum([TaxCost])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="164" UseFont="false" />
                </Item14>
                <Item15 Ref="165" ControlType="XRTableCell" Name="xrTableCell50" Weight="0.070456833972785168" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Disc" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="2,2,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="166" Expression="Sum([DiscountAmount])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="167" UseFont="false" />
                </Item15>
                <Item16 Ref="168" ControlType="XRTableCell" Name="xrTableCell51" Weight="0.080878878215053948" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Total Amount" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="169" Expression="Sum([TotalAmount])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="170" UseFont="false" />
                </Item16>
              </Cells>
            </Item1>
          </Rows>
          <StylePriority Ref="171" UseFont="false" />
        </Item1>
      </Controls>
    </Item6>
  </Bands>
  <ExportOptions Ref="172">
    <Xls Ref="173" ShowGridLines="true" />
    <Xlsx Ref="174" ShowGridLines="true" />
    <Csv Ref="175" Separator=";" TextExportMode="Value" />
  </ExportOptions>
  <StyleSheet>
    <Item1 Ref="176" Name="TableHeader" BorderStyle="Inset" Padding="5,5,5,5,100" Font="Verdana, 6.75pt, style=Bold, charSet=0" ForeColor="255,35,27,42" BackColor="LightGray" BorderColor="White" Sides="Top, Bottom" StringFormat="Near;Center;0;None;Character;Default" TextAlignment="MiddleLeft" BorderWidthSerializable="1" />
    <Item2 Ref="177" Name="TableRow" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="255,84,86,89" StringFormat="Near;Near;0;None;Character;Default" TextAlignment="TopLeft" />
    <Item3 Ref="178" Name="EvenStyle" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" BackColor="White" BorderColor="LightGray" Sides="Bottom" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="1" />
    <Item4 Ref="179" Name="OddStyle" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" BackColor="White" BorderColor="LightGray" Sides="Bottom" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="1" />
    <Item5 Ref="180" Name="ReportTitle" BorderStyle="Inset" Font="Verdana, 12pt, style=Bold, charSet=0" ForeColor="White" BackColor="255,60,65,69" BorderColor="255,83,178,77" Sides="Top" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" />
    <Item6 Ref="181" Name="ReportFooterPaging" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="255,84,86,89" BackColor="White" StringFormat="Near;Near;0;None;Character;Default" />
    <Item7 Ref="182" Name="ReportFooterDateTime" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="255,84,86,89" BackColor="White" StringFormat="Near;Near;0;None;Character;Default" />
    <Item8 Ref="183" Name="ReportNameText" BorderStyle="Inset" Font="Candara, 15.75pt, style=Bold, charSet=0" ForeColor="White" BackColor="255,64,64,64" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" />
    <Item9 Ref="184" Name="ReportHeaderTitleText" BorderStyle="Inset" Padding="2,2,2,2,100" Font="Segoe UI, 9.75pt, style=Bold, Italic, charSet=0" StringFormat="Near;Near;0;None;Character;Default" />
    <Item10 Ref="185" Name="ReportHeaderText" BorderStyle="Inset" Padding="2,2,2,2,100" Font="Segoe UI, 12pt, style=Bold, Italic, charSet=0" ForeColor="White" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" />
    <Item11 Ref="186" Name="ReportHeaderTable1" BorderStyle="Inset" Padding="4,4,4,4,100" BorderColor="255,71,180,86" Sides="Left" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" />
    <Item12 Ref="187" Name="ReportHeaderTableRow" BorderStyle="Inset" Padding="4,1,1,1,100" Font="Verdana, 6.75pt" BorderColor="255,71,180,86" Sides="Left" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" />
    <Item13 Ref="188" Name="RightAlignCell" BorderStyle="Inset" Padding="1,4,1,1,100" ForeColor="255,84,86,89" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" />
    <Item14 Ref="189" Name="TableHeaderRightAlignCell" BorderStyle="Inset" Padding="5,5,5,5,100" Font="Verdana, 6.75pt, style=Bold, charSet=0" ForeColor="255,35,27,42" BackColor="LightGray" BorderColor="White" Sides="Top, Bottom" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" BorderWidthSerializable="1" />
  </StyleSheet>
  <ComponentStorage>
    <Item1 Ref="190" ObjectType="DevExpress.DataAccess.ObjectBinding.ObjectDataSource,DevExpress.DataAccess.v17.2" Name="objectDataSource1" Base64="PE9iamVjdERhdGFTb3VyY2U+PE5hbWU+b2JqZWN0RGF0YVNvdXJjZTE8L05hbWU+PERhdGFTb3VyY2UgVHlwZT0iWm5vZGUuTGlicmFyaWVzLkRldkV4cHJlc3MuUmVwb3J0Lk9yZGVyTW9kZWwsIFpub2RlLkxpYnJhcmllcy5EZXZFeHByZXNzLlJlcG9ydCwgVmVyc2lvbj0wLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwiIC8+PC9PYmplY3REYXRhU291cmNlPg==" />
  </ComponentStorage>
  <ObjectStorage>
    <Item1 Ref="2" ObjectType="DevExpress.XtraReports.Parameters.DynamicListLookUpSettings, DevExpress.Printing.v17.2.Core" DataSource="#Ref-191" ValueMember="Value" DisplayMember="Value" />
    <Item2 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="5" Content="System.DateTime" Type="System.Type" />
    <Item3 Ref="8" ObjectType="DevExpress.XtraReports.Parameters.DynamicListLookUpSettings, DevExpress.Printing.v17.2.Core" DataSource="#Ref-192" ValueMember="Value" DisplayMember="Value" />
    <Item4 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="0" Content="~Xtra#NULL" Type="System.Collections.Generic.List`1[[Znode.Engine.Api.Models.ReportOrderDetailsModel, Znode.Engine.Api.Models, Version=9.0.6.0, Culture=neutral, PublicKeyToken=null]]" />
    <Item5 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="191" Content="~Xtra#NULL" Type="System.Collections.Generic.List`1[[Znode.Engine.Api.Models.DevExpressReportParameterModel, Znode.Engine.Api.Models, Version=9.0.6.0, Culture=neutral, PublicKeyToken=null]]" />
    <Item6 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="192" Content="~Xtra#NULL" Type="System.Collections.Generic.List`1[[Znode.Engine.Api.Models.DevExpressReportParameterModel, Znode.Engine.Api.Models, Version=9.0.6.0, Culture=neutral, PublicKeyToken=null]]" />
  </ObjectStorage>
</XtraReportsLayoutSerializer>')
SET IDENTITY_INSERT [dbo].[ZnodeReportSetting] OFF
END
IF NOT EXISTS(SELECT TOP 1 1 FROM ZnodeReportStyleSheets)
BEGIN
SET IDENTITY_INSERT [dbo].[ZnodeReportStyleSheets] ON 
INSERT [dbo].[ZnodeReportStyleSheets] ([StyleSheetId], [StyleSheetXml], [IsDefault], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'<?xml version="1.0" encoding="utf-8"?><StyleSheetSerializer SerializerVersion="17.2.7.0" FileName=""><Styles><Item1 Name="TableHeader" BorderStyle="Inset" Font="Verdana, 6.75pt, style=Bold, charSet=0" ForeColor="#231B2A" BackColor="#D3D3D3" BorderColor="White" StringFormat="Center;Center;0;None;Character;Default" TextAlignment="MiddleLeft" Padding="5,5,5,5,100" Sides="Top,Bottom" BorderWidthSerializable="1.0" /><Item2 Name="TableRow" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" StringFormat="Near;Near;0;None;Character;Default" TextAlignment="TopLeft" ForeColor="#545659" /><Item3 Name="EvenStyle" BorderStyle="Inset" Padding="1,1,1,1,100" BackColor="255,224,224,224" BorderColor="Gray" Sides="Right, Bottom" StringFormat="Near;Near;0;None;Character;Default" BorderWidtSerializable="1.0" /><Item3 Name="EvenStyle" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" BackColor="#FFFFFF" BorderColor="#D3D3D3" Sides="Bottom" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="1" /><Item4 Name="OddStyle" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" BackColor="#FFFFFF" BorderColor="#D3D3D3" Sides="Bottom" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="1" /><Item5 Name="ReportTitle" BorderStyle="Inset" Font="Verdana, 12pt, style=Bold, charSet=0" ForeColor="White" BackColor="#3C4145" BorderColor="#53B24D" Sides="Top" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4.0" /><Item6 Name="ReportFooterPaging" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="#545659" BackColor="#FFFFFF" StringFormat="Near;Near;0;None;Character;Default" /><Item7 Name="ReportFooterDateTime" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="#545659" BackColor="#FFFFFF" StringFormat="Near;Near;0;None;Character;Default" /><Item8 Name="ReportNameText" BorderStyle="Inset" Font="Candara, 15.75pt, style=Bold, charSet=0" ForeColor="White" BackColor="255,64,64,64" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" /><Item9 Name="ReportHeaderTitleText" BorderStyle="Inset" Padding="2,2,2,2,100" Font="Segoe UI, 9.75pt, style=Bold, Italic, charSet=0" StringFormat="Near;Near;0;None;Character;Default" /><Item10 Name="ReportHeaderText" BorderStyle="Inset" Padding="2,2,2,2,100" Font="Segoe UI, 12pt, style=Bold, Italic, charSet=0" ForeColor="White" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" /><Item11 Name="ReportHeaderTable1" BorderStyle="Inset" Padding="4,4,4,4,100" BorderColor="255,71,180,86" Sides="Left" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" /><Item12 Name="ReportHeaderTableRow" BorderStyle="Inset" Padding="4,1,1,1,100" Font="Verdana, 6.75pt" BorderColor="255,71,180,86" Sides="Left" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" /><Item13 Name="RightAlignCell" BorderStyle="Inset" Padding="1,4,1,1,100" StringFormat="Far;Near;0;None;Character;Default" TextAlignment="MiddleRight" ForeColor="#545659"  /><Item14 Name="TableHeaderRightAlignCell" BorderStyle="Inset" Font="Verdana, 6.75pt, style=Bold, charSet=0" ForeColor="#231B2A" BackColor="#D3D3D3" BorderColor="White" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" Padding="5,5,5,5,100" Sides="Top,Bottom" BorderWidthSerializable="1.0"  /></Styles></StyleSheetSerializer>', 1, 2, CAST(N'2018-11-23T15:28:03.120' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.120' AS DateTime))
SET IDENTITY_INSERT [dbo].[ZnodeReportStyleSheets] OFF
END

IF NOT EXISTS(SELECT TOP 1 1 FROM ZnodeSavedReportViews)
BEGIN
SET IDENTITY_INSERT [dbo].[ZnodeSavedReportViews] ON 
INSERT [dbo].[ZnodeSavedReportViews] ([ReportViewId], [UserId], [ReportCode], [ReportName], [LayoutXml], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (1, 2, N'Orders', N'orders', N'<?xml version="1.0" encoding="utf-8"?>
<XtraReportsLayoutSerializer SerializerVersion="17.2.7.0" Ref="1" ControlType="Orders, Znode.Libraries.DevExpress.Report, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" Landscape="true" Margins="25, 27, 29, 42" PaperKind="Custom" PageWidth="1500" PageHeight="827" Version="17.2" RequestParameters="false" DataSource="#Ref-0" Font="Verdana, 6.75pt">
  <Parameters>
    <Item1 Ref="4" Description="Store Name" LookUpSettings="#Ref-2" MultiValue="true" ValueInfo="Fine Foods|Wine &amp; Cheese|Nut Wholesaler|ModernB2C" Name="StoresName" />
    <Item2 Ref="6" Description="Begin Date" ValueInfo="2018-10-01" Name="BeginDate" Type="#Ref-5" />
    <Item3 Ref="7" Description="End Date" ValueInfo="2018-10-31" Name="EndDate" Type="#Ref-5" />
    <Item4 Ref="9" Description="Visible Columns" LookUpSettings="#Ref-8" MultiValue="true" ValueInfo="Store Name|Order No.|Date and Time|Status|Billing Name|Payment Type|Tax|Shipping Cost|Sub Total|Discount|Total Amount" Name="VisibleColumns" />
  </Parameters>
  <Bands>
    <Item1 Ref="10" ControlType="DetailBand" Name="Detail" HeightF="25" TextAlignment="TopLeft" Padding="0,0,0,0,100">
      <Controls>
        <Item1 Ref="11" ControlType="XRTable" Name="xrTable2" SizeF="1448,25" LocationFloat="0, 0" EvenStyleName="EvenStyle" OddStyleName="OddStyle">
          <Rows>
            <Item1 Ref="12" ControlType="XRTableRow" Name="xrTableRow2" Weight="11.5">
              <Cells>
                <Item1 Ref="13" ControlType="XRTableCell" Name="xrTableCell26" Weight="0.068246096482670215" CanShrink="true" Text="xrTableCell26" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="14" Expression="[StoreName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                </Item1>
                <Item2 Ref="15" ControlType="XRTableCell" Name="xrTableCell18" Weight="0.056427902627327151" CanShrink="true" TextTrimming="None" Text="xrTableCell18" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="16" Expression="[OrderNumber]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="17" UseFont="false" />
                </Item2>
                <Item3 Ref="18" ControlType="XRTableCell" Name="xrTableCell19" Weight="0.061983704302449351" CanShrink="true" TextTrimming="None" Text="xrTableCell19" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="19" Expression="[OrderDate]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="20" UseFont="false" />
                </Item3>
                <Item4 Ref="21" ControlType="XRTableCell" Name="xrTableCell20" Weight="0.06060738129186833" CanShrink="true" TextTrimming="None" Text="xrTableCell20" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="22" Expression="[OrderStatus]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="23" UseFont="false" />
                </Item4>
                <Item5 Ref="24" ControlType="XRTableCell" Name="xrTableCell21" Weight="0.07332425447814632" CanShrink="true" TextTrimming="None" Text="xrTableCell21" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="25" Expression="[BillingFirstName] +'' ''+ [BillingLastName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="26" UseFont="false" />
                </Item5>
                <Item6 Ref="27" ControlType="XRTableCell" Name="xrTableCell24" Weight="0.044430920527531552" CanShrink="true" TextTrimming="None" Text="xrTableCell24" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="28" Expression="[ShippingCity]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="29" UseFont="false" />
                </Item6>
                <Item7 Ref="30" ControlType="XRTableCell" Name="xrTableCell25" Weight="0.038953133068130807" CanShrink="true" TextTrimming="None" Text="xrTableCell25" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="31" Expression="[ShippingStateCode]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="32" UseFont="false" />
                </Item7>
                <Item8 Ref="33" ControlType="XRTableCell" Name="xrTableCell27" Weight="0.061946451723105783" CanShrink="true" TextTrimming="None" Text="xrTableCell27" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="34" Expression="[ShippingCountryCode]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="35" UseFont="false" />
                </Item8>
                <Item9 Ref="36" ControlType="XRTableCell" Name="xrTableCell29" Weight="0.074221364001946219" CanShrink="true" TextTrimming="None" Text="xrTableCell29" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="37" Expression="[BillingEmailId]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="38" UseFont="false" />
                </Item9>
                <Item10 Ref="39" ControlType="XRTableCell" Name="xrTableCell6" Weight="0.062831401313047278" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="40" Expression="[ShippingTypeName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="41" UseFont="false" />
                </Item10>
                <Item11 Ref="42" ControlType="XRTableCell" Name="xrTableCell22" Weight="0.060383592656282244" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableRow" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="43" Expression="[PaymentTypeName]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="44" UseFont="false" />
                </Item11>
                <Item12 Ref="45" ControlType="XRTableCell" Name="xrTableCell30" Weight="0.056358764891089728" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell30" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="1,1,1,1,100">
                  <ExpressionBindings>
                    <Item1 Ref="46" Expression="[SubTotal]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="47" UseFont="false" UsePadding="false" />
                </Item12>
                <Item13 Ref="48" ControlType="XRTableCell" Name="xrTableCell31" Weight="0.064935194096351539" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell31" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="49" Expression="[ShippingCost]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="50" UseFont="false" />
                </Item13>
                <Item14 Ref="51" ControlType="XRTableCell" Name="xrTableCell32" Weight="0.058897048605883351" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell32" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="52" Expression="[TaxCost]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="53" UseFont="false" />
                </Item14>
                <Item15 Ref="54" ControlType="XRTableCell" Name="xrTableCell33" Weight="0.063411221826810277" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell33" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="55" Expression="[DiscountAmount]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="56" UseFont="false" />
                </Item15>
                <Item16 Ref="57" ControlType="XRTableCell" Name="xrTableCell34" Weight="0.072790928700972118" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="xrTableCell34" StyleName="RightAlignCell" Font="Verdana, 6.75pt" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="58" Expression="[TotalAmount]" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="59" UseFont="false" />
                </Item16>
              </Cells>
            </Item1>
          </Rows>
        </Item1>
      </Controls>
    </Item1>
    <Item2 Ref="60" ControlType="TopMarginBand" Name="TopMargin" HeightF="29.16667" TextAlignment="TopLeft" Padding="0,0,0,0,100" />
    <Item3 Ref="61" ControlType="BottomMarginBand" Name="BottomMargin" HeightF="41.91666" TextAlignment="TopLeft" Padding="0,0,0,0,100">
      <Controls>
        <Item1 Ref="62" ControlType="XRPageInfo" Name="xrPageInfo1" PageInfo="DateTime" SizeF="501.5417,23" LocationFloat="0, 6.00001" StyleName="ReportFooterDateTime" Padding="2,2,0,0,100" CanPublish="false" />
        <Item2 Ref="63" ControlType="XRPageInfo" Name="xrPageInfo2" TextFormatString="Page {0} of {1}" TextAlignment="TopRight" SizeF="946.4584,23" LocationFloat="501.5417, 6.00001" StyleName="ReportFooterPaging" Padding="2,2,0,0,100" CanPublish="false" />
      </Controls>
    </Item3>
    <Item4 Ref="64" ControlType="ReportHeaderBand" Name="reportHeaderBand1" HeightF="94.37502">
      <SubBands>
        <Item1 Ref="65" ControlType="SubBand" Name="SubBand1" HeightF="290.625" Visible="false">
          <Controls>
            <Item1 Ref="66" ControlType="XRChart" Name="xrChart1" SizeF="1448,290.625" LocationFloat="0, 0" BorderColor="Black" Borders="None">
              <Chart Ref="67" AutoLayout="true" PaletteName="Flow">
                <DataContainer Ref="68">
                  <SeriesSerializable>
                    <Item1 Ref="69" Name="Series 1" ArgumentDataMember="XAxis" ValueDataMembersSerializable="YAxis" LabelsVisibility="True">
                      <Label Ref="70" Position="Top" TypeNameSerializable="SideBySideBarSeriesLabel" BackColor="Window" LineVisibility="True" TextPattern="{V:$0.00}">
                        <Border Ref="71" Visibility="False" />
                      </Label>
                    </Item1>
                  </SeriesSerializable>
                  <SeriesTemplate Ref="72" ToolTipSeriesPattern="{S:$0.00}">
                    <View Ref="73" ExplodeMode="All" TypeNameSerializable="PieSeriesView">
                      <TotalLabel Ref="74" Visible="true" />
                    </View>
                  </SeriesTemplate>
                </DataContainer>
                <Legend Ref="75" Visibility="False" Name="Default Legend" />
                <OptionsPrint Ref="76" ImageFormat="Metafile" />
                <Diagram Ref="77" TypeNameSerializable="XYDiagram">
                  <AxisX Ref="78" VisibleInPanesSerializable="-1" />
                  <AxisY Ref="79" VisibleInPanesSerializable="-1" />
                  <DefaultPane Ref="80" EnableAxisXScrolling="False" EnableAxisYScrolling="False" EnableAxisXZooming="False" EnableAxisYZooming="False">
                    <StackedBarTotalLabel Ref="81">
                      <ConnectorLineStyle Ref="82" />
                    </StackedBarTotalLabel>
                  </DefaultPane>
                </Diagram>
              </Chart>
            </Item1>
          </Controls>
        </Item1>
      </SubBands>
      <Controls>
        <Item1 Ref="83" ControlType="XRTable" Name="xrTable5" SizeF="825,30.00001" LocationFloat="297.9584, 60.38" StyleName="ReportHeaderTable1">
          <Rows>
            <Item1 Ref="84" ControlType="XRTableRow" Name="xrTableRow6" Weight="0.8">
              <Cells>
                <Item1 Ref="85" ControlType="XRTableCell" Name="xrTableCell54" Weight="1" Text="Store Name Equals:" StyleName="ReportHeaderTable1" Font="Verdana, 6.75pt" Padding="2,2,0,0,100" />
              </Cells>
            </Item1>
            <Item2 Ref="86" ControlType="XRTableRow" Name="xrTableRow7" Weight="0.79999999999999993">
              <Cells>
                <Item1 Ref="87" ControlType="XRTableCell" Name="xrTableCell55" Weight="1" Text="Fine Foods, Wine &amp; Cheese, Nut Wholesaler, ModernB2C" StyleName="ReportHeaderTableRow" Padding="2,2,0,0,100" />
              </Cells>
            </Item2>
          </Rows>
        </Item1>
        <Item2 Ref="88" ControlType="XRTable" Name="xrTable4" SizeF="287.3843,29.99998" LocationFloat="9.536743E-05, 60.38" StyleName="ReportHeaderTable1">
          <Rows>
            <Item1 Ref="89" ControlType="XRTableRow" Name="xrTableRow5" Weight="0.8">
              <Cells>
                <Item1 Ref="90" ControlType="XRTableCell" Name="xrTableCell53" Weight="1" Text="Order Date is Between:" StyleName="ReportHeaderTable1" Font="Verdana, 6.75pt" Padding="2,2,0,0,100" />
              </Cells>
            </Item1>
            <Item2 Ref="91" ControlType="XRTableRow" Name="xrTableRow4" Weight="0.79999999999999993">
              <Cells>
                <Item1 Ref="92" ControlType="XRTableCell" Name="xrTableCell52" Weight="1" Text="10/1/2018 12:00:00 AM - 10/31/2018 12:00:00 AM" StyleName="ReportHeaderTableRow" Font="Verdana, 6.75pt" Padding="2,2,0,0,100" />
              </Cells>
            </Item2>
          </Rows>
        </Item2>
        <Item3 Ref="93" ControlType="XRLabel" Name="xrLabel3" Text="ORDERS" TextAlignment="MiddleLeft" SizeF="211.2345,29.24998" LocationFloat="9.999998, 10.00001" StyleName="ReportHeaderText" Padding="2,2,0,0,100" CanPublish="false">
          <StylePriority Ref="94" UseTextAlignment="false" />
        </Item3>
        <Item4 Ref="95" ControlType="XRLabel" Name="xrLabel1" SizeF="1448,50" LocationFloat="0, 0" StyleName="ReportTitle" Padding="2,2,0,0,100" CanPublish="false" />
      </Controls>
    </Item4>
    <Item5 Ref="96" ControlType="GroupHeaderBand" Name="groupHeaderBand1" GroupUnion="WithFirstDetail" RepeatEveryPage="true" HeightF="48.00002">
      <Controls>
        <Item1 Ref="97" ControlType="XRPanel" Name="xrPanel1" SizeF="1448,48" LocationFloat="0, 0">
          <Controls>
            <Item1 Ref="98" ControlType="XRTable" Name="xrTable1" SizeF="1448,28" LocationFloat="7.947286E-06, 20.00001" Font="Verdana, 6.75pt">
              <Rows>
                <Item1 Ref="99" ControlType="XRTableRow" Name="xrTableRow1" Weight="1">
                  <Cells>
                    <Item1 Ref="100" ControlType="XRTableCell" Name="xrTableCell23" Weight="0.075828990991525846" CanShrink="true" Text="Store Name" StyleName="TableHeader" />
                    <Item2 Ref="101" ControlType="XRTableCell" Name="xrTableCell1" Weight="0.062697712896730956" CanShrink="true" TextTrimming="None" Text="Order No." StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="102" UseFont="false" />
                    </Item2>
                    <Item3 Ref="103" ControlType="XRTableCell" Name="xrTableCell2" Weight="0.068870757355785031" CanShrink="true" TextTrimming="None" Text="Date and Time" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="104" UseFont="false" />
                    </Item3>
                    <Item4 Ref="105" ControlType="XRTableCell" Name="xrTableCell3" Weight="0.067341551226841975" CanShrink="true" TextTrimming="None" Text="Status" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="106" UseFont="false" />
                    </Item4>
                    <Item5 Ref="107" ControlType="XRTableCell" Name="xrTableCell4" Weight="0.081471399904093328" CanShrink="true" TextTrimming="None" Text="Billing Name" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="108" UseFont="false" />
                    </Item5>
                    <Item6 Ref="109" ControlType="XRTableCell" Name="xrTableCell7" Weight="0.049367678877754291" CanShrink="true" TextTrimming="None" Text="Shipping City" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="110" UseFont="false" />
                    </Item6>
                    <Item7 Ref="111" ControlType="XRTableCell" Name="xrTableCell8" Weight="0.043281224224828573" CanShrink="true" TextTrimming="None" Text="Shipping State" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="112" UseFont="false" />
                    </Item7>
                    <Item8 Ref="113" ControlType="XRTableCell" Name="xrTableCell10" Weight="0.068829398553931082" CanShrink="true" TextTrimming="None" Text="Shipping Country" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="114" UseFont="false" />
                    </Item8>
                    <Item9 Ref="115" ControlType="XRTableCell" Name="xrTableCell12" Weight="0.082468141097089123" CanShrink="true" TextTrimming="None" Text="Billing Email ID" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="116" UseFont="false" />
                    </Item9>
                    <Item10 Ref="117" ControlType="XRTableCell" Name="xrTableCell5" Weight="0.069812749685619929" CanShrink="true" TextTrimming="None" Text="Shipping Type" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                      <StylePriority Ref="118" UseFont="false" />
                    </Item10>
                    <Item11 Ref="119" ControlType="XRTableCell" Name="xrTableCell9" Weight="0.0670928480258978" CanShrink="true" TextTrimming="None" Text="Payment Type" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                      <StylePriority Ref="120" UseFont="false" />
                    </Item11>
                    <Item12 Ref="121" ControlType="XRTableCell" Name="xrTableCell13" Weight="0.0626208948088319" CanShrink="true" TextTrimming="None" Text="Sub Total" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="122" UseFont="false" />
                    </Item12>
                    <Item13 Ref="123" ControlType="XRTableCell" Name="xrTableCell14" Weight="0.072150218037083561" CanShrink="true" TextTrimming="None" Text="Shipping Cost" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="124" UseFont="false" />
                    </Item13>
                    <Item14 Ref="125" ControlType="XRTableCell" Name="xrTableCell15" Weight="0.065441166705656251" CanShrink="true" TextTrimming="None" Text="Tax" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="126" UseFont="false" />
                    </Item14>
                    <Item15 Ref="127" ControlType="XRTableCell" Name="xrTableCell16" Weight="0.070456918953480663" CanShrink="true" TextTrimming="None" Text="Discount" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="2,2,0,0,100">
                      <StylePriority Ref="128" UseFont="false" />
                    </Item15>
                    <Item16 Ref="129" ControlType="XRTableCell" Name="xrTableCell17" Weight="0.080878807742879977" CanShrink="true" Multiline="true" TextTrimming="None" Text="Total Amount" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                      <StylePriority Ref="130" UseFont="false" />
                    </Item16>
                  </Cells>
                </Item1>
              </Rows>
              <StylePriority Ref="131" UseFont="false" />
            </Item1>
          </Controls>
        </Item1>
      </Controls>
    </Item5>
    <Item6 Ref="132" ControlType="GroupFooterBand" Name="GroupFooter1" HeightF="28">
      <Controls>
        <Item1 Ref="133" ControlType="XRTable" Name="xrTable3" SizeF="1448,28" LocationFloat="0, 0" Font="Verdana, 6.75pt">
          <Rows>
            <Item1 Ref="134" ControlType="XRTableRow" Name="xrTableRow3" Weight="1">
              <Cells>
                <Item1 Ref="135" ControlType="XRTableCell" Name="xrTableCell35" Weight="0.075828990991525846" CanShrink="true" Text="Total" StyleName="TableHeader" />
                <Item2 Ref="136" ControlType="XRTableCell" Name="xrTableCell36" Weight="0.062697712896730956" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="137" UseFont="false" />
                </Item2>
                <Item3 Ref="138" ControlType="XRTableCell" Name="xrTableCell37" Weight="0.068870746676420208" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="139" UseFont="false" />
                </Item3>
                <Item4 Ref="140" ControlType="XRTableCell" Name="xrTableCell38" Weight="0.067341557226725648" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="141" UseFont="false" />
                </Item4>
                <Item5 Ref="142" ControlType="XRTableCell" Name="xrTableCell39" Weight="0.081471404583574478" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="143" UseFont="false" />
                </Item5>
                <Item6 Ref="144" ControlType="XRTableCell" Name="xrTableCell40" Weight="0.049367678877754291" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="145" UseFont="false" />
                </Item6>
                <Item7 Ref="146" ControlType="XRTableCell" Name="xrTableCell41" Weight="0.043281224224828573" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="147" UseFont="false" />
                </Item7>
                <Item8 Ref="148" ControlType="XRTableCell" Name="xrTableCell42" Weight="0.068829433694955047" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="149" UseFont="false" />
                </Item8>
                <Item9 Ref="150" ControlType="XRTableCell" Name="xrTableCell44" Weight="0.082468105956065185" CanShrink="true" TextTrimming="None" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <StylePriority Ref="151" UseFont="false" />
                </Item9>
                <Item10 Ref="152" ControlType="XRTableCell" Name="xrTableCell45" Weight="0.069812711614798281" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                  <StylePriority Ref="153" UseFont="false" />
                </Item10>
                <Item11 Ref="154" ControlType="XRTableCell" Name="xrTableCell46" Weight="0.067092927303642191" CanShrink="true" TextTrimming="None" TextAlignment="MiddleLeft" StyleName="TableHeader" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100" BorderWidth="0">
                  <StylePriority Ref="155" UseFont="false" />
                </Item11>
                <Item12 Ref="156" ControlType="XRTableCell" Name="xrTableCell47" Weight="0.062620946298987581" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Sub Total" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="157" Expression="Sum([SubTotal])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="158" UseFont="false" />
                </Item12>
                <Item13 Ref="159" ControlType="XRTableCell" Name="xrTableCell48" Weight="0.072150041085525529" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Shipping Cost" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="160" Expression="Sum([ShippingCost])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="161" UseFont="false" />
                </Item13>
                <Item14 Ref="162" ControlType="XRTableCell" Name="xrTableCell49" Weight="0.065441357241460524" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Tax" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="163" Expression="Sum([TaxCost])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="164" UseFont="false" />
                </Item14>
                <Item15 Ref="165" ControlType="XRTableCell" Name="xrTableCell50" Weight="0.070456833972785168" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Disc" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="2,2,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="166" Expression="Sum([DiscountAmount])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="167" UseFont="false" />
                </Item15>
                <Item16 Ref="168" ControlType="XRTableCell" Name="xrTableCell51" Weight="0.080878878215053948" CanShrink="true" TextFormatString="{0:$0.00}" TextTrimming="None" Text="Total Amount" StyleName="TableHeaderRightAlignCell" Font="Verdana, 6.75pt, style=Bold" ForeColor="Transparent" BackColor="Transparent" Padding="4,4,0,0,100">
                  <ExpressionBindings>
                    <Item1 Ref="169" Expression="Sum([TotalAmount])" PropertyName="Text" EventName="BeforePrint" />
                  </ExpressionBindings>
                  <StylePriority Ref="170" UseFont="false" />
                </Item16>
              </Cells>
            </Item1>
          </Rows>
          <StylePriority Ref="171" UseFont="false" />
        </Item1>
      </Controls>
    </Item6>
  </Bands>
  <ExportOptions Ref="172">
    <Xls Ref="173" ShowGridLines="true" />
    <Xlsx Ref="174" ShowGridLines="true" />
    <Csv Ref="175" Separator=";" TextExportMode="Value" />
  </ExportOptions>
  <StyleSheet>
    <Item1 Ref="176" Name="TableHeader" BorderStyle="Inset" Padding="5,5,5,5,100" Font="Verdana, 6.75pt, style=Bold, charSet=0" ForeColor="255,35,27,42" BackColor="LightGray" BorderColor="White" Sides="Top, Bottom" StringFormat="Near;Center;0;None;Character;Default" TextAlignment="MiddleLeft" BorderWidthSerializable="1" />
    <Item2 Ref="177" Name="TableRow" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="255,84,86,89" StringFormat="Near;Near;0;None;Character;Default" TextAlignment="TopLeft" />
    <Item3 Ref="178" Name="EvenStyle" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" BackColor="White" BorderColor="LightGray" Sides="Bottom" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="1" />
    <Item4 Ref="179" Name="OddStyle" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" BackColor="White" BorderColor="LightGray" Sides="Bottom" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="1" />
    <Item5 Ref="180" Name="ReportTitle" BorderStyle="Inset" Font="Verdana, 12pt, style=Bold, charSet=0" ForeColor="White" BackColor="255,60,65,69" BorderColor="255,83,178,77" Sides="Top" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" />
    <Item6 Ref="181" Name="ReportFooterPaging" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="255,84,86,89" BackColor="White" StringFormat="Near;Near;0;None;Character;Default" />
    <Item7 Ref="182" Name="ReportFooterDateTime" BorderStyle="Inset" Padding="4,4,4,4,100" Font="Verdana, 6.75pt, charSet=0" ForeColor="255,84,86,89" BackColor="White" StringFormat="Near;Near;0;None;Character;Default" />
    <Item8 Ref="183" Name="ReportNameText" BorderStyle="Inset" Font="Candara, 15.75pt, style=Bold, charSet=0" ForeColor="White" BackColor="255,64,64,64" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" />
    <Item9 Ref="184" Name="ReportHeaderTitleText" BorderStyle="Inset" Padding="2,2,2,2,100" Font="Segoe UI, 9.75pt, style=Bold, Italic, charSet=0" StringFormat="Near;Near;0;None;Character;Default" />
    <Item10 Ref="185" Name="ReportHeaderText" BorderStyle="Inset" Padding="2,2,2,2,100" Font="Segoe UI, 12pt, style=Bold, Italic, charSet=0" ForeColor="White" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" />
    <Item11 Ref="186" Name="ReportHeaderTable1" BorderStyle="Inset" Padding="4,4,4,4,100" BorderColor="255,71,180,86" Sides="Left" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" />
    <Item12 Ref="187" Name="ReportHeaderTableRow" BorderStyle="Inset" Padding="4,1,1,1,100" Font="Verdana, 6.75pt" BorderColor="255,71,180,86" Sides="Left" StringFormat="Near;Near;0;None;Character;Default" BorderWidthSerializable="4" />
    <Item13 Ref="188" Name="RightAlignCell" BorderStyle="Inset" Padding="1,4,1,1,100" ForeColor="255,84,86,89" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" />
    <Item14 Ref="189" Name="TableHeaderRightAlignCell" BorderStyle="Inset" Padding="5,5,5,5,100" Font="Verdana, 6.75pt, style=Bold, charSet=0" ForeColor="255,35,27,42" BackColor="LightGray" BorderColor="White" Sides="Top, Bottom" StringFormat="Far;Center;0;None;Character;Default" TextAlignment="MiddleRight" BorderWidthSerializable="1" />
  </StyleSheet>
  <ComponentStorage>
    <Item1 Ref="190" ObjectType="DevExpress.DataAccess.ObjectBinding.ObjectDataSource,DevExpress.DataAccess.v17.2" Name="objectDataSource1" Base64="PE9iamVjdERhdGFTb3VyY2U+PE5hbWU+b2JqZWN0RGF0YVNvdXJjZTE8L05hbWU+PERhdGFTb3VyY2UgVHlwZT0iWm5vZGUuTGlicmFyaWVzLkRldkV4cHJlc3MuUmVwb3J0Lk9yZGVyTW9kZWwsIFpub2RlLkxpYnJhcmllcy5EZXZFeHByZXNzLlJlcG9ydCwgVmVyc2lvbj0wLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwiIC8+PC9PYmplY3REYXRhU291cmNlPg==" />
  </ComponentStorage>
  <ObjectStorage>
    <Item1 Ref="2" ObjectType="DevExpress.XtraReports.Parameters.DynamicListLookUpSettings, DevExpress.Printing.v17.2.Core" DataSource="#Ref-191" ValueMember="Value" DisplayMember="Value" />
    <Item2 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="5" Content="System.DateTime" Type="System.Type" />
    <Item3 Ref="8" ObjectType="DevExpress.XtraReports.Parameters.DynamicListLookUpSettings, DevExpress.Printing.v17.2.Core" DataSource="#Ref-192" ValueMember="Value" DisplayMember="Value" />
    <Item4 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="0" Content="~Xtra#NULL" Type="System.Collections.Generic.List`1[[Znode.Engine.Api.Models.ReportOrderDetailsModel, Znode.Engine.Api.Models, Version=9.0.6.0, Culture=neutral, PublicKeyToken=null]]" />
    <Item5 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="191" Content="~Xtra#NULL" Type="System.Collections.Generic.List`1[[Znode.Engine.Api.Models.DevExpressReportParameterModel, Znode.Engine.Api.Models, Version=9.0.6.0, Culture=neutral, PublicKeyToken=null]]" />
    <Item6 ObjectType="DevExpress.XtraReports.Serialization.ObjectStorageInfo, DevExpress.XtraReports.v17.2" Ref="192" Content="~Xtra#NULL" Type="System.Collections.Generic.List`1[[Znode.Engine.Api.Models.DevExpressReportParameterModel, Znode.Engine.Api.Models, Version=9.0.6.0, Culture=neutral, PublicKeyToken=null]]" />
  </ObjectStorage>
</XtraReportsLayoutSerializer>', 2, CAST(N'2018-11-23T15:28:03.123' AS DateTime), 2, CAST(N'2018-11-23T15:28:03.123' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[ZnodeSavedReportViews] OFF
END
