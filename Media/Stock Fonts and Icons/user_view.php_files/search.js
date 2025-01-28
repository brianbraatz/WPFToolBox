function TopsearchFormValidate(){
	/*
	var searchField = GetDocumentElementByID('topsearch');
	if ( searchField.text.value == "" && searchField.color.value == "" && (searchField.copySpace.value=='{"Tolerance":1,"Matrix":[]}' || searchField.copySpace.value=='{"Matrix":[],"Tolerance":1}') ){
		alert("Please enter a search term in the form.");
		return true;
	}
	else {
		return true;
	}
	*/
	return true;
}

////////////////////////////////////////////////////////////////////////////////
/// @name ToggleSearchOptionGroup
/// @desc toggle a search option group <DIV> on or off depending on its current state
///
/// @param groupID string - the ID of the group <DIV>
/// @param iconID string - the ID of the open/close arrow <IMG>
/// @param auto boolean - true if the script is being automatically opened (perhaps
/// programmaticly), false if the group is being manually toggled. For manual toggles,
/// the groups "state" (opened or closed) is stored in a cookie and remembered on
/// subsequent page views.
////////////////////////////////////////////////////////////////////////////////
function ToggleSearchOptionGroup(groupID, iconID, auto){
	groupObj = GetDocumentElementByID(groupID);
	iconObj = GetDocumentElementByID(iconID);
	if ( groupObj.style.display == 'none' || !groupObj.style.display ){
		groupObj.style.display = 'block';
		iconObj.src = '/images/icon_arrow_down.gif';
		if ( !auto ){ SetSearchOptionCookie(groupID, true) };
	} else {
		groupObj.style.display = 'none';
		iconObj.src = '/images/icon_arrow_right.gif';
		if ( !auto ){ SetSearchOptionCookie(groupID, false) };
	}
}

////////////////////////////////////////////////////////////////////////////////
/// @name SetSearchOptionCookie
/// @desc store the current opened/closed state of a search option group <DIV>
/// in a cookie for the benefit of subsequent page views.
/// @see ToggleSearchOptionGroup
///
/// @param groupID string - the ID of the group <DIV>
/// @param auto state - true if the group <DIV> is open, false if it is closed
////////////////////////////////////////////////////////////////////////////////
function SetSearchOptionCookie(groupID, state) {
	var today = new Date();
	var expire = new Date();
	expire.setTime(today.getTime()+3600000*24*30);
	if ( state ){
		stateString = 'on';
	} else {
		stateString = 'off';
	}
	document.cookie = "iStock_" + groupID + "=" + stateString +
	";path=/" +
	";expires="+expire.toGMTString() +
	";domain=.istockphoto.com" +
	";"						
}

function GetDocumentElementByID(objID) {

	// specify how an html element should be obtained based on different browser
	var ie = document.all;
	var dom = document.getElementById;

  if ( ie ) {
    return document.all[objID];
  }
  else if ( dom ) {
    return document.getElementById(objID);
  }
}

