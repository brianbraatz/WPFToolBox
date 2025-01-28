///////////////////////////////////////////////////////////////////////////////
//
//  Silverlight.js   version 0.9
//
//  This file is provided by Microsoft as a helper file for websites that
//  incorporate Silverlight Objects.  It must be used in conjunction with createSilverlight.js, 
//  or alternatively, a custom .js file specific to your site.  The 0.9 version of this file is 
//  hard coded to match Microsoft Silverlight v1.0 Beta, which exposes 0.9 as its version number.     
//  This file is provided as is.
//
///////////////////////////////////////////////////////////////////////////////
if (!window.Sys) 
{
   window.Sys = { };
}
if (!window.Sys.Silverlight) 
{
    window.Sys.Silverlight = { };
}
//////////////////////////////////////////////////////////////////
// isInstalled, checks to see if the correct version is installed
//////////////////////////////////////////////////////////////////
Sys.Silverlight.isInstalled = function(version)
{
 
    var uaString = navigator.userAgent;
    var reqVersionArray = version.split(".");
    reqMajorVer = (reqVersionArray[0] != null) ? reqVersionArray[0] : 0;
    reqMinorVer = (reqVersionArray[1] != null) ? reqVersionArray[1] : 9;
    reqBuildVer = (reqVersionArray[2] != null) ? reqVersionArray[2] : 0;
      
    
    function detectAgControlVersion()
    {
        agVersion = -1;       

        if ((navigator.plugins != null) && (navigator.plugins.length > 0))
        {        
		if (document.getElementById && !document.all && navigator.plugins["WPFe Plug-In"] )
		{
			if (navigator.userAgent.indexOf("Firefox") != -1)
			{
				var tmpAgObjectTag = '<object id="tmpSilverlightVersion" width="1" height="1" type="application/ag-plugin"/>';
				range = document.createRange();	
				range.selectNode(document.body);			
				range.setStartBefore(document.body);
				tmpAgControlDiv = document.createElement('DIV');
				document.body.appendChild(tmpAgControlDiv);
				tmpAgControlDiv.innerHTML=tmpAgObjectTag;
				agVersionElement=document.getElementById("tmpSilverlightVersion");
				agVersion=agVersionElement.settings.version;
				document.body.removeChild(tmpAgControlDiv);
			}
			else
			{
				agVersion = navigator.plugins["WPFe Plug-In"].description;
			}
		}
        }
        else if ((navigator.userAgent.indexOf('Windows') != -1) && (navigator.appVersion.indexOf('MSIE') != -1) )
        {
            try
            {
                var AgControl = new ActiveXObject("AgControl.AgControl");            
                agVersion = AgControl.settings.version;                
                AgControl = null;
                
            }
            catch (e)
            {
                agVersion = -1;
            }
        }
        return agVersion;
    }
    var versionStr = detectAgControlVersion();
    if (versionStr == -1 )
    {
        return false;
    }
    else if (versionStr != 0)
    {
        versionArray = versionStr.split(".");

        var versionMajor = versionArray[0];
        var versionMinor = versionArray[1];
        var versionBuild = versionArray[2];

        if (versionMajor > parseFloat(reqMajorVer))
        {
            return true;
        }
        else if (versionMajor == parseFloat(reqMajorVer))
        {
            if (versionMinor > parseFloat(reqMinorVer))
            {
                return true;
            }
            else if (versionMinor == parseFloat(reqMinorVer))
            {
                if (versionBuild >= parseFloat(reqBuildVer))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
// Silverlight event instance counter for memory mgt
Sys.Silverlight._counterL = 0;

///////////////////////////////////////////////////////////////////////////////
// createObject();  Params:
// parentElement of type Element, the parent element of the Silverlight Control
// source of type String
// id of type string
// properties of type String, object literal notation { name:value, name:value, name:value}, 
//     current properties are: width, height, background, framerate, isWindowless, enableHtmlAccess, inplaceInstallPrompt:  all are of type string
// events of type String, object literal notation { name:value, name:value, name:value}, 
//     current events are onLoad onError, both are type string
// initParams of type Object or object literal notation { name:value, name:value, name:value}
// userContext of type Object 
/////////////////////////////////////////////////////////////////////////////////

Sys.Silverlight.createObject = function(source, parentElement, id, properties, events, initParams, userContext)
{
        
    var slPluginHelper = new Object();
    var slProperties = properties;        
    var slEvents = events;
    slPluginHelper.source = source;
    slPluginHelper.parentElement = parentElement;
    slPluginHelper.id = id;         
    slPluginHelper.width = slProperties.width;
    slPluginHelper.height = slProperties.height;
    slPluginHelper.background = slProperties.background;        
    slPluginHelper.isWindowless = slProperties.isWindowless;
    slPluginHelper.framerate = slProperties.framerate;
    slPluginHelper.ignoreBrowserVer = slProperties.ignoreBrowserVer;    
    slPluginHelper.inplaceInstallPrompt = slProperties.inplaceInstallPrompt;
    slPluginHelper.enableHtmlAccess = slProperties.enableHtmlAccess;
    slPluginHelper.initParams = initParams;        
    
    //memory management for onLoad event      
    if (slEvents.onLoad) 
    {
       var uniqueID = '_sl' + (Sys.Silverlight._counterL++);
       slPluginHelper.loadedHandlerName = 'javascript:' + uniqueID;  

       function _dispose()
       {
        if (window.detachEvent) 
        {
            window.detachEvent('onunload', _dispose);
        }
        else 
        {
            window.removeEventListener('unload', _dispose, false);
        }
        window[uniqueID] = null;
       }

        function _loadedHandler(sender) 
        {
            slEvents.onLoad(document.getElementById(slPluginHelper.id), userContext, sender); 
            _dispose();
        }

        window[uniqueID] = _loadedHandler;  
        if (window.attachEvent) 
        {
            window.attachEvent('onunload', _dispose);
        }
        else 
        {
            window.addEventListener('unload', _dispose, false);
        }
    }
    //set error handler
    if (!slEvents.onError)
    {
        slPluginHelper.onError = "default_error_handler";
    }
    else
    {
        slPluginHelper.onError = slEvents.onError;
    }          
    
    var slPluginHTML = "";
        
    //direct download pointer
    var directDownload;

    if (navigator.userAgent.indexOf('Windows') != -1)
    {
        directDownload = "http://go.microsoft.com/fwlink/?LinkID=86008";
    }

    else if (navigator.userAgent.indexOf('PPC Mac OS X') != -1)
    {
        directDownload = "http://go.microsoft.com/fwlink/?LinkID=87380";
    }

    else if (navigator.userAgent.indexOf('Intel Mac OS X') != -1)
    {
        directDownload = "http://go.microsoft.com/fwlink/?LinkID=87384";
    }
    //point to correct image/landing page for Alpha (0.95.x) and Beta (0.90.x)
    var inDirectDownloadPage, inDirectDownloadImage;
    
    var curVer = slProperties.version.split(".");
        majorVer = curVer[0]; 
        minorVer = curVer[1];
    
    //if Alpha, disallow inPlaceInstall
    if (minorVer == "95")
    {
        slPluginHelper.inplaceInstallPrompt = false;
        inDirectDownloadPage = "http://go.microsoft.com/fwlink/?LinkID=88363";
        inDirectDownloadImage = "http://go.microsoft.com/fwlink/?LinkID=88365";
    }
    else
    {
        inDirectDownloadPage = "http://go.microsoft.com/fwlink/?LinkID=86009";
        inDirectDownloadImage = "http://go.microsoft.com/fwlink/?LinkID=87023";    
    
    }
  
    // text for Silverlight image link, used for non-inplaceInstallPrompt and unsupported browser
    
    var silverlightLink = '<div style="width: 205px; height: 67px; background-color: #FFFFFF"><a href="'+inDirectDownloadPage+'"><img style="border:0";  src="'+inDirectDownloadImage+'"/></a></div>'
    // detect supported browser version & that the correct version of WPF/e is installed, else display install
    
    if (browserIsSupportedVersion(slPluginHelper))
    {   
      
        if (Sys.Silverlight.isInstalled(slProperties.version))
        {
            slPluginHTML = buildHTML(slPluginHelper);
        }
        else if (!slPluginHelper.inplaceInstallPrompt)
        {
            slPluginHTML = silverlightLink;
                        
        }
        else  //inPlaceInstallPrompt
        { 
            slPluginHTML += '<div style="width: 205px; height: 101px background-color: #FFFFFF;"><a href="'+directDownload+'"><img style="border:0"; SRC="http://go.microsoft.com/fwlink/?LinkID=87024"></a>';                
            slPluginHTML += '<div style="margin-top: -60px;text-align: center;color: #FFFFFF; font-size: 10px;font-family: Arial ">By clicking <b>Get Microsoft Silverlight</b> you accept the ';
            slPluginHTML += '<a href="http://go.microsoft.com/fwlink/?LinkID=87025" style="text-decoration: underline;color: #FFFFFF;">Silverlight license agreement.</a></div>';                
            slPluginHTML += '<div style="margin-top: 8px;text-align: center;color: #FFFFFF; font-family: Arial; font-size: 10px;">Silverlight updates automatically, <a href="http://go.microsoft.com/fwlink/?LinkID=87026" style="text-decoration: underline;color: #FFFFFF;">learn more.</a></div></div>';
                    
        } 
    }
    else
    {
        slPluginHTML = silverlightLink;
    
    }        
    // insert the HTML into the requested host element or return <object> tag.
    if(parentElement != null)
    {
        parentElement.innerHTML = slPluginHTML;
    }
    else 
    {
        return slPluginHTML; 
    }
        
}

///////////////////////////////////////////////////////////////////////////////
//  detect to see if this is a supported browser version    
///////////////////////////////////////////////////////////////////////////////
function browserIsSupportedVersion(slPluginHelper)
{
    var supportedBrowser = true;
    
    if (slPluginHelper.ignoreBrowserVer == true)
    {
        return supportedBrowser;
    }
    else
    {    
        var supportedBrowser = false;          
    }     
    
    // detection for Internet Explorer 6.0+, 32-bit only
    if (navigator.userAgent.indexOf('MSIE') != -1)
    {
        if (navigator.userAgent.indexOf('Win64') == -1)
        {           
            var tempVersion = navigator.userAgent.split("MSIE");
            browserMajorVersion = parseInt(tempVersion[1]);
                if (browserMajorVersion >= 6.0)
                {
                    supportedBrowser = true;
                }
        }
    }
    // detection for Firefox 1.5+ and 2.0
    else if (navigator.userAgent.indexOf("Firefox") != -1)
    {
        var tempVersion = navigator.userAgent.split("Firefox/");
        tempVersion = tempVersion[1].split(".");
        browserMajorVersion = parseFloat(tempVersion[0]);
        browserMinorVersion = parseFloat(tempVersion[1]);

        if (browserMajorVersion >= 2)
        {
            supportedBrowser = true;
        }
        else
        {
            if ((browserMinorVersion >= 5))
            {
                supportedBrowser = true;
            }
        }
    }
    else if (navigator.userAgent.indexOf("Safari") != -1)
    {
        supportedBrowser = true;
    }

    return supportedBrowser;
}

///////////////////////////////////////////////////////////////////////////////
//
//  create HTML that instantiates the control
//
///////////////////////////////////////////////////////////////////////////////
function buildHTML(slPluginHelper)
{
    var slPluginHTML = '<object type="application/ag-plugin" id="'+slPluginHelper.id+'" width="'+slPluginHelper.width+'" height="'+slPluginHelper.height+'" >';


    if (slPluginHelper.source != null)
    {
        slPluginHTML += ' <param name="source" value="'+slPluginHelper.source+'" />';
    }
    if (slPluginHelper.framerate != null)
    {
        slPluginHTML += ' <param name="maxFramerate" value="'+slPluginHelper.framerate+'" />';
    }
            
    slPluginHTML += ' <param name="onError" value="'+slPluginHelper.onError+'" />';       
    
    if (slPluginHelper.background != null)
    {
        slPluginHTML += ' <param name="background" value="'+slPluginHelper.background+'" />';
    }
    if (slPluginHelper.isWindowless != null)
    {
        slPluginHTML += ' <param name="windowless" value="'+slPluginHelper.isWindowless+'" />';        
    }
    if (slPluginHelper.initParams != null)
    {
        slPluginHTML += ' <param name="initParams" value="'+slPluginHelper.initParams+'" />';        
    }
    if (slPluginHelper.enableHtmlAccess != null)
    {
        slPluginHTML += ' <param name="enableHtmlAccess" value="'+slPluginHelper.enableHtmlAccess+'" />';        
    }
    if (slPluginHelper.loadedHandlerName != null)
    {
        slPluginHTML += ' <param name="onLoad" value="'+slPluginHelper.loadedHandlerName+'" />';        
    }
        
    slPluginHTML += '<\/object>';
    
    if (navigator.userAgent.indexOf("Safari") != -1)
   {
        // disable Safari caching
        // for more information, see http://developer.apple.com/internet/safari/faq.html#anchor5
        slPluginHTML += "<iframe style='visibility:hidden;height:0;width:0'/>";
   }

    return slPluginHTML;
}

///////////////////////////////////////////////////////////////////////////////
//
//  Default error handling function to be used when a custom error handler is
//  not present
//
///////////////////////////////////////////////////////////////////////////////

function default_error_handler(sender, args) 
{
    var iErrorCode;
    var errorType = args.ErrorType;

    iErrorCode = args.ErrorCode;

    var errMsg = "\nSilverlight error message     \n" ;

    errMsg += "ErrorCode: "+ iErrorCode + "\n";


    errMsg += "ErrorType: " + errorType + "       \n";
    errMsg += "Message: " + args.ErrorMessage + "     \n";

    if (errorType == "ParserError")
    {
        errMsg += "XamlFile: " + args.xamlFile + "     \n";
        errMsg += "Line: " + args.lineNumber + "     \n";
        errMsg += "Position: " + args.charPosition + "     \n";
    }
    else if (errorType == "RuntimeError")
    {           
        if (args.lineNumber != 0)
        {
            errMsg += "Line: " + args.lineNumber + "     \n";
            errMsg += "Position: " +  args.charPosition + "     \n";
        }
        errMsg += "MethodName: " + args.methodName + "     \n";
    }

    alert(errMsg);
}


// createObjectEx, takes a single parameter of all createObject parameters enclosed in {}
        
Sys.Silverlight.createObjectEx = function(params)
{        
    var parameters = params;
    Sys.Silverlight.createObject(parameters.source, parameters.parentElement, parameters.id, parameters.properties, parameters.events, parameters.initParams, parameters.context)
}
 //
// SL2.js
// Additional Bluelight launch code to be appended to Silverlight.js
// $Id: bluelight.debug-0.9.js#1 2007/07/11 19:22:02 $
//

if (!Sys.Silverlight._InvocationService) {
    Sys.Silverlight._InvocationService = "http://silverlight.services.live.com/invoke";

    Sys.Silverlight._AfterLoadFns = [];
    Sys.Silverlight._PageLoaded   = false;
    Sys.Silverlight._Provided     = {};
    Sys.Silverlight._Requests     = [];
    Sys.Silverlight._AppInfo      = {};
    Sys.Silverlight._NextSym      = 1;
    Sys.Silverlight._LoadQueue    = [];
    Sys.Silverlight._EvalDelay    = 0;
    Sys.Silverlight._IsSafari     = /WebKit/i.test(navigator.userAgent);
    Sys.Silverlight._Log          = [];
    Sys.Silverlight._DebugAlert   = false;
}

function Sys$Silverlight$GetLog() {
    return Sys.Silverlight._Log;
}
function Sys$Silverlight$ShowLog() {
    alert(Sys.Silverlight._Log.join('\n'));
}

Sys.Silverlight.Log = function(msg) {
    if (Sys.Silverlight._DebugAlert) {
        alert(msg);
    }
    Sys.Silverlight._Log.push(msg);
    if (window.console) {
        window.console.log(msg);
    }
}

Sys.Silverlight.DebugStr = function(o) {
    if (o == undefined) {
        return "undefined";
    } else if (o == null) {
        return "null";
    } else if (typeof(o) == "object") {
        var s = o.toString();
        for (var x in o) {
           s += "," + x + "=" + o[x];
        }
        return s;
    } else {
        return o.toString();
    }
}

Sys.Silverlight._OnPageLoad = function() {
   Sys.Silverlight._PageLoaded = true;
   for (var i = 0; i < Sys.Silverlight._AfterLoadFns.length; ++i) {
       Sys.Silverlight._AfterLoadFns[i]();
   }
   Sys.Silverlight._AfterLoadFns = [];
}

Sys.Silverlight._SetupOnLoadHandler = function() {
    if (window.addEventListener) {
        window.addEventListener("load", Sys.Silverlight._OnPageLoad, true);
    } else if (window.attachEvent) {
        window.attachEvent("onload", Sys.Silverlight._OnPageLoad);
    } else {
        window.onload = Sys.Silverlight._OnPageLoad;
    }
}
Sys.Silverlight._SetupOnLoadHandler();

Sys.Silverlight._RunAfterLoad = function(fn) {
    if (Sys.Silverlight._PageLoaded) {
        fn();
    } else {
        Sys.Silverlight._AfterLoadFns.push(fn);
    }
}

// Facility for keeping track of which files we've loaded.
Sys.Silverlight._Provide = function(filename) {
    Sys.Silverlight._Provided[filename] = true;
    for (var i = Sys.Silverlight._Requests.length - 1; i >= 0; --i) {
        if (Sys.Silverlight._CheckRequire(Sys.Silverlight._Requests[i])) {
            Sys.Silverlight._Requests.splice(i, 1);   // remove it
        }
    }
}

Sys.Silverlight._Require = function(appId, files, fn) {
    var req = {files: files, fn: fn};
    var allHere = true;
    for (var i = 0; i < req.files.length; ++i) {
        if (!Sys.Silverlight._Provided[req.files[i]]) {
            allHere = false;
        }
    }
    if (allHere) {
        fn();
    } else {
        Sys.Silverlight._Requests.push(req);
    }
}

Sys.Silverlight._CheckRequire = function(req) {
    var Log = Sys.Silverlight.Log;
    Log("Check require");
    for (var i = 0; i < req.files.length; ++i) {
        Log("  " + i + ": " + req.files[i]);
        if (!Sys.Silverlight._Provided[req.files[i]]) {
            Log("  " + i + " not found");
            return false;
        }
    }
    // found them all; do the function
    req.fn();
    return true;
}

Sys.Silverlight._DumbLoadScript = function(url) {
    var tag = document.createElement("script");
    tag.src = url;
    document.body.appendChild(tag);
}

Sys.Silverlight._LoadScript = function(url) {
    if (Sys.Silverlight._LoadQueue.length > 0) {
        // We're already waiting for some script.
        Sys.Silverlight._LoadQueue.push(url);
    } else {
        Sys.Silverlight._LoadQueue.push(url);
        Sys.Silverlight._AddScriptTag(url);
    }
}

Sys.Silverlight._SignalLoadDone = function(url) {
    var Log = Sys.Silverlight.Log;
    if (Sys.Silverlight._LoadQueue[0] != url) {
        Log("done twice with " + url);
        return; // already did this one
    }
    Log("Done with " + url);
    Sys.Silverlight._LoadQueue.shift();
    if (Sys.Silverlight._LoadQueue.length > 0) {
        Sys.Silverlight._AddScriptTag(
            Sys.Silverlight._LoadQueue[0]);
    }
}

// Add a tag and remember that we're working on things.
Sys.Silverlight._AddScriptTag = function(url) {
    var HandleReadyChange = function() {
        Sys.Silverlight.Log("ready change " + tag.readyState + " " + url);
        if (tag.readyState == "loaded" || tag.readyState == "complete") {
            Sys.Silverlight._SignalLoadDone(url);
            Sys.Silverlight._Provide(url);
        }
    }
    var HandleLoad = function() {
        Sys.Silverlight._SignalLoadDone(url);
        Sys.Silverlight._Provide(url);
    }
    Sys.Silverlight.Log("Adding script for " + url);
    var tag = document.createElement("script");
    tag.type               = "text/javascript";
    tag.src                = url;
    tag.onreadystatechange = HandleReadyChange;
    tag.onload             = HandleLoad;
    document.body.appendChild(tag);
}

if (Sys.Silverlight._IsSafari) {
    Sys.Silverlight._AddScriptTag = function(url) {
        var Log = Sys.Silverlight.Log;
        var HandleReadyChange = function() {
            var req = xml;
            Log("ready change " + url + " " + req.readyState);
            if (req.readyState == 4) {
                var provide = function() {
                    Sys.Silverlight._Provide(url);
                }
                var text = req.responseText;
                Log("Got " + text.length + " chars");
                window.setTimeout(text, Sys.Silverlight._EvalDelay);
                window.setTimeout(provide, Sys.Silverlight.EvalDelay);
                Sys.Silverlight._SignalLoadDone(url);
            }
        }
        var xml = new XMLHttpRequest();
        xml.open("GET", url, true);
        xml.onreadystatechange = HandleReadyChange;
        xml.send();
    }
}

Sys.Silverlight.parseInitParams = function(origParams) {
    // Match the way the plug parses this.
    var initParams = ("" + origParams).split(',');
    var result = [];
    for (var i = 0; i < initParams.length; ++i) {
        var idx = initParams[i].indexOf('=');
        if (idx < 0) {
            result[i] = {key: "", value: initParams[i]};
        } else {
            result[i] = {
                key:   initParams[i].substring(0, idx),
                value: initParams[i].substring(idx + 1)
            };
        }
    }
    return result;
}

Sys.Silverlight.createHostedObjectEx = function(params)
{
    var streamingApp = false;
    var app = "/local";
    if (params.source.substring(0, 10) == "streaming:") {
        streamingApp = true;
        app = params.source.substring(10);
    }
    var invParams = '';
    if (params.initParams) {
        var pp = Sys.Silverlight.parseInitParams(params.initParams);
        var idx = 0;
        for (var k = 0; k < pp.length; ++k) {
            if (pp[k].value.substring(0, 10) == "streaming:") {
                invParams += '&p' + (idx++) + '=' + pp[k].value.substring(10);
            }
        }
    }
    if (streamingApp || invParams) {
        var id = "bl" + (++Sys.Silverlight._NextSym);
        var url = Sys.Silverlight._InvocationService +
                  app + "/start.js?id=" + id + 
                  "&u=" + (new Date().valueOf()) +
                  invParams;
        Sys.Silverlight._AppInfo[id] = params;
        var fn = function() {
            Sys.Silverlight._DumbLoadScript(url);
        }
        Sys.Silverlight._RunAfterLoad(fn);
    } else {
        Sys.Silverlight.createObjectEx(params);
    }
}

// Called in containing page
Sys.Silverlight._StartApp = function(appId, appUrl, manifest, jsFiles, params) {
    var info = Sys.Silverlight._AppInfo[appId];
    var ifn = manifest.loadFunction         || "";
    var stx = manifest.source               || "";
    var w   = manifest.width                || "100%";
    var h   = manifest.height               || "100%";
    var bg  = manifest.background           || "";
    var wl  = manifest.isWindowless         || "";
    var fr  = manifest.framerate            || "";
    var onl = manifest.onLoad               || "";
    var one = manifest.onError              || "";
    var eha = manifest.enableHtmlAccess     || "";
    var ipi = manifest.inPlaceInstallPrompt || "";
    var ver = manifest.version              || "0.8.5";

    if (appUrl == null) {
        Sys.Silverlight._StartLocalApp(info, params);
        return;
    }

    var hash = "/" + encodeURIComponent(ifn);
    hash += "/" + encodeURIComponent(appId);
    hash += "/" + encodeURIComponent(stx);
    hash += "/" + encodeURIComponent(bg);
    hash += "/" + encodeURIComponent(wl);
    hash += "/" + encodeURIComponent(fr);
    hash += "/" + encodeURIComponent(onl);
    hash += "/" + encodeURIComponent(one);
    hash += "/" + encodeURIComponent(eha);
    hash += "/" + encodeURIComponent(ipi);
    hash += "/" + encodeURIComponent(ver);

    hash += "/" + jsFiles.length;
    for (var i = 0; i < jsFiles.length; ++i) {
        hash += "/" + encodeURIComponent(jsFiles[i]);
    }
    var iParams = 0;
    if (info.initParams) {
        var pp = Sys.Silverlight.parseInitParams(info.initParams);
        for (var i = 0; i < pp.length; ++i) {
            if (pp[i].value.substring(0, 10) == "streaming:") {
                pp[i].value = params[iParams++];
            }
            if (pp[i].key) {
                hash += "/" + encodeURIComponent(pp[i].key + '=' + pp[i].value);
            } else {
                hash += "/" + encodeURIComponent(pp[i].value);
            }
        }
    }
    // Double encode for Firefox
    hash = encodeURIComponent(hash);
    //alert("host element: " + info.hostElement);
    var text = "<iframe " +
               "src='" + appUrl + "/zziframehtmlzz.html#" + hash + "' " +
               "height='" + h + "' width='" + w + "' " +
               "scrolling='no' " +
               "frameborder='0' " +
               "></iframe>";
    info.parentElement.innerHTML = text;
}

Sys.Silverlight._StartLocalApp = function(info, params) {
    var iParams = 0;
    var newIP = [];
    if (info.initParams) {
        var pp = Sys.Silverlight.parseInitParams(info.initParams);
        for (var i = 0; i < pp.length; ++i) {
            var val = pp[i].value;
            if (val.substring(0, 10) == "streaming:") {
                val = params[iParams++];
            }
            if (pp[i].key) {
                newIP.push(pp[i].key + '=' + val);
            } else {
                newIP.push(val);
            }
        }
    }
    info.initParams = newIP;
    Sys.Silverlight.createObjectEx(info);
}

// Called in the iframe
Sys.Silverlight._FinishStartup = function() {
    var Log = Sys.Silverlight.Log;
    Log("_FinishStartup");
    function Decode(s) {
        var x = decodeURIComponent(s);
        return x == "" ? undefined : x;
    }

    function DecodeProp(bag, name, value) {
        var v = decodeURIComponent(value);
        if (v != "") {
            bag[name] = v;
        }
    }

    //alert(this.location.hash);
    var eltName = "aghostDiv";
    var hash = location.hash.substring(1);
    Sys.Silverlight.Log(hash);
    // See if we need to undo the double encode
    if (hash.charAt(0) != '/') {
        hash = decodeURIComponent(hash);
    }
    var data = hash.split('/');
    var idx = 1;    // first component is null
    var loadFunction = data[idx++];

    var params = {};    // general invocation parameters
    var props  = {width: "100%", height: "100%"};    // SL properties
    var events = {onError: null, onLoad: null};
    var initParams = [];
    params.properties = props;
    params.events     = events;
    params.initParams = initParams;

    DecodeProp(params, "id"              , data[idx++]);
    DecodeProp(params, "source"          , data[idx++]);
    DecodeProp(props , "background"      , data[idx++]);
    DecodeProp(props , "isWindowless"    , data[idx++]);
    DecodeProp(props , "framerate"       , data[idx++]);
    DecodeProp(events, "onLoad"          , data[idx++]);
    DecodeProp(events, "onError"         , data[idx++]);
    DecodeProp(props , "enableHtmlAccess", data[idx++]);
    DecodeProp(props , "inPlaceInstall"  , data[idx++]);
    DecodeProp(props , "version"         , data[idx++]);
    var jsFilesLen = parseInt(decodeURIComponent(data[idx++]));
    var jsFiles = [];
    for (var i = 0; i < jsFilesLen; ++i) {
        jsFiles.push(decodeURIComponent(data[idx++]));
    }
    while (idx < data.length) {
        initParams.push(decodeURIComponent(data[idx++]));
    }

    for (var i = 0; i < jsFiles.length; ++i) {
        Sys.Silverlight._LoadScript(jsFiles[i]);
    }

    params.parentElement = document.getElementById(eltName);
    params.context       = null;
    var startXaml = function() {
        Log("starting...");
        if (loadFunction) {
            Log("loadFunction=" + loadFunction);
            // app is handling object creation
            window[loadFunction](eltName, params.id);
        }
        if (params.source) {
            if (params.events.onLoad) {
                params.events.onLoad = window[params.events.onLoad];
            }
            if (params.events.onError) {
                params.events.onError = window[params.events.onError];
            }
            //alert("eltName: " + eltName + ", appId: " + appId + ", appUrl: " + appUrl + ", rootXaml: " + rootXaml);
            Sys.Silverlight.createObjectEx(params);
        }
    };
    Sys.Silverlight._Require("x", jsFiles, startXaml);
}
