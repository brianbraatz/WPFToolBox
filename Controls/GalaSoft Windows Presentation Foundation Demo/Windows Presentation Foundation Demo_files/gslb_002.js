/* <gslb_version version="V0.0.0/BL0001" /> */
/* Copyright © GalaSoft Laurent Bugnion 2006 - 2007 */
/* Last Base Level: BL0001 */

if ( !window.gslb )
{
  window.gslb = function()
  {
  }
}

gslb.closeAll = function( strTag, strIdPrefix )
{
  var anElements = document.getElementsByTagName( strTag );
  for ( var index = 0; index < anElements.length; index++ )
  {
    var nElement = anElements[ index ];
    if ( nElement.id
      && nElement.id.indexOf( strIdPrefix ) == 0
      && nElement.style
      && nElement.style.display )
    {
      nElement.style.display = "none";
    }
  }
}

gslb.toggleVisibility = function( strId, bForceOpen, lnkToggle, strTextWhenOpen, strTextWhenClosed )
{
  var nNode = document.getElementById( strId );
  if ( nNode
    && nNode.style
    && nNode.style.display )
  {
    if ( nNode.style.display == "none"
      || bForceOpen )
    {
      nNode.style.display = "block";      
      if ( lnkToggle
        && lnkToggle.firstChild
        && lnkToggle.firstChild.nodeValue
        && strTextWhenOpen != null )
      {
        lnkToggle.firstChild.nodeValue = strTextWhenOpen;
      }
    }
    else
    {
      nNode.style.display = "none";      
      if ( lnkToggle
        && lnkToggle.firstChild
        && lnkToggle.firstChild.nodeValue
        && strTextWhenClosed != null )
      {
        lnkToggle.firstChild.nodeValue = strTextWhenClosed;
      }
    }
  }
}
