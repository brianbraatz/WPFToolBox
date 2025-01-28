//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: SharedSize class and SharedSizeType enumeration.
//  Creator: t-tomkm
//  Date Created: 08/17/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Synchronizes shared width or shared height among several SharedSizeDecorators.
    /// </summary>
    public class SharedSize
    {
        #region Fields
        private SharedSizeScope _sharedSizeScope;
        private SharedSizeType _sizeType;
        private string _sharedGroupName;
        private PriorityQueue<SharedSizeDecorator, double> _registeredDecorators;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sharedSizeScope">SharedSizeScope that contains this object.</param>
        /// <param name="sizeType">Type (width or height) of the shared size.</param>
        /// <param name="sharedGroupName">Name of the shared width or height group within the scope.</param>
        public SharedSize(SharedSizeScope sharedSizeScope, SharedSizeType sizeType, string sharedGroupName)
        {
            _sharedSizeScope = sharedSizeScope;
            _sharedGroupName = sharedGroupName;
            _sizeType = sizeType;
            _registeredDecorators = new PriorityQueue<SharedSizeDecorator, double>();
        }

        #region Properties
        /// <summary>
        /// Name of the shared width of height group withing the scope.
        /// </summary>
        public string GroupName
        {
            get { return _sharedGroupName; }
        }

        /// <summary>
        /// Gets size of the biggest control.
        /// </summary>
        public double MaximumSize
        {
            get { return _registeredDecorators.Count > 0 ? _registeredDecorators.MaximumValue : 0.0; }
        }

        /// <summary>
        /// Type of the shared size.
        /// </summary>
        public SharedSizeType SizeType
        {
            get { return _sizeType; }
        }
        #endregion

        #region Registering, unregistering and updating decorators.

        /// <summary>
        /// Registers new decorator.
        /// </summary>
        /// <param name="decorator">Decorator to register.</param>
        public void RegisterDecorator(SharedSizeDecorator decorator)
        {
            _registeredDecorators.Add(decorator, 0.0);
        }

        /// <summary>
        /// Unregisters decorator.
        /// </summary>
        /// <param name="decorator">Decorator to unregister.</param>
        public void UnregisterDecorator(SharedSizeDecorator decorator)
        {
            double previousMaximumSize = MaximumSize;
            _registeredDecorators.Remove(decorator);
            if (_registeredDecorators.Count > 0)
            {
                if (_registeredDecorators.MaximumValue < previousMaximumSize)
                {
                    foreach (SharedSizeDecorator decoratorToInvalidate in _registeredDecorators.Keys)
                    {
                        decoratorToInvalidate.InvalidateMeasure();
                    }
                }
            }
            else
            {
                _sharedSizeScope.UnregisterSharedSize(this);
            }
        }

        /// <summary>
        /// Updates size of the decorator.
        /// </summary>
        /// <param name="decorator">Decorator to update.</param>
        /// <param name="newSize">New minimum size of the decorator.</param>
        public void UpdateDecoratorSize(SharedSizeDecorator decorator, double newSize)
        {
            double previousMaximumSize = MaximumSize;
            _registeredDecorators.UpdateValue(decorator, Math.Max(newSize, 0.0));
            if (_registeredDecorators.MaximumValue != previousMaximumSize)
            {
                foreach (SharedSizeDecorator decoratorToInvalidate in _registeredDecorators.Keys)
                {
                    decoratorToInvalidate.InvalidateMeasure();
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Type of the shared size.
    /// </summary>
    public enum SharedSizeType
    {
        Height,
        Width
    }
}
