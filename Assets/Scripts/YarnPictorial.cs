using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARTEC.Curves;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using UnityEditor.Experimental;

[Serializable]
public class YarnPictorial
{
    public Curve curve;
    public Pattern drawing;
    public Mat contour;
    public MatOfPoint contourPoints;
    public Color debugColor=Color.black;
    public ScriptableYarn yarn;
    public bool adjusted=true;
    public int healedStep = -1;
    public int healedStepGap = -1;
    public bool doubleHealed = false;
    public int[] firstPoint { set; get; }
    public int[] lastPoint{ set; get; }
    
    public YarnPictorial()
    {}
    
    public YarnPictorial(Pictorial copy,MatOfPoint contourPoints)
    {
        if (copy.curve == null)
            curve = null;
        else 
            GameObject.Instantiate(copy.curve);
        drawing = copy.drawing;
        debugColor = copy.debugColor;
        yarn = copy.yarn;
        adjusted = copy.adjusted;
        healedStep = copy.healedStep;
        healedStepGap = copy.healedStepGap;
        doubleHealed = copy.doubleHealed;
        
        this.contourPoints = contourPoints;
    }

    public YarnPictorial(Pictorial copy)
    {
        if (copy.curve == null)
            curve = null;
        else 
            GameObject.Instantiate(copy.curve);
        drawing = copy.drawing;
        debugColor = copy.debugColor;
        contour = null;
        yarn = copy.yarn;
        adjusted = copy.adjusted;
        healedStep = copy.healedStep;
        healedStepGap = copy.healedStepGap;
        doubleHealed = copy.doubleHealed;
        
    }

    public int CalculateBackDepth(Patch patch, int index, int column, int row)
    {
        int acum = -2;
        /*for (int i=0;i<index;i++)
            foreach (var pp in patch.pictoricals[i].processedPictorials)
                if (pp.IsInBack(column, row))
                {
                    acum--;
                    break;
                }*/
        return acum;
    }

    public bool IsInBack(int column, int row)
    {
        if (firstPoint[row]!=-1 && (lastPoint[row]-firstPoint[row])>2)
            if (firstPoint[row] < column && column < lastPoint[row])
                return (!drawing.Value(column, row));
        return false;
    }

    public void CalculateFirstAndLastPointOfRows(Vector2Int resolution)
    {

        firstPoint = new int[resolution.y];
        lastPoint = new int[resolution.y];
        for (int row = 0; row < resolution.y; row++)
        {
            if (adjusted)
            {
                lastPoint[row] = firstPoint[row] = -1;
                for (int column=0;column<resolution.x;column++)
                    if (Value(column, row))
                    {
                        firstPoint[row] = Math.Max(column-1,0);
                        break;
                    }
                for (int column=resolution.x-1;column>firstPoint[row];column--)
                    if (Value(column, row))
                    {
                        lastPoint[row] = Math.Min(column+1,resolution.x);
                        break;
                    }
            }
            else
            {
                {
                    firstPoint[row] = 0;
                    lastPoint[row] = resolution.x;
                }
            }
        }
    }

    public bool Value(int i, int j)
    {
        if (contour != null)
        {
            int norm_i = i * (contour.cols()-1) / drawing.reduced_pattern.cols();
            int norm_j = j * (contour.rows()-1) / drawing.reduced_pattern.rows();
            uint res = (uint)contour.get(norm_j, norm_i)[0];
            if (res == 0)
            //if (Imgproc.pointPolygonTest(new MatOfPoint2f(contourPoints.toArray()),new Point(norm_j,norm_i),false)<0.0f)
                return false;
        }

        return drawing.Value(i, j);
    }

    public void Prepare()
    {
        if (contourPoints != null)
        {
            Size contoursSize = new Size(100, 100);
            contour = Mat.zeros(contoursSize, CvType.CV_8UC1);
            List<MatOfPoint> contours = new List<MatOfPoint>();
            contours.Add(contourPoints);
            Imgproc.drawContours(contour, contours, 0, new Scalar(255), -1);
        }
    }

    public void ReleaseMem()
    {
        if (contour != null)
        {
            contour.release();
            contour = null;
        }
    }
}
