using UnityEngine;

namespace VoxSupport
{
    public partial struct TriangulationFace
    {
        public void Triangulate()
        {
	        for (int i = 0; i < Vertices.Count; i++)
	        {
		        CheckReflex(i);
	        }

	        int breaker = 0;
	        
	        while (Vertices.Count >= 3)
	        {
		        Vertex vertex = Vertices[0];
		        
		        if (!IsVertexEar(vertex))
		        {
			        if (breaker++ > 100)
			        {
				        Debug.LogError("[VoxImporter] Algorithm is stuck");
				        break;
			        }
			        Vertices.RemoveAt(0);
			        Vertices.Add(vertex);
			        continue;
		        }
		        breaker = 0;
		        
		        AddVertexTris();
		        Vertices.RemoveAt(0);

		        CheckReflex(0);
		        CheckReflex(Vertices.Count - 1);
	        }
        }

        private void CheckReflex(int vertexIndex)
        {
	        int prevIndex = (Vertices.Count + vertexIndex - 1) % Vertices.Count;
	        int nextIndex = (vertexIndex + 1) % Vertices.Count;
	        
	        Vector2 a = ConvVertex(Mesh.Vertices[Vertices[prevIndex].Index]);
	        Vector2 b = ConvVertex(Mesh.Vertices[Vertices[vertexIndex].Index]);
	        Vector2 c = ConvVertex(Mesh.Vertices[Vertices[nextIndex].Index]);

	        Vertex vertex = Vertices[vertexIndex];
	        vertex.IsReflex = IsVertexOrientedCounterClockwise(a, b, c);
	        Vertices[vertexIndex] = vertex;
        }
        
        private bool IsVertexOrientedCounterClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
        {
	        return (p2.y - p1.y) * (p3.x - p2.x) - (p2.x - p1.x) * (p3.y - p2.y) <= 0;
        }
        
        private bool IsVertexEar(Vertex vertex)
        {
	        if (vertex.IsReflex)
	        {
		        return false;
	        }
	        
	        Vector2 a = ConvVertex(Mesh.Vertices[Vertices[Vertices.Count - 1].Index]);
	        Vector2 b = ConvVertex(Mesh.Vertices[Vertices[0].Index]);
	        Vector2 c = ConvVertex(Mesh.Vertices[Vertices[1].Index]);
	        
	        for (int i = 2; i < Vertices.Count - 1; i++)
	        {
		        if (Vertices[i].IsReflex)
		        {
			        Vector2 p = ConvVertex(Mesh.Vertices[Vertices[i].Index]);

			        if (IsPointInTriangle(a, b, c, p) && p != a && p != b && p != c)
			        {
				        return false;
			        }
		        }
	        }

	        return true;
        }
        
        private Vector2 ConvVertex(Vector3 vertex)
        {
	        return new Vector2(vertex[Axises.x], vertex[Axises.y]);
        }
        
        private bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p)
        {
	        //Based on Barycentric coordinates
	        float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

	        float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
	        float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
	        float c = 1 - a - b;

	        return a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f;
        }

        private void AddVertexTris()
        {
	        if (IsClockwise)
	        {
		        Mesh.Tris.Add(Vertices[1].Index);
		        Mesh.Tris.Add(Vertices[0].Index);
		        Mesh.Tris.Add(Vertices[Vertices.Count - 1].Index);
	        }
	        else
	        {
		        Mesh.Tris.Add(Vertices[Vertices.Count - 1].Index);
		        Mesh.Tris.Add(Vertices[0].Index);
		        Mesh.Tris.Add(Vertices[1].Index);
	        }
        }
    }
}