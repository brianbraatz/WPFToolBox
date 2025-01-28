using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Ink;
using GadgetFramework;

namespace ScratchPadGadget
{
	public partial class Gadget
	{
        private ObservableCollection<StrokeCollection> notes = new ObservableCollection<StrokeCollection>();

        private int currentNoteIndex;
        public int CurrentNoteIndex
        {
            get { return currentNoteIndex; }
            set
            { 
                currentNoteIndex = value;
                this.OnNotifyPropertyChanged("CurrentNoteIndex");
            }
        }

        private int totalNoteCount;
        public int TotalNoteCount
        {
            get { return totalNoteCount; }
            set
            { 
                totalNoteCount = value;
                this.OnNotifyPropertyChanged("TotalNoteCount");
            }
        }

        public StrokeCollection CurrentNote
        {
            get { return notes[this.CurrentNoteIndex - 1]; }
        }

        private DelegateCommand deleteNoteCommand;
        public DelegateCommand DeleteNoteCommand
        {
            get
            {
                if (this.deleteNoteCommand == null)
                {
                    this.deleteNoteCommand = new DelegateCommand(this.OnDeleteNote);
                }
                return this.deleteNoteCommand;
            }
        }

        private DelegateCommand addNoteCommand;
        public DelegateCommand AddNoteCommand
        {
            get
            {
                if (this.addNoteCommand == null)
                {
                    this.addNoteCommand = new DelegateCommand(this.OnAddNote);
                }
                return this.addNoteCommand;
            }
        }

        private DelegateCommand previousNoteCommand;
        public DelegateCommand PreviousNoteCommand
        {
            get
            {
                if (this.previousNoteCommand == null)
                {
                    this.previousNoteCommand = new DelegateCommand(this.OnPreviousNote);
                }
                return this.previousNoteCommand;
            }
        }

        private DelegateCommand nextNoteCommand;
        public DelegateCommand NextNoteCommand
        {
            get
            {
                if (this.nextNoteCommand == null)
                {
                    this.nextNoteCommand = new DelegateCommand(this.OnNextNote);
                }
                return this.nextNoteCommand;
            }
        }

        private DelegateCommand editingModeCommand;
        public DelegateCommand EditingModeCommand
        {
            get
            {
                if (this.editingModeCommand == null)
                {
                    this.editingModeCommand = new DelegateCommand(this.OnEditingModeChanged);
                }
                return this.editingModeCommand;
            }
        }

		public Gadget()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
            this.DesiredUndockedGadgetHeight = 300;
            this.DesiredUndockedGadgetWidth = 350;

            notes.Add(new StrokeCollection());
            this.CurrentNoteIndex = 1;
            this.TotalNoteCount = 1;

            this.PreviousNoteCommand.IsEnabled = false;
            this.NextNoteCommand.IsEnabled = false;
		}

		public override string GadgetName
		{
			get { return "Scratch Pad"; }
		}

		public override string Description
		{
			get { return "A gadget that lets you scribble as you think..."; }
		}

        private void OnDeleteNote(object arg)
        {
            if (this.notes.Count != 1)
            {
                this.notes.RemoveAt(currentNoteIndex - 1);
                this.CurrentNoteIndex = Math.Max(1, this.CurrentNoteIndex - 1);
                this.TotalNoteCount = this.TotalNoteCount - 1;
            }
            else
            {
                this.notes.Clear();
                notes.Add(new StrokeCollection());
            }

            if (this.CurrentNoteIndex == 1)
            {
                this.PreviousNoteCommand.IsEnabled = false;
            }

            if (this.CurrentNoteIndex == this.notes.Count)
            {
                this.NextNoteCommand.IsEnabled = false;
            }

            this.OnNotifyPropertyChanged("CurrentNote");
        }

        private void OnAddNote(object arg)
        {
            this.notes.Add(new StrokeCollection());
            this.CurrentNoteIndex = this.notes.Count;
            this.TotalNoteCount = this.TotalNoteCount + 1;
            this.OnNotifyPropertyChanged("CurrentNote");

            this.PreviousNoteCommand.IsEnabled = true;
            this.NextNoteCommand.IsEnabled = false;
        }

        private void OnNextNote(object arg)
        {
            this.CurrentNoteIndex = this.CurrentNoteIndex + 1;
            
            this.PreviousNoteCommand.IsEnabled = true;
            if (this.CurrentNoteIndex == this.notes.Count)
            {
                this.NextNoteCommand.IsEnabled = false;
            }

            this.OnNotifyPropertyChanged("CurrentNote");
        }

        private void OnPreviousNote(object arg)
        {
            this.CurrentNoteIndex = this.CurrentNoteIndex - 1;

            this.NextNoteCommand.IsEnabled = true;
            if (this.CurrentNoteIndex == 1)
            {
                this.PreviousNoteCommand.IsEnabled = false;
            }

            this.OnNotifyPropertyChanged("CurrentNote");
        }

        private void OnEditingModeChanged(object arg)
        {
            if (this.EditMode.IsChecked.Value)
            {
                this.InkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
            else
            {
                this.InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        }
	}
}