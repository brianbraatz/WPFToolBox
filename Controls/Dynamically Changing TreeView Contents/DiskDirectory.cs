//----------------------------------------------
// DiskDirectory.cs (c) 2006 by Charles Petzold
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Petzold.DynamicDirectoryTreeView
{
    public class DiskDirectory: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        DirectoryInfo dirinfo;

        // Constructor requires DirectoryInfo object.
        public DiskDirectory(DirectoryInfo dirinfo)
        {
            this.dirinfo = dirinfo;

            FileSystemWatcher watch = 
                new FileSystemWatcher(dirinfo.FullName);
            watch.Created += WatcherOnEvent;
            watch.Deleted += WatcherOnEvent;
            watch.Renamed += WatcherOnEvent;

            try
            {
                watch.EnableRaisingEvents = true;
            }
            catch
            {
            }
        }

        // Name property returns directory name.
        public string Name
        {
            get { return dirinfo.Name; }
        }

        // Subdirectories property returns collection of DiskDirectory objects.
        public List<DiskDirectory> Subdirectories
        {
            get
            {
                List<DiskDirectory> dirs = new List<DiskDirectory>();
                DirectoryInfo[] subdirs;

                try
                {
                    subdirs = dirinfo.GetDirectories();
                }
                catch
                {
                    return dirs;
                }

                foreach (DirectoryInfo subdir in subdirs)
                    dirs.Add(new DiskDirectory(subdir));

                return dirs;
            }
        }
        void WatcherOnEvent(object sender, FileSystemEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, 
                    new PropertyChangedEventArgs("Subdirectories"));
        }
    }
}
