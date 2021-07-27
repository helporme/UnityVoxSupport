using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public interface IVoxBuilder
    {
        public IVox Vox { get; }

        void SetVoxelsGridSize(int3 size);

        void SetPaletteSize(int size);

        void SetVoxelColor(int3 position, byte color);

        void SetPaletteColor(int index, Color color);
    }
}