using System;
using System.Collections.Generic;
using System.Text;

namespace Arthur
{
	internal class DatatreeNode : IComparable<DatatreeNode>
	{
		internal Int64 Magnitude
		{
			get { return _magnitude; }
			set { _magnitude = value; }
		} Int64 _magnitude;

		internal string Name
		{
			get { return _name; }
			set { _name = value; }
		} string _name;

		internal List<DatatreeNode> Nodes = new List<DatatreeNode>();

		protected DatatreeNode()
		{
		}

		protected DatatreeNode(Int64 magnitude)
		{
			_magnitude = magnitude;
		}

		internal void Sort()
		{
			Nodes.Sort();
			foreach (DatatreeNode dtn in Nodes)
			{
				dtn.Sort();
			}
		}

		internal DatatreeNode AddChild(Int64 magnitude)
		{
			DatatreeNode dtn = new DatatreeNode(magnitude);
			Nodes.Add(dtn);
			return dtn;
		}

		#region IComparable<DatatreeNode> Members

		int IComparable<DatatreeNode>.CompareTo(DatatreeNode other)
		{
			if (other == null)
			{
				return 1;
			}
			if (this._magnitude > other._magnitude)
			{
				return -1;
			}
			if (this._magnitude < other._magnitude)
			{
				return 1;
			}
			return 0;
		}

		#endregion
	}

	internal class Datatree : DatatreeNode
	{
		internal Datatree()
			: base()
		{
		}
	}
}
