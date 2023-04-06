$.validator.addMethod("code", function (value, element, params) {
    var keywords = params.split(',')
    if (jQuery.inArray(value, keywords) === -1) return true;
    return false;
});
$.validator.unobtrusive.adapters.add("code", [], function (options) {
    options.rules["code"] = "znode,znode9x,admin,znodellc";
    options.messages["code"] = options.message;
});
$(function () {
    jQuery.validator.methods["date"] = function (value, element) { return true; }
});
! function (t) {
    var a = ":input[IsRequired]", code = ":input[input-type=code]", none = ':input[validationrule=""]', alfanumeric = ':input[validationrule="Alphanumeric"]', roundoff = ':input[validationrule="RoundOff"]', url = ":input[validationrule='Url']", email = ":input[validationrule='Email']", i = ":input[MaxCharacters]", r = ":input[AllowDecimals]", s = ":input[AllowNegative]", n = ":input[MaxNumber]", l = ":input[MinNumber]", h = ":input[Extensions]", o = ":input[WYSIWYGEnabledProperty]", m = "data-val-required", u = "data-val-regex", c = "data-val-length", d = "data-val-range", f = "data-msg-extension", b = "data-val-regex-pattern", v = "data-val-length-max", x = "data-val-range-min", p = "data-val-range-max", g = "data-rule-extension", codeatr = "data-val-code";

    t(a).each(function () {
        t(this).attr(m, t('label[for="' + this.name + '"]').html() + " field is required.")
        t('label[for="' + this.name + '"]').addClass('required')
    }), t(alfanumeric).each(function () {
        t(this).attr(u, " Only alphanumeric are allowed in " + t('label[for="' + this.name + '"]').html() + "."), t(this).attr(b, t(this).attr("RegularExpression"))
    }), t(url).each(function () {
        t(this).attr(u, t('label[for="' + this.name + '"]').html() + " field having incorrect URL."), t(this).attr(b, t(this).attr("RegularExpression"))
    }), t(email).each(function () {
        t(this).attr(u, t('label[for="' + this.name + '"]').html() + " field having incorrect Email."), t(this).attr(b, t(this).attr("RegularExpression"))
    }), t(i).each(function () {
        t(this).attr(c, t('label[for="' + this.name + '"]').html() + " exceed max limit of " + t(this).attr("MaxCharacters") + " characters."), t(this).attr(v, t(this).attr("MaxCharacters"))
    }), t(r).each(function () {
        "false" === t(this).attr("AllowDecimals") && "false" === t(this).attr("AllowNegative") && (t(this).attr(u, " Please enter an integer value between 1 and 99999. " ), t(this).attr(b, "^(1|[0-9][0-9]*)$"))
    }), t(s).each(function () {
        "false" === t(this).attr("AllowNegative") && "true" === t(this).attr("AllowDecimals") && (t(this).attr(u, "Only Positive decimal numbers are allowed in the " + t('label[for="' + this.name + '"]').html() + " field(Ex. 1.123456)."), t(this).attr(b, "^[+]?[0-9]{1,12}(?:\\.[0-9]{1,6})?$"))
    }), t(s).each(function () {
        "true" === t(this).attr("AllowNegative") && "true" === t(this).attr("AllowDecimals") && (t(this).attr(u, "Only numbers are allowed in the " + t('label[for="' + this.name + '"]').html() + " field that should not be greater than 12 digits. "), t(this).attr(b, "^[+-]?[0-9]{1,12}(?:\\.[0-9]{1,6})?$"))
    }), t(r).each(function () {
        "false" === t(this).attr("AllowDecimals") && "true" === t(this).attr("AllowNegative") && (t(this).attr(u, "Only Non-Decimal numbers are allowed in the " + t('label[for="' + this.name + '"]').html() + " field."), t(this).attr(b, "^-?[0-9]*$"))
    }), t(n).each(function () {
        t(this).attr(d, t('label[for="' + this.name + '"]').html() + " must be within a range of " + t(this).attr("MinNumber") + " - " + t(this).attr("MaxNumber")), t(this).attr(p, t(this).attr("MaxNumber")), t(this).attr(x, t(this).attr("MinNumber"))
    }), t(h).each(function () {
        t(this).attr(f, t('label[for="' + this.name + '"]').html() + " The FileExtensions field only accepts files with the following extensions: " + t(this).attr("Extensions")), t(this).attr(g, t(this).attr("Extensions"))
    }), t(o).each(function () {
        if (t(this).attr("WYSIWYGEnabledProperty") === "true")
            t("spam[data-valmsg-for='" + t(this).attr("name") + "']").attr("data-valmsg-for", "mceEditor" + t("spam[data-valmsg-for='" + t(this).attr("name") + "']").attr("data-valmsg-for"))
        "true" === t(this).attr("WYSIWYGEnabledProperty") && (t(this).addClass("mceEditor"), t(this).attr("name", "mceEditor" + t(this).attr("name")))
    }), t(none).each(function () {
        if (t(this).attr("RegularExpression") !== "")
            t(this).attr(u, t('label[for="' + this.name + '"]').html() + " is not according to pattern " + t(this).attr("RegularExpression")), t(this).attr(b, t(this).attr("RegularExpression"))
    }), t(roundoff).each(function () {
        t(this).attr(u, t(this).attr("Message")), t(this).attr(b, t(this).attr("RegularExpression"))
    }),
    t(code).each(function () { t(this).attr(codeatr, "System define keyword..") })
    t.validator.setDefaults({
        ignore: ""
    }),
    t.validator.unobtrusive.parse("form")
}($);

