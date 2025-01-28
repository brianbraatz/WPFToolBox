<!--
function GetContentWnd(){ 
	return parent; 
}

function AddToFavorites(title, url)
{
	var currentWindow = GetContentWnd();
	window.external.addFavorite(currentWindow.location.href, currentWindow.document.title);
	return true;
/*
if (document.all)
window.external.AddFavorite(url,title)
*/
}

function OpenEmail(description)
{
	var currentWindow = GetContentWnd();
	var oDoc = currentWindow.document;
	var strDescription = ((description == null) || (description == "")) ? oDoc.title : description;	
	if(oDoc.title == "")
		currentWindow.location.href = "mailto:?body="+BuildEmailDescription(strDescription, AddParamToURL(currentWindow.location.href, strEmailString));
	else
		currentWindow.location.href = "mailto:?subject="+escape(oDoc.title)+"&body="+BuildEmailDescription(strDescription, currentWindow.location.href);
	return true;
}

function BuildEmailDescription(strDescription,hRef){
	return escape("Here's a great article on ASPAlliance.com that you might be interested in:" +
				String.fromCharCode(13)+ String.fromCharCode(13) + strDescription + String.fromCharCode(13) + "URL: " + hRef);
}
-->