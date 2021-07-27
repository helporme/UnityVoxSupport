namespace VoxSupport.Utils
{
    public static class NormalIndices
    {
        public const byte None = 0;
        public const byte Right = 1 << 0;
        public const byte Left = 1 << 1;
        public const byte Up = 1 << 2;
        public const byte Down = 1 << 3;
        public const byte Forward = 1 << 4;
        public const byte Backward = 1 << 5;

        public const byte All = Right | Left | Up | Down | Forward | Backward;

        public const byte Pos = Right | Up | Forward;
        public const byte Neg = Left | Down | Backward;

        public static readonly byte[] Rev = {1, 0, 3, 2, 5, 4};
    }
}