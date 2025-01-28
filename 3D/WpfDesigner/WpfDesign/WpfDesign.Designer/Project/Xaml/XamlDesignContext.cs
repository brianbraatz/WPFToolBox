﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2440 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Extensions;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	/// <summary>
	/// The design context implementation used when editing XAML.
	/// </summary>
	public sealed class XamlDesignContext : DesignContext
	{
		readonly XamlDocument _doc;
		readonly XamlDesignItem _rootItem;
		internal readonly XamlComponentService _componentService;
		
		internal XamlDocument Document {
			get { return _doc; }
		}
		
		/// <summary>
		/// Creates a new XamlDesignContext instance.
		/// </summary>
		public XamlDesignContext(XmlReader xamlReader)
		{
			if (xamlReader == null)
				throw new ArgumentNullException("xamlReader");
			
			this.Services.AddService(typeof(ISelectionService), new DefaultSelectionService());
			this.Services.AddService(typeof(IToolService), new DefaultToolService(this));
			this.Services.AddService(typeof(UndoService), new UndoService());
			this.Services.AddService(typeof(IErrorService), new DefaultErrorService());
			this.Services.AddService(typeof(ViewService), new DefaultViewService(this));
			this.Services.AddService(typeof(OptionService), new OptionService());
			
			_componentService = new XamlComponentService(this);
			this.Services.AddService(typeof(IComponentService), _componentService);
			
			EditorManager propertyGridEditorManager = new EditorManager();
			this.Services.AddService(typeof(EditorManager), propertyGridEditorManager);
			
			// register extensions from this assembly:
			this.Services.ExtensionManager.RegisterAssembly(typeof(XamlDesignContext).Assembly);
			propertyGridEditorManager.RegisterAssembly(typeof(XamlDesignContext).Assembly);
			
			XamlParserSettings xamlParseSettings = new XamlParserSettings();
			xamlParseSettings.CreateInstanceCallback = this.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory;
			_doc = XamlParser.Parse(xamlReader, xamlParseSettings);
			_rootItem = _componentService.RegisterXamlComponentRecursive(_doc.RootElement);
		}
		
		/// <summary>
		/// Saves the XAML DOM into the XML writer.
		/// </summary>
		public override void Save(System.Xml.XmlWriter writer)
		{
			_doc.Save(writer);
		}
		
		/// <summary>
		/// Gets the root item being designed.
		/// </summary>
		public override DesignItem RootItem {
			get { return _rootItem; }
		}
		
		/// <summary>
		/// Opens a new change group used to batch several changes.
		/// ChangeGroups work as transactions and are used to support the Undo/Redo system.
		/// </summary>
		public override ChangeGroup OpenGroup(string changeGroupTitle, ICollection<DesignItem> affectedItems)
		{
			if (affectedItems == null)
				throw new ArgumentNullException("affectedItems");
			
			UndoService undoService = this.Services.GetService<UndoService>();
			if (undoService == null)
				throw new ServiceRequiredException(typeof(UndoService));
			UndoTransaction g = undoService.StartTransaction(affectedItems);
			g.Title = changeGroupTitle;
			return g;
		}
	}
}
