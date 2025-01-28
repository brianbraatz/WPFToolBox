function SmartDate_Load()
{
    var templiterals = document.getElementsByTagName("abbr");
    var literals = new Array(templiterals.length);
    for (var i = 0; i < templiterals.length; i++)
    {
        literals[i] = templiterals[i].parentNode;        
    }    
    
    for (var i = 0; i < literals.length; i++)
    {
        var classNames = literals[i].className.split(' ');
        var isSmartDate = false;
        for (var index = 0; index < classNames.length; index++)
        {
            if (classNames[index] == "smartDate")
            {
                isSmartDate = true;
                break;
            }                    
        }
        
        if (isSmartDate)
        {
            var showFullDate = false;
            var showDateOnly = false;
            var showTimeOnly = false;
            var noShortDate = false;
            for (var index = 0; index < classNames.length; index++)
            {
                if (classNames[index] == "full")
                {
                    showFullDate = true;
                    break;
                }
                if (classNames[index] == "dateOnly")
                {
                    showDateOnly = true;
                    break;
                }
                if (classNames[index] == "timeOnly")
                {
                    showTimeOnly = true;
                    break;
                }
                if (classNames[index] == "dateOnlyNoShort")
                {
                    showDateOnly = true;
                    noShortDate = true;
                }
            }
         
            var timestamp = parseInt(literals[i].attributes['LocalTimeTicks'].value);
            if (timestamp != Number.NaN)
                literals[i].innerHTML = SmartDate_FormatTimestamp(timestamp, showFullDate, showDateOnly, showTimeOnly, noShortDate);
        }
    }
    var templiterals = null;
    var literals = null;
}

//
// Keep this method in sync with SmartDateTimeLiteral.FormatTimestamp!!
//
function SmartDate_FormatTimestamp(timestamp, showFullDate, showDateOnly, showTimeOnly, noShortDate)
{
    var date = new Date(timestamp * 1000);
    
    // We get more granular as we get further from the date
    var timeText = SmartDate_FormatTime(date, showFullDate);
    var dateText = SmartDate_FormatDate(date, showFullDate, noShortDate);
    
    var text = "";
    if (dateText.length > 0)
        text = dateText;
    
    if (showDateOnly)
        return text;
    if (showTimeOnly)
        text = "";
    
    if (timeText.length > 0)
    {
        if (text.length > 0)
            text += " at ";
        text += timeText;
    }
    
    return text;
}

function SmartDate_FormatTime(dateObject, showFullDate)
{
    var h = dateObject.getHours();
    var m = dateObject.getMinutes();
    var ampm = h < 12 ? "AM" : "PM";
    
    if (h == 0 && m == 0 && dateObject.getSeconds() == 0 && dateObject.getMilliseconds() == 0)
        return "";
  
    if (h > 12)
        h = h - 12;
    
    if (h == 0)
        h = 12;
    
    return h + ":" + SmartDate_ZeroPadNumber(dateObject.getMinutes(), 2) + " " + ampm;
}

function SmartDate_FormatDate(dateObject, showFullDate, noShortDate)
{
    var Y = dateObject.getFullYear();
    var M = dateObject.getMonth();
    var D = dateObject.getDate();
    var d = dateObject.getDay();
    
    var days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    if (showFullDate)
    {
        return days[d] + " " + months[M] + " " + D + " " + Y;
    }
    else
    {
        var now = new Date();
        
        // We get more granular as we get further from the date
        var dateText = "";
        if(!noShortDate)
        {
            if (now.getFullYear() == Y && now.getMonth() == M && now.getDate() == D)
                dateText = "Today";
            else if (now.getFullYear() == Y && now.getMonth() == M && ((now.getDate() > D && (now.getDate() - 6) < D) || (D > now.getDate() && (D - 6) < now.getDate())))
                dateText = days[d];
            else if (now.getFullYear() == Y)
                dateText = months[M] + " " + D;
            else
                dateText = months[M] + " " + D + " " + Y;
                
            return dateText;
        }
        
        dateText = months[M] + " " + D + " " + Y;
        return dateText;
    }
}

function SmartDate_ZeroPadNumber(num, places)
{
    var numString = "" + num + "";
    
    for (var i = 0; i < places - numString.length; i++)
    {
        numString = "0" + numString;
    }
    
    return numString;
}


function SmartDate_AjaxLoad()
{
    var page = $object("_PageRequestManager");
    page.propertyChanged.add(SmartDate_RequestManagerPropertyChanged);    
}

// handles the property changed event to listen for the ajax callback
// and re-run the date load method    
function SmartDate_RequestManagerPropertyChanged(sender, args)
{
    if(args.get_propertyName() == "inPostBack" && !sender.get_inPostBack())
    {
        SmartDate_Load();
    }
}