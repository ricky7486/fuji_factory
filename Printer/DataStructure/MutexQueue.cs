using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PrinterCenter.Printer
{
    public class MutexQueue<T>
    {
        public MutexQueue()
        {
            mut = new Mutex();
        }
        ~MutexQueue()
        {
            mut.Dispose();
        }
        private readonly Queue<T> queue = new Queue<T>();
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

        public virtual void Enqueue(T item)
        {
            mut.WaitOne();
            queue.Enqueue(item);
            mut.ReleaseMutex();
        }

        public virtual T Dequeue()
        {
            mut.WaitOne();
            T item = queue.Dequeue();
            mut.ReleaseMutex();
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
