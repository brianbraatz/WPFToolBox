



function jsTrim(s) {return s.replace(/(^\s+)|(\s+$)/g, "");}

function Track(trackParm, objLink)
{
    if (objLink.innerText && jsTrim(objLink.innerText))
    {
        // anchor tag, use link text
        LinkText = objLink.innerText;
    }
    else if (objLink.all && objLink.all(0).alt)
    {
        // image, use alt text
        LinkText = objLink.all(0).alt;
    }
         
    var strDomain = document.domain;
    var s = trackParm.split("|");
    var strLinkArea;
    var strLinkId;
    var strPageRegion;
    
    if (trackParm.substring(0,1) == "|")
    {
        strPageRegion = s[1];
        strLinkArea = s[2];
        strLinkId = s[2] + s[3];
    }
    else
    {
            strLinkArea = s[0];
        strLinkId = s[0] + s[1];

    }
    
    if (typeof(LinkText)=="undefined" || !LinkText || LinkText == "")
        LinkText = strLinkId;

    ctUrl = objLink.href + "?LinkId=" + strLinkId + "&LinkArea=" + strLinkArea 

    if (typeof(DCSext)!="undefined") 
    {
            DCSext.wt_strHeadlnLocale = detectedLocale;
            DCSext.wt_strCat=strLinkArea+"|"+detectedLocale;
            DCSext.wt_strUrl = window.location.href.toLowerCase();
            DCSext.wt_strArea = strPageRegion;
    }
        return false; 
}



function CopyCode(elemName)
{
    var obj = document.getElementById(elemName)
    window.clipboardData.setData("Text", obj.innerText);
}

var cleanedDivIds, cleanedImgIds, oExpColSpan, oExpColImg;
var expState = true;

function checkExpCollAll()
{
   
   cleanImgVars();
   
   var open = false;
   var closed = false;
   
   for (i = 0; i < cleanedDivIds.length-1; i++)
   {
		// alert(cleanedDivIds[i]);
        try
        {
            oDiv = document.getElementById(cleanedDivIds[i]);
            oImg = document.getElementById(cleanedImgIds[i]);
            
            if (oDiv.style.display == 'block')
            {
                open = true;
            }
            else
            {
                closed = true;
            }
        }
        catch(e)
	    {
			throw e;
	    }
   }
   if (open != closed)
	{	        
	    if (open)
	        expState = false;
	    if (closed)
	        expState = true;
	        
	    if (typeof(expcalallPres)!='undefined' && expcalallPres == true)
	        expCollButtonToggle();
	}
}

function cleanImgVars()
{
   if (typeof(cleanedDivIds)=='undefined')
   {
        var r1, r2, re;
        re = /undefined/g; 
        r1 = ExpCollDivStr.replace(re, "");
        cleanedDivIds = r1.substring(0, r1.length).split(',');
        
        r2 = ExpCollImgStr.replace(re, "");
        cleanedImgIds = r2.substring(0, r2.length).split(',');
   }
}

//	#5406
function ShowHideCollapsibleArea(strAreaId,strImg)
{
	var oCollAreaDiv = document.getElementById(strAreaId);
	var oCollAreaImg = document.getElementById(strImg);	
	if (oCollAreaDiv.style.display == 'none')
	{
		oCollAreaDiv.style.display = 'block';
		oCollAreaImg.src = imgMinus;
	}
	else
	{		
		oCollAreaDiv.style.display = 'none';
		oCollAreaImg.src = imgPlus;	
	}

	checkExpCollAll();
}

