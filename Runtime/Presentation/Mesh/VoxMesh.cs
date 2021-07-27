using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public readonly partial struct VoxMesh : IVox
    {
        public readonly int3 Size;
        public readonly Flatten3DArray<Voxel> Voxels;

        public readonly List<Vector3> Vertices;
        public readonly List<Vector3> Normals;
        public readonly List<Vector2> UVs;
        public readonly List<int> Tris;

        public readonly List<int> Links;
        public readonly Dictionary<int, int> CrossLinks;
        public readonly List<TriangulationFace> Faces;

        public VoxMesh(int3 size)
        {
            Size = size;

            Voxels = new Flatten3DArray<Voxel>(size);
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            UVs = new List<Vector2>();
            Tris = new List<int>();

            Links = new List<int>();
            CrossLinks = new Dictionary<int, int>();
            Faces = new List<TriangulationFace>();
        }

        public void Write(Mesh mesh)
        {
            mesh.SetVertices(Vertices);
            mesh.SetNormals(Normals);
            mesh.SetUVs(0, UVs);
            mesh.SetTriangles(Tris, 0);
        }

        public void Read(Mesh mesh)
        {
            mesh.GetVertices(Vertices);
            mesh.GetNormals(Normals);
            mesh.GetUVs(0, UVs);
            mesh.GetTriangles(Tris, 0);
        }

        public void Recalculate()
        {
            Clear();
            CalculateNormals();
            CalculateCorners();
            CalculateFaces();
            TriangulateFaces();
        }

        public void Clear()
        {
            Vertices.Clear();
            Normals.Clear();
            UVs.Clear();
            Tris.Clear();
            Links.Clear();
            CrossLinks.Clear();
            Faces.Clear();
        }
    }
}