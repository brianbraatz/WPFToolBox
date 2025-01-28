using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

using ContactsModel;
using ContactsViewModel;

// The ContactView namespace is where view-only classes and constructs go
namespace ContactsView
{
	public partial class Scene1
	{
		private ContactDatabase contactDatabase;

		public Scene1()
		{			
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			// This is NOT part of the Model-View-ViewModel pattern 
			// The data context can be initialized many ways, this just 
			// requires very little code
			this.contactDatabase = ContactsDemoUtilities.CreateDummyDatabase();
			this.MasterDetailContainer.DataContext = new ContactDatabaseViewModel(this.contactDatabase);
		}
	}
}
