
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Xml;
using System.IO;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Samples.DragDrop
{
	public static class DragDropManager
	{
		private static UIElement _draggedElt;
		private static bool _isMouseDown = false;
		private static Point _startPoint;
		private static DropPreviewAdorner _overlayElt;

		private static IDragSourceAdvisor _currentSourceAdvisor;
		private static IDropTargetAdvisor _currentTargetAdvisor;

		#region Dependency Properties
		public static readonly DependencyProperty DragSourceAdvisorProperty =
			DependencyProperty.RegisterAttached("DragSourceAdvisor", typeof(IDragSourceAdvisor), typeof(DragDropManager),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDragSourceAdvisorChanged)));

		public static readonly DependencyProperty DropTargetAdvisorProperty =
			DependencyProperty.RegisterAttached("DropTargetAdvisor", typeof(IDropTargetAdvisor), typeof(DragDropManager),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDropTargetAdvisorChanged)));

		public static void SetDragSourceAdvisor(DependencyObject depObj, bool isSet)
		{
			depObj.SetValue(DragSourceAdvisorProperty, isSet);
		}

		public static void SetDropTargetAdvisor(DependencyObject depObj, bool isSet)
		{
			depObj.SetValue(DropTargetAdvisorProperty, isSet);
		}

		public static IDragSourceAdvisor GetDragSourceAdvisor(DependencyObject depObj)
		{
			return depObj.GetValue(DragSourceAdvisorProperty) as IDragSourceAdvisor;
		}

		public static IDropTargetAdvisor GetDropTargetAdvisor(DependencyObject depObj)
		{
			return depObj.GetValue(DropTargetAdvisorProperty) as IDropTargetAdvisor;
		}

		#endregion

		#region Property Change handlers
		private static void OnDragSourceAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
		{
			UIElement sourceElt = depObj as UIElement;
			if (args.NewValue != null && args.OldValue == null)
			{
				sourceElt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
				sourceElt.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
				sourceElt.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonUp);
				sourceElt.PreviewGiveFeedback += new GiveFeedbackEventHandler(DragSource_PreviewGiveFeedback);

				// Set the Drag source UI
				IDragSourceAdvisor advisor = args.NewValue as IDragSourceAdvisor;
				advisor.SourceUI = sourceElt;
			}
			else if (args.NewValue == null && args.OldValue != null)
			{
				sourceElt.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
				sourceElt.PreviewMouseMove -= DragSource_PreviewMouseMove;
				sourceElt.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
				sourceElt.PreviewGiveFeedback -= DragSource_PreviewGiveFeedback;
			}
		}

		static void DragSource_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			// Can be used to set custom cursors
		}

		private static void OnDropTargetAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
		{
			UIElement targetElt = depObj as UIElement;
			if (args.NewValue != null && args.OldValue == null)
			{
				targetElt.PreviewDragEnter += new DragEventHandler(DropTarget_PreviewDragEnter);
				targetElt.PreviewDragOver += new DragEventHandler(DropTarget_PreviewDragOver);
				targetElt.PreviewDragLeave += new DragEventHandler(DropTarget_PreviewDragLeave);
				targetElt.PreviewDrop += new DragEventHandler(DropTarget_PreviewDrop);
				targetElt.AllowDrop = true;

				// Set the Drag source UI
				IDropTargetAdvisor advisor = args.NewValue as IDropTargetAdvisor;
				advisor.TargetUI = targetElt;
			}
			else if (args.NewValue == null && args.OldValue != null)
			{
				targetElt.PreviewDragEnter -= DropTarget_PreviewDragEnter;
				targetElt.PreviewDragOver -= DropTarget_PreviewDragOver;
				targetElt.PreviewDragLeave -= DropTarget_PreviewDragLeave;
				targetElt.PreviewDrop -= DropTarget_PreviewDrop;
				targetElt.AllowDrop = false;
			}
		}
		#endregion

		/* ____________________________________________________________________
		 *		Drop Target events 
		 * ____________________________________________________________________
		 */
		static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;

			IDropTargetAdvisor advisor = GetDropTargetAdvisor(sender as DependencyObject);
			Point dropPoint = e.GetPosition(sender as UIElement);

			// Calculate displacement for (Left, Top)
			Point offset = e.GetPosition(_overlayElt);
			dropPoint.X = dropPoint.X - offset.X;
			dropPoint.Y = dropPoint.Y - offset.Y;

			advisor.OnDropCompleted(e.Data, dropPoint);
		}

		static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;

			RemovePreviewAdorner();

			e.Handled = true;
		}

		static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;

			Point position = e.GetPosition(GetTopContainer());
			_overlayElt.Left = position.X - _startPoint.X;
			_overlayElt.Top = position.Y - _startPoint.Y;

			e.Handled = true;
		}

		static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;

			CreatePreviewAdorner();

			e.Handled = true;
		}

		static bool UpdateEffects(object uiObject, DragEventArgs e)
		{
			IDropTargetAdvisor advisor = GetDropTargetAdvisor(uiObject as DependencyObject);
			if (advisor.IsValidDataObject(e.Data) == false) return false;

			if ((e.AllowedEffects & DragDropEffects.Move) == 0 &&
				(e.AllowedEffects & DragDropEffects.Copy) == 0)
			{
				e.Effects = DragDropEffects.None;
				return false;
			}

			if ((e.AllowedEffects & DragDropEffects.Move) != 0 &&
				(e.AllowedEffects & DragDropEffects.Copy) != 0)
			{
				e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
					DragDropEffects.Copy : DragDropEffects.Move;
			}

			return true;
		}

		/* ____________________________________________________________________
		 *		Drag Source events 
		 * ____________________________________________________________________
		 */
		static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Make this the new drag source
			_currentSourceAdvisor = GetDragSourceAdvisor(sender as DependencyObject);

			if (_currentSourceAdvisor.IsDraggable(e.Source as UIElement) == false) return;

			_draggedElt = e.Source as UIElement;
			_startPoint = e.GetPosition(GetTopContainer());
			_isMouseDown = true;

		}

		static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isMouseDown = false;
		}

		static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (_isMouseDown && IsDragGesture(e.GetPosition(GetTopContainer())))
			{
				DragStarted(sender);
			}
		}

		static void DragStarted(object uiObject)
		{
			_isMouseDown = false;
			Mouse.Capture(_draggedElt);

			DataObject data = _currentSourceAdvisor.GetDataObject(_draggedElt);
			DragDropEffects supportedEffects = _currentSourceAdvisor.SupportedEffects;

			// Perform DragDrop
			DragDropEffects effects = System.Windows.DragDrop.DoDragDrop(_draggedElt, data, supportedEffects);
			_currentSourceAdvisor.FinishDrag(_draggedElt, effects);

			// Clean up
			EndDragDrop();
		}

		static bool IsDragGesture(Point point)
		{
			bool hGesture = Math.Abs(point.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance;
			bool vGesture = Math.Abs(point.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance;

			return (hGesture | vGesture);
		}

		/* ____________________________________________________________________
		 *		Utility functions
		 * ____________________________________________________________________
		 */
		static UIElement GetTopContainer()
		{
			UIElement container = Application.Current.MainWindow.Content as UIElement;
			return container;
		}

		private static void CreatePreviewAdorner()
		{
			// Clear if there is an existing adorner
			RemovePreviewAdorner();

			AdornerLayer layer = AdornerLayer.GetAdornerLayer(GetTopContainer());
			UIElement feedbackUI = _currentSourceAdvisor.GetVisualFeedback(_draggedElt);
			_overlayElt = new DropPreviewAdorner(feedbackUI, _draggedElt, layer);
			layer.Add(_overlayElt);
		}

		private static void RemovePreviewAdorner()
		{
			if (_overlayElt != null)
			{
				AdornerLayer.GetAdornerLayer(GetTopContainer()).Remove(_overlayElt);
				_overlayElt = null;
			}
		}

		private static void EndDragDrop()
		{
			Mouse.Capture(null);
			RemovePreviewAdorner();
		}
	}
}
