document.write('<!-- Template Id = 1 Template Name = Banner Creative (Flash) -->\n<!-- Copyright 2002 DoubleClick Inc., All rights reserved. --><script src=\"http://m1.2mdn.net/879366/flashwrite_1_2.js\"><\/script>');document.write('\n');
 
var dcswf = "http://m1.2mdn.net/1365243/Ph207_DTA_CDE_336x850.swf"; 
var dcgif = "http://m1.2mdn.net/1365243/Ph207_DTA_CDE_336x850.gif"; 
var advurl = "http://ad.doubleclick.net/click%3Bh=v8/35fb/3/0/%2a/g%3B142513310%3B0-0%3B0%3B20724823%3B10408-336/850%3B22885767/22903650/1%3B%3B%7Esscs%3D%3fhttp://ibm.com/software/info/takebackcontrol/r/Ph2DTA?orgid=2211&p_creative=Globe&re=earthweb&s_tact=107A788W";
var dcadvurl = escape(advurl);
var dcminversion = 6;
var dcmaxversion = 9;
var plugin = false;
var dccreativewidth = "336";
var dccreativeheight = "850";
var dcwmode = "opaque";
var dcbgcolor = "";

if (((navigator.appName == "Netscape") && (navigator.userAgent.indexOf("Mozilla") != -1) && (parseFloat(navigator.appVersion) >= 4) && (navigator.javaEnabled()) && navigator.mimeTypes && navigator.mimeTypes["application/x-shockwave-flash"] && navigator.mimeTypes["application/x-shockwave-flash"].enabledPlugin)) {
var plugname=navigator.plugins['Shockwave Flash'].description;var plugsub=plugname.substring(plugname.indexOf("."),-1); var plugsubstr=plugsub.substr(-1)
if( plugsubstr >= dcminversion) { plugin = true;}
}
else if (navigator.userAgent && navigator.userAgent.indexOf("MSIE")>=0 && (navigator.userAgent.indexOf("Opera")<0) && (navigator.userAgent.indexOf("Windows 95")>=0 || navigator.userAgent.indexOf("Windows 98")>=0 || navigator.userAgent.indexOf("Windows NT")>=0) && document.all) 
{
document.write('<script language=VBScript>' + '\n' +
   'dcmaxversion = '+dcmaxversion + '\n' +
   'dcminversion = '+dcminversion + '\n' +
   'Do' + '\n' +
    'On Error Resume Next' + '\n' +
    'plugin = (IsObject(CreateObject(\"ShockwaveFlash.ShockwaveFlash.\" & dcmaxversion & \"\")))' + '\n' +
    'If plugin = true Then Exit Do' + '\n' +
    'dcmaxversion = dcmaxversion - 1' + '\n' +
    'Loop While dcmaxversion >= dcminversion' + '\n' +
  '<\/script>');
}
if ( plugin )  {
 adcode = '<OBJECT classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"'+
  ' ID=FLASH_AD WIDTH="'+ dccreativewidth +'" HEIGHT="'+ dccreativeheight +'">'+
  '<PARAM NAME=movie VALUE="' + dcswf + '?clickTag='+ dcadvurl +'"><PARAM NAME=quality VALUE=high><PARAM NAME=bgcolor VALUE=#'+ dcbgcolor +'><PARAM NAME=wmode VALUE='+ dcwmode +'><PARAM NAME="AllowScriptAccess" VALUE="always">'+
  '<EMBED src="' + dcswf + '?clickTag='+ dcadvurl +'" quality=high wmode='+dcwmode+
  ' swLiveConnect=TRUE WIDTH="'+ dccreativewidth +'" HEIGHT="'+ dccreativeheight +'" bgcolor=#'+ dcbgcolor+
  ' TYPE="application/x-shockwave-flash" AllowScriptAccess="always"></EMBED></OBJECT>';
if(('j'!="j")&&(typeof dclkFlashWrite!="undefined")){dclkFlashWrite(adcode);}else{document.write(adcode);}
} else {
 document.write('<A TARGET="_blank" HREF="http://ad.doubleclick.net/click%3Bh=v8/35fb/3/0/%2a/g%3B142513310%3B0-0%3B0%3B20724823%3B10408-336/850%3B22885767/22903650/1%3B%3B%7Esscs%3D%3fhttp://ibm.com/software/info/takebackcontrol/r/Ph2DTA?orgid=2211&p_creative=Globe&re=earthweb&s_tact=107A788W"><IMG SRC="' + dcgif + '" alt="" BORDER=0></A>');
}
//-->

document.write('<NOSCRIPT><A TARGET=\"_blank\" HREF=\"http://ad.doubleclick.net/click%3Bh=v8/35fb/3/0/%2a/g%3B142513310%3B0-0%3B0%3B20724823%3B10408-336/850%3B22885767/22903650/1%3B%3B%7Esscs%3D%3fhttp://ibm.com/software/info/takebackcontrol/r/Ph2DTA?orgid=2211&p_creative=Globe&re=earthweb&s_tact=107A788W\"><IMG SRC=\"http://m1.2mdn.net/1365243/Ph207_DTA_CDE_336x850.gif\" alt=\"\" BORDER=0></A></NOSCRIPT>');
