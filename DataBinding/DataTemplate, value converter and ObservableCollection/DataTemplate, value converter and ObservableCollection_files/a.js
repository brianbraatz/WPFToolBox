/*
Core JavaScript Library
$Id: core.js 232 2007-10-01 20:32:42Z whitaker $

Copyright (c) 2005, Six Apart, Ltd.
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    * Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following disclaimer
in the documentation and/or other materials provided with the
distribution.

    * Neither the name of "Six Apart" nor the names of its
contributors may be used to endorse or promote products derived from
this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

/* stubs */

log = function() {};
log.error = log.warn = log.debug = log;


/* utility functions */

defined = function( x ) {
    return x === undefined ? false : true;
}


/**
 * Utility method.
 * @param x <code>any</code> Any JavaScript value, including <code>undefined</code>.
 * @return boolean <code>true</code> if the value is not <code>null</code> and is not <code>undefined</code>.
 */
exists = function( x ) {
   return (x === undefined || x === null) ? false : true;
}


finite = function( x ) {
    return isFinite( x ) ? x : 0;
}


finiteInt = function( x, base ) {
    return finite( parseInt( x, base ) );
}


finiteFloat = function( x ) {
    return finite( parseFloat( x ) );
}


max = function() {
    var a = arguments;
    var n = a[ 0 ];
    for( var i = 1; i < a.length; i++ )
        if( a[ i ] > n )
            n = a[ i ];
    return n;
}


min = function() {
    var a = arguments;
    var n = a[ 0 ];
    for( var i = 1; i < a.length; i++ )
        if( a[ i ] < n )
            n = a[ i ];
    return n;
}


/* try block */  
 
Try = {
    these: function() {
        for( var i = 0; i < arguments.length; i++ ) {
            try {
                return arguments[ i ]();
            } catch( e ) {}
        }
        return undefined;
    }
}


/* unique id generator */

Unique = {
    length: 0,
    
    id: function() {
        return ++this.length;
    }
}


/* event methods */

if( !defined( window.Event ) )
    Event = {};


Event.stop = function( event ) {
    event = event || this;
    if( event === Event )
        event = window.event;

    // w3c
    if( event.preventDefault )
        event.preventDefault();
    if( event.stopPropagation )
        event.stopPropagation();

    // ie
    try {
        event.cancelBubble = true;
        event.returnValue = false;
    } catch( e ) {}

    return false;
}


Event.prep = function( event ) {
    event = event || window.event;
    if( !defined( event.stop ) )
        event.stop = this.stop;
    if( !defined( event.target ) )
        event.target = event.srcElement;
    if( !defined( event.relatedTarget ) ) 
        event.relatedTarget = event.toElement;
    return event;
}


try { Event.prototype.stop = Event.stop; }
catch( e ) {}


/* object extensions */

Function.stub = function() {};


if( !Object.prototype.hasOwnProperty ) {
    Object.prototype.hasOwnProperty = function( p ) {
        if( !(p in this) )
            return false;
        try {
            var pr = this.constructor.prototype;
            while( pr ) {
                if( pr[ p ] === this[ p ] )
                    return false;
                if( pr === pr.constructor.prototype )
                    break;
                pr = pr.constructor.prototype;
            }
        } catch( e ) {}
        return true;
    }
}


Object.prototype.extend = function() {
    var a = arguments;
    for( var i = 0; i < a.length; i++ ) {
        var o = a[ i ];
        for( var p in o ) {
            try {
                if( !this[ p ] &&
                    (!o.hasOwnProperty || o.hasOwnProperty( p )) )
                    this[ p ] = o[ p ];
            } catch( e ) {}
        }
    }
    return this;
}


Object.prototype.override = function() {
    var a = arguments;
    for( var i = 0; i < a.length; i++ ) {
        var o = a[ i ];
        for( var p in o ) {
            try {
                if( !o.hasOwnProperty || o.hasOwnProperty( p ) )
                    this[ p ] = o[ p ];
            } catch( e ) {}
        }
    }
    return this;
}


Object.prototype.extend( {
    init: Function.stub,
    destroy: Function.stub
} );



/* function extensions */

Function.prototype.extend( {
    bind: function( object ) {
        var method = this;
        return function() {
            return method.apply( object, arguments );
        };
    },
    
    
    bindEventListener: function( object ) {
        var method = this; // Use double closure to work around IE 6 memory leak.
        return function( event ) {
            try {
                event = Event.prep( event );
            } catch( e ) {}
            return method.call( object, event );
        };
    }
} );


/* class helpers */

indirectObjects = [];


Class = function( superClass ) {

    // Set the constructor:
    var constructor = function() {
        if( arguments.length )
            this.init.apply( this, arguments );
    };    
    //   -- Accomplish static-inheritance:
    constructor.override( Class );  // inherit static methods from Class
    superClass = superClass || Object; 
    constructor.override( superClass ); // inherit static methods from the superClass 
    constructor.superClass = superClass.prototype;
    
    // Set the constructor's prototype (accomplish object-inheritance):
    constructor.prototype = new superClass();
    constructor.prototype.constructor = constructor; // rev. 0.7    
    //   -- extend prototype with Class instance methods
    constructor.prototype.extend( Class.prototype );    
    //   -- override prototype with interface methods
    for( var i = 1; i < arguments.length; i++ )
        constructor.prototype.override( arguments[ i ] );
    
    return constructor;
}


Class.extend( {
    initSingleton: function() {
        if( this.singleton )
            return this.singleton;
        this.singleton = this.singletonConstructor
            ? new this.singletonConstructor()
            : new this();
        this.singleton.init.apply( this.singleton, arguments );
        return this.singleton;
    }
} );


Class.prototype = {
    destroy: function() {
        try {
            if( this.indirectIndex )
                indirectObjects[ this.indirectIndex ] = undefined;
            delete this.indirectIndex;
        } catch( e ) {}
        
        for( var property in this ) {
            try {
                if( this.hasOwnProperty( property ) )
                    delete this[ property ];
            } catch( e ) {}
        }
    },
    
    
    getBoundMethod: function( methodName ) {
        return this[ name ].bind( this );
    },
    
    
    getEventListener: function( methodName ) {
        return this[ methodName ].bindEventListener( this );
    },
    
    
    getIndirectIndex: function() {
        if( !defined( this.indirectIndex ) ) {
            this.indirectIndex = indirectObjects.length;
            indirectObjects.push( this );
        }
        return this.indirectIndex;
    },
    
    
    getIndirectMethod: function( methodName ) {
        if( !this.indirectMethods )
            this.indirectMethods = {};
        var method = this[ methodName ];
        if( typeof method != "function" )
            return undefined;
        var indirectIndex = this.getIndirectIndex();
        if( !this.indirectMethods[ methodName ] ) {
            this.indirectMethods[ methodName ] = new Function(
                "var o = indirectObjects[" + indirectIndex + "];" +
                "return o." + methodName + ".apply( o, arguments );"
            );
        }
        return this.indirectMethods[ methodName ];
    },
    
    
    getIndirectEventListener: function( methodName ) {
        if( !this.indirectEventListeners )
            this.indirectEventListeners = {};
        var method = this[ methodName ];
        if( typeof method != "function" )
            return undefined;
        var indirectIndex = this.getIndirectIndex();
        if( !this.indirectEventListeners[ methodName ] ) {
            this.indirectEventListeners[ methodName ] = new Function( "event",
                "try { event = Event.prep( event ); } catch( e ) {}" +
                "var o = indirectObjects[" + indirectIndex + "];" +
                "return o." + methodName + ".call( o, event );"
            );
        }
        return this.indirectEventListeners[ methodName ];
    }
}


/* string extensions */

String.extend( {
    escapeJSChar: function( c ) {
        // try simple escaping
        switch( c ) {
            case "\\": return "\\\\";
            case "\"": return "\\\"";
            case "'":  return "\\'";
            case "\b": return "\\b";
            case "\f": return "\\f";
            case "\n": return "\\n";
            case "\r": return "\\r";
            case "\t": return "\\t";
        }
        
        // return raw bytes now ... should be UTF-8
        if( c >= " " )
            return c;
        
        // try \uXXXX escaping, but shouldn't make it for case 1, 2
        c = c.charCodeAt( 0 ).toString( 16 );
        switch( c.length ) {
            case 1: return "\\u000" + c;
            case 2: return "\\u00" + c;
            case 3: return "\\u0" + c;
            case 4: return "\\u" + c;
        }
        
        // should never make it here
        return "";
    },
    
    
    encodeEntity: function( c ) {
        switch( c ) {
            case "<": return "&lt;";
            case ">": return "&gt;";
            case "&": return "&amp;";
            case '"': return "&quot;";
            case "'": return "&apos;";
        }
        return c;
    },


    decodeEntity: function( c ) {
        switch( c ) {
            case "amp": return "&";
            case "quot": return '"';
            case "gt": return ">";
            case "lt": return "<";
        }
        var m = c.match( /^#(\d+)$/ );
        if( m && defined( m[ 1 ] ) )
            return String.fromCharCode( m[ 1 ] );
        m = c.match( /^#x([0-9a-f]+)$/i );
        if(  m && defined( m[ 1 ] ) )
            return String.fromCharCode( parseInt( hex, m[ 1 ] ) );
        return c;
    }
} );


String.prototype.extend( {
    escapeJS: function() {
        return this.replace( /([^ -!#-\[\]-~])/g, function( m, c ) { return String.escapeJSChar( c ); } )
    },
    
    
    escapeJS2: function() {
        return this.replace( /([\u0000-\u0031'"\\])/g, function( m, c ) { return String.escapeJSChar( c ); } )
    },
    
    
    escapeJS3: function() {
        return this.replace( /[\u0000-\u0031'"\\]/g, function( m ) { return String.escapeJSChar( m ); } )
    },
    
    
    escapeJS4: function() {
        return this.replace( /./g, function( m ) { return String.escapeJSChar( m ); } )
    },
    
    
    encodeHTML: function() {
        return this.replace( /([<>&"])/g, function( m, c ) { return String.encodeEntity( c ) } );
    },


    decodeHTML: function() {
        return this.replace( /&(.*?);/g, function( m, c ) { return String.decodeEntity( c ) } );
    },
    
    
    cssToJS: function() {
        return this.replace( /-([a-z])/g, function( m, c ) { return c.toUpperCase() } );
    },
    
    
    jsToCSS: function() {
        return this.replace( /([A-Z])/g, function( m, c ) { return "-" + c.toLowerCase() } );
    },
    
    
    firstToLowerCase: function() {
        return this.replace( /^(.)/, function( m, c ) { return c.toLowerCase() } );
    },
    
        
    rgbToHex: function() {
        var c = this.match( /(\d+)\D+(\d+)\D+(\d+)/ );
        if( !c )
            return undefined;
        return "#" +
            finiteInt( c[ 1 ] ).toString( 16 ).pad( 2, "0" ) +
            finiteInt( c[ 2 ] ).toString( 16 ).pad( 2, "0" ) +
            finiteInt( c[ 3 ] ).toString( 16 ).pad( 2, "0" );
    },
    
    
    pad: function( length, padChar ) {
        var padding = length - this.length;
        if( padding <= 0 )
            return this;
        if( !defined( padChar ) )
            padChar = " ";
        var out = [];
        for( var i = 0; i < padding; i++ )
            out.push( padChar );
        out.push( this );
        return out.join( "" );
    },


    trim: function() {
        return this.replace( /^\s+|\s+$/g, "" );
    }

} );


/* extend array object */

Array.extend( { 
    fromPseudo: function ( args ) {
        var out = [];
        for ( var i = 0; i < args.length; i++ )
            out.push( args[ i ] );
        return out;
    }
});


/* extend array object */

Array.prototype.extend( {
    copy: function() {
        var out = [];
        for( var i = 0; i < this.length; i++ )
            out[ i ] = this[ i ];
        return out;
    },


    first: function( callback, object ) {
        var length = this.length;
        for( var i = 0; i < length; i++ ) {
            var result = object
                ? callback.call( object, this[ i ], i, this )
                : callback( this[ i ], i, this );
            if( result )
                return this[ i ];
        }
        return null;
    },


    fitIndex: function( fromIndex, defaultIndex ) {
        if( !defined( fromIndex ) || fromIndex == null )
            fromIndex = defaultIndex;
        else if( fromIndex < 0 ) {
            fromIndex = this.length + fromIndex;
            if( fromIndex < 0 )
                fromIndex = 0;
        } else if( fromIndex >= this.length )
            fromIndex = this.length - 1;
        return fromIndex;
    },


    scramble: function() {
        for( var i = 0; i < this.length; i++ ) {
            var j = Math.floor( Math.random() * this.length );
            var temp = this[ i ];
            this[ i ] = this[ j ];
            this[ j ] = temp;
        }
    },
    
    
    add: function() {
        var a = arguments;
        for( var i = 0; i < a.length; i++ ) {
            var index = this.indexOf( a[ i ] );
            if( index < 0 ) 
                this.push( arguments[ i ] );
        }
        return this.length;
    },
        
    
    remove: function() {
        var a = arguments;
        for( var i = 0; i < a.length; i++ ) {
            var j = this.indexOf( a[ i ] );
            if( j >= 0 )
                this.splice( j, 1 );
        }
        return this.length;
    },


    /* javascript 1.5 array methods */
    /* http://developer-test.mozilla.org/en/docs/Core_JavaScript_1.5_Reference:Objects:Array#Methods */

    every: function( callback, object ) {
        var length = this.length;
        for( var i = 0; i < length; i++ ) {
            var result = object
                ? callback.call( object, this[ i ], i, this )
                : callback( this[ i ], i, this );
            if( !result )
                return false;
        }
        return true;
    },


    filter: function( callback, object ) {
        var out = [];
        var length = this.length;
        for( var i = 0; i < length; i++ ) {
            var result = object
                ? callback.call( object, this[ i ], i, this )
                : callback( this[ i ], i, this );
            if( result )
                out.push( this[ i ] );
        }
        return out;
    },
    
    
    forEach: function( callback, object ) {
        var length = this.length;
        for( var i = 0; i < length; i++ ) {
            object
                ? callback.call( object, this[ i ], i, this )
                : callback( this[ i ], i, this );
        }
    },
    
    
    indexOf: function( value, fromIndex ) {
        fromIndex = this.fitIndex( fromIndex, 0 );
        for( var i = 0; i < this.length; i++ ) {
            if( this[ i ] === value )
                return i; 
        }
        return -1;
    },


    lastIndexOf: function( value, fromIndex ) {
        fromIndex = this.fitIndex( fromIndex, this.length - 1 );
        for( var i = fromIndex; i >= 0; i-- ) {
            if( this[ i ] == value )
                return i;
        }
        return -1;
    },


    some: function( callback, object ) {
        var length = this.length;
        for( var i = 0; i < length; i++ ) {
            var result = object
                ? callback.call( object, this[ i ], i, this )
                : callback( this[ i ], i, this );
            if( result )
                return true;
        }
        return false;
    },


    /* javascript 1.2 array methods */

    concat: function() {
        var a = arguments;
        var out = this.copy();
        for( i = 0; i < a.length; i++ ) {
            var b = a[ i ];
            for( j = 0; j < b.length; j++ )
                out.push( b[ j ] );
        }
        return out;
    },
    

    push: function() {
        var a = arguments;
        for( var i = 0; i < a.length; i++ )
            this[ this.length ] = a[ i ];
        return this.length;     
    },


    pop: function() {
        if( this.length == 0 )
            return undefined;
        var out = this[ this.length - 1 ];
        this.length--;
        return out;
    },
    
    
    unshift: function() {
        var a = arguments;
        for( var i = 0; i < a.length; i++ ) {
            this[ i + a.length ] = this[ i ];
            this[ i ] = a[ i ];
        }
        return this.length;     
    },
    
    
    shift: function() {
        if( this.length == 0 )
            return undefined;
        var out = this[ 0 ];
        for( var i = 1; i < this.length; i++ )
            this[ i - 1 ] = this[ i ];
        this.length--;
        return out;
    }
} );


/* date extensions */

Date.extend( {
    /*  iso 8601 date format parser
        this was fun to write...
        thanks to: http://www.cl.cam.ac.uk/~mgk25/iso-time.html */

    matchISOString: new RegExp(
        "^([0-9]{4})" +                                                     // year
        "(?:-(?=0[1-9]|1[0-2])|$)(..)?" +                                   // month
        "(?:-(?=0[1-9]|[12][0-9]|3[01])|$)([0-9]{2})?" +                    // day of the month
        "(?:T(?=[01][0-9]|2[0-4])|$)T?([0-9]{2})?" +                        // hours
        "(?::(?=[0-5][0-9])|\\+|-|Z|$)([0-9]{2})?" +                        // minutes
        "(?::(?=[0-5][0-9]|60$|60[+|-|Z]|60.0+)|\\+|-|Z|$):?([0-9]{2})?" +  // seconds
        "(\.[0-9]+)?" +                                                     // fractional seconds
        "(Z|\\+[01][0-9]|\\+2[0-4]|-[01][0-9]|-2[0-4])?" +                  // timezone hours
        ":?([0-5][0-9]|60)?$"                                               // timezone minutes
    ),
    
    
    fromISOString: function( string ) {
        var t = this.matchISOString.exec( string );
        if( !t )
            return undefined;

        var year = finiteInt( t[ 1 ], 10 );
        var month = finiteInt( t[ 2 ], 10 ) - 1;
        var day = finiteInt( t[ 3 ], 10 );
        var hours = finiteInt( t[ 4 ], 10 );
        var minutes = finiteInt( t[ 5 ], 10 );
        var seconds = finiteInt( t[ 6 ], 10 );
        var milliseconds = finiteInt( Math.round( parseFloat( t[ 7 ] ) * 1000 ) );
        var tzHours = finiteInt( t[ 8 ], 10 );
        var tzMinutes = finiteInt( t[ 9 ], 10 );

        var date = new this( 0 );
        if( defined( t[ 8 ] ) ) {
            date.setUTCFullYear( year, month, day );
            date.setUTCHours( hours, minutes, seconds, milliseconds );
            var offset = (tzHours * 60 + tzMinutes) * 60000;
            if( offset )
                date = new this( date - offset );
        } else {
            date.setFullYear( year, month, day );
            date.setHours( hours, minutes, seconds, milliseconds );
        }

        return date;
    }
} );


Date.prototype.extend( {
    getISOTimezoneOffset: function() {
        var offset = -this.getTimezoneOffset();
        var negative = false;
        if( offset < 0 ) {
            negative = true;
            offset *= -1;
        }
        var offsetHours = Math.floor( offset / 60 ).toString().pad( 2, "0" );
        var offsetMinutes = Math.floor( offset % 60 ).toString().pad( 2, "0" );
        return (negative ? "-" : "+") + offsetHours + ":" + offsetMinutes;
    },


    toISODateString: function() {
        var year = this.getFullYear();
        var month = (this.getMonth() + 1).toString().pad( 2, "0" );
        var day = this.getDate().toString().pad( 2, "0" );
        return year + "-" + month + "-" + day;
    },


    toUTCISODateString: function() {
        var year = this.getUTCFullYear();
        var month = (this.getUTCMonth() + 1).toString().pad( 2, "0" );
        var day = this.getUTCDate().toString().pad( 2, "0" );
        return year + "-" + month + "-" + day;
    },


    toISOTimeString: function() {
        var hours = this.getHours().toString().pad( 2, "0" );
        var minutes = this.getMinutes().toString().pad( 2, "0" );
        var seconds = this.getSeconds().toString().pad( 2, "0" );
        var milliseconds = this.getMilliseconds().toString().pad( 3, "0" );
        var timezone = this.getISOTimezoneOffset();
        return hours + ":" + minutes + ":" + seconds + "." + milliseconds + timezone;
    },


    toUTCISOTimeString: function() {
        var hours = this.getUTCHours().toString().pad( 2, "0" );
        var minutes = this.getUTCMinutes().toString().pad( 2, "0" );
        var seconds = this.getUTCSeconds().toString().pad( 2, "0" );
        var milliseconds = this.getUTCMilliseconds().toString().pad( 3, "0" );
        return hours + ":" + minutes + ":" + seconds + "." + milliseconds + "Z";
    },


    toISOString: function() {
        return this.toISODateString() + "T" + this.toISOTimeString();
    },


    toUTCISOString: function() {
        return this.toUTCISODateString() + "T" + this.toUTCISOTimeString();
    }
} );


/* ajax */

if( !defined( window.XMLHttpRequest ) ) {
    window.XMLHttpRequest = function() {
        var types = [
            "Microsoft.XMLHTTP",
            "MSXML2.XMLHTTP.5.0",
            "MSXML2.XMLHTTP.4.0",
            "MSXML2.XMLHTTP.3.0",
            "MSXML2.XMLHTTP"
        ];
        
        for( var i = 0; i < types.length; i++ ) {
            try {
                return new ActiveXObject( types[ i ] );
            } catch( e ) {}
        }
        
        return undefined;
    }
}
/*
DOM Library - Copyright 2005 Six Apart
$Id: dom.js 86 2006-12-04 21:35:07Z henrylyne $

Copyright (c) 2005, Six Apart, Ltd.
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    * Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following disclaimer
in the documentation and/or other materials provided with the
distribution.

    * Neither the name of "Six Apart" nor the names of its
contributors may be used to endorse or promote products derived from
this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/


/* Node class */

if( !defined( window.Node ) )
    Node = {};

try {
    Node.extend( {
        ELEMENT_NODE: 1,
        ATTRIBUTE_NODE: 2,
        TEXT_NODE: 3,
        CDATA_SECTION_NODE: 4,  
        COMMENT_NODE: 8,    
        DOCUMENT_NODE: 9,
        DOCUMENT_FRAGMENT_NODE: 11
    } );
} catch( e ) {}


/* DOM class */

if( !defined( window.DOM ) )
    DOM = {};


DOM.extend( {
    getElement: function( e ) {
        return (typeof e == "string" || typeof e == "number") ? document.getElementById( e ) : e;
    },


    addEventListener: function( e, eventName, func, useCapture ) {
        try {
            if( e.addEventListener )
                e.addEventListener( eventName, func, useCapture );
            else if( e.attachEvent )
                e.attachEvent( "on" + eventName, func );
            else
                e[ "on" + eventName ] = func;
        } catch( e ) {}
    },


    removeEventListener: function( e, eventName, func, useCapture ) {
        try {
            if( e.removeEventListener )
                e.removeEventListener( eventName, func, useCapture );
            else if( e.detachEvent )
                e.detachEvent( "on" + eventName, func );
            else
                e[ "on" + eventName ] = undefined;
        } catch( e ) {}
    },
    
    
    focus: function( e ) {
        try {
            e = DOM.getElement( e );
            e.focus();
        } catch( e ) {}
    },


    blur: function( e ) {
        try {
            e = DOM.getElement( e );
            e.blur();
        } catch( e ) {}
    },
    

    /* style */
    
    getComputedStyle: function( e ) {
        if( e.currentStyle )
            return e.currentStyle;
        var style = {};
        var owner = DOM.getOwnerDocument( e );
        if( owner && owner.defaultView && owner.defaultView.getComputedStyle ) {            
            try {
                style = owner.defaultView.getComputedStyle( e, null );
            } catch( e ) {}
        }
        return style;
    },


    getStyle: function( e, p ) {
        var s = DOM.getComputedStyle( e );
        return s[ p ];
    },


    // given a window (or defaulting to current window), returns
    // object with .x and .y of client's usable area
    getClientDimensions: function( w ) {
        if( !w )
            w = window;

        var d = {};

        // most browsers
        if( w.innerHeight ) {
            d.x = w.innerWidth;
            d.y = w.innerHeight;
            return d;
        }

        // IE6, strict
        var de = w.document.documentElement;
        if( de && de.clientHeight ) {
            d.x = de.clientWidth;
            d.y = de.clientHeight;
            return d;
        }

        // IE, misc
        if( document.body ) {
            d.x = document.body.clientWidth;
            d.y = document.body.clientHeight;
            return d;
        }
        
        return undefined;
    },


    getDimensions: function( e ) {
        if( !e )
            return undefined;

        var style = DOM.getComputedStyle( e );

        return {
            offsetLeft: e.offsetLeft,
            offsetTop: e.offsetTop,
            offsetWidth: e.offsetWidth,
            offsetHeight: e.offsetHeight,
            clientWidth: e.clientWidth,
            clientHeight: e.clientHeight,
            
            offsetRight: e.offsetLeft + e.offsetWidth,
            offsetBottom: e.offsetTop + e.offsetHeight,
            clientLeft: finiteInt( style.borderLeftWidth ) + finiteInt( style.paddingLeft ),
            clientTop: finiteInt( style.borderTopWidth ) + finiteInt( style.paddingTop ),
            clientRight: e.clientLeft + e.clientWidth,
            clientBottom: e.clientTop + e.clientHeight
        };
    },


    getAbsoluteDimensions: function( e ) {
        var d = DOM.getDimensions( e );
        if( !d )
            return d;
        d.absoluteLeft = d.offsetLeft;
        d.absoluteTop = d.offsetTop;
        d.absoluteRight = d.offsetRight;
        d.absoluteBottom = d.offsetBottom;
        var bork = 0;
        while( e ) {
            try { // IE 6 sometimes gives an unwarranted error ("htmlfile: Unspecified error").
                e = e.offsetParent;
            } catch ( err ) {
                log( "In DOM.getAbsoluteDimensions: " + err.message ); 
                if ( ++bork > 25 )
                    return null;
            }
            if( !e )
                return d;
            d.absoluteLeft += e.offsetLeft;
            d.absoluteTop += e.offsetTop;
            d.absoluteRight += e.offsetLeft;
            d.absoluteBottom += e.offsetTop;
        }
        return d;
    },
    
    
    getIframeAbsoluteDimensions: function( e ) {
        var d = DOM.getAbsoluteDimensions( e );
        if( !d )
            return d;
        var iframe = DOM.getOwnerIframe( e );
        if( !defined( iframe ) )
            return d;
        
        var d2 = DOM.getIframeAbsoluteDimensions( iframe );
        var scroll = DOM.getWindowScroll( iframe.contentWindow );
        var left = d2.absoluteLeft - scroll.left;
        var top = d2.absoluteTop - scroll.top;
        
        d.absoluteLeft += left;
        d.absoluteTop += top;
        d.absoluteRight += left;
        d.absoluteBottom += top;
        
        return d;
    },
    
    
    setLeft: function( e, v ) { e.style.left = finiteInt( v ) + "px"; },
    setTop: function( e, v ) { e.style.top = finiteInt( v ) + "px"; },
    setRight: function( e, v ) { e.style.right = finiteInt( v ) + "px"; },
    setBottom: function( e, v ) { e.style.bottom = finiteInt( v ) + "px"; },
    setWidth: function( e, v ) { e.style.width = max( 0, finiteInt( v ) ) + "px"; },
    setHeight: function( e, v ) { e.style.height = max( 0, finiteInt( v ) ) + "px"; },
    setZIndex: function( e, v ) { e.style.zIndex = finiteInt( v ); },

    
    getWindowScroll: function( w ) {
        var s = {
            left: 0,
            top: 0
        };
        
        var d = w.document;
        
        // ie
        var de = d.documentElement;
        if( de && defined( de.scrollLeft ) ) {
            s.left = de.scrollLeft;
            s.top = de.scrollTop;
        }
        
        // safari
        else if( defined( w.scrollX ) ) {
            s.left = w.scrollX;
            s.top = w.scrollY;
        }
        
        // opera
        else if( d.body && defined( d.body.scrollLeft ) ) {
            s.left = d.body.scrollLeft;
            s.top = d.body.scrollTop;
        }
        
        return s;
    },
    
    
    getAbsoluteCursorPosition: function( event ) {
        event = event || window.event;
        var s = DOM.getWindowScroll( window );
        return {
            x: s.left + event.clientX,
            y: s.top + event.clientY
        };
    },
    
    
    invisibleStyle: {
        display: "block",
        position: "absolute",
        left: 0,
        top: 0,
        width: 0,
        height: 0,
        margin: 0,
        border: 0,
        padding: 0,
        fontSize: "0.1px",
        lineHeight: 0,
        opacity: 0,
        MozOpacity: 0,
        filter: "alpha(opacity=0)"
    },
    
    
    makeInvisible: function( e ) {
        for( var p in this.invisibleStyle ) {
            if( this.invisibleStyle.hasOwnProperty( p ) )
                e.style[ p ] = this.invisibleStyle[ p ];
        }
    },


    /* text and selection related methods */

    mergeTextNodes: function( n ) {
        var c = 0;
        while( n ) {
            if( n.nodeType == Node.TEXT_NODE && n.nextSibling && n.nextSibling.nodeType == Node.TEXT_NODE ) {
                n.nodeValue += n.nextSibling.nodeValue;
                n.parentNode.removeChild( n.nextSibling );
                c++;
            } else {
                if( n.firstChild )
                    c += DOM.mergeTextNodes( n.firstChild );
                n = n.nextSibling;
            }
        }
        return c;
    },
    
    
    selectElement: function( e ) {  
        var d = e.ownerDocument;  
        
        // internet explorer  
        if( d.body.createControlRange ) {  
            var r = d.body.createControlRange();  
            r.addElement( e );  
            r.select();  
        }  
    }, 
    
    
    /* dom methods */
    
    isImmutable: function( n ) {
        try {
            if( n.getAttribute( "contenteditable" ) == "false" )
                return true;
        } catch( e ) {}
        return false;
    },
    
    
    getImmutable: function( n ) {
        var immutable = null;
        while( n ) {
            if( DOM.isImmutable( n ) )
                immutable = n;
            n = n.parentNode;
        }
        return immutable;
    },


    getOwnerDocument: function( n ) {
        if( !n )
            return document;
        if( n.ownerDocument )
            return n.ownerDocument;
        if( n.getElementById )
            return n;
        return document;
    },


    getOwnerWindow: function( n ) {
        if( !n )
            return window;
        if( n.parentWindow )
            return n.parentWindow;
        var doc = DOM.getOwnerDocument( n );
        if( doc && doc.defaultView )
            return doc.defaultView;
        return window;
    },
    
    
    getOwnerIframe: function( n ) {
        if( !n )
            return undefined;
        var nw = DOM.getOwnerWindow( n );
        var nd = DOM.getOwnerDocument( n );
        var pw = nw.parent || nw.parentWindow;
        if( !pw )
            return undefined;
        var parentDocument = pw.document;
        var es = parentDocument.getElementsByTagName( "iframe" );
        for( var i = 0; i < es.length; i++ ) {
            var e = es[ i ];
            try {
                var d = e.contentDocument || e.contentWindow.document;
                if( d === nd )
                    return e;
            }catch(err) {};
        }
        return undefined;
    },


    filterElementsByClassName: function( es, className ) {
        var filtered = [];
        for( var i = 0; i < es.length; i++ ) {
            var e = es[ i ];
            if( DOM.hasClassName( e, className ) )
                filtered[ filtered.length ] = e;
        }
        return filtered;
    },
    
    
    filterElementsByAttribute: function( es, attr ) {
        if( !es )
            return [];
        if( !defined( attr ) || attr == null || attr == "" )
            return es;
        var filtered = [];
        for( var i = 0; i < es.length; i++ ) {
            var element = es[ i ];
            if( !element )
                continue;
            if( element.getAttribute && ( element.getAttribute( attr ) ) )
                filtered[ filtered.length ] = element;
        }
        return filtered;
    },


    filterElementsByTagName: function( es, tagName ) {
        if( tagName == "*" )
            return es;
        var filtered = [];
        tagName = tagName.toLowerCase();
        for( var i = 0; i < es.length; i++ ) {
            var e = es[ i ];
            if( e.tagName && e.tagName.toLowerCase() == tagName )
                filtered[ filtered.length ] = e;
        }
        return filtered;
    },


    getElementsByTagAndAttribute: function( root, tagName, attr ) {
        if( !root )
            root = document;
        var es = root.getElementsByTagName( tagName );
        return DOM.filterElementsByAttribute( es, attr );
    },
    
    
    getElementsByAttribute: function( root, attr ) {
        return DOM.getElementsByTagAndAttribute( root, "*", attr );
    },


    getElementsByAttributeAndValue: function( root, attr, value ) {
        var es = DOM.getElementsByTagAndAttribute( root, "*", attr );
        var filtered = [];
        for ( var i = 0; i < es.length; i++ )
            if ( es[ i ].getAttribute( attr ) == value )
                filtered.push( es[ i ] );
        return filtered;
    },
    

    getElementsByTagAndClassName: function( root, tagName, className ) {
        if( !root )
            root = document;
        var elements = root.getElementsByTagName( tagName );
        return DOM.filterElementsByClassName( elements, className );
    },


    getElementsByClassName: function( root, className ) {
        return DOM.getElementsByTagAndClassName( root, "*", className );
    },


    getAncestors: function( n, includeSelf ) {
        if( !n )
            return [];
        var as = includeSelf ? [ n ] : [];
        n = n.parentNode;
        while( n ) {
            as.push( n );
            n = n.parentNode;
        }
        return as;
    },
    
    
    getAncestorsByTagName: function( n, tagName, includeSelf ) {
        var es = DOM.getAncestors( n, includeSelf );
        return DOM.filterElementsByTagName( es, tagName );
    },
    
    
    getFirstAncestorByTagName: function( n, tagName, includeSelf ) {
        return DOM.getAncestorsByTagName( n, tagName, includeSelf )[ 0 ];
    },


    getAncestorsByClassName: function( n, className, includeSelf ) {
        var es = DOM.getAncestors( n, includeSelf );
        return DOM.filterElementsByClassName( es, className );
    },


    getFirstAncestorByClassName: function( n, className, includeSelf ) {
        return DOM.getAncestorsByClassName( n, className, includeSelf )[ 0 ];
    },


    getAncestorsByTagAndClassName: function( n, tagName, className, includeSelf ) {
        var es = DOM.getAncestorsByTagName( n, tagName, includeSelf );
        return DOM.filterElementsByClassName( es, className );
    },


    getFirstAncestorByTagAndClassName: function( n, tagName, className, includeSelf ) {
        return DOM.getAncestorsByTagAndClassName( n, tagName, className, includeSelf )[ 0 ];
    },


    getPreviousElement: function( n ) {
        n = n.previousSibling;
        while( n ) {
            if( n.nodeType == Node.ELEMENT_NODE )
                return n;
            n = n.previousSibling;
        }
        return null;
    },


    getNextElement: function( n ) {
        n = n.nextSibling;
        while( n ) {
            if( n.nodeType == Node.ELEMENT_NODE )
                return n;
            n = n.nextSibling;
        }
        return null;
    },


    isInlineNode: function( n ) {
        // text nodes are inline
        if( n.nodeType == Node.TEXT_NODE )
            return n;

        // document nodes are non-inline
        if( n.nodeType == Node.DOCUMENT_NODE )
            return false;

        // all nonelement nodes are inline
        if( n.nodeType != Node.ELEMENT_NODE )
            return n;

        // br elements are not inline
        if( n.tagName && n.tagName.toLowerCase() == "br" )
            return false;

        // examine the style property of the inline n
        var display = DOM.getStyle( n, "display" ); 
        if( display && display.indexOf( "inline" ) >= 0 ) 
            return n;
    },
    
    
    isTextNode: function( n ) {
        if( n.nodeType == Node.TEXT_NODE )
            return n;
    },
    
    
    isInlineTextNode: function( n ) {
        if( n.nodeType == Node.TEXT_NODE )
            return n;
        if( !DOM.isInlineNode( n ) )
            return null;
    },


    /* this and the following classname functions honor w3c case-sensitive classnames */

    getClassNames: function( e ) {
        if( !e || !e.className )
            return [];
        return e.className.split( /\s+/g );
    },


    hasClassName: function( e, className ) {
        if( !e || !e.className )
            return false;
        var cs = DOM.getClassNames( e );
        for( var i = 0; i < cs.length; i++ ) {
            if( cs[ i ] == className )
                return true;
        }
        return false;
    },


    addClassName: function( e, className ) {
        if( !e || !className )
            return false;
        var cs = DOM.getClassNames( e );
        for( var i = 0; i < cs.length; i++ ) {
            if( cs[ i ] == className )
                return true;
        }
        cs.push( className );
        e.className = cs.join( " " );
        return false;
    },


    removeClassName: function( e, className ) {
        var r = false;
        if( !e || !e.className || !className )
            return r;
        var cs = (e.className && e.className.length)
            ? e.className.split( /\s+/g )
            : [];
        var ncs = [];
        for( var i = 0; i < cs.length; i++ ) {
            if( cs[ i ] == className ) {
                r = true;
                continue;
            }
            ncs.push( cs[ i ] );
        }
        if( r )
            e.className = ncs.join( " " );
        return r;
    },
    
    
    /* tree manipulation methods */
    
    replaceWithChildNodes: function( n ) {
        var firstChild = n.firstChild;
        var parentNode = n.parentNode;
        while( n.firstChild )
            parentNode.insertBefore( n.removeChild( n.firstChild ), n );
        parentNode.removeChild( n );
        return firstChild;
    },
    
    
    /* factory methods */
    
    createInvisibleInput: function( d ) {
        if( !d )
            d = window.document;
        var e = document.createElement( "input" );
        e.setAttribute( "autocomplete", "off" );
        e.autocomplete = "off";
        DOM.makeInvisible( e );
        return e;
    },


    getMouseEventAttribute: function( event, a ) {
        if( !a )
            return;
        var es = DOM.getAncestors( event.target, true );
        for( var i = 0; i < es.length; i++ ) {
            try {
                var e = es[ i ]
                var v = e.getAttribute ? e.getAttribute( a ) : null;
                if( v ) {
                    event.attributeElement = e;
                    event.attribute = v;
                    return v;
                }
            } catch( e ) {}
        }
    },
    

    setElementAttribute: function( e, a, v ) {
        /* safari workaround
         * safari's setAttribute assumes you want to use a namespace
         * when you have a colon in your attribute
         */
        if ( navigator.userAgent.toLowerCase().match(/webkit/) ) {
            var at = e.attributes;
            for ( var i = 0; i < at.length; i++ )
                if ( at[ i ].name == a )
                    return at[ i ].nodeValue = v;
        } else
            e.setAttribute( a, v );
    },


    swapAttributes: function( e, tg, at ) {
        var ar = e.getAttribute( tg );
        if( !ar )
            return false;
        
        /* clone the node with all children */
        if ( e.tagName.toLowerCase() == 'script' ) {
            /* only clone and replace script tags */
            var cl = e.cloneNode( true );
            if ( !cl )
                return false;

            DOM.setElementAttribute( cl, at, ar );
            cl.removeAttribute( tg );
        
            /* replace new, old */
            return e.parentNode.replaceChild( cl, e );
        } else {
            DOM.setElementAttribute( e, at, ar );
            e.removeAttribute( tg );
        }
    }
    
    
} );


$ = DOM.getElement;
var HTTPReq = new Object;

HTTPReq.create = function () {
    var xtr;
    var ex;

    if (typeof(XMLHttpRequest) != "undefined") {
        xtr = new XMLHttpRequest();
    } else {
        try {
            xtr = new ActiveXObject("Msxml2.XMLHTTP.4.0");
        } catch (ex) {
            try {
                xtr = new ActiveXObject("Msxml2.XMLHTTP");
            } catch (ex) {
            }
        }
    }

    // let me explain this.  Opera 8 does XMLHttpRequest, but not setRequestHeader.
    // no problem, we thought:  we'll test for setRequestHeader and if it's not present
    // then fall back to the old behavior (treat it as not working).  BUT --- IE6 won't
    // let you even test for setRequestHeader without throwing an exception (you need
    // to call .open on the .xtr first or something)
    try {
        if (xtr && ! xtr.setRequestHeader)
            xtr = null;
    } catch (ex) { }

    return xtr;
};

// opts:
// url, onError, onData, method (GET or POST), data
// url: where to get/post to
// onError: callback on error
// onData: callback on data received
// method: HTTP method, GET by default
// data: what to send to the server (urlencoded)
HTTPReq.getJSON = function (opts) {
    var req = HTTPReq.create();
    if (! req) {
        if (opts.onError) opts.onError("noxmlhttprequest");
        return;
    }

    var state_callback = function () {
        if (req.readyState != 4) return;

        if (req.status != 200) {
            if (opts.onError) opts.onError(req.status ? "status: " + req.status : "no data");
            return;
        }

        var resObj;
        var e;
        try {
            eval("resObj = " + req.responseText + ";");
        } catch (e) {
        }

        if (e || ! resObj) {
            if (opts.onError)
                opts.onError("Error parsing response: \"" + req.responseText + "\"");

            return;
        }

        if (opts.onData)
            opts.onData(resObj);
    };

    req.onreadystatechange = state_callback;

    var method = opts.method || "GET";
    var data = opts.data || null;

    var url = opts.url;
    if (opts.method == "GET" && opts.data) {
        url += url.match(/\?/) ? "&" : "?";
        url += opts.data
    }

    url += url.match(/\?/) ? "&" : "?";
    url += "_rand=" + Math.random();

    req.open(method, url, true);

    // we should send null unless we're in a POST
    var to_send = null;

    if (method.toUpperCase() == "POST") {
        req.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        to_send = data;
    }

    req.send(to_send);
};

HTTPReq.formEncoded = function (vars) {
    var enc = [];
    var e;
    for (var key in vars) {
        try {
            if (!vars.hasOwnProperty(key))
                continue;
            enc.push(encodeURIComponent(key) + "=" + encodeURIComponent(vars[key]));
        } catch( e ) {}
    }
    return enc.join("&");

};

// This file contains general-purpose LJ code

var LiveJournal = new Object;

// The hook mappings
LiveJournal.hooks = {};

LiveJournal.register_hook = function (hook, func) {
    if (! LiveJournal.hooks[hook])
        LiveJournal.hooks[hook] = [];

    LiveJournal.hooks[hook].push(func);
};

// args: hook, params to pass to hook
LiveJournal.run_hook = function () {
    var a = arguments;

    var hookfuncs = LiveJournal.hooks[a[0]];
    if (!hookfuncs || !hookfuncs.length) return;

    var hookargs = [];

    for (var i = 1; i < a.length; i++) {
        hookargs.push(a[i]);
    }

    var rv = null;

    hookfuncs.forEach(function (hookfunc) {
        rv = hookfunc.apply(null, hookargs);
    });

    return rv;
};

LiveJournal.pageLoaded = false;

LiveJournal.initPage = function () {
    // only run once
    if (LiveJournal.pageLoaded)
        return;
    LiveJournal.pageLoaded = 1;

    // set up various handlers for every page
    LiveJournal.initPlaceholders();
    LiveJournal.initLabels();
    LiveJournal.initInboxUpdate();
    LiveJournal.initAds();
    LiveJournal.initPolls();

    // run other hooks
    LiveJournal.run_hook("page_load");
};

// Set up two different ways to test if the page is loaded yet.
// The proper way is using DOMContentLoaded, but only Mozilla supports it.
{
    // Others
    DOM.addEventListener(window, "load", LiveJournal.initPage);

    // Mozilla
    DOM.addEventListener(window, "DOMContentLoaded", LiveJournal.initPage);
}

// Set up a timer to keep the inbox count updated
LiveJournal.initInboxUpdate = function () {
    // Don't run if not logged in or this is disabled
    if (! Site || ! Site.has_remote || ! Site.inbox_update_poll) return;

    // Don't run if no inbox count
    var unread = $("LJ_Inbox_Unread_Count");
    if (! unread) return;

    // Update every five minutes
    window.setInterval(LiveJournal.updateInbox, 1000 * 60 * 5);
};

// Do AJAX request to find the number of unread items in the inbox
LiveJournal.updateInbox = function () {
    var postData = {
        "action": "get_unread_items"
    };

    var opts = {
        "data": HTTPReq.formEncoded(postData),
        "method": "POST",
        "onData": LiveJournal.gotInboxUpdate
    };

    opts.url = Site.currentJournal ? "/" + Site.currentJournal + "/__rpc_esn_inbox" : "/__rpc_esn_inbox";

    HTTPReq.getJSON(opts);
};

// We received the number of unread inbox items from the server
LiveJournal.gotInboxUpdate = function (resp) {
    if (! resp || resp.error) return;

    var unread = $("LJ_Inbox_Unread_Count");
    if (! unread) return;

    unread.innerHTML = resp.unread_count ? "  (" + resp.unread_count + ")" : "";
};

// Search for placeholders and initialize them
LiveJournal.initPlaceholders = function () {
    var placeholders = DOM.getElementsByTagAndClassName(document, "img", "LJ_Placeholder") || [];

    Array.prototype.forEach.call(placeholders, function (placeholder) {
        var parent = DOM.getFirstAncestorByClassName(placeholder, "LJ_Placeholder_Container", false);
        if (!parent) return;

        var container = DOM.filterElementsByClassName(parent.getElementsByTagName("div"), "LJ_Container")[0];
        if (!container) return;

        var html = DOM.filterElementsByClassName(parent.getElementsByTagName("div"), "LJ_Placeholder_HTML")[0];
        if (!html) return;

        var placeholder_html = unescape(html.innerHTML);

        var placeholderClickHandler = function (e) {
            Event.stop(e);
            // have to wrap placeholder_html in another block, IE is weird
            container.innerHTML = "<span>" + placeholder_html + "</span>";
            DOM.makeInvisible(placeholder);
        };

        DOM.addEventListener(placeholder, "click", placeholderClickHandler);

        return false;
    });
};

// set up labels for Safari
LiveJournal.initLabels = function () {
    // disabled because new webkit has labels that work
    return;

    // safari doesn't know what <label> tags are, lets fix them
    if (navigator.userAgent.indexOf('Safari') == -1) return;

    // get all labels
    var labels = document.getElementsByTagName("label");

    for (var i = 0; i < labels.length; i++) {
        DOM.addEventListener(labels[i], "click", LiveJournal.labelClickHandler);
    }
};

LiveJournal.labelClickHandler = function (evt) {
    Event.prep(evt);

    var label = DOM.getAncestorsByTagName(evt.target, "label", true)[0];
    if (! label) return;

    var targetId = label.getAttribute("for");
    if (! targetId) return;

    var target = $(targetId);
    if (! target) return;

    target.click();

    return false;
};

// change drsc to src for ads
LiveJournal.initAds = function () {
    AdEngine.init();
};

// handy utilities to create elements with just text in them
function _textSpan () { return _textElements("span", arguments); }
function _textDiv  () { return _textElements("div", arguments);  }

function _textElements (eleType, txts) {
    var ele = [];
    for (var i = 0; i < txts.length; i++) {
        var node = document.createElement(eleType);
        node.innerHTML = txts[i];
        ele.push(node);
    }

    return ele.length == 1 ? ele[0] : ele;
};

LiveJournal.initPolls = function () {
    var pollLinks = DOM.getElementsByTagAndClassName(document, 'a', "LJ_PollAnswerLink") || [];  

    // attach click handlers to each answer link
    Array.prototype.forEach.call(pollLinks, function (pollLink) {
        DOM.addEventListener(pollLink, "click", LiveJournal.pollAnswerLinkClicked.bindEventListener(pollLink));
    });
};

// invocant is the pollLink from above
LiveJournal.pollAnswerLinkClicked = function (e) {
    Event.stop(e);

    if (! this || ! this.tagName || this.tagName.toLowerCase() != "a")
    return true;

    var pollid = this.getAttribute("lj_pollid");
    if (! pollid) return true;

    var pollqid = this.getAttribute("lj_qid");
    if (! pollqid) return true;

    var action = "get_answers";

    // Do ajax request to replace the link with the answers
    var params = {
        "pollid" : pollid,
        "pollqid": pollqid,
        "action" : action
    };

    var opts = {
        "url"    : LiveJournal.getAjaxUrl("poll"),
        "method" : "POST",
        "data"   : HTTPReq.formEncoded(params),
        "onData" : LiveJournal.pollAnswersReceived,
        "onError": LiveJournal.ajaxError
    };

    HTTPReq.getJSON(opts);
    this.innerHTML = "<div class='lj_pollanswer_loading'>Loading...</div>";

    return false;
};

LiveJournal.pollAnswersReceived = function (answers) {
    if (! answers) return false;
    if (answers.error) return LiveJournal.ajaxError(answers.error);

    var pollid = answers.pollid;
    var pollqid = answers.pollqid;
    if (! pollid || ! pollqid) return false;

    var linkEle = $("LJ_PollAnswerLink_" + pollid + "_" + pollqid);
    if (! linkEle) return false;

    var answerEle = document.createElement("div");
    DOM.addClassName(answerEle, "lj_pollanswer");
    answerEle.innerHTML = answers.answer_html ? answers.answer_html : "(No answers)";

    linkEle.parentNode.insertBefore(answerEle, linkEle);
    linkEle.parentNode.removeChild(linkEle);
};


// gets a url for doing ajax requests
LiveJournal.getAjaxUrl = function (action) {
    // if we are on a journal subdomain then our url will be
    // /journalname/__rpc_action instead of /__rpc_action
    return Site.currentJournal
        ? "/" + Site.currentJournal + "/__rpc_" + action
        : "/__rpc_" + action;
};

// generic handler for ajax errors
LiveJournal.ajaxError = function (err) {
    if (LJ_IPPU) {
        LJ_IPPU.showNote("Error: " + err);
    } else {
        alert("Error: " + err);
    }
};

// utility method to get all items on the page with a certain class name
LiveJournal.getDocumentElementsByClassName = function (className) {
  var domObjects = document.getElementsByTagName("*");
  var items = DOM.filterElementsByClassName(domObjects, className) || [];

  return items;
};

// utility method to add an onclick callback on all items with a classname
LiveJournal.addClickHandlerToElementsWithClassName = function (callback, className) {
  var items = LiveJournal.getDocumentElementsByClassName(className);

  items.forEach(function (item) {
    DOM.addEventListener(item, "click", callback);
  })
};

LiveJournal.insertAdsMulti = function (params) {
  var i = 0;
  var containers = [];

  for (i = 0; i < params.length; i++) {
    if (! params[i].html || params[i].html == "<ul>\n</ul>") continue;
    AdEngine.insertAdResponse( params[i] );
    containers.push(document.getElementById(params[i].id));
  }

    // add the ad box style to the containers
    containers.forEach(function (container) {
      if (! container) return;

      DOM.addClassName(container.parentNode, "lj_content_ad");
      DOM.removeClassName(container.parentNode, "lj_inactive_ad");
    });
};

// given a URL, parse out the GET args and return them in a hash
LiveJournal.parseGetArgs = function (url) {
    var getArgsHash = {};

    var urlParts = url.split("?");
    if (!urlParts[1]) return getArgsHash;
    var getArgs = urlParts[1].split("&");
    for (var arg in getArgs) {
        if (!getArgs.hasOwnProperty(arg)) continue;
        var pair = getArgs[arg].split("=");
        getArgsHash[pair[0]] = pair[1];
    }

    return getArgsHash;
};
/* 
AdEngine
$Id: AdEngine.js 226 2007-09-18 18:20:57Z janine $

Copyright (c) 2006, Six Apart, Ltd.
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    * Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following disclaimer
in the documentation and/or other materials provided with the
distribution.

    * Neither the name of "Six Apart" nor the names of its
contributors may be used to endorse or promote products derived from
this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

AdEngine = {

    /* called from window.onload ONLY on the logged out blog side (view) */
    init: function() {
        var es = document.getElementsByTagName( "script" );
        for ( var i = 0; i < es.length; i++ ) {
            var ar = es[ i ].getAttribute( "defersrc" );
            if( !ar )
                continue;
        
            var cl = es[ i ].cloneNode( true );
            if ( !cl )
                continue;

            cl.setAttribute( "src", ar );
            cl.removeAttribute( "defersrc" );
        
            /* replace new, old */
            try {
                es[ i ].parentNode.replaceChild( cl, es[ i ] );
            } catch (e) {}
        }
    },
    
    
    insertAdResponse: function( params ) {
        var e = document.getElementById( params.id );
        if( !e )
            return;
        if( params.html ) {
            var e2 = document.createElement( "div" );
            e2.innerHTML = params.html;
            e.innerHTML = ""; // clear old content
            e.appendChild( e2 );
        }
        if( params.js )
            return eval( "(" + params.js + ")" );
    },

    insertAdsMulti: function( params ) {
        var i = 0;
        for (i = 0; i < params.length; i++) {
            AdEngine.insertAdResponse( params[i] );
        }
    }

};
var ESN = new Object();

LiveJournal.register_hook("page_load", function () {
  ESN.initCheckAllBtns();
  ESN.initEventCheckBtns();
  ESN.initTrackBtns();
});

// When page loads, set up "check all" checkboxes
ESN.initCheckAllBtns = function () {
  var ntids  = $("ntypeids");
  var catids = $("catids");

  if (!ntids || !catids)
    return;

  ntidList  = ntids.value;
  catidList = catids.value;

  if (!ntidList || !catidList)
    return;

  ntids  = ntidList.split(",");
  catids = catidList.split(",");

  catids.forEach( function (catid) {
    ntids.forEach( function (ntypeid) {
      var className = "SubscribeCheckbox-" + catid + "-" + ntypeid;

      var cab = new CheckallButton();
      cab.init({
        "class": className,
          "button": $("CheckAll-" + catid + "-" + ntypeid),
          "parent": $("CategoryRow-" + catid)
          });
    });
  });
}

// set up auto show/hiding of notification methods
ESN.initEventCheckBtns = function () {
  var viewObjects = document.getElementsByTagName("*");
  var boxes = DOM.filterElementsByClassName(viewObjects, "SubscriptionInboxCheck") || [];

  boxes.forEach( function (box) {
    DOM.addEventListener(box, "click", ESN.eventChecked.bindEventListener());
  });
}

ESN.eventChecked = function (evt) {
  var target = evt.target;
  if (!target)
    return;

  var parentRow = DOM.getFirstAncestorByTagName(target, "tr", false);

  var viewObjects = parentRow.getElementsByTagName("*");
  var boxes = DOM.filterElementsByClassName(viewObjects, "NotificationOptions") || [];

  boxes.forEach( function (box) {
    box.style.visibility = target.checked ? "visible" : "hidden";
  });
}

// attach event handlers to all track buttons
ESN.initTrackBtns = function () {
    // don't do anything if no remote
    if (!Site || !Site.has_remote) return;

    // attach to all ljuser head icons
    var trackBtns = DOM.getElementsByTagAndClassName(document, "img", "TrackButton");

    Array.prototype.forEach.call(trackBtns, function (trackBtn) {
        if (!trackBtn || !trackBtn.getAttribute) return;

        if (!trackBtn.getAttribute("lj_subid") && !trackBtn.getAttribute("lj_journalid")) return;

        DOM.addEventListener(trackBtn, "click",
                             ESN.trackBtnClickHandler.bindEventListener(trackBtn));
    });
};

ESN.trackBtnClickHandler = function (evt) {
    var trackBtn = evt.target;
    if (! trackBtn || trackBtn.tagName.toLowerCase() != "img") return true;

    Event.stop(evt);

    var btnInfo = {};

    ['arg1', 'arg2', 'etypeid', 'newentry_etypeid', 'newentry_token', 'newentry_subid',
     'journalid', 'subid', 'auth_token'].forEach(function (arg) {
        btnInfo[arg] = trackBtn.getAttribute("lj_" + arg);
    });

    // pop up little dialog to either track by inbox/email or go to more options
    var dlg = document.createElement("div");
    var title = _textDiv("Email me when");
    DOM.addClassName(title, "track_title");
    dlg.appendChild(title);

    var TrackCheckbox = function (title, checked) {
        var checkContainer = document.createElement("div");

        var newCheckbox = document.createElement("input");
        newCheckbox.type = "checkbox";
        newCheckbox.id = "newentrytrack" + Unique.id();
        var newCheckboxLabel = document.createElement("label");
        newCheckboxLabel.setAttribute("for", newCheckbox.id);
        newCheckboxLabel.innerHTML = title;

        checkContainer.appendChild(newCheckbox);
        checkContainer.appendChild(newCheckboxLabel);
        dlg.appendChild(checkContainer);

        newCheckbox.checked = checked ? true : false;

        return newCheckbox;
    };

    // global trackPopup so we can only have one
    if (ESN.trackPopup) {
        ESN.trackPopup.hide();
        ESN.trackPopup = null;
    }

    var saveChangesBtn = document.createElement("input");
    saveChangesBtn.type = "button";
    saveChangesBtn.value = "Save Changes";
    DOM.addClassName(saveChangesBtn, "track_savechanges");

    var trackingNewEntries  = Number(btnInfo['newentry_subid']) ? 1 : 0;
    var trackingNewComments = Number(btnInfo['subid']) ? 1 : 0;

    var newEntryTrackBtn;
    var commentsTrackBtn;

    if (Number(trackBtn.getAttribute("lj_dtalkid"))) {
        // this is a thread tracking button
        // always checked: either because they're subscribed, or because
        // they're going to subscribe.
        commentsTrackBtn = TrackCheckbox("someone replies in this comment thread", 1);
    } else {
        // entry tracking button
        newEntryTrackBtn = TrackCheckbox(LJ_cmtinfo["journal"] + " posts a new entry", trackingNewEntries);
        commentsTrackBtn = TrackCheckbox("someone comments on this post", trackingNewComments);
    }

    DOM.addEventListener(saveChangesBtn, "click", function () {
        ESN.toggleSubscriptions(btnInfo, evt, trackBtn, {
            newEntry: newEntryTrackBtn ? newEntryTrackBtn.checked : false,
            newComments: commentsTrackBtn.checked
        });
        if (ESN.trackPopup) ESN.trackPopup.hide();
    });

    var btnsContainer = document.createElement("div");
    DOM.addClassName(btnsContainer, "track_btncontainer");
    dlg.appendChild(btnsContainer);

    btnsContainer.appendChild(saveChangesBtn);

    var custTrackLink = document.createElement("a");
    custTrackLink.href = trackBtn.parentNode.href;
    btnsContainer.appendChild(custTrackLink);
    custTrackLink.innerHTML = "More Options";
    DOM.addClassName(custTrackLink, "track_moreopts");

    ESN.trackPopup = new LJ_IPPU.showNoteElement(dlg, trackBtn, 0);

    DOM.addEventListener(custTrackLink, "click", function (evt) {
        Event.stop(evt);
        document.location.href = trackBtn.parentNode.href;
        if (ESN.trackPopup) ESN.trackPopup.hide();
        return false;
    });

    return false;
}

// toggles subscriptions
ESN.toggleSubscriptions = function (subInfo, evt, btn, subs) {
    subInfo["subid"] = Number(subInfo["subid"]);
    if ((subInfo["subid"] && ! subs["newComments"])
        || (! subInfo["subid"] && subs["newComments"])) {
        ESN.toggleSubscription(subInfo, evt, btn, "newComments");
    }

    subInfo["newentry_subid"] = Number(subInfo["newentry_subid"]);
    if ((subInfo["newentry_subid"] && ! subs["newEntry"])
        || (! subInfo["newentry_subid"] && subs["newEntry"])) {
            var newentrySubInfo = new Object(subInfo);
            newentrySubInfo["subid"] = Number(btn.getAttribute("lj_newentry_subid"));
            ESN.toggleSubscription(newentrySubInfo, evt, btn, "newEntry");
    }
};

// (Un)subscribes to an event
ESN.toggleSubscription = function (subInfo, evt, btn, sub) {
    var action = "";
    var params = {
        auth_token: sub == "newEntry" ? subInfo.newentry_token : subInfo.auth_token
    };

    if (Number(subInfo.subid)) {
        // subscription exists
        action = "delsub";
        params.subid = subInfo.subid;
    } else {
        // create a new subscription
        action = "addsub";

        var param_keys;
        if (sub == "newEntry") {
            params.etypeid = subInfo.newentry_etypeid;
            param_keys = ["journalid"];
        } else {
            param_keys = ["journalid", "arg1", "arg2", "etypeid"];
        }

        param_keys.forEach(function (param) {
            if (Number(subInfo[param]))
                params[param] = parseInt(subInfo[param]);
        });
    }

    params.action = action;

    var reqInfo = {
        "method": "POST",
        "url":    LiveJournal.getAjaxUrl('esn_subs'),
        "data":   HTTPReq.formEncoded(params)
    };

    var gotInfoCallback = function (info) {
        if (! info) return LJ_IPPU.showNote("Error changing subscription", btn);

        if (info.error) return LJ_IPPU.showNote(info.error, btn);

        if (info.success) {
            if (info.msg)
                LJ_IPPU.showNote(info.msg, btn);

            if (info.subscribed) {
                if (info.subid)
                    DOM.setElementAttribute(btn, "lj_subid", info.subid);
                if (info.newentry_subid)
                    DOM.setElementAttribute(btn, "lj_newentry_subid", info.newentry_subid);

                ["journalid", "arg1", "arg2", "etypeid"].forEach(function (param) {
                    DOM.setElementAttribute(btn, "lj_" + param, 0);
                });

                DOM.setElementAttribute(btn, "title", 'Untrack This');

                // update subthread tracking icons
                var dtalkid = btn.getAttribute("lj_dtalkid");
                if (dtalkid)
                    ESN.updateThreadIcons(dtalkid, "on");
                else // not thread tracking button
                    btn.src = Site.imgprefix + "/btn_tracking.gif";
            } else {
                if (info["event_class"] == "LJ::Event::JournalNewComment")
                    DOM.setElementAttribute(btn, "lj_subid", 0);
                else if (info["event_class"] == "LJ::Event::JournalNewEntry")
                    DOM.setElementAttribute(btn, "lj_newentry_subid", 0);

                ["journalid", "arg1", "arg2", "etypeid"].forEach(function (param) {
                    DOM.setElementAttribute(btn, "lj_" + param, info[param]);
                });

                DOM.setElementAttribute(btn, "title", 'Track This');

                // update subthread tracking icons
                var dtalkid = btn.getAttribute("lj_dtalkid");
                if (dtalkid) {
                    // set state to "off" if no parents tracking this,
                    // otherwise set state to "parent"
                    var state = "off";
                    var parentBtn;
                    var parent_dtalkid = dtalkid;
                    while (parentBtn = ESN.getThreadParentBtn(parent_dtalkid)) {
                        parent_dtalkid = parentBtn.getAttribute("lj_dtalkid");
                        if (! parent_dtalkid) {
                            log("could not find parent_dtalkid");
                            break;
                        }

                        if (! Number(parentBtn.getAttribute("lj_subid")))
                            continue;
                        state = "parent";
                        break;
                    }

                    ESN.updateThreadIcons(dtalkid, state);
                } else {
                    // not thread tracking button
                    btn.src = Site.imgprefix + "/btn_track.gif";
                }
            }

            if (info.auth_token)
                DOM.setElementAttribute(btn, "lj_auth_token", info.auth_token);
            if (info.newentry_token)
                DOM.setElementAttribute(btn, "lj_newentry_token", info.newentry_token);
        }
    };

    reqInfo.onData = gotInfoCallback;
    reqInfo.onError = function (err) { LJ_IPPU.showNote("Error: " + err) };

    HTTPReq.getJSON(reqInfo);
};

// given a dtalkid, find the track button for its parent comment (if any)
ESN.getThreadParentBtn = function (dtalkid) {
    var cmtInfo = LJ_cmtinfo[dtalkid + ""];
    if (! cmtInfo) {
        log("no comment info");
        return null;
    }

    var parent_dtalkid = cmtInfo.parent;
    if (! parent_dtalkid)
        return null;

    return $("lj_track_btn_" + parent_dtalkid);
};

// update all the tracking icons under a parent comment
ESN.updateThreadIcons = function (dtalkid, tracking) {
    var btn = $("lj_track_btn_" + dtalkid);
    if (! btn) {
        log("no button");
        return;
    }

    var cmtInfo = LJ_cmtinfo[dtalkid + ""];
    if (! cmtInfo) {
        log("no comment info");
        return;
    }

    if (Number(btn.getAttribute("lj_subid")) && tracking != "on") {
        // subscription already exists on this button, don't mess with it
        return;
    }

    if (cmtInfo.rc && cmtInfo.rc.length) {
        // update children
        cmtInfo.rc.forEach(function (child_dtalkid) {
            window.setTimeout(function () {
                var state;
                switch (tracking) {
                case "on":
                    state = "parent";
                    break;
                case "off":
                    state = "off";
                    break;
                case "parent":
                    state = "parent";
                    break;
                default:
                    alert("Unknown tracking state " + tracking);
                    break;
                }
                ESN.updateThreadIcons(child_dtalkid, state);
            }, 300);
        });
    }

    // update icon
    var uri;
    switch (tracking) {
        case "on":
            uri = "/btn_tracking.gif";
            break;
        case "off":
            uri = "/btn_track.gif";
            break;
        case "parent":
            uri = "/btn_tracking_thread.gif";
            break;
        default:
            alert("Unknown tracking state " + tracking);
            break;
    }

    btn.src = Site.imgprefix + uri;
};
/*
  IPPU methods:
     init([innerHTML]) -- takes innerHTML as optional argument
     show() -- shows the popup
     hide() -- hides popup
     cancel() -- hides and calls cancel callback

  Content setters:
     setContent(innerHTML) -- set innerHTML
     setContentElement(element) -- adds element as a child of the popup

   Accessors:
     getElement() -- returns popup DIV element
     visible() -- returns whether the popup is visible or not

   Titlebar:
     setTitlebar(show) -- true: show titlebar / false: no titlebar
     setTitle(title) -- sets the titlebar text
     getTitlebarElement() -- returns the titlebar element
     setTitlebarClass(className) -- set the class of the titlebar

   Styling:
     setOverflow(overflow) -- sets ele.style.overflow to overflow
     addClass(className) -- adds class to popup
     removeClass(className) -- removes class to popup

   Browser Hacks:
     setAutoHideSelects(autohide) -- when the popup is shown should it find all the selects
                                on the page and hide them (and show them again) (for IE)

   Positioning/Sizing:
     setLocation(left, top) -- set popup location: will be pixels if units not specified
     setLeft(left) -- set left location
     setTop(top)   -- set top location
     setDimensions(width, height) -- set popup dimensions: will be pixels if units not specified
     setAutoCenter(x, y) -- what dimensions to auto-center
     center() -- centers popup on screen
     centerX() -- centers popup horizontally
     centerY() -- centers popup vertically
     setFixedPosition(fixed) -- should the popup stay fixed on the page when it scrolls?
     centerOnWidget(widget) -- center popup on this widget
     setAutoCenterCallback(callback) -- calls callback with this IPPU instance as a parameter
                                        for auto-centering. Some common built-in class methods
                                        you can use as callbacks are:
                                        IPPU.center
                                        IPPU.centerX
                                        IPPU.centerY

     moveForward(amount) -- increases the zIndex by one or amount if specified
     moveBackward(amount) -- decreases the zIndex by one or amount if specified

   Modality:
     setClickToClose(clickToClose) -- if clickToClose is true, clicking outside of the popup
                                      will close it
     setModal(modality) -- If modality is true, then popup will capture all mouse events
                     and optionally gray out the rest of the page. (overrides clickToClose)
     setOverlayVisible(visible) -- If visible is true, when this popup is on the page it
                                   will gray out the rest of the page if this is modal

   Callbacks:
     setCancelledCallback(callback) -- call this when the dialog is closed through clicking
                                       outside, titlebar close button, etc...
     setHiddenCallback(callback) -- called when the dialog is closed in any fashion

   Fading:
     setFadeIn(fadeIn) -- set whether or not to automatically fade the ippu in
     setFadeOut(fadeOut) -- set whether or not to automatically fade the ippu out
     setFadeSpeed(secs) -- sets fade speed

  Class Methods:
   Handy callbacks:
     IPPU.center
     IPPU.centerX
     IPPU.centerY
   Browser testing:
     IPPU.isIE() -- is the browser internet exploder?
     IPPU.ieSafari() -- is this safari?

////////////////////


ippu.setModalDenialCallback(IPPU.cssBorderFlash);


   private:
    Properties:
     ele -- DOM node of div
     shown -- boolean; if element is in DOM
     autoCenterX -- boolean; auto-center horiz
     autoCenterY -- boolean; auto-center vertical
     fixedPosition -- boolean; stay in fixed position when browser scrolls?
     titlebar -- titlebar element
     title -- string; text to go in titlebar
     showTitlebar -- boolean; whether or not to show titlebar
     content -- DIV containing user's specified content
     clickToClose -- boolean; clicking outside popup will close it
     clickHandlerSetup -- boolean; have we set up the click handlers?
     docOverlay -- DIV that overlays the document for capturing clicks
     modal -- boolean; capture all events and prevent user from doing anything
                       until dialog is dismissed
     visibleOverlay -- boolean; make overlay slightly opaque
     clickHandlerFunc -- function; function to handle document clicks
     resizeHandlerFunc -- function; function to handle document resizing
     autoCenterCallback -- function; what callback to call for auto-centering
     cancelledCallback -- function; called when dialog is cancelled
     setAutoHideSelects -- boolean; autohide all SELECT elements on the page when popup is visible
     hiddenSelects -- array; SELECT elements that have been hidden
     hiddenCallback -- funciton; called when dialog is hidden
     fadeIn, fadeOut, fadeSpeed -- fading settings
     fadeMode -- current fading mode (in, out) if there is fading going on

    Methods:
     updateTitlebar() -- create titlebar if it doesn't exist,
                         hide it if titlebar == false,
                         update titlebar text
     updateContent() -- makes sure all currently specified properties are applied
     setupClickCapture() -- if modal, create document-sized div overlay to capture click events
                            otherwise install document onclick handler
     removeClickHandlers() -- remove overlay, event handlers
     clickHandler() -- event handler for clicks
     updateOverlay() -- if we have an overlay, make sure it's where it should be and (in)visible
                        if it should be
     autoCenter() -- centers popup on screen according to autoCenterX and autoCenterY
     hideSelects() -- hide all select element on page
     showSelects() -- show all selects
     _hide () -- actually hides everything, called by hide(), which does fading if necessary
*/

// this belongs somewhere else:
function changeOpac(id, opacity) {
    var e =  $(id);
    if (e && e.style) {
        var object = e.style;
        if (object) {
            //reduce flicker
            if (IPPU.isSafari() && opacity >= 100)
                opacity = 99.99;

            // IE
            if (object.filters)
                object.filters.alpha.opacity = opacity * 100;

            object.opacity = opacity;
        }
    }
}

IPPU = new Class( Object, {
  setFixedPosition: function (fixed) {
    // no fixed position for IE
    if (IPPU.isIE())
      return;

    this.fixedPosition = fixed;
    this.updateContent();
  },

  clickHandler : function (evt) {
    if (!this.clickToClose) return;
    if (!this.visible()) return;

    evt = Event.prep(evt);
    var target = evt.target;
    // don't do anything if inside the popup
    if (DOM.getAncestorsByClassName(target, "ippu", true).length > 0) return;

    this.cancel();
  },

  setCancelledCallback : function (callback) {
    this.cancelledCallback = callback;
  },

  cancel : function () {
    if (this.cancelledCallback)
      this.cancelledCallback();

    this.hide();
  },

  setHiddenCallback: function (callback) {
    this.hiddenCallback = callback;
  },

  setupClickCapture : function () {
    if (!this.visible() || this.clickHandlerSetup) return;
    if (!this.clickToClose && !this.modal) return;

    this.clickHandlerFunc = this.clickHandler.bindEventListener(this);

    if (this.modal) {
      // create document-sized div to capture events
      if (this.overlay) return; // wtf? shouldn't exist yet

      this.overlay = document.createElement("div");
      this.overlay.style.position = "fixed";
      this.overlay.style.left = "0px";
      this.overlay.style.top = "0px";
      this.overlay.style.margin = "0px";
      this.overlay.style.padding = "0px";
      this.overlay.style.backgroundColor = "#000000";

      this.ele.parentNode.insertBefore(this.overlay, this.ele);
      this.updateOverlay();

      DOM.addEventListener(this.overlay, "click", this.clickHandlerFunc);
    } else {
      // simple document onclick handler
      DOM.addEventListener(document, "click", this.clickHandlerFunc);
    }

    this.clickHandlerSetup = true;
  },

  updateOverlay : function () {
    if (this.overlay) {
      var cd = DOM.getClientDimensions();
      this.overlay.style.width = (cd.x - 1) + "px";
      this.overlay.style.height = (cd.y - 1) + "px";

      if (this.visibleOverlay) {
        this.overlay.backgroundColor = "#000000";
        changeOpac(this.overlay, 0.50);
      } else {
        this.overlay.backgroundColor = "#FFFFFF";
        changeOpac(this.overlay, 0.0);
      }
    }
  },

  resizeHandler : function (evt) {
    this.updateContent();
  },

  removeClickHandlers : function () {
    if (!this.clickHandlerSetup) return;

    var myself = this;
    var handlerFunc = function (evt) {
      myself.clickHandler(evt);
    };

    DOM.removeEventListener(document, "click", this.clickHandlerFunc, false);

    if (this.overlay) {
      DOM.removeEventListener(this.overlay, "click", this.clickHandlerFunc, true);
      this.overlay.parentNode.removeChild(this.overlay);
      this.overlay = undefined;
    }

    this.clickHandlerFunc = undefined;
    this.clickHandlerSetup = false;
  },

  setClickToClose : function (clickToClose) {
    this.clickToClose = clickToClose;

    if (!this.clickHandlerSetup && clickToClose && this.visible()) {
      // popup is already visible, need to set up click handler
      var setupClickCaptureCallback = this.setupClickCapture.bind(this);
      window.setTimeout(setupClickCaptureCallback, 100);
    } else if (!clickToClose && this.clickHandlerSetup) {
      this.removeClickHandlers();
    }

    this.updateContent();
  },

  setModal : function (modal) {
    var changed = (this.modal == modal);

    // if it's modal, we don't want click-to-close
    if (modal)
      this.setClickToClose(false);

    this.modal = modal;
    if (changed) {
      this.removeClickHandlers();
      this.updateContent();
    }
  },

  setOverlayVisible : function (vis) {
    this.visibleOverlay = vis;
    this.updateContent();
  },

  updateContent : function () {
    this.autoCenter();
    this.updateTitlebar();
    this.updateOverlay();
    if (this.titlebar)
      this.setTitlebarClass(this.titlebar.className);

    var setupClickCaptureCallback = this.setupClickCapture.bind(this);
    window.setTimeout(setupClickCaptureCallback, 100);

    if (this.fixedPosition && this.ele.style.position != "fixed")
      this.ele.style.position = "fixed";
    else if (!this.fixedPosition && this.ele.style.position == "fixed")
      this.ele.style.position = "absolute";
  },

  getTitlebarElement : function () {
    return this.titlebar;
  },

  setTitlebarClass : function (className) {
    if (this.titlebar)
      this.titlebar.className = className;
  },

  setOverflow : function (overflow) {
    if (this.ele)
      this.ele.style.overflow = overflow;
  },

  visible : function () {
    return this.shown;
  },

  setTitlebar : function (show) {
    this.showTitlebar = show;

    if (show) {
      if (!this.titlebar) {
        // titlebar hasn't been created. Create it.
        var tbar = document.createElement("div");
        if (!tbar) return;
        tbar.style.width = "100%";

        if (this.title) tbar.innerHTML = this.title;
        this.ele.insertBefore(tbar, this.content);
        this.titlebar = tbar;

      }
    } else if (this.titlebar) {
      this.ele.removeChild(this.titlebar);
      this.titlebar = false;
    }
  },

  setTitle : function (title) {
    this.title = title;
    this.updateTitlebar();
  },

  updateTitlebar : function() {
    if (this.showTitlebar && this.titlebar && this.title != this.titlebar.innerHTML) {
      this.titlebar.innerHTML = this.title;
    }
  },

  addClass : function (className) {
    DOM.addClassName(this.ele, className);
  },

  removeClass : function (className) {
    DOM.removeClassName(this.ele, className);
  },

  setAutoCenterCallback : function (callback) {
    this.autoCenterCallback = callback;
  },

  autoCenter : function () {
    if (!this.visible || !this.visible()) return;

    if (this.autoCenterCallback) {
      this.autoCenterCallback(this);
      return;
    }

    if (this.autoCenterX)
      this.centerX();

    if (this.autoCenterY)
      this.centerY();
  },

  center : function () {
    this.centerX();
    this.centerY();
  },

  centerOnWidget : function (widget, offsetTop, offsetLeft) {
        offsetTop = offsetTop || 0;
        offsetLeft = offsetLeft || 0;
        this.setAutoCenter(false, false);
        this.setAutoCenterCallback(null);
  var wd = DOM.getAbsoluteDimensions(widget);
    var ed = DOM.getAbsoluteDimensions(this.ele);
    var newleft = (wd.absoluteRight - wd.offsetWidth / 2 - ed.offsetWidth / 2) + offsetLeft;
    var newtop = (wd.absoluteBottom - wd.offsetHeight / 2 - ed.offsetHeight / 2) + offsetTop;

        newleft = newleft < 0 ? 0 : newleft;
        newtop  = newtop  < 0 ? 0 : newtop;
    DOM.setLeft(this.ele, newleft);
    DOM.setTop(this.ele, newtop);
  },

  centerX : function () {
    if (!this.visible || !this.visible()) return;

    var cd = DOM.getClientDimensions();
    var newleft = cd.x / 2 - DOM.getDimensions(this.ele).offsetWidth / 2;
    DOM.setLeft(this.ele, newleft);
  },

  centerY : function () {
    if (!this.visible || !this.visible()) return;

    var cd = DOM.getClientDimensions();
    var newtop = cd.y / 2 - DOM.getDimensions(this.ele).offsetHeight / 2;
    DOM.setTop(this.ele, newtop);
  },

  setAutoCenter : function (autoCenterX, autoCenterY) {
    this.autoCenterX = autoCenterX || false;
    this.autoCenterY = autoCenterY || false;

    if (!autoCenterX && !autoCenterY) {
        this.setAutoCenterCallback(null);
        return;
    }

    this.autoCenter();
  },

  setDimensions : function (width, height) {
    width = width + "";
    height = height + "";
    if (width.match(/^\d+$/)) width += "px";
    if (height.match(/^\d+$/)) height += "px";

    this.ele.style.width  = width;
    this.ele.style.height = height;
  },

  moveForward : function (howMuch) {
      if (!howMuch) howMuch = 1;
      if (! this.ele) return;

      this.ele.style.zIndex += howMuch;
  },

  moveBackward : function (howMuch) {
      if (!howMuch) howMuch = 1;
      if (! this.ele) return;

      this.ele.style.zIndex -= howMuch;
  },

  setLocation : function (left, top) {
      this.setLeft(left);
      this.setTop(top);
  },

  setTop : function (top) {
    top = top + "";
    if (top.match(/^\d+$/)) top += "px";
    this.ele.style.top = top;
  },

  setLeft : function (left) {
    left = left + "";
    if (left.match(/^\d+$/)) left += "px";
    this.ele.style.left = left;
  },

  getElement : function () {
    return this.ele;
  },

  setContent : function (html) {
    this.content.innerHTML = html;
  },

  setContentElement : function (element) {
      // remove child nodes
      while (this.content.firstChild) {
          this.content.removeChild(this.content.firstChild);
      };

    this.content.appendChild(element);
  },

  setFadeIn : function (fadeIn) {
      this.fadeIn = fadeIn;
  },

  setFadeOut : function (fadeOut) {
      this.fadeOut = fadeOut;
  },

  setFadeSpeed : function (fadeSpeed) {
      this.fadeSpeed = fadeSpeed;
  },

  show : function () {
    this.shown = true;

    if (this.fadeIn) {
        var opp = 0.01;

        changeOpac(this.ele, opp);
    }

    document.body.appendChild(this.ele);
    this.ele.style.position = "absolute";
    if (this.autoCenterX || this.autoCenterY) this.center();

    this.updateContent();

    if (!this.resizeHandlerFunc) {
      this.resizeHandlerFunc = this.resizeHandler.bindEventListener(this);
      DOM.addEventListener(window, "resize", this.resizeHandlerFunc, false);
    }

    if (this.fadeIn)
        this.fade("in");

    this.hideSelects();
  },

  fade : function (mode, callback) {
      var opp;
      var delta;

      var steps = 10.0;

      if (mode == "in") {
          delta = 1 / steps;
          opp = 0.1;
      } else {
          if (this.ele.style.opacity)
          opp = finiteFloat(this.ele.style.opacity);
          else
          opp = 0.99;

          delta = -1 / steps;
      }

      var fadeSpeed = this.fadeSpeed;
      if (!fadeSpeed) fadeSpeed = 1;

      var fadeInterval = steps / fadeSpeed * 5;

      this.fadeMode = mode;

      var self = this;
      var fade = function () {
          opp += delta;

          // did someone start a fade in the other direction? if so,
          // cancel this fade
          if (self.fadeMode && self.fadeMode != mode) {
              if (callback)
                  callback.call(self, []);

              return;
          }

          if (opp <= 0.1) {
              if (callback)
                  callback.call(self, []);

              self.fadeMode = null;

              return;
          } else if (opp >= 1.0) {
              if (callback)
                  callback.call(self, []);

              self.fadeMode = null;

              return;
          } else {
              changeOpac(self.ele, opp);
              window.setTimeout(fade, fadeInterval);
          }
      };

      fade();
  },

  hide : function () {
    if (! this.visible()) return;

    if (this.fadeOut && this.ele) {
        this.fade("out", this._hide.bind(this));
    } else {
        this._hide();
    }
  },

  _hide : function () {
    if (this.hiddenCallback)
      this.hiddenCallback();

    this.shown = false;
    this.removeClickHandlers();

    if (this.ele)
    document.body.removeChild(this.ele);

    if (this.resizeHandlerFunc)
      DOM.removeEventListener(window, "resize", this.resizeHandlerFunc);

    this.showSelects();
  },

  // you probably want this for IE being dumb
  // (IE thinks select elements are cool and puts them in front of every element on the page)
  setAutoHideSelects : function (autohide) {
    this.autoHideSelects = autohide;
    this.updateContent();
  },

  hideSelects : function () {
    if (!this.autoHideSelects || !IPPU.isIE()) return;
    var sels = document.getElementsByTagName("select");
    var ele;
    for (var i = 0; i < sels.length; i++) {
      ele = sels[i];
      if (!ele) continue;

      // if this element is inside the ippu, skip it
      if (DOM.getAncestorsByClassName(ele, "ippu", true).length > 0) continue;

      if (ele.style.visibility != 'hidden') {
        ele.style.visibility = 'hidden';
        this.hiddenSelects.push(ele);
      }
    }
  },

  showSelects : function () {
    if (! this.autoHideSelects) return;
    var ele;
    while (ele = this.hiddenSelects.pop())
      ele.style.visibility = '';
  },

  init: function (html) {
    var ele = document.createElement("div");
    this.ele = ele;
    this.shown = false;
    this.autoCenterX = false;
    this.autoCenterY = false;
    this.titlebar = null;
    this.title = "";
    this.showTitlebar = false;
    this.clickToClose = false;
    this.modal = false;
    this.clickHandlerSetup = false;
    this.docOverlay = false;
    this.visibleOverlay = false;
    this.clickHandlerFunc = false;
    this.resizeHandlerFunc = false;
    this.fixedPosition = false;
    this.autoCenterCallback = null;
    this.cancelledCallback = null;
    this.autoHideSelects = false;
    this.hiddenCallback = null;
    this.fadeOut = false;
    this.fadeIn = false;
    this.hiddenSelects = [];
    this.fadeMode = null;

    ele.style.position = "absolute";
    ele.style.zIndex   = "1000";

    // plz don't remove thx
    DOM.addClassName(ele, "ippu");

    // create DIV to hold user's content
    this.content = document.createElement("div");

    this.content.innerHTML = html;

    this.ele.appendChild(this.content);
  }
});

// class methods
IPPU.center = function (obj) {
  obj.centerX();
  obj.centerY();
};

IPPU.centerX = function (obj) {
  obj.centerX();
};

IPPU.centerY = function (obj) {
  obj.centerY();
};

IPPU.isIE = function () {
    var UA = navigator.userAgent.toLowerCase();
    if (UA.indexOf('msie') != -1) return true;
    return false;
};

IPPU.isSafari = function () {
    var UA = navigator.userAgent.toLowerCase();
    if (UA.indexOf('safari') != -1) return true;
    return false;
};
LJ_IPPU = new Class ( IPPU, {
  init: function(title) {
    if (!title)
      title = "";

    LJ_IPPU.superClass.init.apply(this, []);

    this.uniqId = this.generateUniqId();
    this.cancelThisFunc = this.cancel.bind(this);

    this.setTitle(title);
    this.setTitlebar(true);
    this.setTitlebarClass("lj_ippu_titlebar");

    this.addClass("lj_ippu");

    this.setAutoCenterCallback(IPPU.center);
    this.setDimensions(400, "auto");
    this.setOverflow("hidden");

    this.setFixedPosition(true);
    this.setClickToClose(true);
    this.setAutoHideSelects(true);
  },

  setTitle: function (title) {
    var titlebarContent = "\
      <div style='float:right; padding-right: 8px'>" +
      "<img src='" + Site.imgprefix + "/CloseButton.gif' width='15' height='15' id='" + this.uniqId + "_cancel' /></div>" + title;

    LJ_IPPU.superClass.setTitle.apply(this, [titlebarContent]);
  },

  generateUniqId: function() {
    var theDate = new Date();
    return "lj_ippu_" + theDate.getHours() + theDate.getMinutes() + theDate.getMilliseconds();
  },

  show: function() {
    LJ_IPPU.superClass.show.apply(this);
    var setupCallback = this.setup_lj_ippu.bind(this);
    window.setTimeout(setupCallback, 300);
  },

  setup_lj_ippu: function (evt) {
    var cancelCallback = this.cancelThisFunc;
    DOM.addEventListener($(this.uniqId + "_cancel"), "click", cancelCallback, true);
  },

  hide: function() {
    DOM.removeEventListener($(this.uniqId + "_cancel"), "click", this.cancelThisFunc, true);
    LJ_IPPU.superClass.hide.apply(this);
  }
} );

// Class method to show a popup to show a note to the user
// note = message to show
// underele = element to display the note underneath
LJ_IPPU.showNote = function (note, underele, timeout, style) {
    var noteElement = document.createElement("div");
    noteElement.innerHTML = note;

    return LJ_IPPU.showNoteElement(noteElement, underele, timeout, style);
};

LJ_IPPU.showErrorNote = function (note, underele, timeout) {
    return LJ_IPPU.showNote(note, underele, timeout, "ErrorNote");
};

LJ_IPPU.showNoteElement = function (noteEle, underele, timeout, style) {
    var notePopup = new IPPU();
    notePopup.init();

    var inner = document.createElement("div");
    DOM.addClassName(inner, "Inner");
    inner.appendChild(noteEle);
    notePopup.setContentElement(inner);

    notePopup.setTitlebar(false);
    notePopup.setFadeIn(true);
    notePopup.setFadeOut(true);
    notePopup.setFadeSpeed(4);
    notePopup.setDimensions("auto", "auto");
    if (!style) style = "Note";
    notePopup.addClass(style);

    var dim;
    if (underele) {
        // pop up the box right under the element
        dim = DOM.getAbsoluteDimensions(underele);
        if (!dim) return;
    }

    var bounds = DOM.getClientDimensions();
    if (!bounds) return;

    if (!dim) {
        // no element specified to pop up on, show in the middle
        // notePopup.setModal(true);
        // notePopup.setOverlayVisible(true);
        notePopup.setAutoCenter(true, true);
        notePopup.show();
    } else {
        // default is to auto-center, don't want that
        notePopup.setAutoCenter(false, false);
        notePopup.setLocation(dim.absoluteLeft, dim.absoluteBottom + 4);
        notePopup.show();

        var popupBounds = DOM.getAbsoluteDimensions(notePopup.getElement());
        if (popupBounds.absoluteRight > bounds.x) {
            notePopup.setLocation(bounds.x - popupBounds.offsetWidth - 30, dim.absoluteBottom + 4);
        }
    }

    notePopup.setClickToClose(true);
    notePopup.moveForward();

    if (! defined(timeout)) {
        timeout = 5000;
    }

    if (timeout) {
        window.setTimeout(function () {
            if (notePopup)
                notePopup.hide();
        }, timeout);
    }

    return notePopup;
};

LJ_IPPU.textPrompt = function (title, prompt, callback) {
    title += '';
    var notePopup = new LJ_IPPU(title);

    var inner = document.createElement("div");
    DOM.addClassName(inner, "ljippu_textprompt");

    // label
    if (prompt)
        inner.appendChild(_textDiv(prompt));

    // text field
    var field = document.createElement("textarea");
    DOM.addClassName(field, "htmlfield");
    field.cols = 40;
    field.rows = 5;
    inner.appendChild(field);

    // submit btn
    var btncont = document.createElement("div");
    DOM.addClassName(btncont, "submitbtncontainer");
    var btn = document.createElement("input");
    DOM.addClassName(btn, "submitbtn");
    btn.type = "button";
    btn.value = "Insert";
    btncont.appendChild(btn);
    inner.appendChild(btncont);

    notePopup.setContentElement(inner);

    notePopup.setAutoCenter(true, true);
    notePopup.setDimensions("60%", "auto");
    notePopup.show();
    field.focus();

    DOM.addEventListener(btn, "click", function (e) {
        notePopup.hide();
        if (callback)
            callback.apply(null, [field.value]);
    });
}
// LiveJournal javascript standard interface routines

// create a little animated hourglass at (x,y) with a unique-ish ID
// returns the element created
Hourglass = new Class( Object, {
  init: function(widget, classname) {
    this.ele = document.createElement("img");
    if (!this.ele) return;

    var imgprefix = Site ? Site.imgprefix : '';

    this.ele.src = imgprefix ? imgprefix + "/hourglass.gif" : "/img/hourglass.gif";
    this.ele.style.position = "absolute";

    DOM.addClassName(this.ele, classname);

    if (widget)
      this.hourglass_at_widget(widget);
  },

  hourglass_at: function (x, y) {
    this.ele.width = 17;
    this.ele.height = 17;
    this.ele.style.top = (y - 8) + "px";
    this.ele.style.left = (x - 8) + "px";

    // unique ID
    this.ele.id = "lj_hourglass" + x + "." + y;

    document.body.appendChild(this.ele);
  },

  add_class_name: function (classname) {
      if (this.ele)
      DOM.addClassName(this.ele, classname);
  },

  hourglass_at_widget: function (widget) {
    var dim = DOM.getAbsoluteDimensions(widget);
    var x = dim.absoluteLeft;
    var y = dim.absoluteTop;
    var w = dim.absoluteRight - x;
    var h = dim.absoluteBottom - y;
    if (w && h) {
      x += w/2;
      y += h/2;
    }
    this.hourglass_at(x, y);
  },

  hide: function () {
    if (this.ele) {
      try {
        document.body.removeChild(this.ele);
      } catch (e) {}
    }
  }

} );
var ContextualPopup = new Object;

ContextualPopup.popupDelay  = 500;
ContextualPopup.hideDelay   = 250;
ContextualPopup.disableAJAX = false;
ContextualPopup.debug       = false;

ContextualPopup.cachedResults   = {};
ContextualPopup.currentRequests = {};
ContextualPopup.mouseInTimer    = null;
ContextualPopup.mouseOutTimer   = null;
ContextualPopup.currentId       = null;
ContextualPopup.hourglass       = null;
ContextualPopup.elements        = {};

ContextualPopup.setup = function () {
    // don't do anything if no remote
    if (!Site || !Site.ctx_popup) return;

    // attach to all ljuser head icons
    var ljusers = DOM.getElementsByTagAndClassName(document, 'span', "ljuser");

    var userElements = [];
    ljusers.forEach(function (ljuser) {
        var nodes = ljuser.getElementsByTagName("img");
        for (var i=0; i < nodes.length; i++) {
            var node = nodes.item(i);

            // if the parent (a tag with link to userinfo) has userid in its URL, then
            // this is an openid user icon and we should use the userid
            var parent = node.parentNode;
            var userid;
            if (parent && (userid = parent.href.match(/\?userid=(\d+)/i)))
                node.userid = userid[1];
            else
                node.username = ljuser.getAttribute("lj:user");

            if (!node.username && !node.userid) continue;

            userElements.push(node);
            DOM.addClassName(node, "ContextualPopup");

            // remove alt tag so IE doesn't show the label over the popup
            node.alt = "";
        }
    });

    // attach to all userpics
    var images = document.getElementsByTagName("img") || [];
    Array.prototype.forEach.call(images, function (image) {
        // if the image url matches a regex for userpic urls then attach to it
        if (image.src.match(/userpic\..+\/\d+\/\d+/) ||
            image.src.match(/\/userpic\/\d+\/\d+/)) {
            image.up_url = image.src;
            DOM.addClassName(image, "ContextualPopup");
            userElements.push(image);

            // remove alt tag so IE doesn't show the label over the popup
            image.alt = "";
        }
    });

    var ctxPopupId = 1;
    userElements.forEach(function (userElement) {
        ContextualPopup.elements[ctxPopupId + ""] = userElement;
        userElement.ctxPopupId = ctxPopupId++;
    });

    DOM.addEventListener(document.body, "mousemove", ContextualPopup.mouseOver.bindEventListener());
}

ContextualPopup.isCtxPopElement = function (ele) {
    return (ele && DOM.getAncestorsByClassName(ele, "ContextualPopup", true).length);
}

ContextualPopup.mouseOver = function (e) {
    var target = e.target;
    var ctxPopupId = target.ctxPopupId;

    // if the ctxpopup class isn't fully loaded and set up yet,
    // skip the event handling for now
    if (!eval("ContextualPopup") || !ContextualPopup.isCtxPopElement) return;

    // did the mouse move out?
    if (!target || !ContextualPopup.isCtxPopElement(target)) {
        if (ContextualPopup.mouseInTimer) {
            window.clearTimeout(ContextualPopup.mouseInTimer);
            ContextualPopup.mouseInTimer = null;
        };

        if (ContextualPopup.ippu) {
            if (ContextualPopup.mouseInTimer || ContextualPopup.mouseOutTimer) return;

            ContextualPopup.mouseOutTimer = window.setTimeout(function () {
                ContextualPopup.mouseOut(e);
            }, ContextualPopup.hideDelay);
            return;
        }
    }

    // we're inside a ctxPopElement, cancel the mouseout timer
    if (ContextualPopup.mouseOutTimer) {
        window.clearTimeout(ContextualPopup.mouseOutTimer);
        ContextualPopup.mouseOutTimer = null;
    }

    if (!ctxPopupId)
    return;

    var cached = ContextualPopup.cachedResults[ctxPopupId + ""];

    // if we don't have cached data background request it
    if (!cached) {
        ContextualPopup.getInfo(target);
    }

    // start timer if it's not running
    if (! ContextualPopup.mouseInTimer && (! ContextualPopup.ippu || (
                                                                      ContextualPopup.currentId &&
                                                                      ContextualPopup.currentId != ctxPopupId))) {
        ContextualPopup.mouseInTimer = window.setTimeout(function () {
            ContextualPopup.showPopup(ctxPopupId);
        }, ContextualPopup.popupDelay);
    }
}

// if the popup was not closed by us catch it and handle it
ContextualPopup.popupClosed = function () {
    ContextualPopup.mouseOut();
}

ContextualPopup.mouseOut = function (e) {
    if (ContextualPopup.mouseInTimer)
        window.clearTimeout(ContextualPopup.mouseInTimer);
    if (ContextualPopup.mouseOutTimer)
        window.clearTimeout(ContextualPopup.mouseOutTimer);

    ContextualPopup.mouseInTimer = null;
    ContextualPopup.mouseOutTimer = null;
    ContextualPopup.currentId = null;

    ContextualPopup.hidePopup();
}

ContextualPopup.showPopup = function (ctxPopupId) {
    if (ContextualPopup.mouseInTimer) {
        window.clearTimeout(ContextualPopup.mouseInTimer);
    }
    ContextualPopup.mouseInTimer = null;

    if (ContextualPopup.ippu && (ContextualPopup.currentId && ContextualPopup.currentId == ctxPopupId)) {
        return;
    }

    ContextualPopup.currentId = ctxPopupId;

    ContextualPopup.constructIPPU(ctxPopupId);

    var ele = ContextualPopup.elements[ctxPopupId + ""];
    var data = ContextualPopup.cachedResults[ctxPopupId + ""];

    if (! ele || (data && data.noshow)) {
        return;
    }

    if (ContextualPopup.ippu) {
        var ippu = ContextualPopup.ippu;
        // default is to auto-center, don't want that
        ippu.setAutoCenter(false, false);

        // pop up the box right under the element
        var dim = DOM.getAbsoluteDimensions(ele);
        if (!dim) return;

        var bounds = DOM.getClientDimensions();
        if (!bounds) return;

        // hide the ippu content element, put it on the page,
        // get its bounds and make sure it's not going beyond the client
        // viewport. if the element is beyond the right bounds scoot it to the left.

        var popEle = ippu.getElement();
        popEle.style.visibility = "hidden";
        ContextualPopup.ippu.setLocation(dim.absoluteLeft, dim.absoluteBottom);

        // put the content element on the page so its dimensions can be found
        ContextualPopup.ippu.show();

        var ippuBounds = DOM.getAbsoluteDimensions(popEle);
        if (ippuBounds.absoluteRight > bounds.x) {
            ContextualPopup.ippu.setLocation(bounds.x - ippuBounds.offsetWidth - 30, dim.absoluteBottom);
        }

        // finally make the content visible
        popEle.style.visibility = "visible";
    }
}

ContextualPopup.constructIPPU = function (ctxPopupId) {
    if (ContextualPopup.ippu) {
        ContextualPopup.ippu.hide();
        ContextualPopup.ippu = null;
    }

    var ippu = new IPPU();
    ippu.init();
    ippu.setTitlebar(false);
    ippu.setFadeOut(true);
    ippu.setFadeIn(true);
    ippu.setFadeSpeed(15);
    ippu.setDimensions("auto", "auto");
    ippu.addClass("ContextualPopup");
    ippu.setCancelledCallback(ContextualPopup.popupClosed);
    ContextualPopup.ippu = ippu;

    ContextualPopup.renderPopup(ctxPopupId);
}

ContextualPopup.renderPopup = function (ctxPopupId) {
    var ippu = ContextualPopup.ippu;

    if (!ippu)
    return;

    if (ctxPopupId) {
        var data = ContextualPopup.cachedResults[ctxPopupId];

        if (!data) {
            ippu.setContent("<div class='Inner'>Loading...</div>");
            return;
        } else if (!data.username || !data.success || data.noshow) {
            ippu.hide();
            return;
        }

        var username = data.display_username;

        var inner = document.createElement("div");
        DOM.addClassName(inner, "Inner");

        var content = document.createElement("div");
        DOM.addClassName(content, "Content");

        var bar = document.createElement("span");
        bar.innerHTML = " | ";

        // userpic
        if (data.url_userpic && data.url_userpic != ContextualPopup.elements[ctxPopupId].src) {
            var userpicContainer = document.createElement("div");
            var userpicLink = document.createElement("a");
            userpicLink.href = data.url_allpics;
            var userpic = document.createElement("img");
            userpic.src = data.url_userpic;
            userpic.width = data.userpic_w;
            userpic.height = data.userpic_h;

            userpicContainer.appendChild(userpicLink);
            userpicLink.appendChild(userpic);
            DOM.addClassName(userpicContainer, "Userpic");

            inner.appendChild(userpicContainer);
        }

        inner.appendChild(content);

        // relation
        var relation = document.createElement("div");
        if (data.is_comm) {
            if (data.is_member)
                relation.innerHTML = "You are a member of " + username;
            else if (data.is_friend)
                relation.innerHTML = "You are watching " + username;
            else
                relation.innerHTML = username;
        } else if (data.is_syndicated) {
            if (data.is_friend)
                relation.innerHTML = "You are subscribed to " + username;
            else
                relation.innerHTML = username;
        } else {
            if (data.is_requester) {
                relation.innerHTML = "This is you";
            } else {
                var label = username + " ";

                if (data.is_friend_of) {
                    if (data.is_friend)
                        label += "is your mutual friend";
                    else
                        label += "lists you as a friend";
                } else {
                    if (data.is_friend)
                        label += "is your friend";
                }

                relation.innerHTML = label;
            }
        }
        DOM.addClassName(relation, "Relation");
        content.appendChild(relation);

        // add site-specific content here
        var extraContent = LiveJournal.run_hook("ctxpopup_extrainfo", data);
        if (extraContent) {
            content.appendChild(extraContent);
        }

        // member of community
        if (data.is_logged_in && data.is_comm) {
            var membership      = document.createElement("span");
            var membershipLink  = document.createElement("a");

            var membership_action = data.is_member ? "leave" : "join";

            if (data.is_member) {
                membershipLink.href = data.url_leavecomm;
                membershipLink.innerHTML = "Leave";
            } else {
                membershipLink.href = data.url_joincomm;
                membershipLink.innerHTML = "Join community";
            }

            if (!ContextualPopup.disableAJAX) {
                DOM.addEventListener(membershipLink, "click", function (e) {
                    Event.prep(e);
                    Event.stop(e);
                    return ContextualPopup.changeRelation(data, ctxPopupId, membership_action, e); });
            }

            membership.appendChild(membershipLink);
            content.appendChild(membership);
        }

        // send message
        var message;
        if (data.is_logged_in && data.is_person && ! data.is_requester && data.url_message) {
            message = document.createElement("span");

            var sendmessage = document.createElement("a");
            sendmessage.href = data.url_message;
            sendmessage.innerHTML = "Send message";

            message.appendChild(sendmessage);
        }

        if (message)
            content.appendChild(message);

        // friend
        var friend;
        if (data.is_logged_in && ! data.is_requester) {
            friend = document.createElement("span");

            if (! data.is_friend) {
                // add friend link
                var addFriend = document.createElement("span");
                var addFriendLink = document.createElement("a");
                addFriendLink.href = data.url_addfriend;

                if (data.is_comm)
                    addFriendLink.innerHTML = "Watch community";
                else if (data.is_syndicated)
                    addFriendLink.innerHTML = "Subscribe to feed";
                else
                    addFriendLink.innerHTML = "Add friend";

                addFriend.appendChild(addFriendLink);
                DOM.addClassName(addFriend, "AddFriend");

                if (!ContextualPopup.disableAJAX) {
                    DOM.addEventListener(addFriendLink, "click", function (e) {
                        Event.prep(e);
                        Event.stop(e);
                        return ContextualPopup.changeRelation(data, ctxPopupId, "addFriend", e); });
                }

                friend.appendChild(addFriend);
            } else {
                // remove friend link (omg!)
                var removeFriend = document.createElement("span");
                var removeFriendLink = document.createElement("a");
                removeFriendLink.href = data.url_addfriend;

                if (data.is_comm)
                    removeFriendLink.innerHTML = "Stop watching";
                else if (data.is_syndicated)
                    removeFriendLink.innerHTML = "Unsubscribe";
                else
                    removeFriendLink.innerHTML = "Remove friend";

                removeFriend.appendChild(removeFriendLink);
                DOM.addClassName(removeFriend, "RemoveFriend");

                if (!ContextualPopup.disableAJAX) {
                    DOM.addEventListener(removeFriendLink, "click", function (e) {
                        Event.stop(e);
                        return ContextualPopup.changeRelation(data, ctxPopupId, "removeFriend", e); });
                }

                friend.appendChild(removeFriend);
            }

            DOM.addClassName(relation, "FriendStatus");
        }

        // add a bar between stuff if we have community actions
        if ((data.is_logged_in && data.is_comm) || (message && friend))
            content.appendChild(bar.cloneNode(true));

        if (friend)
            content.appendChild(friend);

        // break
        if (data.is_logged_in && !data.is_requester) content.appendChild(document.createElement("br"));

        // view label
        var viewLabel = document.createElement("span");
        viewLabel.innerHTML = "View: ";
        content.appendChild(viewLabel);

        // journal
        if (data.is_person || data.is_comm || data.is_syndicated) {
            var journalLink = document.createElement("a");
            journalLink.href = data.url_journal;

            if (data.is_person)
                journalLink.innerHTML = "Journal";
            else if (data.is_comm)
                journalLink.innerHTML = "Community";
            else if (data.is_syndicated)
                journalLink.innerHTML = "Feed";

            content.appendChild(journalLink);
            content.appendChild(bar.cloneNode(true));
        }

        // profile
        var profileLink = document.createElement("a");
        profileLink.href = data.url_profile;
        profileLink.innerHTML = "Profile";
        content.appendChild(profileLink);

        // clearing div
        var clearingDiv = document.createElement("div");
        DOM.addClassName(clearingDiv, "ljclear");
        clearingDiv.innerHTML = "&nbsp;";
        content.appendChild(clearingDiv);

        ippu.setContentElement(inner);
    }
}

// ajax request to change relation
ContextualPopup.changeRelation = function (info, ctxPopupId, action, evt) {
    if (!info) return true;

    var postData = {
        "target": info.username,
        "action": action
    };

    // get the authtoken
    var authtoken = info[action + "_authtoken"];
    if (!authtoken) log("no auth token for action" + action);
    postData.auth_token = authtoken;

    // needed on journal subdomains
    var url = LiveJournal.getAjaxUrl("changerelation");

    // callback from changing relation request
    var changedRelation = function (data) {
        if (ContextualPopup.hourglass) ContextualPopup.hideHourglass();

        if (data.error) {
            ContextualPopup.showNote(data.error, ctxPopupId);
            return;
        }

        if (data.note)
        ContextualPopup.showNote(data.note, ctxPopupId);

        if (!data.success) return;

        if (ContextualPopup.cachedResults[ctxPopupId + ""]) {
            var updatedProps = ["is_friend", "is_member"];
            updatedProps.forEach(function (prop) {
                ContextualPopup.cachedResults[ctxPopupId + ""][prop] = data[prop];
            });
        }

        // if the popup is up, reload it
        ContextualPopup.renderPopup(ctxPopupId);
    };

    var opts = {
        "data": HTTPReq.formEncoded(postData),
        "method": "POST",
        "url": url,
        "onError": ContextualPopup.gotError,
        "onData": changedRelation
    };

    // do hourglass at mouse coords
    var mouseCoords = DOM.getAbsoluteCursorPosition(evt);
    if (!ContextualPopup.hourglass && mouseCoords) {
        ContextualPopup.hourglass = new Hourglass();
        ContextualPopup.hourglass.init(null, "lj_hourglass");
        ContextualPopup.hourglass.add_class_name("ContextualPopup"); // so mousing over hourglass doesn't make ctxpopup think mouse is outside
        ContextualPopup.hourglass.hourglass_at(mouseCoords.x, mouseCoords.y);
    }

    HTTPReq.getJSON(opts);

    return false;
}

// create a little popup to notify the user of something
ContextualPopup.showNote = function (note, ctxPopupId) {
    var ele;

    if (ContextualPopup.ippu) {
        // pop up the box right under the element
        ele = ContextualPopup.ippu.getElement();
    } else {
        if (ctxPopupId) {
            var ele = ContextualPopup.elements[ctxPopupId + ""];
        }
    }

    LJ_IPPU.showNote(note, ele);
}

ContextualPopup.hidePopup = function (ctxPopupId) {
    if (ContextualPopup.hourglass) ContextualPopup.hideHourglass();

    // destroy popup for now
    if (ContextualPopup.ippu) {
        ContextualPopup.ippu.hide();
        ContextualPopup.ippu = null;
    }
}

// do ajax request of user info
ContextualPopup.getInfo = function (target) {
    var ctxPopupId = target.ctxPopupId;
    var username = target.username;
    var userid = target.userid;
    var up_url = target.up_url;

    if (!ctxPopupId)
    return;

    if (ContextualPopup.currentRequests[ctxPopupId + ""]) {
        return;
    }

    ContextualPopup.currentRequests[ctxPopupId] = 1;

    if (!username) username = "";
    if (!userid) userid = 0;
    if (!up_url) up_url = "";

    var params = HTTPReq.formEncoded ({
        "user": username,
            "userid": userid,
            "userpic_url": up_url,
            "mode": "getinfo"
    });

    // needed on journal subdomains
    var url = LiveJournal.getAjaxUrl("ctxpopup");
    var url = Site.currentJournal ? "/" + Site.currentJournal + "/__rpc_ctxpopup" : "/__rpc_ctxpopup";

    // got data callback
    var gotInfo = function (data) {
        if (ContextualPopup && ContextualPopup.hourglass) ContextualPopup.hideHourglass();

        ContextualPopup.cachedResults[ctxPopupId] = data;

        if (data.error) {
            if (data.noshow) return;

            ContextualPopup.showNote(data.error, ctxPopupId);
            return;
        }

        if (data.note)
        ContextualPopup.showNote(data.note, data.ctxPopupId);

        ContextualPopup.currentRequests[ctxPopupId] = null;

        ContextualPopup.renderPopup(ctxPopupId);

        // expire cache after 5 minutes
        setTimeout(function () {
            ContextualPopup.cachedResults[ctxPopupId] = null;
        }, 60 * 1000);
    };

    HTTPReq.getJSON({
        "url": url,
            "method" : "GET",
            "data": params,
            "onData": gotInfo,
            "onError": ContextualPopup.gotError
            });
}

ContextualPopup.hideHourglass = function () {
    if (ContextualPopup.hourglass) {
        ContextualPopup.hourglass.hide();
        ContextualPopup.hourglass = null;
    }
}

ContextualPopup.gotError = function (err) {
    if (ContextualPopup.hourglass) ContextualPopup.hideHourglass();

    if (ContextualPopup.debug)
        ContextualPopup.showNote("Error: " + err);
}

// when page loads, set up contextual popups
LiveJournal.register_hook("page_load", ContextualPopup.setup);

Horizon = new Object;

// Holds style info for the top level menu
Horizon.toplevel_hover_style = null;
// Holds the last menu opened
Horizon.opened_menu = null;
// Holds Select elements tht have been hidden
Horizon.hiddenSelects = [];
// Is IE version 6 or lower flag
Horizon.isIE6down = false;

Horizon.scheme_init = function () {
    Horizon.toplevel_hover_style = DOM.getComputedStyle($("Alpha"));
    Horizon.setIsIE6down();
}

Horizon.open_menu = function (m) {
    /* Needed to nicely turn on top level menu items for IE */
    if (Horizon.toplevel_hover_style) {
        m.style.backgroundImage = Horizon.toplevel_hover_style.backgroundImage;
        m.style.borderColor = Horizon.toplevel_hover_style.borderColor;
        m.style.borderWidth = Horizon.toplevel_hover_style.borderWidth;
        m.style.borderStyle = Horizon.toplevel_hover_style.borderStyle;
        var links = m.getElementsByTagName('a');
        links[0].style.color = Horizon.toplevel_hover_style.color;
    }

    Horizon.hideSelects();
    var menus = m.getElementsByTagName('ul');
    for (var i = 0; i <= menus.length; i++) {
        if (!menus[i]) return;
        menus[i].style.display = "block";
    }

    Event.stop(e);
    return false;
}

Horizon.close_menu = function (m) {
    if (Horizon.toplevel_hover_style) {
        m.style.backgroundImage = '';
        m.style.borderColor = '';
        m.style.borderWidth = '';
        m.style.borderStyle = '';
        var links = m.getElementsByTagName('a');
        links[0].style.color = '';
    }

    Horizon.showSelects();
    var menus = m.getElementsByTagName('ul');
    for (var i = 0; i <= menus.length; i++) {
        if (!menus[i]) return;
        menus[i].style.display = "none";
    }

    Event.stop(e);
    return false;
}

Horizon.mouseMove = function (evt) {
    Event.prep(evt);
    if (!evt.target) return;

    var ancestors = DOM.getAncestorsByClassName(evt.target, "NavMenuItem", true);
    if (ancestors.length) {
        if (!Horizon.opened_menu || Horizon.opened_menu != ancestors[0])
            Horizon.open_menu(ancestors[0]);
        if (Horizon.opened_menu && Horizon.opened_menu != ancestors[0])
            Horizon.close_menu(Horizon.opened_menu);
        Horizon.opened_menu = ancestors[0];
    } else {
        if (Horizon.opened_menu) Horizon.close_menu(Horizon.opened_menu);
        Horizon.opened_menu = null;
    }
}

Horizon.hideSelects = function () {
    if (!Horizon.isIE6down) return;
    var sels = document.getElementsByTagName("select");
    var ele;
    for (var i = 0; i < sels.length; i++) {
        ele = sels[i];
        if (!ele) continue;

        if (ele.style.visibility != 'hidden' && ele.className == 'hideable') {
            ele.style.visibility = 'hidden';
            Horizon.hiddenSelects.push(ele);
        }
    }
}

Horizon.showSelects = function () {
    var ele;
    while (ele = Horizon.hiddenSelects.pop())
        ele.style.visibility = '';
}

Horizon.setIsIE6down = function () {
    var UA = navigator.userAgent.toLowerCase();
    if ((UA.indexOf('msie 5') != -1) || (UA.indexOf('msie 6') != -1))
        Horizon.isIE6down = true;
}

/* Try to speed up page load in Mozilla */
if (window.controllers) {
    document.addEventListener("DOMContentLoaded", Horizon.scheme_init, null);
    document.addEventListener("mousemove", Horizon.mouseMove, document);
} else {
    DOM.addEventListener(window, "load", Horizon.scheme_init);
    DOM.addEventListener(document, "mousemove", Horizon.mouseMove);
}

/*
 *  md5.jvs 1.0b 27/06/96
 *
 * Javascript implementation of the RSA Data Security, Inc. MD5
 * Message-Digest Algorithm.
 *
 * Copyright (c) 1996 Henri Torgemane. All Rights Reserved.
 *
 * Permission to use, copy, modify, and distribute this software
 * and its documentation for any purposes and without
 * fee is hereby granted provided that this copyright notice
 * appears in all copies. 
 *
 * Of course, this soft is provided "as is" without express or implied
 * warranty of any kind.
 */



function array(n) {
  for(i=0;i<n;i++) this[i]=0;
  this.length=n;
}

/* Some basic logical functions had to be rewritten because of a bug in
 * Javascript.. Just try to compute 0xffffffff >> 4 with it..
 * Of course, these functions are slower than the original would be, but
 * at least, they work!
 */

function integer(n) { return n%(0xffffffff+1); }

function shr(a,b) {
  a=integer(a);
  b=integer(b);
  if (a-0x80000000>=0) {
    a=a%0x80000000;
    a>>=b;
    a+=0x40000000>>(b-1);
  } else
    a>>=b;
  return a;
}

function shl1(a) {
  a=a%0x80000000;
  if (a&0x40000000==0x40000000)
  {
    a-=0x40000000;  
    a*=2;
    a+=0x80000000;
  } else
    a*=2;
  return a;
}

function shl(a,b) {
  a=integer(a);
  b=integer(b);
  for (var i=0;i<b;i++) a=shl1(a);
  return a;
}

function and(a,b) {
  a=integer(a);
  b=integer(b);
  var t1=(a-0x80000000);
  var t2=(b-0x80000000);
  if (t1>=0) 
    if (t2>=0) 
      return ((t1&t2)+0x80000000);
    else
      return (t1&b);
  else
    if (t2>=0)
      return (a&t2);
    else
      return (a&b);  
}

function or(a,b) {
  a=integer(a);
  b=integer(b);
  var t1=(a-0x80000000);
  var t2=(b-0x80000000);
  if (t1>=0) 
    if (t2>=0) 
      return ((t1|t2)+0x80000000);
    else
      return ((t1|b)+0x80000000);
  else
    if (t2>=0)
      return ((a|t2)+0x80000000);
    else
      return (a|b);  
}

function xor(a,b) {
  a=integer(a);
  b=integer(b);
  var t1=(a-0x80000000);
  var t2=(b-0x80000000);
  if (t1>=0) 
    if (t2>=0) 
      return (t1^t2);
    else
      return ((t1^b)+0x80000000);
  else
    if (t2>=0)
      return ((a^t2)+0x80000000);
    else
      return (a^b);  
}

function not(a) {
  a=integer(a);
  return (0xffffffff-a);
}

/* Here begin the real algorithm */

    var state = new array(4); 
    var count = new array(2);
	count[0] = 0;
	count[1] = 0;                     
    var buffer = new array(64); 
    var transformBuffer = new array(16); 
    var digestBits = new array(16);

    var S11 = 7;
    var S12 = 12;
    var S13 = 17;
    var S14 = 22;
    var S21 = 5;
    var S22 = 9;
    var S23 = 14;
    var S24 = 20;
    var S31 = 4;
    var S32 = 11;
    var S33 = 16;
    var S34 = 23;
    var S41 = 6;
    var S42 = 10;
    var S43 = 15;
    var S44 = 21;

    function F(x,y,z) {
	return or(and(x,y),and(not(x),z));
    }

    function G(x,y,z) {
	return or(and(x,z),and(y,not(z)));
    }

    function H(x,y,z) {
	return xor(xor(x,y),z);
    }

    function I(x,y,z) {
	return xor(y ,or(x , not(z)));
    }

    function rotateLeft(a,n) {
	return or(shl(a, n),(shr(a,(32 - n))));
    }

    function FF(a,b,c,d,x,s,ac) {
        a = a+F(b, c, d) + x + ac;
	a = rotateLeft(a, s);
	a = a+b;
	return a;
    }

    function GG(a,b,c,d,x,s,ac) {
	a = a+G(b, c, d) +x + ac;
	a = rotateLeft(a, s);
	a = a+b;
	return a;
    }

    function HH(a,b,c,d,x,s,ac) {
	a = a+H(b, c, d) + x + ac;
	a = rotateLeft(a, s);
	a = a+b;
	return a;
    }

    function II(a,b,c,d,x,s,ac) {
	a = a+I(b, c, d) + x + ac;
	a = rotateLeft(a, s);
	a = a+b;
	return a;
    }

    function transform(buf,offset) { 
	var a=0, b=0, c=0, d=0; 
	var x = transformBuffer;
	
	a = state[0];
	b = state[1];
	c = state[2];
	d = state[3];
	
	for (i = 0; i < 16; i++) {
	    x[i] = and(buf[i*4+offset],0xff);
	    for (j = 1; j < 4; j++) {
		x[i]+=shl(and(buf[i*4+j+offset] ,0xff), j * 8);
	    }
	}

	/* Round 1 */
	a = FF ( a, b, c, d, x[ 0], S11, 0xd76aa478); /* 1 */
	d = FF ( d, a, b, c, x[ 1], S12, 0xe8c7b756); /* 2 */
	c = FF ( c, d, a, b, x[ 2], S13, 0x242070db); /* 3 */
	b = FF ( b, c, d, a, x[ 3], S14, 0xc1bdceee); /* 4 */
	a = FF ( a, b, c, d, x[ 4], S11, 0xf57c0faf); /* 5 */
	d = FF ( d, a, b, c, x[ 5], S12, 0x4787c62a); /* 6 */
	c = FF ( c, d, a, b, x[ 6], S13, 0xa8304613); /* 7 */
	b = FF ( b, c, d, a, x[ 7], S14, 0xfd469501); /* 8 */
	a = FF ( a, b, c, d, x[ 8], S11, 0x698098d8); /* 9 */
	d = FF ( d, a, b, c, x[ 9], S12, 0x8b44f7af); /* 10 */
	c = FF ( c, d, a, b, x[10], S13, 0xffff5bb1); /* 11 */
	b = FF ( b, c, d, a, x[11], S14, 0x895cd7be); /* 12 */
	a = FF ( a, b, c, d, x[12], S11, 0x6b901122); /* 13 */
	d = FF ( d, a, b, c, x[13], S12, 0xfd987193); /* 14 */
	c = FF ( c, d, a, b, x[14], S13, 0xa679438e); /* 15 */
	b = FF ( b, c, d, a, x[15], S14, 0x49b40821); /* 16 */

	/* Round 2 */
	a = GG ( a, b, c, d, x[ 1], S21, 0xf61e2562); /* 17 */
	d = GG ( d, a, b, c, x[ 6], S22, 0xc040b340); /* 18 */
	c = GG ( c, d, a, b, x[11], S23, 0x265e5a51); /* 19 */
	b = GG ( b, c, d, a, x[ 0], S24, 0xe9b6c7aa); /* 20 */
	a = GG ( a, b, c, d, x[ 5], S21, 0xd62f105d); /* 21 */
	d = GG ( d, a, b, c, x[10], S22,  0x2441453); /* 22 */
	c = GG ( c, d, a, b, x[15], S23, 0xd8a1e681); /* 23 */
	b = GG ( b, c, d, a, x[ 4], S24, 0xe7d3fbc8); /* 24 */
	a = GG ( a, b, c, d, x[ 9], S21, 0x21e1cde6); /* 25 */
	d = GG ( d, a, b, c, x[14], S22, 0xc33707d6); /* 26 */
	c = GG ( c, d, a, b, x[ 3], S23, 0xf4d50d87); /* 27 */
	b = GG ( b, c, d, a, x[ 8], S24, 0x455a14ed); /* 28 */
	a = GG ( a, b, c, d, x[13], S21, 0xa9e3e905); /* 29 */
	d = GG ( d, a, b, c, x[ 2], S22, 0xfcefa3f8); /* 30 */
	c = GG ( c, d, a, b, x[ 7], S23, 0x676f02d9); /* 31 */
	b = GG ( b, c, d, a, x[12], S24, 0x8d2a4c8a); /* 32 */

	/* Round 3 */
	a = HH ( a, b, c, d, x[ 5], S31, 0xfffa3942); /* 33 */
	d = HH ( d, a, b, c, x[ 8], S32, 0x8771f681); /* 34 */
	c = HH ( c, d, a, b, x[11], S33, 0x6d9d6122); /* 35 */
	b = HH ( b, c, d, a, x[14], S34, 0xfde5380c); /* 36 */
	a = HH ( a, b, c, d, x[ 1], S31, 0xa4beea44); /* 37 */
	d = HH ( d, a, b, c, x[ 4], S32, 0x4bdecfa9); /* 38 */
	c = HH ( c, d, a, b, x[ 7], S33, 0xf6bb4b60); /* 39 */
	b = HH ( b, c, d, a, x[10], S34, 0xbebfbc70); /* 40 */
	a = HH ( a, b, c, d, x[13], S31, 0x289b7ec6); /* 41 */
	d = HH ( d, a, b, c, x[ 0], S32, 0xeaa127fa); /* 42 */
	c = HH ( c, d, a, b, x[ 3], S33, 0xd4ef3085); /* 43 */
	b = HH ( b, c, d, a, x[ 6], S34,  0x4881d05); /* 44 */
	a = HH ( a, b, c, d, x[ 9], S31, 0xd9d4d039); /* 45 */
	d = HH ( d, a, b, c, x[12], S32, 0xe6db99e5); /* 46 */
	c = HH ( c, d, a, b, x[15], S33, 0x1fa27cf8); /* 47 */
	b = HH ( b, c, d, a, x[ 2], S34, 0xc4ac5665); /* 48 */

	/* Round 4 */
	a = II ( a, b, c, d, x[ 0], S41, 0xf4292244); /* 49 */
	d = II ( d, a, b, c, x[ 7], S42, 0x432aff97); /* 50 */
	c = II ( c, d, a, b, x[14], S43, 0xab9423a7); /* 51 */
	b = II ( b, c, d, a, x[ 5], S44, 0xfc93a039); /* 52 */
	a = II ( a, b, c, d, x[12], S41, 0x655b59c3); /* 53 */
	d = II ( d, a, b, c, x[ 3], S42, 0x8f0ccc92); /* 54 */
	c = II ( c, d, a, b, x[10], S43, 0xffeff47d); /* 55 */
	b = II ( b, c, d, a, x[ 1], S44, 0x85845dd1); /* 56 */
	a = II ( a, b, c, d, x[ 8], S41, 0x6fa87e4f); /* 57 */
	d = II ( d, a, b, c, x[15], S42, 0xfe2ce6e0); /* 58 */
	c = II ( c, d, a, b, x[ 6], S43, 0xa3014314); /* 59 */
	b = II ( b, c, d, a, x[13], S44, 0x4e0811a1); /* 60 */
	a = II ( a, b, c, d, x[ 4], S41, 0xf7537e82); /* 61 */
	d = II ( d, a, b, c, x[11], S42, 0xbd3af235); /* 62 */
	c = II ( c, d, a, b, x[ 2], S43, 0x2ad7d2bb); /* 63 */
	b = II ( b, c, d, a, x[ 9], S44, 0xeb86d391); /* 64 */

	state[0] +=a;
	state[1] +=b;
	state[2] +=c;
	state[3] +=d;

    }

    function init() {
	count[0]=count[1] = 0;
	state[0] = 0x67452301;
	state[1] = 0xefcdab89;
	state[2] = 0x98badcfe;
	state[3] = 0x10325476;
	for (i = 0; i < digestBits.length; i++)
	    digestBits[i] = 0;
    }

    function update(b) { 
	var index,i;
	
	index = and(shr(count[0],3) , 0x3f);
	if (count[0]<0xffffffff-7) 
	  count[0] += 8;
        else {
	  count[1]++;
	  count[0]-=0xffffffff+1;
          count[0]+=8;
        }
	buffer[index] = and(b,0xff);
	if (index  >= 63) {
	    transform(buffer, 0);
	}
    }

    function finish() {
	var bits = new array(8);
	var	padding; 
	var	i=0, index=0, padLen=0;

	for (i = 0; i < 4; i++) {
	    bits[i] = and(shr(count[0],(i * 8)), 0xff);
	}
        for (i = 0; i < 4; i++) {
	    bits[i+4]=and(shr(count[1],(i * 8)), 0xff);
	}
	index = and(shr(count[0], 3) ,0x3f);
	padLen = (index < 56) ? (56 - index) : (120 - index);
	padding = new array(64); 
	padding[0] = 0x80;
        for (i=0;i<padLen;i++)
	  update(padding[i]);
        for (i=0;i<8;i++) 
	  update(bits[i]);

	for (i = 0; i < 4; i++) {
	    for (j = 0; j < 4; j++) {
		digestBits[i*4+j] = and(shr(state[i], (j * 8)) , 0xff);
	    }
	} 
    }

/* End of the MD5 algorithm */

function hexa(n) {
 var hexa_h = "0123456789abcdef";
 var hexa_c=""; 
 var hexa_m=n;
 for (hexa_i=0;hexa_i<8;hexa_i++) {
   hexa_c=hexa_h.charAt(Math.abs(hexa_m)%16)+hexa_c;
   hexa_m=Math.floor(hexa_m/16);
 }
 return hexa_c;
}


var ascii="01234567890123456789012345678901" +
          " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ"+
          "[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

function MD5(entree) 
{
 var l,s,k,ka,kb,kc,kd;

 init();
 for (k=0;k<entree.length;k++) {
   l=entree.charAt(k);
   update(ascii.lastIndexOf(l));
 }
 finish();
 ka=kb=kc=kd=0;
 for (i=0;i<4;i++) ka+=shl(digestBits[15-i], (i*8));
 for (i=4;i<8;i++) kb+=shl(digestBits[15-i], ((i-4)*8));
 for (i=8;i<12;i++) kc+=shl(digestBits[15-i], ((i-8)*8));
 for (i=12;i<16;i++) kd+=shl(digestBits[15-i], ((i-12)*8));
 s=hexa(kd)+hexa(kc)+hexa(kb)+hexa(ka);
 return s; 
}

/* This implement the MD5 test suite */
var testOk=false;
function teste() {
 if (testOk) return;
 document.test.o1.value=MD5(document.test.i1.value);
 document.test.o2.value=MD5(document.test.i2.value);
 document.test.o3.value=MD5(document.test.i3.value);
 document.test.o4.value=MD5(document.test.i4.value);
 document.test.o5.value=MD5(document.test.i5.value);
 document.test.o6.value=MD5(document.test.i6.value);
 document.test.o7.value=MD5(document.test.i7.value);
 testOk=true;
}
// add chal/resp auth to the "login" form if it exists
// this requires md5.js
LiveJournal.setUpLoginForm = function () {
    var loginForms = DOM.getElementsByTagAndClassName(document, "form", "lj_login_form") || [];

    for (var i=0; i<loginForms.length; i++) {
        var loginForm = loginForms[i];
        DOM.addEventListener(loginForm, "submit", LiveJournal.loginFormSubmitted.bindEventListener(loginForm));
    }
}

// When the login form is submitted, compute the challenge response and clear out the plaintext password field
LiveJournal.loginFormSubmitted = function (evt) {
    var loginform = evt.target;
    if (! loginform)
        return true;

    var chal_field = LiveJournal.loginFormGetField(loginform, "lj_login_chal");
    var resp_field = LiveJournal.loginFormGetField(loginform, "lj_login_response");
    var pass_field = LiveJournal.loginFormGetField(loginform, "lj_login_password");

    if (! chal_field || ! resp_field || ! pass_field)
        return true;

    var pass = pass_field.value;
    var chal = chal_field.value;
    var res = MD5(chal + MD5(pass));
    resp_field.value = res;
    pass_field.value = "";  // dont send clear-text password!
    return true;
}

LiveJournal.loginFormGetField = function (loginform, field) {
    var formChildren = loginform.getElementsByTagName("input");
    var loginFields = DOM.filterElementsByClassName(formChildren, field);
    return loginFields[0];
};

LiveJournal.register_hook("page_load", LiveJournal.setUpLoginForm);
// ljtalk for ctxpopup
LiveJournal.register_hook("ctxpopup_extrainfo", function (userdata) {
    var content = document.createElement("div");

    if (userdata.is_person) {
        if (userdata.is_online !== '') {
            // online status
            var onlineStatusLabel = document.createElement("span");
            var jabberTitle = userdata.jabber_title;
            onlineStatusLabel.innerHTML = jabberTitle + ": ";
            DOM.addClassName(onlineStatusLabel, "OnlineStatus");
            content.appendChild(onlineStatusLabel);

            // build status
            var onlineStatus = document.createElement("span");


            var onlineStatusText = document.createElement("span");
            onlineStatus.appendChild(onlineStatusText);

            content.appendChild(onlineStatus);

            if (userdata.is_online) {
                onlineStatusText.innerHTML = "Online";
            } else if (userdata.is_online == '0') {
                onlineStatusText.innerHTML = "Offline";
            }
        }
    }

    return content;
});

// for updating ljcom widgets from livejournal ones
LiveJournal.register_hook("update_other_widgets", function (from_widget) {
    if (from_widget == "LayoutChooser" && Customize.AdLayout) {
        Customize.AdLayout.updateContent();
    }
});
