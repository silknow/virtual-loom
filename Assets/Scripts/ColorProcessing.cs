using UnityEngine;

public class ColorProcessing: MonoBehaviour
{
    public Texture2D texture;
    private Camera cam;
    public Camera nextCamera;
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        Graphics.Blit(source,destination);
        RenderTexture aux=RenderTexture.active;
        texture = new Texture2D(destination.width, destination.height);
        RenderTexture.active = destination;
        texture.ReadPixels(new Rect(0, 0, destination.width, destination.height), 0, 0);
        texture.Apply();
        if (nextCamera!=null)
            nextCamera.Render();
        RenderTexture.active = aux;
        
    }
}
