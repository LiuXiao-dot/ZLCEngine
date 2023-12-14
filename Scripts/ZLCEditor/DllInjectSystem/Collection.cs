/*using System;
using System.Collections;
using System.Collections.Generic;
namespace ZLCEditor.DllInjectSystem
{
    public class Collection<T> :
        IList<T>,
        ICollection<T>,
        IEnumerable<T>,
        IEnumerable,
        IList,
        ICollection
    {
        internal T[] items;
        internal int size;
        private int version;

        public Collection()
        {
            items = Empty<T>.Array;
        }

        public Collection(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException();
            items = capacity == 0 ? Empty<T>.Array : new T[capacity];
        }

        public Collection(ICollection<T> items)
        {
            this.items = items != null ? new T[items.Count] : throw new ArgumentNullException(nameof(items));
            items.CopyTo(this.items, 0);
            size = this.items.Length;
        }

        public int Capacity
        {
            get {
                return items.Length;
            }
            set {
                if (value < 0 || value < size)
                    throw new ArgumentOutOfRangeException();
                Resize(value);
            }
        }

        bool IList.IsFixedSize
        {
            get {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get {
                return false;
            }
        }

        object IList.this[int index]
        {
            get {
                return this[index];
            }
            set {
                CheckIndex(index);
                try {
                    this[index] = (T)value;
                    return;
                }
                catch (InvalidCastException) {
                }
                catch (NullReferenceException) {
                }
                throw new ArgumentException();
            }
        }

        int ICollection.Count
        {
            get {
                return Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get {
                return this;
            }
        }

        int IList.Add(object value)
        {
            try {
                Add((T)value);
                return size - 1;
            }
            catch (InvalidCastException) {
            }
            catch (NullReferenceException) {
            }
            throw new ArgumentException();
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return ((IList)this).IndexOf(value) > -1;
        }

        int IList.IndexOf(object value)
        {
            try {
                return IndexOf((T)value);
            }
            catch (InvalidCastException) {
            }
            catch (NullReferenceException) {
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            CheckIndex(index);
            try {
                Insert(index, (T)value);
                return;
            }
            catch (InvalidCastException) {
            }
            catch (NullReferenceException) {
            }
            throw new ArgumentException();
        }

        void IList.Remove(object value)
        {
            try {
                Remove((T)value);
            }
            catch (InvalidCastException) {
            }
            catch (NullReferenceException) {
            }
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Array.Copy(items, 0, array, index, size);
        }

        public int Count
        {
            get {
                return size;
            }
        }

        public T this[int index]
        {
            get {
                return index < size ? items[index] : throw new ArgumentOutOfRangeException();
            }
            set {
                CheckIndex(index);
                if (index == size)
                    throw new ArgumentOutOfRangeException();
                OnSet(value, index);
                items[index] = value;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get {
                return false;
            }
        }

        public void Add(T item)
        {
            if (size == items.Length)
                Grow(1);
            OnAdd(item, size);
            items[size++] = item;
            ++version;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(items, item, 0, size);
        }

        public void Insert(int index, T item)
        {
            CheckIndex(index);
            if (size == items.Length)
                Grow(1);
            OnInsert(item, index);
            Shift(index, 1);
            items[index] = item;
            ++version;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= size)
                throw new ArgumentOutOfRangeException();
            OnRemove(items[index], index);
            Shift(index, -1);
            ++version;
        }

        public bool Remove(T item)
        {
            int num = IndexOf(item);
            if (num == -1)
                return false;
            OnRemove(item, num);
            Shift(num, -1);
            ++version;
            return true;
        }

        public void Clear()
        {
            OnClear();
            Array.Clear(items, 0, size);
            size = 0;
            ++version;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(items, 0, array, arrayIndex, size);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public T[] ToArray()
        {
            T[] destinationArray = new T[size];
            Array.Copy(items, 0, destinationArray, 0, size);
            return destinationArray;
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index > size)
                throw new ArgumentOutOfRangeException();
        }

        private void Shift(int start, int delta)
        {
            if (delta < 0)
                start -= delta;
            if (start < size)
                Array.Copy(items, start, items, start + delta, size - start);
            size += delta;
            if (delta >= 0)
                return;
            Array.Clear(items, size, -delta);
        }

        protected virtual void OnAdd(T item, int index)
        {
        }

        protected virtual void OnInsert(T item, int index)
        {
        }

        protected virtual void OnSet(T item, int index)
        {
        }

        protected virtual void OnRemove(T item, int index)
        {
        }

        protected virtual void OnClear()
        {
        }

        internal virtual void Grow(int desired)
        {
            int val2 = size + desired;
            if (val2 <= items.Length)
                return;
            Resize(Math.Max(Math.Max(items.Length * 2, 4), val2));
        }

        protected void Resize(int new_size)
        {
            if (new_size == size)
                return;
            items = new_size >= size ? items.Resize(new_size) : throw new ArgumentOutOfRangeException();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IDisposable, IEnumerator<T>, IEnumerator
        {
            private Collection<T> collection;
            private int next;
            private readonly int version;

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get {
                    CheckState();
                    if (next <= 0)
                        throw new InvalidOperationException();
                    return Current;
                }
            }

            internal Enumerator(Collection<T> collection)
                : this()
            {
                this.collection = collection;
                version = collection.version;
            }

            public bool MoveNext()
            {
                CheckState();
                if (next < 0)
                    return false;
                if (next < collection.size) {
                    Current = collection.items[next++];
                    return true;
                }
                next = -1;
                return false;
            }

            public void Reset()
            {
                CheckState();
                next = 0;
            }

            private void CheckState()
            {
                if (collection == null)
                    throw new ObjectDisposedException(GetType().FullName);
                if (version != collection.version)
                    throw new InvalidOperationException();
            }

            public void Dispose()
            {
                collection = null;
            }
        }
    }
}*/