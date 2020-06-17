using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrinterCenter.Printer
{
    public delegate void EventQueueHandler<T>(object sender, EventQueueEventArgs<T> e);
    public class EventQueueEventArgs<T> : EventArgs
    {
        public EventQueueEventArgs(T data)
        {
            Data = data;
        }
        public T Data;
    }
    public class EventQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        public event EventQueueHandler<T> EnqueueEvent;
        public event EventQueueHandler<T> DequeueEvent;

        public int Count { get { return queue.Count; } }
        protected virtual void OnEnqueue(T item)
        {
            if (EnqueueEvent != null) EnqueueEvent(this, new EventQueueEventArgs<T>(item));
        }
        public virtual void Enqueue(T item)
        {
            queue.Enqueue(item);
            OnEnqueue(item);
        }

        protected virtual void OnDequeue(T item)
        {
            if (DequeueEvent != null) DequeueEvent(this, new EventQueueEventArgs<T>(item));
        }
        public virtual T Dequeue()
        {
            T item = queue.Dequeue();
            OnDequeue(item);
            return item;
        }

        public virtual void Clear()
        {
            queue.Clear();
        }


    }
}
