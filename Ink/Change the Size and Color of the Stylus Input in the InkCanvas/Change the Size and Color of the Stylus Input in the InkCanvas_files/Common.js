function ShowCatalogDialog(zoneID, postBackReference) {
    var features = "resizable: yes; status: no; scroll: yes; help: no; center: yes; dialogWidth: 800px; dialogHeight: 500px;";
    ShowModalDialog("CatalogDialog.aspx", features, postBackReference, zoneID) ;
}



function ShowModalDialog(url, features, postBackReference, args)
{
    //var rv = window.showModalDialog(url, args, features);
    //if( rv != null && rv != "undefined" ) {
    //    eval(postBackReference.replace("[[WEBPART]]", rv));
    //} 
    
    DisplayDialog(url + "?postbackReference=" + postBackReference) ;         
}


function DisplayDialog(url, opts, name) {
	if( opts == null || opts == "undefined" )
		opts = "width=600,height=350,resizable=yes,status=no,scrollbars=yes" ;
		
	var winName = (name) ? winName : "" ;
	
	if( DisplayDialog.arguments.length >= 2 )
		opts =  DisplayDialog.arguments[1] ;
		
	var hwnd = window.open( url, winName, opts ) ;
	if( (document.window != null ) && (!hwnd.opener) )
		hwnd.opener = document.window ;
		
	hwnd.moveTo( 80, 80 ) ;
}

function CloseDialog() {
	if( window.opener ) {
		window.opener.focus() ;
		}
		
	self.close() ;
}


function CloseCatalogDialog(postBackReference, returnValue) {

     alert( "Postback Reference: " + postBackReference + "\nReturn Value: " + returnValue ) ;
	if( window.opener ) {
		window.opener.DoCatalogPostBack(postBackReference, returnValue) ;
	
	    CloseDialog() ;
    }
}



function DoCatalogPostBack(postBackReference, returnValue) {
    eval(postBackReference.replace("[[WEBPART]]", returnValue));
}


function ToggleEditorDisplay( divClientID, imgClientUrl, expandImageUrl, minimizeImageUrl ) {

    var el = document.getElementById(divClientID) ;
    if( el.style.display=='none' ) {
        el.style.display='';
        document.images[imgClientUrl].src= minimizeImageUrl;
    } else {
        el.style.display='none';
        document.images[imgClientUrl].src= expandImageUrl;
    }
}

function CommitSearch( event, e ) {
	var keyCode = (event) ? event.keyCode : keyStroke.which; 
	if (keyCode == 13) {
		top.location = googleSearchUrl + e.value ;
		return false ;
	}else{
		return true ;
	}
}

function CommitSearch2( elementId ) {
	var e = document.getElementById( elementId ) ; 
	top.location = googleSearchUrl + e.value ;
}