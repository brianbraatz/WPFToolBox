using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Samples.DragDrop
{
    public interface IDropTargetAdvisor
    {
		UIElement TargetUI
		{
			get;
			set;
		}
		bool IsValidDataObject(IDataObject obj);
		void OnDropCompleted(IDataObject obj, Point dropPoint);
   }
}
