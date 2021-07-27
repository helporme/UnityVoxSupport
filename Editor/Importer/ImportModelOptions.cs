using System;

namespace VoxSupport.Editor
{
    [Serializable]
    public class ImportModelOptions
    {
        public Orientation orientation = Orientation.XZY;
        public float scale = 0.1f;
        public bool moveToFloor;
        public bool transformMesh = true;

        public enum Orientation
        {
            XYZ,
            XZY,
            ZYX
        }
    }
}