/*
 * Created by: Peter Weissbrod
 * Created: Friday, January 25, 2008
 */
using System;
using System.ComponentModel;
using BloggerApp.Models;

namespace BloggerApp.ViewModels
{
    /// <summary>
    /// A view model is a facsimile of a business object that is safe for databinding and invalidation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ViewModelBase<T> : IDataErrorInfo where T : BusinessObject
    {
        /// <summary>
        /// The model that the ViewModel is representing
        /// </summary>
        protected T Model
        {
            get;
            set;
        }
        /// <summary>
        /// ViewModels can represent new Models that havent been created yet.
        /// If the model is null, then a "construction" will occur uppon the call to ApplyChanges
        /// </summary>
        protected bool IsNewModelInstance
        {
            get { return Model == null; }
        }
        /// <summary>
        /// Setting up for a new model instance
        /// </summary>
        public ViewModelBase()
        {
            Model = null;
        }
        /// <summary>
        /// setting up for editing an existing model
        /// </summary>
        /// <param name="model"></param>
        public ViewModelBase(T model)
        {
            Model = model;
        }
        /// <summary>
        /// Attempt to create or edit the underlying business object
        /// </summary>
        public void ApplyChanges()
        {
            try
            {
                if (IsNewModelInstance)
                    CreateModel();
                else
                    ApplyToModel();
                UpdateViewModelFromBusinessObject();
            }
            catch(Exception e)
            {
                UpdateViewModelFromBusinessObject();
                throw e;
            }
        }
        /// <summary>
        /// edit the underlying model
        /// </summary>
        protected abstract void ApplyToModel();
        /// <summary>
        /// create the underlying model
        /// </summary>
        protected abstract void CreateModel();
        /// <summary>
        /// Any changes to the model should be reflected in the ViewModel
        /// </summary>
        protected abstract void UpdateViewModelFromBusinessObject();

        #region IDataErrorInfo Members
        ///<summary>
        ///Gets the error message for the property with the given name.
        ///</summary>
        ///
        ///<returns>
        ///The error message for the property. The default is an empty string ("").
        ///</returns>
        ///
        ///<param name="columnName">The name of the property whose error message to get. </param>
        public abstract string this[string columnName] { get; }
        ///<summary>
        ///Gets an error message indicating what is wrong with this object.
        ///</summary>
        ///
        ///<returns>
        ///An error message indicating what is wrong with this object. The default is an empty string ("").
        ///</returns>
        ///
        public abstract string Error { get; }
        #endregion
    }
}