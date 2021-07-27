using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public partial struct VoxRaw
    {
        public IVoxReader GetReader()
        {
            return new Reader(this);
        }

        public class Reader : IVoxReader
        {
            public readonly VoxRaw Vox;

            public int3 VoxelsSize => Vox.Voxels.Size;
            public int PaletteSize => Vox.Palette?.Length ?? -1;

            public Reader(VoxRaw vox)
            {
                Vox = vox;
            }

            public byte GetVoxelColor(int3 position)
            {
                return Vox.Voxels[position];
            }

            public Color GetPaletteColor(int index)
            {
                return Vox.Palette[index];
            }
        }
    }
}