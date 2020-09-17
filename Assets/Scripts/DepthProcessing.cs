using System;
using UnityEngine;

public class DepthProcessing : MonoBehaviour
{
    public Texture2D texture;
    private Camera cam;
    public Camera nextCamera;
    // Start is called before the first frame update
    void Start()
    {
        cam=GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }
    
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //draws the pixels from the source texture to the destination texture
        //Graphics.Blit(source, destination,postprocessMaterial);
        Graphics.Blit(source,destination);
        RenderTexture aux=RenderTexture.active;
        texture = new Texture2D(destination.width, destination.height);
        //Graphics.CopyTexture(destination,texture);
        RenderTexture.active = destination;
        texture.ReadPixels(new Rect(0, 0, destination.width, destination.height), 0, 0);
        texture.Apply();
        if (nextCamera!=null)
            nextCamera.Render();
        RenderTexture.active = aux;
        
    }
}
