using System.IO;

namespace VoxSupport
{
    public static class VoxImporter
    {
        public static IVoxImporter Importer { get; set; } = new DefaultVoxImporter();
        
        public static TVox Import<TVox>(string path) where TVox : IVox, new()
        {
            IVoxBuilder builder = new TVox().GetBuilder();
            Import(path, builder);
            return (TVox) builder.Vox;
        }

        public static void Import(string path, IVoxBuilder builder)
        {
            FileStream stream = File.OpenRead(path);
            Import(new BinaryReader(stream), builder);
            stream.Close();
        }

        public static TVox Import<TVox>(BinaryReader reader) where TVox : IVox, new()
        {
            var vox = new TVox();
            IVoxBuilder builder = vox.GetBuilder();
            Import(reader, builder);
            return vox;
        }

        public static void Import(BinaryReader reader, IVoxBuilder builder)
        {
            Importer.Import(reader, builder);
        }
    }
}