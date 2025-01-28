var AjaxUtil = {
	receiveArray: function(responseText) {
		if ( typeof responseText != 'string' ){
			return responseText;
		}
    	eval('var ret = ' + responseText);
    	return ret;
	},
	trim: function(str) {
    	return str.replace(/^(\s+)?(\S*)(\s+)?$/, '$2');
	},
	ltrim: function(str) {
    	return str.replace(/^\s*/, '');
	},
	rtrim: function(str) {
    	return str.replace(/\s*$/, '');
	},
	xmldoc2string: function(xmlDocObject){
		// use the jsolait xmlrpc module to parse the response text xml.
		// The jsolait 2.0 libraries must be included seperately.
		// @see http://jsolait.net/
		var xmlMod=null;
		try{
			var xmlMod = imprt("xml");
		} catch(e){
			//alert(e);
			throw "importing of xml module failed.";
		}
		// try to parse the response (fault responses will throw an exception)
		try {	
			var xmlString = xmlMod.node2XML(xmlDocObject.documentElement);
		} catch(e) {
			throw e;
		}
		return xmlString;
	},
	
	xmlrpc_extractpayload: function(responseText){
		// use the jsolait xmlrpc module to parse the response text xml.
		// The jsolait 2.0 libraries must be included seperately.
		// @see http://jsolait.net/
		// try to import the xmlrpc module
		var xmlrpcMod=null;
		try{
			var xmlrpcMod = imprt("xmlrpc");
		}catch(e){
			//alert(e);
			throw "importing of xmlrpc module failed.";
		}
		// try to parse the response (fault responses will throw an exception)
		try {
			var xmlPayload = xmlrpcMod.unmarshall(responseText);
		} catch(e) {
			if ( e.constructor == xmlrpcMod.Fault ){
				//alert(e.faultCode + ' ' + e.faultString);
				throw e;
			} else {
				alert(e);
			}
		}
		return xmlPayload;
	},
	
	xmlrpc_formatrequest: function(methodName, methodParameters){
		// Use the XMLRPCMessage class to format the request XML.
		// The XMLRPCMessage libraries must be included seperately.
		// @see http://www.scottandrew.com/xml-rpc
		
		// TODO: we could use the jsolait xmlrpc module to do this and
		// eliminate the duplicate functionality of XMLRPCMessage.
		
		var message = new XMLRPCMessage(methodName);
		message.addParameter(methodParameters);
		return message.xml();
	},

	xmlrpc_receivepayload: function(responseText) {
		// try to parse the response (fault responses will throw an exception)
		try {
			// extract the payload
			xmlPayload = this.xmlrpc_extractpayload(responseText);
	
			//create a DOMParser
			//var xmlDOMParser = new DOMParser();
			//create new document from string
			//var xmlDoc = xmlDOMParser.parseFromString(xmlPayload, "text/xml");
			var xmlDoc;
			try {
				// Mozilla, create a new DOMParser
				var xmlDOMParser = new DOMParser();
				xmlDoc = xmlDOMParser.parseFromString(xmlPayload, "text/xml");
			} catch(e){
				// Internet Explorer, create a new XML document using ActiveX
				// and use loadXML as a DOM parser.
				try {
					xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
					xmlDoc.async="false";
					xmlDoc.loadXML(xmlPayload);
				} catch(e){
					throw e;
				}
			}
		} catch(e) {
			/// TODO: deal with exceptions
			if ( typeof e == 'object' && e.faultCode ){
				throw e;
			} else {
				alert(e);
			}
		}
		return xmlDoc;
	},
	
	requestState: function(req) {
		switch (req.readyState) {
			case 1: case 2: case 3:
				return true;
				break;
		default:
			return false;
			break;
		}
	}, 
	
	showTimeoutMessage: function() {
		alert('The request has timed out. Please try again.');
	},
	
	defaultOnFailure: function() {
		alert('The request has failed.');
	},
	
	isFailure: function(req) {
		if(AjaxFailure.isNotXmlHttpRequest(req)) {
			return true;
		}
		if(AjaxFailure.isHeaderlessContent(req)) {
			this.showTimeoutMessage();
			return true;
		}
		if(AjaxFailure.isFailureString(req)) {
			return true;
		}
		return false;
	}
}

var AjaxFailure = {
	isHeaderlessContent: function(req) {
		return (req.getResponseHeader('X-Headerless-Content') == 'YES' ? true : false);
	},
	
	isFailureString: function(req) {
		return (req.responseText == 'ajax_fail' ? true : false);
	},
	
	isNotXmlHttpRequest: function(req) {
		return (req.responseText ? false : true);
	}
}

// global responders for catching timeouts
Ajax.Responders.register({
	onCreate: function(req) {
		if(!req.options['supressTimeout']) { 
			req['TimeoutID'] = window.setTimeout(
				function() {
					if (AjaxUtil.requestState(req.transport)) {
						if(req.options['supressReqAbort']) {
							// don't abort req
							//alert('did not abort');
						} else {
							req.transport.abort();
							//alert('aborted');
						}
						
						// call on the onTimeout Function if exists
						if(req.options['onTimeout']) {
							req.options['onTimeout'](req.transport, req.json);
						} 
						// else run the onFailure method
						else if (req.options['onFailure']) {
							AjaxUtil.showTimeoutMessage();
							req.options['onFailure'](req.transport, req.json);
						} else {
							AjaxUtil.showTimeoutMessage();
							AjaxUtil.defaultOnFailure();
						}
					}
				},
				(req.option['timeout'] ? req.options['timeout'] : 20000) // twenty seconds
			);
		}
	},
	onComplete: function(req) {
		// Clear the timeout, the request completed ok
		if(!req.options['supressTimeout']) {
			window.clearTimeout(req['TimeoutID']);
		}

		if(AjaxUtil.isFailure(req.transport)) {
			if (req.options['onFailure']) {
				req.options['onFailure'](req.transport, req.json);
			} else {
				AjaxUtil.defaultOnFailure();
			}
			return;
		}
	}
});

// modifications to Ajax Request
Ajax.Request.prototype.respondToReadyState = function(readyState) {
	var state = Ajax.Request.Events[readyState];
	var transport = this.transport, json = this.evalJSON();
	if (state == 'Complete') {
		try {
    		this._complete = true;
    		(this.options['on' + this.transport.status]
     		|| this.options['on' + (this.success() ? 'Success' : 'Failure')]
     		|| Prototype.emptyFunction)(transport, json);
  		} catch (e) {
    		this.dispatchException(e);
  		}
  		var contentType = this.getHeader('Content-type');
     	if (contentType && contentType.strip().
        	match(/^(text|application)\/(x-)?(java|ecma)script(;.*)?$/i))
          	this.evalResponse();
	}
	try {
		// added returnFlag, only execute this.options['on'+event]() if its true;
		var returnFlag = Ajax.Responders.dispatch('on' + state, this, transport, json);
		if(returnFlag == false && state == 'Complete') {
    		// don't do anything
		} else {
			(this.options['on' + state] || Prototype.emptyFunction)(transport, json);
		}
	} catch (e) {
		this.dispatchException(e);
	}
	if (state == 'Complete') {
		// avoid memory leak in MSIE: clean up
		this.transport.onreadystatechange = Prototype.emptyFunction;
	}
}