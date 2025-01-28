
//
// MultiFileUpload.js
//
Type.registerNamespace("CodePlex.MultiFileUpload");

CodePlex.MultiFileUpload.UploadControlsTemplate = function(namePrefix,iconRoot) {
    this._namePrefix = namePrefix;
    this._iconRoot = iconRoot;
}

CodePlex.MultiFileUpload.UploadControlsTemplate.prototype = {
    render: function(container) {
        this.baseContainer=container;
        var row = document.createElement("tr");
        this._renderToTableRow(container.appendChild(row), this._namePrefix);
    },
    
    _renderToTableRow: function(row, namePrefix) {
        var typeIconCell= row.insertCell(0);
        var filenameCell = row.insertCell(1);
        var typeCell = row.insertCell(2);
        var locationCell = row.insertCell(3);
        
        this._renderFilenameTemplate(filenameCell, namePrefix);
        this._renderLocationTemplate(locationCell, namePrefix);
        this._renderTypeTemplate(typeCell, namePrefix);
        this._renderTypeIconTemplate(typeIconCell, namePrefix);
    },
    
    _renderFilenameTemplate: function(container, namePrefix) {
        var input = document.createElement("input");
        
        input.setAttribute("type", "text");
        input.name = namePrefix + "_Filename";
        
        input.maxLength = 80;
        
        container.appendChild(document.createTextNode("Filename"));
        container.appendChild(document.createElement("br"));
        container.appendChild(input);
    },
    
    _renderLocationTemplate: function(container, namePrefix) {
        var input = document.createElement("input");
        
        input.setAttribute("type", "file");
        input.id=input.name = namePrefix + "_Location";
        
        container.appendChild(document.createTextNode("Location"));
        container.appendChild(document.createElement("br"));
        container.appendChild(input);
    },
    _renderTypeIconTemplate: function(container, namePrefix) {
        if(!this.typeIcon){
            this.typeIcon = container.appendChild(document.createElement("img"));
            this.typeIcon.className='FileTypeImage';
        }
        this.typeIcon.src=this._iconRoot+this.typeSelect.value+'.gif';
    },    
    _renderTypeTemplate: function(container, namePrefix) {
        var select = this.typeSelect = document.createElement("select");
        
        select.multiple = false;
        select.name = namePrefix + "_Type";
        
        select.onchange=Function.createDelegate(this,this._renderTypeIconTemplate);
        
        this._renderTypeItem(select, "Source Code", "SourceCode");
        this._renderTypeItem(select, "Runtime Binary", "RuntimeBinary");
        this._renderTypeItem(select, "Documentation", "Documentation");
        this._renderTypeItem(select, "Example", "Example");
        
        container.appendChild(document.createTextNode("Type"));
        container.appendChild(document.createElement("br"));
        container.appendChild(select);
    },
    
    _renderTypeItem: function(select, text, value) {
        var option = document.createElement("option");
        
        option.text = text;
        option.value = value;
        
        select.options.add(option);
    }
}

CodePlex.MultiFileUpload.UploadControlsTemplate.registerClass("CodePlex.MultiFileUpload.UploadControlsTemplate");

CodePlex.MultiFileUpload._fileCount = 0;

CodePlex.MultiFileUpload.getFileCount = function() {
    return CodePlex.MultiFileUpload._fileCount;
}

CodePlex.MultiFileUpload.getNextNamePrefix = function(base) {
    return base + "_" + CodePlex.MultiFileUpload.getFileCount();
}

CodePlex.MultiFileUpload.addFileToTable = function(tableId, namePrefix, iconRoot) {
    var table = $get(tableId);
    if (table) {
        if(table.tBodies && table.tBodies.length)table = table.tBodies[0];
        CodePlex.MultiFileUpload._fileCount++;
        var template = new CodePlex.MultiFileUpload.UploadControlsTemplate(namePrefix,iconRoot);
        template.render(table);
    }
}

if (typeof(Sys) !== "undefined") Sys.Application.notifyScriptLoaded();