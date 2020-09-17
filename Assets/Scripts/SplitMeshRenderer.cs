using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class SplitMeshRenderer : MonoBehaviour
{
    public Vector2Int divisions;
    // Start is called before the first frame update
    public void Split()
    {
        Transform parent = transform.parent;
        GameObject go = new GameObject();
        go.name = name + " Splitted";
        go.transform.parent = transform.parent;
        List<int>  [,]indices= new List<int>[divisions.x,divisions.y];
        //Get bounding box
        BoxCollider boundingBox=GetComponentInParent<Patch>().gameObject.GetComponent<BoxCollider>();
        Bounds bounds = GetComponent<MeshFilter>().mesh.bounds;
        Rect r;
        Vector2 orig=new Vector2(bounds.center.x-bounds.size.x/2.0f,bounds.center.z-bounds.size.z/2.0f);
        Vector2 size=(new Vector2(bounds.size.x/divisions.x,bounds.size.z/divisions.y))*1.001f;
        MeshFilter mf = GetComponent<MeshFilter>();
        int[] originalIndexes = mf.mesh.triangles;
        Vector3[] vertices = mf.mesh.vertices;
        for (int i=0;i<divisions.x;i++)
        for (int j = 0; j < divisions.y; j++)
        {
            indices[i,j]=new List<int>();
        }
        for (int i = 0; i < originalIndexes.Length; i += 3)
        {
            int column = (int)((vertices[originalIndexes[i]].x - orig.x) / size.x);
            int row = (int)((vertices[originalIndexes[i]].z - orig.y) / size.y);
            indices[row,column].Add(originalIndexes[i]);
            indices[row,column].Add(originalIndexes[i+1]);
            indices[row,column].Add(originalIndexes[i+2]);
        }
        for (int i=0;i<divisions.x;i++)
        for (int j = 0; j < divisions.y; j++)
        {
            Rect rect=new Rect(orig.x+i*(size.x),orig.y+j*(size.y),size.x,size.y);
            GameObject go2 = createNode(indices[j, i], rect);
            go2.transform.parent = go.transform;
        }
        DestroyImmediate(gameObject);
    }

    protected GameObject createNode(List<int> indices, Rect rect)
    {
        GameObject go = GameObject.Instantiate(gameObject);
        
        MeshFilter mf = go.GetComponent<MeshFilter>();
        
        mf.mesh.triangles = indices.ToArray();
        Vector3 center = new Vector3(rect.center.x,0,rect.center.y);
        Vector3 size = new Vector3(rect.size.x,1,rect.size.y);
        mf.mesh.bounds=new Bounds(center,size);
        return go;
    }

    void RemoveVertices(ref Vector3 []vertices,ref Vector3 []normals,ref Vector2 []uv,ref int []triangles)
    {
        int cont = 0;
        Dictionary<int, int> newIndex = new Dictionary<int, int>();
        //Apunto todos los indices que se usan en un diccionario
        for (int i=0; i < triangles.Length; i++)
        {
            if (!newIndex.ContainsKey(triangles[i]))
            {
                
                newIndex.Add(triangles[i],cont++);
            }
        }
        //Cambio posiciones de los indices que se usan al principio del vector
        for (int i=0; i < triangles.Length; i++)
        {
            var Value = newIndex[triangles[i]];
            Vector3 aux3=vertices[Value];
            vertices[Value] = vertices[i];
            vertices[i] = aux3;
            
            aux3=normals[Value];
            normals[Value] = normals[i];
            normals[i] = aux3;
            
            Vector2 aux2=uv[Value];
            uv[Value] = uv[i];
            uv[i] = aux2;
            int aux=Value;
            triangles[Value] = i;
            triangles[i] = aux;

        }
        //Reducimos los vectors al tamaño que realmente necesitan
        Array.Resize<Vector3 >(ref vertices, cont);
        Array.Resize<Vector3 >(ref normals, cont);
        Array.Resize<Vector2 >(ref uv, cont);
    }
}
