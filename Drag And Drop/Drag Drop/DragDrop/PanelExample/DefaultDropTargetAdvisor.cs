using System;
using System.Collections.Generic;
using System.Text;
using Samples.DragDrop;
using System.Windows;
using System.Xml;
using System.Windows.Markup;
using System.IO;
using System.Windows.Controls;

namespace Samples.DragDrop
{
	public class DefaultDropTargetAdvisor : IDropTargetAdvisor
	{
		private static DataFormat SupportedFormat = DataFormats.GetDataFormat("Samples");
		private UIElement _targetUI;

		public bool IsValidDataObject(IDataObject obj)
		{
			return obj.GetDataPresent(SupportedFormat.Name);
		}

		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			string serializedObject = obj.GetData(SupportedFormat.Name) as string;
			XmlReader reader = XmlReader.Create(new StringReader(serializedObject));
			UIElement elt = XamlReader.Load(reader) as UIElement;

			(TargetUI as Panel).Children.Add(elt);
		}

		public UIElement TargetUI
		{
			get
			{
				return _targetUI;
			}
			set
			{
				_targetUI = value;
			}
		}
	}
}
