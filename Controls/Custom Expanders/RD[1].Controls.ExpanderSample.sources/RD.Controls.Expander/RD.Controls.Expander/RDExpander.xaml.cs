#region Using directives
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
#endregion

namespace RD.Controls.ExpanderSample
{
  public partial class RDExpander : Expander
  {
    #region Routed commands
    public static readonly RoutedCommand ExpandOrCollapseCommand;
    #endregion

    #region Routed events
    private static readonly RoutedEvent ExpandedUpEvent;
    #endregion

    #region Events
    private event RoutedEventHandler ExpandedUp
    {
      add { base.AddHandler(ExpandedUpEvent, value); }
      remove { base.AddHandler(ExpandedUpEvent, value); }
    }
    #endregion

    #region Ctors
    static RDExpander()
    {
      ExpandedUpEvent = EventManager.RegisterRoutedEvent("ExpandedUp", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RDExpander));
      RDExpander.ExpandOrCollapseCommand = new RoutedCommand("ExpandOrCollapse", typeof(RDExpander));
      CommandManager.RegisterClassCommandBinding(typeof(RDExpander), new CommandBinding(ExpandOrCollapseCommand, new ExecutedRoutedEventHandler(OnExecuteCommand), new CanExecuteRoutedEventHandler(OnQueryExecuteCommand)));
    }
    public RDExpander()
    {
      this.InitializeComponent();
    }
    #endregion

    #region Methods
    private static void OnExecuteCommand(object target, ExecutedRoutedEventArgs e)
    {
      RDExpander __expander = (RDExpander)target;
      if (e.Command == RDExpander.ExpandOrCollapseCommand)
      {
        __expander.IsExpanded = !__expander.IsExpanded;
      }
    }
    private static void OnQueryExecuteCommand(object target, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }
    protected override void OnExpanded()
    {
      switch(base.ExpandDirection)
      {
        case ExpandDirection.Down:
          base.OnExpanded();
          break;

        case ExpandDirection.Up:
          RoutedEventArgs __args = new RoutedEventArgs(ExpandedUpEvent);
          base.RaiseEvent(__args);
          break;
      }
    }
    #endregion
  }
}
