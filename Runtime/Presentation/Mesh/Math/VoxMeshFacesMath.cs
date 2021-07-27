using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using VoxSupport.Utils;

namespace VoxSupport
{
    public partial struct VoxMesh
    {
        public void CalculateFaces()
        {
            int[] faceIndices = new int[Links.Count];

            CalculateFaces(faceIndices);
            MergeFaces(faceIndices);
        }

        private void CalculateFaces(int[] faceIndices)
        {
            for (int i = 0; i < Links.Count; i++)
            {
                int startIndex = Links[i];
                if (startIndex < 0)
                {
                    continue;
                }

                CalculateFace(faceIndices, startIndex);
            }
        }

        private void CalculateFace(int[] faceIndices, int startIndex)
        {
            int faceIndex = Faces.Count;
            var faceVertices = new List<Vertex>();

            int crossLinkFrom = -1;
            int crossLinkTo = -1;

            int nextLink = startIndex;
            do
            {
                if (CrossLinks.TryGetValue(nextLink, out int outCrossLink))
                {
                    crossLinkFrom = nextLink;
                    crossLinkTo = outCrossLink;
                }

                int nextLinkBuffer = Links[nextLink];
                faceIndices[nextLink] = faceIndex;
                Links[nextLink] = -(2 + faceVertices.Count);
                faceVertices.Add(new Vertex(nextLink));

                nextLink = nextLinkBuffer;
            } while (nextLink != startIndex);

            faceVertices.Insert(0, faceVertices[faceVertices.Count - 1]);
            faceVertices.RemoveAt(faceVertices.Count - 1);
            Links[faceVertices[0].Index] = -1;

            if (crossLinkTo != -1)
            {
                int crossLinkFace = faceIndices[crossLinkTo];

                // Check for self cross-link and cross-link loop
                if (crossLinkFace != faceIndex && Links[crossLinkTo] < 0)
                {
                    AddCrossLinksToFaces(crossLinkFace, crossLinkTo, crossLinkFrom);
                }
            }

            CreateFace(faceVertices);
        }

        private void CreateFace(List<Vertex> faceVertices)
        {
            var face = new TriangulationFace
            {
                Mesh = this,
                InputCrossLinks = new List<int>(),
                Vertices = faceVertices,
                Axises = GetFaceAxises(faceVertices),
                IsClockwise = IsClockwise(Normals[faceVertices[0].Index])
            };

            Faces.Add(face);
        }

        private void AddCrossLinksToFaces(int crossLinkFace, int crossLinkTo, int crossLinkFrom)
        {
            List<int> inputCrossLinks = Faces[crossLinkFace].InputCrossLinks;

            int localCrossLinkVertIndex = -Links[crossLinkTo] - 1;
            for (int i = 0; i < inputCrossLinks.Count; i++)
            {
                int nextLocalCrossLinkVertIndex = -Links[CrossLinks[inputCrossLinks[i]]] - 1;
                if (localCrossLinkVertIndex > nextLocalCrossLinkVertIndex)
                {
                    inputCrossLinks.Insert(i, crossLinkFrom);
                    return;
                }
            }

            inputCrossLinks.Add(crossLinkFrom);
        }

        private int3 GetFaceAxises(List<Vertex> vertexIndices)
        {
            Vector3 a = Vertices[vertexIndices[0].Index];
            Vector3 b = Vertices[vertexIndices[1].Index];
            Vector3 c = Vertices[vertexIndices[2].Index];
            return GetFaceAxises(a, b, c);
        }

        private int3 GetFaceAxises(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 vDiff = a - b - (b - c);
            for (int i = 0; i < 3; i++)
            {
                if (vDiff[i] == 0)
                {
                    return NormalVectors.Axises[i * 2];
                }
            }
            return -1;
        }

        private bool IsClockwise(Vector3 normal)
        {
            return normal == Vector3.left || normal == Vector3.forward || normal == Vector3.down;
        }

        private void MergeFaces(int[] faceIndices)
        {
            for (int faceIndex = 0; faceIndex < Faces.Count; faceIndex++)
            {
                TriangulationFace face = Faces[faceIndex];

                // Face was merged
                if (face.Vertices.Count == 0)
                {
                    continue;
                }

                MergeFaces(faceIndices, face);
            }
        }

        private void MergeFaces(int[] faceIndices, TriangulationFace face)
        {
            for (int i = 0; i < face.InputCrossLinks.Count; i++)
            {
                int sourceCrossLink = face.InputCrossLinks[i];
                MergeFace(faceIndices, face, sourceCrossLink);
            }

            face.InputCrossLinks.Clear();
        }

        private void MergeFace(int[] faceIndices, TriangulationFace face, int sourceCrossLink)
        {
            TriangulationFace sourceFace = Faces[faceIndices[sourceCrossLink]];

            // Links currently pointing to local face indices of the Vertices,
            // but they were written as -vertexFaceIndex - 1, so to convert it back we need to
            // change sign of the number and decrease it (I don't do that because I need index for insertion (+1)) 

            int sourceBeginVertIndex = -Links[sourceCrossLink] - 1;
            MergeFaces(faceIndices, sourceFace);

            int insertBeginVertIndex = -Links[CrossLinks[sourceCrossLink]];
            Vertex vertexBeforeInsertion = face.Vertices[insertBeginVertIndex - 1];

            for (int j = sourceBeginVertIndex; j < sourceFace.Vertices.Count; j++)
            {
                face.Vertices.Insert(insertBeginVertIndex++, sourceFace.Vertices[j]);
            }
            for (int j = 0; j < sourceBeginVertIndex; j++)
            {
                face.Vertices.Insert(insertBeginVertIndex++, sourceFace.Vertices[j]);
            }

            face.Vertices.Insert(insertBeginVertIndex++, sourceFace.Vertices[sourceBeginVertIndex]);
            face.Vertices.Insert(insertBeginVertIndex, vertexBeforeInsertion);

            sourceFace.Vertices.Clear();
        }

        public void TriangulateFaces()
        {
            foreach (TriangulationFace face in Faces)
            {
                face.Triangulate();
            }
        }
    }
}