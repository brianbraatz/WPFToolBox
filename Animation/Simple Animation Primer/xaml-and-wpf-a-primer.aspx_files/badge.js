
function fb_isIE(){return(navigator.userAgent.toLowerCase().indexOf("msie")!=-1);}
function fb_SetImageSize(img){var image=new Image();image.src=img.getAttribute('src');var badge=fb_GetContainingBadge(img);if(badge){if(badge.className=='fb_horiz'){if(!fb_isIE()){if(image.height<72)img.style.height=image.height+'px';}
fb_HorizBadgeLoaded(badge);}else{if(image.width<108)img.style.width=image.width+'px';}}}
function fb_GetContainingBadge(elem){while(elem.parentNode){elem=elem.parentNode;if(elem.className=='fb_horiz'||elem.className=='fb_vert')return elem;}
return null;}
function fb_LogoImgLoaded(elem){var badge=elem.parentNode.nextSibling;if(badge.className=='fb_horiz'){fb_HorizBadgeLoaded(badge);}}
function fb_HorizBadgeLoaded(elem){elem.style.width='10000px';var width=elem.lastChild.offsetLeft+elem.lastChild.offsetWidth-elem.offsetLeft;elem.style.width=width+'px';if(fb_isIE()){var extraWidth=28;}else{var extraWidth=1;}
elem.parentNode.style.width=width+extraWidth+'px';}
function fb_nextimg(arr,div,num){num++;if(num>=arr.length)num=0;document.getElementById(div).innerHTML=arr[num];return num;}
function fb_previmg(arr,div,num){num--;if(num<0)num=arr.length-1;document.getElementById(div).innerHTML=arr[num];return num;}