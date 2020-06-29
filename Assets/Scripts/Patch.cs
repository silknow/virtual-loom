using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ARTEC.Curves;
using Parabox.STL;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

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

    public List<Pictorical> pictoricals;
    public List<Pictorical> procesedPictorials;
    public int[,] depth;
    public float explodeLevel = 0.0f;
    
    public Bounds bounds;
    public STLGenerator stlNode;
    public MapsGenerator mapsGeneratorNode;
    public void Awake()
    {
        explodeLevel = 0;
        if (pictoricals==null)
            pictoricals = new List<Pictorical>();
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
    
    public bool pictoricalValueAtPixel(Pictorical p, int colunm, int row, out bool healed)
    {
        bool frontFace = p.drawing.Value(colunm, row);
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

    public bool pictoricalHealed(Pictorical p, int column, int row)
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
        Pictorical pictorical = pictoricals[index];
        int i = 0;
        GameObject go = new GameObject();
        Transform p = go.transform;
        go.name = "Pictorical"+index;
        p.parent = transform;
        foreach (var pp in pictorical.processedPictorials)
        {
            WeavePictoricalEspolin(pictorical,p,i++,rect);
        }
    }

    private void WeavePictoricalEspolin(Pictorical pictorial,Transform parent,int index,Rect rect)
    {
        Pictorical pictorical = pictorial.processedPictorials[index];
        // // Create each horizontal yarm (pictorical)
        GameObject go = Instantiate(Resources.Load("Yarn"),parent) as GameObject;
        pictorical.curve = go.GetComponent<Curve>();
        pictorical.curve.speedMultiplier *= gap.x;
        go.name = "P"+index;
        Yarn yarn=go.GetComponent<Yarn>();
        yarn.attributes = pictorical.yarn;
        //Just for debug
        if (pictorical.debugColor != Color.black)
            yarn.attributes.color = pictorical.debugColor;
        
        for (int row = (int)rect.yMin; row < rect.yMax; row++)
        {
            if (pictorical.firstPoint[row]!=-1 && (pictorical.lastPoint[row]-pictorical.firstPoint[row])>2)
            {
                bool currentUpDown = false;
                bool healed,healed2;
                // For each cross with a vertical yarm (warp)
                if ((row%2)==0)
                {
                    for (var column = pictorical.firstPoint[row]; column <= pictorical.lastPoint[row]; column++)
                    {
                        bool upDown = pictoricalValueAtPixel(pictorical,column, row,out healed);
                        bool nextUpDown = pictoricalValueAtPixel(pictorical,column + 1, row,out healed2);

                        // If the yarm doesn't change is not neccesary a control point
                        if ((column != pictorical.firstPoint[row]) && (column != pictorical.lastPoint[row] - 1) && column<resolution.x-1)
                            if ((upDown == currentUpDown) && (upDown == nextUpDown)  && depth[column,row] == depth[column+1,row])
                                continue;
                        float up;
                        if (upDown)
                            up = 2.0f;
                        else
                            up = pictorical.CalculateBackDepth(this,index,column,row);
                        if (healed) up *=0.2f;
                        pictorical.curve.AddControlPoint(
                            new Vector3(
                                (column - rect.x - rect.width * 0.5f) * gap.x,
                                up * gap.y * 0.5f,
                                (row - rect.y - rect.height * 0.5f) * gap.z),
                            Quaternion.LookRotation(Vector3.right, Vector3.forward),
                            currentUpDown != upDown,
                            "Control Point_"+row+"_"+column+" "+healed);
                    
                        currentUpDown = upDown;
                    }
                }
                else
                {
                    for (var column = pictorical.lastPoint[row]; column >= pictorical.firstPoint[row]; column--)
                    {
                        bool upDown = pictoricalValueAtPixel(pictorical,column, row,out healed);
                        bool nextUpDown = pictoricalValueAtPixel(pictorical,column - 1, row,out healed2);
                        // If the yarm doesn't change is not neccesary a control point
                        if ((column != pictorical.firstPoint[row]) && (column != pictorical.lastPoint[row] - 1) && column!=resolution.x && column!=0)
                            if ((upDown == currentUpDown) && (upDown == nextUpDown) && depth[column,row] == depth[column-1,row])
                                continue;
                        float up;
                        if (upDown)
                            up = 2.0f;
                        else
                            up = pictorical.CalculateBackDepth(this,index,column,row);
                        if (healed) up *=0.2f;
                        pictorical.curve.AddControlPoint(
                            new Vector3(
                                (column - rect.x - rect.width * 0.5f) * gap.x,
                                up * gap.y * 0.5f,
                                (row - rect.y - rect.height * 0.5f) * gap.z),
                            Quaternion.LookRotation(Vector3.left, Vector3.back),
                            currentUpDown != upDown,
                            "Control Point_"+row+"_"+column+" "+healed);
                    
                        currentUpDown = upDown;
                    }
                }
            }
        }
        yarn.UpdateMesh();
        bounds.Encapsulate(yarn.bounds);
        //Split mesh
        //go.GetComponentInChildren<SplitMeshRenderer>().Split();
    }
    private void calculateBackDepth(int column, int row)
    {
        foreach (var p in pictoricals)
            foreach (var pp in p.processedPictorials)
            {
                if (pp.IsInBack(column,row))
                        depth[column,row]--;
            }
    }

    private void ReducePatterns()
    {
        backgroundPattern.reducePattern(divider,gap.x/gap.z,true);
        technique.reduced_pattern=technique.pattern; //Copy pattern to reduced_pattern to work with reduced
        foreach (var p in pictoricals)
        {
            p.drawing.reducePattern(divider, gap.x / gap.z,true);
            foreach (var pp in p.processedPictorials)
                pp.drawing.reducePattern(divider, gap.x / gap.z,true);
        }
    }

    public void CleanAll()
    {
        var meshRendererArray = GetComponentsInChildren<MeshRenderer>(true);
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

        PreprocessPictorials();
        
        if (backgroundPattern == null)
            return;

        if (divider < 0.0f) //If divider is negative, we adjust it to 200 yarns of warp
            divider = Mathf.Max(1,backgroundPattern.getOriginalResolution().x / 200.0f);
        ReducePatterns();
        //transform.localScale = Vector3.one / divider;
        resolution.x = (int)(backgroundPattern.getResolution().x);
        resolution.y = (int)(backgroundPattern.getResolution().y); 
        
        foreach (var p in pictoricals)
            foreach (var pp in p.processedPictorials)
                pp.CalculateFirstAndLastPointOfRows(resolution);
        //Init depth buffer for back pictoricals
        depth=new int[resolution.x,resolution.y];
        for (int i=0;i<resolution.x;i++)
            for (int j=0;j<resolution.y;j++)
                depth[i, j] = -1;
        for (int i=0;i<resolution.x;i++)
            for (int j=0;j<resolution.y;j++)
                calculateBackDepth(i,j);
        
        Rect rect = new Rect(Vector2.zero,resolution);
        
        WeaveWarp(rect);
        gap.y = warpYarn.threadSize  * 2*compactnessY;
        WeaveWeft(rect);
        
        for (int i=0;i<pictoricals.Count;i++)
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

        //PrepareMapsCameras();
        PrepareStlModel();
    }
    private void PreprocessPictorials()
    {
        procesedPictorials=new List<Pictorical>();
        int i = 0;
        foreach (var pictorical in pictoricals)
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
        textureResolution.x = resX * mapsGeneratorNode.resolutionMultiplier;
        textureResolution.y = resY * mapsGeneratorNode.resolutionMultiplier;
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
          
        frontCamera.Render();
        backCamera.Render();


    }

    
    public void export_STL(string path)
    {
        stlNode.path = path;
        stlNode.GenerateGeometry();
    }

}
