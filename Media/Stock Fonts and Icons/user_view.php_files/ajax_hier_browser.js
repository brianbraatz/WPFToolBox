var HierBrowser = {
	_debug: false,
  busyURL: 'images/loading.gif',
  busySmallURL: 'images/loading_small.gif',
	kidArrowURL: 'images/hb_arrow.gif',
	dataType: new Array,
	currentID: new Array,
	Prefix: new Array,
	name: new Array,
	knownIDs: new Array,
	knownColumns: new Array,
	knownParents: new Array,
	knownLabels: new Array,
	knownNumKids: new Array,
	currentPathFromRoot: new Array,
	useAddClick: new Array,
	addedIDs: new Array,
	onChangeFunc: new Array,
	addClickFunc: new Array,
	initialSelections: new Array,
	prefLang: new Array,

  failed: function(req){
		alert('An error occured while communicating with the server. Please try again.');
	},

	getBrowserID: function(prefix){
		browserID = this.Prefix.indexOf(prefix);
		if(browserID == -1){
			alert('Browser Not Found: ' + prefix + '\nin: ' + this.Prefix);
		}
		return browserID;
	},

	setDataType: function(prefix, type){
		browserID = this.getBrowserID(prefix);
		this.dataType[browserID] = type;
	},

	setName: function(prefix, name){
		browserID = this.getBrowserID(prefix);
		this.name[browserID] = name;
	},

	setPrefix: function(prefix){
		this.Prefix.push(prefix);
		browserID = this.getBrowserID(prefix);

		this.knownIDs[browserID] = new Array;
		this.knownColumns[browserID] = new Array;
		this.knownParents[browserID] = new Array;
		this.knownLabels[browserID] = new Array;
		this.knownNumKids[browserID] = new Array;
		this.currentPathFromRoot[browserID] = new Array;
		this.addedIDs[browserID] = new Array;
		this.initialSelections[browserID] = new Array;

		this.prefLang[browserID] = '4';
	},

	setPrefLang: function(prefix, langID){
		browserID = this.getBrowserID(prefix);
		this.prefLang[browserID] = langID;
	},

	getCurrentID: function(prefix){
		browserID = this.getBrowserID(prefix);
		return this.currentID[prefix];
	},

	getColumnsAjax: function(prefix, id, columnIndex){
		//alert('Get ID: ' + id + '\nCol: ' + columnIndex);
		browserID = this.getBrowserID(prefix);

		this.busy(prefix, true, id);
		var url = "ajax_class_creator.php";
		var params = "ajax_action=getColumns&ajax_class=hierbrowser" +
			"&prefix=" + prefix +
			"&prefLang=" + this.prefLang[browserID] +
			"&id=" + id +
			"&dataType=" + this.dataType[browserID] +
			"&addClick=" + this.useAddClick[browserID] + 
			"&hbName=" + this.name[browserID] +
			"&columnIndex=" + columnIndex;

		var req = new Ajax.Request(
			url,
			{
				method: 'post',
				parameters: params,
				onComplete: this.showColumnsAjax.bind(this),
				onFailure: this.failed.bind(this)
			}
		);
		this.currentID[browserID] = id;
	},

	showColumnsAjax: function(req){
		//alert(req.responseText);
		var data = AjaxUtil.receiveArray(req.responseText);

		if(data.error == 1) {
			this.failed();
		}

		if(data.HBError){
			alert(data.HBError);
			return;
		}

		prefix = data.prefix;
		browserID = this.getBrowserID(prefix);

		if(data.cols){
			var colDiv = null;
			var colIndex = 0;
			data.cols.each(
				(
					function(col, index){
						colIndex = data.unifiedIndex[index];
						colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
						if(!colDiv){
							var table = $(this.Prefix[browserID] + '_table').tBodies[0];
							var newCol = table.rows[0].insertCell(-1);
							newCol.vAlign = "top";
							newCol.innerHTML = '<div id="' + this.Prefix[browserID] + '_col' + colIndex + '" style="display: none;"></div>';

							var prevCol = $(this.Prefix[browserID] + '_col' + (colIndex-1));

							colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
							colDiv.style.display = 'none';

							colDiv.style.border = prevCol.style.border;
							colDiv.style.margin = prevCol.style.margin;
							colDiv.style.padding = prevCol.style.padding;
							colDiv.style.height = prevCol.style.height;
							colDiv.style.width = prevCol.style.width;
							colDiv.style.overflow = prevCol.style.overflow;
							colDiv.style.position = prevCol.style.position;
						}

						colDiv.innerHTML = col;
						colDiv.style.display = 'block';

						this.storeCol(prefix, data.colIDs[index], col, data.parentIDs[index], data.labels[index], data.numKids[index]);
					}
				).bind(this)
			);

			colIndex++;

			colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
			while(colDiv){
				colDiv.style.display = 'none';
				colDiv.innerHTML = '';

				colIndex++;
				colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
			}

			this.hideAdded(prefix);

			if(data.siblings){
				data.siblings.each(
						(
 						 function(sibling, index){
							this.storeCol(prefix, sibling, null, data.parentID, data.siblingLabels[index], data.siblingNumKids[index]);
						}
					).bind(this)
				);
			}
		}

		//alert(this.onChange);
		this.onChange(prefix, data.id, data.label);

		this.highlightPathFromRoot(prefix, false);
		this.currentPathFromRoot[browserID] = this.getPathFromRoot(prefix, data.id);
		this.debug_showStore();
		this.highlightPathFromRoot(prefix, true);
		this.slideColumns(prefix);
		
		this.busy(prefix, false, data.id);
		return;
	},

	showColumns: function(prefix, id, colIndex){
		browserID = this.getBrowserID(prefix);

		var unifiedIndex = null;
		var col = null;
		var hasKids = false;
		var pathFromRoot = this.getPathFromRoot(prefix,id);
		var colDiv = null;

		if(pathFromRoot == false){
			this.getColumnsAjax(prefix, id, colIndex);
			return 0;
		}

		id = pathFromRoot[colIndex];
		unifiedIndex = this.knownIDs[browserID].indexOf(id);

		do{
			col = this.knownColumns[browserID][unifiedIndex];
			if(col == null){
				this.getColumnsAjax(prefix, id, colIndex);
				return 0;
			}

			colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
			if(!colDiv){
				var table = $(this.Prefix[browserID] + '_table').tBodies[0];
				var newCol = table.rows[0].insertCell(-1);
				newCol.vAlign = "top";
				newCol.innerHTML = '<div id="' + this.Prefix[browserID] + '_col' + colIndex + '" style="display: none;"></div>';

				var prevCol = $(this.Prefix[browserID] + '_col' + (colIndex-1));

				colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
				colDiv.style.display = 'none';

				colDiv.style.border = prevCol.style.border;
				colDiv.style.margin = prevCol.style.margin;
				colDiv.style.padding = prevCol.style.padding;
				colDiv.style.height = prevCol.style.height;
				colDiv.style.width = prevCol.style.width;
				colDiv.style.overflow = prevCol.style.overflow;
				colDiv.style.position = prevCol.style.position;
			}

			colDiv.innerHTML = col;
			colDiv.style.display = 'block';

			colIndex++;
			id = pathFromRoot[colIndex];
			unifiedIndex = this.knownIDs[browserID].indexOf(id);

			hasKids = this.knownNumKids[browserID][unifiedIndex];
			if(hasKids == null){
				hasKids = true;
			}else if(hasKids > 0){
				hasKids = true;
			}else{
				hasKids = false;
			}
		}while((colIndex < pathFromRoot.length) && hasKids);

		colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
		while(colDiv){
			colDiv.style.display = 'none';
			colDiv.innerHTML = '';

			colIndex++;
			colDiv = $(this.Prefix[browserID] + '_col' + colIndex);
		}
		
		this.highlightPathFromRoot(prefix, false);
		this.currentPathFromRoot[browserID] = pathFromRoot;
		this.debug_showStore();
		this.highlightPathFromRoot(prefix, true);
		this.hideAdded(prefix);
		this.slideColumns(prefix);

		unifiedIndex = this.knownIDs[browserID].indexOf(pathFromRoot[pathFromRoot.length - 1]);
		
		this.onChange(prefix, this.knownIDs[browserID][unifiedIndex], this.knownLabels[browserID][unifiedIndex]);

		return;
	},

	onChange: function(prefix, id, label){
		browserID = this.getBrowserID(prefix);

		if(this.onChangeFunc[browserID] == null){
			alert('No onchange function set\nID: ' + id + '\nLabel: ' + label);
		}else{
			this.onChangeFunc[browserID](id, label);
		}
	},

	addInitialSelection: function(prefix, mtID, label){
		browserID = this.getBrowserID(prefix);

		var initSelect = {
			id: mtID,
			label: label
		};

		this.initialSelections[browserID].push(initSelect);
	},

	triggerInitialSelections: function(prefix){
		browserID = this.getBrowserID(prefix);

		this.initialSelections[browserID].each(
			(
				function(selection, index){
					this.addClick(prefix, selection.id, selection.label);
				}
			).bind(this)
		);
	},

	addClick: function(prefix, id, label){
		browserID = this.getBrowserID(prefix);

		var plusSign = $(this.Prefix[browserID] + '_addClick' + id);
		if(plusSign){
			plusSign.style.visibility = 'hidden';
		}
		this.addedIDs[browserID].push(id);

		if(this.addClickFunc[browserID] == null){
			alert('No addClick function set\nID: ' + id + '\nLabel: ' + label);
		}else{
			this.addClickFunc[browserID](id, label);
		}
	},

	removeClick: function(prefix, id){
		browserID = this.getBrowserID(prefix);

		var plusSign = $(this.Prefix[browserID] + '_addClick' + id);
		if(plusSign){
			plusSign.style.visibility = 'visible';
		}
		this.addedIDs[browserID] = this.addedIDs[browserID].without(id);
	},

	hideAdded: function(prefix){
		browserID = this.getBrowserID(prefix);

		var plussign = null;

		this.addedIDs[browserID].each(
			(
				function(id, index){
					plussign = $(this.Prefix[browserID] + '_addClick' + id);
					if(plussign){
						plussign.style.visibility = 'hidden';
					}
				}
			).bind(this)
		);
	},		

	setOnChange: function(prefix, func){
		browserID = this.getBrowserID(prefix);

		this.onChangeFunc[browserID] = func;
	},

	setAddClick: function(prefix, func){
		browserID = this.getBrowserID(prefix);

		this.addClickFunc[browserID] = func;

		if(func != null){
			this.useAddClick[browserID] = true;
		}else{
			this.useAddClick[browserID] = false;
		}
	},

	highlightPathFromRoot: function(prefix, show){
		browserID = this.getBrowserID(prefix);

		var newClassName = 'HBElementSelected';
		if(!show){
			newClassName = 'HBElementUnselected';
		}
		var lastColOffset = 0;
		var lastColWidth = 0;

		this.currentPathFromRoot[browserID].each(
			(
				function(id, index){
					var selectedDiv = $(this.Prefix[browserID] + '_element' + id);
					var col = $(this.Prefix[browserID] + '_col' + (index - 1));
					if(selectedDiv){
						selectedDiv.className = newClassName;
						if(show){
							selectionOffset = Position.positionedOffset(selectedDiv);
							colOffset = Position.positionedOffset(col);
							scrollOffset = selectionOffset[1] - colOffset[1];
							col.scrollTop = scrollOffset;

							lastColOffset = Position.positionedOffset(col)[0];
							lastColWidth = col.offsetWidth;
						}
					}
				}
			).bind(this)
		);

		if(show){
			var viewPort = $(this.Prefix[browserID]);
			var scrollTo = (lastColOffset + (2*lastColWidth)) - viewPort.offsetWidth;
			//alert('Offset: ' + lastColOffset + '\nwidth: ' + lastColWidth + '\nViewport width: ' + viewPort.offsetWidth +'\nscroll left is: ' + scrollTo);
			viewPort.scrollLeft = scrollTo;
		}
	},

	slideColumns: function(prefix){
		return;
	},

	storeCol: function(prefix, ID, col, parentID, label, numKids){
		browserID = this.getBrowserID(prefix);

		var unifiedIndex = this.knownIDs[browserID].indexOf(ID);
		if(unifiedIndex == -1){
			unifiedIndex = this.knownIDs[browserID].length;
			this.knownColumns[browserID][unifiedIndex] = null;
		}

		this.knownIDs[browserID][unifiedIndex] = ID;
		if(col != null){
			this.knownColumns[browserID][unifiedIndex] = col;
		}
		this.knownParents[browserID][unifiedIndex] = parentID;
		this.knownLabels[browserID][unifiedIndex] = label;
		this.knownNumKids[browserID][unifiedIndex] = numKids;

		this.debug_showStore();
	},

	debug_showStore: function(){
		if(!this._debug){
			return;
		}

		html = '';

		if(this.currentPathFromRoot){
			html += 'PFR: ' + this.currentPathFromRoot.join(' -> ') + '<br/>';
		}else{
			html += 'no PFR yet<br/>';
		}

		if(this.knownIDs){
			html += '<table border="1">' +
				'<tr>' +
					'<td>UI</td>' +
					'<td>ID</td>' +
					'<td>label</td>' +
					'<td>parent</td>' +
					'<td>numKids</td>' +
					'<td>col</td>' +
				'</tr>';

			this.knownIDs.each(
				(
					function(id, index){
						var hasCol = 'no';
						if(this.knownColumns[index] != null){
							hasCol = 'yes';
						}
						html += '<tr>'+
							'<td>' + index + '</td>' +
							'<td>' + id + '</td>' +
							'<td>' + this.knownLabels[index] + '</td>' +
							'<td>' + this.knownParents[index] + '</td>' +
							'<td>' + this.knownNumKids[index] + '</td>' +
							'<td>' + hasCol + '</td>' +
						'</tr>';
					}
				).bind(this)
			);
		
			html += '</table>';
		}else{
			html += 'no store yet<br/>';
		}

		$('stat').innerHTML = html;
	},

	getPathFromRoot: function(prefix, id){
		browserID = this.getBrowserID(prefix);

		//alert('Get PFR for: ' + id);
		var path = false;
		var unifiedIndex = this.knownIDs[browserID].indexOf(id);

		if(unifiedIndex < 0){
			//alert('undefined index: ' + id);
			return new Array;
		}
		
		var parentID = this.knownParents[browserID][unifiedIndex];
		
		if(parentID == null){
			//alert('unknown parent: ' + unifiedIndex);
			return new Array;
		}
		
		if(this.isRoot(id)){
			//alert(id + ' is the root');
			path = new Array;
			path[0] = id;
		}else{
			path = this.getPathFromRoot(prefix, parentID);
			if(path === false){
				return false;
			}
			path.push(id);
		}

		return path;
	},

	setPathFromRoot: function(prefix, path){
		browserID = this.getBrowserID(prefix);

		var pathArray = path.split(/,/);
		this.currentPathFromRoot[browserID] = pathArray;
	},

	isRoot: function(id){
		if(id > 1){
			return false;
		}
		return true;
	},

	busy: function(prefix, isBusy, id){
		browserID = this.getBrowserID(prefix);

		var loadingImg = null;
		if(!id){
			loadingImg = $('loadingImage');
			if(loadingImg){
				if(isBusy){
					loadingImg.style.visibility = 'visible';
				}else{
					loadingImg.style.visibility = 'hidden';
				}
			}
		}else{
			var arrowImg = $(this.Prefix[browserID] + '_kidArrow_' + id);
			var loadingImg = $(this.Prefix[browserID] + '_kidLoading_' + id);
			if(!loadingImg || !arrowImg){
				return;
			}

			if(isBusy){
				arrowImg.style.display = 'none';
				loadingImg.style.display = 'inline';
			}else{
				arrowImg.style.display = 'inline';
				loadingImg.style.display = 'none';
			}
		}
	}
}
