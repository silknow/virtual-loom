using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ARTEC.Curves {
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CurvePipeRenderer : MonoBehaviour {

	public Curve curve = null;
	public float r1 = 0.2f;
	public float r2 = 0.2f;
	public float minR1 = 0.2f;
	public float minR2 = 0.2f;
	public float narrowingDistance = 0.93f;
	public int samples = 20;
	public float textureLength = 3.33f;
	private Int32 _numberOfVertex = 0;
	private Mesh _mesh = null;
	private MeshFilter _meshFilter;
	public float angleInc;
	
   
    public void UpdateMesh() {
	    ClippingPlane.instance.matList.Add(GetComponent<Renderer>().material);
	    _meshFilter = GetComponent<MeshFilter>();
	    
	    var points = curve.GetPoints();
	    if ((points == null) || (points.Count <= 0))
		    return;

	    if (_mesh == null)
	    {
		    _mesh = new Mesh();
		    _mesh.indexFormat = IndexFormat.UInt32;
	    }

	    // Vertex: generate an ellipse around each point
	    _numberOfVertex = points.Count * ((samples)+1)+2;
	    
	    var vertex = new Vector3[_numberOfVertex];
	    var normal = new Vector3[_numberOfVertex];
	    var uv = new Vector2[_numberOfVertex];
	    Int32 v = 0;
		
	    var angleStep = Mathf.PI * 2 / samples;
	    var length = 0.0f;
	    float randomAngle = Random.Range(0.0f, 360.0f);
	    for (var i=0; i<points.Count; i++)
	    {

		    float mult =  Mathf.Abs(Mathf.Cos(Mathf.Max(0.0f,(narrowingDistance-points[i].DistanceToKnot)/narrowingDistance) * (float)Math.PI/2));
		    if (i>0)
			    length += (points[i].Pos - points[i-1].Pos).magnitude;
		    for (var j=0; j<=(samples); j++)
		    {
			    Vector3 p = points[i].Rot *
			                new Vector3(Mathf.Cos(angleStep * j + randomAngle) * (minR1 + (r1 - minR1) * mult),
				                Mathf.Sin(angleStep * j + randomAngle) * (minR2 + (r2 - minR2) * mult), 0);
			    normal[v] = p.normalized;
			    uv[v] = new Vector2((float)j/(float)samples, points[i].Distance/textureLength);
			    vertex[v] = points[i].Pos + p;
			    v++;
		    }
	    }
		
	    // Add central point for the two covers
	    normal[v] = transform.InverseTransformDirection(points[0].Pos - points[1].Pos);
	    uv[v] = new Vector2(0.5f,0.5f);
	    vertex[v++] = transform.InverseTransformPoint(points[0].Pos[0] , points[0].Pos[1],points[0].Pos[2]);
		
	    normal[v] = transform.InverseTransformDirection(points[points.Count-1].Pos - points[points.Count-2].Pos);
	    uv[v] = new Vector2(0.5f,0.5f);
	    vertex[v++] = transform.InverseTransformPoint(points[points.Count-1].Pos[0] , points[points.Count-1].Pos[1],points[points.Count-1].Pos[2]);
		
		
	    // Triangles: generate triangle to join the ellipses
	    var numberOfTriangles = (points.Count - 1) * ((samples) * 2);
	    // Add triangles for the two covers
	    numberOfTriangles += 2*((samples));
		
	    var triangles = new Int32[numberOfTriangles * 3];
	    Int32 t = 0;
	    for (Int32 i=0; i<(points.Count-1); i++) {
		    for (Int32 j=0; j<(samples); j++) {
			    Int32 current = i * ((samples)+1) + j;
			    Int32 next = (i + 1) * ((samples)+1) + j;

			    triangles[t++] = current;
			    triangles[t++] = current+1;
			    triangles[t++] = next;

			    triangles[t++] = next;
			    triangles[t++] = current +1;
			    triangles[t++] = next+1;
		    }
	    }
		
	    // Triangles: generate triangles of the two covers
	    Int32 centerCover = _numberOfVertex - 2;
	    Int32 coverI = 0;
	    for (var j=0; j<(samples); j++) {
		    var current = coverI * ((samples)+1) + j;

		    triangles[t++] = current;
		    triangles[t++] = centerCover;
		    triangles[t++] = current+1;
	    }
		
	    centerCover = _numberOfVertex - 1;
	    coverI = points.Count - 1;
	    for (var j=0; j<(samples); j++) {
		    var current = coverI * ((samples)+1) + j;

		    triangles[t++] = current;
		    triangles[t++] = current + 1;
		    triangles[t++] = centerCover;
	    }
		
			    // Create the mesh and assign it to the mesh filter component
	    _mesh.vertices = vertex;
	    _mesh.normals = normal;
	    _mesh.uv = uv;
	    _mesh.triangles = triangles;
	    _mesh.RecalculateBounds();

	    _meshFilter.mesh = _mesh;
	    _meshFilter.mesh.RecalculateNormals();
	}
}

}