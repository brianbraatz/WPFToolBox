using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Timers;
using GadgetFramework;
using System.Collections.ObjectModel;

namespace SlideShowGadget
{
    public partial class Gadget : GadgetFramework.Gadget
    {
        SlideShowModel model;
        public Gadget()
            : base()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
            model = (SlideShowModel)((ObjectDataProvider)this.Resources["SlideShowModelDS"]).Data;
            this.PropertyPageContent = new PropertiesPage(model);
            this.DesiredUndockedGadgetHeight = 250;
            this.DesiredUndockedGadgetWidth = 300;
        }

        private DelegateCommand playPauseCommand;
        public DelegateCommand PlayPauseCommand
        {
            get
            {
                if (playPauseCommand == null)
                {
                    playPauseCommand = new DelegateCommand(this.OnPlayPause);
                }
                return playPauseCommand;
            }
        }

        private DelegateCommand viewImageCommand;
        public DelegateCommand ViewImageCommand
        {
            get
            {
                if (viewImageCommand == null)
                {
                    viewImageCommand = new DelegateCommand(this.ViewImage);
                }
                return viewImageCommand;
            }
        }
	

        private void OnPlayPause(object arg)
        {
            if (!this.PlayPauseButton.IsChecked.Value)
            {
                this.ThrobbingItemsControl.IsThrobbing = false;
            }
            else
            {
                this.ThrobbingItemsControl.IsThrobbing = true;
            }
        }

        private void ViewImage(object arg)
        {
            System.Diagnostics.Process.Start(this.model.ImageCollection[this.ThrobbingItemsControl.CurrentPageIndex]);
        }

        public override string GadgetName
        {
            get { return "Slide Show"; }
        }

        public override string Description
        {
            get { return "A slide-show gadget"; }
        }
    }

    public class SlideShowModel
    {
        private string directoryPath = @"D:\Users\Public\Pictures\Sample Pictures";
        public string DirectoryPath
        {
            get { return directoryPath; }
            set
            { 
                directoryPath = value;
                this.ReInitializeImageCollection();
            }  
        }
	
        private ObservableCollection<string> imageCollection = new ObservableCollection<string>();
        public ObservableCollection<string> ImageCollection
        {
            get { return this.imageCollection; }
        }

        public SlideShowModel()
        {
            this.ReInitializeImageCollection();
        }

        private void ReInitializeImageCollection()
        {
            imageCollection.Clear();
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(this.DirectoryPath);

                FileInfo[] files = directoryInfo.GetFiles("*.jpg");
                for (int i = 0; i < files.Length; ++i)
                {
                    imageCollection.Add(files[i].FullName);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}