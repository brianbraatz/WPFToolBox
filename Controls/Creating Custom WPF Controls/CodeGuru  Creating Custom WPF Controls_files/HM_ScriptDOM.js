/*HM_ScriptDOM.js
* HierMenus Version 5
* Copyright (c) 2003 Peter Belesis. All Rights Reserved.
* Originally published and documented at http://www.hiermenuscentral.com/
* Available solely from Jupitermedia Corporation under exclusive license.
* Contact hiermenus1@jupitermedia.com for more information.
*/

HM_Version="5.31";

HM_IsSafari = ((parseInt(navigator.productSub)>=20020000)&&
               (navigator.vendor.indexOf("Apple Computer")!=-1));
// 5.3.1
if(HM_IsSafari) {
	HM_BrowserPattern = /Safari\/(\d+)/;
	HM_Matches = HM_UserAgent.match(HM_BrowserPattern);
	if (HM_Matches&&HM_Matches[1]) HM_BrowserVersion = (HM_Matches[1]-0);
	else HM_BrowserVersion = 0;
} 

HM_NS6 = ((navigator.product == "Gecko")||(HM_IsSafari));
if(HM_NS6) HM_IE = HM_Konqueror = false;
else if (HM_Konqueror) HM_IE = HM_NS6 = HM_IsSafari = false;

HM_IE5M = HM_IE && HM_Mac;
HM_IE5W = HM_IE && !HM_Mac;
HM_IE50W = (HM_IE5W&&/MSIE\s*5(\.0|\s+)/i.test(HM_UserAgent)) ? true : false;
HM_IEpos = !HM_NS6 || (parseInt(navigator.productSub)>=20010710);
HM_IsEarlyGecko = (parseInt(navigator.productSub)<20010726);
HM_OperaQuirk=false;

HM_CanAssignFrameLoad = ((HM_Konqueror)||(parseInt(navigator.productSub)>=20020823));
// 5.3.1
// if(HM_IsSafari) HM_CanAssignFrameLoad = false;
if(HM_IsSafari&&(HM_BrowserVersion<100)) HM_CanAssignFrameLoad = false;

HM_a_Parameters = [
	["MenuWidth",          150,		"number"],
	["FontFamily",         "Arial,sans-serif"],
	["FontSize",           10,		"number"],
	["FontBold",           false,	"boolean"],
	["FontItalic",         false,	"boolean"],
	["FontColor",          "black"],
	["FontColorOver",      "white"],
	["BGColor",            "white"],
	["BGColorOver",        "black"],
	["ItemPadding",        3,		"number"],
	["BorderWidth",        2,		"number"],
	["BorderColor",        "red"],
	["BorderStyle",        "solid"],
	["SeparatorSize",      1,		"number"],
	["SeparatorColor",     "yellow"],
	["ImageDir",           HM_ScriptDir],
	["ImageSrc",           "HM_More_black_right.gif", "image"],
	["ImageSrcOver",       "", "image"],
	["ImageSrcLeft",       "HM_More_black_left.gif", "image"],
	["ImageSrcLeftOver",   "", "image"],
	["ImageSize",          5,		"number"],
	["ImageHeight",        9,		"number"],
	["ImageHorizSpace",    0,		"number"],
	["ImageVertSpace",     "middle"],
	["KeepHilite",         false,	"boolean"],
	["ClickStart",         false,	"boolean"],
	["ClickKill",          true,	"boolean"],
	["ChildOverlap",       20,		"number"],
	["ChildOffset",        10,		"number"],
	["ChildPerCentOver",   null,	"number"],
	["TopSecondsVisible",  .5,		"number"],
	["ChildSecondsVisible",.3,		"number"],
	["StatusDisplayBuild", 1,		"boolean"],
	["StatusDisplayLink",  1,		"boolean"],
	["UponDisplay",        null,	"delayed"],
	["UponHide",           null,	"delayed"],
	["RightToLeft",        false,	"boolean"],
	["CreateTopOnly",      0,		"boolean"],
	["ShowLinkCursor",     false,	"boolean"],
	["ScrollEnabled",      false,	"boolean"],
	["ScrollOver",         false,	"boolean"],
	["ScrollInterval",     20,	"number"],
	["ScrollBarHeight",    16,      "number"],
	["ScrollBarColor",     "lightgrey"],
	["ScrollImgSrcTop",    "HM_More_black_top.gif", "image"],
	["ScrollImgSrcBot",    "HM_More_black_bot.gif", "image"],
	["ScrollImgWidth",     9,       "number"],
	["ScrollImgHeight",    5,       "number"],
	["ScrollBothBars",     false,   "boolean"],
	["ScrollHeightMin",    30,      "number"],
	["FramesEnabled",      false,   "boolean"],
	["FramesNavFramePos",  ""],
	["FramesMainFrameName","main"],
	["WindowPadding",      15,	"number"],
	["HighestMenuNumber",  100,	"number"],
	["ReloadInterval",     200,	"number"],
	["HoverTimeTop",       0,       "number"],
	["HoverTimeTree",      0,       "number"],
	["CreateChildrenJIT",  false,	"boolean"],
	["CreateMenusOnLoad",  true,    "boolean"],
	["TopKeepInWindowX",   true,	"boolean"],
	["TopKeepInWindowY",   true,	"boolean"],
	["PreloadImages",      true,	"boolean"]
];

HM_MenuIDPrefix = "HM_Menu";
HM_ItemIDPrefix = "HM_Item";
HM_ArrayIDPrefix = "HM_Array";
HM_GroupImagePrefix = "HM_Img";
HM_GroupImageRE = new RegExp("^("+HM_GroupImagePrefix+"\\d+).*$");

Function.prototype.isFunction = true;
Function.prototype.isString = false;
String.prototype.isFunction = false;
String.prototype.isString = true;
String.prototype.isBoolean = false;
String.prototype.isNumber = false;
Number.prototype.isString = false;
Number.prototype.isFunction = false;
Number.prototype.isBoolean = false;
Number.prototype.isNumber = true;
Boolean.prototype.isString = false;
Boolean.prototype.isFunction = false;
Boolean.prototype.isBoolean = true;
Boolean.prototype.isNumber = false;
Array.prototype.itemValidation = false;
Array.prototype.isArray = true;

function HM_f_AssignParameters(paramarray){
	var ParamName = paramarray[0];
	var DefaultValue = paramarray[1];
	var FullParamName = "HM_" + ParamName;
	
	if (typeof eval("window.HM_PG_" + ParamName) == "undefined") {
		if (typeof eval("window.HM_GL_" + ParamName) == "undefined") {
			eval(FullParamName + "= DefaultValue");
		}
		else {
			eval(FullParamName + "= HM_GL_" + ParamName);
		}
	}
	else {
		eval(FullParamName + "= HM_PG_" + ParamName);
	}

	paramarray[0] = FullParamName;
	paramarray[1] = eval(FullParamName);
}

function HM_f_EvalParameters(valuenew,valueold,valuetype){
	var TestString, ParPosition;

	if(typeof valuenew == "undefined" || valuenew == null || (valuenew.isString && valuenew.length == 0)){
		return valueold;
	}

	if(valuetype != "delayed"){
		while(valuenew.isString) {
			ParPosition = valuenew.indexOf("(");
			if(ParPosition !=-1) {
				TestString = "window." + valuenew.substr(0,ParPosition);
				if (typeof eval(TestString) != "undefined" && eval(TestString).isFunction) {
					valuenew = eval(valuenew);
				}
			}
			else break;
		}
	}

	while(valuenew.isFunction) {valuenew = valuenew()}

	switch(valuetype){	
		case "number":
			while (valuenew.isString) {valuenew = eval(valuenew)}
			break;
		case "boolean":
			while (!valuenew.isBoolean) {
				valuenew = (valuenew.isNumber) ? valuenew ? true : false : eval(valuenew);
			}
			break;
		case "image":
			if (HM_ImageDir) valuenew = HM_ImageDir + valuenew;
			break;
	}

	return valuenew;
}

for (var i=0;i<HM_a_Parameters.length;i++) {
	HM_f_AssignParameters(HM_a_Parameters[i]);
	window[HM_a_Parameters[i][0]] = HM_f_EvalParameters(window[HM_a_Parameters[i][0]],null,HM_a_Parameters[i][2]);
}

if(isNaN(HM_ImageVertSpace)) {
	if((HM_ImageVertSpace!="middle")&&(HM_ImageVertSpace!="bottom")) HM_ImageVertSpace = 0;
}
else HM_ImageVertSpace-=0; 

if(HM_PreloadImages) {
	HM_PreloadedImages = [];
	var StandardImages = [HM_ImageSrc,HM_ImageSrcOver,HM_ImageSrcLeft,HM_ImageSrcLeftOver,HM_ScrollImgSrcTop,HM_ScrollImgSrcBot];
	var ImageCounter = 0;
	for(var i=0; i<StandardImages.length; i++) {
		if(StandardImages[i]) {
			HM_PreloadedImages[ImageCounter] = new Image();
			HM_PreloadedImages[ImageCounter++].src = StandardImages[i];
		}
	}
	if(window.HM_o_RolloverImages) {
		for(var f in HM_o_RolloverImages) {
			if(HM_o_RolloverImages[f]) {
				HM_PreloadedImages[ImageCounter] = new Image();
				HM_PreloadedImages[ImageCounter++].src = HM_o_RolloverImages[f];
			}
		}
	}
}

function HM_f_RTLCheck() {
	if(HM_IE5M) return false;
	var TempElement=HM_MenusTarget.document.body;
	while(!TempElement.dir&&TempElement.parentNode) TempElement=TempElement.parentNode;
	return ((typeof(TempElement.dir)=="string")&&(/^rtl$/i.test(TempElement.dir))) ? true : false;
}

HM_ChildPerCentOver = (isNaN(parseFloat(HM_ChildPerCentOver))) ? null : parseFloat(HM_ChildPerCentOver)/100;

HM_ChildMilliSecondsVisible = HM_ChildSecondsVisible * 1000;

function HM_f_ValidateArray(arrayname){
	var MenuArrayIsValid = false;
	var MenuArrayIsObject = (typeof eval("window." + arrayname) == "object");
	if(MenuArrayIsObject) { 
		var TheMenuArray = eval(arrayname);
		if(TheMenuArray.isArray && TheMenuArray.length > 1) {
			MenuArrayIsValid = true;
			if(!TheMenuArray.itemValidation) {
				while((typeof TheMenuArray[TheMenuArray.length-1] != "object") || (!TheMenuArray[TheMenuArray.length-1].isArray)) {
					TheMenuArray.length--;
				}
				TheMenuArray.itemValidation = true;
			}
		}
	}
	return MenuArrayIsValid;
}

if(typeof(HM_a_TreesToBuild)=="undefined") {
	HM_a_TreesToBuild = [];
	if (HM_CreateMenusOnLoad) {
		for(i=1; i<=HM_HighestMenuNumber; i++){
			if(HM_f_ValidateArray(HM_ArrayIDPrefix + i)) HM_a_TreesToBuild[HM_a_TreesToBuild.length] = i;
		}
	}
}

HM_Canvas = null;
HM_AreLoaded = false;
HM_UseMouseEnter = false;
HM_MenusTarget = window;
HM_FrameHasLoadHandler = false;
// 5.3.1
// HM_a_AccessErrors = ["permission","access","security"];
HM_a_AccessErrors = ["permission","access","security","unsafe"];
HM_NavUnloaded = true;
HM_ReloadTimer=null;
HM_BeingCreated = false;
HM_BuildingMenu = false;
HM_LoadCheckDone = false;
HM_IsReloading = true;
HM_a_ElementsCreated = [];
HM_TotalTrees = 0;
HM_CurrentTree = null;
HM_OtherOnLoadCalled=false;

function HM_f_StoreElement(el){
	HM_a_ElementsCreated[HM_a_ElementsCreated.length] = el;
}

function HM_f_ClearElements() {
	HM_TO_El=HM_TO_El1=HM_TO_El2=HM_TO_El3=HM_TO_El4=HM_TO_El5=null;
	HM_CurrentMenu = null;
	HM_BuildMenu = null;
	HM_ScrollMenu = null;
	HM_CurrentArray = null;
	if(HM_CurrentTree!=null) {
		HM_CurrentTree.treeParent = null;
		HM_CurrentTree.startChild = null;
		HM_CurrentTree = null;
	}
	var Eclength = HM_a_ElementsCreated.length;
	for(var i=Eclength-1; i>=0; i--){
		var TempElement = HM_a_ElementsCreated[i];
		if(TempElement) {
			TempElement.parentMenu = null;
			TempElement.parentItem = null;
			TempElement.itemElement = null;
			TempElement.currentItem = null;
			TempElement.child = null;
			TempElement.siblingBelow = null;
			TempElement.imgLyr = null;
			TempElement.scrollParent = null;
			TempElement.scrollbarTop = null;
			TempElement.scrollbarBot = null;
			TempElement.onmouseenter = null;
			TempElement.onmouseleave = null;
			TempElement.onmouseover = null;
			TempElement.onmouseout = null;
			TempElement.onclick = null;
			TempElement.onmousedown = null;
			TempElement.onselectstart = null;
			TempElement.onmousewheel = null;
			TempElement.rolloverImages = null;

			TempElement = null;
		}
		HM_a_ElementsCreated[i] = null;
	}

	for(var i=0; i<HM_TotalTrees; i++) {
		if(HM_a_TopMenus[i]) {
			HM_a_TopMenus[i].tree.startChild = null;
			HM_a_TopMenus[i].tree.treeParent = null;
			HM_a_TopMenus[i] = null;
		}
	}
	HM_TotalTrees = 0;
	HM_a_TopMenus = [];

	HM_a_ElementsCreated = [];
}

function HM_f_InitVars(){
	HM_f_ClearElements();
	HM_AreCreated=false;
	HM_UserOverMenu=false;
	HM_HideAllTimer=null;
	HM_ZIndex=5000;
	HM_ScrollTimer=null;
}

HM_f_InitVars();

if(HM_FramesEnabled){
	if((parent == self) || (!HM_FramesNavFramePos)){
		HM_FramesEnabled = false;
	}
}		

function HM_f_FrameLoad(){
	if(HM_IE5M&&HM_AreCreated) return true;
	HM_BeingCreated = true;
	HM_f_InitVars();
	HM_AreCreated = HM_f_StartIt();
	HM_IsReloading = false;
	HM_AreLoaded = true;
	HM_NavUnloaded = false;

	HM_BeingCreated = false;
	if(!HM_OtherOnLoadCalled) {
		HM_OtherOnLoadCalled=true;
		HM_f_OtherOnLoad();
	}
	return HM_AreCreated; 
}

function HM_f_Return() {
	return true;
}

function HM_f_DocumentMouseDown() {
	HM_f_PageClick();
	return HM_f_OtherMouseDown();
}

function HM_f_InitIt(){
	if((typeof(HM_MenusTarget.document.body) == "undefined") || (HM_MenusTarget.document.body == null)) return false;
	HM_IECSS = (HM_IE && HM_MenusTarget.document.compatMode) ? HM_MenusTarget.document.compatMode ==  "CSS1Compat" : false;
	HM_IEDTD = (HM_IE && HM_MenusTarget.document.doctype) ? HM_MenusTarget.document.doctype.name.indexOf(".dtd")!=-1 : HM_IECSS;
	HM_IEnoDTD = (HM_IE) && !HM_IEDTD;
	HM_GeckoRTLQuirk = (HM_NS6&&!HM_IsSafari&&(parseInt(navigator.productSub)<20030312)&&HM_f_RTLCheck());

	HM_UseMouseEnter = (typeof(HM_MenusTarget.document.body.onmouseenter) != "undefined");
	if(HM_IE || HM_Konqueror) HM_Canvas = HM_IECSS ? HM_MenusTarget.document.documentElement : HM_MenusTarget.document.body;
	if (HM_ClickKill) {
		HM_f_OtherMouseDown = (HM_MenusTarget.document.onmousedown) ? HM_MenusTarget.document.onmousedown : HM_f_Return;
		HM_MenusTarget.document.onmousedown = HM_f_DocumentMouseDown;
	}
	else {
		HM_TopMilliSecondsVisible = HM_TopSecondsVisible * 1000;
	}

	var EventTarget = HM_IE ? HM_MenusTarget.document.body : HM_MenusTarget;
	HM_f_OtherResize = (EventTarget.onresize) ? EventTarget.onresize : HM_f_Return;
	EventTarget.onresize = HM_f_ResizeHandler;
	if(HM_FramesEnabled) {
		HM_f_MainOtherOnUnload = (EventTarget.onunload) ? EventTarget.onunload : HM_f_Return;
		EventTarget.onunload = HM_f_MainUnloadHandler;
	}
	HM_MenusTarget.HM_Initialized=true;

	if ((typeof(window.HM_GL_RightToLeft)=="undefined")&&
	    (typeof(window.HM_PG_RightToLeft)=="undefined")&&
	    (typeof(window.HM_GL_R)=="undefined")&&
	    (typeof(window.HM_PG_R)=="undefined")) HM_RightToLeft = HM_f_RTLCheck();

	return true;
}

function HM_f_StartIt() {
	if(HM_FramesEnabled && HM_IE5M) return false;
	var AccessErrorFound = false;
	if(HM_FramesEnabled){
		var TargetFrame = parent.frames[HM_FramesMainFrameName];
		if(typeof TargetFrame == "undefined"){
			HM_FramesEnabled = false;
			var ReloadMain = HM_f_IsInitialized();
		}
		else {
			HM_MenusTarget = TargetFrame;
			var ReloadMain = HM_f_IsInitialized();
			if(!HM_LoadCheckDone) {
				if(HM_CanAssignFrameLoad) {
					var FrameEl=parent.document.getElementsByName(HM_FramesMainFrameName)[0];
					FrameEl.addEventListener('load',HM_f_FrameLoad,false);
					HM_FrameHasLoadHandler = true;
				} 
// 5.3.1
//				else if(parent.HM_UseFrameLoad) {
				else if(parent.HM_UseFrameLoad&&!HM_IsSafari) {
					HM_FrameHasLoadHandler = true;
					parent.HM_f_LoadMenus = HM_f_FrameLoad;
				}
				HM_LoadCheckDone = true;
				if(ReloadMain) {
					var EventTarget = HM_IE ? HM_MenusTarget.document.body : HM_MenusTarget;
					EventTarget.onunload = HM_f_MainUnloadHandler;
					HM_f_MainOtherOnUnload=HM_f_Return;
					HM_MenusTarget.location.replace(HM_MenusTarget.location.href);
					return false;
				}
			}
		}
	}
	else {
		var ReloadMain = HM_f_IsInitialized();
	}

	try{var TypeOfDocument = typeof(HM_MenusTarget.document);}
	catch(e){
		AccessErrorFound = HM_f_PermissionDenied(e);
	}
	if (AccessErrorFound) return false;
	else AccessErrorFound = false;

	if(HM_IE) {
		if (typeof(HM_MenusTarget.document) == "unknown") return false;
		if (HM_MenusTarget.document.readyState != "complete") return false;
	}

	try{
		var Initialized = ReloadMain ? false : HM_f_InitIt();
	}
	catch(e){
		AccessErrorFound = HM_f_PermissionDenied(e);
	}
	if (AccessErrorFound || !Initialized) return false;
	HM_f_MakeAllTrees();
	return true;
}

function HM_f_GetMenu(menuid) {
	var TheMenu = null;
	if(!HM_f_DocumentCheck()) return TheMenu;
	if ((HM_MenusTarget) &&
	    (HM_MenusTarget.document)) {
		TheMenu = HM_MenusTarget.document.getElementById(menuid);
	}
	return TheMenu;
}

function HM_f_DocumentCheck() {
	var ErrorFound = false;
	var theDocument = null;
	try{
		var TypeOfDocument = typeof(HM_MenusTarget.document);
		if((HM_IE)&&(typeof(HM_MenusTarget.document) == "unknown")) return false;
// 5.3.1
//		if((HM_IE)&&(HM_MenusTarget.document.readyState != "complete")) return false;
		if((HM_IE||HM_IsSafari)&&(HM_MenusTarget.document.readyState != "complete")) return false;
		theDocument = ((HM_MenusTarget)&&(HM_MenusTarget.document)) ? HM_MenusTarget.document : null;
		var DummyMenu = HM_MenusTarget.document.getElementById('HM_dummy_menu');
		if(theDocument) ErrorFound = false;
	}
	catch(e){
		ErrorFound = HM_f_PermissionDenied(e);
	}
	return (!ErrorFound);
}

function HM_f_MakeOneTree(MenuTree) {
	if(!HM_f_ValidateArray(HM_ArrayIDPrefix + MenuTree)) return null;
	HM_CurrentArray = eval(HM_ArrayIDPrefix + MenuTree);

	var TreeParams = HM_CurrentArray[0];
	var TreeHasChildren = false;
	var ItemArray = null;

	for(var i=1; i<HM_CurrentArray.length; i++) {
		ItemArray = HM_CurrentArray[i];
		if(ItemArray[ItemArray.length-1]) {TreeHasChildren = true; break}
	}

	HM_CurrentTree = {
		MenuWidth        : MenuWidth = HM_f_EvalParameters(TreeParams[0],HM_MenuWidth,"number"),
		MenuLeft         : MenuLeft = HM_f_EvalParameters(TreeParams[1],null,"delayed"),
		MenuTop          : MenuTop = HM_f_EvalParameters(TreeParams[2],null,"delayed"),
		ItemWidth        : MenuWidth - (HM_BorderWidth*2),
		FontColor        : HM_f_EvalParameters(TreeParams[3],HM_FontColor),
		FontColorOver    : HM_f_EvalParameters(TreeParams[4],HM_FontColorOver),
		BGColor          : HM_f_EvalParameters(TreeParams[5],HM_BGColor),
		BGColorOver      : HM_f_EvalParameters(TreeParams[6],HM_BGColorOver),
		BorderColor      : HM_f_EvalParameters(TreeParams[7],HM_BorderColor),
		SeparatorColor   : HM_f_EvalParameters(TreeParams[8],HM_SeparatorColor),
		TopIsPermanent   : ((MenuLeft == null) || (MenuTop == null)) ? false : HM_f_EvalParameters(TreeParams[9],false,"boolean"),
		TopIsHorizontal  : TopIsHorizontal = HM_f_EvalParameters(TreeParams[10],false,"boolean"),
		TreeIsHorizontal : TreeHasChildren ? HM_f_EvalParameters(TreeParams[11],false,"boolean") : false,
		PositionUnder    : (!TopIsHorizontal || !TreeHasChildren) ? false : HM_f_EvalParameters(TreeParams[12],false,"boolean"),
		TopImageShow     : TreeHasChildren ? HM_f_EvalParameters(TreeParams[13],true,"boolean")  : false,
		TreeImageShow    : TreeHasChildren ? HM_f_EvalParameters(TreeParams[14],true,"boolean")  : false,
		UponDisplay      : HM_f_EvalParameters(TreeParams[15],HM_UponDisplay,"delayed"),
		UponHide         : HM_f_EvalParameters(TreeParams[16],HM_UponHide,"delayed"),
		RightToLeft      : HM_f_EvalParameters(TreeParams[17],HM_RightToLeft,"boolean"),
		ClickStart	 : HM_f_EvalParameters(TreeParams[18],HM_ClickStart,"boolean"),
		TopIsVariableWidth  : HM_f_EvalParameters(TreeParams[19],false,"boolean"),
		TreeIsVariableWidth  : HM_f_EvalParameters(TreeParams[20],false,"boolean"),
		TopKeepInWindowX : HM_f_EvalParameters(TreeParams[21],HM_TopKeepInWindowX,"boolean"),
		TopKeepInWindowY : HM_f_EvalParameters(TreeParams[22],HM_TopKeepInWindowY,"boolean")
	};

	HM_BuildMenu = null;
	HM_f_MakeMenu(MenuTree);
	HM_a_TopMenus[HM_TotalTrees] = HM_CurrentTree.treeParent;
	HM_TotalTrees++;
	if(HM_CurrentTree.TopIsPermanent){
		with(HM_CurrentTree.treeParent) {
			HM_CurrentTree.treeParent.xPos = eval(HM_CurrentTree.MenuLeft);
			HM_CurrentTree.treeParent.yPos = eval(HM_CurrentTree.MenuTop);
			moveTo(HM_CurrentTree.treeParent.xPos,HM_CurrentTree.treeParent.yPos);
			style.zIndex = HM_ZIndex;
		}
		if(HM_IE5M) {
			var TimeoutCommand = "if ((HM_f_DocumentCheck())&&(HM_MenusTarget)&&(HM_MenusTarget.document)) { var HM_TO_El = HM_MenusTarget.document.getElementById('"+HM_CurrentTree.treeParent.id+"'); ";
			TimeoutCommand += "if (HM_TO_El && HM_TO_El.fixSize) HM_TO_El.fixSize(true); }";
			setTimeout(TimeoutCommand,10);
		} 
		else HM_CurrentTree.treeParent.style.visibility = "visible";
	}
	return HM_BuildMenu;
}

function HM_f_MakeAllTrees(){
	for(var t=0; t<HM_a_TreesToBuild.length; t++) {
		HM_f_MakeOneTree(HM_a_TreesToBuild[t]);
	}
	if(HM_StatusDisplayBuild) window.status = HM_TotalTrees + " Hierarchical Menu Trees Created";
}
HM_f_MakeTrees = HM_f_MakeAllTrees;

function HM_f_SetItemProperties() {
	this.tree        = HM_CurrentTree;
	this.index       = HM_BuildMenu.itemCount - 1;
	this.isLastItem  = (HM_BuildMenu.itemCount == HM_BuildMenu.maxItems);
	this.array	 = HM_BuildMenu.array[HM_BuildMenu.itemCount];
	this.dispText    = (HM_GeckoRTLQuirk&&this.menu.isHorizontal) ? "<span dir='rtl'>"+this.array[0]+"</span>" : (HM_IsSafari&&this.menu.IsVariableWidth) ? "<div>"+this.array[0]+"</div>" : this.array[0];
	this.linkText    = this.array[1];
	this.permHilite  = HM_f_EvalParameters(this.array[3],false,"boolean");
	this.hasRollover = (!this.permHilite && HM_f_EvalParameters(this.array[2],true,"boolean"));
	this.hasMore	 = this.array[4] && HM_f_ValidateArray(HM_ArrayIDPrefix + this.array[4]);
	this.childID	 = this.hasMore ? this.array[4] : null;
	this.child	 = null;

	if(HM_UseMouseEnter){
	 	this.onmouseenter  = HM_f_ItemOver;
		this.onmouseleave  = HM_f_ItemOut;
	}
	else {
	 	this.onmouseover = HM_f_ItemOver;
		this.onmouseout  = HM_f_ItemOut;
	}

	this.setItemStyle = HM_f_SetItemStyle;
	this.hoverChild   = HM_f_HoverChild;
	this.hoverTime = (this.menu == this.tree.startChild) ? HM_HoverTimeTop : HM_HoverTimeTree;

	this.showChild   = HM_f_ShowChild;
	this.showChildPosition = HM_f_ShowChildPosition;
	if(HM_IE5M) this.showChildDelay = HM_f_IE5MShowChildDelay;
	this.displayChild = HM_f_DisplayChild;
	this.itemHilite = HM_f_ItemHilite;
	this.ClickStart = (this.hasMore && this.tree.ClickStart && (this.tree.TopIsPermanent && (this.tree.treeParent==this.menu))) ? true : false;
	if(this.ClickStart) {
		this.linkText = "";
		this.onclick = this.hoverChild;
	}
	this.ChildOverlap = null;
}

function HM_f_MakeElement(menuid) {
	var MenuObject = HM_MenusTarget.document.createElement("DIV");
	HM_f_StoreElement(MenuObject);

        var MenuWidth  = ((HM_IEnoDTD) ? HM_CurrentTree.MenuWidth : HM_CurrentTree.ItemWidth) + "px";
	with(MenuObject){
		id = menuid;
		if(!HM_CurrentTree.RightToLeft&&HM_f_RTLCheck()) dir="ltr";
		with(style) {
			position = "absolute";
			visibility = "hidden";
			left = (HM_IE&&HM_f_RTLCheck()) ? "0px" : "-500px";
			top = "-2000px";
			width = MenuWidth;
		}
	}
	if(HM_ScrollEnabled){
		MenuObject.scrollParent = MenuObject.appendChild(HM_MenusTarget.document.createElement("DIV"));

		with(MenuObject.scrollParent.style) {
			position = "absolute";
			top = "0px";
			left = "0px";
			width = MenuWidth;
		}

		MenuObject.scrollParent.style.width = MenuWidth;

		MenuObject.scrollParent.top = 0;
		if(typeof document.onmousewheel != "undefined") MenuObject.onmousewheel = HM_f_DoWheelScroll;
	}
	else {
		MenuObject.scrollParent = MenuObject;
	}

	return MenuObject;
}

function HM_f_MakeMenu(menucount) {
	if(!HM_f_ValidateArray(HM_ArrayIDPrefix + menucount)) return false;
	HM_CurrentArray = eval(HM_ArrayIDPrefix + menucount);

	var NewMenu = HM_MenusTarget.document.getElementById(HM_MenuIDPrefix + menucount);
	if(NewMenu){
		var MenuParent=NewMenu.parentNode;
		MenuParent.removeChild(NewMenu);
		NewMenu=null;
	}

	NewMenu = HM_f_MakeElement(HM_MenuIDPrefix + menucount);
	if(HM_BuildMenu) {
		NewMenu.parentMenu = HM_BuildMenu;
		NewMenu.parentItem = HM_BuildMenu.itemElement;
		NewMenu.parentItem.child = NewMenu;
		NewMenu.hasParent = true;
		NewMenu.isHorizontal = HM_CurrentTree.TreeIsHorizontal;
		NewMenu.showImage = HM_CurrentTree.TreeImageShow;
	}
	else {
		NewMenu.isHorizontal = HM_CurrentTree.TopIsHorizontal;
		NewMenu.showImage = HM_CurrentTree.TopImageShow;
		HM_CurrentTree.treeParent = HM_CurrentTree.startChild = NewMenu;
	}

	HM_BuildMenu = NewMenu;
	HM_BuildMenu.array = [HM_CurrentArray[0]];
	var j=HM_CurrentArray.length - 1;
	var ReverseArrays = HM_f_RTLCheck();
	for(var i=1; i<HM_CurrentArray.length; i++) {
		if(HM_CurrentArray[i][4]) HM_CurrentArray[i][4] = menucount + "_" + i;
		else HM_CurrentArray[i][4] = "";
		if(ReverseArrays&&HM_BuildMenu.isHorizontal) {
			HM_BuildMenu.array[j] = HM_CurrentArray[i];
			j--;
		}
		else HM_BuildMenu.array[i] = HM_CurrentArray[i];
	}

	HM_BuildMenu.tree  = HM_CurrentTree;
	HM_BuildMenu.itemCount = 0;
	HM_BuildMenu.maxItems = HM_BuildMenu.array.length - 1;
	HM_BuildMenu.IsVariableWidth = ((HM_BuildMenu.hasParent && HM_CurrentTree.TreeIsVariableWidth) || (!HM_BuildMenu.hasParent && HM_CurrentTree.TopIsVariableWidth));
	HM_BuildMenu.showIt = HM_f_ShowIt;
	HM_BuildMenu.count = menucount;
	HM_BuildMenu.keepInWindow = HM_f_KeepInWindow;

	if(HM_UseMouseEnter){
	 	HM_BuildMenu.onmouseenter = HM_f_MenuOver;
		HM_BuildMenu.onmouseleave = HM_f_MenuOut;
	}
	else {
	 	HM_BuildMenu.onmouseover = HM_f_MenuOver;
		HM_BuildMenu.onmouseout = HM_f_MenuOut;
	}
    
	HM_BuildMenu.hideTree = HM_f_HideTree;
	HM_BuildMenu.hideParents = HM_f_HideParents;
	HM_BuildMenu.hideChildren = HM_f_HideChildren;
	HM_BuildMenu.hideTop = HM_f_HideTop;
	HM_BuildMenu.hideSelf = HM_f_HideSelf;
	HM_BuildMenu.hasChildVisible = false;
	HM_BuildMenu.isOn = false;
	HM_BuildMenu.hideTimer = null;
	HM_BuildMenu.currentItem = null;
	HM_BuildMenu.setMenuStyle = HM_f_SetMenuStyle;
	HM_BuildMenu.sizeFixed = false;
	HM_BuildMenu.fixSize = HM_f_FixSize;
	HM_BuildMenu.scrollbarsCreated = false;
	HM_BuildMenu.createScrollbars = HM_f_CreateScrollbars;
	HM_BuildMenu.startScroll = HM_f_StartScroll;
	HM_BuildMenu.checkScroll = HM_f_CheckScroll;
	HM_BuildMenu.scrollbarsVisible = false;

	if(HM_IE) HM_BuildMenu.onselectstart = HM_f_CancelSelect;
	HM_BuildMenu.moveTo = HM_f_MoveTo;
	HM_BuildMenu.setMenuStyle();

	if(HM_IE5M && HM_ScrollEnabled && !HM_BuildMenu.isHorizontal && !(HM_CurrentTree.PositionUnder && HM_BuildMenu.parentMenu == HM_CurrentTree.treeParent) && !(HM_CurrentTree.TopIsPermanent && HM_BuildMenu == HM_CurrentTree.treeParent)) {
		HM_BuildMenu.createScrollbars();
	}

	while (HM_BuildMenu.itemCount < HM_BuildMenu.maxItems) {
		HM_BuildMenu.itemCount++;
		HM_BuildMenu.itemElement = HM_MenusTarget.document.getElementById(HM_ItemIDPrefix + menucount + "_" + HM_BuildMenu.itemCount);

		if(!HM_BuildMenu.itemElement){
			if(HM_StatusDisplayBuild) window.status = "Creating Hierarchical Menus: " + menucount + " / " + HM_BuildMenu.itemCount;
			HM_BuildMenu.itemElement = HM_f_MakeItemElement(menucount);
		}
		if(!HM_CreateChildrenJIT && HM_BuildMenu.itemElement.hasMore && (!HM_CreateTopOnly || HM_BuildMenu.hasParent)) {
			MenuCreated = HM_f_MakeMenu(HM_BuildMenu.itemElement.childID);
			if(MenuCreated) {
				HM_BuildMenu = HM_BuildMenu.parentMenu;
			}
		}
	}

	HM_MenusTarget.document.body.appendChild(HM_BuildMenu);

	if(!HM_IE5M) HM_BuildMenu.fixSize();
	return HM_BuildMenu;
}

function HM_f_SetMenuStyle(){
	with(this.style) {
		borderWidth = HM_BorderWidth + "px";
		borderColor = HM_CurrentTree.BorderColor;
		borderStyle = HM_BorderStyle;
		overflow    = "hidden";
		cursor      = "default";
	}
}

function HM_f_MakeItemElement(menucount) {
	var ItemElement = HM_MenusTarget.document.createElement("DIV");
	ItemElement.id = HM_ItemIDPrefix + menucount + "_" + HM_BuildMenu.itemCount;
	ItemElement.style.position = "absolute";
	ItemElement.style.visibility = "inherit";
	if(HM_GeckoRTLQuirk&&HM_BuildMenu.isHorizontal) {
		ItemElement.dir="ltr";
		ItemElement.style.textAlign="right";
	}

	HM_BuildMenu.scrollParent.appendChild(ItemElement);

	HM_f_StoreElement(ItemElement);

	ItemElement.menu = HM_BuildMenu;
	ItemElement.childTimer = null;
	ItemElement.setItemProperties = HM_f_SetItemProperties;
	ItemElement.setItemProperties();
	ItemElement.siblingBelow = ItemElement.previousSibling;
	if(ItemElement.linkText && !ItemElement.ClickStart) {
		ItemElement.onclick = HM_f_LinkIt;
		if(HM_ShowLinkCursor)ItemElement.style.cursor = (HM_NS6 || HM_Konqueror) ? "pointer" : "hand";
	}

	ItemElement.hasImage = (ItemElement.hasMore && HM_BuildMenu.showImage);
	if(ItemElement.hasImage) {
		var ImageElement = HM_MenusTarget.document.createElement("IMG");

		HM_f_StoreElement(ImageElement);

		ItemElement.imageSrc = HM_CurrentTree.RightToLeft ? HM_ImageSrcLeft : HM_ImageSrc;
		if(!HM_CurrentTree.RightToLeft) {
			ItemElement.hasImageRollover = (HM_ImageSrcOver&&(HM_ImageSrcOver!=HM_ImageSrc));
		}
		else {
			ItemElement.hasImageRollover = (HM_ImageSrcLeftOver&&(HM_ImageSrcLeftOver!=HM_ImageSrcLeft));
		}
		if(ItemElement.hasImageRollover) {
			ItemElement.imageSrcOver = HM_CurrentTree.RightToLeft ? HM_ImageSrcLeftOver : HM_ImageSrcOver;
			if(!ItemElement.imageSrcOver) ItemElement.imageSrcOver = ItemElement.imageSrc;
			if(ItemElement.permHilite) ItemElement.imageSrc = ItemElement.imageSrcOver;
		}

		with(ImageElement){
			width=HM_ImageSize;
			height=HM_ImageHeight;
			if(ItemElement.imageSrc) src = ItemElement.imageSrc;
			hspace = 0;
			vspace = 0;
			with(style) {
				position = "absolute";
				if(typeof(HM_ImageVertSpace)=="number") top = (HM_ImageVertSpace + HM_ItemPadding) + "px";
				else top = HM_ItemPadding + "px";
				if(HM_CurrentTree.RightToLeft) {
					left = (HM_ItemPadding + HM_ImageHorizSpace) + "px";
				}
			}
		}
		ItemElement.imgLyr = ImageElement;
	}
	ItemElement.innerHTML = ItemElement.dispText;
	if(ImageElement) ItemElement.insertBefore(ImageElement,ItemElement.firstChild);
	ItemElement.setItemStyle();

	if(window.HM_o_RolloverImages&&!HM_IE5M) {
		var EmbeddedImages = ItemElement.getElementsByTagName("img");
		for (var i=0; i<EmbeddedImages.length; i++) {
			var ImageId = EmbeddedImages[i].id;
			if(typeof(ImageId)!="undefined") ImageId=ImageId.replace(HM_GroupImageRE,"$1");
			if(ImageId&&HM_o_RolloverImages[ImageId]) {
				if(!ItemElement.rolloverImages) ItemElement.rolloverImages=[];
				EmbeddedImages[i].imageSrc=EmbeddedImages[i].src;
				EmbeddedImages[i].imageSrcOver=HM_o_RolloverImages[ImageId];
				ItemElement.rolloverImages[ItemElement.rolloverImages.length]=EmbeddedImages[i];
			}
		}
	}
	return ItemElement;
}

function HM_f_SetItemStyle() {
	with(this.style){
		backgroundColor = (this.permHilite) ? HM_CurrentTree.BGColorOver : HM_CurrentTree.BGColor;
		color = (this.permHilite) ? HM_CurrentTree.FontColorOver : HM_CurrentTree.FontColor;
		padding = HM_ItemPadding +"px";
		font = ((HM_FontBold) ? "bold " : "normal ") + HM_FontSize + "pt " + HM_FontFamily;
		fontStyle = (HM_FontItalic) ? "italic" : "normal";
		if(HM_IE) overflow = "hidden";

		if((this.menu.showImage && (!this.menu.IsVariableWidth || (this.menu.IsVariableWidth && this.tree.RightToLeft && !this.menu.isHorizontal))) || (this.menu.IsVariableWidth && this.imgLyr)) {
			var FullPadding  = (HM_ItemPadding*2) + HM_ImageSize + HM_ImageHorizSpace;
			if (HM_CurrentTree.RightToLeft) paddingLeft = FullPadding + "px";
			else paddingRight = FullPadding + "px";
		}
		if(!this.isLastItem) {
			var SeparatorString = HM_SeparatorSize + "px solid " + this.tree.SeparatorColor;
			if (this.menu.isHorizontal) borderRight = SeparatorString;
			else borderBottom = SeparatorString;
		}

		if(this.menu.isHorizontal){
			top = "0px";
			if(!this.menu.IsVariableWidth) {
				if(HM_IEnoDTD){
					if(this.isLastItem) width = (HM_CurrentTree.MenuWidth - HM_BorderWidth - HM_SeparatorSize) + "px";
					else width = (HM_CurrentTree.MenuWidth - HM_BorderWidth) + "px";
					left = (this.index * (HM_CurrentTree.MenuWidth - HM_BorderWidth)) + "px";
				}
				else {
					width = (HM_CurrentTree.MenuWidth - HM_BorderWidth - parseInt(paddingLeft) - parseInt(paddingRight) - HM_SeparatorSize) + "px";
					left = ((this.index * parseInt(width)) + ((HM_SeparatorSize * this.index)))  + ((parseInt(paddingLeft) + parseInt(paddingRight)) * this.index) + "px";
				}
				var LeftAndWidth = parseInt(left) + parseInt(width);
				this.menu.style.width = this.menu.scrollParent.style.width = LeftAndWidth + ((HM_IEnoDTD) ? (HM_BorderWidth * 2) : (parseInt(paddingLeft) + parseInt(paddingRight))) + "px";
			}
		}
		else {
			left = "0px";
			if(!this.menu.IsVariableWidth) {
				if(HM_IEnoDTD) width = HM_CurrentTree.ItemWidth + "px";
				else width = (HM_CurrentTree.ItemWidth - (parseInt(paddingLeft) + parseInt(paddingRight))) + "px";
			}
		}
	}
}

function HM_f_FixSize(makevis){
	var Items = this.scrollParent.childNodes;
	var ItemCount = Items.length;
	var TempItem;

	if(this.isHorizontal) {
		if(this.IsVariableWidth) {
			for(var i=0; i<ItemCount; i++) {
				TempItem = Items[i];
				TempItem.realWidth = (HM_IE) ? TempItem.scrollWidth : TempItem.offsetWidth;
				if (isNaN(TempItem.realWidth)) TempItem.realWidth=TempItem.offsetWidth;
				if(HM_IE5M) TempItem.realWidth += (parseInt(TempItem.style.paddingLeft) + parseInt(TempItem.style.paddingRight));
		                if(HM_IEnoDTD){
					TempItem.realWidth=Math.min(TempItem.realWidth,this.tree.ItemWidth);
					if(TempItem.isLastItem) TempItem.style.width = (TempItem.realWidth) + "px";
					else TempItem.style.width = (TempItem.realWidth + HM_SeparatorSize) + "px";
					TempItem.style.left = (TempItem.index ? parseInt(TempItem.siblingBelow.style.left) + parseInt(TempItem.siblingBelow.style.width) : 0) + "px";
				}
				else { 
					TempItem.realWidth -= (parseInt(TempItem.style.paddingLeft) + parseInt(TempItem.style.paddingRight));
					if(!HM_IECSS && !HM_IE5M && !TempItem.isLastItem) TempItem.realWidth -= HM_SeparatorSize;
					TempItem.allowableWidth = TempItem.tree.ItemWidth - (parseInt(TempItem.style.paddingLeft) + parseInt(TempItem.style.paddingRight));
					TempItem.style.width = Math.min(TempItem.allowableWidth,TempItem.realWidth) + "px";
					TempItem.style.left = (TempItem.index ? (parseInt(TempItem.siblingBelow.style.left) + TempItem.siblingBelow.offsetWidth) : 0) + "px";
				}
				if(TempItem.isLastItem) {
					LeftAndWidth = parseInt(TempItem.style.left) + parseInt(TempItem.style.width);
					this.style.width = this.scrollParent.style.width = LeftAndWidth + ((HM_IEnoDTD) ? (HM_BorderWidth * 2) : (parseInt(TempItem.style.paddingLeft) + parseInt(TempItem.style.paddingRight))) + "px";
				}
			}
		}

		var MaxItemHeight = 0;
		for(var i=0; i<ItemCount; i++) {
			TempItem = Items[i];
			if(TempItem.index) {
				var ItemHeight = TempItem.offsetHeight - ((HM_IEnoDTD) ? 0 : (HM_ItemPadding * 2));
				MaxItemHeight = Math.max(MaxItemHeight,ItemHeight);
			}
			else {
				MaxItemHeight = TempItem.offsetHeight - ((HM_IEnoDTD) ? 0 : (HM_ItemPadding * 2));
			}
		}
		for(var i=0; i<ItemCount; i++) {
			TempItem = Items[i];
			TempItem.style.height = MaxItemHeight +"px";
			if(TempItem.imgLyr) {
				if(this.tree.RightToLeft){
					TempItem.imgLyr.style.left = (HM_ItemPadding + HM_ImageHorizSpace) + "px";
				}
				else {
					TempItem.imgLyr.style.left = (TempItem.offsetWidth - ((TempItem.isLastItem ? 0 : HM_SeparatorSize) + HM_ItemPadding + HM_ImageHorizSpace + HM_ImageSize)) + "px";
				}
				if(typeof(HM_ImageVertSpace)!="number") {
					if(HM_ImageVertSpace=="bottom") {
						TempItem.imgLyr.style.top = Math.max((TempItem.offsetHeight-HM_ImageHeight-HM_ItemPadding),HM_ItemPadding) + "px";
					}
					else {
						TempItem.imgLyr.style.top = parseInt(Math.max((TempItem.offsetHeight-HM_ImageHeight),HM_ItemPadding)/2) + "px";
					}
				}
				TempItem.imgLyr.height=HM_ImageHeight;
				TempItem.imgLyr.width=HM_ImageSize;
			}
		}
		this.style.height = this.scrollParent.style.height = MaxItemHeight + ((HM_IEnoDTD) ? HM_BorderWidth * 2 : (HM_ItemPadding * 2)) + "px";
	}
	else {
		if(this.IsVariableWidth) {
			var MaxItemWidth = 0;
			for(var i=0; i<ItemCount; i++) {
				TempItem = Items[i];
				TempItem.realWidth = (HM_IE) ? TempItem.scrollWidth : TempItem.offsetWidth;
				if (isNaN(TempItem.realWidth)) TempItem.realWidth=TempItem.offsetWidth;
				if(HM_IE5M) TempItem.realWidth += (parseInt(TempItem.style.paddingLeft) + parseInt(TempItem.style.paddingRight));
				MaxItemWidth = Math.min((Math.max(MaxItemWidth,TempItem.realWidth)),this.tree.ItemWidth);
			}
			for(var i=0; i<ItemCount; i++) {
				if(HM_IEnoDTD) {
					Items[i].style.width=MaxItemWidth + "px";
				}
				else {
					Items[i].style.width=(MaxItemWidth - parseInt(Items[i].style.paddingRight) - parseInt(Items[i].style.paddingLeft)) + "px";
				}
			}
			this.style.width = this.scrollParent.style.width = (Items[0].offsetWidth + ((HM_IEnoDTD) ? HM_BorderWidth * 2 : 0)) + "px";
		}
		for(var i=0; i<ItemCount; i++) {
			var TempItem = Items[i];
			if (TempItem.index) {
				var SiblingHeight =(TempItem.siblingBelow.offsetHeight);
				TempItem.style.top = parseInt(TempItem.siblingBelow.style.top) + SiblingHeight + "px";
			}
			else TempItem.style.top = "0px";

			if(TempItem.imgLyr) {
				if(this.tree.RightToLeft){
					TempItem.imgLyr.style.left = (HM_ItemPadding + HM_ImageHorizSpace) + "px";
				}
				else {
					TempItem.imgLyr.style.left = (TempItem.offsetWidth - (HM_ItemPadding + HM_ImageHorizSpace + HM_ImageSize)) + "px";
				}
				if(typeof(HM_ImageVertSpace)!="number") {
					var ItemHeight = TempItem.offsetHeight-(TempItem.isLastItem?0:HM_SeparatorSize);
					if(HM_ImageVertSpace=="bottom") {
						TempItem.imgLyr.style.top = Math.max((ItemHeight-HM_ImageHeight-HM_ItemPadding),HM_ItemPadding) + "px";
					}
					else {
						TempItem.imgLyr.style.top = parseInt(Math.max((ItemHeight-HM_ImageHeight),HM_ItemPadding)/2) + "px";
					}
				}
				TempItem.imgLyr.height=HM_ImageHeight;
				TempItem.imgLyr.width=HM_ImageSize;
			}
		}
		this.style.height = this.scrollParent.style.height = parseInt(TempItem.style.top) + (HM_IE5W ? TempItem.scrollHeight : TempItem.offsetHeight) + ((HM_IEnoDTD) ? (HM_BorderWidth * 2) : 0) + "px";
	}
	this.origHeight = this.style.height;

	if(HM_IE5M) {
		if(this.scrollbarsCreated) {
			with(this.scrollbarTop.style){
				width = (parseInt(this.style.width) - (HM_IEnoDTD ? (HM_BorderWidth * 2) : 0)) + "px";
				height = (HM_ScrollBarHeight - (HM_IEnoDTD ? 0 : (HM_BorderWidth * 2))) + "px";
			}
			with(this.scrollbarTop.firstChild.style){
				top = ((HM_ScrollBarHeight - (HM_BorderWidth * 2) - HM_ScrollImgHeight)/2) + "px";
				left = ((parseInt(this.scrollbarTop.style.width) - HM_ScrollImgWidth)/2)+"px";
			}
			with(this.scrollbarBot.style){
				width = (parseInt(this.style.width) - (HM_IEnoDTD ? (HM_BorderWidth * 2) : 0)) + "px";
				height = (HM_ScrollBarHeight - (HM_IEnoDTD ? 0 : (HM_BorderWidth * 2))) + "px";
			}
			with(this.scrollbarBot.firstChild.style){
				top = ((HM_ScrollBarHeight - (HM_BorderWidth * 2) - HM_ScrollImgHeight)/2) + "px";
				left = ((parseInt(this.scrollbarBot.style.width) - HM_ScrollImgWidth)/2)+"px";
			}
		}
		if(window.HM_o_RolloverImages) {
			for(var j=0; j<ItemCount; j++) {
				TempItem = Items[j];
				var EmbeddedImages = TempItem.getElementsByTagName("img");
				for (var i=0; i<EmbeddedImages.length; i++) {
					var ImageId = EmbeddedImages[i].id;
					if(typeof(ImageId)!="undefined") ImageId=ImageId.replace(HM_GroupImageRE,"$1");
					if(ImageId&&HM_o_RolloverImages[ImageId]) {
						if(!TempItem.rolloverImages) TempItem.rolloverImages=[];
						EmbeddedImages[i].imageSrc=EmbeddedImages[i].src;
						EmbeddedImages[i].imageSrcOver=HM_o_RolloverImages[ImageId];
						TempItem.rolloverImages[TempItem.rolloverImages.length]=EmbeddedImages[i];
					}
				}
			}
		}
	}
	this.sizeFixed = true;
	if(makevis)this.style.visibility = "visible";
}

function HM_f_PopUp(menuname,e){
	if(!HM_NS6) e = event;

	if(!HM_AreLoaded) return;
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;

	if(!HM_BeingCreated && !HM_AreCreated){
		if(HM_FrameHasLoadHandler) return;
		if(!HM_f_FrameLoad()) return;
	}

	menuname = menuname.replace("elMenu",HM_MenuIDPrefix);
	var TempMenu = HM_MenusTarget.document.getElementById(menuname);
	if(!TempMenu&&!HM_CreateMenusOnLoad) {
		if(HM_BeingCreated||!HM_AreCreated) return;
		var MenuTree = parseInt(menuname.replace(HM_MenuIDPrefix,""));
		if(isNaN(MenuTree)) return;
		TempMenu=HM_MenusTarget.document.getElementById(HM_MenuIDPrefix+MenuTree);
		if(TempMenu) return;
		TempMenu = HM_f_MakeOneTree(MenuTree);
	}

	if(!TempMenu) return;

	if (TempMenu.tree.ClickStart) {
		var ClickElement = (HM_NS6) ? e.target : e.srcElement;
		if(HM_NS6) {
			while(ClickElement.tagName==null){
				ClickElement = ClickElement.parentNode;
			}
		}
		var OnClickFunction = "return HM_f_PopMenu(event,'" + menuname + "');";
		ClickElement.onclick = new Function("event",OnClickFunction);
	}
	else HM_f_PopMenu(e,menuname);
}

function HM_f_PopMenu(e,menuname){
	if (!HM_AreLoaded || !HM_AreCreated) return true;
	if (HM_IsReloading||!HM_f_DocumentCheck()) return true;
	if(HM_IE) e = event;
	var TempMenu = HM_MenusTarget.document.getElementById(menuname);
	if(!TempMenu) return true;
	HM_CurrentMenu = TempMenu;
	if (HM_CurrentMenu.tree.ClickStart && e.type != "click") return true;
	var mouse_x_position, mouse_y_position;
	HM_f_HideAll();
	HM_CurrentMenu.hasParent = false;
	HM_CurrentMenu.tree.startChild = HM_CurrentMenu;

	if(!HM_FramesEnabled){
		mouse_x_position = (HM_NS6) ? e.pageX : (e.clientX + HM_Canvas.scrollLeft);
		mouse_y_position = (HM_NS6) ? e.pageY : (e.clientY + HM_Canvas.scrollTop);
	}
	else {
		switch (HM_FramesNavFramePos) {
			case "top":
				mouse_x_position = (HM_NS6) ? (e.pageX-window.pageXOffset) + HM_MenusTarget.pageXOffset : (e.clientX + HM_Canvas.scrollLeft);
				mouse_y_position = (HM_NS6) ? HM_MenusTarget.pageYOffset : HM_Canvas.scrollTop;
				break;
			case "bottom":
				mouse_x_position = (HM_NS6) ? (e.pageX-window.pageXOffset)+HM_MenusTarget.pageXOffset : (e.clientX + HM_Canvas.scrollLeft);
				mouse_y_position = (HM_NS6) ? HM_MenusTarget.pageYOffset+HM_MenusTarget.innerHeight : (HM_Canvas.scrollTop + HM_Canvas.clientHeight);
				break;
			case "right":
				mouse_x_position = (HM_NS6) ? (HM_MenusTarget.pageXOffset+HM_MenusTarget.innerWidth) : (HM_Canvas.scrollLeft+HM_Canvas.clientWidth);
				mouse_y_position = (HM_NS6) ? (e.pageY-window.pageYOffset)+HM_MenusTarget.pageYOffset : (e.clientY + HM_Canvas.scrollTop);
				break;
			case "left":
			default:
				mouse_x_position = (HM_NS6) ? HM_MenusTarget.pageXOffset : HM_Canvas.scrollLeft;
				mouse_y_position = (HM_NS6) ? (e.pageY-window.pageYOffset) + HM_MenusTarget.pageYOffset : (e.clientY + HM_Canvas.scrollTop);
				break;
		}
	}

	if (HM_IE&&!HM_IE50W&&HM_f_RTLCheck()) mouse_x_position-=(HM_Canvas.scrollWidth-HM_Canvas.clientWidth);

	HM_CurrentMenu.mouseX = mouse_x_position;
	HM_CurrentMenu.mouseY = mouse_y_position;

	HM_CurrentMenu.xIntended = HM_CurrentMenu.xPos = (HM_CurrentMenu.tree.MenuLeft!=null) ? eval(HM_CurrentMenu.tree.MenuLeft) : mouse_x_position;
	HM_CurrentMenu.yIntended = HM_CurrentMenu.yPos = (HM_CurrentMenu.tree.MenuTop!=null)  ? eval(HM_CurrentMenu.tree.MenuTop)  : mouse_y_position;
	if(HM_IE5M && !HM_CurrentMenu.sizeFixed) HM_CurrentMenu.fixSize(false);
	if(HM_CurrentMenu.scrollbarsCreated) {
		HM_CurrentMenu.style.height = HM_CurrentMenu.origHeight;
		HM_CurrentMenu.checkScroll();
	}

	HM_CurrentMenu.keepInWindow();
	HM_CurrentMenu.moveTo(HM_CurrentMenu.xPos,HM_CurrentMenu.yPos);
	HM_CurrentMenu.isOn = true;

	if((HM_IE5M) || (HM_NS6)){
		var TimeoutCommand = "if ((HM_f_DocumentCheck())&&(HM_MenusTarget)&&(HM_MenusTarget.document)) { var HM_TO_El1 = HM_MenusTarget.document.getElementById('"+HM_CurrentMenu.id+"'); ";
		TimeoutCommand += "if (HM_TO_El1 && HM_TO_El1.isOn && HM_TO_El1.showIt) HM_TO_El1.showIt(true); }";
		setTimeout(TimeoutCommand,10);
	}
	else {
		HM_CurrentMenu.showIt(true);
	}

	return false;
}

function HM_f_MenuOver() {
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if(!this.tree.startChild){this.tree.startChild = this;}
	if(this.tree.startChild == this) HM_f_HideAll(this);
	this.isOn = true;

	HM_UserOverMenu = true;
	HM_CurrentMenu = this;
	if(HM_ScrollEnabled&&(HM_CurrentMenu!=HM_ScrollMenu)) HM_f_StopScroll();
	if (this.hideTimer) clearTimeout(this.hideTimer);

	this.hideTimer = null;
	if (HM_KeepHilite) {
		if(this != this.tree.startChild){
			var ParentMenu = this.parentMenu;
			if (ParentMenu.currentItem && ParentMenu.currentItem.hasRollover) ParentMenu.currentItem.itemHilite(false);
			var ParentItem = this.parentItem;
			if(ParentItem.hasRollover) ParentItem.itemHilite(true);
			ParentMenu.currentItem = ParentItem;
		}
	}
}

function HM_f_MenuOut() {
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if(HM_IE && HM_MenusTarget.event.srcElement.contains(HM_MenusTarget.event.toElement)) return;
	this.isOn = false;
	HM_UserOverMenu = false;
	if(HM_StatusDisplayLink) window.status = "";

	var Items = this.scrollParent.childNodes;
	var ItemCount = Items.length;
	var TempItem;
	for(var i=0;i<ItemCount;i++){
		TempItem = Items[i];
		clearTimeout(TempItem.childTimer);
		TempItem.childTimer = null;
	}
	if(!HM_ClickKill) {
		clearTimeout(HM_HideAllTimer);
		HM_HideAllTimer = null;
		var TimeoutCommand = "var HM_MenuOutMenuToHide = HM_f_GetMenu('" + this.id + "'); if (HM_MenuOutMenuToHide && HM_MenuOutMenuToHide.hideTree) HM_MenuOutMenuToHide.hideTree()";
		HM_HideAllTimer = setTimeout(TimeoutCommand,HM_ChildMilliSecondsVisible);
	}
}

function HM_f_ShowChildPosition() {
	if (this.tree.PositionUnder && (this.menu == this.tree.treeParent)) {
		this.child.xPos = parseInt(this.menu.style.left) + parseInt(this.style.left) + (this.index ? HM_BorderWidth-HM_SeparatorSize : 0);
		if ((this.menu.IsVariableWidth||this.child.IsVariableWidth||this.child.isHorizontal)&&HM_RightToLeft) this.child.xPos-=((this.child.offsetWidth-this.offsetWidth)-HM_SeparatorSize-(this.isLastItem ? HM_BorderWidth : 0));
		this.child.yPos = parseInt(this.menu.style.top)  + this.menu.offsetHeight - (HM_BorderWidth);
	}
	else {
		if(this.ChildOverlap==null) {
			this.DistanceToRightEdge = parseInt(this.style.width);
			if(!HM_IEnoDTD) this.DistanceToRightEdge += (parseInt(this.style.paddingLeft)+parseInt(this.style.paddingRight)) + ((this.menu.isHorizontal && !this.isLastItem) ? HM_SeparatorSize : 0);
			if (!this.menu.isHorizontal || (this.menu.isHorizontal && this.isLastItem)) this.DistanceToRightEdge += HM_BorderWidth;
			this.DistanceToLeftEdge = (!this.menu.isHorizontal || (this.menu.isHorizontal && this.index==0)) ? HM_BorderWidth : HM_SeparatorSize;
			this.ChildOverlap = (parseInt((HM_ChildPerCentOver != null) ? (HM_ChildPerCentOver  * this.DistanceToRightEdge) : HM_ChildOverlap));
		}
		if(HM_IE5M) {
			this.oL = parseInt(this.menu.style.left) - HM_ItemPadding;
			this.oL += this.offsetLeft;
			this.oT = parseInt(this.menu.style.top) - HM_ItemPadding;
			this.oT += this.offsetTop;
			if(HM_ScrollEnabled) {
				this.oT += this.menu.scrollParent.top;
				if(HM_ScrollBothBars && this.menu.scrollbarsVisible) this.oT += HM_ScrollBarHeight;
			}
		}
		else {
			this.oL = (HM_IEpos) ? parseInt(this.menu.style.left) + HM_BorderWidth : 0;
			this.oL += this.offsetLeft;
			this.oT = (HM_IEpos) ? parseInt(this.menu.style.top) : -HM_BorderWidth;
			this.oT += this.offsetTop;
			if(HM_ScrollEnabled && HM_IEpos) {
				this.oT += this.menu.scrollParent.top;
				if(HM_ScrollBothBars && this.menu.scrollbarsVisible) this.oT += HM_ScrollBarHeight;
			}

		}
		if(this.tree.RightToLeft) {
			this.child.xPos = ((this.oL - this.DistanceToLeftEdge) + this.ChildOverlap) - this.child.offsetWidth;
		}
		else {		
			this.child.xPos = (this.oL + this.DistanceToRightEdge) - this.ChildOverlap;
		}
		this.child.yPos = this.oT + HM_ChildOffset + HM_BorderWidth;
	}

	if(!this.tree.PositionUnder || this.menu!=this.tree.treeParent) {
		if(this.child.scrollbarsCreated) {
			this.child.style.height = this.child.origHeight;
		}
		this.child.keepInWindow();
	}	
	this.child.moveTo(this.child.xPos,this.child.yPos);
}

function HM_f_IE5MShowChildDelay() {
	this.child.fixSize(false);
	this.showChildPosition();
	var TimeoutCommand = "if ((HM_f_DocumentCheck())&&(HM_MenusTarget)&&(HM_MenusTarget.document)) { var HM_TO_El2 = HM_MenusTarget.document.getElementById('"+this.id+"'); ";
	TimeoutCommand += "if (HM_TO_El2 && HM_TO_El2.displayChild) HM_TO_El2.displayChild(); }";
	setTimeout(TimeoutCommand,10);
}

function HM_f_ShowChild(){
	if(HM_BeingCreated||HM_BuildingMenu||HM_IsReloading||!HM_f_DocumentCheck()) return;
	HM_BuildingMenu=true;
	if(!this.child) {
		HM_CurrentTree = this.tree;
		HM_BuildMenu = this.menu;
		HM_BuildMenu.itemElement = this;
		this.child = HM_f_MakeMenu(this.childID);
		HM_CurrentTree = this.tree;
	}
	HM_BuildingMenu=false;

	if(this.menu.style.visibility == "hidden") return;

	if(HM_IE5M && !this.child.sizeFixed) {
		var TimeoutCommand = "if ((HM_f_DocumentCheck())&&(HM_MenusTarget)&&(HM_MenusTarget.document)) { var HM_TO_El3 = HM_MenusTarget.document.getElementById('"+this.id+"'); ";
		TimeoutCommand += "if (HM_TO_El3 && HM_TO_El3.showChildDelay) HM_TO_El3.showChildDelay(); }";
		setTimeout(TimeoutCommand,10);
	}
	else {
		this.showChildPosition();

		if((HM_IE5M) || (HM_NS6)){
			var TimeoutCommand = "if ((HM_f_DocumentCheck())&&(HM_MenusTarget)&&(HM_MenusTarget.document)) { var HM_TO_El4 = HM_MenusTarget.document.getElementById('"+this.id+"'); ";
			TimeoutCommand += "if (HM_TO_El4 && HM_TO_El4.displayChild) HM_TO_El4.displayChild(); }";
			setTimeout(TimeoutCommand,10);
		}
		else {
			this.displayChild();
		}
	}
}

function HM_f_DisplayChild(){
	if ((this.menu.currentItem == this)&&(this.menu.isOn)) {
		this.menu.hasChildVisible = true;
		this.menu.visibleChild = this.child;
		this.child.showIt(true);
	} 
}

function HM_f_ItemHilite(on) {
	if(this.rolloverImages) {
		for(var i=0; i<this.rolloverImages.length; i++) this.rolloverImages[i].src = (on) ? this.rolloverImages[i].imageSrcOver : this.rolloverImages[i].imageSrc;
	}
	if(this.hasImageRollover) this.imgLyr.src = (on) ? this.imageSrcOver : this.imageSrc;
	with(this.style){
		backgroundColor = (on) ? this.tree.BGColorOver : this.tree.BGColor;
		color = (on) ? this.tree.FontColorOver : this.tree.FontColor;
	}
}

function HM_f_ItemOver(){
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	var ItemMenu = this.menu;
	if (HM_KeepHilite) {
		if (ItemMenu.currentItem && ItemMenu.currentItem != this && ItemMenu.currentItem.hasRollover) {
			ItemMenu.currentItem.itemHilite(false);
		}
	}
	if(this.hasRollover) {
		this.itemHilite(true);
	}

	if(HM_StatusDisplayLink) window.status = this.linkText;
	ItemMenu.currentItem = this;
	
	var Items = ItemMenu.scrollParent.childNodes;
	var ItemCount = Items.length;
	var TempItem;
	for(var i=0;i<ItemCount;i++){
		TempItem = Items[i];
		clearTimeout(TempItem.childTimer);
		TempItem.childTimer = null;
	}
	
	var TimeoutCommand = "if ((HM_f_DocumentCheck())&&(HM_MenusTarget) && (HM_MenusTarget.document)) { var HM_TO_El5 = HM_MenusTarget.document.getElementById('"+this.id+"'); ";
	TimeoutCommand += "if (HM_TO_El5 && HM_TO_El5.hoverChild) HM_TO_El5.hoverChild(true); }";
	this.childTimer = setTimeout(TimeoutCommand,this.hoverTime);
}

function HM_f_HoverChild(onover){
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if (this.menu.hasChildVisible) {
		if(this.menu.visibleChild == this.child && this.menu.visibleChild.hasChildVisible) this.menu.visibleChild.hideChildren(this);
		else this.menu.hideChildren(this);
	}
	if ((this.ClickStart && (onover!=true)) || (this.hasMore && !this.ClickStart)) this.showChild();
}

function HM_f_ItemOut() {
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if(this.hasRollover){
		this.itemHilite(false);
	}
}

function HM_f_MoveTo(xPos,yPos) {
	this.style.left = xPos + "px";
	this.style.top = yPos + "px";
}

function HM_f_ShowIt(on) {
	if (!(this.tree.TopIsPermanent && (this.tree.treeParent==this))) {
		if(!this.hasParent || (this.hasParent && this.tree.TopIsPermanent && (this.tree.treeParent==this.parentMenu))) {
			var IsVisible = (this.style.visibility == "visible");
			if ((on && !IsVisible) || (!on && IsVisible))
				eval(on ? this.tree.UponDisplay : this.tree.UponHide);
		}
		if (on) this.style.zIndex = ++HM_ZIndex;
		this.style.visibility = (on) ? "visible" : "hidden";
	}
	if (HM_KeepHilite && this.currentItem && this.currentItem.hasRollover) {
		this.currentItem.itemHilite(false);
	}
	this.currentItem = null;
}

function HM_f_KeepInWindow() {
	var ExtraSpace     = (HM_IE||HM_IsSafari) ? 0 : HM_WindowPadding;
	var WindowLeftEdge = (HM_IE) ? HM_Canvas.scrollLeft   : HM_MenusTarget.pageXOffset;
	var WindowTopEdge  = (HM_IE) ? HM_Canvas.scrollTop    : HM_MenusTarget.pageYOffset;
	var WindowWidth    = (HM_IE) ? HM_Canvas.clientWidth  : HM_MenusTarget.innerWidth;
	var WindowHeight   = (HM_IE) ? HM_Canvas.clientHeight : HM_MenusTarget.innerHeight;
	if (HM_IE&&!HM_IE50W&&HM_f_RTLCheck()) WindowLeftEdge-=(HM_Canvas.scrollWidth-WindowWidth);

	var WindowRightEdge  = (WindowLeftEdge + WindowWidth) - ExtraSpace;
	var WindowBottomEdge = (WindowTopEdge + WindowHeight) - ExtraSpace;
	var MenuLeftEdge = this.xPos;
	var MenuRightEdge = MenuLeftEdge + this.offsetWidth;

	if((this.yPos < WindowTopEdge) &&
	   (this.hasParent||this.tree.TopKeepInWindowY)) this.yPos = WindowTopEdge; 

	var MenuBottomEdge = this.yPos + parseInt(this.origHeight);
	MenuBottomEdge += (HM_IEnoDTD) ? 0 : (HM_BorderWidth * 2);
	if (this.hasParent) {
		var ParentLeftEdge = this.parentItem.oL;
	}
	if ((MenuRightEdge > WindowRightEdge) && 
	    (this.hasParent||this.tree.TopKeepInWindowX)) {
		if (this.hasParent) {
			this.xPos = ((ParentLeftEdge - this.parentItem.DistanceToLeftEdge) + this.parentItem.ChildOverlap) - this.offsetWidth;
		}
		else {
			var dif = MenuRightEdge - WindowRightEdge;
			this.xPos -= dif;
		}
		this.xPos = Math.max(0,this.xPos);
	}

	if ((MenuBottomEdge > WindowBottomEdge) &&
	    (this.hasParent||this.tree.TopKeepInWindowY)) {
		var dif = MenuBottomEdge - WindowBottomEdge;
		this.yPos -= dif;
		MenuBottomEdge = WindowBottomEdge;
	}

	if ((MenuLeftEdge < WindowLeftEdge) &&
	    (this.hasParent||this.tree.TopKeepInWindowX)) {
		if (this.hasParent) {
			this.xPos = (ParentLeftEdge + this.parentItem.DistanceToRightEdge) - this.parentItem.ChildOverlap;
			MenuRightEdge = this.xPos + this.offsetWidth;
			if(MenuRightEdge > WindowRightEdge) this.xPos -= (MenuRightEdge - WindowRightEdge);
		}
		else {this.xPos = 0}
	}
	if(HM_ScrollEnabled) {
		if(((this.yPos < WindowTopEdge)&&(this.hasParent||this.tree.TopKeepInWindowY))||(MenuBottomEdge > WindowBottomEdge)){
			var BorderHeightAdjust=(HM_IEnoDTD) ? 0 : (HM_BorderWidth * 2);
			if (this.yPos < WindowTopEdge) {
				if (!HM_FramesEnabled || HM_FramesNavFramePos != "top") {
					this.yPos = (WindowTopEdge + ExtraSpace);
					var edgesOffset = (ExtraSpace * 2);
				} 
				else {
					this.yPos = WindowTopEdge;
					var edgesOffset = ExtraSpace;
				}
				this.style.height = (Math.min((parseInt(this.origHeight)),(Math.max(WindowHeight,((HM_ScrollBarHeight*2)+HM_ScrollHeightMin)))) - edgesOffset - BorderHeightAdjust) + "px";
			}
			else {
 				this.style.height = (Math.min((parseInt(this.origHeight)),(Math.max((WindowBottomEdge-this.yPos),((HM_ScrollBarHeight*2)+HM_ScrollHeightMin)))) - BorderHeightAdjust) + "px";
			}
			if(!this.scrollbarsCreated) this.createScrollbars();
			this.checkScroll();
		}
		else if(this.scrollbarsCreated){
			if(!HM_IE5W){
				if(parseInt(this.origHeight) < WindowHeight) this.style.height = this.origHeight;
			}
			this.checkScroll();
		}
	}
}

function HM_f_LinkIt() {
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if (this.linkText.indexOf("javascript:")!=-1) eval(this.linkText);
	else {
		HM_f_HideAll();
		HM_MenusTarget.location.href = this.linkText;
	}
}

function HM_f_PopDown(menuname){
	if (!HM_AreLoaded || !HM_AreCreated) return;
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	menuname = menuname.replace("elMenu",HM_MenuIDPrefix);
	var MenuToHide = HM_MenusTarget.document.getElementById(menuname);
	if(!MenuToHide)return;
	if(HM_UserOverMenu&&(HM_CurrentMenu==MenuToHide)) return;
	MenuToHide.isOn = false;
	if (!HM_ClickKill) MenuToHide.hideTop();
}

function HM_f_HideAll(callingmenu) {
	clearTimeout(HM_HideAllTimer);
	HM_HideAllTimer = null;
	for(var i=0; i<HM_TotalTrees; i++) {
		var TopMenu = HM_a_TopMenus[i].tree.startChild;
		if(TopMenu == callingmenu)continue;
		TopMenu.isOn = false;
		if (TopMenu.hasChildVisible) TopMenu.hideChildren();
		TopMenu.showIt(false);
	}    
}

function HM_f_HideAllPermanent() {
	clearTimeout(HM_HideAllTimer);
	HM_HideAllTimer = null;
	for(var i=0; i<HM_TotalTrees; i++) {
		var TopMenu = HM_a_TopMenus[i].tree.startChild;
		TopMenu.isOn = false;
		if (TopMenu.hasChildVisible) TopMenu.hideChildren();
		TopMenu.style.visibility="hidden";
	}    
}

function HM_f_HideTree() { 
	HM_HideAllTimer = null;
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if(HM_UserOverMenu) return;
	if(this.hasChildVisible) this.hideChildren();
	this.hideParents();
}

function HM_f_HideTop() {
	var TimeoutCommand = "var HM_TopMenuToHide = HM_f_GetMenu('" + this.id + "'); if (HM_TopMenuToHide && HM_TopMenuToHide.hideSelf) HM_TopMenuToHide.hideSelf()";
	(HM_ClickKill) ? this.hideSelf() : (this.hideTimer = setTimeout(TimeoutCommand,HM_TopMilliSecondsVisible));
}

function HM_f_HideSelf() {
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	this.hideTimer = null;
	if (!this.isOn && !HM_UserOverMenu) this.showIt(false);
}

function HM_f_HideParents() {
	var TempMenu = this;
	while(TempMenu.hasParent) {
		TempMenu.showIt(false);
		TempMenu.parentMenu.isOn = false;        
		TempMenu.parentMenu.hasChildVisible=false;
		TempMenu = TempMenu.parentMenu;
	}
	TempMenu.hideTop();
}

function HM_f_HideChildren(callingitem,forced) {
	var TempMenu = this.visibleChild;
	while(TempMenu.hasChildVisible) {
		TempMenu.visibleChild.showIt(false);
		TempMenu.hasChildVisible = false;
		TempMenu = TempMenu.visibleChild;
	}
	if(forced || ((callingitem && (!callingitem.hasMore || this.visibleChild != callingitem.child)) || (!callingitem && !this.isOn))) {
		this.visibleChild.showIt(false);
		this.hasChildVisible = false;
	}
}

function HM_f_CancelSelect(){return false}

function HM_f_PageClick() {
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if (!HM_UserOverMenu && HM_CurrentMenu!=null && !HM_CurrentMenu.isOn) HM_f_HideAll();
}

function HM_f_CreateScrollbars(){
	var TopScrollElement = HM_MenusTarget.document.createElement("DIV");

	with(TopScrollElement.style){
		position = "absolute";
		top = -(HM_BorderWidth) + "px";
		left = "0px";
		width = (parseInt(this.style.width) - ((HM_IEnoDTD) ? (HM_BorderWidth * 2) : 0)) + "px";
		height = (HM_ScrollBarHeight - ((HM_IEnoDTD) ? 0 : (HM_BorderWidth * 2))) + "px";
		visibility = "hidden";
		backgroundColor = HM_ScrollBarColor;
		textAlign = "center";
		zIndex = 10000;
		var BorderString = (HM_BorderWidth + "px " + this.tree.BorderColor + " " + HM_BorderStyle);
		borderBottom = BorderString;
		borderTop = BorderString;
	}

	var ImageElement = HM_MenusTarget.document.createElement("IMG");
	with(ImageElement.style){
		position = "absolute";
		top = ((HM_ScrollBarHeight - (HM_BorderWidth * 2) - HM_ScrollImgHeight)/2) + "px";
		visibility = "inherit";
		left = ((parseInt(TopScrollElement.style.width) - HM_ScrollImgWidth)/2)+"px";
	}
	TopScrollElement.appendChild(ImageElement);
	var BottomScrollElement = TopScrollElement.cloneNode(true);
	if(HM_ScrollImgSrcBot) BottomScrollElement.firstChild.src = HM_ScrollImgSrcBot;
	if(HM_ScrollImgSrcTop) TopScrollElement.firstChild.src = HM_ScrollImgSrcTop;

	this.appendChild(TopScrollElement);
	this.appendChild(BottomScrollElement);

	TopScrollElement.menu = this;
	this.scrollbarTop = TopScrollElement;

	BottomScrollElement.menu = this;
	this.scrollbarBot = BottomScrollElement;

	if(HM_ScrollOver) {
		if(HM_UseMouseEnter) {
			this.scrollbarTop.onmouseenter=HM_f_StartScrollUp;
			this.scrollbarTop.onmouseleave=HM_f_StopScroll;
			this.scrollbarBot.onmouseenter=HM_f_StartScrollDown;
			this.scrollbarBot.onmouseleave=HM_f_StopScroll;
		}
		else {
			this.scrollbarTop.onmouseover=HM_f_StartScrollUp;
			this.scrollbarTop.onmouseout=HM_f_StopScroll;
			this.scrollbarBot.onmouseover=HM_f_StartScrollDown;
			this.scrollbarBot.onmouseout=HM_f_StopScroll;
		}
	}
	else {
		this.scrollbarTop.onmousedown = HM_f_StartScrollUp;
		this.scrollbarBot.onmousedown = HM_f_StartScrollDown;
	}

	this.scrollbarsCreated = true;
}

function HM_f_StartScrollUp() {
	HM_ScrollMenu=this.menu;
	return this.menu.startScroll(true);
}

function HM_f_StartScrollDown() {
	HM_ScrollMenu=this.menu;
	return this.menu.startScroll(false);
}

function HM_f_StartScroll(up){
	HM_f_StopScroll();
	if(HM_IsReloading||!HM_f_DocumentCheck()) return;
	if (this.hasChildVisible) this.hideChildren(false,true);
	if(!HM_ScrollOver) HM_MenusTarget.document.onmouseup = HM_f_StopScroll;
	HM_ScrollTimer = setInterval("HM_f_DoScroll(" + up + ",10)",HM_ScrollInterval);
	return false;
}

function HM_f_StopScroll(){
	clearInterval(HM_ScrollTimer);
	HM_ScrollTimer=null;
}

function HM_f_DoScroll(up,incr){
	if(HM_IsReloading||!HM_f_DocumentCheck()) {HM_f_StopScroll(); return;}
	var ScrollEl = HM_ScrollMenu.scrollParent;
	if(up){
		ScrollEl.top += incr;
	}
	else{
		ScrollEl.top -= incr;
	}
	HM_ScrollMenu.checkScroll();
}

function HM_f_CheckScroll(){
	var ScrollEl = this.scrollParent;
	var MenuHeight = parseInt(this.style.height);
	var ScrollHeight = parseInt(ScrollEl.style.height);
	var HeightDiff = MenuHeight - ScrollHeight;
	var ScrollTopOffset = HM_ScrollBothBars ? (HM_ScrollBarHeight - HM_BorderWidth) : 0;

	if(!HeightDiff){
		this.scrollbarBot.style.visibility = "hidden";
		this.scrollbarTop.style.visibility = "hidden";
		this.scrollbarsVisible = false;
		ScrollEl.top = 0;
		ScrollEl.style.top = ScrollEl.top +  "px";
		return;
	}
	if(HM_ScrollBothBars) HeightDiff -= (ScrollTopOffset*2);
	if(ScrollEl.top <= (HeightDiff)) {
		ScrollEl.top = HeightDiff;
		HM_f_StopScroll();
		this.scrollbarBot.style.top = (MenuHeight - ((HM_IEnoDTD) ? (HM_BorderWidth) : -(HM_BorderWidth)) - HM_ScrollBarHeight) + "px";
		if(!HM_ScrollBothBars) this.scrollbarBot.style.visibility = "hidden";
	}
	else {
		this.scrollbarBot.style.top = (MenuHeight - ((HM_IEnoDTD) ? (HM_BorderWidth) : -(HM_BorderWidth)) - HM_ScrollBarHeight) + "px";
		this.scrollbarBot.style.visibility = "inherit";
		if(HM_ScrollBothBars){
			this.scrollbarTop.style.visibility = "inherit";
			this.scrollbarsVisible = true;
		}
	}
	if(ScrollEl.top >= 0) {
		ScrollEl.top = 0;
		HM_f_StopScroll();
		if(!HM_ScrollBothBars) this.scrollbarTop.style.visibility = "hidden";
	}
	else {
		this.scrollbarTop.style.visibility = "inherit";
		if(HM_ScrollBothBars){
			this.scrollbarBot.style.visibility = "inherit";
			this.scrollbarsVisible = true;
		}
	}
	ScrollEl.style.top = (ScrollEl.top + ScrollTopOffset) + "px";
}

function HM_f_DoWheelScroll(){
	if(!this.scrollbarsCreated) return;
	var ScrollUp = (HM_MenusTarget.event.wheelDelta == 120);
	HM_ScrollMenu = this;
	HM_f_DoScroll(ScrollUp,this.currentItem.offsetHeight);
	return false;
}

function HM_f_PermissionDenied(e){
	if(HM_IE){
		var ErrorMessage = e.description;
	} else {
		var ErrorMessage = (HM_IsEarlyGecko) ? "Access Denied" :
                                   (typeof(e)=="object") ? e.message : e;
	}
	var AccessErrorFound = false;
	for (var i=0; i<HM_a_AccessErrors.length; i++) {
		if (ErrorMessage.toLowerCase().indexOf(HM_a_AccessErrors[i])!=-1) {
			AccessErrorFound = true;
			break;
		}
	}
	return AccessErrorFound;
}

function HM_f_ResizeHandler(){
	if(HM_IsReloading||!HM_f_DocumentCheck()) return HM_f_OtherResize();
	var mouse_x_position, mouse_y_position;
	for(var i=0; i<HM_TotalTrees; i++) {
		var TopMenu = HM_a_TopMenus[i].tree.startChild;
		if(TopMenu.style.visibility == "visible") {
			TopMenu.oldLeft = TopMenu.xPos;
			TopMenu.oldTop = TopMenu.yPos;
			mouse_x_position = TopMenu.mouseX;
			mouse_y_position = TopMenu.mouseY;
			TopMenu.xPos = eval(TopMenu.tree.MenuLeft);
			TopMenu.yPos = eval(TopMenu.tree.MenuTop);
			if(TopMenu.xPos == null) TopMenu.xPos = TopMenu.xIntended;
			if(TopMenu.yPos == null) TopMenu.yPos = TopMenu.yIntended;
			if(!TopMenu.tree.TopIsPermanent) {
				if(TopMenu.scrollbarsCreated) TopMenu.checkScroll();
				TopMenu.style.height = TopMenu.scrollParent.style.height;
				TopMenu.keepInWindow();
			}

			TopMenu.moveTo(TopMenu.xPos,TopMenu.yPos);
			var TempMenu = TopMenu;
			while(TempMenu.hasChildVisible) {
				TempParent = TempMenu;
				TempMenu = TempMenu.visibleChild;
				TempItem = TempMenu.parentItem;
				TempItem.showChild();
			}
		}
	}
	return HM_f_OtherResize();
}

function HM_f_NavUnloadHandler(){
	HM_NavUnloaded = true;
	clearTimeout(HM_ReloadTimer);
	HM_ReloadTimer = null;
	if(!HM_IsReloading) {
		if(HM_NS6)HM_f_HideAllPermanent();
		HM_IsReloading=true;
	}
	HM_f_ClearElements();

	if(HM_FramesEnabled && HM_IE5M) return HM_f_NavOtherOnUnload();
	if(HM_LoadCheckDone) {
		if(HM_CanAssignFrameLoad) {
			var FrameEl=parent.document.getElementsByName(HM_FramesMainFrameName)[0];
			FrameEl.removeEventListener('load',HM_f_FrameLoad,false);
		}
		else if(parent.HM_UseFrameLoad) {
			parent.HM_f_LoadMenus = null;
		}
	}
	if(window.HM_LoadElement) HM_LoadElement.onload=null;

	return HM_f_NavOtherOnUnload();
}

function HM_f_MainUnloadHandler(){
	HM_IsReloading = true;

	if((typeof(window.HM_NavUnloaded)!="boolean")||(HM_NavUnloaded)) {
		if(typeof(window.HM_f_MainOtherOnUnload)=="function") HM_f_MainOtherOnUnload();
		return;
	}
	if(HM_NS6)HM_f_HideAllPermanent();
	HM_f_InitVars();
	HM_f_MainOtherOnUnload();
	if(!HM_FrameHasLoadHandler) {
		if(HM_IE||HM_IsSafari) HM_f_IEMainUnloadHandler();
		else HM_f_NSMainUnloadHandler();
	}
}

function HM_f_IEMainUnloadHandler() {
	clearTimeout(HM_ReloadTimer);
	HM_ReloadTimer = null;
	HM_ReloadTimer = setTimeout("HM_f_IEKeepTrack()",HM_ReloadInterval);
}

function HM_f_NSMainUnloadHandler(){
	clearTimeout(HM_ReloadTimer);
	HM_ReloadTimer = null;
	HM_ReloadTimer = setTimeout("HM_f_NSKeepTrack()",HM_ReloadInterval);
}

function HM_f_IsInitialized() {
	try{
		if ((HM_MenusTarget) &&
		    (typeof(HM_MenusTarget.HM_Initialized)=="boolean") &&
		    (HM_MenusTarget.HM_Initialized)) return true;
		else return false;
	}
	catch(e){
		return false;
	}
}

function HM_f_NSKeepTrack() {
	if((typeof(window.HM_NavUnloaded)!="boolean")||(HM_NavUnloaded)) return true;
	try{
		if ((typeof(HM_MenusTarget.document) != "object") ||
		    (typeof(HM_MenusTarget.document.body)=="undefined") ||
		    (HM_MenusTarget.document.body==null) ||
		    (HM_f_IsInitialized())) {
			clearTimeout(HM_ReloadTimer);
			HM_ReloadTimer = null;
			HM_ReloadTimer = setTimeout("HM_f_NSKeepTrack()",HM_ReloadInterval);
		}
		else {
			var TargetDocumentBody = HM_MenusTarget.document.body;
			HM_f_FrameLoad();
		}
	}
	catch(e){
		HM_IsReloading = false;
		if(HM_f_PermissionDenied(e)) return true;
	}
}

function HM_f_IEKeepTrack() {
	if((typeof(window.HM_NavUnloaded)!="boolean")||(HM_NavUnloaded)) return true;
	try{
		var typeOfDocumentBody = typeof(HM_MenusTarget.document);
		var typeOfDocumentBody = HM_MenusTarget.document.readyState;
	}
	catch(e){
		HM_IsReloading = false;
		return true;
	}
	if (typeof(HM_MenusTarget.document) != "unknown") {
		if ((HM_MenusTarget.document.readyState != "complete") ||
		    (HM_f_IsInitialized())) {
			clearTimeout(HM_ReloadTimer);
			HM_ReloadTimer = null;
			HM_ReloadTimer = setTimeout("HM_f_IEKeepTrack()",HM_ReloadInterval);
		}
		else HM_f_FrameLoad(); 
	} else HM_IsReloading = false;
}

HM_LoadElement = (HM_FramesEnabled) ? (HM_NS6) ? parent : parent.document.body : window;

if(HM_LoadElement.HM_LoadedOnce) {
	HM_LoadElement = (HM_NS6) ? window : window.document.body;
}
else HM_LoadElement.HM_LoadedOnce=true;

HM_f_OtherOnLoad = (HM_LoadElement.onload) ? HM_LoadElement.onload : HM_f_Return;
HM_LoadElement.onload = function(){setTimeout("HM_f_FrameLoad()",10)};

HM_f_NavOtherOnUnload = (window.onunload) ? window.onunload : HM_f_Return;
window.onunload = HM_f_NavUnloadHandler;

popUp = HM_f_PopUp;
popDown = HM_f_PopDown;

//end
