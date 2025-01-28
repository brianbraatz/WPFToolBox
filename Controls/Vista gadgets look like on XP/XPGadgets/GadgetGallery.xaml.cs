using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Collections.ObjectModel;
using GadgetFramework;
using System.ComponentModel;

namespace XPGadgets
{
	public partial class GadgetGallery
	{
        private const string defaultSearchString = "Search Gadgets...";
        private GadgetGalleryModel model;
        
		public GadgetGallery()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
            this.model = (GadgetGalleryModel)((ObjectDataProvider)this.Resources["GadgetGalleryModelDS"]).Data;
            this.GadgetList.SelectedIndex = 0;
		}

		private void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
		{
			this.DragMove();
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

        private void OnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.SearchTextBox.Text != defaultSearchString)
            {
                bool shouldFilterGadget;
                int firstPossibleSelectionIndex = -1;
                for(int index = 0; index < this.model.Gadgets.Count; ++index)
                {
                    GadgetDisplay gadgetDisplay = this.model.Gadgets[index];
                    shouldFilterGadget = this.ShouldFilterGadget(gadgetDisplay.Gadget);
                    if (shouldFilterGadget)
                    {
                        if (gadgetDisplay.IsFiltered == false)
                        {
                            gadgetDisplay.IsFiltered = true;
                        }
                    }
                    else
                    {
                        if (gadgetDisplay.IsFiltered == true)
                        {
                            gadgetDisplay.IsFiltered = false;
                        }

                        if (firstPossibleSelectionIndex == -1)
                        {
                            firstPossibleSelectionIndex = index;
                        }
                    }
                }

                this.GadgetList.SelectedIndex = firstPossibleSelectionIndex;
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SearchTextBox.SelectAll();
        }

        private void OnGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            this.SearchTextBox.SelectAll();
        }

        private bool ShouldFilterGadget(Gadget gadget)
        {
            string searchText = this.SearchTextBox.Text.Trim();
            if (gadget.GadgetName.Contains(searchText) || gadget.Description.Contains(searchText))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
	}

	public class GadgetGalleryModel
	{
        private ObservableCollection<GadgetDisplay> gadgets = new ObservableCollection<GadgetDisplay>();
        public ObservableCollection<GadgetDisplay> Gadgets
		{
			get { return gadgets; }
		}

		public GadgetGalleryModel()
		{
			foreach (Type gadgetType in KnownGadgets.Gadgets)
			{
				Gadget gadget = (Gadget)Activator.CreateInstance(gadgetType);
				gadget.GadgetState = GadgetState.Gallery;
				this.Gadgets.Add(new GadgetDisplay(gadget));
			}
		}	
	}

    public class GadgetDisplay : INotifyPropertyChanged
    {
        private Gadget gadget;
        public Gadget Gadget
        {
            get { return gadget; }
            set { gadget = value; }
        }

        private bool isFiltered;
        public bool IsFiltered
        {
            get { return isFiltered; }
            set
            { 
                isFiltered = value;
                this.OnPropertyChanged("IsFiltered");
            }
        }

        public GadgetDisplay(Gadget gadget)
        {
            this.Gadget = gadget;
            this.IsFiltered = false;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}