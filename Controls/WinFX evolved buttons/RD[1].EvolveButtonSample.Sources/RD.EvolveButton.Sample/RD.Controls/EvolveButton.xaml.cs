#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
#endregion

namespace RD.Controls
{
	public partial class EvolveButton : Button
  {
    #region Fields
    public static readonly DependencyProperty ButtonTypeProperty;
    #endregion

    #region Ctors
    static EvolveButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EvolveButton), new FrameworkPropertyMetadata(typeof(EvolveButton), FrameworkPropertyMetadataOptions.None));
			EvolveButton.ButtonTypeProperty = DependencyProperty.Register("ButtonType", typeof(EvolveButtonType), typeof(EvolveButton), new FrameworkPropertyMetadata(EvolveButtonType.OK, FrameworkPropertyMetadataOptions.None));
		}
		public EvolveButton()
		{
			this.InitializeComponent();
    }
    #endregion

    #region Properties
    public EvolveButtonType ButtonType
		{
			get { return (EvolveButtonType)base.GetValue(EvolveButton.ButtonTypeProperty); }
			set { base.SetValue(EvolveButton.ButtonTypeProperty, value); }
    }
    #endregion
  }
}
