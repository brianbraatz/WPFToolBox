Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.HoverMenuBehavior = function(element) {
AjaxControlToolkit.HoverMenuBehavior.initializeBase(this, [element]);this._hoverBehavior = null;this._popupBehavior = null;this._mouseEnterHandler = null;this._mouseLeaveHandler = null;this._unhoverHandler = null;this._hoverHandler = null;this._inHover = null;this._oldClass = null;this._popupElement = null;this._popupElement = null;this._hoverCssClass = null;this._offsetX = 0;this._offsetY = 0;this._popDelay = 100;this._popupPosition = null;}
AjaxControlToolkit.HoverMenuBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.HoverMenuBehavior.callBaseMethod(this, 'initialize');this._hoverHandler = Function.createDelegate(this, this._onHover);this._unhoverHandler = Function.createDelegate(this, this._onUnhover);this._mouseEnterHandler = Function.createDelegate(this, this._onmouseover);this._mouseLeaveHandler = Function.createDelegate(this, this._onmouseout);var e = this.get_element();$addHandler(e, "mouseover", this._mouseEnterHandler);$addHandler(e, "mouseout", this._mouseLeaveHandler);if (this._popupElement) {
this._popupBehavior = $create(AjaxControlToolkit.PopupBehavior, { "id":this.get_id()+"_PopupBehavior" }, null, null, this._popupElement);if (this._popupPosition) {
this._popupBehavior.set_positioningMode(AjaxControlToolkit.HoverMenuPopupPosition.Absolute);} else {
this._popupBehavior.set_positioningMode(AjaxControlToolkit.HoverMenuPopupPosition.Center);}
this._hoverBehavior = $create(AjaxControlToolkit.HoverBehavior, { "id":this.get_id()+"_HoverBehavior", "unhoverDelay":this._popDelay, "hoverElement":this._popupElement }, null, null, e);this._hoverBehavior.add_hover(this._hoverHandler);this._hoverBehavior.add_unhover(this._unhoverHandler);}
},
dispose : function() {
if (this._popupBehavior) {
this._popupBehavior.dispose();this._popupBehavior = null;}
if (this._popupElement) { 
this._popupElement = null;}
if (this._mouseEnterHandler) {
$removeHandler(this.get_element(), "mouseover", this._mouseEnterHandler);}
if (this._mouseLeaveHandler) { 
$removeHandler(this.get_element(), "mouseout", this._mouseLeaveHandler);} 
if (this._hoverBehavior) {
if (this._hoverHandler) {
this._hoverBehavior.remove_hover(this._hoverHandler);this._hoverHandler = null;}
if (this._unhoverHandler) {
this._hoverBehavior.remove_hover(this._unhoverHandler);this._unhoverHandler = null;}
this._hoverBehavior.dispose();this._hoverBehavior = null;} 
AjaxControlToolkit.HoverMenuBehavior.callBaseMethod(this, 'dispose');},
_getLeftOffset : function() {
var defaultLeft = CommonToolkitScripts.getLocation(this.get_element()).x;var delta = 0;switch(this._popupPosition) {
case AjaxControlToolkit.HoverMenuPopupPosition.Left:
delta = (-1 * this._popupElement.offsetWidth);break;case AjaxControlToolkit.HoverMenuPopupPosition.Right:
delta = this.get_element().offsetWidth;break;}
return delta + defaultLeft + this._offsetX;},
_getTopOffset : function() {
var defaultTop = CommonToolkitScripts.getLocation(this.get_element()).y;var delta = 0;switch(this._popupPosition) {
case AjaxControlToolkit.HoverMenuPopupPosition.Top:
delta = (-1 * this._popupElement.offsetHeight);break;case AjaxControlToolkit.HoverMenuPopupPosition.Bottom:
delta = this.get_element().offsetHeight;break;}
return defaultTop + delta + this._offsetY;},
_onHover : function() {
if (this._inHover) return;this._inHover = true;AjaxControlToolkit.HoverMenuBehavior.callBaseMethod(this, 'populate');this._popupBehavior.show();if (CommonToolkitScripts.getCurrentStyle(this._popupElement, 'display') == 'none') {
this._popupElement.style.display = 'block';}
this._popupBehavior.set_x(this._getLeftOffset());this._popupBehavior.set_y(this._getTopOffset());},
_onUnhover : function() {
this._inHover = false;this._resetCssClass();this._popupBehavior.hide();},
_onmouseover : function() {
var e = this.get_element();if (this._hoverCssClass && e.className != this._hoverCssClass) {
this._oldClass = e.className;e.className = this._hoverCssClass;} 
},
_onmouseout : function() {
this._resetCssClass();},
_resetCssClass : function() {
var e = this.get_element();if (!this._inHover && this._hoverCssClass && e.className == this._hoverCssClass) {
e.className = this._oldClass;}
},
get_popupElement : function() {
return this._popupElement;},
set_popupElement : function(value) {
if (this._popupElement != value) {
this._popupElement = value;if (this.get_isInitialized() && this._hoverBehavior) {
this._hoverBehavior.set_hoverElement(this._popupElement);}
this.raisePropertyChanged('popupElement');}
},
get_HoverCssClass : function() {
return this._hoverCssClass;},
set_HoverCssClass : function(value) {
if (this._hoverCssClass != value) {
this._hoverCssClass = value;this.raisePropertyChanged('HoverCssClass');}
},
get_OffsetX : function() {
return this._offsetX;},
set_OffsetX : function(value) {
if (this._offsetX != value) {
this._offsetX = value;this.raisePropertyChanged('OffsetX');}
},
get_OffsetY : function() {
return this._offsetY;},
set_OffsetY : function(value) {
if (this._offsetY != value) {
this._offsetY = value;this.raisePropertyChanged('OffsetY');}
},
get_PopupPosition : function() {
return this._popupPosition;},
set_PopupPosition : function(value) {
if (this._popupPosition != value) {
this._popupPosition = value;this.raisePropertyChanged('PopupPosition');}
},
get_PopDelay : function() {
return this._popDelay;},
set_PopDelay : function(value) {
if (this._popDelay != value) {
this._popDelay = value;this.raisePropertyChanged('PopDelay');}
}
}
AjaxControlToolkit.HoverMenuBehavior.registerClass('AjaxControlToolkit.HoverMenuBehavior', AjaxControlToolkit.DynamicPopulateBehaviorBase);AjaxControlToolkit.HoverMenuPopupPosition = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.HoverMenuPopupPosition.prototype = {
Center: 0,
Top: 1,
Left: 2,
Bottom: 3,
Right: 4
}
AjaxControlToolkit.HoverMenuPopupPosition.registerEnum('AjaxControlToolkit.HoverMenuPopupPosition');
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();