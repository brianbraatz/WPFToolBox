using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Xml;
 


namespace Samples
{


    public class ListBoxDragDropDataProvider : IDataDropObjectProvider
    {

        private ListBox _ListBox;
        public ListBoxDragDropDataProvider(ListBox list)
        {
            this._ListBox = list;

        }
        public DragDropProviderActions SupportedActions
        {
            get
            {
                return DragDropProviderActions.Data | DragDropProviderActions.Visual | DragDropProviderActions.Unparent | DragDropProviderActions.MultiFormatData ;
            }
        }



        #region IDataDropObjectProvider Members

        public void AppendData ( ref IDataObject data ,  System.Windows.Input.MouseEventArgs e)
        {

            object o = this._ListBox.SelectedItem;


            // This is cheating .. just for an example's sake.. 
            System.Diagnostics.Debug.Assert(data.GetDataPresent(DataFormats.Text) == false);            
            if ( o.GetType() == typeof(XmlElement)) 
            { 
                data.SetData ( DataFormats.Text ,  ((XmlElement)o).OuterXml ) ;
            } 
            else 
                data.SetData ( DataFormats.Text ,  o.ToString() ) ;


            
            System.Diagnostics.Debug.Assert(data.GetDataPresent(o.GetType().ToString()) == false);
            data.SetData( o.GetType().ToString(), o ); 
            

        }

        public object GetData()
        {
            object o = this._ListBox.SelectedItem;
            return o; 
        } 

        public System.Windows.UIElement GetVisual(System.Windows.Input.MouseEventArgs e)
        {
            return _ListBox.ItemContainerGenerator.ContainerFromItem(_ListBox.SelectedItem) as UIElement;
        }

        public void GiveFeedback(System.Windows.GiveFeedbackEventArgs args)
        {
            throw new NotImplementedException( "Forgot to check the Supported actions??"); 
        }

        public void ContinueDrag(System.Windows.QueryContinueDragEventArgs args)
        {
            throw new NotImplementedException("Forgot to check the Supported actions??"); 
        }
        public bool UnParent()
        {
            // We are passing data, nothing to unparent 
            throw new NotImplementedException("We are passing data, nothing to unparent... what up "); 
            return true;
        }

        #endregion
    }
}
