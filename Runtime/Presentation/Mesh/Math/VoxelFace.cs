using Unity.Mathematics;

namespace VoxSupport
{
    public partial struct VoxelFace
    {
        public readonly VoxMesh Mesh;
        
        public Voxel Voxel;
        public int Corners;
        public readonly int3 Pos;
        
        public readonly int FaceIndex;
        public readonly int3 Axises;
        public readonly int[] CornerVertIndices;
        
        public VoxelFace(VoxMesh mesh, Voxel voxel, int3 pos, int faceIndex)
        {
            Mesh = mesh;
            
            Voxel = voxel;
            Corners = 0;
            Pos = pos;
            
            FaceIndex = faceIndex;
            Axises = NormalVectors.Axises[faceIndex];
            CornerVertIndices = new int[4];
        }
    }
}