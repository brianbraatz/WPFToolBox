using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Microsoft.Samples.KMoore.WPFSamples.Transition
{
    // Base class for all transitions.  
    public class Transition : DependencyObject
    {
        public bool ClipToBounds
        {
            get { return (bool)GetValue(ClipToBoundsProperty); }
            set { SetValue(ClipToBoundsProperty, value); }
        }

        public static readonly DependencyProperty ClipToBoundsProperty =
            DependencyProperty.Register("ClipToBounds",
                typeof(bool),
                typeof(Transition),
                new UIPropertyMetadata(false));

        public bool IsNewContentTopmost
        {
            get { return (bool)GetValue(IsNewContentTopmostProperty); }
            set { SetValue(IsNewContentTopmostProperty, value); }
        }

        public static readonly DependencyProperty IsNewContentTopmostProperty =
            DependencyProperty.Register("IsNewContentTopmost",
                typeof(bool),
                typeof(Transition),
                new UIPropertyMetadata(true));

        // Called when an element is Removed from the TransitionPresenter's visual tree
        protected internal virtual void BeginTransition(TransitionPresenter transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            EndTransition(transitionElement, oldContent, newContent);
        }

        //Transitions should call this method when they are done 
        protected void EndTransition(TransitionPresenter transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            OnTransitionEnded(transitionElement, oldContent, newContent);

            transitionElement.OnTransitionCompleted(this);
        }

        //Transitions can override this to perform cleanup at the end of the transition
        protected virtual void OnTransitionEnded(TransitionPresenter transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
        }

        // Returns a clone of the element and hides it in the main tree
        protected Brush CreateBrush(TransitionPresenter transitionElement, ContentPresenter content)
        {
            ((Decorator)content.Parent).Visibility = Visibility.Hidden;

            VisualBrush brush = new VisualBrush(content);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(brush, 40);
            return brush;
        }
    }
}
