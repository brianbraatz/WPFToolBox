using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Samples.DragDrop
{
    public interface IDragSourceAdvisor
    {
		UIElement SourceUI
		{
			get;
			set;
		}

		DragDropEffects SupportedEffects
		{
			get;
		}
    	
		DataObject GetDataObject(UIElement draggedElt);
		UIElement GetVisualFeedback(UIElement draggedElt);
		void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects);
		bool IsDraggable(UIElement dragElt);
    }
}
