using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CustomControls
{
	public class FishEyeGrid : Grid
	{
		#region Constructor
		public FishEyeGrid()
			: base()
		{
			//Default orientation.
			this.Orientation = Orientation.Horizontal;
			this.Loaded += new RoutedEventHandler(FishEyeGrid_Loaded);
		}


		#endregion Constructor

		#region Properties
		public static DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FishEyeGrid), new PropertyMetadata(new PropertyChangedCallback(OnOrientationPropertyChanged)));
		/// <summary>
		/// Orientation of items inside Grid, Vertical or Horizontal.
		/// </summary>
		public Orientation Orientation
		{
			get
			{
				return (Orientation)this.GetValue(OrientationProperty);
			}

			set
			{
				this.SetValue(OrientationProperty, value);
			}
		}

		/// <summary>
		/// IndexOfChildUnderMouse.
		/// </summary>
		public int IndexOfChildUnderMouse
		{
			get
			{
				for (int i = 0; i < this.Children.Count; ++i)
				{
					if (true == this.Children[i].IsMouseOver)
					{
						return i;
					}
				}
				return -1;
			}
		}
		#endregion Properties

		#region Methods
		void FishEyeGrid_Loaded(object sender, RoutedEventArgs e)
		{
			List<UIElement> children = new List<UIElement>();
			foreach (UIElement child in this.Children)
			{
				children.Add(child);
			}
			this.Children.Clear();
			foreach (UIElement child in children)
			{
				this.Children.Add(child);
			}
		}
		private static void OnOrientationPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			((FishEyeGrid)target).RegenerateGridRowsColumns();
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			RegenerateGridRowsColumns();
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}


		/// <summary>
		/// Adds rows/columns with star width and rearranges items to fit into the row/columns.
		/// </summary>
		private void RegenerateGridRowsColumns()
		{
			RowDefinitionCollection rowDefinitions = this.RowDefinitions;
			ColumnDefinitionCollection columnDefinitions = this.ColumnDefinitions;
			rowDefinitions.Clear();
			columnDefinitions.Clear();
			int index = 0;
			foreach (UIElement child in this.Children)
			{
				if (null != child)
				{
					if (Orientation.Vertical == this.Orientation)
					{
						RowDefinition rowDefinition = new RowDefinition();
						rowDefinition.Height = new GridLength(1, GridUnitType.Star);
						rowDefinition.MaxHeight = double.PositiveInfinity;
						rowDefinition.MinHeight = 0;
						rowDefinitions.Add(rowDefinition);
						child.SetValue(Grid.RowProperty, index);
					}
					else if (Orientation.Horizontal == this.Orientation)
					{
						ColumnDefinition columnDefinition = new ColumnDefinition();
						columnDefinition.Width = new GridLength(1, GridUnitType.Star);
						columnDefinition.MaxWidth = double.PositiveInfinity;
						columnDefinition.MinWidth = 0;
						columnDefinitions.Add(columnDefinition);
						child.SetValue(Grid.ColumnProperty, index);
					}
					child.SetValue(Control.HeightProperty, double.NaN);
					child.SetValue(Control.WidthProperty, double.NaN);
					child.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
					child.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Stretch);
					child.SetValue(Grid.MarginProperty, new Thickness(0, 0, 0, 0));
					++index;
				}
			}
		}


		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			ChangeGridRowColumnSize();
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
		{
			RestoreGridRowColumnSize();
			base.OnMouseLeave(e);
		}

		private void ChangeGridRowColumnSize()
		{
			Point mousePos = System.Windows.Input.Mouse.GetPosition(this);

			double mousePercentY = mousePos.Y / this.ActualHeight;
			double mousePercentX = mousePos.X / this.ActualWidth;

			double variance = .25; //how narrow or wide the function is

			for (int i = 0; i < this.RowDefinitions.Count; ++i)
			{

				double positionPercent = (double)i / this.RowDefinitions.Count;
				double power = (-Math.Pow(((mousePercentY - positionPercent)), 2)) / (2 * variance * variance);
				double newSize = 1 + this.RowDefinitions[i].Height.Value * Math.Pow(Math.E, power);
				this.RowDefinitions[i].Height = new GridLength(newSize, GridUnitType.Star);
			}

			for (int i = 0; i < this.ColumnDefinitions.Count; ++i)
			{

				double positionPercent = (double)i / this.ColumnDefinitions.Count;
				double power = (-Math.Pow(((mousePercentX - positionPercent)), 2)) / (2 * variance * variance);
				double newSize = 1 + this.ColumnDefinitions[i].Width.Value * Math.Pow(Math.E, power);
				this.ColumnDefinitions[i].Width = new GridLength(newSize, GridUnitType.Star);
			}
		}


		private void RestoreGridRowColumnSize()
		{
			GridLength unitGridLength = new GridLength(1, GridUnitType.Star);
			foreach (RowDefinition rd in this.RowDefinitions)
			{
				rd.Height = unitGridLength;
			}

			foreach (ColumnDefinition cd in this.ColumnDefinitions)
			{
				cd.Width = unitGridLength;
			}
		}
		#endregion  Methods

	}
}
