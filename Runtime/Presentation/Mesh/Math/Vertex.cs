namespace VoxSupport
{
    public struct Vertex
    {
        public bool IsReflex;
        public int Index;

        public Vertex(int index)
        {
            IsReflex = false;
            Index = index;
        }
    }
}