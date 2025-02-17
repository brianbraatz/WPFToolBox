﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2246 $</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Contains constants used by the Xaml parser.
	/// </summary>
	public static class XamlConstants
	{
		/// <summary>
		/// The namespace used to identify "xmlns".
		/// Value: "http://www.w3.org/2000/xmlns/"
		/// </summary>
		public const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";
		
		/// <summary>
		/// The namespace used for the XAML schema.
		/// Value: "http://schemas.microsoft.com/winfx/2006/xaml"
		/// </summary>
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		
		/// <summary>
		/// The namespace used for the WPF schema.
		/// Value: "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		/// </summary>
		public const string PresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
	}
}
