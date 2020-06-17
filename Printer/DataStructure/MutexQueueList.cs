using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PrinterCenter.Printer
{
    /// <summary>
    /// 用於Printer給的檔案
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MutexQueueList<T>
    {
        //private IniFile _iniFile = new IniFile();
        //static bool _IsDequeueDeleteFile;
        //private string Folder;
        public MutexQueueList()/*(string folder)*/
        {
            mut = new Mutex();
            //Folder = folder;
            //Boolean.TryParse(_iniFile.Read("FileQueue", "DequeueDelete"), out _IsDequeueDeleteFile);
        }
        ~MutexQueueList()
        {
            mut.Dispose();
        }
        private readonly List<T> queue = new List<T>();
        private Mutex mut;

        public int Count
        {
            get
            {
                int count;
                mut.WaitOne();
                count = queue.Count;
                mut.ReleaseMutex();
                return count;

            }
        }
        public T this[int i]
        {
            get
            {
                return queue[i];
            }
            set
            {
                queue[i] = value;
            }
        }
        public virtual void Enqueue(T item)
        {
            mut.WaitOne();
            queue.Add(item);
            mut.ReleaseMutex();
        }

        public virtual T Dequeue()
        {
            mut.WaitOne();
            T item = queue[0];
            queue.RemoveAt(0);
            mut.ReleaseMutex();
            //if (_IsDequeueDeleteFile)
            //    System.IO.File.Delete( Folder+item.ToString());
            return item;
        }

        public virtual void Clear()
        {
            mut.WaitOne();
            queue.Clear();
            mut.ReleaseMutex();
        }


    }
    
}
