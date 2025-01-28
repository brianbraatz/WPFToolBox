using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.Generic;

namespace Modal
{
	public partial class Window1 :  IModalService
	{
        private Stack<BackNavigationEventHandler> _backFunctions;
    
		public Window1()
		{
			this.InitializeComponent();
            _backFunctions = new Stack<BackNavigationEventHandler>();
            
			// Insert code required on object creation below this point.
		}

        public virtual void NavigateTo(MyUserControl uc, BackNavigationEventHandler backFromDialog)
        {
            foreach (UIElement item in modalGrid.Children)
                item.IsEnabled = false;
            modalGrid.Children.Add(uc);
            _backFunctions.Push(backFromDialog);
        }

        public virtual void GoBackward(bool dialogReturnValue)
        {
            modalGrid.Children.RemoveAt(modalGrid.Children.Count - 1);
            UIElement element = modalGrid.Children[modalGrid.Children.Count - 1];
            element.IsEnabled = true;

            BackNavigationEventHandler handler = _backFunctions.Pop();
            if (handler != null)
                handler(dialogReturnValue);
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalServices.ModalService.NavigateTo(new MyUserControl(), delegate(bool returnValue)
            {
                if (returnValue)
                    MessageBox.Show("Return value == true");
                else
                    MessageBox.Show("Return value == false");
            });

            modalButton.IsEnabled = true;
        }
	}
}