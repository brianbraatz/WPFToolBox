if (!window.FeatureBrowser)
	window.FeatureBrowser = {};
	
function Class() { };
Class.prototype.construct = function() {};
Class.extend = function(def) {
    var classDef = function() {
        if (arguments[0] !== Class) { this.construct.apply(this, arguments); }
    };
    
    var proto = new this(Class);
    var superClass = this.prototype;
    
    for (var n in def) {
        var item = def[n];                        
        if (item instanceof Function) item.$ = superClass;
        proto[n] = item;
    }

    classDef.prototype = proto;
    
    //Give this new class the same static extend method    
    classDef.extend = this.extend;        
    return classDef;
};

function createNamespace( str )
{
    var elements = str.toString().split(".");
    
    if ( !elements ) return;
    
    var pointer = this;
    for ( var name in elements )
    {
        if ( pointer[elements[name]] == null )
            pointer[elements[name]] = {};

        pointer = pointer[elements[name]];
    }
};

function parseAttribute( xmlNode, att )
{
    if ( !window.ActiveXObject )
    {
        return xmlNode.hasAttribute( att ) ? xmlNode.getAttribute( att ) : null;
    }
    else
    {
        return xmlNode.getAttribute( att );
    }  
};

function trim(s)
{
	return rtrim(ltrim(s));
};

function ltrim(s)
{
	var l=0;
	while(l < s.length && s[l] == ' ')
	{	l++; }
	return s.substring(l, s.length);
};

function rtrim(s)
{
	var r=s.length -1;
	while(r > 0 && s[r] == ' ')
	{	r-=1;	}
	return s.substring(0, r+1);
};

var qsParams = new Array();

function extractQSParams() 
{
    var query = window.location.search.substring(1);
    var params = query.split('&');
    for (var i = 0; i < params.length; i++) 
    {
        var pos = params[i].indexOf('=');
        if (pos > 0) 
        {
            var key = params[i].substring(0,pos);
            var val = params[i].substring(pos+1);
            qsParams[key] = val;
        }
    }
}
FeatureBrowser.AGFrameworkElement = Class.extend(
{
    construct: function( parent )
    {
        this.$parent = parent;
    },
    
    init: function( xaml, left, top )
    {
        this.$root = this.createFromXaml( xaml );
        this.$parent.children.add( this.$root );

        this.$root["Canvas.Left"] = left;
        this.$root["Canvas.Top"] = top;
        
        //console.log( this.toString() + " super.init("+[left,top]+"): this.$root=" + this.$root);
        
        this.loadDelegate = this.$root.addEventListener("Loaded", Silverlight.createDelegate( this, this.handleLoad));
    },
    
    cacheElementRef: function( elementName, refName )
    {
        var ref = "$" + ( refName == null ? elementName : refName );
        this[ref] = this.find( elementName );
    },
    
    handleLoad: function( sender ) 
    {
        //console.log ( this.toString() + " handleLoad, this.$root=" + this.$root );

        this.$root.removeEventListener( "Loaded", this.loadDelegate );
        delete this.loadDelegate;
    },
    
    find: function( name ) 
    {
        return this.$root.findName(name);
    },
    
    createFromXaml: function( xaml )
    {
        return this.$parent.getHost().content.createFromXaml(xaml, true);
    },
    
    setTop: function( topPx )
    {
        this.$root["Canvas.Top"] = topPx;
    },
    
    setLeft: function( leftPx )
    {
        this.$root["Canvas.Left"] = leftPx;
    }
});
FeatureBrowser.BrowserData = function()
{
    FeatureBrowser.isIE = document.all;
};


FeatureBrowser.BrowserData.prototype = 
{
    toString: function()
    {
        return "BrowserData[]";
    },
    
    loadXml: function( url, loadedCallback )
    {
        this._loadedCallback = loadedCallback;
        
    	var wRequest = new Sys.Net.WebRequest();
        wRequest.set_url( url );
        wRequest.set_httpVerb("GET");
        wRequest.add_completed( Silverlight.createDelegate( this, this.handleXmlLoaded ) );
        wRequest.invoke(); 
    },

    parseXml: function( rootNode, loadedCallback )
    {
        this._loadedCallback = loadedCallback;
    
        // find correct product
        var productNode = null;
        var productNodes = rootNode.getElementsByTagName("product");
        var productKey = qsParams['key'] != null ? qsParams['key'] : "blend";
        
        for ( var i = 0; i < productNodes.length; i++ )
        {
            if ( productNodes[i].nodeName == "product" && parseAttribute( productNodes[i], "key" ) == productKey )
            {
                productNode = productNodes[i];
                break;
            }       
        } 
        
        var featureNodes = productNode.getElementsByTagName( "feature" );
        
        this.topics = this.parseFeatures( featureNodes );
        
        this._loadedCallback( true );
    },
    
    parseFeatures: function( featureNodes ) /*Array*/
    {
        var topics = [];
    
        for ( var i = 0; i < featureNodes.length; i++ )
        {
            topics.push( this.parseFeature( featureNodes[i] ));
        }
    
        return topics;
    },
    
    parseFeature: function( featureNode ) /*Topic*/
    {
        var title = parseAttribute( featureNode, "title" );
        var id = title;
        var desc = this.parseDescription( featureNode.getElementsByTagName( "description" )[0].childNodes[0].nodeValue );
            
        var topic = new FeatureBrowser.Topic( id, title, desc );

        var videoSrc = parseAttribute( featureNode.getElementsByTagName( "video" )[0], "url" );
        var videoThumb = parseAttribute( featureNode, "image" ).replace( "~", ".." );
        
        topic.addMediaElement( new FeatureBrowser.MediaElement( "tour", "video", videoSrc, videoThumb ) );
 
        var detailNodes = featureNode.getElementsByTagName( "detail" );        
        if ( detailNodes.length != null )
            topic.topics = this.parseDetails( detailNodes );
            
        return topic;
    },

    parseDetails: function( detailNodes ) /*Topic*/
    {
        var topics = [];
    
        for ( var i = 0; i < detailNodes.length; i++ )
        {
            topics.push( this.parseDetail( detailNodes[i] ));
        }
    
        return topics;
    },

    parseDetail: function( detailNode ) /*Topic*/
    {
        var title = parseAttribute( detailNode, "title" );
        var id = title;
        var desc = this.parseDescription( detailNode.getElementsByTagName( "description" )[0].childNodes[0].nodeValue );
    
        var topic = new FeatureBrowser.Topic( id, title, desc );

        var imgSrc = parseAttribute( detailNode, "image" ).replace( "~", ".." );
        
        topic.addMediaElement( new FeatureBrowser.MediaElement( "screenshot", "image", imgSrc, null ) );
            
        return topic;
    },
    
    parseDescription: function( descStr )
    {
        var cleanStr;
        var searchstr = new RegExp( "\\n+", "g" );
        cleanStr = descStr.replace( searchstr, " " );
    
        searchstr = new RegExp( "\\s+", "g" );
        cleanStr = cleanStr.replace( searchstr, " " );
        
        return trim( cleanStr );
    },
    
    // Event Handlers
    
    handleXmlLoaded: function( executor, eventArgs )
    {
        if (executor.get_responseAvailable()) 
        {   
            var rootNode = null;
            var xmlObj = executor.get_xml();

            if (FeatureBrowser.isIE)
            {
                var xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
                xmlDoc.async="false";
                xmlDoc.validateOnParse = false;
                xmlDoc.loadXML( xmlObj.xml );
                
                rootNode = xmlDoc.childNodes[2];
            }
            else
            {
                rootNode = xmlObj.documentElement;
            }

            this.parseXml( rootNode );
        }
        else
        {
            if (executor.get_timedOut())
                alert("Timed Out");
            else
                if (executor.get_aborted())
                    alert("Aborted");
        }
    }
};

// Static Methods

FeatureBrowser.BrowserData.getInstance = function()
{
    var fb = FeatureBrowser.BrowserData;
    if ( fb.__instance == undefined ) fb.__instance = new fb();
    return fb.__instance;
};

FeatureBrowser.uniqueId = 0;

FeatureBrowser.FeaturePage = FeatureBrowser.AGFrameworkElement.extend(
{
    construct: function( data, controller, parent, left, top ) 
    {
        arguments.callee.$.construct.call(this, parent);
        
        this.$controller = controller;
        this.$data = data;
     
        this.init( left, top );
    },

    toString: function()
    {
        return "FeaturePage["+this.$data.title+"]";
    },

    init: function ( left, top )
    {
        var xaml = FeatureBrowser.assetDownloader.getResponseText( "FeaturePage.xaml" );    
        arguments.callee.$.init.call( this, xaml, left, top );
         
        this.cacheElementRef( "contentContainer" );
        this.cacheElementRef( "descTB" );
        this.cacheElementRef( "contentCanvas" );
        this.cacheElementRef( "loadbarCanvas" );
        this.cacheElementRef( "ssLoadbar" );
            
        this.$descTB.setFontSource( FeatureBrowser.fontDownloader );
        this.$descTB.fontFamily = "Segoe UI";
        this.$descTB.text = this.$data.description;
    },

    getWidth: function()
    {
        return this.$contentContainer.width;
    },
    
    stopVideo: function()
    {
        if ( this.$currContent == null ) return;
        
        if ( this.$player != null )
            this.$player.stop();
    },

    playVideo: function()
    {
        if ( this.$currContent == null ) return;
    },
    
    preloadVideo: function()
    {
        if ( this.$currContent == null ) return;
    },
    
    // Event Handlers

    handleLoad: function( sender )
    {
        this.$topicSelector = new FeatureBrowser.TopicSelector( this.$data.topics, this, this.$contentContainer, 8, 100 );        
        this.$topicSelector.setSelectedByIndex( 0 );
        

        arguments.callee.$.handleLoad.call( this, sender );
    },
    
    onTopicSelected: function( topic )
    {
        var isVideo = topic.isVideo == true;
    
        var nextName = "content" + parseInt(FeatureBrowser.uniqueId++).toString();
        
        var sb;
        
        if ( isVideo )
        {
            this.$player = new FeatureBrowser.Player( this.$data.getMediaElement( "tour" ), this, this.$contentCanvas, 0, 0 );
            this.$nextContent = this.$player.$root;
        }
        else
        {
            this.$player = null;
         
            var sc = new FeatureBrowser.StaticContentItem( topic, this, this.$contentCanvas, 0, 0 );
            this.$nextContent = sc.$root;

            var downloader = FeatureBrowser.Scene.plugIn.createObject("downloader");
            downloader.addEventListener("downloadProgressChanged", Silverlight.createDelegate( this, this.onImageDownloadProgressChanged ));
            downloader.addEventListener("completed", Silverlight.createDelegate( this, this.onImageDownloadCompleted ));
            downloader.open("GET", topic.getMediaElement("screenshot").src);
            downloader.send();
                        
            this.find( "ShowLoadbar" ).begin();
        }
        
        this.$contentCanvas.children.insert( 0, this.$nextContent );
        
        if ( this.$currContent != null )
        {
            sb = this.$currContent.findName( "FadeOut" );
            sb.addEventListener( "completed", Silverlight.createDelegate( this, this.onStaticContentFadeOut ));
            sb.begin();
        }
            
        this.$currContent = this.$nextContent;
    },
    
    onImageDownloadProgressChanged: function( sender, eventArgs )
    {
        this.$ssLoadbar.width = 246 * sender.downloadProgress;
    },
    
    onImageDownloadCompleted: function( sender, eventArgs )
    {
        this.find( "HideLoadbar" ).begin();
        
        this.$ssLoadbar.width = 0;
        
        this.$nextContent.findName( "img" ).setSource( sender, "" );
        this.$nextContent.findName( "FadeIn" ).begin();
    },
    
    onStaticContentFadeOut: function( eventObj )
    {
        this.$contentCanvas.children.removeAt( 1 );
    }
});
FeatureBrowser.Topic = function( id /*string*/, title /*string*/, description /*string*/)
{
    this.id = id;
    this.title = title;
    this.description = description;
    this.mediaElements = {};
    this.topics = [];
};

FeatureBrowser.Topic.prototype = 
{
    toString: function()
    {
        return "Topic["+[this.id,this.title]+"]";
    },

    addMediaElement: function( mediaElement /*MediaElement*/ )
    {
        this.mediaElements[mediaElement.id] = mediaElement;
    },
    
    getMediaElement: function( id ) /*:MediaElement*/
    {
        return this.mediaElements[id]; 
    }
};

FeatureBrowser.MediaElement = function( id, type, src, thumb )
{
    this.id = id;
    this.type = type;
    this.src = src;
    this.thumb = thumb;
};
FeatureBrowser.NavBar = FeatureBrowser.AGFrameworkElement.extend(
{
    construct: function( data, controller, parent, left, top ) 
    {
        arguments.callee.$.construct.call( this, parent );
        
        this.$controller = controller;
        this.$data = data;
        
        this.init( left, top );
    },

    toString: function()
    {
        return "NavBar[]";
    },
    
    init: function( left, top )
    {
        var xaml = FeatureBrowser.assetDownloader.getResponseText( "NavBar.xaml" );    
        arguments.callee.$.init.call( this, xaml, left, top );
        
        var topicData;
            
        this._items = [];
        
        for ( var i = 0; i < this.$data.length; i++ )
        {
            topicData = this.$data[i];
            var navItem = new FeatureBrowser.NavItem( topicData, this, this.$root, 0, 0 );
            navItem.name = "navItem" + i;
            this._items.push( navItem );
        }
        
        this.$root.addEventListener( "MouseLeave", Silverlight.createDelegate( this, this.handleMouseLeave ));
    },

    arrangeItems: function()
    {
        var item;
        var left = 0;
        var padding = 3;

        for ( var i = 0; i < this._items.length; i++ )
        {
            item = this._items[i];      
            item.$root["Canvas.Left"] = left; 
            
            left += item.getWidth() + padding;
        }
        
    },
    
    selectItem: function( itemToSelect )
    {
        var item;
        var pageToSelect;
        
        for ( var i = 0; i < this._items.length; i++ )
        {
            item = this._items[i];
            item.setSelected ( item == itemToSelect );
            if ( item == itemToSelect ) 
                pageToSelect = i;
        }
        
        this.$controller.showPage( pageToSelect );
        
        //this.arrangeItems();
    },
    
    setSelectedById: function( id )
    {
        for ( var i = 0; i < this._items.length; i++ )
        {
            if ( this._items[i].$data.id == id )
            {
                this.selectItem( this._items[i] );
                break;
            }
        }
    },
    
    setSelectedByIndex: function( index )
    {
        if ( index >= 0 && index <= this._items.length )
        {
            this.selectItem( this._items[index] );
        }
    },

    // Event Handlers

    handleLoad: function( sender )
    {
        arguments.callee.$.handleLoad.call( this, sender );
    },

    onNavItemLoaded: function( item )
    {
        this.arrangeItems();
    },
    
    onNavItemClicked: function( itemToSelect )
    {
        this.selectItem( itemToSelect );
    },
    
    onNavItemRollover: function( activeItem )
    {
        var item;
        for ( var i = 0; i < this._items.length; i++ )
        {
            item = this._items[i];
            item.setHighlighted ( item == activeItem ) 
        }
    },
    
    onNavItemRollout: function( inactivePane )
    {
        var item;
        for ( var i = 0; i < this._items.length; i++ )
        {
            item = this._items[i];
            item.setHighlighted ( false ) 
        }
    },
    
    handleMouseLeave: function( sender, e )
    {
    }
});
FeatureBrowser.NavItem = FeatureBrowser.AGFrameworkElement.extend(
{
    construct: function( data, controller, parent, left, top ) 
    {
        arguments.callee.$.construct.call( this, parent );
        
        this.$controller = controller;
        this.$data = data;
     
        this.selected = false;
        this.highlighted = false;
        
        this.init( left, top );
    },
    
    toString: function()
    {
        return "NavItem["+this.$data.title+"]";
    },
    
    init: function ( left, top )
    {
        var xaml = FeatureBrowser.assetDownloader.getResponseText( "NavItem.xaml" );
    
        arguments.callee.$.init.call( this, xaml, left, top );
        
        this.cacheElementRef( "bg" );
        this.cacheElementRef( "titleTb" );
        
        this.$root.cursor = "Hand";
        this.$root.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onMouseEnter));
        this.$root.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onMouseLeave));
        this.$root.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onPress));

        this.$titleTb.setFontSource( FeatureBrowser.fontDownloader );
        this.$titleTb.fontFamily = "Segoe UI";
        
        this.$titleTb.text = this.$data.title;
    },
    
    handleLoad: function( sender )
    {
        arguments.callee.$.handleLoad.call( this, sender );
    },
    
    setSelected: function( flag )
    {
        this.selected = flag;
        
        if ( flag ) this.select();
        else this.deselect();
    },
    
    setHighlighted: function( flag )
    {
        if ( this.selected ) return;

        this.highlighted = flag;
        
        if ( flag ) this.highlight();
        else this.unhighlight();
    },
    
    select: function() /*:void*/
    {
        this.unhighlight();
        
        this.find( "SelectAnim" ).begin();
    },
    
    deselect: function() /*:void*/
    {
        this.find( "DeselectAnim" ).begin();
    },
    
    highlight: function()  /*:void*/
    {
        this.find( "HighlightAnim" ).begin();
    },
    
    unhighlight: function()  /*:void*/
    {
        this.find( "UnhighlightAnim" ).begin();
    },
    
    getWidth: function()  /*:Number*/
    {
        return this.$root.width;
    },

    // Event Handlers

    handleLoad: function( sender )
    {
        arguments.callee.$.handleLoad.call( this, sender );

        this.$controller.onNavItemLoaded( this );
    },
    
    onMouseEnter: function( sender, args )
    {
        if ( !this.selected )
        {
            this.$controller.onNavItemRollover( this );
        }
    },
    
    onMouseLeave: function( sender, args )
    {
        if ( !this.selected )
        {
            this.$controller.onNavItemRollout( this );
        }
    },
    
    onPress: function( sender, args )
    {
        if ( !this.selected )
        {
            this.$controller.onNavItemClicked( this );
        }
    }
});FeatureBrowser.Player = FeatureBrowser.AGFrameworkElement.extend(
{
    construct: function( data, controller, parent, left, top ) 
    {
        arguments.callee.$.construct.call( this, parent );
        this.$controller = controller;
        this.$data = data;
     
        this.init( left, top );
    },
    
    toString: function()
    {
        return "Player[]";
    },
    
    init: function ( left, top )
    {
        var xaml = FeatureBrowser.assetDownloader.getResponseText( "Player.xaml" );    
        this.$root = this.createFromXaml( xaml );

        this.$root["Canvas.Left"] = left;
        this.$root["Canvas.Top"] = top;
        
        //console.log( this.toString() + " super.init("+[left,top]+"): this.$root=" + this.$root);
        
        this.loadDelegate = this.$root.addEventListener("Loaded", Silverlight.createDelegate( this, this.handleLoad));
        
        this.cacheElementRef( "timer" );
        this.cacheElementRef( "VideoWindow", "videoWindow" );
        this.cacheElementRef( "PlaceholderImage", "placeholderImage" );
        this.cacheElementRef( "Controls", "controls" );
        this.cacheElementRef( "ClickToPlay", "clickToPlay" );
        this.cacheElementRef( "ClickToPlayIcon", "clickToPlayIcon" );
        this.cacheElementRef( "PlayPauseButton", "playPauseButton" );
        this.cacheElementRef( "PlayIcon", "playIcon" );
        this.cacheElementRef( "PauseIcon", "pauseIcon" );;
        this.cacheElementRef( "PrevButton", "prevButton" );
        this.cacheElementRef( "NextButton", "nextButton" );
        this.cacheElementRef( "StopButton", "stopButton" );
        this.cacheElementRef( "VolumeDownButton", "volumeDownButton" );
        this.cacheElementRef( "VolumeUpButton", "volumeUpButton" );
        this.cacheElementRef( "VolumeIndicator", "volumeIndicator" );
        this.cacheElementRef( "VolumeBars", "volumeBars" );
        this.cacheElementRef( "VolumeBar1", "volumeBar1" );
        this.cacheElementRef( "VolumeBar2", "volumeBar2" );
        this.cacheElementRef( "VolumeBar3", "volumeBar3" );
        this.cacheElementRef( "VolumeSliderControl", "volumeSliderControl" );
        this.cacheElementRef( "VolumeSliderBar", "volumeSliderBar" );
        this.cacheElementRef( "VolumeSlider", "volumeSlider" );
        this.cacheElementRef( "ProgressBarControl", "progressBarControl" );
        this.cacheElementRef( "ProgressBarBG", "progressBarBG" );
        this.cacheElementRef( "DownloadProgressBar", "downloadProgressBar" );
        this.cacheElementRef( "ProgressBarCurrent", "progressBarCurrent" );
        this.cacheElementRef( "ProgressSlider", "progressSlider" );
        this.cacheElementRef( "CurrentTimeTB", "currentTimeTB" );
        this.cacheElementRef( "TotalTimeTB", "totalTimeTB" );
        this.cacheElementRef( "BufferingText", "bufferingText" );
        this.cacheElementRef( "BufferingOverlay", "bufferingOverlay" );

        this.$timer.addEventListener( "completed", Silverlight.createDelegate( this, this.onTimerCompleted ));

        this.$placeholderImage.source = this.$data.thumb;

        this.$mediaOpenedDelegate = Silverlight.createDelegate( this, this.onMediaOpened );
        this.$downloadProgressChangedDelegate = Silverlight.createDelegate( this, this.onDownloadProgressChanged );
        this.$currentStateChangedDelegate = Silverlight.createDelegate( this, this.onMediaCurrentStateChanged );
        this.$mediaEndedDelegate = Silverlight.createDelegate( this, this.onMediaEnded );

        this.$videoWindow.addEventListener("mediaOpened", this.$mediaOpenedDelegate);
        this.$videoWindow.addEventListener("downloadProgressChanged", this.$downloadProgressChangedDelegate);
        this.$videoWindow.addEventListener("currentStateChanged", this.$currentStateChangedDelegate);
        this.$videoWindow.addEventListener("mediaEnded", this.$mediaEndedDelegate);

        this.$clickToPlay.cursor = "Hand";    
        this.$clickToPlay.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onClickToPlayEnter ));
        this.$clickToPlay.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onClickToPlayLeave ));
        this.$clickToPlay.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onClickToPlayDown ));
    },
    
    initListeners: function()
    {
        this.$playPauseButton.cursor = "Hand";
        this.$playPauseButton.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onPlayPauseEnter));
        this.$playPauseButton.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onPlayPauseLeave));
        this.$playPauseButton.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onPlayPauseDown));

        this.$prevButton.cursor = "Hand";
        this.$prevButton.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onPrevButtonEnter));
        this.$prevButton.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onPrevButtonLeave));
        this.$prevButton.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onPrevButtonDown));

        this.$nextButton.cursor = "Hand";
        this.$nextButton.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onNextButtonEnter));
        this.$nextButton.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onNextButtonLeave));
        this.$nextButton.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onNextButtonDown));

        this.$stopButton.cursor = "Hand";
        this.$stopButton.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onStopButtonEnter));
        this.$stopButton.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onStopButtonLeave));
        this.$stopButton.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onStopButtonDown));

        this.$volumeDownButton.cursor = "Hand";
        this.$volumeDownButton.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onVolumeDownButtonEnter));
        this.$volumeDownButton.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onVolumeDownButtonLeave));
        this.$volumeDownButton.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onVolumeDownButtonDown));

        this.$volumeUpButton.cursor = "Hand";
        this.$volumeUpButton.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onVolumeUpButtonEnter));
        this.$volumeUpButton.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onVolumeUpButtonLeave));
        this.$volumeUpButton.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onVolumeUpButtonDown));

        this.$volumeIndicator.cursor = "Hand";
        this.$volumeIndicator.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onVolumeIndicatorEnter));
        this.$volumeIndicator.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onVolumeIndicatorLeave));
        this.$volumeIndicator.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onVolumeIndicatorDown));

        this.$volumeSlider.cursor = "Hand";
        this.$volumeSlider.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onVolumeSliderDown));
        this.$volumeSlider.addEventListener("MouseLeftButtonUp", Silverlight.createDelegate( this, this.onVolumeSliderUp));
        this.$volumeSlider.addEventListener("MouseMove", Silverlight.createDelegate( this, this.onVolumeSliderMove));

        this.$progressSlider.cursor = "Hand";
        this.$progressSlider.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onProgressSliderDown));
        this.$progressSlider.addEventListener("MouseLeftButtonUp", Silverlight.createDelegate( this, this.onProgressSliderUp));
        this.$progressSlider.addEventListener("MouseMove", Silverlight.createDelegate( this, this.onProgressSliderMove));
    },
    
    fadeIn: function()
    {
        this.find( "FadeIn" ).begin();
    },
    
    fadeOut: function()
    {
        this.find( "FadeOut" ).begin();
    },
    
    play: function()
    {
        if ( this.$videoWindow.Source == "" )
            this.$videoWindow.Source = this.$data.src;

        this.$clickToPlay.visibility = "Collapsed";
        this.$controls.isHitTestVisible = "true";
        this.$controls.opacity = "1.0";
        
        this.updateVolumeDisplay();
        
        this.$videoWindow.play();
        
        this.$timer.begin();
    },
    
    stop: function()
    {
        this.$videoWindow.stop();
        //this.$videoWindow.Source = "";
        this.$timer.stop();
        
        //this.$videoWindow.removeEventListener("mediaOpened", this.$mediaOpenedDelegate);
        //this.$videoWindow.removeEventListener("downloadProgressChanged", this.$downloadProgressChangedDelegate);
        //this.$videoWindow.removeEventListener("currentStateChanged", this.$currentStateChangedDelegate);
        //this.$videoWindow.removeEventListener("mediaEnded", this.$mediaEndedDelegate);
        
        this.$placeholderImage.visibility = "Visible";
        this.$clickToPlay.visibility = "Visible";
        this.$controls.isHitTestVisible = "false";
        this.$controls.opacity = "0.3";

        this.$bufferingOverlay.visibility = "Collapsed";
        this.find("BufferingAnim").stop();

        this.$progressBarCurrent.width = 0;
        this.$progressSlider["canvas.left"] = 0;
        this.$currentTimeTB.text = "00:00";
    },
    
    preload: function()
    {
    },
    
    setMediaPosition: function( seconds )
    {
      var position = this.$videoWindow.position;
      
      var loadedSeconds = ( this.$downloadProgressBar.width / this.$progressBarControl.width ) * this.$endPosition;
      
      if ( seconds > loadedSeconds ) 
        seconds = loadedSeconds;
      else if ( seconds < 0 )
        seconds = 0;
            
      position.seconds = seconds;
	    this.$videoWindow.position = position;
      
      this.updateProgressBar(); 
    },
    
    updateProgressBar: function()
    {
        if ( this.$videoWindow.source != "" && this.$videoWindow.position != null )
    	{
    	    var tX = ( this.$videoWindow.position.seconds / this.$videoWindow.naturalDuration.seconds ) * this.$progressBarControl.width;
    	    
    	    this.setSliderPosition( this.$progressSlider, this.$progressBarControl, tX );
    	    this.$progressBarCurrent.width = tX;
    	}          
    },
    
    // Volume Controls
    
    increaseVolume: function()
    {
        if ( this.$videoWindow.source == "" ) return;
        var slider = this.$volumeSlider;        
        this.setSliderPosition( slider, this.$volumeSliderControl, this.getSliderPosition( slider ) + 20 );
        this.setVolume();
    },
    
    decreaseVolume: function()
    {
        if ( this.$videoWindow.source == "" ) return;
        
        var slider = this.$volumeSlider;        
        this.setSliderPosition( slider, this.$volumeSliderControl, this.getSliderPosition( slider ) - 10 );
        this.setVolume();
    },
    
        
    setVolume: function()
    {
        if ( this.$videoWindow.source != "" )
    	{
    	    this.$videoWindow.volume = this.getSliderPosition( this.$volumeSlider ) / this.$volumeSliderControl.width;
    	}    
    },
    
    updateVolumeDisplay: function()
    {
        if ( this.$videoWindow.source != "" )
    	{
    	    this.setSliderPosition( 
    	        this.$volumeSlider, 
    	        this.$volumeSliderControl, 
    	        this.$videoWindow.volume * this.$volumeSliderControl.width );
    	        
    	    this.updateVolumeBars();
    	}     
    },
    
    updateVolumeBars: function()
    {
        this.$volumeBar1.visibility = this.$videoWindow.volume >  0.0  ? "Visible" : "Collapsed";
        this.$volumeBar2.visibility = this.$videoWindow.volume >= 0.4  ? "Visible" : "Collapsed";
        this.$volumeBar3.visibility = this.$videoWindow.volume >= 0.75 ? "Visible" : "Collapsed";
    },    
    
    // Slider helpers
    
    setSliderPosition: function( slider, sliderBounds, x )
    {
        var w = slider.width;
        var tX;
        var pos;
        
        if ( x < w/2 )
            tX = w/2;
        else if ( x > sliderBounds.width - w/2 )
            tX = sliderBounds.width - w/2;
        else
            tX = x;

        pos = tX - w/2;

   	    slider["canvas.left"] = pos;
   	    
   	    return pos;
    },     

    getSliderPosition: function( slider )
    {
        return slider["canvas.left"];
    },
    
    // Loaded Handler
    
    handleLoad: function( sender )
    {
        this.fadeIn();

        arguments.callee.$.handleLoad.call( this, sender );
    },
    
    // Media Handlers
    
    onDownloadProgressChanged: function (sender, args) 
    {
      this.$downloadProgressBar.width = this.$videoWindow.downloadProgress * this.$progressBarBG.width;
      
      //if ( this.$videoWindow.downloadProgress == 1 )
      //    this.$videoWindow.removeEventListener("downloadProgressChanged", this.$downloadProgressChangedDelegate);
    },

    onMediaOpened: function(sender, args) 
    {
      this.$placeholderImage.visibility = "Collapsed";
    
      this.$endPosition = this.$videoWindow.naturalDuration.seconds;
      this.$totalTimeTB.text = this.formatTime(this.$videoWindow.naturalDuration.seconds);
      this.updateVolumeDisplay();
      
      this.initListeners();
    },

    onMediaCurrentStateChanged: function(sender, args) 
    {
      if (sender.currentState == "Playing") 
      {
        // Hide progress
        this.$bufferingOverlay.visibility = "Collapsed";
        this.find("BufferingAnim").stop();

        this.$pauseIcon.visibility = "Visible";
        this.$playIcon.visibility = "Collapsed";
        this.$pauseIcon.opacity = 1;
        this.$playIcon.opacity = 1;
      }
      else if (sender.currentState == "Buffering") 
      {
        // Show progress
        if ( this.$bufferingOverlay.visibility == "Collapsed" ) 
        {
          this.find("BufferingAnim").begin();
          this.$bufferingOverlay.visibility = "Visible";
        }
      }
      else if (sender.currentState == "Paused" || sender.currentState == "Stopped") 
      {
        this.$pauseIcon.visibility = "Collapsed";
        this.$playIcon.visibility = "Visible";
        this.$pauseIcon.opacity = 1;
        this.$playIcon.opacity = 1;
        this.$timer.stop();
      }
      else if ((sender.currentState != "Opening") && (sender.currentState != "Closing") && (sender.currentState != "Closed"))
      {
        //sender.alert('Error: Media State Unknown:' + media.currentState);
        this.$pauseIcon.visibility = "Collapsed";
        this.$playIcon.visibility = "Visible";
        this.$pauseIcon.opacity = .4;
        this.$playIcon.opacity = .4;
      }
    },

    onMediaEnded: function(sender, args) 
    {
      this.$timer.stop();
    },
    
    onTimerCompleted: function( sender, e )
    {
      var vw = this.$videoWindow;
    
      if (vw.currentState == "Playing") 
      {
        this.updateProgressBar();
        
        for (var i = 0; i < 2; i++) 
        {
          if (this.$endPosition) 
          {
            try 
            {
              this.$currentTimeTB.text = this.formatTime( vw.position.seconds );
            } 
            catch (exception) 
            {
              this.$currentTimeTB.text = "00:00";
            }
          }
        }                
      } 
      else if ( vw.currentState != "Paused" ) 
      {
        this.$currentTimeTB.text = "00:00";
      }
      
      this.$timer.begin();
    },
   
    // Button Handlers
    
    // Click to Play
    onClickToPlayEnter: function( sender, e )
    {
        var sb = SplineAnimator.animate( this.$root, "ClickToPlayIcon", "0:0:0.250", { "(Opacity)" : .9 }, Splines.Linear );
        sb.begin();
    },

    onClickToPlayLeave: function( sender, e )
    {
        var sb = SplineAnimator.animate( this.$root, "ClickToPlayIcon", "0:0:0.250", { "(Opacity)" : .7 }, Splines.Linear );
        sb.begin();
    },

    onClickToPlayDown: function( sender, e )
    {
        this.play();
    },
    
    // Play/Pause
    onPlayPauseEnter: function( sender, e )
    {
        this.find( "PlayPauseEnter" ).begin();
    },

    onPlayPauseLeave: function( sender, e )
    {
        this.find( "PlayPauseLeave" ).begin();
    },
    
    onPlayPauseDown: function( sender, e )
    {
        if ( this.$playIcon.opacity ==  0.4) return;

        if ( this.$videoWindow.currentState == "Playing" )
        {
            this.$videoWindow.pause();
        }
        else if (this.$videoWindow.currentState == "Paused" || 
	             this.$videoWindow.currentState == "Stopped" ) 
        {
            if (this.$videoWindow.position.seconds <= this.$endPosition) 
            {
                this.$placeholderImage.visibility = "Collapsed";
                this.$videoWindow.play();
                this.$timer.begin();
            }
        }
        else if ( this.$videoWindow.currentState == "Buffering" )
        {
            if ( this.$pauseIcon.opacity == 1 )
                this.$videoWindow.pause();
        }
    },

    // Prev
    onPrevButtonEnter: function( sender, e )
    {
        this.find( "PrevEnter" ).begin();
    },

    onPrevButtonLeave: function( sender, e )
    {
        this.find( "PrevLeave" ).begin();
    },
    
    onPrevButtonDown: function( sender, e )
    {
        this.setMediaPosition ( this.$videoWindow.position.seconds - 15 ); 
    },

    // Next
    onNextButtonEnter: function( sender, e )
    {
        this.find( "NextEnter" ).begin();
    },

    onNextButtonLeave: function( sender, e )
    {
        this.find( "NextLeave" ).begin();
    },
    
    onNextButtonDown: function( sender, e )
    {
        this.setMediaPosition ( this.$videoWindow.position.seconds + 15 );            
    },

    // Stop
    onStopButtonEnter: function( sender, e )
    {
        this.find( "StopEnter" ).begin();
    },

    onStopButtonLeave: function( sender, e )
    {
        this.find( "StopLeave" ).begin();
    },
    
    onStopButtonDown: function( sender, e )
    {
	    this.stop();
    },   
    
    // VolumeDown
    onVolumeDownButtonEnter: function( sender, e )
    {
        this.find( "VolumeDownEnter" ).begin();
    },

    onVolumeDownButtonLeave: function( sender, e )
    {
        this.find( "VolumeDownLeave" ).begin();
    },
    
    onVolumeDownButtonDown: function( sender, e )
    {
        this.decreaseVolume();
        this.updateVolumeBars();
    },   
    
    // VolumeUp
    onVolumeUpButtonEnter: function( sender, e )
    {
        this.find( "VolumeUpEnter" ).begin();
    },

    onVolumeUpButtonLeave: function( sender, e )
    {
        this.find( "VolumeUpLeave" ).begin();
    },
    
    onVolumeUpButtonDown: function( sender, e )
    {
        this.increaseVolume();
        this.updateVolumeBars();
    },   
        
    // VolumeIndicator
    onVolumeIndicatorEnter: function( sender, e )
    {
    
    },

    onVolumeIndicatorLeave: function( sender, e )
    {
    
    },
    
    onVolumeIndicatorDown: function( sender, e )
    {
        if ( this.$videoWindow.volume != 0 )
        {
            this.$videoWindow.volume = 0;
        }
        else
        {
            this.$videoWindow.volume = .5;
        }
        this.updateVolumeDisplay();
    },   

    // VolumeSlider
    onVolumeSliderDown: function( sender, e )
    {
    	this.$isVolumeDragging = true;
    	this.$volumeSlider.captureMouse();
    },

    onVolumeSliderUp: function( sender, e )
    {
    	this.$isVolumeDragging = false;
    	this.$volumeSlider.releaseMouseCapture(); 
    },

    onVolumeSliderLeave: function( sender, e )
    {
      if ( this.$isVolumeDragging )
      {
      	this.$isVolumeDragging = false;
      	this.$volumeSlider.releaseMouseCapture();
      }
    },
    
    onVolumeSliderMove: function( sender, e )
    {
    	if ( !this.$isVolumeDragging ) return;
    	
	    this.setSliderPosition( this.$volumeSlider, this.$volumeSliderControl, e.getPosition(this.$volumeSliderControl).x );
	    this.setVolume();
	    this.updateVolumeBars();
    },

    // ProgressSlider
    onProgressSliderDown: function( sender, e )
    {
    	this.$isProgressDragging = true;
    	this.$progressSlider.captureMouse();
    	this.$placeholderImage.visibility = "Collapsed";

        if ( this.$videoWindow.currentState == "Playing" )
        {
            this.$resumePlayOnSliderRelease = true;
            this.$videoWindow.pause();
        }

        this.$timer.stop();
    },

    onProgressSliderUp: function( sender, e )
    {
    	this.$isProgressDragging = false;
    	this.$progressSlider.releaseMouseCapture();
    	
        if ( this.$resumePlayOnSliderRelease && this.$videoWindow.currentState == "Paused" )
        {
            this.$resumePlayOnSliderRelease = false;
            this.$videoWindow.play();
        }

        this.$timer.begin();
    },
    
    onProgressSliderMove: function( sender, e )
    {
    	if ( !this.$isProgressDragging ) return;
    	    	
	    var sliderPos = this.setSliderPosition( this.$progressSlider, this.$downloadProgressBar, e.getPosition(this.$progressBarControl).x );
	    this.setMediaPosition( ( sliderPos / this.$progressBarControl.width ) * this.$endPosition );
    },          
  
    onProgressBarBGDown: function( sender, e )
    {
        var sliderPos = this.setSliderPosition( this.$progressSlider, this.$progressBarControl, e.getPosition(this.$progressBarControl).x );
	    this.setMediaPosition( ( sliderPos / this.$progressBarControl.width ) * this.$endPosition );
     },
    
    formatTime: function( time ) 
    {
      time *= 1000;
      var timeString = "";
      var mins = Math.floor(time / 60000);
      timeString += ( mins < 10 ? "0" : "" ) + mins;

      timeString += ":";
      var seconds = Math.floor(time / 1000.0) % 60;
      if (seconds < 10)
              timeString += "0";

      timeString += seconds;
      return timeString;
    }
});FeatureBrowser.Scene = function() 
{
    $scene = this;
};

FeatureBrowser.Scene.prototype =
{
    showPage: function( pageIndex )
    {
        var tL = -pageIndex * 783;
        
        for ( var i = 0; i < this.$pages.length; i++ )
        {
            if ( i == pageIndex )
                this.$pages[i].preloadVideo();
            else
                this.$pages[i].stopVideo();
        }

        var sb = SplineAnimator.animate( this.$stage, "pageContainer", "0:0:0.750", { "(Canvas.Left)" : tL }, Splines.QuadEaseInOut );
        sb.begin();
        sb = SplineAnimator.animate( this.$stage, "bg", "0:0:0.9", { "(Canvas.Left)" : tL }, Splines.QuadEaseInOut );
        sb.begin();
    },
    
    populatePages: function()
    {
        this.$pageContainer = this.$stage.findName( "pageContainer" );
        this.$pages = [];
    
        for ( var i = 0; i < browserData.topics.length; i++ )
        {
            var featurePage = new FeatureBrowser.FeaturePage( browserData.topics[i], this, this.$pageContainer, i * 783, 0 );
            this.$pages.push( featurePage );
        }
    },
	
	handleLoad: function(plugIn, userContext, rootElement) 
	{
		FeatureBrowser.Scene.plugIn = plugIn;
		
		plugIn.settings.EnableFrameRateCounter = true;
		
		this.$stage = rootElement.findName( "stage" );
		
		this.$assetsReady = false;
		this.$fontsReady = false;
		
		// download assets
		var assetDownloader = plugIn.createObject("downloader");
        assetDownloader.addEventListener("downloadProgressChanged", Silverlight.createDelegate( this, this.onDownloaderProgressChanged ) );
        assetDownloader.addEventListener("completed", Silverlight.createDelegate( this, this.onAssetDownloaderCompleted ) );
        assetDownloader.open("GET", "../xaml/featurebrowser/shared.xaml");
        assetDownloader.send();
        
        FeatureBrowser.assetDownloader = assetDownloader;

        /* DISABLED. Can't host .ZIP or .TTF on microsoft.com
		var fontDownloader = plugIn.createObject("downloader");
        fontDownloader.addEventListener("downloadProgressChanged", Silverlight.createDelegate( this, this.onDownloaderProgressChanged ) );
        fontDownloader.addEventListener("completed", Silverlight.createDelegate( this, this.onFontDownloaderCompleted ) );
        
        if (window.location.toString().indexOf ( "microsoft.com" ) != -1 )
            fontDownloader.open("GET", "http://download.microsoft.com/download/4/9/a/49a68c6f-d22f-4c9e-b6bf-d1181b778626/segoeui.zip");
        else
            fontDownloader.open( "GET", "../fonts/fonts.zip" )    
        
        fontDownloader.send();
        
        FeatureBrowser.fontDownloader = fontDownloader;
        */
        
        this.$fontsReady = true;
        FeatureBrowser.fontDownloader = null;
	},
	
	
    onDownloaderProgressChanged: function( sender, args )
    {
    
    },
    
    onAssetDownloaderCompleted: function( sender, args )
    {
        this.$assetsReady = true;
        
        if ( this.$assetsReady && this.$fontsReady )
            this.init();
    },

    onFontDownloaderCompleted: function( sender, args )
    {
        this.$fontsReady = true;
        
        if ( this.$assetsReady && this.$fontsReady )
            this.init();
    },
        
    init: function()
    {
        this.$navBar = new FeatureBrowser.NavBar( browserData.topics, this, this.$stage, 0, 0 );
        
        this.populatePages();
        
        this.visibility = "Collapsed";
        this.$delayedInit = setTimeout( "$scene.delayedInit()", 100 );
    
    },
	
	delayedInit: function()
	{
	    clearTimeout( this.$delayedInit );
	    this.$navBar.setSelectedByIndex( 0 );
	    this.visibility = "Visible";
	}
};

SplineAnimator = new function()
{
};

SplineAnimator.uniqueId = 0;

SplineAnimator.animate = function ( parentCanvas, targetName, durationStr, propValueHash, splineType )
{
    var xaml = "";
    
    var name = "sb_" + parseInt( SplineAnimator.uniqueId++ ).toString();
    
    xaml += '<Storyboard Name="' + name + '">\n';

    for ( var prop in propValueHash )
    {
        xaml += '<DoubleAnimationUsingKeyFrames Storyboard.TargetName="' + targetName + '" ';
        xaml += 'Storyboard.TargetProperty="' + prop + '">\n';
        xaml += '<SplineDoubleKeyFrame KeyTime="' + durationStr + '" ';
        xaml += 'Value="' + parseFloat( propValueHash[prop] ).toString() + '" ';
        xaml += 'KeySpline="' + splineType + '" />\n';
        xaml += '</DoubleAnimationUsingKeyFrames>\n';
    }

    xaml += '</Storyboard>';
    
    var sb = parentCanvas.getHost().content.createFromXaml( xaml );
    
    //var completedFunc = function()
    //{
    //    sb.removeEventListener( "completed", completedDelegate );
    //    parentCanvas.resources.remove( sb );
    //}
    //completedDelegate = Silverlight.createDelegate( this, completedFunc );       
    
    //sb.addEventListener( "completed", completedDelegate );
    
    parentCanvas.resources.add( sb );
    
    return sb;
};


createNamespace( "Splines" );

Splines.Linear = "0, 0, 1, 1";
Splines.QuadEaseIn = "0.5, 0, 1, 0";
Splines.QuadEaseOut = "0, 0, .5, 1";
Splines.QuadEaseInOut = "0.5, 0, 0.5, 1";FeatureBrowser.StaticContentItem = FeatureBrowser.AGFrameworkElement.extend(
{
    construct: function( data, controller, parent, left, top ) 
    {
        arguments.callee.$.construct.call( this, parent );
        
        this.$controller = controller;
        this.$data = data;
     
        this.init( left, top );
    },
    
    toString: function()
    {
        return "NavItem["+this.$data.title+"]";
    },
    
    init: function ( left, top )
    {
        var xaml = FeatureBrowser.assetDownloader.getResponseText( "StaticContentItem.xaml" );
        this.$root = this.createFromXaml( xaml );

        this.$root["Canvas.Left"] = left;
        this.$root["Canvas.Top"] = top;
                
        this.cacheElementRef( "img" );
        this.cacheElementRef( "descTB", "descTb" );
        
        this.$descTb.setFontSource( FeatureBrowser.fontDownloader );
        this.$descTb.fontFamily = "Segoe UI";
        this.$descTb.text = this.$data.description;         
    },
    
    
    setImage: function( image )
    {
        this.$img.setSource( image, "" );
    }
});
FeatureBrowser.TopicItem = FeatureBrowser.AGFrameworkElement.extend(
{
    construct: function( data, controller, parent, left, top ) 
    {
        arguments.callee.$.construct.call( this, parent );
        
        this.$controller = controller;
        this.$data = data;
     
        this.init( left, top );
    },
    
    toString: function()
    {
        return "TopicItem[]";
    },
    
    init: function ( left, top )
    {
        var xaml = FeatureBrowser.assetDownloader.getResponseText( "TopicItem.xaml" );    
        arguments.callee.$.init.call( this, xaml, left, top );
        
        this.cacheElementRef( "titleTB" );
        this.cacheElementRef( "bgRect" );
        this.cacheElementRef( "videoIcon" );
        this.cacheElementRef( "SelectAnim", "selectAnim" );
        this.cacheElementRef( "DeselectAnim", "deselectAnim" );
        this.cacheElementRef( "HighlightAnim", "highlightAnim" );
        this.cacheElementRef( "UnhighlightAnim", "unhighlightAnim" );

        this.$selectAnim.addEventListener( "Completed", Silverlight.createDelegate( this, this.onSelectAnimationCompleted )); 

        this.$root.cursor = "Hand";
        this.$root.addEventListener("MouseEnter", Silverlight.createDelegate( this, this.onMouseEnter));
        this.$root.addEventListener("MouseLeave", Silverlight.createDelegate( this, this.onMouseLeave));
        this.$root.addEventListener("MouseLeftButtonDown", Silverlight.createDelegate( this, this.onPress));
        
        this.$titleTB.text = this.$data.title;
        
        this.$titleTB.setFontSource( FeatureBrowser.fontDownloader );
        this.$titleTB.fontFamily = "Segoe UI";
        
        if ( this.$data.isVideo )
        {
            this.$videoIcon.visibility = "Visible";
            this.$titleTB["Canvas.Left"] = this.$videoIcon["Canvas.Left"] + this.$videoIcon.width + 6;
        }
        
        this.$offColor = "#FFCCCCCC";
        this.$downColor = "#FFFFFFFF";

        this.name = this.$data.id
    },
    
    setSelected: function( flag /*Boolean*/) /*:void*/
    {
        this.selected = flag;
        
        if ( flag ) this.select();
        else this.deselect();
    },
    
    setHighlighted: function( flag /*Boolean*/) /*:void*/
    {
        if ( this.selected ) return;

        this.highlighted = flag;
        
        if ( flag ) this.highlight();
        else this.unhighlight();
    },
    
    select: function() /*:void*/
    {
        this.unhighlight();

        this.$selectAnim.begin();
    },
    
    deselect: function() /*:void*/
    {
        this.$deselectAnim.begin();
    },
    
    highlight: function()  /*:void*/
    {
        this.$highlightAnim.begin();
    },
    
    unhighlight: function()  /*:void*/
    {
        this.$unhighlightAnim.begin();
    },

    onMouseEnter: function( sender, args )
    {
        if ( !this.selected )
        {
            this.$highlightAnim.begin();
        }
    },
    
    onMouseLeave: function( sender, args )
    {
        if ( !this.selected )
        {
            this.$unhighlightAnim.begin();
        }
    },
    
    onPress: function( sender, args )
    {
        if ( !this.selected )
        {
            this.$controller.onTopicItemClicked( this );
        }
    },
    
    onSelectAnimationCompleted: function( sender, args )
    {
         this.$controller.onSelectAnimationComplete();
    }
});
FeatureBrowser.TopicSelector = FeatureBrowser.AGFrameworkElement.extend(
{
    construct: function( data, controller, parent, left, top ) 
    {
        arguments.callee.$.construct.call(this, parent);
        
        this.$controller = controller;
        this.$data = data;
     
        this.init( left, top );
    },

    toString: function()
    {
        return "TopicSelector[]";
    },

    init: function ( left, top )
    {
        var xaml = FeatureBrowser.assetDownloader.getResponseText( "TopicSelector.xaml" );    
        arguments.callee.$.init.call( this, xaml, left, top );
        
        this.populateList();
    },

    getWidth: function()
    {
        return this.$root.width;
    },

    populateList: function()
    {
        this._items = [];
        var topicItem;
        var padding = 2;
        var offset = 0;

        // create feature tour item
        topicItem = new FeatureBrowser.TopicItem( { isVideo: true, title:"Video Feature Tour" }, this, this.$root, 0, 0 );
        this._items.push( topicItem );
        offset += 22 + padding;

        for ( var i = 0; i < this.$data.length; i++ )
        {
            topicItem = new FeatureBrowser.TopicItem( this.$data[i], this, this.$root, 0, offset );
            this._items.push( topicItem );
            offset += 22 + padding;
        }
    },
    
    arrangeItems: function()
    {
    
    },
    
    selectTopicItem: function( itemToSelect )
    {
        if ( this.$isSelecting ) return;
        this.$isSelecting = true;
        
        var item;
        
        for ( var i = 0; i < this._items.length; i++ )
        {
            item = this._items[i];
            item.setSelected ( item == itemToSelect );
        }
                
        this.arrangeItems();
        
        this.$controller.onTopicSelected( itemToSelect.$data );
    },
    
    setSelectedByTopic: function( topic )
    {
        for ( var i = 0; i < this._items.length; i++ )
        {
            if ( this._items[i].$data == topic )
            {
                this.selectTopicItem( this._items[i] );
                break;
            }
        }
    },

    setSelectedByIndex: function( index )
    {
        this.selectTopicItem( this._items[index] );
    },

    // Event Handlers

    onTopicItemClicked: function( itemToSelect )
    {
        this.selectTopicItem( itemToSelect );   
    },
    
    onTopicItemRollover: function( activeItem )
    {
        var item;
        for ( var i = 0; i < this._items.length; i++ )
        {
            item = this._items[i];
            item.setHighlighted ( item == activeItem ) 
        }
        
        this.arrangeItems();
    },
    
    onTopicItemRollout: function( inactiveItem )
    {
        var item;
        for ( var i = 0; i < this._items.length; i++ )
        {
            item = this._items[i];
            item.setHighlighted ( false ) 
        }
        
        this.arrangeItems();
    },
    
    onSelectAnimationComplete: function()
    {
        this.$isSelecting = false;
    }
});