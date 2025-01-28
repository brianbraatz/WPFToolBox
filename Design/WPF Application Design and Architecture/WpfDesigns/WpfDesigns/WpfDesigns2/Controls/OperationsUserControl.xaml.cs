using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDesigns2.Controls
{
    /// <summary>
    /// Interaction logic for OperationsUserControl.xaml
    /// </summary>
    public partial class OperationsUserControl : UserControl
    {
        public static readonly DependencyProperty MatchTextProperty =
            DependencyProperty.Register("MatchText", typeof(string), typeof(OperationsUserControl), new UIPropertyMetadata("*"));

        public event EventHandler<ReflectionOperationEventArgs> ReflectionOperationComplete;
        private ReflectionHelper _reflectionHelper;

        public OperationsUserControl()
        {
            InitializeComponent();

            this._reflectionHelper = new ReflectionHelper();
        }

        public void ShowAssemblies_Click(object sender, RoutedEventArgs e)
        {
            List<string> reflectionResults = this._reflectionHelper.FindAssemblyNames(this.MatchText);

            OnReflectionOperationComplete(new ReflectionOperationEventArgs(reflectionResults));
        }
        public void ShowTypes_Click(object sender, RoutedEventArgs e)
        {
            List<string> reflectionResults = this._reflectionHelper.FindTypeNames(this.MatchText);

            OnReflectionOperationComplete(new ReflectionOperationEventArgs(reflectionResults));
        }

        public string MatchText
        {
            get { return (string)GetValue(MatchTextProperty); }
            set { SetValue(MatchTextProperty, value); }
        }

        protected virtual void OnReflectionOperationComplete(ReflectionOperationEventArgs e)
        {
            if (this.ReflectionOperationComplete != null)
            {
                this.ReflectionOperationComplete(this, e);
            }
        }
    }
}
