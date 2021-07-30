using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace VoxSupport
{
    public readonly struct Flatten3DArray<T> : IEnumerable<T>
    {
        public T this[int x, int y, int z]
        {
            get => _values[z * Size.x * Size.y + y * Size.x + x];
            set => _values[z * Size.x * Size.y + y * Size.x + x] = value;
        }

        public T this[int3 p]
        {
            get => _values[p.z * Size.x * Size.y + p.y * Size.x + p.x];
            set => _values[p.z * Size.x * Size.y + p.y * Size.x + p.x] = value;
        }

        public T this[int i]
        {
            get => _values[i];
            set => _values[i] = value;
        }

        public readonly int3 Size;
        public readonly int Length;

        private readonly T[] _values;

        public Flatten3DArray(int3 size)
        {
            Size = size;
            Length = size.x * size.y * size.z;
            _values = new T[Length];
        }

        public bool InBounds(int3 p)
        {
            return p.x >= 0 && p.x < Size.x && p.y >= 0 && p.y < Size.y && p.z >= 0 && p.z < Size.z;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public void CopyTo(Flatten3DArray<T> outArray)
        {
            int3 size = math.min(Size, outArray.Size);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        outArray[x, y, z] = this[x, y, z];
                    }
                }
            }
        }
    }
}