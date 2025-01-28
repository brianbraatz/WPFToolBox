using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Controls;


namespace Samples
{
    public class DropHelper
    {
        UIElement _dropTarget = null;


        string[] _datatypes = { typeof(UIElement).ToString(), "Text" };

        public string[] AllowedDataTypes
        {
            get { return _datatypes; }
            set
            {
                _datatypes = value;
                for (int x = 0; x < _datatypes.Length; x++)
                {
                    _datatypes[x] = _datatypes[x].ToLower();
                }
            }
        }

        public DropHelper(UIElement wrapper)
        {
            System.Diagnostics.Debug.Assert(wrapper != null);
            _dropTarget = wrapper;

            _dropTarget.AllowDrop = true;
            _dropTarget.DragEnter += new DragEventHandler(_dropTarget_DragEnter);
            _dropTarget.DragOver += new DragEventHandler(DropTarget_DragOver);
            _dropTarget.Drop += new DragEventHandler(DropTarget_Drop);
            _dropTarget.DragLeave += new DragEventHandler(_dropTarget_DragLeave);

        }

        private bool Unparent(DragDataWrapper dw, UIElement uie)
        {
            bool success = false;

            if (dw != null)
            {
                if (dw.AllowChildrenRemove)
                {
                    dw.Shim.UnParent();
                }

            }

            if (!success) // BRUTE FORCE 
            {
                if (uie is FrameworkElement)
                {
                    FrameworkElement fe = uie as FrameworkElement;
                    if (fe.Parent != null)
                    {
                        if (fe.Parent is Panel)
                        {
                            try
                            {
                                ((Panel)(fe.Parent)).Children.Remove(uie);
                                success = true;
                            }
#if DEBUG
                            catch (System.Exception ex)
                            {
                                System.Diagnostics.Debug.Assert(false);
                            }
#endif
                        }
                    }
                    else if (fe.Parent is ContentControl)
                    {
                        ContentControl cc = fe.Parent as ContentControl;
                        cc.Content = null;
                        success = true;
                    }                  
                }
            }
            return success;
        }


        void DropTarget_Drop(object sender, DragEventArgs e)
        {

            IDataObject data = e.Data;
            System.Diagnostics.Debug.Assert(data != null);


            DragDataWrapper dw = null;
            bool isDataOperation = false;

            if (data.GetDataPresent(typeof(DragDataWrapper).ToString()))
            {
                dw = data.GetData(typeof(DragDataWrapper).ToString()) as DragDataWrapper;
                if ((dw.Shim.SupportedActions & DragDropProviderActions.Data) != 0)
                {
                    isDataOperation = true;
                }
            }


            if (!isDataOperation)
            // Try a BRUTE FORCE APPROACH on UIElement just to show how it could be done
            // BUT NOT ENDORSING IT!!! 
            {
                if (data.GetDataPresent(typeof(UIElement).ToString()))
                {
                    UIElement uie = (UIElement)data.GetData(typeof(UIElement).ToString());

                    bool unparented = false;
                    if (e.AllowedEffects == DragDropEffects.Move)
                    {
                        unparented = this.Unparent(dw, uie);

                    }
                    if (unparented)
                    {
                        if (_dropTarget is Panel)
                        {
                            Panel p = (Panel)_dropTarget;
                            if (data.GetDataPresent(typeof(UIElement).ToString()))
                            {
                                p.Children.Add(((UIElement)data.GetData(typeof(UIElement).ToString())));
                            }
                        }
                        else if (_dropTarget is ContentControl)
                        {
                            ((ContentControl)_dropTarget).Content = uie;
                        }
                        else if (_dropTarget is ItemsControl)
                        {
                            ItemsControl ic = _dropTarget as ItemsControl;
                            if (ic.ItemsSource != null)
                            {

                            }
                            else
                            {
                                ic.Items.Insert(0, uie);
                            }
                        }
                    }
                }
                else if (data.GetDataPresent(DataFormats.Text))
                {
                    string datastring = (string) data.GetData(DataFormats.Text); 
                        
                        
                    if (_dropTarget is Panel)
                    {
                        TextBlock t = new TextBlock();
                        t.Text = datastring;
                        t.FontSize = 16; 
                        
                        Panel p = (Panel)_dropTarget;
                        t.MaxWidth = p.ActualWidth;
                        t.TextWrapping = TextWrapping.Wrap; 
                        p.Children.Add(t);
                       
                    }
                    else if (_dropTarget is ContentControl)
                    {
                        ((ContentControl)_dropTarget).Content = datastring ;
                    }
                } 
            }
            else
            {
                // You should know what to to with the data ... 
                // As an example here, I show how to handle the ListBox, which is most interesting .. 
               
                System.Diagnostics.Debug.Assert(dw != null);
                System.Diagnostics.Debug.Assert(dw.Shim != null);

                object rawdata = dw.Data;


                if (_dropTarget is ItemsControl)
                {
                    ItemsControl ic = _dropTarget as ItemsControl;

                    System.Collections.IList list = ic.ItemsSource as System.Collections.IList;

                    if (list == null)
                    {
                        list = ic.Items as System.Collections.IList;
                    }

                    if (list != null)
                    {
                        if (!list.Contains(rawdata))
                        {
                            // Here we do not check for Move | Copy ... because this is a DATA operation .. No parent relationshop at all ... 
                            list.Add(rawdata);
                        }
                        else
                        {
                            // nothing was done ... 
                            System.Diagnostics.Debug.WriteLine("Chose to not duplicate objects.. uncomment the check if you need to duplicate"); 
                            e.Effects = DragDropEffects.None;
                        }
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                else if (_dropTarget is ContentControl)
                {
                    ContentControl c = _dropTarget as ContentControl;
                    c.Content = rawdata; 
                        
                } 


            }


            DeHighlight(sender, e);
        }

        private DragDropEffects _allowedEffects;

        public DragDropEffects AllowedEffects
        {
            get { return _allowedEffects; }
            set { _allowedEffects = value; }
        }


        void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            string[] types = e.Data.GetFormats();
            bool match = false;

            if (_datatypes == null || types == null)
            {
                //TODO: ??? Should we set for DragDropEffects.None? 
                return;
            }

            foreach (string s in types)
            {

                foreach (string type in _datatypes)
                {
                    match = (s.ToLower() == type);
                    if (match)
                        break;
                }
                if (match)
                    break;
            }
            if (match)
            {
                e.Effects = AllowedEffects;
                e.Handled = true;
            }
            System.Diagnostics.Debug.WriteLine("dropTarget_DragOver");

        }

        private Brush _oldBackground;
        private Brush _oldBorderBrush;
        private Thickness _oldBorderThickness;

        void Highlight(object sender, DragEventArgs e)
        {

            if (_dropTarget is Panel)
            {
                Panel panel = _dropTarget as Panel;
                this._oldBackground = panel.Background;
                panel.Background = Brushes.LawnGreen;

            }
            else if (_dropTarget is Control)
            {
                Control c = _dropTarget as Control;
                this._oldBorderThickness = c.BorderThickness;
                this._oldBorderBrush = c.BorderBrush;
                c.BorderBrush = Brushes.LawnGreen;
                c.BorderThickness = new Thickness(3);
            }
        }

        void DeHighlight(object sender, DragEventArgs e)
        {

            if (_dropTarget is Panel)
            {
                Panel panel = _dropTarget as Panel;
                panel.Background = this._oldBackground;


            }
            else if (_dropTarget is Control)
            {
                Control c = _dropTarget as Control;
                c.BorderThickness = this._oldBorderThickness;
                c.BorderBrush = this._oldBorderBrush;
            }
        }

        void _dropTarget_DragLeave(object sender, DragEventArgs e)
        {
            DeHighlight(sender, e);
        }



        void _dropTarget_DragEnter(object sender, DragEventArgs e)
        {
            Highlight(sender, e);
        }


    }

}
