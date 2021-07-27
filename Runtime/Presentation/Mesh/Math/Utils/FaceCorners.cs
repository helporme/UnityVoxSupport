
using Unity.Mathematics;

namespace VoxSupport
{
    public static class FaceCorners
    {
        public const byte None = 0;
        public const byte LeftDown = 1 << 0;
        public const byte LeftUp = 1 << 1;
        public const byte RightUp = 1 << 2;
        public const byte RightDown = 1 << 3;
        public const byte All = LeftDown | LeftUp | RightUp | RightDown;
        
        public const byte LeftSide = LeftUp | LeftDown;
        public const byte DownSide = LeftDown | RightDown;
     
        public static readonly int[] Next = {1, 2, 3, 0};  // (i + 1) % 4
        public static readonly int[] Prev = {3, 0, 1, 2};  // (i + 3) % 4
        
        public static readonly int[] LinkX = {3, 2, 1, 0}; // 3 - i
        public static readonly int[] LinkY = {1, 0, 3, 2}; // (1 - i) % 4
        
        public static readonly int[][] Edges =
        {
            new [] { -1, 0, -1, 3 }, // LeftDown 
            new [] { 0, -1, 1, -1 }, // LeftUp
            new [] { -1, 1, -1, 2 }, // RightUp
            new [] { 3, -1, 2, -1 }  // RightDown
        };
        
        public static readonly int[][] EdgePerpNormalIndices =
        {
            new [] { 5, 2, 4,  3}, // Right
            new [] { 5, 2, 4, 3 }, // Left
            new [] { 1, 4, 0, 5 }, // Up
            new [] { 1, 4, 0, 5 }, // Down
            new [] { 1, 2, 0, 3 }, // Forward
            new [] { 1, 2, 0, 3 }  // Backward
        };
        
        public static readonly bool[][] IsPositiveLinkOrder =
        {
            new [] { false, true, false, true }, // LeftDown 
            new [] { false, false, true, false }, // LeftUp
            new [] { false, false, false, false }, // RightUp
            new [] { false, false, true, false }  // RightDown
        };
        
        public static readonly int[] EdgeClosestBackswingCorners =
        {
            1, 2, 2, 3
        };
        
        public static readonly int2[] CornerDirections =
        {
            new int2(-1, -1),
            new int2(-1, 1),
            new int2(1, 1),
            new int2(1, -1),
        };
    }
}