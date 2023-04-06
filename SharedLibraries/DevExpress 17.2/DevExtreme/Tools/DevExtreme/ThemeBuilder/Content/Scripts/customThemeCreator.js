(function(ThemeBuilder) {
    "use strict";

    var CustomThemeCreator = function(baseColor){
        var dxBaseColor = new DevExpress.Color(baseColor);
        this.getColor = function (tint) {
            if(tint === "transparent")
                return "transparent";
            var dxTint = new DevExpress.Color(tint),
                newTint = new DevExpress.Color("hsl(" + [dxBaseColor.hsl.h, dxBaseColor.hsl.s, dxTint.hsl.l].join(",") + ")");

            return dxTint.a === 1 ? newTint.toHex() :
                "rgba(" + [newTint.r, newTint.g, newTint.b, dxTint.a].join(",") + ")";
        };
    };

    ThemeBuilder.CustomThemeCreator = CustomThemeCreator;

})(ThemeBuilder);
