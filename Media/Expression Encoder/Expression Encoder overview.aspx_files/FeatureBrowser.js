function createFeatureBrowser()
{
 	var scene = new FeatureBrowser.Scene();
	Silverlight.createObjectEx({
		source: '../xaml/FeatureBrowser/Scene.xaml',
		parentElement: document.getElementById('feature-browser'),
		id: 'features-browser-plugin',
		properties: {
			width: '783',
			height: '465',
			background:'#FF000000',
            isWindowless: 'false',
			version: '0.8'
		},
		events: {
		    onError: null,
			onLoad: Silverlight.createDelegate(scene, scene.handleLoad)
		},		
		context: null 
	});
};

if (!window.Silverlight) 
	window.Silverlight = {};

Silverlight.createDelegate = function(instance, method) {
	return function() {
        return method.apply(instance, arguments);
    }
};

var browserData;

function bootstrap()
{
    try
    {
        Sys.Application.initialize();

        createNamespace( FeatureBrowser );
        
        // extract query params
        qsParams['key'] = "web";
        extractQSParams();

        // load XML
        
        var wRequest =  new Sys.Net.WebRequest();
        wRequest.set_url("overview.aspx?method=getproducts");
        wRequest.set_httpVerb("GET");
        wRequest.set_userContext("feature browser");
        wRequest.add_completed(onGetProducts);
        wRequest.invoke(); 
    }
    catch(e)
    {
        Degrade();
    }
};

function onGetProducts( executor, eventArgs )
{
    try
    {
        if (executor.get_responseAvailable())
        {
            var rootNode;

            if (document.all)
            {
                // IE
                var xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
                xmlDoc.async = false;
                xmlDoc.validateOnParse = false;
                xmlDoc.loadXML( executor.get_responseData() );
                
                rootNode = xmlDoc.childNodes[2];
            }
            else
            {
                 var parser = new DOMParser();
                 var doc = parser.parseFromString(executor.get_responseData(),"text/xml");
                 rootNode = doc.documentElement;
            }

            browserData = FeatureBrowser.BrowserData.getInstance();
            browserData.parseXml( rootNode, onBrowserDataReady);      
        }
    }
    catch(e)
    {
        Degrade();
    }
}

function onBrowserDataReady( success )
{
    try
    {
        document.getElementById('feature-browser').style.display = "block";
        Silverlight.InstallAndCreateSilverlight("0.8", document.getElementById("feature-browser-prompt"), createFeatureBrowser);
    }
    catch(e)
    {
        Degrade();
    }
}

function Degrade()
{
    // do nothing for now
}
