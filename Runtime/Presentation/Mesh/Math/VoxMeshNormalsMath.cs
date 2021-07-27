using Unity.Mathematics;
using VoxSupport.Utils;

namespace VoxSupport
{
    public partial struct VoxMesh
    {
        public void CalculateNormals()
        {
            foreach (int3 pos in Voxels.Positions)
            {
                if (Voxels.InBounds(pos) && Voxels[pos].Color != 0)
                {
                    CalculateVoxelNormals(pos);
                }
            }
        }

        private void CalculateVoxelNormals(int3 pos)
        {
            Voxel voxel = Voxels[pos];

            for (int face = 0; face < 6; face++)
            {
                int3 neighbourPos = pos + NormalVectors.ForwardVectors[face];

                if (!Voxels.InBounds(neighbourPos))
                {
                    voxel.Normals |= (byte) (1 << face);
                }
                else
                {
                    Voxel neighbour = Voxels[neighbourPos];
                    if (neighbour.Color == 0)
                    {
                        voxel.Normals |= (byte) (1 << face);
                    }
                    else
                    {
                        neighbour.Normals &= (byte) ~(1 << NormalIndices.Rev[face]);
                        Voxels[neighbourPos] = neighbour;
                    }
                }
            }

            Voxels[pos] = voxel;
        }
    }
}