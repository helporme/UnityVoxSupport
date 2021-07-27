namespace VoxSupport
{
    public interface IVox
    {
        public IVoxBuilder GetBuilder();

        public IVoxReader GetReader();
    }
}