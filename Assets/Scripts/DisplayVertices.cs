using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayVertices : MonoBehaviour
{
    List<Vector3> vertices = new List<Vector3>();
    public int vertexIndex = 0;
    public float radius = 0.1f;
    public bool isDrawing = true;
    void OnDrawGizmos()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;
        if(mesh && isDrawing)
        {
            mesh.GetVertices(vertices);
            for (int i = 0; i < vertices.Count; i++)
            {
                if(i == vertexIndex)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), radius * 2);
                }
                else
                {    
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), radius);
                }
            }
        }
    }
}
