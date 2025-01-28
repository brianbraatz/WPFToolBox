/* <gslb_version version="V1.1.0" /> */
/* Copyright © GalaSoft Laurent Bugnion 2007 */

//*****************************************************************************
//* gslb.csslength.js
//*****************************************************************************
//* Object name             : CssLength
//* Target Hardware         : PC
//* Target                  : Netscape 6+, Internet explorer 5+
//* Language/Compiler       : ECMAScript V3
//* Author                  : Laurent Bugnion (LBu)
//* Created                 : 25.01.2007
//*****************************************************************************
//* Description: see constructor
//* Last Base Level: BL0002
//*****************************************************************************

//*****************************************************************************
//* Imports *******************************************************************
//*****************************************************************************

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
/// Utility class allowing easier handling of CSS "length" strings,
/// for example "2em" or "150px". This class extracts the unit ("em", "pt", "px", ...),
/// and can also convert from the unit ("em", "pt", ...) to pixels. It can also add
/// or substract pixels, or units ("em", "pt", ...) to the current value. To assign
/// the new value to a style property, you may use the "toString" method.
/// </summary>
/// <param name="strCssString" type="string">A valid CSS "length" string,
/// for example "2em" or "150px".</param>
/// <param name="nNode" type="Node">The specific DOM node to which the conversion
/// will apply. If null, the CSS settings for the BODY will be used, but
/// this may lead to incorrect results, for example if the font-size has
/// been set in the node's hierarchy.</param>
gslb.CssLength = function( strCssString, nNode )
{
  //***************************************************************************
  //* Attributes **************************************************************
  //***************************************************************************
  
  /// <summary type="string">
  /// The unit of the CSS string (em, px...)
  /// </summary>
  this.m_strUnit = gslb.CssLength.getUnit( strCssString );
  /// <summary type="float">
  /// The numeric value in the original unit (for example, for 0.8em, this attribute
  /// bears the value 0.8).
  /// </summary>
  this.m_fUnitValue = parseFloat( strCssString );
  /// <summary type="nNode">
  /// The specific DOM node to which the conversion
  /// will apply. If null, the CSS settings for the BODY will be used, but
  /// this may lead to incorrect results, for example if the font-size has
  /// been set in the node's hierarchy.
  /// </summary>
  this.m_nNode = nNode;
  /// <summary type="float">
  /// The factor used to convert the original unit in pixels, and back.
  /// </summary>
  this.m_fPixelPerUnitFactor = gslb.CssLength.getPixelPerUnitFactor( this.m_strUnit, this.m_nNode );
  
  // Post construction operations ---------------------------------------------
  
  if ( isNaN( this.m_fUnitValue ) )
  {
    throw "Invalid CSS length string: " + strCssString; 
  }
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
gslb.CssLength._NAME = "gslb.CssLength";
/// <summary type="string">
/// This class's version.
/// </summary>
gslb.CssLength._VERSION = "V1.1.0";
/// <summary type="string">
/// This class's date.
/// </summary>
gslb.CssLength._DATE = "04.02.2007";

//*****************************************************************************
//* Static attributes *********************************************************
//*****************************************************************************

//*****************************************************************************
//* Static methods ************************************************************
//*****************************************************************************

// ----------------------------------------------------------------------------
/// <summary>
/// Calculates the factor used to convert the given CSS unit (em, pt...) to pixels.
/// This method makes use of the browser's own CSS engine for the calculation,
/// which might fail on older browsers.
/// </summary>
/// <param name="strUnit" type="string">The CSS unit, for example "em".</param>
/// <param name="nNode" type="Node">The specific DOM node to which the conversion
/// will apply. If null, the CSS settings for the BODY will be used, but
/// this may lead to incorrect results, for example if the font-size has
/// been set in the node's hierarchy.</param>
/// <returns type="float">The factor allowing to convert the given CSS unit
/// to pixels.</returns>
gslb.CssLength.getPixelPerUnitFactor = function( strUnit, nNode )
{
  if ( strUnit.toLowerCase() == "px" )
  {
    return 1;
  }
  
  if ( !document.createElement )
  {
    throw "Unsuitable platform";
  }

  // Create a temporary DIV. For greater precision, use a factor.
  var nDivTemp = document.createElement( "div" );
  nDivTemp.style.width = "10000" + strUnit;
  // Set the DIV to hidden to avoid unwanted visual effects.
  nDivTemp.style.visible = "hidden";

  if ( !document.getElementsByTagName )
  {
    throw "Unsuitable platform";
  }
  
  // The value is only valid if the DIV belongs to the document
  
  var anBody = document.getElementsByTagName( "body" );
  
  if ( !anBody
    || !anBody.length
    || !anBody[0] )
  {
    throw "Unsuitable platform";
  }

  var nParent = anBody[0];
  
  if ( nNode
    && nNode.parentNode )
  {
    // If the conversion must be applied to a
    // specific node, append to this node's parent.
    // This avoids errors if the parent's font size has
    // been set in CSS.
    nParent = nNode.parentNode;
  }
  
  if ( !nParent
    || !nParent.appendChild
    || !nParent.removeChild )
  {
    throw "Unsuitable platform";
  }
  
  // Even though pixelWidth is always set, if the
  // temporary DIV is not appended to the BODY, the
  // value will be invalid, except if font-size
  // is set to 100%.  
  nParent.appendChild( nDivTemp );

  if ( nDivTemp.style.pixelWidth != null )
  {
    var iPixelWidth = nDivTemp.style.pixelWidth;
    nParent.removeChild( nDivTemp );
    return iPixelWidth / 10000;
  }
  
  // pixelWidth is not available --> use clientWidth instead.
  if ( nDivTemp.clientWidth != null )
  {
    var iClientWidth = nDivTemp.clientWidth;
    // Once the value is saved, remove the node.
    nParent.removeChild( nDivTemp );
    return iClientWidth / 10000;
  }
  
  throw "Unsuitable platform";
}

// ----------------------------------------------------------------------------
/// <param name="strCssString" type="string">A valid CSS "length" string,
/// for example "2em" or "150px".</param>
/// <returns type="string">The unit as string, for example "em" or "px".</returns>
gslb.CssLength.getUnit = function( strCssString )
{
  var strUnit = null;
  strCssString = strCssString.toLowerCase();
  if ( strCssString.indexOf( "px" ) > 0 )
  {
    strUnit = "px";
  }
  if ( strCssString.indexOf( "em" ) > 0 )
  {
    strUnit = "em";
  }
  if ( strCssString.indexOf( "pt" ) > 0 )
  {
    strUnit = "pt";
  }
  if ( strUnit == null )
  {
    // Unknown unit --> parse it
    strUnit = "";
    var bStop = false;
    var iIndex = strCssString.length - 1;
    while ( !bStop )
    {
      if ( strCssString.charAt( iIndex ) == ';' )
      {
        continue;
      }
      if ( iIndex < 0
        || !isNaN( parseInt( strCssString.charAt( iIndex ) ) ) )
      {
        // Last digit of the string was met --> exit
        break;
      }
      strUnit = strCssString.charAt( iIndex ) + strUnit;
      iIndex--;
    }
  }
  return strUnit;
}

//*****************************************************************************
//* Methods *******************************************************************
//*****************************************************************************

// ----------------------------------------------------------------------------
/// <returns type="int">The numeric value for this instance converted in pixels.</returns>
gslb.CssLength.prototype.getPixelValue = function()
{
  return this.getPixels( this.m_fUnitValue );
}

// ----------------------------------------------------------------------------
/// <summary>
/// Sets the current value to the given number of pixels.
/// </summary>
/// <param name="iValue" type="int">The number of pixels to set as
/// the present value.</param>
gslb.CssLength.prototype.setPixelValue = function( iValue )
{
  this.setUnitValue( this.getUnits( iValue ) );
}

// ----------------------------------------------------------------------------
/// <returns type="float">The numeric value for this instance in original units
/// (according to the CSS string passed to the constructor of this instance).</returns>
gslb.CssLength.prototype.getUnitValue = function()
{
  return this.m_fUnitValue;
}

// ----------------------------------------------------------------------------
/// <summary>
/// Sets the current value to the given number of units (em, px, pt...).
/// </summary>
/// <param name="fValue" type="float">The number of units to set as
/// the present value.</param>
gslb.CssLength.prototype.setUnitValue = function( fValue )
{
  this.m_fUnitValue = fValue;
}

// ----------------------------------------------------------------------------
/// <returns type="">The unit ("em", "px", ...) of this value.</returns>
gslb.CssLength.prototype.getUnit = function()
{
  return this.m_strUnit;
}

// ----------------------------------------------------------------------------
/// <returns type="">A string representation of this value in original units
/// (according to the CSS string passed to the constructor of this instance).</returns>
gslb.CssLength.prototype.toString = function()
{
  return this.m_fUnitValue.toString() + this.m_strUnit;
}

// ----------------------------------------------------------------------------
/// <returns type="string">A string representation of this value converted
/// to pixels.</returns>
gslb.CssLength.prototype.toStringPixels = function()
{
  return this.getPixelValue().toString() + "px";
}

// ----------------------------------------------------------------------------
/// <summary>
/// Adds a number of pixels to the present value.
/// </summary>
/// <param name="iPixels" type="int">The number of pixels to add to
/// the present value.</param>
gslb.CssLength.prototype.addPixels = function( iPixels )
{
  this.addUnits( this.getUnits( iPixels ) );
}

// ----------------------------------------------------------------------------
/// <summary>
/// Substracts a number of pixels from the present value.
/// </summary>
/// <param name="iPixels" type="int">The number of pixels to substract from
/// the present value.</param>
gslb.CssLength.prototype.substractPixels = function( iPixels )
{
  this.substractUnits( this.getUnits( iPixels ) );
}

// ----------------------------------------------------------------------------
/// <summary>
/// Adds a number of the original units (according to the CSS string passed to
/// the constructor of this instance) to the present value.
/// </summary>
/// <param name="fUnits" type="float">The number of units to add to
/// the present value.</param>
gslb.CssLength.prototype.addUnits = function( fUnits )
{
  this.m_fUnitValue += fUnits;
}

// ----------------------------------------------------------------------------
/// <summary>
/// Substracts a number of the original units (according to the CSS string passed to
/// the constructor of this instance) from the present value.
/// </summary>
/// <param name="fUnits" type="float">The number of units to substract from
/// the present value.</param>
gslb.CssLength.prototype.substractUnits = function( fUnits )
{
  this.m_fUnitValue -= fUnits;
}

// Utilities ------------------------------------------------------------------

// ----------------------------------------------------------------------------
/// <param name="fUnitValue" type="float">A number of units to convert.</param>
/// <returns type="int">The number of pixels corresponding to the number of original units
/// (according to the CSS string passed to the constructor of this instance) passed
/// as parameter.</returns>
gslb.CssLength.prototype.getPixels = function( fUnitValue )
{
  return Math.round( fUnitValue * this.m_fPixelPerUnitFactor );
}

// ----------------------------------------------------------------------------
/// <param name="iPixelValue" type="int">A number of pixels to convert.</param>
/// <returns type="float">The number of original units (according to the CSS string passed
/// to the constructor of this instance) corresponding to the number of pixels
/// passed as parameter.</returns>
gslb.CssLength.prototype.getUnits = function( iPixelValue )
{
  return iPixelValue / this.m_fPixelPerUnitFactor;
}
