using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Windows.Forms;
using UnityEngine;
using ARTEC.Curves;
using Parabox.STL;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Patch : MonoBehaviour
{
    // Number of werps and warps
    public Vector2Int resolution;

    public float divider = -1.0f;
    public Vector3 gap;
    public float compactness;
    public float compactnessY;
    public Pattern backgroundPattern;
    public Pattern technique;

    public Vector2Int dilatationSize;
    
    private Curve weft;
    private Curve[] warp;
    
    public ScriptableYarn warpYarn;
    public ScriptableYarn weftYarn;

    public List<Pictorial> pictorials;
    //public int[,] depth;
    public float explodeLevel = 0.0f;
    
    public Bounds bounds;
    public STLGenerator stlNode;
    public MapsGenerator mapsGeneratorNode;
    public void Awake()
    {
        explodeLevel = 0;
        if (pictorials==null)
            pictorials = new List<Pictorial>();
    }
    
    public bool valueAtPixel(int column, int row)
    {
        bool frontFace = true;

        if ((technique != null) && (technique.pattern != null))
            frontFace = technique.Value(column, row);

        if ((backgroundPattern != null) && (backgroundPattern.pattern != null))
        {
            if (!backgroundPattern.Value(column, row))
                frontFace = !frontFace;
        }

        return frontFace;
    }

    public void Update()
    {
        updateExplodeLevel(explodeLevel);
    }

    public void updateExplodeLevel(float level)
    {
        level /= transform.childCount;
        for (int i=0;i<transform.childCount;i++)
        {
            int pos = i;
            if (i > 1)
                pos = transform.childCount - i+1;
            transform.GetChild(i).transform.localPosition = level * pos * gap.y*0.1f*Math.Max(resolution.x,resolution.y)*Vector3.up;
        }
    }

    public void setLayerVisible(int index, bool visible)
    {
        if (index<transform.childCount)
            transform.GetChild(index).gameObject.SetActive(visible);
    }
    
    public bool pictoricalValueAtPixel(YarnPictorial p, int colunm, int row, out bool healed)
    {
        bool frontFace = p.Value(colunm, row);
        healed = false;
        if (pictoricalHealed(p,colunm,row))
        {
            if (frontFace == true || p.doubleHealed)
            {
                frontFace = !frontFace;
                healed = true;
            }
        }
        return frontFace;
    }

    public bool pictoricalHealed(YarnPictorial p, int column, int row)
    {
        bool healed=false;
        if (p.healedStep != -1)
        {
            if (column>=p.firstPoint[row] && column<=p.lastPoint[row] || !p.adjusted)
                if ((column+(row*p.healedStepGap))%p.healedStep==0)
                    healed=true;
        }
        return healed;
    }
    
    private void WeaveWarp(Rect rect)
    {
        Yarn yarn;
        // Create each vertical yarm (warp)
        warp = new Curve[resolution.x];
        GameObject parent=new GameObject("Warp");
        parent.transform.parent = transform;
        for (int i = (int)rect.xMin; i < rect.xMax; i++)
        {
            GameObject go = Instantiate(Resources.Load("Yarn"),parent.transform) as GameObject;
            go.name = "Warp" + i;
            yarn = go.GetComponent<Yarn>();
            yarn.attributes = warpYarn;
            warp[i] = go.GetComponent<Curve>();
            warp[i].speedMultiplier *= gap.z;
            bool currentUpDown = false;

            // For each cross with a horizontal yarm (weft)
            for (int j = (int)rect.yMin; j < rect.yMax; j++)
            {
                bool upDown = valueAtPixel(i, j);
                bool nextUpDown = valueAtPixel(i, j + 1);

                // If the yarm doesn't change is not neccesary a control point
                if ((j != rect.yMin) && (j != rect.yMax - 1))
                    if ((upDown == currentUpDown) && (upDown == nextUpDown))
                        continue;

                float up = 1.0f;
                if (!upDown)
                    up = -1.0f;

                warp[i].AddControlPoint(
                    new Vector3(
                        (i - rect.xMin - rect.width * 0.5f) * gap.x,
                        up * gap.y * 0.5f,
                        (j - rect.yMin - rect.height * 0.5f) *  gap.z),
                    Quaternion.LookRotation(Vector3.forward, Vector3.right),
                    currentUpDown != upDown);
                
                currentUpDown = upDown;
            }
            yarn.UpdateMesh();
            bounds.Encapsulate(yarn.bounds);
        }
    }



    private void WeaveWeft(Rect rect)
    {
        // // Create each horizontal yarm (weft)
        GameObject go = null;
        go = Instantiate(Resources.Load("Yarn"),transform) as GameObject;
        go.name = "Weft";
        Yarn yarn = go.GetComponent<Yarn>();
        yarn.attributes = weftYarn;
        weft = go.GetComponent<Curve>();
        weft.speedMultiplier *= gap.x;
        for (var row = (int)rect.yMin; row < rect.yMax; row++)
        {
            bool currentUpDown = false;
            // For each cross with a vertical yarm (warp)
            if (row%2==0)
            {
                for (int column = (int) rect.xMin; column < rect.xMax; column++)
                {
                    bool upDown = valueAtPixel(column, row);
                    bool nextUpDown = valueAtPixel(column + 1, row);
                    // If the yarm doesn't change is not neccesary a control point
                    if ((column != rect.xMin) && (column != rect.xMax - 1))
                        if ((upDown == currentUpDown) && (upDown == nextUpDown))
                            continue;
                    float up = -1.0f;
                    if (!upDown)
                        up = 1.0f;

                    weft.AddControlPoint(
                        new Vector3(
                            (column - rect.x - (rect.width) * 0.5f) * gap.x,
                            up * gap.y * 0.5f,
                            (row - rect.y - (rect.height) * 0.5f) * gap.z),
                        Quaternion.LookRotation(Vector3.right, Vector3.forward),
                        currentUpDown != upDown);

                    currentUpDown = upDown;
                }
            }
            else
            {
                for (int column = (int) rect.xMax; column >= rect.xMin; column--)
                {
                    bool upDown = valueAtPixel(column, row);
                    bool nextUpDown = valueAtPixel(column - 1, row);
                    // If the yarm doesn't change is not neccesary a control point
                    if ((column != rect.xMin) && (column != rect.xMax - 1))
                        if ((upDown == currentUpDown) && (upDown == nextUpDown))
                            continue;
                    float up = -1.0f;
                    if (!upDown)
                        up = 1.0f;

                    weft.AddControlPoint(
                        new Vector3(
                            (column - rect.x - (rect.width) * 0.5f) * gap.x,
                            up * gap.y * 0.5f,
                            (row - rect.y - (rect.height) * 0.5f) * gap.z),
                        Quaternion.LookRotation(Vector3.left, Vector3.back),
                        currentUpDown != upDown);

                    currentUpDown = upDown;
                }
            }
        }
        yarn.UpdateMesh();
        bounds.Encapsulate(yarn.bounds);
        //Split mesh
        //go.GetComponentInChildren<SplitMeshRenderer>().Split();
        
    }

    private void WeavePictorical(int index,Rect rect)
    {
        Pictorial pictorial = pictorials[index];
        int i = 0;
        GameObject go = new GameObject();
        Transform p = go.transform;
        go.name = "Pictorical"+index;
        p.parent = transform;
        foreach (var pp in pictorial.processedPictorials)
        {
            WeavePictoricalEspolin(pictorial,p,i++,rect);
        }
    }

    private void WeavePictoricalEspolin(Pictorial pictorial,Transform parent,int index,Rect rect)
    {
        YarnPictorial yarnPictorial = pictorial.processedPictorials[index];
        yarnPictorial.Prepare();
        // // Create each horizontal yarm (pictorical)
        GameObject go = Instantiate(Resources.Load("Yarn"),parent) as GameObject;
        yarnPictorial.curve = go.GetComponent<Curve>();
        yarnPictorial.curve.speedMultiplier *= gap.x;
        go.name = "P"+index;
        Yarn yarn=go.GetComponent<Yarn>();
        yarn.attributes = yarnPictorial.yarn;
        //Just for debug
        if (yarnPictorial.debugColor != Color.black)
            yarn.attributes.color = yarnPictorial.debugColor;
        
        for (int row = (int)rect.yMin; row < rect.yMax; row++)
        {
            if (yarnPictorial.firstPoint[row]!=-1 && (yarnPictorial.lastPoint[row]-yarnPictorial.firstPoint[row])>2)
            {
                bool lastUpDown = false;
                bool healed,healed2;
                // For each cross with a vertical yarm (warp)
                if ((row%2)==0)
                {
                    for (var column = yarnPictorial.firstPoint[row]; column <= yarnPictorial.lastPoint[row]; column++)
                    {
                        bool upDown = pictoricalValueAtPixel(yarnPictorial,column, row,out healed);
                        bool nextUpDown = pictoricalValueAtPixel(yarnPictorial,column + 1, row,out healed2);

                        // If the yarm doesn't change is not neccesary a control point
                        if ((column != yarnPictorial.firstPoint[row]) && (column != yarnPictorial.lastPoint[row] - 1) && column<resolution.x-1)
                            if ((upDown == lastUpDown) && (upDown == nextUpDown) )// && depth[column,row] == depth[column+1,row])
                                continue;
                        float up;
                        if (upDown)
                            up = 2.0f;
                        else
                            up = yarnPictorial.CalculateBackDepth(this,index,column,row);
                        if (healed) up *=0.2f;
                        yarnPictorial.curve.AddControlPoint(
                            new Vector3(
                                (column - rect.x - rect.width * 0.5f) * gap.x,
                                up * gap.y * 0.5f,
                                (row - rect.y - rect.height * 0.5f) * gap.z),
                            Quaternion.LookRotation(Vector3.right, Vector3.forward),
                            lastUpDown != upDown,
                            "Control Point_"+row+"_"+column+" "+healed);
                    
                        lastUpDown = upDown;
                    }
                }
                else
                {
                    for (var column = yarnPictorial.lastPoint[row]; column >= yarnPictorial.firstPoint[row]; column--)
                    {
                        bool upDown = pictoricalValueAtPixel(yarnPictorial,column, row,out healed);
                        bool nextUpDown = pictoricalValueAtPixel(yarnPictorial,column - 1, row,out healed2);
                        // If the yarm doesn't change is not neccesary a control point
                        if ((column != yarnPictorial.firstPoint[row]) && (column != yarnPictorial.lastPoint[row] - 1) && column!=resolution.x && column!=0)
                            if ((upDown == lastUpDown) && (upDown == nextUpDown))// && depth[column,row] == depth[column-1,row])
                                continue;
                        float up;
                        if (upDown)
                            up = 2.0f;
                        else
                            up = yarnPictorial.CalculateBackDepth(this,index,column,row);
                        if (healed) up *=0.2f;
                        yarnPictorial.curve.AddControlPoint(
                            new Vector3(
                                (column - rect.x - rect.width * 0.5f) * gap.x,
                                up * gap.y * 0.5f,
                                (row - rect.y - rect.height * 0.5f) * gap.z),
                            Quaternion.LookRotation(Vector3.left, Vector3.back),
                            lastUpDown != upDown,
                            "Control Point_"+row+"_"+column+" "+healed);
                    
                        lastUpDown = upDown;
                    }
                }
            }
        }
        yarn.UpdateMesh();
        bounds.Encapsulate(yarn.bounds);
        yarnPictorial.ReleaseMem();
        //Split mesh
        //go.GetComponentInChildren<SplitMeshRenderer>().Split();
    }
    /*private void calculateBackDepth(int column, int row)
    {
        foreach (var p in pictorials)
            foreach (var pp in p.processedPictorials)
            {
                if (pp.IsInBack(column,row))
                        depth[column,row]--;
            }
    }*/

    private void ReducePatterns()
    {
        backgroundPattern.reducePattern(divider,gap.x/gap.z,true);
        technique.reduced_pattern=technique.pattern; //Copy pattern to reduced_pattern to work with reduced
        foreach (var p in pictorials)
        {
            p.drawing.reducePattern(divider, gap.x / gap.z, true);
        }
    }

    public void CleanAll()
    {
        var meshRendererArray = GetComponentsInChildren<MeshRenderer>(true);
        bounds.size = Vector3.zero;
        bounds.center = Vector3.zero;
        foreach (var mr in meshRendererArray)
        {
            Destroy(mr.material);
        }
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        if (ClippingPlane.instance)
            ClippingPlane.instance.matList.Clear();
        Yarn.CleanMaterials();
        Resources.UnloadUnusedAssets();
    }
    
    public void Weave()
    {

        gap.x = warpYarn.threadSize * 2 / weftYarn.threadAspect*compactness;
        gap.y = weftYarn.threadSize  * 2*compactnessY;
        gap.z = weftYarn.threadSize * 2*compactness;

        CleanAll();

        
        if (backgroundPattern == null)
            return;

        float numYarns = 100;
        switch (PlayerPrefs.GetInt("Quality"))
        {
            case 0:
                numYarns = 100;
                break;
            case 1:
                numYarns = 200;
                break;
            case 2:
                numYarns = 400;
                break;
        }
        divider = Mathf.Max(1,backgroundPattern.getOriginalResolution().x / numYarns);
        
        ReducePatterns();
        
        PreprocessPictorials();

        //transform.localScale = Vector3.one / divider;
        resolution.x = (int)(backgroundPattern.getResolution().x);
        resolution.y = (int)(backgroundPattern.getResolution().y); 
        
        foreach (var p in pictorials)
            foreach (var pp in p.processedPictorials)
                pp.CalculateFirstAndLastPointOfRows(resolution);
        //Init depth buffer for back pictoricals
        /*depth=new int[resolution.x,resolution.y];
        for (int i=0;i<resolution.x;i++)
            for (int j=0;j<resolution.y;j++)
                depth[i, j] = -1;*/
        //Comentado porque genera gran cantidad de geometría para poco efecto visual
        /*for (int i=0;i<resolution.x;i++)
            for (int j=0;j<resolution.y;j++)
                calculateBackDepth(i,j);*/
        
        Rect rect = new Rect(Vector2.zero,resolution);
        
        WeaveWarp(rect);
        gap.y = warpYarn.threadSize  * 2*compactnessY;
        WeaveWeft(rect);
        
        for (int i=0;i<pictorials.Count;i++)
            WeavePictorical(i,rect);
        
        // Update box collider
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc != null)
        {
            bc.center = new Vector3(
                0,
                0,
                0
            );
            bc.size = new Vector3(
                rect.width * gap.x,
                0.4f,
                rect.height * gap.z
            );
        }

        PrepareMapsCameras();
        PrepareStlModel();

        RenderPreparedCameras();
        
        CameraControl.instance.Invoke("resetRotation",0.1f);

    }
    private void PreprocessPictorials()
    {
        int i = 0;
        foreach (var pictorical in pictorials)
        { 
            pictorical.Preprocess(dilatationSize,""+i++);
        }
    }
    public void PrepareStlModel()
    {
        Camera frontCamera=stlNode.frontCamera;
        Camera backCamera=stlNode.backCamera;
        if (frontCamera == null || backCamera == null)
            return;
        BoxCollider bc = GetComponent<BoxCollider>();

        var size = bc.size;
        frontCamera.orthographicSize = size.z / 2;
        frontCamera.aspect = size.x / size.z;
        backCamera.orthographicSize = size.z / 2;
        backCamera.aspect = size.x / size.z;
        Vector2Int textureResolution = new Vector2Int();
        
        var resX = backgroundPattern.getOriginalResolution().x;
        var resY = backgroundPattern.getOriginalResolution().y;
        textureResolution.x = Math.Min(resX * 10,stlNode.resolution);
        textureResolution.y = Math.Min(resY * 10,stlNode.resolution);
        float aspect = 1.0f*resX / resY;
        if (textureResolution.x == stlNode.resolution && aspect>1.0f)
            textureResolution.y = (int)Math.Round(textureResolution.x /aspect);
        else if (textureResolution.y == stlNode.resolution)
            textureResolution.x = (int)Math.Round(textureResolution.y *aspect);
        
        RenderTexture rt = new RenderTexture(textureResolution.x,textureResolution.y,24,RenderTextureFormat.Depth);
        frontCamera.targetTexture = rt;
        float v = bounds.center.y + bounds.extents.y + frontCamera.nearClipPlane;
        frontCamera.transform.localPosition =
            Vector3.up * v;
        frontCamera.farClipPlane = Mathf.Abs(v);
      
        rt = new RenderTexture(textureResolution.x,textureResolution.y,24,RenderTextureFormat.Depth);
        backCamera.targetTexture = rt;
        v = bounds.center.y - bounds.extents.y - backCamera.nearClipPlane;
        backCamera.transform.localPosition =
            Vector3.up * v;
        backCamera.farClipPlane = Mathf.Abs(v);
        stlNode.heightMult = bounds.extents.y/bounds.extents.x*divider;
          
        frontCamera.Render();
    }
    public void PrepareMapsCameras()
    {
        Camera frontCamera=mapsGeneratorNode.frontCamera;
        Camera backCamera=mapsGeneratorNode.backCamera;
        if (frontCamera == null || backCamera == null)
            return;
        BoxCollider bc = GetComponent<BoxCollider>();

        var size = bc.size;
        frontCamera.orthographicSize = size.z / 2;
        frontCamera.aspect = size.x / size.z;
        backCamera.orthographicSize = size.z / 2;
        backCamera.aspect = size.x / size.z;
        Vector2Int textureResolution = new Vector2Int();
        
        var resX = backgroundPattern.getOriginalResolution().x;
        var resY = backgroundPattern.getOriginalResolution().y;
        textureResolution.x = mapsGeneratorNode.resolution;
        textureResolution.y = mapsGeneratorNode.resolution;
        float aspect = 1.0f*resX / resY;
        if (textureResolution.x == stlNode.resolution && aspect>1.0f)
            textureResolution.y = (int)Math.Round(textureResolution.x /aspect);
        else if (textureResolution.y == stlNode.resolution)
            textureResolution.x = (int)Math.Round(textureResolution.y *aspect);
        
        RenderTexture rt = new RenderTexture(textureResolution.x,textureResolution.y,24,RenderTextureFormat.ARGB32);
        frontCamera.targetTexture = rt;
        float v = bounds.center.y + bounds.extents.y + frontCamera.nearClipPlane;
        frontCamera.transform.localPosition =
            Vector3.up * v;
        frontCamera.farClipPlane = Mathf.Abs(v);
        CameraControl.instance.mmrImage.texture = rt;
        CameraControl.instance.setAspect(aspect);
        

        rt = new RenderTexture(textureResolution.x,textureResolution.y,24,RenderTextureFormat.ARGB32);
        backCamera.targetTexture = rt;
        v = bounds.center.y - bounds.extents.y - backCamera.nearClipPlane;
        backCamera.transform.localPosition =
            Vector3.up * v;
        backCamera.farClipPlane = Mathf.Abs(v);

        frontCamera.Render();
        backCamera.Render();
        
    }

    public void RenderPreparedCameras()
    {
        //Sólo se llama a una cámara porque cuando una termina llama a la siguiente.
        //Si se llaman una detrás de otra no funciona correctamente
        stlNode.frontCamera.Render(); 
    }
    public void setYarnActive(int i, bool active)
    {
        transform.GetChild(i).gameObject.SetActive(active);
    }
    public void export_STL(string path)
    {
        stlNode.path = path;
        stlNode.GenerateGeometry();
    }

}
