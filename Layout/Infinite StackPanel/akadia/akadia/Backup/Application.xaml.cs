namespace Akadia
{
	using System.Windows;

	public partial class MainApplication: System.Windows.Application
	{
		// This function gets called when the application starts up.
		// It displays the first window created. 
		protected override void OnStartup(StartupEventArgs e)
		{
			this.MainWindow = new Window1();
			this.MainWindow.Show();

			base.OnStartup(e);
		}
	}
}
