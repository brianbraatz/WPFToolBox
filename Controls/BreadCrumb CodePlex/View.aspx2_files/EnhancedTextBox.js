// Support script for the EnhancedTextBox control
Type.registerNamespace("CodePlex");

CodePlex.EnhancedTextBox=function(){
    // protected fields
    var _id, _ff, _limit, _textarea, _counter, _showCounter, _rowState, _autoEnlarge, _maxAutoEnlarge, _defaultButton, _multiLine, _loadTimer, _keyTimer;

    // public members    
    this.initialize=function(textareaID, limit, counterID, showCounter, stateID, autoEnlarge, maxAutoEnlarge, defaultButtonID){
        _ff=Sys.Browser.name=='Firefox'||Sys.Browser.name=='Mozilla';
        _limit=limit&&limit>0?parseInt(limit):0;
        _textarea=$get(_id=textareaID);
        _counter=$get(counterID);
        _showCounter=showCounter?true:false;
        _rowState=$get(stateID);
        _autoEnlarge=autoEnlarge?true:false;
        _maxAutoEnlarge=maxAutoEnlarge;
        _defaultButton=$get(defaultButtonID);
        _multiLine=_textarea&&_textarea.tagName.toLowerCase()=='textarea';

        initializeTextArea();
        
        CodePlex.EnhancedTextBoxHandler.register(textareaID, this);
    }

    this.dispose=dispose;

    this.enlarge=function(rows){
        if(!_textarea||!_rowState)return;
        _rowState.value=_textarea.rows=parseInt(_textarea.rows)+(rows||5);
    }
    
    // protected members
    function dispose(){
        if(_loadTimer)clearTimeout(_loadTimer);
        if(_keyTimer)clearTimeout(_keyTimer);
        registerEvents(false);
        CodePlex.EnhancedTextBoxHandler.unregister(_id);
        _id=_ff=_limit=_textarea=_counter=_showCounter=_rowState=_autoEnlarge=_maxAutoEnlarge=_defaultButton=_multiLine=_loadTimer=_keyTimer=null;
    }
    
    function initializeTextArea(){
        registerEvents(true);
        if(_loadTimer)clearTimeout(_loadTimer);
        if(!_ff&&_textarea)_textarea.style.display='none';
        if(_textarea)_textarea.disabled=true;
        _loadTimer=setTimeout(populateTextArea, 250);
    }

    function populateTextArea(){
        if(!_textarea)return;
        var txt;
        if(txt=_textarea.getAttribute('text')){
            _textarea.value=txt;
            _textarea.removeAttribute('text');
            onChange();
        }
        if(!_ff)_textarea.style.display='block';
        _textarea.disabled=false;
        
    }
    
    function registerEvents(register){
        var $handler=register||!arguments.length?$addHandler:$removeHandler;
        $handler(_textarea,'keydown',onKeyDown);
        $handler(_textarea,'keyup',onKeyUp);
        $handler(_textarea,'keypress',onKeyPress);
        $handler(_textarea,'change',onChange);
        $handler(window,'unload',dispose);        
    }
    
    function autoEnlargeTextBox(textBox, state, maxAutoRows) {
        var increment=5;
        if (textBox && textBox.rows) {
            if (document.all) { // IE
                while (textBox.scrollHeight > textBox.clientHeight && isLessThanMaxRows(textBox, maxAutoRows)) {
                    textBox.rows += increment;
                }
                textBox.scrollTop = 0;
            } else {
                var lineBreaks = countLineBreaks(textBox.value);
                var wrap = textBox.getAttribute('wrap');
    //            alert("lineBreaks: " + lineBreaks + ", textBox.rows: " + textBox.rows + ", maxAutoRows: " + maxAutoRows);
                if (lineBreaks > parseInt(textBox.rows)) {
                    while (lineBreaks > parseInt(textBox.rows) && isLessThanMaxRows(textBox, maxAutoRows)) {
                        textBox.rows += increment
                    }
                } else if (wrap.toLowerCase() == 'soft' || wrap.toLowerCase() == 'hard') {
                    while (parseInt(textBox.rows) * parseInt(textBox.cols) <= textBox.value.length &&
                           isLessThanMaxRows(textBox, maxAutoRows)) {
                        textBox.rows += increment;
                    }
                }
            }
            if (state) state.value = textBox.rows;
        }
    }
    function updateCounter(text, delay){
        if(!_showCounter)return;
        if(!_limit)return setCounter(text.length, delay);
        return setCounter(text.length<_limit?_limit-text.length:0, delay);
    }
    function setCounter(count, delay){
        if(!_counter)return;
        if(delay && _counter.innerHTML-count<10)return;
        _counter.innerHTML=count;
    }
    function countLineBreaks(text){
        return text.split('\n').length-1;
    }
    function isLessThanMaxRows(maxRows) {
        if (!maxRows) return true;
        return (parseInt(_textarea.rows) < maxRows);
    }
    function isLessThanLimit(text){
        if(!_limit)return true;
        return _textarea.value.length<_limit || (text||normalize(_textarea.value)).length<_limit;
    }
    function isGreaterThanLimit(text){
        if(!_limit)return false;
        return _textarea.value.length>_limit && (text||normalize(_textarea.value)).length>_limit;
    }
    function normalize(text){
        if(text.indexOf("\r")>-1){
            if(text.indexOf("\r\n")>-1)text=text.split("\r\n").join("\n");
            if(text.indexOf("\r")>-1)text=text.split("\r").join("\n");
        }
        return text;
    }
    function getSelection(){
        if(typeof(_textarea.createTextRange)!='undefined'){
            var sel=document.selection&&document.activeElement==_textarea?document.selection.createRange():null;
            return sel?sel.text:'';
        }
        if(typeof(_textarea.selectionStart)!='undefined')return _textarea.value.substring(_textarea.selectionStart,_textarea.selectionEnd);
        return '';
    }
    function cancelEvent(e){
        if(e&&e.preventDefault)e.preventDefault();
        return false;
    }

    // event handlers
    function onKeyDown(e){
        updateCounter(normalize(_textarea.value),1);
    }
    function onKeyUp(e){
        if(_keyTimer)clearTimeout(_keyTimer);
        _keyTimer=setTimeout(onChange,250);
    }
    function onKeyPress(e){
        var doCancel=false;
        if(_multiLine)doCancel=!onKeyPress_MultiLine(e);
        else doCancel=!onKeyPress_SingleLine(e);
        if(doCancel)return cancelEvent(e);
    }
    function onKeyPress_SingleLine(e){
        var keyCode = e.keyCode;
        if (keyCode == 13 && _defaultButton){
            _defaultButton.click();
            return false;
        }
        return true;
    }
    function onKeyPress_MultiLine(e){
        if(!_limit)return true; // don't spin if there is no limit
        var keyCode=e.charCode;
        if(e.ctrlKey || e.shiftKey || e.altKey)return true; // always allow ctrl, shift or alt combos
        var allowMap={         // always allow these keys to be pressed
            8:  true,  // backspace
            34: true, 33: true, 35: true, 36: true,  // page down, page up, end, home
            37: true, 38: true, 39: true, 40: true,  // arrow keys
            46: true  // delete
        };
        if (allowMap[keyCode])return true;
        if(getSelection().length)return true; // always allow selected text to be overwritten
        return isLessThanLimit();
    }
    function onChange(){
        if(!_textarea)return;
        var normalizedText=normalize(_textarea.value);
        if(isGreaterThanLimit(normalizedText)){
//        alert(['setter',normalizedText.length])
            normalizedText=normalizedText.substring(0,_limit||Infinity);
            _textarea.value=normalizedText;
        }
        updateCounter(normalizedText);
        if (_autoEnlarge) {
            autoEnlargeTextBox(_textarea, state);
        }
    }
}

CodePlex.EnhancedTextBox.enlargeTextBox=function(textBoxID, rows){
    var textbox=CodePlex.EnhancedTextBoxHandler.getTextBox(textBoxID);
    if(textbox)textbox.enlarge(rows);
}


CodePlex.EnhancedTextBoxHandler=new function(){
    var _textboxes={};
    this.register=function(textBoxID, enhancedTextBox){

// in a full-clientstate app, this error is correct. with updatepanels, however, all the innerhtml 
// gets wiped, and we have to assume the new textbox is the correct replacement
//        if(_textboxes[textBoxID])throw new Error(String.format("The textbox '{0}' has already been registered.", textBoxID));
        if(_textboxes[textBoxID])_textboxes[textBoxID].dispose();
        
        _textboxes[textBoxID]=enhancedTextBox;
    }
    this.unregister=function(textBoxID){
        if(_textboxes[textBoxID]){
            _textboxes[textBoxID]=null;
            delete _textboxes[textBoxID];
        }
    }
    this.getTextBox=function(textBoxID){
        return _textboxes[textBoxID];
    }
}

// Notify ASP.NET AJAX that we loaded successfully.
if (typeof(Sys) !== "undefined") Sys.Application.notifyScriptLoaded();