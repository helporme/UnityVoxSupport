using Unity.Mathematics;
using UnityEngine;
using VoxSupport.Collections;

namespace VoxSupport
{
    public partial struct VoxRaw : IVox
    {
        public Flatten3DArray<byte> Voxels;
        public Color[] Palette;
        
        public VoxRaw(int3 size)
        {
            Voxels = new Flatten3DArray<byte>(size);
            Palette = new Color[256];
        }
    }
}