function createPlayerFlash(mediaFilePath)
{
	document.write('<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0');
	document.write('width="199" height="21" id="inline_02" align="middle">');
	document.write('<param name="allowScriptAccess" value="sameDomain" />');
	document.write('<param name="movie" value="/MediaPlayers/ExtendablePlayer.swf?theFile=' + mediaFilePath + '" />');
	document.write('<param name="quality" value="high" />');
	document.write('<param name="wmode" value="transparent" />');
	document.write('<embed src="/MediaPlayers/ExtendablePlayer.swf?theFile=' + mediaFilePath + '" quality="high" wmode="transparent" width="199" height="21" name="inline_02" align="middle" allowScriptAccess="sameDomain" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />');
	document.write('</object>');
}
