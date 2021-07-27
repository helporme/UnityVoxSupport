using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public readonly partial struct VoxMesh
    {
        public IVoxBuilder GetBuilder()
        {
            return new Builder(this);
        }

        public class Builder : IVoxBuilder
        {
            public IVox Vox => _vox;
            private VoxMesh _vox;

            public Builder(VoxMesh vox)
            {
                _vox = vox;
            }

            public void SetVoxelsGridSize(int3 size)
            {
                _vox = new VoxMesh(size);
            }

            public void SetPaletteSize(int size)
            {
            }

            public void SetVoxelColor(int3 position, byte color)
            {
                _vox.Voxels[position] = new Voxel(color);
            }

            public void SetPaletteColor(int index, Color color)
            {
            }
        }
    }
}