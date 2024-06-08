using System.Runtime.InteropServices;

namespace Atlas.Utils
{
    internal unsafe ref struct UnsafeStructArray<T> where T : struct
    {
        private T* _items;
        private int _count;
        private int _capacity;

        public UnsafeStructArray(int capacity)
        {
            _capacity = capacity;
            _items = (T*)Marshal.AllocHGlobal(sizeof(T) * _capacity);
            _count = 0;
        }

        public int Count => _count;
        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new IndexOutOfRangeException();
                }

                return ref _items[index];
            }
        }

        public void Add(T item)
        {
            if (_count >= _capacity)
            {
                Resize();
            }

            _items[_count++] = item;
        }

        private void Resize()
        {
            int newSize = _capacity + 4;
            T* newArray = (T*)Marshal.AllocHGlobal(sizeof(T) * newSize);

            for (int i = 0; i < _capacity; i++)
            {
                newArray[i] = _items[i];
            }

            Marshal.FreeHGlobal((IntPtr)_items);
            _items = newArray;
            _capacity = newSize;
        }

        public void Clear()
        {
            _count = 0;
        }

        public void Dispose()
        {
            if (_items != null)
            {
                Marshal.FreeHGlobal((IntPtr)_items);
                _items = null;
            }
        }
    }
}
