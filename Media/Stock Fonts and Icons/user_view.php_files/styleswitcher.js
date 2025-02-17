
// toggle visibility

function toggle( targetId ){
  if (document.getElementById){
  		target = document.getElementById( targetId );
  			if (target.style.display == "none"){
  				target.style.display = "";
  			} else {
  				target.style.display = "none";
  			}
  	}
}


// swap images


function newImage(arg) {
     if (document.images) {
          rslt = new Image();
          rslt.src = arg;
          return rslt;
     }
}

function changeImages() {
     if (document.images && (preloadFlag == true)) {
          for (var i=0; i<changeImages.arguments.length; i+=2) {
               document[changeImages.arguments[i]].src = changeImages.arguments[i+1];
          }
     }
}

var preloadFlag = false;
function preloadImages() {
     if (document.images) {
	blue = newImage("images/blue/toplogo.gif");
	pink = newImage("images/pink/toplogo.gif");
	lime = newImage("images/lime/toplogo.gif");
	gold = newImage("images/gold/toplogo.gif");
	grey = newImage("images/grey/toplogo.gif");
	preloadFlag = true;
     }
}

// switch styles

function setActiveStyleSheet(title) {
  var i, a, main;
  for (i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if (a.getAttribute("rel") &&
        a.getAttribute("rel").indexOf("style") != -1 &&
        a.getAttribute("title")) {
      a.disabled = true;
      if(a.getAttribute("title") == title) a.disabled = false;
    }
  }
}

function getActiveStyleSheet() {
  var i, a;
  for (i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if (a.getAttribute("rel") &&
        a.getAttribute("rel").indexOf("style") != -1 &&
        a.getAttribute("title") &&
        !a.disabled
        ) return a.getAttribute("title");
  }
  return null;
}

function getPreferredStyleSheet() {
  var i, a;
  for (i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if (a.getAttribute("rel") &&
        a.getAttribute("rel").indexOf("style") != -1 &&
        a.getAttribute("rel").indexOf("alt") == -1 &&
        a.getAttribute("title")
        ) return a.getAttribute("title");
  }
  return null;
}

function createCookie(name,value,days) {
  if (days) {
    var date = new Date();
    date.setTime(date.getTime()+(days*24*60*60*1000));
    var expires = "; expires="+date.toGMTString();
  }
  else expires = "";
  document.cookie = name+"="+value+expires+"; path=/";
}

function readCookie(name) {
  var nameEQ = name + "=";
  var ca = document.cookie.split(';');
  for(var i=0;i < ca.length;i++) {
    var c = ca[i];
    while (c.charAt(0)==' ') c = c.substring(1,c.length);
    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
  }
  return null;
}

window.onload = function(e) {
  var cookie = readCookie("style");
  var title = cookie ? cookie : getPreferredStyleSheet();
  setActiveStyleSheet(title);
}

window.onunload = function(e) {
  var title = getActiveStyleSheet();
  createCookie("style", title, 365);
}

var cookie = readCookie("style");
var title = cookie ? cookie : getPreferredStyleSheet();
setActiveStyleSheet(title);

