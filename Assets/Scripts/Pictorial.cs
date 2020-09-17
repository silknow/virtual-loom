using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using ARTEC.Curves;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;

[Serializable]
public class Pictorial
{
    public Curve curve;
    public Pattern drawing;
    public Color debugColor=Color.black;
    public ScriptableYarn yarn;
    public bool adjusted=true;
    public int healedStep = -1;
    public int healedStepGap = -1;
    public bool doubleHealed = false;

    //public int[] firstPoint { set; get; }
    //public int[] lastPoint{ set; get; }

    public List<YarnPictorial> processedPictorials;

    
    internal void  Preprocess(Vector2Int dilatationSize, string prefix)
    {
        processedPictorials=new List<YarnPictorial>();
        //Not adjusted pictoricals are an unique espolin
        if (  !adjusted)
        {
            processedPictorials.Add(new YarnPictorial(this));
        }
        else
        {
            
            SeparateByContours(dilatationSize, prefix);
        }
            
        
    }

    private void SeparateByContours(Vector2Int dilatationSize, string prefix)
    {
        Size contoursSize = new Size(100,100);
        Mat matOrig = new Mat(contoursSize,CvType.CV_8U);
        Imgproc.resize(drawing.pattern,matOrig,contoursSize);
        Mat matDilate = new Mat(contoursSize,CvType.CV_8U);
        Imgproc.threshold(matOrig, matOrig, 1, 255, Imgproc.THRESH_BINARY);
        Mat dilateElement = Imgproc.getStructuringElement (Imgproc.MORPH_RECT, new Size (dilatationSize.x, dilatationSize.y));
        Imgproc.dilate(matOrig, matDilate,dilateElement);
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat srcHierarchy = new Mat ();
        Imgproc.findContours(matDilate,contours,srcHierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
        int i = 0;
        if (contours.Count==1)
            processedPictorials.Add(new YarnPictorial(this));
        else
            foreach (var contour in contours)
            {
                double area = Imgproc.contourArea(contour);
                if (area > 25)
                {
                    YarnPictorial p = new YarnPictorial(this,contour);
                    processedPictorials.Add(p);
                }

                i++;
            }
        }
}
