using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMarcoPolo.Utilities
{
    public class CyclicList<T> : IList<T>
    {
        private List<T> items;
        private int idx;
        public int Index { get { return idx; } }
        public T Value
        {
            get { return items[idx % items.Count]; }
            set { items[idx % items.Count] = value; }
        }

        public T this[int index]
        {
            get { return items[index % items.Count]; }
            set { items[index % items.Count] = value; }
        }

        public int Count
        {
            get { return items.Count; }
        }
        public bool IsReadOnly { get { return false; } }
        public CyclicList()
        {
            items = new List<T>();
        }

        public void Next()
        {
            idx = (idx == items.Count) ? 0 : Math.Min(idx++,items.Count); //Just incase only 1 item
        }

        public T PeekNext()
        {
            return this[(idx == items.Count) ? 0 : Math.Min(idx+1,items.Count)];
        }

        public void Previous()
        {
            idx = (idx == 0) ? idx-- : items.Count;
        }

        public T PeekPrevious()
        {
            return this[(idx == 0) ? idx-1 : items.Count];
        }
   

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddLast(T item)
        {
            Add(item);
        }
        public void Add(T item)
        {
            items.Add(item);
        }
        public void AddFirst(T item)
        {
            Insert(0, item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        public bool RemoveLast()
        {
            try
            {
                items.RemoveAt(items.Count);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveFirst()
        {
            try
            {
                items.RemoveAt(0);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            items.Insert(index % items.Count, item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index % items.Count);
        }


        public void Reset()
        {
            idx = 0;
        }
    }
}
