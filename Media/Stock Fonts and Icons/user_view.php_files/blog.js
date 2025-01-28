function showAllBlog(PostID) {
	var myElement = document.getElementById(PostID);
	var smallName = "small_" + PostID;
	var mySmallElement = document.getElementById(smallName);
	var moreName = "more_" + PostID;
	var myMoreButton = document.getElementById(moreName);
	myMoreButton.style.display='none';
	mySmallElement.style.display='none';
	myElement.style.display='inline';
	//Effect.toggle(myElement, 'blind', {duration: 0.4});
	//Effect.toggle(mySmallElement, 'appear', {duration: 0.2});
	}
function hideAllBlog(PostID) {
	var myElement = document.getElementById(PostID);
	var smallName = "small_" + PostID;
	var mySmallElement = document.getElementById(smallName);
	var moreName = "more_" + PostID;
	var myMoreButton = document.getElementById(moreName);
	myMoreButton.style.display="inline";
	mySmallElement.style.display = "inline";
	
	//Effect.toggle(mySmallElement, 'blind', {duration: 0.5});
	myElement.style.display='none';
	//Effect.toggle(myElement, 'blind', {duration: 0.4});
	}