using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PeteBrown.UserControlScreens.Controls.Base
{
    /// <summary>
    /// Encapsulates functionality used by all controls in this application.
    /// </summary>
    public abstract class ControlBase : Control
    {
        private DependencyObject _rootElement;  // Set by LoadXaml. This is the starting point for all name searches

        /// <summary>
        /// Searches for an element by its name. Since this is a generic function,
        /// it returns a strongly-typed reference to the found element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T FindByName<T>(string name) where T : DependencyObject
        {
            return _rootElement.FindName(name) as T;
        }

        /// <summary>
        /// Set by LoadXaml. This is the starting point for all name searches.
        /// </summary>
        protected DependencyObject RootElement
        {
            get { return _rootElement; }
            set { _rootElement = value; }
        }

        /// <summary>
        /// Handles the InitializeFromXaml call that typically shows up 
        /// in a usercontrol's constructor.
        /// </summary>
        /// <param name="xamlResourceName"></param>
        protected void LoadXaml(string xamlResourceName)
        {
            try
            {
                System.IO.Stream s = this.GetType().Assembly.GetManifestResourceStream(xamlResourceName);
                _rootElement = this.InitializeFromXaml(new System.IO.StreamReader(s).ReadToEnd());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
