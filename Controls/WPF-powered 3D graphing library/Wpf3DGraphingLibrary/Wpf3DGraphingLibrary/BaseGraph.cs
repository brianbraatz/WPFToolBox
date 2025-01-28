using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Wpf3DGraphingLibrary
{
	/// <summary>
	/// An abstract base class that a typical graph would implement.
	/// </summary>
	public abstract class BaseGraph
	{
		#region Fields

		private Viewport3D _Viewport = null;
		
		/// <summary>
		/// This field is used by the hit-testing algorithm to determine which model is closest to the camera.
		/// </summary>
        private double _RayDistance;
        
        /// <summary>
        /// This field is used by the hit-testing algorithm to store which model was hit by the user action.
        /// </summary>
        private BaseGraphPiece _PieceHit;
        
        /// <summary>
        /// This field is used by the hit-testing algorithm to store which model was last left-clicked.
        /// </summary>
        private BaseGraphPiece _PieceLeftClicked;
        
        /// <summary>
        /// This field is used by the hit-testing algorithm to store which model was last right-clicked.
        /// </summary>
        private BaseGraphPiece _PieceRightClicked;
        
        private List<BaseGraphPiece> _Pieces = new List<BaseGraphPiece>();
        
        #endregion
        
        #region Properties
		
        /// <summary>
        /// Get a <see cref="Viewport3D"/> with the graph inside.
        /// </summary>
        public Viewport3D Viewport
        {
        	get { return _Viewport; }
        	protected set { _Viewport = value; }
        }
        
        /// <summary>
        /// A list of all the pieces in the graph.  Implementors must add the pieces to this list to make sure
        /// user interaction works correctly.
        /// </summary>
        protected List<BaseGraphPiece> Pieces
        {
        	get { return _Pieces; }
        }
        
		#endregion
        
		#region Events
		
        /// <summary>
        /// Indicates that the user has moused over a piece of the graph.
        /// </summary>
        public event GraphActionDelegate MouseOver;
        
        /// <summary>
        /// Indicates that the user has left-clicked on a piece of the graph.
        /// </summary>
        public event GraphActionDelegate LeftClicked;
        
        /// <summary>
        /// Indicates that the user has right-clicked on a piece of the graph.
        /// </summary>
        public event GraphActionDelegate RightClicked;
        
        #endregion
        
        /// <summary>
        /// (Re)generates the viewport.  Be sure to call <see cref="AttachEventsToViewport"/> if a new 
        /// viewport is generated.
        /// </summary>
        protected abstract void Generate();
        
        #region User Interaction Handling
        
        /// <summary>
        /// Attaches events to the viewport.  When a new viewport is generated, call this method to get the
        /// user interaction working.
        /// </summary>
        protected void AttachEventsToViewport()
        {
        	_Viewport.MouseMove += new MouseEventHandler(Viewport_MouseMove);
        	_Viewport.MouseLeftButtonDown += new MouseButtonEventHandler(Viewport_MouseLeftButtonDown);
        	_Viewport.MouseLeftButtonUp += new MouseButtonEventHandler(Viewport_MouseLeftButtonUp);
        	_Viewport.MouseRightButtonDown += new MouseButtonEventHandler(Viewport_MouseRightButtonDown);
        	_Viewport.MouseRightButtonUp += new MouseButtonEventHandler(Viewport_MouseRightButtonUp);
        }
        
        private void Viewport_MouseMove(object obj, MouseEventArgs e)
        {
        	Point mousePos = e.GetPosition(_Viewport);
        	PointHitTestParameters pointparams = new PointHitTestParameters(mousePos);

        	_RayDistance = double.MaxValue;
        	_PieceHit = null;
        	VisualTreeHelper.HitTest(_Viewport, null, HTResult, pointparams);
        	if (_PieceHit != null)
	        	OnMouseOver(_PieceHit);
        }
        
        private void Viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(_Viewport);
            PointHitTestParameters pointparams = new PointHitTestParameters(mousePos);

            _PieceLeftClicked = null;
            VisualTreeHelper.HitTest(_Viewport, null, HTResult, pointparams);
            _PieceLeftClicked = _PieceHit;
        }

        private void Viewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_PieceLeftClicked != null)
            {
                Point mousePos = e.GetPosition(_Viewport);
                PointHitTestParameters pointparams = new PointHitTestParameters(mousePos);

                VisualTreeHelper.HitTest(_Viewport, null, HTResult, pointparams);
                if (_PieceLeftClicked.Equals(_PieceHit))
                	OnLeftClick(_PieceLeftClicked);
                _PieceLeftClicked = null;
            }
        }
        
        private void Viewport_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(_Viewport);
            PointHitTestParameters pointparams = new PointHitTestParameters(mousePos);

            _PieceRightClicked = null;
            VisualTreeHelper.HitTest(_Viewport, null, HTResult, pointparams);
            _PieceRightClicked = _PieceHit;
        }

        private void Viewport_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_PieceRightClicked != null)
            {
                Point mousePos = e.GetPosition(_Viewport);
                PointHitTestParameters pointparams = new PointHitTestParameters(mousePos);

                VisualTreeHelper.HitTest(_Viewport, null, HTResult, pointparams);
                if (_PieceRightClicked.Equals(_PieceHit))
                	OnRightClick(_PieceRightClicked);
                _PieceRightClicked = null;
            }
        }

        /// <summary>
        /// Determines which model was hit and stores the result in the _PieceHit field.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult raw)
        {
        	RayMeshGeometry3DHitTestResult rayResult = raw as RayMeshGeometry3DHitTestResult;
        	
        	if (rayResult != null && rayResult.DistanceToRayOrigin < _RayDistance)
        	{
        		if (rayResult.ModelHit is GeometryModel3D)
        		{
        			GeometryModel3D hitgeo = rayResult.ModelHit as GeometryModel3D;
        			foreach (BaseGraphPiece piece in _Pieces)
        			{
        				if (hitgeo.Equals(piece.Geometry))
        				{
                            _RayDistance = rayResult.DistanceToRayOrigin;
                            _PieceHit = piece;
        					break;
        				}
        			}
        		}
        	}

        	return HitTestResultBehavior.Continue;
        }
        
        /// <summary>
        /// When a user mouses over a piece, this method is called.  Typically it fires the
        /// <see cref="MouseOver"/> event.  Override this to change this default behavior.
        /// </summary>
        /// <param name="piece"></param>
        protected virtual void OnMouseOver(BaseGraphPiece piece)
        {
        	if (MouseOver != null)
        		MouseOver(this, piece.Key, piece.Value);
        }
        
        /// <summary>
        /// When a user left-clicks on a piece, this method is called.  Typically it fires the 
        /// <see cref="LeftClicked"/> event.  Override this to change this default behavior.
        /// </summary>
        /// <param name="piece"></param>
        protected virtual void OnLeftClick(BaseGraphPiece piece)
        {
        	if (LeftClicked != null)
        		LeftClicked(this, piece.Key, piece.Value);
        }
        
        /// <summary>
        /// When a user right-clicks on a piece, this method is called.  Typically it fires the 
        /// <see cref="RightClicked"/> event.  Override this to change this default behavior.
        /// </summary>
        /// <param name="piece"></param>
        protected virtual void OnRightClick(BaseGraphPiece piece)
        {
        	if (RightClicked != null)
        		RightClicked(this, piece.Key, piece.Value);
        }

        #endregion
	}
    
    /// <summary>
    /// Used for events that involve the user interacting with a part of the graph.
    /// </summary>
    public delegate void GraphActionDelegate(BaseGraph sender, object key, object value);
}
