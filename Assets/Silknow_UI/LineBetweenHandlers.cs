using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineBetweenHandlers : MonoBehaviour
{
    public RectTransform[] handlers;
    public Material  lineMaterial;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    //Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = handlers.Length;
        for (int i=0; i<handlers.Length; i++) {
            lineRenderer.SetPosition(i,handlers[i].position);
        }
        
    }

/*    public void OnRenderObject()
    {
        if(Camera.current.cameraType != CameraType.Preview) {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            GL.Begin(GL.LINE_STRIP);
            for (int i=0; i<handlers.Length; i++) {
                GL.Vertex(handlers[i].position);
            }
            GL.End();

            GL.Begin(GL.LINE_STRIP);
            GL.Vertex3(0,0,0);
            GL.Vertex3(100,0,0);
            GL.Vertex3(0,100,0);
            GL.End();

            GL.PopMatrix();
        }
    }

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.SetColor("_Color", Color.red);
        }
    }*/

    // GL.Begin(GL.QUADS);
	//         for (int i = 0; i < end; ++i)
	// 		{
	//             Vector3 perpendicular = (new Vector3(linePoints[i+1].y, linePoints[i].x, nearClip) -
	//                                  new Vector3(linePoints[i].y, linePoints[i+1].x, nearClip)).normalized * thisWidth;
	//             Vector3 v1 = new Vector3(linePoints[i].x, linePoints[i].y, nearClip);
	//             Vector3 v2 = new Vector3(linePoints[i+1].x, linePoints[i+1].y, nearClip);
	//             GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
	//             GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
	//             GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
	//             GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
    //     	}

}
