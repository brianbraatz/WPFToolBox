Type.registerNamespace('CodePlex');

var UpdateProgress_Hide;

// UpdateProgress Singleton
CodePlex.UpdateProgress=new function(){
    var _this, _ie6, _ff, _rootEl, _panel, _message, _control, _clientID, _resizeTimer;
    
    this.initialize=function(clientID){
        _this=this;
        _clientID=clientID;
        _ff=Sys.Browser.name=='Firefox'||Sys.Browser.name=='Mozilla';
        _ie6=(Sys.Browser.name=="Microsoft Internet Explorer"&&Sys.Browser.version<7);
        _rootEl=!_ie6&&typeof(document.documentElement.scrollLeft)!="undefined"?document.documentElement:document.body;
        Sys.UI._UpdateProgress.prototype._startRequest=getDelegate(this,updateProgressPanel,[Sys.UI._UpdateProgress.prototype._startRequest]);
        Sys.UI._UpdateProgress.prototype._handleEndRequest=getDelegate(this,resetProgressPanel,[Sys.UI._UpdateProgress.prototype._handleEndRequest]);
        if(ensurePanel()){
            _panel.className='UpdateProgressPanel FullPanel';
            _message=getChild('UpdateProgressMessage',_panel);
            if(_ie6){ // gratuitous runtime hack to iframe shim the ole ie6 for com window objects
                _panel.insertAdjacentHTML('afterbegin','<iframe class="FullPanel" src="javascript:\'<html></html>\';" tabIndex="-1"></iframe>');
            }
            registerEvents();
        }
    }
    
    this.dispose=dispose;
    this.show=show;
    this.reset=resetProgressPanel;

    // protected exposed
    function show(visible){
        updateProgressPanel();
        if(ensureControl())_control.set_visible(visible);
    }
    function dispose(){
        if(_resizeTimer)clearTimeout(_resizeTimer);
        registerEvents(true);
        CodePlex.UpdateProgress=_this.constructor; 
        // or shall we completely snuff our own global reference? 
        // CodePlex.UpdateProgress=null;
        _this=_ie6=_ff=_rootEl=_panel=_message=_control=_clientID=_resizeTimer=null;
    }

    //protected methods
    function ensureControl(){
        if(!_control)_control=$find(_clientID);
        return !!_control;
    }
    function ensurePanel(){
        if(!_panel)_panel=$get(_clientID);
        return !!_panel;
    }
    function ensurePanelLayout(){
        
        if(!ensurePanel())return false;
        _panel.style.top=_panel.style.left=0;
        _panel.style.width=_rootEl.scrollWidth+"px";
        _panel.style.height=_rootEl.scrollHeight+"px";
        if(!ensureControl())return false;
        return true;
    }
    function updateProgressPanel(callback, sender, args){
        if(ensurePanelLayout()){
            if(!_message)return;
            var root=document.documentElement;
            var dims=[((root.offsetWidth-_rootEl.scrollWidth)/2)+root.scrollLeft,((root.offsetHeight-_rootEl.scrollHeight)/2)+root.scrollTop];
            _message.style.left=Math.round(dims[0])+"px";
            _message.style.top=Math.round(dims[1])+"px";
        }
        fireCallback(callback, sender, args);
    }
    function resetProgressPanel(callback,force){
        fireCallback(callback);
        ensurePanelLayout();
        if(!ensurePanel())return;
        _panel.style.width=_panel.style.height="auto";
    }
    function getDelegate(obj,fnc,args){
        return function(){       
            for(var i=0; i<arguments.length; i++) args[args.length] = arguments[i];
            if(fnc)return fnc.apply(obj,args);
        };
    }
    function fireCallback(callback){
        if(typeof(callback)!='function')return;
        var args = [];
        for(var i=1; i<arguments.length; i++) args[args.length] = arguments[i];
        if(ensureControl())getDelegate(_control,callback,args)();
    }
    function getChild(id,elt){
        if(!elt||!elt.nodeType)return null;
        for(var i=0;i<elt.childNodes.length;i++){
            if(elt.childNodes[i].id==id)return elt.childNodes[i];
            var child=getChild(id,elt.childNodes[i]);
            if(child)return child;
        }
        return null;
    }
    function registerEvents(disposing){
        var $handler=!disposing?$addHandler:$removeHandler;
        $handler(window,'unload',dispose);
        $handler(window,'scroll',scrollHandler);
        $handler(document,'keydown',keyHandler);
        for(var i=0;i<document.forms.length;i++){
            $handler(document.forms[i],'submit',showHandler);
        }
    }
    
    // events
    function showHandler(e){
        if(UpdateProgress_Hide)
        {
            UpdateProgress_Hide = false;
            return;
        }
        if(typeof(Page_IsValid)=='undefined'||Page_IsValid)
            show(true);
    }
    function scrollHandler(e){
        if(!ensureControl()||!_control.get_visible())return;
        updateProgressPanel.apply(_control,[null]);
    }
    function resizeHandler(e){
        if(_resizeTimer)clearTimeout(_resizeTimer);
        ensurePanelLayout();
        _resizeTimer=setTimeout(resetProgressPanel,1);
    }
    function keyHandler(e){
        if(ensureControl()&&_control.get_visible()){
            if(e.preventDefault)e.preventDefault();
            if(e.stopPropagation)e.stopPropagation();
            return e.returnValue=false;
        }
    }
};
