if(!window.Silverlight)
{
    window.Silverlight={};
}
Silverlight.InstallAndCreateSilverlight = function(version, installPromptDiv, createSilverlightDelegate)
{
    var RetryTimeout=3000;	              //The interval at which Silverlight instantiation is attempted(ms)
    if (Silverlight.isInstalled(version))
    {
        createSilverlightDelegate();
    }
    else
    {
        if ( installPromptDiv )
        {
            installPromptDiv.innerHTML = Silverlight.createObject(null, null, null, {version: version, inplaceInstallPrompt:true},{}, null);
        }
        TimeoutDelegate = function()
        {
            Silverlight.InstallAndCreateSilverlight(version, null, createSilverlightDelegate);
        }
        setTimeout(TimeoutDelegate, RetryTimeout);
    }
}
