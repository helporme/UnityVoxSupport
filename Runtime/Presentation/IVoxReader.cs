using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public interface IVoxReader
    {
        int3 VoxelsSize { get; }
        int PaletteSize { get; }

        byte GetVoxelColor(int3 position);

        Color GetPaletteColor(int index);
    }
}