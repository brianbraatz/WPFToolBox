using System;
using System.Collections.ObjectModel;

namespace WpfBootcampDemo.Model
{
    /// <summary>
    /// Stores a list of Xray objects.  When items are 
    /// added to or removed from this collection,
    /// the UI is notified because it derives from 
    /// ObservableCollection.  This demo does not add
    /// or remove items from the list, but it could!
    /// </summary>
    public class XrayCollection : ObservableCollection<Xray>
    {
        protected XrayCollection()
        {
        }

        public static XrayCollection Load()
        {
            // For this demo app, we use hard-coded values to keep things simple.
            XrayCollection xrays = new XrayCollection();
            xrays.Add(new Xray(new DateTime(2005, 09, 25), "xray1.jpg", XraySide.Left));
            xrays.Add(new Xray(new DateTime(2006, 10, 26), "xray2.jpg", XraySide.Front));
            xrays.Add(new Xray(new DateTime(2007, 11, 27), "xray3.jpg", XraySide.Right));
            return xrays;
        }
    }
}