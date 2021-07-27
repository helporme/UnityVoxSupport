using UnityEngine;

namespace VoxSupport.Extensions
{
    public static class VoxMeshExtensions
    {
        public static void Transform(this VoxMesh mesh, Matrix4x4 matrix)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = matrix.MultiplyPoint(mesh.Vertices[i]);
            }
        }

        public static void Rotate(this VoxMesh mesh, Matrix4x4 matrix)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i] = matrix.MultiplyPoint(mesh.Vertices[i]);
                mesh.Normals[i] = matrix.MultiplyPoint(mesh.Normals[i]).normalized;
            }
        }

        public static Bounds CalculateBounds(this VoxMesh mesh)
        {
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                Vector3 vertex = mesh.Vertices[i];
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }

            return new Bounds {min = min, max = max};
        }
    }
}