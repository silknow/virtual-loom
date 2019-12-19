using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class editorCilindro : MonoBehaviour
{
    private MeshFilter _meshFilter;

    public float freq;

    public float twist;
    // Start is called before the first frame update
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh.RecalculateNormals(180);
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2[] uv = _meshFilter.mesh.uv;
        for (int i=0;i<uv.Length;i++)
        {
            uv[i]= new Vector2(uv[i].x,Mathf.Max(freq,uv[i].y));
        }

        _meshFilter.mesh.uv = uv;
    }
}
