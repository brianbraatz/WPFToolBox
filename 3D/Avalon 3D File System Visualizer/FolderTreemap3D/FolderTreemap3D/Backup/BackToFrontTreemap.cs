using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Arthur
{
	internal class BackToFrontTreemapLayer
	{
		internal List<TreemapNode> Nodes = new List<TreemapNode>();
	}

	internal class BackToFrontTreemap
	{
		internal List<BackToFrontTreemapLayer> Layers = new List<BackToFrontTreemapLayer>();

		internal BackToFrontTreemap(TreemapNode tmn)
		{
			CopyTreemapNode(tmn, 0);
		}

		private void CopyTreemapNode(TreemapNode tmn, Int32 layer)
		{
			if (Layers.Count <= layer)
				Layers.Add(new BackToFrontTreemapLayer());

			Layers[layer].Nodes.Add(tmn);

			foreach (TreemapNode tnmChild in tmn.Nodes)
			{
				CopyTreemapNode(tnmChild, layer + 1);
			}
		}
	}
}
