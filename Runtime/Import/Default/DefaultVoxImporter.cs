using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public class DefaultVoxImporter : IVoxImporter
    {
        public void Import(BinaryReader reader, IVoxBuilder builder)
        {
            ValidateMagicNumber(reader);
            ValidateVersion(reader);

            ReadChunks(reader, builder);
        }

        private void ValidateMagicNumber(BinaryReader reader)
        {
            byte[] magic = reader.ReadBytes(4);

            if (!Compare4Bytes(magic, 'V', 'O', 'X', ' '))
            {
                throw new VoxImportException($"Invalid vox magic number \"{Encoding.UTF8.GetString(magic)}\".");
            }
        }

        private void ValidateVersion(BinaryReader reader)
        {
            int version = reader.ReadInt32();

            if (version != 150)
            {
                throw new VoxImportException($"Invalid vox version \"{version}\".");
            }
        }

        private void ValidateMainChunk(VoxChunk mainChunk)
        {
            if (!Compare4Bytes(mainChunk.Id, 'M', 'A', 'I', 'N'))
            {
                throw new VoxImportException($"Invalid main chunk \"{mainChunk.Id}\".");
            }
        }

        private void ReadChunks(BinaryReader reader, IVoxBuilder voxBuilder)
        {
            VoxChunk mainChunk = ReadChunk(reader);
            ValidateMainChunk(mainChunk);
            reader.ReadBytes(mainChunk.Size);

            int readSize = 0;

            while (readSize < mainChunk.ChildrenSize)
            {
                VoxChunk chunk = ReadChunk(reader);

                if (Compare4Bytes(chunk.Id, 'P', 'A', 'C', 'K'))
                {
                    reader.ReadInt32();
                }
                else if (Compare4Bytes(chunk.Id, 'S', 'I', 'Z', 'E'))
                {
                    var size = new int3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    voxBuilder.SetVoxelsGridSize(size);
                }
                else if (Compare4Bytes(chunk.Id, 'X', 'Y', 'Z', 'I'))
                {
                    ReadVoxels(reader, voxBuilder);
                }
                else if (Compare4Bytes(chunk.Id, 'R', 'G', 'B', 'A'))
                {
                    ReadPalette(reader, voxBuilder);
                }

                readSize += chunk.Size + chunk.ChildrenSize + 4 * 3;
            }
        }

        private bool Compare4Bytes(byte[] bytes, char c0, char c1, char c2, char c3)
        {
            return bytes[0] == c0 && bytes[1] == c1 && bytes[2] == c2 && bytes[3] == c3;
        }

        private void ReadVoxels(BinaryReader reader, IVoxBuilder voxBuilder)
        {
            int voxelsCount = reader.ReadInt32();
            for (int i = 0; i < voxelsCount; i++)
            {
                int x = reader.ReadByte();
                int y = reader.ReadByte();
                int z = reader.ReadByte();
                byte color = reader.ReadByte();

                voxBuilder.SetVoxelColor(new int3(x, y, z), color);
            }
        }

        private void ReadPalette(BinaryReader reader, IVoxBuilder voxBuilder)
        {
            voxBuilder.SetPaletteSize(256);
            for (int i = 0; i < 256; ++i)
            {
                float r = reader.ReadByte() / 255.0f;
                float g = reader.ReadByte() / 255.0f;
                float b = reader.ReadByte() / 255.0f;
                float a = reader.ReadByte() / 255.0f;

                voxBuilder.SetPaletteColor(i, new Color(r, g, b, a));
            }
        }

        private VoxChunk ReadChunk(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);

            return new VoxChunk
            {
                Id = bytes,
                Size = reader.ReadInt32(),
                ChildrenSize = reader.ReadInt32()
            };
        }
    }
}