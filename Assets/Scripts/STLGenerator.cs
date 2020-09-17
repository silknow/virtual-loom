using System.Collections;
using System.Collections.Generic;
using System.Net;
using Parabox.STL;
using UnityEngine;

public class STLGenerator : MonoBehaviour
{
    public Texture2D heightMapFront;
    public bool negateFrontMap;
    public Texture2D heightMapBack;
    public bool negateBackMap;
    
    public float heightMult = 50;
    public float minHeight = 1;

    public float minGray = 0;
    public float maxGray = 1;
    
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    public Camera frontCamera, backCamera;
    public int resolution;
    public string path;
    public Transform stand;
    public MeshFilter meshFilter;
    
    public void GenerateGeometry()
    {

        heightMapFront = frontCamera.GetComponent<DepthProcessing>().texture;
        heightMapBack = backCamera.GetComponent<DepthProcessing>().texture;
        int width = heightMapFront.width;
        int height = heightMapFront.height;

        //minHeight = 0;
        tris.Clear();
        verts.Clear();
        
        stand.localScale = new Vector3(width*0.055f,width*0.11f,width*1.1f);
        stand.localPosition = new Vector3(-width*0.055f/2,0,width/2.0f);
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {

                float v = heightMapFront.GetPixel(j, i).grayscale;
                v = Mathf.Clamp01((v - minGray) / (maxGray - minGray));
                if (negateFrontMap)
                    v = 1 - v;
        
                //Add each new vertex in the plane
                verts.Add(new Vector3(
                    i, 
                    v * heightMult*width + minHeight,
                    j));
                
                //Skip if a new square on the plane hasn't been formed
                if (i == 0 || j == 0) continue;
                
                //Adds the index of the three vertices in order to make up each of the two tris
                tris.Add(width * i + j); //Top right
                tris.Add(width * i + j - 1); //Bottom right
                tris.Add(width * (i - 1) + j - 1); //Bottom left - First triangle

                tris.Add(width * (i - 1) + j - 1); //Bottom left 
                tris.Add(width * (i - 1) + j); //Top left
                tris.Add(width * i + j); //Top right - Second triangle
            }
        }

        int backOffset = verts.Count;

        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                
                float v = heightMapBack.GetPixel(j, height-i).grayscale;
                v = Mathf.Clamp01((v - minGray) / (maxGray - minGray));
                if (negateFrontMap)
                    v = 1 - v;
                //Add each new vertex in the plane
                verts.Add(new Vector3(
                    i, 
                    ((v * heightMult*width) + minHeight) * -1, 
                    j));
                
               
                //Skip if a new square on the plane hasn't been formed
                if (i == 0 || j == 0) continue;

                //Adds the index of the three vertices in order to make up each of the two tris
                tris.Add(backOffset + width * (i - 1) + j - 1); //Bottom left - First triangle
                tris.Add(backOffset + width * i + j - 1); //Bottom right
                tris.Add(backOffset + width * i + j); //Top right


                tris.Add(backOffset + width * i + j); //Top right - Second triangle
                tris.Add(backOffset + width * (i - 1) + j); //Top left
                tris.Add(backOffset + width * (i - 1) + j - 1); //Bottom left 
            }
        }

        // Cerrar los laterales

        for (int i=1; i<height; i++) {
            tris.Add(i * width);
            tris.Add(backOffset + i * width);
            tris.Add(backOffset + (i - 1) * width);

            tris.Add((i - 1) * width);
            tris.Add(i * width);
            tris.Add(backOffset + (i - 1) * width);
        }

        for (int i = 1; i < height; i++) {
            tris.Add(backOffset + (i - 1) * width + (width - 1));
            tris.Add(backOffset + i * width + (width - 1));
            tris.Add(i * width + (width-1));

            tris.Add(backOffset + (i - 1) * width + (width - 1));
            tris.Add(i * width + (width - 1));
            tris.Add((i - 1) * width + (width - 1));
        }

        for (int j=1; j<width; j++) {
            tris.Add(backOffset + j - 1);
            tris.Add(backOffset + j);
            tris.Add(j);

            tris.Add(backOffset + j - 1);
            tris.Add(j);
            tris.Add(j - 1);
        }

        int offToLastRow = (height - 1) * width;
        for (int j = 1; j < width; j++) {

            tris.Add(j + offToLastRow);
            tris.Add(backOffset + j + offToLastRow);
            tris.Add(backOffset + j - 1 + offToLastRow);

            tris.Add(j - 1 + offToLastRow);
            tris.Add(j + offToLastRow);
            tris.Add(backOffset + j - 1 + offToLastRow);
            

        }

        Vector2[] uvs = new Vector2[verts.Count];
        for (var i = 0; i < uvs.Length; i++) //Give UV coords X,Z world coords
            uvs[i] = new Vector2(verts[i].x, verts[i].z);

        Mesh procMesh = new Mesh();
        procMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        procMesh.vertices = verts.ToArray(); //Assign verts, uvs, and tris to the mesh
        procMesh.uv = uvs;
        procMesh.triangles = tris.ToArray();
        procMesh.RecalculateNormals(); //Determines which way the triangles are facing

        meshFilter.mesh = procMesh;
       Parabox.STL.pb_Stl_Exporter.Export(path, new GameObject[] { gameObject }, FileType.Binary);
    }
}
