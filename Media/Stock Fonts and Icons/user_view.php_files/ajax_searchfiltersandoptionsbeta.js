var SearchFiltersAndOptionsBetaAjax = Class.create();

SearchFiltersAndOptionsBetaAjax.prototype = {
	searchFiltersAndOptionsInitialized: false,
	majorTermsObject: {
		'csv': '',
		'conjunction': ''	
	},
	copySpaceObject: {
		'Tolerance': '1',
		'Matrix': ''
	},
	
	/*
		resetPopupDisplayed: false,
		preventPopClosing: false,
	*/
	
	initialize: function(context, hierPrefix){
		this.context = context;
		this.hierPrefix = hierPrefix;
		this.isDirty = false;
		this.colorSelector='Simple';
		this.initParams="";
		/*this.majorTermsObject.csv=$("MajortermBrowser_selectedList").value;
		this.majorTermsObject.conjunction=$("MajortermBrowser_conjunction").value;*/
		this.copySpaceObject.Tolerance=CopySpace.getTolerance();
		this.copySpaceObject.Matrix=CopySpace.getMatrix();
		this.initColor=$("FO_SearchColor").value;
		this.initSearchMember=$("FO_SearchMembername").value;
	},

	initializeAdvanced: function() {
		if(this.searchFiltersAndOptionsInitialized){
			this.initSearchText();
			return true;
		}
						
		this.params = '';
		var url = "ajax_class_creator.php";
		var params = "ajax_action=Init&ajax_class=searchfiltersandoptionsbeta&context="+this.context;
		var myAjax = new Ajax.Request
			(
				url, 
				{ 
					/*asynchronous: false,*/
					method: "get", 
					parameters: params,
					onComplete: this.initCompleted.bind(this),
					onFailure: this.failed.bind(this)
				}
			);
	},
	
	initCompleted: function(req) {
		var data=AjaxUtil.receiveArray(req.responseText);
		this.buildSelectOptions(eval('(' + data.selectOptions + ')'));
		this.params=eval('(' + data.params + ')');
		this.initParams = this.params;
		this.loadEvents();
		this.loadParams();
		this.searchFiltersAndOptionsInitialized = true;
		this.initSearchText();
	},
	
	initSearchText:function() {
		var basicSearchText = $("SearchBarText");
		var advancedSearchText = $("FO_SearchText");
		
		if(basicSearchText.value!=$('SearchBarTipText').value) {
			advancedSearchText.onfocus();
			advancedSearchText.value=basicSearchText.value;
		} else {
			advancedSearchText.value="";
			advancedSearchText.onblur();
		}
	},

	reset: function() {
		$("FO_SaveLoading").style.display="inline";
		var url = "ajax_class_creator.php";
		var params = "ajax_action=Reset&ajax_class=searchfiltersandoptionsbeta&context="+this.context;
		var myAjax = new Ajax.Request
			(
				url, 
				{ 
					asynchronous: true,
					method: "get", 
					parameters: params,
					onComplete: this.loadCompleted.bind(this),
					onFailure: this.failed.bind(this)
				}
			);
	},
	
	load: function() {
		$("FO_SaveLoading").style.display="inline";
		var url = "ajax_class_creator.php";
		var params = "ajax_action=Load&ajax_class=searchfiltersandoptionsbeta&context="+this.context;
		var myAjax = new Ajax.Request
			(
				url, 
				{ 
					asynchronous: true,
					method: "get", 
					parameters: params,
					onComplete: this.loadCompleted.bind(this),
					onFailure: this.failed.bind(this)
				}
			);
	},
	
	loadCompleted: function(req) {
		$("FO_SaveLoading").style.display="none";
		if($("FO_ResetPopup")) {
			$("FO_ResetPopup").style.display="none";
		}
		
		var data=AjaxUtil.receiveArray(req.responseText);
		this.params=eval('(' + data.params + ')');
		this.isDirty=true;
		this.loadParams(); 
	},
	
	save: function() {
		$("FO_SaveLoading").style.display="inline";
		this.setParams();
		this.initParams = this.params;
		var url = "ajax_class_creator.php";
		var params = "ajax_action=Save&ajax_class=searchfiltersandoptionsbeta&context="+this.context+"&params="+encodeURIComponent(Object.toJSON(this.params));
		var myAjax = new Ajax.Request
			(
				url, 
				{ 
					asynchronous: true,
					method: "post", 
					parameters: params,
					onComplete: this.saveCompleted.bind(this),
					onFailure: this.failed.bind(this)
				}
			);
	},
	
	saveCompleted: function(req) {
		$("FO_SaveLoading").style.display="none";
		var data=AjaxUtil.receiveArray(req.responseText);
		var searchBarParams=eval('('+ data.searchBarParams + ')');
		this.isDirty=false;
		this.updateSearchBar(searchBarParams);
	},
		
	updateSearchBar: function(params) {
		$("SearchBarFileType1").checked = params.FO_Image.value;
		$("SearchBarFileType7").checked = params.FO_Illustration.value;
		$("SearchBarFileType4").checked = params.FO_Flash.value;
		$("SearchBarFileType8").checked = params.FO_Video.value;
		$('SearchBarSortBy').value = params.FO_Sort.value;
	},
	
	updateSearchBarHiddenFields: function() {
		if(typeof this.params != "undefined") {
			this.setParams();
			$('TopSearchFileTypeSizePrice').value = this.buildFileTypeSizeString();
			$('TopSearchOrientation').value = this.params.FO_Shape.value.join(',');
				
			$('TopSearchShowTitle').value = this.params.FO_Titles.value;
			$('TopSearchShowContributor').value = this.params.FO_Contributor.value;
			$('TopSearchShowFileNumber').value = this.params.FO_FileNumbers.value;
			$('TopSearchShowDownload').value = this.params.FO_Downloads.value;
			$('TopSearchEnableLoupe').value = this.params.FO_Loupe.value;
			$('TopSearchOrder').value = this.params.FO_Sort.value;
			$('TopSearchPerPage').value = this.params.FO_PerPage.value;
			
			$('TopSearchShowPeople').value = this.params.FO_People.value;
			$('TopSearchPrintAvailable').value = this.params.FO_PrintAvailable.value;
			$('TopSearchExclusiveArtists').value = this.params.FO_ExclusiveArtists.value;
			$('TopSearchExtendedLicense').value = this.params.FO_ExtendedLicense.value;
	
			if ( $("FO_SearchText").value == $("FO_SearchTipText").value ){
				$('TopSearchText').value = '';
			} else {
				$('TopSearchText').value = $("FO_SearchText").value;
			}

			if ( $("FO_SearchMembername").value == $("FO_SearchTipMembername").value ){
				$('TopSearchMembername').value = '';
			} else {
				$('TopSearchMembername').value = $("FO_SearchMembername").value;
			}

			$('TopSearchMajorterms').value = this.buildMajortermsString();
			$('TopSearchColor').value = $("FO_SearchColor").value;
			$('TopSearchCopySpace').value = this.buildCopySpaceString();
									
			//$('TopSearchMinWidth').value = this.params.FO_MinWidth.value;
			//$('TopSearchMinHeight').value = this.params.FO_MinHeight.value;
			//$('TopSearchIllustrationLimit').value = this.params.FO_IllustrationLimit.value;
			//$('TopSearchFlashLimit').value = this.params.FO_FlashLimit.value;
			//$('TopSearchFilterContent').value = this.params.FO_AdultContentFilter.value;
		}
	},
	
	buildFileTypeSizeString: function() {
		var f = this.params['FO_FileType'].value;
		var jsonString = ''; 
		var jsonString="[{'type':'"+f.FO_Image.name+"','size':'"+f.FO_Image.value+"','priceOption':'1'},{'type':'"+f.FO_Illustration.name+"','size':'Vector Image','priceOption':'"+f.FO_Illustration.value+"'},{'type':'"+f.FO_Flash.name+"','size':'Flash Document','priceOption':'"+f.FO_Flash.value+"'},{'type':'"+f.FO_Video4_3.name+"','size':'"+f.FO_Video4_3.value+"','priceOption':'1'},{'type':'"+f.FO_Video16_9.name+"','size':'"+f.FO_Video16_9.value+"','priceOption':'1'}]";
		return jsonString;
	},
	
	buildMajortermsString:function() {
		this.majorTermsObject.csv=$("MajortermBrowser_selectedList").value;
		this.majorTermsObject.conjunction=($("MajortermBrowser_conjunction").value=="all"?'AND':'OR');
		return Object.toJSON(this.majorTermsObject);
	},
	
	buildCopySpaceString:function() {
		this.copySpaceObject.Matrix=CopySpace.getMatrix();
		this.copySpaceObject.Tolerance=CopySpace.getTolerance();
		return Object.toJSON(this.copySpaceObject);
	},
	
	failed: function(req) {
		//this.enableAllButtons();
	},
	
	setParams: function() {
		this.params['FO_FileType'].value["FO_Image"].value=$('FO_Image').value;
		this.params['FO_FileType'].value["FO_Illustration"].value=$('FO_Illustration').value;
		this.params['FO_FileType'].value["FO_Flash"].value=$('FO_Flash').value;
		this.params['FO_FileType'].value["FO_Video4_3"].value=$('FO_Video4_3').value;	
		this.params['FO_FileType'].value["FO_Video16_9"].value=$('FO_Video16_9').value;	
		this.params['FO_PerPage'].value=$('FO_PerPage').value;
		this.params['FO_Sort'].value=$('FO_Sort').value;
		
		this.params["FO_Shape"].value=[];
		//if($("FO_Shape").checked) {
			var shapes = $("FO_ShapeSelect").immediateDescendants();
			for(var i=0; i< shapes.length; i++) {
				var orientation=shapes[i].id.substr(8);
				if(shapes[i].className=="ShapeSelected"+orientation) {
					this.params["FO_Shape"].value.push(orientation);
				}
			}
		//}

			
		/*if($("FO_Size").checked) {
			this.params["FO_MinWidth"].value=$("FO_MinWidth").value;
			this.params["FO_MinHeight"].value=$("FO_MinHeight").value;
		} else {
			this.params["FO_MinWidth"].value=0;
			this.params["FO_MinHeight"].value=0;
		}*/
				
		this.params["FO_Titles"].value = $("FO_Titles").checked;
		this.params["FO_Contributor"].value = $("FO_Contributor").checked;
		this.params["FO_FileNumbers"].value = $("FO_FileNumbers").checked;
		this.params["FO_Downloads"].value = $("FO_Downloads").checked;
		this.params["FO_Loupe"].value = $("FO_Loupe").checked;
		this.params["FO_People"].value = $("FO_People").checked;
		this.params["FO_PrintAvailable"].value = $("FO_PrintAvailable").checked;
		this.params["FO_ExclusiveArtists"].value = $("FO_ExclusiveArtists").checked;
		this.params["FO_ExtendedLicense"].value = $("FO_ExtendedLicense").checked;
			
		if ($('FO_AdultContentFilterON')) {
			if ($('FO_AdultContentFilterON').style.display == 'inline') {
				this.params['FO_AdultContentFilter'].value=1;
			} else {
				this.params['FO_AdultContentFilter'].value=0;
			}
		} else {
			this.params["FO_AdultContentFilter"].value = $("FO_AdultContentFilter").checked;
		}
	},

	buildSelectOptions: function(filters) {
		var photo = $("FO_Image");
		var p = filters["FO_Image"];
		for(var i=0; i<p.labels.length; i++){
			var option = document.createElement('option');
			option.value=p.values[i];
			option.innerHTML = fixSafariEncode(p.labels[i])+"&nbsp;&nbsp;"; 
			photo.appendChild(option);
		}
		
		var illustration = $("FO_Illustration");
		var v = filters["FO_Illustration"];
		for(var i=0; i<v.labels.length; i++){
			var option = document.createElement('option');
			option.value=v.values[i];
			option.innerHTML=fixSafariEncode(v.labels[i])+"&nbsp;&nbsp;";
			illustration.appendChild(option);
		}
		
		var flash = $("FO_Flash");
		var f = filters["FO_Flash"]
		for(var i=0; i<f.labels.length; i++){
			var option = document.createElement('option');
			option.value=f.values[i];
			option.innerHTML=fixSafariEncode(f.labels[i])+"&nbsp;&nbsp;";
			flash.appendChild(option);
		}
		
		var video43 = $("FO_Video4_3");
		var v43 = filters["FO_Video4_3"];
		for(var i=0; i<v43.labels.length; i++){
			var option = document.createElement('option');
			option.value=v43.values[i];
			option.innerHTML=fixSafariEncode(v43.labels[i])+"&nbsp;&nbsp;";
			video43.appendChild(option);
		}
		
		var video169 = $("FO_Video16_9");
		var v169 = filters["FO_Video16_9"];
		for(var i=0; i<v169.labels.length; i++){
			var option = document.createElement('option');
			option.value=v169.values[i];
			option.innerHTML=fixSafariEncode(v169.labels[i])+"&nbsp;&nbsp;";
			video169.appendChild(option);
		}
				
		var p=filters["FO_PerPage"];
		var perPage = $("FO_PerPage");
		for(var i=0; i<p.labels.length; i++){
			var option = document.createElement('option');
			option.value=p.values[i];
			option.innerHTML=fixSafariEncode(p.labels[i])+"&nbsp;&nbsp;";
			perPage.appendChild(option);
		}
	},
	
	loadParams: function() {
		var options = $("FO_Sort").options;
		for(var i=0; i<options.length; i++) {
			if(options[i].value == this.params['FO_Sort'].value) {
				options[i].selected = true;
				break;
			}
		}
				
		var fileTypeSize=this.params['FO_FileType'].value;
		var options = $("FO_Image").options;
		for(var i=0; i<options.length; i++) {
			if(options[i].value == fileTypeSize['FO_Image'].value) {
				options[i].selected = true;
				if(fileTypeSize['FO_Image'].value =='All' || fileTypeSize['FO_Image'].value == 'None') {
					$("FO_ImageWarning").style.display='none';
				} else {
					$("FO_ImageWarning").style.display='inline';
				}
				break;
			}
		}
		var options = $("FO_Illustration").options;
		for(var i=0; i<options.length; i++) {
			if(options[i].value == fileTypeSize['FO_Illustration'].value) {
				options[i].selected = true;
				if(fileTypeSize['FO_Illustration'].value =='All' || fileTypeSize['FO_Illustration'].value == 'None') {
					$("FO_IllustrationWarning").style.display='none';
				} else {
					$("FO_IllustrationWarning").style.display='inline';
				}
				break;
			}
		}
		$("FO_Illustration").onchange();
		var options = $("FO_Flash").options;
		for(var i=0; i<options.length; i++) {
			if(options[i].value == fileTypeSize['FO_Flash'].value) {
				options[i].selected = true;
				if(fileTypeSize['FO_Flash'].value =='All' || fileTypeSize['FO_Flash'].value == 'None') {
					$("FO_FlashWarning").style.display='none';
				} else {
					$("FO_FlashWarning").style.display='inline';
				}
				break;
			}
		}
		$("FO_Flash").onchange();
		var options = $("FO_Video4_3").options;
		for(var i=0; i<options.length; i++) {
			if(options[i].value == fileTypeSize['FO_Video4_3'].value) {
				options[i].selected = true;
				if(fileTypeSize['FO_Video4_3'].value =='All 4_3'  || fileTypeSize['FO_Video4_3'].value == 'None 4_3') {
					$("FO_Video4_3Warning").style.display='none';
				} else {
					$("FO_Video4_3Warning").style.display='inline';
				}
				break;
			}
		}
		var options = $("FO_Video16_9").options;
		for(var i=0; i<options.length; i++) {
			if(options[i].value == fileTypeSize['FO_Video16_9'].value) {
				options[i].selected = true;
				if(fileTypeSize['FO_Video16_9'].value =='All 16_9' || fileTypeSize['FO_Video16_9'].value == 'None 16_9') {
					$("FO_Video16_9Warning").style.display='none';
				} else {
					$("FO_Video16_9Warning").style.display='inline';
				}
				break;
			}
		}
		var options = $("FO_PerPage").options;
		for(var i=0; i<options.length; i++) {
			if(options[i].value == this.params['FO_PerPage'].value) {
				options[i].selected = true;
				break;
			}
		}
				
		//$("FO_Sort").innerHTML=this.params['FO_Sort'].value;
		/*$("FO_Size").checked=(this.params["FO_MinWidth"].value > 0 || this.params["FO_MinHeight"].value >0 ? true: false);
		$("FO_MinWidth").value=(this.params["FO_MinWidth"].value > 0 ? this.params["FO_MinWidth"].value : "");
		$("FO_MinHeight").value=(this.params["FO_MinHeight"].value > 0 ? this.params["FO_MinHeight"].value : "");*/
		
		
		var shape = $("FO_ShapeSelect");
		shape.checked = true;//($("FO_Size").checked == true ? false : true);
		
		if(shape.checked) {
			var shapes = shape.immediateDescendants();
			for(var i=0; i< shapes.length; i++) {
				var orientation=shapes[i].id.substr(8);
				shapes[i].className="ShapeNormal"+orientation;
				shapes[i].title=this.getShapeToolTip(orientation, true);
			}
			var shapeList = this.params["FO_Shape"].value;
			for(var i=0; i<shapeList.length; i++) {
				this.changeShape(shapeList[i]);
			}
		} else {
			var shapes = shape.immediateDescendants();
			for(var i=0; i< shapes.length; i++) {
				var orientation=shapes[i].id.substr(8);
				shapes[i].className="ShapeSelected"+orientation;
				shapes[i].title=this.getShapeToolTip(orientation, true);
			}
		}
		
		/*$("FO_IllustrationLimit"+this.params["FO_IllustrationLimit"].value).checked=true;
		$("FO_FlashLimit"+this.params["FO_FlashLimit"].value).checked=true;*/
			
		$("FO_Titles").checked = (this.params["FO_Titles"].value==1 ? true : false);
		$("FO_Contributor").checked = (this.params["FO_Contributor"].value==1 ? true : false);
		$("FO_FileNumbers").checked = (this.params["FO_FileNumbers"].value==1 ? true : false);
		$("FO_Downloads").checked = (this.params["FO_Downloads"].value==1 ? true : false);
		$("FO_Loupe").checked = (this.params["FO_Loupe"].value==1 ? true : false);
		
		$("FO_People").checked = (this.params["FO_People"].value==1 ? true : false);
		this.toggleWarning("FO_People", (this.params["FO_People"].value==1 ? true : false));
		$("FO_PrintAvailable").checked = (this.params["FO_PrintAvailable"].value==1 ? true : false);
		this.toggleWarning("FO_PrintAvailable", (this.params["FO_PrintAvailable"].value==1 ? true : false));
		$("FO_ExclusiveArtists").checked = (this.params["FO_ExclusiveArtists"].value==1 ? true : false);
		this.toggleWarning("FO_ExclusiveArtists", (this.params["FO_ExclusiveArtists"].value==1 ? true : false));
		$("FO_ExtendedLicense").checked = (this.params["FO_ExtendedLicense"].value==1 ? true : false);
		this.toggleWarning("FO_ExtendedLicense", (this.params["FO_ExtendedLicense"].value==1 ? true : false));
		
		if ($("FO_AdultContentFilterON")) {
			if (this.params['FO_AdultContentFilter'].value==1) {
				$("FO_AdultContentFilterON").style.display='inline';
				$("FO_AdultContentFilterOFF").style.display='none';
			} else {
				$("FO_AdultContentFilterON").style.display='none';
				$("FO_AdultContentFilterOFF").style.display='inline';
			}
		} else {
			$("FO_AdultContentFilter").checked = (this.params["FO_AdultContentFilter"].value==1 ? true : false);
		}
	},
	
	cancelChanges: function() {
		if(this.isDirty) {
			this.params=this.initParams;
			this.loadParams();
		}
		$("browseResults").innerHTML="";
		var selectedList=$("MajortermBrowser_selectedList").value.split(",");
		for(var i=0; i<selectedList.length; i++) {
			CatBrowserBeta.removeCat(selectedList[i]);
		}
		HierBrowser.triggerInitialSelections(this.hierPrefix);
		/*$("MajortermBrowser_selectedList").value=this.majorTermsObject.csv;
		$("MajortermBrowser_conjunction").value=this.majorTermsObject.conjunction;*/
		UpdateCurrentColorBox(this.initColor);
		CopySpace.updateMatrix(this.copySpaceObject.Matrix);
		CopySpace.updateTolerance(this.copySpaceObject.Tolerance);
		$("FO_SearchMembername").value=this.initSearchMember;
		searchBarAjax.toggleSearchBar(true);
	},
	
	submitSearch:function() {
		$("FO_SearchLoading").style.display="inline";
		this.updateSearchBarHiddenFields();
		if(TopsearchFormValidate()) {
			$('topsearch').submit();
		}
	},
	
	submitViaEnter:function(e) {
		if(e && e.which) {
			key=e.which;
		} else { 
			e=event;
			key=e.keyCode;
		}
		if(key==13) {
			this.submitSearch();		
		}
		return false;
	},
	
	loadEvents: function() {
		/* buttons */
		$("FO_Submit").onclick=this.submitSearch.bind(this);
		$("FO_SearchForm").onsubmit=function() {
			this.submitSearch();
			return false;
		}.bind(this);
		
		
		$("FO_Reset").onclick=this.reset.bind(this);
		if($("FO_Save")) {
			$("FO_Save").onclick=this.save.bind(this);
			$("FO_Load").onclick=this.load.bind(this);
		}
		$("FO_Close").onclick=this.cancelChanges.bind(this);
		$("FO_Cancel").onclick=this.cancelChanges.bind(this);
		$("FO_SearchBasic").onclick=this.cancelChanges.bind(this);	
			
		if ($("FO_ResetTable")) {
		
			/* Reset Button */
			$("FO_ResetOptions").onmouseover=this.showTable.bind(this);
			$("FO_ResetContainerWrapper").onmouseover=this.showTable.bind(this);
			$("FO_SaveSettings").onmouseout=this.hideTable.bind(this);
			
			/* Save Button */
			$("FO_Save").onmouseover=this.setFilterHighlightOn.bind(this);
			$("FO_Save").onmouseout=this.setFilterHighlightOff.bind(this);
		}
		
		if($("FO_ResetSettings")) {
			$("FO_ResetSettings").onmouseover=this.setFilterHighlightOn.bind(this);
			$("FO_ResetSettings").onmouseout=this.setFilterHighlightOff.bind(this);
		}

		/* text and membername */
		$("FO_SearchText").onfocus=function() { this.hideTextTip($("FO_SearchText"), $("FO_SearchTipText")) }.bind(this);
		$("FO_SearchText").onblur=function() { this.showTextTip($("FO_SearchText"), $("FO_SearchTipText")) }.bind(this);
		$("FO_SearchText").onkeyup=this.submitViaEnter.bind(this);
				
		$("FO_SearchMembername").onfocus=function() { this.hideTextTip($("FO_SearchMembername"), $("FO_SearchTipMembername")) }.bind(this);
		$("FO_SearchMembername").onblur=function() { this.showTextTip($("FO_SearchMembername"), $("FO_SearchTipMembername")) }.bind(this);
		$("FO_SearchMembername").onkeyup=this.submitViaEnter.bind(this);
		
		/* search color */
		$("FO_SearchColorHex").onkeyup=function() { this.setHex($("FO_SearchColorHex").value); }.bind(this);
		$("FO_SearchColorSelectorAnchor_Simple").onclick=function() { this.toggleColorSelector('Simple'); }.bind(this);
		$("FO_SearchColorSelectorAnchor_Web").onclick=function() { this.toggleColorSelector('Web'); }.bind(this);
		$("FO_SearchColorSelectorAnchor_RGB").onclick=function() { this.toggleColorSelector('RGB'); }.bind(this);
		$("FO_SearchColorSelectorAnchor_HSV").onclick=function() { this.toggleColorSelector('HSV'); }.bind(this);
		$("FO_SearchColorClear").onclick=function() { UpdateCurrentColorBox(""); };
		
		/* copyspace clear */
		$('FO_SearchCopyspaceClear').onclick=function() { CopySpace.resetBox(); };
		
		/* image size */
		$("FO_Image").onchange=function() {
			this.isDirty = true;
			this.validateSelection($("FO_Image"));
			if($("FO_Image").value=='None') {
				//$("FO_Shape").disabled=true;
				/*$("FO_Size").disabled=true;
				var minWidth = $("FO_MinWidth");
				minWidth.value="";
				minWidth.disabled=true;
				var minHeight = $("FO_MinHeight");
				minHeight.value="";
				minHeight.disabled=true;*/
				$("FO_ShapeVertical").onclick=function(){ }.bind(this);
				$("FO_ShapeHorizontal").onclick=function(){	}.bind(this);
				$("FO_ShapeSquare").onclick=function(){	}.bind(this);
				var shapes = $("FO_ShapeSelect").immediateDescendants();
				for(var i=0; i<shapes.length; i++) {
					var orientation=shapes[i].id.substr(8);
					shapes[i].className="ShapeNormal"+orientation;
				}				
			} else {
				//$("FO_Shape").disabled=false;
				/*$("FO_Size").disabled=false;
				$("FO_MinWidth").disabled=false;
				$("FO_MinHeight").disabled=false;*/
				$("FO_ShapeVertical").onclick=function(){	this.changeShape('Vertical'); }.bind(this);
				$("FO_ShapeHorizontal").onclick=function(){	this.changeShape('Horizontal'); }.bind(this);
				$("FO_ShapeSquare").onclick=function(){	this.changeShape('Square'); }.bind(this);
				this.validateShapeSelection();
			}
			if($("FO_Image").value =='All' || $("FO_Image").value == 'None') {
				$("FO_ImageWarning").style.display='none';
			} else {
				$("FO_ImageWarning").style.display='inline';
			}
		}.bind(this);
		$("FO_ShapeVertical").onclick=function(){ this.changeShape('Vertical'); }.bind(this);
		$("FO_ShapeHorizontal").onclick=function(){	this.changeShape('Horizontal'); }.bind(this);
		$("FO_ShapeSquare").onclick=function(){	this.changeShape('Square'); }.bind(this);		
		/*$("FO_MinHeight").onfocus=function(){ this.changeSizeFocus(); }.bind(this); 
		$("FO_MinWidth").onfocus=function(){ this.changeSizeFocus(); }.bind(this); 
		$("FO_MinHeight").onblur=function(){ this.changeSizeBlur(); }.bind(this); 
		$("FO_MinWidth").onblur=function(){ this.changeSizeBlur(); }.bind(this); 
		$("FO_MinHeight").onkeyup=function(){ this.isNumeric($("FO_MinHeight")); }.bind(this); 
		$("FO_MinWidth").onkeyup=function(){ this.isNumeric($("FO_MinWidth")); }.bind(this);
		$("FO_Shape").onclick=function(){
			$("FO_MinHeight").value="";
			$("FO_MinWidth").value="";
		}
		$("FO_Size").onclick=function() {
			var shapes = $("FO_ShapeSelect").immediateDescendants();
			for(var i=0; i<shapes.length; i++) {
				var orientation=shapes[i].id.substr(8);
				shapes[i].className="ShapeSelected"+orientation;
			}
		}*/
		
		/* illustration price */		
		$("FO_Illustration").onchange=function() {
			this.isDirty = true;
			this.validateSelection($("FO_Illustration"));
			if($("FO_Illustration").value=='None' || $("FO_Illustration").value=='All') {
				/*$("FO_IllustrationLimitExactly").disabled=true;
				$("FO_IllustrationLimitMax").disabled=true;*/
				$("FO_IllustrationWarning").style.display='none';
			} else {
				/*$("FO_IllustrationLimitExactly").disabled=false;
				$("FO_IllustrationLimitMax").disabled=false;*/
				$("FO_IllustrationWarning").style.display='inline';
			}
		}.bind(this);
		/*$("FO_IllustrationLimitExactly").onclick=function() { this.params['FO_IllustrationLimit'].value="Exactly" }.bind(this);
		$("FO_IllustrationLimitMax").onclick=function() { this.params['FO_IllustrationLimit'].value="Max" }.bind(this);*/
		
		
		/* flash price */
		$("FO_Flash").onchange=function() {
			this.isDirty = true;
			this.validateSelection($("FO_Flash"));
			if($("FO_Flash").value=='None' || $("FO_Flash").value=='All') {
				/*$("FO_FlashLimitExactly").disabled=true;
				$("FO_FlashLimitMax").disabled=true;*/
				$("FO_FlashWarning").style.display='none';
			} else {
				/*$("FO_FlashLimitExactly").disabled=false;
				$("FO_FlashLimitMax").disabled=false;*/
				$("FO_FlashWarning").style.display='inline';
			}
		}.bind(this);
		/*$("FO_FlashLimitExactly").onclick=function() { this.params['FO_FlashLimit'].value="Exactly" }.bind(this);
		$("FO_FlashLimitMax").onclick=function() { this.params['FO_FlashLimit'].value="Max" }.bind(this);*/
		
		/* video sizes */
		$("FO_Video4_3").onchange=function() {
			this.isDirty = true;
			this.validateSelection($("FO_Video4_3"));
			if($("FO_Video4_3").value=='None 4_3' || $("FO_Video4_3").value=='All 4_3') {
				$("FO_Video4_3Warning").style.display='none';
			} else {
				$("FO_Video4_3Warning").style.display='inline';
			}
		}.bind(this);
		$("FO_Video16_9").onchange=function() {
			this.isDirty = true;
			this.validateSelection($("FO_Video16_9"));
			if($("FO_Video16_9").value=='None 16_9' || $("FO_Video16_9").value=='All 16_9') {
				$("FO_Video16_9Warning").style.display='none';
			} else {
				$("FO_Video16_9Warning").style.display='inline';
			}
		}.bind(this);
		
		/* filters */		
		$("FO_Titles").onclick=function() { this.isDirty=true; }.bind(this);
		$("FO_Contributor").onclick=function() { this.isDirty=true; }.bind(this);
		$("FO_FileNumbers").onclick=function() { this.isDirty=true; }.bind(this);
		$("FO_Downloads").onclick=function() { this.isDirty=true; }.bind(this);
		$("FO_Loupe").onclick=function() { this.isDirty=true; }.bind(this);
		
		$("FO_People").onclick=function() { this.isDirty = true; this.toggleWarning("FO_People", $("FO_People").checked);}.bind(this);
		$("FO_PrintAvailable").onclick=function() { this.isDirty = true; this.toggleWarning("FO_PrintAvailable", $("FO_PrintAvailable").checked);}.bind(this);
		$("FO_ExclusiveArtists").onclick=function() { this.isDirty = true; this.toggleWarning("FO_ExclusiveArtists", $("FO_ExclusiveArtists").checked);}.bind(this);
		$("FO_ExtendedLicense").onclick=function() { this.isDirty = true; this.toggleWarning("FO_ExtendedLicense", $("FO_ExtendedLicense").checked);}.bind(this);		
		if ($("FO_AdultContentFilter")) {
			$("FO_AdultContentFilter").onclick=function() { this.isDirty = true; }.bind(this);
		}		
	},
	
	toggleHelpOpen: function() {
		var help = $("FO_HelpPopup");
		if(help.style.left=='') {
			var pos = Position.cumulativeOffset($("FO_Help"));
			help.style.left=(pos[0]-265)+"px";
			help.style.top=(pos[1]-200)+"px";
		}
		help.style.display='block';
	},
	
	toggleHelpClose: function() {
		$("FO_HelpPopup").style.display='none';
	},
	
	toggleWarning: function(eleID, warningOn) {
		if(warningOn) {
			$(eleID+"Warning").style.display='inline';
		} else {
			$(eleID+"Warning").style.display='none';
		}
	},

	hideTextTip: function(obj, tipObj) {
		if ( obj.value == tipObj.value ){
			obj.value = '';
			obj.style.color = "#000000";
		}
	},

	showTextTip: function(obj, tipObj) {
		if ( obj.value == '' ){
			obj.value = tipObj.value;
			obj.style.color = "#999999";
		}
	},
	
	changeShape:function(shape) {
		if($("FO_Shape"+shape)) {
			var shapeEle = $("FO_Shape"+shape);
			this.isDirty = true;
			if(shapeEle.className=="ShapeSelected"+shape) {
				shapeEle.className="ShapeNormal"+shape;
				shapeEle.title=this.getShapeToolTip(shape, true);
				this.validateShapeSelection();
			} else {
				shapeEle.className="ShapeSelected"+shape;
				shapeEle.title=this.getShapeToolTip(shape, false);
			}
			//$("FO_Shape").checked=true;
			/*$("FO_MinHeight").value = "";
			$("FO_MinWidth").value = "";*/
		}
	},
	
	getShapeToolTip:function(shape, include) {
		if(include) {
			return $("FO_Shape"+shape+"_HelpInclude").value
		} else {
			return $("FO_Shape"+shape+"_HelpExclude").value
		}
	},
	
	validateShapeSelection: function() {
		var noShapesSelected = true;			
		var shapes = $("FO_ShapeSelect").immediateDescendants();
		for(var i=0; i<shapes.length; i++) {
			var orientation=shapes[i].id.substr(8);
			if(shapes[i].className=="ShapeSelected"+orientation) {
				noShapesSelected=false;
				break;
			}
		}
		if(noShapesSelected) {
			for(var i=0; i<shapes.length; i++) {
				var orientation=shapes[i].id.substr(8);
				shapes[i].className="ShapeSelected"+orientation;
				shapes[i].title=this.getShapeToolTip(orientation, false);
			}
		}
	},
	 
	/*changeSizeFocus:function() {
		$("FO_Size").checked=true;
		var shapes = $("FO_ShapeSelect").immediateDescendants();
		for(var i=0; i< shapes.length; i++) {
			var orientation=shapes[i].id.substr(8);
			shapes[i].className="ShapeSelected"+orientation;
		}
	},
	changeSizeBlur:function() {
		if($("FO_MinHeight").value || $("FO_MinWidth").value) {
			$("FO_Size").checked=true;
		} else {
			$("FO_Shape").checked=true;
		}
	},*/
	isNumeric:function(ele) {
		var ValidChars = '0123456789';
		var IsNumber=true;
		var Char;
		sText = ele.value;
		for (i = 0; i < sText.length && IsNumber == true; i++) { 
			Char = sText.charAt(i); 
			if (ValidChars.indexOf(Char) == -1 || (sText.charAt(0) == 0)) {
		 		IsNumber = false;
		 		ele.value = '';
				ele.focus();
			}
		}      
	},
	
	validateSelection: function(ele) {
		if($("FO_Image").value=='None' && $("FO_Illustration").value=='None' && $("FO_Flash").value=='None' && $("FO_Video4_3").value=='None 4_3' && $("FO_Video16_9").value=='None 16_9') {
			if(ele.id=="FO_Video4_3") {
				ele.value="All 4_3";
			} else if(ele.id=="FO_Video16_9") {
				ele.value="All 16_9"
			} else {
				ele.value='All';
			}
			return false;
		} else {
			return true;
		}
	}, 
		
	hideTable: function() {
		this.setFilterHighlightOff();
		$('FO_ResetTableOptions').style.display="none";
		$('FO_ResetContainerWrapper').style.backgroundColor="#efefef";
		$('FO_ResetContainerWrapper').style.borderRight="1px solid #ededec";
	},
	
	showTable: function() {
		this.setFilterHighlightOn();
		$('FO_ResetTableOptions').style.display="block";
		$('FO_ResetContainerWrapper').style.backgroundColor="#fff";
		$('FO_ResetContainerWrapper').style.borderRight="1px solid #666666";
	},
	
	setFilterHighlightOn: function () {
		$("FO_FilterHighlight").className="divHighlightOn";
	},
	
	setFilterHighlightOff: function () {
		$("FO_FilterHighlight").className="divHighlightOff";
	},
	
	
	setHex: function(value) {
		var rgbVal=ColorSelector.splitHex(value); 
		UpdateCurrentColorBox(rgbVal, true);
	},
	
	toggleColorSelector:function(selector) {
		if(this.colorSelector!=selector) {			
			if($("FO_SearchColorSelector_"+selector).innerHTML=="") {
				var url = "ajax_class_creator.php";
				var params = "ajax_action=GetColorSelector&ajax_class=searchfiltersandoptionsbeta&color="+$('FO_SearchColor').value+"&type="+selector;
				var myAjax = new Ajax.Request
					(
						url, 
						{ 
							method: "get", 
							parameters: params,
							onComplete: this.toggleColorSelectorCompleted.bind(this),
							onFailure: this.failed.bind(this)
						}
					);
			} else {
				this.switchColorSelector(selector);	
			}
		}
	},
	
	toggleColorSelectorCompleted:function(req) {
		var data = AjaxUtil.receiveArray(req.responseText);
		$('FO_SearchColorSelector_'+data['selector']).innerHTML = data['content'];
		this.switchColorSelector(data['selector']);
	},
	
	switchColorSelector:function(selector) {			
		$("FO_SearchColorSelector_"+this.colorSelector).style.display="none";
		$("FO_SearchColorSelectorAnchor_"+this.colorSelector).style.fontWeight="normal";
		$("FO_SearchColorSelector_"+selector).style.display="block";
		$("FO_SearchColorSelectorAnchor_"+selector).style.fontWeight="bold";
		this.colorSelector=selector;
	}
};

function UpdateCurrentColorBox(rgb, retainHexInput) {
	if (rgb == ""){
		ColorSelector.clearColor(retainHexInput);
	} else {
		ColorSelector.setColor(rgb, retainHexInput);
	}
}

var ColorSelector={
	clearColor:function(retainHexInput) {
		$('FO_SearchColor').value='';
		var thebox = $("FO_SearchColorBox");
		thebox.title="";
		thebox.setStyle({
  			backgroundColor: "",
  			backgroundImage: "url(/images/colourblock_bg.gif)"
		});
		if(!retainHexInput) {
			$("FO_SearchColorHex").value="";
		}
		this.refreshColorSelectors('0,0,0');
	},
	setColor:function(rgb, retainHexInput){
		rgb = this.formatRGB(rgb);
		$('FO_SearchColor').value=rgb;
		var hex=this.rgbToHex(rgb);
		if(!retainHexInput) {
			$("FO_SearchColorHex").value=hex;
		}
		var thebox = $("FO_SearchColorBox");
		thebox.title=hex;
		thebox.setStyle({
  			backgroundColor: "rgb("+rgb+")",
  			backgroundImage: "none"
		});
		this.refreshColorSelectors(rgb);
	},
	formatRGB: function(rgb) {
		var value_array = rgb.split(",");
		var newRGBArray = new Array(3);
		newRGBArray[0] = (value_array[0]-0);
		newRGBArray[1] = (value_array[1]-0);
		newRGBArray[2] = (value_array[2]-0);
		return newRGBArray.join(',');
	},
	refreshColorSelectors: function(value) {
		if($("FO_SearchColorSelector_HSV").innerHTML=='' && $("FO_SearchColorSelector_HSV").innerHTML=='') {
		} else {
			var url = "ajax_class_creator.php";
			var params = "ajax_action=RefreshColorSelectors&ajax_class=searchfiltersandoptionsbeta&color="+value;
			var myAjax = new Ajax.Request
				(
					url, 
					{ 
						method: "get", 
						parameters: params,
						onComplete: this.refreshColorSelectorsCompleted.bind(this)
					}
				);
		}
	},
	refreshColorSelectorsCompleted: function(req) {
		var data=AjaxUtil.receiveArray(req.responseText);
		
		if($("FO_SearchColorSelector_HSV").innerHTML!='') {
			$("FO_SearchColorSelector_HSV").innerHTML = data.HSV;
		}
		if($("FO_SearchColorSelector_RGB").innerHTML!='') {
			$("FO_SearchColorSelector_RGB").innerHTML = data.RGB;
		}
	},
	
	hexToDec:function(hex){ 
		return parseInt(hex,16); 
	},
	
	trimHex:function(str){
		return str.replace(/^\#/g, '');
	},
	
	splitHex:function(hex){
		var hex = this.trimHex(hex);
		if(hex.length==6) {
			r = this.hexToDec(hex.substring(0, 2));
			g = this.hexToDec(hex.substring(2, 4));
			b = this.hexToDec(hex.substring(4, 6));
			return r + ',' + g + ',' + b;
		}
		else {
			return false;
		}
	},
	rgbToHex:function(rgb) {
		var value_array = rgb.split(",");
		hexcode = "#";

		if (value_array[0] < 16) hexcode = hexcode + "0";
		hexcode = hexcode + (value_array[0]-0).toString(16);
		if ( value_array[1] < 16) hexcode = hexcode + "0";            
		hexcode = hexcode + (value_array[1]-0).toString(16);
		if (value_array[2] < 16) hexcode = hexcode + "0";            
		hexcode = hexcode + (value_array[2]-0).toString(16); 

		return hexcode;
	}
};

var CopySpace = {
	matrix: ['nw', 'n','ne','w','center', 'e', 'sw','s','se'],
	changebox: function(ele) {
		if (ele.className == 'cs_xxx' || ele.className == 'xxx'){
			ele.className = 'cs_on';
		} else if (ele.className == 'cs_on'){
			ele.className = 'cs_off';
		} else if (ele.className == 'cs_off'){
			ele.className = 'cs_xxx';
		}
	},
	changeSensitivity: function(ele){
		this.resetSensitivity();
		ele.className = "sensitivity_on";
	},
	resetBox:function() {
		for(var i=0; i<this.matrix.length; i++) {
			$("cs_"+this.matrix[i]).className="cs_xxx";
		}
		this.resetSensitivity();
		$('sensitivity3').className="sensitivity_on";
	},
	resetSensitivity:function(){
		for (i = 1; i <= 5; i++){
			$("sensitivity"+i).className = "sensitivity_off";
		}
	},	
	getMatrix:function() {
		var matrix={};
		for(var i=0; i<this.matrix.length; i++) {
			if($("cs_"+this.matrix[i]).className=='cs_on') {
				var loc = (this.matrix[i] == "center" ? "Center" : this.matrix[i].toUpperCase());
				matrix[loc]='Space';
			} else if($("cs_"+this.matrix[i]).className=='cs_off') {
				var loc = (this.matrix[i] == "center" ? "Center" : this.matrix[i].toUpperCase());
				matrix[loc]='Image';
			}
		}
		return matrix;
	}, 
	updateMatrix:function(matrixObj) {
		for(var i=0; i<this.matrix.length; i++) {
			var loc = (this.matrix[i] == "center" ? "Center" : this.matrix[i].toUpperCase());
					
			if(matrixObj[loc] == 'Space') {
				$("cs_"+this.matrix[i]).className = 'cs_on';
			} else if(matrixObj[loc] == 'Image') {
				$("cs_"+this.matrix[i]).className = 'cs_off';
			} else {
				$("cs_"+this.matrix[i]).className = 'cs_xxx';
			}
		}
	},
	getTolerance:function() {
		for (var i=1; i <=5; i++){
			if($("sensitivity"+i).className=="sensitivity_on") {
				return this.sensitivityToTolerance(i);
			}
		}
		return 1;
	},
	updateTolerance:function(tolerance) {
		this.resetSensitivity();
		$("sensitivity"+this.toleranceToSensitivity(tolerance)).className="sensitivity_on";
	},
	sensitivityToTolerance:function(sensitivity) {
		switch(sensitivity) {
			case 5:
				return -8;
			case 4:
				return -5;
			case 2:
				return 3;
			case 1:
				return 6;
			case 3:
			default:
				return 1;
		}
	},
	toleranceToSensitivity:function(tolerance) {
		switch(tolerance) {
			case -8:
				return 5;
			case -5:
				return 4;
			case 3:
				return 2;
			case 6:
				return 1;
			case 1:
			default:
				return 3;
		}
	}
	
};

