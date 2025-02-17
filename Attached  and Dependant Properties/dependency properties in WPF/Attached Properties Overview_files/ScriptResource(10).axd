﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

Type.registerNamespace('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation');

// Create an alias for the namespace to save 25 chars each time it's used since
// this is a very long script and will take awhile to download
var $AA = Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation;

$AA.registerAnimation = function(name, type) {
    /// <summary>
    /// Register an animation with the AJAX Control Toolkit animation framework. This serves a dual purpose:
    /// 1) to add standard utility methods to the animation type (such as a <code>play</code> method that creates
    /// an animation, plays it, and disposes it when the animation is over), and 2) associate a name with the
    /// type that will be used when creating animations from a JSON description.  This method can also be called
    /// by other animation libraries to seamlessly interoperate with the AJAX Control Toolkit's animation
    /// framework.
    /// </summary>
    /// <param name="name" type="String">
    /// Name of the animation that will be used as the XML tag name in the XML animation description.  It
    /// should be a valid XML tag (i.e. an alpha-numeric sequence with no spaces, special characters, etc.).
    /// </param>
    /// <param name="type" type="Type">
    /// The type of the new animation must inherit from <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" />.
    /// </param>
    /// <returns />

    // Make sure the type inherits from Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation
    if (type && ((type === $AA.Animation) || (type.inheritsFrom && type.inheritsFrom($AA.Animation)))) {
        // We'll store the animation name/type mapping in a "static" object off of
        // Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.  If this __animations object hasn't been
        // created yet, demand create it on the first registration.
        if (!$AA.__animations) {
            $AA.__animations = { };
        }
        
        // Add the current type to the collection of animations
        $AA.__animations[name.toLowerCase()] = type;
        
        // Add a play function that will make it very easy to create, play, and
        // dispose of an animation.  This is effectively a "static" function on
        // each animation and will take the same parameters as that animation's
        // constructor.
        type.play = function() {
            /// <summary>
            /// Create an animation, play it immediately, and dispose it when finished.
            /// </summary>
            /// <param parameterArray="true" elementType="Object">
            /// The play function takes the same parameters as the type's constructor
            /// </param>
            /// <returns />
            
            // Create and initialize a new animation of the right type and pass in
            // any arguments given to the play function
            var animation = new type();
            type.apply(animation, arguments);
            animation.initialize();
            
            // Add an event handler to dispose the animation when it's finished
            var handler = Function.createDelegate(animation,
                function() {
                    animation.remove_ended(handler);
                    handler = null;
                    animation.dispose();
                });
            animation.add_ended(handler);
            
            // Once the animation has been created and initialized, play it and
            // dispose it as soon as it's finished
            animation.play();
        }
    } else {
        // Raise an error if someone registers an animation that doesn't inherit
        // from our base Animation class
        throw Error.argumentType('type', type, $AA.Animation,
            'Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.registerAnimation can only register types that inherit from Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation');
    }
}

$AA.buildAnimation = function(json, defaultTarget) {
    /// <summary>
    /// The buildAnimation function is used to turn a JSON animation description into an
    /// actual animation object that can be played.
    /// </summary>
    /// <param name="json" type="String">
    /// JSON description of the animation in the format expected by createAnimation
    /// </param>
    /// <param name="defaultTarget" type="Sys.UI.DomElement">
    /// Target of the animation if none is specified in the JSON description.  The semantics of
    /// target assignment are provided in more detail in createAnimation.
    /// </param>
    /// <returns type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation">
    /// Animation created from the JSON description
    /// </returns>

    // Ensure we have a description to create an animation with
    if (!json || json === '') {
        return null;
    }

    // "Parse" the JSON so we can easily manipulate it
    // (we don't wrap it in a try/catch when debugging to raise any errors)
    var obj;
    json = '(' + json + ')';
    if (!window.debug.isDebug) {
        try { obj = eval(json); } catch (ex) { } 
    } else {
        obj = eval(json);
    }
    
    // Create a new instance of the animation
    return $AA.createAnimation(obj, defaultTarget);    
}

$AA.createAnimation = function(obj, defaultTarget) {
    /// <summary>
    /// The <code>createAnimation</code> function builds a new <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" />
    /// object from an object that describes it.
    /// </summary>
    /// <remarks>
    /// Exceptions are thrown when the <code>AnimationName</code> cannot be found or when a specific target cannot be found
    /// using <code>$get()</code> or <code>$find()</code>. We do not raise exceptions for any properties that have no match
    /// in the animation's properties.
    /// </remarks>
    /// <param name="obj" type="Object">
    /// The object provides a description of the animation to be be generated in a very specific format. It has three
    /// special properties: <code>AnimationName</code>, <code>AnimationTarget</code>, and <code>AnimationChildren</code>.
    /// The <code>AnimationName</code> is required and used to find the type of animation to create (this name should map to
    /// one of the animation names supplied to <code>registerAnimation</code>).  The <code>AnimationTarget</code> is optional
    /// and specifies the name of a <see cref="Sys.UI.DomElement" /> that the animation should target.  The
    /// <code>AnimationChildren</code> property supplies an optional array for animations that use child animations (such as
    /// <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation" /> and <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.SequenceAnimation" />).
    /// The elements of the <code>AnimationChildren</code> array are valid <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" /> objects
    /// that meet these same requirements.  In order for an animation to support child animations, it must derive from the
    /// <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParentAnimation" /> class which provides common methods like <code>add</code>,
    /// <code>clear</code>, etc. The remaining properties of the object are used to set parameters specific to the type of animation
    /// being created (e.g. <code>duration</code>, <code>minimumOpacity</code>, <code>startValue</code>, etc.) and should have a corresponding
    /// property on the animation.
    /// </param>
    /// <param name="defaultTarget" type="Sys.UI.DomElement">
    /// The function also takes a <code>defaultTarget</code> parameter that is used as the target of the animation if the object does not
    /// specify one.  This parameter should be an instance of <see cref="Sys.UI.DomElement" /> and not just the name of an element.  Furthermore,
    /// when recursively building any child animations (from the elements of the <code>AnimationChildren</code> array), the target of the
    /// parent animation will be passed into the function as the <code>defaultTarget</code> for the child - so all child animations will have
    /// the same target as their parent unless explicitly specified using the child's <code>AnimationTarget</code>.
    /// </param>
    /// <returns type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation">Animation created from the description</returns>

    // Create a default instance of the animation by looking up the AnimationName in the global __animations object.
    if (!obj || !obj.AnimationName) {
        throw Error.argument('obj', 'Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.createAnimation requires an object with an AnimationName property');
    }
    var type = $AA.__animations[obj.AnimationName.toLowerCase()];
    if (!type) {
        throw Error.argument('type', 'Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.createAnimation could not find an Animation corresponding to the name ' + obj.AnimationName);
    }
    var animation = new type();
    
    // Set the animation's target by first trying to find an AnimationTarget property to use,
    // but relying on the defaultTarget if nothing else is found.
    var target = defaultTarget;
    if (obj.AnimationTarget) { 
        // Try to find a Sys.UI.DomElement
        var element = $get(obj.AnimationTarget);
        if (element) {
            target = element;
        } else {
           // Try to find the control in the AJAX controls collection
            var ctrl = $find(obj.AnimationTarget);
            if (ctrl) {
                element = ctrl.get_element();
                if (element) {
                    target = element;
                }
            }
        }
    }
    animation.set_target(target);
    
    // If there is an AnimationChildren array and the animation inherits from
    // ParentAnimation, then we will recusively build the child animations.  We will
    // use the target of this animation as the defaultTarget.  It is very important
    // that we create the child animations before setting the animation's properties
    // or initializing (because some properties and initialization will be propogated
    // down from parent to child).
    if (obj.AnimationChildren && obj.AnimationChildren.length) {
        if ($AA.ParentAnimation.isInstanceOfType(animation)) {
            for (var i = 0; i < obj.AnimationChildren.length; i++) {
                var child = $AA.createAnimation(obj.AnimationChildren[i], target);
                if (child) {
                    animation.add(child);
                }
            }
        } else {
            throw Error.argument('obj', 'Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.createAnimation cannot add child animations to type ' +
                type.getName() + ' that does not derive from Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParentAnimation');
        }
    }
    
    // Set any additional properties of the animation that were provided in the object. (Since the object
    // may have a lot of crud in addition to valid properties, and we don't want to handle exceptions
    // thrown by trying to set the properties, we will only copy properties that we know are defined on
    // the type.  To do this, we create a mapping of lowercase names of valid properties to their setters.)
    var properties = type.__animationProperties;
    if (!properties) {
        // Get the properties for this type by walking its prototype - by doing
        // this we'll effectively ignore anything not defined in the prototype
        type.__animationProperties = { };
        type.resolveInheritance();
        for (var name in type.prototype) {
            if (name.startsWith('set_')) {
                type.__animationProperties[name.substr(4).toLowerCase()] = name;
            }
        }
        
        // Remove the id and target properties as these shouldn't be set by the animation
        delete type.__animationProperties['id'];
        delete type.__animationProperties['target'];
        properties = type.__animationProperties;
    }
    
    // Loop through each of the properties in the object and check if it's in the list of valid
    // property names.  We will check the type of the propertyName to make sure it's a String (as other
    // types can be added by the ASP.NET AJAX compatability layers to all objects and cause errors if
    // you don't exclude them)
    for (var property in obj) {
        try {
            var propertyName = properties[property.toLowerCase()];
            if (propertyName && String.isInstanceOfType(propertyName) && animation[propertyName]) {
                animation[propertyName](obj[property]);
            }
        } catch (ex) { 
            // Ignore any exceptions that get raised when setting properties 
        }
    }
            
    return animation;
}


// In the Xml comments for each of the animations below, there is a special <animation /> tag
// that is used to define how the animation is referenced from a generic XML animation description


$AA.Animation = function(target, duration, fps) {
    /// <summary>
    /// <code>Animation</code> is an abstract base class used as a starting point for all the other animations.
    /// It provides the basic mechanics for the animation (playing, pausing, stopping, timing, etc.)
    /// and leaves the actual animation to be done in the abstract methods <code>getAnimatedValue</code>
    /// and <code>setValue</code>.
    /// </summary>
    /// <remarks>
    /// Animations need to be as fast as possible - even in debug mode.  Don't add validation code to
    /// methods involved in every step of the animation.
    /// </remarks>
    /// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
    /// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
    /// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
    /// <animation>Animation</animation>
    $AA.Animation.initializeBase(this);
    
    // Length of the animation in seconds
    this._duration = 1;
    
    // Number of steps per second
    this._fps = 25;
    
    // Target Sys.UI.DomElement of the animation
    this._target = null;
    
    // Tick event handler
    this._tickHandler = null;
    
    // Animation timer
    this._timer = null;
    
    // Percentage of the animation already played
    this._percentComplete = 0;
    
    // Percentage of the animation to play on each step
    this._percentDelta = null;
    
    // Reference to the animation that contains this as a child animation
    this._parentAnimation = null;
    
    // Set the target, duration, and fps if they were provided in the constructor
    if (target) {
        this.set_target(target);
    }
    if (duration) {
        this.set_duration(duration);
    }
    if (fps) { 
        this.set_fps(fps);
    }
}
$AA.Animation.prototype = {
    dispose : function() {
        /// <summary>
        /// Dispose the animation
        /// </summary>
        /// <returns />
        
        if (this._timer) {
            this._timer.dispose();
            this._timer = null;
        }
        
        this._tickHandler = null;
        this._target = null;
        
        $AA.Animation.callBaseMethod(this, 'dispose');
    },
    
    stop : function() {
        /// <summary>
        /// Stop playing the animation.
        /// </summary>
        /// <remarks>
        /// Stopping an animation will perform the last step, thereby leaving the
        /// target element in a state consistent with the animation playing completely.
        /// If this animation is the child of another, you must call <code>stop</code> on
        /// its parent instead.
        /// </remarks>
        /// <returns />
        
        if (!this._parentAnimation) {
            var t = this._timer;
            this._timer = null;
            if (t) {
                t.dispose();
                
                this._updatePercentComplete(100);
                this.onEnd();
                
                this.raisePropertyChanged('isPlaying');
                this.raisePropertyChanged('isActive');
            }
        }
    },
    
    pause : function() {
        /// <summary>
        /// Pause the animation if it is playing.  Calling <code>play</code> will resume where
        /// the animation left off.
        /// </summary>
        /// <remarks>
        /// If this animation is the child of another, you must call <code>pause</code> on its parent instead.
        /// </remarks>
        /// <returns />
        
        if (!this._parentAnimation) {
            if (this._timer) {
                this._timer.set_enabled(false);
                
                this.raisePropertyChanged('isPlaying');
            }
        }
    },
    
    play : function() {
        /// <summary>
        /// Play the animation from the beginning or where it was left off when paused.
        /// </summary>
        /// <remarks>
        /// If this animation is the child of another, you must call <code>play</code> on its parent instead.
        /// </remarks>
        /// <returns />
        
        // If ownership of this animation has been claimed, then we'll require the parent to
        // handle playing the animation (this is very important because then the entire animation
        // tree runs on the same timer and updates consistently)
        if (!this._parentAnimation) {
            var resume = true;
            if (!this._timer) {
                resume = false;
                
                if (!this._tickHandler) {
                    this._tickHandler = Function.createDelegate(this, this._onTimerTick);
                }

                this._timer = new Sys.Timer();
                this._timer.add_tick(this._tickHandler);
               
                this.onStart();
                
                this._timer.set_interval(1000 / this._fps);
                this._percentDelta = 100 / (this._duration * this._fps);
                this._updatePercentComplete(0, true);
            }

            this._timer.set_enabled(true);
            
            this.raisePropertyChanged('isPlaying');
            if (!resume) {
                this.raisePropertyChanged('isActive');
            }
        }
    },
    
    onStart : function() {
        /// <summary>
        /// The <code>onStart</code> method is called just before the animation is played each time.
        /// </summary>
        /// <returns />
        
        this.raiseStarted();
    },
    
    onStep : function(percentage) {
        /// <summary>
        /// The <code>onStep</code> method is called repeatedly to progress the animation through each frame
        /// </summary>
        /// <param name="percentage" type="Number">Percentage of the animation already complete</param>
        /// <returns />
        
        this.setValue(this.getAnimatedValue(percentage));
    },
    
    onEnd : function() {
        /// <summary>
        /// The <code>onEnd</code> method is called just after the animation is played each time.
        /// </summary>
        /// <returns />
        
        this.raiseEnded();
    },
    
    getAnimatedValue : function(percentage) {
        /// <summary>
        /// Determine the state of the animation after the given percentage of its duration has elapsed
        /// </summary>
        /// <param name="percentage" type="Number">Percentage of the animation already complete</param>
        /// <returns type="Object">
        /// State of the animation after the given percentage of its duration has elapsed that will
        /// be passed to <code>setValue</code>
        /// </returns>
        throw Error.notImplemented();
    },
    
    setValue : function(value) {
        /// <summary>
        /// Set the current state of the animation
        /// </summary>
        /// <param name="value" type="Object">Current state of the animation (as retreived from <code>getAnimatedValue</code>)</param>
        /// <returns />
        throw Error.notImplemented();
    },
    
    interpolate : function(start, end, percentage) {
        /// <summary>
        /// The <code>interpolate</code> function is used to find the appropriate value between starting and
        /// ending values given the current percentage.
        /// </summary>
        /// <remarks>
        /// In the future, we hope to make several implementations of this available so we can dynamically
        /// change the apparent speed of the animations, although it may make more sense to modify the
        /// <code>_updatePercentComplete</code> function instead.
        /// </remarks>
        /// <param name="start" type="Number">Start of the range to interpolate</param>
        /// <param name="end" type="Number">End of the range to interpolate</param>
        /// <param name="percentage" type="Number">Percentage completed in the range to interpolate</param>
        /// <returns type="Number">Value the desired percentage between the start and end values</returns>
        return start + (end - start) * (percentage / 100);
    },
    
    _onTimerTick : function() {
        /// <summary>
        /// Handler for the tick event to move the animation along through its duration
        /// </summary>
        /// <returns />
        this._updatePercentComplete(this._percentComplete + this._percentDelta, true);
    },
    
    _updatePercentComplete : function(percentComplete, animate) {
        /// <summary>
        /// Update the animation and its target given the current percentage of its duration that
        /// has already elapsed
        /// </summary>
        /// <param name="percentComplete" type="Number">
        /// Percentage of the animation duration that has already elapsed
        /// </param>
        /// <param name="animate" type="Boolean" optional="true">
        /// Whether or not updating the animation should visually modify the animation's target
        /// </param>
        /// <returns />
        
        if (percentComplete > 100) {
            percentComplete = 100;
        }
        
        this._percentComplete = percentComplete;
        this.raisePropertyChanged('percentComplete');
        
        if (animate) {
            this.onStep(percentComplete);
        }
        
        if (percentComplete === 100) {
            this.stop();
        }
    },
    
    setOwner : function(owner) {
        /// <summary>
        /// Make this animation the child of another animation
        /// </summary>
        /// <param name="owner" type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation">Parent animation</param>
        this._parentAnimation = owner;
    },
    
    raiseStarted : function() {
        /// <summary>
        /// Raise the <code>started</code> event
        /// </summary>
        var handlers = this.get_events().getHandler('started');
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_started : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>started</code> event.
        /// </summary>
        /// <param name="handler" type="Function">The handler to add to the event.</param>
        this.get_events().addHandler("started", handler);
    },
    
    remove_started : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>started</code> event.
        /// </summary>
        /// <param name="handler" type="Function">The handler to remove from the event.</param>
        this.get_events().removeHandler("started", handler);
    },
    
    raiseEnded : function() {
        /// <summary>
        /// Raise the <code>ended</code> event
        /// </summary>
        var handlers = this.get_events().getHandler('ended');
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    add_ended : function(handler) {
        /// <summary>
        /// Adds an event handler for the <code>ended</code> event.
        /// </summary>
        /// <param name="handler" type="Function">The handler to add to the event.</param>
        this.get_events().addHandler("ended", handler);
    },
    
    remove_ended : function(handler) {
        /// <summary>
        /// Removes an event handler for the <code>ended</code> event.
        /// </summary>
        /// <param name="handler" type="Function">The handler to remove from the event.</param>
        this.get_events().removeHandler("ended", handler);
    },
    
    get_target : function() {
        /// <value type="Sys.UI.DomElement">
        /// Target of the animation
        /// </value>
        /// <remarks>
        /// Do not set this property in a generic Xml animation description. It will be set automatically
        /// using either the extender's TargetControlID or the AnimationTarget property.
        /// <remarks>
        return this._target;
    },
    set_target : function(value) {
        if (this._target != value) {
            this._target = value;
            this.raisePropertyChanged('target');
        }
    },
    
    get_duration : function() {
        /// <value type="Number">
        /// Length of the animation in seconds.  The default is 1.
        /// </value>
        return this._duration;
    },
    set_duration : function(value) {
        value = this._getFloat(value);
        if (this._duration != value) {
            this._duration = value;
            this.raisePropertyChanged('duration');
        }
    },
    
    get_fps : function() {
        /// <value type="Number">
        /// Number of steps per second.  The default is 25.
        /// </value>
        return this._fps;
    },
    set_fps : function(value) {
        value = this._getInteger(value);
        if (this.fps != value) {
            this._fps = value;
            this.raisePropertyChanged('fps');
        }
    },
    
    get_isActive : function() {
        /// <value type="Boolean">
        /// <code>true</code> if animation is active, <code>false</code> if not.
        /// </value>
        return (this._timer !== null);
    },
    
    get_isPlaying : function() {
        /// <value type="Boolean">
        /// <code>true</code> if animation is playing, <code>false</code> if not.
        /// </value>
        return (this._timer !== null) && this._timer.get_enabled();
    },
    
    get_percentComplete : function() {
        /// <value type="Number">
        /// Percentage of the animation already played.
        /// </value>
        return this._percentComplete;
    },
    
    _getBoolean : function(value) {
        /// <summary>
        /// Helper to convert strings to booleans for property setters
        /// </summary>
        /// <param name="value" type="Object">Value to convert if it's a string</param>
        /// <returns type="Object">Value that has been converted if it was a string</returns>
        if (String.isInstanceOfType(value)) {
            return Boolean.parse(value);
        }
        return value;
    },
    
    _getInteger : function(value) {
        /// <summary>
        /// Helper to convert strings to integers for property setters
        /// </summary>
        /// <param name="value" type="Object">Value to convert if it's a string</param>
        /// <returns type="Object">Value that has been converted if it was a string</returns>
        if (String.isInstanceOfType(value)) {
            return parseInt(value);
        }
        return value;
    },
    
    _getFloat : function(value) {
        /// <summary>
        /// Helper to convert strings to floats for property setters
        /// </summary>
        /// <param name="value" type="Object">Value to convert if it's a string</param>
        /// <returns type="Object">Value that has been converted if it was a string</returns>
        if (String.isInstanceOfType(value)) {
            return parseFloat(value);
        }
        return value;
    },
    
    _getEnum : function(value, type) {
        /// <summary>
        /// Helper to convert strings to enum values for property setters
        /// </summary>
        /// <param name="value" type="Object">Value to convert if it's a string</param>
        /// <param name="type" type="Type">Type of the enum to convert to</param>
        /// <returns type="Object">Value that has been converted if it was a string</returns>
        if (String.isInstanceOfType(value) && type && type.parse) {
            return type.parse(value);
        }
        return value;
    }
}
$AA.Animation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation', Sys.Component);
$AA.registerAnimation('animation', $AA.Animation);


$AA.ParentAnimation = function(target, duration, fps, animations) {
	/// <summary>
    /// The <code>ParentAnimation</code> serves as a base class for all animations that contain children (such as
    /// <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation" />, <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.SequenceAnimation" />,
    /// etc.).  It does not actually play the animations, so any classes that inherit from it must do so.  Any animation
    /// that requires nested child animations must inherit from this class, although it will likely want to inherit off of
    /// <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation" /> or <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.SequenceAnimation" />
    /// which will actually play their child animations.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="animations" elementType="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" mayBeNull="true" optional="true">
	/// Array of child animations to be played
	/// </param>
	/// <animation>Parent</animation>
    $AA.ParentAnimation.initializeBase(this, [target, duration, fps]);
    
    // Array of child animations (there are no assumptions placed on order because
    // it will matter for some derived animations like SequenceAnimation, but not
    // for others like ParallelAnimation) that is demand created in add
    this._animations = [];
    
    // Add any child animations passed into the constructor
    if (animations && animations.length) {
        for (var i = 0; i < animations.length; i++) {
            this.add(animations[i]);
        }
    }
}
$AA.ParentAnimation.prototype = {
    initialize : function() {
    	/// <summary>
        /// Initialize the parent along with any child animations that have not yet been initialized themselves
    	/// </summary>
    	/// <returns />
        $AA.ParentAnimation.callBaseMethod(this, 'initialize');
        
        // Initialize all the uninitialized child animations
        if (this._animations) {
            for (var i = 0; i < this._animations.length; i++) {
                var animation = this._animations[i];
                if (animation && !animation.get_isInitialized) {
                    animation.initialize();
                }
            }
        }
    },
    
    dispose : function() {
    	/// <summary>
        /// Dispose of the child animations
    	/// </summary>
    	/// <returns />

        this.clear();
        this._animations = null;
        $AA.ParentAnimation.callBaseMethod(this, 'dispose');
    },
    
    get_animations : function() {
    	/// <value elementType="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation">
        /// Array of child animations to be played (there are no assumptions placed on order because it will matter for some
        /// derived animations like <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.SequenceAnimation" />, but not for
        /// others like <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation" />).  To manipulate the child
        /// animations, use the functions <code>add</code>, <code>clear</code>, <code>remove</code>, and <code>removeAt</code>.
    	/// </value>
        return this._animations;
    },
    
    add : function(animation) {
    	/// <summary>
        /// Add an animation as a child of this animation.
    	/// </summary>
    	/// <param name="animation" type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation">Child animation to add</param>
    	/// <returns />

        if (this._animations) {
            Array.add(this._animations, animation);
            this.raisePropertyChanged('animations');
        }
    },
    
    remove : function(animation) {
    	/// <summary>
        /// Remove the animation from the array of child animations.
    	/// </summary>
    	/// <remarks>
    	/// This will dispose the removed animation.
    	/// </remarks>
    	/// <param name="animation" type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation">Child animation to add</param>
    	/// <returns />

        if (this._animations) {
            if (animation) {
                animation.dispose();
            }
            Array.remove(this._animations, animation);            
            this.raisePropertyChanged('animations');
        }
    },
    
    removeAt : function(index) {
    	/// <summary>
        /// Remove the animation at a given index from the array of child animations.
    	/// </summary>
    	/// <param name="index" type="Number">Index of the child animation</param>
    	/// <returns />
        
        if (this._animations) {
            var animation = this._animations[index];
            if (animation) {
                animation.dispose();
            }
            this._animations.removeAt(index);
            this.raisePropertyChanged('animations');
        }
    },
    
    clear : function() {
    	/// <summary>
        /// Clear the array of child animations.
    	/// </summary>
    	/// <remarks>
    	/// This will dispose the cleared child animations.
    	/// </remarks>
    	/// <returns />

        if (this._animations) {
            for (var i = this._animations.length - 1; i >= 0; i--) {
                this._animations[i].dispose();
                this._animations[i] = null;
            }
            Array.clear(this._animations);            
            this._animations = [];
            this.raisePropertyChanged('animations');
        }
    },
    
    setChildTargets : function(target, force) {
    	/// <summary>
        /// Set the target of any child animations with no target defined (or to all the children
        /// if <code>force</code> is <code>true</code>).
    	/// </summary>
    	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target for the child animations</param>
    	/// <param name="force" type="Boolean"></param>
    	/// <returns />

        if (this._animations) {
            for (var i = 0; i < this._animations.length; i++) {
                var animation = this._animations[i];
                if (force || !animation.get_target()) {
                    animation.set_target(target);
                }
            }
        }
    }
}
$AA.ParentAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParentAnimation', $AA.Animation);
$AA.registerAnimation('parent', $AA.ParentAnimation);


$AA.ParallelAnimation = function(target, duration, fps, animations) {
	/// <summary>
    /// The <code>ParallelAnimation</code> plays several animations simultaneously.  It inherits from
    /// <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParentAnimation" />, but makes itself the owner of all
    /// its child animations to allow the use a single timer and syncrhonization mechanisms shared with
    /// all the children (in other words, the <code>duration</code> properties of any child animations
    /// are ignored in favor of the parent's <code>duration</code>).  It is very useful in creating
    /// sophisticated effects through combination of simpler animations.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="animations" elementType="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" mayBeNull="true" optional="true">
	/// Array of child animations
	/// </param>
	/// <animation>Parallel</animation>
    $AA.ParallelAnimation.initializeBase(this, [target, duration, fps, animations]);
}
$AA.ParallelAnimation.prototype = {
    add : function(animation) {
    	/// <summary>
        /// Add an animation as a child of this animation and make ourselves its owner.
    	/// </summary>
    	/// <param name="animation" type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation">Child animation to add</param>
    	/// <returns />
        $AA.ParallelAnimation.callBaseMethod(this, 'add', [animation]);
        animation.setOwner(this);
    },
    
    onStart : function() {
    	/// <summary>
        /// Get the child animations ready to play.
    	/// </summary>
    	/// <returns />

        $AA.ParallelAnimation.callBaseMethod(this, 'onStart');
        var animations = this.get_animations();
        for (var i = 0; i < animations.length; i++) {
            animations[i].onStart();
        }
    },
    
    onStep : function(percentage) {
    	/// <summary>
        /// Progress the child animations through each frame.
        /// </summary>
        /// <param name="percentage" type="Number">Percentage of the animation already complete</param>
        /// <returns />

        var animations = this.get_animations();
        for (var i = 0; i < animations.length; i++) {
            animations[i].onStep(percentage);
        }
    },
    
    onEnd : function() {
    	/// <summary>
        /// Finish playing all of the child animations.
    	/// </summary>
    	/// <returns />

        var animations = this.get_animations();
        for (var i = 0; i < animations.length; i++) {
            animations[i].onEnd();
        }
        $AA.ParallelAnimation.callBaseMethod(this, 'onEnd');
    },
    
    set_target : function(value) {
    	/// <value type="Sys.UI.DomElement">
        /// Override the <code>target</code> property to pass it through to any
        /// child animations that have no target defined
        /// </value>
        /// <remarks>
        /// Do not set this property in a generic Xml animation description. It will be set automatically
        /// using either the extender's TargetControlID or the AnimationTarget property.
        /// <remarks>
        $AA.ParallelAnimation.callBaseMethod(this, 'set_target', [value]);
        this.setChildTargets(value);
    }
}
$AA.ParallelAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation', $AA.ParentAnimation);
$AA.registerAnimation('parallel', $AA.ParallelAnimation);


$AA.SequenceAnimation = function(target, duration, fps, animations, iterations) {
	/// <summary>
    /// The <code>SequenceAnimation</code> runs several animations one after the other.  It can also
    /// repeat the sequence of animations for a specified number of iterations (which defaults to a
    /// single iteration, but will repeat forever if you specify zero or less iterations).  Also, the
    /// <code>SequenceAnimation</code> cannot be a child of a <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation" />
    /// (or any animation inheriting from it).
	/// </summary>
	/// <remarks>
	/// The <code>SequenceAnimation</code> ignores the <code>duration</code> and <code>fps</code>
	/// properties, and will let each of its child animations use any settings they please.
	/// </remarks>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="animations" elementType="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" mayBeNull="true" optional="true">Array of child animations</param>
	/// <param name="iterations" type="Number" mayBeNull="true" optional="true">
	/// Number of times to repeatedly play the sequence.  If zero or less iterations are specified, the sequence
	/// will repeat forever.  The default value is 1 iteration.
	/// </param>
	/// <animation>Sequence</animation>
    $AA.SequenceAnimation.initializeBase(this, [target, duration, fps, animations]);

    // Handler used to determine when an animation has finished
    this._handler = null;
    
    // Flags to note whether we're playing, paused, or stopped
    this._paused = false;
    this._playing = false;
    
    // Index of the currently executing animation in the sequence
    this._index = 0;
    
    // Counter used when playing the animation to determine the remaining number of times to play the entire sequence
    this._remainingIterations = 0;
    
    // Number of iterations
    this._iterations = (iterations ? iterations : 1);
}
$AA.SequenceAnimation.prototype = {
    dispose : function() {
    	/// <summary>
        /// Dispose the animation
        /// </summary>
        /// <returns />

        this._handler = null;
        $AA.SequenceAnimation.callBaseMethod(this, 'dispose');
    },
    
    stop : function() {
        /// <summary>
        /// Stop playing the entire sequence of animations.
        /// </summary>
        /// <remarks>
        /// Stopping this animation will perform the last step of each child animation, thereby leaving their
        /// target elements in a state consistent with the animation playing completely. If this animation is
        /// the child of another, you must call <code>stop</code> on its parent instead.
        /// </remarks>
        /// <returns />

        if (this._playing) {
            var animations = this.get_animations();
            if (this._index < animations.length) {
                // Remove the handler from the currently running animation
                animations[this._index].remove_ended(this._handler);
                // Call stop on all remaining animations to ensure their
                // effects will be seen
                for (var i = this._index; i < animations.length; i++) {
                    animations[i].stop();
                }
            }
            this._playing = false;
            this._paused = false;
            this.raisePropertyChanged('isPlaying');
            this.onEnd();
        }
    },
    
    pause : function() {
        /// <summary>
        /// Pause the animation if it is playing.  Calling <code>play</code> will resume where
        /// the animation left off.
        /// </summary>
        /// <remarks>
        /// If this animation is the child of another, you must call <code>pause</code> on its parent instead.
        /// </remarks>
        /// <returns />

        if (this.get_isPlaying()) {
            var current = this.get_animations()[this._index];
            if (current != null) {
                current.pause();
            }
            this._paused = true;
            this.raisePropertyChanged('isPlaying');
        }
    },
    
    play : function() {
    	/// <summary>
        /// Play the sequence of animations from the beginning or where it was left off when paused.
        /// </summary>
        /// <remarks>
        /// If this animation is the child of another, you must call <code>play</code> on its parent instead.
        /// </remarks>
        /// <returns />

        var animations = this.get_animations();
        if (!this._playing) {
            this._playing = true;
            if (this._paused) {
                this._paused = false;
                var current = animations[this._index];
                if (current != null) {
                    current.play();
                    this.raisePropertyChanged('isPlaying');
                }
            } else {
                this.onStart();
                // Reset the index and attach the handler to the first
                this._index = 0;
                var first = animations[this._index];
                if (first) {
                    first.add_ended(this._handler);
                    first.play();
                    this.raisePropertyChanged('isPlaying');
                } else {
                    this.stop();
                }
            }
        }
    },
    
    onStart : function() {
    	/// <summary>
        /// The <code>onStart</code> method is called just before the animation is played each time.
        /// </summary>
        /// <returns />

        $AA.SequenceAnimation.callBaseMethod(this, 'onStart');
        this._remainingIterations = this._iterations - 1;
        
        // Create the handler we attach to each animation as it plays to determine when we've finished with it
        if (!this._handler) {
            this._handler = Function.createDelegate(this, this._onEndAnimation);
        }
    },
    
    _onEndAnimation : function() {
    	/// <summary>
        /// Wait for the end of each animation, and then continue by playing the other animations remaining
        /// in the sequence.  Stop when it reaches the last animation and there are no remaining iterations.
    	/// </summary>
    	/// <returns />

        // Remove the handler from the current animation
        var animations = this.get_animations();
        var current = animations[this._index++];
        if (current) {
            current.remove_ended(this._handler);
        }
        
        // Keep running animations and stop when we're out
        if (this._index < animations.length) {
            var next = animations[this._index];
            next.add_ended(this._handler);
            next.play();
        } else if (this._remainingIterations >= 1 || this._iterations <= 0) {
            this._remainingIterations--;
            this._index = 0;
            var first = animations[0];
            first.add_ended(this._handler);
            first.play();
        } else {
            this.stop();
        }
    },
    
    onStep : function(percentage) {
        /// <summary>
        /// Raises an invalid operation exception because this will only be called if a <code>SequenceAnimation</code>
        /// has been nested inside an <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation" /> (or a derived type).
        /// </summary>
        /// <param name="percentage" type="Number">Percentage of the animation already complete</param>
        /// <returns />
        throw Error.invalidOperation('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.SequenceAnimation cannot be nested inside Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ParallelAnimation');
    },
    
    onEnd : function() {
        /// <summary>
        /// The <code>onEnd</code> method is called just after the animation is played each time.
        /// </summary>
        /// <returns />

        this._remainingIterations = 0;
        $AA.SequenceAnimation.callBaseMethod(this, 'onEnd');
    },
    
    get_isActive : function() {
    	/// <value type="Boolean">
        /// <code>true</code> if animation is active, <code>false</code> if not.
        /// </value>
        return true;
    },
    
    get_isPlaying : function() {
    	/// <value type="Boolean">
        /// <code>true</code> if animation is playing, <code>false</code> if not.
        /// </value>
        return this._playing && !this._paused;
    },
    
    set_target : function(value) {
        /// <value type="Sys.UI.DomElement">
        /// Override the <code>target</code> property to pass it through to any
        /// child animations that have no target defined
        /// </value>
        /// <remarks>
        /// Do not set this property in a generic Xml animation description. It will be set automatically
        /// using either the extender's TargetControlID or the AnimationTarget property.
        /// <remarks>
        $AA.SequenceAnimation.callBaseMethod(this, 'set_target', [value]);
        this.setChildTargets(value);
    },
    
    get_iterations : function() {
    	/// <value type="Number">
        /// Number of times to repeatedly play the sequence.  If zero or less iterations are specified, the sequence
	    /// will repeat forever.  The default value is 1 iteration.
    	/// </value>
        return this._iterations;
    },
    set_iterations : function(value) {
        value = this._getInteger(value);
        if (this._iterations != value) {
            this._iterations = value;
            this.raisePropertyChanged('iterations');
        }
    },
    
    get_isInfinite : function() {
    	/// <value type="Boolean">
        /// <code>true</code> if this animation will repeat forever, <code>false</code> otherwise.
    	/// </value>
        return this._iterations <= 0;
    }
}
$AA.SequenceAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.SequenceAnimation', $AA.ParentAnimation);
$AA.registerAnimation('sequence', $AA.SequenceAnimation);


$AA.SelectionAnimation = function(target, duration, fps, animations) {
	/// <summary>
    /// The <code>SelectionAnimation</code> will run a single animation chosen from of its child animations. It is
    /// important to note that the <code>SelectionAnimation</code> ignores the <code>duration</code> and <code>fps</code>
    /// properties, and will let each of its child animations use any settings they please.  This is a base class with no
    /// functional implementation, so consider using <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ConditionAnimation" /> or
    /// <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.CaseAnimation" /> instead.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="animations" elementType="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" mayBeNull="true" optional="true">Array of child animations</param>
	/// <animation>Selection</animation>
    $AA.SelectionAnimation.initializeBase(this, [target, duration, fps, animations]);
    
    // Index of the animation selected to play
    this._selectedIndex = -1;
    
    // Reference to the animation selected to play
    this._selected = null;
}
$AA.SelectionAnimation.prototype = {    
    getSelectedIndex : function() {
    	/// <summary>
        /// Get the index of the animation that is selected to be played.  If this returns an index outside the bounds of
        /// the child animations array, then nothing is played.
    	/// </summary>
    	/// <returns type="Number">Index of the selected child animation to play</returns>
        throw Error.notImplemented();
    },
    
    onStart : function() {
    	/// <summary>
        /// The <code>onStart</code> method is called just before the animation is played each time.
        /// </summary>
        /// <returns />
	    $AA.SelectionAnimation.callBaseMethod(this, 'onStart');
	    
	    var animations = this.get_animations();
	    this._selectedIndex = this.getSelectedIndex();
	    if (this._selectedIndex >= 0 && this._selectedIndex < animations.length) {
	        this._selected = animations[this._selectedIndex];
	        if (this._selected) {
	            this._selected.setOwner(this);
	            this._selected.onStart();
	        }
	    }
    },
    
    onStep : function(percentage) {
    	/// <summary>
        /// The <code>onStep</code> method is called repeatedly to progress the animation through each frame
        /// </summary>
        /// <param name="percentage" type="Number">Percentage of the animation already complete</param>
        /// <returns />

        if (this._selected) {
    	    this._selected.onStep(percentage);
    	}
    },
    
    onEnd : function() {
    	/// <summary>
        /// The <code>onEnd</code> method is called just after the animation is played each time.
        /// </summary>
        /// <returns />

        if (this._selected) {
    	    this._selected.onEnd();
    	    this._selected.setOwner(null);
    	}
    	this._selected = null;
    	this._selectedIndex = null;
	    $AA.SelectionAnimation.callBaseMethod(this, 'onEnd');
    },
    
    set_target : function(value) {
        /// <value type="Sys.UI.DomElement">
        /// Override the <code>target</code> property to pass it through to any
        /// child animations that have no target defined
        /// </value>
        /// <remarks>
        /// Do not set this property in a generic Xml animation description. It will be set automatically
        /// using either the extender's TargetControlID or the AnimationTarget property.
        /// <remarks>
        $AA.SelectionAnimation.callBaseMethod(this, 'set_target', [value]);
        this.setChildTargets(value);
    }
}
$AA.SelectionAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.SelectionAnimation', $AA.ParentAnimation);
$AA.registerAnimation('selection', $AA.SelectionAnimation);


$AA.ConditionAnimation = function(target, duration, fps, animations, conditionScript) {
	/// <summary>
    /// The <code>ConditionAnimation</code> is used as a control structure to play a specific child animation
    /// depending on the result of executing the <code>conditionScript</code>.  If the <code>conditionScript</code>
    /// evaluates to <code>true</code>, the first child animation is played.  If it evaluates to <code>false</code>,
    /// the second child animation is played (although nothing is played if a second animation is not present).
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="animations" elementType="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" mayBeNull="true" optional="true">Array of child animations</param>
	/// <param name="conditionScript" type="String" mayBeNull="true" optional="true">
	/// JavaScript that should evaluate to <code>true</code> or <code>false</code> to determine which child
	/// animation to play.
	/// </param>
	/// <animation>Condition</animation>
    $AA.ConditionAnimation.initializeBase(this, [target, duration, fps, animations]);
    
    // Condition to determine which index we will play
    this._conditionScript = conditionScript;   
}
$AA.ConditionAnimation.prototype = {    
   getSelectedIndex : function() {
    	/// <summary>
        /// Get the index of the animation that is selected to be played.  If this returns an index outside the bounds of
        /// the child animations array, then nothing is played.
    	/// </summary>
    	/// <returns type="Number">Index of the selected child animation to play</returns>

        var selected = -1;
        if (this._conditionScript && this._conditionScript.length > 0) {
            try {
                selected = eval(this._conditionScript) ? 0 : 1;
            } catch(ex) {
            }
        }
        return selected;
    },
    
    get_conditionScript : function() {
    	/// <value type="String">
        /// JavaScript that should evaluate to <code>true</code> or <code>false</code> to determine which
        /// child animation to play.
    	/// </value>
        return this._conditionScript;
    },
    set_conditionScript : function(value) {
        if (this._conditionScript != value) {
            this._conditionScript = value;
            this.raisePropertyChanged('conditionScript');
        }
    }
}
$AA.ConditionAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ConditionAnimation', $AA.SelectionAnimation);
$AA.registerAnimation('condition', $AA.ConditionAnimation);


$AA.CaseAnimation = function(target, duration, fps, animations, selectScript) {
	/// <summary>
    /// The <code>CaseAnimation</code> is used as a control structure to play a specific child animation depending on
    /// the result of executing the <code>selectScript</code>, which should return the index of the child animation to
    /// play (this is similar to the <code>case</code> or <code>select</code> statements in C#/VB, etc.).  If the provided
    /// index is outside the bounds of the child animations array (or if nothing was returned) then we will not play anything.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="animations" elementType="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Animation" mayBeNull="true" optional="true">Array of child animations</param>
	/// <param name="" type="" mayBeNull="true" optional="true">
	/// JavaScript that should evaluate to the index of the appropriate child animation to play.
	/// </param>
	/// <animation>Case</animation>
    $AA.CaseAnimation.initializeBase(this, [target, duration, fps, animations]);

    // Condition to determine which index we will play
    this._selectScript = selectScript;
}
$AA.CaseAnimation.prototype = {
    getSelectedIndex : function() {
    	/// <summary>
        /// Get the index of the animation that is selected to be played.  If this returns an index outside the bounds of
        /// the child animations array, then nothing is played.
    	/// </summary>
    	/// <returns type="Number">Index of the selected child animation to play</returns>

        var selected = -1;
        if (this._selectScript && this._selectScript.length > 0) {
            try {
                var result = eval(this._selectScript)
                if (result !== undefined)
                    selected = result;
            } catch (ex) {
            }
        }
        return selected;
    },
    
    get_selectScript : function() {
    	/// <value type="String">
        /// JavaScript that should evaluate to the index of the appropriate child animation to play.
    	/// </value>
        return this._selectScript;
    },
    set_selectScript : function(value) {
        if (this._selectScript != value) {
            this._selectScript = value;
            this.raisePropertyChanged('selectScript');
        }
    }
}
$AA.CaseAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.CaseAnimation', $AA.SelectionAnimation);
$AA.registerAnimation('case', $AA.CaseAnimation);


$AA.FadeEffect = function() {
    /// <summary>
    /// The FadeEffect enumeration determines whether a fade animation is used to fade in or fade out.
    /// </summary>
    /// <field name="FadeIn" type="Number" integer="true" static="true" />
    /// <field name="FadeOut" type="Number" integer="true" static="true" />
    throw Error.invalidOperation();
}
$AA.FadeEffect.prototype = {
    FadeIn : 0,
    FadeOut : 1
}
$AA.FadeEffect.registerEnum("Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeEffect", false);


$AA.FadeAnimation = function(target, duration, fps, effect, minimumOpacity, maximumOpacity, forceLayoutInIE) {
	/// <summary>
    /// The <code>FadeAnimation</code> is used to fade an element in or out of view, depending on the
    /// provided <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeEffect" />, by settings its opacity.
    /// The minimum and maximum opacity values can be specified to precisely control the fade.
    /// You may also consider using <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeInAnimation" /> or
    /// <see cref="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeOutAnimation" /> if you know the only direction you
    /// are fading.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="effect" type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeEffect" mayBeNull="true" optional="true">
	/// Determine whether to fade the element in or fade the element out.  The possible values are <code>FadeIn</code>
	/// and <code>FadeOut</code>.  The default value is <code>FadeOut</code>.
	/// </param>
	/// <param name="minimumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Minimum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 0. 
	/// </param>
	/// <param name="maximumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Maximum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 1. 
	/// </param>
	/// <param name="forceLayoutInIE" type="Boolean" mayBeNull="true" optional="true">
	/// Whether or not we should force a layout to be created for Internet Explorer by giving it a width and setting its
	/// background color (the latter is required in case the user has ClearType enabled). The default value is <code>true</code>.
	/// This is obviously ignored when working in other browsers.
	/// </param>
	/// <animation>Fade</animation>
    $AA.FadeAnimation.initializeBase(this, [target, duration, fps]);

    // The effect determines whether or not we fade in or out
    this._effect = effect ? effect : $AA.FadeEffect.FadeIn;
    
    // Maximum and minimum opacities default to 100% and 0%
    this._max = maximumOpacity ? maximumOpacity : 1;
    this._min = minimumOpacity ? minimumOpacity : 0;
    
    // Starting and ending opacities
    this._start = this._min;
    this._end = this._max;
    
    // Whether the a layout has already been created (to work around IE problems)
    this._layoutCreated = false;

    // Whether or not we should force a layout to be created for IE by giving it a width
    // and setting its background color (the latter is required in case the user has ClearType enabled).
    // http://msdn.microsoft.com/library/default.asp?url=/workshop/author/filter/reference/filters/alpha.asp
    this._forceLayoutInIE = (forceLayoutInIE === undefined || forceLayoutInIE === null) ? true : forceLayoutInIE;
    
    // Properly set up the min/max values provided by the constructor
    this._resetOpacities();
}
$AA.FadeAnimation.prototype = {
    _resetOpacities : function() {
    	/// <summary>
        /// Set the starting and ending opacity values based on the effect (i.e. when we're fading
        /// in we go from <code>_min</code> to <code>_max</code>, but we go <code>_max</code> to
        /// <code>_min</code> when fading out)
    	/// </summary>
    	/// <returns />

        if (this._effect == $AA.FadeEffect.FadeIn) {
            this._start = this._min;
            this._end = this._max;
        } else {
            this._start = this._max;
            this._end = this._min;
        }
    },
    
    _createLayout : function() {
    	/// <summary>
        /// Create a layout when using Internet Explorer (which entails setting a width and also
        /// a background color if it currently has neither)
    	/// </summary>
    	/// <returns />

        var element = this.get_target();
        if (element) {
            // Get the original width/height/back color
            var originalWidth = CommonToolkitScripts.getCurrentStyle(element, 'width');
            var originalHeight = CommonToolkitScripts.getCurrentStyle(element, 'height');
            var originalBackColor = CommonToolkitScripts.getCurrentStyle(element, 'backgroundColor');

            // Set the width which will force the creation of a layout
            if ((!originalWidth || originalWidth == '' || originalWidth == 'auto') &&
                (!originalHeight || originalHeight == '' || originalHeight == 'auto')) {
                element.style.width = element.offsetWidth + 'px';
            }
            
            // Set the back color to avoid ClearType problems
            if (!originalBackColor || originalBackColor == '' || originalBackColor == 'transparent' || originalBackColor == 'rgba(0, 0, 0, 0)') {
                element.style.backgroundColor = CommonToolkitScripts.getInheritedBackgroundColor(element);
            }
            
            // Mark that we've created the layout so we only do it once
            this._layoutCreated = true;
        }
    },
    
    onStart : function() {
    	/// <summary>
        /// The <code>onStart</code> method is called just before the animation is played each time.
        /// </summary>
        /// <returns />       
        $AA.FadeAnimation.callBaseMethod(this, 'onStart');
        
        this.setValue(this._start);
        
        // Force the creation of a layout in IE if we're supposed to and the current browser is Internet Explorer
        if (this._forceLayoutInIE && !this._layoutCreated && Sys.Browser.agent == Sys.Browser.InternetExplorer) {
            this._createLayout();
        }
    },
    
    getAnimatedValue : function(percentage) {
    	/// <summary>
        /// Determine the current opacity after the given percentage of its duration has elapsed
        /// </summary>
        /// <param name="percentage" type="Number">Percentage of the animation already complete</param>
        /// <returns type="Number">
        /// Current opacity after the given percentage of its duration has elapsed that will
        /// be passed to <code>setValue</code>
        /// </returns>
        return this.interpolate(this._start, this._end, percentage);
    },
    
    setValue : function(value) {
    	/// <summary>
        /// Set the current opacity of the element.
        /// </summary>
        /// <remarks>
        /// This method will be replaced by a dynamically generated function that requires no logic
        /// to determine whether it should use filters or the style's opacity.
        /// </remarks>
        /// <param name="value" type="Number">Current opacity (as retreived from <code>getAnimatedValue</code>)</param>
        /// <returns />

        var element = this.get_target();
        if (element) {
            CommonToolkitScripts.setElementOpacity(element, value);
        }
    },
    
//    set_target : function(value) {
//        /// <value type="Sys.UI.DomElement">
//        /// Override the <code>target</code> property to dynamically create the setValue function.
//        /// </value>
//        /// <remarks>
//        /// Do not set this property in a generic Xml animation description. It will be set automatically
//        /// using either the extender's TargetControlID or the AnimationTarget property.
//        /// <remarks>
//        $AA.FadeAnimation.callBaseMethod(this, 'set_target', [value]);
//        
//        var element = value;
//        if (element) {
//            var filters = element.filters;
//            if (filters) {
//                var alphaFilter = null;
//                if (filters.length !== 0) {
//                    alphaFilter = filters['DXImageTransform.Microsoft.Alpha'];
//                }
//                if (!alphaFilter) {
//                    element.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(opacity=' + (this._start * 100) + ')';
//                    alphaFilter = filters['DXImageTransform.Microsoft.Alpha'];
//                }
//                if (alphaFilter) {
//                    this.setValue = function(val) { alphaFilter.opacity = val * 100; }
//                } else {
//                    this.setValue = function(val) {
//                        element.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(opacity=' + (val * 100) + ')';
//                    };
//                }
//            }
//            else {
//                this.setValue = function(val) { element.style.opacity = val; };
//            }
//        }
//    },
    
    get_effect : function() {
    	/// <value type="Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeEffect">
        /// Determine whether to fade the element in or fade the element out.  The possible values are
        /// <code>FadeIn</code> and <code>FadeOut</code>.  The default value is <code>FadeOut</code>.
    	/// </value>
        return this._effect;
    },
    set_effect : function(value) {
        value = this._getEnum(value, $AA.FadeEffect);
        if (this._effect != value) {
            this._effect = value;
            this._resetOpacities();
            this.raisePropertyChanged('effect');
        }
    },
    
    get_minimumOpacity : function() {
    	/// <value type="Number">
	    /// Minimum opacity to use when fading in or out. Its value can range from between 0 to 1.
	    /// The default value is 0. 
	    /// </value>
	    return this._min;
    },
    set_minimumOpacity : function(value) {
        value = this._getFloat(value);
        if (this._min != value) {
            this._min = value;
            this._resetOpacities();
            this.raisePropertyChanged('minimumOpacity');
        }
    },
    
    get_maximumOpacity : function() {
        /// <value type="Number">
	    /// Maximum opacity to use when fading in or out. Its value can range from between 0 to 1.
	    /// The default value is 1. 
	    /// </value>
        return this._max;
    },
    set_maximumOpacity : function(value) {
        value = this._getFloat(value);
        if (this._max != value) {
            this._max = value;
            this._resetOpacities();
            this.raisePropertyChanged('maximumOpacity');
        }
    },
    
    get_forceLayoutInIE : function() {
    	/// <value type="Boolean">
        /// Whether or not we should force a layout to be created for Internet Explorer by giving it a width and setting its
	    /// background color (the latter is required in case the user has ClearType enabled). The default value is <code>true</code>.
	    /// This is obviously ignored when working in other browsers.
    	/// </value>
        return this._forceLayoutInIE;
    },
    set_forceLayoutInIE : function(value) {
        value = this._getBoolean(value);
        if (this._forceLayoutInIE != value) {
            this._forceLayoutInIE = value;
            this.raisePropertyChanged('forceLayoutInIE');
        }
    },
    
    set_startValue : function(value) {
    	/// <value type="Number">
        /// Set the start value (so that child animations can set the current opacity as the start value when fading in or out)
    	/// </value>
        value = this._getFloat(value);
        this._start = value;
    }
}
$AA.FadeAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeAnimation', $AA.Animation);
$AA.registerAnimation('fade', $AA.FadeAnimation);


$AA.FadeInAnimation = function(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE) {
	/// <summary>
    /// The <code>FadeInAnimation</code> will fade the target in by moving from hidden to visible.
    /// It starts the animation the target's current opacity.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="minimumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Minimum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 0. 
	/// </param>
	/// <param name="maximumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Maximum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 1. 
	/// </param>
	/// <param name="forceLayoutInIE" type="Boolean" mayBeNull="true" optional="true">
	/// Whether or not we should force a layout to be created for Internet Explorer by giving it a width and setting its
	/// background color (the latter is required in case the user has ClearType enabled). The default value is <code>true</code>.
	/// This is obviously ignored when working in other browsers.
	/// </param>
	/// <animation>FadeIn</animation>
    $AA.FadeInAnimation.initializeBase(this, [target, duration, fps, $AA.FadeEffect.FadeIn, minimumOpacity, maximumOpacity, forceLayoutInIE]);
}
$AA.FadeInAnimation.prototype = {
    onStart : function() {
    	/// <summary>
        /// The <code>onStart</code> method is called just before the animation is played each time.
        /// </summary>
        /// <returns />
        $AA.FadeInAnimation.callBaseMethod(this, 'onStart');
        
        var element = this.get_target();
        if (element) {
            this.set_startValue(CommonToolkitScripts.getElementOpacity(element));
        }
    }
}
$AA.FadeInAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeInAnimation', $AA.FadeAnimation);
$AA.registerAnimation('fadeIn', $AA.FadeInAnimation);


$AA.FadeOutAnimation = function(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE) {
	/// <summary>
    /// The FadeInAnimation will fade the element out by moving from visible to hidden. It starts the animation
    /// at the element's current opacity.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="minimumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Minimum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 0. 
	/// </param>
	/// <param name="maximumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Maximum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 1. 
	/// </param>
	/// <param name="forceLayoutInIE" type="Boolean" mayBeNull="true" optional="true">
	/// Whether or not we should force a layout to be created for Internet Explorer by giving it a width and setting its
	/// background color (the latter is required in case the user has ClearType enabled). The default value is <code>true</code>.
	/// This is obviously ignored when working in other browsers.
	/// </param>
	/// <animation>FadeOut</animation>
    $AA.FadeOutAnimation.initializeBase(this, [target, duration, fps, $AA.FadeEffect.FadeOut, minimumOpacity, maximumOpacity, forceLayoutInIE]);
}
$AA.FadeOutAnimation.prototype = {
    onStart : function() {
    	/// <summary>
        /// The <code>onStart</code> method is called just before the animation is played each time.
        /// </summary>
        /// <returns />
        $AA.FadeOutAnimation.callBaseMethod(this, 'onStart');

        var element = this.get_target();
        if (element) {
            this.set_startValue(CommonToolkitScripts.getElementOpacity(element));
        }
    }
}
$AA.FadeOutAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.FadeOutAnimation', $AA.FadeAnimation);
$AA.registerAnimation('fadeOut', $AA.FadeOutAnimation);


$AA.PulseAnimation = function(target, duration, fps, iterations, minimumOpacity, maximumOpacity, forceLayoutInIE) {
	/// <summary>
    /// The PulseAnimation fades an element in and our repeatedly to create a pulsating
    /// effect.  The iterations determines how many pulses there will be (which defaults 
    /// to three, but it will repeat infinitely if given zero or less).  The duration
    /// property defines the duration of each fade in or fade out, not the duration of
    /// the animation as a whole.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="iterations" type="Number" mayBeNull="true" optional="true">
	/// Number of times to repeatedly play the sequence.  If zero or less iterations are specified, the sequence
	/// will repeat forever.  The default value is 1 iteration.
	/// </param>
	/// <param name="minimumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Minimum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 0. 
	/// </param>
	/// <param name="maximumOpacity" type="Number" mayBeNull="true" optional="true">
	/// Maximum opacity to use when fading in or out. Its value can range from between 0 to 1. The default value is 1. 
	/// </param>
	/// <param name="forceLayoutInIE" type="Boolean" mayBeNull="true" optional="true">
	/// Whether or not we should force a layout to be created for Internet Explorer by giving it a width and setting its
	/// background color (the latter is required in case the user has ClearType enabled). The default value is <code>true</code>.
	/// This is obviously ignored when working in other browsers.
	/// </param>
	
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Pulse</animation>
    $AA.PulseAnimation.initializeBase(this, [target, duration, fps, null, (iterations ? iterations : 3)]);

    // Create the FadeOutAnimation
    this._out = new $AA.FadeOutAnimation(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE);
    this.add(this._out);
    
    // Create the FadeInAnimation
    this._in = new $AA.FadeInAnimation(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE);
    this.add(this._in);
}
$AA.PulseAnimation.prototype = {
   
    get_minimumOpacity : function() {
    	/// <summary>
    // Minimum Opacity accessors
    	/// </summary>
    	/// <returns />

        return this._out.get_minimumOpacity();
    },
    set_minimumOpacity : function(value) {
        value = this._getFloat(value);
        this._out.set_minimumOpacity(value);
        this._in.set_minimumOpacity(value);
        this.raisePropertyChanged('minimumOpacity');
    },
    
    get_maximumOpacity : function() {
    	/// <summary>
    // Minimum Opacity accessors
    	/// </summary>
    	/// <returns />

        return this._out.get_maximumOpacity();
    },
    set_maximumOpacity : function(value) {
        value = this._getFloat(value);
        this._out.set_maximumOpacity(value);
        this._in.set_maximumOpacity(value);
        this.raisePropertyChanged('maximumOpacity');
    },
    
    get_forceLayoutInIE : function() {
    	/// <summary>
    // Force Layout in IE accessors
    	/// </summary>
    	/// <returns />

        return this._out.get_forceLayoutInIE();
    },
    set_forceLayoutInIE : function(value) {
        value = this._getBoolean(value);
        this._out.set_forceLayoutInIE(value);
        this._in.set_forceLayoutInIE(value);
        this.raisePropertyChanged('forceLayoutInIE');
    },
    
    set_duration : function(value) {
    	/// <summary>
    // Override the duration property
    	/// </summary>
    	/// <returns />

        value = this._getFloat(value);
        $AA.PulseAnimation.callBaseMethod(this, 'set_duration', [value]);
        this._in.set_duration(value);
        this._out.set_duration(value);
    },
    
    set_fps : function(value) {
    	/// <summary>
    // Override the fps property
    	/// </summary>
    	/// <returns />

        value = this._getInteger(value);
        $AA.PulseAnimation.callBaseMethod(this, 'set_fps', [value]);
        this._in.set_fps(value);
        this._out.set_fps(value);
    }
    
}
$AA.PulseAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.PulseAnimation', $AA.SequenceAnimation);
$AA.registerAnimation('pulse', $AA.PulseAnimation);










$AA.PropertyAnimation = function(target, duration, fps, property, propertyKey) {
	/// <summary>
// The PropertyAnimation is a useful base animation that will assign the value from
// getAnimatedValue to a specified property.  You can provide the name of a property
// alongside an optional propertyKey (which indicates the value
// property[propertyKey], like style['backgroundColor']).  Note that for the style
// property, the key must be in a JavaScript friendly format (i.e. backgroundColor
// instead of background-color).
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Property</animation>
    $AA.PropertyAnimation.initializeBase(this, [target, duration, fps]);

    // Name of the property to set
    this._property = property;
    
    // Optional Key of the property to set (i.e., if the property were "style" then
    // this might be "backgroundColor")
    this._propertyKey = propertyKey;
}
$AA.PropertyAnimation.prototype = {
    
    setValue : function(value) {
    	/// <summary>
    // Assign the value to the property
    	/// </summary>
    	/// <returns />

        var element = this.get_target();
        if (element && this._property && this._property.length > 0) { 
            if (this._propertyKey && this._propertyKey.length > 0 && element[this._property]) {
                element[this._property][this._propertyKey] = value;
            } else {
                element[this._property] = value;
            }
        }
        // Sys.TypeDescriptor.setProperty(this.get_target(), this._property, value, this._propertyKey);
    },
    
    getValue : function() {
    	/// <summary>
    // Get the current value from the property
    	/// </summary>
    	/// <returns />

        var element = this.get_target();
        if (element && this._property && this._property.length > 0) { 
            var property = element[this._property];
            if (property) {
                if (this._propertyKey && this._propertyKey.length > 0) {
                    return property[this._propertyKey];
                }
                return property;
            }
        }
        return null;
        // return Sys.TypeDescriptor.getProperty(this.get_target(), this._property, this._propertyKey);
    },
    
    get_property : function() {
    	/// <summary>
    // Property accessors
    	/// </summary>
    	/// <returns />

        return this._property;
    },
    set_property : function(value) {
        if (this._property != value) {
            this._property = value;
            this.raisePropertyChanged('property');
        }
    },
    
    get_propertyKey : function() {
    	/// <summary>
    // PropertyKey accessors
    	/// </summary>
    	/// <returns />

        return this._propertyKey;
    },
    set_propertyKey : function(value) {
        if (this._propertyKey != value) {
            this._propertyKey = value;
            this.raisePropertyChanged('propertyKey');
        }
    }
}
$AA.PropertyAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.PropertyAnimation', $AA.Animation);
$AA.registerAnimation('property', $AA.PropertyAnimation);










$AA.DiscreteAnimation = function(target, duration, fps, property, propertyKey, values) {
	/// <summary>
// The DiscreteAnimation inherits from PropertyAnimation and sets the value of the
// property to the elements in a provided array of values.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Discrete</animation>
    $AA.DiscreteAnimation.initializeBase(this, [target, duration, fps, property, propertyKey]);

    // Values to assign to the property
    this._values = (values && values.length) ? values : [];
}
$AA.DiscreteAnimation.prototype = {
    
    getAnimatedValue : function(percentage) {
    	/// <summary>
    // Assign the value whose index corresponds to the current percentage
    	/// </summary>
    	/// <returns />

        var index = Math.floor(this.interpolate(0, this._values.length - 1, percentage));
        return this._values[index];
    },
    
    get_values : function() {
    	/// <summary>
    // Values accessor
    	/// </summary>
    	/// <returns />

        return this._values;
    },
    set_values : function(value) {
        if (this._values != value) {
            this._values = value;
            this.raisePropertyChanged('values');
        }
    }
}
$AA.DiscreteAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.DiscreteAnimation', $AA.PropertyAnimation);
$AA.registerAnimation('discrete', $AA.DiscreteAnimation);










$AA.InterpolatedAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue) {
	/// <summary>
// The InterpolatedAnimation is used to assign a range of values between start and
// end values to a specific property.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Interpolated</animation>
    $AA.InterpolatedAnimation.initializeBase(this, [target, duration, fps, property, propertyKey]);

    // Start and end values
    this._startValue = startValue;
    this._endValue = endValue;
}
$AA.InterpolatedAnimation.prototype = {

    get_startValue : function() {
    	/// <summary>
    // StartValue accessor
    	/// </summary>
    	/// <returns />

        return this._startValue;
    },
    set_startValue : function(value) {
        value = this._getFloat(value);
        if (this._startValue != value) {
            this._startValue = value;
            this.raisePropertyChanged('startValue');
        }
    },
    
    get_endValue : function() {
    	/// <summary>
    // EndValue accessor    
    	/// </summary>
    	/// <returns />

        return this._endValue;
    },
    set_endValue : function(value) {
        value = this._getFloat(value);
        if (this._endValue != value) {
            this._endValue = value;
            this.raisePropertyChanged('endValue');
        }
    }   
}
$AA.InterpolatedAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.InterpolatedAnimation', $AA.PropertyAnimation);
$AA.registerAnimation('interpolated', $AA.InterpolatedAnimation);









$AA.ColorAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue) {
	/// <summary>
// The ColorAnimation transitions the value of a property between two colors
// (although it does ignore the alpha channel)
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Color</animation>
    $AA.ColorAnimation.initializeBase(this, [target, duration, fps, property, propertyKey, startValue, endValue]);
    
    // Cached start/end RBG triplets
    this._start = null;
    this._end = null;
    
    // Flags indicating whether each dimension of color will be interpolated
    this._interpolateRed = false;
    this._interpolateGreen = false;
    this._interpolateBlue = false;
}
$AA.ColorAnimation.prototype = {
    onStart : function() {
    	/// <summary>
    // Determine which dimensions of color we'll animate transitions for
    	/// </summary>
    	/// <returns />

        $AA.ColorAnimation.callBaseMethod(this, 'onStart');
       
        this._start = $AA.ColorAnimation.getRGB(this.get_startValue());
        this._end = $AA.ColorAnimation.getRGB(this.get_endValue());
        
        this._interpolateRed = (this._start.Red != this._end.Red);
        this._interpolateGreen = (this._start.Green != this._end.Green);
        this._interpolateBlue = (this._start.Blue != this._end.Blue);
    },
    
    getAnimatedValue : function(percentage) {
    	/// <summary>
    // Get the interpolated color values
    	/// </summary>
    	/// <returns />

        var r = this._start.Red;
        var g = this._start.Green;
        var b = this._start.Blue;
        
        if (this._interpolateRed)
            r = Math.round(this.interpolate(r, this._end.Red, percentage));
        
        if (this._interpolateGreen)
            g = Math.round(this.interpolate(g, this._end.Green, percentage));
        
        if (this._interpolateBlue)
            b = Math.round(this.interpolate(b, this._end.Blue, percentage));
        
        return $AA.ColorAnimation.toColor(r, g, b);
    },
    
    set_startValue : function(value) {
        if (this._startValue != value) {
            this._startValue = value;
            this.raisePropertyChanged('startValue');
        }
    },
    
    set_endValue : function(value) {
        if (this._endValue != value) {
            this._endValue = value;
            this.raisePropertyChanged('endValue');
        }
    }   
}
// Convert the color to an RGB triplet
$AA.ColorAnimation.getRGB = function(color) {
    if (!color || color.length != 7) {
        throw "Color must be a 7-character hex representation (e.g. #246ACF), not " + color;
    }
    return { 'Red': parseInt(color.substr(1,2), 16),
             'Green': parseInt(color.substr(3,2), 16),
             'Blue': parseInt(color.substr(5,2), 16) };
}
    
// Convert an RBG triplet into a color
$AA.ColorAnimation.toColor = function(red, green, blue) {
    var r = red.toString(16);
    var g = green.toString(16);
    var b = blue.toString(16);
    if (r.length == 1) r = '0' + r;
    if (g.length == 1) g = '0' + g;
    if (b.length == 1) b = '0' + b;
    return '#' + r + g + b;
}
$AA.ColorAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ColorAnimation', $AA.InterpolatedAnimation);
$AA.registerAnimation('color', $AA.ColorAnimation);









$AA.LengthAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue, unit) {
	/// <summary>
// The LengthAnimation is identical to InterpolatedAnimation except it adds a unit
// to the value (which defaults to 'px') for properties requiring it
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Length</animation>
    $AA.LengthAnimation.initializeBase(this, [target, duration, fps, property, propertyKey, startValue, endValue]);
    
    // Unit of length (which defaults to px)
    this._unit = (unit != null) ? unit : 'px';
}
$AA.LengthAnimation.prototype = {

    getAnimatedValue : function(percentage) {
    	/// <summary>
    // Get the value as a length
    	/// </summary>
    	/// <returns />

        var value = this.interpolate(this.get_startValue(), this.get_endValue(), percentage);
        return Math.round(value) + this._unit;
    },
    
    get_unit : function() {
    	/// <summary>
    // Unit accessors
    	/// </summary>
    	/// <returns />

        return this._unit;
    },
    set_unit : function(value) {
        if (this._unit != value) {
            this._unit = value;
            this.raisePropertyChanged('unit');
        }
    }
}
$AA.LengthAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.LengthAnimation', $AA.InterpolatedAnimation);
$AA.registerAnimation('length', $AA.LengthAnimation);










$AA.MoveAnimation = function(target, duration, fps, horizontal, vertical, relative, unit) {
	/// <summary>
// The MoveAnimation is used to move the target element.  If the relative flag is
// set to true, then it treats the horizontal and vertical properties as offsets to
// move the element.  If the relative flag is false, then it will treat the
// horizontal and vertical properties as coordinates on the page where the target
// element should be moved.  It is important to note that the element must be
// positioned (i.e. absolutely) so that settings its top/left properties will change
// its location.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Move</animation>
    $AA.MoveAnimation.initializeBase(this, [target, duration, fps, null]);

    // Distance to move horizontally and vertically
    this._horizontal = horizontal ? horizontal : 0;
    this._vertical = vertical ? vertical : 0;
    this._relative = (relative === undefined) ? true : relative;
    
    // Length animations representing the movememnts
    this._horizontalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'left', null, null, unit);
    this._verticalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'top', null, null, unit);
    this.add(this._verticalAnimation);
    this.add(this._horizontalAnimation);
}
$AA.MoveAnimation.prototype = {
    
    onStart : function() {
    	/// <summary>
    // Override onStart to inform the animation 
    	/// </summary>
    	/// <returns />

        $AA.MoveAnimation.callBaseMethod(this, 'onStart');
        
        // Set the start and end values of the animations by getting
        // the element's current position and applying the offsets
        var element = this.get_target();
        this._horizontalAnimation.set_startValue(element.offsetLeft);
        this._horizontalAnimation.set_endValue(this._relative ? element.offsetLeft + this._horizontal : this._horizontal);
        this._verticalAnimation.set_startValue(element.offsetTop); 
        this._verticalAnimation.set_endValue(this._relative ? element.offsetTop + this._vertical : this._vertical);
    },
    
    get_horizontal : function() {
    	/// <summary>
    // Horizontal accessors
    	/// </summary>
    	/// <returns />

        return this._horizontal;
    },
    set_horizontal : function(value) {
        value = this._getFloat(value);
        if (this._horizontal != value) {
            this._horizontal = value;
            this.raisePropertyChanged('horizontal');
        }
    },
    
    get_vertical : function() {
    	/// <summary>
    // Vertical accessors    
    	/// </summary>
    	/// <returns />

        return this._vertical;
    },
    set_vertical : function(value) {
        value = this._getFloat(value);
        if (this._vertical != value) {
            this._vertical = value;
            this.raisePropertyChanged('vertical');
        }
    },
    
    get_relative : function() {
    	/// <summary>
    // Relative accessors
    	/// </summary>
    	/// <returns />

        return this._relative;
    },
    set_relative : function(value) {
        value = this._getBoolean(value);
        if (this._relative != value) {
            this._relative = value;
            this.raisePropertyChanged('relative');
        }
    },
    
    get_unit : function() {
    	/// <summary>
    // Unit accessors
    	/// </summary>
    	/// <returns />

        this._horizontalAnimation.get_unit();
    },
    set_unit : function(value) {
        var unit = this._horizontalAnimation.get_unit();
        if (unit != value) {
            this._horizontalAnimation.set_unit(value);
            this._verticalAnimation.set_unit(value);
            this.raisePropertyChanged('unit');
        }
    }
}
$AA.MoveAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.MoveAnimation', $AA.ParallelAnimation);
$AA.registerAnimation('move', $AA.MoveAnimation);










$AA.ResizeAnimation = function(target, duration, fps, width, height, unit) {
	/// <summary>
// Change the size of the element from its current values to the specified width and
// height (using the given unit, which defaults to 'px')
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Resize</animation>
    $AA.ResizeAnimation.initializeBase(this, [target, duration, fps, null]);

    // New size of the element
    this._width = width;
    this._height = height;
    
    // Animations to set the size across both dimensions
    this._horizontalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'width', null, null, unit);
    this._verticalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'height', null, null, unit);
    this.add(this._horizontalAnimation);
    this.add(this._verticalAnimation);
}
$AA.ResizeAnimation.prototype = {
    
    onStart : function() {
    	/// <summary>
    // Animate from the current size to the desired size
    	/// </summary>
    	/// <returns />

        $AA.ResizeAnimation.callBaseMethod(this, 'onStart');
        
        // Set the start and end values of the animations by getting
        // the element's current width and height
        var element = this.get_target();
        this._horizontalAnimation.set_startValue(element.offsetWidth);
        this._verticalAnimation.set_startValue(element.offsetHeight);
        this._horizontalAnimation.set_endValue((this._width !== null && this._width !== undefined) ?
            this._width : element.offsetWidth);
        this._verticalAnimation.set_endValue((this._height !== null && this._height !== undefined) ?
            this._height : element.offsetHeight);
    },
    
    get_width : function() {
    	/// <summary>
    // Width accessors     
    	/// </summary>
    	/// <returns />

        return this._width;
    },
    set_width : function(value) {
        value = this._getFloat(value);
        if (this._width != value) {
            this._width = value;
            this.raisePropertyChanged('width');
        }
    },
    
    get_height : function() {
    	/// <summary>
    // Height accessors
    	/// </summary>
    	/// <returns />

        return this._height;
    },
    set_height : function(value) {
        value = this._getFloat(value);
        if (this._height != value) {
            this._height = value;   
            this.raisePropertyChanged('height');
        }
    },
    
    get_unit : function() {
    	/// <summary>
    // Unit accessors
    	/// </summary>
    	/// <returns />

        this._horizontalAnimation.get_unit();
    },
    set_unit : function(value) {
        var unit = this._horizontalAnimation.get_unit();
        if (unit != value) {
            this._horizontalAnimation.set_unit(value);
            this._verticalAnimation.set_unit(value);
            this.raisePropertyChanged('unit');
        }
    }
}
$AA.ResizeAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ResizeAnimation', $AA.ParallelAnimation);
$AA.registerAnimation('resize', $AA.ResizeAnimation);









$AA.ScaleAnimation = function(target, duration, fps, scaleFactor, unit, center, scaleFont, fontUnit) {
	/// <summary>
// Scale the size of an element by the given scale factor (i.e. a scaleFactor of .5
// will shrink it in half and a scaleFactor of 2.0 will double it).  If scaleFont is
// set to true, the size of the font will also scale with the element (using the
// fontUnit as its unit which defaults to 'pt').  Finally, if center is true, then
// the element's center will not move as it is scaled.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Scale</animation>
    $AA.ScaleAnimation.initializeBase(this, [target, duration, fps]);

    // Percentage to scale
    this._scaleFactor = (scaleFactor === undefined) ? 1 : scaleFactor;
    this._unit = (unit != null) ? unit : 'px';
    
    // Center the content while scaling
    this._center = center;
    
    // Scale the font size as well
    this._scaleFont = scaleFont;
    this._fontUnit = (fontUnit != null) ? fontUnit : 'pt';
    
    // Initial values
    this._element = null;
    this._initialHeight = null;
    this._initialWidth = null;
    this._initialTop = null;
    this._initialLeft = null;
    this._initialFontSize = null;
}
$AA.ScaleAnimation.prototype = {    
    getAnimatedValue : function(percentage) {
    	/// <summary>
    // getAnimatedValue returns the amount to scale by given the current percentage
    	/// </summary>
    	/// <returns />

        return this.interpolate(1.0, this._scaleFactor, percentage);
    },
    
    onStart : function() {
    	/// <summary>
    // Cache the initial values because they will be used to determine how much to
    // scale the element at each step in the animation
    	/// </summary>
    	/// <returns />

        $AA.ScaleAnimation.callBaseMethod(this, 'onStart');
        
        this._element = this.get_target();
        if (this._element) {
            this._initialHeight = this._element.offsetHeight;
            this._initialWidth = this._element.offsetWidth;
            if (this._center) {
                this._initialTop = this._element.offsetTop;
                this._initialLeft = this._element.offsetLeft;
            }
            if (this._scaleFont) {
                // Note: we're assuming this is in the same units as fontUnit
                this._initialFontSize = parseFloat(
                    CommonToolkitScripts.getCurrentStyle(this._element, 'fontSize'));
            }
        }
    },
    
    setValue : function(scale) {
    	/// <summary>
    // Set the values of the element given its current scaleFactor 
    	/// </summary>
    	/// <returns />

        if (this._element) {
            var width = Math.round(this._initialWidth * scale);
            var height = Math.round(this._initialHeight * scale);
            this._element.style.width = width + this._unit; 
            this._element.style.height = height + this._unit;
            
            if (this._center) {
                this._element.style.top = (this._initialTop +
                    Math.round((this._initialHeight - height) / 2)) + this._unit;
                this._element.style.left = (this._initialLeft +
                    Math.round((this._initialWidth - width) / 2)) + this._unit;
            }
            
            if (this._scaleFont) {
                var size = this._initialFontSize * scale;
                if (this._fontUnit == 'px' || this._fontUnit == 'pt') {
                    size = Math.round(size);
                }
                this._element.style.fontSize = size + this._fontUnit;
            }
        }
    },
    
    onEnd : function() {
    	/// <summary>
    // Wipe the cached values
    	/// </summary>
    	/// <returns />

        this._element = null;
        this._initialHeight = null;
        this._initialWidth = null;
        this._initialTop = null;
        this._initialLeft = null;
        this._initialFontSize = null;
        $AA.ScaleAnimation.callBaseMethod(this, 'onEnd');
    },
    
    get_scaleFactor : function() {
    	/// <summary>
    // ScaleFactor accessors
    	/// </summary>
    	/// <returns />

        return this._scaleFactor;
    },
    set_scaleFactor : function(value) {
        value = this._getFloat(value);
        if (this._scaleFactor != value) {
            this._scaleFactor = value;
            this.raisePropertyChanged('scaleFactor');
        }
    },
    
    get_unit : function() {
    	/// <summary>
    // Unit accessors
    	/// </summary>
    	/// <returns />

        return this._unit;
    },
    set_unit : function(value) {
        if (this._unit != value) {
            this._unit = value;
            this.raisePropertyChanged('unit');
        }
    },
    
    get_center : function() {
    	/// <summary>
    // Center accessors
    	/// </summary>
    	/// <returns />

        return this._center;
    },
    set_center : function(value) {
        value = this._getBoolean(value);
        if (this._center != value) {
            this._center = value;
            this.raisePropertyChanged('center');
        }
    },
    
    get_scaleFont : function() {
    	/// <summary>
    // ScaleFont accessors
    	/// </summary>
    	/// <returns />

        return this._scaleFont;
    },
    set_scaleFont : function(value) {
        value = this._getBoolean(value);
        if (this._scaleFont != value) {
            this._scaleFont = value;
            this.raisePropertyChanged('scaleFont');
        }
    },
    
    get_fontUnit : function() {
    	/// <summary>
    // FontUnit accessors
    	/// </summary>
    	/// <returns />

        return this._fontUnit;
    },
    set_fontUnit : function(value) {
        if (this._fontUnit != value) { 
            this._fontUnit = value; 
            this.raisePropertyChanged('fontUnit');
        }
    }
}
$AA.ScaleAnimation.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ScaleAnimation', $AA.Animation);
$AA.registerAnimation('scale', $AA.ScaleAnimation);










$AA.Action = function(target, duration, fps) {
	/// <summary>
// Action is a base class for all "non-animating" animations.  Where animations
// perform an operation in a sequence of small steps spread over an interval, the
// actions perform a single operation instantaneously.  The base class provides
// empty implementations for all necessary functions and adds a doAction function
// that will be called to perform the operation.  By default, all actions have a
// duration of zero.  The actions are very useful for defining complex animations.
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>Action</animation>
    $AA.Action.initializeBase(this, [target, duration, fps]);

    // Set the duration to 0 if it wasn't specified
    if (duration === undefined) {
        this.set_duration(0);
    }
}
$AA.Action.prototype = {
    
    onEnd : function() {
    	/// <summary>
    // Call the doAction method when the animation completes
    	/// </summary>
    	/// <returns />

        this.doAction();
        $AA.Action.callBaseMethod(this, 'onEnd');
    },
    
    doAction : function() {
    	/// <summary>
    // The doAction method must be implemented by derived classes
    	/// </summary>
    	/// <returns />

        throw Error.notImplemented();
    },
    
    getAnimatedValue : function() {
    	/// <summary>
    // Empty implementations for required functions
    	/// </summary>
    	/// <returns />

    },
    setValue : function() {
    }
}
$AA.Action.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.Action', $AA.Animation);
$AA.registerAnimation('action', $AA.Action);










$AA.EnableAction = function(target, duration, fps, enabled) {
	/// <summary>
// The EnableAction changes whether or not an element is disabled (it will enable
// the element by default)
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>EnableAction</animation>
    $AA.EnableAction.initializeBase(this, [target, duration, fps]);

    // Whether to enable or disable
    this._enabled = (enabled !== undefined) ? enabled : true;
}
$AA.EnableAction.prototype = {
    doAction : function() {
    	/// <summary>
    // Set the enabled property of the control
    	/// </summary>
    	/// <returns />

        var element = this.get_target();
        if (element) {
            element.disabled = !this._enabled;
        }
    },
    
    get_enabled : function() {
    	/// <summary>
    // Enabled accessors
    	/// </summary>
    	/// <returns />
 
        return this._enabled;
    },
    set_enabled : function(value) {
        value = this._getBoolean(value);
        if (this._enabled != value) {
            this._enabled = value;
            this.raisePropertyChanged('enabled');
        }
    }
}
$AA.EnableAction.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.EnableAction', $AA.Action);
$AA.registerAnimation('enableAction', $AA.EnableAction);










$AA.HideAction = function(target, duration, fps) {
	/// <summary>
// The HideAction simply hides an element from view (by setting its style's display
// attribute to 'none')
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>HideAction</animation>
    $AA.HideAction.initializeBase(this, [target, duration, fps]);

}
$AA.HideAction.prototype = {
    doAction : function() {
    	/// <summary>
    // Hide an element
    	/// </summary>
    	/// <returns />

        var element = this.get_target();
        if (element) {
            element.style.display = 'none';
        }
    }
}
$AA.HideAction.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.HideAction', $AA.Action);
$AA.registerAnimation('hideAction', $AA.HideAction);










$AA.StyleAction = function(target, duration, fps, attribute, value) {
	/// <summary>
// The StyleAction is used to set a particular attribute of the element's style
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>StyleAction</animation>
    $AA.StyleAction.initializeBase(this, [target, duration, fps]);

    // Style attribute (like "backgroundColor" or "borderWidth"
    this._attribute = attribute;
    
    // Value to assign to the style attribute
    this._value = value;
    
}
$AA.StyleAction.prototype = {
    doAction : function() {
    	/// <summary>
    // Assign the value to the style
    	/// </summary>
    	/// <returns />

        var element = this.get_target();
        if (element) {
            element.style[this._attribute] = this._value;
        }
    },
    
    get_attribute : function() {
    	/// <summary>
    // Attribute accessors
    	/// </summary>
    	/// <returns />
 
        return this._attribute;
    },
    set_attribute : function(value) {
        if (this._attribute != value) {
            this._attribute = value;
            this.raisePropertyChanged('attribute');
        }
    },
    
    get_value : function() {
    	/// <summary>
    // Value accessors
    	/// </summary>
    	/// <returns />

        return this._value;
    },
    set_value : function(value) {
        if (this._value != value) {
            this._value = value;
            this.raisePropertyChanged('value');
        }
    }
}
$AA.StyleAction.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.StyleAction', $AA.Action);
$AA.registerAnimation('styleAction', $AA.StyleAction);










$AA.OpacityAction = function(target, duration, fps, opacity) {
	/// <summary>
// OpacityAction allows you to set the opacity of an element
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>OpacityAction</animation>
    $AA.OpacityAction.initializeBase(this, [target, duration, fps]);
    
    // Opacity
    this._opacity = opacity;
}
$AA.OpacityAction.prototype = {
    doAction : function() {
    	/// <summary>
    // Set the opacity
    	/// </summary>
    	/// <returns />

        var element = this.get_target();
        if (element) {
            CommonToolkitScripts.setElementOpacity(element, this._opacity);
        }
    },
    
    get_opacity : function() {
    	/// <summary>
    // Opacity accessor
    	/// </summary>
    	/// <returns />
 
        return this._opacity;
    },
    set_opacity : function(value) {
        value = this._getFloat(value);
        if (this._opacity != value) {
            this._opacity = value;
            this.raisePropertyChanged('opacity');
        }
    }
}
$AA.OpacityAction.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.OpacityAction', $AA.Action);
$AA.registerAnimation('opacityAction', $AA.OpacityAction);










$AA.ScriptAction = function(target, duration, fps, script) {
	/// <summary>
    /// ScriptAction allows you to execute JavaScript by 'eval'ing it
	/// </summary>
	/// <param name="target" type="Sys.UI.DomElement" mayBeNull="true" optional="true">Target of the animation</param>
	/// <param name="duration" type="Number" mayBeNull="true" optional="true">Length of the animation in seconds.  The default is 1.</param>
	/// <param name="fps" type="Number" mayBeNull="true" optional="true">Number of steps per second.  The default is 25.</param>
	/// <param name="" type="" mayBeNull="true" optional="true"></param>
	/// <animation>ScriptAction</animation>
    $AA.ScriptAction.initializeBase(this, [target, duration, fps]);

    // Script to execute
    this._script = script;
}
$AA.ScriptAction.prototype = {
    doAction : function() {
    	/// <summary>
    // Execute the script
    	/// </summary>
    	/// <returns />

        try {
            eval(this._script);
        } catch (ex) {
        }
    },
    
    get_script : function() {
    	/// <summary>
    // Script accessors
    	/// </summary>
    	/// <returns />
 
        return this._script;
    },
    set_script : function(value) {
        if (this._script != value) {
            this._script = value;
            this.raisePropertyChanged('script');
        }
    }
}
$AA.ScriptAction.registerClass('Microsoft.Mtps.Rendering.Behaviors.Ajax.Animation.ScriptAction', $AA.Action);
$AA.registerAnimation('scriptAction', $AA.ScriptAction);

if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();