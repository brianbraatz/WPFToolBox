#region Using directives
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
#endregion

namespace RD.Controls.ExpanderSample
{
  public class MatrixTextBlock : TextBlock
  {
    #region Fields
    private string _originalText;
    private char[] _chars;
    private int _duration;
    private Storyboard _typewriterStoryboard;
    #endregion

    #region Dependency properties
    public static readonly DependencyProperty WritingSpeedProperty;
    public static readonly DependencyProperty WaitForProperty;
    public static readonly DependencyProperty ImmediateWriteProperty;
    #endregion

    #region Ctors
    static MatrixTextBlock()
    {
      WritingSpeedProperty = DependencyProperty.Register("WritingSpeed", typeof(int), typeof(MatrixTextBlock), new FrameworkPropertyMetadata(50));
      WaitForProperty = DependencyProperty.Register("WaitFor", typeof(TimeSpan), typeof(MatrixTextBlock), new FrameworkPropertyMetadata(new TimeSpan(0)));
      ImmediateWriteProperty = DependencyProperty.Register("ImmediateWrite", typeof(bool), typeof(MatrixTextBlock), new FrameworkPropertyMetadata(true));
    }
    public MatrixTextBlock()
    {
    }
    #endregion

    #region Properties
    public int WritingSpeed
    {
      get { return (int)base.GetValue(WritingSpeedProperty); }
      set { base.SetValue(WritingSpeedProperty, value); }
    }
    public TimeSpan WaitFor
    {
      get { return (TimeSpan)base.GetValue(WaitForProperty); }
      set { base.SetValue(WaitForProperty, value); }
    }
    public bool ImmediateWrite
    {
      get { return (bool)base.GetValue(ImmediateWriteProperty); }
      set { base.SetValue(ImmediateWriteProperty, value); }
    }
    #endregion

    #region Methods
    public override void BeginInit()
    {
      base.BeginInit();
      this.Name = "PART_MatrixTextBlockFX";
        NameScope.SetNameScope(this, new NameScope());
        this.RegisterName(this.Name, this);
    }
    protected override void OnInitialized(EventArgs e)
    {
      if (!string.IsNullOrEmpty(this.Name))
      {
        NameScope.SetNameScope(this, new NameScope());
        this.RegisterName(this.Name, this);
      }

      _typewriterStoryboard = new Storyboard();
      _typewriterStoryboard.Completed += new EventHandler(_OnTypeWriteCompleted);

      this.Text = this.Text.ToUpper();
      _chars = this.Text.ToCharArray();
      this._InitTypeWriting();

      _originalText = this.Text;
      this.Text = string.Empty;

      base.OnInitialized(e);
    }
    public override void EndInit()
    {
      base.EndInit();
      if(ImmediateWrite) this.TypeWrite();
    }
    public void TypeWrite()
    {
      this.Text = string.Empty;
      this.BeginStoryboard(_typewriterStoryboard);
    }
    private void _InitTypeWriting()
    {
      _duration = this.WritingSpeed * _chars.Length;

      _typewriterStoryboard.Children.Clear();

      StringAnimationUsingKeyFrames __frames = new StringAnimationUsingKeyFrames();
      __frames.FillBehavior = FillBehavior.Stop;
      __frames.BeginTime = this.WaitFor;
      __frames.Duration = new System.Windows.Duration(new TimeSpan(0, 0, 0, 0, _duration));
      Storyboard.SetTargetProperty(__frames, new System.Windows.PropertyPath(TextBlock.TextProperty));
      Storyboard.SetTargetName(__frames, this.Name);

      for (int i = 1; i < _chars.Length + 1; i++)
      {
        string __text = (i == _chars.Length) ? this.Text.Substring(0, i) : this.FormatString(this.Text.Substring(0, i));
        DiscreteStringKeyFrame __frame = new DiscreteStringKeyFrame(__text, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,(this.WritingSpeed * i))));
        __frames.KeyFrames.Add(__frame);
      }
      _typewriterStoryboard.Children.Add(__frames);
    }
    private void _OnTypeWriteCompleted(object sender, EventArgs e)
    {
      this.Text = _originalText;
    }
    protected virtual string FormatString(string inputString)
    {
      return string.Format("{0}_", inputString);
    }
    #endregion
  }
}
