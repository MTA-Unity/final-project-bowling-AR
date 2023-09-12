using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    [SerializeField] private float stemLength;
    [SerializeField] private float stemWidth;
    [SerializeField] private float tipLength;
    [SerializeField] private float tipWidth;

    private List<Vector3> _verticesList;
    private List<int> _trianglesList;
 
    Mesh mesh;
 
    void Start()
    {
        // Make sure Mesh Renderer has a material
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }
 
    void Update()
    {
        GenerateArrow();
    }

    // Arrow is generated starting at Vector3.zero
    // Arrow is generated facing right, towards radian 0.
    void GenerateArrow()
    {
        // Setup
        _verticesList = new List<Vector3>();
        _trianglesList = new List<int>();
 
        // Stem setup
        Vector3 stemOrigin = Vector3.zero;
        float stemHalfWidth = stemWidth / 2f;
        
        // Stem points
        _verticesList.Add(stemOrigin + (stemHalfWidth * Vector3.down));
        _verticesList.Add(stemOrigin + (stemHalfWidth * Vector3.up));
        _verticesList.Add(_verticesList[0] + (stemLength * Vector3.right));
        _verticesList.Add(_verticesList[1] + (stemLength * Vector3.right));
  
        // Stem triangles
        _trianglesList.Add(0);
        _trianglesList.Add(1);
        _trianglesList.Add(3);
 
        _trianglesList.Add(0);
        _trianglesList.Add(3);
        _trianglesList.Add(2);
        
        // Tip setup
        Vector3 tipOrigin = stemLength*Vector3.right;
        float tipHalfWidth = tipWidth/2;
 
        // Tip points
        _verticesList.Add(tipOrigin+(tipHalfWidth*Vector3.up));
        _verticesList.Add(tipOrigin+(tipHalfWidth*Vector3.down));
        _verticesList.Add(tipOrigin+(tipLength*Vector3.right));
 
        // Tip triangle
        _trianglesList.Add(4);
        _trianglesList.Add(6);
        _trianglesList.Add(5);
 
        // Assign lists to mesh.
        mesh.vertices = _verticesList.ToArray();
        mesh.triangles = _trianglesList.ToArray();
    }
}