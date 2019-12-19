using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARTEC.Curves;
using UnityEditor;
using UnityEngine.Serialization;

public class Patch : MonoBehaviour
{
    // Number of werps and warps
    public Vector2Int resolution;

    // Information of the yarm radius
    public Vector2 warpWeftRadius;

    public Vector2 warpWeftSpeedMultiplier;
    
    // Computed size
    public Vector2 size;

    // Number of samples between control points
    public Vector2Int samples = new Vector2Int(6,4);

    // Length of the texture to texturize the yarm
    public float textureLength = 0.1f;

    public Pattern drawing;
    public Pattern technique;
    
    private Curve[] _horizontalCurve;
    private Curve[] _verticalCurve;
    public GameObject weftPrefab;
    public GameObject warpPrefab;

    public Rect patchRect;
    public float space = 2.0f;

    public void setResolution(float v) {
        setWarpWeft(2*v,v);
        
    }

    public void setWarpWeft(float x, float y) {
        if (drawing != null) {
            drawing.warpWeft = new Vector2(x,y);
            //Weave();
        }    
    }

    public bool valueAtPixel(int i, int j) {
        bool upDown = true;

        if ((technique != null) && (technique.pattern != null))
            upDown = technique.Value(i,j) ;

        if ((drawing != null) && (drawing.pattern != null)) {
            if (!drawing.Value(i,j))
                upDown = !upDown;
        }

        return upDown;
    }

    public void Weave()
     {
         // Destroy current yarms
         while (transform.GetChild(0).childCount != 0)
         {
             var t = transform.GetChild(0).GetChild(0);
             t.parent = null;
             DestroyImmediate(t.gameObject);
         }
         while (transform.GetChild(1).childCount != 0)
         {
             var t = transform.GetChild(1).GetChild(0);
             t.parent = null;
             DestroyImmediate(t.gameObject);
         }
 
         if (drawing == null)
             return;
 
         resolution = drawing.getWeftWarpResolution();
 
         float maxWarpWeft = Mathf.Max( drawing.warpWeft.x, drawing.warpWeft.y);
 
         Vector2 correctedRadius = new Vector2(warpWeftRadius.x / drawing.warpWeft.x * maxWarpWeft, warpWeftRadius.y/drawing.warpWeft.y * maxWarpWeft);
         Vector2 correctedSpeed = new Vector2(warpWeftSpeedMultiplier.x / drawing.warpWeft.x * maxWarpWeft, warpWeftSpeedMultiplier.y / drawing.warpWeft.y*maxWarpWeft);
          
         // // Create each horizontal yarm (weft)
         _horizontalCurve = new Curve[resolution.y];
         int miny = Mathf.RoundToInt(patchRect.y * resolution.y);
         int maxy = miny + Mathf.RoundToInt(patchRect.height * resolution.y);
         int minx = Mathf.RoundToInt(patchRect.x * resolution.x);
         int maxx = minx + Mathf.RoundToInt(patchRect.width * resolution.x);
         for (var j = miny; j < maxy; j ++)
         {
             var go = Instantiate(weftPrefab, transform.GetChild(0));
             go.name = "Weft" + j;
             
             //Debug.Log(go.name);
             _horizontalCurve[j] = go.GetComponent<Curve>();
             _horizontalCurve[j].samples = samples.y;
             
             _horizontalCurve[j].interpolation = Curve.CurveInterpolation.Hermite;
             _horizontalCurve[j].speedMultiplier = correctedSpeed.y;
             
             bool currentUpDown = false;
 
             // For each cross with a vertical yarm (warp)
             for (var i = minx; i < maxx; i++)
             {
                 bool upDown = valueAtPixel(i,j);
                 bool nextUpDown = valueAtPixel(i+1,j);
 
                 // If the yarm doesn't change is not neccesary a control point
                 if ((i != minx) && (i != maxx-1))
                     if ((upDown == currentUpDown) && (upDown == nextUpDown))
                         continue;
 
                 float up = -1.0f;
                 if (!upDown)
                     up = 1.0f;
                 
                 currentUpDown = upDown;
 
                 // _horizontalCurve[j].AddControlPoint(
                 //      new Vector3(
                 //          (i-0.5f-minx-(maxx-minx)*0.5f) * correctedRadius.x * space,
                 //          0,
                 //          (j-miny-(maxy-miny)*0.5f) * correctedRadius.y * space),
                 //      Quaternion.LookRotation(up * Vector3.up, up * Vector3.right));
 
                 _horizontalCurve[j].AddControlPoint(
                     new Vector3(
                         (i-minx-(maxx-minx)*0.5f) * correctedRadius.x * space,
                         up * correctedRadius.x * space * 0.5f,
                         (j-miny-(maxy-miny)*0.5f) * correctedRadius.y * space),
                     Quaternion.LookRotation(Vector3.right, Vector3.forward));
             }
 
             //_horizontalCurve[j].UpdateCurve();
             go.GetComponent<Yarn>().UpdateMesh();
             //_horizontalCurve[j].GetComponent<CurvePipeRenderer>().curve = _horizontalCurve[j];
             //_horizontalCurve[j].GetComponent<CurvePipeRenderer>().UpdateMesh();
         }
 
         // Create each vertical yarm (warp)
         _verticalCurve = new Curve[resolution.x];
         for (var i = minx; i < maxx; i ++)
         {
             var go = Instantiate(warpPrefab, transform.GetChild(1));
             go.name = "Warp" + i;
             //Debug.Log(go.name);
             _verticalCurve[i] = go.GetComponent<Curve>();
            
             _verticalCurve[i].interpolation = Curve.CurveInterpolation.Hermite;
             _verticalCurve[i].samples = samples.y;
             _verticalCurve[i].speedMultiplier = correctedSpeed.x;
             
             bool currentUpDown = false;
 
             // For each cross with a horizontal yarm (weft)
             for (var j = miny; j < maxy; j++)
             {
                 bool upDown = valueAtPixel(i,j);
                 bool nextUpDown = valueAtPixel(i,j+1);
 
                 // If the yarm doesn't change is not neccesary a control point
                 if ((j != miny) && (j!= maxy-1))
                 if ((upDown == currentUpDown) && (upDown == nextUpDown))
                     continue;
 
                 float up = 1.0f;
                 if (!upDown)
                     up = -1.0f;
 
                 currentUpDown = upDown;
 
                 // _verticalCurve[i].AddControlPoint(
                 //      new Vector3(
                 //          (i - minx - (maxx-minx)*0.5f) * correctedRadius.x * space,
                 //          0,
                 //          (j-0.5f-miny- (maxy-miny)*0.5f) * correctedRadius.y * space),
                 //      Quaternion.LookRotation(up * Vector3.up, up * Vector3.forward));
 
                 _verticalCurve[i].AddControlPoint(
                      new Vector3(
                          (i - minx - (maxx - minx) * 0.5f) * correctedRadius.x * space,
                          up * correctedRadius.y * space * 0.5f,
                          (j - miny - (maxy - miny) * 0.5f) * correctedRadius.y * space),
                     Quaternion.LookRotation(Vector3.forward, Vector3.right));
             }
 
             //_verticalCurve[i].UpdateCurve();
             go.GetComponent<Yarn>().UpdateMesh();
             // _verticalCurve[i].GetComponent<CurvePipeRenderer>().curve = _verticalCurve[i];
             // _verticalCurve[i].GetComponent<CurvePipeRenderer>().UpdateMesh();
         }
 
         // Update box collider
         BoxCollider bc = GetComponent<BoxCollider>();
         if (bc != null) {
             bc.center = new Vector3(
                 0,
                 0,
                 0
             );
             bc.size = new Vector3(
                 (maxx-minx) * correctedRadius.x * space,
                 0.4f,
                 (maxy-miny) * correctedRadius.y * space
             );
         }
     }
 
      
   
      
}
