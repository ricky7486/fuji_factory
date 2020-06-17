using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PrinterCenter.File
{
    #region CDirectoryWatcher
    public class DirectoryWatcher
    {
        private FileSystemWatcher watcher;
       
  
        public DirectoryWatcher(string path, string filter = "")
        {
        
            watcher = new FileSystemWatcher(path);

            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;


            watcher.Filter = filter;

        }
        /// <summary>
        /// after add event handler into this wrapper class , then you can listen
        /// </summary>
        public void BeginWatching()
        {
            if (watcher != null)
                watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// call it when you want to stop wathing
        /// </summary>
        public void StopWatching()
        {
            if (watcher != null)
                watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Add File Changed Event Handler
        /// </summary>
        /// <param name="OnChanged"></param>
        public void AddOnChangedEventHandler(FileSystemEventHandler OnChanged)
        {
            watcher.Changed += new FileSystemEventHandler(OnChanged);
        }
        /// <summary>
        /// Add File Created Event Handler
        /// </summary>
        /// <param name="OnCreated"></param>
        public void AddOnCreatedEventHandler(FileSystemEventHandler OnCreated)
        {
            watcher.Created += new FileSystemEventHandler(OnCreated);
        }
        /// <summary>
        /// Add File Deleted Event Handler
        /// </summary>
        /// <param name="OnDeleted"></param>
        public void AddOnDeletedEventHandler(FileSystemEventHandler OnDeleted)
        {
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        }
        /// <summary>
        /// Add File Disposed Event Handler
        /// </summary>
        /// <param name="OnDisposed"></param>
        public void AddOnDisposedEventHandler(EventHandler OnDisposed)
        {
            watcher.Disposed += new EventHandler(OnDisposed);
        }
        /// <summary>
        /// Add Error Event Handler
        /// </summary>
        /// <param name="OnError"></param>
        public void AddOnErrorEventHandler(ErrorEventHandler OnError)
        {
            watcher.Error += new ErrorEventHandler(OnError);
        }
        /// <summary>
        /// Add File Renamed Event Handler
        /// </summary>
        /// <param name="OnRenamed"></param>
        public void AddOnRenamedEventHandler(RenamedEventHandler OnRenamed)
        {
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }


    }
    /*
        // Define the event handlers in your code and add them into this class thought public event wrapper functions
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed

        }
    */
    #endregion
}
