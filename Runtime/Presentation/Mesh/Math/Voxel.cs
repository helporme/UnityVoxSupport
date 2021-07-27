namespace VoxSupport
{
    public struct Voxel
    {
        public byte Color;
        public byte Normals;
        public int Corners;
        public readonly int[][] CornerVertIndices;

        public Voxel(byte color)
        {
            Color = color;
            Normals = 0;
            Corners = 0;
            CornerVertIndices = new int[6][];
        }
    }
}