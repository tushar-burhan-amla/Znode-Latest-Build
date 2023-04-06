class AttributeFamily extends ZnodeBase {
    _endPoint: Endpoint;
    constructor() {
        super();
        this._endPoint = new Endpoint();
    }
    Init() { }
    DeleteMediaFamily(): any {
        var mediaAttributeFamilyIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (mediaAttributeFamilyIds.length > 0) {
            this._endPoint.DeleteMediaFamily(mediaAttributeFamilyIds, function (res) {
               ZnodeBase.prototype.showDeleteStatus(res);
            });
        }
    }

    Validate(): any {
        var Locales = [];
        $(".LocaleLabel").each(function () {
            Locales.push($(this).attr('localename'));
        });

        var flag = true;
        for (var i = 0; i < Locales.length; i++) {
            var value = $("#Locale" + Locales[i]).val();
            if (value.length > 100) {
                $("#error" + Locales[i]).html(ZnodeBase.prototype.getResourceByKeyName("LocaleError"));
                flag = false;
            }
        }
        return flag;
    }
}