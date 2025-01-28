﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2254 $</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Attribute to specify that the decorated class is a editor for the specified property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
	public sealed class PropertyEditorAttribute : Attribute
	{
		readonly Type propertyDeclaringType;
		readonly string propertyName;
		
		/// <summary>
		/// Creates a new PropertyEditorAttribute that specifies that the decorated class is a editor
		/// for the "<paramref name="propertyDeclaringType"/>.<paramref name="propertyName"/>".
		/// </summary>
		public PropertyEditorAttribute(Type propertyDeclaringType, string propertyName)
		{
			if (propertyDeclaringType == null)
				throw new ArgumentNullException("propertyDeclaringType");
			if (propertyName == null)
				throw new ArgumentNullException("propertyName");
			this.propertyDeclaringType = propertyDeclaringType;
			this.propertyName = propertyName;
		}
		
		/// <summary>
		/// Gets the type that declares the property that the decorated editor supports.
		/// </summary>
		public Type PropertyDeclaringType {
			get { return propertyDeclaringType; }
		}
		
		/// <summary>
		/// Gets the name of the property that the decorated editor supports.
		/// </summary>
		public string PropertyName {
			get { return propertyName; }
		}
	}
}

