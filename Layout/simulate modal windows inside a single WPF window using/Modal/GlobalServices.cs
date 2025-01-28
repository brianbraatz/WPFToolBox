using System;
using System.Collections.Generic;
using System.Text;

namespace Modal
{
        public delegate void BackNavigationEventHandler(bool dialogReturn);
    public interface IModalService
    {
        void NavigateTo(MyUserControl uc, BackNavigationEventHandler backFromDialog);
        void GoBackward(bool dialogReturnValue);
    }

    public class GlobalServices
    {
        public static IModalService ModalService
        {
            get
            {
                
                return (IModalService)App.Current.MainWindow;
            }
        }
    }
}
