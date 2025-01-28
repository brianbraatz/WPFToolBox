/*
 dom.js
 davidfmiller
 http://www.istockphoto.com
 july 18, 2005

 changelog:

	april 15 2004:
	 - created

	september 29 2004
	 - added getRadioText method

	october 20 2004
	 - fixed bug in getParentNamed(),
	 - added getCheckValues()
	 - enable() + disable() now work with <select> and <textarea> elements
	 - getByTagName() can now accept an array as the second parameter
	 - added getLabelText() method and getNodeText() methods

	october 27 2004
	 - changed getByTagName() so that false values would
	 - no longer be included in the array

	october 28 2004
	 - changed sortOptions() and selectOption() so that
	 - they use select.options instead of getByTagName(select, "option")

	february  12 2005
	 - added getByClass()

	july 18 2005
	 - fixed bug with hide(), and show() and hide() can now accept arrays as
	   parameters
*/

/*---------------------------------------------------------------------------
								constants
-----------------------------------------------------------------------------*/

// dom node types
ATTRIBUTE_NODE = 2;
CDATA_SECTION_NODE = 4;
COMMENT_NODE = 8;
DOCUMENT_FRAGMENT_NODE = 11;
DOCUMENT_NODE = 9;
DOCUMENT_TYPE_NODE = 10;
ELEMENT_NODE = 1;
ENTITY_NODE = 6;
ENTITY_REFERENCE_NODE = 5;
NOTATION_NODE = 12;
PROCESSING_INSTRUCTION_NODE = 7;
TEXT_NODE = 3;

// xml request states
UNINITIALIZED = 0;
LOADING = 1;
LOADED = 2;
INTERACTIVE = 3;
COMPLETE = 4;

// server responses
OK = 200;
NOT_FOUND = 404;

DOM_VERSION = "0.1.2";


/*---------------------------------------------------------------------------
								convenience functions
-----------------------------------------------------------------------------*/

// escapes a string to its characters' HTML entities
function encode(s) {

	var l = s.length;
	var cooked = "";
	for (var i = 0; i < l; i++) {
		var c = s.charAt(i);
		switch (c) {
			case '\'':
				cooked += "&apos;";
				break;
			case '<':
				cooked += "&lt;";
				break;
			case '>':
				cooked += "&gt;";
				break;
			case '&':
				cooked += "&amp;";
				break;
			case "\"":
				cooked += "&quot;";
				break;
			default:
				if (c >= 128) {
					cooked += "&#" + (s.charCodeAt(i)) + ";";
				}
				break;
		}
	}	
	return cooked;
}

// if target is visible, then it becomes hidden;
// if target is hidden, it becomes visible
function hideAndSeek(target) {

	for (var i = 0; i < arguments.length; i++) {

		target = arguments[i];
		if (isArray(target)) { 
			var a = true;
			for (var i = 0; i < target.length; i++) {
				a = a && hideAndSeek(target[i]);
			}
			return a;
		} else {
			var t = get(target);
			if (! t) { return false; }

			if (t.style.display == "") {
				t.style.display = "none";
			} else {
				t.style.display = "";
			}
			return true;
		}
	}
}

// makes a node visible
// target: 
function show(target) {
/*	if (isArray(target)) {
		for (var i = 0; i < target.length; i++ ) {
			show(target[i]);
		}
	} else {
*/
		var t = get(target);
		if (! t) { return false; } 
		
		t.style.display = "";
		return true;
//	}
}

// hides a node
// target: a single string or node 
function hide(target) {
/*
	if (isArray(target)) {
		for (var i = 0; i < target.length; i++ ) {
			hide(target[i]);
		}
	} else {
*/
		var t = get(target);
		if (! t) { return false; } 
		t.style.display = "none";

		return true;
//	}
}


// hides all the children of a node
function hideChildren(target, hide) {

	display = '';
	if (arguments.length == 1 || arguments[1] == true) {
		display = 'none';
	}

	var t = get(target);
	if (! t) { return false; }

	var children = t.childNodes;

	// loop through all the children of this node and hide them
	for (var i = 0; i < children.length; i++) {
		if (children[i].nodeType == ELEMENT_NODE) { 
			children[i].style.display = display;
		}
	}

	return true;
}


// sets the text of a node (or its first child if id resolves to an element)
function setText(id, words) {
	var e = get(id);
	if (! e) { return false; }

    // if its a text node, then simply set its nodeValue property
	if (e.nodeType == TEXT_NODE) {
		e.nodeValue = words;

    // otherwise, 
	} else {
    	// remove all of its children...
		while (e.childNodes.length > 0) {
			e.removeChild(e.childNodes[0]);
		}

		// and append a new text node containing the specified text
		e.appendChild(text(words));		
	}

	return true;
}

// returns true if an element is in an array
// array - the haystack
// element - the needle
// func - an optional function to compare the needle to each element in the haystack
function inArray(array, element, func) {

	if (! func) {
		func = _argument;
	}

	var l = array.length;
	for (var i = 0; i < l; i++) {
		if (func(array[i]) == func(element)) {
			return true;
		}
	}

	return false;
}

// the default comparison function for inArray
function _argument(r) {
	return r;
}

// returns the element's next sibling element (which optionally has a specified name)

function getNextElement(id, name) { return nextElement(id, name); }
function nextElement(id, name) {

	var t = get(id);
	if (! t) { return false; }

	// loop through its sibling nodes
	while (t = t.nextSibling) {

		// make sure we have an element
		if (t.nodeType == ELEMENT_NODE) {

			// if no name has been specified, return this element
			if (! name) {
				return t;	

			// otherwise, check to see if its name matches that of the parameter
			} else if (name.toLowerCase() == t.nodeName.toLowerCase()) {
				return t;
			}
		}
	}
	return false;
}


// reverses order of a node's children
function reverseChildren(t) {

	var parent = get(t);
	if (! parent) { return false; }
	
	// get all the child nodes
	var children = parent.childNodes;
	var nodes = Array();
	var count = children.length;

    // add the children to the temporary array, and remove them from the parent
	for (var i = 0; i < count; i++) {
		nodes[i] = children.item(0);
		parent.removeChild(children[0]);
	}

	// now loop through the array in reverse order, appending the children to the parent
	for (var i = nodes.length - 1; i >= 0; i--) {
		parent.appendChild(nodes[i]);
	}
	
	return true;
}

// tag - the node to duplicate
// zid - the id of the element
function duplicate(tag, zid) {

	var e = get(tag);
	if (! e) { return false; }

	if (e.nodeType == TEXT_NODE) {
		var d = text(e.nodeValue);
		return d;
	} else {

		var d = document.createElement(e.tagName);

		if (e.attributes.length) {

			attrs = e.attributes;
			for (var i = 0; i < attrs.length; i++) {
				var av = e.getAttribute(attrs[i].nodeName);
				var a = attrs[i].nodeName;
				if (a == 'id' && arguments.length == 2) {
					var r = new RegExp(":i:", "g");
					av = av.replace(r, zid);
				} 
				d.setAttribute(a, av);
			}
		}



		if (e.hasChildNodes()) {
			var count = e.childNodes.length;
			for (var i = 0; i < count; i++) {
				
				if (arguments.length == 2) {
					var d2 = duplicate(e.childNodes[i], zid);
				} else {
					var d2 = duplicate(e.childNodes[i]);
				}
				d.appendChild(d2);
			}
		} 
	}
	return d;
}

// disables a form element
function disable(element, enabled) {

	if (arguments.length == 1) {
		enabled = false;
	}

	enable(element, enabled);

	return true;
}

// enables a form element
function enable(element, enabled) {

	if (arguments.length == 1) {
		enabled = true;
	}

	var e = get(element);
	if (!e) { return false; }

	var name = e.tagName.toLowerCase();
	if (name == "form") {
		var tags = getByTagName(e, "fieldset");
		if (tags) {
			for (var i = 0; i < tags.length; i++) {
				enable(tags[i], enabled);
			}
		}

		// enable/disable all form elements that are NOT in a fieldset
		var tags = getByTagName(e, ["input", "select", "textarea", "option"]);
		if (tags) {
			for (var i = 0; i < tags.length; i++) {
				disable(tags[i], enabled);
			}
		}
		
	} else if (name == "fieldset") {

		var tags = getByTagName(e, ["input", "select", "textarea", "option"]);
		if (tags) {
			for (var i = 0; i < tags.length; i++) {
				enable(tags[i], enabled);
			}
		}

	} else if (name == "input" || name == "select" || name == "textarea" || name == "option") {

		if (enabled) {
//			if (e.hasAttribute("disabled")) {
				e.removeAttribute("disabled");
//			}
		} else {
			e.setAttribute("disabled", "disabled");
		}
	}

	return true;
}

// get a reference to an element by id
function get(id) {
	if (typeof id == "string") {
		return document.getElementById(id);
	} else {
		return id;
	}
}

// retrieves the value of the class attribute of a node, or null if none exists
function getNodeClass(obj) {
	var result = false;
	if (obj.getAttributeNode("class") != null) {
		result = obj.getAttributeNode("class").value;
	}
	return result;
}

function getByClass(parent, elementName, className) {

	var tag = false;
	if (arguments.length == 1) {
		tag = document;
		elementName = '*';
		className = arguments[0];
	} else if (arguments.length == 2) {
		tag = document;
		className = elementName;
		elementName = parent;
	} else {
		tag = get(parent);
	}

	if (! tag) { return false; }
	var nodes = new Array();

	var elements = getByTagName(tag, elementName);
	if (! elements) { return false; }

	for (var i = 0; i < elements.length; i++) {
		var c = getNodeClass(elements[i]);
		if (c && inArray(c.split(' '), className)) {
			nodes[nodes.length] = elements[i];
		}
	}

	return nodes;
}

// creates a text node
function text(chars) {
	return document.createTextNode(chars);
}

// append a child node to its parent
function add(parent, child) {
	var p = get(parent);
	p.appendChild(node(child));
}

// remove a child node from the tree
function snip(child) {
	var c = get(child);
	if (c) {
		var p = c.parentNode;
		if (! p) { return false; }
		p.removeChild(c);
	}
	return c;
}

// removes all child nodes from a parent
function trimchildren(parent) {
	var p = get(parent);
	if (!p) { return false; } 
	while (p.hasChildNodes()) {
		var t = p.firstChild;
		p.removeChild(t);
	}
	
	return true;
}

// retrieves the first parent element of an element whose tag name is <name>
function getParentNamed(tag, name, level) {

	if (! level) {
		level = 1;
	}

	var n = get(tag);
	if (! n) { return false; }

	var parent;
	var count = 0;

	while (parent = n.parentNode) {
		if (parent.nodeName.toLowerCase() == name.toLowerCase()) {
			count++;
			if (count == level) {
				return parent;
			}
		}
		n = parent;
	}

	return false;
}

// returns the first child of tag whose node name is "name",
// or false if no such node exists
function getFirstChildNamed(tag, name) {

    var parent = get(tag);
    var nodes = getByTagName(parent, name);
    
    if (nodes && nodes.length > 0) {
        return nodes[0];
    }

    return false;
}

// retrieves a list of all child tags with the appropriate name
function getByTagName(tag, name) {
	if (arguments.length == 1) {
		return document.getElementsByTagName(tag);
	} else {
		var t = get(tag);
		if (! t) { return false; }

		if (typeof name == "string") {
			name = new Array(name);
		}
		
	
		var tags = new Array();
		
		for (var i = 0; i < name.length; i++) {
			var result = t.getElementsByTagName(name[i]);
			for (var j = 0; j < result.length; j++) {
				if (result[j]) {
					tags = tags.concat(result[j]);
				}
			}
		}

		if (! tags.length) {
			tags = false;
		}
		return tags;
	}
}

// returns the text value of a text node, or the concatenated text of all the element's child nodes
function getNodeValue(node) {
	var n = get(node);
	if (!n) { return false; }
	
	if (n.nodeType == TEXT_NODE) {
		return n.nodeValue;
	} else if (n.nodeType == ELEMENT_NODE && n.childNodes.length > 0) {

		var text = '';
		for (var i = 0; i < n.childNodes.length; i++) {
			text += getNodeValue(n.childNodes[i]);
		}
		return text;

	} else {
		return '';
	}
}

// selects all options of a select box (optionally provides a way to de-select)
function selectAll(select, b) {
	var s = get(select);
	if (b == null) { b = true; }
	
	var options = getByTagName(s, "option");
	for (var i = 0; i < options.length; i++) {
		options[i].selected = b;
	}
}

// returns the number of selected options in a <select> element
function countSelected(select) {
	var s = get(select);
	var count = 0;
	
	var options = getByTagName(s, "option");
	if (! options) { return 0; }
	for (var i = 0; i < options.length; i++) {
		if (options[i].selected) count++;
	}
	
	return count;
}

// returns an array that contains the values of all selected options for a select widget
// or false if no options are selected
function getSelectedValue(select) {

  var s = get(select);
  if (! s) { return false; }
  var count = 0;
  
  var selected = new Array();

  var options = getByTagName(s, "option");
  if (! options) { return false; }
  
  var j = 0; // the number of selected options
  for (var i = 0; i < options.length; i++) {
  	if (options[i].selected) {
  	  selected[j] = options[i].value;
  	  j++;
  	}
  }

  if (j > 0) {
    return selected;
  } else {
    return false;
  }
}

function getSelectedText(select) {

  var s = get(select);
  if (! s) { return false; }
  var count = 0;
  
  var selected = new Array();

  var options = getByTagName(s, "option");
  if (! options) { return false; }
  var j = 0;
  for (var i = 0; i < options.length; i++) {
  	if (options[i].selected) {
  	  selected[options[i].value] = options[i].text;
      j++;
  	}
  }  
  if (j > 0) {
      return selected;
  } else {
    return false;
  }
}


// an alias for getSelectedText
function getSelected(select) {
    return getSelectedText(select);
}



// an alias for removeOptionByValue
function removeOption(select, value) {
	return removeOptionByValue(select, value);
}

// removes all options from a select box
function clearOptions(select) {

	var s = get(select);
	if (! s) { return false; }

	var options = getByTagName(s, "option");
	for (var i = 0; i < s.childNodes.length; i = 0) {
		s.removeChild(s.lastChild);
	}
	return true;
}

// removes an option from a <select> element based on the option's caption/text
function removeOptionByText(select, text) {

	var s = get(select);
	if (! s) { return false; }
	var flag = false;
	
	var options = getByTagName(s, "option");
	for (var i = 0; i < options.length; ) {
		if (getNodeValue(options[i]) == text) {
			s.removeChild(options[i]);
			flag = true;
		} else {
			i++;
		}
	}
	return flag;
}


// removes an option from a <select> element based on the value attribute
function removeOptionByValue(select, value) {
	var s = get(select);
	if (! s) { return false; }
	
	var options = getByTagName(s, "option");
	for (var i = 0; i < options.length; i++) {
		if (options[i].value == value) {
			s.removeChild(options[i]);
			return true;
		}
	}
	return false;
}

// used to sort option elements by their value
function _valueSort(a, b) {
	if (a.value < b.value) { 
		return -1;
	} else if (a.value > b.value) {
		return 1;
	} else { return 0; }
}

// used to sort option elements by their caption
function _captionSort(a, b) {

	if (a.text < b.text) {
		return -1;
	} else if (a.text > b.text) {
		return 1;
	} else { return 0; } 
}

// sorts a select element's options 
// select
// f - a function to 
function sortOptions(select, f) {

	if (f == null) { 
		f = _valueSort;
	}

	var s = get(select);
	if (! s) {
		return false;
	}

	var options = select.options; //getByTagName(s, "option");
	if (! options) {
		return false;
	}

	var holder = Array();
	for (var i = 0; i < options.length; i++) {
		holder[i] = options[i];
		s.removeChild(options[i]);
	}	
	holder.sort(f);
	for (var i = 0; i < holder.length; i++) {
		s.appendChild(holder[i]);
	}

	return true;
}

function selectOption(s, value) {
	
	if (arguments.length == 1) {
		value = 0;
	}

	var select = get(s);
	if (! select) { return false; }

	var options = select.options; // getByTagName(sel, "option");
	if (! options) { return false; }

	for (var i = 0; i < options.length; i++) {	
		if (options[i].value == value) {
			options[i].selected = true;
			return true;
		}
	}

	return false;
}

function _getBoxValues(type, f, name) {

	var t = get(f);
	if (! t) { return false; }

	var inputs = getByTagName(t, "input");
	var values = new Array();

	for (var i=0; i < inputs.length; i++) {
		if (inputs[i].type == type && inputs[i].checked) {
			if (name) {
				if (inputs[i].name.toUpperCase() == name.toUpperCase()) {
					values.push(inputs[i].value);
				}
			} else {
				values.push(inputs[i].value);
			}
		}
	}
	
	if (values.length == 0) {
		return false;	
	} else {
		return values;
	}
}

function getCheckValues(f, name) {
	return _getBoxValues("checkbox", f, name)
}


function getLabelText(id) {

	var input = get(id);
	if (! input) { return false; }

	var label;
	if (id.nodeType == ELEMENT_NODE && id.nodeName.toLowerCase() == "label") { 
		label = input;
	} else {
		label = getParentNamed(input, "label");
	}
	
	if (! label) { return false; }

	return getNodeText(label);
}

function getNodeText(n) {

	var node = get(n);
	if (! node) { return false; }

	var string = '';
	if (node.nodeType == ELEMENT_NODE) {
		var children = node.childNodes;
		for (var i = 0; i < children.length; i++) {
			var t = getNodeText(children[i], trim);
			string += t ? t : '';
		}
	} else if (node.nodeType == TEXT_NODE) {

		string += node.nodeValue;

	// return a blank string for all other node types
	} else {
		return '';
	}

	return string;
}

// gets the value
function getRadioValue(f, name) {
	
	var radios = _getBoxValues("radio", f, name);
	if (radios.length) {
		return radios[0];
	} else {
		return false;
	}
}

// gets the value
function getRadioText(f, name) {

	var t = get(f);
	if (! t) { return false; }
	
	var inputs = getByTagName(t, "input");
	for (var i = 0; i < inputs.length; i++) {
		if (inputs[i].type=="radio" && inputs[i].checked) {
            if (name) {            
                if (inputs[i].name.toUpperCase() == name.toUpperCase()) {
                    // get the label that contains this radio button
                    var label = getParentNamed(inputs[i], "label");
                    return getNodeValue(label);
                }
            } else {
                var label = getParentNamed(inputs[i], "label");
                return getNodeValue(label);
			}
		}
	}	
	return false;
}

function checkBox(f, name, b, value) {

    var t = get(f);
    if (! t) { return false; }
    
    if (arguments.length < 3) {
        b = true;
    }
    
    var inputs = getByTagName(t, "input");
	for (var i = 0; i < inputs.length; i++) {

        var inputType = inputs[i].type.toUpperCase();
		if (inputType == "RADIO" || inputType == "CHECKBOX") {
            var inputName = inputs[i].name;

            if (name.toUpperCase() == inputName.toUpperCase()) {
                if (arguments.length <  4 || inputs[i].value == value) {
                    inputs[i].checked = b;
                    return true;
                } 
            } 
		}
	}

	return false;
}

/*---------------------------------------------------------------------------
								common elements
-----------------------------------------------------------------------------*/


// arguments: 
// 	#0 : href
// 	#1 : title
// 	#2 : class
// 	#3 : id
// 	#4 : style
// 	#5 : child element
function a(href, title, c, id, s, onclick, child) {
	
	var a = _createElement("a", c, id, s, child);
	
	// required attributes
	a.setAttribute("href", href);
	a.setAttribute("title", title);

	if (onclick) {
		a.setAttribute("onclick", onclick);
	}
	
	return a;
}


// arguments: 
// 	#0 : src
// 	#1 : alt
// 	#2 : width
// 	#3 : height
// 	#4 : class
// 	#5 : id
// 	#6 : style
function img(src, alt, c, id, s, width, height) {
	
	var img = _createElement("img", c, id, s);
	
	// required attributes
	img.setAttribute("src", arguments[0]);
	img.setAttribute("alt", arguments[1]);
	
	// optional attributes
	if (width != null) { img.setAttribute("width", width); } 
	if (height != null) { img.setAttribute("height", height); } 

	return img;
}

// creates a <p>
function p(t, c, id, s) {
	return _createElement("p", c, id, s, t);
}

// creates a <div>
function div(c, id, s, child) {
	return _createElement("div", c, id, s, child);
}

// creates a <span>
function span(child, c, id, s) {
	return _createElement("span", c, id, s, child);	

}

// creates an <hr>
function hr(c, id, s) {
	return _createElement("hr", c, id, s);
}

// creates a <br>
function br() {
	return document.createElement("br");
}

// creates a <strong>
function strong(t, c, id, s) {
	var e = _createElement("strong", c, id, s);
	if (t != null) {
		e.appendChild(node(t));	
	}
	return e;
}

// creates an <em>
function em(t, c, id, s) {
	var e = _createElement("em", c, id, s);
	if (t != null) {
		e.appendChild(node(t));	
	}
	return e;
}

// creates a <pre>
function pre(t, c, id, s) {
	var e = _createElement("pre", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
}

// creates a <code>
function code(t, c, id, s) {
	var e = _createElement("code", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
}

function dfn(t, c, id, s) {
	var e = _createElement("dfn", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
}

function cite(t, c, id, s) {
	var e = _createElement("cite", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
}

function del(t, cite, datetime, c, id, s) {
	var e = _createElement("del", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
	// TODO
	
}

function ins(t, cite, datetime, c, id, s) {
	var e = _createElement("cite", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
	
	// TODO
}

function blockquote(t, c, id, s) {
	var e = _createElement("quote", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
}

function address(t, c, id, s) {
	var e = _createElement("address", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}	
}

function acronym(t, lang, c, id, s) {
	var e = _createElement("address", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}
	
	if (lang != null) {
		e.setAttribute("lang", lang);
	}
}

function abbr(t, lang, c, id, s) {
	var e = _createElement("abbr", c, id, s);
	if (t != null) {
		e.appendChild(node(t));
	}	
	if (lang != null) {
		e.setAttribute("lang", lang);
	}
}

/*---------------------------------------------------------------------------
								form elements
-----------------------------------------------------------------------------*/


function checkbox(name, value, checked, onclick, c, id, s) {
	var c = _createElement("input", c, id, s);
	c.setAttribute("type", "checkbox");
	c.setAttribute("name", name);
	
	if (onclick != null) {
		c.setAttribute("onclick", onclick);
	}
	
	if (value != null) {
		c.setAttribute("value", value);
	}
	
	if (checked != null && checked) { 
		c.setAttribute("checked", "checked");
	}
	
	return c;
}

function radio(name, value, checked, onclick, c, id, s) {
	var c = _createElement("input", c, id, s);
	c.setAttribute("type", "radio");
	c.setAttribute("value", value);
	c.setAttribute("name", name);
	if (onclick != null) {
		c.setAttribute("onclick", onclick);
	}
	if (checked != null && checked) { 
		c.setAttribute("checked", "");
	}
	
	return c;
}

function label(t, input, c, id, s) {

	var l = _createElement("label", arguments[2], arguments[3], arguments[4]);
	l.appendChild(node(t));
	l.appendChild(node(input));	
	
	return l;
}

function select(c, id, s, onchange, size, multiple) {
	var s =  _createElement("select", c, id, s);

	if (size != null) {
		s.setAttribute("size", size);
	}
	
	if (multiple) {
		s.setAttribute("multiple", "multiple");
	}
	
	if (onchange != null) {
		s.setAttribute("onchange", onchange);
	}
	
	return s;
}

function button(name, value, onclick, c, id, s) {
	var b = _createElement("input", c, id, s);
	b.setAttribute("type", "button");
	b.setAttribute("name", name);
	b.setAttribute("value", value);
	
	if (onclick != null) {
		b.setAttribute("onclick", onclick);	
	}
	
	return b;
}

function reset(name, value, onclick, c, id, s) {
	var b = _createElement("input", c, id, s);
	b.setAttribute("type", "reset");
	b.setAttribute("name", name);
	b.setAttribute("value", value);
	
	if (onclick != null) {
		b.setAttribute("onclick", onclick);	
	}
	
	return b;
}

function submit(name, value, onclick, c, id, s) {

	var b = _createElement("input", c, id, s);
	b.setAttribute("type", "submit");
	b.setAttribute("name", name);
	b.setAttribute("value", value);
	
	if (onclick != null) {
		b.setAttribute("onclick", onclick);	
	}
	
	return b;
}

function option(value, name) {
	var o = _createElement("option");
	o.appendChild(node(name));
	o.setAttribute("value", value);
	
	return o;
}


function fileInput(name, c, id, s) {
	var f = _createElement("input", c, id, s);
	f.setAttribute("type", "file");
	
	return f;
}

function password(name, value, c, id, s) {
	
	var p = _createElement("input", c, id, s);
	p.setAttribute("type", "password");
	if (value != null) { 
		p.setAttribute("value", value);
	}
	
	return p;
}

function hiddenInput(name, value, id) {
	var e = _createElement("input");
	e.setAttribute("name", name);
	e.setAttribute("type", "hidden");
	if (value != null) { e.setAttribute("value", value); }
	if (id) {
		e.setAttribute("id", id);
	}
	
	return e;
}

function textInput(name, value, maxlength, size, onchange, id) {
	var t = _createElement("input");
	t.setAttribute("name", name);
	t.setAttribute("type", "text");
	t.setAttribute("value", (value != null ? value : ''));

	if (maxlength) {
		t.setAttribute("maxlength", maxlength);
	}
	
	if (size) {
		t.setAttribute("size", size);
	}

	if (onchange) {
		t.setAttribute("onchange", onchange);
	}

	if (id) {
		t.setAttribute("id", id);
	}
	
	return t;
}

function textarea(name, value, rows, cols, c, id, s) {
	var t = _createElement("textarea", c, id, s);
	t.appendChild(node(value));
	t.setAttribute("rows", rows);
	t.setAttribute("cols", cols);
	
	return t;
}


/*---------------------------------------------------------------------------
								table elements
-----------------------------------------------------------------------------*/

function table(c, id, s, spacing) {
	var t = _createElement("table", c, id, s);
	if (spacing != null) {
		t.setAttribute("cellspacing", spacing);
	}
	
	return t;
}

function tbody(c, id, s) {
	return _createElement("tbody", c, id, s);
}

function thead(c, id, s) {
	return _createElement("thead", c, id, s);
}

function tfoot(c, id, s) {
	return _createElement("tfoot", c, id, s);
}

function tr(c, id, s) { 
	return _createElement("tr", c, id, s);
}

function td(c, id, s, t) {
	var e = _createElement("td", c, id, s);
	if (t != null) { e.appendChild(node(t)); } 
	return e;
}

function th(c, id, s, t) {
	var e = _createElement("th", c, id, s);
	if (t != null) { e.appendChild(node(t)); } 
	return e;
}



/*---------------------------------------------------------------------------
								list elements
-----------------------------------------------------------------------------*/

function ol(c, id, s, start) {
	var e = _createElement("ol", c, id, s);
	if (start != null) {
		e.setAttribute("start", start);
	}
	return e;
}

function dl(c, id, s) {
	var e = _createElement("dl", c, id, s);
}

function ul(c, id, s) {

	var e = _createElement("ul", c, id, s);
	return e;
}

function li (child, c, id, s) {
	var l = _createElement("li", c, id, s);
	l.appendChild(node(child));
	
	return l;
}

function dt(child, c, id, s) {
	var l = _createElement("dt", c, id, s);
	l.appendChild(node(child));
	
	return l;
}

function dd(child, c, id, s) {
	var l = _createElement("dd", c, id, s);
	l.appendChild(node(child));
	return l;
}

/*---------------------------------------------------------------------------
								utility functions
-----------------------------------------------------------------------------*/


function node(arg) {

	if (typeof(arg) == "string") {
		return document.createTextNode(arg);
	} else if (typeof(arg) == "number") { 
		return document.createTextNode("" + arg);
	} else {
		return arg;
	}
}

function _createElement(type, c, id, s, child) {
	
	var e = document.createElement(type);
	if (arguments[1] != null) { e.setAttribute("class", c); } 
	if (arguments[2] != null) { e.setAttribute("id", id); }
	if (arguments[3] != null) { e.setAttribute("style", s); } 
	if (arguments[4] != null) { e.appendChild(node(child)); }
	
	return e;
}

/*---------------------------------------------------------------------------
						stuff related to form validation
-----------------------------------------------------------------------------*/

function checkAllBoxes(f, b, name) {
	
	var form = get(f);
	if (! form) { return false; }
	
	if (arguments.length == 1) {
		b = true;
		name = null;
	} else if (arguments.length == 2) {
		name == null;
	}
	

	var count = 0;
	var children = getByTagName(form, "input");
	for (var i = 0; i < children.length; i++) {
		if (children[i].type == "checkbox") {
			if (name) {
				if (children[i].name == name) {
					children[i].checked = b;
					count++;
				} 
			} else {
				children[i].checked = b;
				count++;
			}
		}
	}

	return count;
}

function countCheckedBoxes(f, name) {

	var form = get(f);
	if (! form) { return false; }
	
	var count = 0;
	var children = getByTagName(form, "input");
	for (var i = 0; i < children.length; i++) {
		if (children[i].type == "checkbox" && children[i].checked) {
			if (name) {
				if (children[i].name == name) {
					count++;
				} 
			} else {
				count++;
			}
		}
	}

	return count;
}

function getInputValues(f, name) {

	var form = get(f);
	if (! form) { return false; }

	var boxes = new Array();
	var children = getByTagName(form, "input");
	for (var i = 0; i < children.length; i++) {
		if (name) {
			if (children[i].name == name) {
				boxes.push(children[i].value);
			}
		} else {
			boxes.push(children[i].value);
		}
	}

	return boxes;
}

/*
 *
 */
function getCheckedBoxValues(f, name) {

	var form = get(f);
	if (! form) { return false; }
	
	var boxes = new Array();
	var children = getByTagName(form, "input");
	for (var i = 0; i < children.length; i++) {
		if (children[i].type == "checkbox" && children[i].checked) {
			if (name) {
				if (children[i].name == name) {
					boxes.push(children[i].value);
				} 
			} else {
				boxes.push(children[i].value);
			}
		}
	}

	return boxes;
}




/*
	functions copied from http://www.crockford.com/javascript/remedial.html
*/
function isAlien(a) { return isObject(a) && typeof a.constructor != 'function'; }
function isArray(obj) { return(typeof(obj.length)=="undefined") ? false : true; }
function isBoolean(a) { return typeof a == 'boolean'; }
function isEmpty(o) {
    var i, v;
    if (isObject(o)) {
        for (i in o) {
            v = o[i];
            if (isUndefined(v) && isFunction(v)) { return false; }
        }
    }
    return true;
}
function isFunction(a) { return typeof a == 'function'; }
function isNull(a) { return typeof a == 'object' && !a;}
function isNumber(a) { return typeof a == 'number' && isFinite(a);}
function isObject(a) { return (a && typeof a == 'object') || isFunction(a);}
function isString(a) { return typeof a == 'string';}
function isUndefined(a) { return typeof a == 'undefined'; } 
