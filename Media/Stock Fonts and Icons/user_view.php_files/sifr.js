/*	sIFR 2.0.1
	Copyright 2004 - 2005 Mike Davidson, Shaun Inman, Tomas Jogin and Mark Wubben

	This software is licensed under the CC-GNU LGPL <http://creativecommons.org/licenses/LGPL/2.1/>

*/



var hasFlash=function(){ 
	var a=6;
	if(navigator.appVersion.indexOf("MSIE")!=-1&&navigator.appVersion.indexOf("Windows")>-1){ 
		document.write('<script language="VBScript"\> \non error resume next \nhasFlash = (IsObject(CreateObject("ShockwaveFlash.ShockwaveFlash." & '+a+'))) \n</script\> \n');
		if(window.hasFlash!=null)return window.hasFlash
	}
	if(navigator.mimeTypes&&navigator.mimeTypes["application/x-shockwave-flash"]&&navigator.mimeTypes["application/x-shockwave-flash"].enabledPlugin){ 
		var b=(navigator.plugins["Shockwave Flash 2.0"]||navigator.plugins["Shockwave Flash"]).description;
		return parseInt(b.charAt(b.indexOf(".")-1))>=a
	}
	return false
}();

String.prototype.normalize=function(){ 
	return this.replace(/\s+/g, " ")
};

if(Array.prototype.push==null){ 
	Array.prototype.push=function(){ 
		var i=0,
			a=this.length,
			b=arguments.length;
			
		while(i<b){ 
			this[a++]=arguments[i++]
		}
		return this.length
	}
}

if(!Function.prototype.apply){ 
	Function.prototype.apply=function(a,b){ 
		var c=[];
		var d,e;
		
		if(!a)a=window;
		if(!b)b=[];
		
		for(var i=0;i<b.length;i++){ 
			c[i]="b["+i+"]"}e="a.__applyTemp__("+c.join(",")+");";
			a.__applyTemp__=this;
			d=eval(e);
			a.__applyTemp__=null;
			return d
	}
}

function named(a){ 
	return new named.Arguments(a)
}

named.Arguments=function(a){this.oArgs=a};
named.Arguments.prototype.constructor=named.Arguments;

named.extract=function(a,b){ 
	var c,d;
	var i=a.length;
	
	while(i--){ 
		d=a[i];
		if(d!=null&&d.constructor!=null&&d.constructor==named.Arguments){ 
			c=a[i].oArgs;break
		}
	}
	
	if(c==null)return;
	for(e in c)
		if(b[e]!=null)b[e](c[e]);
		
	return
};

var parseSelector=function(){ 
	var a=/^([^#.>`]*)(#|\.|\>|\`)(.+)$/;
	
	function r(s,t){ 
		u=new Array();
		if(typeof s != 'string')return;
		var u=s.split(/\s*\,\s*/ );
		var v=[];
		for(var i=0;i<u.length;i++)
			v=v.concat(b(u[i],t));
		
		return v
	}
	
	function b(c,d,e){ 
		c=c.normalize().replace(" ","`");
		var f=c.match(a);
		var g,h,i,j,k,n;
		var l=[];
		
		if(f==null)f=[c,c];		
		if(f[1]=="")f[1]="*";		
		if(e==null)e="`";
		if(d==null)d=document;
		
		switch(f[2]){ 
			case "#":
				k=f[3].match(a);
				if(k==null)k=[null,f[3]];
				
				g=document.getElementById(k[1]);
				if(g==null||(f[1]!="*"&&!o(g,f[1])))return l;
				
				if(k.length==2){ 
					l.push(g);
					return l
				}
				return b(k[3],g,k[2]);
				
			case ".":
				if(e!=">")h=m(d,f[1]);
				else h=d.childNodes;
				
				for(i=0,n=h.length;i<n;i++){ 
					g=h[i];
					if(g.nodeType!=1)continue;
					k=f[3].match(a);
					if(k!=null){ 
						if(g.className==null||g.className.match("\\b"+k[1]+"\\b")==null)continue;
						j=b(k[3],g,k[2]);
						l=l.concat(j)
					}
					else if(g.className!=null&&g.className.match("\\b"+f[3]+"\\b")!=null)l.push(g)
				}
				return l;
				
			case ">":
				if(e!=">")h=m(d,f[1]);
				else h=d.childNodes;
				
				for(i=0,n=h.length;i<n;i++){ 
					g=h[i];
					if(g.nodeType!=1)continue;
					if(!o(g,f[1]))continue;
					j=b(f[3],g,">");
					l=l.concat(j)
				}
				return l;
				
			case "`":
				h=m(d,f[1]);
				for(i=0,n=h.length;i<n;i++){ 
					g=h[i];
					j=b(f[3],g,"`");
					l=l.concat(j)
				}
				return l;
				
			default:
				if(e!=">")h=m(d,f[1]);
				else h=d.childNodes;
				
				for(i=0,n=h.length;i<n;i++){ 
					g=h[i];
					if(g.nodeType!=1)continue;
					if(!o(g,f[1]))continue;
					l.push(g)
				}
				return l
		}
	}
	
	function m(d,o){ 
		if(o=="*"&&d.all!=null)return d.all;
		return d.getElementsByTagName(o)
	}
	
	function o(p,q){ 
		return q=="*"?true:p.nodeName.toLowerCase().replace("html:", "")==q.toLowerCase()
	}
	return r
}();

var sIFR=function(){ 
	var a="http://www.w3.org/1999/xhtml";
	var b=false;
	var c=false;
	var d;
	var ah=[];
	var al=document;
	var ak=al.documentElement;
	var am=window;
	var au=al.addEventListener;
	var av=am.addEventListener;
	
	var f=function(){ 
		var g=navigator.userAgent.toLowerCase();
		var f={ 
			a:g.indexOf("applewebkit")>-1,
			b:g.indexOf("safari")>-1,
			c:navigator.product!=null&&navigator.product.toLowerCase().indexOf("konqueror")>-1,
			d:g.indexOf("opera")>-1,
			e:al.contentType!=null&&al.contentType.indexOf("xml")>-1,
			f:true,
			g:true,
			h:null,
			i:null,
			j:null,
			k:null
		};
		
		f.l=f.a||f.c;
		f.m=!f.a&&navigator.product!=null&&navigator.product.toLowerCase()=="gecko";
		if(f.m)f.j=new Number(g.match(/.*gecko\/(\d{8}).*/)[1]);
		f.n=g.indexOf("msie")>-1&&!f.d&&!f.l&&!f.m;
		f.o=f.n&&g.match(/.*mac.*/)!=null;
		if(f.d)f.i=new Number(g.match(/.*opera(\s|\/)(\d+\.\d+)/)[2] );
		if(f.n||(f.d&&f.i<7.6))f.g=false;
		if(f.a)f.k=new Number(g.match(/.*applewebkit\/(\d+).*/)[1] );
		if(am.hasFlash&&(!f.n||f.o)){ 
			var aj=(navigator.plugins["Shockwave Flash 2.0"]||navigator.plugins["Shockwave Flash"]).description;
			f.h=parseInt(aj.charAt(aj.indexOf(".")-1))
		}
		if(g.match(/.*(windows|mac).*/)==null||f.o||f.c||(f.d&&(g.match(/.*mac.*/)!=null||f.i<7.6))||(f.b&&f.h<7)||(!f.b&&f.a&&f.k<124)||(f.m&&f.j<20020523))f.f=false;
		if(!f.o&&!f.m&&al.createElementNS)
			try{ 
				al.createElementNS(a,"i").innerHTML=""
			}
			catch(e){ 
				f.e=true
			}
			
		f.p=f.c||(f.a&&f.k<312)||f.n;
			
		return f
	}();
	
	function at(){ 
		return{ 
			bIsWebKit:f.a,
			bIsSafari:f.b,
			bIsKonq:f.c,
			bIsOpera:f.d,
			bIsXML:f.e,
			bHasTransparencySupport:f.f,
			bUseDOM:f.g,
			nFlashVersion:f.h,
			nOperaVersion:f.i,
			nGeckoBuildDate:f.j,
			nWebKitVersion:f.k,
			bIsKHTML:f.l,
			bIsGecko:f.m,
			bIsIE:f.n,
			bIsIEMac:f.o,
			bUseInnerHTMLHack:f.p
		}
	}
	
	if(am.hasFlash==false||!al.getElementsByTagName||!al.getElementById||(f.e&&f.p))
		return{UA:at()};
		
	function af(e){ 
		if((!k.bAutoInit&&(am.event||e)!=null)||!l(e))return;		
		b=true;
		for(var i=0,h=ah.length;i<h;i++)j.apply(null,ah[i]);
		ah=[]
	}
	
	var k=af;function l(e){ 
		if(c==false||k.bIsDisabled==true||((f.e&&f.m||f.l)&&e==null&&b==false)||(al.body==null||al.getElementsByTagName("body").length==0))return false;
		return true
	}
	
	function m(n){ 
		if(f.n)return n.replace(new RegExp("%\d{0}","g"),"%25");
		return n.replace(new RegExp("%(?!\d)","g"),"%25")
	}
	
	function as(p,q){ 
		return q=="*"?true:p.nodeName.toLowerCase().replace("html:", "")==q.toLowerCase()
	}
	
	function o(p,q,r,s,t){ 
		var u="";
		var v=p.firstChild;
		var w,x,y,z;
		if(s==null)s=0;
		if(t==null)t="";
		while(v){ 
			if(v.nodeType==3){ 
				z=v.nodeValue.replace("<","&lt;");
				switch(r){ 
					case "lower":
						u+=z.toLowerCase();
						break;
					case "upper":
						u+=z.toUpperCase();
						break;
					default:
						u+=z
				}
			}
			else if(v.nodeType==1){ 
				if(as(v,"a")&&!v.getAttribute("href")==false){ 
					if(v.getAttribute("target"))t+="&sifr_url_"+s+"_target="+v.getAttribute("target");
					t+="&sifr_url_"+s+"="+m(v.getAttribute("href")).replace(/&/g,"%26");
					u+='<a href="asfunction:_root.launchURL,'+s+'">';s++
				}
				else if(as(v,"br"))u+="<br/>";
				
				if(v.hasChildNodes()){ 
					y=o(v,null,r,s,t);
					u+=y.u;
					s=y.s;
					t=y.t
				}
				
				if(as(v,"a"))u+="</a>"}w=v;
				v=v.nextSibling;
				if(q!=null){ 
					x=w.parentNode.removeChild(w);
					q.appendChild(x)
				}
		}
		
		return{"u":u,"s":s,"t":t}
	}
	
	function A(B){ 
		if(al.createElementNS&&f.g)return al.createElementNS(a,B);
		return al.createElement(B)
	}
	
	function C(D,E,z){ 
		var p=A("param");
		p.setAttribute("name",E);
		p.setAttribute("value",z);
		D.appendChild(p)
	}
	
	function F(p,G){ 
		var H=p.className;
		if(H==null)H=G;
		else H=H.normalize()+(H==""?"":" ")+G;
		p.className=H
	}
	
	function aq(ar){ 
		var a=ak;
		if(k.bHideBrowserText==false)a=al.getElementsByTagName("body")[0];
		if((k.bHideBrowserText==false||ar)&&a)
			if(a.className==null||a.className.match(/\bsIFR\-hasFlash\b/)==null)
				F(a, "sIFR-hasFlash")
	}
	
	function j(I,J,K,L,M,N,O,P,Q,R,S,r,T){ 
		if(!l())return ah.push(arguments);
		aq();
		named.extract(arguments,{ 
			sSelector:function(ap){I=ap},
			sFlashSrc:function(ap){J=ap},
			sColor:function(ap){K=ap},
			sLinkColor:function(ap){L=ap},
			sHoverColor:function(ap){M=ap},
			sBgColor:function(ap){N=ap},
			nPaddingTop:function(ap){O=ap},
			nPaddingRight:function(ap){P=ap},
			nPaddingBottom:function(ap){Q=ap},
			nPaddingLeft:function(ap){R=ap},
			sFlashVars:function(ap){S=ap},
			sCase:function(ap){r=ap},
			sWmode:function(ap){T=ap}
		});
		
		var U=parseSelector(I);
		if(U==null||U.length==0)return false;
		if(S!=null)S="&"+S.normalize();
		else S="";
		if(K!=null)S+="&textcolor="+K;
		if(M!=null)S+="&hovercolor="+M;
		if(M!=null||L!=null)S+="&linkcolor="+(L||K);
		if(O==null)O=0;
		if(P==null)P=0;
		if(Q==null)Q=0;
		if(R==null)R=0;
		if(N==null)N="#FFFFFF";
		if(T=="transparent")
			if(!f.f)T="opaque";
		else N="transparent";
		if(T==null)T="";
		
		var p,V,W,X,Y,Z,aa,ab,ac;
		var ad=null;
		
		for(var i=0,h=U.length;i<h;i++){ 
			p=U[i];
			if(p.className!=null&&p.className.match(/\bsIFR\-replaced\b/)!=null)continue;
			V=p.offsetWidth-R-P;
			W=p.offsetHeight-O-Q;
			aa=A("span");
			aa.className="sIFR-alternate";
			ac=o(p,aa,r);
			Z="txt="+m(ac.u).replace(/\+/g,"%2B").replace(/&/g, "%26").replace(/\"/g, "%22").normalize() + S + "&w=" + V + "&h=" + W + ac.t;
			F(p,"sIFR-replaced");
			
			if(ad==null||!f.g){ 
				if(!f.g)p.innerHTML=['<embed class="sIFR-flash" type="application/x-shockwave-flash" src="',J,'" quality="best" wmode="',T,'" bgcolor="',N,'" flashvars="',Z,'" width="',V,'" height="',W,'" sifr="true"></embed>'].join("");
				else{ 
					if(f.d){ 
						ab=A("object");
						ab.setAttribute("data",J);
						C(ab,"quality","best");
						C(ab,"wmode",T);
						C(ab,"bgcolor",N)
					}
					else{ 
						ab=A("embed");
						ab.setAttribute("src",J);
						ab.setAttribute("quality","best");
						ab.setAttribute("flashvars",Z);
						ab.setAttribute("wmode",T);
						ab.setAttribute("bgcolor",N)
					}
					
					ab.setAttribute("sifr","true");
					ab.setAttribute("type","application/x-shockwave-flash");
					ab.className="sIFR-flash";
					
					if(!f.l||!f.e)ad=ab.cloneNode(true)
				}
			}
			else ab=ad.cloneNode(true);
			
			if(f.g){ 
				if(f.d)C(ab,"flashvars",Z);
				else ab.setAttribute("flashvars",Z);
				
				ab.setAttribute("width",V);
				ab.setAttribute("height",W);
				ab.style.width=V+"px";
				ab.style.height=W+"px";
				p.appendChild(ab)
			}
			
			p.appendChild(aa);
			if(f.p)p.innerHTML+=""
		}
		
		if(f.n&&k.bFixFragIdBug)setTimeout(function(){al.title=d},0)
	}
	
	function ai(){d=al.title}
	
	function ae(){ 
		if(k.bIsDisabled==true)return;
		c=true;
		if(k.bHideBrowserText)aq(true);
		if(am.attachEvent)am.attachEvent("onload",af);
		else if(!f.c&&(al.addEventListener||am.addEventListener)){ 
			if(f.a&&f.k>=132&&am.addEventListener)am.addEventListener("load",function(){setTimeout("sIFR({})",1)},false);
			else{ 
				if(al.addEventListener)al.addEventListener("load",af,false);
				if(am.addEventListener)am.addEventListener("load",af,false)
			}
		}
		else if(typeof am.onload=="function"){ 
			var ag=am.onload;
			am.onload=function(){ag();af()}
		}
		else am.onload=af;
		
		if(!f.n||am.location.hash=="")k.bFixFragIdBug=false;
		else ai()
	}
	
	k.UA=at();
	k.bAutoInit=true;
	k.bFixFragIdBug=true;
	k.replaceElement=j;
	k.updateDocumentTitle=ai;
	k.appendToClassName=F;
	k.setup=ae;
	k.debug=function(){aq(true)};
	k.debug.replaceNow=function(){ae();k()};
	k.bIsDisabled=false;
	k.bHideBrowserText=true;
	return k
}();
				
var app_nu='error in method', app_sub_nu='', os='missing test', browser='unknown type',an, rv='';

function sIFRRefresh(){

	// if the page loaded over HTTPS then the swf should be loaded over HTTPS too.
  var locHttp = location.href.split(":")[0];
  if (locHttp.toLowerCase( ) == "https"){ url = "/vag_thin.swf"; }
  if (locHttp.toLowerCase( ) == "http"){ url = "http://www1.istockphoto.com/vag_thin.swf"; }

	if ( typeof sIFR == "function" && !sIFR.UA.bIsIEMac ){
		sIFR.setup();
	}

	// Omni web does not support transparent flash
	if(app_nu == ' Omni'){ 
		if(typeof sIFR == "function"){
			sIFR.replaceElement(named({sSelector: "h1", sFlashSrc: url, sColor: "#696969", nPaddingBottom: 0, nPaddingTop: 0, sCase: ""}));
		}
	} else {
		if(typeof sIFR == "function"){
			sIFR.replaceElement(named({sSelector: "h1", sFlashSrc: url, sWmode: "transparent", sColor: "#696969", nPaddingBottom: 0, nPaddingTop: 0, sCase: ""}));
		}
	}
		// Omni web does not support transparent flash
	if(app_nu == ' Omni'){ 
		if(typeof sIFR == "function"){
			sIFR.replaceElement(named({sSelector: "h3", sFlashSrc: url, sColor: "#696969", nPaddingBottom: 0, nPaddingTop: 0, sCase: ""}));
		}
	} else {
		if(typeof sIFR == "function"){
			sIFR.replaceElement(named({sSelector: "h3", sFlashSrc: url, sWmode: "transparent", sColor: "#696969", nPaddingBottom: 0, nPaddingTop: 0, sCase: ""}));
		}
	}
}



/*
Script Name: Full Featured Javascript Browser/OS detection
Authors: Harald Hope, Tapio Markula, Websites: http://techpatterns.com/
http://www.nic.fi/~tapio1/Teaching/index1.php3
Script Source URI: http://techpatterns.com/downloads/javascript_browser_detection.php
Version 4.2.2
Copyright (C) 08 July 2005

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

Lesser GPL license text:
http://www.gnu.org/licenses/lgpl.txt

Coding conventions:
http://cvs.sourceforge.net/viewcvs.py/phpbb/phpBB2/docs/codingstandards.htm?rev=1.3
*/

/*************************************************************
Full version, use it if you are pushing css to its functional limits, and/or are using 
specialized javascript.

Remember, always use method or object testing as your first choice, for example, if ( dom ) { statement; };

This browser detection includes all possibilities I think for most browsers.
Let me know if you find an error or a failure to properly detect, or if there
is a relevant browser that has special needs for detection at our tech forum:
http://techpatterns.com/forums/forum-11.html
The main script is separated from the initial netscape 4 detection due to certain bugs in
netscape 4 when it comes to unknown things like d.getElementById. The variable declarations
of course are made first to make sure that all the variables are global through the page, 
otherwise a javascript error will occur because you are trying to use an undeclared variable.

We test for both browser type (ie, op, or moz/netscape > 6) and version number, then place 
the version number into a variable which can be tested for < or > values, such as 
if (moz && nu> 1.1){....statement....;}
This seems quite reliable, especially for Opera and Mozilla, where there is no other
easy way to get the actual version number.

For more in depth discussion of css and browser issues go to:
http://www.nic.fi/~tapio1/Teaching/DynamicMenusb.php#detections
http://www.nic.fi/~tapio1/Teaching/FAQ.php3

***************************************************************/
//initialization, browser, os detection
var d, dom, nu='', brow='', ie, ie4, ie5, ie5x, ie6, ie7;
var ns4, moz, moz_rv_sub, release_date='', moz_brow, moz_brow_nu='', moz_brow_nu_sub='', rv_full=''; 
var mac, win, old, lin, ie5mac, ie5xwin, konq, saf, op, op4, op5, op6, op7;

d=document;
n=navigator;
nav=n.appVersion;
nan=n.appName;
nua=n.userAgent;
old=(nav.substring(0,1)<4);
mac=(nav.indexOf('Mac')!=-1);
win=( ( (nav.indexOf('Win')!=-1) || (nav.indexOf('NT')!=-1) ) && !mac)?true:false;
lin=(nua.indexOf('Linux')!=-1);
// begin primary dom/ns4 test
// this is the most important test on the page
if ( !document.layers )
{
	dom = ( d.getElementById ) ? d.getElementById : false;
}
else { 
	dom = false; 
	ns4 = true;// only netscape 4 supports document layers
}
// end main dom/ns4 test

op=(nua.indexOf('Opera')!=-1);
saf=(nua.indexOf('Safari')!=-1);
konq=(!saf && (nua.indexOf('Konqueror')!=-1) ) ? true : false;
moz=( (!saf && !konq ) && ( nua.indexOf('Gecko')!=-1 ) ) ? true : false;
ie=((nua.indexOf('MSIE')!=-1)&&!op);
if (op)
{
	str_pos=nua.indexOf('Opera');
	nu=nua.substr((str_pos+6),4);
	brow = 'Opera';
}
else if (saf)
{
	str_pos=nua.indexOf('Safari');
	nu=nua.substr((str_pos+7),5);
	brow = 'Safari';
}
else if (konq)
{
	str_pos=nua.indexOf('Konqueror');
	nu=nua.substr((str_pos+10),3);
	brow = 'Konqueror';
}
// this part is complicated a bit, don't mess with it unless you understand regular expressions
// note, for most comparisons that are practical, compare the 3 digit rv nubmer, that is the output
// placed into 'nu'.
else if (moz)
{
	// regular expression pattern that will be used to extract main version/rv numbers
	pattern = /[(); \n]/;
	// moz type array, add to this if you need to
	moz_types = new Array( 'Firebird', 'Phoenix', 'Firefox', 'Galeon', 'K-Meleon', 'Camino', 'Epiphany', 
		'Netscape6', 'Netscape', 'MultiZilla', 'Gecko Debian', 'rv' );
	rv_pos = nua.indexOf( 'rv' );// find 'rv' position in nua string
	rv_full = nua.substr( rv_pos + 3, 6 );// cut out maximum size it can be, eg: 1.8a2, 1.0.0 etc
	// search for occurance of any of characters in pattern, if found get position of that character
	rv_slice = ( rv_full.search( pattern ) != -1 ) ? rv_full.search( pattern ) : '';
	//check to make sure there was a result, if not do  nothing
	// otherwise slice out the part that you want if there is a slice position
	( rv_slice ) ? rv_full = rv_full.substr( 0, rv_slice ) : '';
	// this is the working id number, 3 digits, you'd use this for 
	// number comparison, like if nu >= 1.3 do something
	nu = rv_full.substr( 0, 3 );
	for (i=0; i < moz_types.length; i++)
	{
		if ( nua.indexOf( moz_types[i]) !=-1 )
		{
			moz_brow = moz_types[i];
			break;
		}
	}
	if ( moz_brow )// if it was found in the array
	{
		str_pos=nua.indexOf(moz_brow);// extract string position
		moz_brow_nu = nua.substr( (str_pos + moz_brow.length + 1 ) ,3);// slice out working number, 3 digit
		// if you got it, use it, else use nu
		moz_brow_nu = ( isNaN( moz_brow_nu ) ) ? moz_brow_nu = nu: moz_brow_nu;
		moz_brow_nu_sub = nua.substr( (str_pos + moz_brow.length + 1 ), 8);
		// this makes sure that it's only the id number
		sub_nu_slice = ( moz_brow_nu_sub.search( pattern ) != -1 ) ? moz_brow_nu_sub.search( pattern ) : '';
		//check to make sure there was a result, if not do  nothing
		( sub_nu_slice ) ? moz_brow_nu_sub = moz_brow_nu_sub.substr( 0, sub_nu_slice ) : '';
	}
	if ( moz_brow == 'Netscape6' )
	{
		moz_brow = 'Netscape';
	}
	else if ( moz_brow == 'rv' || moz_brow == '' )// default value if no other gecko name fit
	{
		moz_brow = 'Mozilla';
	} 
	if ( !moz_brow_nu )// use rv number if nothing else is available
	{
		moz_brow_nu = nu;
		moz_brow_nu_sub = nu;
	}
	if (n.productSub)
	{
		release_date = n.productSub;
	}
}
else if (ie)
{
	str_pos=nua.indexOf('MSIE');
	nu=nua.substr((str_pos+5),3);
	brow = 'Microsoft Internet Explorer';
}
// default to navigator app name
else 
{
	brow = nan;
}
op5=(op&&(nu.substring(0,1)==5));
op6=(op&&(nu.substring(0,1)==6));
op7=(op&&(nu.substring(0,1)==7));
op8=(op&&(nu.substring(0,1)==8));
op9=(op&&(nu.substring(0,1)==9));
ie4=(ie&&!dom);
ie5=(ie&&(nu.substring(0,1)==5));
ie6=(ie&&(nu.substring(0,1)==6));
ie7=(ie&&(nu.substring(0,1)==7));
// default to get number from navigator app version.
if(!nu) 
{
	nu = nav.substring(0,1);
}
/*ie5x tests only for functionavlity. dom or ie5x would be default settings. 
Opera will register true in this test if set to identify as IE 5*/
ie5x=(d.all&&dom);
ie5mac=(mac&&ie5);
ie5xwin=(win&&ie5x);

function browser_test(){
        var spacer='=======================================\n';
        nps=n.productSub;
        nv=n.vendor;

        if(win){os='Windows';}
        else if(mac){os='MacIntosh';}
        else if(lin){os='Linux';}
        else {}

        if(d.layers)
        {
                app_nu = nav.substring(0,4);
                browser='Netscape Navigator';
        }
        else if ( !moz_brow && !moz_brow_nu )
        {
                browser = brow;
                app_nu = nu;
        }
        else if( moz_brow && moz_brow_nu )
        {
                app_nu = (moz_brow_nu_sub) ? moz_brow_nu_sub : moz_brow_nu;
                browser = moz_brow;
                (!app_nu) ? app_nu = nu : '';
        }

      
}

browser_test();
sIFRRefresh();
