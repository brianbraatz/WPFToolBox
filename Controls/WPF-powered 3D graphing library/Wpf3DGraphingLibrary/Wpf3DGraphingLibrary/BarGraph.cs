using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace Wpf3DGraphingLibrary
{
    /// <summary>
    /// Creates a bar graph off of the given information.  Individual objects
    /// represent each bar in the graph.  The width and height of the bars and the camera position
    /// are determined by the range of values present in the information given.
    /// </summary>
    public class BarGraph : BaseGraph
    {
        private const double SceneHeight = 8d;
        private const double SceneWidth = 8d;
        private static TimeSpan TransitionTime = new TimeSpan(0, 0, 0, 0, 300);

        private double _Min, _Max, _HeightRatio;
        private Color _BarColor, _HighlightColor;
        private TimeSpan _HighlightDuration;
        private bool _CanHighlight = false;
        private BarGraphUserActions _MouseOverAction = BarGraphUserActions.Nothing;
        private BarGraphUserActions _LeftClickAction = BarGraphUserActions.Nothing;
        private BarGraphUserActions _RightClickAction = BarGraphUserActions.Nothing;
        private Dictionary<object, ToolTip3D> _Tooltips = new Dictionary<object, ToolTip3D>();
        private PerspectiveCamera _Camera;

        #region Highlighting properties

        /// <summary>
        /// Returns the key for the currently highlighted bar, or null if no bars are highlighted.
        /// </summary>
        public object HighlightKey
        {
            get
            {
                foreach (Bar b in Pieces)
                {
                    if (b.IsHighlighted)
                        return b.Key;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the value for the currently highlighted bar, or null if no bars are highlighted.
        /// </summary>
        public object HighlightValue
        {
            get
            {
                foreach (Bar b in Pieces)
                {
                    if (b.IsHighlighted)
                        return b.Value;
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
        		foreach (Bar b in Pieces)
        			b.HighlightColor = value;
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
        		foreach (Bar b in Pieces)
        			b.HighlightDuration = value;
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
        		foreach (Bar b in Pieces)
        			b.CanHighlight = _CanHighlight;
        	}
        }
        
        /// <summary>
        /// The action to perform when a bar is moused-over.
        /// </summary>
        public BarGraphUserActions MouseOverAction 
        {
        	get { return _MouseOverAction; }
        	set { _MouseOverAction = value; }
        }
        
        /// <summary>
        /// The action to perform when a bar is left-clicked.
        /// </summary>
        public BarGraphUserActions LeftClickAction 
        {
        	get { return _LeftClickAction; }
        	set { _LeftClickAction = value; }
        }
        
        /// <summary>
        /// The action to perform when a bar is right-clicked.
        /// </summary>
        public BarGraphUserActions RightClickAction 
        {
        	get { return _RightClickAction; }
        	set { _RightClickAction = value; }
        }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a bar graph without highlighting.
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="barColor"></param>
        public BarGraph(IList lst, Color barColor)
        {
        	_BarColor = barColor;
        	_HighlightColor = Colors.White;
        	_HighlightDuration = TimeSpan.Zero;
        	Dictionary<int, object> dict = new Dictionary<int, object>();
        	for (int i = 0; i < lst.Count; i++)
        		dict[i] = lst[i];
        	InitGraph(dict);
        }
        
        /// <summary>
        /// Constructor for a bar graph with highlighting.
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="barColor"></param>
        /// <param name="highlightColor"></param>
        /// <param name="highlightDuration"></param>
        public BarGraph(IList lst, Color barColor, Color highlightColor, TimeSpan highlightDuration)
        {
        	_BarColor = barColor;
        	_HighlightColor = highlightColor;
        	_HighlightDuration = highlightDuration;
        	Dictionary<int, object> dict = new Dictionary<int, object>();
        	for (int i = 0; i < lst.Count; i++)
        		dict[i] = lst[i];
        	InitGraph(dict);
        	CanHighlight = true;
        }

        /// <summary>
        /// Constructor for a bar graph without highlighting.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="barColor"></param>
        public BarGraph(IDictionary dict, Color barColor)
        {
            _BarColor = barColor;
            _HighlightColor = Colors.White;
            _HighlightDuration = TimeSpan.Zero;
        	InitGraph(dict);
        }

        /// <summary>
        /// Constructor for a bar graph with highlighting.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="barColor"></param>
        /// <param name="highlightColor"></param>
        /// <param name="highlightDuration"></param>
        public BarGraph(IDictionary dict, Color barColor, Color highlightColor, TimeSpan highlightDuration)
        {
            _BarColor = barColor;
            _HighlightColor = highlightColor;
            _HighlightDuration = highlightDuration;
        	InitGraph(dict);
        	CanHighlight = true;
        }
		
        /// <summary>
        /// Constructor for a bar graph without highlighting.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="barColor"></param>
		public BarGraph(DataRow dr, Color barColor)
		{
            _BarColor = barColor;
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
        /// Constructor for a bar graph with highlighting.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="barColor"></param>
        /// <param name="highlightColor"></param>
        /// <param name="highlightDuration"></param>
		public BarGraph(DataRow dr, Color barColor, Color highlightColor, TimeSpan highlightDuration)
		{
            _BarColor = barColor;
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
            _Camera = new PerspectiveCamera();
            foreach (object key in dict.Keys)
        	{
                try
                {
                    double val = 0d;
                    val = Convert.ToDouble(dict[key]);
                    Bar bar = new Bar(key, dict[key]);
                    bar.BarColor = _BarColor;
                    bar.HighlightColor = _HighlightColor;
                    bar.HighlightDuration = _HighlightDuration;
                    Pieces.Add(bar);

                    ToolTip3D tt = new ToolTip3D(_Camera, "key: " + key.ToString() + "\nvalue: " + dict[key].ToString());
                    _Tooltips.Add(key, tt);
                    tt.Visible = false;
                    tt.WidthSizeFactor = 100d;
                    tt.HeightSizeFactor = 100d;
                    tt.BackColor = Colors.BlanchedAlmond;
                    tt.DrawBackground = true;
                }
                catch { }
        	}
        	CalculateValues();
        }
        
        private void FindMinMax()
        {
        	double min = 0d;
        	double max = 0d;
        	foreach (BaseGraphPiece piece in Pieces)
        	{
        		if (piece.DVal < min)
        			min = piece.DVal;
        		if (piece.DVal > max)
        			max = piece.DVal;
        	}
        	_Min = min;
        	_Max = max;
        }
        
        private void CalculateValues()
        {
        	FindMinMax();
        	_HeightRatio = SceneHeight / (_Max - _Min);
            double startX = SceneWidth / 2d;
            double stepX = SceneWidth / (double)Pieces.Count;
            double currX = startX - (stepX / 2d);
            int i = 0;
        	foreach (Bar bar in Pieces)
        	{
        		bar.X = currX;
        		bar.Z = 0d;
        		bar.Width = stepX;
        		bar.Height = bar.DVal * _HeightRatio;
        		bar.Depth = 1d;
                ToolTip3D tt = _Tooltips[bar.Key];
                tt.Position = new Point3D(currX, bar.Height / 2d, 0);
                currX -= stepX;
                i++;
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
			double camY = ((_Max - _Min) / 2d + _Min) * _HeightRatio;
			
			if (Viewport != null)
			{
                _Camera = Viewport.Camera as PerspectiveCamera;
				Point3DAnimation anim = new Point3DAnimation(new Point3D(0, camY, -5), TransitionTime);
                _Camera.BeginAnimation(PerspectiveCamera.PositionProperty, anim);
			}
			else
			{
                _Camera.FarPlaneDistance = 30;
                _Camera.NearPlaneDistance = 1;
                _Camera.FieldOfView = 90;
                _Camera.Position = new Point3D(0, camY, -5);
                _Camera.LookDirection = new Vector3D(0, 0, 5);
                _Camera.UpDirection = new Vector3D(0, 1, 0);
			}
			
			
			if (Viewport == null)
			{
				ModelVisual3D mv = new ModelVisual3D();
				Model3DGroup mg = new Model3DGroup();
                mv.Content = mg;
                Viewport = new Viewport3D();
                Viewport.Children.Add(mv);
                foreach (Bar bar in Pieces)
				{
					bar.Generate();
					mg.Children.Add(bar.Geometry);
                    //ToolTip3D tt = _Tooltips[bar.Key];
                    //mv.Children.Add(tt);
				}
				mg.Children.Add(new AmbientLight(Color.FromRgb(128, 128, 128)));
				mg.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));

				Viewport.Camera = _Camera;
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
			HandleAction(_RightClickAction, piece as Bar);
		}
        
		/// <summary>
		/// Handles the left click.
		/// </summary>
		/// <param name="piece"></param>
		protected override void OnLeftClick(BaseGraphPiece piece)
		{
			base.OnLeftClick(piece);
			HandleAction(_LeftClickAction, piece as Bar);
		}
        
        /// <summary>
        /// Handles the mouse over.
        /// </summary>
        /// <param name="piece"></param>
		protected override void OnMouseOver(BaseGraphPiece piece)
		{
			base.OnMouseOver(piece);
			HandleAction(_MouseOverAction, piece as Bar);
		}
		
		private void HandleAction(BarGraphUserActions action, Bar bar)
		{
			switch (action)
			{
				case BarGraphUserActions.Highlight:
					HighlightBar(bar);
					break;
                case BarGraphUserActions.Tooltip:
                    ShowToolTip(bar.Key);
                    break;
			}
		}

        private void HighlightBar(Bar bar)
        {
        	foreach (Bar b in Pieces)
        	{
                if (b.Equals(bar))
                {
                    if (!b.IsHighlighted)
                    {
                        b.Highlight();
                    }
                }
                else
                    b.Unhighlight();
        	}
        }

        private void ShowToolTip(object key)
        {
            foreach (object ttKey in _Tooltips.Keys)
            {
                ToolTip3D tt = _Tooltips[ttKey];
                if (ttKey.Equals(key))
                {
                    tt.Visible = true;
                    if (!Viewport.Children.Contains(tt))
                        Viewport.Children.Add(tt);
                }
                else 
                {
                    tt.Visible = false;
                    if (Viewport.Children.Contains(tt))
                        Viewport.Children.Remove(tt);
                }
            }
        }
		
		#endregion
    }
    
    /// <summary>
    /// Enumerates the various actions that can be performed in response to a user interaction 
    /// with the bar graph.
    /// </summary>
    public enum BarGraphUserActions
    {
    	/// <summary>
    	/// Does nothing.
    	/// </summary>
    	Nothing,
    	
    	/// <summary>
    	/// Highlights the bar that the user interacted with.
    	/// </summary>
    	Highlight,

        /// <summary>
        /// Tooltip over the bar that the user interacted with.
        /// </summary>
        Tooltip,
    }
}
