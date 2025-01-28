using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Controls.Primitives;
namespace XPad
{
    /// <summary>
    /// Allows you to bind diverse commands to the UI surface;
    /// <example>
    /// The 'Save' action for example can be bound to a button in the following way;
    /// <code>
    /// <Button local:Controller.SaveCommand="{Binding RelativeSource={x:Static RelativeSource.Self}}">
    /// </code>
    /// Note that if the UIElement does not support the Command it will not be raised and the binding will not function.
    /// </example>
    /// </summary>
    public static class Controller
    {
        #region Fields
        /// <summary>
        /// A ref to the model
        /// </summary>
        private static TextModel mModel;
       
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public static TextModel Model
        {
            get { return mModel; }
            set { mModel = value; }
        }
        
        
        #endregion

        #region Constructor
        static Controller()
        {
           
           
        }
        #endregion

        #region Methods
        public static object GetService(Type serviceType)
        {


            //if (registeredServices.Contains(serviceType))
            //{
            //    Type servingType = registeredServices[serviceType];

            //    return Activator.CreateInstance(servingType); //we don't keep previous instances in memory, services should operate and disappear thereafter.
            //}
            return null;
        }

        #endregion

        #region Classes
    
        #endregion
    }


   
}
