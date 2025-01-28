function hideAllFeatures(list)
{
    var allListItems = list.getElementsByTagName("li");
    for(var i=0; i<allListItems.length; i++)
    {
        allListItems[i].className = "";
        allPanels = allListItems[i].getElementsByTagName("div");
        allPanels[0].style.visibility = "hidden";
    }
}

function overFeature(listItem)
{
    hideAllFeatures(listItem.parentNode);
    
    listItem.className = "highlightedFeature";
    var allPanels = listItem.getElementsByTagName("div");
    allPanels[0].style.visibility = "visible";
}

function initFeatures()
{
    var allLists = document.getElementsByTagName("ul");
    for(var i=0; i<allLists.length; i++)
    {
        if (allLists[i].className == "feature-list")
        {
            var allListItems = allLists[i].getElementsByTagName("li");
            overFeature(allListItems[0]);
        }
    }
}

function toggleVideo(link, title, activeTitle, url)
{
    var isActive = link.title == activeTitle;
    link.title = isActive ? title : activeTitle;
    var text = null;
    
    if (link.innerText) link.innerText = link.title;
    else if (link.textContent) link.textContent = link.title;
    else if (link.text) link.text = link.title;
            
    if (isActive)
    {
        // Show the image, hide the video
    }
    else
    {
        // Show the video, hide the image
    }
}
