using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls.Breadcrumb
{
  /// <summary>
  /// Interaction logic for Breadcrumb.xaml
  /// </summary>
  public partial class Breadcrumb : System.Windows.Controls.UserControl
  {
    public Breadcrumb()
    {
      InitializeComponent();

      
    }

    /// <summary>
    /// Create Breadcrumb ComboBox with BreadcrumbComboBoxStyle from resource
    /// </summary>
    /// <returns>Breadcrumb ComboBox</returns>
    public System.Windows.Controls.ComboBox CreateBreadcrumbComboBox()
    {
      System.Windows.Controls.ComboBox breadcrumbComboBox = new System.Windows.Controls.ComboBox();
      breadcrumbComboBox.Style = (Style)FindResource("BreadcrumbComboBoxStyle");
      breadcrumbComboBox.BorderBrush = null;
      breadcrumbComboBox.BorderThickness = new System.Windows.Thickness(0);
      breadcrumbComboBox.IsEditable = true; // Anders is textbox niet zichtbaar
      breadcrumbComboBox.IsReadOnly = true;
      return breadcrumbComboBox;
    }

    /// <summary>
    /// Bread crumb comboboxes visible
    /// </summary>
    public bool BreadcrumbVisible
    {
      get { return (BreadcrumbBorder.Visibility == Visibility.Visible); }
      set { BreadcrumbBorder.Visibility = (value) ? Visibility.Visible : Visibility.Hidden; }
    }

    /// <summary>
    /// Return bread crumb comboboxes
    /// </summary>
    public UIElementCollection BreadcrumbItems
    {
      get { return BreadcrumbStackPanel.Children; }
    }

    
  }
}