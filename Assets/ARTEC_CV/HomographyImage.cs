using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class HomographyImage : MonoBehaviour
{
    public RectTransform[] HomographyPoints;
    public RectTransform HomographySpace;
    public Material HomographyMaterial;
    public Texture2D result;
    private float[] HomographyMatrix;

    public RawImage referenceImage;

    private Vector3[] userPoints;
    private static HomographyImage instance = null;
    
    public float aspect = 1.0f;

    private bool mirrorX = false;
    private bool mirrorY = false;


    public int maxWidth = 650;

    public static HomographyImage getInstance() {
        return instance;
    }

    void Awake()
    {
        GetTextureFromReference();
    }

    void Start() {
        instance = this;
        userPoints = new Vector3[4];

        Invoke("UpdateHomography", 0.5f);
    }

    public void setPoint(int index, float x, float y) {
        userPoints[index] = new Vector3(x,y,0);
        UpdateHomography();
    }

    public void ResetHomographyPoints()
    {
        for (int i=0; i<4; i++) {
            HomographyPoints[i].anchoredPosition  = Vector2.zero;
        }
        UpdateHomography();
    }

    public void UpdateHomography() {

        if (HomographyPoints.Length >= 4) {
            // Vector3[] userPoints = new Vector3[4];
            for (int i=0; i<4; i++) {
                userPoints[i] = new Vector3(
                    HomographyPoints[i].localPosition.x / HomographySpace.rect.width,
                    HomographyPoints[i].localPosition.y / HomographySpace.rect.height,
                    0);
            }
        }

        Vector3[] desiredPoints = new Vector3[4] {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(0,1,0)
        };
        
        HomographyMatrix = Homography.CalcHomographyMatrix(ref userPoints);
        HomographyMaterial.SetFloatArray("_InvHomography", HomographyMatrix);
        
        HomographyMaterial.SetFloat("_MirrorX", mirrorX ? 2.0f : 1.0f);
        HomographyMaterial.SetFloat("_MirrorY", mirrorY ? 2.0f : 1.0f);
        
        // PRUEBA PABLO 
        /*
        var newAspect  = Vector3.Distance(userPoints[0],userPoints[2]) / Vector3.Distance(userPoints[1],userPoints[3]);
        Debug.Log(aspect);
        if (GetComponent<AspectRatioFitter>())
        {
            GetComponent<AspectRatioFitter>().aspectRatio = newAspect;
            GetComponent<AspectRatioFitter>().aspectMode = (newAspect > 1f)
                ? AspectRatioFitter.AspectMode.WidthControlsHeight
                : AspectRatioFitter.AspectMode.HeightControlsWidth;
        }
        */
    }
    public void OnSliderValueChanged(float value)
    {
        aspect = value;
    }

    public void RenderHomography(float aspect) {
        var w  = referenceImage.texture.width;
        var h  = referenceImage.texture.height;

        int width, height;
        if (aspect> 1f)
        {
            width = Mathf.Min(w, maxWidth);
            height = (int)(width / aspect);
        }
        else 
        {
            height = Mathf.Min(h, maxWidth);
            width  = (int)(height * aspect);
        }


        WizardController.instance.homographyResult = new Texture2D(width,height,TextureFormat.RGB24,false,false);
            
        Texture2D imageTexture = GetComponent<RawImage>().mainTexture as Texture2D;
        int imageWidth = imageTexture.width;
        int imageHeight = imageTexture.height;

        for (int x=0; x<width; x++)
            for (int y=0; y<height; y++) {

                // Normalized point inside the image
                Vector2 p = new Vector2(
                    (float)x/(float)width,
                     (float)y/(float)height
                );
                
                
                /* PABLO MIRROR */
                Vector2 t;
                t.x= Mathf.Repeat(p.x,1.0f)*( mirrorX ? 2.0f : 1.0f );
                t.y = Mathf.Repeat(p.y,1.0f)*( mirrorY ? 2.0f : 1.0f );
                Vector2 length = Vector2.one;
                p = new Vector2(length.x-Mathf.Abs(t.x-length.x), length.y-Mathf.Abs(t.y-length.y)); 
                
                /* PABLO MIRROR */ 
                
                
                //Calculate HomographyMatix if points have not changed
                if(HomographyMatrix == null)
                    UpdateHomography();
                

                // Remap coordinates with the homography
                float s =  HomographyMatrix[6] * p.x + HomographyMatrix[7] * p.y + HomographyMatrix[8];
	            float u = (HomographyMatrix[0] * p.x + HomographyMatrix[1] * p.y + HomographyMatrix[2]) / s;
	            float v = (HomographyMatrix[3] * p.x + HomographyMatrix[4] * p.y + HomographyMatrix[5]) / s;

                // Read pixel color
                Color pixelColor = imageTexture.GetPixel((int)(u * imageWidth),(int)(v*imageHeight));

                // Assign pixel color
                WizardController.instance.homographyResult.SetPixel(x,y,pixelColor);
            }
        WizardController.instance.homographyResult.Apply();
        WizardController.instance.posterizeResult = null;

//        byte[] resultBytes = result.EncodeToPNG();
//        File.WriteAllBytes(Application.streamingAssetsPath + "/correctedImage.png", resultBytes);
    }

    public void RenderHomography() {
        RenderHomography(aspect);

    }
    
    public void GetTextureFromReference()
    {
        var img = GetComponent<RawImage>();
        if(img != null)
            img.texture  = referenceImage.texture;
        GetComponent<ZoomImage>().UpdateRawImageAspect();
    }
    
    public void ToggleMirrorX()
    {
        mirrorX = !mirrorX;
        UpdateHomography();
    }
    public void ToggleMirrorY()
    {
        mirrorY = !mirrorY;
        UpdateHomography();
    }

    
}
