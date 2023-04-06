class CollapsibleGrid extends ZnodeBase {
    _attributeId: number;
    _familyId: number;
    _url: string;
    _endpoint: Endpoint;

    constructor() {
        super();
        this._endpoint = new Endpoint();
    }

    public HideShowExpandOption(control: any, area: string, controller: string, action: string): void {
		$(control).find("em").toggleClass("z-add z-minus");
        $(control).closest("thead").next("tbody .attributeData").toggle();

		if ($(control).find("em").hasClass("z-minus")) {
            this._url = "/" + area + "/" + controller + "/" + action;
            this._attributeId = <number>$(control).data("attributegroupid");
            this._familyId = <number>$(control).data("attributefamilyid");
            ZnodeBase.prototype.ShowLoader();
            this._endpoint.GetAssociatedAttributes(this._url, this._attributeId,this._familyId, function (response) {
                if (response) {
                    $(control).closest("thead").next("tbody .attributeData").html(response).show();
                    ZnodeBase.prototype.HideLoader();
                }
                else {
                    $(control).closest("thead").next("tbody .attributeData").html("No attributes associated to be displayed.").show();
                    ZnodeBase.prototype.HideLoader();
                }
            });
        }
    }
}