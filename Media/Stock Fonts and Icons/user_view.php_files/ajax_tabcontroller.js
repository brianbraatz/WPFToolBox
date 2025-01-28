/* holds the container's name and the current active tab */
var TabControllerContainer = '';

var TabControllerAjax = {

	/**
	 *	gets the requested tab's info
	 */
	requestTab: function (container, tabID, tabObj) {
		
		if(!this.canRequest(container, tabID)) {
			return;
		}
		
		var url = 'ajax_tabcontroller.php';
		var params = 'ajax_action=GetContent&ContainerName=' + container + '&ShowTab=' + encodeURIComponent(tabObj) + this.getPageParam(container, tabID);
			
		var myAjax = new Ajax.Request
			( 
				url, 
				{ 
					method: 'post', 
					parameters: params,
					onComplete: this.displayRequestedTab.bind(this),
					onFailure: this.requestedTabFailed.bind(this)
				}
			);
	},
	
	/**
	 *	determines if a request can be sent
	 */
	canRequest: function(container, tabID) {
		if($(container+"_Loading").style.display == "block") {
			return false;
		}
		
		this.hideTabContent(container, TabControllerContainer[container]);
		this.hideTabName(TabControllerContainer[container]);
		this.showTabName(tabID);
		this.showLoading(container);
		return true;
	},
	
	/**
	 *	gets page parameter
	 */
	getPageParam: function(container, tabID) {
		var nav = '';
		
		try { 
			nav = (eval(container + "_" + tabID + "_" + "nav"));
		} 
		catch(er) { 
			nav = '';
		}	
		       
		if(nav != '') {
			return '&page=' + nav.page;
		} 
		return '';
	},
	
	/**
	 *	displays the requested tab's info
	 */
	displayRequestedTab: function(req) {
		var data = AjaxUtil.receiveArray(req.responseText);
		
		if(data.error == 1) {
			this.requestedTabFailed(req);
			return;
		}
		
		if(!data.reload) { 
			this.setOnClick(data.container, data.tabID);
		}
		TabControllerContainer[data.container] = data.tabID;
		this.hideLoading(data.container);
		$(data.container + '_' + data.tabID + '_Content').innerHTML = data.content;
		this.showTabContent(data.container, data.tabID);
		return;
	},
	
	/**
	 *	sets the onclick to toggle
	 */
	setOnClick: function(container, tabID) {
		$(tabID).onclick = function onclick(event) { TabControllerAjax.toggleTab( container, tabID); }
	},
	
	/**
	 *	handles request failure
	 */
	requestedTabFailed: function(req) {
		this.hideLoading();
		alert('Error loading tab\'s content. Please try again.');
	},
		
	/**
	 *	shows the requested tab's content
	 */
	showTabContent: function(container, id) {
		$(container + "_" + id + "_Content").style.display="block";
	},
	
	/**
	 *	hides the tab's content
	 */
	hideTabContent: function(container, id) {
		$(container + "_" + id + "_Content").style.display="none";
	},
	
	/**
	 *	shows the requested tab's name
	 */
	showTabName: function(id) {
		$(id).className="tabContainer";
	},
	
	/**
	 *	hides the tab's name
	 */
	hideTabName: function(id) {
		$(id).className="tabContainerOff";
	},
	
	/**
	 *	shows the loading gif
	 */
	showLoading: function(container) { 
		$(container+"_Loading").style.display="block"; 
	},
	
	/**
	 *	hides the loading gif
	 */
	hideLoading: function(container) { 
		$(container+"_Loading").style.display="none"; 
	},
		
	/**
	 *	shows tab without request for its content
	 */
	toggleTab: function(container, tabID){
		this.hideTabContent(container, TabControllerContainer[container]);
		this.hideTabName(TabControllerContainer[container]);
		this.showTabName(tabID);
		this.showTabContent(container, tabID);
		TabControllerContainer[container] = tabID;
		this.setCookie(container, tabID);
		return;
	},
	
	/**
	 *	set the tab to be active when visited next time
	 */
	setCookie: function(container, tabID) {
		var today = new Date();
		var expire = new Date();
		expire.setTime(today.getTime()  +3600000*24*30);
		document.cookie = "iStockContainer_" + container + "=" + tabID +
							";path=/" +
							";expires="+expire.toGMTString() +
							";domain=.istockphoto.com" +
							";"	
	}
}