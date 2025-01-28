// Support script for the EnhancedTextBox control
Type.registerNamespace("CodePlex");

CodePlex.PromptBox=function(promptText){
    var _this, _textBoxID,_promptText, _dirty, _css;
    this.initialize=function(textBoxID){
        _this=this;
        _textBoxID=textBoxID;
        _promptText=promptText;
        _css=' EmptyTextBox';
        blurHandler();
        registerEvents();
        return _this;
    }
    this.dispose=dispose;
    function dispose(){
        registerEvents(true);
        _this=_textBoxID=_promptText=_dirty=_css=null;
    }

    function getElement(){
        return $get(_textBoxID);
    }
    
    function focusHandler(e){
        var elt=getElement();
        if(!elt)return;
        var isBlank=!_dirty;
        if(isBlank)elt.value='';
        if(elt.className.indexOf(_css)>-1)elt.className=elt.className.split(_css).join('');
    }
    function blurHandler(e){
        var elt=getElement();
        if(!elt)return;
        if(!(elt.dirty=_dirty=!!elt.value)){
            if(elt.className.indexOf(_css)<0)elt.className+=_css;
            if(_promptText)elt.value = _promptText;
        }
    }
    function submitHandler(e){
        var elt=getElement();
        if(!elt)return;
        if(!_dirty)elt.value='';
    }
    function keyHandler(e){
        var elt=getElement();
        if(!elt)return;
        switch(e.keyCode){
            case 13:
                blurHandler(e);
                break;
            case 27:
                elt.value='';
                elt.blur();
                break;
        }
    }
    
    function registerEvents(disposing){
        var elt=getElement();
        if(!elt)return;
        var $handler=!disposing?$addHandler:$removeHandler;
        $handler(window,'unload', dispose);
        $handler(elt,'focus', focusHandler);
        $handler(elt,'blur', blurHandler);
        $handler(elt,'keydown', keyHandler);
        for(var i=0;i<document.forms.length;i++){
            $handler(document.forms[i],'submit',submitHandler);
        }
    }
}
