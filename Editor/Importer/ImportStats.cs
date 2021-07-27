using System;

namespace VoxSupport.Editor
{
    [Serializable]
    public class ImportStats
    {
        public float importTime;
        public float convertTime;

        public int vertexCount;
        public int faceCount;
        public int linkCount;
    }
}