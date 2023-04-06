var Dynamsoft: any;
class BarcodeReader extends ZnodeBase {
    constructor() {
        super();
    }
    _iptIndex: number = 0;
    _scanner = null;

    public LoadBarcodeScannerScript(licenseKey: string, callbackMethod) {
        if ($('script[data-productKeys]').length == 0) {
            let script: any = document.createElement('script');
            script.type = "text/javascript";
            script.src = "https://cdn.jsdelivr.net/npm/dynamsoft-javascript-barcode@7.1.3/dist/dbr.min.js";
            script.onload = callbackMethod;
            script.setAttribute("data-productKeys", licenseKey);
            document.body.appendChild(script);
        }
        callbackMethod();
    }

    public InitiateBarcodeScanner(licenseKey: string, barcodeFormats: string[], UIElement: string, callbackOnLoadMethod, callbackResultMethod) {
        BarcodeReader.prototype.LoadBarcodeScannerScript(licenseKey, function () {
            if (Dynamsoft != undefined) {
                if (BarcodeReader.prototype._scanner != null) {
                    BarcodeReader.prototype._scanner.onUnduplicatedRead = undefined;
                    BarcodeReader.prototype.StopScanner();
                }
                
                Dynamsoft.BarcodeScanner.createInstance().then(s => {
                    BarcodeReader.prototype._scanner = s;
                    BarcodeReader.prototype._iptIndex = 0;
                    BarcodeReader.prototype._scanner.bAddSearchRegionCanvasToResult = true;
                    let runtimeSettings = BarcodeReader.prototype._scanner.getRuntimeSettings();

                    barcodeFormats.forEach((item, index) => {
                        if (index == 0) {
                            runtimeSettings.BarcodeFormatIds = BarcodeReader.prototype.GetBarcodeFormatCode(item);
                        }
                        else {
                            runtimeSettings.BarcodeFormatIds += BarcodeReader.prototype.GetBarcodeFormatCode(item);
                        }
                    });

                    BarcodeReader.prototype._scanner.updateRuntimeSettings(runtimeSettings);
                    if (Dynamsoft.BarcodeReader.isLoaded()) {
                        callbackOnLoadMethod(BarcodeReader.prototype._scanner);
                        console.log('Is the loading completed? ' + Dynamsoft.BarcodeReader.isLoaded());
                        console.log('Index? ' + this._iptIndex);
                        BarcodeReader.prototype._scanner.UIElement = document.getElementById(UIElement);
                        BarcodeReader.prototype._scanner.onFrameRead = results => { };
                        BarcodeReader.prototype._scanner.onUnduplicatedRead = (txt, result) => {
                            console.log('result? ' + result);
                            callbackResultMethod(txt, result);
                            if (3 == ++BarcodeReader.prototype._iptIndex) {
                                this._scanner.onUnduplicatedRead = undefined;
                                BarcodeReader.prototype.StopScanner();
                            }
                        };
                    }
                });
            }
        });
    }
    public StartScanner(callbackSuccess, callbackFailed) {
        if (BarcodeReader.prototype._scanner != null) {
            BarcodeReader.prototype._scanner.show().then(_ => { callbackSuccess() }).catch(error => {
                callbackFailed(error);
            });
        }
    }
    public StartScannerOnElement(UIElement: string, callbackSuccess, callbackFailed) {
        if (BarcodeReader.prototype._scanner != null) {
            BarcodeReader.prototype._scanner.UIElement = document.getElementById(UIElement);
            BarcodeReader.prototype.StartScanner(callbackSuccess, callbackFailed);
        }
    }
    public StopScanner() {
        if (BarcodeReader.prototype._scanner != null) {
            BarcodeReader.prototype._scanner.stop();
            BarcodeReader.prototype._scanner.hide();
        }
    }
    public PauseScanner() {
        if (BarcodeReader.prototype._scanner != null) {
            BarcodeReader.prototype._scanner.pause();
        }
    }
    public GetBarcodeFormatCode(Code): any {
        var barcodeFormatCode;
        switch (Code) {
            case "ONED": {
                barcodeFormatCode = Dynamsoft.EnumBarcodeFormat.OneD;
                break;
            }
            case "QR_CODE": {
                barcodeFormatCode = Dynamsoft.EnumBarcodeFormat.QR_CODE;
                break;
            }
            case "CODABAR": {
                barcodeFormatCode = Dynamsoft.EnumBarcodeFormat.CODABAR;
                break;
            }
            default: {
                barcodeFormatCode = Dynamsoft.EnumBarcodeFormat.QR_CODE;
                break;
            }
        }
        return barcodeFormatCode;
    }
}