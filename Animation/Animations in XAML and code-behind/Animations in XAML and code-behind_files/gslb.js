/* <gslb_version version="V1.0.0" /> */
/* Copyright © GalaSoft Laurent Bugnion 2007 */

//*****************************************************************************
//* gslb.Object.js
//*****************************************************************************
//* Project Name           : Utilities
//* Class Name             : Object
//* Target Hardware        : PC
//* Target                 : Netscape 4+, Internet explorer 4+
//* Language/Compiler      : JavaScript 1.2
//* Author                 : Laurent Bugnion
//* Created                : 20.01.2007
//*****************************************************************************
//* Description:
//* Set of utility functions.
//* Last Base Level: BL0001
//*****************************************************************************

//*****************************************************************************
//* Imports *******************************************************************
//*****************************************************************************

// using none

// Create namespace
if ( !window.gslb )
{
  window.gslb = {};
}

//*****************************************************************************
//* Constructor ***************************************************************
//*****************************************************************************

gslb.Object = function()
{
}

//*****************************************************************************
//* Constants *****************************************************************
//*****************************************************************************

/// <summary type="String">
/// This class's name.
/// </summary>
gslb.Object._NAME = "Object";
/// <summary type="String">
/// This class's version.
/// </summary>
gslb.Object._VERSION = "V1.0.0";
/// <summary type="String">
/// This class's date.
/// </summary>
gslb.Object._DATE = "20.01.2007";

//*****************************************************************************
//* Static methods ************************************************************
//*****************************************************************************

// ----------------------------------------------------------------------------
gslb.Object.extend = function( Derived, Base )
{
  function Inheritance() {}
  
  Inheritance.prototype = Base.prototype;

  Derived.prototype = new Inheritance();
  Derived.prototype.constructor = Derived;
  Derived.Base = Base.prototype;
  Derived.Base.fnConstructor = Base;
}