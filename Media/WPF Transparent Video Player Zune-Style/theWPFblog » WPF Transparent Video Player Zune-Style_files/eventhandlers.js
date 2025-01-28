function loaded(sender, args)
{
	var home = sender.findName("home");
	home.mouseEnter = "javascript:homeEnter"
	home.mouseLeave = "javascript:homeLeave"
	home.mouseLeftButtonDown = "javascript:homer"
	
	var other = sender.findName("other");
	other.mouseEnter = "javascript:otherEnter"
	other.mouseLeave = "javascript:otherLeave"
	other.mouseLeftButtonDown = "javascript:otherer"
	
	var about = sender.findName("about");
	about.mouseEnter = "javascript:aboutEnter"
	about.mouseLeave = "javascript:aboutLeave"
	about.mouseLeftButtonDown = "javascript:abouter"
	
	var contact = sender.findName("contact");
	contact.mouseEnter = "javascript:contactEnter"
	contact.mouseLeave = "javascript:contactLeave"
	contact.mouseLeftButtonDown = "javascript:contacter"
}

function homeEnter(sender, args)
{
	var wpf = document.getElementById("wpfeControl1");
	wpf.findName("in").begin();
	wpf.findName("out").stop();
}

function homeLeave(sender, args)
{
	var wpf2 = document.getElementById("wpfeControl1");
	wpf2.findName("out").begin();
	//wpf2.findName("in").begin();
}

function otherEnter(sender, args)
{
	var wpf = document.getElementById("wpfeControl1");
	wpf.findName("in2").begin();
	wpf.findName("out2").stop();
}

function otherLeave(sender, args)
{
	var wpf2 = document.getElementById("wpfeControl1");
	wpf2.findName("out2").begin();
	//wpf2.findName("in2").begin();
}

function aboutEnter(sender, args)
{
	var wpf = document.getElementById("wpfeControl1");
	wpf.findName("in3").begin();
	wpf.findName("out3").stop();
}

function aboutLeave(sender, args)
{
	var wpf2 = document.getElementById("wpfeControl1");
	wpf2.findName("out3").begin();
	//wpf2.findName("in3").begin();
}

function contactEnter(sender, args)
{
	var wpf = document.getElementById("wpfeControl1");
	wpf.findName("in4").begin();
	wpf.findName("out4").stop();
}

function contactLeave(sender, args)
{
	var wpf2 = document.getElementById("wpfeControl1");
	wpf2.findName("out4").begin();
	//wpf2.findName("in4").begin();
}


function homer(sender, args)
{
	location.href="http://www.thewpfblog.com";
}

function otherer(sender, args)
{
	location.href="http://thewpfblog.com/?page_id=39";
}

function abouter(sender, args)
{
	location.href="http://thewpfblog.com/?page_id=37";
}

function contacter(sender, args)
{
	location.href="http://thewpfblog.com/?page_id=38";
}

