using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public partial struct VoxRaw
    {
        public IVoxBuilder GetBuilder()
        {
            return new Builder(this);
        }

        public class Builder : IVoxBuilder
        {
            public IVox Vox => _vox;
            private VoxRaw _vox;

            public Builder(VoxRaw vox)
            {
                _vox = vox;
            }

            public void SetVoxelsGridSize(int3 size)
            {
                _vox.Voxels = new Flatten3DArray<byte>(size);
            }

            public void SetPaletteSize(int size)
            {
                _vox.Palette = new Color[size];
            }

            public void SetVoxelColor(int3 position, byte color)
            {
                _vox.Voxels[position] = color;
            }

            public void SetPaletteColor(int index, Color color)
            {
                _vox.Palette[index] = color;
            }
        }
    }
}