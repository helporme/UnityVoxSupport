using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport.Convert
{
    public static class VoxConverter
    {
        public static TVox Convert<TVox>(this IVox vox) where TVox : IVox, new()
        {
            IVoxBuilder voxBuilder = new TVox().GetBuilder();
            CopyTo(vox.GetReader(), voxBuilder);
            return (TVox) voxBuilder.Vox;
        }

        public static void CopyTo(IVoxReader inputReader, IVoxBuilder outputBuilder)
        {
            int3 voxelGridSize = inputReader.VoxelsSize;
            outputBuilder.SetVoxelsGridSize(voxelGridSize);

            for (int z = 0; z < voxelGridSize.z; z++)
            {
                for (int y = 0; y < voxelGridSize.y; y++)
                {
                    for (int x = 0; x < voxelGridSize.x; x++)
                    {
                        var pos = new int3(x, y, z);
                        byte color = inputReader.GetVoxelColor(pos);
                        outputBuilder.SetVoxelColor(pos, color);
                    }
                }
            }

            int paletteSize = inputReader.PaletteSize;
            outputBuilder.SetPaletteSize(paletteSize);

            for (int i = 0; i < paletteSize; i++)
            {
                Color color = inputReader.GetPaletteColor(i);
                outputBuilder.SetPaletteColor(i, color);
            }
        }
    }
}