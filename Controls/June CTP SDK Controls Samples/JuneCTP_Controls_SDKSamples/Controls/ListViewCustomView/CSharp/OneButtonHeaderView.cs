using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace SDKSample
{
    public class OneButtonHeaderView : ViewBase
    {
        protected override object DefaultStyleKey
        {
          get
          {
            return new ComponentResourceKey(this.GetType(), 
                             "OneButtonHeaderViewDSK");
          }
        }

        protected override object ItemContainerDefaultStyleKey
        {
          get
          {
            return new ComponentResourceKey(this.GetType(), 
                            "OneButtonHeaderViewItemDSK");
          }
        }
    }
}