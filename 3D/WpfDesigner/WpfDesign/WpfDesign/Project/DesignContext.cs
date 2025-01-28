﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2420 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// The context that the designer uses.
	/// </summary>
	public abstract class DesignContext
	{
		readonly ServiceContainer _services = new ServiceContainer();
		
		/// <summary>
		/// Creates a new DesignContext instance.
		/// </summary>
		protected DesignContext()
		{
			_services.AddService(typeof(Extensions.ExtensionManager), new Extensions.ExtensionManager(this));
		}
		
		/// <summary>
		/// Gets the <see cref="ServiceContainer"/>.
		/// </summary>
		public ServiceContainer Services {
			[DebuggerStepThrough]
			get { return _services; }
		}
		
		/// <summary>
		/// Gets the root design item.
		/// </summary>
		public abstract DesignItem RootItem {
			get;
		}
		
		/// <summary>
		/// Save the designed elements as XML.
		/// </summary>
		public abstract void Save(XmlWriter writer);
		
		/// <summary>
		/// Opens a new change group used to batch several changes.
		/// ChangeGroups work as transactions and are used to support the Undo/Redo system.
		/// </summary>
		public abstract ChangeGroup OpenGroup(string changeGroupTitle, ICollection<DesignItem> affectedItems);
	}
}
