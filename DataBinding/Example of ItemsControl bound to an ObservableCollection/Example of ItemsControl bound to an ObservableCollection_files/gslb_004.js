/* <gslb_version version="V1.0.1" /> */
/* Copyright © GalaSoft Laurent Bugnion 2007 */

//*****************************************************************************
//* gslb.positionedbase.js
//*****************************************************************************
//* Control name            : PositionedBase
//* Target Hardware         : PC
//* Target                  : Netscape 6+, Internet explorer 5+
//* Language/Compiler       : ECMAScript V3
//* Author                  : Laurent Bugnion (LBu)
//* Created                 : 16.01.2007
//*****************************************************************************
//* Description: see constructor
//* Last Base Level: BL0002
//*****************************************************************************

//*****************************************************************************
//* Imports *******************************************************************
//*****************************************************************************

// using gslb.Cookie (optional)

//*****************************************************************************
//* Constructor ***************************************************************
//*****************************************************************************

// Create namespace
if ( !window.gslb )
{
  window.gslb = {};
}

// ----------------------------------------------------------------------------
/// <summary>
/// Base class for PositionedNode and DraggedNode. Handles common functionalities
/// like saving cookies.
/// </summary>
/// <param name="oOptions" type="Object">An Options object, constructed using literal notation.
/// All values are optional.
/// <br />bSaveRelocationInCookie: If true, the new position after relocate was called will be
/// saved in a cookie. Note: Setting this value to false doesn't delete a cookie if it
/// had been set previously. To delete the cookie, you must explicitly call the method
/// deleteCookie. Default: false.
/// <br />iCookieExpirationHours: If bSaveRelocationInCookie is true, this specifies the number
/// of hours during which the Node's new location will be saved in a cookie. If
/// iCookieExpirationHours is set to 0, the cookie will only be valid for the
/// current session. Default: 0.</param>
gslb.PositionedBase = function( nNode )
{
  if ( !nNode
    || !nNode.id
    || nNode.style == null
    || nNode.style.left == null
    || nNode.style.top == null
    || nNode.offsetTop == null
    || nNode.offsetLeft == null )
  {
    throw "Impossible to create positioned node (base), please check the documentation";
  }

  //***************************************************************************
  //* Attributes **************************************************************
  //***************************************************************************

  /// <summary>
  /// The DOM node which this control modifies.
  /// </summary>
  this.m_nNode = nNode;
  /// <summary type="String">
  /// The ID used to save cookies with the current state.
  /// </summary>
  this.m_strCookieId = "glsb.PositionedBase." + nNode.id;
  
  // Options
  
  /// <summary>
  /// If true, the new position after relocate was called will be
  /// saved in a cookie. Default: false.
  /// </summary>
  this.m_bSaveRelocationInCookie = null; // Initialize at null to avoid problems
                                         // when setting options the first time
  /// <summary>
  /// If bSaveRelocationInCookie is true, this specifies the number
  /// of hours during which the Node's new location will be saved
  /// in a cookie. Default: 1.
  /// </summary>
  this.m_iCookieExpirationHours = 1;
}

//*****************************************************************************
//* Enums *********************************************************************
//*****************************************************************************

//*****************************************************************************
//* Constants *****************************************************************
//*****************************************************************************

/// <summary type="string">
/// This class's name.
/// </summary>
gslb.PositionedBase._NAME = "gslb.PositionedBase";
/// <summary type="string">
/// This class's version.
/// </summary>
gslb.PositionedBase._VERSION = "V1.0.1";
/// <summary type="string">
/// This class's date.
/// </summary>
gslb.PositionedBase._DATE = "31.01.2007";

//*****************************************************************************
//* Static attributes *********************************************************
//*****************************************************************************

//*****************************************************************************
//* Static methods ************************************************************
//*****************************************************************************

//*****************************************************************************
//* Methods *******************************************************************
//*****************************************************************************

// ----------------------------------------------------------------------------
/// <summary>
/// Sets the options for this instance.
/// </summary>
/// <param name="oOptions" type="Object">An Options object, constructed using literal notation.
/// All values are optional.
/// <br />bSaveRelocationInCookie: If true, the new position after relocate was called will be
/// saved in a cookie. Note: Setting this value to false doesn't delete a cookie if it
/// had been set previously. To delete the cookie, you must explicitly call the method
/// deleteCookie. Default: false.
/// <br />iCookieExpirationHours: If bSaveRelocationInCookie is true, this specifies the number
/// of hours during which the Node's new location will be saved
/// in a cookie. Default: 0.</param>
gslb.PositionedBase.prototype.setOptions = function( oOptions )
{
  if ( oOptions )
  {
    if ( oOptions.iCookieExpirationHours != null )
    {
      this.m_iCookieExpirationHours = oOptions.iCookieExpirationHours;
    }
    if ( oOptions.bSaveRelocationInCookie != null )
    {
      var bCurrentSave = this.m_bSaveRelocationInCookie;
      this.m_bSaveRelocationInCookie = oOptions.bSaveRelocationInCookie;

      // If the option to save the cookie changes to true, save current position
      if ( bCurrentSave == false // Do not use !bCurrentSave to avoid problems when initializing
        && oOptions.bSaveRelocationInCookie )
      {
        this.saveCookieIfNeeded();
      }
    }
  }
}

// ----------------------------------------------------------------------------
/// <summary>
/// Restores the saved position of the Node, according to the value in the cookie.
/// Note: the position wil only be restored if bSaveRelocationInCookie is true
/// in the options.
/// </summary>
gslb.PositionedBase.prototype.restorePosition = function()
{
  if ( !this.m_bSaveRelocationInCookie )
  {
    return;
  }
  if ( !gslb.Cookie )
  {
    return;
  }
  
  var strPosition = gslb.Cookie.getCookie( this.m_strCookieId );
  if ( strPosition )
  {
    var astrPosition = strPosition.split( ";" );
    if ( astrPosition.length == 2 )
    {
      this.m_nNode.style.left = astrPosition[ 0 ];
      this.m_nNode.style.top = astrPosition[ 1 ];
    }
  }
}

// ----------------------------------------------------------------------------
/// <summary>
/// Saves the current position in a cookie. The position will only be saved if
/// bSaveRelocationInCookie is true in the options.
/// If iCookieExpirationHours is set to 0, the cookie will only be valid for the
/// current session. If the value is bigger than 0, it will be set for a corresponding
/// number of hours.
/// </summary>
gslb.PositionedBase.prototype.saveCookieIfNeeded = function()
{
  if ( !gslb.Cookie )
  {
    return;
  }

  if ( this.m_bSaveRelocationInCookie )
  {
    if ( this.m_iCookieExpirationHours >= 0 )
    {
      if ( !gslb.Cookie )
      {
        throw "Error in PositionedBase: gslb.Cookie not found";
      }
      var strPosition = "" + this.m_nNode.style.left + ";" + this.m_nNode.style.top;
      if ( this.m_iCookieExpirationHours > 0 )
      {
        gslb.Cookie.setCookie( this.m_strCookieId,
          strPosition,
          gslb.Cookie.getDateTimeFromNow( 0, 0, 0, this.m_iCookieExpirationHours, 0, 0, 0 ) );
      }
      else
      {
        gslb.Cookie.setCookie( this.m_strCookieId, strPosition );
      }
    }
  }
}

// ----------------------------------------------------------------------------
/// <summary>
/// Deletes the cookie with the Mode's saved position.
/// Note: Setting bSaveRelocationInCookie to false in the options doesn't delete a cookie if it
/// had been set previously. To delete the cookie, you must explicitly call the method
/// deleteCookie.
/// </summary>
gslb.PositionedBase.prototype.deleteCookie = function()
{
  if ( !gslb.Cookie )
  {
    return;
  }
  gslb.Cookie.deleteCookie( this.m_strCookieId );
}
