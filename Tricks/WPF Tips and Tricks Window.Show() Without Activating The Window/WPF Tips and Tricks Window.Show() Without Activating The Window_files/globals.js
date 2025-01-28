var g_AppPath, g_errorCount, g_errors;

function onRaiseError(error)
{
    alert(getFormattedError(error));
}

function onTimeout(error)
{
    alert(getFormattedError(error));
}

function getFormattedError(error)
{
    var errorFormat = 'An unhandled exception occurred\n\nType: {0}\nMessage: {1}\nStack Trace: {2}';
    
    return String.format(errorFormat, error.get_exceptionType(), error.get_message(), error.get_stackTrace());
}

function escapeToBr(textareaValue)
{  
    var replaceWith = '<br />'; 
    textareaValue = escape(textareaValue)
    
    for(q = 0; q < textareaValue.length; q++)
    { 
        if(textareaValue.indexOf("%0D%0A") > -1)
        { 
            textareaValue = textareaValue.replace("%0D%0A",replaceWith)
        }
        else if(textareaValue.indexOf("%0A") > -1)
        { 
            textareaValue = textareaValue.replace("%0A",replaceWith)
        }
        else if(textareaValue.indexOf("%0D") > -1)
        { 
            textareaValue = textareaValue.replace("%0D",replaceWith)
        }
    }
    return unescape(textareaValue); 
}