using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARTEC.Curves;
using Parabox.STL;
using UnityEditor;
using UnityEngine.Serialization;

public class Patch : MonoBehaviour
{
    // Number of werps and warps
    public Vector2Int resolution;

    public float divider = -1.0f;
    public float gap;

    public Pattern backgroundPattern;
    public Pattern technique;

    
    private Curve weft;
    private Curve[] warp;
    
    public ScriptableYarn warpYarn;
    public ScriptableYarn weftYarn;

    public List<Pictorical> pictoricals;
    public int[,] depth;
    public float explodeLevel = 0.0f;
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
            transform.GetChild(i).transform.localPosition = level * pos * gap*0.1f*Math.Max(resolution.x,resolution.y)*Vector3.up;
        }
    }

    public void setLayerVisible(int index, bool visible)
    {
        if (index<transform.childCount)
            transform.GetChild(index).gameObject.SetActive(visible);
    }
    
    public bool pictoricalValueAtPixel(int index, int colunm, int row, out bool healed)
    {
        bool frontFace = pictoricals[index].drawing.Value(colunm, row);
        healed = false;
        if (pictoricalHealed(index,colunm,row))
        {
            if (frontFace == true || pictoricals[index].doubleHealed)
            {
                frontFace = !frontFace;
                healed = true;
            }
        }
        return frontFace;
    }

    public bool pictoricalHealed(int index, int column, int row)
    {
        bool healed=false;
        Pictorical p=pictoricals[index];
        if (p.healedStep != -1)
        {
            if (column>=p.firstPoint[row] && column<=p.lastPoint[row] || !p.adjusted)
                if ((column+(row*p.healedStepGap))%p.healedStep==0)
                    healed=true;
        }
        return healed;
    }
    
    public void WeaveWarp(Rect rect)
    {
        // Create each vertical yarm (warp)
        warp = new Curve[resolution.x];
        GameObject parent=new GameObject("Warp");
        parent.transform.parent = transform;
        for (int i = (int)rect.xMin; i < rect.xMax; i++)
        {
            GameObject go = Instantiate(Resources.Load("Yarn"),parent.transform) as GameObject;
            go.name = "Warp" + i;
            go.GetComponent<Yarn>().attributes = warpYarn;
            warp[i] = go.GetComponent<Curve>();
            warp[i].speedMultiplier *= gap;
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
                        (i - rect.xMin - rect.width * 0.5f) * gap,
                        up * gap * 0.5f,
                        (j - rect.yMin - rect.height * 0.5f) *  gap),
                    Quaternion.LookRotation(Vector3.forward, Vector3.right),
                    currentUpDown != upDown);
                
                currentUpDown = upDown;
            }
            go.GetComponent<Yarn>().UpdateMesh();
        }
    }
    public void WeaveWeft(Rect rect)
    {
        // // Create each horizontal yarm (weft)
        GameObject go = null;
        go = Instantiate(Resources.Load("Yarn"),transform) as GameObject;
        go.name = "Weft";
        go.GetComponent<Yarn>().attributes = weftYarn;
        weft = go.GetComponent<Curve>();
        weft.speedMultiplier *= gap;
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
                            (column - rect.x - (rect.width) * 0.5f) * gap,
                            up * gap * 0.5f,
                            (row - rect.y - (rect.height) * 0.5f) * gap),
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
                            (column - rect.x - (rect.width) * 0.5f) * gap,
                            up * gap * 0.5f,
                            (row - rect.y - (rect.height) * 0.5f) * gap),
                        Quaternion.LookRotation(Vector3.left, Vector3.back),
                        currentUpDown != upDown);

                    currentUpDown = upDown;
                }
            }
        }
        go.GetComponent<Yarn>().UpdateMesh();
    }

    private void WeavePictorical(int index,Rect rect)
    {
        Pictorical pictorical = pictoricals[index];
        // // Create each horizontal yarm (pictorical)
        GameObject go = Instantiate(Resources.Load("Yarn"),transform) as GameObject;
        pictorical.curve = go.GetComponent<Curve>();
        pictorical.curve.speedMultiplier *= gap;
        go.name = "Pictorical"+index;
        go.GetComponent<Yarn>().attributes = pictorical.yarn;
        for (int row = (int)rect.yMin; row < rect.yMax; row++)
        {
            if (pictorical.firstPoint[row]!=-1 && (pictorical.lastPoint[row]-pictorical.firstPoint[row])>2)
            {
                bool currentUpDown = false;
                bool healed;
                // For each cross with a vertical yarm (warp)
                if ((row%2)==0)
                {
                    for (var column = pictorical.firstPoint[row]; column < pictorical.lastPoint[row]; column++)
                    {
                        bool upDown = pictoricalValueAtPixel(index,column, row,out healed);
                        bool nextUpDown = pictoricalValueAtPixel(index,column + 1, row,out healed);

                        // If the yarm doesn't change is not neccesary a control point
                        if ((column != pictorical.firstPoint[row]) && (column != pictorical.lastPoint[row] - 1) && column<resolution.x-1)
                            if ((upDown == currentUpDown) && (upDown == nextUpDown)  && depth[column,row] == depth[column+1,row])
                                continue;
                        float up;
                        if (upDown)
                            up = 2.0f;
                        else
                            up = pictorical.CalculateBackDepth(this,index,column,row);
                        //if (healed) up /= 20.0f;
                        pictorical.curve.AddControlPoint(
                            new Vector3(
                                (column - rect.x - rect.width * 0.5f) * gap,
                                up * gap * 0.5f,
                                (row - rect.y - rect.height * 0.5f) * gap),
                            Quaternion.LookRotation(Vector3.right, Vector3.forward),
                            currentUpDown != upDown);
                    
                        currentUpDown = upDown;
                    }
                }
                else
                {
                    for (var column = pictorical.lastPoint[row]; column >= pictorical.firstPoint[row]; column--)
                    {
                        bool upDown = pictoricalValueAtPixel(index,column, row,out healed);
                        bool nextUpDown = pictoricalValueAtPixel(index,column - 1, row,out healed);
                        // If the yarm doesn't change is not neccesary a control point
                        if ((column != pictorical.firstPoint[row]) && (column != pictorical.lastPoint[row] - 1) && column!=resolution.x && column!=0)
                            if ((upDown == currentUpDown) && (upDown == nextUpDown) && depth[column,row] == depth[column-1,row])
                                continue;
                        float up;
                        if (upDown)
                            up = 2.0f;
                        else
                            up = pictorical.CalculateBackDepth(this,index,column,row);
                        //if (healed) up /= 20.0f;
                        pictorical.curve.AddControlPoint(
                            new Vector3(
                                (column - rect.x - rect.width * 0.5f) * gap,
                                up * gap * 0.5f,
                                (row - rect.y - rect.height * 0.5f) * gap),
                            Quaternion.LookRotation(Vector3.left, Vector3.back),
                            currentUpDown != upDown);
                    
                        currentUpDown = upDown;
                    }
                }
            }
        }
        go.GetComponent<Yarn>().UpdateMesh();
    }

    private void calculateBackDepth(int column, int row)
    {
        foreach (var p in pictoricals)
        {
            if (p.IsInBack(column,row))
                    depth[column,row]--;
        }
    }

    private void ReducePatterns()
    {
        backgroundPattern.reducePattern(divider);
        technique.reducePattern(1.0f);
        foreach (var p in pictoricals)
            p.drawing.reducePattern(divider);
        
    }
    public void Weave()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        if (backgroundPattern == null)
            return;

        if (divider < 1.0f) //If divider is negative, we adjust it to 100 yarns of warp
            divider = Mathf.Max(1,backgroundPattern.getOriginalResolution().x / 200.0f);
        ReducePatterns();
        //transform.localScale = Vector3.one / divider;
        resolution.x = (int)(backgroundPattern.getResolution().x);
        resolution.y = (int)(backgroundPattern.getResolution().y); 
        
        
        foreach (var p in pictoricals)
            p.CalculateFirstAndLastPointOfRows(resolution);
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
                rect.width * gap,
                0.4f,
                rect.height * gap
            );
        }
    }
    public void export_STL(string path)
    {
        Parabox.STL.pb_Stl_Exporter.Export(path, new GameObject[] { gameObject }, FileType.Binary);
    }

}
