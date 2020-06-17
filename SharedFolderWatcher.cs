using PrinterCenter.File;
using PrinterCenter.Log;
using PrinterCenter.Printer;
using PrinterCenter.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PrinterCenter
{
    public class SharedFolderWatcher
    {
        public string  Target { get; set; }
        public eAssignedLane_Printer LaneID { get; set; }
        public DirectoryWatcher Watcher ;
        //public List<string> WatchedFiles = new List<string>();
        //public Queue<string> WatchedFiles = new Queue<string>();//WatchedFiles應該要有mutex
        //public MutexQueue<string> WatchedFiles = new MutexQueue<string>();//WatchedFiles應該要有mutex
        public MutexQueueList<string> WatchedFiles = new MutexQueueList<string>();//WatchedFiles應該要有mutex
        //public MutexFileQueue<string> WatchedFiles ;//WatchedFiles應該要有mutex

        public string Description_of_Target { get; set; }
        public SharedFolderWatcher(string target, eAssignedLane_Printer laneID, string path = "")
        {
            Target = target;
            LaneID = laneID;
            //WatchedFiles = new MutexFileQueue<string>(target);
            WatchedFiles.Clear();
            Watcher = new DirectoryWatcher(target);
            Watcher.AddOnCreatedEventHandler(OnCreateFileInWatchedFolder);
            Watcher.AddOnRenamedEventHandler(OnRenameFileInWatchedFolder);

            if(laneID == eAssignedLane_Printer.Lane1)
            {
                ViewModelLocator.Atom.FlowHostVM.IsLane1WFExist = true;
                ViewModelLocator.Atom.FlowHostVM.Lane1WFDisk = target;
                ViewModelLocator.Atom.FlowHostVM.Lane1WFPath =  path;
            }
                
            else if(laneID == eAssignedLane_Printer.Lane2)
            {
                ViewModelLocator.Atom.FlowHostVM.IsLane2WFExist = true;
                ViewModelLocator.Atom.FlowHostVM.Lane1WFDisk = target;
                ViewModelLocator.Atom.FlowHostVM.Lane2WFPath =  path;
            }
                
            Watcher.BeginWatching();

        }
        private void OnCreateFileInWatchedFolder(object sender, FileSystemEventArgs e)
        {
            //e.FullPath = Z:\\Test.xml
            //e.Name = Test.xml
            WatchedFiles.Enqueue(e.Name);
            Log4.PrinterLogger.InfoFormat("+[{0}] Watched Target:{1} => Create:{2}", LaneID.ToString(),Target,e.Name);
            switch(LaneID)
            {
                case eAssignedLane_Printer.Lane1:// 第一軌
                   
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ViewModelLocator.Atom.FlowHostVM.Lane1WFList.Add(e.Name);
                    }));
            
            
                    break;
                case eAssignedLane_Printer.Lane2:
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                       ViewModelLocator.Atom.FlowHostVM.Lane2WFList.Add(e.Name);
                    }));
                    break;
            }
            
        }

        private void OnRenameFileInWatchedFolder(object sender, RenamedEventArgs e)
        {
            //e.FullPath = Z:\\Test.xml
            //e.Name = Test.xml
            Log4.PrinterLogger.InfoFormat("+[{0}] Watched Target:{1} => Rename:{2} => {3}"
                , LaneID.ToString(), Target
                , Path.GetFileName(e.OldFullPath) //取檔名
                , Path.GetFileName(e.FullPath)); //取檔名
            for(int i=0; i<WatchedFiles.Count; i++)
                if (WatchedFiles[i] == Path.GetFileName(e.OldFullPath))
                    WatchedFiles[i] = Path.GetFileName(e.FullPath);
            switch (LaneID)
            {
                case eAssignedLane_Printer.Lane1:// 第一軌

                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var lst = ViewModelLocator.Atom.FlowHostVM.Lane1WFList;
                        for (int i = 0; i < lst.Count; i++)
                            if (Path.GetFileName(lst[i].Value) == Path.GetFileName(e.OldFullPath))
                                lst[i].Value = Path.GetFileName(e.FullPath);

                    }));


                    break;
                case eAssignedLane_Printer.Lane2:
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var lst = ViewModelLocator.Atom.FlowHostVM.Lane2WFList;
                        for (int i = 0; i < lst.Count; i++)
                            if (Path.GetFileName(lst[i].Value) == Path.GetFileName(e.OldFullPath))
                                lst[i].Value = Path.GetFileName(e.FullPath);
                    }));
                    break;
            }
        }
    }
 
}
