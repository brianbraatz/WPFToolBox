var SearchBarBetaAjax = Class.create();
SearchBarBetaAjax.prototype = {
	initialize: function(context) {
		this.colorSelector='Simple';
	},
	
	toggleFileType: function(fileType) {
		if(!$("SearchBarFileType1").checked && !$("SearchBarFileType4").checked && !$("SearchBarFileType7").checked && !$("SearchBarFileType8").checked) {
			$("SearchBarFileType"+fileType).checked = true;
			return
		}
		
		var url = "ajax_class_creator.php";
		var params = "ajax_action=toggleFileType&ajax_class=searchbarbeta&fileType="+fileType;
		var myAjax = new Ajax.Request
			(
				url, 
				{ 
					method: "get", 
					parameters: params,
					onComplete: this.toggleFileTypeCompleted.bind(this),
					onFailure: this.failed.bind(this)
				}
			);
	},
	
	toggleFileTypeCompleted: function(req) {
		var data = AjaxUtil.receiveArray(req.responseText);	
		if(data['fileType'] == 8) {
			if($(data['advancedFileFilter']+'4_3') && $(data['advancedFileFilter']+'4_3').value !="" && $(data['advancedFileFilter']+'16_9') && $(data['advancedFileFilter']+'16_9').value !="")	{
				$(data['advancedFileFilter']+'4_3').value=data['value']+' 4_3';
				filtersAndOptions.params['FO_FileType'].value[data['advancedFileFilter']+'4_3'].value=data['value']+' 4_3';
				$(data['advancedFileFilter']+'4_3').onchange();
				$(data['advancedFileFilter']+'16_9').value=data['value']+' 16_9';
				filtersAndOptions.params['FO_FileType'].value[data['advancedFileFilter']+'16_9'].value=data['value']+' 16_9';
				$(data['advancedFileFilter']+'16_9').onchange();
			}
		} else {
			if($(data['advancedFileFilter']) && $(data['advancedFileFilter']).value !="") {
				$(data['advancedFileFilter']).value=data['value'];
				filtersAndOptions.params['FO_FileType'].value[data['advancedFileFilter']].value=data['value'];
				$(data['advancedFileFilter']).onchange();
			}
		}
		$("SearchBarFileTypeSizePrice").value=data.jsonValue;
	}, 
	
	setSortBy: function(value, skipAJAX) {

		if ( !skipAJAX ){
			skipAJAX = false;
		}

		$('SearchBarOrder').value=value;

		if ( skipAJAX == false ){
			var url = "ajax_class_creator.php";
			var params = "ajax_action=SetSortBy&ajax_class=searchbarbeta&sortBy="+value;
			var myAjax = new Ajax.Request
				(
					url, 
					{ 
						method: "get", 
						parameters: params,
						onComplete: this.setSortByCompleted.bind(this),
						onFailure: this.failed.bind(this)
					}
				);	
		}
	},
	
	setSortByCompleted: function(req) {
		if($("FO_Sort")) {
			$("FO_Sort").value = req.responseText;
		}
	},
	
	failed: function(req) {
		alert(req.responseText);
	},

	setHiddenSearchFormFields: function(filters){
		for ( i in filters ){
			//alert(i + ' = ' + filters[i]);
			$(i).value = filters[i];
		}
	},

	getTextValue: function(obj, tipObj){
		if ( obj.value == tipObj.value ){
			return '';
		} else {
			return obj.value;
		}
	},

	toggleSearchBar: function(toSimple) {
		var containerBasic = $('SearchBarContainerBasic');
		var containerAdvanced = $('SearchBarContainerAdvanced');
		//var sendState = $('TopSearchAdvancedSearchState');
		
		if(toSimple) {
			containerBasic.style.display = 'block';
			togglePopup(containerAdvanced);
			//sendState.value='';
		} else {
			//make sure it's initalized
			if(!filtersAndOptions.searchFiltersAndOptionsInitialized) {
				buildPopupIFrame(containerAdvanced, false);
				if($(containerAdvanced.id+"IFrame")) {
					positionPopup(containerAdvanced, $(containerAdvanced.id+"IFrame"), 0, 0);
					if($("ControlPanelToggle")) {
						var cp = $("ControlPanelToggle").onclick;
						$("ControlPanelToggle").onclick = function() {
							cp();
							window.setTimeout( 
								function() {
									var containerAdvanced = $('SearchBarContainerAdvanced');
									positionPopup(containerAdvanced, $(containerAdvanced.id+"IFrame"), 0, 0);
								}
							, 1000)
						}
					}
				}
				
			}
			filtersAndOptions.initializeAdvanced();
			containerBasic.style.display = 'none';
			togglePopup(containerAdvanced);
			//sendState.value='1';
		}
		//readjust the zoom control
		if($('ZoomImageDiv')) {
			var pos = Position.cumulativeOffset($("ZoomImageDiv"));
			$("ZoomControlDiv").style.top=(pos[1]+10)+"px";
			this.zoomControlDraggable.options.constraintarea.top = pos[1]+(this.viewport.height-parseInt($("ZoomControlDiv").style.height)-1);				
			this.zoomControlDraggable.options.constraintarea.bottom = -pos[1];
		}
	},
	
	setSearchWithin:function(prefix, disableMemberField) {
		if(prefix=="FO_") {
			var isWithin = $("FO_SearchWithinCheckbox").checked;
			$("SearchWithinCheckbox").checked = isWithin;
		} else {
			var isWithin = $("SearchWithinCheckbox").checked;
			// certain filters that trigger "search within" on the basic
			// form are ignored on the adavanced form, so the search within
			// may not exist in the advanced bar
			if($("FO_SearchWithinCheckbox")) {
				$("FO_SearchWithinCheckbox").checked = isWithin;
			}
		}
		if(isWithin) {
			this.setHiddenSearchFormFields($(prefix+"SearchWithinContract").value.evalJSON(true));
		} else {
			this.setHiddenSearchFormFields($(prefix+"SearchWithinNew").value.evalJSON(true));			
		}
		if(disableMemberField && $("FO_SearchTypeMember")) {
			$("FO_SearchTypeMember").disabled=isWithin;
		}
	}
};
