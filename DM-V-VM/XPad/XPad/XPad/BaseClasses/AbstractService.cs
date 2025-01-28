using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace XPad
{
    /// <summary>
    /// Abstract base class for all services.
    /// </summary>
    public abstract class AbstractService: AbstractWorkItem, IService
    {
        #region Fields
     
       
        #endregion

        #region Properties
    
        #endregion

          #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public AbstractService(string name) : base(name)
        {           
           
        }
        #endregion
			
        #region Methods
      
      
        #endregion

     

    

    }
}
