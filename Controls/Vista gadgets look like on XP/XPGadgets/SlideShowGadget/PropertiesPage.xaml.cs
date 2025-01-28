using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Forms;
using GadgetFramework;
using System.ComponentModel;

namespace SlideShowGadget
{
	public partial class PropertiesPage : INotifyPropertyChanged
	{
        SlideShowModel model;

        private string selectedDirectory;

        public string SelectedDirectory
        {
            get { return selectedDirectory; }
            set 
            { 
                selectedDirectory = value;
                this.OnPropertyChanged("SelectedDirectory");
            }
        }
	

        private DelegateCommand commitChangesCommand;
        public DelegateCommand CommitChangesCommand
        {
            get
            {
                if (this.commitChangesCommand == null)
                {
                    this.commitChangesCommand = new DelegateCommand(this.OnCommitChangesCommand);
                }
                return this.commitChangesCommand;
            }
        }

		public PropertiesPage(SlideShowModel model)
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
            this.model = model;
            this.SelectedDirectory = model.DirectoryPath;
		}

        private void OnShowFileBrowseDialog(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = model.DirectoryPath;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.SelectedDirectory = folderBrowserDialog.SelectedPath;
            }
        }

        private void OnCommitChangesCommand(object arg)
        {
            model.DirectoryPath = this.SelectedDirectory;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

	}
}