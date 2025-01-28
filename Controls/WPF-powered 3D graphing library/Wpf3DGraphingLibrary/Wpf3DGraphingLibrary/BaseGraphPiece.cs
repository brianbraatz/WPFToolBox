using System;
using System.Windows.Media.Media3D;

namespace Wpf3DGraphingLibrary
{
	/// <summary>
	/// An abstract class which defines what basically a piece of a graph needs.
	/// </summary>
	public abstract class BaseGraphPiece
	{
		private object _Key, _Value;
		private double _DVal = 0;
		
		/// <summary>
		/// The key associated with this piece.
		/// </summary>
		public object Key 
		{
			get { return _Key; }
		}
		
		/// <summary>
		/// The value associated with this piece.
		/// </summary>
		public object Value
		{
			get { return _Value; }
			protected set 
			{ 
				_Value = value;
				try
				{
					_DVal = Convert.ToDouble(_Value);
				}
				catch
				{
					_DVal = 0;
				}
			}
		}
		
		/// <summary>
		/// Gets the double value for this piece.
		/// </summary>
		protected internal double DVal
		{
			get { return _DVal; }
		}
		
		/// <summary>
		/// Gets the current geometry for this piece.
		/// </summary>
		public abstract GeometryModel3D Geometry
		{
			get;
		}
		
		/// <summary>
		/// Constructor taking key and value associated with this piece.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		protected BaseGraphPiece(object key, object value)
		{
			_Key = key;
			_Value = value;
			try
			{
				_DVal = Convert.ToDouble(_Value);
			}
			catch
			{
				_DVal = 0;
			}
		}
		
		/// <summary>
		/// (Re)generates the geometry for this piece.
		/// </summary>
		protected internal abstract void Generate();
	}
}
