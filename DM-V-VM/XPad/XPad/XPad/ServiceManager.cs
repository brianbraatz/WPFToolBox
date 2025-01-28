using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{

    /// <summary>
    /// Manages the registered services.
    /// </summary>
    static class ServiceManager 
    {
        #region Fields
        private static Dictionary<Type,Type> registeredServices;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the registered services.
        /// </summary>
        /// <value>The tools.</value>
        public static Dictionary<Type, Type> Services
        {
            get { return registeredServices; }
        }
        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        static ServiceManager()
        {
            registeredServices = new Dictionary<Type, Type>();
            registeredServices.Add(typeof(ITextFileService), typeof(TextFileService));
        }
        #endregion
  
        #region Methods

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        /// <remarks>
        /// In a more elaborate scenario we could configure things by means of an external XML file
        /// so that services are loaded dynamically through reflection, for example.
        /// </remarks>
        public static object GetService(Type serviceType)
        {


            if (registeredServices.ContainsKey(serviceType))
            {
                Type servingType = registeredServices[serviceType];

                return Activator.CreateInstance(servingType); //we don't keep previous instances in memory, services should operate and disappear thereafter.
            }

            return null;
        }

        #endregion

    }
}
