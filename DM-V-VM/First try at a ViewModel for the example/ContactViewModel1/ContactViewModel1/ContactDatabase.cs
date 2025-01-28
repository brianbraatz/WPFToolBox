using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

// Demo model
// Pure, simple data model with minimal dependencies on other technologies
// Represents a list of Contacts such as an address book program might have
//
namespace ContactsModel
{
	public enum Gender
	{
		Male,
		Female
	}

	// The contact class is simple and useless, it just has features for
	// demo'ing what a pre-existing Model in Model-View-ViewModel pattern might
	// look like
	//
	public class Contact
	{
		private string name;
		private Gender gender;
		private string favoriteColor;

		public Contact(string name, Gender gender, string favoriteColor)
		{
			this.name = name;
			this.gender = gender;
			this.favoriteColor = favoriteColor;
		}

		public string Name
		{
			get { return this.name; }
		}

		public Gender Gender
		{
			get { return this.gender; }
		}

		// string for this property will be used as a demonstration
		// of dataconversion in ViewModel
		// Real World app would not probably represent color this way
		public string FavoriteColor
		{
			get { return this.favoriteColor; }
		}

		// Always useful for debugging, and also useful for this example to see
		// what default data binding does
		public override string ToString()
		{
			return this.name + ": " + this.Gender.ToString();
		}
	}


	// The contacts database can change "independent of the UI"...
	// this is to simulate a case where the contact database is a web service
	// or database that the UI is bound against and the UI must update
	// when it changes
	// The event args are intentionally different from the INotifyCollectionChanged
	// pattern that WPF uses to show how a ViewModel can be used to adapt to other
	// patterns.
	public enum ContactsChangedType
	{
		ContactAdded,
		ContactRemoved
	}

	public class ContactsChangedEventArgs : EventArgs
	{
		private ContactsChangedType changeType;
		private int index;
		private Contact changedItem;

		public ContactsChangedEventArgs(ContactsChangedType changeType, int index, Contact changedItem)
		{
			this.changeType = changeType;
			this.index = index;
			this.changedItem = changedItem;
		}

		public ContactsChangedType ListChangedType { get { return this.changeType; } }
		public int Index { get { return this.index; } }
		public Contact ChangedItem { get { return this.changedItem; } }
	}

	public delegate void ContactsChangedEventHandler(object sender, ContactsChangedEventArgs args);


	// This is the contact database itself.  For demo purposes this is very simple
	// and only uses basic Framework constructs, not advanced WinFX ones.
	public class ContactDatabase
	{
		// The Model fires its own change notifications, but they don't match
		// the INotifyCollectionChanged pattern that data-binding expects
		public event ContactsChangedEventHandler ContactDatabaseChanged;

		private List<Contact> contacts = new List<Contact>();
		public ContactDatabase()
		{
		}

		// This simulates a common pattern where the data is cached
		// or other not accessible directly because it is coming from
		// a web service or database.  To actually modify the data you
		// have to go through the API (AddContact, RemoveContact in this case).
		// I a Real World example, the individual Contact data items would also
		// be proxies or read-only ...
		public ReadOnlyCollection<Contact> Contacts
		{
			get
			{
				return this.contacts.AsReadOnly();
			}
		}

		public void AddContact(Contact newContact)
		{
			this.contacts.Add(newContact);
			this.OnContactDatabaseChanged(ContactsChangedType.ContactAdded, this.contacts.Count - 1, newContact);
		}

		public void RemoveContact(int index)
		{
			Contact contact = this.contacts[index];
			this.contacts.Remove(contact);
			this.OnContactDatabaseChanged(ContactsChangedType.ContactRemoved, index, contact);
		}

		private void OnContactDatabaseChanged(ContactsChangedType changeType, int index, Contact contact)
		{
			if (this.ContactDatabaseChanged != null)
			{
				this.ContactDatabaseChanged(this, new ContactsChangedEventArgs(changeType, index, contact));
			}
		}
	}

	// This are NOT part of the Model-View-ViewModel pattern
	// The methods here are helpers to set up certain situations for demo purposes.
	// In a Real World application the Contacts would be loaded from a web service or database,
	// be manipulated by multiple clients etc...
	public static class ContactsDemoUtilities
	{
		public static ContactDatabase CreateDummyDatabase()
		{
			ContactDatabase newDatabase = new ContactDatabase();

			Contact newContact = new Contact("John", Gender.Male, "Blue");
			newDatabase.AddContact(newContact);
			newContact = new Contact("Mike", Gender.Male, "Green");
			newDatabase.AddContact(newContact);

			for (int i = 1; i < 6; i++)
			{
				newContact = new Contact("George" + i, Gender.Male, "Red");
				newDatabase.AddContact(newContact);
			}

			return newDatabase;		
		}
	}
}
