if(!document.itxtInProg){
document.itxtInProg=1;var itxtIas='634bba4f75f9476db50f9514a77469fb',
bvie=navigator.appVersion.indexOf('MSIE')>=0&&navigator.appVersion.indexOf('Opera')<0;
function gIT(o){if(o.nodeType==3)return o.nodeValue;if(o.tagName&&('script'==o.tagName.toLowerCase()))return '';var t='';var cn=o.childNodes;for(var i=0;i<cn.length;i++)t+=gIT(cn[i]);return t;}
function itxtbtl(){if(document.body.innerText!=undefined)return itxtsW(document.body.innerText).length;return gIT(document.body).length;}
function itxtsW(x){var r=x;var v=/\s/g;if(x.search(v)!=-1)r=x.replace(v, '');return r;}
function itxtLES(u){var e=document.createElement('script');e.src=u;e.type='text/javascript';var b=document.getElementsByTagName('body')[0];b.insertBefore(e, b.firstChild);}
function itxtDSB(u){try{if(navigator.appVersion.indexOf('MSIE')>=0 && ('complete'!=document.readyState)){document.write('<s'+'cript language="javascript" src="'+u+'"></s'+'cript>');}else itxtLES(u);}catch(e){}}
function itxtHurraCookie(u){try{if(navigator.appVersion.indexOf('MSIE')>=0 && ('complete'!=document.readyState)){document.write('<img src="http://ads.hurra.de/cgi-local/view.cgi?sid=634bba4f75f9476db50f9514a77469fb" style="display:none" />');}else{var hurraImg=document.createElement('img');hurraImg.src='http://ads.hurra.de/cgi-local/view.cgi?sid=634bba4f75f9476db50f9514a77469fb';document.getElementsByTagName( "body" )[0].appendChild( hurraImg );}}catch(e){}}
var itxturl='http://windows-now.us.intellitxt.com/v3/door.jsp?ts='+(new Date()).getTime()+'&pagecl='+itxtbtl()+'&ias='+itxtIas+'&ipid=3915&sn=comments,header,commentformwrapper&kwpn=3&refurl='+document.location.href.replace(/\&/g,'%26').replace(/\'/g, '%27').replace(/\"/g, '%22').replace(/\#(.)*/,'');
itxtDSB(itxturl);
}
