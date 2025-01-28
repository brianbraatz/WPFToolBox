function EnterKeyFormSubmitHandler(triggeredBy, event, btnIdToSubmit) 
{
    if(event.keyCode == 13) // Handle Enter Key
    {
        event.returnValue = false;
        event.Cancel = true;
        document.getElementById(getFullIdForControl(triggeredBy, btnIdToSubmit)).click();
    }
}

function getElementByClass(tagCollection, className)
{
    var tagCount = tagCollection.length;
    for (i=0; i<tagCount; i++)
    {
        if (tagCollection[i].className==className)
        {
            return tagCollection[i];
        }
    }
    return null;
}

function expandAndCollapseDivs(divClass, divId, expandButtonClass, collapseButtonClass)
{
	var allTags = document.all ? document.all : document.getElementsByTagName("*");
	var divElement = getElementByClass(allTags, divClass);    
    var displayState = getCookie(divId);

    if (displayState == "expanded")
    {
        var expandButton = getElementByClass(allTags, expandButtonClass);
        displayContentDisplay(expandButton, "collapseButton", divId);
    }	
    else
    {
        var collapseButton = getElementByClass(allTags, collapseButtonClass);
        hideContentDisplay(collapseButton, "expandButton", divId);       
    }
}

function displayAllContentInlineDisplay(divClassToShow, divClassToHide, imgClassToShow, imgClassToHide, displayLink, hideLink)
{
    var divs = document.getElementsByTagName("div");
    var imgs = document.getElementsByTagName("img");
    
    var divsToShow = [], divsToHide = [];
    var imgsToShow = [], imgsToHide = [];
    
    for (var i = 0; i < divs.length; i++)
    {
        if (divs[i].className == divClassToShow)
            divsToShow.push(divs[i]);
        else if (divs[i].className == divClassToHide)
            divsToHide.push(divs[i]);
    }
    
    for (var i = 0; i < imgs.length; i++)
    {
        if (imgs[i].className == imgClassToShow)
            imgsToShow.push(imgs[i]);
        else if (imgs[i].className == imgClassToHide)
            imgsToHide.push(imgs[i]);
    }
    
    for (var i = 0; i < divsToShow.length; i++)
    {
        divsToShow[i].style.display = "inline";
    }
    
    for (var i = 0; i < divsToHide.length; i++)
    {
        divsToHide[i].style.display = "none";
    }
    
    for (var i = 0; i < imgsToShow.length; i++)
    {
        imgsToShow[i].style.display = "inline";
    }
    
    for (var i = 0; i < imgsToHide.length; i++)
    {
        imgsToHide[i].style.display = "none";
    }

    var displayLinkAnchor = document.getElementById(displayLink);
    if(displayLinkAnchor)
        displayLinkAnchor.style.display = "inline";
    var hideLinkAnchor = document.getElementById(hideLink);
    if(hideLinkAnchor)
        hideLinkAnchor.style.display = "none";

}

function getFullIdForControl(triggeredBy, controlId) 
{
    var fullId = triggeredBy.id.substring(0, triggeredBy.id.lastIndexOf('_')+1) + controlId;
    return fullId;
}

function displayContentDisplay(triggeredBy, buttonToDisplay, contentDiv) 
{
    var contentSection = document.getElementById(getFullIdForControl(triggeredBy, contentDiv));    
    var imageButtonToDisplay = document.getElementById(getFullIdForControl(triggeredBy, buttonToDisplay));
        
    triggeredBy.style.display = "none";
    imageButtonToDisplay.style.display = "inline";
    contentSection.style.display = "block";
    
    setCookie(contentDiv, "expanded");        
}

function hideContentDisplay(triggeredBy, buttonToDisplay, contentDiv)
{
    var contentSection = document.getElementById(getFullIdForControl(triggeredBy, contentDiv));
    var imageButtonToDisplay = document.getElementById(getFullIdForControl(triggeredBy, buttonToDisplay));

    triggeredBy.style.display = "none";
    imageButtonToDisplay.style.display = "inline";
    contentSection.style.display = "none";
    setCookie(contentDiv, "collapsed");        
}

function displayContentInlineDisplay(triggeredBy, buttonToDisplay, contentDiv, hideContentDiv) 
{
    var contentSection = document.getElementById(getFullIdForControl(triggeredBy, contentDiv));    
    var hideContentSection = document.getElementById(getFullIdForControl(triggeredBy, hideContentDiv));    
    var imageButtonToDisplay = document.getElementById(getFullIdForControl(triggeredBy, buttonToDisplay));
        
    triggeredBy.style.display = "none";
    imageButtonToDisplay.style.display = "inline";
    contentSection.style.display = "inline";
    hideContentSection.style.display = "none";
}



function setCookie(name, value, expires, path, domain, secure)
{
    document.cookie = name + "=" + escape(value) +
        ((expires) ? "; expires=" + expires.toGMTString() : "") +
        ((path) ? "; path=" + path : "") +
        ((domain) ? "; domain=" + domain : "") +
        ((secure) ? "; secure" : "");
}

function getCookie(name)
{
    var start = document.cookie.indexOf( name + "=" );
    var len = start + name.length + 1;
    if ( ( !start ) &&
         ( name != document.cookie.substring( 0, name.length ) ) )
    {
       return null;
    }
    if ( start == -1 ) return null;
    var end = document.cookie.indexOf( ";", len );
    if ( end == -1 ) end = document.cookie.length;
    return unescape( document.cookie.substring( len, end ) );
}
	
function deleteCookie(name, path, domain) 
{
    if (getCookie(name)) {
        document.cookie = name + "=" +
            ((path) ? "; path=" + path : "") +
            ((domain) ? "; domain=" + domain : "") +
            "; expires=Thu, 01-Jan-70 00:00:01 GMT";
    }
}
function getChildNodes(parentDiv,tagName,propertyName,propertyValue,fuzzyMatch){
    var ret=[];
    if(parentDiv&&parentDiv.childNodes){
        for(var i=0;i<parentDiv.childNodes.length;i++){
            if(parentDiv.childNodes[i].nodeType!=1)continue;
            if((!tagName||parentDiv.childNodes[i].tagName.toLowerCase()==tagName.toLowerCase())&&(!propertyName||(!fuzzyMatch&&parentDiv.childNodes[i][propertyName]==propertyValue)||(fuzzyMatch&&parentDiv.childNodes[i][propertyName]&&parentDiv.childNodes[i][propertyName].indexOf(propertyValue)>-1)))ret[ret.length]=parentDiv.childNodes[i];
            else ret=ret.concat(getChildNodes(parentDiv.childNodes[i],tagName,propertyName,propertyValue));
        }
    }
    return ret;
}
function getParentNode(childNode,tagName,propertyName,propertyValue,fuzzyMatch){
    var ret=null;
    var tmp=childNode.parentNode;
    while(tmp){
        if((!tagName||tmp.tagName.toLowerCase()==tagName.toLowerCase())&&(!propertyName||(!fuzzyMatch&&tmp[propertyName]==propertyValue)||(fuzzyMatch&&tmp[propertyName]&&tmp[propertyName].indexOf(propertyValue)>-1))){
            ret=tmp;
            break;
        }
        tmp=tmp.parentNode;
    }
    return ret;
}
function createNode(parentNode,attributes,tagName){
    var elt=document.createElement(tagName?tagName:'div');
    if(attributes)for(var x in attributes)elt[x]=attributes[x];
    if(parentNode)parentNode.appendChild(elt);
    return elt;
}

PagePoint = function(x, y) {
    this.x = x;
    this.y = y;
}

function getElementPosition(element) {
    var x = 0;
    var y = 0;
    while (element.offsetParent) {
        x += element.offsetLeft;
        y += element.offsetTop;
        
        element = element.offsetParent;
    }
    
    x += element.offsetLeft;
    y += element.offsetTop;
    
    return new PagePoint(x, y);
}

// AJAX LIBRARY CORRECTIONS

if(typeof(WebForm_FireDefaultButton)!='undefined'){
    if(!self.CodePlex_Preserve_WebForm_FireDefaultButton)self.CodePlex_Preserve_WebForm_FireDefaultButton = WebForm_FireDefaultButton;
    WebForm_FireDefaultButton=function(event, target) {
        if(!event.srcElement&&event.target)event.srcElement=event.target;
        return CodePlex_Preserve_WebForm_FireDefaultButton(event, target);
    }
}

Type.registerNamespace("CodePlex.UI");
/*
CodePlex.UI.Point = function(x, y) {
    this.x = x;
    this.y = y;
}

CodePlex.UI.getElementPosition = function(element) {
    var x = 0;
    var y = 0;
    while (element.offsetParent) {
        x += element.offsetLeft;
        y += element.offsetTop;
        
        element = element.offsetParent;
    }
    
    x += element.offsetLeft;
    y += element.offsetTop;
    
    return new CodePlex.UI.Point(x, y);
}

CodePlex.UI.setTextBoxFocus = function(textBox) {
    textBox.focus();
}
*/
CodePlex.UI._babel = null;
CodePlex.UI.encodeHtml = function(value, isAttribute) {
    if (!CodePlex.UI._babel) CodePlex.UI._babel = document.createElement("div");
    CodePlex.UI._babel.innerHTML = "";
    CodePlex.UI._babel.appendChild(document.createTextNode(value));
    return (isAttribute ? CodePlex.UI._babel.innerHTML.split('"').join("&quot;") : CodePlex.UI._babel.innerHTML).split(' ').join("&nbsp;");
}

// AJAX JS LIBRARY EXTENSIONS

if(typeof(originalRegularExpressionValidatorEvaluateIsValid) == 'undefined' &&
   typeof(RegularExpressionValidatorEvaluateIsValid) == 'function')
{
    originalRegularExpressionValidatorEvaluateIsValid = RegularExpressionValidatorEvaluateIsValid;
    RegularExpressionValidatorEvaluateIsValid = 
        function (val) {
            if(val.invalidinIEifonlywhitespace)
            {
                if(val.invalidinIEifonlywhitespace == 'true')
                {
                    var value = ValidatorGetValue(val.controltovalidate);
                    if(value.replace(/\s*/g,'').length == 0 && value.length > 0) return false;
                    return originalRegularExpressionValidatorEvaluateIsValid(val);
                }
            }
            
            return originalRegularExpressionValidatorEvaluateIsValid(val);
        }
}