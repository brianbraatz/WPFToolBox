/* <gslb_version version="V1.2.1" /> */
/* Copyright © GalaSoft Laurent Bugnion 2007 */

//*****************************************************************************
//* gslb.positionednode.js
//*****************************************************************************
//* Control name            : PositionedNode
//* Target                  : Netscape 6+, Internet explorer 5+
//* Language/Compiler       : ECMAScript V3
//* Author                  : Laurent Bugnion (LBu)
//* Created                 : 05.01.2007
//*****************************************************************************
//* Description: see constructor
//* Last Base Level: BL0006
//*****************************************************************************

//*****************************************************************************
//* Imports *******************************************************************
//*****************************************************************************

// using gslb.Object
// using gslb.Cookie (optional) (from gslb.PositionedBase)
// using gslb.PositionedBase
// using gslb.CssLength

// Create namespace
if ( !window.gslb )
{
  window.gslb = {};
}

if ( !gslb.Object )
{
  throw "Error in PositionedNode: gslb.Object not found";
}
if ( !gslb.PositionedBase )
{
  throw "gslb.PositionedBase not found";
}

//*****************************************************************************
//* Constructor ***************************************************************
//*****************************************************************************

// ----------------------------------------------------------------------------
/// <summary>
/// Allows moving a DOM Node from the current position to another one at variable
/// speed. Once a node is positioned, if the user scrolls the window,
/// the Node will reposition itself to the same distance from the window's border.
/// </summary>
/// <param name="nNode" type="Node">The DOM node which will be moved.</param>
/// <param name="oOptions" type="Object">An Options object, constructed using literal notation.
/// All values are optional.
/// <br />bRepositionX: If true, the node will be repositioned along the X axis. Default: true.
/// <br />bRepositionY: If true, the node will be repositioned along the Y axis. Default: true.
/// <br />iTimerMillisecondsFast: The duration of one iteration (in milliseconds)
/// while the node is being moved. Default: 1 milliseconds.
/// <br />iTimerMillisecondsSlow: The duration of one iteration (in milliseconds)
/// while the node is standing still. Default: 100 milliseconds.
/// <br />fnRepositionX: The function to use to calculate the Node's speed when it is
/// moved along the X axis. See the class' documentation. Default: null.
/// <br />fnRepositionY: The function to use to calculate the Node's speed when it is
/// moved along the Y axis. See the class' documentation. Default: null.
/// <br />fnOnMoveStartX: A function which will be called when the Node starts moving
/// along the X axis. Default: null.
/// <br />fnOnMoveStartY: A function which will be called when the Node starts moving
/// along the Y axis. Default: null.
/// <br />fnOnMoveStart: A function which will be called when the Node starts moving
/// along any axis. Default: null.
/// <br />fnOnMoveEndX: A function which will be called when the Node stops moving
/// along the X axis. Default: null.
/// <br />fnOnMoveEndY: A function which will be called when the Node stops moving
/// along the Y axis. Default: null.
/// <br />fnOnMoveEnd: A function which will be called when the Node stops moving
/// along both axis. Default: null.
/// <br />bSaveRelocationInCookie: If true, the new position after relocate was called will be
/// saved in a cookie. Note: Setting this value to false doesn't delete a cookie if it
/// had been set previously. To delete the cookie, you must explicitly call the method
/// deleteCookie. Default: false.
/// <br />iCookieExpirationHours: If bSaveRelocationInCookie is true, this specifies the number
/// of hours during which the Node's new location will be saved
/// in a cookie. If the value is set to 0, this is a session cookie (non-persistent). Default: 0.</param>
gslb.PositionedNode = function( nNode, oOptions )
{
  //***************************************************************************
  //* Inheritance *************************************************************
  //***************************************************************************

  // The options in the base class will be set later when setOptions is called
  // in this constructor.
  gslb.PositionedNode.Base.fnConstructor.call( this, nNode, null );

  //***************************************************************************
  //* Attributes **************************************************************
  //***************************************************************************

  // Options

  /// <summary type="bool">
  /// If true, the node will be repositioned along the X axis. Default: true.
  /// </summary>
  this.m_bRepositionX = true;
  /// <summary type="bool">
  /// If true, the node will be repositioned along the Y axis. Default: true.
  /// </summary>
  this.m_bRepositionY = true;
  /// <summary type="int">
  /// The duration of one iteration (in milliseconds)
  /// while the node is being moved. Default: 1.
  /// </summary>
  this.m_iTimerMillisecondsFast = 1;
  /// <summary type="int">
  /// The duration of one iteration (in milliseconds)
  /// while the node is standing still. Default: 100.
  /// </summary>
  this.m_iTimerMillisecondsSlow = 100;
  /// <summary type="Function">
  /// The function to use to calculate the Node's speed when it is
  /// moved along the X axis. See the class' documentation. Default: null.
  /// </summary>
  this.m_fnRepositionX = gslb.PositionedNode.getDefaultDeltaX;
  /// <summary type="Function">
  /// The function to use to calculate the Node's speed when it is
  /// moved along the Y axis. See the class' documentation. Default: null.
  /// </summary>
  this.m_fnRepositionY = gslb.PositionedNode.getDefaultDeltaY;
  /// <summary type="Function">
  /// A function which will be called when the Node starts moving
  /// along the X axis. Default: null.
  /// </summary>
  this.m_fnOnMoveStartX = null;
  /// <summary type="Function">
  /// A function which will be called when the Node starts moving
  /// along the Y axis. Default: null.
  /// </summary>
  this.m_fnOnMoveStartY = null;
  /// <summary type="Function">
  /// A function which will be called when the Node starts moving
  /// along any axis. Default: null.
  /// </summary>
  this.m_fnOnMoveStart = null;
  /// <summary type="Function">
  /// A function which will be called when the Node stops moving
  /// along the X axis. Default: null.
  /// </summary>
  this.m_fnOnMoveEndX = null;
  /// <summary type="Function">
  /// A function which will be called when the Node stops moving
  /// along the Y axis. Default: null.
  /// </summary>
  this.m_fnOnMoveEndY = null;
  /// <summary type="Function">
  /// A function which will be called when the Node stops moving
  /// along both axis. Default: null.
  /// </summary>
  this.m_fnOnMoveEnd = null;

  // Set options --------------------------------------------------------------

  // Assign options
  this.setOptions( oOptions );
  // Set position according to cookie first, so that attributes are set correctly
  this.restorePosition();

  // Other attributes ---------------------------------------------------------

  /// <summary type="gslb.CssLength">
  /// An instance of CssLength used to convert positions along the X axis.
  /// For optimization, CssLength is used only if needed (non-IE platforms)
  /// For IE platforms, use pixelLeft and pixelTop instead (no conversion needed).
  /// </summary>
  this.m_oPositionX = null;
  if ( this.m_nNode.style.pixelLeft == null )
  {
    if ( !gslb.CssLength )
    {
      throw "gslb.CssLength not found";
    }
    var strStyleLeft = nNode.style.left;
    if ( strStyleLeft.length == 0 )
    {
      strStyleLeft = nNode.offsetLeft + "px";
    }    
    this.m_oPositionX = new gslb.CssLength( strStyleLeft, nNode );
  }
  /// <summary type="gslb.CssLength">
  /// An instance of CssLength used to convert positions along the Y axis.
  /// For optimization, CssLength is used only if needed (non-IE platforms)
  /// For IE platforms, use pixelLeft and pixelTop instead (no conversion needed).
  /// </summary>
  this.m_oPositionY = null;
  if ( this.m_nNode.style.pixelTop == null )
  {
    if ( !gslb.CssLength )
    {
      throw "gslb.CssLength not found";
    }
    var strStyleTop = nNode.style.top;
    if ( strStyleTop.length == 0 )
    {
      strStyleTop = nNode.offsetTop + "px";
    }    
    this.m_oPositionY = new gslb.CssLength( strStyleTop, nNode );
  }

  /// <summary type="int">
  /// The Node's position along the X axis (including scrolling) (in pixels).
  /// Note: When the Node is moving, this attribute doesn't contain the
  /// actual position, but rather the target position.
  /// </summary>
  this.m_iTargetX = ( this.m_oPositionX ? this.m_oPositionX.getPixelValue() : this.m_nNode.offsetLeft );
  /// <summary type="int">
  /// The Node's position along the Y axis (including scrolling) (in pixels).
  /// Note: When the Node is moving, this attribute doesn't contain the
  /// actual position, but rather the target position.
  /// </summary>
  this.m_iTargetY = ( this.m_oPositionY ? this.m_oPositionY.getPixelValue() : this.m_nNode.offsetTop );
  /// <summary type="int">
  /// The Node's current position along the X axis (including scrolling) (in pixels).
  /// </summary>
  this.m_iPositionX = this.m_iTargetX;
  /// <summary type="int">
  /// The Node's current position along the Y axis (including scrolling) (in pixels).
  /// </summary>
  this.m_iPositionY = this.m_iTargetY;
  /// <summary type="int">
  /// The distance between the left of the window
  /// and the left of the control (not uncluding scrolling) (in pixels).
  /// </summary>
  this.m_iRelativeTargetX = this.m_iTargetX;
  /// <summary type="int">
  /// The distance between the top of the window
  /// and the top of the control (not uncluding scrolling) (in pixels).
  /// </summary>
  this.m_iRelativeTargetY = this.m_iTargetY;

  /// <summary type="int">
  /// The distance by which the page is scrolled on the X axis (in pixels).
  /// </summary>
  this.m_iScrollX = gslb.PositionedNode.getScroll( "Left" );
  /// <summary type="int">
  /// The distance by which the page is scrolled on the Y axis (in pixels).
  /// </summary>
  this.m_iScrollY = gslb.PositionedNode.getScroll( "Top" );

  /// <summary type="bool">
  /// If true, the Node is pinned on the X axis.
  /// </summary>
  this.m_bPinX = false;
  /// <summary type="bool">
  /// If true, the Node is pinned on the Y axis.
  /// </summary>
  this.m_bPinY = false;

  /// <summary type="bool">
  /// If true, the Node must be relocated on the page along the X axis.
  /// If false, the Node will not move along this axis.
  /// </summary>
  this.m_bRelocateX = false;
  /// <summary type="bool">
  /// If true, the Node must be relocated on the page along the Y axis.
  /// If false, the Node will not move along this axis.
  /// </summary>
  this.m_bRelocateY = false;
  /// <summary type="bool">
  /// If true, the Node is currently moving along the X axis.
  /// </summary>
  this.m_bIsMovingX = false;
  /// <summary type="bool">
  /// If true, the Node is currently moving along the Y axis.
  /// </summary>
  this.m_bIsMovingY = false;
  /// <summary type="bool">
  /// If true, the Node has been disposed, and will not move anymore.
  /// </summary>
  this.m_bIsDisposed = false;
  /// <summary type="int">
  /// The ID of the timer used to reposition the Node or to check its position.
  /// </summary>
  this.m_iTimerId = null;
  /// <summary type="bool">
  /// If true, the "OnStart" function has been executed already. This is to avoid
  /// calling this function twice (X and Y axis).
  /// </summary>
  this.m_bOnMoveStartExecuted = false;

  // Post construction operations ---------------------------------------------

  // Start checks
  this.checkPosition();
}

// Inheritance
gslb.Object.extend( gslb.PositionedNode, gslb.PositionedBase );

//*****************************************************************************
//* Enums *********************************************************************
//*****************************************************************************

//*****************************************************************************
//* Constants *****************************************************************
//*****************************************************************************

/// <summary type="string">
/// This class's name.
/// </summary>
gslb.PositionedNode._NAME = "gslb.PositionedNode";
/// <summary type="string">
/// This class's version.
/// </summary>
gslb.PositionedNode._VERSION = "V1.2.1";
/// <summary type="string">
/// This class's date.
/// </summary>
gslb.PositionedNode._DATE = "19.02.2007";

//*****************************************************************************
//* Static attributes *********************************************************
//*****************************************************************************

//*****************************************************************************
//* Static methods ************************************************************
//*****************************************************************************

// ----------------------------------------------------------------------------
/// <summary>
/// Internal method only.
/// </summary>
gslb.PositionedNode.getDefaultDeltaX = function( iDistanceToTarget )
{
  return gslb.PositionedNode.getDefaultDelta( iDistanceToTarget, 50, 0.5, 1 );
}

// ----------------------------------------------------------------------------
/// <summary>
/// Internal method only.
/// </summary>
gslb.PositionedNode.getDefaultDeltaY = function( iDistanceToTarget )
{
  return gslb.PositionedNode.getDefaultDelta( iDistanceToTarget, 50, 0.5, 1 );
}

// ----------------------------------------------------------------------------
/// <summary>
/// Internal method only.
/// </summary>
gslb.PositionedNode.getDefaultDelta = function( iDistanceToTarget, iThreshold, fFactor, iMinimum )
{
  var fNormalized = ( Math.abs( iDistanceToTarget ) / iThreshold );
  var fDelta = Math.pow( fNormalized, 2 ) * fFactor;

  if ( fDelta < iMinimum )
  {
    fDelta = iMinimum;
  }

  // Avoid overshooting
  if ( fDelta > iDistanceToTarget )
  {
    return iDistanceToTarget;
  }
  return Math.round( fDelta );
}

// Utilities ------------------------------------------------------------------

// ----------------------------------------------------------------------------
/// <returns type="int">
/// If the document is parsed as HTML, returns document.body.scrollTop (if available).
/// If the document is parsed as XHTML, returns document.documentElement.scrollTop (if available).
/// If both are not available, returns 0
/// </returns>
gslb.PositionedNode.getScroll = function( strDirection )
{
  return ( document.body && document.body[ "scroll" + strDirection ]
    ? document.body[ "scroll" + strDirection ]
    : ( document.documentElement && document.documentElement[ "scroll" + strDirection ]
      ? document.documentElement[ "scroll" + strDirection ] : 0 ) );
}

//*****************************************************************************
//* Methods *******************************************************************
//*****************************************************************************

// ----------------------------------------------------------------------------
/// <summary>
/// Relocates the Node to a different position on the screen using the
/// repositioning effect.
/// Note: This method doesn't do anything if bRepositionX, resp bRepositionY
/// are set to false in the options.
/// </summary>
/// <param name="iPositionX" type="int">The position along the X axis. Note:
/// this is the position relative to the left of the visible area, not taking
/// the position of the scrollbars in account. If null, no relocation is
/// done along that axis.</param>
/// <param name="iPositionY" type="int">The position along the Y axis. Note:
/// this is the position relative to the top of the visible area, not taking
/// the position of the scrollbars in account. If null, no relocation is
/// done along that axis.</param>
gslb.PositionedNode.prototype.relocate = function( iPositionX, iPositionY )
{
  if ( iPositionX != null
    && this.m_bRepositionX )
  {
    this.m_iRelativeTargetX = iPositionX;
    this.m_bRelocateX = true;
  }

  if ( iPositionY != null
    && this.m_bRepositionY )
  {
    this.m_iRelativeTargetY = iPositionY;
    this.m_bRelocateY = true;
  }
}

// ----------------------------------------------------------------------------
/// <summary>
/// Relocates the Node to a different position on the screen immediately,
/// without any effect.
/// Note: This method doesn't do anything if bRepositionX, resp bRepositionY
/// are set to false in the options.
/// Note 2: Since the repositioning is immediate, the methods defined in
/// fnOnMoveStartX, fnOnMoveStartY, fnOnMoveStart, fnOnMoveEndX,
/// fnOnMoveEndY and fnOnMoveEnd are *not* executed.
/// </summary>
/// <param name="iPositionX" type="int">The position along the X axis. Note:
/// this is the position relative to the left of the visible area, not taking
/// the position of the scrollbars in account. If null, no relocation is
/// done along that axis.</param>
/// <param name="iPositionY" type="int">The position along the Y axis. Note:
/// this is the position relative to the top of the visible area, not taking
/// the position of the scrollbars in account. If null, no relocation is
/// done along that axis.</param>
gslb.PositionedNode.prototype.relocateNoEffect = function( iPositionX, iPositionY )
{
  if ( iPositionX != null
    && this.m_bRepositionX )
  {
    this.m_iPositionX = iPositionX;
    this.m_iTargetX = iPositionX;
    this.m_iRelativeTargetX = iPositionX;
    if ( this.m_oPositionX )
    {
      this.m_oPositionX.setPixelValue( iPositionX );
      this.m_nNode.style.left = this.m_oPositionX.toString();
    }
    else
    {
      this.m_nNode.style.pixelLeft = iPositionX;
    }
  }

  if ( iPositionY != null
    && this.m_bRepositionY )
  {
    this.m_iPositionY = iPositionY;
    this.m_iTargetY = iPositionY;
    this.m_iRelativeTargetY = iPositionY;
    if ( this.m_oPositionY )
    {
      this.m_oPositionY.setPixelValue( iPositionY );
      this.m_nNode.style.top = this.m_oPositionY.toString();
    }
    else
    {
      this.m_nNode.style.pixelTop = iPositionY;
    }
  }
  this.saveCookieIfNeeded();
}

// ----------------------------------------------------------------------------
/// <summary>
/// Pins the Node to its current position. When pinned, the node won't reposition
/// itself.
/// </summary>
/// <param name="bPinNode" type="bool">If true, the Node is pinned. If false,
/// it is unpinned.</param>
gslb.PositionedNode.prototype.pin = function( bPinNode )
{
  this.pinX( bPinNode );
  this.pinY( bPinNode );
}

// ----------------------------------------------------------------------------
/// <summary>
/// Pins the Node to its current position on the X axis. When pinned, the node
/// won't reposition itself.
/// </summary>
/// <param name="bPinNode" type="bool">If true, the Node is pinned. If false,
/// it is unpinned.</param>
gslb.PositionedNode.prototype.pinX = function( bPinNode )
{
  this.m_bPinX = bPinNode;
}

// ----------------------------------------------------------------------------
/// <summary>
/// Pins the Node to its current position on the Y axis. When pinned, the node
/// won't reposition itself.
/// </summary>
/// <param name="bPinNode" type="bool">If true, the Node is pinned. If false,
/// it is unpinned.</param>
gslb.PositionedNode.prototype.pinY = function( bPinNode )
{
  this.m_bPinY = bPinNode;
}

// ----------------------------------------------------------------------------
/// <returns type="bool">Returns true if the DHTML Node is currently
/// moving along the X axis.</param>
gslb.PositionedNode.prototype.isMovingX = function()
{
  return this.m_bIsMovingX;
}

// ----------------------------------------------------------------------------
/// <returns type="bool">Returns true if the DHTML Node is currently
/// moving along the Y axis.</param>
gslb.PositionedNode.prototype.isMovingY = function()
{
  return this.m_bIsMovingY;
}

// ----------------------------------------------------------------------------
/// <returns type="bool">Returns true if the DHTML Node is currently
/// moving along any axis.</param>
gslb.PositionedNode.prototype.isMoving = function()
{
  return this.m_bIsMovingX || this.m_bIsMovingY;
}

// ----------------------------------------------------------------------------
/// <summary>
/// Disposes this instance. After it has been disposed, the Node will not be
/// repositioned anymore. After disposing, it is recommended to set all references
/// to this instance to null.
/// </summary>
gslb.PositionedNode.prototype.dispose = function()
{
  if ( this.m_iTimerId != null )
  {
    this.m_bIsDisposed = true;
    clearTimeout( this.m_iTimerId );
  }
}

// ----------------------------------------------------------------------------
/// <summary>
/// Internal method only.
/// </summary>
gslb.PositionedNode.prototype.checkPosition = function()
{
  if ( this.m_bIsDisposed )
  {
    return;
  }
  if ( !this.m_bPinX )
  {
    this.setPosition( "X" );
  }
  if ( !this.m_bPinY )
  {
    this.setPosition( "Y" );
  }

  var that = this;
  if ( this.isMoving() )
  {
    this.m_iTimerId = setTimeout( function() { that.checkPosition(); }, this.m_iTimerMillisecondsFast );
  }
  else
  {
    this.m_iTimerId = setTimeout( function() { that.checkPosition(); }, this.m_iTimerMillisecondsSlow );
  }
}

// ----------------------------------------------------------------------------
/// <summary>
/// Internal method only.
/// </summary>
gslb.PositionedNode.prototype.setPosition = function( strAxis )
{
  if ( !this[ "m_bReposition" + strAxis ] )
  {
    return false;
  }

  var strDirection = null;
  switch ( strAxis )
  {
    case "X":
      strDirection = "Left";
      break;
    case "Y":
      strDirection = "Top";
      break;
  }

  var iScroll = gslb.PositionedNode.getScroll( strDirection );

  if ( this[ "m_iScroll" + strAxis ] != iScroll
    || this[ "m_bRelocate" + strAxis ] )
  {
    this[ "m_iTarget" + strAxis ] = this[ "m_iRelativeTarget" + strAxis ] + iScroll;
    this[ "m_iScroll" + strAxis ] = iScroll;
    this[ "m_bRelocate" + strAxis ] = false;
    this[ "m_bIsMoving" + strAxis ] = true;

    if ( this[ "m_fnOnMoveStart" + strAxis ] )
    {
      this[ "m_fnOnMoveStart" + strAxis ]();
    }
    if ( this.m_fnOnMoveStart
      && !this.m_bOnMoveStartExecuted )
    {
      this.m_fnOnMoveStart();
      this.m_bOnMoveStartExecuted = true;
    }
  }

  if ( this[ "m_bIsMoving" + strAxis ] )
  {
    var iDelta = this[ "m_fnReposition" + strAxis ]( Math.abs( this[ "m_iTarget" + strAxis ]
      - this[ "m_iPosition" + strAxis ] ) );

    if ( this[ "m_iTarget" + strAxis ] > this[ "m_iPosition" + strAxis ] )
    {
      this[ "m_iPosition" + strAxis ] += iDelta;
      if ( this[ "m_oPosition" + strAxis ] )
      {
        this[ "m_oPosition" + strAxis ].addPixels( iDelta );
        this.m_nNode.style[ strDirection.toLowerCase() ] = this[ "m_oPosition" + strAxis ].toString();
      }
      else
      {
        this.m_nNode.style[ "pixel" + strDirection ] = this[ "m_iPosition" + strAxis ];
      }
    }
    else
    {
      if ( this[ "m_iTarget" + strAxis ] < this[ "m_iPosition" + strAxis ] )
      {
        this[ "m_iPosition" + strAxis ] -= iDelta;
        if ( this[ "m_oPosition" + strAxis ] )
        {
          this[ "m_oPosition" + strAxis ].substractPixels( iDelta );
          this.m_nNode.style[ strDirection.toLowerCase() ] = this[ "m_oPosition" + strAxis ].toString();
        }
        else
        {
          this.m_nNode.style[ "pixel" + strDirection ] = this[ "m_iPosition" + strAxis ];
        }
      }
      else
      {
        this.saveCookieIfNeeded();
        this[ "m_bIsMoving" + strAxis ] = false;
        if ( this[ "m_fnOnMoveEnd" + strAxis ] )
        {
          this[ "m_fnOnMoveEnd" + strAxis ]();
        }
        if ( this.m_fnOnMoveEnd
          && !this.isMoving() )
        {
          this.m_fnOnMoveEnd();
        }
        this.m_bOnMoveStartExecuted = false;
      }
    }
  }
}

// ----------------------------------------------------------------------------
/// <summary>
/// Sets the options for this instance.
/// </summary>
/// <param name="oOptions" type="Object">An Options object, constructed using literal notation.
/// All values are optional.
/// <br />bRepositionX: If true, the node will be repositioned along the X axis. Default: true.
/// <br />bRepositionY: If true, the node will be repositioned along the Y axis. Default: true.
/// <br />iTimerMillisecondsFast: The duration of one iteration (in milliseconds)
/// while the node is being moved. Default: 1 milliseconds.
/// <br />iTimerMillisecondsSlow: The duration of one iteration (in milliseconds)
/// while the node is standing still. Default: 100 milliseconds.
/// <br />fnRepositionX: The function to use to calculate the Node's speed when it is
/// moved along the X axis. See the class' documentation.
/// <br />fnRepositionY: The function to use to calculate the Node's speed when it is
/// moved along the Y axis. See the class' documentation.
/// <br />fnOnMoveStartX: A function which will be called when the Node starts moving
/// along the X axis.
/// <br />fnOnMoveStartY: A function which will be called when the Node starts moving
/// along the Y axis.</param>
/// <br />fnOnMoveStart: A function which will be called when the Node starts moving
/// along any axis.</param>
/// <br />fnOnMoveEndX: A function which will be called when the Node stops moving
/// along the X axis.
/// <br />fnOnMoveEndY: A function which will be called when the Node stops moving
/// along the Y axis.
/// <br />fnOnMoveEnd: A function which will be called when the Node stops moving
/// along both axis.
/// <br />bSaveRelocationInCookie: If true, the new position after relocate was called will be
/// saved in a cookie. Note: Setting this value to false doesn't delete a cookie if it
/// had been set previously. To delete the cookie, you must explicitly call the method
/// deleteCookie. Default: false.
/// <br />iCookieExpirationHours: If bSaveRelocationInCookie is true, this specifies the number
/// of hours during which the Node's new location will be saved
/// in a cookie. If the value is set to 0, this is a session cookie (non-persistent). Default: 0.</param>
gslb.PositionedNode.prototype.setOptions = function( oOptions )
{
  if ( oOptions )
  {
    if ( oOptions.bRepositionX != null )
    {
      this.m_bRepositionX = oOptions.bRepositionX;
    }
    if ( oOptions.bRepositionY != null )
    {
      this.m_bRepositionY = oOptions.bRepositionY;
    }
    if ( oOptions.iTimerMillisecondsFast != null )
    {
      this.m_iTimerMillisecondsFast = oOptions.iTimerMillisecondsFast;
    }
    if ( oOptions.iTimerMillisecondsSlow != null )
    {
      this.m_iTimerMillisecondsSlow = oOptions.iTimerMillisecondsSlow;
    }
    if ( oOptions.fnRepositionX != null )
    {
      this.m_fnRepositionX = oOptions.fnRepositionX;
    }
    if ( oOptions.fnRepositionY != null )
    {
      this.m_fnRepositionY = oOptions.fnRepositionY;
    }
    if ( oOptions.fnOnMoveStartX != null )
    {
      this.m_fnOnMoveStartX = oOptions.fnOnMoveStartX;
    }
    if ( oOptions.fnOnMoveStartY != null )
    {
      this.m_fnOnMoveStartY = oOptions.fnOnMoveStartY;
    }
    if ( oOptions.fnOnMoveStart != null )
    {
      this.m_fnOnMoveStart = oOptions.fnOnMoveStart;
    }
    if ( oOptions.fnOnMoveEndX != null )
    {
      this.m_fnOnMoveEndX = oOptions.fnOnMoveEndX;
    }
    if ( oOptions.fnOnMoveEndY != null )
    {
      this.m_fnOnMoveEndY = oOptions.fnOnMoveEndY;
    }
    if ( oOptions.fnOnMoveEnd != null )
    {
      this.m_fnOnMoveEnd = oOptions.fnOnMoveEnd;
    }

    // Set options on base class
    gslb.PositionedNode.Base.setOptions.call( this, oOptions );
  }
}
