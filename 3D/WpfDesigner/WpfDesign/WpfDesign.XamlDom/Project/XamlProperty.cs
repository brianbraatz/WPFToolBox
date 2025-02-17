﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2425 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Describes a property on a <see cref="XamlObject"/>.
	/// </summary>
	public sealed class XamlProperty
	{
		XamlObject parentObject;
		internal readonly XamlPropertyInfo propertyInfo;
		XamlPropertyValue propertyValue;
		
		CollectionElementsCollection collectionElements;
		bool isCollection;
		
		static readonly IList<XamlPropertyValue> emptyCollectionElementsArray = new XamlPropertyValue[0];
		
		// for use by parser only
		internal XamlProperty(XamlObject parentObject, XamlPropertyInfo propertyInfo, XamlPropertyValue propertyValue)
			: this(parentObject, propertyInfo)
		{
			this.propertyValue = propertyValue;
			if (propertyValue != null) {
				propertyValue.ParentProperty = this;
			}
		}
		
		internal XamlProperty(XamlObject parentObject, XamlPropertyInfo propertyInfo)
		{
			this.parentObject = parentObject;
			this.propertyInfo = propertyInfo;
			
			if (propertyInfo.IsCollection) {
				isCollection = true;
				collectionElements = new CollectionElementsCollection(this);
			}
		}
		
		/// <summary>
		/// Gets the parent object for which this property was declared.
		/// </summary>
		public XamlObject ParentObject {
			get { return parentObject; }
		}
		
		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string PropertyName {
			get { return propertyInfo.Name; }
		}
		
		/// <summary>
		/// Gets the type the property is declared on.
		/// </summary>
		public Type PropertyTargetType {
			get { return propertyInfo.TargetType; }
		}
		
		/// <summary>
		/// Gets if this property is an attached property.
		/// </summary>
		public bool IsAttached {
			get { return propertyInfo.IsAttached; }
		}
		
		/// <summary>
		/// Gets the return type of the property.
		/// </summary>
		public Type ReturnType {
			get { return propertyInfo.ReturnType; }
		}
		
		/// <summary>
		/// Gets the type converter used to convert property values to/from string.
		/// </summary>
		public TypeConverter TypeConverter {
			get { return propertyInfo.TypeConverter; }
		}
		
		/// <summary>
		/// Gets the category of the property.
		/// </summary>
		public string Category {
			get { return propertyInfo.Category; }
		}
		
		/// <summary>
		/// Gets the value of the property. Can be null if the property is a collection property.
		/// </summary>
		public XamlPropertyValue PropertyValue {
			get { return propertyValue; }
			set {
				if (IsCollection) {
					throw new InvalidOperationException("Cannot set the value of collection properties.");
				}
				
				bool wasSet = this.IsSet;
				
				ResetInternal();
				propertyValue = value;
				propertyValue.AddNodeTo(this);
				propertyValue.ParentProperty = this;
				
				if (!wasSet) {
					if (IsSetChanged != null) {
						IsSetChanged(this, EventArgs.Empty);
					}
				}
				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}
		
		XmlElement _propertyElement;
		
		internal void ParserSetPropertyElement(XmlElement propertyElement)
		{
			XmlElement oldPropertyElement = _propertyElement;
			if (oldPropertyElement == propertyElement) return;
			
			_propertyElement = propertyElement;
			
			if (oldPropertyElement != null && IsCollection) {
				Debug.WriteLine("Property element for " + this.PropertyName + " already exists, merging..");
				foreach (XamlPropertyValue val in this.collectionElements) {
					val.RemoveNodeFromParent();
					val.AddNodeTo(this);
				}
				oldPropertyElement.ParentNode.RemoveChild(oldPropertyElement);
			}
		}
		
		internal void AddChildNodeToProperty(XmlNode newChildNode)
		{
			if (this.IsCollection) {
				// this is the default collection
				InsertNodeInCollection(newChildNode, collectionElements.Count);
				return;
			}
			if (_propertyElement == null) {
				_propertyElement = parentObject.OwnerDocument.XmlDocument.CreateElement(
					this.PropertyTargetType.Name + "." + this.PropertyName,
					parentObject.OwnerDocument.GetNamespaceFor(this.PropertyTargetType)
				);
				parentObject.XmlElement.InsertBefore(_propertyElement, parentObject.XmlElement.FirstChild);
			}
			_propertyElement.AppendChild(newChildNode);
		}
		
		internal void InsertNodeInCollection(XmlNode newChildNode, int index)
		{
			Debug.Assert(index >= 0 && index <= collectionElements.Count);
			XmlElement collection = _propertyElement;
			if (collection == null) {
				if (collectionElements.Count == 0) {
					// we have to create the collection element
					_propertyElement = parentObject.OwnerDocument.XmlDocument.CreateElement(
						this.PropertyTargetType.Name + "." + this.PropertyName,
						parentObject.OwnerDocument.GetNamespaceFor(this.PropertyTargetType)
					);
					parentObject.XmlElement.AppendChild(_propertyElement);
					collection = _propertyElement;
				} else {
					// this is the default collection
					collection = parentObject.XmlElement;
				}
			}
			if (collectionElements.Count == 0) {
				// collection is empty -> we may insert anywhere
				collection.AppendChild(newChildNode);
			} else if (index == collectionElements.Count) {
				// insert after last element in collection
				collection.InsertAfter(newChildNode, collectionElements[collectionElements.Count - 1].GetNodeForCollection());
			} else {
				// insert before specified index
				collection.InsertBefore(newChildNode, collectionElements[index].GetNodeForCollection());
			}
		}
		
		/// <summary>
		/// Gets if the property is a collection property.
		/// </summary>
		public bool IsCollection {
			get { return isCollection; }
		}
		
		/// <summary>
		/// Gets the collection elements of the property. Is empty if the property is not a collection.
		/// </summary>
		public IList<XamlPropertyValue> CollectionElements {
			get { return collectionElements ?? emptyCollectionElementsArray; }
		}
		
		/// <summary>
		/// Gets if the property is set.
		/// </summary>
		public bool IsSet {
			get { return propertyValue != null || collectionElements != null; }
		}
		
		/// <summary>
		/// Occurs when the value of the IsSet property has changed.
		/// </summary>
		public event EventHandler IsSetChanged;
		
		/// <summary>
		/// Occurs when the value of the property has changed.
		/// </summary>
		public event EventHandler ValueChanged;
		
		/// <summary>
		/// Resets the properties value.
		/// </summary>
		public void Reset()
		{
			if (IsSet) {
				propertyInfo.ResetValue(parentObject.Instance);
				
				ResetInternal();
				
				if (IsSetChanged != null) {
					IsSetChanged(this, EventArgs.Empty);
				}
				if (ValueChanged != null) {
					ValueChanged(this, EventArgs.Empty);
				}
			}
		}
		
		void ResetInternal()
		{
			if (propertyValue != null) {
				propertyValue.RemoveNodeFromParent();
				propertyValue.ParentProperty = null;
				propertyValue = null;
			}
			if (_propertyElement != null) {
				_propertyElement.ParentNode.RemoveChild(_propertyElement);
				_propertyElement = null;
			}
		}
		
		/// <summary>
		/// used internally by the XamlParser.
		/// Add a collection element that already is part of the XML DOM.
		/// </summary>
		internal void ParserAddCollectionElement(XmlElement collectionPropertyElement, XamlPropertyValue val)
		{
			if (collectionPropertyElement != null && _propertyElement == null) {
				ParserSetPropertyElement(collectionPropertyElement);
			}
			collectionElements.AddInternal(val);
			val.ParentProperty = this;
			if (collectionPropertyElement != _propertyElement) {
				val.RemoveNodeFromParent();
				val.AddNodeTo(this);
			}
		}
		
		/// <summary>
		/// Gets/Sets the value of the property on the instance without updating the XAML document.
		/// </summary>
		public object ValueOnInstance {
			get{
				return propertyInfo.GetValue(parentObject.Instance);
			}
			set {
				propertyInfo.SetValue(parentObject.Instance, value);
			}
		}
		
		/*public bool IsAttributeSyntax {
			get {
				return attribute != null;
			}
		}
		
		public bool IsElementSyntax {
			get {
				return element != null;
			}
		}
		
		public bool IsImplicitDefaultProperty {
			get {
				return attribute == null && element == null;
			}
		}*/
	}
}
