using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace FeatureMontage
{
    /// <summary>
    /// A content presenter that performs an animation when content changes
    /// Inspired by JeffBog. Ported to Frame and bloated with features
    /// by FFortes
    /// 
    /// This can really only be used within the template for a content control --
    /// specifically NavigationWindow, or possibly Frame
    /// </summary>
    public class TransitioningFrame : Frame
    {
        #region Constructor
        public TransitioningFrame()
            : base()
        {
            _children = new List<UIElement>();
            _newChild = null;
            _oldChild = null;
        }
        #endregion

        #region Public Events

        public event EventHandler TransitionAnimationEnding;

        private void OnTransitionAnimationEnding(object sender, EventArgs e)
        {
            if (TransitionAnimationEnding != null)
            {
                TransitionAnimationEnding(this, e);
            }
        }

        #endregion

        #region Public Dependency Properties
        public static readonly DependencyProperty IsSlideAnimationEnabledProperty = DependencyProperty.Register("IsSlideAnimationEnabled", typeof(bool), typeof(TransitioningFrame), new PropertyMetadata(true));
        public bool IsSlideAnimationEnabled
        {
            get { return (bool)GetValue(IsSlideAnimationEnabledProperty); }
            set { SetValue(IsSlideAnimationEnabledProperty, value); }
        }

        public static readonly DependencyProperty OpacityEnterAnimationProperty = DependencyProperty.Register("OpacityEnterAnimation", typeof(DoubleAnimation), typeof(TransitioningFrame));
        public DoubleAnimation OpacityEnterAnimation
        {
            get { return (DoubleAnimation)GetValue(OpacityEnterAnimationProperty); }
            set { SetValue(OpacityEnterAnimationProperty, value); }
        }

        public static readonly DependencyProperty OpacityExitAnimationProperty = DependencyProperty.Register("OpacityExitAnimation", typeof(DoubleAnimation), typeof(TransitioningFrame));
        public DoubleAnimation OpacityExitAnimation
        {
            get { return (DoubleAnimation)GetValue(OpacityExitAnimationProperty); }
            set { SetValue(OpacityExitAnimationProperty, value); }
        }

        public static readonly DependencyProperty SlideDurationProperty = DependencyProperty.Register("SlideDuration", typeof(Duration), typeof(TransitioningFrame), new PropertyMetadata(new Duration(new TimeSpan(0, 0, 0, 0, 500))));
        public Duration SlideDuration
        {
            get { return (Duration)GetValue(SlideDurationProperty); }
            set { SetValue(SlideDurationProperty, value); }
        }

        public static readonly DependencyProperty SlideDirectionProperty = DependencyProperty.Register("SlideDirection", typeof(SlideDirection), typeof(TransitioningFrame), new PropertyMetadata(SlideDirection.FromLeft));
        public SlideDirection SlideDirection
        {
            get { return (SlideDirection)GetValue(SlideDirectionProperty); }
            set { SetValue(SlideDirectionProperty, value); }
        }

        public static readonly DependencyProperty SlideAccelerationRatioProperty = DependencyProperty.Register("SlideAccelerationRatio", typeof(double), typeof(TransitioningFrame), new PropertyMetadata(0d));
        public double SlideAccelerationRatio
        {
            get { return (double)GetValue(SlideAccelerationRatioProperty); }
            set { SetValue(SlideAccelerationRatioProperty, value); }
        }

        public static readonly DependencyProperty SlideDecelerationRatioProperty = DependencyProperty.Register("SlideDecelerationRatio", typeof(double), typeof(TransitioningFrame), new PropertyMetadata(0d));
        public double SlideDecelerationRatio
        {
            get { return (double)GetValue(SlideDecelerationRatioProperty); }
            set { SetValue(SlideDecelerationRatioProperty, value); }
        }
        #endregion

        public UIElement GetCommandTarget()
        {
            return this.GetTemplateChild("PART_CommandTarget") as UIElement;
        }

        /// <summary>
        /// Called when the content property changes
        /// We do the manipulation of our visual children here and kickoff 
        /// the transition animation
        /// </summary>
        /// <param name="oldContent">Old value of the content property</param>
        /// <param name="newContent">New value of the content property</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            // Update the _oldChild
            _oldChild = _newChild;

            // Check if we actually have new content to display
            if (newContent != null)
            {
                // If the new content is not a UIElement, then we must instantiate a 
                // ContentPresenter in order to display it
                _newChild = newContent as UIElement;
                if (_newChild == null)
                {
                    // We were unable to cast, so let's use a ContentPresenter to
                    // convert the object into something displayable (a UIElement)
                    ContentPresenter contentViewer = new ContentPresenter();
                    contentViewer.ContentTemplate = this.ContentTemplate;
                    contentViewer.ContentTemplateSelector = this.ContentTemplateSelector;
                    contentViewer.Content = newContent;

                    // Use the content presenter
                    _newChild = contentViewer;
                }

                if (_newChild == null)
                {
                    // Don't have content
                    _newChild = null;

                    // Throw for debugging purposes -- should remove this when shipping
                    // but this should never happen because ContentPresenter should always
                    // be able to display something
                    throw new Exception("Could not display new child");
                }
                else
                {
                    // Add the new content to our collection
                    _children.Add(_newChild);

                    // Alert the visual system that there is a new child (this is only a notification)
                    this.AddVisualChild(_newChild);

                    // We need to invalidate our layout information since we have a new child
                    InvalidateMeasure();
                }
            }
            else
            {
                // Don't have content
                _newChild = null;
            }

            // Kick off the transition from old to new
            if (!IsLoaded)
            {
                Loaded += delegate(object sender, RoutedEventArgs args)
                {
                    // Delay until loaded
                    InitiateContentTransition();
                };
            }
            else
            {
                InitiateContentTransition();
            }
        }

        /// <summary>
        /// Activates our transition between old and new content
        /// </summary>
        private void InitiateContentTransition()
        {
           
            if (_newChild != null)
            {
                // Make sure we've laid out this child
                _newChild.UpdateLayout();

                // Create animations
                Storyboard enterStoryboard = new Storyboard();

                if (OpacityEnterAnimation != null)
                {
                    Storyboard.SetTargetProperty(this.OpacityEnterAnimation, new PropertyPath(OpacityProperty));
                    enterStoryboard.Children.Add(this.OpacityEnterAnimation);
                }

                if (IsSlideAnimationEnabled)
                {
                    _newChild.RenderTransform = new TranslateTransform(0, 0);
                    DoubleAnimation slideAnimation = createSlideEnterAnimation(SlideDirection);
                    enterStoryboard.Children.Add(slideAnimation);
                }

                ((FrameworkElement)_newChild).BeginStoryboard(enterStoryboard);
            }

            if (_oldChild != null)
            {
                // Create animations
                Storyboard exitStoryboard = new Storyboard();
                exitStoryboard.FillBehavior = FillBehavior.Stop;

                if (OpacityExitAnimation != null)
                {
                    Storyboard.SetTargetProperty(this.OpacityExitAnimation, new PropertyPath(OpacityProperty));
                    exitStoryboard.Children.Add(this.OpacityExitAnimation);
                }

                if (IsSlideAnimationEnabled)
                {
                    DoubleAnimation slideAnimation = createSlideExitAnimation(SlideDirection);
                    slideAnimation.Completed += new EventHandler(OnTransitionAnimationEnding);

                    exitStoryboard.Children.Add(slideAnimation);
                }

                ((FrameworkElement)_oldChild).BeginStoryboard(exitStoryboard);

                Clock clock = exitStoryboard.CreateClock();

                // Need a handle on the child being animated in order to remove it from the 
                // tree in the delegate below (can't use _oldChild because it will be updated
                // when content is updated -- so if we have multiple animations running we'll
                // remove the wrong file)
                UIElement animatingChild = _oldChild;

                //exitStoryboard.CurrentStateInvalidated += delegate(object sender, EventArgs args)
                {
                    //if (((ClockGroup)sender).CurrentState == ClockState.Stopped)
                    {
                        _children.Remove(animatingChild);
                        this.RemoveVisualChild(animatingChild);
                    }
                }
            }
        }

        /// <summary>
        /// Constructs the slide entrance animation
        /// </summary>
        /// <returns></returns>
        private DoubleAnimation createSlideEnterAnimation(SlideDirection SlideDirection)
        {
            DoubleAnimation slideAnimation = new DoubleAnimation();
            slideAnimation.Duration = SlideDuration;
            slideAnimation.AccelerationRatio = SlideAccelerationRatio;
            slideAnimation.DecelerationRatio = SlideDecelerationRatio;

            if ((SlideDirection == SlideDirection.FromLeft) || (SlideDirection == SlideDirection.FromRight))
            {
                slideAnimation.To = 0;
                slideAnimation.From = (SlideDirection == SlideDirection.FromRight) ? this.RenderSize.Width : -this.RenderSize.Width;
                Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(0).(1)", new DependencyProperty[] { RenderTransformProperty, TranslateTransform.XProperty }));
            }
            else
            {
                slideAnimation.To = 0;
                slideAnimation.From = (SlideDirection == SlideDirection.FromBottom) ? this.RenderSize.Height : -this.RenderSize.Height;
                Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(0).(1)", new DependencyProperty[] { RenderTransformProperty, TranslateTransform.YProperty }));
            }

            return slideAnimation;
        }

        /// <summary>
        /// Constructs the slide exit animation
        /// </summary>
        /// <returns></returns>
        private DoubleAnimation createSlideExitAnimation(SlideDirection SlideDirection)
        {
            DoubleAnimation slideAnimation = new DoubleAnimation();
            slideAnimation.Duration = SlideDuration;
            slideAnimation.AccelerationRatio = SlideAccelerationRatio;
            slideAnimation.DecelerationRatio = SlideDecelerationRatio;

            if ((SlideDirection == SlideDirection.FromLeft) || (SlideDirection == SlideDirection.FromRight))
            {
                slideAnimation.To = (SlideDirection == SlideDirection.FromRight) ? -this.RenderSize.Width : this.RenderSize.Width;
                Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(0).(1)", new DependencyProperty[] { RenderTransformProperty, TranslateTransform.XProperty }));
            }
            else
            {
                slideAnimation.To = (SlideDirection == SlideDirection.FromBottom) ? -this.RenderSize.Height : this.RenderSize.Height;
                Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(0).(1)", new DependencyProperty[] { RenderTransformProperty, TranslateTransform.YProperty }));
            }

            return slideAnimation;
        }

        #region Layout Functions
        /// <summary>
        /// Override the Measure layout pass since we have more than one child
        /// </summary>
        /// <param name="constraint">Maximum size available</param>
        /// <returns>Maximum of the DesiredSize values of our children</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size maxSize = Size.Empty;

            foreach (UIElement child in _children)
            {
                child.Measure(constraint);
                maxSize = new Size(Math.Max(maxSize.Width, child.DesiredSize.Width), Math.Max(maxSize.Height, child.DesiredSize.Height));
            }

            return maxSize;
        }

        /// <summary>
        /// Override the Arrange layout pass -- we arrange all children at arrangeBounds
        /// </summary>
        /// <param name="arrangeBounds">Space assigned to this element</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            foreach (UIElement child in _children)
            {
                child.Arrange(new Rect(arrangeBounds));
            }

            return arrangeBounds;
        }
        #endregion

        #region Visual Management Functions
        /// <summary>
        /// If we're holding on to temporary children for animation, we
        /// must update the number of children
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return _children.Count;
                // return base.VisualChildrenCount;
            }
        }

        /// <summary>
        /// Since we're mucking with our visual children (keeping temporary ones
        /// for our animation), we must override this method in order to return
        /// our temporary children
        /// </summary>
        /// <param name="index">Index of child requested</param>
        /// <returns>Visual for that index</returns>
        protected override Visual GetVisualChild(int index)
        {
            return _children[index];
            //return base.GetVisualChild(index);
        }
        #endregion

        #region Private Members
        private List<UIElement> _children;
        private UIElement _oldChild, _newChild;
        #endregion
    }

    /// <summary>
    /// Tells the frame which direction to slide content
    /// </summary>
    public enum SlideDirection
    {
        FromLeft,
        FromRight,
        FromTop,
        FromBottom,
    }
}
