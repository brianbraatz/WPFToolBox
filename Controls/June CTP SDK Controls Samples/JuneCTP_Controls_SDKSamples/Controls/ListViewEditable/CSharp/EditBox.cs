using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media;

namespace Editing
{
    /// <summary>
    /// EditBox is a custom cotrol that can switch between two modes: 
    /// editing and normal. When it is in editing mode, the content is
    /// displayed in a TextBox that provides editing capbability. When 
    /// the EditBox is in normal, its content is displayed in a TextBlock
    /// that is not editable.
    /// 
    /// This control is designed to be used in with a GridView View.
    /// </summary>
    // <!--<SnippetEditBoxStart>
    public class EditBox : Control
    {
    // <!--</SnippetEditBoxStart>

        #region Static Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static EditBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditBox), 
                new FrameworkPropertyMetadata(typeof(EditBox)));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the tree for the EditBox has been generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBlock textBlock = GetTemplateChild("PART_TextBlockPart") 
                   as TextBlock;
            Debug.Assert(textBlock != null, "No TextBlock!");

            _textBox = new TextBox();
            _adorner = new EditBoxAdorner(textBlock, _textBox);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBlock);
            layer.Add(_adorner);

            _textBox.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);
            _textBox.LostKeyboardFocus += 
              new KeyboardFocusChangedEventHandler(OnTextBoxLostKeyboardFocus);

            //Receive notification of the Revent to handle the the column 
            //resize. 
            HookTemplateParentResizeEvent();

            //Capture the resize event to  handle ListView resize cases.
            HookItemsControlEvents();

            _listViewItem = GetDependencyObjectFromVisualTree(this, 
                typeof(ListViewItem)) as ListViewItem;

            Debug.Assert(_listViewItem != null, "No ListViewItem found");
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// If the ListViewItem that contains the EditBox is selected, 
        /// when the mouse pointer moves over the EditBox, the corresponding
        /// MouseEnter event is the first of two events (MouseUp is the second)
        /// that allow the EditBox to change to editing mode.
        /// </summary>
        //<SnippetOnMouseEnter>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!IsEditing && IsParentSelected)
            {
                _canBeEdit = true;
            }
        }
        //</SnippetOnMouseEnter>

        /// <summary>
        /// If the MouseLeave event occurs for an EditBox control that
        /// is in normal mode, the mode cannot be changed to editing mode
        /// until a MouseEnter event followed by a MouseUp event occurs.
        /// </summary>
        //<SnippetOnMouseLeave>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseWithinScope = false;
            _canBeEdit = false;
        }
        //</SnippetOnMouseLeave>
        /// <summary>
        /// An EditBox switches to editing mode when the MouseUp event occurs
        /// for that EditBox and the following conditions are satisfied:
        /// 1. A MouseEnter event for the EditBox occured before the 
        /// MouseUp event.
        /// 2. The mouse did not leave the EditBox between the
        /// MouseEnter and MouseUp events.
        /// 3. The ListViewItem that contains the EditBox was selected
        /// when the MouseEnter event occurred.
        /// </summary>
        /// <param name="e"></param>
        //<SnippetOnMouseUp>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Right || 
                e.ChangedButton == MouseButton.Middle)
                return;

            if (!IsEditing)
            {
                if (!e.Handled && (_canBeEdit || _isMouseWithinScope))
                {
                    IsEditing = true;
                }

                //If the first MouseUp event selects the parent ListViewItem,
                //then the second MouseUp event puts the EditBox in editing 
                //mode
                if (IsParentSelected)
                    _isMouseWithinScope = true;
            }
        }
        //</SnippetOnMouseUp>

        #endregion

        #region Public Properties

        #region Value

        /// <summary>
        /// ValueProperty DependencyProperty.
        /// </summary>
        //<SnippetValueProperty>
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(
                        "Value",
                        typeof(object),
                        typeof(EditBox),
                        new FrameworkPropertyMetadata(null));
        //</SnippetValueProperty>

        /// <summary>
        /// Gets or sets the value of the EditBox
        /// </summary>
         public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region IsEditing

        /// <summary>
        /// IsEditingProperty DependencyProperty
        /// </summary>
        //<SnippetIsEditingProperty>
        public static DependencyProperty IsEditingProperty =
                DependencyProperty.Register(
                        "IsEditing",
                        typeof(bool),
                        typeof(EditBox),
                        new FrameworkPropertyMetadata(false));
        //</SnippetIsEditingProperty>

        /// <summary>
        /// Returns true if the EditBox control is in editing mode.
        /// </summary>
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            private set
            {
                SetValue(IsEditingProperty, value);
                _adorner.UpdateVisibilty(value);
            }
        }

        #endregion

        #region IsParentSelected

        /// <summary>
        /// Gets whether the ListViewItem that contains the 
        /// EditBox is selected.
        /// </summary>
        private bool IsParentSelected
        {
            get
            {
                if (_listViewItem == null)
                    return false;
                else
                    return _listViewItem.IsSelected;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// When an EditBox is in editing mode, pressing the ENTER or F2
        /// keys switches the EditBox to normal mode.
        /// </summary>
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (IsEditing && (e.Key == Key.Enter || e.Key == Key.F2))
            {
                IsEditing = false;
                _canBeEdit = false;
            }
        }

        /// <summary>
        /// If an EditBox loses focus while it is in editing mode, 
        /// the EditBox mode switches to normal mode.
        /// </summary>
        private void OnTextBoxLostKeyboardFocus(object sender, 
                                             KeyboardFocusChangedEventArgs e)
        {
            IsEditing = false;
        }

        /// <summary>
        /// Sets IsEditing to false when the ListViewItem that contains an
        /// EditBox changes its size
        /// </summary>
        private void OnCouldSwitchToNormalMode(object sender, 
                                               RoutedEventArgs e)
        {
            IsEditing = false;
        }

        /// <summary>
        /// Walk the visual tree to find the ItemsControl and 
        /// hook its some events on it.
        /// </summar
        private void HookItemsControlEvents()
        {
            _itemsControl = GetDependencyObjectFromVisualTree(this, 
                                typeof(ItemsControl)) as ItemsControl;
            if (_itemsControl != null)
            {
                //Handle the Resize/ScrollChange/MouseWheel 
                //events to determine whether to switch to Normal mode
                _itemsControl.SizeChanged += 
                    new SizeChangedEventHandler(OnCouldSwitchToNormalMode);
                _itemsControl.AddHandler(ScrollViewer.ScrollChangedEvent, 
                    new RoutedEventHandler(OnScrollViewerChanged));
                _itemsControl.AddHandler(ScrollViewer.MouseWheelEvent, 
                    new RoutedEventHandler(OnCouldSwitchToNormalMode), true);
            }
        }

        /// <summary>
        /// If an EditBox is in editing mode and the content of a ListView is
        /// scrolled, then the EditBox switches to normal mode.
        /// </summary>
        private void OnScrollViewerChanged(object sender, RoutedEventArgs args)
        {
            if (IsEditing && Mouse.PrimaryDevice.LeftButton == 
                                MouseButtonState.Pressed)
            {
                IsEditing = false;
            }
        }

        /// <summary>
        /// Walk visual tree to find the first DependencyObject 
        /// of the specific type.
        /// </summary>
        private DependencyObject 
            GetDependencyObjectFromVisualTree(DependencyObject startObject, 
                                              Type type)
        {
            //Walk the visual tree to get the parent(ItemsControl) 
            //of this control
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        /// <summary>
        /// When the size of the column containing the EditBox changes
        /// and the EditBox is in editing mode, switch the mode to normal mode 
        /// </summary>
        private void HookTemplateParentResizeEvent()
        {
            FrameworkElement parent = TemplatedParent as FrameworkElement;
            if (parent != null)
            {
                parent.SizeChanged += 
                    new SizeChangedEventHandler(OnCouldSwitchToNormalMode);
            }
        }

        #endregion

        #region private variable
        
        private EditBoxAdorner _adorner; 
        //A TextBox in the visual tree
        private FrameworkElement _textBox;
        //Specifies whether an EditBox can switch to editing mode. 
        //Set to true if the ListViewItem that contains the EditBox is 
        //selected, when the mouse pointer moves over the EditBox
        private bool _canBeEdit = false;
        //Specifies whether an EditBox can switch to editing mode.
        //Set to true when the ListViewItem that contains the EditBox is 
        //selected when the mouse pointer moves over the EditBox.
        private bool _isMouseWithinScope = false;
        //The ListView control that contains the EditBox
        private ItemsControl _itemsControl;
        //The ListViewItem control that contains the EditBox
        private ListViewItem _listViewItem;

        #endregion
   //<SnippetEditBoxEnd>
    }
   //</SnippetEditBoxEnd>

}
