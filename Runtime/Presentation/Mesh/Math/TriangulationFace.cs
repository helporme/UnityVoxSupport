using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace VoxSupport
{
    public partial struct TriangulationFace
    {
        public VoxMesh Mesh;
        public int3 Axises;
        public bool IsClockwise;
        
        public List<int> InputCrossLinks;
        public List<Vertex> Vertices;
    }
}