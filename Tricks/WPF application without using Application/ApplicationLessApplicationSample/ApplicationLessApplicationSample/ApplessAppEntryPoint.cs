using System;
using System.Windows;
using System.Windows.Threading;

namespace ApplicationlessApplicationSample {
  public class ApplessAppEntryPoint {

    [STAThread]
    public static void Main() {
      DispatcherOperationCallback callback = new DispatcherOperationCallback(ApplicationStart);
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, callback, null);
      Dispatcher.Run();
    }

    public static object ApplicationStart(object unused) {
      Window window = new Window();
      window.Closed += delegate {
        Dispatcher.CurrentDispatcher.InvokeShutdown();
      };
      window.Show();
      return null;
    }
  }
}
