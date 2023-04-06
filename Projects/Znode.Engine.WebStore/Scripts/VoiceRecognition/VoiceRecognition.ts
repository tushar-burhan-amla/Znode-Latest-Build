class VoiceRecognitionModel {
    encoder = null;
    wav_format = false;
    windowWeb: any = window;
    result_mode = true ? 'asr' : 'file';
    outfilename_wav = "outputvoice.wav";
    outfilename_flac = "outputvoice.flac";
    navigatorObject: any = window['navigator'];
    samplerate: any = 16000;
    language = 'en-US';
    alternatives = 20;
    google_api_key = "AIzaSyCwtVWGYQq-ddfvjV25NWOGIJGeW50opgE";
    google_api_url = "https://speech.googleapis.com/v1/speech:recognize";
    recording = false;
    stream = null;
    autoSelectSamplerate = true;
    flacdata = { bps: 16, channels: 1, compression: 5 };
    compression = 5;
    audio_context = null;
}
let input = null;
let node = null;
let objVoiceRec: VoiceRecognitionModel = new VoiceRecognitionModel();
class VoiceRecognition extends ZnodeBase {
    constructor() {
        super();
    }
    public startRecording(isChromeSpecific, regionName, callBackFunction) {
        if (isChromeSpecific) {
            ChromeVoiceRecognition.prototype.startRecording(regionName, callBackFunction);
        }
        else {
            VoiceRecognition.prototype.startGoogleSpeechRecording(callBackFunction);
        }
    }

    public startGoogleSpeechRecording(callBackFunction) {
        objVoiceRec.encoder = new Worker('Scripts/lib/VoiceRecognition/encoder.min.js');
        if (objVoiceRec.wav_format == true) {
            objVoiceRec.encoder.postMessage({ cmd: 'save_as_wavfile' });
        }
        objVoiceRec.encoder.onmessage = function (e) {
            VoiceRecognition.prototype.doProcessEncoderMessage(e, callBackFunction);
        };
        if (objVoiceRec.navigatorObject.webkitGetUserMedia)
            objVoiceRec.navigatorObject.webkitGetUserMedia({ video: false, audio: true }, VoiceRecognition.prototype.gotUserMedia, VoiceRecognition.prototype.userMediaFailed);
        else if (objVoiceRec.navigatorObject.mozGetUserMedia)
            objVoiceRec.navigatorObject.mozGetUserMedia({ video: false, audio: true }, VoiceRecognition.prototype.gotUserMedia, VoiceRecognition.prototype.userMediaFailed);
        else
            objVoiceRec.navigatorObject.getUserMedia({ video: false, audio: true }, VoiceRecognition.prototype.gotUserMedia, VoiceRecognition.prototype.userMediaFailed);
    }

    public doProcessEncoderMessage(e, callBackFunction) {
        if (e.data.cmd == 'end') {
            var resultMode = objVoiceRec.result_mode;
            if (resultMode === 'file') {
                var fname = objVoiceRec.wav_format ? objVoiceRec.outfilename_wav : objVoiceRec.outfilename_flac;
                VoiceRecognition.prototype.forceDownload(e.data.buf, fname);
            }
            else if (resultMode === 'asr') {
                if (objVoiceRec.wav_format) {
                    //can only use FLAC format (not WAVE)!
                    console.error('Can only use FLAC format for speech recognition!');
                }
                else {
                    VoiceRecognition.prototype.sendASRRequest(e.data.buf, callBackFunction);
                }
            }
            else {
                console.error('Unknown mode for processing STOP RECORDING event: "' + resultMode + '"!');
            }
            objVoiceRec.encoder.terminate();
            objVoiceRec.encoder = null;
        } else if (e.data.cmd == 'debug') {
            console.log(e.data);
        } else {
            console.error('Unknown event from encoder (WebWorker): "' + e.data.cmd + '"!');
        }
    };

    public stopRecording() {
        if (!objVoiceRec.recording) {
            return;
        }
        console.log('stop recording');
        var tracks = objVoiceRec.stream.getAudioTracks()
        for (var i = tracks.length - 1; i >= 0; --i) {
            tracks[i].stop();
        }
        objVoiceRec.recording = false;
        objVoiceRec.encoder.postMessage({ cmd: 'finish' });
        input.disconnect();
        node.disconnect();
        input = node = null;
    }
    public forceDownload(blob, filename) {
        var url = (objVoiceRec.windowWeb.URL || objVoiceRec.windowWeb.webkitURL).createObjectURL(blob);
        var link = objVoiceRec.windowWeb.document.createElement('a');
        link.href = url;
        link.download = filename || 'output.flac';
        //NOTE: FireFox requires a MouseEvent (in Chrome a simple Event would do the trick)
        var click = document.createEvent("MouseEvent");
        click.initMouseEvent("click", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
        link.dispatchEvent(click);
    }

    public sendASRRequest(blob, callBackFun) {
        // use FileReader to convert Blob to base64 encoded data-URL
        var reader = new objVoiceRec.windowWeb.FileReader();
        reader.readAsDataURL(blob);
        reader.onloadend = function () {
            VoiceRecognition.prototype.googleSpeechAPI(reader.result.replace(/^data:audio\/flac;base64,/, ''), callBackFun);
        }
    }

    public googleSpeechAPI(audioData, callBackFun) {
        //only use base64-encoded data, i.e. remove meta-data from beginning:        
        var data = {
            config: {
                encoding: "FLAC",
                sampleRateHertz: objVoiceRec.samplerate,
                languageCode: objVoiceRec.language,
                maxAlternatives: objVoiceRec.alternatives
            },
            audio: {
                content: audioData
            }
        };
        var oAjaxReq = new XMLHttpRequest();
        oAjaxReq.onload = function () {
            try {
                var result: any = this.responseText;
                var index = 0;
                var i = 0;
                var convertedText = "";
                result = JSON.parse(result);
                var maxconfidence = 0;
                if (result.results.length > 0) {
                    for (index = 0; index < result.results.length; ++index) {
                        for (i = 0; i < result.results[index].alternatives.length; ++i) {
                            if (result.results[index].alternatives[i].confidence > maxconfidence) {
                                maxconfidence = result.results[index].alternatives[i].confidence;
                                convertedText = result.results[index].alternatives[i].transcript;
                            }
                        }
                    }
                }
                if (callBackFun != null)
                    callBackFun(convertedText);
            } catch (exc) {
                console.log('Could not parse result into JSON object: "' + result + '"');
            }
        };
        oAjaxReq.open("post", objVoiceRec.google_api_url + "?key=" + objVoiceRec.google_api_key, true);
        oAjaxReq.setRequestHeader("Content-Type", "application/json");
        oAjaxReq.send(JSON.stringify(data));
    }

    public gotUserMedia(localMediaStream) {
        objVoiceRec.recording = true;
        console.log('success grabbing microphone');
        objVoiceRec.stream = localMediaStream;
        if (typeof objVoiceRec.windowWeb.webkitAudioContext !== 'undefined') {
            objVoiceRec.audio_context = new objVoiceRec.windowWeb.webkitAudioContext;
        } else if (typeof AudioContext !== 'undefined') {
            objVoiceRec.audio_context = new AudioContext;
        }
        else {
            console.error('JavaScript execution environment (Browser) does not support AudioContext interface.');
            console.log('Could not start recording audio:\n Web Audio is not supported by your browser!');

            return;
        }
        input = objVoiceRec.audio_context.createMediaStreamSource(objVoiceRec.stream);

        if (objVoiceRec.windowWeb.input.context.createJavaScriptNode)
            node = objVoiceRec.windowWeb.input.context.createJavaScriptNode(4096, 1, 1);
        else if (objVoiceRec.windowWeb.input.context.createScriptProcessor)
            node = objVoiceRec.windowWeb.input.context.createScriptProcessor(4096, 1, 1);
        else
            console.error('Could not create audio node for JavaScript based Audio Processing.');
        var sampleRate1 = objVoiceRec.audio_context.sampleRate;
        console.log('audioContext.sampleRate: ' + sampleRate1);//DEBUG
        if (objVoiceRec.autoSelectSamplerate) {
            objVoiceRec.samplerate = sampleRate1;
        }
        console.log('initializing encoder with:');//DEBUG
        console.log(' bits-per-sample = ' + objVoiceRec.flacdata.bps);//DEBUG
        console.log(' channels        = ' + objVoiceRec.flacdata.channels);//DEBUG
        console.log(' sample rate     = ' + objVoiceRec.samplerate);//DEBUG
        console.log(' compression     = ' + objVoiceRec.compression);//DEBUG
        objVoiceRec.encoder.postMessage({ cmd: 'init', config: { samplerate: objVoiceRec.samplerate, bps: objVoiceRec.flacdata.bps, channels: objVoiceRec.flacdata.channels, compression: objVoiceRec.compression } });

        node.onaudioprocess = function (e) {
            if (!objVoiceRec.recording)
                return;
            // see also: http://typedarray.org/from-microphone-to-wav-with-getusermedia-and-web-audio/
            var channelLeft = e.inputBuffer.getChannelData(0);
            // var channelRight = e.inputBuffer.getChannelData(1);
            objVoiceRec.encoder.postMessage({ cmd: 'encode', buf: channelLeft });
        };
        input.connect(node);
        node.connect(objVoiceRec.audio_context.destination);
    };
    public userMediaFailed(code) {
        console.log('grabbing microphone failed: ' + code);
    };
}

class ChromeVoiceRecognition extends ZnodeBase {
    public startRecording(regionName, callBackFunction) {
        var recognizer = new objVoiceRec.windowWeb.webkitSpeechRecognition();
        recognizer.lang = regionName;
        recognizer.onresult = function (event) {
            ChromeVoiceRecognition.prototype.doProcessOnResult(event, callBackFunction);
        };
        recognizer.start();
    }
    public doProcessOnResult(event, callBackFunction) {
        if (event.results.length > 0) {
            var result = event.results[event.results.length - 1];
            if (result.isFinal) {
                if (callBackFunction != null) {
                    callBackFunction(result[0].transcript);
                }
            }
        }
    }
}