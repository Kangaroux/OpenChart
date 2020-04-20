using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenChart.Charting
{
    /// <summary>
    /// A generic list class for storing beat objects. The objects are stored in
    /// ascending order by their beat, and no two objects can occur on the same beat.
    /// </summary>
    public class BeatObjectList<T> : ICollection<T>, IChangeNotifier where T : class, IBeatObject
    {
        /// <summary>
        /// Event arguments for adding and removing objects from a ObjectList.
        /// </summary>
        public class ObjectListEventArgs : EventArgs
        {
            /// <summary>
            /// The object that was added/removed.
            /// </summary>
            public readonly T Object;

            public ObjectListEventArgs(T obj)
            {
                Object = obj;
            }
        }

        LinkedList<T> objects;

        /// <summary>
        /// Returns the number of objects in the list.
        /// </summary>
        public int Count => objects.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// An event for when an object is added to the list.
        /// </summary>
        public event EventHandler Added;

        /// <summary>
        /// An event for when an object object is modified (e.g. its beat or value changed).
        /// This event is mostly for the convenience of not having to track every object yourself.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// An event for when the list is completely cleared.
        /// </summary>
        public event EventHandler Cleared;

        /// <summary>
        /// An event for when an object is removed from the list.
        /// </summary>
        public event EventHandler Removed;

        public BeatObjectList()
        {
            objects = new LinkedList<T>();
        }

        /// <summary>
        /// Adds an object to the list. Multiple object changes cannot occur on the same beat.
        /// </summary>
        public void Add(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }

            insertObject(obj);
            OnAdded(obj);
        }

        /// <summary>
        /// Adds multiple objects to the list. This is far more efficient than adding each
        /// object individually.
        /// </summary>
        public void Add(T[] objs)
        {
            // Inserting into linked lists is usually pretty slow since it's O(n), but we can
            // speed things up by taking advantage of the fact that our list is sorted by beats.
            // We can sort the object array by beats and then reuse the LinkedListNode cursor
            // so we don't need to start at the beginning of the list every time.
            var query = from obj in objs
                        orderby obj.Beat.Value
                        select obj;

            var cur = objects.First;

            foreach (var obj in query)
            {
                cur = insertObject(obj, cur);

                // FIXME?: When inserting a large amount of objects at once (such as by copy and pasting)
                // calling an event for each individual object may lead to performance issues.
                OnAdded(obj);
            }
        }

        /// <summary>
        /// Clears all objects from the list.
        /// </summary>
        public void Clear()
        {
            objects.Clear();
            OnCleared();
        }

        /// <summary>
        /// Returns true if the object exists in the list.
        /// </summary>
        public bool Contains(T obj)
        {
            if (obj == null)
            {
                return false;
            }

            return objects.Find(obj) != null;
        }

        /// <summary>
        /// Copies the object list to an array.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            objects.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes an object at the given beat. Returns true if the object exists and was removed.
        /// </summary>
        public bool Remove(Beat beat)
        {
            if (beat == null)
            {
                throw new ArgumentNullException("Beat cannot be null.");
            }

            var cur = objects.First;

            while (cur != null)
            {
                if (cur.Value.Beat.Value == beat.Value)
                {
                    var bpm = cur.Value;
                    objects.Remove(cur);
                    OnRemoved(bpm);
                    return true;
                }

                cur = cur.Next;
            }

            return false;
        }

        /// <summary>
        /// Removes the given object.
        /// </summary>
        public bool Remove(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }
            else if (objects.Remove(obj))
            {
                OnRemoved(obj);
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        /// <summary>
        /// Returns an array of the objects, in ascending order sorted by beats.
        /// </summary>
        public T[] ToArray()
        {
            var array = new T[Count];

            CopyTo(array, 0);

            return array;
        }

        /// <summary>
        /// Inserts an object into the list such that the list is sorted in ascending
        /// order by beat.
        /// </summary>
        /// <param name="cur">The starting point for the cursor. Defaults to the start of the list.</param>
        private LinkedListNode<T> insertObject(T obj, LinkedListNode<T> cur = null)
        {
            if (cur == null)
            {
                cur = objects.First;
            }

            while (cur != null)
            {
                if (obj.Beat.Value == cur.Value.Beat.Value)
                {
                    throw new ArgumentException("An object at the given beat already exists.");
                }
                else if (obj.Beat.Value < cur.Value.Beat.Value)
                {
                    if (cur.Previous == null || cur.Previous.Value.Beat.Value < obj.Beat.Value)
                    {
                        obj.CheckValid(cur.Previous?.Value, cur.Next?.Value);

                        return objects.AddBefore(cur, obj);
                    }
                }

                cur = cur.Next;
            }

            obj.CheckValid(objects.Last?.Value, null);

            return objects.AddLast(obj);
        }

        /// <summary>
        /// This method is called when an object in the list is modified.
        /// </summary>
        private void onObjectChanged(object o, EventArgs e)
        {
            var obj = (T)o;
            var node = objects.Find(obj);

            if ((node.Previous != null && node.Previous.Value.Beat.Value >= obj.Beat.Value)
            || (node.Next != null && node.Next.Value.Beat.Value <= obj.Beat.Value))
            {
                // If the object's beat changed and it messed up the ordering, remove it from
                // the list and reinsert it.
                objects.Remove(node);
                insertObject(obj);
            }

            OnChanged(obj);
        }

        protected virtual void OnAdded(T obj)
        {
            if (obj is IChangeNotifier eventObj)
            {
                eventObj.Changed += onObjectChanged;
            }

            var handler = Added;
            handler?.Invoke(this, new ObjectListEventArgs(obj));
        }

        protected virtual void OnChanged(T obj)
        {
            var handler = Changed;
            handler?.Invoke(this, new ObjectListEventArgs(obj));
        }

        protected virtual void OnCleared()
        {
            var handler = Cleared;
            handler?.Invoke(this, null);
        }

        protected virtual void OnRemoved(T obj)
        {
            if (obj is IChangeNotifier eventObj)
            {
                eventObj.Changed -= onObjectChanged;
            }

            var handler = Removed;
            handler?.Invoke(this, new ObjectListEventArgs(obj));
        }
    }
}
