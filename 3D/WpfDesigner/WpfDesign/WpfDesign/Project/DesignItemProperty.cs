﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2421 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Represents a property of a DesignItem.
	/// All changes done via the DesignItemProperty class are represented in the underlying model (e.g. XAML).
	/// All such changes are recorded in the currently running designer transaction (<see cref="ChangeGroup"/>),
	/// enabling Undo/Redo support.
	/// Warning: Changes directly done to control instances might not be reflected in the model.
	/// </summary>
	public abstract class DesignItemProperty
	{
		/// <summary>
		/// Gets the property name.
		/// </summary>
		public abstract string Name { get; }
		
		/// <summary>
		/// Gets the return type of the property.
		/// </summary>
		public abstract Type ReturnType { get; }
		
		/// <summary>
		/// Gets the type that declares the property.
		/// </summary>
		public abstract Type DeclaringType { get; }
		
		/// <summary>
		/// Gets the category of the property.
		/// </summary>
		public abstract string Category { get; }
		
		/// <summary>
		/// Gets the type converter used to convert property values to/from string.
		/// </summary>
		public virtual TypeConverter TypeConverter {
			get { return TypeDescriptor.GetConverter(this.ReturnType); }
		}
		
		/// <summary>
		/// Gets if the property represents a collection.
		/// </summary>
		public abstract bool IsCollection { get; }
		
		/// <summary>
		/// Gets the elements represented by the collection.
		/// </summary>
		public abstract IList<DesignItem> CollectionElements { get; }
		
		/// <summary>
		/// Gets the value of the property. This property returns null if the value is not set,
		/// or if the value is set to a primitive value.
		/// </summary>
		public abstract DesignItem Value { get; }
		
		/// <summary>
		/// Is raised when the value of the property changes (by calling <see cref="SetValue"/> or <see cref="Reset"/>).
		/// </summary>
		public abstract event EventHandler ValueChanged;
		
		/// <summary>
		/// Gets/Sets the value of the property on the designed instance.
		/// If the property is not set, this returns the default value.
		/// The setter does NOT update the underlying model, use SetValue() instead!
		/// </summary>
		public abstract object ValueOnInstance { get; set; }
		
		/// <summary>
		/// Sets the value of the property.
		/// </summary>
		public abstract void SetValue(object value);
		
		/// <summary>
		/// Gets if the property is set on the design item.
		/// </summary>
		public abstract bool IsSet { get; }
		
		/// <summary>
		/// Occurs when the value of the IsSet property changes.
		/// </summary>
		public abstract event EventHandler IsSetChanged;
		
		/// <summary>
		/// Resets the property value to the default, possibly removing it from the list of properties.
		/// </summary>
		public abstract void Reset();
	}
	
	/// <summary>
	/// Represents a collection of design item properties.
	/// </summary>
	public abstract class DesignItemPropertyCollection : IEnumerable<DesignItemProperty>
	{
		/// <summary>
		/// Gets the property with the specified name.
		/// </summary>
		public DesignItemProperty this[string name] {
			get { return GetProperty(name); }
		}
		
		/// <summary>
		/// Gets the design item property representing the specified dependency property.
		/// The property must not be an attached property.
		/// </summary>
		public DesignItemProperty this[DependencyProperty dependencyProperty] {
			get {
				Debug.Assert(DependencyPropertyDescriptor.FromProperty(dependencyProperty, dependencyProperty.OwnerType).IsAttached == false);
				return GetProperty(dependencyProperty);
			}
		}
		
		/// <summary>
		/// Gets the design item property with the specified name.
		/// </summary>
		public abstract DesignItemProperty GetProperty(string name);
		
		/// <summary>
		/// Gets the attached property on the specified owner with the specified name.
		/// </summary>
		public abstract DesignItemProperty GetAttachedProperty(Type ownerType, string name);
		
		/// <summary>
		/// Gets the design item property representing the specified dependency property.
		/// The property must not be an attached property.
		/// </summary>
		public DesignItemProperty GetProperty(DependencyProperty dependencyProperty)
		{
			if (dependencyProperty == null)
				throw new ArgumentNullException("dependencyProperty");
			return GetProperty(dependencyProperty.Name);
		}
		
		/// <summary>
		/// Gets the design item property representing the specified attached dependency property.
		/// </summary>
		public DesignItemProperty GetAttachedProperty(DependencyProperty dependencyProperty)
		{
			if (dependencyProperty == null)
				throw new ArgumentNullException("dependencyProperty");
			return GetAttachedProperty(dependencyProperty.OwnerType, dependencyProperty.Name);
		}
		
		/// <summary>
		/// Gets an enumerator to enumerate the properties that have a non-default value.
		/// </summary>
		public abstract IEnumerator<DesignItemProperty> GetEnumerator();
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
