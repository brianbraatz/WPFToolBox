﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2425 $</version>
// </file>

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Xaml;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	/// <summary>
	/// Base class for model tests.
	/// </summary>
	public class ModelTestHelper
	{
		protected StringBuilder log;
		
		protected XamlDesignContext CreateContext(string xaml)
		{
			log = new StringBuilder();
			XamlDesignContext context = new XamlDesignContext(new XmlTextReader(new StringReader(xaml)));
			/*context.Services.Component.ComponentRegistered += delegate(object sender, DesignItemEventArgs e) {
				log.AppendLine("Register " + ItemIdentity(e.Item));
			};
			context.Services.Component.ComponentUnregistered += delegate(object sender, DesignItemEventArgs e) {
				log.AppendLine("Unregister " + ItemIdentity(e.Item));
			};*/
			return context;
		}
		
		protected DesignItem CreateCanvasContext(string xaml)
		{
			XamlDesignContext context = CreateContext(@"<Canvas
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  " + xaml + "</Canvas>");
			Canvas canvas = (Canvas)context.RootItem.Component;
			DesignItem canvasChild = context.Services.Component.GetDesignItem(canvas.Children[0]);
			Assert.IsNotNull(canvasChild);
			
			return canvasChild;
		}
		
		protected void AssertCanvasDesignerOutput(string expectedXaml, DesignContext context)
		{
			expectedXaml =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" +
				("<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
				 "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n" + expectedXaml.Trim())
				.Replace("\r", "").Replace("\n", "\n  ")
				+ "\n</Canvas>";
			
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
			xmlWriter.Formatting = Formatting.Indented;
			context.Save(xmlWriter);
			
			string actualXaml = stringWriter.ToString().Replace("\r", "");;
			if (expectedXaml != actualXaml) {
				Debug.WriteLine("expected xaml:");
				Debug.WriteLine(expectedXaml);
				Debug.WriteLine("actual xaml:");
				Debug.WriteLine(actualXaml);
			}
			Assert.AreEqual(expectedXaml, actualXaml);
		}
		
		static string ItemIdentity(DesignItem item)
		{
			return item.ComponentType.Name + " (" + item.GetHashCode() + ")";
		}
		
		protected void AssertLog(string expectedLog)
		{
			expectedLog = expectedLog.Replace("\r", "");
			string actualLog = log.ToString().Replace("\r", "");
			if (expectedLog != actualLog) {
				Debug.WriteLine("expected log:");
				Debug.WriteLine(expectedLog);
				Debug.WriteLine("actual log:");
				Debug.WriteLine(actualLog);
			}
			Assert.AreEqual(expectedLog, actualLog);
		}
	}
}
