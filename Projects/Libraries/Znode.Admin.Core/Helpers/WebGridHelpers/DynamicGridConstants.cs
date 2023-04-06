namespace Znode.Engine.Admin.Helpers
{
    public struct DynamicGridConstants
    {
        public const string ViewMode = "viewmode";
        public const string Date = "Date";
        public const string SmallDate = "date";
        public const string Time = "Time";
        public const string DateTime = "DateTime";
        public const string DateFormat = "dd-MMM-yyyy";
        public const string DefaultText = "All";
        public const string Yes = "y";
        public const string No = "n";
        public const string EditKey = "Edit";
        public const string DeleteKey = "Delete";
        public const string ViewKey = "View";
        public const string ImageKey = "Image";
        public const string MediaKey = "Media";
        public const string CopyKey = "Copy";
        public const string ManageKey = "Manage";
        public const string CheckboxKey = "Checkbox";
        public const string ControlKey = "Control";
        public const string IsLinkKey = "IsLink";
        public const string columnKey = "column";
        public const string nameKey = "name";
        public const string headertextKey = "headertext";
        public const string datatypeKey = "datatype";
        public const string isallowsearchKey = "isallowsearch";
        public const string isvisibleKey = "isvisible";
        public const string editactionurlKey = "editactionurl";
        public const string editparamfieldKey = "editparamfield";
        public const string deleteactionurlKey = "deleteactionurl";
        public const string deleteparamfieldKey = "deleteparamfield";
        public const string viewactionurlKey = "viewactionurl";
        public const string viewparamfieldKey = "viewparamfield";
        public const string imageparamfieldKey = "imageparamfield";
        public const string manageactionurlKey = "manageactionurl";
        public const string manageparamfieldKey = "manageparamfield";
        public const string copyactionurlKey = "copyactionurl";
        public const string copyparamfieldKey = "copyparamfield";
        public const string sortKey = "sort";
        public const string sortDirKey = "sortdir";
        public const string RecordPerPageKey = "recordperpage";
        public const string PageKey = "page";
        public const string DESCKey = "DESC";
        public const string ASCKey = "ASC";
        public const string Hav = "h";
        public const string xmlConfigDocSessionKey = "xmlConfigDoc";
        public const string NewRowColumns = "newRowColumns";
        public const string AddNewRowData = "AddNewRowData";
        public const string IsResourceRequired = "IsResourceRequired";
        public const string XMLStringSessionKey = "XMlstring";
        public const string listTypeSessionKey = "listType";
        public const string ColumnListSessionKey = "ColumnList";
        public const string IsPaging = "IsPaging";
        public const string AdvanceFilterCollectionKey = "AdvanceFilterCollection";
        public const string checkboxactionurlKey = "checkboxactionurl";
        public const string checkboxparamfieldKey = "checkboxparamfield";
        public const string displaytextKey = "displaytext";
        public const string ischeckboxKey = "ischeckbox";
        public const string isallowlinkKey = "isallowlink";
        public const string islinkactionurlKey = "islinkactionurl";
        public const string islinkparamfieldKey = "islinkparamfield";
        public const string iscontrolKey = "iscontrol";
        public const string controlparamfieldKey = "controlparamfield";
        public const string IsVisible = "IsVisible";
        public const string ColumnListWithNewSessionKey = "ColumnListWithNewSessionKey";
        public const string GridViewMode = "GridViewMode";
        public const string FolderId = "folderid";
        public const string FilterState = "FilterState";

        public const string controltypeKey = "controltype";

        public const string CheckboxHtml = "<input type=\"checkbox\" name=\"{1}\" id=\"rowcheck_{0}\" class=\"grid-row-checkbox {2}\">  <span class=\"lbl padding-8 {2}\"></span>";
        public const string DeleteHtml = "<a id=\"spanDelete_{3}\" href=\"javascript:void(0);\" class=\"z-delete active-icon\" title=\"Delete\"  onclick=\"CommonHelper.BindDeleteConfirmDialog('{1}','{2}','{0}')\"></a>";
        public const string EditHtml = "<a href=\"{0}\"  class=\"z-edit active-icon\" title=\"Edit\"></a>";
        public const string ViewHtml = "<a href=\"{0}\" class=\"z-view active-icon\" title=\"View\"></a>";
        public const string ManageHtml = "<div>" +
                                            "<a href=\"#\" class='action-link'>" +
                                                "<i class='dg-manage font-20 light-grey-text-color' data-toggle='tooltip' data-placement='top' title='{1}'></i>" +
                                           " </a>" +
                                           "<ul class='action-ui'>" +
                                                    "{0}" +
                                           "</ul>" +
                                        "</div>";
        public const string TileManageHtml = "<ul class='action-ui'>{0}</ul>";

        public const string CopyHtml = "<a href=\"{0}\" class=\"z-copy active-icon\" title=\"Copy\"></a>";
        public const string ImageHtml = "<img src=\"{0}\" class=\"grid-img\"/>";
        public const string LinkHtml = "<a href=\"{0}{1}\">{3}</a>";
        public const string DynamicLinkHtml = "<a href='#' onclick='javascript:DynamicGrid.prototype.DynamicPartialLoad(\"{0}{1}\")'>{2}</a>";
        public const string DisableHtml = "<a id=\"spanDisable_{4}\" title='{5}' href=\"javascript:void(0);\" class=\"z-{3} active-icon\"  onclick=\"CommonHelper.BindDeleteConfirmDialog('{1}','{2}','{0}')\"></a>";
        public const string IconTrue = "<i class='z-active'></i>";
        public const string IconFalse = "<i class='z-inactive'></i>";
        public const string ControlHtml = "<input type=\"{0}\" id=\"control_{2}\" value=\"{1}\"/>";
        public const string SubgridHtml = "<input id=\"recored-id\" type=\"hidden\" value=\"{0}\"/><input id=\"type-name\" type=\"hidden\" value=\"{1}\"/><input id=\"method-name\" type=\"hidden\" value=\"{2}\"/>";

        public const string FieldsCollectionSessionKey = "FieldsCollection";
        public const string OperatorsCollectionSessionKey = "OperatorsCollection";
        public const string ValuesCollectionSessionKey = "ValuesCollection";
        public const string RelativePathSessionKey = "relativePath";
        public const string FilterCollectionsSessionKey = "FilterCollections";
        public const string SortCollectionSessionKey = "SortCollection";
        public const string IsViewRequest = "IsViewRequest";
        public const string DisableKey = "Disable";
        public const string enabledisableactionurlKey = "enabledisableactionurl";
        public const string enabledisableparamfieldKey = "enabledisableparamfield";
        public const string OperatorXmlSettingSessionKey = "OperatorXmlSetting";

        //Data Types
        public const string StringKey = "string";
        public const string Int16Key = "int16";
        public const string Int32Key = "int32";
        public const string Int64Key = "int64";
        public const string BooleanKey = "boolean";
        public const string DateTimeKey = "datetime";
        public const string DecimalKey = "decimal";
        public const string DoubleKey = "double";
        public const string SingleKey = "single";

        //Operators
        public const string IsOperator = "is";
        public const string BeginswithOperator = "begins with";
        public const string EndswithOperator = "ends with";
        public const string ContainsOperator = "contains";
        public const string EqualsOperator = "equals";
        public const string GreaterthanOperator = "greater than";
        public const string GreaterorequalOperator = "greater or equal";
        public const string LessthanOperator = "less than";
        public const string LessorequalOperator = "less or equal";
        public const string OnOperator = "on";
        public const string OnOrBeforeOperator = "on or before";
        public const string AfterOperator = "after";
        public const string BeforeOperator = "before";
        public const string OnOrAfterOperator = "on or after";
        public const string NotOnOperator = "not on";

        //oprator XML name
        public const string OperatorXML = "OperatorXML";

        //default filter for data oprator id
        public const string DataOperatorId = "DataOperatorId";

        //Css Class Name
        public const string ClassCenter = "center-align";
        public const string ClassRight = "right-align";
        public const string OperatordefinitionKey = "operatordefinition";

        #region View
        public const string ListBoxView = "~/Views/XMLGenerator/_ListViewBox.cshtml";
        public const string CreateXMLView = "~/Views/XMLGenerator/CreateXML.cshtml";
        public const string GridColumnSetting = "~/Views/XMLGenerator/_GridColumnSettings.cshtml";
        #endregion
        public const string DeleteSuccessMsg = "XML configuration settings deleted successfully.";
        public const string DeleteErrorMsg = "Unable to delete XML configuration settings.";
        public const string Success = "success";
        public const string Fail = "fail";
        public const string Error = "error";

        public const string BulkAddColumnListSessionKey = "BulkAddColumnList";
        public const string RadioKey = "Radio";
        public const string ButtonKey = "Button";
        public const string DropDownKey = "DropDown";
        public const string RuntimeHtmlKey = "RuntimeHtml";
        public const string TextKey = "Text";
        public const string HiddenFieldKey = "HiddenField";
        public const string MultiCheckboxListKey = "MultiCheckboxList";
        public const string LabelKey = "Label";
        public const string DynamicHtmlKey = "DynamicHtml";
        public const string RowWiseRadioKey = "RowRadio";
        public const string MulticheckBoxListKey = "CheckBoxList";
        public const string DropDownListKey = "List";
        public const string RuntimeCodeKey = "RuntimeCode";
        public const string SearchControlTypeKey = "SearchControlType";
        public const string SearchControlParametersKey = "SearchControlParameters";
        public const string DynamicHtml = "<div data-columnname='{1}' data-controltype='DynamicHtml'>{0}</div>";
        public const string LabelHtml = "<label data-columnname='{1}'>{0}</label>";
        //public const string InlineEditHtml = "<label data-dgview='show' data-columnname='{1}' style='!imporant;display:none;'>{0}</label> <input type='text' data-dgview='edit' style='width:150px !imporant;display:block;' data-columnname='{1}' maxlength={2} value='{0}' class=\"input-text\"/>";
        public const string InlineEditHtml = "<label data-dgview='show' data-columnname='{1}'>{0}</label> <input type='text' data-dgview='edit' style='width:{3} !imporant;display:none;' data-columnname='{1}' maxlength={2} value=\"{0}\" class=\"input-text\"/>";
        public const string HidenfieldHtml = "<input type='hidden' data-dgview='edit' data-columnname='{1}' value='{0}'/>";
        public const string RowRadioHtml = "<label><input type=\"radio\" name=\"rowRadio_{2}\" data-id='{2}' {3} value=\"{0}\" data-columnname='{1}' class=\"radio-btn\"/ ><span class=\"lbl padding-8\"></span></label>";
        public const string RadioHtml = "<label><input type=\"radio\" name=\"radio_{1}\" data-id='{2}' {3} value=\"{0}\" data-columnname='{1}' class=\"radio-btn\"/ ><span class=\"lbl padding-8\"></span></label>";
        public const string ButtonHtml = "<input type=\"button\" data-dgbtn='button' class=\"{0}\" name=\"{1}\" value=\"{2}\"  data-parametervalue='{3}' data-columnname='{1}' />";
        public const string ImageActionLinkHtml = "<a href=\"{1}\"><img src=\"{0}\" class=\"grid-img\"/></a>";
        public const string ManageChildHtml = "<li ><a class='{0}' {1} data-parameter='{2}' data-managelink=\"{3}\" title='{4}' {5} {6}></a></li>";
        public const string DownloadChildHtml = "<li ><a class='{0}' {1} data-parameter='{2}' data-managelink=\"{3}\" title='{4}' onclick=\"Theme.prototype.Reload();\"></a></li>";
        public const string TileViewManageChildHtml = "<li><a class='{0}' href=\"{1}{2}\" data-parameter='{2}'  data-managelink=\"{3}\" ></a></li>";
        public const string SelectedcheckBoxList = "Selected";
        public const string GridCheckbox = "grid-checkbox";
        public const string GridAction = "grid-action";

        //Use Mode
        public const string Custom = "Custom";
        public const string DataBase = "DataBase";

        public const string DateFormatOfDatabase = "yyyy-MM-dd";
        public const string ManageSearch = "ManageSearch";
        public const string ListViewsCollection = "ListViewsCollection";
        #region XML Editor
        public const string IdKey = "id";
        public const string FormatKey = "format";
        public const string ColumntypeKey = "columntype";
        public const string AllowsortingKey = "allowsorting";
        public const string AllowpagingKey = "allowpaging";
        public const string MustshowKey = "mustshow";
        public const string MusthideKey = "musthide";
        public const string MaxlengthKey = "maxlength";
        public const string IsconditionalKey = "isconditional";
        public const string WidthKey = "width";
        public const string imageactionurlKey = "imageactionurl";
        public const string xaxis = "xaxis";
        public const string yaxis = "yaxis";
        public const string isadvancesearch = "isadvancesearch";

        public const string DisplayOpionErrReq = "Please enter display options.";
        public const string PageErrReq = "Please enter front end page name.";
        public const string ObjectErrReq = "Please enter front end object name.";
        public const string EntityNameErrReq = "Please enter entity name.";
        public const string EditSuccessMsg = "XML configuration settings updated successfully.";
        public const string EditErrorMsg = "Unable to update XML configuration settings.";
        public const string AddSuccessMsg = "XML configuration settings saved successfully.";
        public const string AddErrorMsg = "Unable to save XML configuration settings.";//

        public const string AutoFillTextKey = "AutoFillText";
        public const string DateTextKey = "DateText";
        public const string NoTextKey = "NoText";
        public const string StringTextKey = "StringText";
        public const string EmailTextKey = "EmailText";
        public const string ColorTextKey = "ColorText";
        public const string ClassKey = "Class";
        public const string DbParamFieldKey = "DbParamField";
        public const string UseMode = "useMode";
        public const string IsGraphKey = "IsGraph";
        public const string isallowdetailKey = "allowdetailview";

        //media family constant.
        public const string Video = "video";
        public const string Audio = "audio";
        public const string File = "file";

        public const string RemovedFilters = "RemovedFilters";
        public const string VideoTag = "<div class='tile-view-container' onmouseover='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{0}\",false)' onMouseOut='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{1}\",true)'><div class=\"img\" data-rowcheckid=\"rowcheck_{2}\" title=\"Click here to select\"><i class='z-video'> </i> <input id='hiddensrc' type='hidden' value=\"{3}\" /> <label><input id=\"rowcheck_{4}\" name=\"{5}\" type=\"checkbox\" class=\"grid-row-checkbox\" style=\"display:{6}\" onclick=\"DynamicGrid.prototype.ShowHideTileOverlay(this)\"></input><span class=\"lbl padding-8\" style=\"display:{7}\"></span><input id='hiddenFamily' type='hidden' value=\"Video\" /></label><input type=\"hidden\" id=\"hdnSize_{8}\" value=\"{9}\" /> </div><div class=\"title\">{10}</div><div id='Tile{11}' class=\"tile-action\">{12}</div></div>";
        public const string AudioTag = "<div class='tile-view-container' onmouseover='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{0}\",false)' onMouseOut='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{1}\",true)'><div class=\"img\" data-rowcheckid=\"rowcheck_{2}\" title=\"Click here to select\"><i class='z-audio'></i> <input id='hiddensrc' type='hidden' value=\"{3}\" /> <label><input id=\"rowcheck_{4}\" name=\"{5}\" type=\"checkbox\" class=\"grid-row-checkbox\" style=\"display:{6}\" onclick=\"DynamicGrid.prototype.ShowHideTileOverlay(this)\"></input><span class=\"lbl padding-8\" style=\"display:{7}\"></span><input id='hiddenFamily' type='hidden' value=\"Audio\" /></label><input type=\"hidden\" id=\"hdnSize_{8}\" value=\"{9}\" /></div><div class=\"title\">{10}</div><div id='Tile{11}' class=\"tile-action\">{12}</div></div>";
        public const string FileTag = "<div class='tile-view-container' onmouseover='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{0}\",false)' onMouseOut='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{1}\",true)'><div class=\"img\" data-rowcheckid=\"rowcheck_{2}\"  title=\"Click here to select\"><i class='z-file-text'></i> <input id='hiddensrc' type='hidden' value=\"{3}\" /> <label><input id=\"rowcheck_{4}\" name=\"{5}\" type=\"checkbox\" class=\"grid-row-checkbox\" style=\"display:{6}\" onclick=\"DynamicGrid.prototype.ShowHideTileOverlay(this)\"></input><span class=\"lbl padding-8\" style=\"display:{7}\"></span><input id='hiddenFamily' type='hidden' value=\"File\" /></label><input type=\"hidden\" id=\"hdnSize_{8}\" value=\"{9}\" /></div><div class=\"title\">{10}</div><div id='Tile{11}' class=\"tile-action\">{12}</div></div>";
        public const string ImageTag = "<div class='tile-view-container' onmouseover='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{0}\",false)' onMouseOut='DynamicGrid.prototype.ShowHideTileContext(\"#Tile{1}\",true)'><div class=\"img\" data-rowcheckid=\"rowcheck_{2}\" title=\"Click here to select\"><input id='hiddensrc' type='hidden' value=\"{3}\"/><img class=\"img-responsive\" src=\"{4}\"></img><label><input id=\"rowcheck_{5}\" name=\"{6}\" type=\"checkbox\" class=\"grid-row-checkbox\" style=\"display:{7}\" onclick=\"DynamicGrid.prototype.ShowHideTileOverlay(this)\"></input><span class=\"lbl padding-8\" style=\"display:{8}\"></span><input id='hiddenFamily' type='hidden' value=\"Image\" /></label><input type=\"hidden\" id=\"hdnSize_{9}\" value=\"{10}\" /> </div><div class=\"title\" title=\"{11}\">{11}</div><div id=Tile{12} class=\"tile-action\">{13}</div></div>";

        #endregion

        #region Global Search Constants
        public const string GlobalSearchFilter = "GlobalSearchFilter";
        public const string ProductFilter = "Product";
        public const string OrderFilter = "Order";
        public const string CatalogFilter = "Catalog";
        public const string CategoryFilter = "Category";
        public const string UserFilter = "User";
        #endregion
    }
}