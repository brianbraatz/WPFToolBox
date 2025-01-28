﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2225 $</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Delegate used for XamlParserSettings.CreateInstanceCallback.
	/// </summary>
	public delegate object CreateInstanceCallback(Type type, object[] arguments);
	
	/// <summary>
	/// Settings used for the XamlParser.
	/// </summary>
	public sealed class XamlParserSettings
	{
		CreateInstanceCallback _createInstanceCallback = Activator.CreateInstance;
		XamlTypeFinder _typeFinder = XamlTypeFinder.CreateWpfTypeFinder();
		
		/// <summary>
		/// Gets/Sets the method used to create object instances.
		/// </summary>
		public CreateInstanceCallback CreateInstanceCallback {
			get { return _createInstanceCallback; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				_createInstanceCallback = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets the type finder to do type lookup.
		/// </summary>
		public XamlTypeFinder TypeFinder {
			get { return _typeFinder; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				_typeFinder = value;
			}
		}
	}
}
