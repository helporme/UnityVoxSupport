using Unity.Mathematics;

namespace VoxSupport.Utils
{
    public static class NormalVectors
    {
        public static readonly int3 Right = new int3(1, 0, 0);
        public static readonly int3 Left = new int3(-1, 0, 0);
        public static readonly int3 Up = new int3(0, 1, 0);
        public static readonly int3 Down = new int3(0, -1, 0);
        public static readonly int3 Forward = new int3(0, 0, 1);
        public static readonly int3 Backward = new int3(0, 0, -1);

        public static readonly int3[] ForwardVectors = {Right, Left, Up, Down, Forward, Backward};
        public static readonly int3[] RightVectors = {Forward, Forward, Right, Right, Right, Right};
        public static readonly int3[] UpVectors = {Up, Up, Forward, Forward, Up, Up};

        public static readonly int3[] Axises =
        {
            new int3(2, 1, 0), new int3(2, 1, 0),
            new int3(0, 2, 1), new int3(0, 2, 1),
            new int3(0, 1, 2), new int3(0, 1, 2)
        };
    }
}