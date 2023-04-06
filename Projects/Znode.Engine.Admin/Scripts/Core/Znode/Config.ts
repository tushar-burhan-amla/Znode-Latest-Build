module Config {
    export const PaymentScriptUrl = $("#hdnPaymentAppUrl").val() + "/script/znodeapijs";
    export const PaymentScriptUrlForACH = $("#hdnPaymentAppUrl").val() + "/script/znodeapijsforach";
    export const PaymentApplicationUrl = $("#hdnPaymentAppUrl").val() + "/";
    export const PaymentTwoCoUrl = $("#hdnAdminUrl").val() + "/orders/twoco?paymentSettingId=";
}
