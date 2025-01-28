using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Samples.DragDrop
{
    public class DropPreviewAdorner : Adorner
    {
		private ContentPresenter _presenter;
        private double _left;
        private double _top;
        private AdornerLayer _adornerLayer;
        private UIElement _adornedElement;

        public double Left
        {
            get { return _left; }
            set 
            { 
                _left = value;
                UpdatePosition();
            }
        }

        public double Top
        {
            get { return _top; }
            set 
            {
                _top = value;
                UpdatePosition();
            }
        }

        public DropPreviewAdorner(UIElement feedbackUI, UIElement adornedElt, AdornerLayer layer) : base(adornedElt)
        {
            _adornedElement = adornedElt;
            _adornerLayer = layer;

			_presenter = new ContentPresenter();
			_presenter.Content = feedbackUI;
			_presenter.IsHitTestVisible = false;
        }

        private void UpdatePosition()
        {
			if (_adornerLayer != null)
            {
				_adornerLayer.Update(_adornedElement);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
			_presenter.Measure(constraint);
			return _presenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
			_presenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
			return _presenter;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(Left, Top));
            result.Children.Add(base.GetDesiredTransform(transform));

            return result;
        }
    }
}
