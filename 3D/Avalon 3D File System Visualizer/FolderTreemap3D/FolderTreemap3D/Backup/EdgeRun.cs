using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Arthur
{
	internal enum EdgeRunDirection
	{
		Height,
		Width
	}

	internal class EdgeRunNode
	{
		internal Rect _remainingRect = Rect.Empty;
		internal EdgeRunDirection _edgeDirection;
		internal double _edgeLength;
		internal DatatreeNode _dtn;
		internal double _k;

		internal Rect _rect = new Rect(0, 0, 1, double.MaxValue);

		internal EdgeRunNode(Rect remainingRect, EdgeRunDirection edgeDirection, DatatreeNode dtn, double k)
		{
			_remainingRect = remainingRect;
			_edgeDirection = edgeDirection;
			_edgeLength = (_edgeDirection == EdgeRunDirection.Height) ? _remainingRect.Height : _remainingRect.Width;
			_dtn = dtn;
			_k = k;
		}

		internal void Lock(double otherEdgeLength, double edgeStartingPoint)
		{
			double edgeLengthShare = (_dtn.Magnitude * _k) / otherEdgeLength;

			if (_edgeDirection == EdgeRunDirection.Height)
			{
				_rect = new Rect(_remainingRect.Left, edgeStartingPoint - edgeLengthShare, otherEdgeLength, edgeLengthShare);
			}
			else
			{
				_rect = new Rect(edgeStartingPoint, _remainingRect.Bottom - otherEdgeLength, edgeLengthShare, otherEdgeLength);
			}
		}

		internal bool AspectRatioBetter(double otherEdgeLength)
		{
			double arBefore = AspectRatio();

			double edgeLengthShare = (_dtn.Magnitude * _k) / otherEdgeLength;

			if (_edgeDirection == EdgeRunDirection.Height)
			{
				_rect = new Rect(0, 0, otherEdgeLength, edgeLengthShare);
			}
			else
			{
				_rect = new Rect(0, 0, edgeLengthShare, otherEdgeLength);
			}

			double arAfter = AspectRatio();
			return arAfter < arBefore;
		}

		internal double AspectRatio()
		{
			return Math.Max(_rect.Height / _rect.Width, _rect.Width / _rect.Height);
		}
	}

	internal class EdgeRun
	{
		private DatatreeNode _dtn;
		private Int32 _dtIX;
		private Rect _remainingRect;
		private double _k;

		double _edgeLength;

		private EdgeRunDirection _edgeDirection = EdgeRunDirection.Height;

		internal List<EdgeRunNode> Nodes = new List<EdgeRunNode>();

		internal EdgeRun(DatatreeNode dataTreeNode, Int32 dtIX, ref Rect remainingRect, double k)
		{
			_dtn = dataTreeNode;
			_dtIX = dtIX;
			_remainingRect = remainingRect;
			_k = k;

			if (remainingRect.Height > remainingRect.Width)
				_edgeDirection = EdgeRunDirection.Width;

			_edgeLength = (_edgeDirection == EdgeRunDirection.Height) ? remainingRect.Height : remainingRect.Width;

			double totalMagnitude = 0;
			double otherEdgeLength;
			while (dataTreeNode.Nodes.Count > dtIX)
			{
				totalMagnitude += dataTreeNode.Nodes[dtIX].Magnitude;

				otherEdgeLength = (totalMagnitude * _k) / _edgeLength;

				EdgeRunNode ern = new EdgeRunNode(remainingRect, _edgeDirection, dataTreeNode.Nodes[dtIX], k);

				bool aspectRatioBetter;

				if (Nodes.Count > 0)
				{
					aspectRatioBetter = Nodes[0].AspectRatioBetter(otherEdgeLength);
				}
				else
				{
					aspectRatioBetter = ern.AspectRatioBetter(otherEdgeLength);
				}

				if (aspectRatioBetter)
				{
					Nodes.Add(ern);
				}
				else
				{
					otherEdgeLength = ((totalMagnitude - dataTreeNode.Nodes[dtIX].Magnitude) * _k) / _edgeLength;
					LockNodes(ref remainingRect, otherEdgeLength);
					return;
				}

				++dtIX;
			}
			otherEdgeLength = (totalMagnitude * _k) / _edgeLength;
			LockNodes(ref remainingRect, otherEdgeLength);
		}

		internal void LockNodes(ref Rect remainingRect, double otherEdgeLength)
		{
			double edgeStartingPoint;
			if (_edgeDirection == EdgeRunDirection.Height)
			{
				edgeStartingPoint = remainingRect.Bottom;
				if (remainingRect.Width >= otherEdgeLength)
					remainingRect.Width -= otherEdgeLength;
				else
					remainingRect.Width = 0;
				remainingRect.X += otherEdgeLength;
			}
			else
			{
				edgeStartingPoint = remainingRect.Left;
				if (remainingRect.Height - 0.001 >= otherEdgeLength)
					remainingRect.Height -= otherEdgeLength;
				else
					remainingRect.Height = 0;
			}

			foreach (EdgeRunNode ern in Nodes)
			{
				ern.Lock(otherEdgeLength, edgeStartingPoint);
				if (_edgeDirection == EdgeRunDirection.Height)
				{
					edgeStartingPoint -= ern._rect.Height;
				}
				else
				{
					edgeStartingPoint += ern._rect.Width;
				}
			}
		}
	}

	internal class RemainingRectangle
	{
		internal static void Process(TreemapNode treemapNode, DatatreeNode dataTreeNode, Int32 dtIX)
		{
			Rect rect = treemapNode.Rect;

			double k = (rect.Width * rect.Height) / dataTreeNode.Magnitude;

			while (dataTreeNode.Nodes.Count > dtIX)
			{
				EdgeRun edgeRun = new EdgeRun(dataTreeNode, dtIX, ref rect, k);

				Int32 nodesConsumed = edgeRun.Nodes.Count;

				for (Int32 erIX = 0; erIX < edgeRun.Nodes.Count; ++erIX)
				{
					TreemapNode tmn = new TreemapNode(dataTreeNode.Nodes[dtIX].Magnitude, dataTreeNode.Nodes[dtIX].Name, edgeRun.Nodes[erIX]._rect);
					Process(tmn, dataTreeNode.Nodes[dtIX], 0);
					treemapNode.AddChild(tmn);
					++dtIX;
				}
			}
		}
	}
}
