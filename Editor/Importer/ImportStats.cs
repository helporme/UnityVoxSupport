using System;

namespace VoxSupport.Editor.Importer
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