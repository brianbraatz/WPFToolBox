using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace TheWpfWay.Controls
{
	[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
	[TemplatePart(Name = "PART_MinimizeButton", Type = typeof(Button))]
	[TemplatePart(Name = "PART_MaximizeButton", Type = typeof(Button))]
	[TemplatePart(Name = "PART_NResizer", Type = typeof(Path))]
	[TemplatePart(Name = "PART_SResizer", Type = typeof(Path))]
	[TemplatePart(Name = "PART_EResizer", Type = typeof(Path))]
	[TemplatePart(Name = "PART_WResizer", Type = typeof(Path))]
	[TemplatePart(Name = "PART_NWResizer", Type = typeof(Rectangle))]
	[TemplatePart(Name = "PART_NEResizer", Type = typeof(Rectangle))]
	[TemplatePart(Name = "PART_SWResizer", Type = typeof(Rectangle))]
	[TemplatePart(Name = "PART_SEResizer", Type = typeof(Rectangle))]
	[TemplatePart(Name = "PART_TitleBar", Type = typeof(Grid))]
	public class GlassWindow : Window
	{
		private WindowInteropHelper _interopHelper;

		static GlassWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GlassWindow),
				new FrameworkPropertyMetadata(typeof(GlassWindow)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_interopHelper = new WindowInteropHelper(this);
			AttachToVisualTree();
		}

		private void AttachToVisualTree()
		{
			// Close Button
			Button closeButton = GetChildControl("PART_CloseButton") as Button;
			if (closeButton != null)
				closeButton.Click += new RoutedEventHandler(OnCloseButtonClick);

			// Minimize Button
			Button minimizeButton = GetChildControl("PART_MinimizeButton") as Button;
			if (minimizeButton != null)
				minimizeButton.Click += new RoutedEventHandler(OnMinimizeButtonClick);

			// Maximize Button
			Button maximizeButton = GetChildControl("PART_MaximizeButton") as Button;
			if (maximizeButton != null)
				maximizeButton.Click += new RoutedEventHandler(OnMaximizeButtonClick);

			// Resizers
			if (ResizeMode != ResizeMode.NoResize)
			{
				Path sResizer = GetChildControl("PART_SResizer") as Path;
				if (sResizer != null)
					sResizer.MouseDown += new MouseButtonEventHandler(OnSizeSouth);

				Path nResizer = GetChildControl("PART_NResizer") as Path;
				if (nResizer != null)
					nResizer.MouseDown += new MouseButtonEventHandler(OnSizeNorth);

				Path eResizer = GetChildControl("PART_EResizer") as Path;
				if (eResizer != null)
					eResizer.MouseDown += new MouseButtonEventHandler(OnSizeEast);

				Path wResizer = GetChildControl("PART_WResizer") as Path;
				if (wResizer != null)
					wResizer.MouseDown += new MouseButtonEventHandler(OnSizeWest);

				Rectangle seResizer = GetChildControl("PART_SEResizer") as Rectangle;
				if (seResizer != null)
					seResizer.MouseDown += new MouseButtonEventHandler(OnSizeSouthEast);

				Rectangle swResizer = GetChildControl("PART_SWResizer") as Rectangle;
				if (swResizer != null)
					swResizer.MouseDown += new MouseButtonEventHandler(OnSizeSouthWest);

				Rectangle neResizer = GetChildControl("PART_NEResizer") as Rectangle;
				if (neResizer != null)
					neResizer.MouseDown += new MouseButtonEventHandler(OnSizeNorthEast);

				Rectangle nwResizer = GetChildControl("PART_NWResizer") as Rectangle;
				if (nwResizer != null)
					nwResizer.MouseDown += new MouseButtonEventHandler(OnSizeNorthWest);
			}

			// Title Bar
			Grid titleBar = GetChildControl("PART_TitleBar") as Grid;
			if (titleBar != null)
				titleBar.MouseLeftButtonDown += new MouseButtonEventHandler(OnTitleBarMouseDown);
		}

		private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
		{
			ToggleMaximize();
		}

		private void ToggleMaximize()
		{
			this.WindowState = (this.WindowState == WindowState.Maximized) ?
			                   WindowState.Normal : WindowState.Maximized;
		}

		void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (ResizeMode != ResizeMode.NoResize && e.ClickCount == 2)
			{
				ToggleMaximize();
				return;
			}
			this.DragMove();
		}

		#region Sizing Event Handlers

		void OnSizeSouth(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.South);
		}

		void OnSizeNorth(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.North);
		}

		void OnSizeEast(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.East);
		}

		void OnSizeWest(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.West);
		}

		void OnSizeNorthWest(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.NorthWest);
		}

		void OnSizeNorthEast(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.NorthEast);
		}

		void OnSizeSouthEast(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.SouthEast);
		}

		void OnSizeSouthWest(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragSize(_interopHelper.Handle, SizingAction.SouthWest);
		}


		#endregion

		#region P/Invoke and Helper Method

		const int WM_SYSCOMMAND = 0x112;
		const int SC_SIZE = 0xF000;


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		void DragSize(IntPtr handle, SizingAction sizingAction)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + sizingAction), IntPtr.Zero);
				SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
			}
		}

		public enum SizingAction
		{
			North = 3,
			South = 6,
			East = 2,
			West = 1,
			NorthEast = 5,
			NorthWest = 4,
			SouthEast = 8,
			SouthWest = 7
		}

		#endregion

		private DependencyObject GetChildControl(string ctrlName)
		{
			DependencyObject ctrl = GetTemplateChild(ctrlName);
			return ctrl;
		}

		void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}
	}
}
