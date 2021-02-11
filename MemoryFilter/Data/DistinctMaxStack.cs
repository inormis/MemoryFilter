using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MemoryFilter.Data {

    [Serializable]
    public class DistinctMaxStack<T> : ICollection<T>, INotifyCollectionChanged {
        #region Constructors

        public DistinctMaxStack(int maxSize, params T[] values) {
            _limit = maxSize;
            _list = new LinkedList<T>(values);
        }

        #endregion

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #region Fields

        private readonly int _limit;
        private readonly LinkedList<T> _list;

        #endregion

        #region Public Stack Implementation

        public void Push(T value) {
            if (_list.Count == _limit) {
                _list.RemoveLast();
            }

            if (Contains(value)) {
                Remove(value);
            }

            _list.AddFirst(value);

            RaiseEvent();
        }

        private void RaiseEvent() {
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(T item) {
            if (Equals(item, null)) {
                return;
            }

            Push(item);
        }

        public void Clear() {
            _list.Clear();
            RaiseEvent();
        }

        public bool Remove(T item) {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly { get; } = false;

        public bool Contains(T value) {
            var result = false;
            if (Count > 0) {
                result = _list.Contains(value);
            }

            return result;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return _list.GetEnumerator();
        }

        public IEnumerator GetEnumerator() {
            return _list.GetEnumerator();
        }

        #endregion
    }

}