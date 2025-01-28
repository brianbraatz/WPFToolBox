using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace Ricciolo.Controls
{
    /// <summary>
    /// Interaction logic for TabControl3D.xaml
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(Item3D))]
    public partial class Selector3D : Selector
    {

        private List<GeometryModel3D> faces = new List<GeometryModel3D>();
        private Dictionary<Object, VisualBrush> brushes = new Dictionary<object, VisualBrush>();
        private Panel panel;
        private Viewport3D viewbox;
        private bool storyNextApplied;
        private bool storyBackApplied;
        private int currentFace = -1;
        private ItemsPresenter presenter;

        public static RoutedCommand MoveNextCommand = new RoutedCommand("MoveNext", typeof(Selector3D));
        public static RoutedCommand MoveBackCommand = new RoutedCommand("MoveBack", typeof(Selector3D));

        static Selector3D()
        {
            // Prepare commands
            CommandManager.RegisterClassCommandBinding(typeof(Selector3D), new CommandBinding(MoveNextCommand, new ExecutedRoutedEventHandler(OnMoveNext), new CanExecuteRoutedEventHandler(OnCanMoveNext)));
            CommandManager.RegisterClassCommandBinding(typeof(Selector3D), new CommandBinding(MoveBackCommand, new ExecutedRoutedEventHandler(OnMoveBack), new CanExecuteRoutedEventHandler(OnCanMoveBack)));

            ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Panel3D)));
            template.Seal();
            ItemsControl.ItemsPanelProperty.OverrideMetadata(typeof(Selector3D), new FrameworkPropertyMetadata(template));
        }

        public Selector3D()
        {
            InitializeComponent();
        }

        private static void OnMoveNext(object s, RoutedEventArgs e)
        {
            Selector3D tab = (Selector3D)s;
            // Check is not already rotating
            if (tab.IsRotating) return;

            tab.Items.MoveCurrentToNext();
            //return;
            //if (tab.SelectedIndex >= tab.Items.Count - 1)
            //    tab.SelectedIndex = 0;
            //else
            //    tab.SelectedIndex++;
        }

        private static void OnMoveBack(object s, RoutedEventArgs e)
        {
            Selector3D tab = (Selector3D)s;
            // Check is not already rotating
            if (tab.IsRotating) return;

            tab.Items.MoveCurrentToPrevious();
            //if (tab.SelectedIndex == 0)
            //    tab.SelectedIndex = tab.Items.Count - 1;
            //else
            //    tab.SelectedIndex--;
        }

        private static void OnCanMoveBack(object s, CanExecuteRoutedEventArgs e)
        {
            Selector3D tab = (Selector3D)s;
            e.CanExecute = (tab.SelectedIndex > 0);
        }

        private static void OnCanMoveNext(object s, CanExecuteRoutedEventArgs e)
        {
            Selector3D tab = (Selector3D)s;
            e.CanExecute = (tab.SelectedIndex < tab.Items.Count - 1);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Item3D();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is Item3D);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            // Prepare brush
            PrepareItemBrush(item, (Item3D)element);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.OldItems != null)
            {
                foreach (object i in e.OldItems)
                {
                    this.brushes.Remove(i);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            presenter = (ItemsPresenter)this.GetTemplateChild("presenter");

            faces.Clear();
            brushes.Clear();
            panel = (Panel)this.GetTemplateChild("panel");

            // Enumerate box faces
            viewbox = (Viewport3D)this.GetTemplateChild("viewPort3D");
            Model3DGroup cube = (Model3DGroup)this.GetTemplateChild("cube");
            foreach (Model3D m in cube.Children)
            {
                if (m is GeometryModel3D)
                    faces.Add((GeometryModel3D)m);
            }

            // Prepare materials
            for (int x = 0; x < faces.Count; x++)
            {
                MaterialGroup mg = (MaterialGroup)faces[x].Material;
                DiffuseMaterial df = new DiffuseMaterial();
                mg.Children.Add(df);

                SpecularMaterial sm = new SpecularMaterial(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DBDBDB")), 64);
                mg.Children.Add(sm);
            }

            // Remove all childs and prepare visual
            for (int x = 0; x < this.Items.Count; x++)
            {
                Item3D ti = GetItemByIndex(x);
                PrepareItemBrush(this.Items[x], ti);
            }

            ((Storyboard)panel.FindResource("RotationEffectNext")).Completed += new EventHandler(story_Completed);
            ((Storyboard)panel.FindResource("RotationEffectBack")).Completed += new EventHandler(story_Completed);

            // Autoselect first index
            if (this.SelectedIndex != 0)
                this.SelectedIndex = 0;
        }

        private void PrepareItemBrush(object item, Item3D ti)
        {
            VisualBrush vb = new VisualBrush(ti);
            vb.ViewboxUnits = BrushMappingMode.Absolute;

            this.brushes.Add(ti, vb);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            // First selections
            if (e.RemovedItems.Count == 0)
                ShowItem();
            else
            {
                // Compute if go forward or back
                int previousIndex = this.Items.IndexOf(e.RemovedItems[0]);
                if (this.SelectedIndex > previousIndex)
                    SelectNextFace(previousIndex);
                else
                    SelectBackFace(previousIndex);
            }
        }

        private void ShowItem()
        {
            presenter.Visibility = Visibility.Visible;
            viewbox.Visibility = Visibility.Hidden;
        }

        private Item3D GetItemByIndex(int index)
        {
            Item3D item = this.Items[index] as Item3D;
            if (item == null)
                item = (Item3D)this.ItemContainerGenerator.ContainerFromIndex(index);
            return item;
        }

        private void SelectNextFace(int previousIndex)
        {
            if (this.IsRotating) return;

            // Remove previus item
            panel.Children.Remove(GetItemByIndex(previousIndex));

            this.currentFace++;
            if (this.currentFace > this.faces.Count - 1)
                this.currentFace = 0;

            storyNextApplied = true;
            RotateCube(this.viewbox, (Storyboard)panel.FindResource("RotationEffectNext"), previousIndex, (this.SelectedIndex < this.Items.Count - 1) ? this.SelectedIndex + 1 : 0);
        }

        private void SelectBackFace(int previousIndex)
        {
            if (this.IsRotating) return;

            // Remove previous item
            panel.Children.Remove(GetItemByIndex(previousIndex));

            this.currentFace--;
            if (this.currentFace < 0)
                this.currentFace = this.faces.Count - 1;

            storyBackApplied = true;

            RotateCube(this.viewbox, (Storyboard)panel.FindResource("RotationEffectBack"), (this.SelectedIndex > 0) ? this.SelectedIndex - 1 : this.Items.Count - 1, previousIndex);
        }

        public bool IsRotating
        {
            get
            {
                Storyboard story = (Storyboard)panel.FindResource("RotationEffectNext");
                Storyboard story2 = (Storyboard)panel.FindResource("RotationEffectBack");
                return ((storyNextApplied && story.GetCurrentState(this.viewbox) == ClockState.Active) || (storyBackApplied && story2.GetCurrentState(this.viewbox) == ClockState.Active));
            }
        }

        private void RotateCube(FrameworkElement sender, Storyboard story, int previousIndex, int nextIndex)
        {
            double w = panel.ActualWidth;
            double h = panel.ActualHeight;

            SetFaceVisual(w, h, this.currentFace, this.SelectedIndex);
            // Previous
            SetFaceVisual(w, h, (this.currentFace > 0) ? this.currentFace - 1 : this.faces.Count - 1, previousIndex);
            // Next
            SetFaceVisual(w, h, (this.currentFace < this.faces.Count - 1) ? this.currentFace + 1 : 0, nextIndex);

            viewbox.Visibility = Visibility.Visible;
            presenter.Visibility = Visibility.Hidden;

            story.Begin(sender, true);
        }

        private void SetFaceVisual(double w, double h, int faceIndex, int itemIndex)
        {
            MaterialGroup mg = (MaterialGroup)faces[faceIndex].Material;
            DiffuseMaterial df = (DiffuseMaterial)mg.Children[0];
            VisualBrush vb = this.brushes[this.GetItemByIndex(itemIndex)];
            vb.Viewbox = new Rect(0, 0, w, h);
            df.Brush = vb;
        }

        void story_Completed(object sender, EventArgs e)
        {
            // Clear material
            foreach (GeometryModel3D face in faces)
            {
                MaterialGroup mg = (MaterialGroup)face.Material;
                DiffuseMaterial df = (DiffuseMaterial)mg.Children[0];
                df.ClearValue(DiffuseMaterial.BrushProperty);
            }

            ShowItem();
        }
    }

    public class Item3D : ContentControl
    {

        public Item3D()
        {
        }

    }

    internal class Panel3D : Panel
    {

        private Selector itemsControl;

        public Panel3D()
        {

        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            itemsControl = ((ItemsPresenter)VisualParent).TemplatedParent as Selector;
            itemsControl.SelectionChanged += new SelectionChangedEventHandler(itemsControl_SelectionChanged);
        }

        void itemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentIndex = this.itemsControl.SelectedIndex;
            if (currentIndex < 0) return;

            int z = 0;
            for (int x = 0; x < this.InternalChildren.Count; x++)
                if (x != currentIndex)
                {
                    Panel.SetZIndex(this.Children[x], z);
                    z++;
                }
            z++;
            Debug.WriteLine(currentIndex);
            Panel.SetZIndex(this.InternalChildren[currentIndex], z);

            //this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int currentIndex = this.itemsControl.SelectedIndex;
            if (currentIndex < 0) return finalSize;

            Rect r = new Rect(new Point(0, 0), finalSize);

            //for (int x = Math.Max(currentIndex - 1, 0); x < Math.Min(this.InternalChildren.Count, currentIndex + 3); x++)
            for (int x = 0; x < this.InternalChildren.Count; x++)
                this.InternalChildren[x].Arrange(r);

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement e in this.Children)
            {
                e.Measure(availableSize);
            }

            return new Size();
        }
    }
}