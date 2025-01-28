function a()
{
	var a;
	/*@cc_on @*/
	/*@if (@_jscript_version >= 5)
	// JScript gives us Conditional compilation, we can cope with old IE versions.
	// and security blocked creation of the objects.
	try {
	a = new ActiveXObject("Msxml2.XMLHTTP");
	} catch (e) {
	try {
	a = new ActiveXObject("Microsoft.XMLHTTP");
	} catch (E) {
	a = false;
	}
	}
	@end @*/

	if (!a && typeof XMLHttpRequest!='undefined')
	{
		a = new XMLHttpRequest();
	}

	return a;
}

function dc(d, s, a)
{
	o = document.getElementById(d);
	o.className = s;
	o.innerHTML = '<table><tr><td style="vertical-align:top;text-align:right;">Replying To:<p style="text-align:center;"><a href="#AddComment" onclick="document.getElementById(\'parent\').value=0;document.getElementById(\'reply\').className=\'hidden\';document.getElementById(\'reply\').innerHTML=\'\';location.hash=\'AddComment\';return false;">Cancel</a></p></td><td>' + a.responseText + '</td></tr></table>';
}

function g(a, t, u, d, s, f)
{
	a.open(t, u, false);
	a.send(null);
	if (a.readyState==4)
	{
		eval(f);
	}
}

function gc(t, ct, i, d, s)
{
	ac = a();
	if (ac)
	{
		u = "/" + t + ".aspx?" + ct + "=" + i;
		g(ac, t, u, d, s, "dc(d, s, a)");
		document.getElementById("parent").value = i;
		location.hash = "AddComment";
		return false;
	}
	else
	{
		return true;
	}
}