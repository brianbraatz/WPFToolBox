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

namespace AdvancedCustomTreeViewLayout
{
	public partial class Window1 : System.Windows.Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		void OnNavigateToCity( object sender, RequestNavigateEventArgs e )
		{
			// Show a wait cursor.
			this.ForceCursor = true;
			this.Cursor = Cursors.Wait;

			this.Title = "TreeView Explorer - [Loading...please be patient]";

			// Navigate to the selected URI (this can take a long time).
			this.webPageFrame.NavigationService.Navigate( e.Uri );
		}

		void OnWebPageFrameLoadCompleted( object sender, NavigationEventArgs e )
		{
			// Reset the cursor.
			this.ForceCursor = false;
			this.Cursor = Cursors.Arrow;

			this.Title = "TreeView Explorer";
		} 
	}
}