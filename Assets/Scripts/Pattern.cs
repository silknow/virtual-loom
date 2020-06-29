using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine;


[Serializable]
public class Pattern
{
    
    public Texture2D reduced_pattern;
    public Texture2D pattern;

    public Pattern() {}

    public Pattern(Pattern copy)
    {
        if (copy.reduced_pattern == null)
            reduced_pattern = null;
        else
            reduced_pattern = (Texture2D)GameObject.Instantiate(copy.reduced_pattern);
        if (copy.pattern == null)
            pattern = null;
        else
            pattern = (Texture2D)GameObject.Instantiate(copy.pattern);
    }
    public void reducePattern(float divider,float gapAspect, bool createFrame = false)
    {
        int[] face = {Imgproc.FONT_HERSHEY_SIMPLEX, Imgproc.FONT_HERSHEY_PLAIN, Imgproc.FONT_HERSHEY_DUPLEX, Imgproc.FONT_HERSHEY_COMPLEX, 
            Imgproc.FONT_HERSHEY_TRIPLEX, Imgproc.FONT_HERSHEY_COMPLEX_SMALL, Imgproc.FONT_HERSHEY_SCRIPT_SIMPLEX, 
            Imgproc.FONT_HERSHEY_SCRIPT_COMPLEX, Imgproc.FONT_ITALIC
        };
        //if (divider > 1.0f || gapAspect!=1f)
        {
            Size size= new Size((int) (pattern.width / divider), (int) (pattern.height / divider) * gapAspect);
            Mat mat = new Mat(pattern.height,pattern.width,CvType.CV_8UC3);
            if (createFrame)
                Imgproc.line(mat,new Point(0,0),new Point(40,40), new Scalar(0,255,0),10 );
            Mat matDst = new Mat((int)size.height,(int)size.width,CvType.CV_8UC3);
            Utils.texture2DToMat(pattern,mat);
            Imgproc.resize(mat,matDst,size);
            Imgproc.line(matDst,new Point(0,0),new Point(0,matDst.height()), new Scalar(0,0,0),1 );
            Imgproc.line(matDst,new Point(matDst.width()-1,0),new Point(matDst.width()-1,matDst.height()), new Scalar(0,0,0),1 );
            reduced_pattern = new Texture2D((int)size.width,(int)size.height,TextureFormat.RGB24,false,false);
            Utils.matToTexture2D(matDst, reduced_pattern);
            
        }
        // else
        // {
        //     reduced_pattern = pattern;
        // }

            
    }
    public Vector2Int getResolution() {
        if (reduced_pattern == null)
            return Vector2Int.zero;

        return new Vector2Int(reduced_pattern.width, reduced_pattern.height);
    }
       
    public Vector2Int getOriginalResolution() {
        if (pattern == null)
            return Vector2Int.zero;

        return new Vector2Int(pattern.width, pattern.height);
    }
       
    // Compute local coordinates in repeate mode
    public Vector2Int CoordenadasPunto(Vector2Int pos)
    {
        pos.x = (int)(pos.x) % reduced_pattern.width;
        pos.y = (int)(pos.y) % reduced_pattern.height;
        return pos;
    }
    
    public bool Value(int i, int j)
    {
        var pos = CoordenadasPunto(new Vector2Int(i, j));
        return (reduced_pattern.GetPixel(pos.x, pos.y).r > 0.2);
    }
}
