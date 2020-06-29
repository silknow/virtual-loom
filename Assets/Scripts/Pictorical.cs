using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARTEC.Curves;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;

[Serializable]
public class Pictorical
{
    public Curve curve;
    public Pattern drawing;
    public Color debugColor=Color.black;
    public ScriptableYarn yarn;
    public bool adjusted=true;
    public int healedStep = -1;
    public int healedStepGap = -1;
    public bool doubleHealed = false;
    public int[] firstPoint { set; get; }
    public int[] lastPoint{ set; get; }

    public List<Pictorical> processedPictorials;

    public Pictorical()
    {}
    
    Pictorical(Pictorical copy, Mat mat)
    {
        if (copy.curve == null)
            curve = null;
        else 
            GameObject.Instantiate(copy.curve);
        drawing = new Pattern(copy.drawing);
        debugColor = copy.debugColor;
        yarn = copy.yarn;
        adjusted = copy.adjusted;
        healedStep = copy.healedStep;
        healedStepGap = copy.healedStepGap;
        doubleHealed = copy.doubleHealed;
        if (firstPoint == null)
            copy.firstPoint = null;
        else 
            Array.Copy(firstPoint,copy.firstPoint,copy.firstPoint.Length);
        if (lastPoint == null)
            copy.lastPoint = null;
        else
            Array.Copy(lastPoint,copy.lastPoint,copy.lastPoint.Length);
        
        Utils.matToTexture2D(mat,  drawing.pattern);

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
                    if (drawing.Value(column, row))
                    {
                        firstPoint[row] = Math.Max(column-1,0);
                        break;
                    }
                for (int column=resolution.x-1;column>firstPoint[row];column--)
                    if (drawing.Value(column, row))
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

    internal void  Preprocess(Vector2Int dilatationSize, string prefix)
    {
        processedPictorials=new List<Pictorical>();
        //Not adjusted pictoricals are an unique espolin
        if (!adjusted)
        {
            processedPictorials.Add(this);  
            return;
        }
        else
            SeparateByContours(dilatationSize, prefix);
        
    }

    private void SeparateByContours(Vector2Int dilatationSize, string prefix)
    {
        Size size= new Size((int) (drawing.pattern.width ), (int) (drawing.pattern.height));
        Mat matOrig = new Mat(drawing.pattern.height,drawing.pattern.width,CvType.CV_8UC1);
        Mat matDilate = new Mat(drawing.pattern.height,drawing.pattern.width,CvType.CV_8UC1);
        Imgproc.threshold(matOrig, matOrig, 128, 255, Imgproc.THRESH_BINARY);
        Mat dilateElement = Imgproc.getStructuringElement (Imgproc.MORPH_RECT, new Size (dilatationSize.x, dilatationSize.y));
        Utils.texture2DToMat(drawing.pattern,matOrig);
        Imgproc.dilate(matOrig, matDilate,dilateElement);
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat srcHierarchy = new Mat ();
        Imgproc.findContours(matDilate,contours,srcHierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
        int i = 0;
        foreach (var contour in contours)
        {
            double area = Imgproc.contourArea(contour);
            //Debug.Log("Area contorno: "+area);
            Mat matContour = Mat.zeros(drawing.pattern.height,drawing.pattern.width,CvType.CV_8UC1);
            Mat matContour2 = Mat.zeros(drawing.pattern.height,drawing.pattern.width,CvType.CV_8UC1);
            Imgproc.drawContours(matContour,contours,i,new Scalar(255,0,0),-1);
            Imgproc.drawContours(matContour2,contours,i,new Scalar(255,0,0),2);
            Core.bitwise_and(matOrig,matContour,matContour);
            Pictorical p = new Pictorical(this,matContour);
            if (area > 30)
            {
                //OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite(prefix + " contour" + i + "(area " + area + ").png",
                //    matContour);
                //OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite(prefix + " contour" + i + "(area " + area + ")2.png",
                //    matContour2);
                processedPictorials.Add(p);
            }
            else
            {
                //OpenCVForUnity.ImgcodecsModule.Imgcodecs.imwrite(prefix + " contour" + i + "(area " + area + ")-descartado.png",
                //    matContour);
            }

            i++;
        }
    }
}
