// Original idea from: The Man in Blue Resolution Dependent Layout Script
// http://www.themaninblue.com/experiment/ResolutionLayout/

// Modified by Filipe Fortes ( http://fortes.com ) to get sizes from querystring
// and to use sandbox-style class selectors instead of separate stylesheets
function getBrowserWidth()
{
	var width = 0;
	if (window.innerWidth)
		width = window.innerWidth
	else if (document.documentElement && document.documentElement.clientWidth != 0)
		width = document.documentElement.clientWidth;
	else if (document.body)
		width = document.body.clientWidth;
	
	// Check the cookie, in case it's there
	if (width == 0)
	{
		var widthCookie = document.cookie.match(/(^|;)browser_width[^;]*(;|$)/);

		if (widthCookie != null)
		{
			width = parseInt((widthCookie[0].split("=")[1]));
			if (isNaN(width))
				width = 0;
		}
		
		// Have to wait to get the width
		addLoadListener(checkBrowserWidth);
	}
	else
	{
		document.cookie = "browser_width=" + width;
	}
	
	return width;
}

function addLoadListener(fn)
{
	if (typeof window.addEventListener != 'undefined')
		window.addEventListener('load', fn, false);
	else if (typeof document.addEventListener != 'undefined')
		document.addEventListener('load', fn, false);
	else if (typeof window.attachEvent != 'undefined')
		window.attachEvent('onload', fn);
	else
		return false;

	return true;
}

function attachEventListener(target, eventType, functionRef, capture)
{
    if (typeof target.addEventListener != "undefined")
        target.addEventListener(eventType, functionRef, capture);
    else if (typeof target.attachEvent != "undefined")
        target.attachEvent("on" + eventType, functionRef);
    else
        return false;

    return true;
}

function getStyleName()
{
	var theWidth = getBrowserWidth();
	var i, styleName;
	if (theWidth != 0)
	{
		for (i = 0; i < sizes.length; i++)
		{
			if (theWidth >= sizes[i])
			{
				styleName = prefix + sizes[i];
				break;
			}
		}
		
		if (styleName == null)
			styleName = prefix + 'min';
	}
	else
		styleName = prefix + 'unknown';
		
	return styleName;
}

function checkBrowserWidth()
{
	var styleName = getStyleName();

	if (document.body)
	{
		var newClass = document.body.className.replace(styleMatch, '') + " " + styleName;
		document.cookie = 'bodySizeClass=' + styleName;
		document.body.className = newClass;
	}
	
	return true;
}

function scriptMatch(s)
{
	return ((s.src) && (s.src.match(/dynamicLayout\.js(\?.*)?$/)));
}

// Find out what sizes we're using
var sizes, prefix, script;
for (var i = 0; script = document.getElementsByTagName('script')[i]; i++)
{
	if (script.src.match(/dynamiclayout/))
	{
		sizes = script.src.match(/sizes=([0-9]|,)+/);
		if (sizes)
		{
			sizes = sizes[0].split('=')[1].split(',');
			// Want sizes in decreasing order ... 
			sizes.sort(function s(a,b) { return parseInt(b) - parseInt(a); });
		}
		else
		{
			sizes = null;
		}
		
		// Styles must start with a letter
		prefix = script.src.match(/prefix=([a-zA-Z])([^&]+)/);
		if (prefix)
			prefix = prefix[0].split('=')[1];
		else
			prefix = null;
	}
}

if (sizes == null) sizes = Array(1600,1400,1200,1000,800,600);
if (prefix == null) prefix = 'browserwidth-';
var styleMatch = new RegExp(prefix + '(min|unknown|[0-9]+)', 'ig');

// Make sure to adjust on resizes
checkBrowserWidth();
attachEventListener(window, "resize", checkBrowserWidth, false);