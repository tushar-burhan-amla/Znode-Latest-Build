var bLazy: any;
var ScannerObject;
class Home extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {
        $(document).ready(function () {
            $(".product-list-widget .owl-next").off("click");
            $(".product-list-widget .owl-next").on("click", Home.prototype.loadImages);
        });
    }

    loadImages(): void {
        var productParent = $(this).parentsUntil('.product-list-widget');
        var unloadedElements = productParent ? productParent.find(".b-lazy:not(.b-loaded)") : null;
        if (unloadedElements && unloadedElements.length > 0)
            bLazy.load($(unloadedElements));
    }

    ValidationForEmailID(): any {
        $("#newslettererrormessage").removeClass();
        var signUpEmail: string = $("#txtNewsLetterSignUp").val();
        var pattern = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

        if (signUpEmail != null && signUpEmail != "") {
            if (!pattern.test(signUpEmail)) {
                $("#newslettererrormessage").html(ZnodeBase.prototype.getResourceByKeyName("ErrorEmailAddress"))
                $("#newslettererrormessage").addClass("error-msg");
                $("#newslettererrormessage").show();
                return false;
            }
            else {
                $("#newslettererrormessage").html('');
                $("#newslettererrormessage").removeClass("error-msg");
                $("#newslettererrormessage").hide();
                Endpoint.prototype.SignUpForNewsLetter(signUpEmail, function (response) {
                    if (response.sucess) {
                        $("#txtNewsLetterSignUp").val('');
                        $("#newslettererrormessage").addClass("success-msg");
                        $("#newslettererrormessage").show().html(response.message);
                    } else {
                        $("#newslettererrormessage").addClass("error-msg");
                        $("#newslettererrormessage").show().html(response.message);
                    }
                });
            }
        }
        else {
            $("#newslettererrormessage").html(ZnodeBase.prototype.getResourceByKeyName("RequiredEmailId"))
            $("#newslettererrormessage").addClass("error-msg");
            $("#newslettererrormessage").show();
            return false;
        }
    }

    //Get Cart Count for Donut Caching
    public GetCartCount(): any {
        Endpoint.prototype.GetCartCount(function (response) {
            return $(".cartcount").val(response);
        });
    }

    //load barcode scanner
    LoadBarcodeScanner(): any {
        let licenseKey: string = "";
        let UIElement: string = "div-video-container";
        let publishProductId: number = 0;
        let seoURL: string = "";
        let barcodeFormats: string[];
        let enableSpecificSearch: boolean = false;
        $('#quick-view-content').html("<span style='position:absolute;top:0;bottom:0;left:0;right:0;text-align:center;transform:translate(0px, 45%);font-weight:600;'>Loading...</span>");
        $(".quick-view-popup").first().modal('show');
        $(".quick-view-popup .modal-content").css('min-height', '10vw');
        $(".quick-view-popup .modal-content").css('max-width', '75vw');
        $(".quick-view-popup .modal-content").css('margin', '0 auto');
        Endpoint.prototype.GetBarcodeScanner(function (res) {
            if (res != null && res != "") {
                $("#quick-view-content").html(res);
                licenseKey = $("#quick-view-content").find("#LicenseKey").val();
                barcodeFormats = $("#quick-view-content").find("#BarcodeFormates").val().split(',');
                BarcodeReader.prototype.InitiateBarcodeScanner(licenseKey, barcodeFormats, UIElement, function (scanner) {
                    ScannerObject = scanner;
                    BarcodeReader.prototype.StartScannerOnElement(UIElement, function () { }, function (error) {
                        $('#quick-view-content').html('');
                        $('#quick-view-content').html("<span style='position:absolute;top:0;bottom:0;left:0;right:0;text-align:center;transform:translate(0px, 45%);font-weight:600;'>" + error + "</span>");
                    });
                }, function (scanText, result) {
                    $("#lblScannerCode").html(scanText);
                    BarcodeReader.prototype.PauseScanner();
                    enableSpecificSearch = $("#quick-view-content").find("#EnableSpecificSearch").val().toLocaleLowerCase();
                    Endpoint.prototype.GetProductDetail(scanText, enableSpecificSearch, function (productResponse) {
                        if (productResponse != null && productResponse != "") {
                            try {

                                var jsonPublishProduct = JSON.parse(productResponse);
                                if (jsonPublishProduct.Type == "success") {
                                    publishProductId = jsonPublishProduct.Data.PublishProductId;
                                    seoURL = jsonPublishProduct.Data.SEOUrl;
                                    $(".quick-view-popup").first().modal('hide');
                                    BarcodeReader.prototype.StopScanner();
                                    window.location.href = (seoURL != null && seoURL.length > 0) ? seoURL : "/product/" + publishProductId;
                                }
                                else {
                                    $("#lblScannerCode").html(" " + jsonPublishProduct.Message);
                                }
                            } catch (e) {
                                $("#lblScannerCode").html(" " + ZnodeBase.prototype.getResourceByKeyName("BarcodeInvalidMessage"));
                            }
                        }
                        else {
                            $("#lblScannerCode").html(" " + ZnodeBase.prototype.getResourceByKeyName("BarcodeInvalidMessage"));
                        }
                    });
                });
            }
            else {
                $("#quick-view-content").html(ZnodeBase.prototype.getResourceByKeyName("BarcodeLoadErrorMessage"));
            }
        });
        $('.quick-view-popup').on('hidden.bs.modal', function (e) {
            $(".quick-view-popup .modal-content").removeAttr('style');
            BarcodeReader.prototype.StopScanner();
        });
    }

    DownloadMediaByMediaId(mediaId: number): any {
        if (mediaId > 0) {
            Endpoint.prototype.DownloadMediaById(mediaId, function (res) {
                if (res != null && res != "") {
                    Home.prototype.SaveToDisk(res.status.MediaServerPath, res.status.FileName)
                }
            });
        }
    }
    DownloadMediaByGuid(guid: string): any{
        if (guid != null) {
            Endpoint.prototype.DownloadMediaByGuid(guid, function (res) {
                if (res != null && res != "") {
                    Home.prototype.SaveToDisk(res.status.MediaServerPath, res.status.FileName)
                }
            });
        }
    }
    SaveToDisk(url: string, fileName: string): any {
        fetch(url)
            .then(response => response.blob())
            .then(blob => {
                const link = document.createElement("a");
                link.href = URL.createObjectURL(blob);
                link.download = fileName;
                link.click();
            })
            .catch(console.error);
    }
}