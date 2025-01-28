var NavigationBarAjax = Class.create();
NavigationBarAjax.prototype = {
	initialize: function() {
		this.page = 1;
		this.link = '';
	}, 
	
	setLink: function(link) {
		this.link = link;
	},
	
	getLink: function(page) {
		this.page = parseInt(page);
		this.link();
		return true;
	}
};