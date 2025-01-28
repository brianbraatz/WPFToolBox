using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace Wpf3DGraphingLibrary
{
	/// <summary>
	/// Represents one bar in a bar graph.
	/// </summary>
    internal class Bar : BaseGraphPiece
	{
		#region Fields
    	
    	private double _Width, _Height, _Depth, _X, _Z;
		private Color _BarColor;
		private Color _HighlightColor;
		private Transform3DGroup _Transforms;
		private GeometryModel3D _Geometry;
		private bool _IsHighlighted = false, _CanHighlight = false;
		private TimeSpan _HighlightDuration;

		#endregion
		
		#region Properties
		
        /// <summary>
        /// Width of the bar.
        /// </summary>
		internal double Width 
		{
			get { return _Width; }
			set { _Width = value; }
		}
		
		/// <summary>
		/// Height of the bar.
		/// </summary>
		internal double Height 
		{
			get { return _Height; }
			set { _Height = value; }
		}
		
		/// <summary>
		/// Depth of the bar.
		/// </summary>
		internal double Depth 
		{
			get { return _Depth; }
			set { _Depth = value; }
		}
		
		/// <summary>
		/// X position of the bar in the scene.
		/// </summary>
		internal double X
		{
			get { return _X; }
			set { _X = value; }
		}
		
		/// <summary>
		/// Z position of the bar in the scene.
		/// </summary>
		internal double Z
		{
			get { return _Z; }
			set { _Z = value; }
		}
		
		/// <summary>
		/// Bar color.
		/// </summary>
		internal Color BarColor
		{
			get { return _BarColor; }
			set { _BarColor = value; }
		}
		
		/// <summary>
		/// Highlight color
		/// </summary>
		internal Color HighlightColor
		{
			get { return _HighlightColor; }
			set { _HighlightColor = value; }
		}
		
		/// <summary>
		/// Time needed to switch between bar color and highlight color.
		/// </summary>
		internal TimeSpan HighlightDuration 
		{
			get { return _HighlightDuration; }
			set { _HighlightDuration = value; }
		}
		
		/// <summary>
		/// Whether or not the bar is highlighted.
		/// </summary>
		internal bool IsHighlighted
		{
			get { return _IsHighlighted; }
		}
		
		/// <summary>
		/// Whether or not the bar can be highlighted.
		/// </summary>
		internal bool CanHighlight
		{
			get { return _CanHighlight; }
			set { _CanHighlight = value; }
		}

		/// <summary>
		/// Gets the current geometry.
		/// </summary>
		public override GeometryModel3D Geometry 
		{
			get { return _Geometry; }
		}

		#endregion
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		internal Bar(object key, object value) : base(key, value) {}
		
        /// <summary>
        /// Creates the geometry for this bar.
        /// </summary>
        /// <returns></returns>
		protected internal override void Generate()
		{
			if (_Geometry == null)
			{
				double w = _Width / 2d;
				double h = _Height;
				double d = _Depth / 2d;
				double b = h < 0 ? h : 0;
				h = h < 0 ? 0 : h;
				MeshGeometry3D mg3d = new MeshGeometry3D();
				
				// First set up the triangle positions.
				mg3d.Positions.Add(new Point3D(-w, b, -d)); 	// front, lower left - 0
				mg3d.Positions.Add(new Point3D(w, b, -d));		// front, lower right - 1
				mg3d.Positions.Add(new Point3D(w, h, -d));		// front, upper right - 2
				mg3d.Positions.Add(new Point3D(-w, h, -d));		// front, upper left - 3
				
				mg3d.Positions.Add(new Point3D(w, b, d)); 		// back, lower left - 4
				mg3d.Positions.Add(new Point3D(-w, b, d));		// back, lower right - 5
				mg3d.Positions.Add(new Point3D(-w, h, d));		// back, upper right - 6
				mg3d.Positions.Add(new Point3D(w, h, d));		// back, upper left - 7
				
				int[] triIndices = {
					0, 3, 2, 2, 1, 0, // Front
					0, 1, 4, 4, 5, 0, // Bottom
					0, 5, 3, 3, 5, 6, // Right
					6, 7, 3, 3, 7, 2, // Top
					2, 4, 1, 2, 7, 4, // Left
					4, 7, 6, 6, 5, 4, // Back
				};
				foreach (int i in triIndices)
					mg3d.TriangleIndices.Add(i);
				
				_Geometry = new GeometryModel3D();
				_Geometry.Geometry = mg3d;
				MaterialGroup matg = new MaterialGroup();
				matg.Children.Add(new DiffuseMaterial(new SolidColorBrush(_BarColor)));
				matg.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 85));
				_Geometry.Material = matg;
				
				_Transforms = new Transform3DGroup();
				TranslateTransform3D translate = new TranslateTransform3D(_X, 0d, _Z);
				_Transforms.Children.Add(translate);
				_Geometry.Transform = _Transforms;
			}
		}
		
		internal void Highlight()
		{
			if (_CanHighlight && !_IsHighlighted)
			{
				ColorAnimation ca = new ColorAnimation(_HighlightColor, _HighlightDuration);
				MaterialGroup matg = _Geometry.Material as MaterialGroup;
				foreach (Material m in matg.Children)
				{
					if (m is DiffuseMaterial)
					{
						DiffuseMaterial dm = m as DiffuseMaterial;
						dm.Brush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
						_IsHighlighted = true;
						break;
					}
				}
			}
		}
		
		internal void Unhighlight()
		{
			if (_CanHighlight && _IsHighlighted) 
			{
				ColorAnimation ca = new ColorAnimation(_BarColor, _HighlightDuration);
				MaterialGroup matg = _Geometry.Material as MaterialGroup;
				foreach (Material m in matg.Children)
				{
					if (m is DiffuseMaterial)
					{
						DiffuseMaterial dm = m as DiffuseMaterial;
						dm.Brush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
						_IsHighlighted = false;
						break;
					}
				}
			}
		}
		
		/// <summary>
		/// Sets the color of this bar.
		/// </summary>
		/// <param name="c"></param>
		internal void SetColor(Color c)
		{
			SetColor(c, TimeSpan.Zero);
		}
		
		/// <summary>
		/// Sets the color of this bar using an animation with the given duration.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="dur"></param>
		internal void SetColor(Color c, Duration dur)
		{
			_BarColor = c;
			if (!_IsHighlighted)
			{
				ColorAnimation ca = new ColorAnimation(_BarColor, dur);
				MaterialGroup matg = _Geometry.Material as MaterialGroup;
				foreach (Material m in matg.Children)
				{
					if (m is DiffuseMaterial)
					{
						DiffuseMaterial dm = m as DiffuseMaterial;
						dm.Brush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
						break;
					}
				}
			}
		}
	}
}
