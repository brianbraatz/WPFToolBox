Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.PositioningMode = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.PositioningMode.prototype = {
Absolute: 0,
Center: 1,
BottomLeft: 2,
BottomRight: 3,
TopLeft: 4,
TopRight: 5
}
AjaxControlToolkit.PositioningMode.registerEnum('AjaxControlToolkit.PositioningMode');AjaxControlToolkit.PopupBehavior = function(element) {
AjaxControlToolkit.PopupBehavior.initializeBase(this, [element]);this._x = 0;this._y = 0;this._positioningMode = AjaxControlToolkit.PositioningMode.Absolute;this._parentElement = null;this._parentElementID = null;this._moveHandler = null;this._firstPopup = true;this._originalParent = null;}
AjaxControlToolkit.PopupBehavior.prototype = {
get_parentElement : function() {
if (!this._parentElement && this._parentElementID) {
this.set_parentElement($get(this._parentElementID));Sys.Debug.assert(this._parentElement != null, String.format(AjaxControlToolkit.Resources.PopupExtender_NoParentElement, this._parentElementID));} 
return this._parentElement;}, 
set_parentElement : function(element) {
this._parentElement = element;this.raisePropertyChanged('parentElement');},
get_parentElementID : function() {
if (this._parentElement) return this._parentElement.id
return this._parentElementID;},
set_parentElementID : function(elementID) {
this._parentElementID = elementID;if (this.get_isInitialized()) {
this.set_parentElement($get(elementID));}
},
get_positioningMode : function() {
return this._positioningMode;},
set_positioningMode : function(mode) {
this._positioningMode = mode;this.raisePropertyChanged('positioningMode');},
get_x : function() {
return this._x;},
set_x : function(value) {
if (value != this._x) {
this._x = value;if (Sys.UI.DomElement.getVisible(this.get_element())) {
this.show();}
this.raisePropertyChanged('x');}
},
get_y : function() {
return this._y;},
set_y : function(value) {
if (value != this._y) {
this._y = value;if (Sys.UI.DomElement.getVisible(this.get_element())) {
this.show();}
this.raisePropertyChanged('y');}
}, 
hide : function() {
var elt = this.get_element();Sys.UI.DomElement.setVisible(elt, false);if (this._moveHandler) {
Sys.UI.DomEvent.removeHandler(elt, "move", this._moveHandler);this._moveHandler = null;}
if (elt.originalWidth) {
elt.style.width = elt.originalWidth + "px";elt.originalWidth = null;}
if (Sys.Browser.agent === Sys.Browser.InternetExplorer) {
var childFrame = elt._hideWindowedElementsIFrame;if (childFrame) {
childFrame.style.display = "none";}
}
}, 
show : function() {
var elt = this.get_element();Sys.UI.DomElement.setVisible(elt, true);var offsetParent = elt.offsetParent || document.documentElement;var diff;var parentBounds;if(this._parentElement) {
parentBounds = CommonToolkitScripts.getBounds(this._parentElement);var offsetParentLocation = CommonToolkitScripts.getLocation(offsetParent);diff = {x: parentBounds.x - offsetParentLocation.x, y:parentBounds.y - offsetParentLocation.y};}
else {
parentBounds = CommonToolkitScripts.getBounds(offsetParent);diff = {x:0, y:0};}
var width = elt.offsetWidth - (elt.clientLeft ? elt.clientLeft * 2 : 0);var height = elt.offsetHeight - (elt.clientTop ? elt.clientTop * 2 : 0);var position;switch (this._positioningMode) {
case AjaxControlToolkit.PositioningMode.Center:
position = {
x: Math.round(parentBounds.width / 2 - width / 2),
y: Math.round(parentBounds.height / 2 - height / 2)
};break;case AjaxControlToolkit.PositioningMode.BottomLeft:
position = {
x: 0,
y: parentBounds.height
};break;case AjaxControlToolkit.PositioningMode.BottomRight:
position = {
x: parentBounds.width - width,
y: parentBounds.height
};break;case AjaxControlToolkit.PositioningMode.TopLeft:
position = {
x: 0,
y: -elt.offsetHeight
};break;case AjaxControlToolkit.PositioningMode.TopRight:
position = {
x: parentBounds.width - width,
y: -elt.offsetHeight
};break;default:
position = {x: 0, y: 0};}
position.x += this._x + diff.x;position.y += this._y + diff.y;CommonToolkitScripts.setLocation(elt, position);if(this._firstPopup) {
elt.style.width = width + "px";}
this._firstPopup = false;var newPosition = CommonToolkitScripts.getBounds(elt);var updateNeeded = false;var documentWidth = self.innerWidth ? self.innerWidth : document.documentElement.clientWidth;if (!documentWidth) {
documentWidth = document.body.clientWidth;}
if (newPosition.x + newPosition.width > documentWidth - 5) {
position.x -= newPosition.x + newPosition.width - documentWidth + 5;updateNeeded = true;}
if (newPosition.x < 0) {
position.x -= newPosition.x;updateNeeded = true;}
if (newPosition.y < 0) {
position.y -= newPosition.y;updateNeeded = true;}
if (updateNeeded) { 
CommonToolkitScripts.setLocation(elt, position);}
elt.zIndex = 1000;if ((Sys.Browser.agent === Sys.Browser.InternetExplorer) && (Sys.Browser.version < 7)) {
var childFrame = elt._hideWindowedElementsIFrame;if (!childFrame) {
childFrame = document.createElement("iframe");childFrame.src = "javascript:'<html></html>';";childFrame.style.position = "absolute";childFrame.style.display = "none";childFrame.scrolling = "no";childFrame.frameBorder = "0";childFrame.tabIndex = "-1";childFrame.style.filter = "progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)";elt.parentNode.insertBefore(childFrame, elt);elt._hideWindowedElementsIFrame = childFrame;this._moveHandler = Function.createDelegate(this, this._onMove);Sys.UI.DomEvent.addHandler(elt, "move", this._moveHandler);}
CommonToolkitScripts.setBounds(childFrame, newPosition);childFrame.style.display = elt.style.display;if (elt.currentStyle && elt.currentStyle.zIndex) {
childFrame.style.zIndex = elt.currentStyle.zIndex;}
else if (elt.style.zIndex) {
childFrame.style.zIndex = elt.style.zIndex;}
}
},
initialize : function() {
AjaxControlToolkit.PopupBehavior.callBaseMethod(this, 'initialize');this.hide();this.get_element().style.position = "absolute";},
dispose : function() {
var elt = this.get_element();if (elt) {
if (Sys.UI.DomElement.getVisible(elt)) {
this.hide();}
if (this._originalParent) {
elt.parentNode.removeChild(elt);this._originalParent.appendChild(elt);this._originalParent = null;}
}
this._parentElement = null;AjaxControlToolkit.PopupBehavior.callBaseMethod(this, 'dispose');},
_onMove : function() {
var elt = this.get_element();if (elt._hideWindowedElementsIFrame) {
elt.parentNode.insertBefore(elt._hideWindowedElementsIFrame, elt);elt._hideWindowedElementsIFrame.style.top = elt.style.top;elt._hideWindowedElementsIFrame.style.left = elt.style.left;}
}
}
AjaxControlToolkit.PopupBehavior.registerClass('AjaxControlToolkit.PopupBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();