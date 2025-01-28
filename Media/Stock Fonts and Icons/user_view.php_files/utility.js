function PopupWindow(theURL,winName,features) { //v2.0
	window.open(theURL,winName,features);
}
function bodyElement(){
	return (!window.opera && document.compatMode && document.compatMode!="BackCompat")? document.documentElement : document.body
}
function getCookie(name){
	if (document.cookie.length>0){
  		start=document.cookie.indexOf(name + "=")
  		if (start!=-1){ 
    		start=start + name.length+1 
    		end=document.cookie.indexOf(";",start)
    		if (end==-1) {
    			end=document.cookie.length
    		}
    		return unescape(document.cookie.substring(start,end))
    	} 
  	}
	return ""
}

function toggleCustomRadio(formEle, checkedEle) {
	var radios = formEle.elements[checkedEle.name];
	for(var i=0; i<radios.length; i++){
		radios[i].checked=(radios[i].id == checkedEle.id ? true:false);
		$(radios[i].id+"Label").className=(radios[i].checked? 'radioon' :'radiooff');
	}
}

function positionPopup(parentEle, popupEle, relativeLeft, relativeTop) {
	if(parentEle && popupEle) {
		// temporary unhide the parent element to get the offsets
		if(parentEle.style.display=='none') {
			parentEle.style.display='block';
			var pos=Position.cumulativeOffset(parentEle);
			parentEle.style.display='none';
		} else {
			var pos=Position.cumulativeOffset(parentEle);
		}
		var left=(pos[0]+relativeLeft)+"px";
		var top=(pos[1]+relativeTop)+"px";
		popupEle.style.left=left;
		popupEle.style.top=top;
		
		if($(popupEle.id+"IFrame")) {
			var iFrame=$(popupEle.id+"IFrame");
			iFrame.style.left=left;
			iFrame.style.top=top;
		}
	}
}

function buildPopupIFrame(popupEle, allBrowsers) {
	if( (navigator.appName=="Microsoft Internet Explorer" || allBrowsers) && popupEle && !$(popupEle.id+"IFrame")) {
		var iFrame = document.createElement("iframe");
		iFrame.id=popupEle.id+"IFrame";
		iFrame.src="blank.html";
		iFrame.style.border="1px solid red";	
		iFrame.style.display="none";
		iFrame.style.position="absolute";
		iFrame.style.filter='progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)';
		document.body.appendChild(iFrame);
	}
}

function togglePopup(popupEle, newState, zIndex) {
	if(popupEle) {		
		if(newState) {
			newState = (newState=="block"?"block":"none");
		}else {
			newState = (popupEle.style.display == "block" ? "none" : "block");
		}
		popupEle.style.display=newState;	
		
		var popupIFrame = $(popupEle.id+"IFrame");
		if(popupIFrame) {
			if(!popupIFrame.style.width) {
				popupIFrame.style.width=popupEle.offsetWidth+"px";
				popupIFrame.style.height=popupEle.offsetHeight+"px";
				if(!zIndex) {
					zIndex = 1500;
				}
				popupEle.style.zIndex=zIndex;
				popupIFrame.style.zIndex=popupEle.style.zIndex-1;
			}
			popupIFrame.style.display=newState;
		}
	}
}

function fixSafariEncode(s){
	var fixedString = s;
	try {
		fixedString = decodeURI(escape(fixedString));
	}
	catch(err){
		//the decode from utf8 failed
	}
	return fixedString;
}
