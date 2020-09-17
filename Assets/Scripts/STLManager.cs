using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class STLManager : MonoBehaviour
{
    public Camera front;
    public MeshFilter frontMesh;
    public Camera back;
    public MeshFilter backMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Mesh f=CreateMesh(front.targetTexture,0.1f);
            frontMesh.sharedMesh = f;
            Mesh b = CreateMesh(back.targetTexture, 0.1f);
            backMesh.sharedMesh = b;
        }
    }
    
    
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, GraphicsFormat.R32_SFloat,TextureCreationFlags.None);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    private Mesh CreateMesh(RenderTexture texture,float gap)
    {
        Vector3[] vertices = new Vector3[texture.width*texture.height];
        int[] triangles = new int[3*2*(texture.width-1)*(texture.height-1)];
        int cont = 0;
        Texture2D tex = toTexture2D(texture);
        for (int i=0;i<texture.width;i++)
        for (int j = 0; j < texture.height; j++)
        {
            int index = i * texture.height + j;
            vertices[index]=new Vector3(i*gap,j*gap,tex.GetPixel(i,j).r);
            if (i < (texture.width - 1) && j < (texture.height - 1))
            {
                triangles[cont++] = index;
                triangles[cont++] = index + 1;
                triangles[cont++] = index + texture.height;
                triangles[cont++] = index + 1;
                triangles[cont++] = index + texture.height+1;
                triangles[cont++] = index + texture.height;
            }
        }
        Mesh m = new Mesh {vertices = vertices, triangles = triangles,indexFormat = IndexFormat.UInt32};
        m.triangles = triangles;
        m.RecalculateBounds();
        m.RecalculateNormals();

        return m;
    }
}
