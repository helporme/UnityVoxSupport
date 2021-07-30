using System.IO;

namespace VoxSupport
{
    public interface IVoxImporter
    {
        public void Import(BinaryReader reader, IVoxBuilder builder);
    }
}