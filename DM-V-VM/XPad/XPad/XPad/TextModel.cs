using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.IO;


namespace XPad
{
    public class TextModel : DependencyObject, INotifyPropertyChanged, ITextModel
    {
        private PropertyChangedEventHandler _propertyChangedEvent;
        private Dispatcher _dispatcher;
        private string currentFileName = string.Empty;

        /// <summary>
        /// the IsSaved field
        /// </summary>
        private bool mIsSaved;
        /// <summary>
        /// Gets or sets the IsSaved
        /// </summary>
        public bool IsSaved
        {
            get { return mIsSaved; }
            set { mIsSaved = value; }
        }

        public string CurrentFileName
        {
            get { return currentFileName; }
            set { currentFileName = value; }
        }
        public TextModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            this.Text = "Default text.";
            SendPropertyChanged("Text");
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); SendPropertyChanged("Text"); }
        }


        public void LoadFile(string fileName, string content)
        {
            currentFileName = fileName;
            
        }

        public void SaveFile()
        {
            
        
        }
        /// <summary>
        /// The Dispatcher associated with the model.
        /// </summary>
        public Dispatcher Dispatcher
        {
            get { return null;// _dispatcher; 
            }
        }
      

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextModel));


        /// <summary>
        /// PropertyChanged event for INotifyPropertyChanged implementation.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                VerifyCalledOnUIThread();
                _propertyChangedEvent += value;
            }
            remove
            {
                VerifyCalledOnUIThread();
                _propertyChangedEvent -= value;
            }
        }

        /// <summary>
        /// Debugging utility to make sure functions are called on the UI thread.
        /// </summary>
        [Conditional("Debug")]
        protected void VerifyCalledOnUIThread()
        {
            //Debug.Assert(Dispatcher.CurrentDispatcher == this.Dispatcher, "Call must be made on UI thread.");
        }
        /// <summary>
        /// Utility function for use by subclasses to notify that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void SendPropertyChanged(string propertyName)
        {
            VerifyCalledOnUIThread();
            if (_propertyChangedEvent != null)
            {
                _propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
