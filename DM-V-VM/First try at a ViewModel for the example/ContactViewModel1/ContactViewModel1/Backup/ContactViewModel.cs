using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
// for INotifyCollectionChanged
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
// for ICommand
using System.Windows.Input;

using ContactsModel;


namespace ContactsViewModel
{
	public class ContactDatabaseViewModel
	{
		private ContactDatabase model;
		private ICommand addCommand;
		private ICommand removeCommand;

		// Interesting terminology clash...what WPF calls the ListCollectionView, I would
		// actually call the ListCollectionViewModel...it adapts a pure model collection
		// so it can be databound to by a ListBox or other control, which is what we think
		// of as the actual "View"
		private ContactsCollectionView contactsView;

		public ContactDatabaseViewModel(ContactDatabase model)
		{
			this.model = model;
			this.contactsView = new ContactsCollectionView(this.model); 
		}

		public CollectionView Contacts
		{
			get
			{
				return this.contactsView;
			}
		}

		public ICommand Add
		{
			get
			{
				if (this.addCommand == null)
				{
					this.addCommand = new AddCommand(this);
				}
				return this.addCommand;
			}
		}

		public ICommand Remove
		{
			get
			{
				if (this.removeCommand == null)
				{
					this.removeCommand = new RemoveCommand(this);
				}
				return this.removeCommand;
			}
		}

		#region Commands

		// Base command class for this viewmodel
		// Sometimes helpful to implement a base class for commands targeting
		// a particular type of model
		private abstract class ContactDatabaseCommand : ICommand
		{
			protected ContactDatabaseViewModel viewModel;

			public ContactDatabaseCommand(ContactDatabaseViewModel viewModel)
			{
				this.viewModel = viewModel;
			}

			public abstract void Execute(object parameter);

			public event EventHandler CanExecuteChanged;
			// Most commands are enabled most of the time on this model
			public virtual bool CanExecute(object parameter)
			{
				return true;
			}

			protected void OnCanExecuteChanged()
			{
				if (this.CanExecuteChanged != null)
				{
					this.CanExecuteChanged(this, EventArgs.Empty);
				}
			}
		}

		private class AddCommand : ContactDatabaseCommand
		{
			public AddCommand(ContactDatabaseViewModel viewModel) : base(viewModel)
			{
			}

			public override void Execute(object parameter)
			{
				// Simplification for demo purposes
				// In Real World application we'd have a more complex way to decide the new Contact
				this.viewModel.model.AddContact(new Contact("New Guy", Gender.Male, "Unknown"));
			}
		}

		private class RemoveCommand : ContactDatabaseCommand
		{
			public RemoveCommand(ContactDatabaseViewModel viewModel) : base(viewModel)
			{
				this.viewModel.Contacts.CurrentChanged += new EventHandler(Contacts_CurrentChanged);
			}

			void  Contacts_CurrentChanged(object sender, EventArgs e)
			{
				this.OnCanExecuteChanged();
			}

			public override bool CanExecute(object parameter)
			{
				// Command is enabled if something is selected in the contactsView
				return (this.viewModel.contactsView.CurrentPosition >= 0);
			}

			public override void Execute(object parameter)
			{
				this.viewModel.model.RemoveContact(this.viewModel.contactsView.CurrentPosition);
			}
		}
		#endregion
	}

	class ContactsCollectionView : ListCollectionView
	{
		private ContactDatabase model;

		public ContactsCollectionView(ContactDatabase model) : base(model.Contacts)
		{
			this.model = model;
			this.model.ContactDatabaseChanged += new ContactsChangedEventHandler(model_ContactDatabaseChanged);
		}

		void model_ContactDatabaseChanged(object sender, ContactsChangedEventArgs args)
		{
			NotifyCollectionChangedEventArgs newArgs = null;
			switch (args.ListChangedType)
			{
				case ContactsChangedType.ContactAdded:
					newArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, args.ChangedItem, args.Index);
					break;
				case ContactsChangedType.ContactRemoved:
					newArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, args.ChangedItem, args.Index);
					break;
			}

			this.OnCollectionChanged(newArgs);
		}
	}
}
