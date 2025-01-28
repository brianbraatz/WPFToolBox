using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Threading;
using _3DTools;

namespace Wpf3DGraphingLibrary
{
	/// <summary>
	/// Represents one piece in a pie graph.
	/// </summary>
    internal class PiePiece : BaseGraphPiece
	{
    	#region Fields

    	internal const double RadStep = PieGraph.PI_2 / 360d; // The step is 1 degree at a time.
		
		private double _StartRadix, _EndRadix;
		private Color _Color;
		private Color _HighlightColor;
		private GeometryModel3D _Geometry;
		private ScreenSpaceLines3D _Outline;
		private bool _IsHighlighted = false, _CanHighlight = false;
		private TimeSpan _HighlightDuration;
        private bool _IsFocused = false;
        
        #endregion
		
        #region Properties

        /// <summary>
        /// The starting radix in the circle for this piece.
        /// </summary>
        internal double StartRadix
        {
        	get { return _StartRadix; }
        	set { _StartRadix = value; }
        }
        
        /// <summary>
        /// The ending radix in the circle for this piece.
        /// </summary>
        internal double EndRadix
        {
        	get { return _EndRadix; }
        	set { _EndRadix = value; }
        }
        
        /// <summary>
        /// Color of the piece.
        /// </summary>
		internal Color Color
		{
			get { return _Color; }
			set { _Color = value; }
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
		/// Time needed to switch between piece color and highlight color.
		/// </summary>
		internal TimeSpan HighlightDuration 
		{
			get { return _HighlightDuration; }
			set { _HighlightDuration = value; }
		}
		
		/// <summary>
		/// Whether or not the piece is highlighted
		/// </summary>
		internal bool IsHighlighted
		{
			get { return _IsHighlighted; }
		}
		
		/// <summary>
		/// Whether or not this piece can be highlighted.
		/// </summary>
		internal bool CanHighlight
		{
			get { return _CanHighlight; }
			set { _CanHighlight = value; }
		}

		/// <summary>
		/// Whether or not the piece is focused
		/// </summary>
        internal bool IsFocused
        {
            get { return _IsFocused; }
        }

        /// <summary>
        /// Lines that outline this piece
        /// </summary>
        internal ScreenSpaceLines3D Outline
        {
        	get { return _Outline; }
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
        /// <param name="key"></param>
        /// <param name="value"></param>
        public PiePiece(object key, object value) : base(key, value) {}

        /// <summary>
        /// Creates the geometry for this piece.
        /// </summary>
		protected internal override void Generate() 
		{
			if (_Geometry == null)
			{
				MeshGeometry3D mg3d = new MeshGeometry3D();
				mg3d.Positions.Add(new Point3D(0, 0, 0));

				// Create the top.
				double rad = _StartRadix;
				int steps = 0;
				while (true)
				{
					double x = PieGraph.Radius * Math.Sin(rad);
					double y = PieGraph.Radius * Math.Cos(rad);
					mg3d.Positions.Add(new Point3D(x, y, 0));
					if (rad == _EndRadix)
						break;
					rad += RadStep;
					if (rad > _EndRadix)
						rad = _EndRadix;
					steps++;
				}

                for (int i = 1; i <= steps; i++)
                {
                    mg3d.TriangleIndices.Add(0);
                    mg3d.TriangleIndices.Add(i);
                    mg3d.TriangleIndices.Add(i + 1);
                }

                // Create the bottom.
                mg3d.Positions.Add(new Point3D(0, 0, 1));
                int placeMarker = steps + 2;
                mg3d.TriangleIndices.Add(0);
                mg3d.TriangleIndices.Add(placeMarker);
                mg3d.TriangleIndices.Add(placeMarker);

                rad = _StartRadix;
                steps = 0;
                while (true)
                {
                    double x = PieGraph.Radius * Math.Sin(rad);
                    double y = PieGraph.Radius * Math.Cos(rad);
                    mg3d.Positions.Add(new Point3D(x, y, 1));
                    if (rad == _EndRadix)
                        break;
                    rad += RadStep;
                    if (rad > _EndRadix)
                        rad = _EndRadix;
                    steps++;
                }

                for (int i = placeMarker + 1; i <= steps + placeMarker; i++)
                {
                    mg3d.TriangleIndices.Add(placeMarker);
                    mg3d.TriangleIndices.Add(i + 1);
                    mg3d.TriangleIndices.Add(i);
                }

                // Create the outer edge.
                for (int i = 1, j = placeMarker + 1; i < steps; i++, j++)
                {
                    mg3d.TriangleIndices.Add(i);
                    mg3d.TriangleIndices.Add(j);
                    mg3d.TriangleIndices.Add(j + 1);
                    mg3d.TriangleIndices.Add(j + 1);
                    mg3d.TriangleIndices.Add(i + 1);
                    mg3d.TriangleIndices.Add(i);
                }

                // Create the ending edge.
                mg3d.TriangleIndices.Add(steps);
                mg3d.TriangleIndices.Add(steps + placeMarker);
                mg3d.TriangleIndices.Add(placeMarker);
                mg3d.TriangleIndices.Add(placeMarker);
                mg3d.TriangleIndices.Add(0);
                mg3d.TriangleIndices.Add(steps);

                // Create the leading edge.
                mg3d.TriangleIndices.Add(0);
                mg3d.TriangleIndices.Add(placeMarker);
                mg3d.TriangleIndices.Add(placeMarker + 1);
                mg3d.TriangleIndices.Add(placeMarker + 1);
                mg3d.TriangleIndices.Add(1);
                mg3d.TriangleIndices.Add(0);
                
                ScreenSpaceLines3D lines = new ScreenSpaceLines3D();
                lines.Color = Colors.Black;
                for (int i = 0; i <= steps; i++)
                	lines.Points.Add(mg3d.Positions[i]);
                lines.Points.Add(mg3d.Positions[0]);
                lines.Points.Add(mg3d.Positions[1]);
                lines.Points.Add(mg3d.Positions[placeMarker + 1]);
                lines.Points.Add(mg3d.Positions[placeMarker]);
                lines.Points.Add(mg3d.Positions[0]);
                lines.Points.Add(mg3d.Positions[steps]);
                lines.Points.Add(mg3d.Positions[placeMarker + steps]);
                for (int i = placeMarker; i <= placeMarker + steps; i++)
                	lines.Points.Add(mg3d.Positions[i]);
                lines.Points.Add(mg3d.Positions[placeMarker]);
                lines.Thickness = 1;
                _Outline = lines;

                _Geometry = new GeometryModel3D();
				_Geometry.Geometry = mg3d;
				MaterialGroup matg = new MaterialGroup();
				matg.Children.Add(new DiffuseMaterial(new SolidColorBrush(_Color)));
				matg.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 85));
				_Geometry.Material = matg;
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
				ColorAnimation ca = new ColorAnimation(_Color, _HighlightDuration);
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

        internal void Focus()
        {
            if (!_IsFocused)
            {
                Transform3DGroup t3dg = null;
                if (!(_Geometry.Transform is Transform3DGroup))
                {
                    t3dg = new Transform3DGroup();
                    _Geometry.Transform = t3dg;
                    _Outline.Transform = t3dg;
                }
                else
                    t3dg = _Geometry.Transform as Transform3DGroup;

                TranslateTransform3D tt = new TranslateTransform3D(0, 0, 0);
                t3dg.Children.Add(tt);
                DoubleAnimation da = new DoubleAnimation(-2, _HighlightDuration);
                tt.BeginAnimation(TranslateTransform3D.OffsetZProperty, da);
                _IsFocused = true;
            }
        }

        internal void Unfocus()
        {
            if (_IsFocused)
            {
                Transform3DGroup t3dg = null;
                if (_Geometry.Transform == null)
                {
                    t3dg = new Transform3DGroup();
                    _Geometry.Transform = t3dg;
                }
                else
                    t3dg = _Geometry.Transform as Transform3DGroup;

                TranslateTransform3D tt = new TranslateTransform3D(0, 0, 0);
                t3dg.Children.Add(tt);
                DoubleAnimation da = new DoubleAnimation(2, _HighlightDuration);
                tt.BeginAnimation(TranslateTransform3D.OffsetZProperty, da);
                _IsFocused = false;
            }
        }
    }
}
