
function AjaxComment(form) {
	var url = 'http://scorbs.com/wp-content/themes/k2/comments-ajax.php';
	if (!$('commentlist')) { new Insertion.Before('pinglist', '<ol id="commentlist"></ol>'); };
	new Ajax.Updater( {
		success: 'commentlist',
		failure: 'error'
	}, url, {
		asynchronous: true,
		evalScripts: true,
		insertion: Insertion.Bottom,
		onLoading: function() { 
			$('commentload').show();
			$('error').update('');
			$('error').setStyle( { visibility: 'hidden' } );
			Form.disable('commentform');
		},
		onComplete: function(request) {
 			if (request.status == 200) {				
				if ($('leavecomment')) { $('leavecomment').remove(); }
				new Effect.Appear($('commentlist').lastChild, { duration: 1.0, afterFinish: function() { new Effect.ScrollTo($('commentlist').lastChild); } } );
				$('comments').innerHTML = parseInt($('comments').innerHTML) + 1;
				Field.clear('comment');
				Form.disable('commentform');
				setTimeout('Form.enable("commentform")',15000);
			}
  			Element.hide('commentload');
		},
		onFailure: function() {
			$('error').show();
			$('error').setStyle( { visibility: 'visible' } );
			Form.enable('commentform');
		},
		parameters: Form.serialize(form) 
		}
	);
}

function initComment() {
	if ( document.getElementById('commentform') ) {
		$('commentform').onsubmit = function() { AjaxComment(this); return false; };
		new Insertion.After('comment', '<span id="error"></span>');
		new Insertion.After('submit','<img src="http://scorbs.com/wp-content/themes/k2/images/spinner.gif" id="commentload" />');
		$('commentload').hide();
		$('error').hide();
	}
}

//Event.observe(window, 'load', initComment, false);
FastInit.addOnLoad(initComment);
