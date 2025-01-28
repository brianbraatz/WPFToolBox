using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Arthur
{
	internal class TreemapNode
	{
		internal Int64 Magnitude
		{
			get { return _magnitude; }
		} Int64 _magnitude;

		internal Rect Rect;

		internal string Name
		{
			get { return _name; }
			set { _name = value; }
		} string _name;

		internal List<TreemapNode> Nodes = new List<TreemapNode>();

		internal TreemapNode(Int64 magnitude, string name, Rect rect)
		{
			_magnitude = magnitude;
			_name = name;
			Rect = rect;
		}

		internal void AddChild(TreemapNode tmn)
		{
			Nodes.Add(tmn);
		}
	}

	internal class Treemap : TreemapNode
	{
		internal Treemap(Int64 magnitude, string name, Rect rect)
			: base(magnitude, name, rect)
		{
		}
	}
}
