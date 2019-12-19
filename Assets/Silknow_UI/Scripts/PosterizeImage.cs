using System;
using System.Collections;
using System.Collections.Generic;
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
        
        // Reshape the matrix to create a matrix of n-pixels elements each one with 3 features (R,G,B)
        int numberOfPixels = _imageMatrix.cols() * _imageMatrix.rows();
        
        
        
        Mat _imageReshaped = _imageMatrix.reshape(1,numberOfPixels);
        
        Mat samples32f = new Mat();
        _imageReshaped.convertTo(samples32f,CvType.CV_32F, 1.0/255.0);
        
        //Debug.Log("Reshaped size: " + _imageReshaped.width() + " x " + _imageReshaped.height() + " x " +_imageReshaped.channels());

        Mat bestLabels = new Mat();//;new Mat(1,_imageReshaped.width(), CvType.CV_32S);
        
        Mat centers = new Mat();
        
        TermCriteria criteria = new TermCriteria(TermCriteria.EPS + TermCriteria.MAX_ITER,10,1.0);
        double ret = OpenCVForUnity.CoreModule.Core.kmeans(samples32f, WizardController.instance.clusterCount, bestLabels, criteria,20,OpenCVForUnity.CoreModule.Core.KMEANS_PP_CENTERS,centers);

        
        
        centers.convertTo(centers, CvType.CV_8UC1, 255.0);
        centers.reshape(3);

        WizardController.instance.posterizeResult = new Texture2D(_imageMatrix.height(),_imageMatrix.width(),TextureFormat.RGB24,false,false);

        WizardController.instance.labelsMatrix = bestLabels.clone();
        
        
        Mat dst = _imageMatrix.clone();
        int rows = 0;
        
        
        WizardController.instance.imageProcessingResults.Clear();
        WizardController.instance.clusterList.Clear();
        for (var i = 0; i < centers.rows(); i++)
        {
            int r = (int)centers.get(i, 2)[0];
            int g = (int)centers.get(i, 1)[0];
            int b = (int)centers.get(i, 0)[0];
            WizardController.instance.clusterList.Add(new Color(b/255.0f,g/255.0f,r/255.0f));
            //Generate 1 texture for each cluster; Revisar formato 
            var ipTex = new Texture2D(_imageMatrix.height(),_imageMatrix.width(),TextureFormat.RGB24,false,false);
            WizardController.instance.imageProcessingResults.Add(ipTex);
        }
        
        WizardController.instance.backgroundWhiteTexture = new Texture2D(_imageMatrix.height(),_imageMatrix.width(),TextureFormat.RGB24,false,false);
        
        for(int x = 0; x < _imageMatrix.cols(); x++) {
            for(int y = 0; y < _imageMatrix.rows(); y++) {
                for (var i = 0; i < centers.rows(); i++)
                {
                    WizardController.instance.imageProcessingResults[i].SetPixel(y,x,Color.black);
                }
                int label = (int)bestLabels.get(rows, 0)[0];
                int r = (int)centers.get(label, 2)[0];
                int g = (int)centers.get(label, 1)[0];
                int b = (int)centers.get(label, 0)[0];
                //dst.put(y, x, b,g,r);
                WizardController.instance.imageProcessingResults[label].SetPixel(y,x,Color.white);
                WizardController.instance.posterizeResult.SetPixel(y,x,new Color(b/255.0f,g/255.0f,r/255.0f));
                WizardController.instance.backgroundWhiteTexture.SetPixel(y,x,new Color(255.0f,255.0f,255.0f));
                rows++;
            }
        }

        for (var i = 0; i < centers.rows(); i++)
        {
            WizardController.instance.imageProcessingResults[i].Apply();
        }

        WizardController.instance.posterizeResult.Apply();
        
        GetComponent<RawImage>().texture = WizardController.instance.posterizeResult;
    }
}
