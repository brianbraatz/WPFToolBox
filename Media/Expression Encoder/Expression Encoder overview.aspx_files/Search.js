function setSearchBoxFormCode0(cmCode)
{
    document.getElementById("sbFormCode0").value = cmCode;
}

function checkForEnterKey()
{
    var retVal = true;
    var keycode = 0;
    if ((typeof(window.event) != "undefined") && (window.event != null))
    {
        keycode = window.event.keyCode;
    }
    else if ((typeof(e) != "undefined") && (e != null))
    {
        keycode = e.which;
    }
    if (keycode == 13)
    {
        doSearch();
        retVal = false;
    }
    return retVal;
}

function findKeywordTextbox()
{
    var ret = null;
    
    var inputs = document.getElementsByTagName("input");
    for (var i=0; i<inputs.length; i++)
    {
        if(inputs[i].className == "keywords")
        {
            ret = inputs[i];
            break;
        }
    }
    
    return ret;
}

function doSearch()
{
    var keywordsTextbox = findKeywordTextbox();
    if (keywordsTextbox != null)
    {
        var keywords = keywordsTextbox.value;
        if (keywords.length > 0)
        {
            var q = "q=" + escape(keywords);
            var l = "l=" + (document.getElementById("radLang10").checked ? "en" : "2");
            var form = "FORM=" + escape(document.getElementById("sbFormCode0").value);
            var mkt = "mkt=" + escape(document.getElementById("hdMarket0").value);

            // Restrict the search to just pages in this site
            var s100 = "s100=on";
            var otherSite = "OtherSite=http%3A%2F%2Fwww.microsoft.com%2Fproducts%2Fexpression";

            var url = "http://search.microsoft.com/results.aspx?" + q + "&" + l + "&" + form + "&" + mkt + "&" + s100 + "&" + otherSite;
            window.location.href = url;
        }
    }
}
