﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2420 $</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Stores information about the placement of an individual item during a <see cref="PlacementOperation"/>.
	/// </summary>
	public sealed class PlacementInformation
	{
		Rect originalBounds, bounds;
		readonly DesignItem item;
		readonly PlacementOperation operation;
		
		internal PlacementInformation(DesignItem item, PlacementOperation operation)
		{
			this.item = item;
			this.operation = operation;
		}
		
		/// <summary>
		/// The item being placed.
		/// </summary>
		public DesignItem Item {
			get { return item; }
		}
		
		/// <summary>
		/// The <see cref="PlacementOperation"/> to which this PlacementInformation belongs.
		/// </summary>
		public PlacementOperation Operation {
			get { return operation; }
		}
		
		/// <summary>
		/// Gets/sets the original bounds.
		/// </summary>
		public Rect OriginalBounds {
			get { return originalBounds; }
			set { originalBounds = value; }
		}
		
		/// <summary>
		/// Gets/sets the current bounds of the item.
		/// </summary>
		public Rect Bounds {
			get { return bounds; }
			set { bounds = value; }
		}
	}
}
