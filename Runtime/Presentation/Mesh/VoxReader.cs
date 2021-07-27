using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public readonly partial struct VoxMesh
    {
        public IVoxReader GetReader()
        {
            return new Reader(this);
        }
        
        public class Reader : IVoxReader
        {
            public int3 VoxelsSize => _vox.Size;
            public int PaletteSize => 0;

            private readonly VoxMesh _vox;

            public Reader(VoxMesh vox)
            {
                _vox = vox;
            }
            
            public byte GetVoxelColor(int3 position)
            {
                return _vox.Voxels[position].Color;
            }

            public Color GetPaletteColor(int index)
            {
                return Color.black;
            }
        }
    }
}