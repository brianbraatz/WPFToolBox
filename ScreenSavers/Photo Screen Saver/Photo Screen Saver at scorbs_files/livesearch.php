
Livesearch = Class.create();

Livesearch.prototype = {
	initialize: function(searchform, targetitem, hideitem, url, searchprompt, buttonvalue) {
		this.searchform = searchform;
		this.targetitem = targetitem;
		this.hideitem = hideitem;
		this.url = url;
		this.searchprompt = searchprompt;
		this.buttonvalue = buttonvalue;

		this.searchfield = 's';
		this.loaditem = 'searchload';
		this.resetbutton = 'searchreset';
		this.submitbutton = 'searchsubmit';

		this.lastsearch = '';
		this.t = null;  // Init timeout variable
		this.isActive = false;

		Event.observe(window, "load", this.onLoading.bindAsEventListener(this));
	},

	onLoading: function() {
		if ( $(this.searchform) ) {
			$(this.searchform).update('<input type="text" id="'+this.searchfield+'" name="'+this.searchfield+'" class="livesearch" autocomplete="off" value="'+this.searchprompt+'" /><span id="'+this.resetbutton+'"></span><span id="'+this.loaditem+'"></span><input type="submit" id="'+this.submitbutton+'" value="'+this.buttonvalue+'" />');

			$(this.submitbutton).hide();
			$(this.loaditem).hide();
			new Effect.Fade(this.resetbutton, { duration: 0, to: 0.3 });

			Event.observe(this.searchfield, 'focus', this.onInputFocus.bindAsEventListener(this));
			Event.observe(this.searchfield, 'blur', this.onInputBlur.bindAsEventListener(this));
			Event.observe(this.searchfield, 'keyup', this.readyLivesearch.bindAsEventListener(this));

			this.resetListener = this.resetLivesearch.bindAsEventListener(this);
		}
	},

	onInputFocus: function() {
		if ($F(this.searchfield) == this.searchprompt) {
			$(this.searchfield).value = '';
		}
	},

	onInputBlur: function() {
		if ($F(this.searchfield) == '') {
			$(this.searchfield).value = this.searchprompt;
		}
	},

	readyLivesearch: function(event) {
		var code = event.keyCode;
		if (code == Event.KEY_ESC || ((code == Event.KEY_DELETE || code == Event.KEY_BACKSPACE) && $F(this.searchfield) == '')) {
			this.resetLivesearch.bind(this);
		} else if (code != Event.KEY_RETURN) {
			if (this.t) clearTimeout(this.t);
	        this.t = setTimeout(this.doLivesearch.bind(this), 400);
		}
	},

    doLivesearch: function() {
		if ($F(this.searchfield) == this.lastsearch) return;

		new Effect.Fade(this.resetbutton, { duration: 0.1 });
		new Effect.Appear(this.loaditem, { duration: 0.1 });

		if (!this.isActive) {
			this.isActive = true;

			if ( ! $(this.hideitem) && K2.RollingArchives ) {
				K2.RollingArchives.saveRollingState();
			}
		}

		this.lastsearch = $F(this.searchfield);
		var query = $(this.searchfield).serialize();

		new Ajax.Updater(
			this.targetitem,
			this.url,
			{
				method: 'get',
				evalScripts: true,
				parameters: query + '&k2dynamic=1',
				onComplete: this.searchComplete.bind(this)
		});
	},
	
	searchComplete: function() {
		if ( $(this.hideitem) ) {
			$(this.hideitem).hide();
		}

		new Effect.Fade(this.loaditem, { duration: 0.1 });
		
		Event.observe(this.resetbutton, 'click', this.resetListener);
		new Effect.Appear(this.resetbutton, { duration: 0.1 });
		$(this.resetbutton).style.cursor = 'pointer';
	},

	resetLivesearch: function() {
		this.isActive = false;
		this.lastsearch = '';

		$(this.searchfield).value = this.searchprompt;

		Event.stopObserving(this.resetbutton, 'click', this.resetListener);
		new Effect.Fade(this.resetbutton, { duration: 0.1, to: 0.3 });
		$(this.resetbutton).style.cursor = 'default';

		if ( $(this.hideitem) ) {
			$(this.targetitem).hide();
			$(this.hideitem).show();
			$(this.targetitem).update();
		} else if ( K2.RollingArchives ) {
			K2.RollingArchives.restoreRollingState();
		}
	}
}
