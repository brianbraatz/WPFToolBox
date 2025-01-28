//---------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation, 2006
//
//  Description: SharedSizeScope class.
//  Creator: t-tomkm
//  Date Created: 08/17/2006
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Microsoft.Samples.WPF.CustomControls
{
    /// <summary>
    /// Class that returns SharedSize object for given width or height group name.
    /// </summary>
    public class SharedSizeScope
    {
        #region Static members
        /// <summary>
        /// SharedSizeScope attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SharedSizeScopeProperty;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static SharedSizeScope()
        {
            SharedSizeScopeProperty = DependencyProperty.RegisterAttached("SharedSizeScope",
                typeof(SharedSizeScope), typeof(SharedSizeScope));            
        }

        /// <summary>
        /// Gets SharedSizeScope attached property from the given target.
        /// </summary>
        /// <param name="target">Target that has the property attached.</param>
        /// <returns>Value of SharedSzieScope attached property from the given target.</returns>
        public static SharedSizeScope GetSharedSizeScope(DependencyObject target)
        {
            return (SharedSizeScope)target.GetValue(SharedSizeScopeProperty);
        }
        
        /// <summary>
        /// Sets SharedSizeScope attached property for the given target.
        /// </summary>
        /// <param name="target">Target that has the property attached.</param>
        /// <param name="value">Value of the property.</param>
        public static void SetSharedSizeScope(DependencyObject target, SharedSizeScope value)
        {
            target.SetValue(SharedSizeScopeProperty, value);
        }
        #endregion

        #region Fields
        private Dictionary<string, SharedSize> _sharedWidths;
        private Dictionary<string, SharedSize> _sharedHeights;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SharedSizeScope()
        {
            _sharedWidths = new Dictionary<string, SharedSize>();
            _sharedHeights = new Dictionary<string, SharedSize>();
        }

        /// <summary>
        /// Removes the SharedSize object. This method is 
        /// invoked by SharedSize object when it unregisters its last decorator.
        /// </summary>
        /// <param name="sharedSize">SharedSize object to unregister.</param>
        public void UnregisterSharedSize(SharedSize sharedSize)
        {
            switch (sharedSize.SizeType)
            {
                case SharedSizeType.Height: 
                    _sharedHeights.Remove(sharedSize.GroupName); 
                    break;

                case SharedSizeType.Width:
                    _sharedWidths.Remove(sharedSize.GroupName);
                    break;
            }
        }

        /// <summary>
        /// Returns SharedSize for given width group name. If it doesn't exist, the new
        /// object will be created.
        /// </summary>
        /// <param name="name">Name of the width group.</param>
        /// <returns>SharedSize object with the given width group name.</returns>
        public SharedSize GetSharedWidthByName(string name)
        {
            if (_sharedWidths.ContainsKey(name))
            {
                return _sharedWidths[name];
            }
            else
            {
                SharedSize result = new SharedSize(this, SharedSizeType.Width, name);
                _sharedWidths[name] = result;
                return result;
            }
        }

        /// <summary>
        /// Returns SharedSize for given height group name. If it doesn't exist, the new 
        /// object will be created.
        /// </summary>
        /// <param name="name">Name of the height group.</param>
        /// <returns>SharedSize object with the given height group name.</returns>
        public SharedSize GetSharedHeightByName(string name)
        {
            if (_sharedHeights.ContainsKey(name))
            {
                return _sharedHeights[name];
            }
            else
            {
                SharedSize result = new SharedSize(this, SharedSizeType.Height, name);
                _sharedHeights[name] = result;
                return result;
            }
        }
    }
}
