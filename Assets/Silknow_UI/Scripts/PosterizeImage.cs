using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine.Experimental.Rendering;

public class PosterizeImage : MonoBehaviour
{
    private int _clusterCount = 1;
    private Mat _imageMatrix = null;
    private void OnEnable()
    {
        if (WizardController.instance.posterizeResult == null) 
            Invoke("GetHomographyTexture",0.5f);
        
    }

    private void GetHomographyTexture()
    {
        _imageMatrix = new Mat(WizardController.instance.homographyResult.width,WizardController.instance.homographyResult.height,CvType.CV_8UC3);
        Utils.fastTexture2DToMat(WizardController.instance.homographyResult,_imageMatrix,false);
        
        Texture2D tex = new Texture2D(WizardController.instance.homographyResult.width,WizardController.instance.homographyResult.height,TextureFormat.RGB24,false,false);
        
        
        Utils.fastMatToTexture2D(_imageMatrix,tex,false);
        GetComponent<RawImage>().texture = tex;
        GetComponent<ZoomImage>().UpdateRawImageAspect();
    }
    public void ApplyPosterizeImage()
    {
      
        if (_imageMatrix == null)
            return;
        
        WizardController.instance.ToggleProcessingWindow();
        
        // Reshape the matrix to create a matrix of n-pixels elements each one with 3 features (R,G,B)
        int numberOfPixels = _imageMatrix.cols() * _imageMatrix.rows();
        Mat _imageReshaped = new Mat();
        _imageMatrix.copyTo(_imageReshaped);
        _imageReshaped = _imageReshaped.reshape(1,numberOfPixels);
        _imageReshaped.convertTo(_imageReshaped,CvType.CV_32F);
        Mat bestLabels = new Mat();
        Mat centers = new Mat();
        TermCriteria criteria = new TermCriteria(TermCriteria.EPS + TermCriteria.MAX_ITER,10,1.0);
        double ret = Core.kmeans(_imageReshaped, WizardController.instance.clusterCount, bestLabels, criteria,20,Core.KMEANS_PP_CENTERS,centers);

        centers.convertTo(centers, CvType.CV_8UC1);

       
        centers.reshape(_imageReshaped.channels());

        WizardController.instance.posterizeResult = new Texture2D(_imageMatrix.height(),_imageMatrix.width(),TextureFormat.RGB24,false,false);
        
        WizardController.instance.labelsMatrix = bestLabels.clone();
        int rows = 0;
        
        WizardController.instance.imageProcessingResults.Clear();
        WizardController.instance.clusterList.Clear();
        Resources.UnloadUnusedAssets();
     

        int h = _imageMatrix.cols();
        int w = _imageMatrix.rows();

        var colorlist = new List<Color>();
        for (var i = 0; i < centers.rows(); i++)
        {
            int r = (int)centers.get(i, 0)[0];
            int g = (int)centers.get(i, 1)[0];
            int b = (int)centers.get(i, 2)[0];
            colorlist.Add(new Color(r/255.0f,g/255.0f,b/255.0f));
            Mat ipr= Mat.zeros(_imageMatrix.size(),CvType.CV_8U);
            WizardController.instance.imageProcessingResults.Add(ipr);
        }
        WizardController.instance.clusterList.AddRange(colorlist);

        WizardController.instance.backgroundWhiteTexture =Mat.ones(_imageMatrix.size(),CvType.CV_8U);
        WizardController.instance.backgroundWhiteTexture *= 255;

       
        Color[] pixels = new Color[w*h];
        
        for(int x = 0; x < _imageMatrix.cols(); x++) {
            for(int y = 0; y < _imageMatrix.rows(); y++) {
                int label = (int)bestLabels.get(rows, 0)[0];
                WizardController.instance.imageProcessingResults[label].put(y, x, 255);
                pixels[rows] = colorlist[label];
                rows++;
            }
        }
        WizardController.instance.posterizeResult.SetPixels(0,0,w,h,pixels);

        for (var i = 0; i < centers.rows(); i++)
            WizardController.instance.imageProcessingResults[i]=WizardController.instance.imageProcessingResults[i].t();
        WizardController.instance.backgroundWhiteTexture = WizardController.instance.backgroundWhiteTexture.t();
        WizardController.instance.posterizeResult.Apply();

        GetComponent<RawImage>().texture = WizardController.instance.posterizeResult;
        WizardController.instance.ToggleProcessingWindow();

    }
    Mat Flatten(Mat input)
    {
        return input.reshape(1, 1);
    }

    private void SavePosterize(Texture2D tex)
    {
        byte[] bytesPosterized = tex.EncodeToPNG ();
        File.WriteAllBytes (Application.persistentDataPath + "/before_posterized_"+Time.timeSinceLevelLoad+".png", bytesPosterized);
 
    }
}
