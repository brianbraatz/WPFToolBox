function google_ad_request_done(google_ads) {
var s = '';
var i;

if ( google_ads.length == 0 ) { return; }

if ( 'flash' == google_ads[0].type ) {

   s += '<a href=\"' + google_info.feedback_url + '\" style="color:000000">Ads by Google</a><br>' +
'<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"' +
' codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0"' +
' WIDTH="' + google_ad.image_width +
'" HEIGHT="' + google_ad.image_height + '">' +
'<PARAM NAME="movie" VALUE="' + google_ad.image_url + '">' +
'<PARAM NAME="quality" VALUE="high">' +
'<PARAM NAME="AllowScriptAccess" VALUE="never">' +
'<EMBED src="' + google_ad.image_url +
'" WIDTH="' + google_ad.image_width +
'" HEIGHT="' + google_ad.image_height +
'" TYPE="application/x-shockwave-flash"' +
' AllowScriptAccess="never" ' +
' PLUGINSPAGE="http://www.macromedia.com/go/getflashplayer"></EMBED></OBJECT>';

} else if ( 'image' == google_ads[0].type ) {

   s += '<a href=\"' + google_info.feedback_url + '\" style="color:000000">Ads by Google</a><br>' +
'<a href="' + google_ads[0].url +
'" target="_top" title="go to ' + google_ads[0].visible_url +
'"><img border="0" src="' + google_ads[0].image_url +
'"width="' + google_ads[0].image_width +
'"height="' + google_ads[0].image_height + '"></a>';

} else {

if (google_ads.length == 1) {

s = '<p style="text-align: left; width: 100%;">' + 
'<a href=\"' + google_info.feedback_url + '\" style="color:000000">Ads by Google</a>' +
'<a href="s-p: Go to ' + google_ads[0].visible_url + '" ' +
'onclick="window.location=\'' + google_ads[0].url + '\'; return false;" ' +
'style="display: block; float: left; padding-right: 10px; text-decoration: none; border-bottom: none; text-align: center; font-size: 130%; margin: auto;">' +
'<span class="gad-head" style="text-decoration: underline; font-size: 1.2em; font-weight: bold;">' + google_ads[0].line1 + '</span><br />' +
'<span style="color:#' + color_text + ';">' +
google_ads[0].line2 + ' ' +
google_ads[0].line3 + '<br /></span>' +
'<span style="color:#' + color_url + ';">' +
google_ads[0].visible_url + '</a>' +
'<br clear="all" /></p>';

} else if (google_ads.length == 2) {
s = '<p style="text-align: left; width: 100%;">' + 
'<a style="text-decoration: none; border-bottom: none;" href="' + google_info.feedback_url + '">Ads by Google</a>';
for(i = 0; i < google_ads.length; ++i) {
	s +='<a href="s-p: Go to ' + google_ads[i].visible_url + '" ' +
	'onclick="window.location=\'' + google_ads[i].url + '\'; return false;" ' +
	'style="display: block; float: left; padding-right: 10px; text-decoration: none; border-bottom: none;">' +
	'<span class="gad-head" style="text-decoration: underline; font-size: 1.2em;">' + google_ads[i].line1 + '</span><br />' +
	'<span class="gad-text" style="font-weight: normal; color:#' + color_text + '">' +
	google_ads[i].line2 + '<br />' +
	google_ads[i].line3 + '</span><br />' +
	'<span class="gad-url" style="color:#' + color_url + '">' + google_ads[i].visible_url + '</span></a>';
}
s += '<br clear="all" /></p>';
} else if (google_ads.length > 2) {
s = '<p style="text-align: left; width: 100%;">' + 
'<a style="text-decoration: none; border-bottom: none;" href="' + google_info.feedback_url + '">Ads by Google</a>';
for(i = 0; i < google_ads.length; ++i) {
	s +='<a href="s-p: Go to ' + google_ads[i].visible_url + '" ' +
	'onclick="window.location=\'' + google_ads[i].url + '\'; return false;" ' +
	'style="display: block; width: 100%; padding-bottom: 5px; text-decoration: none; border-bottom: none; font-weight: normal;">' +
	'<span class="gad-head" style="text-decoration: underline; font-size: 1.2em;">' + google_ads[i].line1 + '</span><br />' +
	'<span class="gad-text" style="font-weight: normal; color:#' + color_text + '">' +
	google_ads[i].line2 + ' ' + google_ads[i].line3 + '</span><br />' +
	'<span class="gad-url" style="color:#' + color_url + '" style="font-weight: normal">' + google_ads[i].visible_url + '</span></a>';
}
s += '<br clear="all" /></p>';
}
}
document.write(s);
return;
}