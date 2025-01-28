using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Wpf3DGraphingLibrary
{
    /// <summary>
    /// Creates a pie graph off of the given information.  Individual <see cref="PiePiece"/> objects
    /// represent each piece in the pie graph.  Their size is determined by their value with respect
    /// to the sum of all the values.
    /// </summary>
    public class PieGraph : BaseGraph
	{
		internal const double Radius = 4d;
		internal const double PI_2 = Math.PI * 2;
		
		private static Color[] PieColors = new Color[] { Colors.Red, Colors.Orange, Colors.Yellow, 
			Colors.Green, Colors.Blue, Colors.Indigo, Colors.Violet };
		
        private double _Sum;
        private Color _HighlightColor;
        private TimeSpan _HighlightDuration;
        private bool _Outlining = true, _CanHighlight = false;
        private PieGraphUserActions _MouseOverAction = PieGraphUserActions.Nothing;
        private PieGraphUserActions _LeftClickAction = PieGraphUserActions.Nothing;
        private PieGraphUserActions _RightClickAction = PieGraphUserActions.Nothing;

        #region Highlighting properties

        /// <summary>
        /// Returns the key for the currently highlighted pie piece, or null if no pieces are highlighted.
        /// </summary>
        public object HighlightKey
        {
            get
            {
                foreach (PiePiece p in Pieces)
                {
                    if (p.IsHighlighted)
                        return p.Key;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the value for the currently highlighted pie piece, or null if no pieces are highlighted.
        /// </summary>
        public object HighlightValue
        {
            get
            {
                foreach (PiePiece p in Pieces)
                {
                    if (p.IsHighlighted)
                        return p.Value;
                }
                return null;
            }
        }

        #endregion

        #region Focusing properties

        /// <summary>
        /// Returns the key for the currently focused pie piece, or null if no pieces are focused.
        /// </summary>
        public object FocusKey
        {
        	get 
        	{
        		foreach (PiePiece p in Pieces)
        		{
        			if (p.IsFocused)
        				return p.Key;
        		}
        		return null;
        	}
        }
        
        /// <summary>
        /// Returns the value for the currently focused pie piece, or null if no pieces are focused.
        /// </summary>
        public object FocusValue
        {
        	get 
        	{
        		foreach (PiePiece p in Pieces)
        		{
        			if (p.IsFocused)
        				return p.Value;
        		}
        		return null;
        	}
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the highlight color for the bars.
        /// </summary>
        public Color HighlightColor
        {
        	get { return _HighlightColor; }
        	set 
        	{ 
        		foreach (PiePiece p in Pieces)
        			p.HighlightColor = value;
        		_HighlightColor = value;
        	}
        }
        
        /// <summary>
        /// Gets or sets the time taken to switch between the bar color and highlight color during a highlight.
        /// </summary>
        public TimeSpan HighlightDuration
        {
        	get { return _HighlightDuration; }
        	set
        	{
        		foreach (PiePiece p in Pieces)
        			p.HighlightDuration = value;
        		_HighlightDuration = value;
        	}
        }
        
        /// <summary>
        /// Indicates that the bar graph can be highlighted.
        /// </summary>
        public bool CanHighlight
        {
        	get { return _CanHighlight; }
        	set 
        	{ 
        		_CanHighlight = value; 
        		foreach (PiePiece p in Pieces)
        			p.CanHighlight = _CanHighlight;
        	}
        }

        /// <summary>
        /// The action to perform when a piece is moused-over.
        /// </summary>
        public PieGraphUserActions MouseOverAction 
        {
        	get { return _MouseOverAction; }
        	set { _MouseOverAction = value; }
        }
        
        /// <summary>
        /// The action to perform when a piece is left-clicked.
        /// </summary>
        public PieGraphUserActions LeftClickAction 
        {
        	get { return _LeftClickAction; }
        	set { _LeftClickAction = value; }
        }
        
        /// <summary>
        /// The action to perform when a piece is right-clicked.
        /// </summary>
        public PieGraphUserActions RightClickAction 
        {
        	get { return _RightClickAction; }
        	set { _RightClickAction = value; }
        }

        /// <summary>
        /// Turns outlining on or off.  Be sure to set this before calling <see cref="Initialize"/>.
        /// Default is on.
        /// </summary>
        public bool Outlining
        {
            get { return _Outlining; }
            set { _Outlining = value; }
        }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a pie graph without highlighting.
        /// </summary>
        /// <param name="lst"></param>
        public PieGraph(IList lst)
		{
        	_HighlightColor = Colors.White;
        	_HighlightDuration = TimeSpan.Zero;
        	Dictionary<int, object> dict = new Dictionary<int, object>();
        	for (int i = 0; i < lst.Count; i++)
        		dict[i] = lst[i];
        	InitGraph(dict);
		}

        /// <summary>
        /// Constructor for a pie graph with highlighting.
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="highlightColor"></param>
        /// <param name="highlightDuration"></param>
		public PieGraph(IList lst, Color highlightColor, TimeSpan highlightDuration)
		{
        	_HighlightColor = highlightColor;
        	_HighlightDuration = highlightDuration;
        	Dictionary<int, object> dict = new Dictionary<int, object>();
        	for (int i = 0; i < lst.Count; i++)
        		dict[i] = lst[i];
        	InitGraph(dict);
        	CanHighlight = true;
		}

		/// <summary>
		/// Constructor for a pie graph without highlighting.
		/// </summary>
		/// <param name="dict"></param>
        public PieGraph(IDictionary dict)
		{
        	_HighlightColor = Colors.White;
        	_HighlightDuration = TimeSpan.Zero;
        	InitGraph(dict);
		}
        
        /// <summary>
        /// Constructor for a pie graph with highlighting.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="highlightColor"></param>
        /// <param name="highlightDuration"></param>
		public PieGraph(IDictionary dict, Color highlightColor, TimeSpan highlightDuration)
		{
        	_HighlightColor = highlightColor;
        	_HighlightDuration = highlightDuration;
        	InitGraph(dict);
        	CanHighlight = true;
		}
		
		/// <summary>
		/// Constructor for a pie graph without highlighting.
		/// </summary>
		/// <param name="dr"></param>
		public PieGraph(DataRow dr)
		{
        	_HighlightColor = Colors.White;
        	_HighlightDuration = TimeSpan.Zero;
        	Dictionary<string, object> dict = new Dictionary<string, object>();
        	foreach (DataColumn dc in dr.Table.Columns)
        	{
        		string key = dc.ColumnName;
        		object val = dr[key];
        		dict[key] = val;
        	}
        	InitGraph(dict);
		}
		
		/// <summary>
		/// Constructor for a pie graph with highlighting.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="highlightColor"></param>
		/// <param name="highlightDuration"></param>
		public PieGraph(DataRow dr, Color highlightColor, TimeSpan highlightDuration)
		{
        	_HighlightColor = highlightColor;
        	_HighlightDuration = highlightDuration;
        	Dictionary<string, object> dict = new Dictionary<string, object>();
        	foreach (DataColumn dc in dr.Table.Columns)
        	{
        		string key = dc.ColumnName;
        		object val = dr[key];
        		dict[key] = val;
        	}
        	InitGraph(dict);
        	CanHighlight = true;
        }

        #endregion

        #region Methods
        
        private void InitGraph(IDictionary dict)
        {
        	int c = 0;
        	foreach (object key in dict.Keys)
        	{
                try
                {
                    double val = 0d;
                    val = Convert.ToDouble(dict[key]);
                    PiePiece piece = new PiePiece(key, dict[key]);
                    piece.Color = PieColors[c++];
                    piece.HighlightColor = _HighlightColor;
                    piece.HighlightDuration = _HighlightDuration;
                    Pieces.Add(piece);
        			if (c >= PieColors.Length)
        				c = 0;
                }
                catch { }
        	}
        	CalculateValues();
        }

        private void CalculateValues() 
		{
        	_Sum = 0d;
        	foreach (PiePiece piece in Pieces)
        		_Sum += piece.DVal;
        	
        	double rad = 0d;
        	foreach (PiePiece piece in Pieces)
        	{
       			double ratio = piece.DVal / _Sum;
       			ratio *= PI_2;
       			piece.StartRadix = rad;
       			piece.EndRadix = rad + ratio;
       			rad += ratio;
        	}
        }

        /// <summary>
        /// Initializes the graph and returns the viewport.
        /// </summary>
        /// <returns></returns>
        public Viewport3D Initialize()
        {
        	Generate();
        	return Viewport;
        }

        /// <summary>
        /// Creates the viewport for this bar graph.
        /// </summary>
		protected override void Generate()
        {
			CalculateValues();
        	
        	if (Viewport == null)
        	{
        		ModelVisual3D mv = new ModelVisual3D();
        		Model3DGroup mg = new Model3DGroup();
        		foreach (PiePiece piece in Pieces)
        		{
        			piece.Generate();
        			mg.Children.Add(piece.Geometry);
                    if (_Outlining)
        			    mv.Children.Add(piece.Outline);
        		}
        		mg.Children.Add(new AmbientLight(Color.FromRgb(128, 128, 128)));
				mg.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));
				mv.Content = mg;
				
        		PerspectiveCamera pc = new PerspectiveCamera();
            	pc.FarPlaneDistance = 30;
            	pc.NearPlaneDistance = 1;
            	pc.FieldOfView = 90;
        	    pc.Position = new Point3D(0, -4.5, -4.5);
    	        pc.LookDirection = new Vector3D(0, 4.5, 4.5);
	            pc.UpDirection = new Vector3D(0, 1, 0);

				Viewport = new Viewport3D();
				Viewport.Children.Add(mv);
				Viewport.Camera = pc;
				AttachEventsToViewport();
        	}
        }
		
        #endregion

        #region User interaction handling methods
        
        /// <summary>
        /// Handles the right click.
        /// </summary>
        /// <param name="piece"></param>
		protected override void OnRightClick(BaseGraphPiece piece)
		{
			base.OnRightClick(piece);
			HandleAction(_RightClickAction, piece as PiePiece);
		}
        
		/// <summary>
		/// Handles the left click.
		/// </summary>
		/// <param name="piece"></param>
		protected override void OnLeftClick(BaseGraphPiece piece)
		{
			base.OnLeftClick(piece);
			HandleAction(_LeftClickAction, piece as PiePiece);
		}
        
        /// <summary>
        /// Handles the mouse over.
        /// </summary>
        /// <param name="piece"></param>
		protected override void OnMouseOver(BaseGraphPiece piece)
		{
			base.OnMouseOver(piece);
			HandleAction(_MouseOverAction, piece as PiePiece);
		}
		
		private void HandleAction(PieGraphUserActions action, PiePiece piece)
		{
			switch (action)
			{
				case PieGraphUserActions.Highlight:
					HighlightPiece(piece);
					break;
				case PieGraphUserActions.Focus:
					FocusPiece(piece);
					break;
			}
		}
		
        private void HighlightPiece(PiePiece piece)
        {
        	foreach (PiePiece p in Pieces)
        	{
        		if (p.Equals(piece))
                {
                    if (!p.IsHighlighted)
                        p.Highlight();
                }
                else
        			p.Unhighlight();
        	}
        }

        private void FocusPiece(PiePiece piece)
        {
            foreach (PiePiece p in Pieces)
            {
                if (p.Equals(piece))
                {
                	if (!p.IsFocused)
                    	p.Focus();
                }
                else
                    p.Unfocus();
            }
        }
		
		#endregion
    }
    
    /// <summary>
    /// Enumerates the various actions that can be performed in response to a user interaction 
    /// with the pie graph.
    /// </summary>
    public enum PieGraphUserActions
    {
    	/// <summary>
    	/// Does nothing.
    	/// </summary>
    	Nothing,
    	
    	/// <summary>
    	/// Highlights the piece that the user interacted with.
    	/// </summary>
    	Highlight,
    	
    	/// <summary>
    	/// Focuses the piece that the user interacted with.  This basically means raising it out of the rest of the pie.
    	/// </summary>
    	Focus,
    }
}
