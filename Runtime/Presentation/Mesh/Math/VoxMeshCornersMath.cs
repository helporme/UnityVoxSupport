using Unity.Mathematics;
using VoxSupport.Utils;

namespace VoxSupport
{
    public partial struct VoxMesh
    {
        public void CalculateCorners()
        {
            for (int x = 0; x < Voxels.Size.x; x++)
            {
                for (int z = 0; z < Voxels.Size.z; z++)
                {
                    for (int y = 0; y < Voxels.Size.y; y++)
                    {
                        var pos = new int3(x, y, z);
                        if (Voxels[pos].Color != 0)
                        {
                            CalculateVoxelCorners(pos);
                        }
                    }
                }
            }
        }

        private void CalculateVoxelCorners(int3 pos)
        {
            Voxel voxel = Voxels[pos];

            if (voxel.Normals == NormalIndices.None)
            {
                return;
            }

            for (int faceIndex = 0; faceIndex < 6; faceIndex++)
            {
                if ((voxel.Normals & (1 << faceIndex)) == NormalIndices.None)
                {
                    continue;
                }

                var face = new VoxelFace(this, voxel, pos, faceIndex);
                face.CalculateCorners();
                face.CalculateLinks();
                voxel = face.Voxel;
            }

            Voxels[pos] = voxel;
        }
    }
}