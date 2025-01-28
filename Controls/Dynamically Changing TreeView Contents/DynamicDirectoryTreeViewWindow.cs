//---------------------------------------------------------
// DynamicDirectoryTreeView.cs (c) 2006 by Charles Petzold
//---------------------------------------------------------
using System;
using System.IO;
using System.Windows;

namespace Petzold.DynamicDirectoryTreeView
{
    public partial class DynamicDirectoryTreeViewWindow : Window
    {
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new DynamicDirectoryTreeViewWindow());
        }
        public DynamicDirectoryTreeViewWindow()
        {
            InitializeComponent();

            // Create a DiskDirectory object for the system drive.
            DiskDirectory dir = new DiskDirectory(
                new DirectoryInfo(
                    Path.GetPathRoot(Environment.SystemDirectory)));

            // Set it in the TreeView.
            treevue.Items.Add(dir);
        }
    }
}
