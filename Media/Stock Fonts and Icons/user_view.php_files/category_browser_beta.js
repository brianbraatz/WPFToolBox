var CatBrowserBeta = {
	addedIDs: new Array(),
	addedLabels: new Array(),
	boolMode: 'all',
	prefix: '',

	init: function(prefix, prefLang){
		if(!prefix){
			return;
		}

		this.prefix = prefix;
		HierBrowser.setOnChange(prefix, this.switchToCat.bind(this));
		HierBrowser.setAddClick(prefix, this.addCat.bind(this));
		HierBrowser.setPrefLang(prefix, prefLang);
		HierBrowser.triggerInitialSelections(prefix);
	},

	switchToCat: function(id, label){
		this.showSelected();
		return true;
	},

	addCat: function(id, label){
		if(this.addedIDs.indexOf(id) > -1){
			return;
		}

		this.addedIDs.push(id);
		var unifiedIndex = this.addedIDs.indexOf(id);

		this.addedLabels[unifiedIndex] = label;
		this.showSelected();
		
		return true;
	},

	removeCat: function(id){
		var unifiedIndex = this.addedIDs.indexOf(id);
		if(unifiedIndex < 0){
			return false;
		}

		this.addedIDs.splice(unifiedIndex, 1);
		this.addedLabels.splice(unifiedIndex, 1);

		HierBrowser.removeClick(this.prefix, id);
		
		this.showSelected();
	},

	setBoolMode: function(mode){
		if(mode == 'any' || mode.toLowerCase() == 'or'){
			this.boolMode = 'any';
		}else{
			this.boolMode = 'all';
		}
		this.showSelected();
	},

	showSelected: function(){
		var resultsDiv = $('browseResults');
		
		var table = '';
		if(this.addedIDs.length > 0){
			table += 
				'<table width="99%">' +
					'<tr>' +
						'<td valign="top" width="200">' +
							'Match ';
			if(this.addedIDs.length > 1){
				var anySelected = '';
				var allSelected = '';
				if(this.boolMode == 'any'){
					anySelected = 'selected ';
				}else{
					allSelected = 'selected ';
				}

				table +=
								'<select ' +
									'id="MajortermBrowser_boolMode" ' +
									'style="' +
										'width: 55px; ' +
									'" ' +
									'onchange="CatBrowserBeta.setBoolMode(this.value); " ' +
								'>' +
									'<option id="any" value="any" ' + anySelected + '>Any</option>' +
									'<option id="all" value="all" ' + allSelected + '>All</option>' +
								'</select>' +
							' of the following:';
			}else{
				table +=
					'the following:';
			}
			table +=
						'</td>' +
						'<td valign="top">' +
							'<div ' +
								'style="' +
									'height: 72px; ' +

									'overflow: auto; ' +
								'" ' +
							'>' +
								'<table>';
		}
		this.addedIDs.each(
			(
				function(id, index){
					table += 
						'<tr>' +
							'<td>' +
								'<img ' +
									'src="/images/minussign_small.gif" ' +
									'onclick="CatBrowserBeta.removeCat(\''+ id + '\');" ' +
									'style="' +
										'cursor: pointer; ' +
									'"' +
								'/>' +
							'</td>' +
							'<td>' +
								'<div '+
									'onclick="HierBrowser.showColumns(\'' + this.prefix + '\', \'' + id + '\', 0);" ' +
									'style="' +
										'cursor: pointer; ' +
									'" ' +
								'>' +
									this.addedLabels[index] +
								'</div>' +
							'</td>' +
						'</tr>';
				}
			).bind(this)
		);
		if(this.addedIDs.length > 0){
			table += 
								'</table>' +
							'</div>' +
						'</td>' +
					'</tr>' +
				'</table>';
		}

		$('MajortermBrowser_selectedList').value = this.addedIDs.join(',');
		$('MajortermBrowser_conjunction').value = this.boolMode;

		resultsDiv.innerHTML = table;
	}
}
