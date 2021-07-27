using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public partial struct VoxelFace
    {
        public void CalculateCorners()
        {
            int3 right = NormalVectors.RightVectors[FaceIndex];
            int3 up = NormalVectors.UpVectors[FaceIndex];
            
            for (int cornerIndex = 0; cornerIndex < 4; cornerIndex++)
            {
                int2 dir = FaceCorners.CornerDirections[cornerIndex];
                int cornerVertIndex = CalculateCornerVertex(right * dir.x, up * dir.y);
                
                if (cornerVertIndex != -1)
                {
                    Voxel.Corners |= 1 << (FaceIndex * 4 + cornerIndex);
                    Corners |= 1 << cornerIndex;
                }
                
                CornerVertIndices[cornerIndex] = cornerVertIndex;
            }
            
            Voxel.CornerVertIndices[FaceIndex] = CornerVertIndices;
        }

        private int CalculateCornerVertex(int3 right, int3 up)
        {
            int3 diag0 = Pos + right;
            int3 diag1 = Pos + up;
            int3 perp = Pos + right + up;

            bool diag0Empty = IsFaceEmpty(diag0);
            bool diag1Empty = IsFaceEmpty(diag1);
            bool perpEmpty = IsFaceEmpty(perp);
            
            if (!diag0Empty && !diag1Empty && perpEmpty || diag0Empty && diag1Empty)
            {
                return SetCornerVertex(right, up);
            }
            
            return -1;
        }

        private bool IsFaceEmpty(int3 pos)
        {
            if (Mesh.Voxels.InBounds(pos))
            {
                Voxel faceSideVoxel = Mesh.Voxels[pos];
                return faceSideVoxel.Color != Voxel.Color ||
                       (faceSideVoxel.Normals & 1 << FaceIndex) == NormalIndices.None;
            }
            return true;
        }

        private int SetCornerVertex(int3 right, int3 up)
        {
            float3 forward = NormalVectors.ForwardVectors[FaceIndex];
            float3 offset = right + up + forward;
            float3 vertex = Pos + offset / 2f - Mesh.Size / 2;

            int index = Mesh.Vertices.Count;
            Mesh.Vertices.Add(vertex);
            Mesh.Normals.Add(forward);
            Mesh.UVs.Add(new Vector2((Voxel.Color - 1) / 256f + 1 / 512f, 0.5f));
            Mesh.Links.Add(-1);
            
            return index;
        }
    }
}