/* ************************************************* */
/* Div Effects by Ryan Scherf                        */
/* ************************************************* */
var expImg = "/icom_includes/footers/img/expand.gif"; var conImg = "/icom_includes/footers/img/contract.gif"; function applyDecorators()
{ applyCollapsibleIcons(); applyClosableIcons();}
function getElementsByAttribute(oElm, strTagName, strAttributeName, strAttributeValue)
{ var arrElements = (strTagName == "*" && document.all)? document.all : oElm.getElementsByTagName(strTagName); var arrReturnElements = new Array(); var oAttributeValue = (typeof strAttributeValue != "undefined")? new RegExp("(^|\\s)" + strAttributeValue + "(\\s|$)") : null; var oCurrent; var oAttribute; for(var i=0; i<arrElements.length; i++){ oCurrent = arrElements[i]; oAttribute = oCurrent.getAttribute(strAttributeName); if(typeof oAttribute == "string" && oAttribute.length > 0){ if(typeof strAttributeValue == "undefined" || (oAttributeValue && oAttributeValue.test(oAttribute))){ arrReturnElements.push(oCurrent);}
}
}
return arrReturnElements;}
function getElementsByClassName(oElm, strTagName, strClassName){ var arrElements = (strTagName == "*" && oElm.all)? oElm.all : oElm.getElementsByTagName(strTagName); var arrReturnElements = new Array(); strClassName = strClassName.replace(/\-/g, "\\-"); var oRegExp = new RegExp("(^|\\s)" + strClassName + "(\\s|$)"); var oElement; for(var i=0; i<arrElements.length; i++){ oElement = arrElements[i]; if(oRegExp.test(oElement.className)){ arrReturnElements.push(oElement);}
}
return (arrReturnElements)
}
function applyCollapsibleWrapper()
{ var oCollapsible = getElementsByAttribute(document, "div", "collapsible", "true"); var n; for(var i = 0; i < oCollapsible.length; i++)
{ n = oCollapsible[i].childNodes[0]; while(n.nodeType != 1)
n = n.nextSibling; var header = oCollapsible[i].removeChild(n); var hTag = header.tagName.toLowerCase(); var oldHTML = oCollapsible[i].innerHTML; oCollapsible[i].innerHTML = '<' + hTag + ' style="cursor:pointer;" class="' + header.className + '" onmouseover="changeFade(this, 1);" onmouseout="changeFade(this, 0);">' + header.innerHTML + '</' + hTag + '>'; oCollapsible[i].innerHTML += '<div class="collapseWrap" style="display:none; margin-top: -10px; clear: left;">' + oldHTML + '</div>';}
}
function applyClosableIcons()
{ var closableDivs = getElementsByAttribute(document, "div", "closable", "true"); var iconDiv; var mouseOverDisplay = "Close this item"; for(var i = 0; i < closableDivs.length; i++)
{ iconDiv = '<div class="floatingIcon"><a href="javascript://" title="' + mouseOverDisplay + '" onclick="closeDivById(\'' + closableDivs[i].id + '\');"><img src="/icom_includes/footers/img/contract.gif" alt="' + mouseOverDisplay + '" title="' + mouseOverDisplay + '" /></a></div>'
closableDivs[i].innerHTML = iconDiv + closableDivs[i].innerHTML;}
}
function applyCollapsibleIcons()
{ applyCollapsibleWrapper(); var collapseDivs = getElementsByAttribute(document, "div", "collapsible", "true"); var iconDiv; var collapseImg = conImg; var mouseOverDisplay = "Expand/Contract this item"; for(var i = 0; i < collapseDivs.length; i++)
{ if(getElementsByClassName(collapseDivs[i], "div", "collapseWrap")[0].style.display == "none") collapseImg = expImg; else collapseImg = conImg; iconDiv = '<div class="floatingIcon"><a href="javascript://" title="' + mouseOverDisplay + '" onclick="toggleDiv(this.parentNode.parentNode);"><img src="' + collapseImg + '" alt="' + mouseOverDisplay + '" title="' + mouseOverDisplay + '" /></a></div>'
collapseDivs[i].innerHTML = iconDiv + collapseDivs[i].innerHTML;}
}
function toggleDiv(elm)
{ var insideDiv = getElementsByClassName(elm, "div", "collapseWrap")[0]; var collapseIcon = elm.getElementsByTagName("img"); var oIcon = getCollapseIcon(collapseIcon); if (insideDiv.style.display == "" || insideDiv.style.display == "block")
{ Effect.BlindUp(insideDiv, {duration: 0.3} ); oIcon.src = expImg;}
else
{ Effect.BlindDown(insideDiv, {duration: 0.3} ); oIcon.src = conImg;}
}
function changeFade(elm, state)
{ var collapseIcon = elm.parentNode.getElementsByTagName("img"); var oIcon = getCollapseIcon(collapseIcon); (state == 0) ? oIcon.className = "" : oIcon.className = "fade";}
function getCollapseIcon(imgList)
{ var cIcon = imgList[0]; if(imgList.length == 2) cIcon = imgList[1]; return cIcon;}
function toggleDivById(id)
{ var divObj = document.getElementById(id); if (divObj) (divObj.style.display == "" || divObj.style.display == "block") ? Effect.DropOut(divObj) : Effect.Appear(divObj,{duration: 0.3});}
function closeDivById(id)
{ Effect.DropOut(id);}
