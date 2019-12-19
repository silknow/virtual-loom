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

    public void reducePattern(float divider)
    {
        if (divider > 1.0f)
        {
            Size size= new Size((int) (pattern.width / divider), (int) (pattern.height / divider));
            Mat mat = new Mat(pattern.height,pattern.width,CvType.CV_8UC3);
            Mat matDst = new Mat((int)size.height,(int)size.width,CvType.CV_8UC3);
            Utils.fastTexture2DToMat(pattern,mat);
            Imgproc.resize(mat,matDst,size);
            reduced_pattern = new Texture2D((int)size.width,(int)size.height,TextureFormat.RGB24,false,false);
            Utils.fastMatToTexture2D(matDst, reduced_pattern);
        }
        else
        {
            reduced_pattern = pattern;
        }

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
