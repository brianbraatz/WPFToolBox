// Begin DDJ script	

var url = this.location + "";
var ipid = "0";
var dex = "2";
var server="";
  
function stripSessionID(urlPre)
	{//mostly used for jsession but also for random ;
	if((urlPre.indexOf(";") != -1)||(urlPre.indexOf("?") != -1))
	{
	if ((urlPre.indexOf(";") < urlPre.indexOf("?") && urlPre.indexOf(";") != -1)||(urlPre.indexOf("?") < urlPre.indexOf(";") && urlPre.indexOf("?") == -1))
			{
		    var begin = urlPre.indexOf(";");
			}else if ((urlPre.indexOf("?") < urlPre.indexOf(";") && urlPre.indexOf("?") != -1)||(urlPre.indexOf(";") < urlPre.indexOf("?") && urlPre.indexOf(";") == -1))
			{
			var begin = urlPre.indexOf("?");			
			}	
			
	var str=urlPre.substring(begin);
	
	urlPre=urlPre.replace(str,"");	
	return urlPre;
	}else{
	return urlPre;	
	}
	}
function stripQuery(urlPre)
	{	
	if(urlPre.indexOf("&") != -1 || urlPre.indexOf("#") != -1)
		{
		if ((urlPre.indexOf("&") < urlPre.indexOf("#") && urlPre.indexOf("&") != -1)||(urlPre.indexOf("#") < urlPre.indexOf("&") && urlPre.indexOf("#") == -1))
			{
		    var begin = urlPre.indexOf("&");
			}else if ((urlPre.indexOf("#") < urlPre.indexOf("&") && urlPre.indexOf("#") != -1)||(urlPre.indexOf("&") < urlPre.indexOf("#") && urlPre.indexOf("&") == -1))
			{
			var begin = urlPre.indexOf("#");			
			}	
			
		var str=urlPre.substring(begin);
		urlPre=urlPre.replace(str,"");	
		return urlPre;
		}else{
		return urlPre;
		}
	}


//ddj 
if (((url.indexOf("ddj.com") != -1 )||(url.indexOf("drdobbs.com") != -1 ))&&( url.indexOf("devel2") == -1 )&&( url.indexOf("searchResults")==-1 ))
	{
	if(url.indexOf("drdobbs.com") != -1 ){url=url.replace("drdobbs.com","ddj.com");}
	server="cmp";
	
	ipid="1311";
	dex="1";

	if(( url.indexOf("index.htm")!= -1))
		{
		ipid="0";
		}else if ((url.indexOf("/blog/")!= -1 ))
			{
			dex="0";
			}
		
	}
	
	
	
if( ((url.indexOf("pgno=") != -1)||(url.indexOf("http://www.") == -1))&&(dex =="1"))
	{//dont want to index other page2
		if (url.indexOf("http://ddj") != -1)
		{
		url=url.replace("http://","http://www.");
		}
	dex="0";
	}

if (("0"!=ipid)||("2"== dex)){


	if (dex=="1")
		{
		var app="";	 
		url=stripQuery(url);
		url=stripSessionID(url);
		}else{
		var app="&ieix=0";
		url=stripQuery(url);
		url=stripSessionID(url);
		}	

url=escape(url);

 document.write(
'<SCR'+'IPT language="javascript" type="text/javascript" ' +
'src="http://' + server + '.us.intellitxt.com/intellitxt/front.asp?ipid=' + ipid +
'&ieru=' + url + app +
'">' +
'</SCR'+'IPT>'
);
}