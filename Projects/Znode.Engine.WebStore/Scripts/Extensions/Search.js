CustomHome.prototype.newFunction = function () {
    alert("using prototype add function in existing class");
}

CustomHome["newFunction2"] = function () {
    alert("without prototype add function in existing class");
}