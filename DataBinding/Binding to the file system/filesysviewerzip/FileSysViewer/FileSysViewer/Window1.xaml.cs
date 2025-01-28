using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace FileSysViewer
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : System.Windows.Window
	{
		public Window1()
		{
			InitializeComponent();

			this.CommandBindings.Add(new CommandBinding(
				ApplicationCommands.Open, 
				this.ExecuteOpen, 
				this.CanExecuteOpen));			
		}

		/// <summary>
		/// Handles the Open command.
		/// </summary>
		void ExecuteOpen(object sender, ExecutedRoutedEventArgs e)
		{
			Hyperlink link = e.OriginalSource as Hyperlink;
			if (link == null)
				return;

			FileInfo file = link.DataContext as FileInfo;
			if (file == null)
				return;

			try
			{
				Process.Start(file.FullName);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error occurred when opening file: " + ex.Message);
			}
		}

		void CanExecuteOpen(object sender, CanExecuteRoutedEventArgs e)
		{			
			e.CanExecute = true;
		}
	}
}